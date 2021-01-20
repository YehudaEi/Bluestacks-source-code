// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GetProcessExecutionPath
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Common
{
  public static class GetProcessExecutionPath
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool QueryFullProcessImageName(
      IntPtr hwnd,
      int flags,
      [Out] StringBuilder buffer,
      out int size);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr OpenProcess(int flags, bool handle, UIntPtr procId);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    public static List<string> GetApplicationPath(Process[] procList)
    {
      List<string> stringList = new List<string>();
      if (procList != null)
      {
        foreach (Process proc in procList)
        {
          try
          {
            string applicationPathFromProcess = GetProcessExecutionPath.GetApplicationPathFromProcess(proc);
            if (!string.IsNullOrEmpty(applicationPathFromProcess))
              stringList.Add(applicationPathFromProcess);
          }
          catch (Exception ex)
          {
          }
        }
      }
      return stringList;
    }

    public static string GetApplicationPathFromProcess(Process proc)
    {
      try
      {
        if (!SystemUtils.IsOSWinXP())
          return GetProcessExecutionPath.GetExecutablePathAboveVista(new UIntPtr((uint) proc?.Id.Value));
        if (SystemUtils.IsAdministrator())
          return proc?.MainModule.FileName.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch
      {
      }
      return string.Empty;
    }

    public static string GetExecutablePathAboveVista(UIntPtr dwProcessId)
    {
      StringBuilder buffer = new StringBuilder(1024);
      IntPtr num = GetProcessExecutionPath.OpenProcess(4096, false, dwProcessId);
      if (num != IntPtr.Zero)
      {
        try
        {
          int size = buffer.Capacity;
          if (GetProcessExecutionPath.QueryFullProcessImageName(num, 0, buffer, out size))
            return buffer.ToString();
        }
        finally
        {
          GetProcessExecutionPath.CloseHandle(num);
        }
      }
      return string.Empty;
    }
  }
}
