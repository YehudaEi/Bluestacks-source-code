// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SettingsWindowBase
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
  public abstract class SettingsWindowBase : UserControl, IComponentConnector
  {
    internal Grid mGrid;
    internal CustomPictureBox mSettingsWindowIcon;
    internal Label mLblBlueStacksSettings;
    internal CustomPictureBox mCrossButton;
    internal CustomPopUp mEnableVTPopup;
    internal TextBlock EnableVtInfo;
    internal Grid mBottomGrid;
    internal StackPanel settingsStackPanel;
    internal Line mSelectedLine;
    internal Grid settingsWindowGrid;
    private bool _contentLoaded;

    protected UserControl visibleControl { get; private set; }

    public string StartUpTab { get; set; } = "STRING_ENGINE_SETTING";

    public List<string> SettingsControlNameList { get; set; } = new List<string>();

    public Dictionary<string, UserControl> SettingsWindowControlsDict { get; set; } = new Dictionary<string, UserControl>();

    public bool IsVtxLearned { get; set; }

    public CustomPopUp EnableVTPopup
    {
      get
      {
        return this.mEnableVTPopup;
      }
    }

    public Grid SettingsWindowGrid
    {
      get
      {
        return this.settingsWindowGrid;
      }
    }

    public StackPanel SettingsWindowStackPanel
    {
      get
      {
        return this.settingsStackPanel;
      }
    }

    protected virtual void SetPopupOffset()
    {
    }

    public abstract void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);

    public SettingsWindowBase()
    {
      this.LoadViewFromUri("/HD-Common;component/Settings/SettingsWindowBase.xaml");
    }

    public void AddControlInGridAndDict(string btnName, UserControl control)
    {
      this.SettingsWindowControlsDict[btnName] = control;
      if (this.settingsWindowGrid.Children.Contains((UIElement) control))
        return;
      this.settingsWindowGrid.Children.Add((UIElement) control);
    }

    public void BringToFront(UserControl control)
    {
      if (control == null)
        return;
      if (this.visibleControl != null && this.visibleControl != control)
        this.visibleControl.Visibility = Visibility.Collapsed;
      control.Visibility = Visibility.Visible;
      this.visibleControl = control;
      switch (control)
      {
        case EngineSettingBase engineSettingBase when engineSettingBase.DataContext is EngineSettingBaseViewModel dataContext:
          dataContext.Init();
          engineSettingBase.SetGraphicMode(dataContext.GraphicsMode);
          engineSettingBase.SetAdvancedGraphicMode(dataContext.UseAdvancedGraphicEngine);
          dataContext.NotifyPropertyChangedAllProperties();
          break;
        case DisplaySettingsBase displaySettingsBase:
          displaySettingsBase.Init();
          break;
      }
      this.SetPopupOffset();
    }

    public bool CheckWidth()
    {
      return this.settingsStackPanel.ActualWidth == this.settingsStackPanel.ActualWidth && this.settingsWindowGrid.ActualWidth == this.settingsWindowGrid.ActualWidth;
    }

    public void SettingsBtn_Click(object sender, RoutedEventArgs e)
    {
      CustomSettingsButton customSettingsButton = (CustomSettingsButton) sender;
      if (customSettingsButton == null)
        return;
      customSettingsButton.IsSelected = true;
      UserControl control = this.SettingsWindowControlsDict[customSettingsButton.Name];
      Logger.Info("Clicked {0} button", (object) customSettingsButton.Name);
      this.BringToFront(control);
      if (customSettingsButton.Name.Equals("STRING_SHORTCUT_KEY_SETTINGS", StringComparison.OrdinalIgnoreCase))
        Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_open", (string) null, (string) null, (string) null, (string) null, (string) null, "Android", 0);
      else
        Stats.SendMiscellaneousStatsAsync("settings", RegistryManager.Instance.UserGuid, LocaleStrings.GetLocalizedString(customSettingsButton.Name, ""), "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android", 0);
    }

    private void mCrossButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void EnableVtInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked Enable Vt popup Settings window");
      this.IsVtxLearned = true;
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&article={1}", (object) WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null), (object) "enable_virtualization");
      if (Oem.IsOEMDmm)
        url = "http://help.dmm.com/-/detail/=/qid=45997/";
      Utils.OpenUrl(url);
    }

    private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mEnableVTPopup.IsOpen = false;
    }

    private void mEnableVTPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.mEnableVTPopup.IsOpen = false;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Window.GetWindow((DependencyObject) this).LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.UserControl_LostKeyboardFocus);
    }

    private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mEnableVTPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/settings/settingswindowbase.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.UserControl_Loaded);
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.UserControl_MouseLeftButtonUp);
          break;
        case 2:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Grid_MouseLeftButtonUp);
          break;
        case 3:
          this.mGrid = (Grid) target;
          break;
        case 4:
          this.mSettingsWindowIcon = (CustomPictureBox) target;
          break;
        case 5:
          this.mLblBlueStacksSettings = (Label) target;
          break;
        case 6:
          this.mCrossButton = (CustomPictureBox) target;
          break;
        case 7:
          this.mEnableVTPopup = (CustomPopUp) target;
          break;
        case 8:
          this.EnableVtInfo = (TextBlock) target;
          this.EnableVtInfo.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.EnableVtInfo_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mBottomGrid = (Grid) target;
          break;
        case 10:
          this.settingsStackPanel = (StackPanel) target;
          break;
        case 11:
          this.mSelectedLine = (Line) target;
          break;
        case 12:
          this.settingsWindowGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
