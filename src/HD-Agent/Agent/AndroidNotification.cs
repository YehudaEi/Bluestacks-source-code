// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.AndroidNotification
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using System;

namespace BlueStacks.Agent
{
  public class AndroidNotification
  {
    private DateTime mNotificationTime;

    public AndroidNotification(string pkg, string name, string msg, string vmName, string id = "")
    {
      this.Package = pkg;
      this.AppName = name;
      this.Message = msg;
      this.VmName = vmName;
      this.Id = id;
      this.NotificationSent = false;
      this.mNotificationTime = DateTime.Now;
    }

    public bool NotificationSent { get; set; }

    public bool OldNotificationFlag
    {
      get
      {
        return TimeSpan.Compare(DateTime.Now.Subtract(this.mNotificationTime), new TimeSpan(0, 0, 5)) > -1;
      }
    }

    public string Package { get; }

    public string AppName { get; }

    public string Message { get; }

    public string Id { get; }

    public string VmName { get; }
  }
}
