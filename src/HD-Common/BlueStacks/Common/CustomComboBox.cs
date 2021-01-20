// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomComboBox
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
  public class CustomComboBox : ComboBox, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register(nameof (ToolTipText), typeof (string), typeof (CustomComboBox), new PropertyMetadata((object) string.Empty));
    public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register(nameof (Highlight), typeof (bool), typeof (CustomComboBox), new PropertyMetadata((object) false));
    public static readonly DependencyProperty SetToolTipProperty = DependencyProperty.Register(nameof (SetToolTip), typeof (bool), typeof (CustomComboBox), new PropertyMetadata((object) false, new PropertyChangedCallback(CustomComboBox.OnSetToolTipChanged)));
    public static readonly DependencyProperty ToolTipWhenTrimmedProperty = DependencyProperty.Register(nameof (ToolTipWhenTrimmed), typeof (bool), typeof (CustomComboBox), new PropertyMetadata((object) false));
    private bool _contentLoaded;

    public CustomComboBox()
    {
      this.InitializeComponent();
    }

    private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
        return;
      e.Handled = true;
    }

    public bool Highlight
    {
      get
      {
        return (bool) this.GetValue(CustomComboBox.HighlightProperty);
      }
      set
      {
        this.SetValue(CustomComboBox.HighlightProperty, (object) value);
      }
    }

    public string ToolTipText
    {
      get
      {
        return (string) this.GetValue(CustomComboBox.ToolTipTextProperty);
      }
      set
      {
        this.SetValue(CustomComboBox.ToolTipTextProperty, (object) value);
      }
    }

    public bool SetToolTip
    {
      get
      {
        return (bool) this.GetValue(CustomComboBox.SetToolTipProperty);
      }
      set
      {
        this.SetValue(CustomComboBox.SetToolTipProperty, (object) value);
      }
    }

    public bool ToolTipWhenTrimmed
    {
      get
      {
        return (bool) this.GetValue(CustomComboBox.ToolTipWhenTrimmedProperty);
      }
      set
      {
        this.SetValue(CustomComboBox.ToolTipWhenTrimmedProperty, (object) value);
      }
    }

    private static void OnSetToolTipChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      CustomComboBox customComboBox = d as CustomComboBox;
      if (!customComboBox.ToolTipWhenTrimmed)
        return;
      customComboBox.OnSetToolTipChanged(e);
    }

    private void OnSetToolTipChanged(DependencyPropertyChangedEventArgs args)
    {
      bool result;
      if (bool.TryParse(args.NewValue.ToString(), out result) & result && this.IsTextTrimmed(this.ToolTipText))
      {
        ToolTipService.SetIsEnabled((DependencyObject) this, true);
        this.ToolTip = (object) this.ToolTipText;
      }
      else
        ToolTipService.SetIsEnabled((DependencyObject) this, false);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customcombobox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
        return;
      ((Style) target).Setters.Add((SetterBase) new EventSetter()
      {
        Event = FrameworkElement.RequestBringIntoViewEvent,
        Handler = (Delegate) new RequestBringIntoViewEventHandler(this.OnRequestBringIntoView)
      });
    }
  }
}
