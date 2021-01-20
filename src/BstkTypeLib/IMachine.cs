// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IMachine
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("C4598DF7-093A-46DD-B8DF-1C8A04BC6693")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IMachine
  {
    [DispId(1610743808)]
    VirtualBox Parent { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743809)]
    byte[] Icon { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] set; }

    [DispId(1610743811)]
    int Accessible { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    IVirtualBoxErrorInfo AccessError { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743813)]
    string Name { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743815)]
    string Description { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743817)]
    string Id { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743818)]
    string[] Groups { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] set; }

    [DispId(1610743820)]
    string OSTypeId { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743822)]
    string HardwareVersion { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743824)]
    string HardwareUUID { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743826)]
    uint CPUCount { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743828)]
    int CPUHotPlugEnabled { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743830)]
    uint CPUExecutionCap { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743832)]
    uint CPUIDPortabilityLevel { [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743834)]
    uint MemorySize { [DispId(1610743834), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743834), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743836)]
    uint MemoryBalloonSize { [DispId(1610743836), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743836), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743838)]
    int PageFusionEnabled { [DispId(1610743838), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743838), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743840)]
    [ComAliasName("BstkTypeLib.GraphicsControllerType")]
    GraphicsControllerType GraphicsControllerType { [DispId(1610743840), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.GraphicsControllerType")] get; [DispId(1610743840), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.GraphicsControllerType"), In] set; }

    [DispId(1610743842)]
    uint VRAMSize { [DispId(1610743842), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743842), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743844)]
    int Accelerate3DEnabled { [DispId(1610743844), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743844), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743846)]
    int Accelerate2DVideoEnabled { [DispId(1610743846), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743846), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743848)]
    uint MonitorCount { [DispId(1610743848), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743848), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743850)]
    int VideoCaptureEnabled { [DispId(1610743850), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743850), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743852)]
    int[] VideoCaptureScreens { [DispId(1610743852), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; [DispId(1610743852), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] set; }

    [DispId(1610743854)]
    string VideoCaptureFile { [DispId(1610743854), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743854), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743856)]
    uint VideoCaptureWidth { [DispId(1610743856), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743856), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743858)]
    uint VideoCaptureHeight { [DispId(1610743858), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743858), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743860)]
    uint VideoCaptureRate { [DispId(1610743860), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743860), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743862)]
    uint VideoCaptureFPS { [DispId(1610743862), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743862), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743864)]
    uint VideoCaptureMaxTime { [DispId(1610743864), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743864), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743866)]
    uint VideoCaptureMaxFileSize { [DispId(1610743866), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743866), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743868)]
    string VideoCaptureOptions { [DispId(1610743868), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743868), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743870)]
    IBIOSSettings BIOSSettings { [DispId(1610743870), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743871)]
    [ComAliasName("BstkTypeLib.FirmwareType")]
    FirmwareType FirmwareType { [DispId(1610743871), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.FirmwareType")] get; [DispId(1610743871), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.FirmwareType"), In] set; }

    [DispId(1610743873)]
    [ComAliasName("BstkTypeLib.PointingHIDType")]
    PointingHIDType PointingHIDType { [DispId(1610743873), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.PointingHIDType")] get; [DispId(1610743873), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.PointingHIDType"), In] set; }

    [ComAliasName("BstkTypeLib.KeyboardHIDType")]
    [DispId(1610743875)]
    KeyboardHIDType KeyboardHIDType { [DispId(1610743875), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.KeyboardHIDType")] get; [DispId(1610743875), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.KeyboardHIDType"), In] set; }

    [DispId(1610743877)]
    int HPETEnabled { [DispId(1610743877), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743877), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743879)]
    [ComAliasName("BstkTypeLib.ChipsetType")]
    ChipsetType ChipsetType { [DispId(1610743879), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.ChipsetType")] get; [DispId(1610743879), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.ChipsetType"), In] set; }

    [DispId(1610743881)]
    string SnapshotFolder { [DispId(1610743881), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743881), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743883)]
    IVRDEServer VRDEServer { [DispId(1610743883), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743884)]
    int EmulatedUSBCardReaderEnabled { [DispId(1610743884), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743884), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743886)]
    IMediumAttachment[] MediumAttachments { [DispId(1610743886), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743887)]
    IUSBController[] USBControllers { [DispId(1610743887), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743888)]
    IUSBDeviceFilters USBDeviceFilters { [DispId(1610743888), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743889)]
    IAudioAdapter AudioAdapter { [DispId(1610743889), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743890)]
    IStorageController[] StorageControllers { [DispId(1610743890), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743891)]
    string SettingsFilePath { [DispId(1610743891), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743892)]
    string SettingsAuxFilePath { [DispId(1610743892), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743893)]
    int SettingsModified { [DispId(1610743893), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743894)]
    [ComAliasName("BstkTypeLib.SessionState")]
    SessionState SessionState { [DispId(1610743894), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.SessionState")] get; }

    [DispId(1610743895)]
    string SessionName { [DispId(1610743895), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743896)]
    uint SessionPID { [DispId(1610743896), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743897)]
    [ComAliasName("BstkTypeLib.MachineState")]
    MachineState State { [DispId(1610743897), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.MachineState")] get; }

    [DispId(1610743898)]
    long LastStateChange { [DispId(1610743898), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743899)]
    string StateFilePath { [DispId(1610743899), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743900)]
    string LogFolder { [DispId(1610743900), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743901)]
    ISnapshot CurrentSnapshot { [DispId(1610743901), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743902)]
    uint SnapshotCount { [DispId(1610743902), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743903)]
    int CurrentStateModified { [DispId(1610743903), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743904)]
    ISharedFolder[] SharedFolders { [DispId(1610743904), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743905)]
    [ComAliasName("BstkTypeLib.ClipboardMode")]
    ClipboardMode ClipboardMode { [DispId(1610743905), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.ClipboardMode")] get; [DispId(1610743905), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.ClipboardMode"), In] set; }

    [ComAliasName("BstkTypeLib.DnDMode")]
    [DispId(1610743907)]
    DnDMode DnDMode { [DispId(1610743907), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.DnDMode")] get; [DispId(1610743907), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.DnDMode"), In] set; }

    [DispId(1610743909)]
    int TeleporterEnabled { [DispId(1610743909), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743909), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743911)]
    uint TeleporterPort { [DispId(1610743911), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743911), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743913)]
    string TeleporterAddress { [DispId(1610743913), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743913), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743915)]
    string TeleporterPassword { [DispId(1610743915), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743915), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743917)]
    [ComAliasName("BstkTypeLib.ParavirtProvider")]
    ParavirtProvider ParavirtProvider { [DispId(1610743917), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.ParavirtProvider")] get; [DispId(1610743917), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.ParavirtProvider"), In] set; }

    [ComAliasName("BstkTypeLib.FaultToleranceState")]
    [DispId(1610743919)]
    FaultToleranceState FaultToleranceState { [DispId(1610743919), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.FaultToleranceState")] get; [DispId(1610743919), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.FaultToleranceState"), In] set; }

    [DispId(1610743921)]
    uint FaultTolerancePort { [DispId(1610743921), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743921), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743923)]
    string FaultToleranceAddress { [DispId(1610743923), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743923), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743925)]
    string FaultTolerancePassword { [DispId(1610743925), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743925), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743927)]
    uint FaultToleranceSyncInterval { [DispId(1610743927), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743927), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743929)]
    int RTCUseUTC { [DispId(1610743929), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743929), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743931)]
    int IOCacheEnabled { [DispId(1610743931), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743931), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743933)]
    uint IOCacheSize { [DispId(1610743933), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743933), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743935)]
    IPCIDeviceAttachment[] PCIDeviceAssignments { [DispId(1610743935), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743936)]
    IBandwidthControl BandwidthControl { [DispId(1610743936), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743937)]
    int TracingEnabled { [DispId(1610743937), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743937), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743939)]
    string TracingConfig { [DispId(1610743939), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743939), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743941)]
    int AllowTracingToAccessVM { [DispId(1610743941), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743941), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743943)]
    int AutostartEnabled { [DispId(1610743943), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743943), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743945)]
    uint AutostartDelay { [DispId(1610743945), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743945), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [ComAliasName("BstkTypeLib.AutostopType")]
    [DispId(1610743947)]
    AutostopType AutostopType { [DispId(1610743947), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.AutostopType")] get; [DispId(1610743947), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.AutostopType"), In] set; }

    [DispId(1610743949)]
    string DefaultFrontend { [DispId(1610743949), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743949), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743951)]
    int USBProxyAvailable { [DispId(1610743951), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743952)]
    string VMProcessPriority { [DispId(1610743952), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743952), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743954)]
    string ParavirtDebug { [DispId(1610743954), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743954), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743956)]
    string CPUProfile { [DispId(1610743956), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743956), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743958)]
    uint InternalAndReservedAttribute1IMachine { [DispId(1610743958), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743959)]
    uint InternalAndReservedAttribute2IMachine { [DispId(1610743959), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743960)]
    uint InternalAndReservedAttribute3IMachine { [DispId(1610743960), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743961)]
    uint InternalAndReservedAttribute4IMachine { [DispId(1610743961), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743962)]
    uint InternalAndReservedAttribute5IMachine { [DispId(1610743962), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743963)]
    uint InternalAndReservedAttribute6IMachine { [DispId(1610743963), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743964)]
    uint InternalAndReservedAttribute7IMachine { [DispId(1610743964), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743965)]
    uint InternalAndReservedAttribute8IMachine { [DispId(1610743965), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743966)]
    uint InternalAndReservedAttribute9IMachine { [DispId(1610743966), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743967)]
    uint InternalAndReservedAttribute10IMachine { [DispId(1610743967), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743968)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void LockMachine([MarshalAs(UnmanagedType.Interface), In] Session aSession, [ComAliasName("BstkTypeLib.LockType"), In] LockType aLockType);

    [DispId(1610743969)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress LaunchVMProcess([MarshalAs(UnmanagedType.Interface), In] Session aSession, [MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aEnvironment);

    [DispId(1610743970)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetBootOrder([In] uint aPosition, [ComAliasName("BstkTypeLib.DeviceType"), In] DeviceType aDevice);

    [DispId(1610743971)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DeviceType")]
    DeviceType GetBootOrder([In] uint aPosition);

    [DispId(1610743972)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AttachDevice(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [In] int aControllerPort,
      [In] int aDevice,
      [ComAliasName("BstkTypeLib.DeviceType"), In] DeviceType aType,
      [MarshalAs(UnmanagedType.Interface), In] IMedium aMedium);

    [DispId(1610743973)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AttachDeviceWithoutMedium(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [In] int aControllerPort,
      [In] int aDevice,
      [ComAliasName("BstkTypeLib.DeviceType"), In] DeviceType aType);

    [DispId(1610743974)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DetachDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice);

    [DispId(1610743975)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PassthroughDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [In] int aPassthrough);

    [DispId(1610743976)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void TemporaryEjectDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [In] int aTemporaryEject);

    [DispId(1610743977)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NonRotationalDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [In] int aNonRotational);

    [DispId(1610743978)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetAutoDiscardForDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [In] int aDiscard);

    [DispId(1610743979)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetHotPluggableForDevice(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [In] int aControllerPort,
      [In] int aDevice,
      [In] int aHotPluggable);

    [DispId(1610743980)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetBandwidthGroupForDevice(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [In] int aControllerPort,
      [In] int aDevice,
      [MarshalAs(UnmanagedType.Interface), In] IBandwidthGroup aBandwidthGroup);

    [DispId(1610743981)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetNoBandwidthGroupForDevice([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice);

    [DispId(1610743982)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnmountMedium([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [In] int aForce);

    [DispId(1610743983)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void MountMedium([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice, [MarshalAs(UnmanagedType.Interface), In] IMedium aMedium, [In] int aForce);

    [DispId(1610743984)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IMedium GetMedium([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aControllerPort, [In] int aDevice);

    [DispId(1610743985)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)]
    IMediumAttachment[] GetMediumAttachmentsOfController([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743986)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IMediumAttachment GetMediumAttachment(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [In] int aControllerPort,
      [In] int aDevice);

    [DispId(1610743987)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AttachHostPCIDevice([In] int aHostAddress, [In] int aDesiredGuestAddress, [In] int aTryToUnbind);

    [DispId(1610743988)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DetachHostPCIDevice([In] int aHostAddress);

    [DispId(1610743989)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetworkAdapter GetNetworkAdapter([In] uint aSlot);

    [DispId(1610743990)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorageController AddStorageController(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aConnectionType);

    [DispId(1610743991)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorageController GetStorageControllerByName([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743992)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorageController GetStorageControllerByInstance(
      [ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aConnectionType,
      [In] uint aInstance);

    [DispId(1610743993)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveStorageController([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743994)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetStorageControllerBootable([MarshalAs(UnmanagedType.BStr), In] string aName, [In] int aBootable);

    [DispId(1610743995)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBController AddUSBController([MarshalAs(UnmanagedType.BStr), In] string aName, [ComAliasName("BstkTypeLib.USBControllerType"), In] USBControllerType aType);

    [DispId(1610743996)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveUSBController([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743997)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBController GetUSBControllerByName([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743998)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetUSBControllerCountByType([ComAliasName("BstkTypeLib.USBControllerType"), In] USBControllerType aType);

    [DispId(1610743999)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ISerialPort GetSerialPort([In] uint aSlot);

    [DispId(1610744000)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IParallelPort GetParallelPort([In] uint aSlot);

    [DispId(1610744001)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
    string[] GetExtraDataKeys();

    [DispId(1610744002)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetExtraData([MarshalAs(UnmanagedType.BStr), In] string aKey);

    [DispId(1610744003)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetExtraData([MarshalAs(UnmanagedType.BStr), In] string aKey, [MarshalAs(UnmanagedType.BStr), In] string aValue);

    [DispId(1610744004)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetCPUProperty([ComAliasName("BstkTypeLib.CPUPropertyType"), In] CPUPropertyType aProperty);

    [DispId(1610744005)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetCPUProperty([ComAliasName("BstkTypeLib.CPUPropertyType"), In] CPUPropertyType aProperty, [In] int aValue);

    [DispId(1610744006)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCPUIDLeaf(
      [In] uint aId,
      out uint aValEax,
      out uint aValEbx,
      out uint aValEcx,
      out uint aValEdx);

    [DispId(1610744007)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetCPUIDLeaf([In] uint aId, [In] uint aValEax, [In] uint aValEbx, [In] uint aValEcx, [In] uint aValEdx);

    [DispId(1610744008)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveCPUIDLeaf([In] uint aId);

    [DispId(1610744009)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveAllCPUIDLeaves();

    [DispId(1610744010)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHWVirtExProperty([ComAliasName("BstkTypeLib.HWVirtExPropertyType"), In] HWVirtExPropertyType aProperty);

    [DispId(1610744011)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetHWVirtExProperty([ComAliasName("BstkTypeLib.HWVirtExPropertyType"), In] HWVirtExPropertyType aProperty, [In] int aValue);

    [DispId(1610744012)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress SetSettingsFilePath([MarshalAs(UnmanagedType.BStr), In] string aSettingsFilePath);

    [DispId(1610744013)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SaveSettings();

    [DispId(1610744014)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DiscardSettings();

    [DispId(1610744015)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)]
    IMedium[] Unregister([ComAliasName("BstkTypeLib.CleanupMode"), In] CleanupMode aCleanupMode);

    [DispId(1610744016)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DeleteConfig([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH), In] IMedium[] aMedia);

    [DispId(1610744017)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IVirtualSystemDescription ExportTo(
      [MarshalAs(UnmanagedType.Interface), In] IAppliance aAppliance,
      [MarshalAs(UnmanagedType.BStr), In] string aLocation);

    [DispId(1610744018)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ISnapshot FindSnapshot([MarshalAs(UnmanagedType.BStr), In] string aNameOrId);

    [DispId(1610744019)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CreateSharedFolder([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aHostPath, [In] int aWritable, [In] int aAutoMount);

    [DispId(1610744020)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveSharedFolder([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610744021)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int CanShowConsoleWindow();

    [DispId(1610744022)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    long ShowConsoleWindow();

    [DispId(1610744023)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetGuestProperty([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr)] out string aValue, out long aTimeStamp, [MarshalAs(UnmanagedType.BStr)] out string aFlags);

    [DispId(1610744024)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetGuestPropertyValue([MarshalAs(UnmanagedType.BStr), In] string aProperty);

    [DispId(1610744025)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    long GetGuestPropertyTimestamp([MarshalAs(UnmanagedType.BStr), In] string aProperty);

    [DispId(1610744026)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetGuestProperty([MarshalAs(UnmanagedType.BStr), In] string aProperty, [MarshalAs(UnmanagedType.BStr), In] string aValue, [MarshalAs(UnmanagedType.BStr), In] string aFlags);

    [DispId(1610744027)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetGuestPropertyValue([MarshalAs(UnmanagedType.BStr), In] string aProperty, [MarshalAs(UnmanagedType.BStr), In] string aValue);

    [DispId(1610744028)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DeleteGuestProperty([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610744029)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnumerateGuestProperties(
      [MarshalAs(UnmanagedType.BStr), In] string aPatterns,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aNames,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aValues,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I8)] out long[] aTimestamps,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] aFlags);

    [DispId(1610744030)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void QuerySavedGuestScreenInfo(
      [In] uint aScreenId,
      out uint aOriginX,
      out uint aOriginY,
      out uint aWidth,
      out uint aHeight,
      out int aEnabled);

    [DispId(1610744031)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] ReadSavedThumbnailToArray(
      [In] uint aScreenId,
      [ComAliasName("BstkTypeLib.BitmapFormat"), In] BitmapFormat aBitmapFormat,
      out uint aWidth,
      out uint aHeight);

    [DispId(1610744032)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    BitmapFormat[] QuerySavedScreenshotInfo(
      [In] uint aScreenId,
      out uint aWidth,
      out uint aHeight);

    [DispId(1610744033)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] ReadSavedScreenshotToArray(
      [In] uint aScreenId,
      [ComAliasName("BstkTypeLib.BitmapFormat"), In] BitmapFormat aBitmapFormat,
      out uint aWidth,
      out uint aHeight);

    [DispId(1610744034)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void HotPlugCPU([In] uint aCpu);

    [DispId(1610744035)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void HotUnplugCPU([In] uint aCpu);

    [DispId(1610744036)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetCPUStatus([In] uint aCpu);

    [DispId(1610744037)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.ParavirtProvider")]
    ParavirtProvider GetEffectiveParavirtProvider();

    [DispId(1610744038)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string QueryLogFilename([In] uint aIdx);

    [DispId(1610744039)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] ReadLog([In] uint aIdx, [In] long aOffset, [In] long aSize);

    [DispId(1610744040)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress CloneTo([MarshalAs(UnmanagedType.Interface), In] IMachine aTarget, [ComAliasName("BstkTypeLib.CloneMode"), In] CloneMode aMode, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] CloneOptions[] aOptions);

    [DispId(1610744041)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress SaveState();

    [DispId(1610744042)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AdoptSavedState([MarshalAs(UnmanagedType.BStr), In] string aSavedStateFile);

    [DispId(1610744043)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DiscardSavedState([In] int aFRemoveFile);

    [DispId(1610744044)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress TakeSnapshot([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aDescription, [In] int aPause, [MarshalAs(UnmanagedType.BStr)] out string aId);

    [DispId(1610744045)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DeleteSnapshot([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610744046)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DeleteSnapshotAndAllChildren([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610744047)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DeleteSnapshotRange([MarshalAs(UnmanagedType.BStr), In] string aStartId, [MarshalAs(UnmanagedType.BStr), In] string aEndId);

    [DispId(1610744048)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress RestoreSnapshot([MarshalAs(UnmanagedType.Interface), In] ISnapshot aSnapshot);

    [DispId(1610744049)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ApplyDefaults([MarshalAs(UnmanagedType.BStr), In] string aFlags);

    [DispId(1610744050)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IMachine();

    [DispId(1610744051)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IMachine();

    [DispId(1610744052)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3IMachine();

    [DispId(1610744053)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4IMachine();

    [DispId(1610744054)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod5IMachine();

    [DispId(1610744055)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod6IMachine();

    [DispId(1610744056)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod7IMachine();
  }
}
