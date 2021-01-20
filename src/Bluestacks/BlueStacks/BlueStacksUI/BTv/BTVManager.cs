// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BTv.BTVManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI.BTv
{
  internal sealed class BTVManager : IDisposable
  {
    private static object syncRoot = new object();
    public static string sNetwork = "twitch";
    private static string sBTvUrl = "https://cloud.bluestacks.com/bs4/btv/GetBTVFile";
    public object sPingBTVLock = new object();
    private static volatile BTVManager instance;
    public bool sStreaming;
    public bool sRecording;
    public bool sWasRecording;
    public bool sStopPingBTVThread;
    private CustomMessageWindow sWindow;
    private bool sDownloading;
    private LegacyDownloader sDownloader;
    private bool disposedValue;

    private BTVManager()
    {
    }

    public static BTVManager Instance
    {
      get
      {
        if (BTVManager.instance == null)
        {
          lock (BTVManager.syncRoot)
          {
            if (BTVManager.instance == null)
              BTVManager.instance = new BTVManager();
          }
        }
        return BTVManager.instance;
      }
    }

    public static bool sWritingToFile
    {
      set
      {
        HTTPServer.FileWriteComplete = !value;
      }
    }

    public void StartBlueStacksTV()
    {
      using (Process process = new Process())
      {
        string installDir = RegistryStrings.InstallDir;
        process.StartInfo.FileName = Path.Combine(installDir, "BlueStacksTV.exe");
        process.StartInfo.Arguments = "-u";
        process.Start();
        Thread.Sleep(1000);
        new Thread(new ThreadStart(this.StartPingBTVThread))
        {
          IsBackground = true
        }.Start();
      }
    }

    private void BtvWindow_Closing(object sender, CancelEventArgs e)
    {
    }

    internal static void BringToFront(CustomWindow win)
    {
      try
      {
        win.Dispatcher.Invoke((Delegate) (() =>
        {
          if (win.WindowState == WindowState.Minimized)
            win.WindowState = WindowState.Normal;
          win.Visibility = Visibility.Visible;
          win.Show();
          win.BringIntoView();
          if (win.Topmost)
            return;
          win.Topmost = true;
          win.Topmost = false;
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("An error was triggered in bringing BTv downloader to front", (object) ex.Message);
      }
    }

    public static void ReportObsErrorHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got ReportObsErrorHandler");
      HTTPUtils.ParseRequest(req);
      try
      {
        StreamManager.Instance.ReportObsError("obs_error");
        StreamManager.Instance = (StreamManager) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReportObsHandler");
        Logger.Error(ex.ToString());
      }
    }

    private void CancelBTvDownload(object sender, EventArgs e)
    {
      Logger.Info("User cancelled BTV download");
      this.sDownloading = false;
      if (this.sDownloader == null)
        return;
      this.sDownloader.AbortDownload();
      if (!BTVManager.IsBTVInstalled())
        return;
      Directory.Delete(RegistryStrings.ObsDir, true);
    }

    private void CancelDownloadConfirmation(object sender, EventArgs e)
    {
      MainWindow mainWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_BTV_DOWNLOAD_CANCEL", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_CANCEL", new EventHandler(this.CancelBTvDownload), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CONTINUE", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) mainWindow;
      customMessageWindow.ShowDialog();
    }

    internal static bool IsBTVInstalled()
    {
      return Directory.Exists(RegistryStrings.BtvDir) && Directory.Exists(RegistryStrings.ObsDir);
    }

    internal static bool IsDirectXComponentsInstalled()
    {
      string systemDirectory = Environment.SystemDirectory;
      string[] strArray = new string[4]
      {
        "D3DX10_43.DLL",
        "D3D10_1.DLL",
        "DXGI.DLL",
        "D3DCompiler_43.dll"
      };
      foreach (string path2 in strArray)
      {
        if (!System.IO.File.Exists(Path.Combine(systemDirectory, path2)))
          return false;
      }
      return true;
    }

    public void MaybeDownloadAndLaunchBTv(MainWindow parentWindow)
    {
      if (!BTVManager.IsBTVInstalled())
      {
        if (this.sDownloading && this.sWindow != null)
        {
          BTVManager.BringToFront((CustomWindow) this.sWindow);
        }
        else
        {
          ExtensionPopupControl btvExtPopup = new ExtensionPopupControl();
          btvExtPopup.LoadExtensionPopupFromFolder("BTVExtensionPopup");
          System.Action action1;
          System.Action action2;
          btvExtPopup.DownloadClicked += (EventHandler) ((o, e) =>
          {
            BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) btvExtPopup);
            this.sDownloading = true;
            this.sWindow = new CustomMessageWindow();
            BlueStacksUIBinding.Bind(this.sWindow.TitleTextBlock, "STRING_BTV_DOWNLOAD", "");
            BlueStacksUIBinding.Bind(this.sWindow.BodyTextBlock, "STRING_BTV_INSTALL_WAIT", "");
            BlueStacksUIBinding.Bind(this.sWindow.BodyWarningTextBlock, "STRING_BTV_WARNING", "");
            this.sWindow.AddButton(ButtonColors.Blue, "STRING_CANCEL", new EventHandler(this.CancelDownloadConfirmation), (string) null, false, (object) null, true);
            this.sWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
            this.sWindow.ProgressBarEnabled = true;
            this.sWindow.IsWindowMinizable = true;
            this.sWindow.IsWindowClosable = false;
            this.sWindow.ImageName = "BTVTopBar";
            this.sWindow.ShowInTaskbar = true;
            this.sWindow.Owner = (Window) parentWindow;
            this.sWindow.IsShowGLWindow = true;
            this.sWindow.Show();
            new Thread(closure_3 ?? (closure_3 = (ThreadStart) (() =>
            {
              if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
                BTVManager.sBTvUrl = RegistryManager.Instance.BtvDevServer;
              string redirectedUrl = BTVManager.GetRedirectedUrl(BTVManager.sBTvUrl);
              if (redirectedUrl == null)
              {
                Logger.Error("The download url was null");
              }
              else
              {
                string downloadPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(new Uri(redirectedUrl).LocalPath));
                this.sDownloader = new LegacyDownloader(3, redirectedUrl, downloadPath);
                this.sDownloader.Download((LegacyDownloader.UpdateProgressCallback) (percent =>
                {
                  // ISSUE: variable of a compiler-generated type
                  BTVManager.\u003C\u003Ec__DisplayClass25_0 cDisplayClass250 = this;
                  int percent1 = percent;
                  // ISSUE: reference to a compiler-generated field
                  this.sWindow.Dispatcher.Invoke((Delegate) (() => cDisplayClass250.\u003C\u003E4__this.sWindow.CustomProgressBar.Value = (double) percent1));
                }), (LegacyDownloader.DownloadCompletedCallback) (filePath =>
                {
                  this.sWindow.Dispatcher.Invoke((Delegate) (action1 ?? (action1 = (System.Action) (() =>
                  {
                    this.sWindow.CustomProgressBar.Value = 100.0;
                    this.sWindow.Close();
                  }))));
                  Logger.Info("Successfully downloaded BlueStacks TV");
                  this.sDownloading = false;
                  BTVManager.ExtractBTv(downloadPath);
                  parentWindow.Dispatcher.Invoke((Delegate) (action2 ?? (action2 = (System.Action) (() => parentWindow.mTopBar.mBtvButton.ImageName = "btv"))));
                }), (LegacyDownloader.ExceptionCallback) (ex => Logger.Error("Failed to download file: {0}. err: {1}", (object) downloadPath, (object) ex.Message)), (LegacyDownloader.ContentTypeCallback) null, (LegacyDownloader.SizeDownloadedCallback) null, (LegacyDownloader.PayloadInfoCallback) null);
              }
            })))
            {
              IsBackground = true
            }.Start();
          });
          btvExtPopup.Height = parentWindow.ActualHeight * 0.8;
          btvExtPopup.Width = btvExtPopup.Height * 16.0 / 9.0;
          ContainerWindow containerWindow = new ContainerWindow(parentWindow, (System.Windows.Controls.UserControl) btvExtPopup, (double) (int) btvExtPopup.Width, (double) (int) btvExtPopup.Height, false, true, false, -1.0, (Brush) null);
        }
      }
      else
        this.StartBlueStacksTV();
    }

    internal static void ReportOpenGLCaptureError(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got open gl CaptureError");
      try
      {
        StreamManager.Instance.ReportObsError("opengl_capture_error");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReportObsHandler");
        Logger.Error(ex.ToString());
      }
    }

    internal static void ReportCaptureError(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got ReportCaptureError");
      HTTPUtils.ParseRequest(req);
      try
      {
        StreamManager.Instance.ReportObsError("capture_error");
        StreamManager.Instance = (StreamManager) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReportObsHandler");
        Logger.Error(ex.ToString());
      }
    }

    internal static void ObsStatusHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got ObsStatus {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (requestData.Data.Count <= 0 || !(requestData.Data.AllKeys[0] == "Error") || StreamManager.sStopInitOBSQueue)
          return;
        if (string.Equals(requestData.Data[0], "OBSAlreadyRunning", StringComparison.InvariantCulture))
          StreamManager.sStopInitOBSQueue = true;
        if (StreamManager.Instance == null)
          return;
        new Thread((ThreadStart) (() => StreamManager.Instance.ReportObsError(requestData.Data[0])))
        {
          IsBackground = true
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ObsStatus");
        Logger.Error(ex.ToString());
      }
    }

    internal static string GetRedirectedUrl(string url)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.Method = "GET";
      httpWebRequest.AllowAutoRedirect = true;
      string str = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
      httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + str;
      httpWebRequest.Headers.Add("x_oem", RegistryManager.Instance.Oem);
      httpWebRequest.Headers.Set("x_email", RegistryManager.Instance.RegisteredEmail);
      httpWebRequest.Headers.Add("x_guid", RegistryManager.Instance.UserGuid);
      httpWebRequest.Headers.Add("x_prod_ver", RegistryManager.Instance.Version);
      httpWebRequest.Headers.Add("x_home_app_ver", RegistryManager.Instance.ClientVersion);
      try
      {
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
            {
              JObject jobject = JObject.Parse(streamReader.ReadToEnd());
              return jobject["success"].ToObject<bool>() ? jobject["file_url"].ToString() : (string) null;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting redirected url for BTV " + ex.ToString());
        return (string) null;
      }
    }

    internal static bool ExtractBTv(string downloadPath)
    {
      try
      {
        if (System.IO.File.Exists(downloadPath))
        {
          if (MiscUtils.Extract7Zip(downloadPath, RegistryManager.Instance.UserDefinedDir) == 0)
            return true;
          Logger.Error("Could not extract BTv zip file.");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Could not extract BTv zip file. Error: " + ex.ToString());
      }
      return false;
    }

    public void StartPingBTVThread()
    {
      lock (this.sPingBTVLock)
      {
        Logger.Info("Starting btv ping thread");
        while (true)
        {
          this.PingBTV();
          if (!this.sStopPingBTVThread)
            Thread.Sleep(5000);
          else
            break;
        }
      }
    }

    public void ShowStreamWindow()
    {
      if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
        this.StartBlueStacksTV();
      else
        BTVManager.SendBTVAsyncRequest("showstreamwindow", (Dictionary<string, string>) null);
    }

    public void HideStreamWindow()
    {
      if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
        return;
      BTVManager.SendBTVAsyncRequest("hidestreamwindow", (Dictionary<string, string>) null);
    }

    public void HideStreamWindowFromTaskbar()
    {
      BTVManager.SendBTVAsyncRequest("hidestreamwindowfromtaskbar", (Dictionary<string, string>) null);
    }

    public static void GetStreamDimensionInfo(
      out int startX,
      out int startY,
      out int width,
      out int height)
    {
      Point p = new Point();
      MainWindow activatedWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      activatedWindow.Dispatcher.Invoke((Delegate) (() => p = activatedWindow.mFrontendGrid.TranslatePoint(new Point(0.0, 0.0), (UIElement) activatedWindow.mFrontendGrid)));
      startX = Convert.ToInt32(p.X) * SystemUtils.GetDPI() / 96;
      startY = Convert.ToInt32(p.Y) * SystemUtils.GetDPI() / 96;
      width = (int) activatedWindow.mFrontendGrid.ActualWidth * SystemUtils.GetDPI() / 96;
      height = (int) activatedWindow.mFrontendGrid.ActualHeight * SystemUtils.GetDPI() / 96;
    }

    public void PingBTV()
    {
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        string json = BTVManager.SendBTVRequest("ping", (Dictionary<string, string>) null);
        JArray.Parse(json);
        JObject jobject = JObject.Parse(json[0].ToString((IFormatProvider) CultureInfo.InvariantCulture));
        if (jobject["success"].ToObject<bool>())
        {
          flag1 = jobject["recording"].ToObject<bool>();
          flag2 = jobject["streaming"].ToObject<bool>();
        }
        Logger.Info("Ping BTV response recording: {0}, streaming: {1}", (object) flag1, (object) flag2);
        this.sStopPingBTVThread = false;
      }
      catch (Exception ex)
      {
        this.sStopPingBTVThread = true;
        Logger.Error("PingBTV : {0}", (object) ex.Message);
      }
      this.sRecording = flag1;
      this.sStreaming = flag2;
    }

    public void SetFrontendPosition(int width, int height, bool isPortrait)
    {
      if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
        return;
      BTVManager.SendBTVRequest("setfrontendposition", new Dictionary<string, string>()
      {
        {
          nameof (width),
          width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (height),
          height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (isPortrait),
          isPortrait.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      });
    }

    public void WindowResized()
    {
      if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
        return;
      try
      {
        BTVManager.SendBTVRequest("windowresized", (Dictionary<string, string>) null);
      }
      catch (Exception ex)
      {
        Logger.Error("{0}", (object) ex);
      }
    }

    public void StreamStarted()
    {
      BTVManager.sWritingToFile = true;
      this.sRecording = true;
      this.sStreaming = true;
    }

    public void StreamStopped()
    {
      BTVManager.sWritingToFile = false;
      this.sStreaming = false;
      this.sRecording = false;
      BTVManager.RestrictWindowResize(false);
    }

    public void RecordStarted()
    {
      BTVManager.sWritingToFile = true;
      this.sRecording = true;
      this.sWasRecording = true;
    }

    public void SetConfig()
    {
      int startX;
      int startY;
      int width;
      int height;
      BTVManager.GetStreamDimensionInfo(out startX, out startY, out width, out height);
      BTVManager.SendBTVRequest("setconfig", new Dictionary<string, string>()
      {
        {
          "startX",
          startX.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "startY",
          startY.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "width",
          width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "height",
          height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      });
    }

    public void RecordStopped()
    {
      BTVManager.sWritingToFile = false;
      this.sRecording = false;
      BTVManager.RestrictWindowResize(false);
    }

    public void SendTabChangeData(string[] tabChangedData)
    {
      new Thread((ThreadStart) (() => BTVManager.SendBTVRequest("tabchangeddata", new Dictionary<string, string>()
      {
        {
          "type",
          tabChangedData[0]
        },
        {
          "name",
          tabChangedData[1]
        },
        {
          "data",
          tabChangedData[2]
        }
      })))
      {
        IsBackground = true
      }.Start();
    }

    public static void ReplayBufferSaved()
    {
      MainWindow mainWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      mainWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        SaveFileDialog saveFileDialog = new SaveFileDialog()
        {
          Filter = "Flash Video (*.flv)|*.flv",
          FilterIndex = 1,
          RestoreDirectory = true,
          FileName = "Replay"
        };
        if (saveFileDialog.ShowDialog() != DialogResult.OK)
          return;
        System.IO.File.Copy(Path.Combine(RegistryManager.Instance.ClientInstallDir, "replay.flv"), saveFileDialog.FileName);
      }));
    }

    public void Stop()
    {
      if (!this.sStreaming && !this.sRecording)
        return;
      BTVManager.SendBTVRequest("sessionswitch", (Dictionary<string, string>) null);
      this.sWasRecording = false;
    }

    public void CloseBTV()
    {
      this.sWasRecording = false;
    }

    public void CheckNewFiltersAvailable()
    {
      BTVManager.SendBTVRequest("checknewfilters", (Dictionary<string, string>) null);
    }

    public static void SendBTVAsyncRequest(string request, Dictionary<string, string> data)
    {
      new Thread((ThreadStart) (() =>
      {
        Logger.Info("Sending btv async request");
        BTVManager.SendBTVRequest(request, data);
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static string SendBTVRequest(string _1, Dictionary<string, string> _2)
    {
      return "";
    }

    public static void RestrictWindowResize(bool enable)
    {
      MainWindow activatedWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      activatedWindow.Dispatcher.Invoke((Delegate) (() => activatedWindow.RestrictWindowResize(enable)));
    }

    public void RecordVideoOfApp()
    {
      if (StreamManager.Instance == null)
        StreamManager.Instance = new StreamManager(BlueStacksUIUtils.DictWindows.Values.First<MainWindow>());
      string handle;
      string pid;
      StreamManager.GetStreamConfig(out handle, out pid);
      StreamManager.Instance.Init(handle, pid);
      StreamManager.Instance.SetHwnd(handle);
      StreamManager.Instance.EnableVideoRecording(true);
      StreamManager.Instance.StartObs();
      StreamManager.Instance.StartRecordForVideo();
    }

    private void StopRecordVideo()
    {
      StreamManager.Instance.StopRecord();
    }

    internal void RecordStartedVideo()
    {
    }

    public void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.disposedValue = true;
    }

    ~BTVManager()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
