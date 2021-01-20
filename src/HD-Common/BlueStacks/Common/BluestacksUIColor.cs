// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BluestacksUIColor
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
  public class BluestacksUIColor
  {
    public static readonly LinearGradientBrush mScrolledOpacityMask = new LinearGradientBrush(new GradientStopCollection((IEnumerable<GradientStop>) new List<GradientStop>()
    {
      new GradientStop(Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0), 0.0),
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 0.15),
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 0.8),
      new GradientStop(Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0), 1.0)
    }), new Point(0.0, 0.0), new Point(0.0, 1.0));
    public static readonly LinearGradientBrush mTopOpacityMask = new LinearGradientBrush(new GradientStopCollection((IEnumerable<GradientStop>) new List<GradientStop>()
    {
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 0.0),
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 0.8),
      new GradientStop(Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0), 1.0)
    }), new Point(0.0, 0.0), new Point(0.0, 1.0));
    public static readonly LinearGradientBrush mBottomOpacityMask = new LinearGradientBrush(new GradientStopCollection((IEnumerable<GradientStop>) new List<GradientStop>()
    {
      new GradientStop(Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0), 0.0),
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 0.15),
      new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0), 1.0)
    }), new Point(0.0, 0.0), new Point(0.0, 1.0));
    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public const string ThemeFileName = "ThemeFile";

    internal static string ThemeFilePath
    {
      get
      {
        return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.ClientThemeName), "ThemeFile");
      }
    }

    public SerializableDictionary<string, string> DictCategory { get; set; }

    public SerializableDictionary<string, Brush> DictBrush { get; set; }

    public SerializableDictionary<string, Geometry> DictGeometry { get; set; }

    public SerializableDictionary<string, Transform> DictTransform { get; set; }

    public SerializableDictionary<string, CornerRadius> DictCornerRadius { get; set; }

    public SerializableDictionary<string, string> DictThemeAvailable { get; set; }

    public BluestacksUIColor()
    {
      this.DictCategory = new SerializableDictionary<string, string>();
      this.DictBrush = new SerializableDictionary<string, Brush>();
      this.DictGeometry = new SerializableDictionary<string, Geometry>();
      this.DictTransform = new SerializableDictionary<string, Transform>();
      this.DictCornerRadius = new SerializableDictionary<string, CornerRadius>();
      this.DictThemeAvailable = new SerializableDictionary<string, string>();
    }

    internal static event EventHandler SourceUpdatedEvent;

    internal static BluestacksUIColor Load(string themePath)
    {
      BluestacksUIColor bluestacksUiColor = new BluestacksUIColor();
      try
      {
        if (!File.Exists(themePath))
          throw new Exception("Theme file not found exception");
        string inputUri = themePath;
        using (XmlReader reader = XmlReader.Create(inputUri, new XmlReaderSettings()
        {
          XmlResolver = (XmlResolver) null
        }))
        {
          bluestacksUiColor = (BluestacksUIColor) XamlReader.Load(reader);
          if (bluestacksUiColor.AddNewParameters())
            bluestacksUiColor.Save(themePath);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error loading Theme file from path : " + themePath + ex.ToString());
        if (string.Compare(BlueStacksUIColorManager.GetThemeFilePath("Assets"), themePath, StringComparison.OrdinalIgnoreCase) == 0)
          bluestacksUiColor.InitalizeDefault();
      }
      return bluestacksUiColor;
    }

    private bool AddNewParameters()
    {
      BluestacksUIColor bluestacksUiColor = new BluestacksUIColor();
      bluestacksUiColor.InitalizeDefault();
      bool flag = false;
      foreach (KeyValuePair<string, Brush> keyValuePair in (Dictionary<string, Brush>) bluestacksUiColor.DictBrush)
      {
        if (!this.DictBrush.ContainsKey(keyValuePair.Key))
        {
          flag = true;
          this.DictBrush.Add(keyValuePair.Key, keyValuePair.Value);
          if (!this.DictCategory.ContainsKey(keyValuePair.Key))
            this.DictCategory.Add(keyValuePair.Key, "*New Color*");
        }
      }
      foreach (KeyValuePair<string, CornerRadius> dictCornerRadiu in (Dictionary<string, CornerRadius>) bluestacksUiColor.DictCornerRadius)
      {
        if (!this.DictCornerRadius.ContainsKey(dictCornerRadiu.Key))
        {
          flag = true;
          this.DictCornerRadius.Add(dictCornerRadiu.Key, dictCornerRadiu.Value);
        }
      }
      foreach (KeyValuePair<string, Geometry> keyValuePair in (Dictionary<string, Geometry>) bluestacksUiColor.DictGeometry)
      {
        if (!this.DictGeometry.ContainsKey(keyValuePair.Key))
        {
          flag = true;
          this.DictGeometry.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      foreach (KeyValuePair<string, Transform> keyValuePair in (Dictionary<string, Transform>) bluestacksUiColor.DictTransform)
      {
        if (!this.DictTransform.ContainsKey(keyValuePair.Key))
        {
          flag = true;
          this.DictTransform.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      try
      {
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) bluestacksUiColor.DictThemeAvailable)
        {
          if (!this.DictThemeAvailable.ContainsKey(keyValuePair.Key))
          {
            flag = true;
            this.DictThemeAvailable.Add(keyValuePair.Key, keyValuePair.Value);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in adding theme availability:" + ex.ToString());
        flag = false;
      }
      return flag;
    }

    public void NotifyUIElements()
    {
      if (BluestacksUIColor.SourceUpdatedEvent == null)
        return;
      BluestacksUIColor.SourceUpdatedEvent((object) this, (EventArgs) null);
    }

    public void Save(string saveFilePath)
    {
      try
      {
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          Indent = true,
          NewLineOnAttributes = true,
          ConformanceLevel = ConformanceLevel.Fragment
        };
        StringBuilder output = new StringBuilder();
        using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
        {
          XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(xmlWriter)
          {
            XamlWriterMode = XamlWriterMode.Expression
          };
          if (this.DictCategory.Count == 0)
            this.DictCategory.Add(string.Empty, string.Empty);
          XamlWriter.Save((object) this, manager);
          if (string.IsNullOrEmpty(saveFilePath))
            File.WriteAllText(BluestacksUIColor.ThemeFilePath, output.ToString());
          else
            File.WriteAllText(saveFilePath, output.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error Saving Theme file " + ex.ToString());
      }
    }

    private Color GetMainColor(string colorTheme)
    {
      ColorUtils mainThemeColor = this.MainThemeColor;
      ColorUtils mainColorTheme = new ColorUtils((Color) ColorConverter.ConvertFromString("#1E2138"));
      Tuple<double, double, double, double> unitColor = BluestacksUIColor.GetUnitColor(colorTheme, mainColorTheme);
      return BluestacksUIColor.Compute(unitColor.Item1, unitColor.Item2, unitColor.Item3, unitColor.Item4, mainThemeColor);
    }

    private Color GetForegroundColor(string colorTheme)
    {
      ColorUtils mainForeGroundColor = this.MainForeGroundColor;
      ColorUtils mainColorTheme = new ColorUtils((Color) ColorConverter.ConvertFromString("#F8F8EE"));
      Tuple<double, double, double, double> unitColor = BluestacksUIColor.GetUnitColor(colorTheme, mainColorTheme);
      return BluestacksUIColor.Compute(unitColor.Item1, unitColor.Item2, unitColor.Item3, unitColor.Item4, mainForeGroundColor);
    }

    private Color GetContrastColor(string colorTheme)
    {
      ColorUtils contrastThemeColor = this.ContrastThemeColor;
      ColorUtils mainColorTheme = new ColorUtils((Color) ColorConverter.ConvertFromString("#585A6C"));
      Tuple<double, double, double, double> unitColor = BluestacksUIColor.GetUnitColor(colorTheme, mainColorTheme);
      return BluestacksUIColor.Compute(unitColor.Item1, unitColor.Item2, unitColor.Item3, unitColor.Item4, contrastThemeColor);
    }

    private Color GetContrastForegroundColor(string colorTheme)
    {
      ColorUtils contrastForegroundColor = this.ContrastForegroundColor;
      ColorUtils mainColorTheme = new ColorUtils((Color) ColorConverter.ConvertFromString("#20233A"));
      Tuple<double, double, double, double> unitColor = BluestacksUIColor.GetUnitColor(colorTheme, mainColorTheme);
      return BluestacksUIColor.Compute(unitColor.Item1, unitColor.Item2, unitColor.Item3, unitColor.Item4, contrastForegroundColor);
    }

    private Color GetHighlighterColor(string colorTheme)
    {
      ColorUtils highlighterColor = this.ApplicationHighlighterColor;
      ColorUtils mainColorTheme = new ColorUtils((Color) ColorConverter.ConvertFromString("#F87C06"));
      Tuple<double, double, double, double> unitColor = BluestacksUIColor.GetUnitColor(colorTheme, mainColorTheme);
      return BluestacksUIColor.Compute(unitColor.Item1, unitColor.Item2, unitColor.Item3, unitColor.Item4, highlighterColor);
    }

    private static Color Compute(double r, double g, double b, double a, ColorUtils c)
    {
      return ColorUtils.FromHSLA((double) c.H / r, (double) c.S / g, (double) c.L / b, (double) c.A / a).WPFColor;
    }

    private static Tuple<double, double, double, double> GetUnitColor(
      string colorString,
      ColorUtils mainColorTheme)
    {
      if (!colorString.Contains<char>('#'))
        colorString = "#" + colorString;
      ColorUtils colorUtils = new ColorUtils((Color) ColorConverter.ConvertFromString(colorString));
      return new Tuple<double, double, double, double>((double) mainColorTheme.H / (double) colorUtils.H, (double) mainColorTheme.S / (double) colorUtils.S, (double) mainColorTheme.L / (double) colorUtils.L, (double) ((int) mainColorTheme.A / (int) colorUtils.A));
    }

    internal ColorUtils ApplicationHighlighterColor
    {
      get
      {
        return new ColorUtils((this.DictBrush[nameof (ApplicationHighlighterColor)] as SolidColorBrush).Color);
      }
      set
      {
        this.DictBrush[nameof (ApplicationHighlighterColor)] = (Brush) new SolidColorBrush(value.WPFColor);
      }
    }

    internal ColorUtils MainThemeColor
    {
      get
      {
        return new ColorUtils((this.DictBrush[nameof (MainThemeColor)] as SolidColorBrush).Color);
      }
      set
      {
        this.DictBrush[nameof (MainThemeColor)] = (Brush) new SolidColorBrush(value.WPFColor);
      }
    }

    internal ColorUtils MainForeGroundColor
    {
      get
      {
        return new ColorUtils((this.DictBrush[nameof (MainForeGroundColor)] as SolidColorBrush).Color);
      }
      set
      {
        this.DictBrush[nameof (MainForeGroundColor)] = (Brush) new SolidColorBrush(value.WPFColor);
      }
    }

    internal ColorUtils ContrastThemeColor
    {
      get
      {
        return new ColorUtils((this.DictBrush[nameof (ContrastThemeColor)] as SolidColorBrush).Color);
      }
      set
      {
        this.DictBrush[nameof (ContrastThemeColor)] = (Brush) new SolidColorBrush(value.WPFColor);
      }
    }

    internal ColorUtils ContrastForegroundColor
    {
      get
      {
        return new ColorUtils((this.DictBrush[nameof (ContrastForegroundColor)] as SolidColorBrush).Color);
      }
      set
      {
        this.DictBrush[nameof (ContrastForegroundColor)] = (Brush) new SolidColorBrush(value.WPFColor);
      }
    }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SkewTransform TextBoxTransForm
    {
      get
      {
        return (SkewTransform) this.DictTransform[nameof (TextBoxTransForm)];
      }
      set
      {
        this.DictTransform[nameof (TextBoxTransForm)] = (Transform) value;
      }
    }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SkewTransform TextBoxAntiTransForm
    {
      get
      {
        return (SkewTransform) this.DictTransform[nameof (TextBoxAntiTransForm)];
      }
      set
      {
        this.DictTransform[nameof (TextBoxAntiTransForm)] = (Transform) value;
      }
    }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SkewTransform TabTransform
    {
      get
      {
        return (SkewTransform) this.DictTransform[nameof (TabTransform)];
      }
      set
      {
        this.DictTransform[nameof (TabTransform)] = (Transform) value;
      }
    }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SkewTransform TabTransformPortrait
    {
      get
      {
        return (SkewTransform) this.DictTransform[nameof (TabTransformPortrait)];
      }
      set
      {
        this.DictTransform[nameof (TabTransformPortrait)] = (Transform) value;
      }
    }

    internal CornerRadius TabRadius
    {
      get
      {
        return this.DictCornerRadius[nameof (TabRadius)];
      }
      set
      {
        this.DictCornerRadius[nameof (TabRadius)] = value;
      }
    }

    internal CornerRadius TextBoxRadius
    {
      get
      {
        return this.DictCornerRadius[nameof (TextBoxRadius)];
      }
      set
      {
        this.DictCornerRadius[nameof (TextBoxRadius)] = value;
      }
    }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RectangleGeometry AppIconRectangleGeometry
    {
      get
      {
        return (RectangleGeometry) this.DictGeometry[nameof (AppIconRectangleGeometry)];
      }
      set
      {
        this.DictGeometry[nameof (AppIconRectangleGeometry)] = (Geometry) value;
      }
    }

    private void InitalizeDefault()
    {
      this.DictBrush["MainThemeColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#1E2138"));
      this.DictBrush["MainForeGroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#F8F8EE"));
      this.DictBrush["ContrastThemeColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#585A6C"));
      this.DictBrush["ContrastForegroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      this.DictBrush["ApplicationHighlighterColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#F87C06"));
      this.DictThemeAvailable["UserAvailable"] = "true";
      this.DictThemeAvailable["themeName"] = RegistryManager.ClientThemeName;
      this.DictThemeAvailable["ThemeDisplayName"] = "BlueStacks";
      this.DictThemeAvailable["LocationUnrestricted"] = "true";
      this.DictGeometry["AppIconRectangleGeometry"] = (Geometry) new RectangleGeometry();
      (this.DictGeometry["AppIconRectangleGeometry"] as RectangleGeometry).RadiusX = 10.0;
      (this.DictGeometry["AppIconRectangleGeometry"] as RectangleGeometry).RadiusY = 10.0;
      (this.DictGeometry["AppIconRectangleGeometry"] as RectangleGeometry).Rect = new Rect(0.0, 0.0, 68.0, 68.0);
      this.DictTransform["TabTransform"] = (Transform) new SkewTransform(0.0, 0.0);
      this.DictTransform["TabTransformPortrait"] = (Transform) new SkewTransform(0.0, 0.0);
      this.DictTransform["TextBoxTransForm"] = (Transform) new SkewTransform(0.0, 0.0);
      this.DictTransform["TextBoxAntiTransForm"] = (Transform) new SkewTransform(0.0, 0.0);
      this.DictCornerRadius["TabRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["TextBoxRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["SearchButtonPadding"] = new CornerRadius(0.0, 0.0, 0.0, 0.0);
      this.DictCornerRadius["SearchButtonMargin"] = new CornerRadius(0.0, 0.0, -40.0, 8.0);
      this.DictCornerRadius["TabMarginLandScape"] = new CornerRadius(2.0, 0.0, 0.0, 0.0);
      this.DictCornerRadius["TabMarginPortrait"] = new CornerRadius(2.0, 0.0, 0.0, 0.0);
      this.DictCornerRadius["CloseTabButtonLandScape"] = new CornerRadius(3.0, 3.0, 10.0, 3.0);
      this.DictCornerRadius["CloseTabButtonDropDown"] = new CornerRadius(0.0);
      this.DictCornerRadius["SpeedUpTipsRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["SettingsWindowRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["MessageWindowRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["PreferenceDropDownRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["BeginnerGuideRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["PopupRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["ButtonCornerRadius"] = new CornerRadius(0.0);
      this.DictCornerRadius["SidebarElementCornerRadius"] = new CornerRadius(0.0);
      this.CalculateAndNotify(false);
    }

    public void CalculateAndNotify(bool isNotify = true)
    {
      foreach (KeyValuePair<string, Brush> keyValuePair in (Dictionary<string, Brush>) this.IntializeFurther())
        this.DictBrush[keyValuePair.Key] = keyValuePair.Value;
      if (!isNotify)
        return;
      this.NotifyUIElements();
    }

    internal SerializableDictionary<string, Brush> IntializeFurther()
    {
      SerializableDictionary<string, Brush> serializableDictionary = new SerializableDictionary<string, Brush>();
      LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush();
      serializableDictionary["ApplicationBorderBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["DimOverlayColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#A0000000"));
      serializableDictionary["DimOverlayForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#8AFFFFFF"));
      serializableDictionary["ViewXpackPopupColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#99000000"));
      serializableDictionary["ViewXpackPopupHoverColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#BF000000"));
      serializableDictionary["ViewXpackPopupClickedColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF000000"));
      serializableDictionary["GuidanceKeyWarningBorderColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#F09200"));
      serializableDictionary["GuidanceKeyWarningBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF121429"));
      serializableDictionary["LightBandingColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      serializableDictionary["DarkBandingColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF34375C"));
      serializableDictionary["WidgetBarBorder"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF34375C"));
      serializableDictionary["DualTextBlockLightForegroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFFFF"));
      serializableDictionary["OverlayLabelBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#B3000000"));
      serializableDictionary["OverlayLabelBorderColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#66FFFFFF"));
      serializableDictionary["XPackPopupColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF402F"));
      serializableDictionary["PopupBorderBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["ApplicationBackgroundBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["AppIconTextColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["AppIconDropShadowBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF02A6F4"));
      serializableDictionary["BeginnersGuideTextColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["BluestacksTitleColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["ModalShadowColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#99000000"));
      serializableDictionary["PopupShadowColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#99000000"));
      serializableDictionary["TopBarColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF232642"));
      LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush();
      serializableDictionary["StreamingTopBarColor"] = (Brush) linearGradientBrush2;
      linearGradientBrush2.StartPoint = new Point(0.0, 0.0);
      linearGradientBrush2.EndPoint = new Point(1.0, 0.0);
      linearGradientBrush2.GradientStops.Add(new GradientStop(this.GetMainColor("#741BFF"), 0.0));
      linearGradientBrush2.GradientStops.Add(new GradientStop(this.GetMainColor("#4C07B7"), 1.0));
      serializableDictionary["BottomBarColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      LinearGradientBrush linearGradientBrush3 = new LinearGradientBrush();
      serializableDictionary["GuidanceVideoDescriptionColor"] = (Brush) linearGradientBrush3;
      linearGradientBrush3.StartPoint = new Point(0.0, 0.38);
      linearGradientBrush3.EndPoint = new Point(1.0, 0.64);
      linearGradientBrush3.GradientStops.Add(new GradientStop(this.GetMainColor("#B909BC"), -0.03));
      linearGradientBrush3.GradientStops.Add(new GradientStop(this.GetMainColor("#8013B3"), 0.64));
      serializableDictionary["SelectedTabBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["SelectedTabForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#B4FFFFFF"));
      serializableDictionary["SelectedTabBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["AppTabBorderBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF464A75"));
      serializableDictionary["AppTabsPopupBorder"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["AppTabsPopupbackground"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF232642"));
      serializableDictionary["TabBackgroundColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["TabForegroundColor"] = (Brush) new SolidColorBrush(this.GetContrastForegroundColor("#8CFFFFFF"));
      serializableDictionary["TabBackgroundHoverColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF464A75"));
      serializableDictionary["HomeAppTabBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF266AB8"));
      serializableDictionary["HomeAppTabForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF8B9FC2"));
      serializableDictionary["HomeAppTabBackgroundHoverColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF0B7DDA"));
      serializableDictionary["HomeAppTabForegroundHoverColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFC6CEE1"));
      serializableDictionary["SelectedHomeAppTabBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF328CF2"));
      serializableDictionary["SelectedHomeAppTabForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFEAF2FB"));
      serializableDictionary["HomeAppBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF1E2138"));
      serializableDictionary["HomeAppTabButtonBaseColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF151833"));
      serializableDictionary["BeginnerGuideBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF283055"));
      serializableDictionary["ContextMenuItemBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["ContextMenuItemBackgroundHighlighterColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF222949"));
      serializableDictionary["ContextMenuItemForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["FrontendPopupTitleColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFCBD6EF"));
      serializableDictionary["ContextMenuItemForegroundDimColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["ContextMenuItemForegroundDimDimColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF626480"));
      serializableDictionary["ContextMenuItemForegroundHighlighterColor"] = (Brush) new SolidColorBrush(this.GetHighlighterColor("#FFF09200"));
      serializableDictionary["ContextMenuItemForegroundGreenColor"] = (Brush) new SolidColorBrush(this.GetHighlighterColor("#FF2BBD00"));
      serializableDictionary["PopupWindowWarningForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFED5547"));
      serializableDictionary["ContextMenuItemBackgroundHoverColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["SliderButtonColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF328CF2"));
      serializableDictionary["HorizontalSeparator"] = (Brush) new SolidColorBrush(this.GetContrastColor("#50757B9F"));
      serializableDictionary["ProgressBarForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF01D328"));
      serializableDictionary["ProgressBarBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["ProgressBarBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["ProgressBarProgressColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF038CEF"));
      serializableDictionary["BootPromotionTextColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["GenericBrushLight"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFFFF"));
      serializableDictionary["TextBoxBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["TextBoxBorderColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF626480"));
      serializableDictionary["TextBoxHoverBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF008BEF"));
      serializableDictionary["TextBoxFocussedBackgroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF121429"));
      serializableDictionary["TextBoxFocussedBorderColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF008BEF"));
      serializableDictionary["TextBoxFocussedForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["TextBoxErrorBackgroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#20FF402F"));
      serializableDictionary["TextBoxErrorBorderColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFF402F"));
      serializableDictionary["TextBoxWarningBackgroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF121429"));
      serializableDictionary["TextBoxWarningBorderColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF09200"));
      serializableDictionary["SearchTextBoxBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#00000000"));
      serializableDictionary["SearchGridForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#8CFFFFFF"));
      serializableDictionary["SearchGridForegroundFocusedColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["SearchGridBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#DC232642"));
      serializableDictionary["SearchGridBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#6EFFFFFF"));
      serializableDictionary["FullScreenTopBarBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["FullScreenTopBarButtonBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF328CF2"));
      serializableDictionary["FullScreenTopBarButtonBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF328CF2"));
      serializableDictionary["FullScreenTopBarForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["BlockerAdControlBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["BlockerAdControlHighlightedForegroundColor"] = (Brush) new SolidColorBrush(this.GetHighlighterColor("#FFF09200"));
      serializableDictionary["BlockerAdControlForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFF8F8EE"));
      serializableDictionary["ComboBoxBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["ComboBoxBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FFA5A7C2"));
      serializableDictionary["ComboBoxForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["ComboBoxItemBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["ComboBoxScrollBarColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF464A75"));
      LinearGradientBrush linearGradientBrush4 = new LinearGradientBrush();
      serializableDictionary["ComboBoxHorizontalScrollBarBackgroundColor"] = (Brush) linearGradientBrush4;
      linearGradientBrush4.StartPoint = new Point(0.0, 0.0);
      linearGradientBrush4.EndPoint = new Point(0.0, 1.0);
      linearGradientBrush4.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFFFC3C3"), 0.0));
      linearGradientBrush4.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFFFDBDB"), 0.2));
      linearGradientBrush4.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFFFDBDB"), 0.8));
      linearGradientBrush4.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFFFC7C7"), 1.0));
      LinearGradientBrush linearGradientBrush5 = new LinearGradientBrush();
      serializableDictionary["ComboBoxVerticalScrollBarBackgroundColor"] = (Brush) linearGradientBrush5;
      linearGradientBrush5.StartPoint = new Point(0.0, 0.0);
      linearGradientBrush5.EndPoint = new Point(0.0, 1.0);
      linearGradientBrush5.GradientStops.Add(new GradientStop(this.GetContrastColor("#FF464A75"), 0.0));
      linearGradientBrush5.GradientStops.Add(new GradientStop(this.GetContrastColor("#FF464A75"), 0.2));
      linearGradientBrush5.GradientStops.Add(new GradientStop(this.GetContrastColor("#FF464A75"), 0.8));
      linearGradientBrush5.GradientStops.Add(new GradientStop(this.GetContrastColor("#FF464A75"), 1.0));
      serializableDictionary["WidgetBarBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#E6232642"));
      serializableDictionary["WidgetBarForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF94A3C9"));
      serializableDictionary["SettingsWindowBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["SettingsWindowTitleBarBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["SettingsWindowTitleBarForeGround"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["SettingsWindowForegroundDimColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["SettingsWindowForegroundDimDimColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF626480"));
      serializableDictionary["SettingsWindowBorderColor"] = (Brush) new SolidColorBrush(this.GetHighlighterColor("#FF34375C"));
      serializableDictionary["SettingsWindowTextBoxBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF3D4566"));
      serializableDictionary["SettingsWindowTextBoxBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["SettingsWindowForegroundDimDimDimColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF626480"));
      serializableDictionary["SettingsWindowForegroundChangeColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFDC143C"));
      serializableDictionary["SettingsWindowBorderBrushColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["SettingsWindowTabMenuBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["SettingsWindowTabMenuItemForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["SettingsWindowTabMenuItemSelectedForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["SettingsWindowTabMenuItemLegendForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["SettingsWindowTabMenuItemUnderline"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF008BEF"));
      serializableDictionary["VerticalSeparator"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF121429"));
      serializableDictionary["SettingsWindowTabMenuItemBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["SettingsWindowTabMenuItemHoverBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["SettingsWindowTabMenuItemSelectedBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["WarningGridBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["WarningGridBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["CustomSliderBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["CustomSlideRoundButtonrBrush"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["CustomSliderForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["CustomSliderThumbColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF008BEF"));
      serializableDictionary["CustomSliderThumbBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF43ACED"));
      serializableDictionary["MultiInstanceManagerForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF34375C"));
      serializableDictionary["MultiInstanceManagerBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF283055"));
      serializableDictionary["MultiInstanceManagerBorderColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["MultiInstanceManagerInstanceColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF3E446D"));
      serializableDictionary["MultiInstanceManagerTextBoxBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF252B4A"));
      serializableDictionary["MultiInstanceManagerTextBoxBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["ScrollViewerDisabledBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFF4F4F4"));
      serializableDictionary["ScrollViewerBorderColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["ScrollViewerBackgroundHoverColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FFCBD6EF"));
      serializableDictionary["NoInternetControlBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["NoInternetControlBorderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["NoInternetControlForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF34375C"));
      serializableDictionary["NoInternetControlTitleForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["AdvancedGameControlBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["AdvancedGameControlHeaderBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["AdvancedGameControlHeaderForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["AdvancedGameControlActionHeaderForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA9B9CC"));
      serializableDictionary["AdvancedGameControlButtonGridBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["KeymapCanvasWindowBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#40000000"));
      serializableDictionary["GameControlWindowFooterForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["GameControlWindowBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["GameControlWindowBorderColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["GameControlHeaderBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#00000000"));
      serializableDictionary["GameControlWindowHeaderForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GameControlWindowFooterTitleForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GameControlWindowHeaderColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["GameControlWindowGuidanceKeyForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFB80000"));
      serializableDictionary["GameControlWindowBottomBarForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["GameControlCategoryHeaderForeground"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FFA5A7C2"));
      serializableDictionary["GameControlSelectedCategoryHeaderForeground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FFFFFFFF"));
      serializableDictionary["GameControlCategoryGroupBoxForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GuidanceVideoElementHeaderForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GuidanceVideoElementForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA9B9CC"));
      serializableDictionary["GameControlWindowVerticalDividerColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["AdvancedSettingsItemPanelBorder"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["AdvancedSettingsItemPanelBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["AdvancedSettingsItemPanelForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["GuidanceKeyForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GuidanceKeyTextboxBorder"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF34375C"));
      serializableDictionary["GuidanceKeyTextboxSelectedBorder"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF4A90E2"));
      serializableDictionary["GuidanceKeyTextboxSelectedBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["GuidanceTextColorForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GoogleSigninPopupTextColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF858585"));
      LinearGradientBrush linearGradientBrush6 = new LinearGradientBrush();
      serializableDictionary["GuidanceTextBorderBrush"] = (Brush) linearGradientBrush6;
      linearGradientBrush6.StartPoint = new Point(0.0, 0.0);
      linearGradientBrush6.EndPoint = new Point(0.0, 20.0);
      linearGradientBrush6.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFA5A7C2"), 0.0));
      linearGradientBrush6.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFA5A7C2"), 0.05));
      linearGradientBrush6.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFA5A7C2"), 0.07));
      linearGradientBrush6.GradientStops.Add(new GradientStop(this.GetContrastColor("#FFA5A7C2"), 1.0));
      RadialGradientBrush radialGradientBrush1 = new RadialGradientBrush()
      {
        GradientOrigin = new Point(0.5, 0.0),
        Center = new Point(0.5, 0.0),
        RadiusX = 5.0,
        RadiusY = 0.5
      };
      radialGradientBrush1.GradientStops.Add(new GradientStop(this.GetMainColor("#99393C65"), 1.0));
      serializableDictionary["GameControlNavigationBackgroundColor"] = (Brush) radialGradientBrush1;
      RadialGradientBrush radialGradientBrush2 = new RadialGradientBrush()
      {
        GradientOrigin = new Point(0.5, 0.0),
        Center = new Point(0.5, 0.0),
        RadiusX = 0.5,
        RadiusY = 0.5
      };
      radialGradientBrush2.GradientStops.Add(new GradientStop(this.GetMainColor("#FF232642"), 1.0));
      serializableDictionary["GameControlContentBackgroundColor"] = (Brush) radialGradientBrush2;
      serializableDictionary["GuidanceVideoElementBorder"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF5B5C6F"));
      serializableDictionary["GuidanceVideoElementBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF1D1E36"));
      serializableDictionary["KeymapExtraSettingsWindowBorder"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF4A90E2"));
      serializableDictionary["KeymapExtraSettingsWindowBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF232642"));
      serializableDictionary["KeymapExtraSettingsWindowForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA5A7C2"));
      serializableDictionary["KeymapDummyMobaForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFCD41"));
      serializableDictionary["CanvasElementsBackgroundColor"] = (Brush) new SolidColorBrush(this.GetMainColor("#FFFFFFFF"));
      serializableDictionary["DualTextblockControlBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF121429"));
      serializableDictionary["DualTextblockControlOuterBackground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF34375C"));
      serializableDictionary["DualTextBlockForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFFFFFFF"));
      serializableDictionary["GameControlSchemeForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FFA9B9CC"));
      serializableDictionary["GuidanceKeyBorderBackgroundColor"] = (Brush) new SolidColorBrush(this.GetContrastColor("#FF51546E"));
      serializableDictionary["DeleteComboTextForeground"] = (Brush) new SolidColorBrush(this.GetForegroundColor("FFFF402F"));
      serializableDictionary["HyperLinkForegroundColor"] = (Brush) new SolidColorBrush(this.GetForegroundColor("#FF047CD2"));
      serializableDictionary["InstallerWindowBorderBrush"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF34375C"));
      serializableDictionary["InstallerTextForeground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF636363"));
      serializableDictionary["InstallerWindowBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF283155"));
      serializableDictionary["InstallerWindowTextForeground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF008BEF"));
      serializableDictionary["InstallerWindowMouseOverTextForeground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF0A98FF"));
      serializableDictionary["InstallerWindowLightTextForeground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFCBD6EF"));
      serializableDictionary["InstallerWindowWhiteTextColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFFFF"));
      serializableDictionary["InstallerWindowTextBoxBackgroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF1A2147"));
      serializableDictionary["MaterialDesignBlueBtnBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF008BEF"));
      serializableDictionary["MaterialDesignBlueBtnBorderBrush"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF008BFF"));
      serializableDictionary["MaterialDesignBlueBtnMouseOverBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF0A98FF"));
      serializableDictionary["MaterialDesignBlueBtnMouseDownBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF007CD6"));
      serializableDictionary["MaterialDesignRedBtnBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFF402F"));
      serializableDictionary["MaterialDesignRedBtnBorderBrush"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFF404F"));
      serializableDictionary["MaterialDesignRedBtnMouseOverBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFF5749"));
      serializableDictionary["MaterialDesignRedBtnMouseDownBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFF2916"));
      serializableDictionary["SidebarBackground"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      serializableDictionary["SidebarElementNormal"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      serializableDictionary["SidebarElementHover"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      serializableDictionary["SidebarElementClick"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF232642"));
      serializableDictionary["MacroPlayRecorderControlBorder"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFCBD6EF"));
      serializableDictionary["LogCollectorWatermarkTextColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF34375C"));
      serializableDictionary["SyncHyperlinkForegroundColor"] = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF4A90E2"));
      this.AddButtonsColor((Dictionary<string, Brush>) serializableDictionary);
      return serializableDictionary;
    }

    private void AddButtonsColor(Dictionary<string, Brush> dict)
    {
      dict["RedMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF403F");
      dict["RedMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF402F");
      dict["RedMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["RedMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF5749");
      dict["RedMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF5749");
      dict["RedMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["RedMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF2910");
      dict["RedMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF2910");
      dict["RedMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["RedDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FF403F");
      dict["RedDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FF402F");
      dict["RedDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["WhiteMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["WhiteMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["WhiteMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["WhiteMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF1F1F1");
      dict["WhiteMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF1F1F1");
      dict["WhiteMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["WhiteMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFEAEAEA");
      dict["WhiteMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFEAEAEA");
      dict["WhiteMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["WhiteDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["WhiteDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["WhiteDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80008BEF");
      dict["WhiteWithGreyFGMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["WhiteWithGreyFGMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["WhiteWithGreyFGMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF858585");
      dict["WhiteWithGreyFGMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF1F1F1");
      dict["WhiteWithGreyFGMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF1F1F1");
      dict["WhiteWithGreyFGMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF858585");
      dict["WhiteWithGreyFGMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFEAEAEA");
      dict["WhiteWithGreyFGMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFEAEAEA");
      dict["WhiteWithGreyFGMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF858585");
      dict["WhiteWithGreyFGDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["WhiteWithGreyFGDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["WhiteWithGreyFGDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80858585");
      dict["BlueMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["BlueMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["BlueMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BlueMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF0A98FF");
      dict["BlueMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF0A98FF");
      dict["BlueMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BlueMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF007CD6");
      dict["BlueMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF007CD6");
      dict["BlueMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BlueDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80008BEF");
      dict["BlueDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80008BEF");
      dict["BlueDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["OrangeMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF09200");
      dict["OrangeMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF09200");
      dict["OrangeMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["OrangeMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF9F0B");
      dict["OrangeMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF9F0B");
      dict["OrangeMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["OrangeMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFD78200");
      dict["OrangeMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFD78200");
      dict["OrangeMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["OrangeDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80F09200");
      dict["OrangeDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80F09200");
      dict["OrangeDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["BackgroundMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF34375C");
      dict["BackgroundMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF34375C");
      dict["BackgroundMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF444B6F");
      dict["BackgroundMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF444B6F");
      dict["BackgroundMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF2B3255");
      dict["BackgroundMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF2B3255");
      dict["BackgroundMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#8030385F");
      dict["BackgroundDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#8030385F");
      dict["BackgroundDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["BackgroundBlueBorderMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["BackgroundBlueBorderMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#802B3255");
      dict["BackgroundBlueBorderMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["BackgroundBlueBorderMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["BackgroundBlueBorderMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF444B6F");
      dict["BackgroundBlueBorderMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundBlueBorderMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF008BEF");
      dict["BackgroundBlueBorderMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF34375C");
      dict["BackgroundBlueBorderMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundBlueDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80008BEF");
      dict["BackgroundBlueDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#802B3255");
      dict["BackgroundBlueDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["BackgroundOrangeBorderMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF09200");
      dict["BackgroundOrangeBorderMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFF09200");
      dict["BackgroundOrangeBorderMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundOrangeBorderMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF9F0B");
      dict["BackgroundOrangeBorderMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFF9F0B");
      dict["BackgroundOrangeBorderMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundOrangeBorderMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFD78200");
      dict["BackgroundOrangeBorderMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFD78200");
      dict["BackgroundOrangeBorderMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundOrangeBorderDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00000000");
      dict["BackgroundOrangeBorderDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#1AFFFFFF");
      dict["BackgroundOrangeBorderDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#33FFFFFF");
      dict["BackgroundWhiteBorderMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BackgroundWhiteBorderMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BackgroundWhiteBorderMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BackgroundWhiteBorderMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["BackgroundWhiteBorderDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BackgroundWhiteBorderDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["GreenMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF26AA00");
      dict["GreenMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF26AA00");
      dict["GreenMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["GreenMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF40C319");
      dict["GreenMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF40C319");
      dict["GreenMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["GreenMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF2BBD00");
      dict["GreenMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF2BBD00");
      dict["GreenMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["GreenDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#8026AA00");
      dict["GreenDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#8026AA00");
      dict["GreenDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["BorderMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF555E73");
      dict["BorderMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BorderMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFCBD6EF");
      dict["BorderMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF8A92A5");
      dict["BorderMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BorderMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFCBD6EF");
      dict["BorderMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFCBD6EF");
      dict["BorderMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BorderMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFCBD6EF");
      dict["BorderDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#80555E73");
      dict["BorderDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["BorderDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80CBD6EF");
      dict["BorderRedBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#20FF402F");
      dict["TransparentMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#96FFFFFF");
      dict["TransparentMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["TransparentMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#C8FFFFFF");
      dict["TransparentDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["TransparentDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#80FFFFFF");
      dict["OverlayMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["OverlayMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#01FFFFFF");
      dict["OverlayMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#96FFFFFF");
      dict["OverlayMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["OverlayMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#01FFFFFF");
      dict["OverlayMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["OverlayMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00FFFFFF");
      dict["OverlayMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#01FFFFFF");
      dict["OverlayMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#C8FFFFFF");
      dict["OverlayDisabledBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#00000000");
      dict["OverlayDisabledGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#1AFFFFFF");
      dict["OverlayDisabledForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#33FFFFFF");
      dict["HyperlinkForeground"] = (Brush) new SolidColorBrush(this.GetMainColor("#FF328CF2"));
      dict["DarkRedMouseOutBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF671000");
      dict["DarkRedMouseOutGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF671000");
      dict["DarkRedMouseOutForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["DarkRedMouseInBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF991700");
      dict["DarkRedMouseInGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF991700");
      dict["DarkRedMouseInForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
      dict["DarkRedMouseDownBorderBackground"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF4D0B00");
      dict["DarkRedMouseDownGridBackGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FF4D0B00");
      dict["DarkRedMouseDownForeGround"] = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFFFF");
    }

    public static void ScrollBarScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (sender == null)
        return;
      ScrollViewer scrollViewer = sender as ScrollViewer;
      double verticalOffset = scrollViewer.VerticalOffset;
      if (scrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Visible)
        scrollViewer.OpacityMask = (Brush) null;
      else if (verticalOffset < 10.0)
        scrollViewer.OpacityMask = (Brush) BluestacksUIColor.mTopOpacityMask;
      else if (scrollViewer.ExtentHeight - scrollViewer.ActualHeight - 10.0 <= verticalOffset)
        scrollViewer.OpacityMask = (Brush) BluestacksUIColor.mBottomOpacityMask;
      else
        scrollViewer.OpacityMask = (Brush) BluestacksUIColor.mScrolledOpacityMask;
    }
  }
}
