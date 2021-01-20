// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IGraphics
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.Text;

namespace BlueStacks.Player
{
  public interface IGraphics
  {
    void PgaLoggerInit(Logger.HdLoggerCallback cb);

    int PgaUtilsIsHotAttach();

    int PgaServerInit(
      IntPtr h,
      int x,
      int y,
      int width,
      int height,
      SafeWaitHandle evt,
      int glRenderMode,
      string engine);

    void PgaServerHandleCommand(int scancode);

    IntPtr PgaServerGetSubwindow();

    IntPtr PgaServerHandleOrientation(float hscale, float vscale, int orientation);

    IntPtr PgaServerHandleAppActivity(string package, string activity);

    int GetPgaServerInitStatus(
      StringBuilder glVendor,
      StringBuilder glRenderer,
      StringBuilder glVersion);

    int PgaIsGLES3();

    void PgaSetFps(int fps);

    void PgaShowFps(int enable);

    void ToggleFarmMode(bool enable);

    void PgaSetVSyncState(int enable);

    void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc);

    void PgaSetAstcConfig(int config);

    int PgaIsHwAstcSupported();
  }
}
