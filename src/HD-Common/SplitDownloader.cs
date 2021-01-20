// Decompiled with JetBrains decompiler
// Type: SplitDownloader
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using BlueStacks.Common;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

public class SplitDownloader
{
  private string m_ManifestURL;
  private string m_DirPath;
  private string m_UserGUID;
  private string m_UserAgent;
  private SplitDownloader.ProgressCb m_ProgressCb;
  private SplitDownloader.CompletedCb m_CompletedCb;
  private SplitDownloader.ExceptionCb m_ExceptionCb;
  private SplitDownloader.FileSizeCb m_FileSizeCb;
  private int m_NrWorkers;
  private SerialWorkQueue[] m_Workers;
  private bool m_WorkersStarted;
  private Manifest m_Manifest;
  private float m_PercentDownloaded;

  public SplitDownloader(string manifestURL, string dirPath, string userGUID, int nrWorkers)
  {
    this.m_ManifestURL = manifestURL;
    this.m_DirPath = dirPath;
    this.m_UserGUID = userGUID;
    this.m_UserAgent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SplitDownloader {0}/{1}/{2}", (object) "BlueStacks", (object) "4.250.0.1070", (object) this.m_UserGUID);
    this.m_NrWorkers = nrWorkers;
    this.m_Workers = new SerialWorkQueue[nrWorkers];
    for (int index = 0; index < this.m_NrWorkers; ++index)
      this.m_Workers[index] = new SerialWorkQueue();
    this.m_WorkersStarted = false;
  }

  public void Download(
    SplitDownloader.ProgressCb progressCb,
    SplitDownloader.CompletedCb completedCb,
    SplitDownloader.ExceptionCb exceptionCb)
  {
    this.Download(progressCb, completedCb, exceptionCb, (SplitDownloader.FileSizeCb) null);
  }

  public void Download(
    SplitDownloader.ProgressCb progressCb,
    SplitDownloader.CompletedCb completedCb,
    SplitDownloader.ExceptionCb exceptionCb,
    SplitDownloader.FileSizeCb fileSizeCb)
  {
    this.m_ProgressCb = progressCb;
    this.m_CompletedCb = completedCb;
    this.m_ExceptionCb = exceptionCb;
    this.m_FileSizeCb = fileSizeCb;
    try
    {
      this.m_Manifest = this.GetManifest();
      this.GetManifestFilePath();
      if (this.m_FileSizeCb != null)
        this.m_FileSizeCb(this.m_Manifest.FileSize);
      this.StartWorkers();
      this.m_ProgressCb(this.m_Manifest.PercentDownloaded());
      for (int index = 0; (long) index < this.m_Manifest.Count; ++index)
      {
        SerialWorkQueue.Work work = this.MakeWork(this.m_Manifest[index]);
        this.m_Workers[index % this.m_NrWorkers].Enqueue(work);
      }
      this.StopAndWaitWorkers();
      if (!this.m_Manifest.Check())
        throw new CheckFailedException();
      string filePath = this.m_Manifest.MakeFile();
      this.m_Manifest.DeleteFileParts();
      this.m_Manifest.DeleteManifest();
      this.m_CompletedCb(filePath);
    }
    catch (Exception ex)
    {
      Logger.Error(ex.ToString());
      this.m_ExceptionCb(ex);
    }
    finally
    {
      if (this.m_WorkersStarted)
        this.StopAndWaitWorkers();
    }
  }

  private void StartWorkers()
  {
    for (int index = 0; index < this.m_NrWorkers; ++index)
      this.m_Workers[index].Start();
    this.m_WorkersStarted = true;
  }

  private void StopAndWaitWorkers()
  {
    for (int index = 0; index < this.m_NrWorkers; ++index)
      this.m_Workers[index].Stop();
    for (int index = 0; index < this.m_NrWorkers; ++index)
      this.m_Workers[index].Join();
    this.m_WorkersStarted = false;
  }

