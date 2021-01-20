// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RegSAM
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common
{
  public enum RegSAM
  {
    QueryValue = 1,
    SetValue = 2,
    CreateSubKey = 4,
    EnumerateSubKeys = 8,
    Notify = 16, // 0x00000010
    CreateLink = 32, // 0x00000020
    WOW64_64Key = 256, // 0x00000100
    WOW64_32Key = 512, // 0x00000200
    WOW64_Res = 768, // 0x00000300
    Write = 131078, // 0x00020006
    Execute = 131097, // 0x00020019
    Read = 131097, // 0x00020019
    AllAccess = 983103, // 0x000F003F
  }
}
