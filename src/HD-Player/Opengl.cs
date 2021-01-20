// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Opengl
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Text;
using System.Threading;

namespace BlueStacks.Player
{
  internal class Opengl
  {
    internal static GlWindowAction glWindowAction = GlWindowAction.None;
    internal static bool userInteracted = false;
    private static int mGLMode = int.MinValue;
    private static bool initialized = false;
    private static object syncRoot = new object();
    private static IGraphics mGraphics = (IGraphics) null;
    private static EventWaitHandle glReadyEvent;
    private const int GL_MODE_SYS_SOFT = 0;
    private const int GL_MODE_SYS_PGA = 1;
    private const int GL_MODE_SYS_GGA = 2;

    public static int GlMode
    {
      get
      {
        if (Opengl.mGLMode == int.MinValue)
          Opengl.mGLMode = RegistryManager.Instance.DefaultGuest.GlMode;
        return Opengl.mGLMode;
      }
      set
      {
        Opengl.mGLMode = value;
      }
    }

    public static IGraphics Graphics
    {
      get
      {
        if (Opengl.mGraphics == null)
        {
          lock (Opengl.syncRoot)
          {
            if (Opengl.mGraphics == null)
            {
              switch (Opengl.GlMode)
              {
                case 1:
                  Opengl.mGraphics = (IGraphics) new SysPGA();
                  break;
                case 2:
                  Opengl.mGraphics = (IGraphics) new SysGGA();
                  break;
              }
            }
          }
        }
        return Opengl.mGraphics;
      }
      set
      {
        Opengl.mGraphics = value;
      }
    }

