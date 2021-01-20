// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.IMapTextBox
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  public class IMapTextBox : XTextBox, IComponentConnector
  {
    public static readonly DependencyProperty IsKeyBoardInFocusProperty = DependencyProperty.Register(nameof (IsKeyBoardInFocus), typeof (bool), typeof (IMapTextBox), new PropertyMetadata((object) false, new PropertyChangedCallback(IMapTextBox.OnKeyBoardInFocusChanged)));
    public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(nameof (PropertyType), typeof (Type), typeof (IMapTextBox), new PropertyMetadata());
    public static readonly DependencyProperty ActionTypeProperty = DependencyProperty.Register(nameof (ActionType), typeof (KeyActionType), typeof (IMapTextBox), new PropertyMetadata());
    public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register(nameof (IMActionItems), typeof (ObservableCollection<IMActionItem>), typeof (IMapTextBox), new PropertyMetadata());
    private List<Key> mKeyList = new List<Key>();
    internal IMapTextBox mTextBox;
    private bool _contentLoaded;

    public IMapTextBox()
    {
      this.InitializeComponent();
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this, false);
      this.ClearValue(IMapTextBox.IMActionItemsProperty);
      this.Loaded += new RoutedEventHandler(this.IMapTextBox_Loaded);
    }

    public bool IsKeyBoardInFocus
    {
      get
      {
        return (bool) this.GetValue(IMapTextBox.IsKeyBoardInFocusProperty);
      }
      set
      {
        this.SetValue(IMapTextBox.IsKeyBoardInFocusProperty, (object) value);
      }
    }

    private static void OnKeyBoardInFocusChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      bool result;
      if (!(sender is IMapTextBox imapTextBox) || !bool.TryParse(args.NewValue.ToString(), out result))
        return;
      KMManager.CurrentIMapTextBox = result ? imapTextBox : (IMapTextBox) null;
    }

    public Type PropertyType
    {
      get
      {
        return (Type) this.GetValue(IMapTextBox.PropertyTypeProperty);
      }
      set
      {
        this.SetValue(IMapTextBox.PropertyTypeProperty, (object) value);
      }
    }

    public KeyActionType ActionType
    {
      get
      {
        return (KeyActionType) this.GetValue(IMapTextBox.ActionTypeProperty);
      }
      set
      {
        this.SetValue(IMapTextBox.ActionTypeProperty, (object) value);
      }
    }

    public ObservableCollection<IMActionItem> IMActionItems
    {
      get
      {
        return (ObservableCollection<IMActionItem>) this.GetValue(IMapTextBox.IMActionItemsProperty);
      }
      set
      {
        if (value == null)
          this.ClearValue(IMapTextBox.IMActionItemsProperty);
        else
          this.SetValue(IMapTextBox.IMActionItemsProperty, (object) value);
      }
    }

    private void IMapTextBox_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.TextBlock != null)
      {
        this.TextBlock.TextTrimming = TextTrimming.None;
        this.TextBlock.TextWrapping = TextWrapping.Wrap;
      }
      if (string.IsNullOrEmpty(this.Tag.ToString()))
        return;
      string[] strArray = this.Tag.ToString().Split('+');
      if (strArray.Length == 0)
        return;
      if (this.IMActionItems[0].ActionItem.Contains("_alt1", StringComparison.InvariantCultureIgnoreCase) || this.IMActionItems[0].ActionItem.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
        this.Text = string.Join(" + ", ((IEnumerable<string>) strArray).ToList<string>().Select<string, string>((Func<string, string>) (x => LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.CheckForGamepadSuffix(x.Trim())), ""))).ToArray<string>());
      else
        this.Text = string.Join(" + ", ((IEnumerable<string>) strArray).ToList<string>().Select<string, string>((Func<string, string>) (x => LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(x.Trim()), ""))).ToArray<string>());
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      KMManager.CurrentIMapTextBox = this;
      KMManager.pressedGamepadKeyList.Clear();
      KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow, "true");
      this.TextChanged -= new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      this.TextChanged += new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      this.PreviewMouseWheel -= new MouseWheelEventHandler(this.IMapTextBox_PreviewMouseWheel);
      this.PreviewMouseWheel += new MouseWheelEventHandler(this.IMapTextBox_PreviewMouseWheel);
      this.SetCaretIndex();
    }

    private void IMapTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
      if (args != null && args.Delta != 0 && (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>()))
      {
        foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
        {
          if (imActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
          {
            this.Tag = args.Delta < 0 ? (object) "MouseWheelDown" : (object) "MouseWheelUp";
            this.SetValueHandling(imActionItem);
            BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(this.Tag.ToString()));
          }
          if (this.PropertyType.Equals(typeof (bool)))
          {
            bool flag = !Convert.ToBoolean(imActionItem.IMAction[imActionItem.ActionItem], (IFormatProvider) CultureInfo.InvariantCulture);
            this.Tag = (object) flag;
            IMapTextBox.Setvalue(imActionItem, flag.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + this.Tag?.ToString());
            args.Handled = true;
          }
        }
        args.Handled = true;
      }
      this.SetCaretIndex();
    }

    private void IMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
      {
        foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
          this.SetValueHandling(imActionItem);
        KMManager.CheckAndCreateNewScheme();
      }
      this.SetCaretIndex();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      this.TextChanged -= new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      this.PreviewMouseWheel -= new MouseWheelEventHandler(this.IMapTextBox_PreviewMouseWheel);
      KMManager.CurrentIMapTextBox = (IMapTextBox) null;
      this.InputTextValidity = TextValidityOptions.Success;
      if (this.ToolTip is ToolTip toolTip)
        toolTip.IsOpen = false;
      KMManager.CurrentIMapTextBox = (IMapTextBox) null;
      base.OnLostFocus(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs args)
    {
      if (args != null && args.Key != Key.Escape)
      {
        if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
        {
          foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
          {
            if (imActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
            {
              if (imActionItem.IMAction.Type == KeyActionType.Tap || imActionItem.IMAction.Type == KeyActionType.TapRepeat || imActionItem.IMAction.Type == KeyActionType.Script)
              {
                if (args.Key == Key.Back || args.SystemKey == Key.Back)
                {
                  this.Tag = (object) string.Empty;
                  BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + this.Tag?.ToString());
                }
                else if (IMAPKeys.mDictKeys.ContainsKey(args.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(args.Key))
                {
                  if (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt || args.SystemKey == Key.F10)
                    this.mKeyList.AddIfNotContain<Key>(args.SystemKey);
                  else if (args.KeyboardDevice.Modifiers != ModifierKeys.None)
                  {
                    if (args.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                      this.mKeyList.AddIfNotContain<Key>(args.SystemKey);
                    else if (args.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
                      this.mKeyList.AddIfNotContain<Key>(args.SystemKey);
                    else
                      this.mKeyList.AddIfNotContain<Key>(args.Key);
                  }
                  else
                    this.mKeyList.AddIfNotContain<Key>(args.Key);
                }
              }
              else
              {
                if (args.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(args.SystemKey))
                {
                  this.Tag = (object) IMAPKeys.GetStringForFile(args.SystemKey);
                  BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.SystemKey));
                }
                else if (IMAPKeys.mDictKeys.ContainsKey(args.Key))
                {
                  this.Tag = (object) IMAPKeys.GetStringForFile(args.Key);
                  BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.Key));
                }
                else if (args.Key == Key.Back)
                {
                  this.Tag = (object) string.Empty;
                  BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + string.Empty);
                }
                args.Handled = true;
              }
            }
            if (string.Equals(imActionItem.ActionItem, "GamepadStick", StringComparison.InvariantCulture))
            {
              if (args.Key == Key.Back || args.Key == Key.Delete)
              {
                this.Tag = (object) string.Empty;
                BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + string.Empty);
              }
              args.Handled = true;
            }
            if (this.PropertyType.Equals(typeof (bool)))
            {
              bool flag = !Convert.ToBoolean(imActionItem.IMAction[imActionItem.ActionItem], (IFormatProvider) CultureInfo.InvariantCulture);
              this.Tag = (object) flag;
              IMapTextBox.Setvalue(imActionItem, flag.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + this.Tag?.ToString());
              if (imActionItem.IMAction.Type == KeyActionType.EdgeScroll && imActionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
                KMManager.AssignEdgeScrollMode(flag.ToString((IFormatProvider) CultureInfo.InvariantCulture), (TextBox) this);
              args.Handled = true;
            }
          }
        }
        this.Focus();
        args.Handled = true;
      }
      if (this.PropertyType.Equals(typeof (bool)))
        KMManager.CheckAndCreateNewScheme();
      this.SetCaretIndex();
      base.OnPreviewKeyDown(args);
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
    {
      if (args != null)
      {
        if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
        {
          foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
          {
            if (imActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
            {
              if (args.MiddleButton == MouseButtonState.Pressed)
              {
                args.Handled = true;
                this.Tag = (object) "MouseMButton";
                BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + "MouseMButton");
              }
              else if (args.RightButton == MouseButtonState.Pressed)
              {
                args.Handled = true;
                this.Tag = (object) "MouseRButton";
                BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + "MouseRButton");
              }
              else if (args.XButton1 == MouseButtonState.Pressed)
              {
                args.Handled = true;
                this.Tag = (object) "MouseXButton1";
                BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + "MouseXButton1");
              }
              else if (args.XButton2 == MouseButtonState.Pressed)
              {
                args.Handled = true;
                this.Tag = (object) "MouseXButton2";
                BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + "MouseXButton2");
              }
            }
            if (this.PropertyType.Equals(typeof (bool)))
            {
              bool flag = !Convert.ToBoolean(imActionItem.IMAction[imActionItem.ActionItem], (IFormatProvider) CultureInfo.InvariantCulture);
              this.Tag = (object) flag;
              IMapTextBox.Setvalue(imActionItem, flag.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              BlueStacksUIBinding.Bind((TextBox) this, Constants.ImapLocaleStringsConstant + this.Tag?.ToString());
              if (imActionItem.IMAction.Type == KeyActionType.EdgeScroll && imActionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
                KMManager.AssignEdgeScrollMode(flag.ToString((IFormatProvider) CultureInfo.InvariantCulture), (TextBox) this);
            }
          }
        }
        if (args.LeftButton == MouseButtonState.Pressed && this.IsKeyboardFocusWithin)
          args.Handled = true;
        this.Focus();
        args.Handled = true;
      }
      if (this.PropertyType.Equals(typeof (bool)))
        KMManager.CheckAndCreateNewScheme();
      this.SetCaretIndex();
      base.OnPreviewMouseDown(args);
    }

    protected override void OnKeyUp(KeyEventArgs args)
    {
      if (args != null)
      {
        if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
        {
          foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
          {
            if (imActionItem.IMAction.Type == KeyActionType.Tap || imActionItem.IMAction.Type == KeyActionType.TapRepeat || imActionItem.IMAction.Type == KeyActionType.Script)
            {
              if (this.mKeyList.Count >= 2)
              {
                string str = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1));
                this.Tag = (object) (IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1)));
                this.Text = str;
                this.SetValueHandling(imActionItem);
              }
              else if (this.mKeyList.Count == 1)
              {
                string stringForUi = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(0));
                this.Tag = (object) IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(0));
                this.Text = stringForUi;
                this.SetValueHandling(imActionItem);
              }
              this.mKeyList.Clear();
            }
          }
        }
        args.Handled = true;
      }
      if (this.PropertyType.Equals(typeof (bool)))
        KMManager.CheckAndCreateNewScheme();
      this.SetCaretIndex();
      base.OnKeyUp(args);
    }

    private void SetValueHandling(IMActionItem item)
    {
      string str = item.IMAction[item.ActionItem].ToString();
      if (this.IsLoaded)
        KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow, "true");
      if (this.PropertyType.Equals(typeof (double)))
      {
        if (double.TryParse(this.Text, out double _))
          str = this.Text;
        else if (!string.IsNullOrEmpty(this.Text))
          this.Text = str;
      }
      else if (this.PropertyType.Equals(typeof (int)))
      {
        if (int.TryParse(this.Text, out int _))
          str = this.Text;
        else if (!string.IsNullOrEmpty(this.Text))
          this.Text = str;
      }
      else
        str = !this.PropertyType.Equals(typeof (bool)) ? this.Tag.ToString() : this.Tag.ToString();
      IMapTextBox.Setvalue(item, str);
    }

    internal static void Setvalue(IMActionItem item, string value)
    {
      if (!string.Equals(item.IMAction[item.ActionItem].ToString(), value, StringComparison.InvariantCulture))
        item.IMAction[item.ActionItem] = (object) value;
      Logger.Debug("GUIDANCE: " + item.IMAction.Type.ToString());
    }

    private void SetCaretIndex()
    {
      if (string.IsNullOrEmpty(this.Text))
        return;
      this.CaretIndex = this.Text.Length;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/guidancemodels/imaptextbox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.mTextBox = (IMapTextBox) target;
      else
        this._contentLoaded = true;
    }
  }
}
