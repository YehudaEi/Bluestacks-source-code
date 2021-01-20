// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MediaManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlueStacks.Player
{
  internal static class MediaManager
  {
    internal static volatile bool mIsMutedExplicitly = RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].IsMuted || RegistryManager.Instance.AreAllInstancesMuted;
    private static object syncRoot = new object();

    public static int GetGuestVolume(string mediaType = null)
    {
      int num = -1;
      try
      {
        lock (MediaManager.syncRoot)
        {
          Dictionary<string, string> data = new Dictionary<string, string>();
          if (string.IsNullOrEmpty(mediaType))
            data.Add("mediatype", "");
          else
            data.Add("mediatype", mediaType);
          JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getVolume", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          if (jobject["result"].ToString() == "ok")
            return Convert.ToInt32(jobject["volume"].ToString());
          Logger.Error("Couldn't get volume");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while getting the volume from guest: {0}", (object) ex);
      }
      return num;
    }

    public static bool SetGuestVolume(int volume)
    {
      try
      {
        lock (MediaManager.syncRoot)
        {
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "vol",
              volume.ToString()
            }
          };
          Logger.Info("Sending request to set volume {0}", (object) volume);
          string guest = HTTPUtils.SendRequestToGuest("setVolume", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          Logger.Info("The response for SetVolume is {0}", (object) guest);
          return JObject.Parse(guest)["result"].ToString() == "ok";
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while setting guest volume: {0}", (object) ex);
      }
      return false;
    }

    internal static void MuteEngine(bool isMutedExplicitly = false)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Logger.Info("Mute engine start with muteFromUser {0}", (object) isMutedExplicitly.ToString());
        MediaManager.SendMuteEventToGuest(true);
        if (!isMutedExplicitly)
          return;
        MediaManager.mIsMutedExplicitly = isMutedExplicitly;
      }));
    }

    internal static void UnmuteEngine()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Logger.Info("Unmute Engine Start");
        MediaManager.mIsMutedExplicitly = false;
        MediaManager.SendMuteEventToGuest(false);
      }));
    }

    private static void SendMuteEventToGuest(bool mute)
    {
      lock (MediaManager.syncRoot)
      {
        string empty = string.Empty;
        try
        {
          string str = !mute ? HTTPUtils.SendRequestToGuest("unmuteAppPlayer", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp") : HTTPUtils.SendRequestToGuest("muteAppPlayer", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          Logger.Info("The result for mute: {0} is {1}", (object) mute, (object) str);
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception in SendMuteEventToGuest. Err: " + ex.ToString());
        }
      }
    }
  }
}
