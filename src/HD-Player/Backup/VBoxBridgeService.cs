// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VBoxBridgeService
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using BstkTypeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Player
{
  public class VBoxBridgeService : VBoxBridgeBase
  {
    private static VBoxBridgeService sInstance;
    private VBoxBridgeService.BooleanCallback mStartCallback;
    private VBoxBridgeService.BooleanCallback mStopCallback;
    private IConsole mConsole;
    private IMachine mMachine;
    private IDisplay mDisplay;
    private IProgress mPowerProgress;

    internal static VBoxBridgeService Instance
    {
      get
      {
        if (VBoxBridgeService.sInstance == null)
          VBoxBridgeService.sInstance = new VBoxBridgeService();
        return VBoxBridgeService.sInstance;
      }
    }

    public bool StartMachineAsync(
      string name,
      VBoxBridgeService.BooleanCallback startCont,
      VBoxBridgeService.BooleanCallback stopCont)
    {
      Logger.Info("{0} -> {1}", (object) MethodBase.GetCurrentMethod().Name, (object) name);
      bool success = false;
      this.mWorkQueue.DispatchSync((SerialWorkQueue.Work) (() =>
      {
        this.mStartCallback = startCont;
        this.mStopCallback = stopCont;
        if (!this.StartMachine_Begin(name))
        {
          Logger.Info("Cannot begin starting guest");
        }
        else
        {
          this.mWorkQueue.DispatchAsync(new SerialWorkQueue.Work(this.StartMachine_Tick));
          success = true;
        }
      }));
      return success;
    }

    public bool StopMachineAsync()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      bool success = false;
      this.mWorkQueue.DispatchSync((SerialWorkQueue.Work) (() =>
      {
        if (!this.StopMachine_Begin())
        {
          Logger.Info("Cannot begin stopping guest");
        }
        else
        {
          this.mWorkQueue.DispatchAsync(new SerialWorkQueue.Work(this.StopMachine_Tick));
          success = true;
        }
      }));
      return success;
    }

    private int ProcessEvent(IEvent ev, VBoxBridgeService.StateChangeCallback cb)
    {
      VBoxEventType type = ev.Type;
      if (type != VBoxEventType.VBoxEventType_OnMachineStateChanged)
      {
        Logger.Info("Unexpected event type {0}", (object) type);
        return -1;
      }
      IMachineStateChangedEvent stateChangedEvent = (IMachineStateChangedEvent) ev;
      if (stateChangedEvent == null)
      {
        Logger.Info("Cannot query interface");
        return -1;
      }
      if (stateChangedEvent.State == MachineState.MachineState_PoweredOff)
        this.InternalCloseMachine();
      cb(stateChangedEvent.State);
      if (stateChangedEvent.State != MachineState.MachineState_PoweredOff)
        return -1;
      this.mMachine = (IMachine) null;
      return 0;
    }

    public void RegisterStateChangeEvent(VBoxBridgeService.StateChangeCallback cb)
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      IEventSource es = this.mVirtualBox.EventSource;
      IEventListener listener = es.CreateListener();
      VBoxEventType[] aInteresting = new VBoxEventType[1]
      {
        VBoxEventType.VBoxEventType_OnMachineStateChanged
      };
      es.RegisterListener(listener, aInteresting, 0);
      new Thread((ThreadStart) (() =>
      {
        int num1;
        for (num1 = 0; num1 < 60; ++num1)
        {
          IEvent @event = es.GetEvent(listener, 1000);
          if (@event != null)
          {
            int num2 = this.ProcessEvent(@event, cb);
            es.EventProcessed(listener, @event);
            if (num2 == 0)
              break;
          }
          else
            Logger.Info("Waited for approx {0} seconds for PowerOff event", (object) (num1 + 1));
        }
        if (num1 == 60)
          cb(MachineState.MachineState_Stuck);
        es.UnregisterListener(listener);
      }))
      {
        IsBackground = true
      }.Start();
    }

    private IMediumAttachment[] GetMediumAttachments(ref string controllerName)
    {
      IMediumAttachment[] attachmentsOfController;
      try
      {
        attachmentsOfController = this.mMachine.GetMediumAttachmentsOfController("SCSI");
        if (attachmentsOfController == null || attachmentsOfController.Length == 0)
        {
          Logger.Debug("SCSI controller doesn't exist {0}. Trying to get attachments on SATA");
          attachmentsOfController = this.mMachine.GetMediumAttachmentsOfController("SATA");
          controllerName = "SATA";
        }
        else
        {
          Logger.Debug("Found SCSI controller");
          controllerName = "SCSI";
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Neither SCSI nor SATA controller exists {0} - DEBUG!!!", (object) ex.ToString());
        throw;
      }
      return attachmentsOfController;
    }

    private void CreateDifferencingDiskOfBaseMedium(IMedium srcMedium, string diffMediumName)
    {
      Logger.Info("In CreateDifferencingDiskofBaseMedium");
      try
      {
        string controllerName = (string) null;
        if (srcMedium.Type != MediumType.MediumType_Normal)
          throw new Exception("Unexpected medium type");
        if (srcMedium.Children.Length == 0 && srcMedium.MachineIds == null)
        {
          IMedium medium = this.mVirtualBox.CreateMedium("vdi", Path.Combine(Path.GetDirectoryName(srcMedium.Location), diffMediumName), AccessMode.AccessMode_ReadWrite, DeviceType.DeviceType_HardDisk);
          MediumVariant[] aVariant = new MediumVariant[1]
          {
            MediumVariant.MediumVariant_Diff
          };
          IProgress diffStorage = srcMedium.CreateDiffStorage(medium, aVariant);
          Logger.Info("Successfully created difference image when medium is not attached to any machine");
          while (diffStorage.Completed == 0)
            Thread.Sleep(10);
        }
        else
        {
          IMediumAttachment[] mediumAttachments = this.GetMediumAttachments(ref controllerName);
          for (int index = 0; index < mediumAttachments.Length; ++index)
          {
            if (mediumAttachments[index].Medium.Base.Name.Equals(srcMedium.Name))
            {
              int port = mediumAttachments[index].Port;
              int device = mediumAttachments[index].Device;
              IMedium medium = this.mVirtualBox.CreateMedium("vdi", Path.Combine(Path.GetDirectoryName(srcMedium.Location), diffMediumName), AccessMode.AccessMode_ReadWrite, DeviceType.DeviceType_HardDisk);
              MediumVariant[] aVariant = new MediumVariant[1]
              {
                MediumVariant.MediumVariant_Diff
              };
              this.mMachine.DetachDevice(controllerName, port, device);
              this.mMachine.SaveSettings();
              IProgress diffStorage = srcMedium.CreateDiffStorage(medium, aVariant);
              Logger.Info("Successfully created difference image for medium {0}", (object) srcMedium.Name);
              while (diffStorage.Completed == 0)
                Thread.Sleep(10);
              this.mMachine.AttachDevice(controllerName, port, device, DeviceType.DeviceType_HardDisk, medium);
              this.mMachine.SaveSettings();
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(" Exception in creating differencing storage. Err : " + ex.ToString());
      }
    }

    internal bool CheckIfAppPlayerRooted()
    {
      if (this.mVirtualBoxClient == null)
        return false;
      string controllerName = (string) null;
      IMediumAttachment[] mediumAttachments = this.GetMediumAttachments(ref controllerName);
      for (int index = 0; index < mediumAttachments.Length; ++index)
      {
        if (mediumAttachments[index].Port == 0)
        {
          IMedium medium = mediumAttachments[index].Medium;
          return !(medium.Name == "Root.vdi") || !(medium.Id == "fca296ce-8268-4ed7-a57f-d32ec11ab304") || medium.Type != MediumType.MediumType_Readonly;
        }
      }
      return false;
    }

    private void AddExtraDataKeysIfNotExists()
    {
      Logger.Info("In AddExtraDataKeysIfNotExists");
      List<string> list = ((IEnumerable<string>) this.mMachine.GetExtraDataKeys()).ToList<string>();
      if (!list.Contains("VBoxInternal/Devices/bstsensor/0/PCIBusNo"))
      {
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIBusNo", "0");
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIDeviceNo", "15");
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIFunctionNo", "0");
      }
      else
        Logger.Info("bstsensor is already present");
      if (list.Contains("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo") && this.mMachine.GetExtraData("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo") != "16")
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo", "16");
      if (!list.Contains("VBoxInternal/Devices/bstaudio/0/PCIBusNo"))
      {
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIBusNo", "0");
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIDeviceNo", "9");
        this.mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIFunctionNo", "0");
      }
      else
        Logger.Info("bstaudio is already present");
      this.mMachine.SaveSettings();
    }

    private void DisableAudioAdapterSettingIfEnabled()
    {
      Logger.Info("In DisableAudioAdapterSettingIfEnabled");
      if (this.mMachine.AudioAdapter.AudioDriver == AudioDriverType.AudioDriverType_Null)
        return;
      this.mMachine.AudioAdapter.AudioDriver = AudioDriverType.AudioDriverType_Null;
      this.mMachine.AudioAdapter.Enabled = 0;
      this.mMachine.SaveSettings();
    }

    private void CreateDifferencingImagesIfRequired()
    {
      string str1 = "Data.vdi";
      string str2 = Path.Combine(RegistryStrings.DataDir, "Android\\" + str1);
      if (!File.Exists(str2))
      {
        Logger.Info("Medium {0} doesn't exist, mediumPath is {1}", (object) str1, (object) str2);
      }
      else
      {
        IMedium srcMedium = this.mVirtualBox.OpenMedium(str2, DeviceType.DeviceType_HardDisk, AccessMode.AccessMode_ReadWrite, 0);
        string path = str1;
        string directoryName = Path.GetDirectoryName(str2);
        string diffMediumName = Path.GetFileNameWithoutExtension(path) + "_0" + Path.GetExtension(path);
        string path2 = diffMediumName;
        if (!File.Exists(Path.Combine(directoryName, path2)))
        {
          Logger.Info("Difference image {0} don't exist. Creating now!", (object) diffMediumName);
          this.CreateDifferencingDiskOfBaseMedium(srcMedium, diffMediumName);
        }
        else
          Logger.Info("Differencing disk {0} already exists", (object) diffMediumName);
      }
    }

    private bool StartMachine_Begin(string name)
    {
      Logger.Info("In StartMachine_Begin");
      RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", RegistryManager.Instance.CurrentEngine, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
      this.SerialQueueCheck();
      try
      {
        this.InternalOpenMachine(name);
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot open virtual machine");
        Logger.Info(ex.ToString());
        return false;
      }
      string val = (string) null + this.mMachine.SharedFolders[0].Name;
      for (int index = 1; index < this.mMachine.SharedFolders.Length; ++index)
        val = val + "," + this.mMachine.SharedFolders[index].Name;
      RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("SF", val, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
      if (RegistryManager.Instance.Guest[name].FixVboxConfig)
      {
        this.AddExtraDataKeysIfNotExists();
        this.DisableAudioAdapterSettingIfEnabled();
        RegistryManager.Instance.Guest[name].FixVboxConfig = false;
      }
      Logger.Info("Setting machine memory to {0} ", (object) RegistryManager.Instance.DefaultGuest.Memory);
      this.mMachine.MemorySize = (uint) RegistryManager.Instance.DefaultGuest.Memory;
      int vcpUs = RegistryManager.Instance.DefaultGuest.VCPUs;
      try
      {
        if (vcpUs <= 0 || vcpUs > 32)
        {
          this.mPowerProgress = this.mConsole.PowerUp();
        }
        else
        {
          if ((long) this.mMachine.CPUCount != (long) vcpUs)
          {
            Logger.Info("Overriding VCPUs({0}) from registry", (object) vcpUs);
            this.mMachine.CPUCount = (uint) vcpUs;
          }
          if (string.Compare(RegistryManager.Instance.EnginePreference, "raw", true) == 0)
          {
            Logger.Info("Overriding cpu count to 1 for raw mode");
            this.mMachine.CPUCount = 1U;
            this.mMachine.SetHWVirtExProperty(HWVirtExPropertyType.HWVirtExPropertyType_Enabled, 0);
            RegistryManager.Instance.CurrentEngine = EngineState.raw.ToString();
            MultiInstanceUtils.SetDeviceCapsRegistry("Forcefully setting to raw", EngineState.raw.ToString());
            RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", EngineState.raw.ToString(), RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
          }
          this.mPowerProgress = this.mConsole.PowerUp();
        }
        TimelineStatsSender.HandleEngineBootEvent(EngineStatsEvent.vm_launched.ToString());
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot power up virtual machine");
        Logger.Info(ex.ToString());
        return false;
      }
      return true;
    }

    private bool StopMachine_Begin()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      try
      {
        this.mPowerProgress = this.mConsole.PowerDown();
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot power down virtual machine:");
        Logger.Info(ex.ToString());
        return false;
      }
      return true;
    }

    private void InternalCloseMachine()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      if (this.mSession == null || this.mSession.State == SessionState.SessionState_Unlocked)
        return;
      this.mSession.UnlockMachine();
      Logger.Info("Successfully unlocked the machine");
    }

    private void InternalOpenMachine(string name)
    {
      Logger.Info("In InternalOpenMachine with vmName: {0}", (object) name);
      this.SerialQueueCheck();
      IMachine machine = (IMachine) null;
      try
      {
        machine = this.mVirtualBox.FindMachine(name);
        Logger.Info("Current machine state: " + machine.State.ToString());
        machine.LockMachine(this.mSession, LockType.LockType_VM);
      }
      catch (InvalidCastException ex)
      {
        Logger.Warning("exception in lock vm machine: {0}", (object) ex);
        ComRegistration.Register();
        Logger.Info("Restarting player in case of invalid cast exception after Registering COM components");
        HTTPUtils.SendRequestToClient("restartFrontend", new Dictionary<string, string>()
        {
          {
            "vmname",
            name
          }
        }, name, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      this.mConsole = this.mSession.Console;
      this.mMachine = this.mConsole.Machine;
      this.mDisplay = this.mConsole.Display;
      Logger.Info("Shared folders for the VM are");
      for (int index = 0; index < this.mMachine.SharedFolders.Length; ++index)
        Logger.Info("SF[{0}] -> {1}", (object) index, (object) machine.SharedFolders[index].Name);
    }

    private void StartMachine_Tick()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      try
      {
        if (this.IsPowerOperationPending())
          this.mWorkQueue.DispatchAfter(500.0, new SerialWorkQueue.Work(this.StartMachine_Tick));
        else if (this.mPowerProgress.ResultCode == 0)
        {
          TimelineStatsSender.HandleEngineBootEvent(EngineStatsEvent.vm_running.ToString());
          this.StartMachine_End();
          this.mStartCallback(true);
          if (this.mMachine.GetHWVirtExProperty(HWVirtExPropertyType.HWVirtExPropertyType_Enabled) == 0)
            return;
          MultiInstanceUtils.SetDeviceCapsRegistry("", EngineState.plus.ToString(), CpuHvmState.True, BiosHvmState.True);
        }
        else
        {
          IVirtualBoxErrorInfo errorInfo = this.mPowerProgress.ErrorInfo;
          Logger.Info("IProgress:ErrInfo: mPowerProgress.ResultCode={0:X}, ResultDetail={1:X}, mPowerProgress.ErrorInfo.Text={2}", (object) errorInfo.ResultCode, (object) errorInfo.ResultDetail, (object) errorInfo.Text);
          bool flag1 = errorInfo.Text.Contains("VERR_VMX_NO_VMX") || errorInfo.Text.Contains("VERR_SVM_NO_SVM");
          CpuHvmState cpuHvm = flag1 ? CpuHvmState.False : CpuHvmState.True;
          bool flag2 = errorInfo.Text.Contains("VERR_VMX_MSR_VMXON_DISABLED") || errorInfo.Text.Contains("VERR_SVM_DISABLED") || (errorInfo.Text.Contains("VERR_VMX_MSR_ALL_VMX_DISABLED") || errorInfo.Text.Contains("VERR_VMX_MSR_VMX_DISABLED")) || errorInfo.Text.Contains("VERR_VMX_INVALID_VXMON_PTR") || errorInfo.Text.Contains("VERR_VMX_IN_VMX_ROOT_MODE");
          BiosHvmState biosHvm = flag2 ? BiosHvmState.False : BiosHvmState.True;
          if (errorInfo.Text.Contains("VERR_UNSUPPORTED_CPU"))
          {
            Logger.Info("UnSupported CPU error came, so quiting frontend with MessageBox");
            this.QuitFrontEnd("Cannot start virtual machine");
          }
          else if (errorInfo.Text.Contains("VERR_VM_DRIVER_NOT_INSTALLED"))
          {
            Logger.Error("Driver not installed error, exiting with {0}", (object) PlayerErrorCodes.VBOX_DRIVER_NOT_INSTALLED);
            AndroidBootUp.SendBootFailureLogs();
            Environment.Exit(-7);
          }
          else
          {
            Logger.Info("VM Name: " + BlueStacks.Common.Strings.CurrentDefaultVmName);
            if (!(flag1 | flag2))
              throw new Exception("Unexpected Power operation failure...");
            Logger.Info("Restarting service in raw mode...");
            RegistryManager.Instance.CurrentEngine = EngineState.raw.ToString();
            MultiInstanceUtils.SetDeviceCapsRegistry(errorInfo.Text, EngineState.raw.ToString(), cpuHvm, biosHvm);
            RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", RegistryManager.Instance.CurrentEngine, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
            if (!Oem.Instance.IsAndroid64Bit)
            {
              Logger.Info("Closing the previous vbox instance before starting raw mode...");
              try
              {
                this.InternalCloseMachine();
                this.DisConnect();
              }
              catch (Exception ex)
              {
                Logger.Error("failed in closing/disconnect: " + ex.ToString());
                return;
              }
              Logger.Info("Opening up new connection...");
              try
              {
                this.Connect();
                this.InternalOpenMachine(MultiInstanceStrings.VmName);
              }
              catch (Exception ex)
              {
                Logger.Error("failed in opening/connect: " + ex.ToString());
                return;
              }
              Logger.Info("Overriding cpu count to 1 for raw mode");
              this.mMachine.CPUCount = 1U;
              this.mMachine.SetHWVirtExProperty(HWVirtExPropertyType.HWVirtExPropertyType_Enabled, 0);
              this.mPowerProgress = this.mConsole.PowerUp();
              this.mWorkQueue.DispatchAfter(100.0, new SerialWorkQueue.Work(this.StartMachine_Tick));
            }
            else
              Environment.Exit(-10);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot start virtual machine");
        Logger.Info(ex.ToString());
        AndroidBootUp.HandleBootError();
      }
    }

    private void QuitFrontEnd(string plus_failure_reason)
    {
      try
      {
        AndroidBootUp.HandleBootError();
      }
      catch (Exception ex)
      {
        Logger.Info(ex.ToString());
      }
    }

    private void StopMachine_Tick()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      try
      {
        if (this.IsPowerOperationPending())
        {
          this.mWorkQueue.DispatchAfter(100.0, new SerialWorkQueue.Work(this.StopMachine_Tick));
        }
        else
        {
          this.StopMachine_End();
          this.InternalCloseMachine();
          this.mStopCallback(true);
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot stop virtual machine");
        Logger.Info(ex.ToString());
        this.mStopCallback(false);
      }
    }

    private bool IsPowerOperationPending()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      return this.mPowerProgress.Completed == 0;
    }

    private void StartMachine_End()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.CompletePowerOperation();
    }

    private void StopMachine_End()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.CompletePowerOperation();
    }

    private void CompletePowerOperation()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.mPowerProgress.WaitForCompletion(-1);
      int resultCode = this.mPowerProgress.ResultCode;
      if (resultCode != 0)
        throw new COMException("Cannot get power progress", resultCode);
    }

    public bool AddNetworkRedirect(bool isTcp, int guestPort, int hostPort)
    {
      Logger.Info("Adding network redirect {0} guestPort({1}) hostPort({2})", (object) isTcp, (object) guestPort, (object) hostPort);
      NATProtocol aProto = isTcp ? NATProtocol.NATProtocol_TCP : NATProtocol.NATProtocol_UDP;
      INATEngine natEngine = this.mMachine.GetNetworkAdapter(0U).NATEngine;
      if (RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].AllowRemoteAccess == "May I please have remote access?")
        natEngine.AddRedirect((string) null, aProto, (string) null, (ushort) hostPort, (string) null, (ushort) guestPort);
      else
        natEngine.AddRedirect((string) null, aProto, "127.0.0.1", (ushort) hostPort, (string) null, (ushort) guestPort);
      if (!isTcp)
      {
        Logger.Info("Setting hostforwardsensorport");
        RegistryManager.Instance.DefaultGuest.HostForwardSensorPort = hostPort;
      }
      return true;
    }

    public delegate void BooleanCallback(bool success);

    public delegate void StateChangeCallback(MachineState state);
  }
}
