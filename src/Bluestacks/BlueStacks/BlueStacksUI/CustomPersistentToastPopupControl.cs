// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CustomPersistentToastPopupControl
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
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class CustomPersistentToastPopupControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Border mPersistentToastPopupBorder;
    internal CustomPictureBox mCloseIcon;
    internal TextBlock mPersistentToastTextblock;
    internal CustomPopUp mCloseSettingsPopup;
    internal Grid dummyGrid;
    internal Border mCloseSettingsPopupBorder;
    internal Border mMaskBorder1;
    private bool _contentLoaded;

    public CustomPersistentToastPopupControl()
    {
      this.InitializeComponent();
    }

    public CustomPersistentToastPopupControl(Window window)
    {
      this.InitializeComponent();
      if (window == null)
        return;
      Grid grid = new Grid();
      object content = window.Content;
      window.Content = (object) grid;
      grid.Children.Add(content as UIElement);
      grid.Children.Add((UIElement) this);
    }

    public bool Init(MainWindow window, string text)
    {
      this.ParentWindow = window;
      if (this.ParentWindow == null || !this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled || !RegistryManager.Instance.IsShootingModeTooltipVisible)
        return false;
      this.Visibility = Visibility.Visible;
      this.mPersistentToastTextblock.Text = text;
      this.mPersistentToastPopupBorder.HorizontalAlignment = HorizontalAlignment.Center;
      this.mPersistentToastPopupBorder.VerticalAlignment = VerticalAlignment.Center;
      this.UpdateLayout();
      return true;
    }

    private void MCloseIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mCloseSettingsPopup.IsOpen = true;
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#33FFFFFF"));
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void mNeverShowAgain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mCloseSettingsPopup.IsOpen = false;
      this.Visibility = Visibility.Collapsed;
      RegistryManager.Instance.IsShootingModeTooltipVisible = false;
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mCloseSettingsPopup.IsOpen = false;
      this.Visibility = Visibility.Collapsed;
      this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/custompersistenttoastpopupcontrol.xaml", UriKind.Relative));
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
          this.mPersistentToastPopupBorder = (Border) target;
          break;
        case 2:
          this.mCloseIcon = (CustomPictureBox) target;
          this.mCloseIcon.MouseLeftButtonUp += new MouseButtonEventHandler(this.MCloseIcon_MouseLeftButtonUp);
          break;
        case 3:
          this.mPersistentToastTextblock = (TextBlock) target;
          break;
        case 4:
          this.mCloseSettingsPopup = (CustomPopUp) target;
          break;
        case 5:
          this.dummyGrid = (Grid) target;
          break;
        case 6:
          this.mCloseSettingsPopupBorder = (Border) target;
          break;
        case 7:
          this.mMaskBorder1 = (Border) target;
          break;
        case 8:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.mNeverShowAgain_MouseLeftButtonUp);
          break;
        case 9:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
