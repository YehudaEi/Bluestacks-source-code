// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Downloader
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace BlueStacks.Common
{
  public class Downloader
  {
    private const int DEFAULT_BUFFER_LENGTH = 10485760;

    public event Downloader.DownloadRetryEventHandler DownloadRetry;

    public event Downloader.DownloadExceptionEventHandler DownloadException;

    public event Downloader.UnsupportedResumeEventHandler UnsupportedResume;

    public event Downloader.FilePayloadInfoReceivedHandler FilePayloadInfoReceived;

    public event Downloader.DownloadFileCompletedEventHandler DownloadFileCompleted;

    public event Downloader.DownloadProgressChangedEventHandler DownloadProgressChanged;

    public event Downloader.DownloadProgressPercentChangedEventHandler DownloadProgressPercentChanged;

    public event Downloader.DownloadCancelledEventHandler DownloadCancelled;

    public bool IsDownLoadCanceled { get; set; }

    public bool IsDownloadInProgress { get; private set; }

    protected virtual void OnDownloadProgressChanged(long bytes)
    {
      Downloader.DownloadProgressChangedEventHandler downloadProgressChanged = this.DownloadProgressChanged;
      if (downloadProgressChanged == null)
        return;
      downloadProgressChanged(bytes);
    }

    protected virtual void OnDownloadPercentProgressChanged(double percent)
    {
      Downloader.DownloadProgressPercentChangedEventHandler progressPercentChanged = this.DownloadProgressPercentChanged;
      if (progressPercentChanged == null)
        return;
      progressPercentChanged(percent);
    }

    protected virtual void OnDownloadException(Exception e)
    {
      Downloader.DownloadExceptionEventHandler downloadException = this.DownloadException;
      if (downloadException == null)
        return;
      downloadException(e);
    }

    protected virtual void OnDownloadFileCompleted()
    {
      Downloader.DownloadFileCompletedEventHandler downloadFileCompleted = this.DownloadFileCompleted;
      if (downloadFileCompleted == null)
        return;
      downloadFileCompleted((object) this, new EventArgs());
    }

    protected virtual void OnFilePayloadInfoReceived(long size)
    {
      Downloader.FilePayloadInfoReceivedHandler payloadInfoReceived = this.FilePayloadInfoReceived;
      if (payloadInfoReceived == null)
        return;
      payloadInfoReceived(size);
    }

    protected virtual void OnUnsupportedResume(HttpStatusCode code)
    {
      Downloader.UnsupportedResumeEventHandler unsupportedResume = this.UnsupportedResume;
      if (unsupportedResume == null)
        return;
      unsupportedResume(code);
    }

    protected virtual void OnDownloadCancelled()
    {
      Downloader.DownloadCancelledEventHandler downloadCancelled = this.DownloadCancelled;
      if (downloadCancelled == null)
        return;
      downloadCancelled();
    }

    protected virtual void OnDownloadRetryEvent()
    {
      Downloader.DownloadRetryEventHandler downloadRetry = this.DownloadRetry;
      if (downloadRetry == null)
        return;
      downloadRetry((object) this, new EventArgs());
    }

    public void DownloadFile(string url, string fileDestination)
    {
      this.IsDownloadInProgress = true;
      FileStream fileStream = (FileStream) null;
      HttpWebRequest httpWebRequest = (HttpWebRequest) null;
      HttpWebResponse httpWebResponse = (HttpWebResponse) null;
      try
      {
        if (System.IO.File.Exists(fileDestination))
        {
          Logger.Info("{0} already downloaded to {1}", (object) url, (object) fileDestination);
          this.OnDownloadFileCompleted();
        }
        else
        {
          string str = fileDestination + ".tmp";
          try
          {
            fileStream = new FileStream(str, FileMode.Append, FileAccess.Write, FileShare.None);
          }
          catch (Exception ex)
          {
            this.OnDownloadException(ex);
            return;
          }
          long length = fileStream.Length;
          int num1 = 0;
          while (true)
          {
            long num2 = length;
            try
            {
              httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
              Downloader.AddRangeToRequest((WebRequest) httpWebRequest, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-", (object) length), "bytes");
              httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
              HttpStatusCode statusCode = ((HttpWebResponse) ex.Response).StatusCode;
              if (statusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
              {
                Logger.Warning("Unsupported resume! {0}", (object) statusCode);
                fileStream?.Close();
                this.OnUnsupportedResume(statusCode);
                return;
              }
              Logger.Warning("An error occured while creating a request. WebEx: {0}", (object) ex.Message);
              goto label_40;
            }
            catch (Exception ex)
            {
              Logger.Warning("An error occured while creating a request. Ex: {0}", (object) ex.Message);
              goto label_40;
            }
            if (httpWebResponse.StatusCode != HttpStatusCode.PartialContent && httpWebResponse.StatusCode != HttpStatusCode.OK)
              Logger.Warning("Got an unexpected status code: {0}", (object) httpWebResponse.StatusCode);
            else if (length == 0L || httpWebResponse.StatusCode == HttpStatusCode.PartialContent)
            {
              long size = httpWebResponse.ContentLength + length;
              this.OnFilePayloadInfoReceived(size);
              Stream responseStream;
              try
              {
                responseStream = httpWebResponse.GetResponseStream();
              }
              catch (Exception ex)
              {
                Logger.Warning("An error occured while getting a response stream: {0}", (object) ex.Message);
                goto label_40;
              }
              byte[] buffer = new byte[10485760];
              while (true)
              {
                int count;
                try
                {
                  if (this.IsDownLoadCanceled)
                  {
                    fileStream.Close();
                    fileStream = (FileStream) null;
                    if (System.IO.File.Exists(fileDestination))
                      System.IO.File.Delete(fileDestination);
                    if (System.IO.File.Exists(str))
                      System.IO.File.Delete(str);
                    this.OnDownloadCancelled();
                    return;
                  }
                  count = responseStream.Read(buffer, 0, 10485760);
                }
                catch (Exception ex)
                {
                  Logger.Warning("Some error while reading from the stream. Ex: {0}", (object) ex.Message);
                  goto label_40;
                }
                if (count != 0)
                {
                  try
                  {
                    fileStream.Write(buffer, 0, count);
                  }
                  catch (Exception ex)
                  {
                    Logger.Warning("Some error while writing the stream to file. Ex: {0}", (object) ex.Message);
                    this.OnDownloadException(ex);
                    return;
                  }
                  length += (long) count;
                  this.OnDownloadProgressChanged(length);
                  this.OnDownloadPercentProgressChanged((double) Math.Round(Decimal.Divide((Decimal) length, (Decimal) size) * new Decimal(100), 2));
                }
                else
                  break;
              }
              if (length != size)
                Logger.Error("Stream does not have more bytes to read. {0} != {1}", (object) length, (object) size);
              else
                goto label_35;
            }
            else
              break;
label_40:
            this.OnDownloadRetryEvent();
            if (num2 == length)
              ++num1;
            if (num1 == 20)
              this.OnDownloadException((Exception) new UnknownErrorException());
            httpWebRequest?.Abort();
            httpWebRequest = (HttpWebRequest) null;
            httpWebResponse?.Close();
            httpWebResponse = (HttpWebResponse) null;
            int num3 = num1 <= 10 ? Convert.ToInt32(Math.Pow(2.0, (double) num1)) : 1800;
            Logger.Info("Will retry after {0}s", (object) num3);
            Thread.Sleep(num3 * 1000);
          }
          this.OnUnsupportedResume(httpWebResponse.StatusCode);
          return;
label_35:
          try
          {
            fileStream.Close();
            fileStream = (FileStream) null;
            System.IO.File.Move(str, fileDestination);
            this.OnDownloadFileCompleted();
          }
          catch (Exception ex)
          {
            Logger.Warning("Could not move file to destination. Ex: {0}", (object) ex.Message);
            this.OnDownloadException(ex);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Unable to download the file: {0}", (object) ex.Message);
        Downloader.ThrowOnFatalException(ex);
        this.OnDownloadException(ex);
      }
      finally
      {
        fileStream?.Close();
        httpWebRequest?.Abort();
        httpWebResponse?.Close();
        this.IsDownloadInProgress = false;
      }
    }

    private long GetSizeFromResponseHeaders(WebHeaderCollection headers)
    {
      return Convert.ToInt64(headers["Content-Range"].Split('/')[1], (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private static void ThrowOnFatalException(Exception e)
    {
      switch (e)
      {
        case ThreadAbortException _:
        case StackOverflowException _:
        case OutOfMemoryException _:
          throw e;
      }
    }

    private static void AddRangeToRequest(WebRequest req, string range, string rangeSpecifier = "bytes")
    {
      MethodInfo method = typeof (WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
      string str1 = "Range";
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) rangeSpecifier, (object) range);
      method.Invoke((object) req.Headers, new object[2]
      {
        (object) str1,
        (object) str2
      });
    }

    private long GetSizeFromContentRange(HttpWebResponse webResponse)
    {
      string[] strArray = webResponse.Headers["Content-Range"].Split('/');
      return Convert.ToInt64(strArray[strArray.Length - 1], (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public delegate void DownloadRetryEventHandler(object sender, EventArgs args);

    public delegate void DownloadFileCompletedEventHandler(object sender, EventArgs args);

    public delegate void FilePayloadInfoReceivedHandler(long size);

    public delegate void DownloadExceptionEventHandler(Exception e);

    public delegate void DownloadProgressChangedEventHandler(long bytes);

    public delegate void UnsupportedResumeEventHandler(HttpStatusCode sc);

    public delegate void DownloadCancelledEventHandler();

    public delegate void DownloadProgressPercentChangedEventHandler(double percent);
  }
}
