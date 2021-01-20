// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppSettings
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public class AppSettings
  {
    private bool mIsAppOnboardingCompleted = true;
    private bool mIsGeneralAppOnBoardingCompleted = true;
    private bool mIsCloseGuidanceOnboardingCompleted = true;
    private string mAppInstallTime = string.Empty;
    private string mCfgStored = "";
    private bool mIsKeymappingTooltipShown;
    private bool mIsDefaultSchemeRecorded;
    private bool mIsForcedLandscapeEnabled;
    private bool mIsForcedPortraitEnabled;

    [JsonProperty("IsKeymappingTooltipShown")]
    public bool IsKeymappingTooltipShown
    {
      get
      {
        return this.mIsKeymappingTooltipShown;
      }
      set
      {
        this.mIsKeymappingTooltipShown = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsDefaultSchemeRecorded")]
    public bool IsDefaultSchemeRecorded
    {
      get
      {
        return this.mIsDefaultSchemeRecorded;
      }
      set
      {
        this.mIsDefaultSchemeRecorded = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsAppOnboardingCompleted")]
    public bool IsAppOnboardingCompleted
    {
      get
      {
        return this.mIsAppOnboardingCompleted;
      }
      set
      {
        this.mIsAppOnboardingCompleted = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsGeneralAppOnBoardingCompleted")]
    public bool IsGeneralAppOnBoardingCompleted
    {
      get
      {
        return this.mIsGeneralAppOnBoardingCompleted;
      }
      set
      {
        this.mIsGeneralAppOnBoardingCompleted = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsCloseGuidanceOnboardingCompleted")]
    public bool IsCloseGuidanceOnboardingCompleted
    {
      get
      {
        return this.mIsCloseGuidanceOnboardingCompleted;
      }
      set
      {
        this.mIsCloseGuidanceOnboardingCompleted = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("AppInstallTime")]
    public string AppInstallTime
    {
      get
      {
        return this.mAppInstallTime;
      }
      set
      {
        this.mAppInstallTime = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsForcedLandscapeEnabled")]
    public bool IsForcedLandscapeEnabled
    {
      get
      {
        return this.mIsForcedLandscapeEnabled;
      }
      set
      {
        this.mIsForcedLandscapeEnabled = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("IsForcedPortraitEnabled")]
    public bool IsForcedPortraitEnabled
    {
      get
      {
        return this.mIsForcedPortraitEnabled;
      }
      set
      {
        this.mIsForcedPortraitEnabled = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonProperty("CfgStored")]
    public string CfgStored
    {
      get
      {
        return this.mCfgStored;
      }
      set
      {
        this.mCfgStored = value;
        AppConfigurationManager.Save();
      }
    }

    [JsonExtensionData]
    public IDictionary<string, object> ExtraData { get; set; }
  }
}
