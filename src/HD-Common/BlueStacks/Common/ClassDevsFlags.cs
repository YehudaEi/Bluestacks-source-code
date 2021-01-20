// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ClassDevsFlags
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  [Flags]
  public enum ClassDevsFlags
  {
    DIGCF_DEFAULT = 1,
    DIGCF_PRESENT = 2,
    DIGCF_ALLCLASSES = 4,
    DIGCF_PROFILE = 8,
    DIGCF_DEVICEINTERFACE = 16, // 0x00000010
  }
}