  private SerialWorkQueue.Work MakeWork(FilePart filePart)
  {
    return (SerialWorkQueue.Work) (() =>
    {
      try
      {
        if (filePart.Check())
          Logger.Info(filePart.Path + " is already downloaded");
        else
          this.DownloadFilePart(filePart);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    });
  }

  private string GetManifestFilePath()
  {
    return Path.Combine(this.m_DirPath, Path.GetFileName(new Uri(this.m_ManifestURL).AbsolutePath));
  }

  private Manifest GetManifest()
  {
    string manifestFilePath = this.GetManifestFilePath();
    Logger.Info("Downloading " + this.m_ManifestURL + " to " + manifestFilePath);
    bool downloaded = false;
    Exception capturedException = (Exception) null;
    SplitDownloader.DownloadFile(this.m_ManifestURL, manifestFilePath, this.m_UserAgent, (SplitDownloader.DownloadFileProgressCb) ((downloadedSize, totalSize) => Logger.Info("Downloaded (" + downloadedSize.ToString() + " bytes) out of " + totalSize.ToString())), (SplitDownloader.DownloadFileCompletedCb) (filePath =>
    {
      downloaded = true;
      Logger.Info("Downloaded " + this.m_ManifestURL + " to " + filePath);
    }), (SplitDownloader.DownloadFileExceptionCb) (e =>
    {
      downloaded = false;
      capturedException = e;
      Logger.Error(e.ToString());
    }));
    if (!downloaded)
      throw capturedException;
    Manifest manifest = new Manifest(manifestFilePath);
    manifest.Build();
    return manifest;
  }

  private void DownloadFilePart(FilePart filePart)
  {
    string filePartURL = filePart.URL(this.m_ManifestURL);
    Logger.Info("Downloading " + filePartURL + " to " + filePart.Path);
    bool downloaded = false;
    Exception capturedException = (Exception) null;
    SplitDownloader.DownloadFile(filePartURL, filePart.Path, this.m_UserAgent, (SplitDownloader.DownloadFileProgressCb) ((downloadedSize, totalSize) =>
    {
      filePart.DownloadedSize = downloadedSize;
      if ((double) this.m_PercentDownloaded != (double) this.m_Manifest.PercentDownloaded())
        this.m_ProgressCb(this.m_Manifest.PercentDownloaded());
      this.m_PercentDownloaded = this.m_Manifest.PercentDownloaded();
    }), (SplitDownloader.DownloadFileCompletedCb) (filePath =>
    {
      downloaded = true;
      Logger.Info("Downloaded " + filePartURL + " to " + filePart.Path);
    }), (SplitDownloader.DownloadFileExceptionCb) (e =>
    {
      downloaded = false;
      capturedException = e;
      Logger.Error(e.ToString());
    }));
    if (!downloaded)
      throw capturedException;
  }

  private static void DownloadFile(
    string url,
    string filePath,
    string userAgent,
    SplitDownloader.DownloadFileProgressCb progressCb,
    SplitDownloader.DownloadFileCompletedCb completedCb,
    SplitDownloader.DownloadFileExceptionCb exceptionCb)
  {
    FileStream fileStream = (FileStream) null;
    HttpWebResponse httpWebResponse = (HttpWebResponse) null;
    Stream stream = (Stream) null;
    bool flag = false;
    try
    {
      if (System.IO.File.Exists(filePath))
        System.IO.File.Delete(filePath);
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.UserAgent = userAgent;
      httpWebRequest.KeepAlive = false;
      httpWebRequest.ReadWriteTimeout = 60000;
      httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
      long contentLength = httpWebResponse.ContentLength;
      stream = httpWebResponse.GetResponseStream();
      Logger.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP Response Header\nStatusCode: {0}\n{1}", (object) (int) httpWebResponse.StatusCode, (object) httpWebResponse.Headers));
      int count1 = 4096;
      byte[] buffer = new byte[count1];
      long downloaded = 0;
      fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
      int count2;
      while ((count2 = stream.Read(buffer, 0, count1)) > 0)
      {
        fileStream.Write(buffer, 0, count2);
        downloaded += (long) count2;
        progressCb(downloaded, contentLength);
      }
      if (contentLength != downloaded)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "totalContentRead({0}) != contentLength({1})", (object) downloaded, (object) contentLength));
      flag = true;
    }
    catch (Exception ex)
    {
      Logger.Error(ex.ToString());
      exceptionCb(ex);
    }
    finally
    {
      stream?.Close();
      httpWebResponse?.Close();
      if (fileStream != null)
      {
        fileStream.Flush();
        fileStream.Close();
        Thread.Sleep(1000);
      }
    }
    if (!flag)
      return;
    completedCb(filePath);
  }

  public delegate void ProgressCb(float percent);

  public delegate void CompletedCb(string filePath);

  public delegate void ExceptionCb(Exception e);

  public delegate void FileSizeCb(long fileSize);

  public delegate void DownloadFileProgressCb(long downloaded, long size);

  public delegate void DownloadFileCompletedCb(string filePath);

  public delegate void DownloadFileExceptionCb(Exception e);
}
