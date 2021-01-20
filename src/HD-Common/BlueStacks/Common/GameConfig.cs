// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GameConfig
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace BlueStacks.Common
{
  public class GameConfig
  {
    private static string sFilePath = string.Empty;
    private static object syncRoot = new object();
    public const string sConfigFilename = "game_config.json";
    private static volatile GameConfig sInstance;

    public static GameConfig Instance
    {
      get
      {
        if (GameConfig.sInstance == null)
        {
          lock (GameConfig.syncRoot)
          {
            GameConfig.sInstance = new GameConfig();
            GameConfig.Init();
          }
        }
        return GameConfig.sInstance;
      }
    }

    private static void Init()
    {
      GameConfig.sFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game_config.json");
      if (!File.Exists(GameConfig.sFilePath))
        return;
      Logger.Info("Loading cfg from : " + GameConfig.sFilePath);
      GameConfig.LoadFile(GameConfig.sFilePath);
    }

    private static void LoadFile(string sFilePath)
    {
      try
      {
        JObject jobject = JObject.Parse(File.ReadAllText(sFilePath));
        GameConfig.Instance.AppName = GameConfig.GetJsonStringValue(jobject, "app_name");
        GameConfig.Instance.PkgName = GameConfig.GetJsonStringValue(jobject, "pkg_name");
        GameConfig.Instance.ActivityName = GameConfig.GetJsonStringValue(jobject, "activity_name");
        GameConfig.Instance.ControlPanelEntryName = GameConfig.GetJsonStringValue(jobject, "control_panel_name");
        GameConfig.Instance.ControlPanelPublisher = GameConfig.GetJsonStringValue(jobject, "control_panel_publisher");
        GameConfig.Instance.InstallerCopyrightText = GameConfig.GetJsonStringValue(jobject, "installer_copyright");
        GameConfig.Instance.AppGenericAction = (GenericAction) Enum.Parse(typeof (GenericAction), GameConfig.GetJsonStringValue(jobject, "app_generic_action"));
        GameConfig.Instance.AppCDNURL = GameConfig.GetJsonStringValue(jobject, "app_cdn_url");
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while parsing config, maybe an invalid file. Ex: {0}", (object) ex.Message);
      }
    }

    private static string GetJsonStringValue(JObject obj, string keyName)
    {
      string empty = string.Empty;
      return obj.ContainsKey(keyName) ? obj[keyName].ToString().Trim() : empty;
    }

    public string ControlPanelEntryName { get; private set; } = string.Empty;

    public string ControlPanelPublisher { get; private set; } = string.Empty;

    public string InstallerCopyrightText { get; private set; } = string.Empty;

    public string AppName { get; private set; } = string.Empty;

    public string PkgName { get; private set; } = string.Empty;

    public string ActivityName { get; private set; } = string.Empty;

    public GenericAction AppGenericAction { get; private set; } = GenericAction.InstallPlay;

    public string AppCDNURL { get; private set; } = string.Empty;
  }
}
