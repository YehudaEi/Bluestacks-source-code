// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Program
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public static class Program
  {
    private static Thread UiThread;
    public static Mutex sFrontendLock;

    [DllImport("HD-Plus-Devices.dll")]
    private static extern int BstPlusDevicesInit();

    [DllImport("HD-Plus-Devices.dll")]
    private static extern int BstSetComputedGuid(string systemGuid);

    [DllImport("HD-Audio-Native.dll")]
    private static extern void InitAudioLogger(Logger.HdLoggerCallback cb);

    private static void HandleEndpointPropertiesImpl([MarshalAs(UnmanagedType.LPWStr), In] string endPointProperties)
    {
      Logger.Info("Callback HandleEndpointPropertiesImpl");
      Logger.Debug("Endpoint props buffer: " + endPointProperties);
      try
      {
        JObject jsonObj = JObject.Parse(endPointProperties);
        Program.UpdateIPAddressInBootParams(jsonObj);
        Program.UpdatePorts(jsonObj);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't parse jsonobj: {0}", (object) endPointProperties);
        Logger.Error(ex.ToString());
      }
    }

    private static void UpdatePorts(JObject jsonObj)
    {
      List<string> stringList = new List<string>();
      try
      {
        foreach (JObject jobject in JArray.Parse(jsonObj["Policies"].ToString()))
        {
          try
          {
            string str1 = jobject["ExternalPort"].ToString().Trim();
            string str2 = jobject["InternalPort"].ToString().Trim();
            string str3 = jobject["Protocol"].ToString().Trim();
            Logger.Info("{0} --> {1}. Protocol: {2}", (object) str2, (object) str1, (object) str3);
            string format = "{0}:{1}:{2}";
            stringList.Add(string.Format(format, (object) str3.ToLower(), (object) str2, (object) str1));
            if (str2 != null)
            {
              if (!(str2 == "5555"))
              {
                if (str2 == "9999")
                  RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAndroidPort = Convert.ToInt32(str1);
              }
              else
                RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAdbPort = Convert.ToInt32(str1);
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Some error while assigning ports. Ex: {0}", (object) ex);
          }
        }
        if (stringList.Count > 0)
        {
          Logger.Info("Updating network inbound rules");
          RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].NetworkInboundRules = stringList.ToArray();
        }
        Logger.Debug("Inbound rules for {0} are: {1}", (object) MultiInstanceStrings.VmName, (object) string.Join(",", RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].NetworkInboundRules));
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while getting policies. Ex: {0}", (object) ex);
      }
    }

    private static void UpdateIPAddressInBootParams(JObject jsonObj)
    {
      string str = "";
      try
      {
        str = jsonObj["IPAddress"].ToString().Trim();
        Logger.Info("Updating guest ip to {0}", (object) str);
        Utils.UpdateValueInBootParams("GUEST_IP", str, MultiInstanceStrings.VmName, true, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't update Guest IP: {0}", (object) str);
        Logger.Error(ex.ToString());
      }
    }

    private static void AddSharedFoldersInBootParams(JObject jObject)
    {
      List<string> stringList = new List<string>();
      foreach (JToken jtoken in (JArray) jObject["VirtualMachine"][(object) "Devices"][(object) "Plan9"][(object) "Shares"])
        stringList.Add(jtoken[(object) "Name"].ToString());
      RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("SF", string.Join(",", stringList.ToArray()), RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
    }

    [MTAThread]
    private static void Main(string[] args)
    {
      Program.InitLog();
      Strings.CurrentDefaultVmName = args[0];
      MultiInstanceStrings.VmName = args[0];
      Program.UiThread = new Thread((ThreadStart) (() => Program.UIMain(args)));
      Program.UiThread.SetApartmentState(ApartmentState.STA);
      Program.UiThread.Start();
    }

    private static void SetupAndRunApplication(Opt opt)
    {
      Program.CheckIfAlreadyRunning(Opt.Instance.h);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if (!VMWindow.CheckAndroidFilesIntegrity())
      {
        Logger.Error("Android File Integrity check failed");
        Environment.Exit(-2);
      }
      TimelineStatsSender.HandleEngineBootEvent(EngineStatsEvent.android_file_integrity_passed.ToString());
      InputMapper.RegisterGuestBootLogsHandler();
      AndroidService.StartAsync();
      HTTPHandler.StartServer();
      Program.InitAudioLogger(Logger.GetHdLoggerCallback());
      VMWindow vmWindow = new VMWindow(opt.h, opt.w);
      Application.Run();
    }

    [STAThread]
    public static void UIMain(string[] args)
    {
      ProcessUtils.LogParentProcessDetails();
      TimelineStatsSender.Init(MultiInstanceStrings.VmName);
      TimelineStatsSender.HandleEngineBootEvent(EngineStatsEvent.player_launched.ToString());
      Program.BstPlusDevicesInit();
      Opt.Instance.Parse(args);
      if (Opt.Instance.help)
        Program.Usage();
      Stats.SendFrontendStatusUpdate("frontend-launched", MultiInstanceStrings.VmName);
      if (!MultiInstanceUtils.VerifyVmId(MultiInstanceStrings.VmName))
      {
        Logger.Error("VmName {0} , not part of VmList {1} , Exiting Process", (object) MultiInstanceStrings.VmName, (object) RegistryManager.Instance.VmList.ToString());
        Environment.Exit(1);
      }
      Logger.InitVmInstanceName(MultiInstanceStrings.VmName);
      InputManagerProxy.SetUp();
      if (HyperV.Instance.HyperVStatus == HyperV.ReturnCodes.MicrosoftHyperV)
      {
        Logger.Error("Hyper-V enabled for non Hyper-V build, exiting");
        Environment.Exit(-5);
      }
      else
      {
        if (HyperV.Instance.HyperVStatus != HyperV.ReturnCodes.None)
          Logger.Info("Non-microsoft Hyper-V may be active, continuing");
        Program.SetupAndRunApplication(Opt.Instance);
      }
    }

    private static void CheckIfAlreadyRunning(bool hideMode)
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock(Strings.GetPlayerLockName(MultiInstanceStrings.VmName, "bgp"), out Program.sFrontendLock))
        return;
      Logger.Info("Frontend already running");
      IntPtr zero = IntPtr.Zero;
      if (!hideMode)
      {
        IntPtr front = Program.BringToFront(MultiInstanceStrings.VmName);
        if (front != IntPtr.Zero)
        {
          Logger.Info("Sending WM_USER_SHOW_WINDOW to Frontend handle {0}", (object) front);
          InteropWindow.SendMessage(front, 1025U, IntPtr.Zero, IntPtr.Zero);
        }
        else
        {
          Dictionary<string, string> data = new Dictionary<string, string>();
          try
          {
            Logger.Info("ShowWindow result: " + HTTPUtils.SendRequestToEngine("showWindow", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp"));
          }
          catch (Exception ex)
          {
            Logger.Warning("Exception in ShowWindow request failed. Err: {0}", (object) ex.Message);
          }
        }
      }
      Environment.Exit(0);
    }

    private static IntPtr BringToFront(string vmname)
    {
      Logger.Info(string.Format("Starting BlueStacks {0} Frontend", (object) vmname));
      string name = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
      IntPtr num = IntPtr.Zero;
      try
      {
        num = InteropWindow.BringWindowToFront(name, false, false);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in bringing existing frontend window for VM {0} to the foreground", (object) (vmname + " Err : " + ex.ToString()));
      }
      return num;
    }

    private static void Usage()
    {
      string processName = Process.GetCurrentProcess().ProcessName;
      string caption = string.Format("{0} Frontend", (object) Strings.ProductDisplayName);
      string text = "" + string.Format("Usage:\n") + string.Format("    {0} [OPTION] \n", (object) processName) + string.Format("  -vmname:vmname   Specify the vmId for which to run HD-Player, default value is Android");
      if (Oem.Instance.IsMessageBoxToBeDisplayed)
      {
        Logger.Info("Displaying Message box");
        int num = (int) MessageBox.Show(text, caption);
        Logger.Info("Displayed Message box");
      }
      Environment.Exit(1);
    }

    private static void InitLog()
    {
      Logger.InitLog("Player", "Player", true);
      Logger.Info("BOOT_STAGE: Player starting");
      Console.SetOut(Logger.GetWriter());
      Console.SetError(Logger.GetWriter());
      AppDomain.CurrentDomain.ProcessExit += new EventHandler(Program.CurrentDomain_ProcessExit);
      Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
    }

    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      Logger.Info("Exiting frontend PID {0}", (object) Process.GetCurrentProcess().Id);
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Logger.Error("Unhandled Exception:");
      Logger.Error(e.ExceptionObject.ToString());
      Environment.Exit(-3);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.Error("Unhandled Exception:");
      Logger.Error(e.Exception.ToString());
      Environment.Exit(-3);
    }
  }
}
