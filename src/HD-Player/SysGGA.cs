// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.SysGGA
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
  public class SysGGA : IGraphics
  {
    private const string GGL_Native_DLL = "libOpenglRender.dll";
    private const string GGL_Native_V2_DLL = "libGLES_V2_translator";

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaSetAstcConfig(int config);

    [DllImport("libOpenglRender.dll")]
    private static extern int PgaIsHwAstcSupported();

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaLoggerInit(Logger.HdLoggerCallback cb);

    [DllImport("libOpenglRender.dll")]
    private static extern int PgaUtilsIsHotAttach();

    [DllImport("libOpenglRender.dll")]
    private static extern int PgaServerInit(
      IntPtr h,
      int x,
      int y,
      int width,
      int height,
      SafeWaitHandle evt,
      int glRenderMode,
      string engine);

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaServerHandleCommand(int scancode);

    [DllImport("libOpenglRender.dll")]
    private static extern IntPtr PgaServerGetSubwindow();

    [DllImport("libOpenglRender.dll")]
    private static extern IntPtr PgaServerHandleOrientation(
      float hscale,
      float vscale,
      int orientation);

    [DllImport("libOpenglRender.dll")]
    private static extern IntPtr PgaServerHandleAppActivity(string package, string activity);

    [DllImport("libOpenglRender.dll")]
    private static extern int GetPgaServerInitStatus(
      StringBuilder glVendor,
      StringBuilder glRenderer,
      StringBuilder glVersion);

    [DllImport("libOpenglRender.dll")]
    private static extern int PgaIsGLES3();

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaSetFps(int fps);

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaShowFps(int enable);

    [DllImport("libOpenglRender.dll")]
    private static extern void ToggleFarmMode(bool enable);

    [DllImport("libOpenglRender.dll")]
    private static extern void PgaSetVSyncState(int enable);

    [DllImport("libGLES_V2_translator")]
    private static extern void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc);

    void IGraphics.PgaSetAstcConfig(int config)
    {
      SysGGA.PgaSetAstcConfig(config);
    }

    int IGraphics.PgaIsHwAstcSupported()
    {
      return SysGGA.PgaIsHwAstcSupported();
    }

    int IGraphics.GetPgaServerInitStatus(
      StringBuilder glVendor,
      StringBuilder glRenderer,
      StringBuilder glVersion)
    {
      return SysGGA.GetPgaServerInitStatus(glVendor, glRenderer, glVersion);
    }

    void IGraphics.PgaLoggerInit(Logger.HdLoggerCallback cb)
    {
      SysGGA.PgaLoggerInit(cb);
    }

    IntPtr IGraphics.PgaServerGetSubwindow()
    {
      return SysGGA.PgaServerGetSubwindow();
    }

    IntPtr IGraphics.PgaServerHandleAppActivity(string package, string activity)
    {
      return SysGGA.PgaServerHandleAppActivity(package, activity);
    }

    void IGraphics.PgaServerHandleCommand(int scancode)
    {
      SysGGA.PgaServerHandleCommand(scancode);
    }

    IntPtr IGraphics.PgaServerHandleOrientation(
      float hscale,
      float vscale,
      int orientation)
    {
      return SysGGA.PgaServerHandleOrientation(hscale, vscale, orientation);
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
      return SysGGA.PgaServerInit(h, x, y, width, height, evt, glRenderMode, engine);
    }

    int IGraphics.PgaUtilsIsHotAttach()
    {
      return SysGGA.PgaUtilsIsHotAttach();
    }

    int IGraphics.PgaIsGLES3()
    {
      return SysGGA.PgaIsGLES3();
    }

    void IGraphics.PgaSetFps(int fps)
    {
      SysGGA.PgaSetFps(fps);
    }

    void IGraphics.PgaShowFps(int enable)
    {
      SysGGA.PgaShowFps(enable);
    }

    void IGraphics.ToggleFarmMode(bool enable)
    {
      SysGGA.ToggleFarmMode(enable);
    }

    void IGraphics.ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc)
    {
      SysGGA.ImgdUpdateScreenPoint(ref xPos, ref yPos, ref crc);
    }

    void IGraphics.PgaSetVSyncState(int enable)
    {
      SysGGA.PgaSetVSyncState(enable);
    }
  }
}
