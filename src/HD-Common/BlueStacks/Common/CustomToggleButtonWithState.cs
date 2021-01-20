// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomToggleButtonWithState
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CustomToggleButtonWithState : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof (ImageWidth), typeof (double), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 32.0));
    public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof (ImageHeight), typeof (double), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 16.0));
    public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register(nameof (LabelVisibility), typeof (Visibility), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible));
    public static readonly DependencyProperty BoolValueProperty = DependencyProperty.Register(nameof (BoolValue), typeof (bool), typeof (CustomToggleButtonWithState), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsShowTextProperty = DependencyProperty.Register(nameof (IsShowTextProperty), typeof (bool), typeof (CustomToggleButtonWithState), new PropertyMetadata((object) true));
    internal CustomToggleButtonWithState mCustomToggleButton;
    private bool _contentLoaded;

    public CustomToggleButtonWithState()
    {
      this.InitializeComponent();
    }

    public double ImageWidth
    {
      get
      {
        return (double) this.GetValue(CustomToggleButtonWithState.ImageWidthProperty);
      }
      set
      {
        this.SetValue(CustomToggleButtonWithState.ImageWidthProperty, (object) value);
      }
    }

    public double ImageHeight
    {
      get
      {
        return (double) this.GetValue(CustomToggleButtonWithState.ImageHeightProperty);
      }
      set
      {
        this.SetValue(CustomToggleButtonWithState.ImageHeightProperty, (object) value);
      }
    }

    public Visibility LabelVisibility
    {
      get
      {
        return (Visibility) this.GetValue(CustomToggleButtonWithState.LabelVisibilityProperty);
      }
      set
      {
        this.SetValue(CustomToggleButtonWithState.LabelVisibilityProperty, (object) value);
      }
    }

    public bool BoolValue
    {
      get
      {
        return (bool) this.GetValue(CustomToggleButtonWithState.BoolValueProperty);
      }
      set
      {
        this.SetValue(CustomToggleButtonWithState.BoolValueProperty, (object) value);
      }
    }

    public bool IsShowText
    {
      get
      {
        return (bool) this.GetValue(CustomToggleButtonWithState.IsShowTextProperty);
      }
      set
      {
        this.SetValue(CustomToggleButtonWithState.IsShowTextProperty, (object) value);
      }
    }

    private void mToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.BoolValue = !this.BoolValue;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customtogglebuttonwithstate.xaml", UriKind.Relative));
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
        this.mCustomToggleButton = (CustomToggleButtonWithState) target;
      else
        this._contentLoaded = true;
    }
  }
}
