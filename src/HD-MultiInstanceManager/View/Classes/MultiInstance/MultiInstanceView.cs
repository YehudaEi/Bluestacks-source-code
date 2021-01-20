// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.MultiInstance.MultiInstanceView
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace MultiInstanceManagerMVVM.View.Classes.MultiInstance
{
  public class MultiInstanceView : UiWindowBase, IComponentConnector, IStyleConnector
  {
    private DispatcherTimer mEcoModeHelpPopupTimer;
    internal MultiInstanceView MultiInstanceWindow;
    internal Grid mMainGrid;
    internal ToggleButton mConfigButton;
    internal CustomButton mBatchDeleteButton;
    internal CustomComboBox SortOptionComboBox;
    internal StackPanel mEcoModeStackPanel;
    internal CustomPictureBox mEcoModeHelp;
    internal Grid mOverlayGrid;
    internal CustomPopUp mConfigMenuItem;
    internal Border mShadowBorder;
    internal Grid mGrid;
    internal Border mMaskBorder;
    internal CustomPopUp mEcoModeHelpPopup;
    internal Border mEcoModeHelpPopupBorder;
    private bool _contentLoaded;

    public MultiInstanceView()
    {
      this.InitializeComponent();
    }

    private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (sender == null)
        return;
      (sender as TextBlock).SetTextblockTooltip();
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void mConfigMenuItem_Opened(object sender, EventArgs e)
    {
      this.mConfigMenuItem.HorizontalOffset = -(this.mShadowBorder.ActualWidth - 28.0);
      this.mConfigMenuItem.VerticalOffset = 4.0;
    }

    private void EcoModeHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mEcoModeHelpPopup.IsOpen = true;
    }

    private void EcoModeHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mEcoModeHelpPopup.IsOpen)
        return;
      if (this.mEcoModeHelpPopupTimer == null)
      {
        this.mEcoModeHelpPopupTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 1)
        };
        this.mEcoModeHelpPopupTimer.Tick += new EventHandler(this.EcoModeHelpPopupTimer_Tick);
      }
      else
        this.mEcoModeHelpPopupTimer.Stop();
      this.mEcoModeHelpPopupTimer.Start();
    }

    private void EcoModeHelpPopupTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mEcoModeHelpPopup.IsMouseOver && !this.mEcoModeHelp.IsMouseOver)
        this.mEcoModeHelpPopup.IsOpen = false;
      (sender as DispatcherTimer).Stop();
    }

    private void EcoModeHelpPopup_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mEcoModeHelpPopup.IsOpen = false;
    }

    private void MultiInstanceWindow_Deactivated(object sender, EventArgs e)
    {
      this.mEcoModeHelpPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/multiinstance/multiinstanceview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.MultiInstanceWindow = (MultiInstanceView) target;
          this.MultiInstanceWindow.Deactivated += new EventHandler(this.MultiInstanceWindow_Deactivated);
          break;
        case 3:
          this.mMainGrid = (Grid) target;
          break;
        case 4:
          this.mConfigButton = (ToggleButton) target;
          break;
        case 5:
          this.mBatchDeleteButton = (CustomButton) target;
          break;
        case 6:
          this.SortOptionComboBox = (CustomComboBox) target;
          break;
        case 7:
          this.mEcoModeStackPanel = (StackPanel) target;
          break;
        case 8:
          this.mEcoModeHelp = (CustomPictureBox) target;
          this.mEcoModeHelp.MouseEnter += new MouseEventHandler(this.EcoModeHelp_MouseEnter);
          this.mEcoModeHelp.MouseLeave += new MouseEventHandler(this.EcoModeHelp_MouseLeave);
          break;
        case 9:
          this.mOverlayGrid = (Grid) target;
          break;
        case 10:
          this.mConfigMenuItem = (CustomPopUp) target;
          this.mConfigMenuItem.Opened += new EventHandler(this.mConfigMenuItem_Opened);
          break;
        case 11:
          this.mShadowBorder = (Border) target;
          break;
        case 12:
          this.mGrid = (Grid) target;
          break;
        case 13:
          this.mMaskBorder = (Border) target;
          break;
        case 14:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 15:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 16:
          this.mEcoModeHelpPopup = (CustomPopUp) target;
          this.mEcoModeHelpPopup.MouseLeave += new MouseEventHandler(this.EcoModeHelp_MouseLeave);
          break;
        case 17:
          this.mEcoModeHelpPopupBorder = (Border) target;
          break;
        case 18:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.EcoModeHelpPopup_MouseDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((Style) target).Setters.Add((SetterBase) new EventSetter()
      {
        Event = FrameworkElement.SizeChangedEvent,
        Handler = (Delegate) new SizeChangedEventHandler(this.TextBlock_SizeChanged)
      });
    }
  }
}
