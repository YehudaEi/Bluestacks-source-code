// Decompiled with JetBrains decompiler
// Type: BlueStacks.ComRegistrar.ComRegistrar
// Assembly: HD-ComRegistrar, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: E05F62B1-3170-42C6-BFA0-DC982106896F
// Assembly location: C:\Program Files\BlueStacks\HD-ComRegistrar.exe

using BlueStacks.Common;
using System;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.ComRegistrar
{
  public class ComRegistrar
  {
    private static Mutex sLock = (Mutex) null;
    internal static RegComOpt sOpt = new RegComOpt();

    [STAThread]
    public static void Main(string[] args)
    {
      BlueStacks.ComRegistrar.ComRegistrar.Init();
      BlueStacks.ComRegistrar.ComRegistrar.sOpt.Parse(args);
      if (args.Length != 1 || !BlueStacks.ComRegistrar.ComRegistrar.sOpt.reg && !BlueStacks.ComRegistrar.ComRegistrar.sOpt.unreg)
      {
        Logger.Error("No/invalid arguments given. Exiting");
        Environment.Exit(2);
      }
      if (!SystemUtils.IsAdministrator())
      {
        Logger.Fatal("Not admin, exiting");
        Environment.Exit(4);
      }
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_UnRegRegCom_Lockbgp", out BlueStacks.ComRegistrar.ComRegistrar.sLock))
      {
        Logger.Info("Another instance of ComRegistrar is already running");
        Environment.Exit(1);
      }
      if (args.Length != 0)
        Logger.Info("args: {0}", (object) string.Join(" ", args));
      if (BlueStacks.ComRegistrar.ComRegistrar.sOpt.reg)
      {
        Logger.Info("Validating oleaut registries");
        if (FixUpOle.FixOle((string) null))
          Logger.Info("Oleaut Validation successful");
        else
          Logger.Info("Oleaut Validation failed, continuing anyway");
      }
      if (BlueStacks.ComRegistrar.ComRegistrar.sOpt.reg)
      {
        ulong num = RegisterProxyStub.DllRegisterServer();
        if (num != 0UL)
        {
          MultiInstanceUtils.SetDeviceCapsRegistry(string.Format("Registration error: {0}", (object) num), EngineState.legacy.ToString());
          Logger.Fatal("Unable to register BstkProxyStub.dll, Error: {0}", (object) num);
          Environment.Exit(10);
        }
      }
      else if (BlueStacks.ComRegistrar.ComRegistrar.sOpt.unreg)
      {
        Logger.Info("Unregistering components");
        long num = (long) RegisterProxyStub.DllUnregisterServer();
      }
      Environment.Exit(0);
    }

    private static void Init()
    {
      Logger.InitUserLog();
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(3);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(3);
      });
      ProcessUtils.LogProcessContextDetails();
      ProcessUtils.LogParentProcessDetails();
    }
  }
}
