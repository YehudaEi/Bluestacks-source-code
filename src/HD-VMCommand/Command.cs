// Decompiled with JetBrains decompiler
// Type: BlueStacks.VMCommand.Command
// Assembly: HD-VMCommand, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 9C835E8E-0893-41DA-9DD5-39573EAF8B50
// Assembly location: C:\Program Files\BlueStacks\HD-VMCommand.dll

using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.VMCommand
{
  public class Command
  {
    private Random random = new Random();
    private StringBuilder outputBuffer = new StringBuilder();
    private StringBuilder errorBuffer = new StringBuilder();
    private const string NATIVE_DLL = "HD-VMCommand-Native.dll";
    private SafeFileHandle vmHandle;
    private uint unitId;
    private Command.LineHandler userOutputHandler;
    private Command.LineHandler userErrorHandler;

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
      Command.ChunkHandler outHandler,
      Command.ChunkHandler errHandler,
      ref int exitCode);

    [DllImport("HD-VMCommand-Native.dll")]
    private static extern int CommandKill(SafeFileHandle vmHandle, uint unitId);

    public void Attach(string vmName)
    {
      uint vmId = MonitorLocator.Lookup(vmName);
      this.unitId = (uint) this.random.Next();
      this.vmHandle = Command.CommandAttach(vmId, this.unitId);
      if (this.vmHandle.IsInvalid)
        throw new ApplicationException("Cannot attach to monitor: " + Marshal.GetLastWin32Error().ToString());
    }

    public void SetOutputHandler(Command.LineHandler handler)
    {
      this.userOutputHandler = handler;
    }

    public void SetErrorHandler(Command.LineHandler handler)
    {
      this.userErrorHandler = handler;
    }

    public int Run(string[] argv)
    {
      int exitCode = 0;
      int error1 = Command.CommandPing(this.vmHandle, this.unitId);
      if (error1 != 0)
        throw new ApplicationException("Cannot ping VM", (Exception) new Win32Exception(error1));
      int error2 = Command.CommandRun(this.vmHandle, this.unitId, argv.Length, argv, new Command.ChunkHandler(this.OutputHandler), new Command.ChunkHandler(this.ErrorHandler), ref exitCode);
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
      Command.CommonHandler(chunk, this.outputBuffer, this.userOutputHandler);
    }

    private void ErrorHandler(string chunk)
    {
      Command.CommonHandler(chunk, this.errorBuffer, this.userErrorHandler);
    }

    private static void CommonHandler(string chunk, StringBuilder sb, Command.LineHandler handler)
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
      Command.CommandKill(this.vmHandle, this.unitId);
    }

    public delegate void ChunkHandler(string chunk);

    public delegate void LineHandler(string line);
  }
}
