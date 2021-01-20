// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BindingProxy
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Windows;

namespace BlueStacks.Common
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
