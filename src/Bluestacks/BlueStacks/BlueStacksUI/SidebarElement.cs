// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SidebarElement
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
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class SidebarElement : UserControl, IComponentConnector
  {
    private bool mRedDotVisibility;
    internal SidebarElement mSidebarElement;
    internal Border mBorder;
    internal Grid mGrid;
    internal CustomPictureBox mImage;
    internal Ellipse mElementNotification;
    private bool _contentLoaded;

    public CustomPictureBox Image
    {
      get
      {
        return this.mImage;
      }
    }

    public bool IsLastElementOfGroup { get; set; }

    public bool IsCurrentLastElementOfGroup { get; set; }

    public bool IsInMainSidebar { get; set; } = true;

    public string mSidebarElementTooltipKey { get; set; } = string.Empty;

    public bool RedDotVisibility
    {
      get
      {
        return this.mRedDotVisibility;
      }
      set
      {
        this.mRedDotVisibility = value;
        this.mElementNotification.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public SidebarElement()
    {
      this.InitializeComponent();
    }

    private void SidebarElement_Loaded(object sender, RoutedEventArgs e)
    {
      this.SetColor(false);
    }

    private void MImage_Loaded(object sender, RoutedEventArgs e)
    {
      this.mImage = sender as CustomPictureBox;
    }

    private void SetColor(bool isPressed = false)
    {
      if (isPressed)
      {
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BorderBrushProperty, "SidebarElementClick");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BackgroundProperty, "SidebarElementClick");
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.ForegroundProperty, "SidebarElementClick");
      }
      else if (this.IsMouseOver)
      {
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BorderBrushProperty, "SidebarElementHover");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BackgroundProperty, "SidebarElementHover");
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.ForegroundProperty, "SidebarElementHover");
      }
      else
      {
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BorderBrushProperty, "SidebarElementNormal");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Control.BackgroundProperty, "SidebarElementNormal");
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.ForegroundProperty, "SidebarElementNormal");
      }
    }

    private void SidebarElement_MouseEnter(object sender, MouseEventArgs e)
    {
      this.SetColor(false);
    }

    private void SidebarElement_MouseLeave(object sender, MouseEventArgs e)
    {
      this.SetColor(false);
    }

    private void SidebarElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.SetColor(true);
    }

    private void SidebarElement_IsEnabledChanged(object _, DependencyPropertyChangedEventArgs e)
    {
      this.Opacity = (bool) e.NewValue ? 1.0 : 0.5;
    }

    private void SidebarElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.SetColor(false);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/sidebarelement.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mSidebarElement = (SidebarElement) target;
          this.mSidebarElement.MouseEnter += new MouseEventHandler(this.SidebarElement_MouseEnter);
          this.mSidebarElement.MouseLeave += new MouseEventHandler(this.SidebarElement_MouseLeave);
          this.mSidebarElement.PreviewMouseDown += new MouseButtonEventHandler(this.SidebarElement_PreviewMouseDown);
          this.mSidebarElement.PreviewMouseUp += new MouseButtonEventHandler(this.SidebarElement_PreviewMouseUp);
          this.mSidebarElement.Loaded += new RoutedEventHandler(this.SidebarElement_Loaded);
          this.mSidebarElement.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.SidebarElement_IsEnabledChanged);
          break;
        case 2:
          this.mBorder = (Border) target;
          break;
        case 3:
          this.mGrid = (Grid) target;
          break;
        case 4:
          this.mImage = (CustomPictureBox) target;
          this.mImage.Loaded += new RoutedEventHandler(this.MImage_Loaded);
          break;
        case 5:
          this.mElementNotification = (Ellipse) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
