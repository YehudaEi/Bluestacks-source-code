// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Wrapper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Windows;

namespace BlueStacks.Common
{
  public class Wrapper : DependencyObject
  {
    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(nameof (ErrorMessage), typeof (string), typeof (Wrapper), (PropertyMetadata) new FrameworkPropertyMetadata((object) ""));
    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof (Min), typeof (int), typeof (Wrapper), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof (Max), typeof (int), typeof (Wrapper), (PropertyMetadata) new FrameworkPropertyMetadata((object) int.MaxValue));

    public string ErrorMessage
    {
      get
      {
        return (string) this.GetValue(Wrapper.ErrorMessageProperty);
      }
      set
      {
        this.SetValue(Wrapper.ErrorMessageProperty, (object) value);
      }
    }

    public int Min
    {
      get
      {
        return (int) this.GetValue(Wrapper.MinProperty);
      }
      set
      {
        this.SetValue(Wrapper.MinProperty, (object) value);
      }
    }

    public int Max
    {
      get
      {
        return (int) this.GetValue(Wrapper.MaxProperty);
      }
      set
      {
        this.SetValue(Wrapper.MaxProperty, (object) value);
      }
    }
  }
}
