// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class GuidanceWindow : CustomWindow, IComponentConnector
  {
    private List<string> lstPanTags = new List<string>();
    private List<string> lstMOBATags = new List<string>();
    private int mSidebarWidth = 220;
    internal double mGuidanceWindowLeft = -1.0;
    internal double mGuidanceWindowTop = -1.0;
    private GuidanceData mGuidanceData = new GuidanceData();
    private bool isViewState = true;
    private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();
    internal MainWindow ParentWindow;
    private bool mIsGuidanceVideoPresent;
    private CustomToastPopupControl mToastPopup;
    internal bool mIsGamePadTabSelected;
    internal bool mIsOnboardingPopupToBeShownOnGuidanceClose;
    internal bool mGuidanceHasBeenMoved;
    internal static bool IsDirty;
    private string mHelpArticleUrl;
    internal Grid mGuidanceMainGrid;
    internal Border mGameControlBorder;
    internal DockPanel mHeaderGrid;
    internal Grid mControlsTab;
    internal TextBlock mControlsTabTextBlock;
    internal Grid mEditKeysGrid;
    internal TextBlock mEditKeysGridTextBlock;
    internal CustomPictureBox mCloseSideBarWindow;
    internal Grid mControlsGrid;
    internal StackPanel mSchemePanel;
    internal TextBlock mSchemeTextBlock;
    internal CustomComboBox mSchemesComboBox;
    internal Border mVideoBorder;
    internal CustomPictureBox mVideoThumbnail;
    internal Border mHowToPlayGrid;
    internal CustomPictureBox mHowToPlayCollapseExpand;
    internal Border mQuickLearnBorder;
    internal Border mVideoTutorialBorder;
    internal Border mReadArticleBorder;
    internal DockPanel mKeysIconGrid;
    internal Grid mKeyboardIcon;
    internal CustomPictureBox mKeyboardIconImage;
    internal Grid mKeyboardIconSeparator;
    internal Grid mGamepadIcon;
    internal CustomPictureBox mGamepadIconImage;
    internal Grid mGamepadIconSeparator;
    internal StackPanel mReadArticlePanel;
    internal Grid separator;
    internal Border mGuidanceKeyBorder;
    internal Grid mGuidanceKeysGrid;
    internal ListBox mGuidanceListBox;
    internal StackPanel noGameGuidePanel;
    internal DockPanel mViewDock;
    internal CustomButton mEditBtn;
    internal CustomButton mRevertBtn;
    internal Grid mEditDock;
    internal CustomButton mDiscardBtn;
    internal CustomButton mSaveBtn;
    internal Grid mOverlayGrid;
    private bool _contentLoaded;

    private bool IsVideoTutorialAvailable { get; set; }

    public bool IsViewState
    {
      get
      {
        return this.isViewState;
      }
      set
      {
        this.isViewState = value;
        this.mSchemesComboBox.IsEnabled = this.isViewState;
        if (this.isViewState)
        {
          this.mEditKeysGrid.Visibility = Visibility.Collapsed;
          this.mControlsTab.Visibility = Visibility.Visible;
          this.mSchemeTextBlock.Visibility = Visibility.Visible;
          this.mSchemesComboBox.Visibility = Visibility.Visible;
        }
        else
        {
          this.mEditKeysGrid.Visibility = Visibility.Visible;
          this.mControlsTab.Visibility = Visibility.Collapsed;
          this.mSchemeTextBlock.Visibility = Visibility.Collapsed;
          this.mSchemesComboBox.Visibility = Visibility.Collapsed;
          this.mVideoBorder.Visibility = Visibility.Collapsed;
          this.mReadArticlePanel.Visibility = Visibility.Collapsed;
          this.mHowToPlayGrid.Visibility = Visibility.Collapsed;
        }
      }
    }

    public static bool sIsDirty
    {
      get
      {
        return GuidanceWindow.IsDirty;
      }
      set
      {
        GuidanceWindow.IsDirty = value;
      }
    }

    internal GuidanceWindow(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Owner = (Window) window;
      this.IsShowGLWindow = true;
      this.mIsOnboardingPopupToBeShownOnGuidanceClose = false;
      this.ShowWithParentWindow = true;
      if (window.WindowState != WindowState.Normal)
        window.RestoreWindows(false);
      KMManager.CloseWindows();
      this.ResizeGuidanceWindow();
      this.mGuidanceHasBeenMoved = false;
      this.ResetGuidanceTab();
      this.Init();
      GuidanceWindow.HideOnNextLaunch(false);
    }

    internal void Init()
    {
      this.FillProfileComboBox();
    }

    internal void InitUI()
    {
      GuidanceWindow.IsDirty = false;
      this.mGuidanceData.Clear();
      foreach (IMAction gameControl in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
      {
        if (gameControl.Type == KeyActionType.Pan)
          this.lstPanTags.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name);
        else if (gameControl.Type == KeyActionType.MOBADpad)
          this.lstMOBATags.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name);
        string category = string.Empty;
        if (this.ParentWindow.SelectedConfig.SelectedControlScheme.IsCategoryVisible)
          category = string.Equals(gameControl.GuidanceCategory.Trim(), "MISC", StringComparison.InvariantCulture) ? LocaleStrings.GetLocalizedString("STRING_" + gameControl.GuidanceCategory.Trim(), "") : this.ParentWindow.SelectedConfig.GetUIString(gameControl.GuidanceCategory.Trim());
        Dictionary<string, string> dictionary = gameControl.Guidance.DeepCopy<Dictionary<string, string>>();
        if (gameControl.Type == KeyActionType.Dpad)
        {
          dictionary.Clear();
          foreach (DPadControls dpadControls in Enum.GetValues(typeof (DPadControls)))
          {
            if (gameControl.Guidance.ContainsKey(dpadControls.ToString()))
              dictionary.Add(dpadControls.ToString(), gameControl.Guidance[dpadControls.ToString()]);
          }
        }
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          string str = string.Empty;
          if (gameControl is MOBASkill mobaSkill && keyValuePair.Key.Contains("KeyActivate"))
            str = GuidanceWindow.AppendMOBASkillModeInGuidance(mobaSkill);
          int num = keyValuePair.Key.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
          if (num == 0 && IMAction.DictPropertyInfo[gameControl.Type].ContainsKey(keyValuePair.Key))
            this.mGuidanceData.AddGuidance(false, category, this.ParentWindow.SelectedConfig.GetUIString(gameControl.Guidance[keyValuePair.Key]) + str, gameControl[keyValuePair.Key].ToString(), keyValuePair.Key, gameControl);
          string index = num != 0 ? keyValuePair.Key : keyValuePair.Key + "_alt1";
          if (IMAction.DictPropertyInfo[gameControl.Type].ContainsKey(index))
            this.mGuidanceData.AddGuidance(true, category, this.ParentWindow.SelectedConfig.GetUIString(gameControl.Guidance[keyValuePair.Key]) + str, gameControl[index].ToString(), index, gameControl);
        }
      }
      this.mGamepadIcon.Visibility = this.mGuidanceData.GamepadViewGuidance.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
      this.mGuidanceData.SaveOriginalData();
      if ((this.mGuidanceData.GamepadViewGuidance.Count > 0 ? 1 : (this.mGuidanceData.KeymapViewGuidance.Count > 0 ? 1 : 0)) != 0)
      {
        this.mKeyboardIcon.Visibility = Visibility.Visible;
        this.separator.Visibility = Visibility.Visible;
        this.noGameGuidePanel.Visibility = Visibility.Collapsed;
        this.AddVideoElementInUI();
      }
      else
      {
        this.mKeyboardIcon.Visibility = Visibility.Collapsed;
        this.separator.Visibility = Visibility.Collapsed;
        this.noGameGuidePanel.Visibility = Visibility.Visible;
      }
      this.ShowGuidance();
    }

    private static string AppendMOBASkillModeInGuidance(MOBASkill mobaSkill)
    {
      string str = string.Empty;
      if (!mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_MANUAL_MODE", "") + ")");
      else if (mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_AUTOCAST", "") + ")");
      else if (mobaSkill.AdvancedMode && mobaSkill.AutocastEnabled)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_QUICK_CAST", "") + ")");
      return str;
    }

    private void ResetGuidanceTab()
    {
      this.mIsGamePadTabSelected = false;
      this.mGamepadIconImage.ImageName = "gamepad_overlay_icon";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
      this.mKeyboardIconImage.ImageName = "guidance_controls_click";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
    }

    private int CompareSchemesAlphabetically(IMControlScheme x, IMControlScheme y)
    {
      string strA = x.Name.ToLower(CultureInfo.InvariantCulture).Trim();
      string strB = y.Name.ToLower(CultureInfo.InvariantCulture).Trim();
      return strA.Contains(strB) || !strB.Contains(strA) && string.CompareOrdinal(strA, strB) >= 0 ? 1 : -1;
    }

    internal void OrderingControlSchemes()
    {
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      this.ParentWindow.SelectedConfig.ControlSchemes.Sort(new Comparison<IMControlScheme>(this.CompareSchemesAlphabetically));
      foreach (IMControlScheme imControlScheme in new List<IMControlScheme>((IEnumerable<IMControlScheme>) this.ParentWindow.SelectedConfig.ControlSchemes))
      {
        if (imControlScheme.BuiltIn)
        {
          if (imControlScheme.IsBookMarked)
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
            this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index3, imControlScheme);
            ++index3;
            ++index2;
            ++index1;
          }
          else
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
            this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index2, imControlScheme);
            ++index2;
            ++index1;
          }
        }
        else if (imControlScheme.IsBookMarked)
        {
          this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
          this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index1, imControlScheme);
          ++index1;
        }
      }
    }

    internal void FillProfileComboBox()
    {
      this.OrderingControlSchemes();
      this.mSchemesComboBox.Items.Clear();
      if (this.ParentWindow.SelectedConfig.ControlSchemes == null || this.ParentWindow.SelectedConfig.ControlSchemes.Count <= 0)
        return;
      bool flag = false;
      foreach (IMControlScheme imControlScheme in this.ParentWindow.SelectedConfig.ControlSchemesDict.Values)
      {
        ComboBoxItem comboBoxItem1 = new ComboBoxItem();
        comboBoxItem1.Content = (object) imControlScheme.Name;
        ComboBoxItem comboBoxItem2 = comboBoxItem1;
        if (string.Equals(imControlScheme.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))
        {
          comboBoxItem2.IsSelected = true;
          flag = true;
        }
        comboBoxItem2.ToolTip = comboBoxItem2.Content;
        this.mSchemesComboBox.Items.Add((object) comboBoxItem2);
      }
      if (!flag)
        ((ListBoxItem) this.mSchemesComboBox.Items[0]).IsSelected = true;
      this.mSchemePanel.Visibility = this.ParentWindow.SelectedConfig.ControlSchemesDict.Count == 1 ? Visibility.Collapsed : Visibility.Visible;
      this.InitUI();
    }

    private void AddVideoElementInUI()
    {
      foreach (AppInfo appInfo in ((IEnumerable<AppInfo>) new JsonParser(BlueStacks.Common.Strings.CurrentDefaultVmName).GetAppList()).ToList<AppInfo>())
      {
        if (string.Equals(appInfo.Package, KMManager.sPackageName, StringComparison.InvariantCulture))
          this.mIsGuidanceVideoPresent = appInfo.VideoPresent;
      }
      this.UpdateVideoElement(this.mIsGamePadTabSelected);
    }

    private void UpdateVideoElement(bool isGamepadTabSelected = false)
    {
      KMManager.sVideoMode = !isGamepadTabSelected ? (this.lstMOBATags.Contains(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name) || this.lstMOBATags.Contains("GlobalValidTag") ? GuidanceVideoType.Moba : (this.lstPanTags.Contains(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name) || this.lstPanTags.Contains("GlobalValidTag") ? GuidanceVideoType.Pan : GuidanceVideoType.Default)) : GuidanceVideoType.Gamepad;
      this.UpdateTutorialGrid();
      this.UpdateReadArticleGrid();
      this.GuidanceVisualInfoVisibility();
    }

    private void UpdateTutorialGrid()
    {
      string str = "";
      try
      {
        Dictionary<string, CustomThumbnail> customThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.CustomThumbnails;
        Dictionary<GuidanceVideoType, VideoThumbnailInfo> defaultThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.DefaultThumbnails;
        if (customThumbnails.ContainsKey(KMManager.sPackageName))
        {
          if (KMManager.sVideoMode == GuidanceVideoType.Gamepad && customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
          {
            str = ((VideoThumbnailInfo) customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).ImagePath;
          }
          else
          {
            CustomThumbnail customThumbnail1 = customThumbnails[KMManager.sPackageName];
            GuidanceVideoType guidanceVideoType = GuidanceVideoType.SchemeSpecific;
            string index1 = guidanceVideoType.ToString();
            if (customThumbnail1[index1] != null)
            {
              CustomThumbnail customThumbnail2 = customThumbnails[KMManager.sPackageName];
              guidanceVideoType = GuidanceVideoType.SchemeSpecific;
              string index2 = guidanceVideoType.ToString();
              if (((Dictionary<string, VideoThumbnailInfo>) customThumbnail2[index2]).ContainsKey(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name))
              {
                string name = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
                CustomThumbnail customThumbnail3 = customThumbnails[KMManager.sPackageName];
                guidanceVideoType = GuidanceVideoType.SchemeSpecific;
                string index3 = guidanceVideoType.ToString();
                str = ((Dictionary<string, VideoThumbnailInfo>) customThumbnail3[index3])[name].ImagePath;
                KMManager.sVideoMode = GuidanceVideoType.SchemeSpecific;
                goto label_14;
              }
            }
            CustomThumbnail customThumbnail4 = customThumbnails[KMManager.sPackageName];
            guidanceVideoType = GuidanceVideoType.Special;
            string index4 = guidanceVideoType.ToString();
            if (customThumbnail4[index4] != null)
            {
              CustomThumbnail customThumbnail2 = customThumbnails[KMManager.sPackageName];
              guidanceVideoType = GuidanceVideoType.Special;
              string index2 = guidanceVideoType.ToString();
              str = ((VideoThumbnailInfo) customThumbnail2[index2]).ImagePath;
              KMManager.sVideoMode = GuidanceVideoType.Special;
            }
            else if (customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
              str = ((VideoThumbnailInfo) customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).ImagePath;
          }
        }
        else if (defaultThumbnails.ContainsKey(KMManager.sVideoMode))
          str = defaultThumbnails[KMManager.sVideoMode].ImagePath;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in evaluating tutorial grid : " + ex.ToString());
      }
label_14:
      this.mVideoThumbnail.ImageName = str;
      this.IsVideoTutorialAvailable = !string.IsNullOrEmpty(str);
    }

    private void UpdateReadArticleGrid()
    {
      this.mHelpArticleUrl = (string) null;
      try
      {
        Dictionary<string, HelpArticle> helpArticles = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.HelpArticles;
        if (helpArticles.ContainsKey(KMManager.sPackageName) && helpArticles[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
        {
          string name = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
          Dictionary<string, HelpArticleInfo> dictionary = (Dictionary<string, HelpArticleInfo>) helpArticles[KMManager.sPackageName][GuidanceVideoType.SchemeSpecific.ToString()];
          if (KMManager.sVideoMode == GuidanceVideoType.SchemeSpecific && dictionary.ContainsKey(name))
          {
            this.mHelpArticleUrl = dictionary[name].HelpArticleUrl;
          }
          else
          {
            if (KMManager.sVideoMode == GuidanceVideoType.SchemeSpecific)
              return;
            this.mHelpArticleUrl = ((HelpArticleInfo) helpArticles[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).HelpArticleUrl;
          }
        }
        else
        {
          if (!helpArticles.ContainsKey("default") || helpArticles["default"][KMManager.sVideoMode.ToString()] == null)
            return;
          this.mHelpArticleUrl = ((HelpArticleInfo) helpArticles["default"][KMManager.sVideoMode.ToString()]).HelpArticleUrl;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in evaluating read article : " + ex.ToString());
      }
    }

    private void GuidanceWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Activate();
    }

    internal void ResizeGuidanceWindow()
    {
      bool flag = false;
      IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.ParentWindow.Handle, true);
      double width = this.ParentWindow.Width * MainWindow.sScalingFactor;
      double height = this.ParentWindow.Height * MainWindow.sScalingFactor;
      if (width + (double) this.mSidebarWidth * MainWindow.sScalingFactor + this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor > (double) fullscreenMonitorSize.Width)
      {
        width = (double) fullscreenMonitorSize.Width - (double) this.mSidebarWidth * MainWindow.sScalingFactor - this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor;
        height = this.ParentWindow.GetHeightFromWidth(width, true, false);
        flag = true;
      }
      if (height > (double) fullscreenMonitorSize.Height)
      {
        height = (double) fullscreenMonitorSize.Height;
        width = this.ParentWindow.GetWidthFromHeight(height, true, false);
        flag = true;
      }
      double top;
      if (this.ParentWindow.Top * MainWindow.sScalingFactor + height > (double) (fullscreenMonitorSize.Height + fullscreenMonitorSize.Y))
      {
        top = (double) (fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) - height;
        flag = true;
      }
      else
        top = this.ParentWindow.Top * MainWindow.sScalingFactor;
      double left;
      if (this.ParentWindow.Left * MainWindow.sScalingFactor + width + ((double) this.mSidebarWidth + this.ParentWindow.mSidebar.Width) * MainWindow.sScalingFactor > (double) (fullscreenMonitorSize.Width + fullscreenMonitorSize.X))
      {
        left = (double) (fullscreenMonitorSize.X + fullscreenMonitorSize.Width) - width - ((double) this.mSidebarWidth + this.ParentWindow.mSidebar.Width) * MainWindow.sScalingFactor;
        flag = true;
      }
      else
        left = this.ParentWindow.Left * MainWindow.sScalingFactor;
      if (flag)
        this.ParentWindow.ChangeHeightWidthTopLeft(width, height, top, left);
      this.Left = this.mGuidanceWindowLeft == -1.0 ? this.ParentWindow.Left + this.ParentWindow.ActualWidth : this.mGuidanceWindowLeft;
      this.Top = this.mGuidanceWindowTop == -1.0 ? this.ParentWindow.Top : this.mGuidanceWindowTop;
      this.Height = this.ParentWindow.ActualHeight;
    }

    internal void DimOverLayVisibility(Visibility visible)
    {
      this.mOverlayGrid.Visibility = visible;
    }

    private void GuidanceWindow_Closing(object sender, CancelEventArgs e1)
    {
      if (!this.IsViewState && (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged((object) this.mGuidanceData)))
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANCEL_CONFIG_CHANGES", "");
        customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), (EventHandler) ((o, e2) =>
        {
          KMManager.LoadIMActions(this.ParentWindow, KMManager.sPackageName);
          GuidanceWindow.sIsDirty = false;
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) ((o, evt) => e1.Cancel = true), (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) => e1.Cancel = true), (object) null);
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }
      this.mGuidanceWindowLeft = this.Left;
      this.mGuidanceWindowTop = this.Top;
    }

    private void GuidanceWindow_Closed(object sender, EventArgs e)
    {
      if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>) (appSettings => appSettings.IsCloseGuidanceOnboardingCompleted), out string _) && this.mIsOnboardingPopupToBeShownOnGuidanceClose)
      {
        this.ParentWindow.mSidebar?.ShowViewGuidancePopup();
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
      }
      KMManager.sGuidanceWindow = (GuidanceWindow) null;
      this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
      this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsAnyOperationPendingForTab = false;
    }

    private void ProfileComboBox_ProfileChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!this.mSchemesComboBox.IsDropDownOpen)
        return;
      this.mSchemesComboBox.IsDropDownOpen = false;
      if (this.mSchemesComboBox.SelectedItem == null)
        return;
      string schemeSelected = ((ContentControl) this.mSchemesComboBox.SelectedItem).Content.ToString();
      if (!this.SelectControlScheme(schemeSelected))
        return;
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_USING_SCHEME", "") + " : " + schemeSelected);
      KMManager.SendSchemeChangedStats(this.ParentWindow, "game_guide");
      KMManager.ShowShootingModeTooltip(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
    }

    internal bool SelectControlScheme(string schemeSelected)
    {
      if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(schemeSelected))
      {
        if (!this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected].Selected)
        {
          this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
          this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected];
          this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
          if (!GuidanceWindow.sIsDirty)
          {
            GuidanceWindow.sIsDirty = true;
            this.SaveGuidanceChanges();
          }
          else
            GuidanceWindow.sIsDirty = false;
          this.mSchemesComboBox.SelectedValue = (object) schemeSelected;
          return true;
        }
        this.InitUI();
      }
      return false;
    }

    private void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((Window) this);
        this.mToastPopup.Init((Window) this, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mIsOnboardingPopupToBeShownOnGuidanceClose = true;
      this.ShowWithParentWindow = false;
      this.Close();
      GuidanceWindow.HideOnNextLaunch(true);
      this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.Focus();
    }

    internal void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
    {
      Logger.Info("Restarting Game Tab.");
      new Thread((ThreadStart) (() => this.ParentWindow.mTopBar.mAppTabButtons.RestartTab(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName)))
      {
        IsBackground = true
      }.Start();
    }

    public static void HideOnNextLaunch(bool updatedFlag)
    {
      List<string> list = new List<string>((IEnumerable<string>) RegistryManager.Instance.DisabledGuidancePackages);
      if (updatedFlag)
        list.AddIfNotContain<string>(KMManager.sPackageName);
      else if (list.Contains(KMManager.sPackageName))
        list.Remove(KMManager.sPackageName);
      RegistryManager.Instance.DisabledGuidancePackages = list.ToArray();
    }

    internal OnBoardingPopupWindow GuidanceSchemeOnboardingBlurb()
    {
      OnBoardingPopupWindow boardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      boardingPopupWindow.Owner = (Window) this.ParentWindow;
      boardingPopupWindow.Title = "SelectedGameSchemeBlurb";
      boardingPopupWindow.PlacementTarget = (UIElement) this.mSchemesComboBox;
      boardingPopupWindow.LeftMargin = 50;
      boardingPopupWindow.TopMargin = 4;
      boardingPopupWindow.Startevent += (System.Action) (() => this.mSchemesComboBox.Highlight = true);
      boardingPopupWindow.Endevent += (System.Action) (() => this.mSchemesComboBox.Highlight = false);
      boardingPopupWindow.IsBlurbRelatedToGuidance = true;
      boardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE", "");
      boardingPopupWindow.BodyContent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE_MESSAGE", ""), (object) this.ParentWindow.SelectedConfig.SelectedControlScheme.Name);
      boardingPopupWindow.Left = this.mSchemesComboBox.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow.LeftMargin;
      boardingPopupWindow.Top = this.mSchemesComboBox.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow.TopMargin;
      return boardingPopupWindow;
    }

    internal OnBoardingPopupWindow GuidanceOnboardingBlurb()
    {
      if (this.mGuidanceKeysGrid.ActualHeight < 1.0)
        return (OnBoardingPopupWindow) null;
      OnBoardingPopupWindow boardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      boardingPopupWindow.Owner = (Window) this.ParentWindow;
      boardingPopupWindow.PlacementTarget = (UIElement) this.mGuidanceKeysGrid;
      boardingPopupWindow.Title = "GameControlBlurb";
      boardingPopupWindow.LeftMargin = 320;
      boardingPopupWindow.TopMargin = (230 - (int) this.mGuidanceKeysGrid.ActualHeight) / 2;
      boardingPopupWindow.Startevent += (System.Action) (() => this.mGuidanceKeyBorder.BorderThickness = new Thickness(2.0));
      boardingPopupWindow.Endevent += (System.Action) (() => this.mGuidanceKeyBorder.BorderThickness = new Thickness(0.0));
      boardingPopupWindow.IsBlurbRelatedToGuidance = true;
      boardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_HEADER", "");
      boardingPopupWindow.BodyContent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_MESSAGE", ""));
      boardingPopupWindow.PopArrowAlignment = PopupArrowAlignment.Right;
      boardingPopupWindow.SetValue(Window.LeftProperty, (object) (this.mGuidanceKeysGrid.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow.LeftMargin));
      boardingPopupWindow.SetValue(Window.TopProperty, (object) (this.mGuidanceKeysGrid.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow.TopMargin));
      return boardingPopupWindow;
    }

    internal OnBoardingPopupWindow GuidanceVideoOnboardingBlurb()
    {
      if (this.mVideoBorder.Visibility != Visibility.Visible && this.mQuickLearnBorder.Visibility != Visibility.Visible)
        return (OnBoardingPopupWindow) null;
      OnBoardingPopupWindow boardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      boardingPopupWindow.Owner = (Window) this.ParentWindow;
      boardingPopupWindow.Title = "GuidanceVideoBlurb";
      boardingPopupWindow.IsBlurbRelatedToGuidance = true;
      boardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_HEADER", "");
      boardingPopupWindow.BodyContent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_MESSAGE", ""));
      boardingPopupWindow.PopArrowAlignment = PopupArrowAlignment.Right;
      boardingPopupWindow.LeftMargin = 320;
      if (this.mVideoBorder.Visibility == Visibility.Visible)
      {
        boardingPopupWindow.PlacementTarget = (UIElement) this.mVideoBorder;
        boardingPopupWindow.TopMargin = ((int) this.mVideoBorder.ActualHeight - 80) / 2;
        boardingPopupWindow.Startevent += (System.Action) (() => this.mVideoBorder.BorderThickness = new Thickness(2.0));
        boardingPopupWindow.Endevent += (System.Action) (() => this.mVideoBorder.BorderThickness = new Thickness(0.0));
        boardingPopupWindow.SetValue(Window.LeftProperty, (object) (this.mVideoBorder.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow.LeftMargin));
        boardingPopupWindow.SetValue(Window.TopProperty, (object) (this.mVideoBorder.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow.TopMargin));
      }
      else
      {
        boardingPopupWindow.PlacementTarget = (UIElement) this.mQuickLearnBorder;
        boardingPopupWindow.TopMargin = ((int) this.mQuickLearnBorder.ActualHeight + 160) / 2;
        boardingPopupWindow.Startevent += (System.Action) (() => this.mQuickLearnBorder.BorderThickness = new Thickness(2.0));
        boardingPopupWindow.Endevent += (System.Action) (() => this.mQuickLearnBorder.BorderThickness = new Thickness(0.0));
        boardingPopupWindow.SetValue(Window.LeftProperty, (object) (this.mQuickLearnBorder.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow.LeftMargin));
        boardingPopupWindow.SetValue(Window.TopProperty, (object) (this.mQuickLearnBorder.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow.TopMargin));
      }
      return boardingPopupWindow;
    }

    private void ControlsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mControlsGrid.Visibility = Visibility.Visible;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
    }

    private void SettingsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mControlsGrid.Visibility = Visibility.Collapsed;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowForegroundDimColor");
    }

    private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("guidance-howtoplay", "watchvideo", this.ParentWindow.mVmName, KMManager.sPackageName, "", "", "");
      using (GuidanceVideoWindow guidanceVideoWindow = new GuidanceVideoWindow(this.ParentWindow))
      {
        guidanceVideoWindow.Owner = (Window) this.ParentWindow;
        guidanceVideoWindow.Width = Math.Max(this.ParentWindow.ActualWidth * 0.7, 700.0);
        guidanceVideoWindow.Height = Math.Max(this.ParentWindow.ActualHeight * 0.7, 450.0);
        guidanceVideoWindow.Loaded += new RoutedEventHandler(this.Window_Loaded);
        guidanceVideoWindow.ShowDialog();
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (KMManager.sGuidanceWindow == null || KMManager.sGuidanceWindow.mGuidanceHasBeenMoved)
          return;
        CustomWindow customWindow = sender as CustomWindow;
        customWindow.Left = this.ParentWindow.Left + (this.ParentWindow.Width + KMManager.sGuidanceWindow.ActualWidth - customWindow.ActualWidth) / 2.0;
        customWindow.Top = this.ParentWindow.Top + (this.ParentWindow.Height - customWindow.ActualHeight) / 2.0;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in setting position guidance video window: " + ex.ToString());
      }
    }

    private void GamepadIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mKeyboardIconImage.ImageName = "guidance_controls";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
      this.mGamepadIconImage.ImageName = "gamepad_overlay_icon_click";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
      this.mIsGamePadTabSelected = true;
      this.ShowGuidance();
    }

    private void KeyboardIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mGamepadIconImage.ImageName = "gamepad_overlay_icon";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
      this.mKeyboardIconImage.ImageName = "guidance_controls_click";
      BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
      this.mIsGamePadTabSelected = false;
      this.ShowGuidance();
    }

    private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("guidance-howtoplay", "readarticle", this.ParentWindow.mVmName, KMManager.sPackageName, "", "", "");
      if (this.mHelpArticleUrl != null)
        Utils.OpenUrl(this.mHelpArticleUrl);
      e.Handled = true;
    }

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (!this.mHeaderGrid.IsMouseOver || e.OriginalSource.GetType().Equals(typeof (TextBlock)) || this.mControlsTab.IsMouseOver)
          return;
        this.DragMove();
        this.mGuidanceHasBeenMoved = true;
        this.ResizeMode = ResizeMode.CanResizeWithGrip;
      }
      catch
      {
      }
    }

    internal void GuidanceWindowTabSelected(string mSelectedTab)
    {
      mSelectedTab = string.IsNullOrEmpty(mSelectedTab) ? (this.mGuidanceData.GamepadViewGuidance.Count <= 0 || !this.ParentWindow.IsGamepadConnected ? "default" : "gamepad") : mSelectedTab;
      if (mSelectedTab == "gamepad")
      {
        if (this.mGamepadIcon.Visibility != Visibility.Visible)
          return;
        this.GamepadIconPreviewMouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
      }
      else
      {
        if (this.mKeyboardIcon.Visibility != Visibility.Visible)
          return;
        this.KeyboardIconPreviewMouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
      }
    }

    private void GuidanceWindow_KeyDown(object sender, KeyEventArgs e)
    {
      string b = string.Empty;
      if (e.Key != Key.None)
      {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
          b = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
          b = b + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
          b = b + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
        b += IMAPKeys.GetStringForFile(e.Key);
      }
      Logger.Debug("SHORTCUT: KeyPressed.." + b);
      if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance == null)
        return;
      foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
      {
        if (string.Equals(shortcutKeys.ShortcutKey, b, StringComparison.InvariantCulture))
        {
          if (string.Equals(shortcutKeys.ShortcutName, "STRING_TOGGLE_KEYMAP_WINDOW", StringComparison.InvariantCulture))
            this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("default", false);
          else if (string.Equals(shortcutKeys.ShortcutName, "STRING_GAMEPAD_CONTROLS", StringComparison.InvariantCulture))
            this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad", false);
        }
      }
    }

    internal void UpdateSize()
    {
      if (this.mGuidanceHasBeenMoved)
        return;
      this.Left = this.mGuidanceWindowLeft == -1.0 ? this.ParentWindow.Left + this.ParentWindow.ActualWidth : this.mGuidanceWindowLeft;
      this.Top = this.mGuidanceWindowTop == -1.0 ? this.ParentWindow.Top : this.mGuidanceWindowTop;
      this.Height = this.ParentWindow.ActualHeight;
    }

    private void CustomWindow_StateChanged(object sender, EventArgs e)
    {
      this.WindowState = WindowState.Normal;
    }

    private void GuidanceWindow_Activated(object sender, EventArgs e)
    {
      try
      {
        if (!RegistryManager.Instance.ShowKeyControlsOverlay || !KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
          return;
        KMManager.dictOverlayWindow[this.ParentWindow]?.Show();
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception in GuidanceWindow_Activated {0}", (object) ex));
      }
    }

    private void GuidanceWindow_Deactivated(object sender, EventArgs e)
    {
      try
      {
        if (this.ParentWindow.IsActive || !KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
          return;
        KMManager.dictOverlayWindow[this.ParentWindow].Hide();
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception in GuidanceWindow_Deactivated {0}", (object) ex));
      }
    }

    private void GuidanceKeyTextChanged(object sender, TextChangedEventArgs e)
    {
      IEnumerable<GuidanceCategoryEditModel> source = this.mGuidanceData.KeymapEditGuidance.Union<GuidanceCategoryEditModel>((IEnumerable<GuidanceCategoryEditModel>) this.mGuidanceData.GamepadEditGuidance);
      source.SelectMany<GuidanceCategoryEditModel, GuidanceEditModel>((Func<GuidanceCategoryEditModel, IEnumerable<GuidanceEditModel>>) (cgem => (IEnumerable<GuidanceEditModel>) cgem.GuidanceEditModels)).OfType<GuidanceEditTextModel>().ToList<GuidanceEditTextModel>().ForEach((System.Action<GuidanceEditTextModel>) (gem => gem.TextValidityOption = TextValidityOptions.Success));
      if (sender is IMapTextBox imapTextBox)
      {
        GuidanceEditTextModel guidanceEditTextModel = imapTextBox.DataContext as GuidanceEditTextModel;
        if (guidanceEditTextModel != null && !string.Equals(guidanceEditTextModel.OriginalGuidanceKey, guidanceEditTextModel.GuidanceKey, StringComparison.OrdinalIgnoreCase))
        {
          GuidanceWindow.sIsDirty = true;
          if (imapTextBox.ToolTip is ToolTip toolTip)
            toolTip.PlacementTarget = (UIElement) imapTextBox;
          if (source.SelectMany<GuidanceCategoryEditModel, GuidanceEditModel>((Func<GuidanceCategoryEditModel, IEnumerable<GuidanceEditModel>>) (cgem => (IEnumerable<GuidanceEditModel>) cgem.GuidanceEditModels)).OfType<GuidanceEditTextModel>().Where<GuidanceEditTextModel>((Func<GuidanceEditTextModel, bool>) (gem => !string.IsNullOrEmpty(gem.GuidanceKey) && (object) gem.PropertyType == (object) typeof (string) && string.Equals(guidanceEditTextModel.GuidanceKey, gem.GuidanceKey, StringComparison.OrdinalIgnoreCase))).Count<GuidanceEditTextModel>() > 1)
          {
            imapTextBox.InputTextValidity = TextValidityOptions.Warning;
            if (toolTip != null)
              toolTip.IsOpen = true;
          }
        }
      }
      this.mSaveBtn.IsEnabled = GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged((object) this.mGuidanceData);
    }

    private void StepperTextChanged(object sender, TextChangedEventArgs e)
    {
      if (sender is StepperTextBox stepperTextBox && stepperTextBox.DataContext is GuidanceEditDecimalModel dataContext && !string.Equals(dataContext.OriginalGuidanceKey, dataContext.GuidanceKey, StringComparison.OrdinalIgnoreCase))
        GuidanceWindow.sIsDirty = true;
      this.mSaveBtn.IsEnabled = GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged((object) this.mGuidanceData);
    }

    private void EditBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ShowEditGuidance();
      this.DataModificationTracker.Lock((object) this.mGuidanceData.DeepCopy<GuidanceData>(), new List<string>()
      {
        "KeymapViewGuidance",
        "GamepadViewGuidance",
        "Item",
        "TextValidityOption",
        "GuidanceText",
        "OriginalGuidanceKey",
        "IMActionItems",
        "PropertyType",
        "ActionType"
      }, true);
      ClientStats.SendKeyMappingUIStatsAsync("guide_edit", KMManager.sPackageName, "");
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
      this.SaveGuidanceChanges();
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      ClientStats.SendKeyMappingUIStatsAsync("guide_save", KMManager.sPackageName, "");
    }

    private void SaveGuidanceChanges()
    {
      Logger.Debug(string.Format("ExtraLog: SaveGuidanceChanges, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) this.ParentWindow.mVmName, (object) this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, (object) this.ParentWindow.SelectedConfig.ControlSchemes.Count));
      bool flag = false;
      if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count != this.ParentWindow.SelectedConfig.ControlSchemes.Count)
        flag = true;
      if (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged((object) this.mGuidanceData))
      {
        GuidanceWindow.sIsDirty = true;
        KMManager.SaveIMActions(this.ParentWindow, true, false);
      }
      if (flag)
        this.FillProfileComboBox();
      else
        this.InitUI();
      this.ShowViewGuidance();
      if (!KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) || KMManager.dictOverlayWindow[this.ParentWindow] == null)
        return;
      KMManager.dictOverlayWindow[this.ParentWindow].Init();
      if (!RegistryManager.Instance.ShowKeyControlsOverlay)
        return;
      KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
    }

    private void DiscardBtn_Click(object sender, RoutedEventArgs e1)
    {
      if (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged((object) this.mGuidanceData))
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_CHANGES", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_GUIDANCE_CHANGES", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_DISCARD", (EventHandler) ((o, e2) =>
        {
          string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
          IEnumerable<IMControlScheme> source = this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme_ => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase)));
          if (!source.Any<IMControlScheme>())
            return;
          this.mGuidanceData.Reset();
          IMControlScheme imControlScheme1 = source.Count<IMControlScheme>() == 1 ? source.First<IMControlScheme>() : source.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme_ => !scheme_.BuiltIn)).First<IMControlScheme>();
          if (imControlScheme1.BuiltIn)
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
            IMControlScheme imControlScheme2 = this.ParentWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture))).FirstOrDefault<IMControlScheme>();
            if (imControlScheme2 != null)
            {
              imControlScheme2.Selected = true;
              this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme2;
              this.ParentWindow.SelectedConfig.ControlSchemesDict[imControlScheme2.Name] = imControlScheme2;
              GuidanceWindow.sIsDirty = true;
            }
          }
          else
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
            this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme1.DeepCopy();
            this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = this.ParentWindow.SelectedConfig.SelectedControlScheme;
            this.ParentWindow.SelectedConfig.ControlSchemes.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme);
            this.InitUI();
          }
          this.ShowViewGuidance();
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) ((o, e2) => {}), (string) null, false, (object) null, true);
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }
      else
        this.ShowViewGuidance();
    }

    private void ShowGuidance()
    {
      if (this.IsViewState)
        this.ShowViewGuidance();
      else
        this.ShowEditGuidance();
    }

    private void ShowViewGuidance()
    {
      this.IsViewState = true;
      this.mViewDock.Visibility = Visibility.Visible;
      this.mEditDock.Visibility = Visibility.Collapsed;
      if (this.mGuidanceData.GamepadViewGuidance.Count == 0)
        this.mIsGamePadTabSelected = false;
      ObservableCollection<GuidanceCategoryViewModel> observableCollection = this.mIsGamePadTabSelected ? this.mGuidanceData.GamepadViewGuidance : this.mGuidanceData.KeymapViewGuidance;
      if (observableCollection != null)
      {
        if (observableCollection.Count == 1)
        {
          this.mGuidanceListBox.DataContext = (object) observableCollection[0].GuidanceViewModels;
          this.mGuidanceListBox.AlternationCount = 2;
        }
        else
        {
          this.mGuidanceListBox.DataContext = (object) observableCollection;
          this.mGuidanceListBox.AlternationCount = 0;
        }
      }
      this.mEditBtn.Visibility = this.ParentWindow.SelectedConfig.SelectedControlScheme == null || !this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>() || !this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany<IMAction, KeyValuePair<string, string>>((Func<IMAction, IEnumerable<KeyValuePair<string, string>>>) (action => (IEnumerable<KeyValuePair<string, string>>) action.Guidance)).Any<KeyValuePair<string, string>>() ? Visibility.Collapsed : Visibility.Visible;
      this.mRevertBtn.Visibility = this.ParentWindow.SelectedConfig.ControlSchemes.Count<IMControlScheme>((Func<IMControlScheme, bool>) (x => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))) == 2 ? Visibility.Visible : Visibility.Collapsed;
      this.UpdateVideoElement(this.mIsGamePadTabSelected);
      GuidanceWindow.sIsDirty = false;
      this.mSaveBtn.IsEnabled = false;
    }

    private void ShowEditGuidance()
    {
      this.IsViewState = false;
      this.mViewDock.Visibility = Visibility.Collapsed;
      this.mEditDock.Visibility = Visibility.Visible;
      ObservableCollection<GuidanceCategoryEditModel> observableCollection = this.mIsGamePadTabSelected ? this.mGuidanceData.GamepadEditGuidance : this.mGuidanceData.KeymapEditGuidance;
      if (observableCollection == null)
        return;
      if (observableCollection.Count == 1)
      {
        this.mGuidanceListBox.DataContext = (object) observableCollection[0].GuidanceEditModels;
        this.mGuidanceListBox.AlternationCount = 2;
      }
      else
      {
        this.mGuidanceListBox.DataContext = (object) observableCollection;
        this.mGuidanceListBox.AlternationCount = 0;
      }
    }

    private void RevertBtn_Click(object sender, RoutedEventArgs e1)
    {
      if (this.ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESET", (EventHandler) ((o, e2) =>
      {
        Logger.Debug(string.Format("ExtraLog: Revert Clicked, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) this.ParentWindow.mVmName, (object) this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, (object) this.ParentWindow.SelectedConfig.ControlSchemes.Count));
        string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
        bool isBookMarked = this.ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
        this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
        IMControlScheme imControlScheme = this.ParentWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture) && scheme.BuiltIn)).FirstOrDefault<IMControlScheme>();
        if (imControlScheme == null)
          return;
        Logger.Debug(string.Format("ExtraLog: Updating scheme dictionary, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) this.ParentWindow.mVmName, (object) this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, (object) this.ParentWindow.SelectedConfig.ControlSchemes.Count));
        imControlScheme.Selected = true;
        imControlScheme.IsBookMarked = isBookMarked;
        this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme;
        this.ParentWindow.SelectedConfig.ControlSchemesDict[imControlScheme.Name] = imControlScheme;
        GuidanceWindow.sIsDirty = true;
        this.SaveGuidanceChanges();
        ClientStats.SendKeyMappingUIStatsAsync("guide_reset", KMManager.sPackageName, "");
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) ((o, e2) => {}), (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
    }

    public void Highlight()
    {
      if (!(BlueStacksUIBinding.Instance.ColorModel["BlueMouseDownBorderBackground"] is SolidColorBrush solidColorBrush))
        return;
      Border border1 = new Border();
      border1.BorderThickness = new Thickness(this.ActualWidth / 2.0, this.ActualHeight / 2.0, this.ActualWidth / 2.0, this.ActualHeight / 2.0);
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Colors.Transparent,
        Offset = 0.0
      });
      GradientStop gradientStop = new GradientStop();
      gradientStop.Offset = 1.0;
      int a = (int) solidColorBrush.Color.A;
      Color color = solidColorBrush.Color;
      int r = (int) color.R;
      color = solidColorBrush.Color;
      int g = (int) color.G;
      color = solidColorBrush.Color;
      int b = (int) color.B;
      gradientStop.Color = Color.FromArgb((byte) a, (byte) r, (byte) g, (byte) b);
      gradientStopCollection.Add(gradientStop);
      RadialGradientBrush radialGradientBrush = new RadialGradientBrush(gradientStopCollection);
      radialGradientBrush.RadiusX = 1.0;
      radialGradientBrush.RadiusY = 1.0;
      radialGradientBrush.Opacity = 0.5;
      border1.BorderBrush = (Brush) radialGradientBrush;
      Border border = border1;
      this.mGuidanceMainGrid.Children.Add((UIElement) border);
      new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(500);
        this.Dispatcher.BeginInvoke((Delegate) (() => this.mGuidanceMainGrid.Children.Remove((UIElement) border)));
      })).Start();
    }

    private void GuidanceWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.Visibility == Visibility.Visible)
        this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide_active");
      else
        this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
    }

    private void QuickLearnBorder_MouseUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("guidance-howtoplay", "quicklearn", this.ParentWindow.mVmName, KMManager.sPackageName, "", "", "");
      if (!PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.OnBoardingInfo.OnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName).GetValueOrDefault())
        return;
      this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl = new GameOnboardingControl(this.ParentWindow, KMManager.sPackageName, "guidancewindow");
      KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Visible);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl);
    }

    private void HowToPlay_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mHowToPlayCollapseExpand.ImageName = string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? "outline_settings_expand" : "outline_settings_collapse";
      this.GuidanceVisualInfoVisibility();
    }

    private void GuidanceVisualInfoVisibility()
    {
      if (string.Equals(KMManager.sPackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
      {
        this.mHowToPlayGrid.Visibility = Visibility.Visible;
        this.mQuickLearnBorder.Visibility = !PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.OnBoardingInfo.OnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName).GetValueOrDefault() || !string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
        this.mVideoTutorialBorder.Visibility = !this.IsVideoTutorialAvailable || !string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
        this.mReadArticleBorder.Visibility = string.IsNullOrEmpty(this.mHelpArticleUrl) || !string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
      }
      else
      {
        this.mVideoBorder.Visibility = this.IsVideoTutorialAvailable ? Visibility.Visible : Visibility.Collapsed;
        this.mReadArticlePanel.Visibility = !string.IsNullOrEmpty(this.mHelpArticleUrl) ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    internal void RefreshGameGuide()
    {
      this.Dispatcher.Invoke((Delegate) (() => this.FillProfileComboBox()));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/guidancewindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((Window) target).StateChanged += new EventHandler(this.CustomWindow_StateChanged);
          ((Window) target).Closing += new CancelEventHandler(this.GuidanceWindow_Closing);
          ((Window) target).Closed += new EventHandler(this.GuidanceWindow_Closed);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.GuidanceWindow_Loaded);
          ((UIElement) target).KeyDown += new KeyEventHandler(this.GuidanceWindow_KeyDown);
          ((UIElement) target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.GuidanceWindow_IsVisibleChanged);
          ((Window) target).Activated += new EventHandler(this.GuidanceWindow_Activated);
          ((Window) target).Deactivated += new EventHandler(this.GuidanceWindow_Deactivated);
          break;
        case 2:
          this.mGuidanceMainGrid = (Grid) target;
          break;
        case 3:
          this.mGameControlBorder = (Border) target;
          break;
        case 4:
          this.mHeaderGrid = (DockPanel) target;
          this.mHeaderGrid.MouseLeftButtonDown += new MouseButtonEventHandler(this.Grid_MouseLeftButtonDown);
          break;
        case 5:
          this.mControlsTab = (Grid) target;
          this.mControlsTab.MouseLeftButtonUp += new MouseButtonEventHandler(this.ControlsTabMouseLeftButtonUp);
          break;
        case 6:
          this.mControlsTabTextBlock = (TextBlock) target;
          break;
        case 7:
          this.mEditKeysGrid = (Grid) target;
          break;
        case 8:
          this.mEditKeysGridTextBlock = (TextBlock) target;
          break;
        case 9:
          this.mCloseSideBarWindow = (CustomPictureBox) target;
          this.mCloseSideBarWindow.MouseDown += new MouseButtonEventHandler(this.CustomPictureBox_MouseDown);
          this.mCloseSideBarWindow.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 10:
          this.mControlsGrid = (Grid) target;
          break;
        case 11:
          this.mSchemePanel = (StackPanel) target;
          break;
        case 12:
          this.mSchemeTextBlock = (TextBlock) target;
          break;
        case 13:
          this.mSchemesComboBox = (CustomComboBox) target;
          this.mSchemesComboBox.SelectionChanged += new SelectionChangedEventHandler(this.ProfileComboBox_ProfileChanged);
          break;
        case 14:
          this.mVideoBorder = (Border) target;
          break;
        case 15:
          ((UIElement) target).MouseUp += new MouseButtonEventHandler(this.CustomPictureBox_MouseUp);
          break;
        case 16:
          this.mVideoThumbnail = (CustomPictureBox) target;
          break;
        case 17:
          this.mHowToPlayGrid = (Border) target;
          break;
        case 18:
          ((UIElement) target).MouseUp += new MouseButtonEventHandler(this.HowToPlay_MouseUp);
          break;
        case 19:
          this.mHowToPlayCollapseExpand = (CustomPictureBox) target;
          break;
        case 20:
          this.mQuickLearnBorder = (Border) target;
          this.mQuickLearnBorder.MouseUp += new MouseButtonEventHandler(this.QuickLearnBorder_MouseUp);
          break;
        case 21:
          this.mVideoTutorialBorder = (Border) target;
          this.mVideoTutorialBorder.MouseUp += new MouseButtonEventHandler(this.CustomPictureBox_MouseUp);
          break;
        case 22:
          this.mReadArticleBorder = (Border) target;
          this.mReadArticleBorder.MouseUp += new MouseButtonEventHandler(this.ReadMoreLinkMouseLeftButtonUp);
          break;
        case 23:
          this.mKeysIconGrid = (DockPanel) target;
          break;
        case 24:
          this.mKeyboardIcon = (Grid) target;
          break;
        case 25:
          this.mKeyboardIconImage = (CustomPictureBox) target;
          this.mKeyboardIconImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.KeyboardIconPreviewMouseLeftButtonUp);
          break;
        case 26:
          this.mKeyboardIconSeparator = (Grid) target;
          break;
        case 27:
          this.mGamepadIcon = (Grid) target;
          break;
        case 28:
          this.mGamepadIconImage = (CustomPictureBox) target;
          this.mGamepadIconImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.GamepadIconPreviewMouseLeftButtonUp);
          break;
        case 29:
          this.mGamepadIconSeparator = (Grid) target;
          break;
        case 30:
          this.mReadArticlePanel = (StackPanel) target;
          break;
        case 31:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.ReadMoreLinkMouseLeftButtonUp);
          break;
        case 32:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.ReadMoreLinkMouseLeftButtonUp);
          break;
        case 33:
          this.separator = (Grid) target;
          break;
        case 34:
          this.mGuidanceKeyBorder = (Border) target;
          break;
        case 35:
          this.mGuidanceKeysGrid = (Grid) target;
          break;
        case 36:
          this.mGuidanceListBox = (ListBox) target;
          break;
        case 37:
          this.noGameGuidePanel = (StackPanel) target;
          break;
        case 38:
          this.mViewDock = (DockPanel) target;
          break;
        case 39:
          this.mEditBtn = (CustomButton) target;
          this.mEditBtn.Click += new RoutedEventHandler(this.EditBtn_Click);
          break;
        case 40:
          this.mRevertBtn = (CustomButton) target;
          this.mRevertBtn.Click += new RoutedEventHandler(this.RevertBtn_Click);
          break;
        case 41:
          this.mEditDock = (Grid) target;
          break;
        case 42:
          this.mDiscardBtn = (CustomButton) target;
          this.mDiscardBtn.Click += new RoutedEventHandler(this.DiscardBtn_Click);
          break;
        case 43:
          this.mSaveBtn = (CustomButton) target;
          this.mSaveBtn.Click += new RoutedEventHandler(this.SaveBtn_Click);
          break;
        case 44:
          this.mOverlayGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
