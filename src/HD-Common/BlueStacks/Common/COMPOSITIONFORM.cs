// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.COMPOSITIONFORM
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
