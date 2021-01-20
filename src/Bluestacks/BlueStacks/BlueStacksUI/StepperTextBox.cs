// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.StepperTextBox
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class StepperTextBox : XTextBox, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(nameof (PropertyType), typeof (Type), typeof (StepperTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (int)));
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof (MaxValue), typeof (double), typeof (StepperTextBox));
    public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register(nameof (IMActionItems), typeof (ObservableCollection<IMActionItem>), typeof (StepperTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) new ObservableCollection<IMActionItem>()));
    private bool _contentLoaded;

    public StepperTextBox()
    {
      this.InitializeComponent();
      this.ClearValue(StepperTextBox.IMActionItemsProperty);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this, false);
    }

    public Type PropertyType
    {
      get
      {
        return (Type) this.GetValue(StepperTextBox.PropertyTypeProperty);
      }
      set
      {
        this.SetValue(StepperTextBox.PropertyTypeProperty, (object) value);
      }
    }

    public double MaxValue
    {
      get
      {
        return (double) this.GetValue(StepperTextBox.MaxValueProperty);
      }
      set
      {
        this.SetValue(StepperTextBox.MaxValueProperty, (object) value);
      }
    }

    public double MinValue { get; set; }

    public ObservableCollection<IMActionItem> IMActionItems
    {
      get
      {
        return (ObservableCollection<IMActionItem>) this.GetValue(StepperTextBox.IMActionItemsProperty);
      }
      set
      {
        if (value == null)
          this.ClearValue(StepperTextBox.IMActionItemsProperty);
        else
          this.SetValue(StepperTextBox.IMActionItemsProperty, (object) value);
      }
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs args)
    {
      if (args != null)
      {
        string s;
        if (this.SelectionLength > 0)
        {
          StringBuilder stringBuilder = new StringBuilder(this.Text);
          stringBuilder.Remove(this.SelectionStart, this.SelectionLength);
          stringBuilder.Insert(this.SelectionStart, args.Text);
          s = stringBuilder.ToString();
        }
        else
          s = this.Text.Insert(this.SelectionStart, args.Text);
        if ((object) this.PropertyType == (object) typeof (int))
        {
          int result;
          args.Handled = !int.TryParse(s, out result) || (double) result < this.MinValue || (double) result > this.MaxValue;
        }
        else if ((object) this.PropertyType == (object) typeof (double))
        {
          string str = s.Replace(',', '.');
          double result;
          if (double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
          {
            args.Handled = this.MinValue > result || result > this.MaxValue;
          }
          else
          {
            if (string.Equals(str, ".", StringComparison.InvariantCultureIgnoreCase))
            {
              this.Text = "0.";
              KMManager.CheckAndCreateNewScheme();
              if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
              {
                foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
                  this.SetValueHandling(imActionItem);
              }
              this.CaretIndex = this.Text.Length;
            }
            args.Handled = true;
          }
        }
      }
      base.OnPreviewTextInput(args);
    }

    private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      e.Handled = e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste;
    }

    private void OnIncrease(object sender, RoutedEventArgs e)
    {
      string s = this.Text.Replace(',', '.');
      double result1;
      if ((object) this.PropertyType == (object) typeof (double) && double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
      {
        if (this.CanIncrease(result1, 0.05))
        {
          this.Text = (result1 + 0.05).ToString((IFormatProvider) CultureInfo.CurrentCulture);
          KMManager.CheckAndCreateNewScheme();
        }
      }
      else
      {
        int result2;
        if ((object) this.PropertyType == (object) typeof (int) && int.TryParse(s, out result2) && this.CanIncrease((double) result2, 1.0))
        {
          ++result2;
          this.Text = result2.ToString((IFormatProvider) CultureInfo.CurrentCulture);
          KMManager.CheckAndCreateNewScheme();
        }
      }
      foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
        this.SetValueHandling(imActionItem);
    }

    private bool CanIncrease(double doubleVal, double val)
    {
      return doubleVal + val <= this.MaxValue;
    }

    private bool CanDecrease(double doubleVal, double val)
    {
      return doubleVal - val >= this.MinValue;
    }

    private void OnDecrease(object sender, RoutedEventArgs e)
    {
      string s = this.Text.Replace(',', '.');
      double result1;
      if ((object) this.PropertyType == (object) typeof (double) && double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1) && result1 > 0.0)
      {
        if (this.CanDecrease(result1, 0.05))
        {
          this.Text = (result1 - 0.05).ToString((IFormatProvider) CultureInfo.CurrentCulture);
          KMManager.CheckAndCreateNewScheme();
        }
      }
      else
      {
        int result2;
        if ((object) this.PropertyType == (object) typeof (int) && int.TryParse(s, out result2) && (result2 > 0 && this.CanDecrease((double) result2, 1.0)))
        {
          --result2;
          this.Text = result2.ToString((IFormatProvider) CultureInfo.CurrentCulture);
          KMManager.CheckAndCreateNewScheme();
        }
      }
      foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
        this.SetValueHandling(imActionItem);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs args)
    {
      if (args != null)
      {
        if (args.Key == Key.Space)
          args.Handled = true;
        else if (args.Key == Key.Return || args.Key == Key.Return)
        {
          TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
          if (Keyboard.FocusedElement is UIElement focusedElement && focusedElement.MoveFocus(request))
            args.Handled = true;
        }
        else
        {
          if (args.Key == Key.Escape)
            return;
          this.Focus();
        }
      }
      base.OnPreviewKeyDown(args);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      this.TextChanged -= new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      this.TextChanged += new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      this.CaretIndex = this.Text.Length;
    }

    private void IMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.IMActionItems == null || !this.IMActionItems.Any<IMActionItem>())
        return;
      foreach (IMActionItem imActionItem in (Collection<IMActionItem>) this.IMActionItems)
        this.SetValueHandling(imActionItem);
      KMManager.CheckAndCreateNewScheme();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      this.TextChanged -= new TextChangedEventHandler(this.IMapTextBox_TextChanged);
      base.OnLostFocus(e);
    }

    private void SetValueHandling(IMActionItem item)
    {
      string str = item.IMAction[item.ActionItem].ToString();
      if (this.PropertyType.Equals(typeof (double)))
      {
        string s = this.Text.Replace(',', '.');
        if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out double _))
          str = s;
        else if (!string.IsNullOrEmpty(s))
          this.Text = str;
      }
      else if (this.PropertyType.Equals(typeof (Decimal)))
      {
        if (Decimal.TryParse(this.Text, out Decimal _))
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
      this.Setvalue(item, str);
    }

    internal void Setvalue(IMActionItem item, string value)
    {
      string a = item.IMAction[item.ActionItem].ToString().Replace(',', '.');
      if (!string.Equals(a, value, StringComparison.InvariantCulture))
        item.IMAction[item.ActionItem] = (object) value;
      try
      {
        if (string.Equals(item.ActionItem, "Sensitivity", StringComparison.InvariantCulture))
        {
          double num1 = Convert.ToDouble(item.IMAction["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture);
          double num2 = Convert.ToDouble(a, (IFormatProvider) CultureInfo.InvariantCulture);
          double num3 = Convert.ToDouble(item.IMAction[item.ActionItem], (IFormatProvider) CultureInfo.InvariantCulture);
          double num4 = num2 == 0.0 ? num1 : num2 * num1;
          if (num1 != 0.0)
            item.IMAction["SensitivityRatioY"] = num3 == 0.0 || num4 == 0.0 ? (object) num4.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) (num4 / num3).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
        else if (string.Equals(item.ActionItem, "SensitivityRatioY", StringComparison.InvariantCulture))
        {
          double num1 = Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture);
          double num2 = Convert.ToDouble(item.IMAction["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture);
          item.IMAction[item.ActionItem] = num1 == 0.0 ? (object) "0" : (num2 == 0.0 ? (object) num1.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) (num1 / num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Unable to correctly set Sensitivity ratio: " + ex?.ToString());
      }
      if (item.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
        this.Text = this.Text.ToUpper(CultureInfo.InvariantCulture);
      if (item.ActionItem.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
        this.Text = this.Text.ToUpper(CultureInfo.InvariantCulture);
      Logger.Debug("GUIDANCE: " + item.IMAction.Type.ToString());
    }

    private void Path_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Path), Shape.FillProperty, "SettingsWindowTabMenuItemLegendForeground");
    }

    private void Path_MouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Path), Shape.FillProperty, "SettingsWindowForegroundDimColor");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/guidancemodels/steppertextbox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        ((UIElement) target).AddHandler(CommandManager.PreviewExecutedEvent, (Delegate) new ExecutedRoutedEventHandler(this.OnPreviewExecuted));
      else
        this._contentLoaded = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnIncrease);
          break;
        case 3:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Path_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Path_MouseLeave);
          break;
        case 4:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnDecrease);
          break;
        case 5:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Path_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Path_MouseLeave);
          break;
      }
    }
  }
}
