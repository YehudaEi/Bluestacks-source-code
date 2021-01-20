// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ShortcutHelper
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BlueStacks.Common
{
  public static class ShortcutHelper
  {
    public static readonly string sDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static string sCommonStartMenuPath = (string) null;
    private static string sCommonDesktopPath = (string) null;
    private const int CSIDL_COMMON_DESKTOPDIRECTORY = 25;
    private const int CSIDL_COMMON_STARTMENU = 22;

    [DllImport("shell32.dll")]
    private static extern bool SHGetSpecialFolderPath(
      IntPtr hwndOwner,
      [Out] StringBuilder lpszPath,
      int nFolder,
      bool fCreate);

    public static string CommonStartMenuPath
    {
      get
      {
        if (string.IsNullOrEmpty(ShortcutHelper.sCommonStartMenuPath))
        {
          StringBuilder lpszPath = new StringBuilder(260);
          ShortcutHelper.SHGetSpecialFolderPath(IntPtr.Zero, lpszPath, 22, false);
          ShortcutHelper.sCommonStartMenuPath = Path.Combine(lpszPath.ToString(), "Programs");
        }
        return ShortcutHelper.sCommonStartMenuPath;
      }
    }

    public static string CommonDesktopPath
    {
      get
      {
        if (string.IsNullOrEmpty(ShortcutHelper.sCommonDesktopPath))
        {
          StringBuilder lpszPath = new StringBuilder(260);
          ShortcutHelper.SHGetSpecialFolderPath(IntPtr.Zero, lpszPath, 25, false);
          ShortcutHelper.sCommonDesktopPath = lpszPath.ToString();
        }
        return ShortcutHelper.sCommonDesktopPath;
      }
    }

    private static void DeleteFileIfExists(string filePath)
    {
      string path1 = ShortcutHelper.FixFileName(filePath, "");
      if (File.Exists(path1))
      {
        Logger.Info("Deleting: " + path1);
        File.Delete(path1);
      }
      string path2 = ShortcutHelper.FixFileName(filePath, "ncsoft");
      if (!File.Exists(path2))
        return;
      Logger.Info("Deleting: " + path2);
      File.Delete(path2);
    }

    private static string FixFileName(string fileName, string package = "")
    {
      string str1 = package.Contains("ncsoft") ? " - BlueStacks.lnk" : ".lnk";
      string str2 = fileName;
      bool? nullable = fileName?.EndsWith(str1, StringComparison.InvariantCultureIgnoreCase);
      if (0 == (nullable.GetValueOrDefault() ? 1 : 0) & nullable.HasValue)
        str2 = fileName + str1;
      return str2;
    }

    public static void DeleteDesktopShortcut(string shortcutName)
    {
      ShortcutHelper.DeleteFileIfExists(Path.Combine(ShortcutHelper.sDesktopPath, shortcutName));
    }

    public static void DeleteStartMenuShortcut(string shortcutName)
    {
      ShortcutHelper.DeleteFileIfExists(Path.Combine(ShortcutHelper.CommonStartMenuPath, shortcutName));
    }

    public static void DeleteCommonDesktopShortcut(string shortcutName)
    {
      ShortcutHelper.DeleteFileIfExists(Path.Combine(ShortcutHelper.CommonDesktopPath, shortcutName));
    }

    public static void DeleteCommonStartMenuShortcut(string shortcutName)
    {
      ShortcutHelper.DeleteFileIfExists(Path.Combine(ShortcutHelper.CommonStartMenuPath, shortcutName));
    }

    public static void CreateCommonDesktopShortcut(
      string shortcutName,
      string shortcutIconPath,
      string targetApplication,
      string paramsString,
      string description,
      string package = "")
    {
      ShortcutHelper.CreateShortcut(Path.Combine(ShortcutHelper.CommonDesktopPath, shortcutName), shortcutIconPath, targetApplication, paramsString, description, package);
    }

    public static void CreateCommonStartMenuShortcut(
      string shortcutName,
      string shortcutIconPath,
      string targetApplication,
      string paramsString,
      string description,
      string package = "")
    {
      ShortcutHelper.CreateShortcut(Path.Combine(ShortcutHelper.CommonStartMenuPath, shortcutName), shortcutIconPath, targetApplication, paramsString, description, package);
    }

    public static void CreateDesktopShortcut(
      string shortcutName,
      string shortcutIconPath,
      string targetApplication,
      string paramsString,
      string description,
      string package = "")
    {
      ShortcutHelper.CreateShortcut(Path.Combine(ShortcutHelper.sDesktopPath, shortcutName), shortcutIconPath, targetApplication, paramsString, description, package);
    }

    public static void CreateStartMenuShortcut(
      string shortcutName,
      string shortcutIconPath,
      string targetApplication,
      string paramsString,
      string description)
    {
      ShortcutHelper.CreateShortcut(Path.Combine(ShortcutHelper.CommonStartMenuPath, shortcutName), shortcutIconPath, targetApplication, paramsString, description, "");
    }

    public static void CreateShortcut(
      string shortcutPath,
      string shortcutIconPath,
      string targetApplication,
      string paramsString,
      string description,
      string package = "")
    {
      try
      {
        package = package ?? string.Empty;
        shortcutPath = ShortcutHelper.FixFileName(shortcutPath, package);
        ShortcutHelper.DeleteFileIfExists(shortcutPath);
        ShortcutHelper.IShellLink shellLink = (ShortcutHelper.IShellLink) new ShortcutHelper.ShellLink();
        shellLink.SetDescription(description);
        shellLink.SetPath(targetApplication);
        shellLink.SetIconLocation(shortcutIconPath, 0);
        shellLink.SetArguments(paramsString);
        ((IPersistFile) shellLink).Save(shortcutPath, false);
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not create shortcut for " + shortcutPath + " . " + ex.ToString());
      }
    }

    public static string GetShortcutArguments(string shortcutPath)
    {
      try
      {
        ShortcutHelper.IShellLink shellLink;
        ((IPersistFile) (shellLink = (ShortcutHelper.IShellLink) new ShortcutHelper.ShellLink())).Load(shortcutPath, 0);
        StringBuilder stringBuilder = new StringBuilder(260);
        ShortcutHelper.WIN32_FIND_DATAW wiN32FindDataw = new ShortcutHelper.WIN32_FIND_DATAW();
        StringBuilder pszFile = stringBuilder;
        int capacity = stringBuilder.Capacity;
        ref ShortcutHelper.WIN32_FIND_DATAW local = ref wiN32FindDataw;
        shellLink.GetPath(pszFile, capacity, out local, 0);
        return stringBuilder.ToString().Trim();
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not get Shortcut target path. " + ex.ToString());
      }
      return string.Empty;
    }

    public static string GetShortcutIconLocation(string shortcutPath)
    {
      try
      {
        ShortcutHelper.IShellLink shellLink;
        ((IPersistFile) (shellLink = (ShortcutHelper.IShellLink) new ShortcutHelper.ShellLink())).Load(shortcutPath, 0);
        StringBuilder stringBuilder = new StringBuilder(260);
        StringBuilder pszIconPath = stringBuilder;
        int capacity = stringBuilder.Capacity;
        int num;
        ref int local = ref num;
        shellLink.GetIconLocation(pszIconPath, capacity, out local);
        return stringBuilder.ToString().Trim();
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not get Shortcut target path. " + ex.ToString());
      }
      return string.Empty;
    }

    public static void UpdateTargetPathAndIcon(string shortcutPath, string target, string iconPath)
    {
      try
      {
        ShortcutHelper.IShellLink shellLink;
        ((IPersistFile) (shellLink = (ShortcutHelper.IShellLink) new ShortcutHelper.ShellLink())).Load(shortcutPath, 0);
        shellLink.SetPath(target);
        shellLink.SetIconLocation(iconPath, 0);
        ((IPersistFile) shellLink).Save(shortcutPath, false);
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not get Shortcut target path. " + ex.ToString());
      }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WIN32_FIND_DATAW
    {
      public uint dwFileAttributes;
      public long ftCreationTime;
      public long ftLastAccessTime;
      public long ftLastWriteTime;
      public uint nFileSizeHigh;
      public uint nFileSizeLow;
      public uint dwReserved0;
      public uint dwReserved1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string cFileName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
      public string cAlternateFileName;
    }

    [Guid("00021401-0000-0000-C000-000000000046")]
    [ComImport]
    internal class ShellLink
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      public extern ShellLink();
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [ComImport]
    internal interface IShellLink
    {
      void GetPath(
        [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszFile,
        int cchMaxPath,
        out ShortcutHelper.WIN32_FIND_DATAW pfd,
        int fFlags);

      void GetIDList(out IntPtr ppidl);

      void SetIDList(IntPtr pidl);

      void GetDescription([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszName, int cchMaxName);

      void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

      void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszDir, int cchMaxPath);

      void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

      void GetArguments([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszArgs, int cchMaxPath);

      void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

      void GetHotkey(out short pwHotkey);

      void SetHotkey(short wHotkey);

      void GetShowCmd(out int piShowCmd);

      void SetShowCmd(int iShowCmd);

      void GetIconLocation([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

      void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

      void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

      void Resolve(IntPtr hwnd, int fFlags);

      void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
  }
}
