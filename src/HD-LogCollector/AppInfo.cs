// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.AppInfo
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using Newtonsoft.Json.Linq;

namespace BlueStacks.LogCollector
{
  public class AppInfo
  {
    internal string name;
    internal string package;
    internal string version;

    internal AppInfo(JObject app)
    {
      try
      {
        this.name = app["appname"].ToString();
        this.package = app[nameof (package)].ToString();
        this.version = app[nameof (version)].ToString();
      }
      catch
      {
        this.name = "";
        this.package = "";
        this.version = "";
      }
    }

    public AppInfo(string InName, string InPackage, string InVersion)
    {
      this.name = InName;
      this.package = InPackage;
      this.version = InVersion;
    }
  }
}
