// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.CameraManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Player
{
  public class CameraManager
  {
    private static IntPtr s_IoHandle = IntPtr.Zero;
    private static object s_IoHandleLock = new object();
    private int framerate = 30;
    private int width = 640;
    private int height = 480;
    private int jpegQuality = 100;
    private bool cameraStoped = true;
    private IntPtr m_buffer = IntPtr.Zero;
    private CameraManager.fpStartStopCamera s_fpStartStopCamera;
    private static Monitor s_Monitor;
    private IntPtr overWrite;
    private Camera camera;
    private Camera.getFrameCB cb;
    private bool bShutDown;
    private int unit;
    private int keyEnableCam;
    private SupportedColorFormat m_color;
    private int m_StartCount;

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr handle);

    private void BstStartStopCamera(
      int startStop,
      int unit,
      int width,
      int height,
      int framerate)
    {
      lock (CameraManager.s_IoHandleLock)
      {
        if (this.unit != unit && startStop == 1)
        {
          this.camStop();
          this.m_StartCount = 0;
        }
        if (this.unit == unit && startStop == 0)
        {
          --this.m_StartCount;
          if (this.m_StartCount == 0)
            this.camStop();
        }
        if (startStop != 1)
          return;
        ++this.m_StartCount;
        this.camStart(unit, width, height, framerate);
        this.unit = unit;
      }
    }

    public static Monitor Monitor
    {
      get
      {
        return CameraManager.s_Monitor;
      }
      set
      {
        CameraManager.s_Monitor = value;
      }
    }

    public void InitCamera(string[] args)
    {
      if (args.Length != 1)
        throw new SystemException("InitCamera: Should have vmName as one arg");
      string vmName = args[0];
      this.keyEnableCam = RegistryManager.Instance.DefaultGuest.Camera;
      if (this.keyEnableCam != 1)
      {
        Logger.Info("Camera is Disabled");
      }
      else
      {
        uint vmId = MonitorLocator.Lookup(vmName);
        lock (CameraManager.s_IoHandleLock)
        {
          if (CameraManager.s_IoHandle != IntPtr.Zero)
            throw new SystemException("I/O handle is already open");
          Logger.Info("Attaching to monitor ID {0}", (object) vmId);
          CameraManager.s_IoHandle = HDPlusModule.CameraIoAttach(vmId);
          if (CameraManager.s_IoHandle == IntPtr.Zero)
            throw new SystemException("Cannot attach for I/O", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
        }
        this.s_fpStartStopCamera = new CameraManager.fpStartStopCamera(this.BstStartStopCamera);
        HDPlusModule.SetStartStopCamerCB(this.s_fpStartStopCamera);
        Logger.Info("Waiting for Camera messages...");
        new Thread((ThreadStart) (() =>
        {
          while (!this.bShutDown)
          {
            int num = HDPlusModule.CameraIoProcessMessages(CameraManager.s_IoHandle);
            if (num != 0)
            {
              Logger.Error("Camera: Cannot process VM messages. Error: " + num.ToString());
              this.Shutdown();
            }
          }
        }))
        {
          IsBackground = true
        }.Start();
      }
    }

    public void Shutdown()
    {
      lock (CameraManager.s_IoHandleLock)
      {
        if (this.keyEnableCam != 1)
          return;
        this.bShutDown = true;
        if (this.camera != null || !this.cameraStoped)
          this.camera.StopCamera();
        this.camera = (Camera) null;
        if (!(CameraManager.s_IoHandle != IntPtr.Zero))
          return;
        CameraManager.CloseHandle(CameraManager.s_IoHandle);
        CameraManager.s_IoHandle = IntPtr.Zero;
      }
    }

    public void getFrame(IntPtr ip, int width, int height, int stride)
    {
      if (ip == IntPtr.Zero || this.camera == null || (CameraManager.s_IoHandle == IntPtr.Zero || this.cameraStoped))
        return;
      IntPtr stream = ip;
      if (this.m_color == SupportedColorFormat.RGB24)
      {
        if (this.m_buffer == IntPtr.Zero)
          this.m_buffer = Marshal.AllocCoTaskMem(width * height * 2);
        HDPlusModule.convertRGB24toYUV422(ip, width, height, this.m_buffer);
        stream = this.m_buffer;
      }
      HDPlusModule.CameraSendCaptureStream(CameraManager.s_IoHandle, stream, width * height * 2, width, height, stride);
    }

    public void camStart(int unit, int w, int h, int f)
    {
      if (this.camera != null || this.keyEnableCam != 1 || !this.cameraStoped)
        return;
      if (w > 0)
        this.width = w;
      if (h > 0)
        this.height = h;
      if (f > 0)
        this.framerate = f;
      Logger.Info("Starting Camera {0}. Frame width: {1}, height: {2}, framerate: {3}", (object) unit, (object) this.width, (object) this.height, (object) this.framerate);
      this.cameraStoped = false;
      this.cb = new Camera.getFrameCB(this.getFrame);
      for (int index = 0; index < 2; ++index)
      {
        if (this.camera == null)
        {
          try
          {
            this.m_color = (SupportedColorFormat) index;
            this.camera = new Camera(unit, this.width, this.height, this.framerate, this.jpegQuality, this.m_color);
          }
          catch (ColorFormatNotSupported ex)
          {
            Logger.Info("Trying with other color." + ex.ToString());
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in to initialize the camera. Err : ", (object) ex.ToString());
          }
        }
        else
          break;
      }
      if (this.camera == null)
      {
        Logger.Error("Cannot start the host camera.");
      }
      else
      {
        this.camera.registerFrameCB(this.cb);
        this.camera.StartCamera();
      }
    }

    public void camStop()
    {
      if (this.camera == null || this.cameraStoped)
        return;
      Logger.Info("Stoping Camera.");
      this.cameraStoped = true;
      this.camera.StopCamera();
      this.camera = (Camera) null;
      if (!(this.m_buffer != IntPtr.Zero))
        return;
      Marshal.FreeCoTaskMem(this.m_buffer);
      this.m_buffer = IntPtr.Zero;
    }

    public void resumeCamera()
    {
      lock (CameraManager.s_IoHandleLock)
      {
        if (this.keyEnableCam != 1 || !this.cameraStoped || this.camera == null)
          return;
        Logger.Info("Resuming Camera");
        this.camStart(this.unit, this.width, this.height, this.framerate);
      }
    }

    public void pauseCamera()
    {
      lock (CameraManager.s_IoHandleLock)
      {
        if (this.keyEnableCam != 1 || this.camera == null || this.cameraStoped)
          return;
        Logger.Info("Pausing Camera");
        this.camStop();
      }
    }

    public delegate void fpStartStopCamera(
      int startStop,
      int unit,
      int width,
      int height,
      int framerate);
  }
}
