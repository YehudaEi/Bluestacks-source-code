// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ImageTranslateControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class ImageTranslateControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    private Thread httpBackGroundThread;
    internal Grid mGrid;
    internal Grid mTopBar;
    internal Label mTitleLabel;
    internal CustomPictureBox mCloseButton;
    internal CustomPictureBox mFrontEndImage;
    internal CustomPictureBox mLoadingImage;
    internal TextBlock mBootText;
    private bool _contentLoaded;

    public static ImageTranslateControl Instance { get; private set; }

    public ImageTranslateControl(MainWindow parentWindow)
    {
      this.InitializeComponent();
      ImageTranslateControl.Instance = this;
      this.ParentWindow = parentWindow;
      if (this.ParentWindow != null)
      {
        this.Width = parentWindow.FrontendParentGrid.ActualWidth;
        this.Height = parentWindow.FrontendParentGrid.ActualHeight;
      }
      this.mLoadingImage.Visibility = Visibility.Visible;
      this.mFrontEndImage.Visibility = Visibility.Collapsed;
      this.mTopBar.Visibility = Visibility.Collapsed;
      this.mBootText.Visibility = Visibility.Visible;
      this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_LOADING_MESSAGE", "");
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Window.GetWindow((DependencyObject) this).KeyDown += new KeyEventHandler(this.UserControl_KeyDown);
    }

    public void GetTranslateImage(Bitmap bitmap)
    {
      if (bitmap == null)
        return;
      this.httpBackGroundThread = new Thread((ThreadStart) (() =>
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          try
          {
            bitmap.Save((Stream) memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0L;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string a = RegistryManager.Instance.UserSelectedLocale.Substring(0, 2);
            if (string.Equals(a, "zh-CN", StringComparison.InvariantCulture) || string.Equals(a, "zh-TW", StringComparison.InvariantCulture))
              a = RegistryManager.Instance.UserSelectedLocale;
            if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocale))
              a = RegistryManager.Instance.TargetLocale;
            parameters.Add("locale", (object) a);
            parameters.Add("inputImage", (object) new FormFile()
            {
              Name = "image.jpg",
              ContentType = "image/jpeg",
              Stream = (Stream) memoryStream
            });
            parameters.Add("oem", (object) RegistryManager.Instance.Oem);
            parameters.Add("guid", (object) RegistryManager.Instance.UserGuid);
            parameters.Add("prod_ver", (object) RegistryManager.Instance.ClientVersion);
            string str1 = Convert.ToBase64String(memoryStream.ToArray()) + RegistryManager.Instance.UserGuid + "BstTranslate";
            _MD5 md5 = new _MD5() { Value = str1 };
            parameters.Add("token", (object) md5.FingerPrint);
            string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/translate/postimage");
            if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocaleUrl))
              url = RegistryManager.Instance.TargetLocaleUrl;
            string empty = string.Empty;
            byte[] dataArray = (byte[]) null;
            string str2;
            try
            {
              str2 = BstHttpClient.PostMultipart(url, parameters, out dataArray);
            }
            catch (Exception ex)
            {
              Logger.Error("error while downloading translated image.." + ex.ToString());
              str2 = "error";
            }
            if (str2.Contains("error"))
              this.Dispatcher.Invoke((Delegate) (() =>
              {
                this.mLoadingImage.Visibility = Visibility.Collapsed;
                this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
              }));
            else
              this.Dispatcher.Invoke((Delegate) (() =>
              {
                this.mFrontEndImage.Source = (ImageSource) ImageUtils.ByteArrayToImage(dataArray);
                this.mFrontEndImage.ReloadImages();
                this.mFrontEndImage.Visibility = Visibility.Visible;
                this.mTopBar.Visibility = Visibility.Visible;
                this.mLoadingImage.Visibility = Visibility.Collapsed;
                this.mBootText.Visibility = Visibility.Collapsed;
              }));
          }
          catch (Exception ex)
          {
            Logger.Error("Error in GetTranslateImage " + ex?.ToString());
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              this.mLoadingImage.Visibility = Visibility.Collapsed;
              this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
            }));
          }
        }
      }))
      {
        IsBackground = true
      };
      this.httpBackGroundThread.Start();
    }

    public bool Close()
    {
      try
      {
        ImageTranslateControl.Instance = (ImageTranslateControl) null;
        this.httpBackGroundThread?.Abort();
        this.ParentWindow?.HideDimOverlay();
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close imagetranslateontrol from dimoverlay " + ex.ToString());
      }
      return false;
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Escape)
        return;
      this.Close();
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return true;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow { get; set; } = true;

    public bool ShowTransparentWindow { get; set; } = true;

    bool IDimOverlayControl.Close()
    {
      this.Close();
      return true;
    }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/imagetranslatecontrol.xaml", UriKind.Relative));
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
          this.mGrid = (Grid) target;
          break;
        case 3:
          this.mTopBar = (Grid) target;
          break;
        case 4:
          this.mTitleLabel = (Label) target;
          break;
        case 5:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 6:
          this.mFrontEndImage = (CustomPictureBox) target;
          break;
        case 7:
          this.mLoadingImage = (CustomPictureBox) target;
          break;
        case 8:
          this.mBootText = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
