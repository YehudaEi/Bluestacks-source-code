// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FrontendPopupControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class FrontendPopupControl : UserControl, IComponentConnector
  {
    private MainWindow mMainWindow;
    private EventHandler<EventArgs> RequestedAppDisplayed;
    private string mGooglePlayStoreArg;
    private bool mIsWindowForcedTillLoaded;
    internal DimControlWithProgresBar mBaseControl;
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

    public PlayStoreAction mAction { get; set; }

    public FrontendPopupControl()
    {
      this.InitializeComponent();
      this.RequestedAppDisplayed += new EventHandler<EventArgs>(this.RequestedApp_Displayed);
    }

    private void ProcessArgs(string googlePlayStoreArg, bool isWindowForcedTillLoaded)
    {
      this.mGooglePlayStoreArg = googlePlayStoreArg;
      this.mIsWindowForcedTillLoaded = isWindowForcedTillLoaded;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mBaseControl.Init((Control) this, (Panel) this.ParentWindow.mFrontendGrid, false, isWindowForcedTillLoaded);
        this.mBaseControl.DimBackground();
        this.ParentWindow.mCommonHandler.SetCustomCursorForApp("com.android.vending");
        if (this.mAction == PlayStoreAction.OpenApp)
        {
          if (this.ParentWindow.mAppHandler.IsAppInstalled(googlePlayStoreArg))
          {
            this.ParentWindow.mAppHandler.SendRunAppRequestAsync(googlePlayStoreArg, "", false);
          }
          else
          {
            AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
            this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(googlePlayStoreArg);
          }
          this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
        }
        else if (this.mAction == PlayStoreAction.SearchApp)
        {
          AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
          this.ParentWindow.mAppHandler.SendSearchPlayRequestAsync(googlePlayStoreArg);
          this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
        }
        else
        {
          if (this.mAction != PlayStoreAction.CustomActivity)
            return;
          AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
          this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
          this.ParentWindow.mAppHandler.StartCustomActivity(new Dictionary<string, string>()
          {
            {
              "action",
              googlePlayStoreArg
            }
          });
        }
      }));
    }

    internal void Reload()
    {
      if (this.Visibility != Visibility.Visible || string.IsNullOrEmpty(this.mGooglePlayStoreArg))
        return;
      this.ProcessArgs(this.mGooglePlayStoreArg, this.mIsWindowForcedTillLoaded);
    }

    internal void RequestedApp_Displayed(object sender, EventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.mBaseControl.ShowContent()));
    }

    internal void Init(
      string args,
      string appName,
      PlayStoreAction action,
      bool isWindowForcedTillLoaded = false)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!this.ParentWindow.mGuestBootCompleted)
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE", "");
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_GUEST_NOT_BOOTED", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.Owner = (Window) this.ParentWindow;
          customMessageWindow.ShowDialog();
        }
        else if (action == PlayStoreAction.OpenApp && this.ParentWindow.mAppHandler.IsAppInstalled(args) && !"com.android.vending".Equals(args, StringComparison.InvariantCultureIgnoreCase))
        {
          AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(args);
          if (appIcon == null)
            return;
          if (appIcon.AppIncompatType != AppIncompatType.None && !this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(appIcon.PackageName))
            GrmHandler.HandleCompatibility(appIcon.PackageName, this.ParentWindow.mVmName);
          else
            this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
        }
        else
        {
          if (string.IsNullOrEmpty(args))
            return;
          if (!this.ParentWindow.WelcomeTabParentGrid.IsVisible)
            this.ParentWindow.mCommonHandler.HomeButtonHandler(false, false);
          this.mBaseControl.mTitleLabel.Content = (object) appName;
          this.mAction = action;
          this.Visibility = Visibility.Visible;
          this.ParentWindow.ChangeOrientationFromClient(false, false);
          this.ProcessArgs(args, isWindowForcedTillLoaded);
        }
      }));
    }

    internal void HideWindow()
    {
      this.mBaseControl.HideWindow();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/frontendpopupcontrol.xaml", UriKind.Relative));
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
      if (connectionId == 1)
        this.mBaseControl = (DimControlWithProgresBar) target;
      else
        this._contentLoaded = true;
    }
  }
}
