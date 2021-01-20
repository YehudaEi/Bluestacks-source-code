// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Camera
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Threading;

namespace BlueStacks.Player
{
  public class Camera
  {
    public IntPtr pFrame = IntPtr.Zero;
    protected Thread previewThread;
    private volatile bool m_bStop;
    private CaptureGraph VidCapture;
    private static Camera.getFrameCB s_sendFrame;
    private int m_Unit;
    private int m_Width;
    private int m_Height;
    private int m_Framerate;
    private int m_Quality;
    private SupportedColorFormat m_color;

    public bool registerFrameCB(Camera.getFrameCB cb)
    {
      if (cb == null)
        return false;
      Camera.s_sendFrame = new Camera.getFrameCB(cb.Invoke);
      return true;
    }

    public Camera(
      int unit,
      int width,
      int height,
      int framerate,
      int quality,
      SupportedColorFormat color)
    {
      this.m_bStop = true;
      this.m_Unit = unit;
      this.m_Width = width;
      this.m_Height = height;
      this.m_Framerate = framerate;
      this.m_Quality = quality;
      this.m_color = color;
      this.VidCapture = new CaptureGraph(this.m_Unit, this.m_Width, this.m_Height, this.m_Framerate, this.m_color);
    }

    public void StartCamera()
    {
      this.previewThread = new Thread(new ThreadStart(this.Run));
      this.previewThread.Start();
    }

    public void StopCamera()
    {
      if (this.previewThread != null)
      {
        this.m_bStop = true;
        this.previewThread.Join();
        if (this.VidCapture != null)
        {
          this.VidCapture.Dispose();
          this.VidCapture = (CaptureGraph) null;
        }
        if (Camera.s_sendFrame != null)
          Camera.s_sendFrame = (Camera.getFrameCB) null;
      }
      this.previewThread = (Thread) null;
    }

    protected void Run()
    {
      this.m_bStop = false;
      try
      {
        this.VidCapture.Run();
        do
        {
          try
          {
            this.pFrame = IntPtr.Zero;
            this.pFrame = this.VidCapture.getSignleFrame();
            if (Camera.s_sendFrame != null && this.pFrame != IntPtr.Zero)
              Camera.s_sendFrame(this.pFrame, this.VidCapture.Width, this.VidCapture.Height, this.VidCapture.Stride);
            if (!this.m_bStop)
              continue;
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in send frame callback. Err : {0}", (object) ex.ToString());
            throw;
          }
          this.VidCapture.Pause();
        }
        while (!this.m_bStop);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Graph Run. Err : {0}", (object) ex.ToString());
      }
      finally
      {
        if (this.VidCapture != null)
        {
          this.VidCapture.Dispose();
          this.VidCapture = (CaptureGraph) null;
        }
      }
    }

    public delegate void getFrameCB(IntPtr ip, int width, int height, int stride);
  }
}
