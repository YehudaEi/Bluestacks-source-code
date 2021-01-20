// Decompiled with JetBrains decompiler
// Type: LegacyDownloader
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

public class LegacyDownloader
{
  private List<KeyValuePair<Thread, LegacyDownloader.Worker>> mWorkers;
  private WebHeaderCollection mResponseHeaders;
  private readonly string mUrl;
  private string mFileName;
  private int mNrWorkers;
  private LegacyDownloader.UpdateProgressCallback mUpdateProgressCallback;
  private LegacyDownloader.DownloadCompletedCallback mDownloadCompletedCallback;
  private LegacyDownloader.ExceptionCallback mExceptionCallback;
  private LegacyDownloader.ContentTypeCallback mContentTypeCallback;
  private LegacyDownloader.SizeDownloadedCallback mSizeDownloadedCallback;
  private LegacyDownloader.PayloadInfoCallback mPayloadInfoCallback;

  public LegacyDownloader(int nrWorkers, string url, string fileName)
  {
    this.mUrl = url;
    this.mFileName = fileName;
    this.mNrWorkers = nrWorkers;
  }

  public void Download(
    LegacyDownloader.UpdateProgressCallback updateProgressCb,
    LegacyDownloader.DownloadCompletedCallback downloadedCb,
    LegacyDownloader.ExceptionCallback exceptionCb,
    LegacyDownloader.ContentTypeCallback contentTypeCb = null,
    LegacyDownloader.SizeDownloadedCallback sizeDownloadedCb = null,
    LegacyDownloader.PayloadInfoCallback pInfoCb = null)
  {
    this.mUpdateProgressCallback = updateProgressCb;
    this.mDownloadCompletedCallback = downloadedCb;
    this.mExceptionCallback = exceptionCb;
    this.mContentTypeCallback = contentTypeCb;
    this.mSizeDownloadedCallback = sizeDownloadedCb;
    this.mPayloadInfoCallback = pInfoCb;
    Logger.Info("Downloading {0} to: {1}", (object) this.mUrl, (object) this.mFileName);
    string mFileName = this.mFileName;
    try
    {
      string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName(this.mFileName));
      string filePath;
      if (System.IO.File.Exists(path))
      {
        Logger.Info("{0} already downloaded to {1}", (object) this.mUrl, (object) path);
        filePath = path;
      }
      else
      {
        PayloadInfo remotePayloadInfo;
        try
        {
          remotePayloadInfo = this.GetRemotePayloadInfo(this.mUrl);
          if (pInfoCb != null)
            pInfoCb(remotePayloadInfo.Size);
          if (remotePayloadInfo.InvalidHTTPStatusCode)
          {
            Logger.Error("Invalid http status code.");
            exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_STATUS_CODE, (IFormatProvider) CultureInfo.InvariantCulture)));
            return;
          }
          string mResponseHeader = this.mResponseHeaders["Content-Type"];
          if (mResponseHeader == "application/vnd.android.package-archive")
            this.mFileName = Path.ChangeExtension(this.mFileName, ".apk");
          filePath = this.mFileName;
          if (contentTypeCb != null)
          {
            if (!contentTypeCb(mResponseHeader))
            {
              Logger.Info("Cancelling download");
              return;
            }
          }
        }
        catch (WebException ex)
        {
          if (ex.Status == WebExceptionStatus.NameResolutionFailure)
          {
            Logger.Error("The hostname could not be resolved. Url = " + this.mUrl);
            exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_HOSTNAME_NOT_RESOLVED, (IFormatProvider) CultureInfo.InvariantCulture)));
            return;
          }
          if (ex.Status == WebExceptionStatus.Timeout)
          {
            Logger.Error("The operation has timed out. Url = " + this.mUrl);
            exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_OPERATION_TIMEOUT, (IFormatProvider) CultureInfo.InvariantCulture)));
            return;
          }
          Logger.Error("A WebException has occured. Url = " + this.mUrl);
          exceptionCb((Exception) ex);
          return;
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to send to {0}", (object) this.mUrl));
          throw;
        }
        if (System.IO.File.Exists(this.mFileName))
        {
          if (LegacyDownloader.IsPayloadOk(this.mFileName, remotePayloadInfo.Size))
          {
            Logger.Info(this.mUrl + " already downloaded");
            goto label_27;
          }
          else
            System.IO.File.Delete(this.mFileName);
        }
        if (!remotePayloadInfo.SupportsRangeRequest)
          this.mNrWorkers = 1;
        this.mWorkers = this.MakeWorkers(this.mNrWorkers, this.mUrl, this.mFileName, remotePayloadInfo.Size);
        Logger.Info("Starting download of " + this.mFileName);
        int prevAverageTotalPercent = 0;
        LegacyDownloader.StartWorkers(this.mWorkers, (LegacyDownloader.ProgressCallback) (() =>
        {
          int num = 0;
          long size = 0;
          foreach (KeyValuePair<Thread, LegacyDownloader.Worker> mWorker in this.mWorkers)
          {
            num += mWorker.Value.PercentComplete;
            size += mWorker.Value.TotalFileDownloaded;
          }
          LegacyDownloader.SizeDownloadedCallback downloadedCallback = sizeDownloadedCb;
          if (downloadedCallback != null)
            downloadedCallback(size);
          int percent = num / this.mWorkers.Count;
          if (percent != prevAverageTotalPercent)
            updateProgressCb(percent);
          prevAverageTotalPercent = percent;
        }));
        LegacyDownloader.WaitForWorkers(this.mWorkers);
        LegacyDownloader.MakePayload(this.mNrWorkers, this.mFileName);
        if (!LegacyDownloader.IsPayloadOk(this.mFileName, remotePayloadInfo.Size))
        {
          string str = "Downloaded file not of the correct size";
          Logger.Info(str);
          System.IO.File.Delete(this.mFileName);
          throw new Exception(str);
        }
        Logger.Info("File downloaded correctly");
        LegacyDownloader.DeletePayloadParts(this.mNrWorkers, this.mFileName);
      }
