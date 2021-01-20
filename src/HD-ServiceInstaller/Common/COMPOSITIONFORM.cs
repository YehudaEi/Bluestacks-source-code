// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.COMPOSITIONFORM
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.Drawing;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  public struct COMPOSITIONFORM
  {
    public int dwStyle { [IsReadOnly] get; set; }

    public Point ptCurrentPos { [IsReadOnly] get; set; }

    public RECT rcArea { [IsReadOnly] get; set; }
  }
}
