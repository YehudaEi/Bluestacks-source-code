// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VmMonitor
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace BlueStacks.Player
{
  public class VmMonitor
  {
    private SafeFileHandle mHandle;
    private IntPtr mListener;
    private UdpClient mUdpClient;
    private VmMonitor.ReceiverCallback mReceiverCallback;
    private Thread mReceiverThread;
    private EventWaitHandle mReceiverWakeup;
    public static int lPort;
    public static string data;

    private VmMonitor(UdpClient c)
    {
      Logger.Warning("{0} Monitor constructor initing UDP socket", (object) MethodBase.GetCurrentMethod().Name);
      this.mUdpClient = c;
      this.mListener = this.mUdpClient.Client.Handle;
    }

    private VmMonitor(SafeFileHandle handle)
    {
      this.mHandle = handle;
    }

    public static VmMonitor Connect(string vmName, uint cls)
    {
      VmMonitor.lPort = 2921;
      int num = VmMonitor.lPort + 40;
      UdpClient c = (UdpClient) null;
      for (int lPort = VmMonitor.lPort; lPort <= num; ++lPort)
      {
        try
        {
          c = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), lPort));
          VmMonitor.lPort = lPort;
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while setting up UDPServer on port. {0}, Err : ", (object) lPort, (object) ex.ToString());
          if (lPort == num)
            throw;
        }
      }
      RegistryManager.Instance.DefaultGuest.HostSensorPort = VmMonitor.lPort;
      Logger.Warning("Host sensor port is {0}", (object) VmMonitor.lPort);
      return new VmMonitor(c);
    }

    public void Close()
    {
      this.mUdpClient.Close();
    }

    public void Send(IntPtr msg)
    {
      if (HDPlusModule.SensorSendMsg(msg))
        return;
      CommonError.ThrowLastWin32Error("Cannot send message to guest");
    }

    public void SendShutdown(IntPtr msg)
    {
      if (HDPlusModule.SensorSendMsg(msg))
        return;
      CommonError.ThrowLastWin32Error("Cannot send message to guest");
    }

    public void StartReceiver(VmMonitor.ReceiverCallback callback)
    {
      this.mReceiverCallback = callback;
      this.mReceiverWakeup = (EventWaitHandle) new ManualResetEvent(false);
      this.mReceiverThread = new Thread((ThreadStart) (() =>
      {
        try
        {
          if (HDPlusModule.SensorRecvMsg(this.mReceiverCallback))
            return;
          CommonError.ThrowLastWin32Error("Cannot receive monitor message");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception, receiver thread died. Err : " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      };
      this.mReceiverThread.Start();
    }

    public void StopReceiver()
    {
      Logger.Warning("{0} NOP for VBox", (object) MethodBase.GetCurrentMethod().Name);
    }

    public delegate void SendMessage(IntPtr msg);

    public delegate void ReceiverCallback(IntPtr msg);
  }
}
