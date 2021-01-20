// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Manager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  public class Manager
  {
    private IntPtr handle = IntPtr.Zero;

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr handle);

    private Manager(IntPtr handle)
    {
      this.handle = handle;
    }

    private Manager()
    {
    }

    public static Manager Open()
    {
      return new Manager();
    }

    public Monitor Attach(uint id, Monitor.ExitHandler exitHandler)
    {
      if (!HDPlusModule.ManagerAttach(this.handle, id))
        CommonError.ThrowLastWin32Error("Cannot attach to monitor " + id.ToString());
      return new Monitor(this.handle, id, exitHandler);
    }

    public Monitor Attach(uint id, bool verbose)
    {
      return new Monitor(id, verbose);
    }

    public Monitor Attach(uint id, bool verbose, bool isMonAttach)
    {
      return isMonAttach ? new Monitor(id, verbose) : new Monitor(id);
    }
  }
}
