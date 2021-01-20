// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IPin
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a86891-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IPin
  {
    int Connect([In] IPin pReceivePin, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int ReceiveConnection([In] IPin pReceivePin, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int Disconnect();

    int ConnectedTo(out IPin ppPin);

    int ConnectionMediaType([MarshalAs(UnmanagedType.LPStruct), Out] AMMediaType pmt);

    int QueryPinInfo(out PinInfo pInfo);

    int QueryDirection(out PinDirection pPinDir);

    int QueryId([MarshalAs(UnmanagedType.LPWStr)] out string Id);

    int QueryAccept([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int EnumMediaTypes(out IEnumMediaTypes ppEnum);

    int QueryInternalConnections([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] IPin[] ppPins, [In, Out] ref int nPin);

    int EndOfStream();

    int BeginFlush();

    int EndFlush();

    int NewSegment([In] long tStart, [In] long tStop, [In] double dRate);
  }
}
