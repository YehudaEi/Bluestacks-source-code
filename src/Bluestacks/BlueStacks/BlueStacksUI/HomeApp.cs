// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HomeApp
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class HomeApp : UserControl, IComponentConnector
  {
    private static object syncRoot = new object();
    internal List<RecommendedApps> sAppRecommendationsPool = new List<RecommendedApps>();
    private string defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
    private WrapPanel InstalledAppsDrawer;
    private AppIconUI moreAppsIcon;
    private MainWindow mMainWindow;
    private SidePanelVisibility? mCurrentSidePanelVisibility;
    private bool mIsSidePanelContentLoadedOnce;
    private DispatcherTimer searchHoverTimer;
    private bool mIsShowSearchRecommendations;
    internal CustomPictureBox mBackgroundImage;
    internal Label mInstalledAppText;
    internal Grid mGridSeparator;
    internal CustomPictureBox mAppSettings;
    internal CustomPopUp mAppSettingsPopup;
    internal Grid dummyGrid;
    internal Border mAppSettingsPopupBorder;
    internal Border mMaskBorder1;
    internal System.Windows.Shapes.Path LeftArrow;
    internal ScrollViewer InstalledAppsDrawerScrollBar;
    internal Grid mAppRecommendationsGrid;
    internal TextBlock mDiscoverApps;
    internal ScrollViewer appRecomScrollViewer;
    internal StackPanel mAppRecommendationSectionsPanel;
    internal StackPanel mAppRecommendationsGenericMessages;
    internal CustomPictureBox mAppRecommendationsGenericMessageImage;
    internal TextBlock mAppRecommendationsGenericMessageText;
    internal Border mSearchGrid;
    internal Border searchTextBoxBorder;
    internal TextBox mSearchTextBox;
    internal Border Mask;
    internal Border mSearchRecommendationBorder;
    internal StackPanel searchRecomItems;
    internal Grid mMultiInstanceControlsGrid;
    internal Border mDockGridBorder;
    internal Grid mDockGrid;
    internal StackPanel mDockPanel;
    internal CustomPopUp mDockAppIconToolTipPopup;
    internal Border mMaskBorder2;
    internal TextBlock mDockIconText;
    internal System.Windows.Shapes.Path mDownArrow;
    internal CustomPopUp mMoreAppsDockPopup;
    internal Border mMaskBorder3;
    internal TextBlock mMoreAppsDockIconText;
    internal CustomPictureBox mCustomMessageBoxCloseButton;
    internal StackPanel mMoreAppsDockPanel;
    internal CustomPopUp mSuggestedAppPopUp;
    internal Border mMaskBorder4;
    internal CustomPictureBox mCloseAppSuggPopup;
    internal TextBlock mAppSuggestionPopUp;
    internal System.Windows.Shapes.Path UpArrow;
    internal ProgressBar mLoadingGrid;
    private bool _contentLoaded;

    private MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public HomeApp(MainWindow window)
    {
      this.InitializeComponent();
      this.mMainWindow = window;
      this.SetWallpaper();
      this.InstalledAppsDrawer = this.InstalledAppsDrawerScrollBar.Content as WrapPanel;
      this.mLoadingGrid.ProgressText = "STRING_LOADING_ENGINE";
      if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this) && PromotionObject.Instance != null)
        PromotionObject.BackgroundPromotionHandler += new EventHandler(this.HomeApp_BackgroundPromotionHandler);
      if (!FeatureManager.Instance.IsMultiInstanceControlsGridVisible)
        this.mMultiInstanceControlsGrid.Visibility = Visibility.Hidden;
      if (!FeatureManager.Instance.IsAppSettingsAvailable)
      {
        this.mAppSettings.Visibility = Visibility.Hidden;
        this.mGridSeparator.Visibility = Visibility.Hidden;
      }
      this.searchHoverTimer = new DispatcherTimer()
      {
        Interval = TimeSpan.FromMilliseconds(700.0)
      };
      this.searchHoverTimer.Tick += (EventHandler) ((_param1, _param2) => this.OpenSearchSuggestions());
      this.Mask.CornerRadius = new CornerRadius(0.0, this.searchTextBoxBorder.CornerRadius.TopRight, this.searchTextBoxBorder.CornerRadius.BottomRight, 0.0);
      this.GetSearchTextFromCloud();
    }

    internal void AddDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
    {
      int index = 0;
      foreach (AppIconUI appIconUi in this.mDockPanel.Children.OfType<AppIconUI>())
      {
        if (appIconUi.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
          ++index;
        else
          break;
      }
      AppIconUI newAppIconUi = this.GetNewAppIconUI(icon, downloadInstallApk);
      this.mDockPanel.Children.Insert(index, (UIElement) newAppIconUi);
      this.mDockPanel.Children.Remove((UIElement) this.moreAppsIcon);
      this.mDockPanel.Children.Add((UIElement) this.moreAppsIcon);
    }

    internal void AddMoreAppsDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
    {
      int index = 0;
      foreach (AppIconUI appIconUi in this.mMoreAppsDockPanel.Children.OfType<AppIconUI>())
      {
        if (appIconUi.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
          ++index;
        else
          break;
      }
      AppIconUI newAppIconUi = this.GetNewAppIconUI(icon, downloadInstallApk);
      this.mMoreAppsDockPanel.Children.Insert(index, (UIElement) newAppIconUi);
    }

    internal void AddInstallDrawerIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
    {
      int index = 0;
      foreach (AppIconUI appIconUi in this.InstalledAppsDrawer.Children.OfType<AppIconUI>())
      {
        if (appIconUi.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
          ++index;
        else
          break;
      }
      AppIconUI newAppIconUi = this.GetNewAppIconUI(icon, downloadInstallApk);
      this.InstalledAppsDrawer.Children.Insert(index, (UIElement) newAppIconUi);
    }

    internal void RemoveAppIconFromUI(AppIconModel appIcon)
    {
      this.InstalledAppsDrawer.Children.Remove((UIElement) this.InstalledAppsDrawer.Children.Cast<AppIconUI>().Where<AppIconUI>((Func<AppIconUI, bool>) (s => s.mAppIconModel.PackageName == appIcon.PackageName)).FirstOrDefault<AppIconUI>());
      this.mDockPanel.Children.Remove((UIElement) this.mDockPanel.Children.Cast<AppIconUI>().Where<AppIconUI>((Func<AppIconUI, bool>) (s => s.mAppIconModel.PackageName == appIcon.PackageName)).FirstOrDefault<AppIconUI>());
      this.mMoreAppsDockPanel.Children.Remove((UIElement) this.mMoreAppsDockPanel.Children.Cast<AppIconUI>().Where<AppIconUI>((Func<AppIconUI, bool>) (s => s.mAppIconModel.PackageName == appIcon.PackageName)).FirstOrDefault<AppIconUI>());
    }

    internal void InitUIAppPromotionEvents()
    {
      if (PromotionObject.Instance == null)
        return;
      PromotionObject.AppRecommendationHandler += new System.Action<bool>(this.ShowAppRecommendations);
    }

    private void HomeApp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource != this.InstalledAppsDrawerScrollBar)
        return;
      this.ParentWindow.StaticComponents.ShowUninstallButtons(false);
    }

    internal void InitMoreAppsIcon()
    {
      AppIconModel model = new AppIconModel();
      model.AppName = LocaleStrings.GetLocalizedString("STRING_MORE_APPS", "");
      model.ImageName = "moreapps";
      model.AddToDock(50.0, 50.0);
      this.moreAppsIcon = new AppIconUI(this.ParentWindow, model);
      this.moreAppsIcon.Click += new RoutedEventHandler(this.MoreAppsIcon_Click);
      this.mDockPanel.Children.Add((UIElement) this.moreAppsIcon);
    }

    private void MoreAppsIcon_Click(object sender, RoutedEventArgs e)
    {
      AppIconUI appIconUi = sender as AppIconUI;
      this.mDockAppIconToolTipPopup.IsOpen = false;
      this.mMoreAppsDockPopup.PlacementTarget = (UIElement) appIconUi.mAppImage;
      this.mMoreAppsDockPopup.IsOpen = true;
    }

    private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mMoreAppsDockPopup.IsOpen = false;
      this.mMoreAppsDockPopup.StaysOpen = false;
    }

    private void InstalledAppsDrawerScrollBar_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      double verticalOffset = this.InstalledAppsDrawerScrollBar.VerticalOffset;
      if (this.InstalledAppsDrawerScrollBar.ComputedVerticalScrollBarVisibility != Visibility.Visible)
        this.InstalledAppsDrawerScrollBar.OpacityMask = (Brush) null;
      else if (verticalOffset > 1.0)
      {
        if (verticalOffset == this.InstalledAppsDrawerScrollBar.ScrollableHeight)
          this.InstalledAppsDrawerScrollBar.OpacityMask = (Brush) BluestacksUIColor.mBottomOpacityMask;
        else
          this.InstalledAppsDrawerScrollBar.OpacityMask = (Brush) BluestacksUIColor.mScrolledOpacityMask;
      }
      else
        this.InstalledAppsDrawerScrollBar.OpacityMask = (Brush) BluestacksUIColor.mTopOpacityMask;
    }

    private void mCloseAppSuggPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mSuggestedAppPopUp.IsOpen = false;
    }

    private AppIconUI GetNewAppIconUI(
      AppIconModel iconModel,
      DownloadInstallApk downloadInstallApk = null)
    {
      AppIconUI appIconUi = new AppIconUI(this.ParentWindow, iconModel);
      if (downloadInstallApk != null)
        appIconUi.InitAppDownloader(downloadInstallApk);
      return appIconUi;
    }

    internal bool SideHtmlBrowserInited { get; set; }

    internal BrowserControl SideHtmlBrowser { get; set; }

    internal void InitiateSideHtmlBrowser()
    {
      lock (HomeApp.syncRoot)
      {
        if (!FeatureManager.Instance.IsHtmlSideBar || this.SideHtmlBrowserInited || (!CefHelper.CefInited || RegistryManager.Instance.IsPremium))
          return;
        this.CreateSideHtmlBrowserControl();
      }
    }

    private void CreateSideHtmlBrowserControl()
    {
      this.SideHtmlBrowserInited = true;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        BrowserControl browserControl = new BrowserControl(BlueStacksUIUtils.GetHtmlSidePanelUrl())
        {
          Visibility = Visibility.Visible
        };
        CustomPictureBox customPictureBox = new CustomPictureBox()
        {
          HorizontalAlignment = HorizontalAlignment.Center,
          VerticalAlignment = VerticalAlignment.Center,
          Height = 30.0,
          Width = 30.0,
          ImageName = "loader",
          IsImageToBeRotated = true
        };
        this.mAppRecommendationsGrid.Children.Add((UIElement) browserControl);
        this.mAppRecommendationsGrid.Children.Add((UIElement) customPictureBox);
        browserControl.CreateNewBrowser();
        this.SideHtmlBrowser = browserControl;
      }));
    }

    private void ChangeSideRecommendationsVisibility(
      bool isAppRecommendationsVisible,
      bool isSearchBarVisible)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isAppRecommendationsVisible)
        {
          this.mAppRecommendationsGrid.Visibility = Visibility.Visible;
          this.InstalledAppsDrawerScrollBar.SetValue(Grid.ColumnSpanProperty, (object) 1);
          this.mMultiInstanceControlsGrid.SetValue(Grid.ColumnSpanProperty, (object) 1);
          if (isSearchBarVisible)
          {
            this.mDiscoverApps.Visibility = Visibility.Visible;
            this.appRecomScrollViewer.Visibility = Visibility.Visible;
            this.mSearchGrid.Visibility = Visibility.Visible;
            this.mSearchGrid.Margin = new Thickness(20.0, 54.0, 20.0, 0.0);
            this.mSearchGrid.Width = 240.0;
            this.mIsShowSearchRecommendations = false;
            this.mSearchRecommendationBorder.Margin = new Thickness(20.0, 88.0, 20.0, 0.0);
            this.mSearchRecommendationBorder.Width = 240.0;
          }
          else
          {
            this.mSearchGrid.Visibility = Visibility.Collapsed;
            this.mDiscoverApps.Visibility = Visibility.Collapsed;
            this.appRecomScrollViewer.Visibility = Visibility.Collapsed;
            this.mAppRecommendationsGrid.Width = 345.0;
          }
        }
        else
        {
          if (isAppRecommendationsVisible)
            return;
          this.mAppRecommendationsGrid.Visibility = Visibility.Collapsed;
          this.InstalledAppsDrawerScrollBar.SetValue(Grid.ColumnSpanProperty, (object) 2);
          this.mMultiInstanceControlsGrid.SetValue(Grid.ColumnSpanProperty, (object) 2);
          if (isSearchBarVisible)
          {
            this.mSearchGrid.Visibility = Visibility.Visible;
            this.mSearchGrid.Margin = new Thickness(0.0, 20.0, 20.0, 0.0);
            this.mSearchGrid.Width = 350.0;
            this.mIsShowSearchRecommendations = true;
            this.mSearchRecommendationBorder.Margin = new Thickness(0.0, 59.0, 20.0, 0.0);
            this.mSearchRecommendationBorder.Width = 350.0;
          }
          else
            this.mSearchGrid.Visibility = Visibility.Collapsed;
        }
      }));
    }

    private void ShowAppRecommendations(bool isContentReloadRequired)
    {
      try
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (this.ParentWindow != null && this.ParentWindow.ActualWidth <= 700.0 || (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsSearchBarVisible) || RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
          {
            if (this.mCurrentSidePanelVisibility.HasValue)
            {
              SidePanelVisibility? sidePanelVisibility1 = this.mCurrentSidePanelVisibility;
              SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.BothSearchBarAndSidepanelHidden;
              if (sidePanelVisibility1.GetValueOrDefault() == sidePanelVisibility2 & sidePanelVisibility1.HasValue)
                return;
            }
            this.ChangeSideRecommendationsVisibility(false, false);
            this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.BothSearchBarAndSidepanelHidden);
          }
          else
          {
            if (!RegistryManager.Instance.IsPremium)
            {
              if (!FeatureManager.Instance.IsHtmlSideBar)
              {
                if (FeatureManager.Instance.IsShowAppRecommendations)
                {
                  AppRecommendationSection appRecommendations = PromotionObject.Instance.AppRecommendations;
                  if ((appRecommendations != null ? (appRecommendations.AppSuggestions.Count == 0 ? 1 : 0) : 0) != 0)
                    goto label_8;
                }
                else
                  goto label_8;
              }
              if (FeatureManager.Instance.IsHtmlSideBar)
              {
                if (!this.SideHtmlBrowserInited)
                  this.InitiateSideHtmlBrowser();
                if (RegistryManager.Instance.IsPremium)
                  return;
                if (this.mCurrentSidePanelVisibility.HasValue)
                {
                  SidePanelVisibility? sidePanelVisibility1 = this.mCurrentSidePanelVisibility;
                  SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.OnlySidepanelVisible;
                  if (sidePanelVisibility1.GetValueOrDefault() == sidePanelVisibility2 & sidePanelVisibility1.HasValue)
                    return;
                }
                this.ChangeSideRecommendationsVisibility(true, false);
                this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.OnlySidepanelVisible);
                return;
              }
              if (isContentReloadRequired || !this.mIsSidePanelContentLoadedOnce)
              {
                this.mAppRecommendationSectionsPanel.Children.Clear();
                AppRecommendationSection appRecommendations = PromotionObject.Instance.AppRecommendations;
                RecommendedAppsSection recommendedAppsSection = new RecommendedAppsSection(appRecommendations.AppSuggestionHeader);
                recommendedAppsSection.AddSuggestedApps(this.ParentWindow, appRecommendations.AppSuggestions, appRecommendations.ClientShowCount);
                if (recommendedAppsSection.mAppRecommendationsPanel.Children.Count != 0)
                {
                  this.mAppRecommendationSectionsPanel.Children.Add((UIElement) recommendedAppsSection);
                  this.mAppRecommendationSectionsPanel.Visibility = Visibility.Visible;
                  this.mAppRecommendationsGenericMessages.Visibility = Visibility.Collapsed;
                  this.SendAppRecommendationsImpressionStats();
                }
                this.mIsSidePanelContentLoadedOnce = true;
              }
              if (this.mCurrentSidePanelVisibility.HasValue)
              {
                SidePanelVisibility? sidePanelVisibility1 = this.mCurrentSidePanelVisibility;
                SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.BothSearchBarAndSidepanelVisible;
                if (sidePanelVisibility1.GetValueOrDefault() == sidePanelVisibility2 & sidePanelVisibility1.HasValue)
                  return;
              }
              this.ChangeSideRecommendationsVisibility(true, true);
              this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.BothSearchBarAndSidepanelVisible);
              return;
            }
