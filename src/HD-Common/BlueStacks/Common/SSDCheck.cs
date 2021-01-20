// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SSDCheck
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Common
{
  public static class SSDCheck
  {
    private const uint FILE_SHARE_READ = 1;
    private const uint FILE_SHARE_WRITE = 2;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 128;
    private const uint FILE_DEVICE_MASS_STORAGE = 45;
    private const uint IOCTL_STORAGE_BASE = 45;
    private const uint METHOD_BUFFERED = 0;
    private const uint FILE_ANY_ACCESS = 0;
    private const uint IOCTL_VOLUME_BASE = 86;
    private const uint StorageDeviceTrimEnabledProperty = 8;
    private const uint PropertyStandardQuery = 0;
    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 4096;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern SafeFileHandle CreateFileW(
      [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
      SafeFileHandle hDevice,
      uint dwIoControlCode,
      ref SSDCheck.STORAGE_PROPERTY_QUERY lpInBuffer,
      uint nInBufferSize,
      ref SSDCheck.DEVICE_TRIM_DESCRIPTOR lpOutBuffer,
      uint nOutBufferSize,
      out uint lpBytesReturned,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
      SafeFileHandle hDevice,
      uint dwIoControlCode,
      IntPtr lpInBuffer,
      uint nInBufferSize,
      ref SSDCheck.VOLUME_DISK_EXTENTS lpOutBuffer,
      uint nOutBufferSize,
      out uint lpBytesReturned,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint FormatMessage(
      uint dwFlags,
      IntPtr lpSource,
      uint dwMessageId,
      uint dwLanguageId,
      StringBuilder lpBuffer,
      uint nSize,
      IntPtr Arguments);

    private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
    {
      return (uint) ((int) DeviceType << 16 | (int) Access << 14 | (int) Function << 2) | Method;
    }

    public static bool IsMediaTypeSSD(string path)
    {
      try
      {
        string pathRoot = Path.GetPathRoot(path);
        Logger.Info("Checking if media type ssd for drive: " + pathRoot);
        string str = pathRoot.TrimEnd('\\').TrimEnd(':');
        if (str.Length <= 1)
          return SSDCheck.HasTrimEnabled("\\\\.\\PhysicalDrive" + SSDCheck.GetDiskExtents(str.ToCharArray()[0]).ToString());
        Logger.Info("Invalid drive letter " + str + ". returning!!");
        return false;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to find if media is ssd. Ex : " + ex.ToString());
      }
      return false;
    }

    private static bool HasTrimEnabled(string drive)
    {
      Logger.Info("Checking trim enabled for drive: " + drive);
      SafeFileHandle fileW = SSDCheck.CreateFileW(drive, 0U, 3U, IntPtr.Zero, 3U, 128U, IntPtr.Zero);
      if (fileW == null || fileW.IsInvalid)
      {
        Logger.Error("CreateFile failed with message: " + SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error()));
        throw new Exception("check the error message above");
      }
      uint dwIoControlCode = SSDCheck.CTL_CODE(45U, 1280U, 0U, 0U);
      SSDCheck.STORAGE_PROPERTY_QUERY lpInBuffer = new SSDCheck.STORAGE_PROPERTY_QUERY()
      {
        PropertyId = 8,
        QueryType = 0
      };
      SSDCheck.DEVICE_TRIM_DESCRIPTOR lpOutBuffer = new SSDCheck.DEVICE_TRIM_DESCRIPTOR();
      int num = SSDCheck.DeviceIoControl(fileW, dwIoControlCode, ref lpInBuffer, (uint) Marshal.SizeOf((object) lpInBuffer), ref lpOutBuffer, (uint) Marshal.SizeOf((object) lpOutBuffer), out uint _, IntPtr.Zero) ? 1 : 0;
      fileW?.Close();
      if (num == 0)
      {
        Logger.Error("DeviceIoControl failed to query trim enabled. " + SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error()));
        throw new Exception("check the error message above");
      }
      bool trimEnabled = lpOutBuffer.TrimEnabled;
      Logger.Info(string.Format("Is Trim Enabled: {0}", (object) trimEnabled));
      return trimEnabled;
    }

    private static uint GetDiskExtents(char cDrive)
    {
      if (new DriveInfo(cDrive.ToString((IFormatProvider) CultureInfo.InvariantCulture)).DriveType != DriveType.Fixed)
        Logger.Info(string.Format("The drive {0} is not fixed drive.", (object) cDrive));
      string lpFileName = "\\\\.\\" + cDrive.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ":";
      SafeFileHandle fileW = SSDCheck.CreateFileW(lpFileName, 0U, 3U, IntPtr.Zero, 3U, 128U, IntPtr.Zero);
      if (fileW == null || fileW.IsInvalid)
      {
        string errorMessage = SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error());
        Logger.Error("CreateFile failed for " + lpFileName + ".  " + errorMessage);
        throw new Exception("check the error message above");
      }
      uint dwIoControlCode = SSDCheck.CTL_CODE(86U, 0U, 0U, 0U);
      SSDCheck.VOLUME_DISK_EXTENTS lpOutBuffer = new SSDCheck.VOLUME_DISK_EXTENTS();
      int num = SSDCheck.DeviceIoControl(fileW, dwIoControlCode, IntPtr.Zero, 0U, ref lpOutBuffer, (uint) Marshal.SizeOf((object) lpOutBuffer), out uint _, IntPtr.Zero) ? 1 : 0;
      fileW?.Close();
      if (num == 0 || lpOutBuffer.Extents.Length != 1)
      {
        Logger.Error("DeviceIoControl failed to query disk extension. " + SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error()));
        throw new Exception("check the error message above");
      }
      uint diskNumber = lpOutBuffer.Extents[0].DiskNumber;
      Logger.Info(string.Format("The physical drive number is: {0}", (object) diskNumber));
      return diskNumber;
    }

    private static string GetErrorMessage(int code)
    {
      StringBuilder lpBuffer = new StringBuilder((int) byte.MaxValue);
      int num = (int) SSDCheck.FormatMessage(4096U, IntPtr.Zero, (uint) code, 0U, lpBuffer, (uint) lpBuffer.Capacity, IntPtr.Zero);
      return lpBuffer.ToString();
    }

    private struct STORAGE_PROPERTY_QUERY
    {
      public uint PropertyId;
      public uint QueryType;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
      public byte[] AdditionalParameters;
    }

    private struct DEVICE_TRIM_DESCRIPTOR
    {
      public uint Version;
      public uint Size;
      [MarshalAs(UnmanagedType.U1)]
      public bool TrimEnabled;
    }

    private struct DISK_EXTENT
    {
      public uint DiskNumber;
      public long StartingOffset;
      public long ExtentLength;
    }

    private struct VOLUME_DISK_EXTENTS
    {
      public uint NumberOfDiskExtents;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
      public SSDCheck.DISK_EXTENT[] Extents;
    }
  }
}
