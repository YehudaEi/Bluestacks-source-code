// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DownloadInstallOem
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  internal class DownloadInstallOem
  {
    private MainWindow ParentWindow;
    private AppPlayerModel currentDownloadingOem;
    private long mSizeInBytes;
    private int mLastPercentSend;
    private string vmDisplayName;

    public DownloadInstallOem(MainWindow mainWindow)
    {
      this.ParentWindow = mainWindow;
    }

    public void DownloadOem(AppPlayerModel appPlayerModel)
    {
      this.currentDownloadingOem = appPlayerModel;
      if (InstalledOem.InstalledCoexistingOemList.Contains(appPlayerModel.AppPlayerOem))
      {
        using (BackgroundWorker backgroundWorker = new BackgroundWorker())
        {
          backgroundWorker.DoWork += new DoWorkEventHandler(this.BGCreateNewInstance_DoWork);
          backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGCreateNewInstance_RunWorkerCompleted);
          backgroundWorker.RunWorkerAsync();
        }
      }
      else
      {
        Publisher.PublishMessage(BrowserControlTags.oemDownloadStarted, this.ParentWindow.mVmName, (JObject) null);
        if (appPlayerModel.DownLoadOem((Downloader.DownloadExceptionEventHandler) (_1 => Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", ""),
          ["ActionType"] = (JToken) "failed"
        })), (Downloader.DownloadProgressChangedEventHandler) (size =>
        {
          Decimal num = Decimal.Divide((Decimal) size, (Decimal) this.mSizeInBytes) * new Decimal(100);
          if (this.mLastPercentSend + 4 >= (int) num)
            return;
          Publisher.PublishMessage(BrowserControlTags.oemDownloadCurrentProgress, this.ParentWindow.mVmName, new JObject()
          {
            ["DownloadPercent"] = (JToken) num
          });
          this.mLastPercentSend = (int) num;
        }), (Downloader.DownloadFileCompletedEventHandler) ((_1, _2) =>
        {
          Publisher.PublishMessage(BrowserControlTags.oemDownloadCompleted, this.ParentWindow.mVmName, (JObject) null);
          this.InstallOemOperation();
        }), (Downloader.FilePayloadInfoReceivedHandler) (fileSize => this.mSizeInBytes = fileSize), (Downloader.UnsupportedResumeEventHandler) (_1 => Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_FAILED_DOWNLOAD_RETRY", ""),
          ["ActionType"] = (JToken) "retry"
        })), false))
          return;
        Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", ""),
          ["ActionType"] = (JToken) "failed"
        });
      }
    }

    private void BGCreateNewInstance_DoWork(object sender, DoWorkEventArgs e)
    {
      Publisher.PublishMessage(BrowserControlTags.oemDownloadStarted, this.ParentWindow.mVmName, (JObject) null);
      Publisher.PublishMessage(BrowserControlTags.oemDownloadCompleted, this.ParentWindow.mVmName, (JObject) null);
      Publisher.PublishMessage(BrowserControlTags.oemInstallStarted, this.ParentWindow.mVmName, (JObject) null);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        JObject jobject1 = new JObject()
        {
          {
            "cpu",
            (JToken) 2
          },
          {
            "ram",
            (JToken) 2048
          },
          {
            "dpi",
            (JToken) 240
          },
          {
            "abi",
            (JToken) this.currentDownloadingOem.AbiValue
          },
          {
            "resolutionwidth",
            (JToken) 1920
          },
          {
            "resolutionheight",
            (JToken) 1080
          }
        };
        data["settings"] = jobject1.ToString(Formatting.None);
        data["vmtype"] = "fresh";
        data["vmname"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Android_{0}", (object) Utils.GetVmIdToCreate(this.currentDownloadingOem.AppPlayerOem));
        JObject jobject2 = JObject.Parse(HTTPUtils.SendRequestToAgent("createInstance", data, "Android", 240000, (Dictionary<string, string>) null, false, 1, 0, this.currentDownloadingOem.AppPlayerOem, true));
        if (jobject2["success"].ToObject<bool>())
        {
          RegistryManager.ClearRegistryMangerInstance();
          string str = JObject.Parse(jobject2["vmconfig"].ToString().Trim())["vmname"].ToString().Trim();
          dictionary["vmname"] = str;
          dictionary["status"] = "success";
        }
        else
        {
          dictionary["status"] = "fail";
          dictionary["reason"] = jobject2["reason"].ToString().Trim();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("error in creating new instance" + ex.ToString());
        dictionary["status"] = "fail";
        dictionary["reason"] = "UnknownException";
      }
      finally
      {
        e.Result = (object) dictionary;
      }
    }

    private void BGCreateNewInstance_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      if (!(e.Result is Dictionary<string, string> result))
        return;
      if (result["status"].Equals("success", StringComparison.InvariantCultureIgnoreCase))
      {
        InstalledOem.SetInstalledCoexistingOems();
        RegistryManager.RegistryManagers[this.currentDownloadingOem.AppPlayerOem].Guest[result["vmname"]].DisplayName = Strings.ProductDisplayName + " " + Utils.GetVmIdFromVmName(result["vmname"]) + " " + this.currentDownloadingOem.Suffix;
        Publisher.PublishMessage(BrowserControlTags.oemInstallCompleted, this.ParentWindow.mVmName, (JObject) null);
      }
      else
      {
        if (!result["status"].Equals("fail", StringComparison.InvariantCultureIgnoreCase))
          return;
        Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", ""),
          ["ActionType"] = (JToken) "failed"
        });
      }
    }

    private void InstallOemOperation()
    {
      try
      {
        Publisher.PublishMessage(BrowserControlTags.oemInstallStarted, this.ParentWindow.mVmName, (JObject) null);
        int mInstallFailedErrorCode = this.currentDownloadingOem.InstallOem();
        if (mInstallFailedErrorCode != 0 || !RegistryManager.CheckOemInRegistry(this.currentDownloadingOem.AppPlayerOem, "Android"))
        {
          Logger.Warning("Installation failed: " + mInstallFailedErrorCode.ToString());
          string str = mInstallFailedErrorCode == 0 ? LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "") : InstallerErrorHandling.AssignErrorStringForInstallerExitCodes(mInstallFailedErrorCode, "STRING_INSTALLATION_FAILED");
          Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, new JObject()
          {
            ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""),
            ["MessageBody"] = (JToken) str,
            ["ActionType"] = (JToken) "failed"
          });
        }
        else
        {
          InstalledOem.SetInstalledCoexistingOems();
          if (this.currentDownloadingOem.AppPlayerOem.Contains("bgp64"))
            Utils.UpdateValueInBootParams("abivalue", this.currentDownloadingOem.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Android", true, this.currentDownloadingOem.AppPlayerOem);
          RegistryManager.RegistryManagers[this.currentDownloadingOem.AppPlayerOem].Guest["Android"].DisplayName = Strings.ProductDisplayName + " " + this.currentDownloadingOem.Suffix;
          Publisher.PublishMessage(BrowserControlTags.oemInstallCompleted, this.ParentWindow.mVmName, (JObject) null);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed after running installer process: " + ex?.ToString());
        Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", ""),
          ["ActionType"] = (JToken) "failed"
        });
      }
    }

    public void CreateInstanceSameEngine(string vmdisplayname)
    {
      this.vmDisplayName = vmdisplayname;
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(this.BGCreateInstanceSameEngine_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGCreateInstanceSameEngine_RunWorkerCompleted);
        backgroundWorker.RunWorkerAsync();
      }
    }

    private void BGCreateInstanceSameEngine_DoWork(object sender, DoWorkEventArgs e)
    {
      Publisher.PublishMessage(BrowserControlTags.oemInstallStarted, this.ParentWindow.mVmName, (JObject) null);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Dictionary<string, string> fromBootParamString = Utils.GetBootParamsDictFromBootParamString(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].BootParameters);
        string s1 = fromBootParamString.ContainsKey("DPI") ? fromBootParamString["DPI"] : "240";
        string s2 = fromBootParamString.ContainsKey("abivalue") ? fromBootParamString["abivalue"] : "15";
        string oem = fromBootParamString.ContainsKey("OEM") ? fromBootParamString["OEM"] : "bgp";
        JObject jobject1 = new JObject()
        {
          {
            "cpu",
            (JToken) 2
          },
          {
            "ram",
            (JToken) 2048
          },
          {
            "dpi",
            (JToken) int.Parse(s1, (IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "abi",
            (JToken) int.Parse(s2, (IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "resolutionwidth",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth
          },
          {
            "resolutionheight",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight
          }
        };
        data["settings"] = jobject1.ToString(Formatting.None);
        data["vmtype"] = "fresh";
        data["vmname"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Android_{0}", (object) Utils.GetVmIdToCreate(oem));
        JObject jobject2 = JObject.Parse(HTTPUtils.SendRequestToAgent("createInstance", data, "Android", 240000, (Dictionary<string, string>) null, false, 1, 0, oem, true));
        if (jobject2["success"].ToObject<bool>())
        {
          RegistryManager.ClearRegistryMangerInstance();
          string str = JObject.Parse(jobject2["vmconfig"].ToString().Trim())["vmname"].ToString().Trim();
          dictionary["vmname"] = str;
          dictionary["status"] = "success";
          dictionary["DPI"] = s1;
          dictionary["abivalue"] = s2;
          dictionary["oem"] = oem;
        }
        else
        {
          dictionary["status"] = "fail";
          dictionary["reason"] = jobject2["reason"].ToString().Trim();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("error in creating new instance" + ex.ToString());
        dictionary["status"] = "fail";
        dictionary["reason"] = "UnknownException";
      }
      finally
      {
        e.Result = (object) dictionary;
      }
    }

    private void BGCreateInstanceSameEngine_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      if (!(e.Result is Dictionary<string, string> result))
        return;
      if (result["status"].Equals("success", StringComparison.InvariantCultureIgnoreCase))
      {
        InstalledOem.SetInstalledCoexistingOems();
        JObject extraData = new JObject();
        extraData["vmname"] = (JToken) result["vmname"];
        extraData["averageBootTime"] = (JToken) RegistryManager.Instance.AvgBootTime;
        string[] vmList = RegistryManager.Instance.VmList;
        List<string> stringList = new List<string>();
        foreach (string index in vmList)
          stringList.Add(RegistryManager.Instance.Guest[index].DisplayName);
        if (!string.IsNullOrEmpty(this.vmDisplayName) && stringList.Contains(this.vmDisplayName))
        {
          int num = 1;
          while (stringList.Contains(this.vmDisplayName + " (" + num.ToString() + ")"))
            ++num;
          this.vmDisplayName = this.vmDisplayName + " (" + num.ToString() + ")";
        }
        RegistryManager.Instance.Guest[result["vmname"]].DisplayName = this.vmDisplayName;
        string resolution = "Landscape " + RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth.ToString() + " x " + RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight.ToString();
        string str = this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages?.GetAppPackageObject(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName)?.ExtraInfo["game_popup_id"];
        BlueStacks.Common.Stats.SendMultiInstanceStatsAsync("instance_creation_completed", this.vmDisplayName, "Medium (2 CPU cores) and Medium (2 GB)", resolution, int.Parse(result["abivalue"], (IFormatProvider) CultureInfo.InvariantCulture), result["DPI"], 1, result["oem"], "", (string) null, "Fresh", Utils.GetVmIdFromVmName(result["vmname"]), "", true, str);
        Publisher.PublishMessage(BrowserControlTags.oemInstallCompleted, this.ParentWindow.mVmName, extraData);
      }
      else
      {
        if (!result["status"].Equals("fail", StringComparison.InvariantCultureIgnoreCase))
          return;
        Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, new JObject()
        {
          ["MessageTitle"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""),
          ["MessageBody"] = (JToken) LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", ""),
          ["ActionType"] = (JToken) "failed"
        });
      }
    }
  }
}
