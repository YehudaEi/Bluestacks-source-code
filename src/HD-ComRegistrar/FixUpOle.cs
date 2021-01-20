// Decompiled with JetBrains decompiler
// Type: BlueStacks.ComRegistrar.FixUpOle
// Assembly: HD-ComRegistrar, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: E05F62B1-3170-42C6-BFA0-DC982106896F
// Assembly location: C:\Program Files\BlueStacks\HD-ComRegistrar.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace BlueStacks.ComRegistrar
{
  public class FixUpOle
  {
    private static uint MAX_BUFF = 64;
    private static string[] oleKeyPathsClassesRoot = new string[6]
    {
      "CLSID\\{00020420-0000-0000-C000-000000000046}\\InProcServer32",
      "CLSID\\{00020421-0000-0000-C000-000000000046}\\InProcServer32",
      "CLSID\\{00020422-0000-0000-C000-000000000046}\\InProcServer32",
      "CLSID\\{00020423-0000-0000-C000-000000000046}\\InProcServer32",
      "CLSID\\{00020424-0000-0000-C000-000000000046}\\InProcServer32",
      "CLSID\\{00020425-0000-0000-C000-000000000046}\\InProcServer32"
    };
    private static string[] oleKeyPathsCurrentUser = new string[6]
    {
      "Software\\Classes\\CLSID\\{00020420-0000-0000-C000-000000000046}\\InProcServer32",
      "Software\\Classes\\CLSID\\{00020421-0000-0000-C000-000000000046}\\InProcServer32",
      "Software\\Classes\\CLSID\\{00020422-0000-0000-C000-000000000046}\\InProcServer32",
      "Software\\Classes\\CLSID\\{00020423-0000-0000-C000-000000000046}\\InProcServer32",
      "Software\\Classes\\CLSID\\{00020424-0000-0000-C000-000000000046}\\InProcServer32",
      "Software\\Classes\\CLSID\\{00020425-0000-0000-C000-000000000046}\\InProcServer32"
    };
    private static string[] oleKeyPathsLocalMachine = new string[6]
    {
      "SOFTWARE\\Classes\\CLSID\\{00020420-0000-0000-C000-000000000046}\\InprocServer32",
      "SOFTWARE\\Classes\\CLSID\\{00020421-0000-0000-C000-000000000046}\\InprocServer32",
      "SOFTWARE\\Classes\\CLSID\\{00020422-0000-0000-C000-000000000046}\\InprocServer32",
      "SOFTWARE\\Classes\\CLSID\\{00020423-0000-0000-C000-000000000046}\\InprocServer32",
      "SOFTWARE\\Classes\\CLSID\\{00020424-0000-0000-C000-000000000046}\\InprocServer32",
      "SOFTWARE\\Classes\\CLSID\\{00020425-0000-0000-C000-000000000046}\\InprocServer32"
    };
    private static string olePath;

    [DllImport("kernel32.dll")]
    private static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWow64Process([In] IntPtr hProcess, out bool wow64Process);

    public static bool IsOs64Bit()
    {
      switch (IntPtr.Size)
      {
        case 4:
          if (!FixUpOle.InternalCheckIsWow64())
            break;
          goto case 8;
        case 8:
          return true;
      }
      return false;
    }

    private static bool InternalCheckIsWow64()
    {
      if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) && Environment.OSVersion.Version.Major < 6)
        return false;
      using (Process currentProcess = Process.GetCurrentProcess())
      {
        bool wow64Process;
        return FixUpOle.IsWow64Process(currentProcess.Handle, out wow64Process) && wow64Process;
      }
    }

    public static bool FixOle(string arg)
    {
      bool blAdd = true;
      if (arg != null && arg.Contains("s"))
        blAdd = false;
      StringBuilder lpBuffer = new StringBuilder();
      int systemDirectory = (int) FixUpOle.GetSystemDirectory(lpBuffer, FixUpOle.MAX_BUFF);
      FixUpOle.olePath = Path.Combine(lpBuffer.ToString(), "oleaut32.dll");
      foreach (string str1 in FixUpOle.oleKeyPathsClassesRoot)
      {
        RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(str1);
        if (registryKey != null)
        {
          string str2 = (string) registryKey.GetValue((string) null);
          if (str2 != null && !str2.Equals(FixUpOle.olePath))
          {
            Logger.Info("Trying to fix: classes root - {0}", (object) str1);
            if (!FixUpOle.WriteTrustedRegistry(0, str1, blAdd))
              return false;
          }
        }
      }
      foreach (string str1 in FixUpOle.oleKeyPathsCurrentUser)
      {
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(str1);
        if (registryKey != null)
        {
          string str2 = (string) registryKey.GetValue((string) null);
          if (str2 != null && !str2.Equals(FixUpOle.olePath))
          {
            Logger.Info("Trying to fix: current user - {0}", (object) str1);
            if (!FixUpOle.WriteTrustedRegistry(1, str1, blAdd))
              return false;
          }
        }
      }
      foreach (string str1 in FixUpOle.oleKeyPathsLocalMachine)
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(str1);
        if (registryKey != null)
        {
          string str2 = (string) registryKey.GetValue((string) null);
          if (str2 != null && !str2.Equals(FixUpOle.olePath))
          {
            Logger.Info("Trying to fix: local machine - {0}", (object) str1);
            if (!FixUpOle.WriteTrustedRegistry(2, str1, blAdd))
              return false;
          }
        }
      }
      return true;
    }

    public static bool WriteTrustedRegistry(int regType, string regPath, bool blAdd)
    {
      try
      {
        WindowsIdentity current = WindowsIdentity.GetCurrent();
        if (!Trust.MySetPrivilege("SeTakeOwnershipPrivilege", true))
        {
          Logger.Info("Failed to take ownership privilege");
          return false;
        }
        if (!Trust.MySetPrivilege("SeRestorePrivilege", true))
        {
          Logger.Info("Failed to restore ownership privilege");
          return false;
        }
        RegistryKey registryKey = (RegistryKey) null;
        switch (regType)
        {
          case 0:
            registryKey = Registry.ClassesRoot.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);
            break;
          case 1:
            registryKey = Registry.CurrentUser.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);
            break;
          case 2:
            registryKey = Registry.LocalMachine.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);
            break;
        }
        if (registryKey == null)
          return true;
        RegistrySecurity accessControl = registryKey.GetAccessControl(AccessControlSections.All);
        SecurityIdentifier securityIdentifier = new SecurityIdentifier(accessControl.GetOwner(typeof (SecurityIdentifier)).ToString());
        accessControl.SetOwner((IdentityReference) current.User);
        registryKey.SetAccessControl(accessControl);
        RegistryAccessRule rule = new RegistryAccessRule((IdentityReference) current.User, RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
        accessControl.AddAccessRule(rule);
        registryKey.SetAccessControl(accessControl);
        registryKey.Close();
        switch (regType)
        {
          case 0:
            registryKey = Registry.ClassesRoot.OpenSubKey(regPath, true);
            break;
          case 1:
            registryKey = Registry.CurrentUser.OpenSubKey(regPath, true);
            break;
          case 2:
            registryKey = Registry.LocalMachine.OpenSubKey(regPath, true);
            break;
        }
        if (blAdd)
          registryKey.SetValue((string) null, (object) FixUpOle.olePath);
        else
          registryKey.SetValue((string) null, (object) "oleaut32.dll");
        accessControl.SetOwner((IdentityReference) securityIdentifier);
        registryKey.SetAccessControl(accessControl);
        accessControl.RemoveAccessRule(rule);
        registryKey.SetAccessControl(accessControl);
        registryKey.Close();
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private enum RegBase
    {
      CLASSES_ROOT,
      CURRENT_USER,
      LOCAL_MACHINE,
    }
  }
}
