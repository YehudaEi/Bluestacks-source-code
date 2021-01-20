// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GrmHandler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.Grm;
using BlueStacks.BlueStacksUI.Grm.Evaluators;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  internal class GrmHandler
  {
    private static Dictionary<string, Dictionary<string, GrmRuleSet>> sDictAppRuleSet = new Dictionary<string, Dictionary<string, GrmRuleSet>>();
    private static CustomMessageWindow AppCompatErrorWindow = (CustomMessageWindow) null;

    internal static void RequirementConfigUpdated(string vmName = "Android")
    {
      if (AppRequirementsParser.Instance.Requirements == null)
        return;
      foreach (AppInfo appInfo in ((IEnumerable<AppInfo>) new JsonParser(vmName).GetAppList()).ToList<AppInfo>())
        GrmHandler.RefreshGrmIndication(appInfo.Package, vmName);
      GrmHandler.SendUpdateGrmPackagesToAndroid(vmName);
      GrmHandler.SendUpdateGrmPackagesToBrowser(vmName);
    }

    internal static void SendUpdateGrmPackagesToAndroid(string vmName)
    {
      try
      {
        if (!GrmHandler.sDictAppRuleSet.ContainsKey(vmName) || GrmHandler.sDictAppRuleSet[vmName].Count == 0 || !Utils.IsGuestBooted(vmName, "bgp"))
          return;
        HTTPUtils.SendRequestToGuestAsync("grmPackages", new Dictionary<string, string>()
        {
          {
            "data",
            new JObject()
            {
              {
                "GrmPackageList",
                (JToken) JArray.FromObject((object) GrmHandler.sDictAppRuleSet[vmName].Keys)
              }
            }.ToString(Formatting.None)
          }
        }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendUpdateGrmPackagesToAndroid: " + ex.ToString());
      }
    }

    internal static void SendUpdateGrmPackagesToBrowser(string vmName)
    {
      try
      {
        if (!GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
          return;
        JObject jobject = new JObject();
        foreach (KeyValuePair<string, GrmRuleSet> keyValuePair in GrmHandler.sDictAppRuleSet[vmName])
          jobject.Add((object) new JProperty(keyValuePair.Key, (object) keyValuePair.Value.MessageWindow.MessageType));
        Publisher.PublishMessage(BrowserControlTags.grmAppListUpdate, vmName, new JObject((object) new JProperty("GrmPackageData", (object) jobject)));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendUpdateGrmPackagesToBrowser: " + ex.ToString());
      }
    }

    internal static void RefreshGrmIndicationForAllInstances(string package)
    {
      foreach (string key in BlueStacksUIUtils.DictWindows.Keys)
      {
        GrmHandler.RefreshGrmIndication(package, key);
        GrmHandler.SendUpdateGrmPackagesToAndroid(key);
        GrmHandler.SendUpdateGrmPackagesToBrowser(key);
      }
    }

    internal static void RefreshGrmIndication(string package, string vmName = "Android")
    {
      try
      {
        List<AppRequirement> requirements = AppRequirementsParser.Instance.Requirements;
        if (requirements == null)
          return;
        if (!GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
          GrmHandler.sDictAppRuleSet[vmName] = new Dictionary<string, GrmRuleSet>();
        AppIconModel appIcon = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package);
        if (appIcon == null)
          return;
        if (appIcon.AppIncompatType != AppIncompatType.None && !requirements.Any<AppRequirement>((Func<AppRequirement, bool>) (_ => string.Equals(_.PackageName, package, StringComparison.OrdinalIgnoreCase))))
          GrmHandler.RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
        AppRequirement appRequirement = requirements.Where<AppRequirement>((Func<AppRequirement, bool>) (_ => string.Compare(_.PackageName, package, StringComparison.OrdinalIgnoreCase) == 0)).FirstOrDefault<AppRequirement>() ?? requirements.Where<AppRequirement>((Func<AppRequirement, bool>) (_ =>
        {
          if (!_.PackageName.EndsWith("*", StringComparison.InvariantCulture))
            return false;
          return package.StartsWith(_.PackageName.Trim('*'), StringComparison.InvariantCulture);
        })).FirstOrDefault<AppRequirement>();
        if (appRequirement == null)
          return;
        GrmRuleSet requirement = appRequirement.EvaluateRequirement(package, vmName);
        if (requirement != null)
          GrmHandler.AddGRMIndicationForIncompatibleApp(appIcon, BlueStacksUIUtils.DictWindows[vmName], requirement);
        else
          GrmHandler.RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in RefreshGrmIndication. Exception: " + ex?.ToString());
      }
    }

    internal static void HandleCompatibility(string package, string vmName)
    {
      try
      {
        GrmHandler.AppCompatErrorWindow = new CustomMessageWindow();
        GrmHandler.AppCompatErrorWindow.TitleTextBlock.Text = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package).AppName;
        if (!string.IsNullOrEmpty(AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey)))
        {
          GrmHandler.AppCompatErrorWindow.BodyTextBlockTitle.Text = AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey);
          GrmHandler.AppCompatErrorWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
        }
        GrmHandler.AppCompatErrorWindow.BodyTextBlock.Text = AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageStringKey);
        if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageType == MessageType.Info.ToString())
          GrmHandler.AppCompatErrorWindow.MessageIcon.ImageName = "message_info";
        else if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageType == MessageType.Error.ToString())
          GrmHandler.AppCompatErrorWindow.MessageIcon.ImageName = "message_error";
        GrmHandler.AppCompatErrorWindow.MessageIcon.Visibility = Visibility.Visible;
        if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.DontShowOption)
        {
          GrmHandler.AppCompatErrorWindow.CheckBox.Content = (object) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
          GrmHandler.AppCompatErrorWindow.CheckBox.Visibility = Visibility.Visible;
        }
        foreach (GrmMessageButton button1 in GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.Buttons)
        {
          GrmMessageButton button = button1;
          ButtonColors color = EnumHelper.Parse<ButtonColors>(button.ButtonColor, ButtonColors.Blue);
          GrmHandler.AppCompatErrorWindow.AddButton(color, AppRequirementsParser.Instance.GetLocalizedString(button.ButtonStringKey), (EventHandler) ((o, e) => GrmHandler.PerformGrmActions(button.Actions, package, BlueStacksUIUtils.DictWindows[vmName])), (string) null, false, (object) null, true);
        }
        GrmHandler.AppCompatErrorWindow.Owner = (Window) BlueStacksUIUtils.DictWindows[vmName];
        BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay((IDimOverlayControl) null);
        GrmHandler.AppCompatErrorWindow.ShowDialog();
        BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while showing appcompat message to user. Exception: " + ex.ToString());
        BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(package, false);
      }
    }

    private static void PerformGrmActions(
      List<GrmAction> actions,
      string package,
      MainWindow ParentWindow)
    {
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += (DoWorkEventHandler) ((obj, e) => GrmHandler.PerformGrmActionsWorker_DoWork(e, actions, package, ParentWindow));
        backgroundWorker.RunWorkerCompleted += (RunWorkerCompletedEventHandler) ((obj, e) => GrmHandler.PerformGrmActionsWorker_RunWorkerCompleted(e, ParentWindow));
        backgroundWorker.RunWorkerAsync();
      }
    }

    private static void PerformGrmActionsWorker_DoWork(
      DoWorkEventArgs e,
      List<GrmAction> actions,
      string package,
      MainWindow ParentWindow)
    {
      try
      {
        ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          ParentWindow.mFrontendGrid.Visibility = Visibility.Hidden;
          ParentWindow.mExitProgressGrid.ProgressText = LocaleStrings.GetLocalizedString("STRING_PERFORMING_ACTIONS", "");
          ParentWindow.mExitProgressGrid.Visibility = Visibility.Visible;
        }));
        try
        {
          ClientStats.SendMiscellaneousStatsAsync("grm_action_clicked", RegistryManager.Instance.UserGuid, string.Join(",", actions.Select<GrmAction, string>((Func<GrmAction, string>) (_ => _.ActionType.ToString((IFormatProvider) CultureInfo.InvariantCulture))).ToArray<string>()), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp", package, (string) null, (string) null, "Android");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while sending misc stat for grm. " + ex?.ToString());
        }
        e.Result = (object) false;
        foreach (GrmAction action1 in actions)
        {
          GrmActionType grmActionType = EnumHelper.Parse<GrmActionType>(action1.ActionType, GrmActionType.NoAction);
          bool? nullable1 = new bool?(false);
          EngineState engineState;
          switch (grmActionType)
          {
            case GrmActionType.UpdateRam:
              int num1 = int.Parse(action1.ActionDictionary["actionValue"], (IFormatProvider) CultureInfo.InvariantCulture);
              int num2 = 4096;
              int result;
              if (int.TryParse(PhysicalRamEvaluator.RAM, out result))
                num2 = (int) ((double) result * 0.5);
              string currentEngine1 = RegistryManager.Instance.CurrentEngine;
              engineState = EngineState.raw;
              string str1 = engineState.ToString();
              if (currentEngine1 == str1 && num2 >= 3072)
                num2 = 3072;
              if (num2 < num1)
                num1 = num2;
              RegistryManager.Instance.Guest[ParentWindow.mVmName].Memory = num1;
              nullable1 = new bool?(true);
              break;
            case GrmActionType.UserBrowser:
              ParentWindow.Utils.HandleGenericActionFromDictionary(action1.ActionDictionary, "grm", "");
              nullable1 = new bool?(false);
              break;
            case GrmActionType.DownloadFileAndExecute:
              Random random = new Random();
              string str2 = action1.ActionDictionary["fileName"] + " ";
              string str3 = str2.Substring(0, str2.IndexOf(' '));
              string str4 = str2.Substring(str2.IndexOf(' ') + 1);
              string str5 = Path.Combine(RegistryStrings.PromotionDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) random.Next(), (object) str3));
              try
              {
                using (WebClient webClient = new WebClient())
                  webClient.DownloadFile(action1.ActionDictionary["url"], str5);
                Thread.Sleep(2000);
                using (Process process = new Process())
                {
                  process.StartInfo.UseShellExecute = true;
                  process.StartInfo.CreateNoWindow = true;
                  if ((str5.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase) || str5.ToUpperInvariant().EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) && !BlueStacksUtils.IsSignedByBlueStacks(str5))
                  {
                    Logger.Info("Not executing unsigned binary " + str5);
                    GrmHandler.GrmExceptionMessageBox(ParentWindow);
                    return;
                  }
                  if (str5.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase))
                  {
                    process.StartInfo.FileName = "msiexec";
                    str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/i {0} {1}", (object) str5, (object) str4);
                    process.StartInfo.Arguments = str4;
                  }
                  else
                  {
                    process.StartInfo.FileName = str5;
                    process.StartInfo.Arguments = str4;
                  }
                  Logger.Info("Starting process: {0} {1}", (object) process.StartInfo.FileName, (object) str4);
                  process.Start();
                }
              }
              catch (Exception ex)
              {
                GrmHandler.GrmExceptionMessageBox(ParentWindow);
                Logger.Error("Failed to download and execute. err: " + ex?.ToString());
              }
              nullable1 = new bool?(false);
              break;
            case GrmActionType.NoAction:
              nullable1 = new bool?(false);
              break;
            case GrmActionType.ContinueAnyway:
              ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
                ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
                if (GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].MessageWindow.DontShowOption)
                {
                  GrmHandler.DonotShowCheckboxHandling(ParentWindow, package);
                  GrmHandler.RefreshGrmIndication(package, ParentWindow.mVmName);
                  GrmHandler.SendUpdateGrmPackagesToAndroid(ParentWindow.mVmName);
                  GrmHandler.SendUpdateGrmPackagesToBrowser(ParentWindow.mVmName);
                }
                ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, false);
              }));
              nullable1 = new bool?();
              break;
            case GrmActionType.GlMode:
              string action2 = action1.ActionDictionary["actionValue"];
              int glRenderMode = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode;
              int glMode1 = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode;
              GlMode glMode2 = GlMode.PGA_GL;
              if (glRenderMode == 1 && glMode1 == 1)
                glMode2 = GlMode.PGA_GL;
              else if (glRenderMode == 1 && glMode1 == 2)
              {
                glMode2 = GlMode.AGA_GL;
              }
              else
              {
                switch (glMode1)
                {
                  case 1:
                    glMode2 = GlMode.PGA_DX;
                    break;
                  case 2:
                    glMode2 = GlMode.AGA_DX;
                    break;
                }
              }
              List<string> list1 = ((IEnumerable<string>) action2.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>();
              if (list1.Contains<string>(glMode2.ToString(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
              {
                nullable1 = new bool?(false);
                break;
              }
              string str6 = list1.RandomElement<string>();
              string str7 = "";
              int num3;
              string str8;
              if (string.Compare(str6.Split('_')[1].Trim(), "GL", StringComparison.OrdinalIgnoreCase) == 0)
              {
                num3 = 1;
                str8 = str7 + "1";
              }
              else
              {
                num3 = 4;
                str8 = str7 + "4";
              }
              int num4;
              string args;
              if (string.Compare(str6.Split('_')[0].Trim(), "PGA", StringComparison.OrdinalIgnoreCase) == 0)
              {
                num4 = 1;
                args = str8 + " 1";
              }
              else
              {
                num4 = 2;
                args = str8 + " 2";
              }
              if (RunCommand.RunCmd(Path.Combine(RegistryStrings.InstallDir, "HD-GlCheck"), args, true, true, false, 0).ExitCode == 0)
              {
                RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode = num3;
                Utils.UpdateValueInBootParams("GlMode", num4.ToString((IFormatProvider) CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp");
                RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode = num4;
              }
              else
              {
                GrmHandler.GrmExceptionMessageBox(ParentWindow);
                Logger.Info("GL check execution for the required combination failed.");
              }
              nullable1 = new bool?(true);
              break;
            case GrmActionType.DeviceProfile:
              string strA = action1.ActionDictionary["pCode"];
              string empty = string.Empty;
              string str9;
              if (string.Compare(strA, "custom", StringComparison.OrdinalIgnoreCase) == 0)
              {
                str9 = "{" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", (object) "true") + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"model\":\"{0}\",", (object) action1.ActionDictionary["model"]) + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", (object) action1.ActionDictionary["brand"]) + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\"", (object) action1.ActionDictionary["manufacturer"]) + "}";
              }
              else
              {
                List<string> list2 = ((IEnumerable<string>) strA.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>();
                string valueInBootParams = Utils.GetValueInBootParams("pcode", ParentWindow.mVmName, "", "bgp");
                if (!list2.Contains(valueInBootParams))
                {
                  strA = list2.RandomElement<string>();
                  str9 = "{" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", (object) "false") + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"pcode\":\"{0}\"", (object) strA) + "}";
                }
                else
                  break;
              }
              if (string.Equals(VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) "changeDeviceProfile", (object) str9), ParentWindow.mVmName, "bgp"), "ok", StringComparison.InvariantCulture))
              {
                Utils.UpdateValueInBootParams("pcode", strA, ParentWindow.mVmName, false, "bgp");
                if (PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(package))
                  HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string>()
                  {
                    {
                      nameof (package),
                      package
                    }
                  }, ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              }
              else
              {
                GrmHandler.GrmExceptionMessageBox(ParentWindow);
                Logger.Error("Setting device profile for the required combination failed.");
              }
              nullable1 = new bool?(false);
              break;
            case GrmActionType.BootParam:
              Utils.UpdateValueInBootParams(action1.ActionDictionary["param"].Trim(), action1.ActionDictionary["actionValue"].Trim(), ParentWindow.mVmName, true, "bgp");
              nullable1 = new bool?(true);
              break;
            case GrmActionType.DPI:
              string action3 = action1.ActionDictionary["actionValue"];
              Utils.SetDPIInBootParameters(RegistryManager.Instance.Guest[ParentWindow.mVmName].BootParameters, action3, ParentWindow.mVmName, "bgp");
              nullable1 = new bool?(true);
              break;
            case GrmActionType.CpuCores:
              int num5 = int.Parse(action1.ActionDictionary["actionValue"], (IFormatProvider) CultureInfo.InvariantCulture);
              string currentEngine2 = RegistryManager.Instance.CurrentEngine;
              engineState = EngineState.raw;
              string str10 = engineState.ToString();
              if (currentEngine2 != str10)
                RegistryManager.Instance.Guest[ParentWindow.mVmName].VCPUs = num5 > 8 ? 8 : num5;
              nullable1 = new bool?(true);
              break;
            case GrmActionType.Resolution:
              List<string> list3 = ((IEnumerable<string>) action1.ActionDictionary["actionValue"].Split(',')).Select<string, string>((Func<string, string>) (_ => _.Replace(" ", string.Empty))).ToList<string>();
              string str11 = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x" + RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              if (!list3.Contains<string>(str11, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
              {
                string str12 = list3.RandomElement<string>();
                RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth = int.Parse(str12.Split('x')[0].Trim(), (IFormatProvider) CultureInfo.InvariantCulture);
                RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight = int.Parse(str12.Split('x')[1].Trim(), (IFormatProvider) CultureInfo.InvariantCulture);
                nullable1 = new bool?(true);
                break;
              }
              break;
            case GrmActionType.RestartBluestacks:
              nullable1 = new bool?(true);
              break;
            case GrmActionType.RestartMachine:
              Process.Start("shutdown.exe", "-r -t 0");
              break;
            case GrmActionType.Fps:
              int newFps = int.Parse(action1.ActionDictionary["actionValue"], (IFormatProvider) CultureInfo.InvariantCulture);
              RegistryManager.Instance.Guest[ParentWindow.mVmName].FPS = newFps;
              Utils.UpdateValueInBootParams("fps", newFps.ToString((IFormatProvider) CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp");
              Utils.SendChangeFPSToInstanceASync(ParentWindow.mVmName, newFps, "bgp");
              nullable1 = new bool?(false);
              break;
            case GrmActionType.EditRegistry:
              string action4 = action1.ActionDictionary["location"];
              if (string.Compare(action4, "registryManager", StringComparison.OrdinalIgnoreCase) == 0)
              {
                PropertyInfo property = typeof (RegistryManager).GetProperty(action1.ActionDictionary["propertyName"]);
                object obj = Convert.ChangeType((object) action1.ActionDictionary["propertyValue"], Type.GetTypeCode(property.PropertyType), (IFormatProvider) CultureInfo.InvariantCulture);
                property.SetValue((object) RegistryManager.Instance, obj, (object[]) null);
              }
              else if (string.Compare(action4, "instanceManager", StringComparison.OrdinalIgnoreCase) == 0)
              {
                PropertyInfo property = typeof (InstanceRegistry).GetProperty(action1.ActionDictionary["propertyName"]);
                object obj = Convert.ChangeType((object) action1.ActionDictionary["propertyValue"], Type.GetTypeCode(property.PropertyType), (IFormatProvider) CultureInfo.InvariantCulture);
                property.SetValue((object) RegistryManager.Instance.Guest[ParentWindow.mVmName], obj, (object[]) null);
              }
              else
              {
                string registryPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\{1}", (object) BlueStacks.Common.Strings.GetOemTag(), (object) action1.ActionDictionary["propertyPath"].Replace("vmName", ParentWindow.mVmName));
                object obj = (object) null;
                RegistryValueKind type = EnumHelper.Parse<RegistryValueKind>(action1.ActionDictionary["propertyRegistryKind"], RegistryValueKind.String);
                switch (type)
                {
                  case RegistryValueKind.String:
                    obj = (object) action1.ActionDictionary["propertyValue"];
                    break;
                  case RegistryValueKind.DWord:
                    obj = (object) int.Parse(action1.ActionDictionary["propertyValue"], (IFormatProvider) CultureInfo.InvariantCulture);
                    break;
                }
                RegistryUtils.SetRegistryValue(registryPath, action1.ActionDictionary["propertyName"], obj, type, RegistryKeyKind.HKEY_LOCAL_MACHINE);
              }
              nullable1 = new bool?(false);
              break;
            case GrmActionType.ClearAppData:
              HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string>()
              {
                {
                  nameof (package),
                  package
                }
              }, ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              break;
          }
          bool? nullable2 = nullable1;
          bool flag = true;
          if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
            e.Result = (object) true;
          else if (!nullable1.HasValue)
            e.Result = (object) null;
        }
        Thread.Sleep(1000);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in performing grm actions, ex: " + ex.ToString());
        ClientStats.SendMiscellaneousStatsAsync("grm_action_error", RegistryManager.Instance.UserGuid, GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp", package, ex.Message, (string) null, "Android");
        GrmHandler.GrmExceptionMessageBox(ParentWindow);
        e.Result = (object) null;
      }
    }

    private static void PerformGrmActionsWorker_RunWorkerCompleted(
      RunWorkerCompletedEventArgs e,
      MainWindow ParentWindow)
    {
      if (e.Result == null)
        return;
      if (!(bool) e.Result)
      {
        ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
          ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
        }));
        GrmHandler.RequirementConfigUpdated(ParentWindow.mVmName);
      }
      else
        BlueStacksUIUtils.RestartInstance(ParentWindow.mVmName, false);
    }

    private static void GrmExceptionMessageBox(MainWindow ParentWindow)
    {
      ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
        ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ERROR", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_GRM_EXCEPTION_MESSAGE", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) ParentWindow;
        ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        ParentWindow.HideDimOverlay();
      }));
    }

    private static void DonotShowCheckboxHandling(MainWindow ParentWindow, string package)
    {
      bool? isChecked = GrmHandler.AppCompatErrorWindow.CheckBox.IsChecked;
      bool flag = true;
      if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue))
        return;
      List<string> list = ((IEnumerable<string>) RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList).ToList<string>();
      if (!list.Contains(GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId))
        list.Add(GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId);
      RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList = list.ToArray();
    }

    private static void AddGRMIndicationForIncompatibleApp(
      AppIconModel appIcon,
      MainWindow ParentWindow,
      GrmRuleSet passedRuleSet)
    {
      ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        switch (EnumHelper.Parse<MessageType>(passedRuleSet.MessageWindow.MessageType, MessageType.None))
        {
          case MessageType.None:
            appIcon.AppIncompatType = AppIncompatType.Error;
            GrmHandler.sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
            break;
          case MessageType.Info:
            appIcon.AppIncompatType = AppIncompatType.Info;
            GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
            break;
          case MessageType.Error:
            appIcon.AppIncompatType = AppIncompatType.Error;
            GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
            break;
        }
      }));
    }

    private static void RemoveAppCompatError(AppIconModel appIcon, MainWindow ParentWindow)
    {
      ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        appIcon.AppIncompatType = AppIncompatType.None;
        GrmHandler.sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
      }));
    }

    internal static void RemovePackageFromGrmList(string packageName, string vmName)
    {
      if (!GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
        return;
      GrmHandler.sDictAppRuleSet[vmName].Remove(packageName);
    }
  }
}
