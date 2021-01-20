// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.__MIDL___MIDL_itf_VirtualBox_0000_0000_0079
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

namespace BstkTypeLib
{
  public enum __MIDL___MIDL_itf_VirtualBox_0000_0000_0079
  {
    MediumFormatCapabilities_Uuid = 1,
    MediumFormatCapabilities_CreateFixed = 2,
    MediumFormatCapabilities_CreateDynamic = 4,
    MediumFormatCapabilities_CreateSplit2G = 8,
    MediumFormatCapabilities_Differencing = 16, // 0x00000010
    MediumFormatCapabilities_Asynchronous = 32, // 0x00000020
    MediumFormatCapabilities_File = 64, // 0x00000040
    MediumFormatCapabilities_Properties = 128, // 0x00000080
    MediumFormatCapabilities_TcpNetworking = 256, // 0x00000100
    MediumFormatCapabilities_VFS = 512, // 0x00000200
    MediumFormatCapabilities_Discard = 1024, // 0x00000400
    MediumFormatCapabilities_Preferred = 2048, // 0x00000800
    MediumFormatCapabilities_CapabilityMask = 4095, // 0x00000FFF
  }
}
