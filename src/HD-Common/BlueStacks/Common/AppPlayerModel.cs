// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppPlayerModel
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

namespace BlueStacks.Common
{
  [Serializable]
  public class AppPlayerModel
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "app_player_win_version")]
    public string AppPlayerWinVersion { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "source")]
    public string Source { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "app_player_os_arch")]
    public string AppPlayerOsArch { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "oem")]
    public string AppPlayerOem { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "prod_ver")]
    public string AppPlayerProdVer { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "app_player_language")]
    public string AppPlayerLanguage { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "display_name")]
    public string AppPlayerOemDisplayName { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "download_url")]
    public string DownLoadUrl { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "abi_value")]
    public int AbiValue { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "suffix")]
    public string Suffix { get; set; }

    [JsonIgnore]
    private string DownloadPath { get; set; } = string.Empty;

    [JsonIgnore]
    private Downloader MDownloader { get; set; }

    [JsonIgnore]
    private bool IsOemDownloadCancelling { get; set; }

    public bool DownLoadOem(
      Downloader.DownloadExceptionEventHandler downloadException,
      Downloader.DownloadProgressChangedEventHandler downloadProgressChanged,
      Downloader.DownloadFileCompletedEventHandler downloadFileCompleted,
      Downloader.FilePayloadInfoReceivedHandler filePayloadInfoReceived,
      Downloader.UnsupportedResumeEventHandler unsupportedResume,
      bool isRetry = false)
    {
      try
      {
        if (!string.IsNullOrEmpty(this.DownLoadUrl))
        {
          Logger.Info("The new engine url is : " + this.DownLoadUrl);
          if (!isRetry)
            this.DownloadPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(new Uri(this.DownLoadUrl).LocalPath));
          new Thread((ThreadStart) (() =>
          {
            while (this.IsOemDownloadCancelling)
              Thread.Sleep(1000);
            if (!isRetry && System.IO.File.Exists(this.DownloadPath))
            {
              Downloader.DownloadFileCompletedEventHandler completedEventHandler = downloadFileCompleted;
              if (completedEventHandler == null)
                return;
              completedEventHandler((object) null, (EventArgs) null);
            }
            else
            {
              this.MDownloader = new Downloader();
              this.MDownloader.DownloadException += new Downloader.DownloadExceptionEventHandler(this.Downloader_DownloadException);
              this.MDownloader.DownloadException += downloadException;
              this.MDownloader.DownloadProgressChanged += downloadProgressChanged;
              this.MDownloader.DownloadFileCompleted += downloadFileCompleted;
              this.MDownloader.FilePayloadInfoReceived += filePayloadInfoReceived;
              this.MDownloader.UnsupportedResume += new Downloader.UnsupportedResumeEventHandler(this.Downloader_UnsupportedResume);
              this.MDownloader.UnsupportedResume += unsupportedResume;
              this.MDownloader.DownloadCancelled += new Downloader.DownloadCancelledEventHandler(this.Downloader_Cancelled);
              this.MDownloader.DownloadFile(this.DownLoadUrl, this.DownloadPath);
            }
          })).Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error while download file: {0}", (object) ex);
        return false;
      }
      return true;
    }

    private void Downloader_DownloadException(Exception e)
    {
      this.DeleteFiles();
    }

    private void Downloader_UnsupportedResume(HttpStatusCode sc)
    {
      this.DeleteFiles();
    }

    private void Downloader_Cancelled()
    {
      this.IsOemDownloadCancelling = false;
    }

    public void CancelOemDownload()
    {
      if (this.MDownloader == null || !this.MDownloader.IsDownloadInProgress)
        return;
      this.IsOemDownloadCancelling = true;
      this.MDownloader.IsDownLoadCanceled = true;
    }

    private void DeleteFiles()
    {
      try
      {
        if (System.IO.File.Exists(this.DownloadPath))
          System.IO.File.Delete(this.DownloadPath);
        if (!System.IO.File.Exists(this.DownloadPath + ".tmp"))
          return;
        System.IO.File.Delete(this.DownloadPath + ".tmp");
      }
      catch (Exception ex)
      {
        Logger.Error("Error while deleting files from temp folder " + this.DownloadPath + " " + ex?.ToString());
      }
    }

    public int InstallOem()
    {
      string str = Path.Combine(Path.GetDirectoryName(RegistryManager.Instance.UserDefinedDir), "BlueStacks" + (this.AppPlayerOem == "bgp" ? string.Empty : "_" + this.AppPlayerOem));
      Process process = Process.Start(new ProcessStartInfo()
      {
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-s -pddir:\"{0}\"", (object) str),
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = this.DownloadPath,
        UseShellExecute = false
      });
      process.WaitForExit();
      if (process.ExitCode == 0 && RegistryManager.CheckOemInRegistry(this.AppPlayerOem, "Android"))
        this.DeleteFiles();
      return process.ExitCode;
    }
  }
}
