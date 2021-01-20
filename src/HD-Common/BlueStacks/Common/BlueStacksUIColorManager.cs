// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BlueStacksUIColorManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common
{
  public sealed class BlueStacksUIColorManager
  {
    private static volatile BlueStacksUIColorManager mInstance = (BlueStacksUIColorManager) null;
    private static object syncRoot = new object();
    private static BluestacksUIColor mAppliedTheme = (BluestacksUIColor) null;
    public const string ThemeFileName = "ThemeFile";

    private BlueStacksUIColorManager()
    {
    }

    public static string GetThemeFilePath(string themeName)
    {
      string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThemeFile");
      return File.Exists(path) ? path : Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, themeName), "ThemeFile");
    }

    public static BlueStacksUIColorManager Instance
    {
      get
      {
        if (BlueStacksUIColorManager.mInstance == null)
        {
          lock (BlueStacksUIColorManager.syncRoot)
          {
            if (BlueStacksUIColorManager.mInstance == null)
              BlueStacksUIColorManager.mInstance = new BlueStacksUIColorManager();
          }
        }
        return BlueStacksUIColorManager.mInstance;
      }
    }

    public static BluestacksUIColor AppliedTheme
    {
      get
      {
        if (BlueStacksUIColorManager.mAppliedTheme == null)
        {
          lock (BlueStacksUIColorManager.syncRoot)
          {
            if (BlueStacksUIColorManager.mAppliedTheme == null)
            {
              BluestacksUIColor bluestacksUiColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(RegistryManager.ClientThemeName));
              if (bluestacksUiColor != null && bluestacksUiColor.DictBrush.Count > 0)
                BlueStacksUIColorManager.mAppliedTheme = bluestacksUiColor;
              if (BlueStacksUIColorManager.mAppliedTheme != null)
                BlueStacksUIColorManager.mAppliedTheme.NotifyUIElements();
            }
          }
        }
        return BlueStacksUIColorManager.mAppliedTheme;
      }
      private set
      {
        if (value == null)
          return;
        BlueStacksUIColorManager.mAppliedTheme = value;
        BlueStacksUIColorManager.mAppliedTheme.NotifyUIElements();
      }
    }

    public static void ReloadAppliedTheme(string themeName)
    {
      BluestacksUIColor bluestacksUiColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName));
      if (bluestacksUiColor == null || bluestacksUiColor.DictBrush.Count <= 0)
        return;
      BlueStacksUIColorManager.AppliedTheme = bluestacksUiColor;
      RegistryManager.Instance.SetClientThemeNameInRegistry(themeName);
      CustomPictureBox.UpdateImagesFromNewDirectory("");
    }

    public static IEnumerable<string> GetThemes()
    {
      List<string> stringList = new List<string>();
      foreach (string directory in Directory.GetDirectories(RegistryManager.Instance.ClientInstallDir))
      {
        if (File.Exists(Path.Combine(directory, "ThemeFile")))
          stringList.Add(Path.GetFileName(directory));
      }
      return (IEnumerable<string>) stringList;
    }

    public static string GetThemeName(string themeName)
    {
      try
      {
        if (File.Exists(BlueStacksUIColorManager.GetThemeFilePath(themeName)))
          return LocaleStrings.GetLocalizedString(BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName)).DictThemeAvailable["ThemeDisplayName"], "");
        throw new Exception("Theme file not found exception " + themeName);
      }
      catch (Exception ex)
      {
        Logger.Warning("Error checking for theme availability in Theme file " + themeName + Environment.NewLine + ex.ToString());
        return "";
      }
    }

    public static void ApplyTheme(string themeName)
    {
      try
      {
        if (!File.Exists(BlueStacksUIColorManager.GetThemeFilePath(themeName)))
          throw new Exception("Theme file not found exception " + themeName);
        BluestacksUIColor bluestacksUiColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName));
        if (bluestacksUiColor == null || bluestacksUiColor.DictBrush.Count <= 0)
          return;
        BlueStacksUIColorManager.AppliedTheme = bluestacksUiColor;
      }
      catch (Exception ex)
      {
        Logger.Error("Error checking for theme availability in Theme file " + themeName + Environment.NewLine + ex.ToString());
      }
    }
  }
}
