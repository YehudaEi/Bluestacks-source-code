// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MemoryMappedFile
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public static class MemoryMappedFile
  {
    private const uint STANDARD_RIGHTS_REQUIRED = 983040;
    private const uint SECTION_QUERY = 1;
    private const uint SECTION_MAP_WRITE = 2;
    private const uint SECTION_MAP_READ = 4;
    private const uint SECTION_MAP_EXECUTE = 8;
    private const uint SECTION_EXTEND_SIZE = 16;
    private const uint SECTION_ALL_ACCESS = 983071;
    private const uint FILE_MAP_ALL_ACCESS = 983071;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr OpenFileMapping(
      uint dwDesiredAccess,
      bool bInheritHandle,
      string lpName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr MapViewOfFile(
      IntPtr hFileMappingObject,
      uint dwDesiredAccess,
      uint dwFileOffsetHigh,
      uint dwFileOffsetLow,
      UIntPtr dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    public static int GetNCSoftAgentPort(string SharedMemoryName, uint NumBytes)
    {
      IntPtr num1 = MemoryMappedFile.OpenFileMapping(983071U, false, SharedMemoryName);
      if (IntPtr.Zero == num1)
      {
        Logger.Error("Shared Memory Handle not found. Last Error : " + Marshal.GetLastWin32Error().ToString());
        return -1;
      }
      IntPtr ptr = MemoryMappedFile.MapViewOfFile(num1, 983071U, 0U, 0U, new UIntPtr(NumBytes));
      if (ptr == IntPtr.Zero)
      {
        Logger.Error("Cannot map view of file. Last Error : " + Marshal.GetLastWin32Error().ToString());
        return -1;
      }
      int num2 = -1;
      try
      {
        num2 = Marshal.ReadInt32(ptr);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to read memory as int32");
        Logger.Error(ex.ToString());
      }
      if (IntPtr.Zero != num1)
      {
        MemoryMappedFile.CloseHandle(num1);
        IntPtr zero = IntPtr.Zero;
      }
      IntPtr zero1 = IntPtr.Zero;
      return num2;
    }
  }
}
