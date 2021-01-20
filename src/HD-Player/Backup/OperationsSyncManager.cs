// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.OperationsSyncManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;

namespace BlueStacks.Player
{
  internal sealed class OperationsSyncManager
  {
    private static object syncRoot = new object();
    internal static bool mIsBroadcasting = false;
    internal static bool mIsReceiving = false;
    private static volatile OperationsSyncManager sInstance;

    private OperationsSyncManager()
    {
    }

    public static OperationsSyncManager Instance
    {
      get
      {
        if (OperationsSyncManager.sInstance == null)
        {
          lock (OperationsSyncManager.syncRoot)
          {
            if (OperationsSyncManager.sInstance == null)
              OperationsSyncManager.sInstance = new OperationsSyncManager();
          }
        }
        return OperationsSyncManager.sInstance;
      }
    }

    private string GetPipeName(string fromVM, string toVM)
    {
      return "Sync" + fromVM + toVM;
    }

    internal void StartSyncToInstances(IEnumerable<string> instances)
    {
      OperationsSyncManager.mIsBroadcasting = true;
      InputMapper.Instance.StartOperationsSync();
      foreach (string instance in instances)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          {
            "instance",
            MultiInstanceStrings.VmName
          }
        };
        try
        {
          HTTPUtils.SendRequestToEngine("startSyncConsumer", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, instance, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Post request failed. vmclient = {0}, data = {1}. Err : {2}", (object) instance, (object) dictionary.ToDebugString<string, string>(), (object) ex.ToString());
        }
      }
    }

    internal void StopOperationsSync()
    {
      OperationsSyncManager.mIsBroadcasting = false;
      InputMapper.Instance.StopOperationsSync();
    }

    internal void StartSyncConsumer(string fromInstance)
    {
      OperationsSyncManager.mIsReceiving = true;
      InputMapper.Instance.StartSyncConsumer(fromInstance);
    }

    internal void StopSyncConsumer()
    {
      InputMapper.Instance.StopSyncConsumer();
      OperationsSyncManager.mIsReceiving = false;
    }

    internal void PlayPauseOperationsSync(bool isPause)
    {
      OperationsSyncManager.mIsBroadcasting = !isPause;
      InputMapper.Instance.PlayPauseOperationsSync(isPause);
    }
  }
}
