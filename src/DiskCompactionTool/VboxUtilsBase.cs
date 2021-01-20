// Decompiled with JetBrains decompiler
// Type: BlueStacks.DiskCompactionTool.VboxUtilsBase
// Assembly: DiskCompactionTool, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0D774D0F-793E-496D-B768-12A2EDB900B5
// Assembly location: C:\Program Files\BlueStacks\DiskCompactionTool.exe

using BlueStacks.Common;
using BstkTypeLib;
using System;

namespace BlueStacks.DiskCompactionTool
{
  public class VboxUtilsBase
  {
    protected static IVirtualBoxClient VirtualBoxClient { get; set; }

    public IVirtualBox VirtualBox { get; set; }

    public Session Session { get; set; }

    public IConsole Console { get; set; }

    public IMachine Machine { get; set; }

    public int VmId { get; set; }

    protected bool Init()
    {
      Logger.Info("In VboxUtilsBase init");
      try
      {
        if (this.VirtualBox != null)
          return true;
        VboxUtilsBase.VirtualBoxClient = (IVirtualBoxClient) new VirtualBoxClientClass();
        this.VirtualBox = (IVirtualBox) VboxUtilsBase.VirtualBoxClient.VirtualBox;
      }
      catch (Exception ex1)
      {
        Logger.Warning("Virtual box init failed, error : {0}", (object) ex1.ToString());
        try
        {
          ComRegistration.Register();
          Logger.Info("Retrying to init VBox");
          if (this.VirtualBox != null)
            return true;
          VboxUtilsBase.VirtualBoxClient = (IVirtualBoxClient) new VirtualBoxClientClass();
          this.VirtualBox = (IVirtualBox) VboxUtilsBase.VirtualBoxClient.VirtualBox;
        }
        catch (Exception ex2)
        {
          Logger.Error("Virtual box init failed, error : {0}", (object) ex2.ToString());
          return false;
        }
      }
      return true;
    }

    protected void InternalOpenMachine(string vmName)
    {
      Logger.Info("In InternalOpenMachine");
      if (this.VirtualBox == null)
        this.Init();
      this.Session = VboxUtilsBase.VirtualBoxClient.Session;
      this.VirtualBox.FindMachine(vmName).LockMachine(this.Session, LockType.LockType_VM);
      this.Console = this.Session.Console;
      this.Machine = this.Console.Machine;
    }

    protected void InternalCloseMachine()
    {
      Logger.Info("In InternalCloseMachine");
      if (this.Session != null)
        Logger.Info("Session state is {0}", (object) this.Session.State);
      if (this.Session == null || this.Session.State == SessionState.SessionState_Unlocked)
        return;
      try
      {
        this.Session.UnlockMachine();
        Logger.Info("Successfully Unlocked machine");
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not unlock the machine due to exception {0}", (object) ex.ToString());
      }
    }

    protected IMediumAttachment[] GetMediumAttachments(ref string controllerName)
    {
      IMediumAttachment[] attachmentsOfController = this.Machine.GetMediumAttachmentsOfController("SCSI");
      if (attachmentsOfController == null || attachmentsOfController.Length == 0)
      {
        attachmentsOfController = this.Machine.GetMediumAttachmentsOfController("SATA");
        controllerName = "SATA";
      }
      else
        controllerName = "SCSI";
      return attachmentsOfController;
    }
  }
}
