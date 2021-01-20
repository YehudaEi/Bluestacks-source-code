// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.CrossButton
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;
using System.Windows.Controls;

namespace BlueStacks.Core
{
  public class CrossButton : Button
  {
    static CrossButton()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CrossButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CrossButton)));
    }
  }
}
