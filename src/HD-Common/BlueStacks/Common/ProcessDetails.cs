// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ProcessDetails
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public static class ProcessDetails
  {
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CreateToolhelp32Snapshot([In] uint dwFlags, [In] uint th32ProcessID);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool Process32First(
      [In] IntPtr hSnapshot,
      ref ProcessDetails.PROCESSENTRY32 lppe);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool Process32Next(
      [In] IntPtr hSnapshot,
      ref ProcessDetails.PROCESSENTRY32 lppe);

    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle([In] IntPtr hObject);

    public static int? GetParentProcessId(int pid)
    {
      return ProcessDetails.GetParentProcess(pid)?.Id;
    }

    public static Process GetParentProcess(int pid)
    {
      Process process = (Process) null;
      int id = Process.GetCurrentProcess().Id;
      IntPtr num = IntPtr.Zero;
      try
      {
        ProcessDetails.PROCESSENTRY32 lppe = new ProcessDetails.PROCESSENTRY32()
        {
          dwSize = (uint) Marshal.SizeOf(typeof (ProcessDetails.PROCESSENTRY32))
        };
        num = ProcessDetails.CreateToolhelp32Snapshot(2U, 0U);
        if (ProcessDetails.Process32First(num, ref lppe))
        {
          while ((long) pid != (long) lppe.th32ProcessID)
          {
            if (!ProcessDetails.Process32Next(num, ref lppe))
              goto label_8;
          }
          process = Process.GetProcessById((int) lppe.th32ParentProcessID);
        }
        else
          throw new ApplicationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed with win32 error code {0}", (object) Marshal.GetLastWin32Error()));
      }
      catch (Exception ex)
      {
        Logger.Error("Can't get the process. Ex: {0}", (object) ex.ToString());
      }
      finally
      {
        ProcessDetails.CloseHandle(num);
      }
label_8:
      return process;
    }

    public static string CurrentProcessParentFileName
    {
      get
      {
        return Path.GetFileName(ProcessDetails.CurrentProcessParentFullPath);
      }
    }

    public static string CurrentProcessParentFullPath
    {
      get
      {
        return ProcessDetails.CurrentProcessParent.MainModule.FileName;
      }
    }

    public static Process CurrentProcessParent
    {
      get
      {
        return ProcessDetails.GetParentProcess(Process.GetCurrentProcess().Id);
      }
    }

    public static int? CurrentProcessParentId
    {
      get
      {
        return ProcessDetails.GetParentProcessId(Process.GetCurrentProcess().Id);
      }
    }

    public static int CurrentProcessId
    {
      get
      {
        return Process.GetCurrentProcess().Id;
      }
    }

    public static int? GetNthParentPid(int pid, int order)
    {
      int? nullable;
      for (nullable = new int?(pid); order > 0 && nullable.HasValue; --order)
        nullable = ProcessDetails.GetParentProcessId(nullable.Value);
      return nullable;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct PROCESSENTRY32
    {
      private const int MAX_PATH = 260;
      internal uint dwSize;
      internal uint cntUsage;
      internal uint th32ProcessID;
      internal IntPtr th32DefaultHeapID;
      internal uint th32ModuleID;
      internal uint cntThreads;
      internal uint th32ParentProcessID;
      internal int pcPriClassBase;
      internal uint dwFlags;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      internal string szExeFile;
    }

    [Flags]
    private enum SnapshotFlags : uint
    {
      HeapList = 1,
      Process = 2,
      Thread = 4,
      Module = 8,
      Module32 = 16, // 0x00000010
      Inherit = 2147483648, // 0x80000000
      All = Module32 | Module | Thread | Process | HeapList, // 0x0000001F
      NoHeaps = 1073741824, // 0x40000000
    }
  }
}
