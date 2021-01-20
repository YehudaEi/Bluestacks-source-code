// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FileOpenAction
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("12BC97E2-4FC6-A8B4-4F84-0CBF4AB970D2")]
  public enum FileOpenAction
  {
    FileOpenAction_OpenExisting = 1,
    FileOpenAction_OpenOrCreate = 2,
    FileOpenAction_CreateNew = 3,
    FileOpenAction_CreateOrReplace = 4,
    FileOpenAction_OpenExistingTruncated = 5,
    FileOpenAction_AppendOrCreate = 99, // 0x00000063
  }
}
