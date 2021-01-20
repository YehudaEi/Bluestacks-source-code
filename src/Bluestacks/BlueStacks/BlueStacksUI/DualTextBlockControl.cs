// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DualTextBlockControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class DualTextBlockControl : UserControl, IComponentConnector
  {
    private List<IMAction> lstActionItem = new List<IMAction>();
    private List<Key> mKeyList = new List<Key>();
    private MainWindow ParentWindow;
    private KeymapExtraSettingWindow ParentExtraSettingsWindow;
    private string mActionItemProperty;
    private Type PropertyType;
    internal bool IsAddDirectionAttribute;
    internal ColumnDefinition mValueColumn;
    internal TextBox mKeyPropertyName;
    internal TextBox mKeyTextBox;
    internal TextBox mKeyPropertyNameTextBox;
    private bool _contentLoaded;

    internal List<IMAction> LstActionItem
    {
      get
      {
        return this.lstActionItem;
      }
    }

    internal string ActionItemProperty
    {
      get
      {
        return this.mActionItemProperty;
      }
      set
      {
        this.mActionItemProperty = value;
        if (value == "Tags" || value == "EnableCondition" || (value == "StartCondition" || value == "Note"))
        {
          this.mValueColumn.Width = new GridLength(1.0, GridUnitType.Star);
          this.mKeyTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
          this.mKeyPropertyName.Visibility = Visibility.Collapsed;
          this.mKeyTextBox.MaxWidth = double.PositiveInfinity;
          this.mKeyPropertyNameTextBox.Visibility = Visibility.Collapsed;
        }
        if (!(value == "DpadTitle") && !value.Equals("KeyWheel", StringComparison.InvariantCultureIgnoreCase) && (!(value == "KeyMove") || this.ParentExtraSettingsWindow.ListAction.First<IMAction>().Type != KeyActionType.MOBADpad))
          return;
        this.mKeyTextBox.IsEnabled = false;
      }
    }

    public DualTextBlockControl(
      MainWindow window,
      KeymapExtraSettingWindow keymapExtraSettingWindow = null)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.ParentExtraSettingsWindow = keymapExtraSettingWindow;
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mKeyTextBox, false);
    }

    private void KeyPropertyNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void KeyTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      KeymapCanvasWindow.sIsDirty = true;
      if (e.Key == Key.Escape)
        return;
      if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
      {
        if (this.lstActionItem[0].Type == KeyActionType.Tap || this.lstActionItem[0].Type == KeyActionType.TapRepeat || this.lstActionItem[0].Type == KeyActionType.Script)
        {
          if (e.Key == Key.Back || e.SystemKey == Key.Back)
          {
            this.mKeyTextBox.Tag = (object) string.Empty;
            BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + this.mKeyTextBox.Tag?.ToString());
          }
          else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
          {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10)
              this.mKeyList.AddIfNotContain<Key>(e.SystemKey);
            else if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
              if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                this.mKeyList.AddIfNotContain<Key>(e.SystemKey);
              else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
                this.mKeyList.AddIfNotContain<Key>(e.SystemKey);
              else
                this.mKeyList.AddIfNotContain<Key>(e.Key);
            }
            else
              this.mKeyList.AddIfNotContain<Key>(e.Key);
          }
        }
        else
        {
          if (e.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
          {
            this.mKeyTextBox.Tag = (object) IMAPKeys.GetStringForFile(e.SystemKey);
            BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
          }
          else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
          {
            this.mKeyTextBox.Tag = (object) IMAPKeys.GetStringForFile(e.Key);
            BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
          }
          else if (e.Key == Key.Back)
          {
            this.mKeyTextBox.Tag = (object) string.Empty;
            BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + string.Empty);
          }
          e.Handled = true;
        }
      }
      if (this.PropertyType.Equals(typeof (bool)))
      {
        this.mKeyTextBox.Tag = (object) !Convert.ToBoolean(this.lstActionItem.First<IMAction>()[this.ActionItemProperty], (IFormatProvider) CultureInfo.InvariantCulture);
        BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + this.mKeyTextBox.Tag?.ToString());
        if (this.lstActionItem.First<IMAction>().Type == KeyActionType.TapRepeat && KMManager.CanvasWindow.mCanvasElement != null)
          KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(this.lstActionItem.First<IMAction>(), false);
        if (this.lstActionItem.First<IMAction>().Type == KeyActionType.EdgeScroll && this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
          KMManager.AssignEdgeScrollMode(this.mKeyTextBox.Tag.ToString(), this.mKeyTextBox);
        e.Handled = true;
      }
      if (this.PropertyType.Equals(typeof (int)) && this.lstActionItem.First<IMAction>().Type == KeyActionType.FreeLook && KMManager.CanvasWindow.mCanvasElement != null)
        KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(this.lstActionItem.First<IMAction>(), false);
      if (string.Equals(this.ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase) && (e.Key == Key.Back || e.SystemKey == Key.Back))
      {
        this.mKeyTextBox.Tag = (object) string.Empty;
        BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + this.mKeyTextBox.Tag?.ToString());
      }
      if (!this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) || this.lstActionItem[0].Type != KeyActionType.Tap && this.lstActionItem[0].Type != KeyActionType.TapRepeat && this.lstActionItem[0].Type != KeyActionType.Script || e.Key != Key.Tab)
        return;
      e.Handled = true;
    }

    private void KeyTextBox_KeyUp(object sender, KeyEventArgs e)
    {
      if (this.lstActionItem[0].Type != KeyActionType.Tap && this.lstActionItem[0].Type != KeyActionType.TapRepeat && this.lstActionItem[0].Type != KeyActionType.Script)
        return;
      if (this.mKeyList.Count >= 2)
      {
        string str1 = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1));
        string str2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1));
        this.mKeyTextBox.Text = str1;
        this.mKeyTextBox.Tag = (object) str2;
        this.SetValueHandling();
      }
      else if (this.mKeyList.Count == 1)
      {
        string stringForUi = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(0));
        string stringForFile = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(0));
        this.mKeyTextBox.Text = stringForUi;
        this.mKeyTextBox.Tag = (object) stringForFile;
        this.SetValueHandling();
      }
      if (!this.ActionItemProperty.Equals("EnableCondition", StringComparison.InvariantCultureIgnoreCase) && !this.ActionItemProperty.Equals("StartCondition", StringComparison.InvariantCultureIgnoreCase) && !this.ActionItemProperty.Equals("Note", StringComparison.InvariantCultureIgnoreCase))
        this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
      this.mKeyList.Clear();
    }

    private void KeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
      {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
          e.Handled = true;
          this.mKeyTextBox.Tag = (object) "MouseMButton";
          BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseMButton");
        }
        else if (e.RightButton == MouseButtonState.Pressed)
        {
          e.Handled = true;
          this.mKeyTextBox.Tag = (object) "MouseRButton";
          BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseRButton");
        }
        else if (e.XButton1 == MouseButtonState.Pressed)
        {
          e.Handled = true;
          this.mKeyTextBox.Tag = (object) "MouseXButton1";
          BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton1");
        }
        else if (e.XButton2 == MouseButtonState.Pressed)
        {
          e.Handled = true;
          this.mKeyTextBox.Tag = (object) "MouseXButton2";
          BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton2");
        }
      }
      if (!this.PropertyType.Equals(typeof (bool)))
        return;
      this.mKeyTextBox.Tag = (object) !Convert.ToBoolean(this.lstActionItem.First<IMAction>()[this.ActionItemProperty], (IFormatProvider) CultureInfo.InvariantCulture);
      BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + this.mKeyTextBox.Tag?.ToString());
      if (this.lstActionItem.First<IMAction>().Type != KeyActionType.EdgeScroll || !this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
        return;
      KMManager.AssignEdgeScrollMode(this.mKeyTextBox.Tag.ToString(), this.mKeyTextBox);
    }

    internal bool AddActionItem(IMAction action)
    {
      this.PropertyType = IMAction.DictPropertyInfo[action.Type][this.ActionItemProperty].PropertyType;
      if (!this.PropertyType.Equals(typeof (string)) && !string.Equals(this.ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) && (!string.Equals(this.ActionItemProperty, "EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "GamepadSensitivity", StringComparison.InvariantCultureIgnoreCase)) && (!string.Equals(this.ActionItemProperty, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "SensitivityRatioY", StringComparison.InvariantCultureIgnoreCase) && (action.Type != KeyActionType.Scroll || !string.Equals(this.ActionItemProperty, "Speed", StringComparison.InvariantCultureIgnoreCase))) || action.Type == KeyActionType.State && (string.Equals(this.ActionItemProperty, "Name", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Model", StringComparison.InvariantCultureIgnoreCase)))
      {
        this.mKeyPropertyNameTextBox.IsEnabled = false;
      }
      else
      {
        this.mKeyPropertyNameTextBox.IsEnabled = true;
        int length = this.mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
        string origKey = this.mActionItemProperty;
        if (length > 0)
          origKey = this.mActionItemProperty.Substring(0, length);
        this.AssignGuidanceText(action, origKey);
      }
      if ((action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom) && (string.Equals(this.ActionItemProperty, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Acceleration", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Amplitude", StringComparison.InvariantCultureIgnoreCase)))
      {
        this.mKeyPropertyNameTextBox.IsEnabled = true;
        this.AssignGuidanceText(action, this.mActionItemProperty);
      }
      this.lstActionItem.Add(action);
      string str1 = this.mActionItemProperty;
      if (this.mActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
      {
        int length = this.mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
        if (length > 0)
          str1 = this.mActionItemProperty.Substring(0, length);
      }
      if (this.IsAddDirectionAttribute)
        BlueStacksUIBinding.Bind(this.mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + str1 + action.Direction.ToString());
      else
        BlueStacksUIBinding.Bind(this.mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + str1);
      string str2 = action[this.ActionItemProperty].ToString();
      if (string.Equals(this.ActionItemProperty, "SensitivityRatioY", StringComparison.InvariantCulture))
      {
        double num1 = Convert.ToDouble(action[this.ActionItemProperty], (IFormatProvider) CultureInfo.InvariantCulture);
        double num2 = Convert.ToDouble(action["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture);
        str2 = num1 == 0.0 || num2 == 0.0 ? (num1 == 0.0 ? 0.ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture) : num1.ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture)) : (num2 * num1).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture);
      }
      this.mKeyTextBox.Tag = action[this.ActionItemProperty];
      if (this.ActionItemProperty.StartsWith("Key", StringComparison.CurrentCultureIgnoreCase))
        BlueStacksUIBinding.Bind(this.mKeyTextBox, KMManager.GetStringsToShowInUI(str2));
      else
        this.mKeyTextBox.Text = str2.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      if (this.ActionItemProperty.Equals("KeyWheel", StringComparison.InvariantCultureIgnoreCase))
      {
        this.mKeyTextBox.ToolTip = (object) this.mKeyTextBox.Text;
        ToolTipService.SetShowOnDisabled((DependencyObject) this.mKeyTextBox, true);
      }
      if (this.lstActionItem.First<IMAction>().Type == KeyActionType.EdgeScroll && this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
        KMManager.AssignEdgeScrollMode(str2, this.mKeyTextBox);
      if (!str2.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase) && !this.ActionItemProperty.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
        return false;
      BlueStacksUIBinding.Bind(this.mKeyTextBox, KMManager.GetKeyUIValue(str2));
      this.mKeyTextBox.ToolTip = (object) this.mKeyTextBox.Text;
      return true;
    }

    private void AssignGuidanceText(IMAction action, string origKey)
    {
      if (action.Guidance.ContainsKey(origKey) && !string.IsNullOrEmpty(action.Guidance[origKey]))
        this.mKeyPropertyNameTextBox.Text = this.ParentWindow.SelectedConfig.GetUIString(action.Guidance[origKey]);
      else if (action.Guidance.ContainsKey(this.mActionItemProperty) && !string.IsNullOrEmpty(action.Guidance[this.mActionItemProperty]))
      {
        this.mKeyPropertyNameTextBox.Text = this.ParentWindow.SelectedConfig.GetUIString(action.Guidance[this.mActionItemProperty]);
        if (action.Guidance.ContainsKey(origKey) || string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text.Trim()))
          return;
        action.Guidance.Add(origKey, this.mKeyPropertyNameTextBox.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      else
      {
        BlueStacksUIBinding.Bind(this.mKeyPropertyNameTextBox, "STRING_ENTER_GUIDANCE_TEXT");
        this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Italic;
        this.mKeyPropertyNameTextBox.FontWeight = FontWeights.ExtraLight;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
      }
    }

    private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.SetValueHandling();
    }

    private void SetValueHandling()
    {
      string s1 = this.lstActionItem[0][this.ActionItemProperty].ToString();
      if (this.PropertyType.Equals(typeof (double)))
      {
        double result1;
        if (double.TryParse(s1, out result1))
          s1 = result1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string s2 = this.mKeyTextBox.Text.Replace(',', '.');
        double result2;
        if (double.TryParse(s2, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
        {
          if (!string.Equals(this.ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "SensitivityRatioY", StringComparison.InvariantCultureIgnoreCase))
            s1 = s2;
          else if (string.Equals(this.ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase))
          {
            if (0.0 <= result2 && result2 <= 10.0)
              s1 = result2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            else
              this.mKeyTextBox.Text = s1;
            if (this.lstActionItem[0] is Pan && this.ParentExtraSettingsWindow != null && this.ParentExtraSettingsWindow.mDictDualTextBox.ContainsKey("Fields~SensitivityRatioY"))
            {
              double num1 = Convert.ToDouble(s1, (IFormatProvider) CultureInfo.InvariantCulture);
              double num2 = Convert.ToDouble(this.ParentExtraSettingsWindow.mDictDualTextBox["Fields~SensitivityRatioY"].mKeyTextBox.Text.Replace(',', '.'), (IFormatProvider) CultureInfo.InvariantCulture);
              this.lstActionItem[0]["SensitivityRatioY"] = num2 == 0.0 || num1 == 0.0 ? (object) num2.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) (num2 / num1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
          }
          else
          {
            double num1 = Convert.ToDouble(this.lstActionItem[0]["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture);
            double num2 = Convert.ToDouble(this.mKeyTextBox.Text.Replace(',', '.'), (IFormatProvider) CultureInfo.InvariantCulture);
            if (0.0 <= result2 && result2 <= 10.0)
            {
              s1 = num2 == 0.0 ? "0" : (num1 == 0.0 ? num2.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (num2 / num1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            else
            {
              double num3 = Convert.ToDouble(this.lstActionItem[0]["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture);
              this.mKeyTextBox.Text = num3 == 0.0 || num1 == 0.0 ? (num3 == 0.0 ? 0.ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture) : num3.ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture)) : (num1 * num3).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture);
            }
          }
        }
        else if (string.Equals(this.mKeyTextBox.Text, ".", StringComparison.InvariantCultureIgnoreCase))
        {
          this.mKeyTextBox.Text = "0.";
          s1 = "0";
          this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
        }
        else if (string.IsNullOrEmpty(this.mKeyTextBox.Text))
        {
          this.mKeyTextBox.Text = "0";
          s1 = "0";
          this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
        }
        else if (!string.IsNullOrEmpty(this.mKeyTextBox.Text))
          this.mKeyTextBox.Text = s1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      else if (this.PropertyType.Equals(typeof (int)))
      {
        if (int.TryParse(this.mKeyTextBox.Text, out int _))
          s1 = this.mKeyTextBox.Text;
        else if (!string.IsNullOrEmpty(this.mKeyTextBox.Text))
          this.mKeyTextBox.Text = s1;
      }
      else
        s1 = !this.PropertyType.Equals(typeof (bool)) ? (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) || this.ActionItemProperty.StartsWith("Gamepad", StringComparison.InvariantCultureIgnoreCase) ? this.mKeyTextBox.Tag.ToString() : this.mKeyTextBox.Text) : this.mKeyTextBox.Tag.ToString();
      this.Setvalue(s1);
    }

    internal void Setvalue(string value)
    {
      foreach (IMAction imAction in this.lstActionItem)
      {
        if (imAction[this.ActionItemProperty].ToString().Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
          this.mKeyTextBox.ToolTip = (object) this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
        if (!string.Equals(imAction[this.ActionItemProperty].ToString(), value, StringComparison.InvariantCultureIgnoreCase))
        {
          imAction[this.ActionItemProperty] = (object) value;
          KeymapCanvasWindow.sIsDirty = true;
        }
      }
      if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
        this.mKeyTextBox.Text = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
      if (!this.ActionItemProperty.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
        return;
      this.mKeyTextBox.Text = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
      this.mKeyTextBox.ToolTip = (object) this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
    }

    private void KeyPropertyNameTextBox_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs _2)
    {
      if (this.mKeyPropertyNameTextBox.IsVisible)
        return;
      string key = this.ActionItemProperty;
      if (this.ActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
      {
        int length = this.ActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
        if (length > 0)
          key = this.ActionItemProperty.Substring(0, length);
      }
      if (string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), this.mKeyPropertyNameTextBox.Text, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text.Trim()))
      {
        foreach (IMAction imAction in this.lstActionItem)
          imAction.Guidance.Remove(key);
      }
      else
      {
        KeymapCanvasWindow.sIsDirty = true;
        foreach (IMAction imAction in this.lstActionItem)
          imAction.Guidance[key] = this.mKeyPropertyNameTextBox.Text;
        this.ParentWindow.SelectedConfig.AddString(this.mKeyPropertyNameTextBox.Text);
      }
    }

    private void KeyTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.PropertyType.Equals(typeof (double)) && !double.TryParse(this.mKeyTextBox.Text, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out double _))
      {
        this.Setvalue("0");
        this.mKeyTextBox.Text = "0";
      }
      if (!this.PropertyType.Equals(typeof (int)) || int.TryParse(this.mKeyTextBox.Text, out int _))
        return;
      this.Setvalue("0");
      this.mKeyTextBox.Text = "0";
    }

    private void KeyPropertyNameTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), this.mKeyPropertyNameTextBox.Text, StringComparison.InvariantCulture))
        return;
      this.mKeyPropertyNameTextBox.Text = "";
      this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Normal;
      this.mKeyPropertyNameTextBox.FontWeight = FontWeights.Normal;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockForeground");
    }

    private void KeyPropertyNameTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text))
        return;
      this.mKeyPropertyNameTextBox.Text = LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", "");
      this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Italic;
      this.mKeyPropertyNameTextBox.FontWeight = FontWeights.ExtraLight;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
    }

    private void mKeyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.IsLoaded)
        return;
      KMManager.sGamepadDualTextbox = this;
      KMManager.pressedGamepadKeyList.Clear();
      KMManager.CallGamepadHandler(this.ParentWindow, "true");
    }

    private void KeyTextBoxPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (!this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
        return;
      if (e.Delta > 0)
      {
        e.Handled = true;
        this.mKeyTextBox.Tag = (object) "MouseWheelUp";
        BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
      }
      else
      {
        if (e.Delta >= 0)
          return;
        e.Handled = true;
        this.mKeyTextBox.Tag = (object) "MouseWheelDown";
        BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/dualtextblockcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mValueColumn = (ColumnDefinition) target;
          break;
        case 2:
          this.mKeyPropertyName = (TextBox) target;
          this.mKeyPropertyName.TextChanged += new TextChangedEventHandler(this.KeyPropertyNameTextBox_TextChanged);
          this.mKeyPropertyName.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.KeyPropertyNameTextBox_IsVisibleChanged);
          break;
        case 3:
          this.mKeyTextBox = (TextBox) target;
          this.mKeyTextBox.PreviewMouseDown += new MouseButtonEventHandler(this.KeyTextBox_MouseDown);
          this.mKeyTextBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.mKeyTextBox_PreviewMouseLeftButtonDown);
          this.mKeyTextBox.TextChanged += new TextChangedEventHandler(this.KeyTextBox_TextChanged);
          this.mKeyTextBox.PreviewKeyDown += new KeyEventHandler(this.KeyTextBox_KeyDown);
          this.mKeyTextBox.KeyUp += new KeyEventHandler(this.KeyTextBox_KeyUp);
          this.mKeyTextBox.LostFocus += new RoutedEventHandler(this.KeyTextBox_LostFocus);
          this.mKeyTextBox.PreviewMouseWheel += new MouseWheelEventHandler(this.KeyTextBoxPreviewMouseWheel);
          break;
        case 4:
          this.mKeyPropertyNameTextBox = (TextBox) target;
          this.mKeyPropertyNameTextBox.GotFocus += new RoutedEventHandler(this.KeyPropertyNameTextBox_GotFocus);
          this.mKeyPropertyNameTextBox.LostFocus += new RoutedEventHandler(this.KeyPropertyNameTextBox_LostFocus);
          this.mKeyPropertyNameTextBox.TextChanged += new TextChangedEventHandler(this.KeyPropertyNameTextBox_TextChanged);
          this.mKeyPropertyNameTextBox.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.KeyPropertyNameTextBox_IsVisibleChanged);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
