// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.WpfUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.Windows;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public static class WpfUtils
  {
    public static double PrimaryWidth
    {
      get
      {
        return SystemParameters.PrimaryScreenWidth;
      }
    }

    public static double PrimaryHeight
    {
      get
      {
        return SystemParameters.PrimaryScreenHeight;
      }
    }

    public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
        if (child != null && child is T obj1)
          return obj1;
        T visualChild = WpfUtils.FindVisualChild<T>(child);
        if ((object) visualChild != null)
          return visualChild;
      }
      return default (T);
    }

    public static void GetDefaultSize(
      out double width,
      out double height,
      out double left,
      double aspectRatio,
      bool isGMWindow)
    {
      int num = WpfUtils.PrimaryWidth * 0.8 / aspectRatio > WpfUtils.PrimaryHeight * 0.8 ? (int) (WpfUtils.PrimaryHeight * 0.8 * aspectRatio) : (int) (WpfUtils.PrimaryWidth * 0.8);
      if (!isGMWindow)
      {
        width = (double) (num / 4 * 3);
        left = (double) ((int) (WpfUtils.PrimaryWidth - (double) num) / 2);
      }
      else
      {
        width = (double) num;
        left = (double) ((int) (WpfUtils.PrimaryWidth - (double) num) / 2);
      }
      if (width < 912.0)
      {
        width = 912.0;
        left = 20.0;
      }
      height = (double) ((int) width / 16 * 9);
    }

    public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(child);
      if (parent == null)
        return default (T);
      return parent is T obj ? obj : WpfUtils.FindVisualParent<T>(parent);
    }
  }
}
