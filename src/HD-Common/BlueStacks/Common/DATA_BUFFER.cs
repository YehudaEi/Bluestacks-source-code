﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DATA_BUFFER
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public struct DATA_BUFFER
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string Buffer;
  }
}
