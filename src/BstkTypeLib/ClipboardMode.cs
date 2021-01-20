// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ClipboardMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("33364716-4008-4701-8F14-BE0FA3D62950")]
  public enum ClipboardMode
  {
    ClipboardMode_Disabled,
    ClipboardMode_HostToGuest,
    ClipboardMode_GuestToHost,
    ClipboardMode_Bidirectional,
  }
}