label_8:
            if (this.mCurrentSidePanelVisibility.HasValue)
            {
              SidePanelVisibility? sidePanelVisibility1 = this.mCurrentSidePanelVisibility;
              SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.OnlySearchBarVisible;
              if (sidePanelVisibility1.GetValueOrDefault() == sidePanelVisibility2 & sidePanelVisibility1.HasValue)
                return;
            }
            this.ChangeSideRecommendationsVisibility(false, true);
            this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.OnlySearchBarVisible);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in showing app recommendations, " + ex.ToString());
      }
    }

    internal void UpdateRecommendedAppsInstallStatus(string package)
    {
      if (this.mAppRecommendationSectionsPanel.Children.Count <= 0)
        return;
      int index1 = -1;
      RecommendedAppsSection child1 = this.mAppRecommendationSectionsPanel.Children[0] as RecommendedAppsSection;
      RecommendedApps recommendedApps1 = (RecommendedApps) null;
      for (int index2 = 0; index2 < child1.mAppRecommendationsPanel.Children.Count; ++index2)
      {
        RecommendedApps child2 = child1.mAppRecommendationsPanel.Children[index2] as RecommendedApps;
        if (child2.AppRecomendation.AppPackage.Equals(package, StringComparison.InvariantCultureIgnoreCase))
        {
          index1 = index2;
          recommendedApps1 = child2;
          break;
        }
      }
      if (index1 == -1)
        return;
      child1.mAppRecommendationsPanel.Children.RemoveAt(index1);
      if (this.sAppRecommendationsPool.Count > 0)
      {
        int count = 1;
        for (int index2 = 0; index2 < this.sAppRecommendationsPool.Count; ++index2)
        {
          RecommendedApps recommendedApps2 = this.sAppRecommendationsPool[index2];
          if (!this.ParentWindow.mAppHandler.IsAppInstalled(recommendedApps2.AppRecomendation.ExtraPayload["click_action_packagename"]))
          {
            recommendedApps2.RecommendedAppPosition = recommendedApps1.RecommendedAppPosition;
            child1.mAppRecommendationsPanel.Children.Insert(index1, (UIElement) recommendedApps2);
            JArray jarray = new JArray();
            JObject jobject1 = new JObject();
            jobject1.Add("app_loc", (JToken) (recommendedApps2.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN" ? "cdn" : "gplay"));
            jobject1.Add("app_pkg", (JToken) recommendedApps2.AppRecomendation.ExtraPayload["click_action_packagename"]);
            jobject1.Add("is_installed", (JToken) (this.ParentWindow.mAppHandler.IsAppInstalled(recommendedApps2.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false"));
            int num = recommendedApps2.RecommendedAppPosition;
            jobject1.Add("app_position", (JToken) num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            num = recommendedApps2.RecommendedAppRank;
            jobject1.Add("app_rank", (JToken) num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            JObject jobject2 = jobject1;
            jarray.Add((JToken) jobject2);
            ClientStats.SendFrontendClickStats("apps_recommendation", "impression", (string) null, (string) null, (string) null, (string) null, (string) null, jarray.ToString(Formatting.None));
            break;
          }
          ++count;
        }
        this.sAppRecommendationsPool.RemoveRange(0, count);
      }
      if (child1.mAppRecommendationsPanel.Children.Count == 0)
        this.mAppRecommendationSectionsPanel.Children.Remove((UIElement) child1);
      if (this.mAppRecommendationSectionsPanel.Children.Count != 0)
        return;
      this.mAppRecommendationSectionsPanel.Visibility = Visibility.Collapsed;
      this.mAppRecommendationsGenericMessages.Visibility = Visibility.Visible;
    }

    private void SendAppRecommendationsImpressionStats()
    {
      JArray jarray = new JArray();
      RecommendedAppsSection child1 = this.mAppRecommendationSectionsPanel.Children[0] as RecommendedAppsSection;
      for (int index = 0; index < child1.mAppRecommendationsPanel.Children.Count; ++index)
      {
        RecommendedApps child2 = child1.mAppRecommendationsPanel.Children[index] as RecommendedApps;
        JObject jobject = new JObject()
        {
          {
            "app_loc",
            (JToken) (child2.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN" ? "cdn" : "gplay")
          },
          {
            "app_pkg",
            (JToken) child2.AppRecomendation.ExtraPayload["click_action_packagename"]
          },
          {
            "is_installed",
            (JToken) (this.ParentWindow.mAppHandler.IsAppInstalled(child2.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false")
          },
          {
            "app_position",
            (JToken) child2.RecommendedAppPosition.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "app_rank",
            (JToken) child2.RecommendedAppRank.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        jarray.Add((JToken) jobject);
      }
      ClientStats.SendFrontendClickStats("apps_recommendation", "impression", (string) null, (string) null, (string) null, (string) null, (string) null, jarray.ToString(Formatting.None));
    }

    private void Search_MouseEnter(object sender, MouseEventArgs e)
    {
      this.searchHoverTimer.Start();
    }

    private void search_MouseLeave(object sender, MouseEventArgs e)
    {
      this.searchHoverTimer.Stop();
      if (this.mSearchTextBox.IsFocused || this.mSearchRecommendationBorder.IsMouseOver || this.mSearchGrid.IsMouseOver)
        return;
      this.HideSearchSuggestions();
    }

    private void OpenSearchSuggestions()
    {
      try
      {
        if (this.mSearchRecommendationBorder.Visibility == Visibility.Visible || !string.IsNullOrEmpty(this.mSearchTextBox.Text) && !(this.mSearchTextBox.Text == this.defaultSearchBoxText) || (PromotionObject.Instance.SearchRecommendations.Count <= 0 || !this.mIsShowSearchRecommendations))
          return;
        this.searchRecomItems.Children.Clear();
        Separator separator1 = new Separator();
        separator1.Margin = new Thickness(0.0);
        separator1.Style = this.FindResource((object) ToolBar.SeparatorStyleKey) as Style;
        Separator separator2 = separator1;
        BlueStacksUIBinding.BindColor((DependencyObject) separator2, Control.BackgroundProperty, "VerticalSeparator");
        this.searchRecomItems.Children.Add((UIElement) separator2);
        Label label1 = new Label();
        label1.Content = (object) LocaleStrings.GetLocalizedString("STRING_MOST_SEARCHED_APPS", "");
        Label label2 = label1;
        BlueStacksUIBinding.BindColor((DependencyObject) label2, Control.ForegroundProperty, "SearchGridForegroundColor");
        label2.FontSize = 14.0;
        label2.Padding = new Thickness(10.0, 5.0, 5.0, 5.0);
        this.searchRecomItems.Children.Add((UIElement) label2);
        foreach (KeyValuePair<string, SearchRecommendation> searchRecommendation in (Dictionary<string, SearchRecommendation>) PromotionObject.Instance.SearchRecommendations)
        {
          RecommendedAppItem recommendedAppItem = new RecommendedAppItem();
          recommendedAppItem.Populate(this.ParentWindow, searchRecommendation.Value);
          recommendedAppItem.Padding = new Thickness(5.0, 0.0, 0.0, 0.0);
          this.searchRecomItems.Children.Add((UIElement) recommendedAppItem);
        }
        Border recommendationBorder = this.mSearchRecommendationBorder;
        CornerRadius cornerRadius1 = this.searchTextBoxBorder.CornerRadius;
        double bottomRight = cornerRadius1.BottomRight;
        cornerRadius1 = this.searchTextBoxBorder.CornerRadius;
        double bottomLeft = cornerRadius1.BottomLeft;
        CornerRadius cornerRadius2 = new CornerRadius(0.0, 0.0, bottomRight, bottomLeft);
        recommendationBorder.CornerRadius = cornerRadius2;
        Border mask = this.Mask;
        cornerRadius1 = this.searchTextBoxBorder.CornerRadius;
        CornerRadius cornerRadius3 = new CornerRadius(0.0, cornerRadius1.TopRight, 0.0, 0.0);
        mask.CornerRadius = cornerRadius3;
        Border searchTextBoxBorder = this.searchTextBoxBorder;
        cornerRadius1 = this.searchTextBoxBorder.CornerRadius;
        double topLeft = cornerRadius1.TopLeft;
        cornerRadius1 = this.searchTextBoxBorder.CornerRadius;
        double topRight = cornerRadius1.TopRight;
        CornerRadius cornerRadius4 = new CornerRadius(topLeft, topRight, 0.0, 0.0);
        searchTextBoxBorder.CornerRadius = cornerRadius4;
        this.mSearchRecommendationBorder.Visibility = Visibility.Visible;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when trying to open search recommendations. " + ex.ToString());
      }
    }

    private void HideSearchSuggestions()
    {
      if (this.mSearchRecommendationBorder.Visibility != Visibility.Visible)
        return;
      this.searchTextBoxBorder.CornerRadius = new CornerRadius(this.searchTextBoxBorder.CornerRadius.TopLeft, this.searchTextBoxBorder.CornerRadius.TopRight, this.mSearchRecommendationBorder.CornerRadius.BottomRight, this.mSearchRecommendationBorder.CornerRadius.BottomLeft);
      this.Mask.CornerRadius = new CornerRadius(0.0, this.searchTextBoxBorder.CornerRadius.TopRight, this.mSearchRecommendationBorder.CornerRadius.BottomRight, 0.0);
      this.mSearchRecommendationBorder.Visibility = Visibility.Collapsed;
    }

    private void SearchTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (this.ParentWindow.mWelcomeTab.IsPromotionVisible)
        return;
      if (this.mSearchTextBox.Text == this.defaultSearchBoxText)
        this.mSearchTextBox.Text = string.Empty;
      this.OpenSearchSuggestions();
      BlueStacksUIBinding.BindColor((DependencyObject) this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
    }

    private void SearchTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (!this.mSearchRecommendationBorder.IsMouseOver)
        this.HideSearchSuggestions();
      if (this.ParentWindow.mWelcomeTab.IsPromotionVisible)
        return;
      if (string.IsNullOrEmpty(this.mSearchTextBox.Text))
        this.mSearchTextBox.Text = this.defaultSearchBoxText;
      if (string.Equals(this.mSearchTextBox.Text, this.defaultSearchBoxText, StringComparison.InvariantCulture))
        BlueStacksUIBinding.BindColor((DependencyObject) this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundColor");
      else
        BlueStacksUIBinding.BindColor((DependencyObject) this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
    }

    private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.SearchApp();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (this.mSearchRecommendationBorder.Visibility == Visibility.Visible)
        this.HideSearchSuggestions();
      if (e.Key != Key.Return)
        return;
      this.SearchApp();
    }

    private void SearchApp()
    {
      if (this.ParentWindow.mWelcomeTab.IsPromotionVisible || string.IsNullOrEmpty(this.mSearchTextBox.Text))
        return;
      this.ParentWindow.mCommonHandler.SearchAppCenter(this.mSearchTextBox.Text);
    }

    private void GetSearchTextFromCloud()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          this.defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
          string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/app_center_searchdefaultquery", (string) null, (string) null, (string) null);
          Logger.Debug("url for search api :" + urlWithParams);
          string json = BstHttpClient.Get(urlWithParams, (Dictionary<string, string>) null, false, string.Empty, 0, 1, 0, false, "bgp");
          Logger.Debug("result for app_center_searchdefaultquery : " + json);
          JObject jobject = JObject.Parse(json);
          if ((bool) jobject["success"])
          {
            string str = jobject["result"].ToString().Trim();
            if (!string.IsNullOrEmpty(str))
              this.defaultSearchBoxText = str;
            Logger.Debug("response from search text cloud api :" + str);
          }
        }
        catch (Exception ex)
        {
          Logger.Warning("Failed to fetch text from cloud... Err : " + ex.ToString());
        }
        this.Dispatcher.Invoke((Delegate) (() => this.mSearchTextBox.Text = this.defaultSearchBoxText));
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void SetWallpaper()
    {
      if (!File.Exists(HomeAppManager.BackgroundImagePath))
        return;
      this.mBackgroundImage.IsFullImagePath = true;
      this.mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
    }

    private void HomeApp_BackgroundPromotionHandler(object sender, EventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (string.IsNullOrEmpty(PromotionObject.Instance.BackgroundPromotionImagePath))
        {
          this.SetWallpaper();
        }
        else
        {
          this.mBackgroundImage.IsFullImagePath = true;
          this.mBackgroundImage.ImageName = PromotionObject.Instance.BackgroundPromotionImagePath;
        }
      }));
    }

    internal void RestoreWallpaperImage()
    {
      this.mBackgroundImage.IsFullImagePath = false;
      this.mBackgroundImage.ImageName = "fancybg.jpg";
      try
      {
        if (!File.Exists(HomeAppManager.BackgroundImagePath))
          return;
        File.Delete(HomeAppManager.BackgroundImagePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in deletion of image:" + ex.ToString());
      }
    }

    internal void ApplyWallpaperImage()
    {
      this.mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
      this.mBackgroundImage.ReloadImages();
    }

    private void mAppSettingsPopup_Opened(object sender, EventArgs e)
    {
      this.mAppSettings.IsEnabled = false;
    }

    private void mAppSettingsPopup_Closed(object sender, EventArgs e)
    {
      this.mAppSettings.IsEnabled = true;
    }

    private void mInstallApkGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      new DownloadInstallApk(this.ParentWindow).ChooseAndInstallApk();
    }

    private void mDeleteAppsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.StaticComponents.ShowUninstallButtons(true);
      this.mAppSettingsPopup.IsOpen = false;
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void mAppSettings_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mAppSettingsPopup.IsOpen = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/homeapp.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).PreviewMouseDown += new MouseButtonEventHandler(this.HomeApp_PreviewMouseDown);
          break;
        case 2:
          this.mBackgroundImage = (CustomPictureBox) target;
          break;
        case 3:
          this.mInstalledAppText = (Label) target;
          break;
        case 4:
          this.mGridSeparator = (Grid) target;
          break;
        case 5:
          this.mAppSettings = (CustomPictureBox) target;
          this.mAppSettings.MouseEnter += new MouseEventHandler(this.mAppSettings_MouseEnter);
          break;
        case 6:
          this.mAppSettingsPopup = (CustomPopUp) target;
          break;
        case 7:
          this.dummyGrid = (Grid) target;
          break;
        case 8:
          this.mAppSettingsPopupBorder = (Border) target;
          break;
        case 9:
          this.mMaskBorder1 = (Border) target;
          break;
        case 10:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.mInstallApkGrid_MouseLeftButtonUp);
          break;
        case 11:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.mDeleteAppsGrid_MouseLeftButtonUp);
          break;
        case 12:
          this.LeftArrow = (System.Windows.Shapes.Path) target;
          break;
        case 13:
          this.InstalledAppsDrawerScrollBar = (ScrollViewer) target;
          this.InstalledAppsDrawerScrollBar.ScrollChanged += new ScrollChangedEventHandler(this.InstalledAppsDrawerScrollBar_ScrollChanged);
          break;
        case 14:
          this.mAppRecommendationsGrid = (Grid) target;
          break;
        case 15:
          this.mDiscoverApps = (TextBlock) target;
          break;
        case 16:
          this.appRecomScrollViewer = (ScrollViewer) target;
          break;
        case 17:
          this.mAppRecommendationSectionsPanel = (StackPanel) target;
          break;
        case 18:
          this.mAppRecommendationsGenericMessages = (StackPanel) target;
          break;
        case 19:
          this.mAppRecommendationsGenericMessageImage = (CustomPictureBox) target;
          break;
        case 20:
          this.mAppRecommendationsGenericMessageText = (TextBlock) target;
          break;
        case 21:
          this.mSearchGrid = (Border) target;
          this.mSearchGrid.MouseEnter += new MouseEventHandler(this.Search_MouseEnter);
          this.mSearchGrid.MouseLeave += new MouseEventHandler(this.search_MouseLeave);
          break;
        case 22:
          this.searchTextBoxBorder = (Border) target;
          break;
        case 23:
          this.mSearchTextBox = (TextBox) target;
          this.mSearchTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.SearchTextBox_GotKeyboardFocus);
          this.mSearchTextBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.SearchTextBox_LostKeyboardFocus);
          this.mSearchTextBox.KeyDown += new KeyEventHandler(this.SearchTextBox_KeyDown);
          break;
        case 24:
          this.Mask = (Border) target;
          break;
        case 25:
          ((UIElement) target).MouseUp += new MouseButtonEventHandler(this.CustomPictureBox_MouseUp);
          break;
        case 26:
          this.mSearchRecommendationBorder = (Border) target;
          this.mSearchRecommendationBorder.MouseLeave += new MouseEventHandler(this.search_MouseLeave);
          break;
        case 27:
          this.searchRecomItems = (StackPanel) target;
          break;
        case 28:
          this.mMultiInstanceControlsGrid = (Grid) target;
          break;
        case 29:
          this.mDockGridBorder = (Border) target;
          break;
        case 30:
          this.mDockGrid = (Grid) target;
          break;
        case 31:
          this.mDockPanel = (StackPanel) target;
          break;
        case 32:
          this.mDockAppIconToolTipPopup = (CustomPopUp) target;
          break;
        case 33:
          this.mMaskBorder2 = (Border) target;
          break;
        case 34:
          this.mDockIconText = (TextBlock) target;
          break;
        case 35:
          this.mDownArrow = (System.Windows.Shapes.Path) target;
          break;
        case 36:
          this.mMoreAppsDockPopup = (CustomPopUp) target;
          break;
        case 37:
          this.mMaskBorder3 = (Border) target;
          break;
        case 38:
          this.mMoreAppsDockIconText = (TextBlock) target;
          break;
        case 39:
          this.mCustomMessageBoxCloseButton = (CustomPictureBox) target;
          this.mCustomMessageBoxCloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Close_PreviewMouseLeftButtonUp);
          break;
        case 40:
          this.mMoreAppsDockPanel = (StackPanel) target;
          break;
        case 41:
          this.mSuggestedAppPopUp = (CustomPopUp) target;
          break;
        case 42:
          this.mMaskBorder4 = (Border) target;
          break;
        case 43:
          this.mCloseAppSuggPopup = (CustomPictureBox) target;
          this.mCloseAppSuggPopup.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseAppSuggPopup_MouseLeftButtonUp);
          break;
        case 44:
          this.mAppSuggestionPopUp = (TextBlock) target;
          break;
        case 45:
          this.UpArrow = (System.Windows.Shapes.Path) target;
          break;
        case 46:
          this.mLoadingGrid = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
