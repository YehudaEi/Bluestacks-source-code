// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.HTTPUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace BlueStacks.Common
{
  public static class HTTPUtils
  {
    private const string sLoopbackUrl = "http://127.0.0.1";

    public static string MultiInstanceServerUrl
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.Instance.MultiInstanceServerPort);
      }
    }

    public static string PartnerServerUrl(string oem = "bgp")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.RegistryManagers[oem].PartnerServerPort);
    }

    public static string AgentServerUrl(string oem = "bgp")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.RegistryManagers[oem].AgentServerPort);
    }

    public static string FrontendServerUrl(string vmName = "Android", string oem = "bgp")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.RegistryManagers[oem].Guest[vmName].FrontendServerPort);
    }

    public static string GuestServerUrl(string vmName = "Android", string oem = "bgp")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.RegistryManagers[oem].Guest[vmName].BstAndroidPort);
    }

    public static string UrlForBstCommandProcessor(string url)
    {
      try
      {
        Uri uri = new Uri(url);
        foreach (string vm in RegistryManager.Instance.VmList)
        {
          if (uri.Segments.Length > 1 && string.Compare("ping", uri.Segments[1], StringComparison.OrdinalIgnoreCase) != 0 && uri.Port == RegistryManager.Instance.Guest[vm].BstAndroidPort)
            return vm;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error Occured, Err: {0}", (object) ex.ToString());
      }
      return (string) null;
    }

    public static NameValueCollection GetRequestHeaderCollection(string vmName)
    {
      NameValueCollection nameValueCollection = new NameValueCollection()
      {
        {
          "x_oem",
          RegistryManager.Instance.Oem
        },
        {
          "x_email",
          RegistryManager.Instance.RegisteredEmail
        },
        {
          "x_machine_id",
          GuidUtils.GetBlueStacksMachineId()
        },
        {
          "x_version_machine_id",
          GuidUtils.GetBlueStacksVersionId()
        }
      };
      if (!string.IsNullOrEmpty(vmName))
      {
        if (vmName.Contains("Android"))
        {
          nameValueCollection.Add("vmname", vmName);
          nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry(vmName));
          nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry(vmName));
          if (string.Equals(vmName, "Android", StringComparison.InvariantCultureIgnoreCase))
            nameValueCollection.Add("vmid", "0");
          else
            nameValueCollection.Add("vmid", vmName.Split('_')[1]);
        }
        else
        {
          nameValueCollection.Add("vmid", vmName);
          if (vmName.Equals("0", StringComparison.InvariantCultureIgnoreCase))
          {
            nameValueCollection.Add("vmname", "Android");
            nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry("Android"));
            nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry("Android"));
          }
          else
          {
            nameValueCollection.Add("vmname", "Android_" + vmName);
            nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry("Android_" + vmName));
            nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry("Android_" + vmName));
          }
        }
      }
      return nameValueCollection;
    }

    public static RequestData ParseRequest(HttpListenerRequest req)
    {
      return HTTPUtils.ParseRequest(req, true);
    }

    public static RequestData ParseRequest(HttpListenerRequest req, bool printData)
    {
      RequestData requestData1 = new RequestData();
      bool flag = false;
      string s = (string) null;
      requestData1.Headers = req?.Headers;
      requestData1.RequestVmId = 0;
      requestData1.RequestVmName = "Android";
      foreach (string allKey in requestData1.Headers.AllKeys)
      {
        if (requestData1.Headers[allKey].Contains("multipart"))
        {
          s = "--" + requestData1.Headers[allKey].Substring(requestData1.Headers[allKey].LastIndexOf("=", StringComparison.OrdinalIgnoreCase) + 1);
          Logger.Debug("boundary: {0}", (object) s);
          flag = true;
        }
        if (allKey.Contains("oem", StringComparison.InvariantCultureIgnoreCase) && requestData1.Headers[allKey] != null)
          requestData1.Oem = requestData1.Headers[allKey];
        else if (allKey == "vmid" && requestData1.Headers[allKey] != null)
        {
          if (!requestData1.Headers[allKey].Equals("0", StringComparison.OrdinalIgnoreCase))
          {
            requestData1.RequestVmId = int.Parse(requestData1.Headers["vmid"], (IFormatProvider) CultureInfo.InvariantCulture);
            if (requestData1.RequestVmName == "Android")
            {
              RequestData requestData2 = requestData1;
              requestData2.RequestVmName = requestData2.RequestVmName + "_" + requestData1.Headers[allKey].ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
          }
        }
        else if (allKey == "vmname" && requestData1.Headers[allKey] != null)
          requestData1.RequestVmName = requestData1.Headers[allKey].ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      requestData1.QueryString = req.QueryString;
      if (!req.HasEntityBody)
        return requestData1;
      Stream inputStream = req.InputStream;
      byte[] buffer1 = new byte[16384];
      MemoryStream memoryStream = new MemoryStream();
      int count1;
      while ((count1 = inputStream.Read(buffer1, 0, buffer1.Length)) > 0)
        memoryStream.Write(buffer1, 0, count1);
      byte[] array = memoryStream.ToArray();
      memoryStream.Close();
      inputStream.Close();
      Logger.Debug("byte array size {0}", (object) array.Length);
      string str1 = Encoding.UTF8.GetString(array);
      if (!flag)
      {
        if (!req.ContentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase))
        {
          requestData1.Data = HttpUtility.ParseQueryString(str1);
        }
        else
        {
          JObject jobject = JObject.Parse(str1);
          NameValueCollection nameValueCollection = new NameValueCollection();
          foreach (string name in jobject.Properties().Select<JProperty, string>((Func<JProperty, string>) (p => p.Name)).ToList<string>())
            nameValueCollection.Add(name, jobject[name].ToString());
          requestData1.Data = nameValueCollection;
        }
        return requestData1;
      }
      byte[] bytes1 = Encoding.UTF8.GetBytes(s);
      List<int> intList = HTTPUtils.IndexOf(array, bytes1);
      for (int index = 0; index < intList.Count - 1; ++index)
      {
        Logger.Info("Creating part");
        int srcOffset1 = intList[index];
        int num = intList[index + 1];
        int count2 = num - srcOffset1;
        byte[] bytes2 = new byte[count2];
        Logger.Debug("Start: {0}, End: {1}, Length: {2}", (object) srcOffset1, (object) num, (object) count2);
        Logger.Debug("byteData length: {0}", (object) array.Length);
        Buffer.BlockCopy((Array) array, srcOffset1, (Array) bytes2, 0, count2);
        Logger.Debug("bytePart length: {0}", (object) bytes2.Length);
        string input = Encoding.UTF8.GetString(bytes2);
        Match match1 = new Regex("(?<=Content\\-Type:)(.*?)(?=\\r\\n)").Match(input);
        Match match2 = new Regex("(?<=filename\\=\\\")(.*?)(?=\\\")").Match(input);
        string name = new Regex("(?<=name\\=\\\")(.*?)(?=\\\")").Match(input).Value.Trim();
        Logger.Info("Got name: {0}", (object) name);
        if (match1.Success && match2.Success)
        {
          Logger.Debug("Found file");
          Logger.Debug("Got contenttype: {0}", (object) match1.Value.Trim());
          string path2 = match2.Value.Trim();
          Logger.Info("Got filename: {0}", (object) path2);
          int srcOffset2 = input.IndexOf("\r\n\r\n", StringComparison.OrdinalIgnoreCase) + "\r\n\r\n".Length;
          Encoding.UTF8.GetBytes("\r\n" + s);
          int count3 = count2 - srcOffset2;
          byte[] buffer2 = new byte[count3];
          Logger.Debug("startindex: {0}, contentlength: {1}", (object) srcOffset2, (object) count3);
          Buffer.BlockCopy((Array) bytes2, srcOffset2, (Array) buffer2, 0, count3);
          string path1 = RegistryStrings.BstUserDataDir;
          if (path2.StartsWith("tombstone", StringComparison.OrdinalIgnoreCase))
            path1 = RegistryStrings.BstLogsDir;
          try
          {
            string path = Path.Combine(path1, path2);
            FileStream fileStream = System.IO.File.OpenWrite(path);
            fileStream.Write(buffer2, 0, count3);
            fileStream.Close();
            requestData1.Files.Add(name, path);
          }
          catch (Exception ex)
          {
            Logger.Warning("Exception in generating file: " + ex.ToString());
          }
        }
        else
        {
          Logger.Info("No file in this part");
          int startIndex = input.LastIndexOf("\r\n\r\n", StringComparison.OrdinalIgnoreCase);
          string str2 = input.Substring(startIndex, input.Length - startIndex).Trim();
          if (printData)
            Logger.Info("Got value: {0}", (object) str2);
          else
            Logger.Info("Value hidden");
          requestData1.Data.Add(name, str2);
        }
      }
      return requestData1;
    }

    private static List<int> IndexOf(byte[] searchWithin, byte[] searchFor)
    {
      List<int> intList = new List<int>();
      int startIndex = 0;
      int num = Array.IndexOf<byte>(searchWithin, searchFor[0], startIndex);
      Logger.Debug("boundary size = {0}", (object) searchFor.Length);
      do
      {
        int index = 0;
        while (num + index < searchWithin.Length && (int) searchWithin[num + index] == (int) searchFor[index])
        {
          ++index;
          if (index == searchFor.Length)
          {
            intList.Add(num);
            Logger.Debug("Got boundary postion: {0}", (object) num);
            break;
          }
        }
        if (num + index <= searchWithin.Length)
          num = Array.IndexOf<byte>(searchWithin, searchFor[0], num + index);
        else
          break;
      }
      while (num != -1);
      return intList;
    }

    public static string MergeQueryParams(
      string urlOriginal,
      string urlOverideParams,
      bool paramsOnly = false)
    {
      NameValueCollection nameValueCollection = !paramsOnly ? HttpUtility.ParseQueryString(new UriBuilder(urlOverideParams).Query) : HttpUtility.ParseQueryString(urlOverideParams);
      UriBuilder uriBuilder = new UriBuilder(urlOriginal);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      foreach (object key in nameValueCollection.Keys)
        queryString.Set(key.ToString(), nameValueCollection[key.ToString()]);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri.OriginalString;
    }

    public static void Write(StringBuilder sb, HttpListenerResponse res)
    {
      HTTPUtils.Write(sb?.ToString(), res);
    }

    public static void Write(string s, HttpListenerResponse res)
    {
      try
      {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        if (res == null)
          return;
        res.ContentLength64 = (long) bytes.Length;
        res.OutputStream.Write(bytes, 0, bytes.Length);
        res.OutputStream.Flush();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in writing response to http output stream:{0}", (object) ex);
      }
    }

    public static HTTPServer SetupServer(
      int startingPort,
      int maxPort,
      Dictionary<string, HTTPServer.RequestHandler> routes,
      string s_RootDir)
    {
      HTTPServer httpServer = (HTTPServer) null;
      int port;
      for (port = startingPort; port < maxPort; ++port)
      {
        try
        {
          httpServer = new HTTPServer(port, routes, s_RootDir);
          httpServer.Start();
          port = httpServer.Port;
          Logger.Info("Server listening on port " + httpServer.Port.ToString());
          if (!string.IsNullOrEmpty(httpServer.RootDir))
          {
            Logger.Info("Serving static content from " + httpServer.RootDir);
            break;
          }
          break;
        }
        catch (Exception ex)
        {
          Logger.Warning("Error occured, port: {0} Err: {1}", (object) port, (object) ex);
        }
      }
      if (port == maxPort || httpServer == null)
      {
        Logger.Fatal("No free port available or server could not be started, exiting.");
        Environment.Exit(2);
      }
      return httpServer;
    }

    public static void SendRequestToMultiInstanceAsync(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToMultiInstance(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToMultiInstance(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToClientAsync(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToClient(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToClient(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToEngineAsync(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToEngine(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "", oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToEngine. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToEngine(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "", oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToEngine. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToAgentAsync(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToAgent(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem, true);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToAgent. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToAgent(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem, true);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToAgent. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToGuestAsync(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToGuest(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToGuest. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToGuest(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToGuest. route: {0}, \n{1}", (object) route, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToCloudAsync(
      string api,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0)
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToCloud(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToCloud. route: {0}, \n{1}", (object) api, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToCloud(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToCloud. route: {0}, \n{1}", (object) api, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static void SendRequestToCloudWithParamsAsync(
      string api,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0)
    {
      if (retries == 1)
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            HTTPUtils.SendRequestToCloudWithParams(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToCloudWithParams. route: {0}, \n{1}", (object) api, (object) ex);
          }
        }));
      else
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToCloudWithParams(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec);
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in SendRequestToCloudWithParams. route: {0}, \n{1}", (object) api, (object) ex);
          }
        }))
        {
          IsBackground = true
        }.Start();
    }

    public static string SendRequestToMultiInstance(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) HTTPUtils.MultiInstanceServerUrl, (object) route), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem);
    }

    public static string SendRequestToClient(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (oem == null)
        oem = "bgp";
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) HTTPUtils.PartnerServerUrl(oem), (object) route), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem);
    }

    public static string SendRequestToEngine(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string destinationVmName = "",
      string oem = "bgp")
    {
      if (string.IsNullOrEmpty(destinationVmName))
        destinationVmName = vmName;
      if (oem == null)
        oem = "bgp";
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) HTTPUtils.FrontendServerUrl(destinationVmName, oem), (object) route), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem);
    }

    public static string SendRequestToAgent(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp",
      bool isCheckForAgentRunning = true)
    {
      if (oem == null)
        oem = "bgp";
      if (isCheckForAgentRunning && !ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lock" + oem))
      {
        Process process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "HD-Agent.exe");
        Logger.Info("Utils: Starting Agent");
        process.Start();
        if (!Utils.WaitForAgentPingResponse(vmName, oem))
          return (string) null;
      }
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) HTTPUtils.AgentServerUrl(oem), (object) route), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem);
    }

    public static string SendRequestToGuest(
      string route,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      string oem = "bgp")
    {
      if (oem == null)
        oem = "bgp";
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) HTTPUtils.GuestServerUrl(vmName, oem), (object) route), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem);
    }

    public static string SendRequestToCloud(
      string api,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      bool isOnUIThreadOnPurpose = false)
    {
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) api), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, isOnUIThreadOnPurpose, "bgp");
    }

    public static string SendRequestToCloudWithParams(
      string api,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0)
    {
      return HTTPUtils.SendHTTPRequest(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) api), (string) null, (string) null, (string) null), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp");
    }

    public static string SendRequestToNCSoftAgent(
      int port,
      string api,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0)
    {
      return HTTPUtils.SendHTTPRequest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}/{2}", (object) "http://127.0.0.1", (object) port, (object) api), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp");
    }

    private static string SendHTTPRequest(
      string url,
      Dictionary<string, string> data = null,
      string vmName = "Android",
      int timeout = 0,
      Dictionary<string, string> headers = null,
      bool printResponse = false,
      int retries = 1,
      int sleepTimeMSec = 0,
      bool isOnUIThreadOnPurpose = false,
      string oem = "bgp")
    {
      string str;
      if (data == null)
      {
        Logger.Info("Sending GET to {0}", (object) url);
        str = BstHttpClient.Get(url, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
      }
      else
      {
        Logger.Info("Sending POST to {0}", (object) url);
        str = BstHttpClient.Post(url, data, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
      }
      if (printResponse)
        Logger.Info("Loopback resp: {0}", (object) str);
      else
        Logger.Debug("Loopback resp: {0}", (object) str);
      return str;
    }

    public static void WriteSuccessArrayJson(HttpListenerResponse res, string reason = "")
    {
      if (string.IsNullOrEmpty(reason))
        HTTPUtils.Write(JSonTemplates.SuccessArrayJSonTemplate, res);
      else
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "success",
              (JToken) true
            },
            {
              nameof (reason),
              (JToken) reason
            }
          }
        }.ToString(Formatting.None), res);
    }

    public static void WriteErrorArrayJson(HttpListenerResponse res, string reason = "")
    {
      if (string.IsNullOrEmpty(reason))
        HTTPUtils.Write(JSonTemplates.FailedArrayJSonTemplate, res);
      else
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "success",
              (JToken) false
            },
            {
              nameof (reason),
              (JToken) reason
            }
          }
        }.ToString(Formatting.None), res);
    }

    public static void WriteArrayJson(HttpListenerResponse res, Dictionary<string, string> data)
    {
      JArray jarray = new JArray();
      if (data != null)
      {
        JObject jobject = new JObject();
        foreach (KeyValuePair<string, string> keyValuePair in data)
          jobject.Add(keyValuePair.Key, (JToken) keyValuePair.Value);
        jarray.Add((JToken) jobject);
      }
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    public static void WriteArrayJson(HttpListenerResponse res, Dictionary<string, object> data)
    {
      JArray jarray = new JArray();
      if (data != null)
      {
        JObject jobject = new JObject();
        foreach (KeyValuePair<string, object> keyValuePair in data)
          jobject.Add(keyValuePair.Key, JToken.FromObject(keyValuePair.Value));
        jarray.Add((JToken) jobject);
      }
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    public static void WriteSuccessJson(HttpListenerResponse res, string reason = "")
    {
      if (string.IsNullOrEmpty(reason))
        HTTPUtils.Write(JSonTemplates.SuccessJSonTemplate, res);
      else
        HTTPUtils.Write(new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            nameof (reason),
            (JToken) reason
          }
        }.ToString(Formatting.None), res);
    }

    public static void WriteErrorJson(HttpListenerResponse res, string reason = "")
    {
      if (string.IsNullOrEmpty(reason))
        HTTPUtils.Write(JSonTemplates.FailedJSonTemplate, res);
      else
        HTTPUtils.Write(new JObject()
        {
          {
            "success",
            (JToken) false
          },
          {
            nameof (reason),
            (JToken) reason
          }
        }.ToString(Formatting.None), res);
    }
  }
}
