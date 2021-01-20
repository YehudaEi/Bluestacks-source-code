// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SideBarImageNameToLocalizedTooltipMultiConverter
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.BlueStacksUI
{
  public class SideBarImageNameToLocalizedTooltipMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      string key = values == null || values[0] == null ? string.Empty : values[0].ToString();
      return (object) LocaleStrings.GetLocalizedString(Sidebar.mSidebarIconDict.ContainsKey(key) ? Sidebar.mSidebarIconDict[key] : key, "");
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      return (object[]) null;
    }
  }
}
