// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PromptGoogleSigninControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class PromptGoogleSigninControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal CustomPictureBox CloseBtn;
    internal CustomButton SigninLaterBtn;
    internal CustomButton SigninBtn;
    private bool _contentLoaded;

    public PromptGoogleSigninControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
    }

    private void CloseBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClose", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, (string) null, (string) null, RegistryManager.Instance.InstallID, (string) null, (string) null, (string) null, "Android");
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CloseBtn_MouseLeftButtonUp. Exception: " + ex?.ToString());
      }
    }

    private void SigninBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
        if (appIcon != null)
          this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
        ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClick", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, (string) null, (string) null, RegistryManager.Instance.InstallID, (string) null, (string) null, (string) null, "Android");
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SigninBtn_Click. Exception: " + ex?.ToString());
      }
    }

    private void SigninLaterBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ClientStats.SendMiscellaneousStatsAsync("GoogleSigninLater", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, (string) null, (string) null, RegistryManager.Instance.InstallID, (string) null, (string) null, (string) null, "Android");
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SigninLaterBtn_Click. Exception: " + ex?.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/promptgooglesignincontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.CloseBtn = (CustomPictureBox) target;
          this.CloseBtn.PreviewMouseDown += new MouseButtonEventHandler(this.CloseBtn_PreviewMouseDown);
          this.CloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        case 2:
          this.SigninLaterBtn = (CustomButton) target;
          this.SigninLaterBtn.Click += new RoutedEventHandler(this.SigninLaterBtn_Click);
          break;
        case 3:
          this.SigninBtn = (CustomButton) target;
          this.SigninBtn.Click += new RoutedEventHandler(this.SigninBtn_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
