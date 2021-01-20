// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VBoxBridgeBase
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using BstkTypeLib;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  public class VBoxBridgeBase
  {
    protected SerialWorkQueue mWorkQueue;
    protected IVirtualBoxClient mVirtualBoxClient;
    protected IVirtualBox mVirtualBox;
    protected Session mSession;

    protected VBoxBridgeBase()
    {
      this.mWorkQueue = new SerialWorkQueue("VBoxBridge")
      {
        ExceptionHandler = new SerialWorkQueue.ExceptionHandlerCallback(this.HandleWorkQueueException)
      };
      this.mWorkQueue.Start();
    }

    private void HandleWorkQueueException(Exception exc)
    {
      Logger.Info("Exception in VBoxBridge work queue:");
      Logger.Info("{0}", (object) exc.ToString());
      Environment.Exit(-8);
    }

    protected void SerialQueueCheck()
    {
      if (!this.mWorkQueue.IsCurrentWorkQueue())
        throw new ApplicationException("Cannot run VBoxBridge on this thread");
    }

    public void Connect()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.mVirtualBoxClient = (IVirtualBoxClient) new VirtualBoxClientClass();
      this.mVirtualBox = (IVirtualBox) this.mVirtualBoxClient.VirtualBox;
      Logger.Info("Version: " + this.mVirtualBox.Version);
      this.mSession = this.mVirtualBoxClient.Session;
    }

    public void DisConnect()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      Marshal.FinalReleaseComObject((object) this.mVirtualBoxClient);
      this.mSession = (Session) null;
      this.mVirtualBox = (IVirtualBox) null;
      this.mVirtualBoxClient = (IVirtualBoxClient) null;
    }
  }
}