    public static bool Init(
      string vmName,
      IntPtr h,
      int x,
      int y,
      int width,
      int height,
      Opengl.GlReadyHandler glReadyHandler,
      Opengl.GlInitFailedHandler glInitFailedHandler)
    {
      Logger.Info("GLMode: " + Opengl.GlMode.ToString());
      if (Opengl.GlMode == 0)
      {
        glReadyHandler();
        Opengl.SignalGlReady(vmName);
        return true;
      }
      Opengl.Graphics.PgaLoggerInit(Logger.GetHdLoggerCallback());
      EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      Logger.Info("Initializing System Renderer");
      Opengl.Graphics.PgaServerInit(h, 0, 0, width, height, eventWaitHandle.SafeWaitHandle, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].GlRenderMode, RegistryManager.Instance.CurrentEngine);
      eventWaitHandle.WaitOne();
      Opengl.initialized = true;
      if (!Opengl.GetPgaServerInitStatus())
      {
        glInitFailedHandler();
        return false;
      }
      Opengl.Graphics.PgaSetAstcConfig((int) RegistryManager.Instance.Guest[vmName].ASTCOption);
      new Thread((ThreadStart) (() =>
      {
        try
        {
          int num = Opengl.Graphics.PgaIsHwAstcSupported();
          Logger.Info("value of isHwAstcSuppported is .." + num.ToString());
          RegistryManager.Instance.Guest[vmName].IsHardwareAstcSupported = num == 1;
        }
        catch (Exception ex)
        {
          Logger.Error("error while gl3 check.." + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
      new Thread((ThreadStart) (() =>
      {
        try
        {
          int num = Opengl.Graphics.PgaIsGLES3();
          Logger.Info("value of gl3 is .." + num.ToString());
          RegistryManager.Instance.GLES3 = num == 1;
        }
        catch (Exception ex)
        {
          Logger.Error("error while gl3 check.." + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
      glReadyHandler();
      Opengl.SignalGlReady(vmName);
      Opengl.SetVsync(RegistryManager.Instance.Guest[vmName].EnableVSync);
      if (RegistryManager.Instance.Guest[vmName].ShowFPS == 1)
        Opengl.ShowFPS(1);
      return true;
    }

    public static void SetAstcOption(int astcConfig = 1)
    {
      Opengl.Graphics.PgaSetAstcConfig(astcConfig);
    }

    public static void ShowFPS(int isShowFPS = 1)
    {
      Opengl.Graphics.PgaShowFps(isShowFPS);
    }

    public static void SetVsync(int enable)
    {
      Opengl.Graphics.PgaSetVSyncState(enable);
    }

    public static void ToggleFarmMode(bool enable)
    {
      Opengl.Graphics.ToggleFarmMode(enable);
    }

    public static void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc)
    {
      Opengl.Graphics.ImgdUpdateScreenPoint(ref xPos, ref yPos, ref crc);
      Logger.Info("OPENGL: X: " + xPos.ToString() + "Y: " + yPos.ToString() + "crc: " + crc.ToString());
    }

    private static bool ToGenerateId()
    {
      return (uint) RegistryManager.Instance.DefaultGuest.UpdatedVersion > 0U;
    }

    private static bool GetPgaServerInitStatus()
    {
      StringBuilder glVendor = new StringBuilder(512);
      StringBuilder glRenderer = new StringBuilder(512);
      StringBuilder glVersion = new StringBuilder(512);
      Logger.Info("Calling GetPgaServerInitStatus");
      int num = -1;
      try
      {
        num = Opengl.Graphics.GetPgaServerInitStatus(glVendor, glRenderer, glVersion);
      }
      catch (AccessViolationException ex)
      {
        Logger.Info("Error Occured" + ex.ToString());
      }
      if (num != 0)
      {
        Logger.Info("PgaServerInit failed");
        return false;
      }
      Profile.GlVendor = glVendor.ToString();
      Profile.GlRenderer = glRenderer.ToString();
      Profile.GlVersion = glVersion.ToString();
      RegistryManager.Instance.AvailableGPUDetails = Profile.GlRenderer ?? "";
      Logger.Info("GlVendor: " + Profile.GlVendor);
      Logger.Info("GlRenderer: " + Profile.GlRenderer);
      Logger.Info("GlVersion: " + Profile.GlVersion);
      return true;
    }

    internal static IntPtr GetSubWindow()
    {
      return !Opengl.initialized ? IntPtr.Zero : Opengl.Graphics.PgaServerGetSubwindow();
    }

    public static bool ShowSubWindow()
    {
      IntPtr subWindow = Opengl.GetSubWindow();
      if (subWindow == IntPtr.Zero)
        return false;
      InteropWindow.ShowWindow(subWindow, 8);
      return true;
    }

    public static bool HideSubWindow()
    {
      IntPtr subWindow = Opengl.GetSubWindow();
      if (subWindow == IntPtr.Zero)
        return false;
      InteropWindow.ShowWindow(subWindow, 0);
      return true;
    }

    public static bool IsSubWindowVisible()
    {
      IntPtr subWindow = Opengl.GetSubWindow();
      return !(subWindow == IntPtr.Zero) && InteropWindow.IsWindowVisible(subWindow);
    }

    public static bool ResizeSubWindow(int x, int y, int cx, int cy)
    {
      InteropWindow.SetWindowPos(Opengl.GetSubWindow(), IntPtr.Zero, x, y, cx, cy, 16468U);
      return true;
    }

    public static void HandleOrientation(float hscale, float vscale, int orientation)
    {
      Opengl.Graphics.PgaServerHandleOrientation(hscale, vscale, orientation);
    }

    public static void HandleCommand(int scancode)
    {
      Opengl.Graphics.PgaServerHandleCommand(scancode);
    }

    public static void HandleAppActivity(string package, string activity)
    {
      Opengl.Graphics.PgaServerHandleAppActivity(package, activity);
    }

    public static void StopZygote(string vmName)
    {
      int num = 0;
      while (num < 3)
      {
        ++num;
        Monitor monitor;
        try
        {
          monitor = Manager.Open().Attach(MonitorLocator.Lookup(vmName), true);
        }
        catch (Exception ex)
        {
          Logger.Debug("Cannot attach to the monitor: " + ex.ToString());
          Thread.Sleep(500);
          Logger.Debug("Retrying...");
          continue;
        }
        try
        {
          monitor.SendControl(Monitor.BstInputControlType.BST_INPUT_CONTROL_TYPE_STOP);
        }
        catch (Exception ex)
        {
          Logger.Debug("Cannot stop Zygote: " + ex.ToString());
          Thread.Sleep(500);
          Logger.Debug("Retrying...");
          continue;
        }
        monitor?.Close();
        break;
      }
    }

    private static void SetId(string vmName)
    {
      Logger.Info("Starting id manager");
      string id = Id.GenerateID();
      bool flag;
      do
      {
        Thread.Sleep(50);
        flag = false;
        try
        {
          VMCommand vmCommand = new VMCommand();
          vmCommand.Attach(vmName);
          int num = vmCommand.Run(new string[2]
          {
            "iSetId",
            id
          });
          if (num != 0)
          {
            Logger.Info("Failed to set Id:" + num.ToString());
            flag = true;
          }
        }
        catch
        {
          Logger.Info("Retrying to set Id");
          flag = true;
        }
      }
      while (flag);
      Logger.Info("Set Id success!");
    }

    public static void StartZygote(string vmName)
    {
      Monitor monitor;
      while (true)
      {
        uint id = MonitorLocator.Lookup(vmName);
        Manager manager = Manager.Open();
        try
        {
          monitor = manager.Attach(id, true);
        }
        catch (Exception ex)
        {
          Logger.Debug("Cannot attach to the monitor: " + ex.ToString());
          Thread.Sleep(500);
          Logger.Debug("Retrying...");
          continue;
        }
        try
        {
          monitor.SendControl(Monitor.BstInputControlType.BST_INPUT_CONTROL_TYPE_START);
          break;
        }
        catch (Exception ex)
        {
          Logger.Debug("Cannot start Zygote: " + ex.ToString());
          Thread.Sleep(500);
          Logger.Debug("Retrying...");
        }
      }
      monitor?.Close();
    }

    private static void SignalGlReady(string vmName)
    {
      Opengl.glReadyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, string.Format("Global\\BlueStacks_Frontend_Gl_Ready_{0}", (object) vmName));
      Opengl.glReadyEvent.Set();
    }

    public delegate void GlReadyHandler();

    public delegate void GlInitFailedHandler();
  }
}
