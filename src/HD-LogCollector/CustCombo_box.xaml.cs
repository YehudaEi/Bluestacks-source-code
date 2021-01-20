// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.CustCombo_box
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.LogCollector
{
  public partial class CustCombo_box : ComboBox, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty BorderBrushPropertyName = DependencyProperty.Register(nameof (BorderBrushName), typeof (SolidColorBrush), typeof (CustCombo_box), (PropertyMetadata) new UIPropertyMetadata((object) (SolidColorBrush) new BrushConverter().ConvertFrom((object) LogCollectorUtils.sDefaultBorderColorHex)));
    internal CustCombo_box _this;
    private bool _contentLoaded;

    public CustCombo_box()
    {
      this.InitializeComponent();
    }

    private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
        return;
      e.Handled = true;
    }

    public SolidColorBrush BorderBrushName
    {
      get
      {
        return (SolidColorBrush) this.GetValue(CustCombo_box.BorderBrushPropertyName);
      }
      set
      {
        this.SetValue(CustCombo_box.BorderBrushPropertyName, (object) value);
      }
    }

    internal void ChangeBorderBrushColor(string hex)
    {
      this.BorderBrushName = (SolidColorBrush) new BrushConverter().ConvertFrom((object) hex);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-LogCollector;component/custcombo_box.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this._this = (CustCombo_box) target;
      else
        this._contentLoaded = true;
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
        Event = FrameworkElement.RequestBringIntoViewEvent,
        Handler = (Delegate) new RequestBringIntoViewEventHandler(this.OnRequestBringIntoView)
      });
    }
  }
}
