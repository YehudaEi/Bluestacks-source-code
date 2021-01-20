// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppInfo
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
  public class AppInfo
  {
    public string Name { get; set; }

    public string Img { get; set; }

    public string Package { get; set; }

    public string Activity { get; set; }

    public string System { get; set; }

    public string Url { get; set; }

    public string Appstore { get; set; }

    public string Version { get; set; }

    public string VersionName { get; set; } = "Unknown";

    public bool Gl3Required { get; set; }

    public bool VideoPresent { get; set; }

    public bool IsGamepadCompatible { get; set; }

    public AppInfo()
    {
    }

    public AppInfo(JObject app)
    {
      this.Name = app?["name"].ToString();
      this.Img = app["img"].ToString();
      this.Package = app["package"].ToString();
      this.Activity = app["activity"].ToString();
      this.System = app["system"].ToString();
      this.Url = app.ContainsKey("url") ? app["url"].ToString() : (string) null;
      this.Appstore = app.ContainsKey("appstore") ? app["appstore"].ToString() : "Unknown";
      this.Version = app.ContainsKey("version") ? app["version"].ToString() : "Unknown";
      this.Gl3Required = app.ContainsKey("gl3required") && app["gl3required"].ToObject<bool>();
      this.VideoPresent = app.ContainsKey("videopresent") && app["videopresent"].ToObject<bool>();
      this.IsGamepadCompatible = app.ContainsKey("isgamepadcompatible") && app["isgamepadcompatible"].ToObject<bool>();
      if (!app.ContainsKey("versionName"))
        return;
      this.VersionName = app["versionName"].ToString();
    }

    public AppInfo(
      string InName,
      string InImage,
      string InPackage,
      string InActivity,
      string InSystem,
      string InAppStore,
      string InVersion,
      bool InGl3required,
      bool InVideoPresent,
      string appVersionName,
      bool isGamepadCompatible = false)
    {
      this.Name = InName;
      this.Img = InImage;
      this.Package = InPackage;
      this.Activity = InActivity;
      this.System = InSystem;
      this.Url = (string) null;
      this.Appstore = InAppStore;
      this.Version = InVersion;
      this.Gl3Required = InGl3required;
      this.VideoPresent = InVideoPresent;
      this.VersionName = appVersionName;
      this.IsGamepadCompatible = isGamepadCompatible;
    }
  }
}
