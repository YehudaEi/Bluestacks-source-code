// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.StateMachine
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using BstkTypeLib;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace BlueStacks.Player
{
  internal class StateMachine
  {
    internal static int mForceShutdownDueTime = 3000;
    private string mVmName;
    private SerialWorkQueue mWorkQueue;
    private EventWaitHandle mTerminationEvent;
    private StateMachine.State mState;
    private StateMachine.BooleanCallback mStartCallback;
    private StateMachine.VoidCallback mRunningCallback;
    private Timer mShutdownTimer;

    public StateMachine(string vmName)
    {
      this.mVmName = vmName;
      this.mWorkQueue = new SerialWorkQueue(nameof (StateMachine))
      {
        ExceptionHandler = new SerialWorkQueue.ExceptionHandlerCallback(this.HandleWorkQueueException)
      };
      this.mWorkQueue.Start();
      this.mTerminationEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
      this.mState = StateMachine.State.Init;
    }

    private void HandleWorkQueueException(Exception exc)
    {
      Logger.Info("Exception in StateMachine work queue:");
      Logger.Info(exc.ToString());
      this.mWorkQueue.DispatchAsync((SerialWorkQueue.Work) (() => this.EnterStateError()));
    }

    private void SerialQueueCheck()
    {
      if (!this.mWorkQueue.IsCurrentWorkQueue())
        throw new ApplicationException("Cannot run StateMachine on this thread");
    }

    public StateMachine.VoidCallback RunningCallback
    {
      set
      {
        this.mRunningCallback = value;
      }
    }

    public void Start(StateMachine.BooleanCallback startCallback)
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      Exception exc = (Exception) null;
      this.mWorkQueue.DispatchSync((SerialWorkQueue.Work) (() =>
      {
        if (this.mState != StateMachine.State.Init)
        {
          exc = (Exception) new ApplicationException("Cannot start state machine in state " + this.mState.ToString());
        }
        else
        {
          this.mStartCallback = startCallback;
          this.EnterStateStarting();
        }
      }));
      if (exc != null)
        throw exc;
    }

    public void RequestTermination()
    {
      this.mWorkQueue.DispatchAsync((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("{0} -> {1}", (object) MethodBase.GetCurrentMethod().Name, (object) this.mState);
        switch (this.mState)
        {
          case StateMachine.State.Init:
            this.EnterStateError();
            break;
          case StateMachine.State.Starting:
            this.EnterStateStartingCancel();
            break;
          case StateMachine.State.WaitingForNetwork:
          case StateMachine.State.Running:
            this.EnterStateShuttingDown();
            break;
        }
      }));
    }

    public void WaitForTermination()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.mTerminationEvent.WaitOne();
    }

    private void EnterStateStarting()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.mState = StateMachine.State.Starting;
      try
      {
        VBoxBridgeService.Instance.Connect();
      }
      catch (Exception ex1)
      {
        Logger.Info("Cannot connect VBoxBridge");
        Logger.Info(ex1.ToString());
        try
        {
          ComRegistration.Register();
          Logger.Info("Reconnecting to VBoxBridge");
          VBoxBridgeService.Instance.Connect();
        }
        catch (Exception ex2)
        {
          Logger.Info("Got exception {0} while re-registering COM", (object) ex2.ToString());
          AndroidBootUp.HandleBootError();
        }
      }
      if (VBoxBridgeService.Instance.StartMachineAsync(this.mVmName, (VBoxBridgeService.BooleanCallback) (success => this.mWorkQueue.DispatchAsync((SerialWorkQueue.Work) (() => this.StartMachineCompletion(success)))), (VBoxBridgeService.BooleanCallback) (success => this.mWorkQueue.DispatchAsync((SerialWorkQueue.Work) (() => this.StopMachineCompletion(success))))))
        return;
      Logger.Info("Cannot begin starting guest");
      this.EnterStateError();
      AndroidBootUp.HandleBootError();
    }

    private void StartMachineCompletion(bool success)
    {
      Logger.Info("Start callback -> {0}", (object) success);
      if (success)
      {
        switch (this.mState)
        {
          case StateMachine.State.Starting:
            this.EnterStateWaitingForNetwork();
            break;
          case StateMachine.State.StartingCancel:
            this.EnterStateStopping();
            break;
          case StateMachine.State.WaitingForNetwork:
            Logger.Info("Start machine continuationcalled in state {0}", (object) this.mState);
            this.mStartCallback = (StateMachine.BooleanCallback) null;
            return;
          default:
            Logger.Info("Start machine continuation called in state " + this.mState.ToString());
            break;
        }
      }
      else
        this.EnterStateError();
      StateMachine.BooleanCallback mStartCallback = this.mStartCallback;
      if (mStartCallback != null)
        mStartCallback(success);
      this.mStartCallback = (StateMachine.BooleanCallback) null;
    }

    private void StopMachineCompletion(bool success)
    {
      Logger.Info("Start callback -> {0}", (object) success);
      if (success)
      {
        if (this.mState == StateMachine.State.Stopping)
          this.EnterStateStopped();
        else
          Logger.Info("Stop machine in state: " + this.mState.ToString());
      }
      else
        this.EnterStateError();
      this.mTerminationEvent.Set();
    }

    private void EnterStateStartingCancel()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
    }

    private void EnterStateWaitingForNetwork()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.mState = StateMachine.State.WaitingForNetwork;
      this.mWorkQueue.DispatchAfter(1.0, (SerialWorkQueue.Work) (() =>
      {
        if (this.mState != StateMachine.State.WaitingForNetwork)
          return;
        this.EnterStateRunning();
      }));
    }

    private void EnterStateRunning()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.mState = StateMachine.State.Running;
      StateMachine.VoidCallback mRunningCallback = this.mRunningCallback;
      if (mRunningCallback == null)
        return;
      mRunningCallback();
    }

    private void EnterStateShuttingDown()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.mState = StateMachine.State.ShuttingDown;
      VBoxBridgeService.Instance.RegisterStateChangeEvent((VBoxBridgeService.StateChangeCallback) (newMstate => this.mWorkQueue.DispatchSync((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("Callback for state {0} called...", (object) newMstate);
        switch (newMstate)
        {
          case MachineState.MachineState_PoweredOff:
            this.mShutdownTimer.Change(-1, -1);
            this.StopMachineCompletion(true);
            break;
          case MachineState.MachineState_Stuck:
            this.StopMachineCompletion(false);
            break;
          case MachineState.MachineState_Stopping:
            this.mState = StateMachine.State.Stopping;
            break;
          default:
            Logger.Info("Invalid machine state");
            break;
        }
      }))));
      if (this.mShutdownTimer == null)
      {
        this.mShutdownTimer = new Timer((TimerCallback) (x => this.ForceShutdown()), (object) null, StateMachine.mForceShutdownDueTime, -1);
        Logger.Info("Shutdown timer started with due time {0}", (object) StateMachine.mForceShutdownDueTime);
      }
      using (InputManagerProxy inputManagerProxy = new InputManagerProxy(Process.GetCurrentProcess().Id))
        inputManagerProxy.SendControlShutdown();
    }

    private void ForceShutdown()
    {
      Logger.Warning("No callback recieved, exiting player");
      this.mShutdownTimer.Change(-1, -1);
      Environment.Exit(-9);
    }

    private void EnterStateStopping()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      this.mState = StateMachine.State.Stopping;
      if (VBoxBridgeService.Instance.StopMachineAsync())
        return;
      Logger.Info("Cannot stop guest");
      this.EnterStateError();
    }

    private void EnterStateStopped()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      VBoxBridgeService.Instance.DisConnect();
      this.mState = StateMachine.State.Stopped;
    }

    private void EnterStateError()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      this.SerialQueueCheck();
      StateMachine.BooleanCallback mStartCallback = this.mStartCallback;
      if (mStartCallback != null)
        mStartCallback(false);
      this.mState = StateMachine.State.Error;
      this.mTerminationEvent.Set();
    }

    private enum State
    {
      Init,
      Starting,
      StartingCancel,
      WaitingForNetwork,
      Running,
      ShuttingDown,
      Stopping,
      Stopped,
      Error,
    }

    public delegate void VoidCallback();

    public delegate void BooleanCallback(bool success);
  }
}
