// Decompiled with JetBrains decompiler
// Type: BlueStacks.DiskCompactionTool.DiskCompactionHelper
// Assembly: DiskCompactionTool, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0D774D0F-793E-496D-B768-12A2EDB900B5
// Assembly location: C:\Program Files\BlueStacks\DiskCompactionTool.exe

using BlueStacks.Common;
using BstkTypeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BlueStacks.DiskCompactionTool
{
  public class DiskCompactionHelper : VboxUtilsBase
  {
    private static object syncRoot = new object();
    internal static int sDiskCompactionStatusCode = 0;
    private MainWindow ParentWindow = (MainWindow) null;
    private const string SU_PATH = "/system/xbin/bstk/su";
    private System.Windows.Forms.Timer progressTimer;
    private static DiskCompactionHelper mInstance;
    private BackgroundWorker mBGCompactionWorker;

    public static DiskCompactionHelper Instance
    {
      get
      {
        if (DiskCompactionHelper.mInstance == null)
        {
          lock (DiskCompactionHelper.syncRoot)
          {
            if (DiskCompactionHelper.mInstance == null)
              DiskCompactionHelper.mInstance = new DiskCompactionHelper();
          }
        }
        return DiskCompactionHelper.mInstance;
      }
    }

    public void InitProperties(MainWindow parentWindow)
    {
      this.ParentWindow = parentWindow;
    }

    public BackgroundWorker BGCompactionWorker
    {
      get
      {
        if (this.mBGCompactionWorker == null)
        {
          this.mBGCompactionWorker = new BackgroundWorker();
          this.mBGCompactionWorker.DoWork += new DoWorkEventHandler(this.BGCompactionWorker_DoWork);
          this.mBGCompactionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGCompactionWorker_RunWorkerCompleted);
        }
        return this.mBGCompactionWorker;
      }
    }

    internal void StartCompaction()
    {
      if (this.BGCompactionWorker.IsBusy)
        return;
      this.BGCompactionWorker.RunWorkerAsync();
    }

    private void BGCompactionWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        this.ParentWindow.UpdateProgress(1.0);
        Utils.KillCurrentOemProcessByName("HD-MultiInstanceManager", (string) null);
        Utils.RunHDQuit(true, false, false, "bgp");
        this.ParentWindow.UpdateProgress(5.0);
        DiskCompactionHelper.sDiskCompactionStatusCode = this.MergeAndCompact();
        Stats.SendMiscellaneousStatsSync("DiskCompactionStats", "DiskCompaction Completed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, App.sOpt.vmname, ((DiskCompactionToolCodes) DiskCompactionHelper.sDiskCompactionStatusCode).ToString(), (string) null, (string) null, "Android", 0);
        if (DiskCompactionHelper.sDiskCompactionStatusCode != 0)
          return;
        this.ParentWindow.UpdateProgress(100.0);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in compaction worker :" + ex.ToString());
        DiskCompactionHelper.sDiskCompactionStatusCode = -2;
      }
    }

    private void BGCompactionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Logger.Info("In RunWorkerCompleted");
      if (!App.sOpt.s)
      {
        if (DiskCompactionHelper.sDiskCompactionStatusCode == 0)
        {
          if (App.sOpt.relaunch)
            this.ParentWindow.LaunchBlueStacks();
          else
            this.ParentWindow.UpdateUi(MainWindow.UiStates.success);
        }
        else
          this.ParentWindow.UpdateUi(MainWindow.UiStates.error);
      }
      else
        App.ExitApplication(DiskCompactionHelper.sDiskCompactionStatusCode);
    }

    internal int MergeAndCompact()
    {
      if (!this.Init())
        return -4;
      string uuid = (string) null;
      string str = "Android";
      if (!string.IsNullOrEmpty(App.sOpt.vmname))
        str = App.sOpt.vmname;
      if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(str))
        return -9;
      IMedium baseMedium = (IMedium) null;
      if (!this.MergeToBase(str, ref uuid, ref baseMedium))
        return -5;
      string name = baseMedium.Name;
      this.ParentWindow.UpdateProgress(18.0);
      Utils.StartHiddenFrontend(str, "bgp");
      if (!Utils.WaitForBootComplete(str, "bgp"))
        return -6;
      this.ParentWindow.UpdateProgress(25.0);
      if (!this.FreeDiskSpace(str))
        return -7;
      Utils.StopFrontend(str, true);
      Thread.Sleep(2000);
      this.ParentWindow.UpdateProgress(98.0);
      return !this.CompactMedium(name, str, true) ? -8 : 0;
    }

    internal bool MergeToBase(string vm, ref string uuid, ref IMedium baseMedium)
    {
      Logger.Info("In MergeToBase");
      try
      {
        this.InternalOpenMachine(vm);
        string controllerName = (string) null;
        IMediumAttachment[] mediumAttachments = this.GetMediumAttachments(ref controllerName);
        int aControllerPort = 0;
        int aDevice = 0;
        IMedium medium1 = (IMedium) null;
        foreach (IMediumAttachment mediumAttachment in mediumAttachments)
        {
          Logger.Info("Attached mediums are " + mediumAttachment.Medium.Name + " " + mediumAttachment.Type.ToString());
          if (mediumAttachment.Medium.Name.Contains("Data"))
          {
            IMedium medium2 = mediumAttachment.Medium;
            medium1 = this.GetTargetMedium(medium2);
            baseMedium = medium1;
            Logger.Info("Found TargetMedium is =" + medium1.Name);
            if (medium1 == medium2)
            {
              Logger.Info("Disk not having a parent tree so need of merging");
              DiskCompactionHelper.mInstance.InternalCloseMachine();
              return true;
            }
            aControllerPort = mediumAttachment.Port;
            aDevice = mediumAttachment.Device;
            this.Machine.DetachDevice(controllerName, aControllerPort, aDevice);
            this.Machine.SaveSettings();
            this.InternalCloseMachine();
            Utils.GetVmIdFromVmName(vm);
            IProgress progress = medium2.MergeTo(medium1);
            double num = 5.0;
            while (progress.Completed == 0)
            {
              Thread.Sleep(200);
              this.ParentWindow.UpdateProgress(num + (double) (progress.Percent / 10U));
              Logger.Debug(string.Format("Merging : {0}%", (object) progress.Percent));
            }
            uuid = medium1.Id;
            break;
          }
        }
        DiskCompactionHelper.mInstance.InternalOpenMachine(vm);
        this.Machine.AttachDevice(controllerName, aControllerPort, aDevice, DeviceType.DeviceType_HardDisk, medium1);
        this.Machine.SaveSettings();
        this.InternalCloseMachine();
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in merging disk err:" + ex.ToString());
      }
      return false;
    }

    private bool FreeDiskSpace(string vm)
    {
      Logger.Info("In FreeDiskSpace");
      AdbCommandRunner runner = new AdbCommandRunner(vm);
      try
      {
        if (!runner.Connect(vm))
        {
          Logger.Warning("Cannot connect to guest");
          return false;
        }
        if (!runner.RunShellPrivileged("stop"))
          return false;
        runner.RunShellPrivileged("swapoff /data/swap_space");
        Thread.Sleep(2000);
        if (!runner.RunShellPrivileged("mount -o remount,ro /data"))
          return false;
        this.ParentWindow.UpdateProgress(30.0);
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          this.progressTimer = new System.Windows.Forms.Timer();
          this.progressTimer.Tick += new EventHandler(this.ProgressTimer_Tick);
          this.progressTimer.Interval = 6000;
          this.progressTimer.Start();
        }));
        if (!this.RunZeroFreeAndroid(runner))
          return false;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in freeing disk space err:" + ex.ToString());
        return false;
      }
      finally
      {
        runner.Dispose();
      }
      return true;
    }

    private void ProgressTimer_Tick(object sender, EventArgs e)
    {
      this.ParentWindow.UpdateProgress(Math.Min(this.ParentWindow.mProgressBar.Value + 2.0, 94.0));
      Logger.Debug("value: " + this.ParentWindow.mProgressBar.Value.ToString());
    }

    private bool RunZeroFreeAndroid(AdbCommandRunner runner)
    {
      string str = "zerofree -v /dev/block/sdb1";
      bool flag = this.RunCmdAndUpdateProgress(string.Format("-s 127.0.0.1:{0} shell {1} -c {2}", (object) runner.Port, (object) "/system/xbin/bstk/su", (object) str), runner.Path, true);
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.progressTimer.Stop();
        this.progressTimer.Dispose();
      }));
      return flag;
    }

    private bool RunCmdAndUpdateProgress(string cmd, string filePath, bool retry)
    {
      Logger.Info("ADB CMD: " + cmd);
      Process process = new Process();
      process.StartInfo.FileName = filePath;
      process.StartInfo.Arguments = cmd;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.CreateNoWindow = true;
      process.OutputDataReceived += (DataReceivedEventHandler) ((sender, evt) => Logger.Info("ADB OUT: " + evt.Data));
      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      process.WaitForExit();
      int num = process.ExitCode;
      Logger.Info("ADB EXIT: " + num.ToString());
      if (num != 0 && retry)
      {
        Thread.Sleep(4000);
        num = this.RunCmdAndUpdateProgress(cmd, filePath, false) ? 0 : 1;
      }
      return num == 0;
    }

    private bool CompactMedium(string mediumName, string vm, bool retry = true)
    {
      int num = Utils.RunCmd(Path.Combine(RegistryStrings.InstallDir, "BstkVMMgr.exe"), string.Format("modifymedium disk \"{0}\" --compact", (object) Path.Combine(RegistryStrings.DataDir, Path.Combine(vm, mediumName))), (string) null).ExitCode;
      if (num != 0 && retry)
        num = this.CompactMedium(mediumName, vm, false) ? 0 : 1;
      return num == 0;
    }

    private IMedium GetTargetMedium(IMedium medium)
    {
      IMedium parent = medium.Parent;
      return ((IEnumerable<IMedium>) parent.Children).Count<IMedium>() > 1 || ((IEnumerable<IMedium>) parent.Children).Count<IMedium>() == 1 && parent.Name == "Data.vdi" ? medium : this.GetTargetMedium(parent);
    }
  }
}
