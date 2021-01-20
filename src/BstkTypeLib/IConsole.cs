// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IConsole
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("3B4AAF2F-0D88-4FAA-B184-3DEB0A81D83D")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IConsole
  {
    [DispId(1610743808)]
    IMachine Machine { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [ComAliasName("BstkTypeLib.MachineState")]
    [DispId(1610743809)]
    MachineState State { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.MachineState")] get; }

    [DispId(1610743810)]
    IGuest Guest { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743811)]
    IKeyboard Keyboard { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743812)]
    IMouse Mouse { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743813)]
    IDisplay Display { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743814)]
    IMachineDebugger Debugger { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743815)]
    IUSBDevice[] USBDevices { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743816)]
    IHostUSBDevice[] RemoteUSBDevices { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743817)]
    ISharedFolder[] SharedFolders { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743818)]
    IVRDEServerInfo VRDEServerInfo { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743819)]
    IEventSource EventSource { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743820)]
    IPCIDeviceAttachment[] AttachedPCIDevices { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743821)]
    int UseHostClipboard { [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743823)]
    IEmulatedUSB EmulatedUSB { [DispId(1610743823), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743824)]
    uint InternalAndReservedAttribute1IConsole { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743825)]
    uint InternalAndReservedAttribute2IConsole { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743826)]
    uint InternalAndReservedAttribute3IConsole { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743827)]
    uint InternalAndReservedAttribute4IConsole { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743828)]
    uint InternalAndReservedAttribute5IConsole { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743829)]
    uint InternalAndReservedAttribute6IConsole { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743830)]
    uint InternalAndReservedAttribute7IConsole { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743831)]
    uint InternalAndReservedAttribute8IConsole { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743832)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress PowerUp();

    [DispId(1610743833)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress PowerUpPaused();

    [DispId(1610743834)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress PowerDown();

    [DispId(1610743835)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Reset();

    [DispId(1610743836)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Pause();

    [DispId(1610743837)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Resume();

    [DispId(1610743838)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PowerButton();

    [DispId(1610743839)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SleepButton();

    [DispId(1610743840)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetPowerButtonHandled();

    [DispId(1610743841)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetGuestEnteredACPIMode();

    [DispId(1610743842)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    DeviceActivity[] GetDeviceActivity([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DeviceType[] aType);

    [DispId(1610743843)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AttachUSBDevice([MarshalAs(UnmanagedType.BStr), In] string aId, [MarshalAs(UnmanagedType.BStr), In] string aCaptureFilename);

    [DispId(1610743844)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBDevice DetachUSBDevice([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743845)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBDevice FindUSBDeviceByAddress([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743846)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBDevice FindUSBDeviceById([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743847)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CreateSharedFolder([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aHostPath, [In] int aWritable, [In] int aAutoMount);

    [DispId(1610743848)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveSharedFolder([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743849)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress Teleport(
      [MarshalAs(UnmanagedType.BStr), In] string aHostname,
      [In] uint aTcpport,
      [MarshalAs(UnmanagedType.BStr), In] string aPassword,
      [In] uint aMaxDowntime);

    [DispId(1610743850)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddDiskEncryptionPassword([MarshalAs(UnmanagedType.BStr), In] string aId, [MarshalAs(UnmanagedType.BStr), In] string aPassword, [In] int aClearOnSuspend);

    [DispId(1610743851)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddDiskEncryptionPasswords([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aIds, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aPasswords, [In] int aClearOnSuspend);

    [DispId(1610743852)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveDiskEncryptionPassword([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743853)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ClearAllDiskEncryptionPasswords();

    [DispId(1610743854)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IConsole();

    [DispId(1610743855)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IConsole();

    [DispId(1610743856)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3IConsole();

    [DispId(1610743857)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4IConsole();

    [DispId(1610743858)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod5IConsole();

    [DispId(1610743859)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod6IConsole();

    [DispId(1610743860)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod7IConsole();

    [DispId(1610743861)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod8IConsole();
  }
}
