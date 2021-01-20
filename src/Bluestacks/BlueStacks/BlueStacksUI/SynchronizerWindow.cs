// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SynchronizerWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class SynchronizerWindow : CustomWindow, IComponentConnector
  {
    private MainWindow ParentWindow;
    private bool mIsActiveWindowPresent;
    private bool mStopEventFromPropagatingFurther;
    internal Border mMaskBorder;
    internal Grid mTopGrid;
    internal Border mLineSeperator;
    internal Grid mNoActiveWindowsGrid;
    internal ScrollViewer mActiveWindowsListScrollbar;
    internal CustomCheckbox mSelectAllCheckbox;
    internal StackPanel mActiveWindowsPanel;
    internal Grid mBottomGrid;
    internal Border mLineSeperator1;
    internal CustomButton mStartSyncBtn;
    internal CustomButton mLaunchInstanceManagerBtn;
    internal TextBlock mSyncHelp;
    internal Hyperlink mHyperLink;
    private bool _contentLoaded;

    public SynchronizerWindow(MainWindow parent)
    {
      this.ParentWindow = parent;
      this.Owner = (Window) parent;
      this.IsShowGLWindow = true;
      this.InitializeComponent();
      this.mHyperLink.NavigateUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=" + "operation_synchronization");
      this.mHyperLink.Inlines.Clear();
      this.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
      BlueStacksUIBinding.Instance.PropertyChanged += new PropertyChangedEventHandler(this.Binding_PropertyChanged);
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      this.mSyncHelp.Visibility = Visibility.Collapsed;
    }

    private void Binding_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "LocaleModel"))
        return;
      this.mHyperLink.Inlines.Clear();
      this.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
    }

    internal void Init(bool closeSyncWindowIfEmpty = false)
    {
      this.mIsActiveWindowPresent = false;
      this.mActiveWindowsPanel.Children.Clear();
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        if (dictWindow.Key != this.ParentWindow.mVmName && (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(dictWindow.Key) || this.ParentWindow.mSelectedInstancesForSync.Contains(dictWindow.Key)))
        {
          CustomCheckbox customCheckbox1 = new CustomCheckbox();
          customCheckbox1.Content = (object) SynchronizerWindow.GetInstanceGameOrDisplayName(dictWindow.Key);
          customCheckbox1.Tag = (object) dictWindow.Key;
          CustomCheckbox customCheckbox2 = customCheckbox1;
          if (customCheckbox2.Image != null)
          {
            customCheckbox2.Image.Height = 16.0;
            customCheckbox2.Image.Width = 16.0;
          }
          customCheckbox2.Height = 25.0;
          customCheckbox2.FontSize = 16.0;
          customCheckbox2.Margin = new Thickness(12.0, 8.0, 0.0, 0.0);
          if (this.ParentWindow.mSelectedInstancesForSync.Contains(customCheckbox2.Tag.ToString()))
            customCheckbox2.IsChecked = new bool?(true);
          else
            customCheckbox2.IsChecked = new bool?(false);
          customCheckbox2.MouseEnter += new MouseEventHandler(this.InstanceCheckbox_MouseEnter);
          customCheckbox2.MouseLeave += new MouseEventHandler(this.InstanceCheckbox_MouseLeave);
          customCheckbox2.Checked += new RoutedEventHandler(this.InstanceCheckbox_Checked);
          customCheckbox2.Unchecked += new RoutedEventHandler(this.InstanceCheckbox_Unchecked);
          this.mActiveWindowsPanel.Children.Add((UIElement) customCheckbox2);
          this.mIsActiveWindowPresent = true;
          this.mActiveWindowsListScrollbar.Visibility = Visibility.Visible;
        }
      }
      if (this.mIsActiveWindowPresent)
      {
        this.mLaunchInstanceManagerBtn.Visibility = Visibility.Collapsed;
        this.mNoActiveWindowsGrid.Visibility = Visibility.Collapsed;
        this.mStartSyncBtn.Visibility = Visibility.Visible;
        if (this.ParentWindow.mIsSynchronisationActive)
          this.mStartSyncBtn.IsEnabled = false;
        else
          this.ToggleStartSyncButton();
        this.ToggleSelectAllCheckboxSelection();
      }
      else if (closeSyncWindowIfEmpty || FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.Close_MouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
      }
      else
      {
        this.mActiveWindowsListScrollbar.Visibility = Visibility.Collapsed;
        this.mNoActiveWindowsGrid.Visibility = Visibility.Visible;
        this.mStartSyncBtn.Visibility = Visibility.Collapsed;
        this.mLaunchInstanceManagerBtn.Visibility = Visibility.Visible;
      }
    }

    private void InstanceCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      CustomCheckbox customCheckbox = sender as CustomCheckbox;
      customCheckbox.IsChecked = new bool?(false);
      this.ParentWindow.mSelectedInstancesForSync.Remove(customCheckbox.Tag.ToString());
      this.ToggleSelectAllCheckboxSelection();
      if (this.ParentWindow.mIsSynchronisationActive)
      {
        HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>) null, customCheckbox.Tag.ToString(), 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.HideSyncPanel();
        if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
          BlueStacksUIUtils.sSyncInvolvedInstances.Remove(customCheckbox.Tag.ToString());
        if (this.ParentWindow.mSelectedInstancesForSync.Count == 0)
        {
          this.ParentWindow.mIsSynchronisationActive = false;
          this.ParentWindow.mIsSyncMaster = false;
          if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
            BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
          this.ParentWindow._TopBar.HideSyncPanel();
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
        }
        this.UpdateOtherSyncWindows(false);
      }
      if (!this.ParentWindow.mIsSynchronisationActive)
        this.ToggleStartSyncButton();
      this.mStopEventFromPropagatingFurther = false;
    }

    private void InstanceCheckbox_Checked(object sender, RoutedEventArgs e)
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      CustomCheckbox customCheckbox = sender as CustomCheckbox;
      customCheckbox.IsChecked = new bool?(true);
      this.ParentWindow.mSelectedInstancesForSync.Add(customCheckbox.Tag.ToString());
      this.ToggleSelectAllCheckboxSelection();
      if (this.ParentWindow.mIsSynchronisationActive)
      {
        HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", new Dictionary<string, string>()
        {
          {
            "instance",
            this.ParentWindow.mVmName
          }
        }, BlueStacksUIUtils.DictWindows[(sender as CustomCheckbox).Tag.ToString()].mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.ShowSyncPanel(false);
        if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
          BlueStacksUIUtils.sSyncInvolvedInstances.Add(customCheckbox.Tag.ToString());
        this.UpdateOtherSyncWindows(false);
      }
      else
        this.ToggleStartSyncButton();
      this.mStopEventFromPropagatingFurther = false;
    }

    private void mSelectAll_Checked(object sender, RoutedEventArgs e)
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      foreach (CustomCheckbox child in this.mActiveWindowsPanel.Children)
      {
        child.IsChecked = new bool?(true);
        if (!this.ParentWindow.mSelectedInstancesForSync.Contains(child.Tag.ToString()))
        {
          this.ParentWindow.mSelectedInstancesForSync.Add(child.Tag.ToString());
          if (this.ParentWindow.mIsSynchronisationActive)
          {
            HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", new Dictionary<string, string>()
            {
              {
                "instance",
                this.ParentWindow.mVmName
              }
            }, child.Tag.ToString(), 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            BlueStacksUIUtils.DictWindows[child.Tag.ToString()]._TopBar.ShowSyncPanel(false);
            if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(child.Tag.ToString()))
              BlueStacksUIUtils.sSyncInvolvedInstances.Add(child.Tag.ToString());
            this.UpdateOtherSyncWindows(false);
          }
        }
      }
      this.ToggleStartSyncButton();
      this.mStopEventFromPropagatingFurther = false;
    }

    private void mSelectAll_Unchecked(object sender, RoutedEventArgs e)
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      foreach (CustomCheckbox child in this.mActiveWindowsPanel.Children)
      {
        child.IsChecked = new bool?(false);
        if (this.ParentWindow.mSelectedInstancesForSync.Contains(child.Tag.ToString()))
        {
          this.ParentWindow.mSelectedInstancesForSync.Remove(child.Tag.ToString());
          if (this.ParentWindow.mIsSynchronisationActive)
          {
            HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>) null, child.Tag.ToString(), 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            BlueStacksUIUtils.DictWindows[child.Tag.ToString()]._TopBar.HideSyncPanel();
            if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(child.Tag.ToString()))
              BlueStacksUIUtils.sSyncInvolvedInstances.Remove(child.Tag.ToString());
          }
        }
      }
      if (this.ParentWindow.mIsSynchronisationActive)
      {
        this.ParentWindow.mIsSynchronisationActive = false;
        this.ParentWindow.mIsSyncMaster = false;
        if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
          BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
        this.ParentWindow._TopBar.HideSyncPanel();
        this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
        this.UpdateOtherSyncWindows(false);
      }
      this.ToggleStartSyncButton();
      this.mStopEventFromPropagatingFurther = false;
    }

    private void InstanceCheckbox_MouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as CustomCheckbox), Control.BackgroundProperty, "SettingsWindowBackground");
    }

    private void InstanceCheckbox_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as CustomCheckbox), Control.BackgroundProperty, "GameControlNavigationBackgroundColor");
    }

    private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void mStartSyncBtn_Click(object sender, RoutedEventArgs e)
    {
      this.mStartSyncBtn.IsEnabled = false;
      this.ParentWindow._TopBar.ShowSyncPanel(true);
      this.ParentWindow.mIsSyncMaster = true;
      ClientStats.SendMiscellaneousStatsAsync("MultipleInstancesSynced", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, (string) null, (string) null, "Android");
      Dictionary<string, string> data = new Dictionary<string, string>();
      IEnumerable<CustomCheckbox> source = this.mActiveWindowsPanel.Children.OfType<CustomCheckbox>().Where<CustomCheckbox>((Func<CustomCheckbox, bool>) (_ =>
      {
        bool? isChecked = _.IsChecked;
        bool flag = true;
        return isChecked.GetValueOrDefault() == flag & isChecked.HasValue;
      }));
      if (source.Any<CustomCheckbox>())
      {
        this.ParentWindow.mIsSynchronisationActive = true;
        data.Add("instances", string.Join(",", source.Select<CustomCheckbox, string>((Func<CustomCheckbox, string>) (_ => _.Tag.ToString())).ToArray<string>()));
        this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startOperationsSync", data);
        source.ToList<CustomCheckbox>().ForEach((System.Action<CustomCheckbox>) (customCheckbox => BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.ShowSyncPanel(false)));
      }
      foreach (CustomCheckbox customCheckbox in source.ToList<CustomCheckbox>())
      {
        if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
          BlueStacksUIUtils.sSyncInvolvedInstances.Add(customCheckbox.Tag.ToString());
      }
      if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
        BlueStacksUIUtils.sSyncInvolvedInstances.Add(this.ParentWindow.mVmName);
      this.UpdateOtherSyncWindows(true);
      this.Close_MouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
      if (!RegistryManager.Instance.IsShowToastNotification)
        return;
      this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STARTED", ""));
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Hide();
      this.ShowWithParentWindow = false;
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.Focus();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
        BlueStacksUIUtils.OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in opening url" + ex.ToString());
      }
    }

    private void mLaunchInstanceManagerBtn_Click(object sender, RoutedEventArgs e)
    {
      BlueStacksUIUtils.LaunchMultiInstanceManager();
      ClientStats.SendMiscellaneousStatsAsync("syncWindow", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void ToggleStartSyncButton()
    {
      if (this.ParentWindow.mSelectedInstancesForSync.Count > 0)
        this.mStartSyncBtn.IsEnabled = true;
      else
        this.mStartSyncBtn.IsEnabled = false;
    }

    private void ToggleSelectAllCheckboxSelection()
    {
      this.mStopEventFromPropagatingFurther = true;
      if (this.ParentWindow.mSelectedInstancesForSync.Count == this.mActiveWindowsPanel.Children.Count)
        this.mSelectAllCheckbox.IsChecked = new bool?(true);
      else
        this.mSelectAllCheckbox.IsChecked = new bool?(false);
      this.mStopEventFromPropagatingFurther = false;
    }

    private void SynchronizerWindow_Activated(object sender, EventArgs e)
    {
      if (this.mActiveWindowsPanel.Children.Count == 0)
      {
        if (FeatureManager.Instance.IsCustomUIForNCSoft)
        {
          this.Close_MouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
        }
        else
        {
          this.mIsActiveWindowPresent = false;
          this.mActiveWindowsListScrollbar.Visibility = Visibility.Collapsed;
          this.mStartSyncBtn.Visibility = Visibility.Collapsed;
          this.mNoActiveWindowsGrid.Visibility = Visibility.Visible;
          this.mLaunchInstanceManagerBtn.Visibility = Visibility.Visible;
          this.mNoActiveWindowsGrid.Height = double.NaN;
          this.SizeToContent = SizeToContent.WidthAndHeight;
        }
      }
      this.Left = this.ParentWindow.Left + (this.ParentWindow.Width - this.Width) / 2.0;
      this.Top = this.ParentWindow.Top + (this.ParentWindow.Height - this.Height) / 2.0;
    }

    internal void PauseAllSyncOperations()
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      foreach (string index in this.ParentWindow.mSelectedInstancesForSync)
        BlueStacksUIUtils.DictWindows[index]._TopBar.HideSyncPanel();
      HTTPUtils.SendRequestToEngineAsync("playPauseSync", new Dictionary<string, string>()
      {
        {
          "pause",
          "true"
        }
      }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      this.mStopEventFromPropagatingFurther = false;
    }

    internal void StopAllSyncOperations()
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      this.ParentWindow.mIsSynchronisationActive = false;
      this.ParentWindow.mIsSyncMaster = false;
      foreach (string index in this.ParentWindow.mSelectedInstancesForSync)
      {
        BlueStacksUIUtils.DictWindows[index]._TopBar.HideSyncPanel();
        if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(index))
          BlueStacksUIUtils.sSyncInvolvedInstances.Remove(index);
      }
      if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
        BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
      this.UpdateOtherSyncWindows(false);
      this.ParentWindow.mSelectedInstancesForSync.Clear();
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
      this.Init(false);
      this.mStopEventFromPropagatingFurther = false;
    }

    internal void PlayAllSyncOperations()
    {
      if (this.mStopEventFromPropagatingFurther)
        return;
      this.mStopEventFromPropagatingFurther = true;
      foreach (string index in this.ParentWindow.mSelectedInstancesForSync)
        BlueStacksUIUtils.DictWindows[index]._TopBar.ShowSyncPanel(false);
      HTTPUtils.SendRequestToEngineAsync("playPauseSync", new Dictionary<string, string>()
      {
        {
          "pause",
          "false"
        }
      }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      this.mStopEventFromPropagatingFurther = false;
    }

    private void UpdateOtherSyncWindows(bool closeSyncWindowIfEmpty = false)
    {
      try
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
          {
            if (dictWindow.Key != this.ParentWindow.mVmName && dictWindow.Value.mSynchronizerWindow != null && dictWindow.Value.mSynchronizerWindow.IsVisible)
              dictWindow.Value.mSynchronizerWindow.Init(closeSyncWindowIfEmpty);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
      }
    }

    private static string GetInstanceGameOrDisplayName(string vmName)
    {
      string appName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.AppName;
      string characterName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.CharacterName;
      return string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(characterName) ? Utils.GetDisplayName(vmName, "bgp") : appName + " " + characterName;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/synchronizerwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((Window) target).Activated += new EventHandler(this.SynchronizerWindow_Activated);
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          this.mTopGrid = (Grid) target;
          this.mTopGrid.MouseDown += new MouseButtonEventHandler(this.Topbar_MouseDown);
          break;
        case 4:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 5:
          this.mLineSeperator = (Border) target;
          break;
        case 6:
          this.mNoActiveWindowsGrid = (Grid) target;
          break;
        case 7:
          this.mActiveWindowsListScrollbar = (ScrollViewer) target;
          break;
        case 8:
          this.mSelectAllCheckbox = (CustomCheckbox) target;
          this.mSelectAllCheckbox.Checked += new RoutedEventHandler(this.mSelectAll_Checked);
          this.mSelectAllCheckbox.Unchecked += new RoutedEventHandler(this.mSelectAll_Unchecked);
          break;
        case 9:
          this.mActiveWindowsPanel = (StackPanel) target;
          break;
        case 10:
          this.mBottomGrid = (Grid) target;
          break;
        case 11:
          this.mLineSeperator1 = (Border) target;
          break;
        case 12:
          this.mStartSyncBtn = (CustomButton) target;
          this.mStartSyncBtn.Click += new RoutedEventHandler(this.mStartSyncBtn_Click);
          break;
        case 13:
          this.mLaunchInstanceManagerBtn = (CustomButton) target;
          this.mLaunchInstanceManagerBtn.Click += new RoutedEventHandler(this.mLaunchInstanceManagerBtn_Click);
          break;
        case 14:
          this.mSyncHelp = (TextBlock) target;
          break;
        case 15:
          this.mHyperLink = (Hyperlink) target;
          this.mHyperLink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
