// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CommonInstallUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;

namespace BlueStacks.Common
{
  public static class CommonInstallUtils
  {
    internal static List<string> sDisallowedDeletionStrings = new List<string>()
    {
      "*",
      "\\",
      Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
      Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
      Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
    };
    private static bool is64BitProcess = IntPtr.Size == 8;
    private const string OpenGL_Native_DLL = "HD-OpenGl-Native.dll";
    private const int CSIDL_COMMON_DESKTOPDIRECTORY = 25;

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern int SHGetFolderPath(
      IntPtr hwndOwner,
      int nFolder,
      IntPtr hToken,
      uint dwFlags,
      [Out] StringBuilder pszPath);

    [DllImport("HD-LibraryHandler.dll", CharSet = CharSet.Auto)]
    private static extern int DeleteLibrary(string libraryName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool CreateHardLink(
      string lpFileName,
      string lpExistingFileName,
      IntPtr lpSecurityAttributes);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWow64Process([In] IntPtr hProcess, out bool wow64Process);

    [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int RegRenameKey(UIntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string oldname, [MarshalAs(UnmanagedType.LPWStr)] string newname);

    [DllImport("HD-OpenGl-Native.dll")]
    public static extern int IsVulkanSupported();

    [DllImport("HD-OpenGl-Native.dll")]
    public static extern void PgaLoggerInit(Logger.HdLoggerCallback cb);

    public static bool Is64BitOperatingSystem
    {
      get
      {
        return CommonInstallUtils.is64BitProcess || CommonInstallUtils.InternalCheckIsWow64();
      }
    }

    private static bool InternalCheckIsWow64()
    {
      if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) && Environment.OSVersion.Version.Major < 6)
        return false;
      using (Process currentProcess = Process.GetCurrentProcess())
      {
        bool wow64Process;
        return CommonInstallUtils.IsWow64Process(currentProcess.Handle, out wow64Process) && wow64Process;
      }
    }

    public static void KillBlueStacksProcesses(string clientInstallDir = null, bool killPlayerProcess = false)
    {
      Logger.Info("Killing all BlueStacks processes");
      Utils.KillCurrentOemProcessByName(new string[11]
      {
        "BlueStacks",
        "Keymapui",
        "HD-OBS",
        "HD-Agent",
        "HD-Adb",
        "HD-RunApp",
        "HD-LogCollector",
        "HD-DataManager",
        "HD-QuitMultiInstall",
        "HD-MultiInstanceManager",
        "BlueStacksHelper"
      }, clientInstallDir);
      if (!killPlayerProcess)
        return;
      Utils.KillCurrentOemProcessByName("HD-Player", clientInstallDir);
      Utils.KillCurrentOemProcessByName("BstkSVC", clientInstallDir);
    }

    public static string EngineInstallDir
    {
      get
      {
        return (string) Utils.GetRegistryHKLMValue(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\BlueStacks{0}", (object) Strings.GetOemTag()), "InstallDir", (object) string.Empty);
      }
    }

    public static void RunHdQuit(string hdQuitPath)
    {
      try
      {
        string str = Path.Combine(hdQuitPath, "HD-Quit.exe");
        using (Process process = new Process())
        {
          process.StartInfo.FileName = str;
          process.Start();
          process.WaitForExit();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in running hd-quit err: {0}", (object) ex.ToString());
      }
    }

    public static void ModifyDirectoryPermissionsForEveryone(string dir)
    {
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);
      try
      {
        string identity = new SecurityIdentifier("S-1-1-0").Translate(typeof (NTAccount)).ToString();
        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
        accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        directoryInfo.SetAccessControl(accessControl);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set permissions. err: " + ex.ToString());
      }
      try
      {
        if (!SystemUtils.IsOSWinXP())
        {
          foreach (string file in Directory.GetFiles(dir))
          {
            string identity = new SecurityIdentifier("S-1-1-0").Translate(typeof (NTAccount)).ToString();
            FileInfo fileInfo = new FileInfo(file);
            FileSecurity accessControl = fileInfo.GetAccessControl();
            accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow));
            fileInfo.SetAccessControl(accessControl);
          }
        }
        foreach (string directory in Directory.GetDirectories(dir))
          CommonInstallUtils.ModifyDirectoryPermissionsForEveryone(directory);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set permissions. err: " + ex.ToString());
      }
    }

