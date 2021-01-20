// Decompiled with JetBrains decompiler
// Type: BlueStacks.RunApp.newRunApp
// Assembly: HD-RunApp, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 014B0069-3213-44F0-90E8-01E3DB2F6392
// Assembly location: C:\Program Files\BlueStacks\HD-RunApp.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.RunApp
{
  public class newRunApp
  {
    private static string s_AppName = "";
    private static string s_AppIcon = "";
    private static string s_AppPackage = "";
    private static string s_AppActivity = "";
    private static string s_ApkUrl = "";
    private static bool s_HideMode = false;
    private static string s_CampaignId = "";
    private static string s_SourceVersion = "";
    private static string s_Oem = "";
    private static string s_CampaignHash = "";
    private static Dictionary<string, string> data = new Dictionary<string, string>();
    private static int s_CampaignAbiValue = 15;
    private static bool s_AddFlePkg = false;
    private static int s_AgentPort;

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    public static int Main(string[] args)
    {
      newRunApp.Opt opt = new newRunApp.Opt();
      opt.Parse(args);
      newRunApp.s_AppName = opt.name;
      newRunApp.s_AppPackage = opt.p;
      newRunApp.s_AppActivity = opt.a;
      newRunApp.s_ApkUrl = opt.url;
      newRunApp.s_HideMode = opt.h;
      newRunApp.s_CampaignId = opt.campaign_id;
      newRunApp.s_SourceVersion = opt.source_version;
      BlueStacks.Common.Strings.CurrentDefaultVmName = opt.vmname;
      newRunApp.s_Oem = opt.oem;
      newRunApp.s_CampaignHash = opt.campaignhash;
      newRunApp.s_AddFlePkg = opt.addflepkg;
      Logger.InitUserLog();
      InstalledOem.SetAllInstalledOems();
      for (int index = 0; index < args.Length; ++index)
        Logger.Info("CMD: arg{0}: {1}", (object) index, (object) args[index]);
      newRunApp.InitExceptionHandlers();
      string runningProcName;
      if (!opt.force && ProcessUtils.IsAnyInstallerProcesRunning(out runningProcName) && !string.IsNullOrEmpty(runningProcName))
      {
        Logger.Error(runningProcName + " process is running. Exiting BlueStacks Runapp.");
        Environment.Exit(-1);
      }
      RegistryManager.Instance.DefaultGuest.RunAppProcessId = Process.GetCurrentProcess().Id;
      if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(BlueStacks.Common.Strings.CurrentDefaultVmName))
      {
        Logger.Info("VM Name: " + BlueStacks.Common.Strings.CurrentDefaultVmName + " does not exist");
        Environment.Exit(-8);
      }
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(newRunApp.ValidateRemoteCertificate);
      if (string.IsNullOrEmpty(opt.Json))
        opt.Json = new JObject()
        {
          {
            "app_icon_url",
            (JToken) ""
          },
          {
            "app_name",
            (JToken) newRunApp.s_AppName
          },
          {
            "app_url",
            (JToken) ""
          },
          {
            "app_pkg",
            (JToken) newRunApp.s_AppPackage
          },
          {
            "campaign_id",
            (JToken) newRunApp.s_CampaignId
          },
          {
            "source_version",
            (JToken) newRunApp.s_SourceVersion
          }
        }.ToString(Formatting.None);
      if (Oem.Instance.IsGameManagerToBeStartedOnRunApp)
      {
        if (!string.IsNullOrEmpty(opt.Json))
        {
          if (newRunApp.s_AddFlePkg && !newRunApp.CheckAndSetVmName())
          {
            JObject jobject = JObject.Parse(opt.Json);
            string str = jobject["app_pkg"].ToString();
            jobject.Add("check_fle_pkg", (JToken) str);
            opt.Json = jobject.ToString(Formatting.None);
          }
          newRunApp.StartGameManager(string.Empty, string.Empty, opt.Json);
        }
        else if (!string.IsNullOrEmpty(newRunApp.s_AppPackage) && !string.IsNullOrEmpty(newRunApp.s_AppActivity))
          newRunApp.StartGameManager(newRunApp.s_AppPackage, newRunApp.s_AppActivity, "");
        else if (args.Length >= 1 && args[0].StartsWith("bluestacks:"))
        {
          if (args.Length >= 3)
            newRunApp.StartGameManager(args[1], args[2], "");
        }
        else if (!newRunApp.s_HideMode)
          newRunApp.StartGameManager(newRunApp.s_AppPackage, newRunApp.s_AppActivity, "");
        else
          newRunApp.StartGameManagerHidden(newRunApp.s_AppPackage, newRunApp.s_AppActivity, true, "");
        Environment.Exit(0);
      }
      try
      {
        if (args.Length != 0 && args[0].Equals("Android"))
        {
          if (args.Length <= 2 || args.Length == 3 && args[2].Equals("nolookup"))
          {
            if (Oem.Instance.IsMessageBoxToBeDisplayed)
            {
              int num = (int) MessageBox.Show("Invalid Call", BlueStacks.Common.Strings.ProductDisplayName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            Logger.Info("RunApp arguments: ");
            foreach (string str in args)
              Logger.Info("arg: " + str);
            Logger.Info("Backward compatibility arguments not in correct form. Exiting RunApp");
            Environment.Exit(1);
          }
          newRunApp.s_AppPackage = args[1];
          newRunApp.s_AppActivity = args[2];
          if (args.Length >= 4)
            opt.nl = args[3].Equals("nolookup");
        }
        if (!opt.nl && !string.IsNullOrEmpty(newRunApp.s_AppActivity) && (!string.IsNullOrEmpty(newRunApp.s_AppPackage) && !new JsonParser(BlueStacks.Common.Strings.CurrentDefaultVmName).GetAppData(newRunApp.s_AppPackage, newRunApp.s_AppActivity, out newRunApp.s_AppName, out newRunApp.s_AppIcon)) && !opt.nl)
        {
          if (Oem.Instance.IsMessageBoxToBeDisplayed)
          {
            int num = (int) MessageBox.Show("This app is not installed. Please install the app and try again.", string.Format("{0} Error", (object) BlueStacks.Common.Strings.ProductDisplayName), MessageBoxButtons.OK, MessageBoxIcon.Hand);
          }
          Logger.Info("RunApp arguments: ");
          foreach (string str in args)
            Logger.Info("arg: " + str);
          Logger.Info("App not found. Exiting RunApp");
          Environment.Exit(1);
        }
        newRunApp.LaunchFrontend(newRunApp.s_HideMode);
        Logger.Info("appname: " + newRunApp.s_AppName);
        if (!string.IsNullOrEmpty(newRunApp.s_AppPackage))
        {
          if (!string.IsNullOrEmpty(newRunApp.s_AppActivity))
          {
            newRunApp.data.Clear();
            newRunApp.data.Add("package", newRunApp.s_AppPackage);
            newRunApp.data.Add("activity", newRunApp.s_AppActivity);
            Logger.Info("package: " + newRunApp.s_AppPackage);
            Logger.Info("activity: " + newRunApp.s_AppActivity);
            if (newRunApp.s_ApkUrl != "")
              newRunApp.data.Add("apkUrl", newRunApp.s_ApkUrl);
            newRunApp.s_AgentPort = RegistryManager.Instance.AgentServerPort;
            return JObject.Parse(HTTPUtils.SendRequestToAgent("runApp", newRunApp.data, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, new Dictionary<string, string>()
            {
              {
                "vmname",
                BlueStacks.Common.Strings.CurrentDefaultVmName
              }
            }, false, 10, 500, "bgp", true))["success"].ToObject<bool>() ? 0 : 1;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in RunApp: {0}", (object) ex);
      }
      Environment.Exit(1);
      return 1;
    }

    private static bool CheckAndSetVmName()
    {
      JObject caCode = newRunApp.GetCaCode();
      if (caCode != null && caCode.ContainsKey("success") && (string.Equals(caCode["success"].ToString().Trim(), "true", StringComparison.InvariantCultureIgnoreCase) && caCode.ContainsKey("abi_value")))
        newRunApp.s_CampaignAbiValue = caCode["abi_value"].ToObject<int>();
      else
        Logger.Warning("Error while fetching CaCode");
      foreach (string vm in RegistryManager.RegistryManagers[newRunApp.s_Oem].VmList)
      {
        if (int.Parse(Utils.GetValueInBootParams("abivalue", vm, string.Empty, newRunApp.s_Oem), (IFormatProvider) CultureInfo.InvariantCulture) == newRunApp.s_CampaignAbiValue)
        {
          BlueStacks.Common.Strings.CurrentDefaultVmName = vm;
          return true;
        }
      }
      return false;
    }

    private static JObject GetCaCode()
    {
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "api/getcacode");
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "locale",
          RegistryManager.RegistryManagers[newRunApp.s_Oem].UserSelectedLocale
        },
        {
          "guid",
          RegistryManager.RegistryManagers[newRunApp.s_Oem].UserGuid
        },
        {
          "campaign_hash",
          newRunApp.s_CampaignHash
        },
        {
          "install_id",
          ""
        }
      };
      try
      {
        return JObject.Parse(BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, "", 0, 1, 0, false, "bgp"));
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while fetching info from cloud for CACode : " + ex.ToString());
      }
      return (JObject) null;
    }

    private static void StartGameManager(string package, string activity, string json = "")
    {
      newRunApp.StartGameManagerHidden(package, activity, false, json);
    }

    private static void StartGameManagerHidden(
      string package,
      string activity,
      bool isHidden,
      string json = "")
    {
      Logger.Info("RunApp: Starting Game Manager");
      string partnerExecutablePath = Utils.GetPartnerExecutablePath();
      string arguments = " ";
      if (isHidden || newRunApp.s_HideMode)
        arguments += " -h";
      if (!string.IsNullOrEmpty(json))
        arguments = string.Format("{0} -json \"{1}\"", (object) arguments, (object) Uri.EscapeUriString(json));
      if (!string.IsNullOrEmpty(BlueStacks.Common.Strings.CurrentDefaultVmName))
        arguments = string.Format("{0} -vmname {1}", (object) arguments, (object) BlueStacks.Common.Strings.CurrentDefaultVmName);
      Logger.Info("Launching {0} with args {1}", (object) partnerExecutablePath, (object) arguments);
      Process.Start(partnerExecutablePath, arguments);
    }

    private static void LaunchFrontend(bool hidemode)
    {
      Logger.Info("In LaunchFrontend");
      string fileName = Path.Combine(RegistryStrings.InstallDir, "HD-Player.exe");
      if (hidemode)
      {
        Logger.Info("Starting hidden frontend");
        Process.Start(fileName, string.Format("{0} -h", (object) BlueStacks.Common.Strings.CurrentDefaultVmName));
      }
      else
      {
        Logger.Info("Starting visible frontend");
        string arguments = BlueStacks.Common.Strings.CurrentDefaultVmName;
        if (newRunApp.s_AppName != "" && newRunApp.s_AppIcon == "")
        {
          newRunApp.s_AppIcon = newRunApp.s_AppPackage + "." + newRunApp.s_AppActivity + ".png";
          arguments = string.Format("{0} -n \"{1}\" -i \"{2}\"", (object) BlueStacks.Common.Strings.CurrentDefaultVmName, (object) newRunApp.s_AppName, (object) newRunApp.s_AppIcon);
        }
        else if (newRunApp.s_AppPackage != "")
          arguments = string.Format("{0} -pkg \"{1}\"", (object) arguments, (object) newRunApp.s_AppPackage);
        Logger.Info("Starting Frontend with args : {0}", (object) arguments);
        Process.Start(fileName, arguments);
      }
    }

    private static bool IsAppInstalled(string appPackage)
    {
      Logger.Info("In IsAppInstalled");
      bool flag;
      try
      {
        BstHttpClient.Get(string.Format("http://127.0.0.1:{0}/{1}", (object) RegistryManager.Instance.DefaultGuest.BstAndroidPort, (object) "ping"), (Dictionary<string, string>) null, false, BlueStacks.Common.Strings.CurrentDefaultVmName, 3000, 1, 0, false, "bgp");
        Logger.Info("Guest booted. Will send request.");
        if (JObject.Parse(BstHttpClient.Post(string.Format("http://127.0.0.1:{0}/{1}", (object) RegistryManager.Instance.DefaultGuest.BstAndroidPort, (object) "isPackageInstalled"), new Dictionary<string, string>()
        {
          {
            "package",
            appPackage
          }
        }, (Dictionary<string, string>) null, false, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp"))["result"].ToString().Trim() == "ok")
        {
          Logger.Info("App installed");
          flag = true;
        }
        else
        {
          Logger.Info("App not installed");
          flag = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Guest not booted. Will read from apps.json");
        if (new JsonParser(BlueStacks.Common.Strings.CurrentDefaultVmName).IsAppInstalled(appPackage, out string _))
        {
          Logger.Info("Found in apps.json");
          flag = true;
        }
        else
        {
          Logger.Info("Not found in apps.json");
          flag = false;
        }
      }
      return flag;
    }

    private static void InitExceptionHandlers()
    {
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("RunApp: Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(1);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("RunApp: Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(1);
      });
    }

    public class Opt : GetOpt
    {
      private string json = "";
      public string p = "";
      public string a = "";
      public string url = "";
      public string name = "";
      public string vmname = "Android";
      public string campaign_id = "";
      public string source_version = "";
      public string oem = "bgp";
      public string campaignhash = "";
      public bool nl;
      public bool h;
      public bool t;
      public bool force;
      public bool addflepkg;

      public string Json
      {
        get
        {
          return this.json;
        }
        set
        {
          this.json = Uri.UnescapeDataString(value);
        }
      }
    }
  }
}
