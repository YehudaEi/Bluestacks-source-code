// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IGuestSession
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("6C9C616A-3654-4E9B-B428-22228F806B17")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IGuestSession
  {
    [DispId(1610743808)]
    string User { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743809)]
    string Domain { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743810)]
    string Name { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743811)]
    uint Id { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    uint Timeout { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743814)]
    uint ProtocolVersion { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743815)]
    [ComAliasName("BstkTypeLib.GuestSessionStatus")]
    GuestSessionStatus Status { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.GuestSessionStatus")] get; }

    [DispId(1610743816)]
    string[] EnvironmentChanges { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] set; }

    [DispId(1610743818)]
    string[] EnvironmentBase { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743819)]
    IGuestProcess[] Processes { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743820)]
    [ComAliasName("BstkTypeLib.PathStyle")]
    PathStyle PathStyle { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.PathStyle")] get; }

    [DispId(1610743821)]
    string CurrentDirectory { [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743823)]
    IGuestDirectory[] Directories { [DispId(1610743823), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743824)]
    IGuestFile[] Files { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743825)]
    IEventSource EventSource { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743826)]
    uint InternalAndReservedAttribute1IGuestSession { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743827)]
    uint InternalAndReservedAttribute2IGuestSession { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743828)]
    uint InternalAndReservedAttribute3IGuestSession { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743829)]
    uint InternalAndReservedAttribute4IGuestSession { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743830)]
    uint InternalAndReservedAttribute5IGuestSession { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743831)]
    uint InternalAndReservedAttribute6IGuestSession { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743832)]
    uint InternalAndReservedAttribute7IGuestSession { [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743833)]
    uint InternalAndReservedAttribute8IGuestSession { [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743834)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Close();

    [DispId(1610743835)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DirectoryCopy(
      [MarshalAs(UnmanagedType.BStr), In] string aSource,
      [MarshalAs(UnmanagedType.BStr), In] string aDestination,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryCopyFlags[] aFlags);

    [DispId(1610743836)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DirectoryCopyFromGuest(
      [MarshalAs(UnmanagedType.BStr), In] string aSource,
      [MarshalAs(UnmanagedType.BStr), In] string aDestination,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryCopyFlags[] aFlags);

    [DispId(1610743837)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DirectoryCopyToGuest(
      [MarshalAs(UnmanagedType.BStr), In] string aSource,
      [MarshalAs(UnmanagedType.BStr), In] string aDestination,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryCopyFlags[] aFlags);

    [DispId(1610743838)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DirectoryCreate([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] uint aMode, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryCreateFlag[] aFlags);

    [DispId(1610743839)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string DirectoryCreateTemp([MarshalAs(UnmanagedType.BStr), In] string aTemplateName, [In] uint aMode, [MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aSecure);

    [DispId(1610743840)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int DirectoryExists([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks);

    [DispId(1610743841)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestDirectory DirectoryOpen(
      [MarshalAs(UnmanagedType.BStr), In] string aPath,
      [MarshalAs(UnmanagedType.BStr), In] string aFilter,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryOpenFlag[] aFlags);

    [DispId(1610743842)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DirectoryRemove([MarshalAs(UnmanagedType.BStr), In] string aPath);

    [DispId(1610743843)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress DirectoryRemoveRecursive([MarshalAs(UnmanagedType.BStr), In] string aPath, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DirectoryRemoveRecFlag[] aFlags);

    [DispId(1610743844)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnvironmentScheduleSet([MarshalAs(UnmanagedType.BStr), In] string aName, [MarshalAs(UnmanagedType.BStr), In] string aValue);

    [DispId(1610743845)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnvironmentScheduleUnset([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743846)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string EnvironmentGetBaseVariable([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743847)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int EnvironmentDoesBaseVariableExist([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743848)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress FileCopy([MarshalAs(UnmanagedType.BStr), In] string aSource, [MarshalAs(UnmanagedType.BStr), In] string aDestination, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FileCopyFlag[] aFlags);

    [DispId(1610743849)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress FileCopyFromGuest(
      [MarshalAs(UnmanagedType.BStr), In] string aSource,
      [MarshalAs(UnmanagedType.BStr), In] string aDestination,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FileCopyFlag[] aFlags);

    [DispId(1610743850)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress FileCopyToGuest([MarshalAs(UnmanagedType.BStr), In] string aSource, [MarshalAs(UnmanagedType.BStr), In] string aDestination, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FileCopyFlag[] aFlags);

    [DispId(1610743851)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestFile FileCreateTemp(
      [MarshalAs(UnmanagedType.BStr), In] string aTemplateName,
      [In] uint aMode,
      [MarshalAs(UnmanagedType.BStr), In] string aPath,
      [In] int aSecure);

    [DispId(1610743852)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int FileExists([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks);

    [DispId(1610743853)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestFile FileOpen(
      [MarshalAs(UnmanagedType.BStr), In] string aPath,
      [ComAliasName("BstkTypeLib.FileAccessMode"), In] FileAccessMode aAccessMode,
      [ComAliasName("BstkTypeLib.FileOpenAction"), In] FileOpenAction aOpenAction,
      [In] uint aCreationMode);

    [DispId(1610743854)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestFile FileOpenEx(
      [MarshalAs(UnmanagedType.BStr), In] string aPath,
      [ComAliasName("BstkTypeLib.FileAccessMode"), In] FileAccessMode aAccessMode,
      [ComAliasName("BstkTypeLib.FileOpenAction"), In] FileOpenAction aOpenAction,
      [ComAliasName("BstkTypeLib.FileSharingMode"), In] FileSharingMode aSharingMode,
      [In] uint aCreationMode,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FileOpenExFlags[] aFlags);

    [DispId(1610743855)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    long FileQuerySize([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks);

    [DispId(1610743856)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int FsObjExists([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks);

    [DispId(1610743857)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestFsObjInfo FsObjQueryInfo([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks);

    [DispId(1610743858)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void FsObjRemove([MarshalAs(UnmanagedType.BStr), In] string aPath);

    [DispId(1610743859)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void FsObjRename([MarshalAs(UnmanagedType.BStr), In] string aOldPath, [MarshalAs(UnmanagedType.BStr), In] string aNewPath, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FsObjRenameFlag[] aFlags);

    [DispId(1610743860)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress FsObjMove([MarshalAs(UnmanagedType.BStr), In] string aSource, [MarshalAs(UnmanagedType.BStr), In] string aDestination, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] FsObjMoveFlags[] aFlags);

    [DispId(1610743861)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void FsObjSetACL([MarshalAs(UnmanagedType.BStr), In] string aPath, [In] int aFollowSymlinks, [MarshalAs(UnmanagedType.BStr), In] string aAcl, [In] uint aMode);

    [DispId(1610743862)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestProcess ProcessCreate(
      [MarshalAs(UnmanagedType.BStr), In] string aExecutable,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aArguments,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aEnvironmentChanges,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] ProcessCreateFlag[] aFlags,
      [In] uint aTimeoutMS);

    [DispId(1610743863)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestProcess ProcessCreateEx(
      [MarshalAs(UnmanagedType.BStr), In] string aExecutable,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aArguments,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aEnvironmentChanges,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] ProcessCreateFlag[] aFlags,
      [In] uint aTimeoutMS,
      [ComAliasName("BstkTypeLib.ProcessPriority"), In] ProcessPriority aPriority,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] int[] aAffinity);

    [DispId(1610743864)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IGuestProcess ProcessGet([In] uint aPID);

    [DispId(1610743865)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SymlinkCreate([MarshalAs(UnmanagedType.BStr), In] string aSymlink, [MarshalAs(UnmanagedType.BStr), In] string aTarget, [ComAliasName("BstkTypeLib.SymlinkType"), In] SymlinkType aType);

    [DispId(1610743866)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int SymlinkExists([MarshalAs(UnmanagedType.BStr), In] string aSymlink);

    [DispId(1610743867)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string SymlinkRead([MarshalAs(UnmanagedType.BStr), In] string aSymlink, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] SymlinkReadFlag[] aFlags);

    [DispId(1610743868)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.GuestSessionWaitResult")]
    GuestSessionWaitResult WaitFor([In] uint aWaitFor, [In] uint aTimeoutMS);

    [DispId(1610743869)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.GuestSessionWaitResult")]
    GuestSessionWaitResult WaitForArray(
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] GuestSessionWaitForFlag[] aWaitFor,
      [In] uint aTimeoutMS);

    [DispId(1610743870)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IGuestSession();

    [DispId(1610743871)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IGuestSession();

    [DispId(1610743872)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3IGuestSession();

    [DispId(1610743873)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4IGuestSession();

    [DispId(1610743874)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod5IGuestSession();

    [DispId(1610743875)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod6IGuestSession();

    [DispId(1610743876)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod7IGuestSession();

    [DispId(1610743877)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod8IGuestSession();
  }
}
