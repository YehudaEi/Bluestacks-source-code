// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VMCommand
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Player
{
  internal class VMCommand
  {
    private Random random = new Random();
    private StringBuilder outputBuffer = new StringBuilder();
    private StringBuilder errorBuffer = new StringBuilder();
    private const string NATIVE_DLL = "HD-VMCommand-Native.dll";
    private SafeFileHandle vmHandle;
    private uint unitId;
    private VMCommand.LineHandler userOutputHandler;
    private VMCommand.LineHandler userErrorHandler;

    [DllImport("HD-VMCommand-Native.dll", SetLastError = true)]
    private static extern SafeFileHandle CommandAttach(uint vmId, uint unitId);

    [DllImport("HD-VMCommand-Native.dll")]
    private static extern int CommandPing(SafeFileHandle vmHandle, uint unitId);

    [DllImport("HD-VMCommand-Native.dll")]
    private static extern int CommandRun(
      SafeFileHandle vmHandle,
      uint unitId,
      int argc,
      string[] argv,
      VMCommand.ChunkHandler outHandler,
      VMCommand.ChunkHandler errHandler,
      ref int exitCode);

    [DllImport("HD-VMCommand-Native.dll")]
    private static extern int CommandKill(SafeFileHandle vmHandle, uint unitId);

    public void Attach(string vmName)
    {
      uint vmId = MonitorLocator.Lookup(vmName);
      this.unitId = (uint) this.random.Next();
      this.vmHandle = VMCommand.CommandAttach(vmId, this.unitId);
      if (this.vmHandle.IsInvalid)
        throw new ApplicationException("Cannot attach to monitor: " + Marshal.GetLastWin32Error().ToString());
    }

    public void SetOutputHandler(VMCommand.LineHandler handler)
    {
      this.userOutputHandler = handler;
    }

    public void SetErrorHandler(VMCommand.LineHandler handler)
    {
      this.userErrorHandler = handler;
    }

    public int Run(string[] argv)
    {
      int exitCode = 0;
      int error1 = VMCommand.CommandPing(this.vmHandle, this.unitId);
      if (error1 != 0)
        throw new ApplicationException("Cannot ping VM", (Exception) new Win32Exception(error1));
      int error2 = VMCommand.CommandRun(this.vmHandle, this.unitId, argv.Length, argv, new VMCommand.ChunkHandler(this.OutputHandler), new VMCommand.ChunkHandler(this.ErrorHandler), ref exitCode);
      if (error2 != 0)
        throw new ApplicationException("Cannot run VM command", (Exception) new Win32Exception(error2));
      if (this.outputBuffer.Length > 0 && this.userOutputHandler != null)
        this.userOutputHandler(this.outputBuffer.ToString());
      if (this.errorBuffer.Length > 0 && this.userErrorHandler != null)
        this.userErrorHandler(this.errorBuffer.ToString());
      return exitCode;
    }

    private void OutputHandler(string chunk)
    {
      VMCommand.CommonHandler(chunk, this.outputBuffer, this.userOutputHandler);
    }

    private void ErrorHandler(string chunk)
    {
      VMCommand.CommonHandler(chunk, this.errorBuffer, this.userErrorHandler);
    }

    private static void CommonHandler(
      string chunk,
      StringBuilder sb,
      VMCommand.LineHandler handler)
    {
      sb.Append(chunk);
      string[] strArray = sb.ToString().Split('\n');
      if (strArray.Length < 2)
        return;
      for (int index = 0; index < strArray.Length - 1; ++index)
      {
        if (handler != null)
          handler(strArray[index]);
      }
      sb.Remove(0, sb.Length);
      sb.Append(strArray[strArray.Length - 1]);
    }

    public void Kill()
    {
      VMCommand.CommandKill(this.vmHandle, this.unitId);
    }

    public delegate void ChunkHandler(string chunk);

    public delegate void LineHandler(string line);
  }
}
