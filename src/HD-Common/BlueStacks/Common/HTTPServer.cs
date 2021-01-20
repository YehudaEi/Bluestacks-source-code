// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.HTTPServer
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

namespace BlueStacks.Common
{
  public class HTTPServer : IDisposable
  {
    private HttpListener mListener;
    private bool mShutDown;
    private readonly Dictionary<string, HTTPServer.RequestHandler> Routes;
    private bool disposedValue;

    public int Port { get; }

    public string RootDir { get; set; }

    public static bool FileWriteComplete { get; set; } = true;

    public HTTPServer(
      int port,
      Dictionary<string, HTTPServer.RequestHandler> routes,
      string rootDir)
    {
      this.Port = port;
      this.Routes = routes;
      this.RootDir = rootDir;
    }

    public void Start()
    {
      string uriPrefix = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://{0}:{1}/", (object) "*", (object) this.Port);
      this.mListener = new HttpListener();
      this.mListener.Prefixes.Add(uriPrefix);
      try
      {
        this.mShutDown = false;
        this.mListener.Start();
      }
      catch (HttpListenerException ex)
      {
        Logger.Error("Failed to start listener. err: " + ex.ToString());
        throw new ENoPortAvailableException("No free port available");
      }
    }

    public void Run()
    {
      while (!this.mShutDown)
      {
        HttpListenerContext context;
        try
        {
          context = this.mListener.GetContext();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while processing HTTP context: " + ex.ToString());
          continue;
        }
        ThreadPool.QueueUserWorkItem(new WaitCallback(new HTTPServer.Worker(context, this.Routes, this.RootDir).ProcessRequest));
      }
    }

    public void Stop()
    {
      if (this.mListener == null)
        return;
      try
      {
        this.mShutDown = true;
        this.mListener.Close();
      }
      catch (HttpListenerException ex)
      {
        Logger.Error("Failed to stop listener. err: " + ex.ToString());
      }
    }

