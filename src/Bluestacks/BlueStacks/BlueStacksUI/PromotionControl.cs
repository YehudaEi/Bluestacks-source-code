// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PromotionControl
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
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class PromotionControl : System.Windows.Controls.UserControl, IDisposable, IComponentConnector
  {
    private bool mRunPromotion = true;
    private double mProgress = 0.1;
    private System.Windows.Forms.Timer progressTimer = new System.Windows.Forms.Timer();
    private int mBootPromotionImageTimeout = 4000;
    private SerializableDictionary<string, BootPromotion> dictRunningPromotions = new SerializableDictionary<string, BootPromotion>();
    private System.Windows.Forms.Timer mBootStringTimer = new System.Windows.Forms.Timer();
    private List<string> mBootStrings = new List<string>()
    {
      "STRING_TIP_ZOOM",
      "STRING_TIP_SCREENSHOT",
      "STRING_TIP_APK_DRAG_DROP",
      "STRING_TIP_HELP",
      "STRING_TIP_ICONSIZE",
      "STRING_TIP_FPSCOUNTER",
      "STRING_TIP_GAMEPAD",
      "STRING_TIP_BACKUP",
      "STRING_TIP_FULLSCREEN",
      "STRING_TIP_PLAY_AND_WIN",
      "STRING_TIP_SHOOTINGMODE",
      "STRING_TIP_OVERLAY"
    };
    internal SerializableDictionary<string, string> mExtraPayloadClicked = new SerializableDictionary<string, string>();
    private bool isPerformActionOnClose;
    private bool mForceComplete;
    internal string mActionValue;
    internal string mTextOnActionBtn;
    internal bool mIsActionButtonToShow;
    internal PromotionControl PromoControl;
    internal BootPromotion currentBootPromotion;
    private Thread mSliderAnimationThread;
    private int mThreadId;
    internal static Dictionary<BootPromotion, int> sBootPromotionDisplayed;
    private int mBootStringIndex;
    private MainWindow mMainWindow;
    private bool disposedValue;
    internal Grid mPromotionImageGrid;
    internal CustomPictureBox mPromotionImage;
    internal CustomButton mPromoButton;
    internal Border mPromotionInfoBorder;
    internal TextBlock mPromoInfoText;
    internal CustomPictureBox mCloseButton;
    internal BlueProgressBar mProgressBar;
    internal TextBlock BootText;
    private bool _contentLoaded;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public PromotionControl()
    {
      this.InitializeComponent();
      this.PromoControl = this;
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this.PromoControl))
        return;
      if (!string.IsNullOrEmpty(RegistryManager.Instance.PromotionId) || FeatureManager.Instance.IsPromotionFixed)
      {
        this.mPromotionImage.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promotions/promotion.jpg");
        this.mPromotionImageGrid.Background = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0));
      }
      this.mBootStringIndex = new Random().Next(this.mBootStrings.Count);
      this.mBootStringTimer.Tick += new EventHandler(this.BootStringTimer_Tick);
      this.mBootStringTimer.Interval = 3000;
      BlueStacksUIBinding.Bind(this.BootText, this.mBootStrings[this.mBootStringIndex], "");
      int num = RegistryManager.Instance.AvgBootTime / 500;
      if (num <= 0)
      {
        RegistryManager.Instance.AvgBootTime = 20000;
        RegistryManager.Instance.NoOfBootCompleted = 0;
        num = 40;
      }
      this.progressTimer.Tick += new EventHandler(this.ProgressTimer_Tick);
      this.progressTimer.Interval = num;
      this.progressTimer.Start();
      if (PromotionObject.Instance == null)
        PromotionObject.LoadDataFromFile();
      PromotionObject.BootPromotionHandler += new EventHandler(this.PromotionControl_BootPromotionHandler);
    }

    private void PromotionControl_BootPromotionHandler(object sender, EventArgs e)
    {
      this.PromoControl.Dispatcher.Invoke((Delegate) (() =>
      {
        Fraction fraction = new Fraction((long) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth, (long) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight);
        if (!(App.defaultResolution == fraction) || this.dictRunningPromotions.Keys.All<string>(new Func<string, bool>(((Enumerable) PromotionObject.Instance.DictBootPromotions.Keys).Contains<string>)) && PromotionObject.Instance.DictBootPromotions.Count == this.dictRunningPromotions.Count)
          return;
        this.StopSlider();
        this.StartAnimation(PromotionObject.Instance.DictBootPromotions);
      }));
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.mBootStringTimer.Start();
      Fraction fraction = new Fraction((long) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth, (long) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight);
      if (App.defaultResolution != fraction)
        this.StartAnimation(new SerializableDictionary<string, BootPromotion>());
      else
        this.StartAnimation(PromotionObject.Instance.DictBootPromotions);
    }

    private void StartAnimation(SerializableDictionary<string, BootPromotion> dict)
    {
      this.PromoControl.Dispatcher.Invoke((Delegate) (() =>
      {
        this.dictRunningPromotions = dict.DeepCopy<SerializableDictionary<string, BootPromotion>>();
        if (dict.Count <= 0)
          return;
        List<KeyValuePair<string, BootPromotion>> myList = dict.ToList<KeyValuePair<string, BootPromotion>>();
        myList.Sort((Comparison<KeyValuePair<string, BootPromotion>>) ((pair1, pair2) => pair1.Value.Order.CompareTo(pair2.Value.Order)));
        this.mPromotionImageGrid.Background = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0));
        this.mRunPromotion = true;
        this.mSliderAnimationThread = new Thread((ThreadStart) (() =>
        {
          this.mThreadId = this.mSliderAnimationThread.ManagedThreadId;
          Dictionary<BootPromotion, int> bootPromos = new Dictionary<BootPromotion, int>();
          while (this.mRunPromotion)
          {
            if (this.mThreadId != Thread.CurrentThread.ManagedThreadId)
              break;
            try
            {
              foreach (KeyValuePair<string, BootPromotion> keyValuePair in myList)
              {
                KeyValuePair<string, BootPromotion> item = keyValuePair;
                if (this.mRunPromotion)
                {
                  if (this.mThreadId == Thread.CurrentThread.ManagedThreadId)
                  {
                    if (this.currentBootPromotion == null)
                      this.currentBootPromotion = item.Value;
                    this.PromoControl.Dispatcher.Invoke((Delegate) (() =>
                    {
                      this.HandleAnimation(item.Value);
                      this.currentBootPromotion = item.Value;
                      this.SetLoadingText(item.Value.ButtonText);
                      if (bootPromos.ContainsKey(item.Value))
                        bootPromos[item.Value] = bootPromos[item.Value] + 1;
                      else
                        bootPromos.Add(item.Value, 1);
                      PromotionControl.sBootPromotionDisplayed = bootPromos;
                    }));
                    this.mBootPromotionImageTimeout = PromotionObject.Instance.BootPromoDisplaytime;
                    Thread.Sleep(this.mBootPromotionImageTimeout);
                  }
                  else
                    break;
                }
                else
                  break;
              }
            }
            catch (Exception ex)
            {
              Logger.Error(ex.ToString());
            }
          }
        }))
        {
          IsBackground = true
        };
        this.mSliderAnimationThread.Start();
      }));
    }

    private void ProgressTimer_Tick(object sender, EventArgs e)
    {
      try
      {
        this.ParentWindow.mWelcomeTab.mHomeAppManager.InitiateHtmlSidePanel();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while creating HTML sidepanel .Exception: " + ex.ToString());
      }
      if (this.mProgress >= 99.0 && !this.mForceComplete)
      {
        this.mProgressBar.Value = this.mProgress;
        this.mProgress += 0.0;
      }
      else if (this.mProgress >= 95.0 && !this.mForceComplete)
      {
        this.progressTimer.Interval = this.progressTimer.Interval;
        this.mProgressBar.Value = this.mProgress;
        this.mProgress += 0.025;
      }
      else
      {
        this.mProgressBar.Value = this.mProgress;
        this.mProgress += 0.25;
      }
    }

    private void mPromoButton_Click(object sender, RoutedEventArgs e)
    {
      this.StopSlider();
      ClientStats.SendMiscellaneousStatsAsync("BootPromotion", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, JsonConvert.SerializeObject((object) this.currentBootPromotion.ExtraPayload), (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      GenericAction genericAction = GenericAction.None;
      if (this.currentBootPromotion.ExtraPayload.ContainsKey("click_generic_action"))
        genericAction = EnumHelper.Parse<GenericAction>(this.currentBootPromotion.ExtraPayload["click_generic_action"], GenericAction.None);
      switch (genericAction)
      {
        case GenericAction.ApplicationBrowser:
        case GenericAction.UserBrowser:
        case GenericAction.HomeAppTab:
        case GenericAction.SettingsMenu:
          this.isPerformActionOnClose = false;
          break;
        default:
          this.isPerformActionOnClose = true;
          break;
      }
      if (this.isPerformActionOnClose)
      {
        this.mPromotionInfoBorder.Visibility = Visibility.Visible;
        this.mPromoButton.Visibility = Visibility.Hidden;
        this.mPromoInfoText.Text = this.currentBootPromotion.PromoBtnClickStatusText.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (!string.Equals(this.currentBootPromotion.ThemeEnabled, "true", StringComparison.InvariantCultureIgnoreCase))
          return;
        this.ParentWindow.Utils.ApplyTheme(this.currentBootPromotion.ThemeName);
      }
      else
      {
        this.mExtraPayloadClicked = this.currentBootPromotion.ExtraPayload;
        this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.currentBootPromotion.ExtraPayload, "boot_promo", "");
      }
    }

    private void StopSlider()
    {
      try
      {
        if (!this.mRunPromotion)
          return;
        this.mRunPromotion = false;
        if (PromotionControl.sBootPromotionDisplayed == null || !PromotionControl.sBootPromotionDisplayed.Any<KeyValuePair<BootPromotion, int>>())
          return;
        Dictionary<BootPromotion, int> bootPromos = PromotionControl.sBootPromotionDisplayed;
        PromotionControl.sBootPromotionDisplayed = (Dictionary<BootPromotion, int>) null;
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj => PromotionControl.SendPromotionStats(bootPromos)));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception aborting thread" + ex.ToString());
      }
    }

    private static void SendPromotionStats(Dictionary<BootPromotion, int> bootPromos)
    {
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "prod_ver",
            RegistryManager.Instance.ClientVersion
          },
          {
            "eng_ver",
            RegistryManager.Instance.Version
          },
          {
            "guid",
            RegistryManager.Instance.UserGuid
          },
          {
            "locale",
            RegistryManager.Instance.UserSelectedLocale
          },
          {
            "oem",
            RegistryManager.Instance.Oem
          },
          {
            "partner",
            RegistryManager.Instance.Partner
          },
          {
            "campaign_json",
            RegistryManager.Instance.CampaignJson
          }
        };
        List<BootBanner> bootBannerList = new List<BootBanner>();
        foreach (KeyValuePair<BootPromotion, int> bootPromo in bootPromos)
          bootBannerList.Add(new BootBanner()
          {
            Frequency = bootPromo.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            ClickActionPackagename = bootPromo.Key.ExtraPayload["click_action_packagename"],
            ClickGenericAction = bootPromo.Key.ExtraPayload["click_generic_action"],
            ClickActionValue = bootPromo.Key.ExtraPayload["click_action_value"],
            Id = bootPromo.Key.Id,
            ButtonText = bootPromo.Key.ButtonText,
            Order = bootPromo.Key.Order.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            ImageUrl = bootPromo.Key.ImageUrl,
            HashTags = bootPromo.Key.ExtraPayload["hash_tags"]
          });
        data.Add("boot_banners", JsonConvert.SerializeObject((object) bootBannerList));
        BstHttpClient.Post(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "bs4/stats/client_boot_promotion_stats"), (string) null, (string) null, (string) null), data, (Dictionary<string, string>) null, false, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error(nameof (SendPromotionStats), (object) ex);
      }
    }

    private void SetLoadingText(string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        this.mPromoButton.Content = (object) text;
        this.mPromoButton.Visibility = Visibility.Visible;
      }
      else
        this.mPromoButton.Visibility = Visibility.Hidden;
      this.isPerformActionOnClose = false;
      this.mPromotionInfoBorder.Visibility = Visibility.Collapsed;
    }

    internal void Stop()
    {
      this.progressTimer.Stop();
      this.progressTimer.Dispose();
      this.mBootStringTimer.Stop();
      this.mBootStringTimer.Dispose();
      this.StopSlider();
    }

    internal void HandlePromotionEventAfterBoot()
    {
      if (!this.isPerformActionOnClose)
        return;
      this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.currentBootPromotion.ExtraPayload, "boot_promo", "");
      this.isPerformActionOnClose = false;
    }

    private void HandleAnimation(BootPromotion promo)
    {
      PromotionControl.AnimateImage(this.mPromotionImage, promo.ImagePath);
      if (string.IsNullOrEmpty(promo.ButtonText))
        return;
      this.mPromoButton.Visibility = Visibility.Visible;
      this.mPromoButton.Content = (object) promo.ButtonText;
    }

    private static void AnimateImage(CustomPictureBox image, string imagePath)
    {
      TimeSpan timeSpan1 = TimeSpan.FromSeconds(0.6);
      TimeSpan timeSpan2 = TimeSpan.FromSeconds(0.6);
      DoubleAnimation fadeInAnimation = new DoubleAnimation(1.0, (Duration) timeSpan1);
      if (image.Source == null)
      {
        image.Opacity = 1.0;
        image.IsFullImagePath = true;
        image.ImageName = imagePath;
      }
      else
      {
        if (!(image.ImageName != imagePath))
          return;
        DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, (Duration) timeSpan2);
        doubleAnimation.Completed += (EventHandler) ((o, e) =>
        {
          image.IsFullImagePath = true;
          image.ImageName = imagePath;
          image.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline) fadeInAnimation);
        });
        image.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline) doubleAnimation);
      }
    }

    private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mPromotionInfoBorder.Visibility = Visibility.Collapsed;
    }

    private void BootStringTimer_Tick(object sender, EventArgs e1)
    {
      ++this.mBootStringIndex;
      this.mBootStringIndex = this.mBootStringIndex >= this.mBootStrings.Count ? 0 : this.mBootStringIndex;
      BlueStacksUIBinding.Bind(this.BootText, this.mBootStrings[this.mBootStringIndex], "");
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.progressTimer != null)
      {
        this.progressTimer.Tick -= new EventHandler(this.ProgressTimer_Tick);
        this.progressTimer.Dispose();
      }
      if (this.mBootStringTimer != null)
      {
        this.mBootStringTimer.Tick -= new EventHandler(this.BootStringTimer_Tick);
        this.mBootStringTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~PromotionControl()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/promotioncontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.UserControl_Loaded);
          break;
        case 2:
          this.mPromotionImageGrid = (Grid) target;
          break;
        case 3:
          this.mPromotionImage = (CustomPictureBox) target;
          break;
        case 4:
          this.mPromoButton = (CustomButton) target;
          this.mPromoButton.Click += new RoutedEventHandler(this.mPromoButton_Click);
          break;
        case 5:
          this.mPromotionInfoBorder = (Border) target;
          break;
        case 6:
          this.mPromoInfoText = (TextBlock) target;
          break;
        case 7:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_PreviewMouseLeftButtonUp);
          break;
        case 8:
          this.mProgressBar = (BlueProgressBar) target;
          break;
        case 9:
          this.BootText = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
