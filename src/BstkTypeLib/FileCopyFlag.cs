// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FileCopyFlag
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("791909D7-4C64-2FA4-4303-ADB10658D347")]
  public enum FileCopyFlag
  {
    FileCopyFlag_None = 0,
    FileCopyFlag_NoReplace = 1,
    FileCopyFlag_FollowLinks = 2,
    FileCopyFlag_Update = 4,
  }
}
