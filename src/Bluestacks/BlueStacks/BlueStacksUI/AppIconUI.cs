// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppIconUI
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.BlueStacksUI
{
  public class AppIconUI : Button, IComponentConnector
  {
    private Thread threadShowingUninstallButton;
    private ImageAnimationController mGifController;
    private MainWindow ParentWindow;
    private DownloadInstallApk mDownloader;
    internal Grid mMainGrid;
    internal Grid mImageGrid;
    internal Border mAppImageBorder;
    internal CustomPictureBox mAppImage;
    internal Grid mProgressGrid;
    internal BlueProgressBar CustomProgressBar;
    internal Grid mBusyGrid;
    internal CustomPictureBox mBusyImage;
    internal Grid mErrorGrid;
    internal Grid mRetryGrid;
    internal CustomPictureBox mSuggestedAppPromotionImage;
    internal CustomPictureBox mUnInstallTabButton;
    internal CustomPictureBox mGl3ErrorIcon;
    internal CustomPictureBox mGl3InfoIcon;
    internal CustomPictureBox mRedDotNotifIcon;
    internal TextBlock AppNameTextBox;
    internal Grid mGamePadGrid;
    internal CustomPictureBox mGamepadIcon;
    internal CustomPopUp mGamePadToolTipPopup;
    internal Border mMaskBorder;
    internal TextBlock mIconText;
    internal System.Windows.Shapes.Path mUpArrow;
    private bool _contentLoaded;

    internal AppIconModel mAppIconModel { get; private set; }

    public AppIconUI(MainWindow window, AppIconModel model)
    {
      this.ParentWindow = window;
      this.mAppIconModel = model;
      this.DataContext = (object) this.mAppIconModel;
      this.InitializeComponent();
      if (this.mAppIconModel == null)
        return;
      this.SetAppIconLocation(this.mAppIconModel.IconLocation, this.mAppIconModel.IconHeight, this.mAppIconModel.IconWidth);
    }

    internal void InitAppDownloader(DownloadInstallApk downloadInstallApk = null)
    {
      this.mDownloader = downloadInstallApk;
      this.mAppIconModel.mAppDownloadingEvent += new System.Action<AppIconDownloadingPhases>(this.DownloadingApp);
    }

    private void Button_Loaded(object sender, RoutedEventArgs e)
    {
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      this.Loaded -= new RoutedEventHandler(this.Button_Loaded);
      this.ParentWindow.StaticComponents.ShowAllUninstallButtons += new EventHandler(this.Button_MouseHoldAction);
      this.ParentWindow.StaticComponents.HideAllUninstallButtons += new EventHandler(this.AppIcon_HideUninstallButton);
      if (this.mAppIconModel.IsGifIcon)
      {
        this.ParentWindow.StaticComponents.PlayAllGifs += new System.Action(this.GifAppIconPlay);
        this.ParentWindow.StaticComponents.PauseAllGifs += new System.Action(this.GifAppIconPause);
      }
      this.mAppIconModel.IsGamepadConnected = this.ParentWindow.IsGamepadConnected;
    }

    private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.threadShowingUninstallButton != null)
        return;
      this.threadShowingUninstallButton = new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(1000);
        if (this.threadShowingUninstallButton == null)
          return;
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          (sender as UIElement).ReleaseMouseCapture();
          this.ParentWindow.StaticComponents.ShowUninstallButtons(true);
          this.threadShowingUninstallButton = (Thread) null;
        }));
      }))
      {
        IsBackground = true
      };
      this.threadShowingUninstallButton.Start();
    }

    private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.threadShowingUninstallButton == null || !this.threadShowingUninstallButton.IsAlive)
        return;
      this.threadShowingUninstallButton = (Thread) null;
    }

    private void Button_MouseHoldAction(object sender, EventArgs e)
    {
      this.ShowAppUninstallButton(true);
    }

    private void AppIcon_HideUninstallButton(object sender, EventArgs e)
    {
      this.ShowAppUninstallButton(false);
    }

    private void ShowAppUninstallButton(bool isShow)
    {
      if (isShow && this.mAppIconModel.mIsAppRemovable && (!this.mAppIconModel.IsInstalling || this.mAppIconModel.IsInstallingFailed))
        this.mUnInstallTabButton.Visibility = Visibility.Visible;
      else
        this.mUnInstallTabButton.Visibility = Visibility.Hidden;
    }

    private void GamepadIcon_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mIconText.Text = this.mAppIconModel.IsGamepadConnected ? LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", "") : LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", "");
      this.mAppIconModel.AppNameTooltip = (string) null;
      this.mGamePadToolTipPopup.PlacementTarget = (UIElement) this.mGamepadIcon;
      this.mGamePadToolTipPopup.IsOpen = true;
      this.mGamePadToolTipPopup.StaysOpen = true;
    }

    private void GamepadIcon_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mGamePadToolTipPopup.IsOpen = false;
      this.mAppIconModel.AppNameTooltip = this.mAppIconModel.AppName;
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.ParentWindow.StaticComponents.IsDeleteButtonVisible)
        this.ShowAppUninstallButton(false);
      this.mRetryGrid.Visibility = Visibility.Hidden;
      ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0);
      this.mAppImage.RenderTransformOrigin = new Point(0.0, 0.0);
      this.mAppImage.RenderTransform = (Transform) scaleTransform;
      this.AppNameTextBox.RenderTransformOrigin = new Point(0.0, 0.0);
      this.AppNameTextBox.RenderTransform = (Transform) scaleTransform;
      this.mAppImageBorder.Effect = (Effect) new DropShadowEffect()
      {
        Color = Colors.Black,
        Direction = 270.0,
        ShadowDepth = 1.0,
        BlurRadius = 6.0,
        Opacity = 0.3
      };
      if (this.mAppIconModel.IsAppSuggestionActive)
        this.ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
      if (this.mAppIconModel.IconLocation != AppIconLocation.Dock && this.mAppIconModel.IconLocation != AppIconLocation.Moreapps)
        return;
      this.ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, false);
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mAppIconModel.IsDownloading)
        this.ShowAppUninstallButton(true);
      if (this.mAppIconModel.IsDownLoadingFailed || this.mAppIconModel.IsInstallingFailed)
        this.mRetryGrid.Visibility = Visibility.Visible;
      if (this.mBusyGrid.Visibility == Visibility.Visible)
        return;
      DropShadowEffect dropShadowEffect = new DropShadowEffect();
      BlueStacksUIBinding.BindColor((DependencyObject) dropShadowEffect, DropShadowEffect.ColorProperty, "AppIconDropShadowBrush");
      dropShadowEffect.Direction = 270.0;
      dropShadowEffect.ShadowDepth = 1.0;
      dropShadowEffect.BlurRadius = 20.0;
      dropShadowEffect.Opacity = 1.0;
      this.mAppImageBorder.Effect = (Effect) dropShadowEffect;
      ScaleTransform scaleTransform = new ScaleTransform(1.02, 1.02);
      this.mAppImage.RenderTransformOrigin = new Point(0.5, 0.5);
      this.mAppImage.RenderTransform = (Transform) scaleTransform;
      this.AppNameTextBox.RenderTransformOrigin = new Point(0.5, 0.5);
      this.AppNameTextBox.RenderTransform = (Transform) scaleTransform;
      if (this.mAppIconModel.IconLocation == AppIconLocation.Dock || this.mAppIconModel.IconLocation == AppIconLocation.Moreapps)
      {
        this.ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, true);
        dropShadowEffect.BlurRadius = 12.0;
        this.mAppImageBorder.Effect = (Effect) dropShadowEffect;
      }
      if (!this.mAppIconModel.IsAppSuggestionActive)
        return;
      this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenAppSuggestionPopup(this.mAppIconModel.AppSuggestionInfo, (UIElement) this.AppNameTextBox, true);
    }

    private void UninstallButtonClicked()
    {
      try
      {
        if (this.mAppIconModel.IsDownloading)
        {
          if (this.mDownloader == null)
            return;
          this.mDownloader.AbortApkDownload(this.mAppIconModel.PackageName);
          this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, (AppIconModel) null);
        }
        else if (this.mAppIconModel.IsInstalling)
        {
          if (!this.mAppIconModel.IsInstallingFailed)
            return;
          this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, (AppIconModel) null);
        }
        else if (this.mAppIconModel.IsAppSuggestionActive)
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_REMOVE_ICON", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ICON_REMOVE", "");
          customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_REMOVE", ""), new EventHandler(this.RemoveAppSuggestion), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_KEEP", ""), (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.Owner = (Window) this.ParentWindow;
          this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.ShowDialog();
          this.ParentWindow.HideDimOverlay();
        }
        else
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0", ""), (object) this.mAppIconModel.AppName);
          customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0_BS", ""), (object) this.mAppIconModel.AppName);
          customMessageWindow.AddButton(ButtonColors.Red, "STRING_UNINSTALL", (EventHandler) ((o, e) => this.AppIcon_UninstallConfirmationClicked(o, e)), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
          this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
          customMessageWindow.ShowDialog();
          this.ParentWindow.HideDimOverlay();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UninstallButtonClicked. Err : " + ex.ToString());
      }
    }

    private void RemoveAppSuggestion(object sender, EventArgs e)
    {
      this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, (AppIconModel) null);
      ClientStats.SendMiscellaneousStatsAsync("cross_promotion_icon_removed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, this.mAppIconModel.PackageName, "bgp", (string) null, (string) null, (string) null, "Android");
      try
      {
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true
        };
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[1]
        {
          new XmlQualifiedName("", "")
        });
        string str = System.IO.Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
        string data = "";
        if (File.Exists(str))
          data = File.ReadAllText(str);
        List<string> stringList = new List<string>();
        if (!string.IsNullOrEmpty(data))
          stringList = AppIconUI.DoDeserialize<List<string>>(data);
        if (!stringList.Contains(this.mAppIconModel.PackageName))
        {
          if (stringList.Count >= 20)
            stringList.RemoveAt(0);
          stringList.Add(this.mAppIconModel.PackageName);
        }
        using (XmlWriter xmlWriter = XmlWriter.Create(str, settings))
          new XmlSerializer(typeof (List<string>)).Serialize(xmlWriter, (object) stringList, namespaces);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in writing removed suggested app icon package name in file " + ex.ToString());
      }
    }

    private static T DoDeserialize<T>(string data) where T : class
    {
      using (XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(data))))
        return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
    }

    private void AppIcon_UninstallConfirmationClicked(object _1, EventArgs _2)
    {
      Logger.Info("Clicked app icon uninstall popup package name {0}", (object) this.mAppIconModel.PackageName);
      this.ParentWindow.mAppInstaller.UninstallApp(this.mAppIconModel.PackageName);
      this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, (AppIconModel) null);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("Clicked app icon, package name {0}", (object) this.mAppIconModel.PackageName);
      if (this.mUnInstallTabButton.IsMouseOver)
        this.UninstallButtonClicked();
      else if (this.mErrorGrid.IsVisible)
      {
        this.mErrorGrid.Visibility = Visibility.Hidden;
        if (this.mAppIconModel.IsDownLoadingFailed)
        {
          this.mDownloader?.DownloadApk(this.mAppIconModel.ApkUrl, this.mAppIconModel.PackageName, false, false, "");
        }
        else
        {
          if (!this.mAppIconModel.IsInstallingFailed)
            return;
          this.mDownloader?.InstallApk(this.mAppIconModel.PackageName, this.mAppIconModel.ApkFilePath, false, false, "");
        }
      }
      else
      {
        if (this.mAppIconModel.IsDownloading)
          return;
        if (this.mAppIconModel.IsInstalling)
        {
          if (this.mDownloader != null)
            return;
          this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(this.mAppIconModel.PackageName, this.mAppIconModel.AppName, PlayStoreAction.OpenApp, false);
        }
        else if (this.mAppIconModel.IsRerollIcon)
          this.HandleRerollClick();
        else if (this.mAppIconModel.IsAppSuggestionActive)
        {
          this.HandleAppSuggestionClick();
          if (!this.mAppIconModel.IsRedDotVisible)
            return;
          this.mAppIconModel.IsRedDotVisible = false;
          HomeAppManager.AddPackageInRedDotShownRegistry(this.mAppIconModel.PackageName);
        }
        else
        {
          if (string.IsNullOrEmpty(this.mAppIconModel.PackageName))
            return;
          if (string.Equals(this.mAppIconModel.PackageName, "help_center", StringComparison.InvariantCulture))
            this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(BlueStacksUIUtils.GetHelpCenterUrl(), "STRING_FEEDBACK", "help_center", true, "STRING_FEEDBACK", false);
          else if (string.Equals(this.mAppIconModel.PackageName, "instance_manager", StringComparison.InvariantCulture))
            BlueStacksUIUtils.LaunchMultiInstanceManager();
          else if (string.Equals(this.mAppIconModel.PackageName, "macro_recorder", StringComparison.InvariantCulture))
            this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
          else
            this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(this.mAppIconModel.PackageName, true);
        }
      }
    }

    private void HandleAppSuggestionClick()
    {
      this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.mAppIconModel.AppSuggestionInfo.ExtraPayload, "my_apps", this.mAppIconModel.ImageName);
      this.SendAppSuggestionIconClickStats();
    }

    private void SendAppSuggestionIconClickStats()
    {
      ClientStats.SendPromotionAppClickStatsAsync(new Dictionary<string, string>()
      {
        {
          "op",
          "init"
        },
        {
          "status",
          "success"
        },
        {
          "app_pkg",
          this.mAppIconModel.PackageName
        },
        {
          "extraPayload",
          JsonConvert.SerializeObject((object) this.mAppIconModel.AppSuggestionInfo.ExtraPayload)
        },
        {
          "app_name",
          this.mAppIconModel.AppName
        },
        {
          "app_promotion_id",
          this.mAppIconModel.mPromotionId
        },
        {
          "promotion_type",
          "cross_promotion"
        }
      }, "app_activity");
    }

    private void HandleRerollClick()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_REROLL_0", ""), (object) this.mAppIconModel.AppName);
      customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_START_REROLL", ""), (object) this.mAppIconModel.AppName);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_REROLL_APP_PREFIX", new EventHandler(this.StartRerollAccepted), (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      if (customMessageWindow.ClickedButton == ButtonColors.White)
        return;
      this.ParentWindow.HideDimOverlay();
    }

    private void StartRerollAccepted(object sender, EventArgs e)
    {
      this.ParentWindow.ShowRerollOverlay();
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startReroll", new Dictionary<string, string>()
      {
        {
          "packageName",
          this.mAppIconModel.PackageName
        },
        {
          "rerollName",
          ""
        }
      });
    }

    private void GifAppIconPlay()
    {
      try
      {
        this.mGifController = ImageBehavior.GetAnimationController((Image) this.mAppImage);
        if (this.mGifController != null)
        {
          this.mGifController.Play();
        }
        else
        {
          if (this.mAppIconModel.ImageName == null)
            return;
          ImageBehavior.SetAnimatedSource((Image) this.mAppImage, (ImageSource) new BitmapImage(new Uri(this.mAppIconModel.ImageName)));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in animating appicon for package " + this.mAppIconModel.PackageName + Environment.NewLine + ex.ToString());
      }
    }

    private void GifAppIconPause()
    {
      try
      {
        this.mGifController = ImageBehavior.GetAnimationController((Image) this.mAppImage);
        this.mGifController.Pause();
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to pause gif. Err : " + ex.Message);
      }
    }

    private void DownloadingApp(AppIconDownloadingPhases downloadPhase)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        switch (downloadPhase)
        {
          case AppIconDownloadingPhases.DownloadStarted:
            this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
            this.mErrorGrid.Visibility = Visibility.Hidden;
            this.mProgressGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.DownloadFailed:
            this.mErrorGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.Downloading:
            this.mProgressGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.DownloadCompleted:
            this.mProgressGrid.Visibility = Visibility.Hidden;
            this.mBusyGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.InstallStarted:
            this.ShowAppUninstallButton(false);
            this.mErrorGrid.Visibility = Visibility.Hidden;
            this.mBusyGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.InstallFailed:
            if (this.mAppIconModel.mIsAppInstalled)
              break;
            this.mBusyGrid.Visibility = Visibility.Hidden;
            this.mErrorGrid.Visibility = Visibility.Visible;
            break;
          case AppIconDownloadingPhases.InstallCompleted:
            this.mBusyGrid.Visibility = Visibility.Hidden;
            this.mDownloader = (DownloadInstallApk) null;
            this.mAppIconModel.mAppDownloadingEvent -= new System.Action<AppIconDownloadingPhases>(this.DownloadingApp);
            break;
        }
      }));
    }

    private void SetAppIconLocation(AppIconLocation iconLocation, double height, double width)
    {
      switch (iconLocation)
      {
        case AppIconLocation.Dock:
          if (width > 68.0)
            this.mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
          else
            this.mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
          if (height < 68.0)
            this.mMainGrid.RowDefinitions[1].Height = new GridLength(height);
          GridLength gridLength1 = new GridLength(0.0);
          this.mMainGrid.ColumnDefinitions[1].Width = gridLength1;
          this.mMainGrid.ColumnDefinitions[4].Width = gridLength1;
          this.Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
          this.mAppImage.Height = height;
          this.mAppImage.Width = width;
          this.mAppImage.Clip = (Geometry) new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
          this.AppNameTextBox.Visibility = Visibility.Collapsed;
          this.mAppIconModel.AppNameTooltip = (string) null;
          break;
        case AppIconLocation.Moreapps:
          if (width > 68.0)
            this.mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
          else
            this.mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
          if (height < 68.0)
            this.mMainGrid.RowDefinitions[1].Height = new GridLength(height);
          GridLength gridLength2 = new GridLength(0.0);
          this.mMainGrid.ColumnDefinitions[1].Width = gridLength2;
          this.mMainGrid.ColumnDefinitions[4].Width = gridLength2;
          this.mMainGrid.RowDefinitions[3].Height = gridLength2;
          this.mMainGrid.RowDefinitions[5].Height = gridLength2;
          this.Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
          this.mAppImage.Height = height;
          this.mAppImage.Width = width;
          this.mAppImage.Clip = (Geometry) new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
          this.AppNameTextBox.Visibility = Visibility.Collapsed;
          this.mAppIconModel.AppNameTooltip = (string) null;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/appiconui.xaml", UriKind.Relative));
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
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Button_Click);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Button_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Button_Loaded);
          ((UIElement) target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.Button_PreviewMouseLeftButtonDown);
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Button_PreviewMouseLeftButtonUp);
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        case 3:
          this.mImageGrid = (Grid) target;
          break;
        case 4:
          this.mAppImageBorder = (Border) target;
          break;
        case 5:
          this.mAppImage = (CustomPictureBox) target;
          break;
        case 6:
          this.mProgressGrid = (Grid) target;
          break;
        case 7:
          this.CustomProgressBar = (BlueProgressBar) target;
          break;
        case 8:
          this.mBusyGrid = (Grid) target;
          break;
        case 9:
          this.mBusyImage = (CustomPictureBox) target;
          break;
        case 10:
          this.mErrorGrid = (Grid) target;
          break;
        case 11:
          this.mRetryGrid = (Grid) target;
          break;
        case 12:
          this.mSuggestedAppPromotionImage = (CustomPictureBox) target;
          break;
        case 13:
          this.mUnInstallTabButton = (CustomPictureBox) target;
          break;
        case 14:
          this.mGl3ErrorIcon = (CustomPictureBox) target;
          break;
        case 15:
          this.mGl3InfoIcon = (CustomPictureBox) target;
          break;
        case 16:
          this.mRedDotNotifIcon = (CustomPictureBox) target;
          break;
        case 17:
          this.AppNameTextBox = (TextBlock) target;
          break;
        case 18:
          this.mGamePadGrid = (Grid) target;
          break;
        case 19:
          this.mGamepadIcon = (CustomPictureBox) target;
          this.mGamepadIcon.MouseEnter += new MouseEventHandler(this.GamepadIcon_MouseEnter);
          this.mGamepadIcon.MouseLeave += new MouseEventHandler(this.GamepadIcon_MouseLeave);
          break;
        case 20:
          this.mGamePadToolTipPopup = (CustomPopUp) target;
          break;
        case 21:
          this.mMaskBorder = (Border) target;
          break;
        case 22:
          this.mIconText = (TextBlock) target;
          break;
        case 23:
          this.mUpArrow = (System.Windows.Shapes.Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
