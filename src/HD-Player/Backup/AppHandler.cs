// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.AppHandler
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace BlueStacks.Player
{
  internal class AppHandler
  {
    internal static bool sAppLaunchedFromRunApp = false;
    internal static string sAppIconName = "";
    internal static bool appLaunched = false;
    internal static object sCurrentAppDisplayedLockObject = new object();
    internal static Dictionary<string, long> sAppPackagesCountClicks = new Dictionary<string, long>();
    internal static Dictionary<string, long> sDictCountClicks = new Dictionary<string, long>();
    internal static string sAppPackage;
    internal static string mCurrentAppPackage;
    internal static string sLastAppDisplayed;
    internal static string mCurrentAppActivity;

    internal static void SetPackagesForCountingInteractions(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      Logger.Info(nameof (SetPackagesForCountingInteractions));
      try
      {
        Dictionary<string, long> dictionary = JObject.Parse(HTTPUtils.ParseRequest(req).Data["data"]).ToDictionary<long>() as Dictionary<string, long>;
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, long> packagesCountClick in AppHandler.sAppPackagesCountClicks)
        {
          if (!dictionary.ContainsKey(packagesCountClick.Key))
            stringList.Add(packagesCountClick.Key);
        }
        foreach (string key in stringList)
          AppHandler.sAppPackagesCountClicks.Remove(key);
        foreach (KeyValuePair<string, long> keyValuePair in dictionary)
        {
          if (!AppHandler.sAppPackagesCountClicks.ContainsKey(keyValuePair.Key.ToLower()))
            AppHandler.sAppPackagesCountClicks.Add(keyValuePair.Key.ToLower(), 0L);
        }
        foreach (KeyValuePair<string, long> packagesCountClick in AppHandler.sAppPackagesCountClicks)
          Logger.Info("questpackage: " + packagesCountClick.Key.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SetPackagesForCountingInteractions. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    internal static void SetCurrentAppData(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("In SetCurrentAppData");
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        AppHandler.mCurrentAppPackage = request.Data["package"];
        AppHandler.mCurrentAppActivity = request.Data["activity"];
        Logger.Info("SetCurrentAppData mCurrentAppPackage = " + AppHandler.mCurrentAppPackage);
        Logger.Info("SetCurrentAppData mCurrentAppActivity = " + AppHandler.mCurrentAppActivity);
        Logger.Info("Looking for: " + AppHandler.sAppPackage);
        if (AppHandler.sAppLaunchedFromRunApp || AppHandler.sAppIconName.Contains(AppHandler.mCurrentAppActivity))
        {
          AppHandler.appLaunched = true;
          AppHandler.sAppLaunchedFromRunApp = false;
        }
        if (!Opt.Instance.sysPrep && Oem.Instance.IsSendGameManagerRequest)
        {
          string str = request.Data["callingPackage"];
          HTTPUtils.SendRequestToClient("appLaunched", new Dictionary<string, string>()
          {
            {
              "package",
              AppHandler.mCurrentAppPackage
            },
            {
              "activity",
              AppHandler.mCurrentAppActivity
            },
            {
              "callingPackage",
              str
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        if (Features.ExitOnHome() && AppHandler.appLaunched && (AppHandler.mCurrentAppPackage == "com.bluestacks.gamepophome" || AppHandler.mCurrentAppPackage == "com.bluestacks.appmart"))
        {
          Logger.Info("Reached home app. Closing frontend.");
          Environment.Exit(0);
        }
        Opengl.HandleAppActivity(AppHandler.mCurrentAppPackage, AppHandler.mCurrentAppActivity);
        InputMapper.Instance.SetPackage(AppHandler.mCurrentAppPackage);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SetCurrentAppData. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }
  }
}
