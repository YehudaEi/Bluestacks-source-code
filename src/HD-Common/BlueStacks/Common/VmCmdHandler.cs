// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.VmCmdHandler
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Common
{
  public static class VmCmdHandler
  {
    private static string s_Received;

    public static void SyncConfig(string keyMapParserVersion, string vmName)
    {
      VmCmdHandler.RunCommand("settime " + ((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000L).ToString(), vmName, "bgp");
      Utils.SetTimeZoneInGuest(vmName);
      Dictionary<string, string> data = new Dictionary<string, string>();
      string currentKeyboardLayout = Utils.GetCurrentKeyboardLayout();
      data.Add("keyboardlayout", currentKeyboardLayout);
      string path = "setkeyboardlayout";
      Logger.Info("Sending request for " + path + " with data : ");
      foreach (KeyValuePair<string, string> keyValuePair in data)
        Logger.Info("key : " + keyValuePair.Key + " value : " + keyValuePair.Value);
      string str = VmCmdHandler.SendRequest(path, data, vmName, out JObject _, "bgp");
      if (str == null || str.Contains("error"))
      {
        Logger.Info("Failed to set keyboard layout in sync config...checking for latinime");
        try
        {
          if (Utils.IsLatinImeSelected(vmName))
            HTTPUtils.SendRequestToEngine("setPcImeWorkflow", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
          else if (Oem.Instance.IsSendGameManagerRequest)
            HTTPUtils.SendRequestToClient("showIMESwitchPrompt", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Warning("Error in informing engine/client. Ex: " + ex.Message);
        }
      }
      if (VmCmdHandler.RunCommand("setkeymappingparserversion " + keyMapParserVersion, vmName, "bgp") == null)
        Logger.Error("setkeymappingparserversion did not work, will try again on frontend restart");
      else
        RegistryManager.Instance.Guest[vmName].ConfigSynced = 1;
    }

    public static void SetMachineType(bool isDesktop, string vmName)
    {
      VmCmdHandler.RunCommand(!isDesktop ? "istablet" : "isdesktop", vmName, "bgp");
    }

    public static void SetKeyboard(bool isDesktop, string vmName)
    {
      VmCmdHandler.RunCommand(!isDesktop ? "usesoftkeyboard" : "usehardkeyboard", vmName, "bgp");
    }

    public static string FqdnSend(int port, string serverIn, string vmName)
    {
      try
      {
        string str1;
        if (string.Compare(serverIn, "agent", StringComparison.OrdinalIgnoreCase) == 0)
          str1 = "setWindowsAgentAddr";
        else if (string.Compare(serverIn, "frontend", StringComparison.OrdinalIgnoreCase) == 0)
        {
          str1 = "setWindowsFrontendAddr";
        }
        else
        {
          Logger.Error("Unknown server: " + serverIn);
          return (string) null;
        }
        if (port == 0)
        {
          if (string.Compare(serverIn, "agent", StringComparison.OrdinalIgnoreCase) == 0)
            port = RegistryManager.Instance.AgentServerPort;
          else if (string.Compare(serverIn, "frontend", StringComparison.OrdinalIgnoreCase) == 0)
            port = RegistryManager.Instance.Guest[vmName].FrontendServerPort;
        }
        string str2 = "10.0.2.2:" + port.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        return VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) str1, (object) str2), vmName, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when sending fqdn post request: " + ex.Message);
        return (string) null;
      }
    }

    public static string RunCommand(string cmd, string vmName, string oem = "bgp")
    {
      int length = -1;
      if (cmd != null)
        length = cmd.IndexOf(' ');
      string path;
      string str;
      if (length == -1)
      {
        path = cmd;
        str = "";
      }
      else
      {
        path = cmd?.Substring(0, length);
        str = cmd?.Substring(length + 1);
      }
      Logger.Info("Will send command: {0} to {1}", (object) str, (object) path);
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "arg",
          str
        }
      };
      return VmCmdHandler.SendRequest(path, data, vmName, out JObject _, oem);
    }

    public static string SendRequest(
      string path,
      Dictionary<string, string> data,
      string vmName,
      out JObject response,
      string oem = "bgp")
    {
      int num1 = 60;
      int num2 = 3;
      response = (JObject) null;
      while (num1 > 0)
      {
        try
        {
          if (num2 != 0)
            --num2;
          string json = path == "runex" || path == "run" || path == "powerrun" ? HTTPUtils.SendRequestToGuest(path, data, vmName, 3000, (Dictionary<string, string>) null, false, 1, 0, oem) : (!(path == "setWindowsAgentAddr") ? HTTPUtils.SendRequestToGuest(path, data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, oem) : HTTPUtils.SendRequestToGuest(path, data, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, oem));
          Logger.Info("Got response for {0}: {1}", (object) path, (object) json);
          response = JObject.Parse(json);
          VmCmdHandler.s_Received = (string) response["result"];
          if (VmCmdHandler.s_Received != "ok")
          {
            if (VmCmdHandler.s_Received != "error")
              VmCmdHandler.s_Received = (string) null;
          }
        }
        catch (Exception ex)
        {
          if (num2 != 0)
            Logger.Error("Exception in SendRequest for {0}: {1}", (object) path, (object) ex.Message);
          VmCmdHandler.s_Received = (string) null;
        }
        --num1;
        if (VmCmdHandler.s_Received != null)
          return VmCmdHandler.s_Received;
        if (num1 > 0)
          Thread.Sleep(1000);
      }
      return (string) null;
    }

    public static void RunCommandAsync(
      string cmd,
      UIHelper.Action continuation,
      Control control,
      string vmName)
    {
      System.Threading.Timer timer = new System.Threading.Timer((TimerCallback) (obj =>
      {
        VmCmdHandler.RunCommand(cmd, vmName, "bgp");
        if (continuation == null)
          return;
        UIHelper.RunOnUIThread(control, continuation);
      }), (object) null, 0, -1);
    }
  }
}
