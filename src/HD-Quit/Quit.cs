// Decompiled with JetBrains decompiler
// Type: BlueStacks.Quit.Quit
// Assembly: HD-Quit, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 6D7605E4-5739-4FE8-92A7-85632FD37BD3
// Assembly location: C:\Program Files\BlueStacks\HD-Quit.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Quit
{
  public class Quit
  {
    private static string BlueStacksServicesPrefix = "BstHd";
    private static string BlueStacksServicesPlusPrefix = "BstkDrv";
    public static bool sIgnoreAgent = false;
    public static bool sKillService = false;
    public static bool sIsFromClient = false;

    [STAThread]
    public static void Main(string[] args)
    {
      BlueStacks.Quit.Quit.Init();
      BlueStacks.Quit.Quit.Opt opt = new BlueStacks.Quit.Quit.Opt();
      opt.Parse(args);
      BlueStacks.Quit.Quit.sIgnoreAgent = opt.ignoreAgent;
      BlueStacks.Quit.Quit.sIsFromClient = opt.isFromClient;
      ProcessUtils.LogParentProcessDetails();
      BlueStacks.Quit.Quit.QuitAll();
    }

    private static void Init()
    {
      Logger.InitUserLog();
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        if (Oem.Instance.IsMessageBoxToBeDisplayed)
        {
          int num = (int) MessageBox.Show("Caught unhandled exception. Please check the log for details.", string.Format("{0} Quit Utility", (object) Strings.ProductDisplayName));
        }
        Environment.Exit(1);
      });
    }

    private static void QuitAll()
    {
      Logger.Info("Quit: Killing all processes");
      if (BlueStacks.Quit.Quit.sKillService)
      {
        Logger.Info("Unloading driver " + Strings.BlueStacksDriverName);
        ServiceManager.StopService(Strings.BlueStacksDriverName, true);
      }
      if (!BlueStacks.Quit.Quit.sIsFromClient)
      {
        Utils.KillCurrentOemProcessByName("Bluestacks", (string) null);
        Utils.StopFrontend(string.Empty, true);
      }
      Utils.KillCurrentOemProcessByName(new string[8]
      {
        "HD-ApkHandler",
        "HD-Adb",
        "HD-RunApp",
        "HD-Updater",
        "BluestacksTV",
        "HD-OBS",
        "HD-Troubleshooter",
        "HD-XapkHandler"
      }, (string) null);
      if (!BlueStacks.Quit.Quit.sIgnoreAgent)
      {
        Logger.Info("Killing Agent");
        try
        {
          Logger.Info("Removing agent icon from system tray");
          HTTPUtils.SendRequestToAgent("sysTrayVisibility", new Dictionary<string, string>()
          {
            {
              "visible",
              "false"
            }
          }, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", false);
        }
        catch (Exception ex)
        {
          Logger.Error("Unable to remove agent icon from system tray", (object) ex);
        }
        Utils.KillCurrentOemProcessByName("HD-Agent", (string) null);
      }
      if (BlueStacks.Quit.Quit.sKillService)
        Utils.KillComServer();
      Logger.Info("Quit: Killed all processes. Exiting BlueStacks");
      Application.Exit();
    }

    public class Opt : GetOpt
    {
      public bool ignoreAgent;
      public bool isFromClient;
    }
  }
}
