// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IInternalMachineControl
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("49978C66-80A7-46E5-9BB6-54B2B27ADC93")]
  [ComImport]
  public interface IInternalMachineControl
  {
    [DispId(1610743808)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UpdateState([ComAliasName("BstkTypeLib.MachineState"), In] MachineState aState);

    [DispId(1610743809)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void BeginPowerUp([MarshalAs(UnmanagedType.Interface), In] IProgress aProgress);

    [DispId(1610743810)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EndPowerUp([In] int aResult);

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void BeginPoweringDown([MarshalAs(UnmanagedType.Interface)] out IProgress aProgress);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EndPoweringDown([In] int aResult, [MarshalAs(UnmanagedType.BStr), In] string aErrMsg);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RunUSBDeviceFilters([MarshalAs(UnmanagedType.Interface), In] IUSBDevice aDevice, out int aMatched, out uint aMaskedInterfaces);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CaptureUSBDevice([MarshalAs(UnmanagedType.BStr), In] string aId, [MarshalAs(UnmanagedType.BStr), In] string aCaptureFilename);

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DetachUSBDevice([MarshalAs(UnmanagedType.BStr), In] string aId, [In] int aDone);

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AutoCaptureUSBDevices();

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DetachAllUSBDevices([In] int aDone);

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress OnSessionEnd([MarshalAs(UnmanagedType.Interface), In] Session aSession);

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void FinishOnlineMergeMedium();

    [DispId(1610743820)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PullGuestProperties(
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aNames,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aValues,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I8)] out long[] aTimestamps,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aFlags);

    [DispId(1610743821)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PushGuestProperty([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aValue, [In] long aTimeStamp, [MarshalAs(UnmanagedType.BStr), In] string aFlags);

    [DispId(1610743822)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void LockMedia();

    [DispId(1610743823)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnlockMedia();

    [DispId(1610743824)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IMediumAttachment EjectMedium([MarshalAs(UnmanagedType.Interface), In] IMediumAttachment aAttachment);

    [DispId(1610743825)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ReportVmStatistics(
      [In] uint aValidStats,
      [In] uint aCpuUser,
      [In] uint aCpuKernel,
      [In] uint aCpuIdle,
      [In] uint aMemTotal,
      [In] uint aMemFree,
      [In] uint aMemBalloon,
      [In] uint aMemShared,
      [In] uint aMemCache,
      [In] uint aPagedTotal,
      [In] uint aMemAllocTotal,
      [In] uint aMemFreeTotal,
      [In] uint aMemBalloonTotal,
      [In] uint aMemSharedTotal,
      [In] uint aVmNetRx,
      [In] uint aVmNetTx);

    [DispId(1610743826)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AuthenticateExternal([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aAuthParams, [MarshalAs(UnmanagedType.BStr)] out string aResult);
  }
}
