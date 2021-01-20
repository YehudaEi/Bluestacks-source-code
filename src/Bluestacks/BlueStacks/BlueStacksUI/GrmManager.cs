// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GrmManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class GrmManager
  {
    internal static void UpdateGrmAsync(IEnumerable<string> listOfPackages = null)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        if (AppRequirementsParser.Instance.Requirements == null)
          AppRequirementsParser.Instance.PopulateRequirementsFromFile();
        GrmManager.GetGrmFromCloud(listOfPackages);
      }));
    }

    private static void GetGrmFromCloud(IEnumerable<string> listOfPackages = null)
    {
      try
      {
        if (listOfPackages != null && listOfPackages.Any<string>())
        {
          List<string> first = new List<string>();
          foreach (string vm in RegistryManager.Instance.VmList)
            first = first.Union<string>((IEnumerable<string>) JsonParser.GetInstalledAppsList(vm)).ToList<string>();
          if (!listOfPackages.Intersect<string>((IEnumerable<string>) first).Any<string>())
            return;
        }
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToCloud("grm/files", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, false));
        if ((int) jobject["code"] != 200 || !jobject["data"].Value<bool>((object) "success"))
          return;
        string url = jobject["data"][(object) "files"].Value<string>((object) "translations_file");
        string fullJson = BstHttpClient.Get(jobject["data"][(object) "files"].Value<string>((object) "config_file"), (Dictionary<string, string>) null, false, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp");
        string currentDefaultVmName = BlueStacks.Common.Strings.CurrentDefaultVmName;
        string translationJson = BstHttpClient.Get(url, (Dictionary<string, string>) null, false, currentDefaultVmName, 0, 1, 0, false, "bgp");
        AppRequirementsParser.Instance.UpdateOverwriteRequirements(fullJson, translationJson);
      }
      catch (Exception ex)
      {
        Logger.Info("Error Getting Grm json " + ex.ToString());
      }
    }
  }
}