    public static bool MoveDirectory(string srcDir, string dstDir)
    {
      Logger.Info("Moving directory {0} to {1}", (object) srcDir, (object) dstDir);
      try
      {
        if (Directory.Exists(dstDir))
          Directory.Delete(dstDir, true);
        Directory.Move(srcDir, dstDir);
      }
      catch (Exception ex1)
      {
        Logger.Info("------------ FOR DEV TRACKING--------------- Moving failed");
        Logger.Info("Caught exception when moving directory {0} to {1} err :{2}", (object) srcDir, (object) dstDir, (object) ex1.ToString());
        if (!Directory.Exists(dstDir))
          Directory.CreateDirectory(dstDir);
        foreach (string file in Directory.GetFiles(srcDir))
        {
          FileInfo fileInfo = new FileInfo(file);
          string str = Path.Combine(dstDir, fileInfo.Name);
          try
          {
            if (File.Exists(str))
            {
              File.SetAttributes(file, FileAttributes.Normal);
              File.Delete(str);
            }
            File.Move(file, str);
          }
          catch (Exception ex2)
          {
            Logger.Info("Exception in file move {0} to {1}. Copying instead.. ex:{2}", (object) file, (object) str, (object) ex2.ToString());
            try
            {
              File.Copy(file, str, true);
            }
            catch (Exception ex3)
            {
              Logger.Error("Exception in file copy: THIS WILL RESULT IN DEPLOYMENT FAILURE" + ex3.ToString());
              return false;
            }
          }
        }
        foreach (string directory in Directory.GetDirectories(srcDir))
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(directory);
          string dstDir1 = Path.Combine(dstDir, directoryInfo.Name);
          string srcDir1 = Path.Combine(srcDir, directoryInfo.Name);
          if (!CommonInstallUtils.MoveDirectory(srcDir1, dstDir1))
          {
            Logger.Warning("Returing false in directory move for {0} to {1}", (object) srcDir1, (object) dstDir1);
            return false;
          }
        }
        try
        {
          Directory.Delete(srcDir);
        }
        catch (Exception ex2)
        {
          Logger.Info("Ignoring exception when trying to delete srcDir at the end err:{0}", (object) ex2.ToString());
        }
      }
      return true;
    }

    public static string GetFolderPath(int CSIDL)
    {
      StringBuilder pszPath = new StringBuilder(260);
      CommonInstallUtils.SHGetFolderPath(IntPtr.Zero, CSIDL, IntPtr.Zero, 0U, pszPath);
      return pszPath.ToString();
    }

    public static void DeleteLegacyShortcuts()
    {
      Logger.Info("Deleting legacy shortcuts");
      ShortcutHelper.DeleteDesktopShortcut("Start BlueStacks.lnk");
      ShortcutHelper.DeleteStartMenuShortcut("Start BlueStacks.lnk");
      ShortcutHelper.DeleteStartMenuShortcut("Programs\\BlueStacks\\Start BlueStacks.lnk");
      if (!string.IsNullOrEmpty(Oem.Instance.DesktopShortcutFileName))
      {
        ShortcutHelper.DeleteDesktopShortcut(Oem.Instance.DesktopShortcutFileName);
        ShortcutHelper.DeleteStartMenuShortcut(Oem.Instance.DesktopShortcutFileName);
      }
      if (Oem.Instance.CreateMultiInstanceManagerIcon)
      {
        ShortcutHelper.DeleteDesktopShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
        ShortcutHelper.DeleteStartMenuShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
      }
      try
      {
        string path = Path.Combine(CommonInstallUtils.GetFolderPath(25), Oem.Instance.DesktopShortcutFileName);
        if (!File.Exists(path))
          return;
        File.Delete(path);
      }
      catch
      {
      }
    }

    public static bool CheckIfSDCardPresent()
    {
      return File.Exists(Path.Combine(Path.Combine(RegistryStrings.DataDir, "Android"), "SDCard.vdi"));
    }

    public static void DeleteOldShortcuts()
    {
      try
      {
        CommonInstallUtils.DeleteLegacyShortcuts();
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to delete old shortcuts. Err: " + ex.ToString());
      }
    }

    public static void CreateDesktopAndStartMenuShortcuts(
      string shortcutName,
      string iconPath,
      string targetBinPath,
      string args = "",
      string description = "",
      string package = "")
    {
      try
      {
        if (!Oem.Instance.IsCreateDesktopAndStartMenuShortcut || string.IsNullOrEmpty(shortcutName))
          return;
        if (string.IsNullOrEmpty(description))
          description = shortcutName;
        ShortcutHelper.CreateCommonDesktopShortcut(shortcutName, iconPath, targetBinPath, args, description, package);
        ShortcutHelper.CreateCommonStartMenuShortcut(shortcutName, iconPath, targetBinPath, args, shortcutName, package);
        if (!File.Exists(Path.Combine(ShortcutHelper.CommonDesktopPath, shortcutName.Replace(".lnk", "") + ".lnk")))
        {
          Logger.Info("Failed to make common desktop shortcut...atempting user desktop");
          ShortcutHelper.CreateDesktopShortcut(shortcutName, iconPath, targetBinPath, args, description, package);
        }
        Utils.SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to create BlueStacks shortcuts. ex: " + ex.ToString());
      }
    }

    public static void UpdateOldAppDesktopShortcutTarget(string oldInstallDir, string newInstallDir)
    {
      try
      {
        foreach (string file in Directory.GetFiles(ShortcutHelper.sDesktopPath, "*.lnk", SearchOption.AllDirectories))
        {
          try
          {
            if (File.Exists(file))
            {
              if (ShortcutHelper.GetShortcutArguments(file).TrimEnd('\\').ToLower(CultureInfo.InvariantCulture).Equals(Path.Combine(oldInstallDir, "HD-RunApp.exe").ToLower(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
              {
                string shortcutIconLocation = ShortcutHelper.GetShortcutIconLocation(file);
                if (shortcutIconLocation.ToLower(CultureInfo.InvariantCulture).Contains("library\\icons"))
                {
                  string iconPath = shortcutIconLocation.Replace("Library\\Icons", "Gadget");
                  ShortcutHelper.UpdateTargetPathAndIcon(file, Path.Combine(newInstallDir, "HD-RunApp.exe"), iconPath);
                }
              }
            }
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to fix app shortcut target");
        Logger.Error(ex.ToString());
      }
    }

    public static void CreatePartnerRegistryEntry(string clientInstallDir)
    {
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\Config", (object) Strings.GetOemTag());
      Logger.Info("CreatePartnerRegistryEntry start");
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true))
      {
        if (registryKey != null)
          registryKey.SetValue("PartnerExePath", (object) Path.Combine(clientInstallDir, "BlueStacks.exe"));
        else
          Logger.Info("Not writing partner exe path , registry not exists");
      }
    }

    public static bool RecreateAddRemoveRegistry(
      string pfDir,
      string iconPath,
      string displayName,
      string publisher)
    {
      try
      {
        string subkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\BlueStacks" + Strings.GetOemTag();
        using (RegistryKey subKey = Registry.LocalMachine.CreateSubKey(subkey))
        {
          subKey.SetValue("DisplayName", (object) displayName);
          subKey.SetValue("DisplayVersion", (object) "4.250.0.1070");
          subKey.SetValue("DisplayIcon", (object) iconPath);
          subKey.SetValue("EstimatedSize", (object) 2096128, RegistryValueKind.DWord);
          subKey.SetValue("InstallDate", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:yyyyMMdd}", (object) DateTime.Now));
          subKey.SetValue("NoModify", (object) "1", RegistryValueKind.DWord);
          subKey.SetValue("NoRepair", (object) "1", RegistryValueKind.DWord);
          subKey.SetValue("Publisher", (object) publisher);
          subKey.SetValue("UninstallString", (object) Path.Combine(pfDir, "BlueStacksUninstaller.exe -tmp"));
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Info("Error in creating ControlPanel registry: {0}", (object) ex.ToString());
      }
      return false;
    }

    public static List<string> GetUninstallCurrentVersionSubKey(string keyToSearch)
    {
      List<string> stringList = new List<string>();
      using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true))
      {
        foreach (string subKeyName in registryKey1.GetSubKeyNames())
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, true))
          {
            string str = (string) registryKey2.GetValue("DisplayName");
            if (str != null)
            {
              if (str.Equals(keyToSearch, StringComparison.OrdinalIgnoreCase))
                stringList.Add(subKeyName);
            }
          }
        }
      }
      return stringList;
    }

    public static CmdRes ExtractZip(string zipFilePath, string extractDirectory)
    {
      string cmd = Path.Combine(Directory.GetCurrentDirectory(), "7zr.exe");
      if (!Directory.Exists(extractDirectory))
        Directory.CreateDirectory(extractDirectory);
      string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "x \"{0}\" -o\"{1}\" -aoa", (object) zipFilePath, (object) extractDirectory);
      return RunCommand.RunCmd(cmd, args, false, true, false, 0);
    }

    public static void CopyLogFileToLogDir(string logFilePath, string destFileName)
    {
      try
      {
        string destFileName1 = Path.Combine(RegistryManager.Instance.LogDir, destFileName);
        File.Copy(logFilePath, destFileName1, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Got exception when copying isntaller logs in log dir ex :{0}", (object) ex.ToString());
      }
    }

    public static void DeleteDirectory(List<string> directories, bool throwError = false)
    {
      if (directories == null)
        return;
      foreach (string directory in directories)
      {
        try
        {
          CommonInstallUtils.DeleteDirectory(directory);
        }
        catch (Exception ex)
        {
          Logger.Warning("Failed to delete directory {0}, ignoring", (object) directory);
          Logger.Warning(ex.Message);
          if (throwError)
            throw;
        }
      }
    }

    public static void DeleteDirectory(string targetDir)
    {
      if (!string.IsNullOrEmpty(targetDir))
      {
        if (!CommonInstallUtils.sDisallowedDeletionStrings.Any<string>((Func<string, bool>) (str => str.Equals(targetDir, StringComparison.CurrentCultureIgnoreCase))))
        {
          try
          {
            Logger.Info("Deleting directory : " + targetDir);
            Directory.Delete(targetDir, true);
            return;
          }
          catch (DirectoryNotFoundException ex)
          {
            Logger.Warning("Could not delete {0} as it does not exist", (object) targetDir);
            return;
          }
          catch (Exception ex1)
          {
            Logger.Warning("Got exception when deleting {0}, err:{1}", (object) targetDir, (object) ex1.ToString());
            Logger.Info("------------- FOR DEV TRACKING --------------");
            foreach (string file in Directory.GetFiles(targetDir))
            {
              File.SetAttributes(file, FileAttributes.Normal);
              File.Delete(file);
            }
            foreach (string directory in Directory.GetDirectories(targetDir))
              CommonInstallUtils.DeleteDirectory(directory);
            try
            {
              Directory.Delete(targetDir, true);
              return;
            }
            catch (IOException ex2)
            {
              Thread.Sleep(100);
              try
              {
                Directory.Delete(targetDir, true);
                return;
              }
              catch
              {
                return;
              }
            }
            catch (UnauthorizedAccessException ex2)
            {
              Thread.Sleep(100);
              try
              {
                Directory.Delete(targetDir, true);
                return;
              }
              catch
              {
                return;
              }
            }
          }
        }
      }
      Logger.Warning("A hazardous DirectoryDelete for '{0}' was invoked. Ignoring the call", (object) targetDir);
    }

    public static void GetScreenResolution(
      out int windowWidth,
      out int windowHeight,
      out int guestWidth,
      out int guestHeight)
    {
      double aspectRatio = 16.0 / 9.0;
      double width;
      WpfUtils.GetDefaultSize(out width, out double _, out double _, aspectRatio, true);
      windowWidth = (int) (width - 17.0) / 16 * 16;
      windowHeight = windowWidth / 16 * 9;
      Utils.GetGuestWidthAndHeight(windowWidth, windowHeight, out guestWidth, out guestHeight);
      if (Oem.Instance.IsPixelParityToBeIgnored)
        Utils.GetWindowWidthAndHeight(out guestWidth, out guestHeight);
      Logger.Info("Resolution values: guestWidth: {0} guestHeight: {1} widowWidth: {2} windowHeight: {3}", (object) guestWidth, (object) guestHeight, (object) windowWidth, (object) windowHeight);
    }

    public static void CreateWebUrlScheme(string clientInstallDir)
    {
      try
      {
        string bgpKeyName = Strings.BGPKeyName;
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(bgpKeyName))
        {
          subKey.SetValue("", (object) "BlueStacks Web Url Scheme");
          subKey.SetValue("URL Protocol", (object) "");
        }
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(bgpKeyName + "\\DefaultIcon"))
          subKey.SetValue("", (object) Path.Combine(clientInstallDir, "ProductLogo.ico"));
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(bgpKeyName + "\\Shell"))
          subKey.SetValue("", (object) "open");
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(bgpKeyName + "\\Shell\\open"))
          subKey.SetValue("", (object) "Open BlueStacks Game Platform");
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(bgpKeyName + "\\Shell\\open\\command"))
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\" %1", (object) Path.Combine(RegistryManager.Instance.InstallDir, "Bluestacks.exe"));
          subKey.SetValue("", (object) str);
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to create web url scheme. Err: " + ex.ToString());
      }
    }

    public static void CreateInstallApkScheme(
      string installDir,
      string baseKeyName,
      string targetBinary)
    {
      Logger.Info("CreateInstallApkScheme start");
      try
      {
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(baseKeyName))
          subKey.SetValue("", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Android Package File", (object) Strings.ProductDisplayName));
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\DefaultIcon"))
          subKey.SetValue("", (object) RegistryStrings.ProductIconCompletePath);
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell"))
          subKey.SetValue("", (object) "open");
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell\\open"))
          subKey.SetValue("", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Open with {0} APK Installer", (object) Strings.ProductDisplayName));
        using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell\\open\\command"))
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} \"%1\"", (object) Path.Combine(installDir, targetBinary));
          subKey.SetValue("", (object) str);
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Some error while setting APKHandler extention association, ex: " + ex.Message);
      }
      try
      {
        if (baseKeyName != null && baseKeyName.Equals("BlueStacks.Apk", StringComparison.OrdinalIgnoreCase))
        {
          using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(".apk"))
            subKey.SetValue("", (object) baseKeyName, RegistryValueKind.String);
        }
        else
        {
          using (RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(".xapk"))
            subKey.SetValue("", (object) baseKeyName, RegistryValueKind.String);
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Some error while setting main .apk extention association, ex: " + ex.Message);
      }
      Logger.Info("CreateInstallApkScheme end");
    }

    public static void DeleteInstallApkScheme(string installDir, string baseKeyName)
    {
      Logger.Info("DeleteInstallApkScheme start");
      try
      {
        string str1 = "junkPath";
        using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(baseKeyName + "\\Shell\\open\\command"))
          str1 = (string) registryKey.GetValue("");
        string str2 = str1.Trim('"');
        string str3;
        if (installDir == null)
          str3 = (string) null;
        else
          str3 = installDir.Trim('"');
        installDir = str3;
        if (str2.StartsWith(installDir, StringComparison.OrdinalIgnoreCase))
          Registry.ClassesRoot.DeleteSubKeyTree(baseKeyName);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to delete apk handler registry. Ex : " + ex.ToString());
      }
      Logger.Info("DeleteInstallApkScheme end");
    }

    public static void LogAllServiceNames()
    {
      try
      {
        Logger.Info("Installed kernel level services:-");
        foreach (ServiceController device in ServiceController.GetDevices())
          Logger.Info("ServiceName: {0},\tDisplayName: {1}, status: {2}", (object) device.ServiceName, (object) device.DisplayName, (object) device.Status);
        Logger.Info("Installed services:-");
        foreach (ServiceController service in ServiceController.GetServices())
          Logger.Info("ServiceName: {0},\tDisplayName: {1}, status: {2}", (object) service.ServiceName, (object) service.DisplayName, (object) service.Status);
      }
      catch (Exception ex)
      {
        Logger.Warning("Some error occured while logging services, ex: " + ex.Message);
      }
    }

    public static void CheckForActiveHandles(string installerExtractedPath)
    {
      Logger.Info("Checking for active Handles");
      try
      {
        RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey("Software\\Sysinternals\\Handle", true);
        if (registryKey1 == null)
          Registry.CurrentUser.CreateSubKey("Software\\Sysinternals\\Handle");
        else
          registryKey1.Close();
        using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Software\\Sysinternals\\Handle", true))
        {
          registryKey2.SetValue("EulaAccepted", (object) 1, RegistryValueKind.DWord);
          Logger.Info("Accepted");
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't access registry, ex: {0}", (object) ex.Message);
      }
      try
      {
        RunCommand.RunCmd(Path.Combine(installerExtractedPath, "HD-Handle.exe"), "bluestacks", true, true, false, 0);
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't check for active handles, ex: {0}", (object) ex.Message);
      }
    }

    public static void RemoveAndAddFirewallRule(string ruleName, string binPath)
    {
      CommonInstallUtils.RemoveFirewallRule(ruleName);
      CommonInstallUtils.AddFirewallRule(ruleName, binPath);
    }

    public static void AddFirewallRule(string ruleName, string binPath)
    {
      Logger.Info("Adding firewall rule fo bin {0}", (object) binPath);
      try
      {
        RunCommand.RunCmd("netsh.exe", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "advfirewall firewall add rule name=\"{0}\" dir=in action=allow program=\"{1}\" enable=yes", (object) ruleName, (object) binPath), false, true, false, 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in adding firewall rule", (object) ex.ToString());
      }
    }

    public static void RemoveFirewallRule(string ruleName)
    {
      try
      {
        RunCommand.RunCmd("netsh.exe", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "advfirewall firewall delete rule name=\"{0}\"", (object) ruleName), false, true, false, 0);
      }
      catch (Exception ex)
      {
        Logger.Error("{0} Firewall rule dont exist {1}", (object) ruleName, (object) ex.ToString());
      }
    }

    public static bool CreateHardLinkForFile(string existingFilePath, string newFilePath)
    {
      try
      {
        Logger.Info("Creating link from " + existingFilePath + " to " + newFilePath);
        if (!CommonInstallUtils.CreateHardLink(newFilePath, existingFilePath, IntPtr.Zero))
        {
          Logger.Error("Failed to create hard link: " + Marshal.GetLastWin32Error().ToString());
          if (File.Exists(existingFilePath))
          {
            Logger.Error("Copying the file instead");
            File.Copy(existingFilePath, newFilePath, true);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed in creating hard link, Ex: " + ex.ToString());
      }
      return false;
    }

    public static bool CreateHardLinkForDirectory(string existingDirPath, string newDirPath)
    {
      try
      {
        Logger.Info("Creating link from " + existingDirPath + " to " + newDirPath);
        if (!Directory.Exists(newDirPath))
          Directory.CreateDirectory(newDirPath);
        foreach (FileInfo file in new DirectoryInfo(existingDirPath).GetFiles())
        {
          if (!CommonInstallUtils.CreateHardLinkForFile(file.FullName, Path.Combine(newDirPath, file.Name)))
          {
            Logger.Error("Failed to create hard link for file : " + file.FullName);
            return false;
          }
        }
        foreach (DirectoryInfo directory in new DirectoryInfo(existingDirPath).GetDirectories())
        {
          if (!CommonInstallUtils.CreateHardLinkForDirectory(Path.Combine(existingDirPath, directory.Name), Path.Combine(newDirPath, directory.Name)))
            return false;
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to create hard link for directory, Err: " + ex.ToString());
      }
      return false;
    }

    public static bool IsDiskFull(Exception ex)
    {
      int num = Marshal.GetHRForException(ex) & (int) ushort.MaxValue;
      Logger.Info("Win32 error code: " + num.ToString());
      Logger.Info("Disk full error codes are: {0} and {1}", (object) 112, (object) 39);
      return num == 39 || num == 112;
    }

    public static bool StopAndUninstallService(
      string svcName,
      int timeoutSeconds = 15,
      bool isKernelDriver = false)
    {
      Logger.Info("Uninstalling service {0}", (object) svcName);
      try
      {
        CommonInstallUtils.StopService(svcName, timeoutSeconds);
        CommonInstallUtils.UninstallService(svcName, isKernelDriver);
        return true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Ignoring exception when uninstalling service {0} ex : {1}", (object) svcName, (object) ex.ToString());
        return false;
      }
    }

    private static void StopService(string serviceName, int timeoutSeconds = 15)
    {
      Logger.Info("Stopping service {0} with timeout {1}s", (object) serviceName, (object) timeoutSeconds);
      using (ServiceController serviceController = new ServiceController(serviceName))
      {
        TimeSpan timeout = TimeSpan.FromSeconds((double) timeoutSeconds);
        if (serviceController.Status != ServiceControllerStatus.Stopped)
        {
          if (serviceController.Status != ServiceControllerStatus.StopPending)
          {
            try
            {
              Logger.Info("Service is running , stopping the service {0}", (object) serviceName);
              serviceController.Stop();
              serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
              Logger.Info("Service stopped successfully");
              return;
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
              Logger.Error("Timed out while waiting for service to stop");
              throw;
            }
            catch (Exception ex)
            {
              Logger.Error("Got exception stopping service {0}, Err: {1}", (object) serviceName, (object) ex.ToString());
              throw;
            }
          }
        }
        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        Logger.Info("Service {0} stopped", (object) serviceName);
      }
    }

    private static void UninstallService(string name, bool isKernelDriverService = false)
    {
      ServiceManager.UninstallService(name, isKernelDriverService);
    }

    public static bool DoesRegistryHKLMKeyExist(string keyPath)
    {
      RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath);
      int num = registryKey != null ? 1 : 0;
      if (num == 0)
        return num != 0;
      registryKey.Close();
      return num != 0;
    }

    public static string ConvertIntToEnumString(int enumCode)
    {
      return Enum.GetName(typeof (InstallerCodes), (object) (InstallerCodes) enumCode);
    }

    public static string GetCurrentLocale()
    {
      try
      {
        return Thread.CurrentThread.CurrentCulture.Name;
      }
      catch
      {
        return "en-US";
      }
    }

    public static void StopBlueStacksIfUpgrade(
      bool isUpgrade,
      List<string> svcNames,
      string clientInstallDir,
      out bool isServiceStopped)
    {
      isServiceStopped = true;
      if (!isUpgrade)
        return;
      CommonInstallUtils.KillBlueStacksProcesses(clientInstallDir, false);
      CommonInstallUtils.RunHdQuit(CommonInstallUtils.EngineInstallDir);
      Utils.KillCurrentOemProcessByName("HD-Player", (string) null);
      Utils.KillCurrentOemProcessByName("BstkSVC", (string) null);
      if (svcNames == null)
        return;
      foreach (string svcName in svcNames)
      {
        ServiceManager.StopService(svcName, false);
        isServiceStopped = isServiceStopped && CommonInstallUtils.CheckIfServiceStopped(svcName);
      }
    }

    private static bool CheckIfServiceStopped(string svcName)
    {
      try
      {
        Logger.Info("Checking if {0} is stopped", (object) svcName);
        foreach (ServiceController device in ServiceController.GetDevices())
        {
          if (device.ServiceName == svcName)
          {
            ServiceControllerStatus status = device.Status;
            Logger.Info("Service status of {0} is {1}", (object) svcName, (object) status);
            if (status != ServiceControllerStatus.Stopped)
            {
              Logger.Warning("Service is not stopped, returning false");
              return false;
            }
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while checking if svc is stopped, ex: " + ex.ToString());
      }
      return true;
    }

    public static bool IsUpgradePossible(string clientKeyPath)
    {
      Logger.Info("Checking if upgrade possible");
      bool flag = false;
      System.Version version1 = new System.Version("3.52.66.1905");
      System.Version version2 = new System.Version("2.52.66.8704");
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(clientKeyPath))
      {
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        if (registryKey != null)
        {
          empty1 = (string) registryKey.GetValue("ClientVersion", (object) "");
          if (string.IsNullOrEmpty(empty1))
            empty2 = (string) registryKey.GetValue("Version", (object) "");
        }
        if (!string.IsNullOrEmpty(empty1))
        {
          System.Version version3 = new System.Version(empty1);
          Logger.Info("Previous client version is {0}", (object) empty1);
          if (version3 >= version1)
            flag = true;
        }
        else if (!string.IsNullOrEmpty(empty2))
        {
          System.Version version3 = new System.Version(empty2);
          Logger.Info("Previous engine version is {0}", (object) empty2);
          if (version3 >= version2)
            flag = true;
        }
        else
        {
          Logger.Info("ClientVersion as well as Version registry not found so setting isUpgradePossible to false");
          flag = false;
        }
      }
      Logger.Info("IsUpgradePossible: {0}", (object) flag);
      return flag;
    }

    public static string GenerateUniqueInstallId()
    {
      Logger.Info("Generating install ID");
      try
      {
        string str = Guid.NewGuid().ToString();
        Logger.Info("GeneratedID: " + str);
        return str;
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to generate unique install id");
        Logger.Warning(ex.ToString());
      }
      return string.Empty;
    }

    public static void LogAllDirs(List<string> listOfDirs)
    {
      Logger.Info("-----------------------------------------------------");
      Logger.Info("Logging all dirs");
      foreach (string dir in listOfDirs.Distinct<string>().ToList<string>())
        CommonInstallUtils.LogDir(dir, false);
      Logger.Info("-----------------------------------------------------");
    }

    public static void LogDir(string dir, bool onlyDirs = false)
    {
      try
      {
        CommonInstallUtils.DumpDirListing(dir, onlyDirs);
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't log dir {0}, ignoring exception: {1}", (object) dir, (object) ex.Message);
      }
    }

    private static void DumpDirListing(string dir, bool onlyDirs = false)
    {
      string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/c dir \"{0}\" /s", (object) dir);
      if (onlyDirs)
        args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/c dir \"{0}\" /ad /s", (object) dir);
      RunCommand.RunCmd("cmd", args, true, true, false, 0);
    }

    public static bool CheckForOldConfigFiles(string dataDir)
    {
      try
      {
        string path = Path.Combine(dataDir, "UserData\\InputMapper\\UserFiles");
        if (Directory.Exists(path))
        {
          foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*.cfg"))
          {
            if (file.Length != 0L)
              return true;
            Logger.Warning("Zero length config file found, ignoring: {0}", (object) file.Name);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error in checking for old Config Files: " + ex.Message);
      }
      return false;
    }

    public static bool CheckIfValidJsonFile(string jsonFileName)
    {
      try
      {
        Logger.Info("Checking if {0} is a valid json file", (object) jsonFileName);
        JArray.Parse(File.ReadAllText(jsonFileName));
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("{0} may be corrupt. Ex: {1}", (object) jsonFileName, (object) ex.Message);
      }
      return false;
    }

    public static string ZipLogsAndRegistry(string logsDir, string currentInstallerLogPath)
    {
      Logger.Info("In ZipLogsAndRegistry");
      try
      {
        string stagingDir = CommonInstallUtils.CreateStagingDir();
        string str = Path.Combine(stagingDir, "Logs");
        if (!Directory.Exists(logsDir))
          return (string) null;
        Directory.CreateDirectory(str);
        CommonInstallUtils.CopyFiles(logsDir, str);
        CommonInstallUtils.ExportBluestacksRegistry(stagingDir, "RegHKLM.txt");
        string destFileName = Path.Combine(stagingDir, Path.GetFileName(currentInstallerLogPath));
        File.Copy(currentInstallerLogPath, destFileName);
        string zipFilePath = Path.Combine(Path.GetTempPath(), "Installer.zip");
        CommonInstallUtils.CreateZipFile(stagingDir, zipFilePath);
        Directory.Delete(str, true);
        return zipFilePath;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating zip file " + ex.ToString());
        return (string) null;
      }
    }

    private static void CreateZipFile(string zipFolderTempPath, string zipFilePath)
    {
      try
      {
        string prog = Path.Combine(Directory.GetCurrentDirectory(), "7zr.exe");
        string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "a {0} -m0=LZMA:a=2 {1}\\*", (object) zipFilePath, (object) zipFolderTempPath);
        if (File.Exists(zipFilePath))
          File.Delete(zipFilePath);
        Utils.RunCmd(prog, args, (string) null);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating Zip " + ex.ToString());
      }
    }

    private static void ExportBluestacksRegistry(string destination, string fileName)
    {
      try
      {
        Utils.RunCmd("reg.exe", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EXPORT HKLM\\{0} \"{1}\"", (object) Strings.RegistryBaseKeyPath, (object) Path.Combine(destination, fileName)), (string) null);
      }
      catch
      {
      }
    }

    private static void CopyFiles(string src, string dest)
    {
      foreach (string file in Directory.GetFiles(src))
      {
        string fileName = Path.GetFileName(file);
        File.Copy(file, Path.Combine(dest, fileName));
      }
    }

    private static string CreateStagingDir()
    {
      string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      if (Directory.Exists(path))
        Directory.Delete(path);
      Directory.CreateDirectory(path);
      return path;
    }

    public static Dictionary<string, string> GetSupportedGLModes(string glCheckDir)
    {
      Logger.Info("Checking for supported GL Modes");
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      List<string> stringList = new List<string>();
      try
      {
        foreach (string args in new List<string>()
        {
          "1 1",
          "4 1",
          "1 2",
          "4 2"
        })
        {
          if (CommonInstallUtils.RunGLCheck(glCheckDir, args) == 0)
            stringList.Add(args);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while checking for supported GLModes");
        Logger.Error(ex.ToString());
      }
      dictionary["supported_glmodes"] = string.Join(",", stringList.ToArray());
      Logger.Info("Supported GL Modes: " + string.Join(",", stringList.ToArray()));
      return dictionary;
    }

    private static int RunGLCheck(string glCheckDir, string args)
    {
      try
      {
        return RunCommand.RunCmd(Path.Combine(glCheckDir, "HD-GLCheck.exe"), args, true, true, false, 10000).ExitCode;
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while running glcheck, ignorning.");
        Logger.Warning(ex.Message);
      }
      return -1;
    }

    public static void CheckIfVulkanSupported()
    {
    }

    public static void WriteClientVersionInFile(string version)
    {
      int num = 5;
      while (num > 0)
      {
        try
        {
          string str = Path.Combine(RegistryManager.Instance.UserDefinedDir, "bst_params.txt");
          CommonInstallUtils.WriteToFile(str, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "version={0}", (object) version), nameof (version));
          Logger.Info("version written to file successfully");
          string identity = new SecurityIdentifier("S-1-1-0").Translate(typeof (NTAccount)).ToString();
          FileInfo fileInfo = new FileInfo(str);
          FileSecurity accessControl = fileInfo.GetAccessControl();
          accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow));
          fileInfo.SetAccessControl(accessControl);
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to write agent port to bst_params.txt. Ex: " + ex.ToString());
        }
        Logger.Info("retrying..." + num.ToString());
        --num;
        Thread.Sleep(500);
      }
    }

    private static void WriteToFile(string path, string text, string searchText = "")
    {
      bool flag = true;
      List<string> stringList = new List<string>();
      if (File.Exists(path))
      {
        foreach (string readAllLine in File.ReadAllLines(path))
        {
          if (readAllLine.Contains("="))
          {
            if (readAllLine.Contains(searchText))
            {
              stringList.Add(text);
              flag = false;
            }
            else
              stringList.Add(readAllLine);
          }
        }
      }
      if (flag)
      {
        using (TextWriter textWriter = (TextWriter) new StreamWriter(path, true))
        {
          textWriter.WriteLine(text);
          textWriter.Flush();
        }
      }
      else
      {
        using (TextWriter textWriter = (TextWriter) new StreamWriter(path, false))
        {
          foreach (string str in stringList)
            textWriter.WriteLine(str);
          textWriter.Flush();
        }
      }
    }

    public static int SetupVmConfig(string installDir, string dataDir)
    {
      Logger.Info("In SetupVmConfig");
      if (CommonInstallUtils.InstallVmConfig(installDir, dataDir))
        return 0;
      Logger.Info("Throwing error cannot create android.bstk from in file");
      return -42;
    }

    public static int SetupBstkGlobalConfig(string dataDir)
    {
      Logger.Info("In SetupBstkGlobalConfig");
      if (CommonInstallUtils.InstallVirtualBoxConfig(dataDir, false))
        return 0;
      Logger.Info("Throwing error cannot create bstkglobal from in file");
      return -41;
    }

    public static bool InstallVirtualBoxConfig(string dataDir, bool isUpgrade = false)
    {
      Logger.Info("InstallVirtualBoxConfig()");
      string path1 = Path.Combine(dataDir, "Manager");
      string path2 = Path.Combine(Path.Combine(dataDir, "Manager"), "BstkGlobal.xml");
      string path3 = Path.Combine(path1, "BstkGlobal.xml.in");
      string str1 = (string) null;
      try
      {
        using (StreamReader streamReader = new StreamReader(path3))
          str1 = streamReader.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot read '" + path3 + "': " + ex?.ToString());
        return false;
      }
      string str2 = dataDir;
      if (isUpgrade)
        str2 = RegistryStrings.DataDir.TrimEnd('\\');
      string str3 = str1.Replace("@@USER_DEFINED_DIR@@", str2 + "\\");
      try
      {
        using (StreamWriter streamWriter = new StreamWriter(path2))
          streamWriter.Write(str3);
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot write '" + path2 + "': " + ex?.ToString());
        return false;
      }
      return true;
    }

    public static bool InstallVmConfig(string installDir, string dataDir)
    {
      Logger.Info("InstallVmConfig()");
      string path1 = Path.Combine(dataDir, "Android");
      string str1 = Path.Combine(path1, "Android.bstk");
      string path = Path.Combine(path1, "Android.bstk.in");
      string str2 = (string) null;
      if (File.Exists(str1))
      {
        Logger.Info("android.bstk already present returning");
        return true;
      }
      try
      {
        using (StreamReader streamReader = new StreamReader(path))
          str2 = streamReader.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot read '" + path + "': " + ex?.ToString());
        return false;
      }
      string str3 = str2.Replace("@@HD_PLUS_DEVICES_DLL_PATH@@", SecurityElement.Escape(Path.Combine(installDir, "HD-Plus-Devices.dll")));
      string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      string str4;
      if (!string.IsNullOrEmpty(folderPath1))
      {
        string newValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<SharedFolder name=\"Documents\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", (object) SecurityElement.Escape(folderPath1));
        str4 = str3.Replace("@@USER_DOCUMENTS_FOLDER@@", newValue);
      }
      else
        str4 = str3.Replace("@@USER_DOCUMENTS_FOLDER@@", "");
      string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      string str5;
      if (!string.IsNullOrEmpty(folderPath2))
      {
        string newValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<SharedFolder name=\"Pictures\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", (object) SecurityElement.Escape(folderPath2));
        str5 = str4.Replace("@@USER_PICTURES_FOLDER@@", newValue);
      }
      else
        str5 = str4.Replace("@@USER_PICTURES_FOLDER@@", "");
      string xml = str5.Replace("@@INPUT_MAPPER_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\InputMapper"))).Replace("@@BST_SHARED_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\SharedFolder"))).Replace("@@BST_VM_MEMORY_SIZE@@", SecurityElement.Escape(Utils.GetAndroidVMMemory(true).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        xmlDocument.Save(str1);
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot write '" + str1 + "': " + ex?.ToString());
        return false;
      }
      return true;
    }

    public static bool CheckSupportedGlRenderMode(
      out int glRenderMode,
      out string glVendor,
      out string glRenderer,
      out string glVersion,
      out GLMode glMode,
      List<GlProperty> glCheckOrder,
      List<GlProperty> fallbackGlCheckOrder)
    {
      glRenderMode = 4;
      glVersion = "";
      glRenderer = "";
      glVendor = "";
      glMode = GLMode.PGA;
      try
      {
        if (glCheckOrder != null)
        {
          if (glCheckOrder.Count > 0)
          {
            foreach (GlProperty glProperty in glCheckOrder)
            {
              Logger.Info("gl check with args.." + glProperty.Gl_renderer.ToString() + " and " + glProperty.Gl_mode.ToString());
              if (BlueStacksGL.GLCheckInstallation(glProperty.Gl_renderer, glProperty.Gl_mode, out glVendor, out glRenderer, out glVersion) == 0)
              {
                glRenderMode = (int) glProperty.Gl_renderer;
                glMode = glProperty.Gl_mode;
                return true;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("exception in getting gl check from list.." + ex?.ToString());
      }
      if (Oem.Instance.CheckForAGAInInstaller && CommonInstallUtils.CheckSupportedGlRenderMode(GLMode.AGA, out glRenderMode, out glVendor, out glRenderer, out glVersion))
      {
        Logger.Info("AGA supported!");
        glMode = GLMode.AGA;
        return true;
      }
      if (fallbackGlCheckOrder != null && fallbackGlCheckOrder.Count > 0)
      {
        foreach (GlProperty glProperty in fallbackGlCheckOrder)
        {
          Logger.Info("gl check with args.." + glProperty.Gl_renderer.ToString() + " and " + glProperty.Gl_mode.ToString());
          if (BlueStacksGL.GLCheckInstallation(glProperty.Gl_renderer, glProperty.Gl_mode, out glVendor, out glRenderer, out glVersion) == 0)
          {
            glRenderMode = (int) glProperty.Gl_renderer;
            glMode = glProperty.Gl_mode;
            return true;
          }
        }
      }
      return false;
    }

    public static bool CheckSupportedGlRenderMode(
      GLMode mode,
      out int glRenderMode,
      out string glVendor,
      out string glRenderer,
      out string glVersion)
    {
      glRenderMode = 4;
      glVersion = "";
      glRenderer = "";
      glVendor = "";
      try
      {
        int num;
        if (BlueStacksGL.GLCheckInstallation(GLRenderer.OpenGL, mode, out glVendor, out glRenderer, out glVersion) == 0)
        {
          num = 0;
          glRenderMode = 1;
        }
        else if (SystemUtils.IsOs64Bit())
        {
          if (CommonInstallUtils.GpuWithDx9SupportOnly())
          {
            Logger.Info("Machine has gpu which runs on DX9 only");
            glRenderMode = 2;
            num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
          }
          else
          {
            glRenderMode = 4;
            num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX11FallbackDX9, mode, out glVendor, out glRenderer, out glVersion);
            if (num != 0)
            {
              glRenderMode = 2;
              num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
            }
          }
        }
        else
        {
          glRenderMode = 2;
          num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
        }
        if (num == 0)
          return true;
        Logger.Info("DirectX not supported.");
        glRenderMode = -1;
        return false;
      }
      catch (Exception ex)
      {
        Logger.Error("Some error occured while checking for GL. Ex: {0}", (object) ex);
      }
      return false;
    }

    public static bool GpuWithDx9SupportOnly()
    {
      string[] strArray = new string[6]
      {
        "Mobile Intel(R) 4 Series Express Chipset Family",
        "Mobile Intel(R) 45 Express Chipset Family",
        "Mobile Intel(R) 965 Express Chipset Family",
        "Intel(R) G41 Express Chipset",
        "Intel(R) G45/G43 Express Chipset",
        "Intel(R) Q45/Q43 Express Chipset"
      };
      string str = "";
      try
      {
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration"))
        {
          foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
          {
            foreach (PropertyData property in managementBaseObject.Properties)
            {
              if (property.Name == "Description")
                str = property.Value.ToString();
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while runninq query. Error: ", (object) ex.ToString());
      }
      Logger.Info("Graphics card: {0}", (object) str);
      string lower = str.ToLower(CultureInfo.InvariantCulture);
      if (!lower.Contains("intel") || !lower.Contains("express chipset"))
        return false;
      Logger.Info("graphicsCard : {0} part of the list of graphics card to be forced to dx9", (object) str);
      return true;
    }

    public static bool IsProcessorIntel()
    {
      try
      {
        Dictionary<string, string> dictionary = Profile.Info();
        return dictionary != null && dictionary["Processor"].Contains("Intel");
      }
      catch
      {
        Logger.Error("Unable to detect for intel processor");
        return false;
      }
    }

    public static string GetPrebundledVdiUid(string file)
    {
      string empty = string.Empty;
      if (!File.Exists(file))
        return "";
      string s = File.ReadAllText(file);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.XmlResolver = (XmlResolver) null;
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
      nsmgr.AddNamespace("bstk", "http://www.virtualbox.org/");
      StringReader stringReader = new StringReader(s);
      using (XmlReader reader = XmlReader.Create((TextReader) stringReader, new XmlReaderSettings()
      {
        XmlResolver = (XmlResolver) null
      }))
      {
        xmlDocument.Load(reader);
        foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:MediaRegistry//bstk:HardDisks//bstk:HardDisk", nsmgr))
        {
          if (selectNode.Attributes["location"].Value.Equals("Prebundled.vdi", StringComparison.InvariantCultureIgnoreCase))
          {
            empty = selectNode.Attributes["uuid"].Value;
            break;
          }
        }
        return empty;
      }
    }

    public static string ReadFile(string path)
    {
      string str = (string) null;
      try
      {
        using (StreamReader streamReader = new StreamReader(path))
          str = streamReader.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.Warning("Cannot read '{0}', Ex: {1}", (object) path, (object) ex);
      }
      return str;
    }

    public static bool FixVMConfigJson(
      string oldVmSettingsPath,
      string newVmFolder,
      string defaultNewVmFolder)
    {
      try
      {
        Logger.Info("Will update settings from oldVmSettings {0} in new folder {1} using defaults from folder: {2}", (object) oldVmSettingsPath, (object) newVmFolder, (object) defaultNewVmFolder);
        string str1 = CommonInstallUtils.ReadFile(oldVmSettingsPath);
        if (str1 == null)
          return false;
        string str2 = CommonInstallUtils.ReadFile(Path.Combine(defaultNewVmFolder, "Android.json"));
        if (str2 == null)
          return false;
        JObject jobject1 = (JObject) JsonConvert.DeserializeObject(str1);
        JObject jobject2 = (JObject) JsonConvert.DeserializeObject(str2);
        Logger.Info("Modifying attachments");
        string source = jobject1["Owner"].ToString();
        JObject jobject3 = (JObject) jobject1["VirtualMachine"][(object) "Devices"][(object) "Scsi"][(object) "Boot Disk Controller"][(object) "Attachments"];
        Dictionary<string, JObject> dictionary = (Dictionary<string, JObject>) null;
        int num = 0;
        foreach (JProperty property in jobject3.Properties())
        {
          if (dictionary == null)
            dictionary = new Dictionary<string, JObject>();
          string lower = ((JObject) property.Value).ToString().ToLower(CultureInfo.InvariantCulture);
          if (!lower.Contains("fastboot.vhdx") && !lower.Contains("prebundled.vhdx"))
          {
            dictionary[num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (JObject) property.Value;
            ++num;
          }
        }
        JObject jobject4 = new JObject();
        foreach (string key in dictionary.Keys)
          jobject4[key] = (JToken) dictionary[key];
        jobject2["VirtualMachine"][(object) "Devices"][(object) "Scsi"][(object) "Boot Disk Controller"][(object) "Attachments"] = (JToken) jobject4;
        if (!source.Contains("bgp", StringComparison.InvariantCultureIgnoreCase))
          source += "_bgp";
        jobject2["Owner"] = (JToken) source;
        Logger.Info("Setting owner: {0}", (object) source);
        string path = Path.Combine(newVmFolder, "Android.json");
        Logger.Info("Writing setting file at {0}", (object) path);
        File.WriteAllText(path, JsonConvert.SerializeObject((object) jobject2, Newtonsoft.Json.Formatting.Indented));
        return true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Some error with removing fastboot entry. Ex: {0}", (object) ex);
        return false;
      }
    }

    public static bool UpdateEndpointWithNewGuid(string vmFolder)
    {
      string path1 = Path.Combine(vmFolder, "Android.json");
      string path2 = Path.Combine(vmFolder, "Android.Endpoint.json");
      string str1 = CommonInstallUtils.ReadFile(path1);
      string str2 = CommonInstallUtils.ReadFile(path2);
      if (str1 == null || str2 == null)
        return false;
      Logger.Info("Updating endpoints with new guid in {0}", (object) vmFolder);
      JObject jobject1 = (JObject) JsonConvert.DeserializeObject(str1);
      JObject jobject2 = (JObject) JsonConvert.DeserializeObject(str2);
      Guid guid = Guid.NewGuid();
      jobject1["VirtualMachine"][(object) "Devices"][(object) "NetworkAdapters"][(object) "default"][(object) "EndpointId"] = (JToken) guid.ToString();
      jobject2["VirtualNetwork"] = (JToken) guid.ToString();
      Logger.Info("Writing new guid {0}", (object) guid);
      File.WriteAllText(path1, JsonConvert.SerializeObject((object) jobject1, Newtonsoft.Json.Formatting.Indented));
      File.WriteAllText(path2, JsonConvert.SerializeObject((object) jobject2, Newtonsoft.Json.Formatting.Indented));
      return true;
    }
  }
}
