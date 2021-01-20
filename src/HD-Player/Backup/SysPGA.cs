// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.SysPGA
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Player
{
  public class SysPGA : IGraphics
  {
    private const string OpenGL_Native_DLL = "HD-OpenGl-Native.dll";

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaSetAstcConfig(int config);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern int PgaIsHwAstcSupported();

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaLoggerInit(Logger.HdLoggerCallback cb);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern int PgaUtilsIsHotAttach();

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern int PgaServerInit(
      IntPtr h,
      int x,
      int y,
      int width,
      int height,
      SafeWaitHandle evt,
      int glRenderMode,
      string engine);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaServerHandleCommand(int scancode);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern IntPtr PgaServerGetSubwindow();

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern IntPtr PgaServerHandleOrientation(
      float hscale,
      float vscale,
      int orientation);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern IntPtr PgaServerHandleAppActivity(string package, string activity);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern int GetPgaServerInitStatus(
      StringBuilder glVendor,
      StringBuilder glRenderer,
      StringBuilder glVersion);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern int PgaIsGLES3();

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaSetFps(int fps);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaShowFps(int enable);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void PgaSetVSyncState(int enable);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void ToggleFarmMode(bool enable);

    [DllImport("HD-OpenGl-Native.dll")]
    private static extern void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc);

    void IGraphics.PgaSetAstcConfig(int config)
    {
      SysPGA.PgaSetAstcConfig(config);
    }

    int IGraphics.PgaIsHwAstcSupported()
    {
      return SysPGA.PgaIsHwAstcSupported();
    }

    int IGraphics.GetPgaServerInitStatus(
      StringBuilder glVendor,
      StringBuilder glRenderer,
      StringBuilder glVersion)
    {
      return SysPGA.GetPgaServerInitStatus(glVendor, glRenderer, glVersion);
    }

    void IGraphics.PgaLoggerInit(Logger.HdLoggerCallback cb)
    {
      SysPGA.PgaLoggerInit(cb);
    }

    IntPtr IGraphics.PgaServerGetSubwindow()
    {
      return SysPGA.PgaServerGetSubwindow();
    }

    IntPtr IGraphics.PgaServerHandleAppActivity(string package, string activity)
    {
      return SysPGA.PgaServerHandleAppActivity(package, activity);
    }

    void IGraphics.PgaServerHandleCommand(int scancode)
    {
      SysPGA.PgaServerHandleCommand(scancode);
    }

    IntPtr IGraphics.PgaServerHandleOrientation(
      float hscale,
      float vscale,
      int orientation)
    {
      return SysPGA.PgaServerHandleOrientation(hscale, vscale, orientation);
    }

    int IGraphics.PgaServerInit(
      IntPtr h,
      int x,
      int y,
      int width,
      int height,
      SafeWaitHandle evt,
      int glRenderMode,
      string engine)
    {
      return SysPGA.PgaServerInit(h, x, y, width, height, evt, glRenderMode, engine);
    }

    int IGraphics.PgaUtilsIsHotAttach()
    {
      return SysPGA.PgaUtilsIsHotAttach();
    }

    int IGraphics.PgaIsGLES3()
    {
      return SysPGA.PgaIsGLES3();
    }

    void IGraphics.PgaSetFps(int fps)
    {
      SysPGA.PgaSetFps(fps);
    }

    void IGraphics.PgaShowFps(int enable)
    {
      SysPGA.PgaShowFps(enable);
    }

    void IGraphics.ToggleFarmMode(bool enable)
    {
      SysPGA.ToggleFarmMode(enable);
    }

    void IGraphics.ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc)
    {
      SysPGA.ImgdUpdateScreenPoint(ref xPos, ref yPos, ref crc);
    }

    void IGraphics.PgaSetVSyncState(int enable)
    {
      SysPGA.PgaSetVSyncState(enable);
    }
  }
}
