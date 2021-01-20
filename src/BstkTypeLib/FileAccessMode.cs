// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FileAccessMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("231A578F-47FB-EA30-3B3E-8489558227F0")]
  public enum FileAccessMode
  {
    FileAccessMode_ReadOnly = 1,
    FileAccessMode_WriteOnly = 2,
    FileAccessMode_ReadWrite = 3,
    FileAccessMode_AppendOnly = 4,
    FileAccessMode_AppendRead = 5,
  }
}
