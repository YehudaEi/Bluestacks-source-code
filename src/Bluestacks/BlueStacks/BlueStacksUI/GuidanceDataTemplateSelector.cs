// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceDataTemplateSelector
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System.Windows;
using System.Windows.Controls;

namespace BlueStacks.BlueStacksUI
{
  public class GuidanceDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (container is FrameworkElement frameworkElement)
      {
        switch (item)
        {
          case GuidanceViewModel _:
            return frameworkElement.FindResource((object) "GuidanceViewModelTemplate") as DataTemplate;
          case GuidanceEditTextModel _:
            return frameworkElement.FindResource((object) "GuidanceEditTextModelTemplate") as DataTemplate;
          case GuidanceEditDecimalModel _:
            return frameworkElement.FindResource((object) "GuidanceEditDecimalModelTemplate") as DataTemplate;
          case GuidanceCategoryViewModel _:
            return frameworkElement.FindResource((object) "GuidanceCategoryViewModelTemplate") as DataTemplate;
          case GuidanceCategoryEditModel _:
            return frameworkElement.FindResource((object) "GuidanceCategoryEditModelTemplate") as DataTemplate;
        }
      }
      return (DataTemplate) null;
    }
  }
}
