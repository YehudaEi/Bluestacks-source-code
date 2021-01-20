// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RichNotificationPopup
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class RichNotificationPopup : CustomWindow, IComponentConnector
  {
    internal CustomPictureBox mBackgroundImage;
    internal Grid mCloseButtonGrid;
    internal StackPanel mCloseButtonStackPanel;
    internal CustomPictureBox mMuteButton;
    internal CustomPictureBox mCloseButton;
    internal Grid mBottomGrid;
    internal CustomPictureBox mGameIcon;
    internal Grid mGameDescriptionGrid;
    internal TextBlock mGameTitle;
    internal TextBlock mGameDeveloper;
    internal CustomButton mButton;
    private bool _contentLoaded;

    public string BackgroundImage
    {
      set
      {
        this.mBackgroundImage.ImageName = value;
      }
    }

    public string GameIcon
    {
      set
      {
        this.mGameIcon.ImageName = value;
      }
    }

    public string GameTitleText
    {
      set
      {
        this.mGameTitle.Text = value;
      }
    }

    public string GameDeveloperText
    {
      set
      {
        this.mGameDeveloper.Text = value;
      }
    }

    public CustomButton Button
    {
      get
      {
        return this.mButton;
      }
      set
      {
        this.mButton = value;
      }
    }

    public MouseButtonEventHandler CloseButtonHandler
    {
      set
      {
        this.mCloseButton.PreviewMouseLeftButtonUp += value;
      }
    }

    public MouseButtonEventHandler MuteButtonHandler
    {
      set
      {
        this.mMuteButton.PreviewMouseLeftButtonUp += value;
      }
    }

    public bool IsCentered
    {
      set
      {
        if (value)
          this.SetWindowStyle(RichPopupStyles.Centered);
        else
          this.SetWindowStyle(RichPopupStyles.Simple);
      }
    }

    public string AssetFolderPath { get; set; } = Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.ClientThemeName);

    private void SetWindowStyle(RichPopupStyles style)
    {
      if (style != RichPopupStyles.Centered)
      {
        if (style != RichPopupStyles.Simple)
          return;
        this.Width = 320.0;
        this.Height = 210.0;
        this.Left = SystemParameters.FullPrimaryScreenWidth - this.Width - 16.0;
        this.Top = SystemParameters.FullPrimaryScreenHeight - this.Height;
        this.mMuteButton.Height = this.mMuteButton.Width = 16.0;
        this.mMuteButton.Margin = new Thickness(0.0, 0.0, 5.0, 0.0);
        this.mCloseButton.Height = this.mCloseButton.Width = 16.0;
        this.mBottomGrid.Margin = new Thickness(10.0);
        this.mBottomGrid.Height = 26.0;
        this.mGameTitle.FontSize = 11.0;
        Grid.SetRowSpan((UIElement) this.mGameTitle, 2);
        this.mGameTitle.VerticalAlignment = VerticalAlignment.Center;
        this.mGameDeveloper.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.Width = 600.0;
        this.Height = 380.0;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      }
    }

    public RichNotificationPopup()
    {
      this.InitializeComponent();
    }

    private void mCloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    public void ShowWindow()
    {
      string path1 = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client\\Helper");
      this.mMuteButton.ImageName = Path.Combine(path1, "mute2.png");
      this.mCloseButton.ImageName = Path.Combine(path1, "close.png");
      this.mMuteButton.ToolTip = (object) LocaleStrings.GetLocalizedString("STRING_MUTE_NOTIFICATION_TOOLTIP", "");
      this.mCloseButton.ToolTip = (object) LocaleStrings.GetLocalizedString("STRING_CLOSE", "");
      this.Show();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/wpf/richnotificationpopup.xaml", UriKind.Relative));
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
          this.mBackgroundImage = (CustomPictureBox) target;
          break;
        case 2:
          this.mCloseButtonGrid = (Grid) target;
          break;
        case 3:
          this.mCloseButtonStackPanel = (StackPanel) target;
          break;
        case 4:
          this.mMuteButton = (CustomPictureBox) target;
          break;
        case 5:
          this.mCloseButton = (CustomPictureBox) target;
          break;
        case 6:
          this.mBottomGrid = (Grid) target;
          break;
        case 7:
          this.mGameIcon = (CustomPictureBox) target;
          break;
        case 8:
          this.mGameDescriptionGrid = (Grid) target;
          break;
        case 9:
          this.mGameTitle = (TextBlock) target;
          break;
        case 10:
          this.mGameDeveloper = (TextBlock) target;
          break;
        case 11:
          this.mButton = (CustomButton) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
