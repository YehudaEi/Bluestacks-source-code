// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FileSharingMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("F87DFE58-425B-C5BA-7D6D-22ADEEA25DE1")]
  public enum FileSharingMode
  {
    FileSharingMode_Read = 1,
    FileSharingMode_Write = 2,
    FileSharingMode_ReadWrite = 3,
    FileSharingMode_Delete = 4,
    FileSharingMode_ReadDelete = 5,
    FileSharingMode_WriteDelete = 6,
    FileSharingMode_All = 7,
  }
}
