// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.Wrapper
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;

namespace BlueStacks.Core
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
