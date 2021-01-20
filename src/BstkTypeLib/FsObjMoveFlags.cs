// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FsObjMoveFlags
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("98FDD11F-4063-AC60-5737-E49092AAB95F")]
  public enum FsObjMoveFlags
  {
    FsObjMoveFlags_None = 0,
    FsObjMoveFlags_Replace = 1,
    FsObjMoveFlags_FollowLinks = 2,
    FsObjMoveFlags_AllowDirectoryMoves = 4,
  }
}
