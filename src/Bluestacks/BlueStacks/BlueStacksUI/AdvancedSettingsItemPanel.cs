// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AdvancedSettingsItemPanel
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
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BlueStacks.BlueStacksUI
{
  public class AdvancedSettingsItemPanel : UserControl, IComponentConnector
  {
    private EventHandler mTap;
    private EventHandler mMouseDragStart;
    private KeyActionType mActionType;
    private Point? mousePressedPosition;
    private Point? mouseReleasedPosition;
    internal Border mBorder;
    internal CustomPictureBox mDragImage;
    internal CustomPictureBox mImage;
    internal TextBlock mActionHeader;
    private bool _contentLoaded;

    public EventHandler Tap
    {
      get
      {
        return this.mTap;
      }
      set
      {
        this.mTap = value;
      }
    }

    public EventHandler MouseDragStart
    {
      get
      {
        return this.mMouseDragStart;
      }
      set
      {
        this.mMouseDragStart = value;
      }
    }

    public KeyActionType ActionType
    {
      get
      {
        return this.mActionType;
      }
      set
      {
        this.mActionType = value;
        this.mImage.ImageName = this.mActionType.ToString() + "_sidebar";
        BlueStacksUIBinding.Bind(this.mActionHeader, Constants.ImapLocaleStringsConstant + this.mActionType.ToString() + "_Header_Edit_UI", "");
      }
    }

    public AdvancedSettingsItemPanel()
    {
      this.InitializeComponent();
    }

    private void Image_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
      this.mDragImage.Visibility = Visibility.Visible;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BorderBrushProperty, "AdvancedGameControlHeaderBackgroundColor");
      this.mBorder.Effect = (Effect) new DropShadowEffect()
      {
        Direction = 270.0,
        ShadowDepth = 3.0,
        BlurRadius = 12.0,
        Opacity = 0.75,
        Color = ((SolidColorBrush) this.mBorder.Background).Color
      };
    }

    private void Image_MouseLeave(object sender, MouseEventArgs e)
    {
      if (KMManager.sDragCanvasElement == null)
        this.Cursor = Cursors.Arrow;
      this.mDragImage.Visibility = Visibility.Hidden;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BorderBrushProperty, "AdvancedSettingsItemPanelBorder");
      this.mBorder.Effect = (Effect) null;
    }

    private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mousePressedPosition = new Point?(e.GetPosition((IInputElement) this));
    }

    private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mouseReleasedPosition = new Point?(e.GetPosition((IInputElement) this));
      if (this.mousePressedPosition.Equals((object) this.mouseReleasedPosition))
      {
        EventHandler tap = this.Tap;
        if (tap != null)
          tap((object) this, (EventArgs) null);
      }
      else
        KMManager.ClearElement();
      this.ReatchedMouseMove();
    }

    private void OnTimedElapsed(object sender, ElapsedEventArgs e)
    {
      if (this.mousePressedPosition.Equals((object) this.mouseReleasedPosition))
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        EventHandler mouseDragStart = this.MouseDragStart;
        if (mouseDragStart == null)
          return;
        mouseDragStart((object) this, (EventArgs) null);
      }));
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
      if (!this.mousePressedPosition.HasValue || this.mousePressedPosition.Equals((object) this.mouseReleasedPosition))
        return;
      this.MouseMove -= new MouseEventHandler(this.Image_MouseMove);
      EventHandler mouseDragStart = this.MouseDragStart;
      if (mouseDragStart == null)
        return;
      mouseDragStart((object) this, (EventArgs) null);
    }

    public void ReatchedMouseMove()
    {
      this.mousePressedPosition = new Point?();
      this.MouseMove -= new MouseEventHandler(this.Image_MouseMove);
      this.MouseMove += new MouseEventHandler(this.Image_MouseMove);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/advancedsettingsitempanel.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Image_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Image_MouseLeave);
          ((UIElement) target).PreviewMouseDown += new MouseButtonEventHandler(this.Image_PreviewMouseDown);
          ((UIElement) target).MouseMove += new MouseEventHandler(this.Image_MouseMove);
          ((UIElement) target).PreviewMouseUp += new MouseButtonEventHandler(this.Image_PreviewMouseUp);
          break;
        case 2:
          this.mBorder = (Border) target;
          break;
        case 3:
          this.mDragImage = (CustomPictureBox) target;
          break;
        case 4:
          this.mImage = (CustomPictureBox) target;
          break;
        case 5:
          this.mActionHeader = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
