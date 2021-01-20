// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.CloudAnnouncement
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Input;

namespace BlueStacks.Agent
{
  internal class CloudAnnouncement
  {
    private static int sMsgId = -1;
    private static bool sUploadStats = true;
    private static string sOemForSilentLogCollect = "bgp";
    private static bool sIsSilentLogCollectForAllOems = false;
    private static string configString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n                <configuration>\r\n                    <startup>\r\n                        <supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.0\"/>\r\n                        <supportedRuntime version=\"v2.0.50727\" />\r\n                    </startup>\r\n                    <system.net>\r\n                        <defaultProxy useDefaultCredentials=\"true\"/>\r\n                    </system.net>\r\n                </configuration>";

    public static string Dir { get; } = Path.Combine(RegistryStrings.BstUserDataDir, "Announcements");

    public static bool ShowAnnouncement(string vmName)
    {
      if (!Features.IsFeatureEnabled(1UL))
      {
        Logger.Debug("Broadcast message feature disabled. Ignoring...");
        return false;
      }
      Logger.Info("Checking for announcement");
      try
      {
        if (Directory.Exists(CloudAnnouncement.Dir))
        {
          string[] files = Directory.GetFiles(CloudAnnouncement.Dir);
          for (int index = 0; index < files.Length; ++index)
          {
            try
            {
              if (System.IO.File.Exists(files[index]))
                System.IO.File.Delete(files[index]);
            }
            catch (Exception ex)
            {
              Logger.Error("Failed to delete file. err: " + ex.Message);
            }
          }
        }
        else
          Directory.CreateDirectory(CloudAnnouncement.Dir);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to delete/create announcement dir. err: " + ex.Message);
        if (!Directory.Exists(CloudAnnouncement.Dir))
          Directory.CreateDirectory(CloudAnnouncement.Dir);
      }
      return CloudAnnouncement.ShowAnnouncementResponse(BstHttpClient.Get(string.Format("{0}/getAnnouncement", (object) RegistryManager.Instance.Host), new Dictionary<string, string>()
      {
        {
          "x_locale",
          CultureInfo.CurrentCulture.Name.ToLower()
        }
      }, false, vmName, 0, 1, 0, false, "bgp"), vmName);
    }

