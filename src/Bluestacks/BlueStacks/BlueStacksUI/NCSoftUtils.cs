// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NCSoftUtils
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class NCSoftUtils
  {
    private static object sync = new object();
    private static NCSoftUtils mInstance = (NCSoftUtils) null;
    internal List<string> BlackListedApps = new List<string>()
    {
      "com.bluestacks",
      "com.google",
      "com.android",
      "com.uncube"
    };
    private int mNCSoftAgentPort = -1;

    internal static NCSoftUtils Instance
    {
      get
      {
        if (NCSoftUtils.mInstance == null)
        {
          lock (NCSoftUtils.sync)
          {
            if (NCSoftUtils.mInstance == null)
              NCSoftUtils.mInstance = new NCSoftUtils();
          }
        }
        return NCSoftUtils.mInstance;
      }
    }

    private int GetNCSoftAgentPort()
    {
      if (this.mNCSoftAgentPort != -1)
        return this.mNCSoftAgentPort;
      string SharedMemoryName = "ngpmmf";
      uint NumBytes = 2;
      try
      {
        this.mNCSoftAgentPort = MemoryMappedFile.GetNCSoftAgentPort(SharedMemoryName, NumBytes);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get ncsoft agent port");
        Logger.Error(ex.ToString());
      }
      return this.mNCSoftAgentPort;
    }

    internal void SendAppCrashEvent(string crashReason, string vmName)
    {
      try
      {
        int ncSoftAgentPort = this.GetNCSoftAgentPort();
        if (ncSoftAgentPort == -1)
          return;
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "vm_name",
            vmName
          },
          {
            "err_message",
            crashReason
          }
        };
        Logger.Info("Sending app crash event to NCSoft Agent for vm: " + vmName);
        Logger.Info("Reason: " + crashReason);
        string ncSoftAgent = HTTPUtils.SendRequestToNCSoftAgent(ncSoftAgentPort, "error/crash", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0);
        Logger.Info("app crash event resp:");
        Logger.Info(ncSoftAgent);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to report app crash. Ex : " + ex.ToString());
      }
    }

    internal void SendGoogleLoginEventAsync(string vmName)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (o =>
      {
        try
        {
          int ncSoftAgentPort = this.GetNCSoftAgentPort();
          if (ncSoftAgentPort == -1)
            return;
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "vm_name",
              vmName
            },
            {
              "first",
              "true"
            }
          };
          Logger.Info("Sending google login event to NCSoft Agent for vm: " + vmName);
          string ncSoftAgent = HTTPUtils.SendRequestToNCSoftAgent(ncSoftAgentPort, "account/google/login", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0);
          Logger.Info("account google login event resp:");
          Logger.Info(ncSoftAgent);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to report google login. Ex : " + ex.ToString());
        }
      }));
    }

    internal void SendStreamingEvent(string vmName, string streamingStatus)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (o =>
      {
        try
        {
          int ncSoftAgentPort = this.GetNCSoftAgentPort();
          if (ncSoftAgentPort == -1)
            return;
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "button",
              "streaming"
            },
            {
              "state",
              streamingStatus
            },
            {
              "vm_name",
              vmName
            }
          };
          Logger.Info("Sending streaming event to NCSoft Agent for vm: " + vmName);
          Logger.Info("Status : " + streamingStatus);
          string ncSoftAgent = HTTPUtils.SendRequestToNCSoftAgent(ncSoftAgentPort, "action/button/streaming", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0);
          Logger.Info("action button streaming event resp:");
          Logger.Info(ncSoftAgent);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to report action button streaming. Ex : " + ex.ToString());
        }
      }));
    }
  }
}
