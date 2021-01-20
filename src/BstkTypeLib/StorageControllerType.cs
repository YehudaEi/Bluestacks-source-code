// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.StorageControllerType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("9427F309-82E7-468F-9964-ABFEFC4D3058")]
  public enum StorageControllerType
  {
    StorageControllerType_Null,
    StorageControllerType_LsiLogic,
    StorageControllerType_BusLogic,
    StorageControllerType_IntelAhci,
    StorageControllerType_PIIX3,
    StorageControllerType_PIIX4,
    StorageControllerType_ICH6,
    StorageControllerType_I82078,
    StorageControllerType_LsiLogicSas,
    StorageControllerType_USB,
    StorageControllerType_NVMe,
  }
}
