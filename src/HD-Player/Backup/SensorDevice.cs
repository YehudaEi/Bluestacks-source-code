// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.SensorDevice
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class SensorDevice
  {
    private const string NATIVE_DLL = "HD-Sensor-Native.dll";
    private VmMonitor mMonitor;
    private SensorDevice.LoggerCallback mLogger;
    private bool mRunning;
    private Dictionary<SensorDevice.Type, SensorDevice.State> mStateMap;
    private Thread mAccelerometerThread;
    private SensorDevice.AccelerometerCallback mAccelerometerCallback;
    private SensorDevice.EnableHandler mEnableHandler;
    private SensorDevice.SetDelayHandler mSetDelayHandler;
    private SerialWorkQueue mSerialQueue;
    private static SensorDevice mSensorDevice;

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void LoggerSetCallback(SensorDevice.LoggerCallback cb);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void SensorMsgInit(
      SensorDevice.EnableHandler enableHandler,
      SensorDevice.SetDelayHandler setDelayHandler);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void SensorMsgHandleMessage(IntPtr msg);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void SensorMsgSendReattach(
      SensorDevice.Type sensor,
      VmMonitor.SendMessage handler);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void SensorMsgSendAccelerometerEvent(
      float x,
      float y,
      float z,
      VmMonitor.SendMessage handler);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void SensorMsgSendStopReceiver(VmMonitor.SendMessage handler);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern bool HostInit();

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void HostSetOrientation(int orientation);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern bool HostSetupAccelerometer(SensorDevice.AccelerometerCallback callback);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void HostEnableSensor(SensorDevice.Type sensor, bool enable);

    [DllImport("HD-Sensor-Native.dll")]
    private static extern void HostSetSensorPeriod(SensorDevice.Type sensor, uint msec);

    internal static SensorDevice Instance
    {
      get
      {
        if (SensorDevice.mSensorDevice == null)
          SensorDevice.mSensorDevice = new SensorDevice();
        return SensorDevice.mSensorDevice;
      }
    }

    public SensorDevice()
    {
      this.mStateMap = new Dictionary<SensorDevice.Type, SensorDevice.State>()
      {
        [SensorDevice.Type.Accelerometer] = (SensorDevice.State) new SensorDevice.AccelerometerState()
      };
    }

    public void StartThreads()
    {
      this.mAccelerometerThread = new Thread(new ThreadStart(this.AccelerometerThreadEntry))
      {
        IsBackground = true
      };
      this.mAccelerometerThread.Start();
    }

    public void Start(string vmName)
    {
      this.mRunning = true;
      this.mLogger = (SensorDevice.LoggerCallback) (msg => Logger.Info("SensorDevice: " + msg));
      SensorDevice.LoggerSetCallback(this.mLogger);
      this.UpdateOrientation((object) null, (EventArgs) null);
      SystemEvents.DisplaySettingsChanged += new EventHandler(this.UpdateOrientation);
      this.mSerialQueue = new SerialWorkQueue();
      this.mSerialQueue.Start();
      EventWaitHandle evt = (EventWaitHandle) new ManualResetEvent(false);
      this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        this.SetupHostSensors();
        evt.Set();
      }));
      evt.WaitOne();
      this.mEnableHandler = new SensorDevice.EnableHandler(this.EnableHandlerImpl);
      this.mSetDelayHandler = new SensorDevice.SetDelayHandler(this.SetDelayHandlerImpl);
      SensorDevice.SensorMsgInit(this.mEnableHandler, this.mSetDelayHandler);
      this.mMonitor = VmMonitor.Connect(vmName, 0U);
      this.mMonitor.StartReceiver(new VmMonitor.ReceiverCallback(SensorDevice.SensorMsgHandleMessage));
      SensorDevice.SensorMsgSendReattach(SensorDevice.Type.Accelerometer, new VmMonitor.SendMessage(this.SendMessage));
    }

    public void Stop()
    {
      this.mRunning = false;
      SystemEvents.DisplaySettingsChanged -= new EventHandler(this.UpdateOrientation);
      try
      {
        SensorDevice.SensorMsgSendStopReceiver(new VmMonitor.SendMessage(this.SendShutdownMessage));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending Stop receiver msg. Err : " + ex?.ToString());
      }
      this.mMonitor.StopReceiver();
      this.mMonitor.Close();
      this.mMonitor = (VmMonitor) null;
    }

    private SensorDevice.State LookupState(SensorDevice.Type sensor)
    {
      return this.mStateMap != null && this.mStateMap.ContainsKey(sensor) ? this.mStateMap[sensor] : (SensorDevice.State) null;
    }

    private void SetupHostSensors()
    {
      if (!SensorDevice.HostInit())
      {
        Logger.Warning("Cannot initialize host sensors");
      }
      else
      {
        Logger.Info("Setting up host accelerometer");
        this.mAccelerometerCallback = new SensorDevice.AccelerometerCallback(this.SetAccelerometerVector);
        if (SensorDevice.HostSetupAccelerometer(this.mAccelerometerCallback))
          this.mStateMap[SensorDevice.Type.Accelerometer].HasPhysical = true;
        else
          Logger.Warning("Cannot setup host accelerometer");
      }
    }

    public void ControllerAttach(SensorDevice.Type sensor)
    {
      this.ControllerAttachDetach(sensor, true);
    }

    public void ControllerDetach(SensorDevice.Type sensor)
    {
      this.ControllerAttachDetach(sensor, false);
    }

    private void ControllerAttachDetach(SensorDevice.Type sensor, bool attach)
    {
      SensorDevice.State state = this.LookupState(sensor);
      if (state == null)
        return;
      if (sensor != SensorDevice.Type.Accelerometer)
        Logger.Warning("Don't know how to do controller override for sensor type " + sensor.ToString());
      else
        this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
        {
          if (attach)
            ++state.ControllerCount;
          else
            --state.ControllerCount;
          Logger.Info("Sensor device sees {0} controllers", (object) state.ControllerCount);
          if (state.ControllerCount < 0)
            Logger.Error("Bad sensor device controller count");
          else if (attach && state.ControllerCount == 1)
          {
            Logger.Info("Switching from host accelerometer to controller accelerometer");
            if (!state.HasPhysical)
              return;
            SensorDevice.HostEnableSensor(sensor, false);
          }
          else
          {
            if (attach || state.ControllerCount != 0)
              return;
            Logger.Info("Switching from controller accelerometer to host accelerometer");
            if (!state.HasPhysical)
              return;
            SensorDevice.HostEnableSensor(sensor, true);
          }
        }));
    }

    public void SetAccelerometerVector(float origX, float origY, float origZ)
    {
      SensorDevice.AccelerometerState accelerometerState = (SensorDevice.AccelerometerState) this.LookupState(SensorDevice.Type.Accelerometer);
      if (accelerometerState == null)
        return;
      float num1;
      float num2;
      if (!LayoutManager.mEmulatedPortraitMode)
      {
        num1 = -origX;
        num2 = -origY;
      }
      else
      {
        num1 = -origY;
        num2 = origX;
      }
      if (LayoutManager.mRotateGuest180)
      {
        num1 = -num1;
        num2 = -num2;
      }
      lock (accelerometerState.Lock)
      {
        accelerometerState.X = num1;
        accelerometerState.Y = num2;
        accelerometerState.Z = origZ;
      }
    }

    private void AccelerometerThreadEntry()
    {
      SensorDevice.AccelerometerState accelerometerState = (SensorDevice.AccelerometerState) this.LookupState(SensorDevice.Type.Accelerometer);
      Logger.Info("Starting accelerometer sensor thread");
      while (true)
      {
        long num1;
        do
        {
          long ticks1 = DateTime.Now.Ticks;
          int num2;
          if (this.mRunning && accelerometerState != null && (accelerometerState.Enabled && accelerometerState.Period != 0U))
          {
            float x;
            float y;
            float z;
            lock (accelerometerState.Lock)
            {
              x = accelerometerState.X;
              y = accelerometerState.Y;
              z = accelerometerState.Z;
            }
            if (InputMapper.isSendSensorDeviceData)
              this.SendAccelerometerVector(x, y, z);
            num2 = (int) accelerometerState.Period;
          }
          else
            num2 = 200;
          long ticks2 = DateTime.Now.Ticks;
          num1 = (long) (num2 * 10000) - ticks2 + ticks1;
        }
        while (num1 <= 0L);
        Thread.Sleep((int) (num1 / 10000L));
      }
    }

    private void SendAccelerometerVector(float x, float y, float z)
    {
      try
      {
        SensorDevice.SensorMsgSendAccelerometerEvent(x, y, z, new VmMonitor.SendMessage(this.SendMessage));
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot send accelerometer event: " + ex?.ToString());
      }
    }

    private void EnableHandlerImpl(SensorDevice.Type sensor, bool enable)
    {
      Logger.Info("SensorDevice.EnableHandlerImpl({0}, {1})", (object) sensor, (object) enable);
      SensorDevice.State state = this.LookupState(sensor);
      if (state == null)
      {
        Logger.Error("Enable/disable for invalid sensor " + sensor.ToString());
      }
      else
      {
        state.Enabled = enable;
        this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
        {
          if (!state.HasPhysical || state.ControllerCount != 0)
            return;
          SensorDevice.HostEnableSensor(sensor, enable);
        }));
      }
    }

    private void SetDelayHandlerImpl(SensorDevice.Type sensor, uint msec)
    {
      Logger.Info("SensorDevice.SetDelayHandlerImpl({0}, {1})", (object) sensor, (object) msec);
      SensorDevice.State state = this.LookupState(sensor);
      if (state == null)
      {
        Logger.Error("Set delay for invalid sensor " + sensor.ToString());
      }
      else
      {
        state.Period = msec;
        this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() => SensorDevice.HostSetSensorPeriod(sensor, msec)));
      }
    }

    private void SendMessage(IntPtr msg)
    {
      this.mMonitor.Send(msg);
    }

    private void SendShutdownMessage(IntPtr msg)
    {
      this.mMonitor.SendShutdown(msg);
    }

    private void UpdateOrientation(object obj, EventArgs evt)
    {
      switch (SystemInformation.ScreenOrientation)
      {
        case ScreenOrientation.Angle0:
          SensorDevice.HostSetOrientation(0);
          break;
        case ScreenOrientation.Angle90:
          SensorDevice.HostSetOrientation(1);
          break;
        case ScreenOrientation.Angle180:
          SensorDevice.HostSetOrientation(2);
          break;
        case ScreenOrientation.Angle270:
          SensorDevice.HostSetOrientation(3);
          break;
      }
    }

    public enum Type
    {
      Accelerometer = 1,
    }

    private class State
    {
      public bool Enabled;
      public uint Period;
      public bool HasPhysical;
      public int ControllerCount;
    }

    private class AccelerometerState : SensorDevice.State
    {
      public object Lock = new object();
      public float X;
      public float Y;
      public float Z;
    }

    private delegate void LoggerCallback(string msg);

    private delegate void AccelerometerCallback(float x, float y, float z);

    private delegate void EnableHandler(SensorDevice.Type sensor, bool enable);

    private delegate void SetDelayHandler(SensorDevice.Type sensor, uint msec);
  }
}
