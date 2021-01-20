// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.StorageBus
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("21371490-8542-4B5A-A74D-EE9AC2D45A90")]
  public enum StorageBus
  {
    StorageBus_Null,
    StorageBus_IDE,
    StorageBus_SATA,
    StorageBus_SCSI,
    StorageBus_Floppy,
    StorageBus_SAS,
    StorageBus_USB,
    StorageBus_PCIe,
  }
}
