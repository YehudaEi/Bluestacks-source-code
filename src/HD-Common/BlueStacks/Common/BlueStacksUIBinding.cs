// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BlueStacksUIBinding
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
  public class BlueStacksUIBinding : INotifyPropertyChanged
  {
    private static BlueStacksUIBinding _Instance;

    public static BlueStacksUIBinding Instance
    {
      get
      {
        if (BlueStacksUIBinding._Instance == null)
          BlueStacksUIBinding._Instance = new BlueStacksUIBinding();
        return BlueStacksUIBinding._Instance;
      }
      set
      {
        BlueStacksUIBinding._Instance = value;
      }
    }

    private BlueStacksUIBinding()
    {
      LocaleStrings.SourceUpdatedEvent += new EventHandler(this.Locale_Updated);
      BluestacksUIColor.SourceUpdatedEvent += new EventHandler(this.BluestacksUIColor_Updated);
      CustomPictureBox.SourceUpdatedEvent += new EventHandler(this.BluestacksImage_Updated);
    }

    public void Locale_Updated(object sender, EventArgs e)
    {
      this.NotifyPropertyChanged("LocaleModel");
    }

    public void BluestacksUIColor_Updated(object sender, EventArgs e)
    {
      this.NotifyPropertyChanged("ColorModel");
      this.NotifyPropertyChanged("GeometryModel");
      this.NotifyPropertyChanged("CornerRadiusModel");
      this.NotifyPropertyChanged("TransformModel");
    }

    public void BluestacksImage_Updated(object sender, EventArgs e)
    {
      this.NotifyPropertyChanged("ImageModel");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public Dictionary<string, Brush> ColorModel
    {
      get
      {
        return (Dictionary<string, Brush>) BlueStacksUIColorManager.AppliedTheme.DictBrush;
      }
      set
      {
      }
    }

    public Dictionary<string, Geometry> GeometryModel
    {
      get
      {
        return (Dictionary<string, Geometry>) BlueStacksUIColorManager.AppliedTheme.DictGeometry;
      }
      set
      {
      }
    }

    public Dictionary<string, CornerRadius> CornerRadiusModel
    {
      get
      {
        return (Dictionary<string, CornerRadius>) BlueStacksUIColorManager.AppliedTheme.DictCornerRadius;
      }
      set
      {
      }
    }

    public Dictionary<string, Transform> TransformModel
    {
      get
      {
        return (Dictionary<string, Transform>) BlueStacksUIColorManager.AppliedTheme.DictTransform;
      }
      set
      {
      }
    }

    public Dictionary<string, string> LocaleModel
    {
      get
      {
        return LocaleStrings.DictLocalizedString;
      }
      set
      {
      }
    }

    public Dictionary<string, Tuple<BitmapImage, bool>> ImageModel
    {
      get
      {
        return CustomPictureBox.sImageAssetsDict;
      }
      set
      {
      }
    }

    public void NotifyPropertyChanged(string name)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    public static void Bind(UserControl uc, string path)
    {
      BindingOperations.SetBinding((DependencyObject) uc, FrameworkElement.ToolTipProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(CustomRadioButton tb, string path)
    {
      BindingOperations.SetBinding((DependencyObject) tb, ContentControl.ContentProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(ToggleButton tb, string path)
    {
      BindingOperations.SetBinding((DependencyObject) tb, ContentControl.ContentProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(GroupBox gb, string path)
    {
      BindingOperations.SetBinding((DependencyObject) gb, HeaderedContentControl.HeaderProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(Label label, string path)
    {
      BindingOperations.SetBinding((DependencyObject) label, ContentControl.ContentProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(Run run, string path)
    {
      BindingOperations.SetBinding((DependencyObject) run, TextBlock.TextProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(Window wind, string path)
    {
      BindingOperations.SetBinding((DependencyObject) wind, Window.TitleProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(TextBlock tb, string path, string stringFormat = "")
    {
      Binding localeBinding = BlueStacksUIBinding.GetLocaleBinding(path);
      if (!string.IsNullOrEmpty(stringFormat))
        localeBinding.StringFormat = stringFormat;
      BindingOperations.SetBinding((DependencyObject) tb, TextBlock.TextProperty, (BindingBase) localeBinding);
    }

    public static void Bind(DependencyObject icon, string path, DependencyProperty dp)
    {
      BindingOperations.SetBinding(icon, dp, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void ClearBind(DependencyObject icon, DependencyProperty dp)
    {
      BindingOperations.ClearBinding(icon, dp);
    }

    public static void Bind(ComboBoxItem comboBoxItem, string path)
    {
      BindingOperations.SetBinding((DependencyObject) comboBoxItem, ContentControl.ContentProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(Button button, string path)
    {
      BindingOperations.SetBinding((DependencyObject) button, ContentControl.ContentProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(Image button, string path)
    {
      BindingOperations.SetBinding((DependencyObject) button, FrameworkElement.ToolTipProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static void Bind(TextBox textBox, string path)
    {
      BindingOperations.SetBinding((DependencyObject) textBox, TextBox.TextProperty, (BindingBase) BlueStacksUIBinding.GetLocaleBinding(path));
    }

    public static Binding GetLocaleBinding(string path)
    {
      Binding binding = new Binding()
      {
        Source = (object) BlueStacksUIBinding.Instance
      };
      string str = "";
      if (path != null)
      {
        foreach (char ch in path)
          str = str + "^" + ch.ToString();
      }
      binding.Path = new PropertyPath("Instance.LocaleModel.[" + str.ToUpper(CultureInfo.InvariantCulture) + "]", new object[0]);
      binding.Mode = BindingMode.OneWay;
      binding.FallbackValue = (object) LocaleStrings.RemoveConstants(path);
      binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
      LocaleStrings.AppendLocaleIfDoesntExist(path, LocaleStrings.RemoveConstants(path));
      return binding;
    }

    public static void BindColor(DependencyObject dObj, DependencyProperty dp, string path)
    {
      BindingOperations.SetBinding(dObj, dp, (BindingBase) BlueStacksUIBinding.GetColorBinding(path));
    }

    public static void BindCornerRadius(DependencyObject dObj, DependencyProperty dp, string path)
    {
      BindingOperations.SetBinding(dObj, dp, (BindingBase) BlueStacksUIBinding.GetCornerRadiusBinding(path));
    }

    public static void BindCornerRadiusToDouble(
      DependencyObject dObj,
      DependencyProperty dp,
      string path)
    {
      BindingOperations.SetBinding(dObj, dp, (BindingBase) BlueStacksUIBinding.GetCornerRadiusDoubleBinding(path));
    }

    public static Binding GetColorBinding(string path)
    {
      return new Binding()
      {
        Converter = (IValueConverter) new BrushToColorConvertor(),
        Source = (object) BlueStacksUIBinding.Instance,
        Path = new PropertyPath("Instance.ColorModel.[" + path + "]", new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
    }

    public static Binding GetCornerRadiusBinding(string path)
    {
      return new Binding()
      {
        Converter = (IValueConverter) new CornerRadiusToThicknessConvertor(),
        Source = (object) BlueStacksUIBinding.Instance,
        Path = new PropertyPath("Instance.CornerRadiusModel.[" + path + "]", new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
    }

    public static Binding GetCornerRadiusDoubleBinding(string path)
    {
      return new Binding()
      {
        Converter = (IValueConverter) new CornerRadiusToDoubleConvertor(),
        Source = (object) BlueStacksUIBinding.Instance,
        Path = new PropertyPath("Instance.CornerRadiusModel.[" + path + "]", new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
    }

    public static void Bind(Image button, DependencyProperty dp, string path)
    {
      BindingOperations.SetBinding((DependencyObject) button, dp, (BindingBase) BlueStacksUIBinding.GetImageBinding(path));
    }

    public static Binding GetImageBinding(string path)
    {
      return new Binding()
      {
        Source = (object) BlueStacksUIBinding.Instance,
        Path = new PropertyPath("Instance.ImageModel.[" + path + "].Item1", new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
    }

    public static void BindTransform(DependencyObject button, DependencyProperty dp, string path)
    {
      BindingOperations.SetBinding(button, dp, (BindingBase) BlueStacksUIBinding.GetTransformBinding(path));
    }

    public static Binding GetTransformBinding(string path)
    {
      return new Binding()
      {
        Source = (object) BlueStacksUIBinding.Instance,
        Path = new PropertyPath("Instance.TransformModel.[" + path + "]", new object[0]),
        Mode = BindingMode.OneWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
    }
  }
}
