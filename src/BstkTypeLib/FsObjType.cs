// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FsObjType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("34A0D1AA-491E-E209-E150-84964D6CEE5F")]
  public enum FsObjType
  {
    FsObjType_Unknown = 1,
    FsObjType_Fifo = 2,
    FsObjType_DevChar = 3,
    FsObjType_Directory = 4,
    FsObjType_DevBlock = 5,
    FsObjType_File = 6,
    FsObjType_Symlink = 7,
    FsObjType_Socket = 8,
    FsObjType_WhiteOut = 9,
  }
}
