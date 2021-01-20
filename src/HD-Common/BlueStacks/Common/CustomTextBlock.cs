// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomTextBlock
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CustomTextBlock : TextBlock, IComponentConnector
  {
    public static readonly DependencyProperty SetToolTipProperty = DependencyProperty.Register(nameof (SetToolTip), typeof (bool), typeof (CustomTextBlock), new PropertyMetadata((object) false, new PropertyChangedCallback(CustomTextBlock.OnSetToolTipChanged)));
    public static readonly DependencyProperty MouseOverForegroundChangedProperty = DependencyProperty.RegisterAttached(nameof (HoverForegroundProperty), typeof (bool), typeof (CustomTextBlock), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ForcedTooltipProperty = DependencyProperty.Register(nameof (ForcedTooltip), typeof (bool), typeof (CustomButton), new PropertyMetadata((object) false));
    private bool _contentLoaded;

    public CustomTextBlock()
    {
      this.InitializeComponent();
    }

    public bool ForcedTooltip
    {
      get
      {
        return (bool) this.GetValue(CustomTextBlock.ForcedTooltipProperty);
      }
      set
      {
        this.SetValue(CustomTextBlock.ForcedTooltipProperty, (object) value);
      }
    }

    public bool SetToolTip
    {
      get
      {
        return (bool) this.GetValue(CustomTextBlock.SetToolTipProperty);
      }
      set
      {
        this.SetValue(CustomTextBlock.SetToolTipProperty, (object) value);
      }
    }

    public bool HoverForegroundProperty
    {
      get
      {
        return (bool) this.GetValue(CustomTextBlock.SetToolTipProperty);
      }
      set
      {
        this.SetValue(CustomTextBlock.SetToolTipProperty, (object) value);
      }
    }

    private static void OnSetToolTipChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as CustomTextBlock).OnSetToolTipChanged(e);
    }

    private void OnSetToolTipChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.ForcedTooltip)
        return;
      bool result;
      if (bool.TryParse(args.NewValue.ToString(), out result) & result && this.IsTextTrimmed())
      {
        ToolTipService.SetIsEnabled((DependencyObject) this, true);
        if (this.ToolTip != null)
          return;
        this.ToolTip = (object) this.Text;
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
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customtextblock.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
