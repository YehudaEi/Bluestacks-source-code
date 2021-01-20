// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DimOverlayControl
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
  public class DimOverlayControl : CustomWindow, IComponentConnector
  {
    private IDimOverlayControl mControl;
    private ContainerWindow cw;
    internal Grid mGrid;
    private bool _contentLoaded;

    public MainWindow Owner
    {
      get
      {
        return (MainWindow) base.Owner;
      }
      set
      {
        if (value != null)
        {
          if (value != base.Owner)
          {
            value.LocationChanged += new EventHandler(this.ParentWindow_LocationChanged);
            value.SizeChanged += new SizeChangedEventHandler(this.ParentWindow_SizeChanged);
          }
        }
        else if (base.Owner != null)
        {
          base.Owner.LocationChanged -= new EventHandler(this.ParentWindow_LocationChanged);
          base.Owner.SizeChanged -= new SizeChangedEventHandler(this.ParentWindow_SizeChanged);
        }
        this.Owner = (Window) value;
      }
    }

    internal IDimOverlayControl Control
    {
      get
      {
        return this.mControl;
      }
      set
      {
        if (value != null)
          this.AddControl(value);
        this.mControl = value;
      }
    }

    public bool IsWindowVisible { get; set; }

    private void ParentWindow_LocationChanged(object sender, EventArgs e)
    {
      this.UpadteSizeLocation();
    }

    private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UpadteSizeLocation();
    }

    public DimOverlayControl(MainWindow owner)
    {
      this.Owner = owner;
      this.InitializeComponent();
      this.IsShowGLWindow = true;
    }

    internal void AddControl(IDimOverlayControl el)
    {
      if (el.ShowControlInSeparateWindow)
        return;
      if (!this.mGrid.Children.Contains(el as UIElement))
        this.mGrid.Children.Add(el as UIElement);
      else
        (el as UIElement).Visibility = Visibility.Visible;
    }

    internal void RemoveControl()
    {
      if (!this.Control.ShowControlInSeparateWindow)
      {
        if (!this.mGrid.Children.Contains(this.Control as UIElement))
          return;
        this.mGrid.Children.Remove(this.Control as UIElement);
        this.Control.Close();
      }
      else
      {
        if (this.Control != null)
        {
          BlueStacksUIUtils.RemoveChildFromParent((UIElement) this.Control);
          this.Control.Close();
        }
        if (this.cw == null)
          return;
        this.cw.Close();
      }
    }

    internal void UpadteSizeLocation()
    {
      if (this.Owner == null || PresentationSource.FromVisual((Visual) this.Owner) == null)
        return;
      Point point = PresentationSource.FromVisual((Visual) this.Owner).CompositionTarget.TransformFromDevice.Transform(this.Owner.PointToScreen(new Point(0.0, 0.0)));
      this.Left = point.X;
      this.Top = point.Y;
      this.Width = this.Owner.ActualWidth;
      this.Height = this.Owner.ActualHeight;
    }

    internal void ShowWindow()
    {
      if (this.IsWindowVisible)
        return;
      this.IsWindowVisible = true;
      this.Show();
      if (this.Control == null)
        return;
      if (!this.Control.ShowControlInSeparateWindow)
      {
        this.Control.Show();
      }
      else
      {
        this.Control.Show();
        this.cw = new ContainerWindow(this.Owner, (UserControl) this.Control, this.Control.Width, this.Control.Height, false, false, this.Control.ShowTransparentWindow, -1.0, (Brush) null);
        this.cw.Show();
      }
    }

    internal void HideWindow(bool isFromOverlayClick)
    {
      if (!this.IsWindowVisible)
        return;
      if (this.Control != null)
      {
        if (isFromOverlayClick)
        {
          if (this.Control.IsCloseOnOverLayClick)
          {
            this.IsWindowVisible = false;
            this.RemoveControl();
            this.Hide();
          }
          else
          {
            if (this.cw == null)
              return;
            this.cw.Focus();
          }
        }
        else
        {
          this.IsWindowVisible = false;
          this.RemoveControl();
          this.Hide();
        }
      }
      else
      {
        this.IsWindowVisible = false;
        this.Hide();
      }
    }

    public new void Hide()
    {
      if (this.IsWindowVisible)
        return;
      this.Dispatcher.Invoke((Delegate) (() => base.Hide()));
    }

    private void DimWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.System || e.SystemKey != Key.F4 || (this.Control == null || !FeatureManager.Instance.IsCustomUIForNCSoft) || ((object) this.Control.GetType() != (object) BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance.GetType() || !BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance.IsVisible))
        return;
      e.Handled = true;
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.HideWindow(true);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/dimoverlaycontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).KeyDown += new KeyEventHandler(this.DimWindow_KeyDown);
          break;
        case 2:
          this.mGrid = (Grid) target;
          break;
        case 3:
          ((UIElement) target).MouseUp += new MouseButtonEventHandler(this.Grid_MouseUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
