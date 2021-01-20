// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.PortMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("7485FCFD-D603-470A-87AF-26D33BEB7DE9")]
  public enum PortMode
  {
    PortMode_Disconnected,
    PortMode_HostPipe,
    PortMode_HostDevice,
    PortMode_RawFile,
    PortMode_TCP,
  }
}
