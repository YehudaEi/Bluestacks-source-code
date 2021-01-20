// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IInternalSessionControl
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("9C9937AE-0381-477C-926C-6E8267FE9143")]
  [ComImport]
  public interface IInternalSessionControl
  {
    [DispId(1610743808)]
    uint PID { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    IConsole RemoteConsole { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743810)]
    [ComAliasName("BstkTypeLib.MachineState")]
    MachineState NominalState { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.MachineState")] get; }

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AssignRemoteMachine([MarshalAs(UnmanagedType.Interface), In] IMachine aMachine, [MarshalAs(UnmanagedType.Interface), In] IConsole aConsole);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UpdateMachineState([ComAliasName("BstkTypeLib.MachineState"), In] MachineState aMachineState);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Uninitialize();

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnNetworkAdapterChange([MarshalAs(UnmanagedType.Interface), In] INetworkAdapter aNetworkAdapter, [In] int aChangeAdapter);

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnSerialPortChange([MarshalAs(UnmanagedType.Interface), In] ISerialPort aSerialPort);

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnParallelPortChange([MarshalAs(UnmanagedType.Interface), In] IParallelPort aParallelPort);

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnStorageControllerChange();

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnMediumChange([MarshalAs(UnmanagedType.Interface), In] IMediumAttachment aMediumAttachment, [In] int aForce);

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnStorageDeviceChange([MarshalAs(UnmanagedType.Interface), In] IMediumAttachment aMediumAttachment, [In] int aRemove, [In] int aSilent);

    [DispId(1610743820)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnClipboardModeChange([ComAliasName("BstkTypeLib.ClipboardMode"), In] ClipboardMode aClipboardMode);

    [DispId(1610743821)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnDnDModeChange([ComAliasName("BstkTypeLib.DnDMode"), In] DnDMode aDnDMode);

    [DispId(1610743822)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnCPUChange([In] uint aCpu, [In] int aAdd);

    [DispId(1610743823)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnCPUExecutionCapChange([In] uint aExecutionCap);

    [DispId(1610743824)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnVRDEServerChange([In] int aRestart);

    [DispId(1610743825)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnVideoCaptureChange();

    [DispId(1610743826)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnUSBControllerChange();

    [DispId(1610743827)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnSharedFolderChange([In] int aGlobal);

    [DispId(1610743828)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnUSBDeviceAttach(
      [MarshalAs(UnmanagedType.Interface), In] IUSBDevice aDevice,
      [MarshalAs(UnmanagedType.Interface), In] IVirtualBoxErrorInfo aError,
      [In] uint aMaskedInterfaces,
      [MarshalAs(UnmanagedType.BStr), In] string aCaptureFilename);

    [DispId(1610743829)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnUSBDeviceDetach([MarshalAs(UnmanagedType.BStr), In] string aId, [MarshalAs(UnmanagedType.Interface), In] IVirtualBoxErrorInfo aError);

    [DispId(1610743830)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnShowWindow([In] int aCheck, out int aCanShow, out long aWinId);

    [DispId(1610743831)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnBandwidthGroupChange([MarshalAs(UnmanagedType.Interface), In] IBandwidthGroup aBandwidthGroup);

    [DispId(1610743832)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AccessGuestProperty(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [MarshalAs(UnmanagedType.BStr), In] string aValue,
      [MarshalAs(UnmanagedType.BStr), In] string aFlags,
      [In] uint aAccessMode,
      [MarshalAs(UnmanagedType.BStr)] out string aRetValue,
      out long aRetTimestamp,
      [MarshalAs(UnmanagedType.BStr)] out string aRetFlags);

    [DispId(1610743833)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnumerateGuestProperties(
      [MarshalAs(UnmanagedType.BStr), In] string aPatterns,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aKeys,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aValues,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I8)] out long[] aTimestamps,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aFlags);

    [DispId(1610743834)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnlineMergeMedium(
      [MarshalAs(UnmanagedType.Interface), In] IMediumAttachment aMediumAttachment,
      [In] uint aSourceIdx,
      [In] uint aTargetIdx,
      [MarshalAs(UnmanagedType.Interface), In] IProgress aProgress);

    [DispId(1610743835)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ReconfigureMediumAttachments([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH), In] IMediumAttachment[] aAttachments);

    [DispId(1610743836)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnableVMMStatistics([In] int aEnable);

    [DispId(1610743837)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PauseWithReason([ComAliasName("BstkTypeLib.Reason"), In] Reason aReason);

    [DispId(1610743838)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ResumeWithReason([ComAliasName("BstkTypeLib.Reason"), In] Reason aReason);

    [DispId(1610743839)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int SaveStateWithReason(
      [ComAliasName("BstkTypeLib.Reason"), In] Reason aReason,
      [MarshalAs(UnmanagedType.Interface), In] IProgress aProgress,
      [MarshalAs(UnmanagedType.BStr), In] string aStateFilePath,
      [In] int aPauseVM);

    [DispId(1610743840)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CancelSaveStateWithReason();

    [DispId(1610743841)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AssignMachine([MarshalAs(UnmanagedType.Interface), In] IMachine aMachine, [ComAliasName("BstkTypeLib.LockType"), In] LockType aLockType, [MarshalAs(UnmanagedType.BStr), In] string aTokenId);
  }
}