label_27:
      downloadedCb(filePath);
    }
    catch (Exception ex)
    {
      Logger.Error("Exception in Download. err: " + ex.ToString());
      exceptionCb(ex);
    }
  }

  public static string MakePartFileName(string fileName, int id)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_part_{1}", (object) fileName, (object) id);
  }

  private static long GetSizeFromContentRange(HttpWebResponse res)
  {
    string[] strArray = res.Headers["Content-Range"].Split('/');
    return Convert.ToInt64(strArray[strArray.Length - 1], (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private PayloadInfo GetRemotePayloadInfo(string url)
  {
    HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
    req.Method = "Head";
    req.KeepAlive = false;
    HttpWebResponse response;
    PayloadInfo payloadInfo;
    try
    {
      LegacyDownloader.Add64BitRange(req, 0L, 0L);
      response = (HttpWebResponse) req.GetResponse();
      string httpResponseHeaders = LegacyDownloader.GetHTTPResponseHeaders(response);
      this.mResponseHeaders = response.Headers;
      Logger.Warning(httpResponseHeaders);
      payloadInfo = response.StatusCode != HttpStatusCode.PartialContent ? (response.StatusCode != HttpStatusCode.OK ? new PayloadInfo(false, 0L, true) : (!httpResponseHeaders.Contains("Accept-Ranges: bytes") ? new PayloadInfo(false, response.ContentLength, false) : new PayloadInfo(true, response.ContentLength, false))) : new PayloadInfo(true, LegacyDownloader.GetSizeFromContentRange(response), false);
    }
    catch (Exception ex)
    {
      Logger.Error(ex.ToString());
      throw;
    }
    response.Close();
    return payloadInfo;
  }

  private List<KeyValuePair<Thread, LegacyDownloader.Worker>> MakeWorkers(
    int nrWorkers,
    string url,
    string payloadFileName,
    long payloadSize)
  {
    long num = payloadSize / (long) nrWorkers;
    List<KeyValuePair<Thread, LegacyDownloader.Worker>> keyValuePairList = new List<KeyValuePair<Thread, LegacyDownloader.Worker>>();
    for (int id = 0; id < nrWorkers; ++id)
    {
      long from = (long) id * num;
      long to = id != nrWorkers - 1 ? (long) (id + 1) * num - 1L : (long) (id + 1) * num + payloadSize % (long) nrWorkers - 1L;
      KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair = new KeyValuePair<Thread, LegacyDownloader.Worker>(new Thread(new ParameterizedThreadStart(this.DoWork))
      {
        IsBackground = true
      }, new LegacyDownloader.Worker(id, url, payloadFileName, new Range(from, to)));
      keyValuePairList.Add(keyValuePair);
    }
    return keyValuePairList;
  }

  private static void StartWorkers(
    List<KeyValuePair<Thread, LegacyDownloader.Worker>> workers,
    LegacyDownloader.ProgressCallback progressCallback)
  {
    foreach (KeyValuePair<Thread, LegacyDownloader.Worker> worker in workers)
    {
      worker.Value.ProgressCallback = progressCallback;
      worker.Key.Start((object) worker.Value);
    }
  }

  private static void MakePayload(int nrWorkers, string payloadName)
  {
    Stream stream1 = (Stream) new FileStream(payloadName, FileMode.Create, FileAccess.Write, FileShare.None);
    int count1 = 16384;
    byte[] buffer = new byte[count1];
    for (int id = 0; id < nrWorkers; ++id)
    {
      Stream stream2 = (Stream) new FileStream(LegacyDownloader.MakePartFileName(payloadName, id), FileMode.Open, FileAccess.Read);
      int count2;
      while ((count2 = stream2.Read(buffer, 0, count1)) > 0)
        stream1.Write(buffer, 0, count2);
      stream2.Close();
    }
    stream1.Flush();
    stream1.Close();
  }

  private static void DeletePayloadParts(int nrParts, string payloadName)
  {
    for (int id = 0; id < nrParts; ++id)
      System.IO.File.Delete(LegacyDownloader.MakePartFileName(payloadName, id));
  }

  private static string GetHTTPResponseHeaders(HttpWebResponse res)
  {
    return "HTTP Response Headers\n" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StatusCode: {0}\n", (object) (int) res.StatusCode) + res.Headers?.ToString();
  }

  public void DoWork(object data)
  {
    LegacyDownloader.Worker worker = (LegacyDownloader.Worker) data;
    Range range = worker?.Range;
    Stream stream1 = (Stream) null;
    HttpWebResponse res = (HttpWebResponse) null;
    Stream stream2 = (Stream) null;
    try
    {
      Logger.Info("WorkerId {0} range.From = {1}, range.To = {2}", (object) worker.Id, (object) range.From, (object) range.To);
      HttpWebRequest req = (HttpWebRequest) WebRequest.Create(worker.URL);
      req.KeepAlive = true;
      if (System.IO.File.Exists(worker.PartFileName))
      {
        stream1 = (Stream) new FileStream(worker.PartFileName, FileMode.Append, FileAccess.Write, FileShare.None);
        if (stream1.Length == range.Length)
        {
          worker.TotalFileDownloaded = stream1.Length;
          worker.PercentComplete = 100;
          Logger.Info("WorkerId {0} already downloaded", (object) worker.Id);
          return;
        }
        worker.TotalFileDownloaded = stream1.Length;
        worker.PercentComplete = (int) (stream1.Length * 100L / range.Length);
        Logger.Info("WorkerId {0} Resuming from range.From = {1}, range.To = {2}", (object) worker.Id, (object) (range.From + stream1.Length), (object) range.To);
        if (this.mNrWorkers > 1)
          LegacyDownloader.Add64BitRange(req, range.From + stream1.Length, range.To);
      }
      else
      {
        worker.TotalFileDownloaded = 0L;
        worker.PercentComplete = 0;
        stream1 = (Stream) new FileStream(worker.PartFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        if (this.mNrWorkers > 1)
          LegacyDownloader.Add64BitRange(req, range.From + stream1.Length, range.To);
      }
      req.ReadWriteTimeout = 60000;
      res = (HttpWebResponse) req.GetResponse();
      long contentLength = res.ContentLength;
      stream2 = res.GetResponseStream();
      int count1 = 65536;
      byte[] buffer = new byte[count1];
      long num = 0;
      Logger.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WorkerId {0}\n", (object) worker.Id) + LegacyDownloader.GetHTTPResponseHeaders(res));
      int count2;
      while ((count2 = stream2.Read(buffer, 0, count1)) > 0)
      {
        if (worker.Cancelled)
          throw new OperationCanceledException("Download cancelled by user.");
        stream1.Write(buffer, 0, count2);
        num += (long) count2;
        worker.TotalFileDownloaded = stream1.Length;
        worker.PercentComplete = (int) (stream1.Length * 100L / range.Length);
      }
      if (contentLength != num)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "totalContentRead({0}) != contentLength({1})", (object) num, (object) contentLength));
    }
    catch (Exception ex)
    {
      worker.Exception = ex;
      Logger.Error(ex.ToString());
    }
    finally
    {
      stream2?.Close();
      res?.Close();
      if (stream1 != null)
      {
        stream1.Flush();
        stream1.Close();
      }
    }
    Logger.Info("WorkerId {0} Finished", (object) worker.Id);
  }

  private static bool IsPayloadOk(string payloadFileName, long remoteSize)
  {
    long length = new FileInfo(payloadFileName).Length;
    Logger.Info("payloadSize = " + length.ToString() + " remoteSize = " + remoteSize.ToString());
    return length == remoteSize;
  }

  public void AbortDownload()
  {
    if (this.mWorkers == null)
      return;
    Logger.Info("Downloader: Aborting all threads...");
    foreach (KeyValuePair<Thread, LegacyDownloader.Worker> mWorker in this.mWorkers)
    {
      try
      {
        mWorker.Value.Cancelled = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Downloader: could not abort thread. Error: " + ex.Message);
      }
    }
  }

  private static void WaitForWorkers(
    List<KeyValuePair<Thread, LegacyDownloader.Worker>> workers)
  {
    foreach (KeyValuePair<Thread, LegacyDownloader.Worker> worker in workers)
      worker.Key.Join();
    foreach (KeyValuePair<Thread, LegacyDownloader.Worker> worker in workers)
    {
      if (worker.Value.Exception != null)
        throw new WorkerException(worker.Value.Exception.Message, worker.Value.Exception);
    }
  }

  private static void Add64BitRange(HttpWebRequest req, long start, long end)
  {
    MethodInfo method = typeof (WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
    string str1 = "Range";
    string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "bytes={0}-{1}", (object) start, (object) end);
    method.Invoke((object) req.Headers, new object[2]
    {
      (object) str1,
      (object) str2
    });
  }

  public delegate void UpdateProgressCallback(int percent);

  public delegate void DownloadCompletedCallback(string filePath);

  public delegate void ExceptionCallback(Exception e);

  public delegate bool ContentTypeCallback(string contentType);

  public delegate void SizeDownloadedCallback(long size);

  public delegate void PayloadInfoCallback(long pInfo);

  private delegate void ProgressCallback();

  private class Worker
  {
    private readonly string m_PayloadName;
    private int m_PercentComplete;

    public Worker(int id, string url, string payloadName, Range range)
    {
      this.Id = id;
      this.URL = url;
      this.m_PayloadName = payloadName;
      this.Range = range;
    }

    public bool Cancelled { get; set; }

    public int Id { get; }

    public string PartFileName
    {
      get
      {
        return LegacyDownloader.MakePartFileName(this.m_PayloadName, this.Id);
      }
    }

    public Range Range { get; }

    public string URL { get; }

    public int PercentComplete
    {
      get
      {
        return this.m_PercentComplete;
      }
      set
      {
        this.m_PercentComplete = value;
        this.ProgressCallback();
      }
    }

    public long TotalFileDownloaded { get; set; }

    public LegacyDownloader.ProgressCallback ProgressCallback { get; set; }

    public Exception Exception { get; set; }
  }
}
