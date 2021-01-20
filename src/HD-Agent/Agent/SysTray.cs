// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.SysTray
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.Agent
{
  internal class SysTray
  {
    private static string s_AgentOnlineText = "App Player online";
    private static NotifyIcon s_SysTrayIcon = (NotifyIcon) null;
    private static ContextMenuStrip s_ContextMenuStrip = (ContextMenuStrip) null;

    public static void Init(string vmName)
    {
      if (!Features.IsFeatureEnabled(64UL))
      {
        Logger.Info("Disabling systray support because feature is disabled.");
      }
      else
      {
        SysTray.s_AgentOnlineText = LocaleStrings.GetLocalizedString("STRING_APP_PLAYER_ONLINE", "");
        SysTray.s_AgentOnlineText = Oem.Instance.GetTitle(SysTray.s_AgentOnlineText);
        SysTray.s_SysTrayIcon = new NotifyIcon();
        SysTray.s_SysTrayIcon.BalloonTipClicked += new EventHandler(SysTray.AppAndExplorerLauncher);
        SysTray.s_SysTrayIcon.Icon = Utils.GetApplicationIcon();
        SysTray.s_SysTrayIcon.Text = SysTray.s_AgentOnlineText;
        if (Oem.Instance.IsShowVersionOnSysTrayToolTip)
          SysTray.s_SysTrayIcon.Text += " (4.250.0.1070)";
        SysTray.s_SysTrayIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(SysTray.OnSysTrayMouseDown);
        SysTray.s_SysTrayIcon.MouseUp += (System.Windows.Forms.MouseEventHandler) ((x, e) => SysTray.OnSysTrayMouseUp(x, e, vmName));
        SysTray.s_SysTrayIcon.Visible = true;
        if (!Features.IsFeatureEnabled(16384UL))
          SysTray.s_SysTrayIcon.Visible = false;
        if (!Oem.Instance.IsSysTrayIconTextToBeBlueStacks3)
          return;
        SysTray.s_SysTrayIcon.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS", "");
      }
    }

    public static void SetTrayIconVisibility(bool visible)
    {
      if (SysTray.s_SysTrayIcon == null || !Features.IsFeatureEnabled(16384UL))
        return;
      SysTray.s_SysTrayIcon.Visible = visible;
      NotificationWindow.Instance.EnablePopups(visible);
    }

    private static void AddContextMenus(string vmName)
    {
      if (SysTray.s_ContextMenuStrip != null)
        SysTray.s_ContextMenuStrip.Dispose();
      SysTray.s_ContextMenuStrip = new ContextMenuStrip();
      SysTray.s_SysTrayIcon.ContextMenuStrip = SysTray.s_ContextMenuStrip;
      SysTray.AddAppPlayerMenuItems(vmName);
    }

    private static void AddAppPlayerMenuItems(string vmName)
    {
      if (Oem.Instance.IsShowAllInstancesToBeAddedInContextMenuOfSysTray && NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict.ContainsValue(true))
        SysTray.AddShowAllInstancesContextMenu();
      if (Oem.Instance.IsOnlyStopButtonToBeAddedInContextMenuOFSysTray)
      {
        SysTray.AddStopContextMenu(vmName);
      }
      else
      {
        ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
        ToolStripSeparator toolStripSeparator2 = new ToolStripSeparator();
        SysTray.AddZipLogsContextMenu();
        if (Features.IsFeatureEnabled(32UL))
          SysTray.AddBstUsageContextMenu();
        SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripSeparator1);
        SysTray.AddStopContextMenu(vmName);
        SysTray.s_ContextMenuStrip.ShowCheckMargin = false;
        SysTray.s_ContextMenuStrip.ShowImageMargin = false;
      }
    }

    private static void OnSysTrayMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Right)
        return;
      SysTray.s_SysTrayIcon.ContextMenuStrip = (ContextMenuStrip) null;
    }

    private static void OnSysTrayMouseUp(object sender, System.Windows.Forms.MouseEventArgs e, string vmName)
    {
      if (e.Button == MouseButtons.Left && Oem.Instance.IsLefClickOnTrayIconLaunchPartner)
      {
        Logger.Info("Starting Partner on systray icon click");
        Stats.SendCommonClientStatsAsync("notification_mode", "systray_clicked", vmName, string.Empty, "open", "", "");
        Logger.Info("Notification mode : Off");
        ProcessUtils.GetProcessObject(Utils.GetPartnerExecutablePath(), vmName, false).Start();
      }
      else
      {
        SysTray.AddContextMenus(vmName);
        typeof (NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) SysTray.s_SysTrayIcon, (object[]) null);
      }
    }

    public static void ShowInfoShort(
      string title,
      string message,
      string vmName,
      string package,
      string id = "0")
    {
      title = Oem.Instance.GetTitle(title);
      id = !id.Equals("200", StringComparison.InvariantCultureIgnoreCase) || !package.Equals("com.bluestacks.filemanager", StringComparison.InvariantCultureIgnoreCase) ? Guid.NewGuid().ToString() : "200";
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "app_name",
          title
        },
        {
          "msg",
          message
        },
        {
          "pkg",
          package
        },
        {
          "vmname",
          vmName
        },
        {
          nameof (id),
          id
        }
      };
      switch (NotificationManager.Instance.IsShowNotificationForKey(title, vmName))
      {
        case MuteState.NotMuted:
        case MuteState.AutoHide:
          HTTPUtils.SendRequestToClient("addNotificationInDrawer", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          break;
      }
      if (!NotificationManager.Instance.IsDesktopNotificationToBeShown(title))
        return;
      SysTray.ShowTrayStatus(ToolTipIcon.Info, title, message, 1000, vmName, package, id);
    }

    public static void ShowTrayStatus(
      ToolTipIcon icon,
      string title,
      string message,
      int timeout,
      string vmName,
      string package,
      string id = "0")
    {
      CustomAlert.ShowAlert((string) null, title, message, true, (MouseButtonEventHandler) null, false, vmName, false, package, id);
    }

    public static void LaunchExplorer(string message)
    {
      try
      {
        string[] strArray = message.Split('\n');
        string fullName = Directory.GetParent(strArray[0]).FullName;
        Process.Start("explorer.exe", strArray.Length == 1 ? string.Format("/Select, {0}", (object) strArray[0]) : fullName);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
      }
    }

    public static void AppAndExplorerLauncher(object sender, EventArgs e)
    {
      Logger.Info("Clicked on BalloonTip");
      string vmName = "Android";
      try
      {
        string localizedString1 = LocaleStrings.GetLocalizedString("STRING_INSTALL_SUCCESS", "");
        string localizedString2 = LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATES", "");
        string localizedString3 = LocaleStrings.GetLocalizedString("STRING_UNINSTALL_SUCCESS", "");
        string balloonTipTitle = ((NotifyIcon) sender).BalloonTipTitle;
        string balloonTipText = ((NotifyIcon) sender).BalloonTipText;
        string str;
        if (balloonTipText.Contains(localizedString1) || balloonTipText.Contains(localizedString2))
        {
          str = balloonTipText.Substring(0, balloonTipText.LastIndexOf(localizedString1) - 1);
        }
        else
        {
          if (balloonTipText.Contains(localizedString3))
            return;
          str = balloonTipTitle;
        }
        if (string.Compare(str, "Successfully copied files:", true) == 0 || string.Compare(str, "Cannot copy files:", true) == 0)
        {
          SysTray.LaunchExplorer(balloonTipText);
        }
        else
        {
          Logger.Info("Launching " + str);
          string packageName = "com.bluestacks.appmart";
          string activityName = "com.bluestacks.appmart.StartTopAppsActivity";
          string fileName = HDAgent.s_InstallDir + "\\HD-RunApp.exe";
          if (!new JsonParser(vmName).GetAppInfoFromAppName(str, out packageName, out string _, out activityName))
          {
            Logger.Error("Failed to launch app: {0}. No info found in json. Starting home app", (object) str);
            Process.Start(fileName, string.Format("-p {0} -a {1} -vmname:{2}", (object) packageName, (object) activityName, (object) vmName));
          }
          else
            Process.Start(fileName, string.Format("-p {0} -a {1} -vmname:{2}", (object) packageName, (object) activityName, (object) vmName));
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    private static void AddBstUsageContextMenu()
    {
      ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripSeparator);
      int installedAppCount = new JsonParser("Android").GetInstalledAppCount();
      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(string.Format("{0} {1} {2}", (object) installedAppCount, installedAppCount > 1 ? (object) LocaleStrings.GetLocalizedString("STRING_APPS", "") : (object) LocaleStrings.GetLocalizedString("STRING_APP", ""), (object) LocaleStrings.GetLocalizedString("STRING_INSTALLED", "")));
      toolStripMenuItem1.Enabled = false;
      ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem2);
    }

    private static void AddZipLogsContextMenu()
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(LocaleStrings.GetLocalizedString("STRING_UPLOAD_DEBUG_LOGS", ""));
      toolStripMenuItem.Click += (EventHandler) ((o, a) => SysTray.ZipLogsToEmail());
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem);
    }

    private static void AddNetEaseProblemReportMenu()
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("问题咨询");
      toolStripMenuItem.Click += (EventHandler) ((o, a) =>
      {
        try
        {
          Process.Start("http://gh.163.com/m");
        }
        catch (Exception ex)
        {
          Logger.Error("Error Occurred, Err: {0}", (object) ex.ToString());
        }
      });
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem);
    }

    private static void ZipLogsToEmail()
    {
      string installDir = RegistryStrings.InstallDir;
      ProcessStartInfo startInfo = new ProcessStartInfo()
      {
        FileName = installDir + "HD-LogCollector.exe"
      };
      Logger.Info("SysTray: Starting " + startInfo.FileName);
      Process.Start(startInfo);
    }

    private static void AddStopContextMenu(string vmName)
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(LocaleStrings.GetLocalizedString("STRING_QUIT_BLUESTACKS", ""));
      toolStripMenuItem.Click += (EventHandler) ((o, a) =>
      {
        Stats.SendCommonClientStatsAsync("notification_mode", "systray_clicked", vmName, string.Empty, "quit", "", "");
        Logger.Info("Instance quit from systray : " + vmName);
        SysTray.Quit(false, vmName);
      });
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem);
    }

    internal static void AddShowAllInstancesContextMenu()
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(LocaleStrings.GetLocalizedString("STRING_SHOW_ALL_INSTANCES", ""));
      toolStripMenuItem.Click += (EventHandler) ((o, a) =>
      {
        Stats.SendCommonClientStatsAsync("notification_mode", "systray_clicked", "Android", string.Empty, "open_all_instances", "", "");
        Logger.Info("Notification mode : Off");
        ProcessUtils.GetProcessObject(Utils.GetPartnerExecutablePath(), "--launchedFromSysTray", false).Start();
      });
      SysTray.s_ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem);
    }

    public static void DisposeIcon()
    {
      if (SysTray.s_SysTrayIcon == null)
        return;
      SysTray.s_SysTrayIcon.Dispose();
    }

    private static void Quit(bool exitSelfProcess, string vmName)
    {
      Logger.Info("SysTray: Exiting BlueStacks");
      try
      {
        HTTPUtils.SendRequestToClient("quit", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in quitting client, " + ex.ToString());
      }
      Utils.StopFrontend(string.Empty, true);
      Utils.KillCurrentOemProcessByName(new string[6]
      {
        "HD-ApkHandler",
        "HD-Adb",
        "HD-RunApp",
        "HD-OBS",
        "BlueStacksTV",
        "HD-XapkHandler"
      }, (string) null);
      if (exitSelfProcess)
      {
        SysTray.s_SysTrayIcon.Dispose();
        try
        {
          SystemEvents.DisplaySettingsChanged -= new EventHandler(NotificationWindow.Instance.HandleDisplaySettingsChanged);
        }
        catch (Exception ex)
        {
          Logger.Error("Problem in unwinding events" + ex?.ToString());
        }
        Application.Exit();
      }
      else
        SysTray.SetTrayIconVisibility(false);
      Environment.Exit(0);
    }
  }
}
