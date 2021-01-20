// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.BindingProxy
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;

namespace BlueStacks.Core
{
  public class BindingProxy : Freezable
  {
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof (Data), typeof (object), typeof (BindingProxy), new PropertyMetadata((PropertyChangedCallback) null));

    protected override Freezable CreateInstanceCore()
    {
      return (Freezable) new BindingProxy();
    }

    public object Data
    {
      get
      {
        return this.GetValue(BindingProxy.DataProperty);
      }
      set
      {
        this.SetValue(BindingProxy.DataProperty, value);
      }
    }
  }
}
