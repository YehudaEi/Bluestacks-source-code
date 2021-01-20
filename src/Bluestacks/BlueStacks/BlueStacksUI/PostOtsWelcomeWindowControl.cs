// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PostOtsWelcomeWindowControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class PostOtsWelcomeWindowControl : UserControl, IDisposable, IComponentConnector
  {
    private bool? mSuccess;
    private Timer loginSyncTimer;
    private MainWindow ParentWindow;
    private bool disposedValue;
    internal CustomPictureBox mCloseButton;
    internal CustomPictureBox mPostOtsImage;
    internal CustomPictureBox mLoadingImage;
    internal Label mPostOtsLabel;
    internal TextBlock mPostOtsWarning;
    internal CustomButton mPostOtsButton;
    private bool _contentLoaded;

    public PostOtsWelcomeWindowControl(MainWindow ParentWindow)
    {
      this.InitializeComponent();
      this.ParentWindow = ParentWindow;
    }

    private void PostOtsWelcome_Loaded(object sender, RoutedEventArgs e)
    {
      Logger.Info("PostOtsWelcome window loaded");
      this.loginSyncTimer = new Timer(10000.0);
      this.loginSyncTimer.Elapsed += new ElapsedEventHandler(this.OnLoginSyncTimeout);
      this.loginSyncTimer.AutoReset = false;
      if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
        this.ChangeBasedonTokenReceived("true");
      else
        this.StartingTimer();
    }

    public void ChangeBasedonTokenReceived(string status)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          Logger.Info("In ChangeBasedonTokenReceived");
          this.mLoadingImage.Visibility = Visibility.Collapsed;
          if (status.Equals("true", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mPostOtsImage.ImageName = "success_ots_icon";
            this.mPostOtsWarning.Visibility = Visibility.Collapsed;
            this.mCloseButton.Visibility = Visibility.Collapsed;
            BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_SUCCESS_MESSAGE");
            BlueStacksUIBinding.Bind((Button) this.mPostOtsButton, "STRING_POST_OTS_SUCCESS_BUTTON_MESSAGE");
            this.mSuccess = new bool?(true);
          }
          else
          {
            this.mPostOtsImage.ImageName = "failure_ots_icon";
            this.mPostOtsWarning.Visibility = Visibility.Visible;
            this.mCloseButton.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_FAILED_MESSAGE");
            BlueStacksUIBinding.Bind((Button) this.mPostOtsButton, "STRING_POST_OTS_FAILED_BUTTON_MESSAGE");
            this.mSuccess = new bool?(false);
          }
          if (this.loginSyncTimer != null)
            this.loginSyncTimer.Stop();
          this.mPostOtsButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
          Logger.Error(" Exception in ChangeBasedOnTokenReceived Status: " + status + Environment.NewLine + "Error: " + ex.ToString());
        }
      }));
    }

    private void StartingTimer()
    {
      Logger.Info("Starting Timer");
      this.loginSyncTimer.Stop();
      this.loginSyncTimer.Start();
      this.loginSyncTimer.Enabled = true;
    }

    private void OnLoginSyncTimeout(object source, ElapsedEventArgs e)
    {
      try
      {
        Logger.Error("Login Sync timed out.");
        if (this.mSuccess.HasValue)
          return;
        this.ChangeBasedonTokenReceived("false");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in login sync timer timeout " + ex.ToString());
      }
    }

    private void mPostOtsButton_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("mPostOtsButton clicked");
      if (!this.mSuccess.HasValue)
        return;
      if (this.mSuccess.Value)
      {
        this.loginSyncTimer.Dispose();
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
      }
      else
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          this.mPostOtsImage.ImageName = "syncing_ots_icon";
          this.mLoadingImage.Visibility = Visibility.Visible;
          this.mPostOtsWarning.Visibility = Visibility.Collapsed;
          this.mCloseButton.Visibility = Visibility.Collapsed;
          BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_SYNCING_MESSAGE");
          BlueStacksUIBinding.Bind((Button) this.mPostOtsButton, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE");
          this.mPostOtsButton.IsEnabled = false;
        }));
        this.SendRetryBluestacksLoginRequest(this.ParentWindow.mVmName);
      }
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        Logger.Info("Clicked postotswelcome window close button");
        this.ParentWindow.CloseWindow();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing bluestacks from postotswelcome window, " + ex.ToString());
      }
    }

    private void SendRetryBluestacksLoginRequest(string vmName)
    {
      try
      {
        Logger.Info("Sending retry call for token to android, since token is not received successfully");
        this.mSuccess = new bool?();
        this.StartingTimer();
        BlueStacksUIUtils.SendBluestacksLoginRequest(vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendRetryBluestacksLoginRequest: " + ex.ToString());
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.loginSyncTimer != null)
      {
        this.loginSyncTimer.Elapsed -= new ElapsedEventHandler(this.OnLoginSyncTimeout);
        this.loginSyncTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~PostOtsWelcomeWindowControl()
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/postotswelcomewindowcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.PostOtsWelcome_Loaded);
          break;
        case 2:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 3:
          this.mPostOtsImage = (CustomPictureBox) target;
          break;
        case 4:
          this.mLoadingImage = (CustomPictureBox) target;
          break;
        case 5:
          this.mPostOtsLabel = (Label) target;
          break;
        case 6:
          this.mPostOtsWarning = (TextBlock) target;
          break;
        case 7:
          this.mPostOtsButton = (CustomButton) target;
          this.mPostOtsButton.Click += new RoutedEventHandler(this.mPostOtsButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
