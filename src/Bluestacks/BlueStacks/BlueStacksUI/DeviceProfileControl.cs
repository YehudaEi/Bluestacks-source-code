// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DeviceProfileControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class DeviceProfileControl : UserControl, IComponentConnector
  {
    private Dictionary<string, string> mPreDefinedProfilesList = new Dictionary<string, string>();
    private Dictionary<string, ComboBoxItem> mDeviceProfileComboBoxItems = new Dictionary<string, ComboBoxItem>();
    private Dictionary<string, string> mMobileOperatorsList = new Dictionary<string, string>();
    private Dictionary<string, ComboBoxItem> mMobileOperatorComboboxItems = new Dictionary<string, ComboBoxItem>();
    private JObject mCurrentDeviceProfileObject;
    private CustomToastPopupControl mToastPopup;
    private MainWindow ParentWindow;
    private bool mGettingProfilesFromCloud;
    private bool mCurrentRootAccessStatus;
    private bool mIsProfileChanged;
    internal ScrollViewer mScrollBar;
    internal Border mProfileLoader;
    internal Border mNoInternetWarning;
    internal Grid mChildGrid;
    internal CustomRadioButton mChooseProfile;
    internal CustomComboBox mPredefinedProfilesComboBox;
    internal CustomRadioButton mCustomProfile;
    internal Grid mCustomProfileGrid;
    internal CustomTextBox mManufacturerTextBox;
    internal CustomTextBox mBrandTextBox;
    internal CustomTextBox mModelNumberTextBox;
    internal Grid mTryAgainBtnGrid;
    internal Grid mMobileOperatorGrid;
    internal TextBlock mMobileOpertorText;
    internal TextBlock mMobileNetworkSetupText;
    internal CustomComboBox mMobileOperatorsCombobox;
    internal Grid mRootAccessGrid;
    internal CustomCheckbox mEnableRootAccessCheckBox;
    internal CustomPictureBox mInfoIcon;
    internal CustomButton mSaveChangesBtn;
    private bool _contentLoaded;

    public DeviceProfileControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Init();
    }

    public void Init()
    {
      this.Visibility = Visibility.Hidden;
      this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.DeviceProfileControl_IsVisibleChanged);
      this.mManufacturerTextBox.TextChanged += new TextChangedEventHandler(this.MManufacturerTextBox_TextChanged);
      this.mModelNumberTextBox.TextChanged += new TextChangedEventHandler(this.MManufacturerTextBox_TextChanged);
      this.mBrandTextBox.TextChanged += new TextChangedEventHandler(this.MManufacturerTextBox_TextChanged);
      if (PromotionObject.Instance.IsRootAccessEnabled || FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.mRootAccessGrid.Visibility = Visibility.Visible;
        this.mCurrentRootAccessStatus = DeviceProfileControl.GetRootAccessStatusFromAndroid(this.ParentWindow?.mVmName);
        this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus);
      }
      this.mScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
      this.mGettingProfilesFromCloud = false;
    }

    private static bool GetRootAccessStatusFromAndroid(string vmname)
    {
      try
      {
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getprop", new Dictionary<string, string>()
        {
          {
            "d",
            "bst.config.bindmount"
          }
        }, vmname, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        return string.Equals(jobject["result"].ToString(), "ok", StringComparison.InvariantCulture) && string.Equals(jobject["value"].ToString(), "1", StringComparison.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Getting root status from android: " + ex.ToString());
        return false;
      }
    }

    private void ChangeLoadingGridVisibility(bool state)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (state)
        {
          this.mProfileLoader.Visibility = Visibility.Visible;
          this.mNoInternetWarning.Visibility = Visibility.Collapsed;
          this.mChildGrid.Visibility = Visibility.Collapsed;
          this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
          this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
        }
        else
        {
          this.mProfileLoader.Visibility = Visibility.Collapsed;
          this.mNoInternetWarning.Visibility = Visibility.Collapsed;
          this.mChildGrid.Visibility = Visibility.Visible;
          if (RegistryManager.Instance.IsCacodeValid)
            this.mMobileOperatorGrid.Visibility = Visibility.Visible;
          this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
        }
      }));
    }

    private void ChangeNoInternetGridVisibility(bool state)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (state)
        {
          this.mProfileLoader.Visibility = Visibility.Collapsed;
          this.mNoInternetWarning.Visibility = Visibility.Visible;
          this.mChildGrid.Visibility = Visibility.Collapsed;
          this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
          this.mTryAgainBtnGrid.Visibility = Visibility.Visible;
        }
        else
        {
          this.mProfileLoader.Visibility = Visibility.Visible;
          this.mNoInternetWarning.Visibility = Visibility.Collapsed;
          this.mChildGrid.Visibility = Visibility.Collapsed;
          this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
          this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
        }
      }));
    }

    private void DeviceProfileControl_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs _2)
    {
      if (this.IsVisible)
      {
        if (this.mGettingProfilesFromCloud)
          return;
        this.mGettingProfilesFromCloud = true;
        this.ChangeLoadingGridVisibility(true);
        this.ChangeNoInternetGridVisibility(false);
        this.GetPreDefinedProfilesFromCloud();
      }
      else
      {
        this.mSaveChangesBtn.IsEnabled = false;
        this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus);
      }
    }

    private void SetUIAccordingToCurrentDeviceProfile()
    {
      this.mPredefinedProfilesComboBox.SelectionChanged -= new SelectionChangedEventHandler(this.mPredefinedProfilesComboBox_SelectionChanged);
      this.mMobileOperatorsCombobox.SelectionChanged -= new SelectionChangedEventHandler(this.MobileOperatorsCombobox_SelectionChanged);
      if (this.mCurrentDeviceProfileObject == null)
      {
        this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
        this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
      }
      else
      {
        if (string.Equals(this.mCurrentDeviceProfileObject["pcode"]?.ToString(), "custom", StringComparison.InvariantCulture))
        {
          this.mPredefinedProfilesComboBox.Visibility = Visibility.Collapsed;
          this.mCustomProfileGrid.Visibility = Visibility.Visible;
          this.mModelNumberTextBox.Text = this.mCurrentDeviceProfileObject["model"].ToString();
          this.mBrandTextBox.Text = this.mCurrentDeviceProfileObject["brand"].ToString();
          this.mManufacturerTextBox.Text = this.mCurrentDeviceProfileObject["manufacturer"].ToString();
          this.mCustomProfile.IsChecked = new bool?(true);
          this.mPredefinedProfilesComboBox.SelectedItem = (object) null;
        }
        else
        {
          this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
          this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
          if (this.mDeviceProfileComboBoxItems.ContainsKey(this.mCurrentDeviceProfileObject["pcode"]?.ToString()))
            this.mPredefinedProfilesComboBox.SelectedItem = (object) this.mDeviceProfileComboBoxItems[this.mCurrentDeviceProfileObject["pcode"].ToString()];
          this.mChooseProfile.IsChecked = new bool?(true);
          this.mModelNumberTextBox.Text = string.Empty;
          this.mBrandTextBox.Text = string.Empty;
          this.mManufacturerTextBox.Text = string.Empty;
        }
        if (this.mMobileOperatorComboboxItems.ContainsKey(this.mCurrentDeviceProfileObject["caSelector"]?.ToString()))
          this.mMobileOperatorsCombobox.SelectedItem = (object) this.mMobileOperatorComboboxItems[this.mCurrentDeviceProfileObject["caSelector"].ToString()];
      }
      this.mMobileOperatorsCombobox.SelectionChanged += new SelectionChangedEventHandler(this.MobileOperatorsCombobox_SelectionChanged);
      this.mPredefinedProfilesComboBox.SelectionChanged += new SelectionChangedEventHandler(this.mPredefinedProfilesComboBox_SelectionChanged);
      this.ChangeLoadingGridVisibility(false);
    }

    private void GetPreDefinedProfilesFromCloud()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          this.GetCurrentDeviceProfileFromAndroid(this.ParentWindow.mVmName);
          if (this.mPreDefinedProfilesList.Count == 0 || this.mMobileOperatorsList.Count == 0)
          {
            string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) RegistryManager.Instance.Host, (object) "bs4", (object) "get_device_profile_list");
            Dictionary<string, string> commonPostData = WebHelper.GetCommonPOSTData();
            commonPostData.Add("ca_code", Utils.GetValueInBootParams("caCode", this.ParentWindow.mVmName, "", "bgp"));
            Dictionary<string, string> data = commonPostData;
            string mVmName = this.ParentWindow.mVmName;
            JObject jobject1 = JObject.Parse(BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, mVmName, 0, 1, 0, false, "bgp"));
            if (jobject1 == null || !(bool) jobject1["success"])
              return;
            if (!JsonExtensions.IsNullOrEmptyBrackets(jobject1["device_profile_list"].ToString()))
            {
              foreach (JObject jobject2 in jobject1["device_profile_list"].ToArray<JToken>())
                this.mPreDefinedProfilesList[jobject2["pcode"].ToString()] = jobject2["display_name"].ToString();
            }
            if (jobject1.ContainsKey("ca_selector_list") && !JsonExtensions.IsNullOrEmptyBrackets(jobject1["ca_selector_list"].ToString()))
            {
              foreach (JObject jobject2 in jobject1["ca_selector_list"].ToArray<JToken>())
                this.mMobileOperatorsList[jobject2["ca_selector"].ToString()] = jobject2["display_name"].ToString();
            }
            this.AddPreDefinedProfilesinComboBox();
          }
          else
            this.AddPreDefinedProfilesinComboBox();
        }
        catch (Exception ex)
        {
          Logger.Error("Error while getting device profile from cloud : " + ex.ToString());
          this.ChangeNoInternetGridVisibility(true);
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal void GetCurrentDeviceProfileFromAndroid(string vmName)
    {
      if (!string.Equals(VmCmdHandler.SendRequest("currentdeviceprofile", (Dictionary<string, string>) null, vmName, out this.mCurrentDeviceProfileObject, "bgp"), "ok", StringComparison.InvariantCulture))
        return;
      this.mCurrentDeviceProfileObject.Remove("result");
    }

    private void AddPreDefinedProfilesinComboBox()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        foreach (string key in this.mPreDefinedProfilesList.Keys)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem()
          {
            Content = (object) this.mPreDefinedProfilesList[key]
          };
          this.mPredefinedProfilesComboBox.Items.Add((object) comboBoxItem);
          if (this.mDeviceProfileComboBoxItems.ContainsKey(key))
            this.mDeviceProfileComboBoxItems[key] = comboBoxItem;
          else
            this.mDeviceProfileComboBoxItems.Add(key, comboBoxItem);
        }
        foreach (string key in this.mMobileOperatorsList.Keys)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem()
          {
            Content = (object) this.mMobileOperatorsList[key]
          };
          this.mMobileOperatorsCombobox.Items.Add((object) comboBoxItem);
          this.mMobileOperatorComboboxItems[key] = comboBoxItem;
        }
        this.SetUIAccordingToCurrentDeviceProfile();
      }));
    }

    private void Profile_Checked(object sender, RoutedEventArgs e)
    {
      if (this.mChooseProfile.IsChecked.Value)
      {
        this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
        this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
        this.mIsProfileChanged = string.Equals(this.mCurrentDeviceProfileObject["pcode"]?.ToString(), "custom", StringComparison.InvariantCulture);
      }
      else
      {
        if (!this.mCustomProfile.IsChecked.Value)
          return;
        this.mPredefinedProfilesComboBox.Visibility = Visibility.Collapsed;
        this.mCustomProfileGrid.Visibility = Visibility.Visible;
        this.mIsProfileChanged = !string.Equals(this.mCurrentDeviceProfileObject["pcode"]?.ToString(), "custom", StringComparison.InvariantCulture);
      }
    }

    private void mPredefinedProfilesComboBox_SelectionChanged(
      object sender,
      SelectionChangedEventArgs e)
    {
      JObject deviceProfileObject = this.GetChangedDeviceProfileObject(out string _);
      CustomButton mSaveChangesBtn = this.mSaveChangesBtn;
      int num;
      if (JToken.DeepEquals((JToken) this.mCurrentDeviceProfileObject, (JToken) deviceProfileObject))
      {
        bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
        bool rootAccessStatus = this.mCurrentRootAccessStatus;
        num = !(isChecked.GetValueOrDefault() == rootAccessStatus & isChecked.HasValue) ? 1 : 0;
      }
      else
        num = 1;
      mSaveChangesBtn.IsEnabled = num != 0;
    }

    private void MManufacturerTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      JObject deviceProfileObject = this.GetChangedDeviceProfileObject(out string _);
      CustomButton mSaveChangesBtn = this.mSaveChangesBtn;
      int num;
      if (JToken.DeepEquals((JToken) this.mCurrentDeviceProfileObject, (JToken) deviceProfileObject))
      {
        bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
        bool rootAccessStatus = this.mCurrentRootAccessStatus;
        num = !(isChecked.GetValueOrDefault() == rootAccessStatus & isChecked.HasValue) ? 1 : 0;
      }
      else
        num = 1;
      mSaveChangesBtn.IsEnabled = num != 0;
    }

    private JObject GetChangedDeviceProfileObject(out string jsonString)
    {
      jsonString = "{";
      JObject jobject = new JObject();
      string key1 = this.mCurrentDeviceProfileObject["pcode"].ToString();
      string key2 = this.mCurrentDeviceProfileObject["caSelector"].ToString();
      KeyValuePair<string, string> keyValuePair;
      if (this.mChooseProfile.IsChecked.Value)
      {
        if (this.mPredefinedProfilesComboBox.SelectedItem != null)
        {
          string selectedDeviceProfile = (this.mPredefinedProfilesComboBox.SelectedItem as ComboBoxItem).Content.ToString();
          keyValuePair = this.mPreDefinedProfilesList.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Value == selectedDeviceProfile));
          key1 = keyValuePair.Key;
        }
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", (object) "false");
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"pcode\":\"{0}\",", (object) key1);
        jobject["pcode"] = (JToken) key1;
      }
      else
      {
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", (object) "true");
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"model\":\"{0}\",", (object) this.mModelNumberTextBox.Text);
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", (object) this.mBrandTextBox.Text);
        jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\",", (object) this.mManufacturerTextBox.Text);
        jobject["pcode"] = (JToken) "custom";
        jobject["model"] = (JToken) this.mModelNumberTextBox.Text;
        jobject["brand"] = (JToken) this.mBrandTextBox.Text;
        jobject["manufacturer"] = (JToken) this.mManufacturerTextBox.Text;
      }
      if (this.mMobileOperatorsCombobox.SelectedItem != null)
      {
        string selectedMobileOperator = (this.mMobileOperatorsCombobox.SelectedItem as ComboBoxItem).Content.ToString();
        if (!string.IsNullOrEmpty(selectedMobileOperator))
        {
          keyValuePair = this.mMobileOperatorsList.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Value == selectedMobileOperator));
          key2 = keyValuePair.Key;
        }
      }
      jsonString += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"caSelector\":\"{0}\"", (object) key2);
      jsonString += "}";
      jobject.Add("caSelector", (JToken) key2);
      return jobject;
    }

    private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
    {
      this.mSaveChangesBtn.IsEnabled = false;
      this.mIsProfileChanged = false;
      string jsonString;
      JObject deviceProfileObject = this.GetChangedDeviceProfileObject(out jsonString);
      this.SendDeviceProfileChangeToGuest(jsonString, deviceProfileObject);
      bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
      bool rootAccessStatus = this.mCurrentRootAccessStatus;
      if (!(isChecked.GetValueOrDefault() == rootAccessStatus & isChecked.HasValue))
      {
        string res = (string) null;
        new Thread((ThreadStart) (() =>
        {
          try
          {
            res = this.mCurrentRootAccessStatus ? HTTPUtils.SendRequestToGuest("unbindmount", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp") : HTTPUtils.SendRequestToGuest("bindmount", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            if (string.Equals(JObject.Parse(res)["result"].ToString(), "ok", StringComparison.InvariantCulture))
            {
              this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
              this.mCurrentRootAccessStatus = !this.mCurrentRootAccessStatus;
              this.SendStatsOfRootAccessStatusAsync("success", this.mCurrentRootAccessStatus);
              if (!SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName) || !this.mCurrentRootAccessStatus)
                return;
              SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
            }
            else
            {
              this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_ROOT_ACCESS_FAILURE", ""));
              this.Dispatcher.Invoke((Delegate) (() => this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus)));
              this.SendStatsOfRootAccessStatusAsync("failed", this.mCurrentRootAccessStatus);
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in sending mount unmount request to Android: " + ex.ToString());
          }
        }))
        {
          IsBackground = true
        }.Start();
      }
      ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Device-Settings", "", (string) null, this.ParentWindow.mVmName, (string) null, (string) null, "Android");
    }

    private void AddToastPopup(string message)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((UserControl) this);
        this.mToastPopup.Init((UserControl) this, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(new Thickness(0.0, 0.0, 0.0, 50.0)), 12, new Thickness?(), (Brush) null);
        this.mToastPopup.ShowPopup(1.3);
      }));
    }

    private static void SendStatsOfDeviceProfileChangeAsync(
      string successString,
      JObject newDeviceProfile,
      JObject oldDeviceProfile)
    {
      ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, successString, JsonConvert.SerializeObject((object) newDeviceProfile), JsonConvert.SerializeObject((object) oldDeviceProfile), RegistryManager.Instance.Version, "DeviceProfileSetting", (string) null, "Android");
    }

    private void SendStatsOfRootAccessStatusAsync(string successString, bool rootedstatus)
    {
      string str = rootedstatus ? "Rooted" : "Unrooted";
      ClientStats.SendMiscellaneousStatsAsync("DeviceRootingStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, successString, str, this.ParentWindow.mVmName, (string) null, (string) null, "Android");
    }

    private void TryAgainBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ChangeNoInternetGridVisibility(false);
      this.ChangeLoadingGridVisibility(true);
      this.GetPreDefinedProfilesFromCloud();
    }

    private void mEnableRootAccessCheckBox_Click(object sender, RoutedEventArgs e)
    {
      JObject deviceProfileObject = this.GetChangedDeviceProfileObject(out string _);
      CustomButton mSaveChangesBtn = this.mSaveChangesBtn;
      int num;
      if (JToken.DeepEquals((JToken) this.mCurrentDeviceProfileObject, (JToken) deviceProfileObject))
      {
        bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
        bool rootAccessStatus = this.mCurrentRootAccessStatus;
        num = !(isChecked.GetValueOrDefault() == rootAccessStatus & isChecked.HasValue) ? 1 : 0;
      }
      else
        num = 1;
      mSaveChangesBtn.IsEnabled = num != 0;
    }

    private void SendDeviceProfileChangeToGuest(string json, JObject changedDeviceProfileObject)
    {
      if (!Utils.CheckIfDeviceProfileChanged(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
        return;
      string command = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) "changeDeviceProfile", (object) json);
      Logger.Info("Command for device profile change: " + command);
      new Thread((ThreadStart) (() =>
      {
        try
        {
          string a = VmCmdHandler.RunCommand(command, this.ParentWindow.mVmName, "bgp");
          Logger.Info("Result for device profile change command: " + a);
          if (string.Equals(a, "ok", StringComparison.InvariantCulture))
          {
            this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
            DeviceProfileControl.SendStatsOfDeviceProfileChangeAsync("success", changedDeviceProfileObject, this.mCurrentDeviceProfileObject);
            this.mCurrentDeviceProfileObject = changedDeviceProfileObject;
            Utils.UpdateValueInBootParams("pcode", changedDeviceProfileObject["pcode"].ToString(), this.ParentWindow.mVmName, false, "bgp");
            Utils.UpdateValueInBootParams("caSelector", changedDeviceProfileObject["caSelector"].ToString(), this.ParentWindow.mVmName, false, "bgp");
            if (SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName))
              SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROFILE_CHANGED, string.Empty);
          }
          else
          {
            this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""));
            DeviceProfileControl.SendStatsOfDeviceProfileChangeAsync("failed", changedDeviceProfileObject, this.mCurrentDeviceProfileObject);
          }
          this.Dispatcher.Invoke((Delegate) (() => this.SetUIAccordingToCurrentDeviceProfile()));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in change to predefined Pcode call to android: " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void MobileOperatorsCombobox_SelectionChanged(
      object sender,
      SelectionChangedEventArgs e)
    {
      JObject deviceProfileObject = this.GetChangedDeviceProfileObject(out string _);
      CustomButton mSaveChangesBtn = this.mSaveChangesBtn;
      int num;
      if (JToken.DeepEquals((JToken) this.mCurrentDeviceProfileObject, (JToken) deviceProfileObject))
      {
        bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
        bool rootAccessStatus = this.mCurrentRootAccessStatus;
        num = !(isChecked.GetValueOrDefault() == rootAccessStatus & isChecked.HasValue) ? 1 : 0;
      }
      else
        num = 1;
      mSaveChangesBtn.IsEnabled = num != 0;
    }

    public bool IsDirty()
    {
      return this.mSaveChangesBtn.IsEnabled || this.mIsProfileChanged;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/deviceprofilecontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mScrollBar = (ScrollViewer) target;
          break;
        case 2:
          this.mProfileLoader = (Border) target;
          break;
        case 3:
          this.mNoInternetWarning = (Border) target;
          break;
        case 4:
          this.mChildGrid = (Grid) target;
          break;
        case 5:
          this.mChooseProfile = (CustomRadioButton) target;
          this.mChooseProfile.Checked += new RoutedEventHandler(this.Profile_Checked);
          break;
        case 6:
          this.mPredefinedProfilesComboBox = (CustomComboBox) target;
          this.mPredefinedProfilesComboBox.SelectionChanged += new SelectionChangedEventHandler(this.mPredefinedProfilesComboBox_SelectionChanged);
          break;
        case 7:
          this.mCustomProfile = (CustomRadioButton) target;
          this.mCustomProfile.Checked += new RoutedEventHandler(this.Profile_Checked);
          break;
        case 8:
          this.mCustomProfileGrid = (Grid) target;
          break;
        case 9:
          this.mManufacturerTextBox = (CustomTextBox) target;
          break;
        case 10:
          this.mBrandTextBox = (CustomTextBox) target;
          break;
        case 11:
          this.mModelNumberTextBox = (CustomTextBox) target;
          break;
        case 12:
          this.mTryAgainBtnGrid = (Grid) target;
          break;
        case 13:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.TryAgainBtn_Click);
          break;
        case 14:
          this.mMobileOperatorGrid = (Grid) target;
          break;
        case 15:
          this.mMobileOpertorText = (TextBlock) target;
          break;
        case 16:
          this.mMobileNetworkSetupText = (TextBlock) target;
          break;
        case 17:
          this.mMobileOperatorsCombobox = (CustomComboBox) target;
          this.mMobileOperatorsCombobox.SelectionChanged += new SelectionChangedEventHandler(this.MobileOperatorsCombobox_SelectionChanged);
          break;
        case 18:
          this.mRootAccessGrid = (Grid) target;
          break;
        case 19:
          this.mEnableRootAccessCheckBox = (CustomCheckbox) target;
          this.mEnableRootAccessCheckBox.Click += new RoutedEventHandler(this.mEnableRootAccessCheckBox_Click);
          break;
        case 20:
          this.mInfoIcon = (CustomPictureBox) target;
          break;
        case 21:
          this.mSaveChangesBtn = (CustomButton) target;
          this.mSaveChangesBtn.Click += new RoutedEventHandler(this.SaveChangesBtn_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
