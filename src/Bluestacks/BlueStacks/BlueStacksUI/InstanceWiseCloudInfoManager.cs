// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.InstanceWiseCloudInfoManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class InstanceWiseCloudInfoManager
  {
    internal InstanceWiseCloudInfo mInstanceWiseCloudInfo = new InstanceWiseCloudInfo();
    private string BstPostBootInstanceFilePath = Path.Combine(RegistryStrings.PromotionDirectory, "bst_postboot");
    private string Url = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) RegistryManager.Instance.Host, (object) "/bs4/multi_instance"), (string) null, (string) null, (string) null);
    private const string sPostBootInstanceInfoFilename = "bst_postboot";

    internal void GetInstanceWisePostBootDataAsync(MainWindow mainWindow)
    {
      this.BstPostBootInstanceFilePath = this.BstPostBootInstanceFilePath + "_" + mainWindow.mVmName;
      new Thread((ThreadStart) (() =>
      {
        try
        {
          JToken res;
          if (!string.IsNullOrEmpty(mainWindow.WindowLaunchParams))
          {
            JObject jobject = JObject.Parse(mainWindow.WindowLaunchParams);
            res = jobject["campaign_id"] == null || jobject["isFarmingInstance"] == null ? this.GetInstanceWisePostBootData(mainWindow.mVmName, "") : this.GetInstanceWisePostBootData(mainWindow.mVmName, jobject["campaign_id"].ToString());
          }
          else
            res = this.GetInstanceWisePostBootData(mainWindow.mVmName, "");
          InstanceWiseCloudInfoManager.SetGameFeatureOnboardingPackages(this.mInstanceWiseCloudInfo, res);
          InstanceWiseCloudInfoManager.SaveInstanceDataToFile(this.mInstanceWiseCloudInfo, this.BstPostBootInstanceFilePath);
        }
        catch (Exception ex)
        {
          Logger.Error("Error in Fetching PostBoot Instance Data err: " + ex.ToString());
        }
      })).Start();
    }

    private JToken GetInstanceWisePostBootData(string vmName, string campaignId = "")
    {
      JToken jtoken = (JToken) null;
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "all_installed_apps",
            Utils.GetInstalledAppDataFromAllVms()
          }
        };
        data.Add("current_vm", "vm" + Utils.GetVmIdFromVmName(vmName));
        string json = string.IsNullOrEmpty(campaignId) ? BstHttpClient.Post(this.Url, data, (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp") : BstHttpClient.Post(this.Url + "&feature_campaign_id=" + campaignId, data, (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp");
        Logger.Debug("PostBoot Instance Specific data Url: " + this.Url);
        jtoken = JToken.Parse(json);
      }
      catch (Exception ex)
      {
        Logger.Error("Error Getting Post Boot Instance Data err: " + ex.ToString());
      }
      return jtoken;
    }

    private static void SetGameFeatureOnboardingPackages(
      InstanceWiseCloudInfo currentInstanceWiseCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("game_feature_onboarding_apps"));
        if (jtoken[(object) "app_pkg_info"] != null && jtoken[(object) "app_pkg_info"] is JArray jarray)
          currentInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages = new AppPackageListObject(jarray.ToObject<List<AppPackageObject>>());
        foreach (AppPackageObject cloudPackage in currentInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages.CloudPackageList)
          cloudPackage.ExtraInfo.Add("isPopupShown", "false");
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing game feature onboarding packages: " + ex.ToString());
      }
    }

    private static void SaveInstanceDataToFile(
      InstanceWiseCloudInfo currentInstanceWiseCloudInfo,
      string fileName)
    {
      try
      {
        string contents = JsonConvert.SerializeObject((object) currentInstanceWiseCloudInfo, Formatting.Indented, Utils.GetSerializerSettings());
        if (!Directory.Exists(RegistryStrings.PromotionDirectory))
          Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
        File.WriteAllText(fileName, contents);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in saving InstancePostBootInfo to file err: " + ex.ToString());
      }
    }
  }
}