    ~HTTPServer()
    {
      this.Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.mListener?.Close();
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public delegate void RequestHandler(HttpListenerRequest req, HttpListenerResponse res);

    private class Worker
    {
      private Dictionary<string, HTTPServer.RequestHandler> mRoutes;
      private HttpListenerContext mCtx;
      private string mRootDir;

      public Worker(
        HttpListenerContext ctx,
        Dictionary<string, HTTPServer.RequestHandler> routes,
        string rootDir)
      {
        this.mCtx = ctx;
        this.mRoutes = routes;
        this.mRootDir = rootDir;
      }

      [STAThread]
      public void ProcessRequest(object stateInfo)
      {
        try
        {
          if (this.mCtx.Request.Url.AbsolutePath.StartsWith("/static/", StringComparison.OrdinalIgnoreCase))
            this.StaticFileHandler(this.mCtx.Request, this.mCtx.Response);
          else if (this.mCtx.Request.Url.AbsolutePath.StartsWith("/static2/", StringComparison.OrdinalIgnoreCase))
            this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, "");
          else if (this.mCtx.Request.Url.AbsolutePath.StartsWith("/staticicon/", StringComparison.OrdinalIgnoreCase))
          {
            if (this.mCtx.Request.QueryString != null && this.mCtx.Request.QueryString.Count > 0)
            {
              string index = HttpUtility.ParseQueryString(this.mCtx.Request.Url.Query).Get("oem");
              if (!InstalledOem.InstalledCoexistingOemList.Contains(index))
                return;
              this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, Path.Combine(RegistryManager.RegistryManagers[index].EngineDataDir, "UserData\\Gadget"));
            }
            else
              this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\Gadget"));
          }
          else if (this.mRoutes.ContainsKey(this.mCtx.Request.Url.AbsolutePath))
          {
            HTTPServer.RequestHandler mRoute = this.mRoutes[this.mCtx.Request.Url.AbsolutePath];
            if (mRoute == null)
              return;
            if (this.mCtx.Request.UserAgent != null)
            {
              Logger.Info("Request received {0}", (object) this.mCtx.Request.Url.AbsolutePath);
              Logger.Debug("UserAgent = {0}", (object) this.mCtx.Request.UserAgent);
            }
            if (HTTPServer.Worker.IsTokenValid(this.mCtx.Request.Headers))
            {
              mRoute(this.mCtx.Request, this.mCtx.Response);
            }
            else
            {
              Logger.Warning("Token validation check failed, unauthorized access");
              HTTPUtils.WriteErrorJson(this.mCtx.Response, "Unauthorized Access(401)");
              this.mCtx.Response.StatusCode = 401;
            }
          }
          else
          {
            Logger.Warning("Exception: No Handler registered for " + this.mCtx.Request.Url.AbsolutePath);
            HTTPUtils.WriteErrorJson(this.mCtx.Response, "Request NotFound(404)");
            this.mCtx.Response.StatusCode = 404;
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while processing HTTP handler: " + ex.ToString());
          HTTPUtils.WriteErrorJson(this.mCtx.Response, "Internal Server Error(500)");
          this.mCtx.Response.StatusCode = 500;
        }
        finally
        {
          try
          {
            this.mCtx.Response.OutputStream.Close();
          }
          catch (Exception ex)
          {
            Logger.Warning("Exception during mCtx.Response.OutputStream.Close(): " + ex.ToString());
          }
        }
      }

      private static bool IsTokenValid(NameValueCollection headers)
      {
        return headers["x_api_token"] != null && headers["x_api_token"].ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals(RegistryManager.Instance.ApiToken, StringComparison.OrdinalIgnoreCase);
      }

      public void StaticFileHandler(HttpListenerRequest req, HttpListenerResponse res)
      {
        string absolutePath = req.Url.AbsolutePath;
        string path = Path.Combine(this.mRootDir, absolutePath.Substring(absolutePath.Substring(1).IndexOf("/", StringComparison.OrdinalIgnoreCase) + 2).Replace("/", "\\"));
        if (System.IO.File.Exists(path))
        {
          byte[] buffer = System.IO.File.ReadAllBytes(path);
          if (path.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            res.Headers.Add("Content-Type: text/css");
          else if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            res.Headers.Add("Content-Type: application/javascript");
          res.OutputStream.Write(buffer, 0, buffer.Length);
        }
        else
        {
          Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File {0} doesn't exist", (object) path));
          res.StatusCode = 404;
          res.StatusDescription = "Not Found.";
        }
      }

      public void StaticFileChunkHandler(
        HttpListenerRequest req,
        HttpListenerResponse res,
        string dir = "")
      {
        string absolutePath = req.Url.AbsolutePath;
        string str1 = absolutePath.Substring(absolutePath.Substring(1).IndexOf("/", StringComparison.OrdinalIgnoreCase) + 2);
        string str2 = string.IsNullOrEmpty(dir) ? Path.Combine(this.mRootDir, str1.Replace("/", "\\")) : Path.Combine(dir, str1.Replace("/", "\\"));
        int num1 = 0;
        while (!System.IO.File.Exists(str2))
        {
          ++num1;
          Thread.Sleep(100);
          if (num1 == 20)
            break;
        }
        int num2 = 0;
        if (System.IO.File.Exists(str2))
        {
          FileInfo fileInfo = new FileInfo(str2);
          long length = fileInfo.Length;
          DateTimeOffset lastWriteTimeUtc = (DateTimeOffset) fileInfo.LastWriteTimeUtc;
          DateTimeOffset universalTime = new DateTimeOffset(lastWriteTimeUtc.Year, lastWriteTimeUtc.Month, lastWriteTimeUtc.Day, lastWriteTimeUtc.Hour, lastWriteTimeUtc.Minute, lastWriteTimeUtc.Second, lastWriteTimeUtc.Offset).ToUniversalTime();
          long num3 = universalTime.ToFileTime() ^ length;
          if (string.Equals("\"" + Convert.ToString(num3, 16) + "\"", req.Headers.Get("If-None-Match"), StringComparison.InvariantCultureIgnoreCase) && string.Equals(Convert.ToString((object) universalTime, (IFormatProvider) CultureInfo.InvariantCulture), req.Headers.Get("If-Modified-Since"), StringComparison.InvariantCultureIgnoreCase))
          {
            res.StatusCode = 304;
            res.StatusDescription = "Not Modified.";
          }
          else
          {
            if (str2.EndsWith(".flv", StringComparison.OrdinalIgnoreCase))
              res.Headers.Add("Content-Type: video/x-flv");
            if (str2.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
              res.Headers.Add("Content-Type: image/png");
            res.Headers.Add("Cache-Control: public,max-age=120");
            res.Headers.Add("ETag: \"" + Convert.ToString(num3, 16) + "\"");
            res.Headers.Add("Last-Modified: " + Convert.ToString((object) universalTime, (IFormatProvider) CultureInfo.InvariantCulture));
            int count1 = 1048576;
            bool flag = false;
            FileStream fileStream = new FileStream(str2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            while (true)
            {
              byte[] buffer = new byte[count1];
              int count2 = fileStream.Read(buffer, 0, count1);
              if (count2 != 0)
              {
                res.OutputStream.Write(buffer, 0, count2);
                flag = true;
              }
              else if (!(num2++ == 50 | flag))
                Thread.Sleep(100);
              else
                break;
            }
            fileStream.Close();
          }
        }
        else
        {
          Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File {0} doesn't exist", (object) str2));
          res.StatusCode = 404;
          res.StatusDescription = "Not Found.";
        }
      }
    }
  }
}
