// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.WINDOWPLACEMENT
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  [Serializable]
  public struct WINDOWPLACEMENT
  {
    public int length { [IsReadOnly] get; set; }

    public int flags { [IsReadOnly] get; set; }

    public int showCmd { [IsReadOnly] get; set; }

    public POINT minPosition { [IsReadOnly] get; set; }

    public POINT maxPosition { [IsReadOnly] get; set; }

    public RECT normalPosition { [IsReadOnly] get; set; }
  }
}
