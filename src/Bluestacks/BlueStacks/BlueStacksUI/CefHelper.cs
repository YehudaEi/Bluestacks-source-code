// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CefHelper
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.IO;
using System.Windows;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
  internal class CefHelper : CefApp
  {
    private bool mDevToolEnable;

    public CefHelper()
    {
      if (RegistryManager.Instance.CefDevEnv != 0)
        return;
      this.mDevToolEnable = false;
    }

    protected override CefRenderProcessHandler GetRenderProcessHandler()
    {
      return (CefRenderProcessHandler) new RenderProcessHandler();
    }

    protected override void OnBeforeCommandLineProcessing(
      string processType,
      CefCommandLine commandLine)
    {
      if (!string.IsNullOrEmpty(processType))
        return;
      commandLine.AppendSwitch("disable-gpu");
      commandLine.AppendSwitch("disable-gpu-compositing");
      commandLine.AppendSwitch("disable-smooth-scrolling");
      commandLine.AppendSwitch("--enable-system-flash");
      commandLine.AppendSwitch("ppapi-flash-path", Path.Combine(RegistryManager.Instance.CefDataPath, "pepflashplayer.dll"));
      commandLine.AppendSwitch("plugin-policy", "allow");
      commandLine.AppendSwitch("enable-media-stream", "1");
      if (!this.mDevToolEnable)
        return;
      commandLine.AppendSwitch("enable-begin-frame-scheduling");
    }

    internal static bool CefInited { get; set; }

    internal static bool InitCef(string[] args, string mBSTProcessIdentifier)
    {
      try
      {
        Logger.Info("Install Boot: CefRuntime.Load");
        CefRuntime.Load(RegistryManager.Instance.CefDataPath);
      }
      catch (DllNotFoundException ex)
      {
        Logger.Info("Install Boot: DllNotFoundException");
        int num = (int) MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
        return false;
      }
      catch (CefRuntimeException ex)
      {
        Logger.Info("Install Boot: CefRuntimeException");
        int num = (int) MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
        return false;
      }
      catch (Exception ex)
      {
        Logger.Info("Install Boot: ex");
        int num = (int) MessageBox.Show(ex.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
        return false;
      }
      CefMainArgs args1 = new CefMainArgs(args);
      CefHelper cefHelper = new CefHelper();
      CefRuntime.EnableHighDpiSupport();
      if (CefRuntime.ExecuteProcess(args1, (CefApp) cefHelper, IntPtr.Zero) != -1)
        return false;
      string str = "Mozilla/5.0(Windows NT 6.2; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
      if (!SystemUtils.IsOs64Bit())
        str = "Mozilla/5.0(Windows NT 6.2; WOW64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
      CefSettings settings = new CefSettings()
      {
        SingleProcess = false,
        WindowlessRenderingEnabled = true,
        MultiThreadedMessageLoop = true,
        LogSeverity = CefLogSeverity.Verbose,
        BackgroundColor = new CefColor(byte.MaxValue, (byte) 39, (byte) 41, (byte) 65),
        CachePath = Path.Combine(RegistryManager.Instance.CefDataPath, "Cache"),
        PersistSessionCookies = true,
        UserAgent = str,
        Locale = RegistryManager.Instance.UserSelectedLocale
      };
      if (RegistryManager.Instance.CefDebugPort != 0)
        settings.RemoteDebuggingPort = RegistryManager.Instance.CefDebugPort;
      try
      {
        CefRuntime.Initialize(args1, settings, (CefApp) cefHelper, IntPtr.Zero);
        Logger.Info("Install Boot: cef Initialized");
      }
      catch (CefRuntimeException ex)
      {
        int num = (int) MessageBox.Show(ex.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
        return false;
      }
      CefHelper.CefInited = true;
      Logger.Info("Install Boot: cef Initialize completed");
      return true;
    }
  }
}
