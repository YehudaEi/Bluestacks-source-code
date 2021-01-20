// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.AnnouncementMessage
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using Newtonsoft.Json.Linq;

namespace BlueStacks.Agent
{
  internal class AnnouncementMessage
  {
    public string ImagePath { get; }

    public string Title { get; }

    public string Msg { get; }

    public string Action { get; }

    public string PkgName { get; }

    public string ActionURL { get; }

    public string FileName { get; set; }

    public int AnnouncementID { get; set; }

    public AnnouncementMessage(
      string imagePath,
      int msgID,
      string title,
      string msg,
      string action,
      string pkgName,
      string actionURL,
      string fileName)
    {
      this.ImagePath = imagePath;
      this.AnnouncementID = msgID;
      this.Title = title;
      this.Msg = msg;
      this.Action = action;
      this.PkgName = pkgName;
      this.ActionURL = actionURL;
      this.FileName = fileName;
    }

    public AnnouncementMessage(
      string title,
      string msg,
      string action,
      string pkgName,
      string actionURL,
      string fileName)
      : this(string.Empty, -1, title, msg, action, pkgName, actionURL, fileName)
    {
    }

    public AnnouncementMessage(string imagePath, int msgId, JObject o)
      : this(imagePath, msgId, o["title"].ToString().Trim(), o["msg"].ToString().Trim(), o["action"].ToString().Trim(), o["pkgName"].ToString().Trim(), o["actionUrl"].ToString().Trim(), o["fileName"].ToString().Trim())
    {
    }
  }
}
