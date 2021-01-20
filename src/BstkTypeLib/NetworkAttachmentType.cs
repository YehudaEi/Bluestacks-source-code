// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.NetworkAttachmentType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("524A8F9D-4B86-4B51-877D-1AA27C4EBEAC")]
  public enum NetworkAttachmentType
  {
    NetworkAttachmentType_Null,
    NetworkAttachmentType_NAT,
    NetworkAttachmentType_Bridged,
    NetworkAttachmentType_Internal,
    NetworkAttachmentType_HostOnly,
    NetworkAttachmentType_Generic,
    NetworkAttachmentType_NATNetwork,
  }
}