    public static bool ShowAnnouncementResponse(string resp, string vmName)
    {
      if (resp == null)
      {
        Logger.Error("Failed to get announcement data.");
        return false;
      }
      Logger.Info("Announcement get resp: " + resp);
      JObject o = JObject.Parse(resp);
      string strA = o["success"].ToString().Trim();
      string str = o["reason"].ToString().Trim();
      if (string.Compare(strA, "false", true) == 0)
      {
        Logger.Info("Could not get announcement msg: " + str);
        return false;
      }
      CloudAnnouncement.sMsgId = Convert.ToInt32(o["msgId"].ToString().Trim());
      string imagePath = CloudAnnouncement.DownloadDisplayImage(o["imageUrl"].ToString().Trim());
      try
      {
        CloudAnnouncement.sOemForSilentLogCollect = o["oem"].ToString().Trim();
      }
      catch
      {
        Logger.Warning("OEM not passed in announcement");
      }
      try
      {
        CloudAnnouncement.sIsSilentLogCollectForAllOems = bool.Parse(o["all_oem"].ToString().Trim());
      }
      catch
      {
        Logger.Warning("All_Oem parameter not passed in announcement");
      }
      AnnouncementMessage m = new AnnouncementMessage(imagePath, CloudAnnouncement.sMsgId, o);
      if (m.FileName.Length < 3)
        m.FileName = "downloadedFile.exe";
      try
      {
        CloudAnnouncement.sUploadStats = true;
        CloudAnnouncement.ShowFetchedMsg(m, vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to fetch announcement message. error: " + ex.ToString());
        return false;
      }
      return true;
    }

    public static void ShowNotification(
      string action,
      string title,
      string message,
      string actionURL,
      string fileName,
      string imageURL,
      string vmName)
    {
      string imagePath = string.Empty;
      if (imageURL != null)
        imagePath = CloudAnnouncement.DownloadDisplayImage(imageURL);
      AnnouncementMessage m = new AnnouncementMessage(imagePath, -1, title, message, action, "", actionURL, fileName);
      CloudAnnouncement.sUploadStats = false;
      string vmName1 = vmName;
      CloudAnnouncement.ShowFetchedMsg(m, vmName1);
    }

    private static void ShowFetchedMsg(AnnouncementMessage m, string vmName)
    {
      Logger.Info("ShowFetchedMsg called for: " + m.Action);
      switch (m.Action)
      {
        case "Download and Execute":
          CustomAlert.ShowCloudAnnouncement(m.ImagePath, m.Title, m.Msg, false, (MouseButtonEventHandler) ((o, e) =>
          {
            CloudAnnouncement.UpdateClickStats(vmName);
            new Thread((ThreadStart) (() =>
            {
              Random random = new Random();
              m.FileName += " ";
              string str1 = m.FileName.Substring(0, m.FileName.IndexOf(' '));
              string str2 = m.FileName.Substring(m.FileName.IndexOf(' ') + 1);
              string str3 = Path.Combine(CloudAnnouncement.Dir, string.Format("{0}_{1}", (object) random.Next(), (object) str1));
              try
              {
                new WebClient().DownloadFile(m.ActionURL, str3);
                Thread.Sleep(2000);
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                if ((str3.ToLowerInvariant().EndsWith(".msi") || str3.ToLowerInvariant().EndsWith(".exe")) && !BlueStacksUtils.IsSignedByBlueStacks(str3))
                {
                  Logger.Info("Not executing unsigned binary " + str3);
                }
                else
                {
                  if (str3.ToLowerInvariant().EndsWith(".msi"))
                  {
                    process.StartInfo.FileName = "msiexec";
                    str2 = string.Format("/i {0} {1}", (object) str3, (object) str2);
                    process.StartInfo.Arguments = str2;
                  }
                  else
                  {
                    process.StartInfo.FileName = str3;
                    process.StartInfo.Arguments = str2;
                  }
                  Logger.Info("Starting process: {0} {1}", (object) process.StartInfo.FileName, (object) str2);
                  process.Start();
                }
              }
              catch (Exception ex)
              {
                Logger.Error("Failed to download and execute. err: " + ex.ToString());
              }
            }))
            {
              IsBackground = true
            }.Start();
          }), vmName);
          break;
        case "None":
          CustomAlert.ShowCloudAnnouncement(m.ImagePath, m.Title, m.Msg, false, (MouseButtonEventHandler) null, vmName);
          break;
        case "Silent Install":
          Logger.Info("Got update request. Initializing silent install...");
          new Thread((ThreadStart) (() =>
          {
            Random random = new Random();
            m.FileName += " ";
            string str1 = m.FileName.Substring(0, m.FileName.IndexOf(' '));
            string str2 = m.FileName.Substring(m.FileName.IndexOf(' ') + 1);
            string str3 = Path.Combine(CloudAnnouncement.Dir, string.Format("{0}_{1}", (object) random.Next(), (object) str1));
            try
            {
              new WebClient().DownloadFile(m.ActionURL, str3);
              Thread.Sleep(2000);
              Process process = new Process();
              process.StartInfo.UseShellExecute = true;
              process.StartInfo.CreateNoWindow = true;
              if ((str3.ToLowerInvariant().EndsWith(".msi") || str3.ToLowerInvariant().EndsWith(".exe")) && !BlueStacksUtils.IsSignedByBlueStacks(str3))
              {
                Logger.Info("Not executing unsigned binary " + str3);
              }
              else
              {
                if (str3.ToLowerInvariant().EndsWith(".msi"))
                {
                  process.StartInfo.FileName = "msiexec";
                  str2 = string.Format("/i {0} {1}", (object) str3, (object) str2);
                  process.StartInfo.Arguments = str2;
                }
                else
                {
                  Logger.Info("Creating file: " + str3 + ".config");
                  try
                  {
                    System.IO.File.WriteAllText(str3 + ".config", CloudAnnouncement.configString);
                  }
                  catch (Exception ex)
                  {
                    Logger.Error("Exception in create config file: " + ex.ToString());
                  }
                  process.StartInfo.FileName = str3;
                  process.StartInfo.Arguments = str2;
                }
                Logger.Info("Starting process: {0} {1}", (object) process.StartInfo.FileName, (object) str2);
                process.Start();
              }
            }
            catch (Exception ex)
            {
              Logger.Error("Silent install failed.");
              Logger.Error("Failed to download and execute. err: " + ex.ToString());
            }
          }))
          {
            IsBackground = true
          }.Start();
          break;
        case "Silent LogCollect":
          if (!string.Equals(CloudAnnouncement.sOemForSilentLogCollect, "bgp", StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Info("This is not the OEM for silent log collection. OEM to launch: " + CloudAnnouncement.sOemForSilentLogCollect);
            CloudAnnouncement.StartSilentLogCollectorForCustomOEM(CloudAnnouncement.sOemForSilentLogCollect);
            break;
          }
          Logger.Info("Starting silent log collection");
          new Thread((ThreadStart) (() =>
          {
            try
            {
              string fileName = Path.Combine(HDAgent.s_InstallDir, "HD-LogCollector.exe");
              if (CloudAnnouncement.sIsSilentLogCollectForAllOems)
                Process.Start(fileName, "-silent -logAllOems");
              else
                Process.Start(fileName, "-silent");
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in starting HD-logCollector.exe: " + ex.ToString());
            }
          }))
          {
            IsBackground = true
          }.Start();
          break;
        case "Start Android App":
          CustomAlert.ShowCloudAnnouncement(m.ImagePath, m.Title, m.Msg, false, (MouseButtonEventHandler) ((o, e) =>
          {
            CloudAnnouncement.UpdateClickStats(vmName);
            try
            {
              string fileName = HDAgent.s_InstallDir + "\\HD-RunApp.exe";
              string[] strArray = m.FileName.Split(' ');
              Logger.Info("Broadcast: Starting RunApp: {0} with args: -p {1} -a {2} -nl -vmname:{3}", (object) fileName, (object) strArray[0], (object) strArray[1], (object) vmName);
              Process.Start(fileName, string.Format("-p {0} -a {1} -nl -vmname:{2}", (object) strArray[0], (object) strArray[1], (object) vmName));
            }
            catch (Exception ex)
            {
              Logger.Error("Failed to start android app: {0}. Error: {1}", (object) m.FileName, (object) ex.ToString());
            }
          }), vmName);
          break;
        case "Web URL":
          NotificationManager.Instance.AddNewNotification(m.ImagePath, m.AnnouncementID, m.Title, m.Msg, m.ActionURL);
          CustomAlert.ShowCloudAnnouncement(m.ImagePath, m.Title, m.Msg, false, (MouseButtonEventHandler) ((o, e) =>
          {
            Logger.Info("Announcement msg clicked. Opening url: " + m.ActionURL);
            Process.Start(m.ActionURL);
            CloudAnnouncement.UpdateClickStats(vmName);
          }), vmName);
          break;
        case "Web URL GM":
          NotificationManager.Instance.AddNewNotification(m.ImagePath, m.AnnouncementID, m.Title, m.Msg, m.ActionURL);
          CustomAlert.ShowCloudAnnouncement(m.ImagePath, m.Title, m.Msg, false, (MouseButtonEventHandler) ((o, e) =>
          {
            Logger.Info("Announcement msg clicked. Opening tab: " + m.ActionURL);
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (System.IO.File.Exists(m.ImagePath))
              data.Add("image", m.ImagePath);
            data.Add("url", m.ActionURL);
            data.Add("name", m.Title);
            if (!Utils.IsUIProcessAlive(vmName, "bgp"))
            {
              Logger.Info("Frontend not running");
              CloudAnnouncement.WaitUntilClientServerReady(vmName);
            }
            Logger.Info("Will open tab for url: " + m.ActionURL);
            HTTPUtils.SendRequestToClient("launchWebTab", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }), vmName);
          break;
        default:
          Logger.Error("Announcement: Invalid msg type rcvd: " + m.Action);
          break;
      }
    }

    private static void StartSilentLogCollectorForCustomOEM(string oem)
    {
      try
      {
        string name = "Software\\BlueStacks";
        string str = oem.Equals("bgp") ? "" : "_" + oem;
        if (!string.IsNullOrEmpty(str))
          name += str;
        string empty = string.Empty;
        Logger.Info("Opening reg: {0} for InstallDir", (object) name);
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
        {
          if (registryKey != null)
            empty = (string) registryKey.GetValue("InstallDir", (object) HDAgent.s_InstallDir);
          else
            Logger.Warning("Registry not found, oem does not exist in users machine");
        }
        Logger.Info("InstallDir returned is: " + empty);
        Process.Start(Path.Combine(empty, "HD-logCollector.exe"), "-silent");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in starting HD-logCollector.exe: " + ex.ToString());
      }
    }

    private static string DownloadDisplayImage(string imageURL)
    {
      Stream stream = new WebClient().OpenRead(imageURL);
      Bitmap bitmap = new Bitmap(stream);
      stream.Flush();
      stream.Close();
      string filename = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Path.GetRandomFileName());
      bitmap.Save(filename);
      return filename;
    }

    public static void UpdateClickStats(string vmName)
    {
      if (!CloudAnnouncement.sUploadStats)
        return;
      new Thread((ThreadStart) (() =>
      {
        try
        {
          if (BstHttpClient.Get(string.Format("{0}/updateAnnouncementStats", (object) RegistryManager.Instance.Host), new Dictionary<string, string>()
          {
            {
              "x_last_msg_id",
              Convert.ToString(CloudAnnouncement.sMsgId)
            }
          }, false, vmName, 0, 1, 0, false, "bgp") != null)
            return;
          Logger.Info("Could not send click stats.");
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send click stats: " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static void WaitUntilClientServerReady(string vmName)
    {
      Logger.Info("Starting client");
      ProcessUtils.GetProcessObject(Utils.GetPartnerExecutablePath(), vmName, false).Start();
      while (true)
      {
        try
        {
          if (HTTPUtils.SendRequestToClient("ping", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp").Contains("true", StringComparison.InvariantCultureIgnoreCase))
            break;
        }
        catch (Exception ex)
        {
          Logger.Warning("Partner server not ready yet. Err: " + ex.Message);
        }
        Thread.Sleep(1000);
      }
    }
  }
}
