// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IMachineDebugger
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("4458EFA8-87C7-476D-A828-5329FF859DE8")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IMachineDebugger
  {
    [DispId(1610743808)]
    int SingleStep { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743810)]
    int RecompileUser { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743812)]
    int RecompileSupervisor { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743814)]
    int ExecuteAllInIEM { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743816)]
    int PATMEnabled { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743818)]
    int CSAMEnabled { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743820)]
    int LogEnabled { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743822)]
    string LogDbgFlags { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743823)]
    string LogDbgGroups { [DispId(1610743823), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743824)]
    string LogDbgDestinations { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743825)]
    string LogRelFlags { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743826)]
    string LogRelGroups { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743827)]
    string LogRelDestinations { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743828)]
    int HWVirtExEnabled { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743829)]
    int HWVirtExNestedPagingEnabled { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743830)]
    int HWVirtExVPIDEnabled { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743831)]
    int HWVirtExUXEnabled { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743832)]
    string OSName { [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743833)]
    string OSVersion { [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743834)]
    int PAEEnabled { [DispId(1610743834), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743835)]
    uint VirtualTimeRate { [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743837)]
    long VM { [DispId(1610743837), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743838)]
    long Uptime { [DispId(1610743838), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743839)]
    uint InternalAndReservedAttribute1IMachineDebugger { [DispId(1610743839), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743840)]
    uint InternalAndReservedAttribute2IMachineDebugger { [DispId(1610743840), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743841)]
    uint InternalAndReservedAttribute3IMachineDebugger { [DispId(1610743841), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743842)]
    uint InternalAndReservedAttribute4IMachineDebugger { [DispId(1610743842), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743843)]
    uint InternalAndReservedAttribute5IMachineDebugger { [DispId(1610743843), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743844)]
    uint InternalAndReservedAttribute6IMachineDebugger { [DispId(1610743844), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743845)]
    uint InternalAndReservedAttribute7IMachineDebugger { [DispId(1610743845), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743846)]
    uint InternalAndReservedAttribute8IMachineDebugger { [DispId(1610743846), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743847)]
    uint InternalAndReservedAttribute9IMachineDebugger { [DispId(1610743847), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743848)]
    uint InternalAndReservedAttribute10IMachineDebugger { [DispId(1610743848), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743849)]
    uint InternalAndReservedAttribute11IMachineDebugger { [DispId(1610743849), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743850)]
    uint InternalAndReservedAttribute12IMachineDebugger { [DispId(1610743850), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743851)]
    uint InternalAndReservedAttribute13IMachineDebugger { [DispId(1610743851), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743852)]
    uint InternalAndReservedAttribute14IMachineDebugger { [DispId(1610743852), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743853)]
    uint InternalAndReservedAttribute15IMachineDebugger { [DispId(1610743853), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743854)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DumpGuestCore([MarshalAs(UnmanagedType.BStr), In] string aFileName, [MarshalAs(UnmanagedType.BStr), In] string aCompression);

    [DispId(1610743855)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DumpHostProcessCore([MarshalAs(UnmanagedType.BStr), In] string aFileName, [MarshalAs(UnmanagedType.BStr), In] string aCompression);

    [DispId(1610743856)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string Info([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aArgs);

    [DispId(1610743857)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InjectNMI();

    [DispId(1610743858)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ModifyLogGroups([MarshalAs(UnmanagedType.BStr), In] string aSettings);

    [DispId(1610743859)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ModifyLogFlags([MarshalAs(UnmanagedType.BStr), In] string aSettings);

    [DispId(1610743860)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ModifyLogDestinations([MarshalAs(UnmanagedType.BStr), In] string aSettings);

    [DispId(1610743861)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] ReadPhysicalMemory([In] long aAddress, [In] uint aSize);

    [DispId(1610743862)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void WritePhysicalMemory([In] long aAddress, [In] uint aSize, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aBytes);

    [DispId(1610743863)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] ReadVirtualMemory([In] uint aCpuId, [In] long aAddress, [In] uint aSize);

    [DispId(1610743864)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void WriteVirtualMemory([In] uint aCpuId, [In] long aAddress, [In] uint aSize, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aBytes);

    [DispId(1610743865)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string LoadPlugIn([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743866)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnloadPlugIn([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743867)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string DetectOS();

    [DispId(1610743868)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string QueryOSKernelLog([In] uint aMaxMessages);

    [DispId(1610743869)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetRegister([In] uint aCpuId, [MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743870)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetRegisters([In] uint aCpuId, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aNames, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aValues);

    [DispId(1610743871)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetRegister([In] uint aCpuId, [MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aValue);

    [DispId(1610743872)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetRegisters([In] uint aCpuId, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aNames, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aValues);

    [DispId(1610743873)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string DumpGuestStack([In] uint aCpuId);

    [DispId(1610743874)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ResetStats([MarshalAs(UnmanagedType.BStr), In] string aPattern);

    [DispId(1610743875)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DumpStats([MarshalAs(UnmanagedType.BStr), In] string aPattern);

    [DispId(1610743876)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetStats([MarshalAs(UnmanagedType.BStr), In] string aPattern, [In] int aWithDescriptions);

    [DispId(1610743877)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IMachineDebugger();

    [DispId(1610743878)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IMachineDebugger();

    [DispId(1610743879)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3IMachineDebugger();

    [DispId(1610743880)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4IMachineDebugger();

    [DispId(1610743881)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod5IMachineDebugger();

    [DispId(1610743882)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod6IMachineDebugger();

    [DispId(1610743883)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod7IMachineDebugger();

    [DispId(1610743884)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod8IMachineDebugger();

    [DispId(1610743885)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod9IMachineDebugger();

    [DispId(1610743886)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod10IMachineDebugger();

    [DispId(1610743887)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod11IMachineDebugger();

    [DispId(1610743888)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod12IMachineDebugger();

    [DispId(1610743889)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod13IMachineDebugger();

    [DispId(1610743890)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod14IMachineDebugger();

    [DispId(1610743891)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod15IMachineDebugger();

    [DispId(1610743892)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod16IMachineDebugger();
  }
}
