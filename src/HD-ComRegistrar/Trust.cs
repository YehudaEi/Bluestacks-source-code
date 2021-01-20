// Decompiled with JetBrains decompiler
// Type: BlueStacks.ComRegistrar.Trust
// Assembly: HD-ComRegistrar, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: E05F62B1-3170-42C6-BFA0-DC982106896F
// Assembly location: C:\Program Files\BlueStacks\HD-ComRegistrar.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.ComRegistrar
{
  internal class Trust
  {
    public const string CreateToken = "SeCreateTokenPrivilege";
    public const string AssignPrimaryToken = "SeAssignPrimaryTokenPrivilege";
    public const string LockMemory = "SeLockMemoryPrivilege";
    public const string IncreaseQuota = "SeIncreaseQuotaPrivilege";
    public const string UnsolicitedInput = "SeUnsolicitedInputPrivilege";
    public const string MachineAccount = "SeMachineAccountPrivilege";
    public const string TrustedComputingBase = "SeTcbPrivilege";
    public const string Security = "SeSecurityPrivilege";
    public const string TakeOwnership = "SeTakeOwnershipPrivilege";
    public const string LoadDriver = "SeLoadDriverPrivilege";
    public const string SystemProfile = "SeSystemProfilePrivilege";
    public const string SystemTime = "SeSystemtimePrivilege";
    public const string ProfileSingleProcess = "SeProfileSingleProcessPrivilege";
    public const string IncreaseBasePriority = "SeIncreaseBasePriorityPrivilege";
    public const string CreatePageFile = "SeCreatePagefilePrivilege";
    public const string CreatePermanent = "SeCreatePermanentPrivilege";
    public const string Backup = "SeBackupPrivilege";
    public const string Restore = "SeRestorePrivilege";
    public const string Shutdown = "SeShutdownPrivilege";
    public const string Debug = "SeDebugPrivilege";
    public const string Audit = "SeAuditPrivilege";
    public const string SystemEnvironment = "SeSystemEnvironmentPrivilege";
    public const string ChangeNotify = "SeChangeNotifyPrivilege";
    public const string RemoteShutdown = "SeRemoteShutdownPrivilege";
    public const string Undock = "SeUndockPrivilege";
    public const string SyncAgent = "SeSyncAgentPrivilege";
    public const string EnableDelegation = "SeEnableDelegationPrivilege";
    public const string ManageVolume = "SeManageVolumePrivilege";
    public const string Impersonate = "SeImpersonatePrivilege";
    public const string CreateGlobal = "SeCreateGlobalPrivilege";
    public const string TrustedCredentialManagerAccess = "SeTrustedCredManAccessPrivilege";
    public const string ReserveProcessor = "SeReserveProcessorPrivilege";

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AdjustTokenPrivileges(
      [In] IntPtr accessTokenHandle,
      [MarshalAs(UnmanagedType.Bool), In] bool disableAllPrivileges,
      [In] ref Trust.TOKEN_PRIVILEGES newState,
      [In] int bufferLength,
      [In, Out] ref Trust.TOKEN_PRIVILEGES previousState,
      [In, Out] ref int returnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseHandle([In] IntPtr handle);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenProcessToken(
      IntPtr ProcessHandle,
      uint DesiredAccess,
      out IntPtr TokenHandle);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool LookupPrivilegeName(
      [In] string systemName,
      [In] ref Trust.LUID luid,
      [In, Out] StringBuilder name,
      [In, Out] ref int nameLength);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool LookupPrivilegeValue(
      [In] string systemName,
      [In] string name,
      [In, Out] ref Trust.LUID luid);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool OpenProcessToken(
      [In] IntPtr processHandle,
      [In] Trust.TokenAccessRights desiredAccess,
      [In, Out] ref IntPtr tokenHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern int GetLastError();

    public static bool MySetPrivilege(string sPrivilege, bool enablePrivilege)
    {
      Trust.TOKEN_PRIVILEGES newState = new Trust.TOKEN_PRIVILEGES();
      Trust.TOKEN_PRIVILEGES previousState = new Trust.TOKEN_PRIVILEGES();
      Trust.LUID luid = new Trust.LUID();
      int returnLength = 0;
      IntPtr zero = IntPtr.Zero;
      if (!Trust.OpenProcessToken(Trust.GetCurrentProcess(), Trust.TokenAccessRights.AllAccess, ref zero) || !Trust.LookupPrivilegeValue((string) null, sPrivilege, ref luid))
        return false;
      newState.PrivilegeCount = 1;
      newState.Privileges = new Trust.LUID_AND_ATTRIBUTES[64];
      newState.Privileges[0].Luid = luid;
      newState.Privileges[0].Attributes = !enablePrivilege ? 0 : 2;
      previousState.PrivilegeCount = 64;
      previousState.Privileges = new Trust.LUID_AND_ATTRIBUTES[64];
      if (Trust.AdjustTokenPrivileges(zero, false, ref newState, 16, ref previousState, ref returnLength))
        return true;
      Trust.GetLastError();
      return false;
    }

    public struct LUID
    {
      public int lowPart;
      public int highPart;
    }

    public struct LUID_AND_ATTRIBUTES
    {
      public Trust.LUID Luid;
      public int Attributes;
    }

    public struct TOKEN_PRIVILEGES
    {
      public int PrivilegeCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
      public Trust.LUID_AND_ATTRIBUTES[] Privileges;
    }

    [Flags]
    public enum PrivilegeAttributes
    {
      Disabled = 0,
      EnabledByDefault = 1,
      Enabled = 2,
      Removed = 4,
      UsedForAccess = -2147483648, // 0x80000000
    }

    [Flags]
    public enum TokenAccessRights
    {
      AssignPrimary = 0,
      Duplicate = 1,
      Impersonate = 4,
      Query = 8,
      QuerySource = 16, // 0x00000010
      AdjustPrivileges = 32, // 0x00000020
      AdjustGroups = 64, // 0x00000040
      AdjustDefault = 128, // 0x00000080
      AdjustSessionId = 256, // 0x00000100
      AllAccess = 983549, // 0x000F01FD
      Read = 131080, // 0x00020008
      Write = 131296, // 0x000200E0
      Execute = 131076, // 0x00020004
    }

    [Flags]
    internal enum AccessTypeMasks
    {
      Delete = 65536, // 0x00010000
      ReadControl = 131072, // 0x00020000
      WriteDAC = 262144, // 0x00040000
      WriteOwner = 524288, // 0x00080000
      Synchronize = 1048576, // 0x00100000
      StandardRightsRequired = WriteOwner | WriteDAC | ReadControl | Delete, // 0x000F0000
      StandardRightsRead = ReadControl, // 0x00020000
      StandardRightsWrite = StandardRightsRead, // 0x00020000
      StandardRightsExecute = StandardRightsWrite, // 0x00020000
      StandardRightsAll = StandardRightsExecute | Synchronize | WriteOwner | WriteDAC | Delete, // 0x001F0000
      SpecificRightsAll = 65535, // 0x0000FFFF
    }
  }
}
