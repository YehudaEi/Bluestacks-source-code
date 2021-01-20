// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.DnDMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("07AF8800-F936-4B33-9172-CD400E83C148")]
  public enum DnDMode
  {
    DnDMode_Disabled,
    DnDMode_HostToGuest,
    DnDMode_GuestToHost,
    DnDMode_Bidirectional,
  }
}
