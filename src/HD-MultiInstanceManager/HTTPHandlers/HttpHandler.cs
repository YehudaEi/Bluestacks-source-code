// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.HTTPHandlers.HttpHandler
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows.Threading;

namespace MultiInstanceManagerMVVM.HTTPHandlers
{
  internal class HttpHandler
  {
    public static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      HttpHandler.WriteSuccessJsonWithVmName(HTTPUtils.ParseRequest(req).RequestVmName, res);
    }

    private static void WriteSuccessJsonWithVmName(string vmName, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      jobject.Add((object) new JProperty("success", (object) true));
      jobject.Add((object) new JProperty("vmname", (object) vmName));
      jarray.Add((JToken) jobject);
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    internal static void ToggleMIMFarmMode(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Received request for ToggleMIMFarmMode");
      Dispatcher.CurrentDispatcher.Invoke((Delegate) (() => Messenger.Default.Send<ToggleFarmModeMessage>(new ToggleFarmModeMessage())));
    }
  }
}
