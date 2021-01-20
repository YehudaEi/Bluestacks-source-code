// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.OtherAppGameSetting
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public class OtherAppGameSetting : ViewModelBase
  {
    private ObservableCollection<IMActionItem> mIMActionItemsX = new ObservableCollection<IMActionItem>();
    private ObservableCollection<IMActionItem> mIMActionItemsY = new ObservableCollection<IMActionItem>();
    private Visibility mShowSensitivity = Visibility.Collapsed;
    private Visibility mPlayInLandscapeModeVisibility = Visibility.Collapsed;
    private Visibility mPlayInPortraitModeVisibility = Visibility.Collapsed;
    private string mMouseSenstivityX;
    private string mMouseSenstivityY;
    private Type mPropertyType;
    private string mOldMouseSenstivityX;
    private string mOldMouseSenstivityY;
    private bool mPlayInLandscapeMode;
    private bool mPlayInPortraitMode;

    public string AppName { get; }

    public string PackageName { get; }

    public MainWindow ParentWindow { get; }

    public string MouseSenstivityX
    {
      get
      {
        return this.mMouseSenstivityX;
      }
      set
      {
        this.SetProperty<string>(ref this.mMouseSenstivityX, value, (string) null);
      }
    }

    public string MouseSenstivityY
    {
      get
      {
        return this.mMouseSenstivityY;
      }
      set
      {
        this.SetProperty<string>(ref this.mMouseSenstivityY, value, (string) null);
      }
    }

    public Type SensitivityPropertyType
    {
      get
      {
        return this.mPropertyType;
      }
      set
      {
        this.SetProperty<Type>(ref this.mPropertyType, value, (string) null);
      }
    }

    public ObservableCollection<IMActionItem> SensitivityIMActionItemsX
    {
      get
      {
        return this.mIMActionItemsX;
      }
      set
      {
        this.SetProperty<ObservableCollection<IMActionItem>>(ref this.mIMActionItemsX, value, (string) null);
      }
    }

    public ObservableCollection<IMActionItem> SensitivityIMActionItemsY
    {
      get
      {
        return this.mIMActionItemsY;
      }
      set
      {
        this.SetProperty<ObservableCollection<IMActionItem>>(ref this.mIMActionItemsY, value, (string) null);
      }
    }

    private void InitMouseSenstivity()
    {
      IMAction imAction = this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Where<IMAction>((Func<IMAction, bool>) (item => (object) item.GetType() == (object) typeof (Pan))).FirstOrDefault<IMAction>();
      if (imAction != null && imAction is Pan pan && (pan.Tweaks & 32) == 0)
      {
        this.mShowSensitivity = Visibility.Visible;
        this.SensitivityPropertyType = IMAction.DictPropertyInfo[imAction.Type]["Sensitivity"].PropertyType;
        this.MouseSenstivityX = imAction["Sensitivity"].ToString();
        this.MouseSenstivityY = Convert.ToDouble(imAction["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture) == 0.0 ? Convert.ToDouble(imAction["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture) : (Convert.ToDouble(imAction["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture) * Convert.ToDouble(imAction["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture)).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture);
        this.SensitivityIMActionItemsX.Add(new IMActionItem()
        {
          ActionItem = "Sensitivity",
          IMAction = imAction
        });
        this.SensitivityIMActionItemsY.Add(new IMActionItem()
        {
          ActionItem = "SensitivityRatioY",
          IMAction = imAction
        });
      }
      else
        this.mShowSensitivity = Visibility.Collapsed;
    }

    public Visibility ShowSensitivity
    {
      get
      {
        return this.mShowSensitivity;
      }
      set
      {
        this.SetProperty<Visibility>(ref this.mShowSensitivity, value, (string) null);
      }
    }

    public bool PlayInLandscapeMode
    {
      get
      {
        return this.mPlayInLandscapeMode;
      }
      set
      {
        this.SetProperty<bool>(ref this.mPlayInLandscapeMode, value, (string) null);
      }
    }

    public Visibility PlayInLandscapeModeVisibility
    {
      get
      {
        return this.mPlayInLandscapeModeVisibility;
      }
      set
      {
        this.SetProperty<Visibility>(ref this.mPlayInLandscapeModeVisibility, value, (string) null);
      }
    }

    public bool PlayInPortraitMode
    {
      get
      {
        return this.mPlayInPortraitMode;
      }
      set
      {
        this.SetProperty<bool>(ref this.mPlayInPortraitMode, value, (string) null);
      }
    }

    public Visibility PlayInPortraitModeVisibility
    {
      get
      {
        return this.mPlayInPortraitModeVisibility;
      }
      set
      {
        this.SetProperty<Visibility>(ref this.mPlayInPortraitModeVisibility, value, (string) null);
      }
    }

    public OtherAppGameSetting(MainWindow parentWindow, string appName, string packageName)
    {
      this.AppName = appName;
      this.PackageName = packageName;
      this.ParentWindow = parentWindow;
    }

    public virtual void Init()
    {
      string orientationForPackage = GuidanceCloudInfoManager.GetCloudOrientationForPackage(this.PackageName);
      this.PlayInLandscapeModeVisibility = Visibility.Collapsed;
      this.PlayInPortraitModeVisibility = Visibility.Collapsed;
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName] = new AppSettings();
      switch (orientationForPackage.ToLowerInvariant())
      {
        case "landscape":
          this.PlayInLandscapeModeVisibility = Visibility.Visible;
          this.PlayInLandscapeMode = AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled;
          break;
        case "portrait":
          this.PlayInPortraitModeVisibility = Visibility.Visible;
          this.PlayInPortraitMode = AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled;
          break;
      }
      this.InitMouseSenstivity();
    }

    public virtual void LockOriginal()
    {
      this.mOldMouseSenstivityX = this.MouseSenstivityX;
      this.mOldMouseSenstivityY = this.MouseSenstivityY;
    }

    public virtual bool HasChanged()
    {
      return this.HasLandscapeModeChanged() || this.HasPortraitModeChanged() || this.MouseSenstivityX != this.mOldMouseSenstivityX || this.MouseSenstivityY != this.mOldMouseSenstivityY;
    }

    public bool HasLandscapeModeChanged()
    {
      return this.PlayInLandscapeModeVisibility == Visibility.Visible && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName) && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled != this.PlayInLandscapeMode;
    }

    public bool HasPortraitModeChanged()
    {
      return this.PlayInPortraitModeVisibility == Visibility.Visible && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName) && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled != this.PlayInPortraitMode;
    }

    private bool HasMouseSenstivityChanged()
    {
      return this.mOldMouseSenstivityX != this.MouseSenstivityX || this.mOldMouseSenstivityY != this.MouseSenstivityY;
    }

    public virtual bool Save(bool restartReq)
    {
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName] = new AppSettings();
      ScreenMode screenMode;
      if (this.HasLandscapeModeChanged())
      {
        Utils.SetCustomAppSize(this.ParentWindow.mVmName, this.PackageName, this.PlayInLandscapeMode ? ScreenMode.full : ScreenMode.original);
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled = this.PlayInLandscapeMode;
        KMManager.SelectSchemeIfPresent(this.ParentWindow, this.PlayInLandscapeMode ? "Landscape" : "Portrait", "gamesettings", false);
        string userGuid = RegistryManager.Instance.UserGuid;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        string packageName = this.PackageName;
        string str;
        if (!this.PlayInLandscapeMode)
        {
          str = ScreenMode.original.ToString();
        }
        else
        {
          screenMode = ScreenMode.full;
          str = screenMode.ToString();
        }
        string oem = RegistryManager.Instance.Oem;
        ClientStats.SendMiscellaneousStatsAsync("client_game_settings", userGuid, "landscapeMode", clientVersion, packageName, str, oem, (string) null, (string) null, "Android");
        restartReq = true;
      }
      if (this.HasPortraitModeChanged())
      {
        Utils.SetCustomAppSize(this.ParentWindow.mVmName, this.PackageName, this.PlayInPortraitMode ? ScreenMode.small : ScreenMode.original);
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled = this.PlayInPortraitMode;
        KMManager.SelectSchemeIfPresent(this.ParentWindow, this.PlayInPortraitMode ? "Portrait" : "Landscape", "gamesettings", false);
        string userGuid = RegistryManager.Instance.UserGuid;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        string packageName = this.PackageName;
        string str;
        if (!this.PlayInPortraitMode)
        {
          screenMode = ScreenMode.original;
          str = screenMode.ToString();
        }
        else
        {
          screenMode = ScreenMode.small;
          str = screenMode.ToString();
        }
        string oem = RegistryManager.Instance.Oem;
        ClientStats.SendMiscellaneousStatsAsync("client_game_settings", userGuid, "portraitMode", clientVersion, packageName, str, oem, (string) null, (string) null, "Android");
        restartReq = true;
      }
      if (this.HasMouseSenstivityChanged())
      {
        KeymapCanvasWindow.sIsDirty = true;
        KMManager.SaveIMActions(this.ParentWindow, true, false);
        if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && KMManager.sGuidanceWindow.IsVisible)
          KMManager.sGuidanceWindow.InitUI();
        ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "mouseSenstivityChanged", string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.PackageName, (string) null, "Android");
        this.InitMouseSenstivity();
      }
      return restartReq;
    }

    public void Reset()
    {
      string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
      IEnumerable<IMControlScheme> source = this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme_ => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase)));
      if (!source.Any<IMControlScheme>())
        return;
      IMControlScheme imControlScheme1 = source.Count<IMControlScheme>() == 1 ? source.First<IMControlScheme>() : source.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme_ => !scheme_.BuiltIn)).First<IMControlScheme>();
      if (imControlScheme1.BuiltIn)
      {
        this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
        IMControlScheme imControlScheme2 = this.ParentWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture))).FirstOrDefault<IMControlScheme>();
        if (imControlScheme2 == null)
          return;
        imControlScheme2.Selected = true;
        this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme2;
        this.ParentWindow.SelectedConfig.ControlSchemesDict[imControlScheme2.Name] = imControlScheme2;
      }
      else
      {
        this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
        this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme1.DeepCopy();
        this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = this.ParentWindow.SelectedConfig.SelectedControlScheme;
        this.ParentWindow.SelectedConfig.ControlSchemes.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme);
      }
    }
  }
}
