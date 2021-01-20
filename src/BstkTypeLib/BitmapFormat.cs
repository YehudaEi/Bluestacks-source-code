// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.BitmapFormat
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("AFB2BF39-8B1E-4F9F-8948-D1B887F83EB0")]
  public enum BitmapFormat
  {
    BitmapFormat_Opaque = 0,
    BitmapFormat_PNG = 541544016, // 0x20474E50
    BitmapFormat_BGR = 542263106, // 0x20524742
    BitmapFormat_BGR0 = 810698562, // 0x30524742
    BitmapFormat_RGBA = 1094862674, // 0x41424752
    BitmapFormat_BGRA = 1095911234, // 0x41524742
    BitmapFormat_JPEG = 1195724874, // 0x4745504A
  }
}
