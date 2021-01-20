// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IAMCopyCaptureFileProgress
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("670d1d20-a068-11d0-b3f0-00aa003761c5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAMCopyCaptureFileProgress
  {
    int Progress(int iProgress);
  }
}
