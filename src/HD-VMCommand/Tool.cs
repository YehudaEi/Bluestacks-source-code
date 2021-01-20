// Decompiled with JetBrains decompiler
// Type: BlueStacks.VMCommand.Tool
// Assembly: HD-VMCommand, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 9C835E8E-0893-41DA-9DD5-39573EAF8B50
// Assembly location: C:\Program Files\BlueStacks\HD-VMCommand.dll

using BlueStacks.Common;
using System;
using System.Diagnostics;

namespace BlueStacks.VMCommand
{
  public class Tool
  {
    public static void Main(string[] args)
    {
      if (args.Length < 2)
        Tool.Usage();
      string vmName = args[0];
      string[] argv = new string[args.Length - 1];
      Array.Copy((Array) args, 1, (Array) argv, 0, args.Length - 1);
      Command cmd = new Command();
      cmd.Attach(vmName);
      cmd.SetOutputHandler((Command.LineHandler) (line => Console.WriteLine(line)));
      cmd.SetErrorHandler((Command.LineHandler) (line => Console.Error.WriteLine(line)));
      ConsoleControl.SetHandler((ConsoleControl.Handler) (ctrl =>
      {
        cmd.Kill();
        return true;
      }));
      try
      {
        Environment.Exit(cmd.Run(argv));
      }
      catch (Exception ex)
      {
        string str = ex.Message;
        if (ex.InnerException != null)
          str = str + " --> " + ex.InnerException.Message;
        Console.WriteLine("Cannot run VM command: " + str);
        Environment.Exit(1);
      }
    }

    private static void Usage()
    {
      Console.Error.WriteLine("Usage: {0} <vm name> <cmd> [args ...]", (object) Process.GetCurrentProcess().ProcessName);
      Environment.Exit(1);
    }
  }
}
