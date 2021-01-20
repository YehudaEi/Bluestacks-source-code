// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.QuitPopupControl
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
  public class QuitPopupControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    private bool mHasSuccessfulEventOccured;
    internal Grid mParentGrid;
    internal Grid mTitleGrid;
    internal CustomPictureBox mCrossButtonPictureBox;
    internal TextBlock mTitleText;
    internal Grid mOptionsGrid;
    internal StackPanel mQuitElementStackPanel;
    internal Grid mFooterGrid;
    internal CustomButton mReturnBlueStacksButton;
    internal CustomButton mCloseBlueStacksButton;
    private bool _contentLoaded;

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow { get; set; } = true;

    public bool ShowTransparentWindow { get; set; }

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

    public QuitPopupControl(MainWindow window)
    {
      this.ParentWindow = window;
      this.InitializeComponent();
    }

    private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
      ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_close");
    }

    public TextBlock TitleTextBlock
    {
      get
      {
        return this.mTitleText;
      }
    }

    public CustomButton CloseBlueStacksButton
    {
      get
      {
        return this.mCloseBlueStacksButton;
      }
    }

    public CustomButton ReturnBlueStacksButton
    {
      get
      {
        return this.mReturnBlueStacksButton;
      }
    }

    public CustomPictureBox CrossButtonPictureBox
    {
      get
      {
        return this.mCrossButtonPictureBox;
      }
    }

    private void CloseBlueStacksButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.Close();
      if (this.HasSuccessfulEventOccured)
        ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_continue_bluestacks");
      else
        ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "popup_closed");
    }

    public bool HasSuccessfulEventOccured
    {
      get
      {
        return this.mHasSuccessfulEventOccured;
      }
      set
      {
        if (!value)
          return;
        this.mHasSuccessfulEventOccured = value;
        this.mTitleGrid.Background = (Brush) new BrushConverter().ConvertFrom((object) "#0BA200");
      }
    }

    public string CurrentPopupTag { get; set; } = string.Empty;

    public void AddQuitActionItem(QuitActionItem item)
    {
      int num = (uint) this.mQuitElementStackPanel.Children.Count > 0U ? 1 : 0;
      QuitActionElement quitActionElement1 = new QuitActionElement(this.ParentWindow, this);
      quitActionElement1.Width = 210.0;
      quitActionElement1.ActionElement = item;
      quitActionElement1.ParentPopupTag = this.CurrentPopupTag;
      QuitActionElement quitActionElement2 = quitActionElement1;
      if (num != 0)
        quitActionElement2.Margin = new Thickness(32.0, 0.0, 0.0, 0.0);
      this.mQuitElementStackPanel.Children.Add((UIElement) quitActionElement2);
    }

    internal bool Close()
    {
      try
      {
        this.ParentWindow.HideDimOverlay();
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close quitpopup from dimoverlay " + ex.ToString());
      }
      return false;
    }

    private void ReturnBlueStacksButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.Close();
      ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_return_bluestacks");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/quitpopupcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mParentGrid = (Grid) target;
          break;
        case 2:
          this.mTitleGrid = (Grid) target;
          break;
        case 3:
          this.mCrossButtonPictureBox = (CustomPictureBox) target;
          this.mCrossButtonPictureBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Close_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mTitleText = (TextBlock) target;
          break;
        case 5:
          this.mOptionsGrid = (Grid) target;
          break;
        case 6:
          this.mQuitElementStackPanel = (StackPanel) target;
          break;
        case 7:
          this.mFooterGrid = (Grid) target;
          break;
        case 8:
          this.mReturnBlueStacksButton = (CustomButton) target;
          this.mReturnBlueStacksButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ReturnBlueStacksButton_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mCloseBlueStacksButton = (CustomButton) target;
          this.mCloseBlueStacksButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBlueStacksButton_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
