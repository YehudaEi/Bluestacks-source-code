// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroSettingsWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class MacroSettingsWindow : CustomWindow, IComponentConnector
  {
    private MainWindow ParentWindow;
    private MacroRecorderWindow mMacroRecorderWindow;
    private MacroRecording mRecording;
    internal Border mMaskBorder;
    internal TextBlock mSettingsHeaderText;
    internal StackPanel mRepeactActionPanel;
    internal CustomRadioButton mRepeatActionInSession;
    internal Grid mRepeatActionInSessionGrid;
    internal CustomTextBox mLoopCountTextBox;
    internal BlueStacks.Common.CustomPopUp mErrorNamePopup;
    internal Border mMaskBorder1;
    internal TextBlock mErrorText;
    internal Path mDownArrow;
    internal StackPanel mRepeatActionTimePanel;
    internal CustomRadioButton mRepeatActionTime;
    internal Grid mRepeatActionTimePanelGrid;
    internal CustomTextBox mLoopHours;
    internal CustomTextBox mLoopMinutes;
    internal CustomTextBox mLoopSeconds;
    internal StackPanel mRepeatSessionInfinitePanel;
    internal CustomRadioButton mRepeatSessionInfinite;
    internal CustomTextBox mLoopIntervalMinsTextBox;
    internal CustomTextBox mLoopIntervalSecondsTextBox;
    internal CustomComboBox mAccelerationCombobox;
    internal CustomCheckbox mPlayOnStartCheckBox;
    internal CustomCheckbox mRestartPlayerCheckBox;
    internal CustomTextBox mRestartPlayerIntervalTextBox;
    internal TextBlock mRestartTextBlock;
    internal CustomCheckbox mDonotShowWindowOnFinishCheckBox;
    internal CustomButton mSaveButton;
    private bool _contentLoaded;

    internal MacroSettingsWindow(
      MainWindow window,
      MacroRecording record,
      MacroRecorderWindow singleMacroControl)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Owner = (Window) this.ParentWindow;
      this.mMacroRecorderWindow = singleMacroControl;
      this.mRecording = record;
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopCountTextBox, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopHours, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopMinutes, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopSeconds, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopIntervalMinsTextBox, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mRestartPlayerIntervalTextBox, false);
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mLoopIntervalSecondsTextBox, false);
      this.InitSettings();
    }

    private void InitSettings()
    {
      this.mSettingsHeaderText.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) this.mRecording.Name, (object) LocaleStrings.GetLocalizedString("STRING_SETTINGS", "").ToLower(CultureInfo.InvariantCulture));
      this.mLoopCountTextBox.Text = this.mRecording.LoopNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mLoopHours.Text = (this.mRecording.LoopTime / 3600).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mLoopMinutes.Text = (this.mRecording.LoopTime / 60 % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mLoopSeconds.Text = (this.mRecording.LoopTime % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mLoopIntervalMinsTextBox.Text = (this.mRecording.LoopInterval / 60).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mLoopIntervalSecondsTextBox.Text = (this.mRecording.LoopInterval % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mRestartPlayerCheckBox.IsChecked = new bool?(this.mRecording.RestartPlayer);
      this.mPlayOnStartCheckBox.IsChecked = new bool?(this.mRecording.PlayOnStart);
      this.mDonotShowWindowOnFinishCheckBox.IsChecked = new bool?(this.mRecording.DonotShowWindowOnFinish);
      this.mRestartPlayerIntervalTextBox.Text = this.mRecording.RestartPlayerAfterMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.mAccelerationCombobox.Items.Clear();
      for (int index = -1; index <= 8; ++index)
      {
        ComboBoxItem comboBoxItem = new ComboBoxItem();
        double num = (double) (index + 2) * 0.5;
        comboBoxItem.Content = (object) (num.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x");
        this.mAccelerationCombobox.Items.Add((object) comboBoxItem);
      }
      if (this.mRecording.Acceleration == 0.0)
        this.mAccelerationCombobox.SelectedIndex = 1;
      else
        this.mAccelerationCombobox.SelectedIndex = (int) (this.mRecording.Acceleration / 0.5 - 1.0);
      this.mRestartTextBlock.ToolTip = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_AFTER", "") + " " + this.mRestartPlayerIntervalTextBox.Text + " " + LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", ""));
      this.SelectRepeatExecutionSetting();
      this.mLoopCountTextBox.TextChanged += new TextChangedEventHandler(this.LoopCountTextBox_TextChanged);
    }

    private void SelectRepeatExecutionSetting()
    {
      switch (this.mRecording.LoopType)
      {
        case OperationsLoopType.TillLoopNumber:
          this.mRepeatActionTimePanelGrid.IsEnabled = false;
          this.mRepeatActionInSession.IsChecked = new bool?(true);
          break;
        case OperationsLoopType.TillTime:
          this.mRepeatActionInSessionGrid.IsEnabled = false;
          this.mRepeatActionTime.IsChecked = new bool?(true);
          break;
        case OperationsLoopType.UntilStopped:
          this.mRepeatActionTimePanelGrid.IsEnabled = false;
          this.mRepeatActionInSessionGrid.IsEnabled = false;
          this.mRepeatSessionInfinite.IsChecked = new bool?(true);
          break;
      }
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.IsMacroSettingsChanged())
        this.GetUnsavedChangesWindow().ShowDialog();
      this.CloseWindow();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(this.mLoopHours.Text))
        this.mLoopHours.Text = "0";
      if (string.IsNullOrEmpty(this.mLoopMinutes.Text))
        this.mLoopMinutes.Text = "0";
      if (string.IsNullOrEmpty(this.mLoopSeconds.Text))
        this.mLoopSeconds.Text = "0";
      if (string.IsNullOrEmpty(this.mLoopCountTextBox.Text))
        this.mLoopCountTextBox.Text = "0";
      if (string.IsNullOrEmpty(this.mLoopIntervalMinsTextBox.Text))
        this.mLoopIntervalMinsTextBox.Text = "0";
      if (string.IsNullOrEmpty(this.mLoopIntervalSecondsTextBox.Text))
        this.mLoopIntervalSecondsTextBox.Text = "0";
      if (string.IsNullOrEmpty(this.mRestartPlayerIntervalTextBox.Text))
        this.mRestartPlayerIntervalTextBox.Text = "0";
      bool flag = this.IsMacroSettingsChanged();
      if (!string.IsNullOrEmpty(this.mRestartPlayerIntervalTextBox.Text) && int.Parse(this.mRestartPlayerIntervalTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture) > 0)
      {
        if (flag)
        {
          this.SaveScriptSettings();
          if (sender == null)
            return;
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, true);
        }
        else
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_CHANGES_SAVE", ""), 4.0, true);
      }
      else
      {
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_MACRO_RESTART_INTERVAL_NULL", ""), 4.0, true);
        this.mRestartPlayerIntervalTextBox.Text = this.mRecording.RestartPlayerAfterMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    private CustomMessageWindow GetUnsavedChangesWindow()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_TOOL", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE_WINDOW", "");
      customMessageWindow.IsWindowClosable = false;
      customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), (EventHandler) ((s, e) => this.SaveAndCloseWindow()), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.ParentWindow;
      return customMessageWindow;
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
    }

    private void CloseWindow()
    {
      this.Close();
      this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Hidden;
    }

    private void SaveAndCloseWindow()
    {
      this.SaveButton_Click((object) null, (RoutedEventArgs) null);
      this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.IsMacroSettingsChanged())
        this.GetUnsavedChangesWindow().ShowDialog();
      this.CloseWindow();
    }

    private void SaveScriptSettings()
    {
      try
      {
        this.mRecording.LoopType = this.FindLoopType();
        this.mRecording.Acceleration = (double) this.mAccelerationCombobox.SelectedIndex >= 0.0 ? (double) (this.mAccelerationCombobox.SelectedIndex + 1) * 0.5 : 1.0;
        bool? isChecked = this.mPlayOnStartCheckBox.IsChecked;
        bool flag = true;
        if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
        {
          if (this.ParentWindow.mAutoRunMacro != null)
          {
            this.ParentWindow.mAutoRunMacro.PlayOnStart = false;
            CommonHandlers.SaveMacroJson(this.ParentWindow.mAutoRunMacro, this.ParentWindow.mAutoRunMacro.Name + ".json");
          }
          foreach (SingleMacroControl child in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
          {
            if (this.ParentWindow.mAutoRunMacro != null && child.mScriptName.Text == this.ParentWindow.mAutoRunMacro.Name)
              child.mAutorunImage.Visibility = Visibility.Hidden;
            if (child.mScriptName.Text == this.mRecording.Name)
              child.mAutorunImage.Visibility = Visibility.Visible;
          }
          this.ParentWindow.mAutoRunMacro = this.mRecording;
        }
        this.mRecording.LoopTime = Convert.ToInt32(this.mLoopHours.Text, (IFormatProvider) CultureInfo.InvariantCulture) * 3600 + Convert.ToInt32(this.mLoopMinutes.Text, (IFormatProvider) CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(this.mLoopSeconds.Text, (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.mLoopCountTextBox.InputTextValidity == TextValidityOptions.Success)
          this.mRecording.LoopNumber = Convert.ToInt32(this.mLoopCountTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture);
        this.mRecording.LoopInterval = Convert.ToInt32(this.mLoopIntervalMinsTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(this.mLoopIntervalSecondsTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture);
        this.mRecording.PlayOnStart = Convert.ToBoolean((object) this.mPlayOnStartCheckBox.IsChecked, (IFormatProvider) CultureInfo.InvariantCulture);
        this.mRecording.DonotShowWindowOnFinish = Convert.ToBoolean((object) this.mDonotShowWindowOnFinishCheckBox.IsChecked, (IFormatProvider) CultureInfo.InvariantCulture);
        this.mRecording.RestartPlayer = Convert.ToBoolean((object) this.mRestartPlayerCheckBox.IsChecked, (IFormatProvider) CultureInfo.InvariantCulture);
        this.mRecording.RestartPlayerAfterMinutes = Convert.ToInt32(this.mRestartPlayerIntervalTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.mRecording.RecordingType == RecordingTypes.SingleRecording)
          CommonHandlers.SaveMacroJson(this.mRecording, this.mRecording.Name + ".json");
        CommonHandlers.RefreshAllMacroRecorderWindow();
        CommonHandlers.OnMacroSettingChanged(this.mRecording);
        this.InitSettings();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in saving macro settings: " + ex.ToString());
      }
    }

    private OperationsLoopType FindLoopType()
    {
      if (this.mRepeatActionInSession.IsChecked.Value)
        return OperationsLoopType.TillLoopNumber;
      return this.mRepeatActionTime.IsChecked.Value ? OperationsLoopType.TillTime : OperationsLoopType.UntilStopped;
    }

    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = !this.IsTextAllowed(e.Text);
    }

    private bool IsTextAllowed(string text)
    {
      return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
    }

    private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof (string)))
      {
        if (this.IsTextAllowed((string) e.DataObject.GetData(typeof (string))))
          return;
        e.CancelCommand();
      }
      else
        e.CancelCommand();
    }

    private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Space)
        return;
      e.Handled = true;
    }

    private bool IsMacroSettingsChanged()
    {
      if (this.mLoopHours.Text != (this.mRecording.LoopTime / 3600).ToString((IFormatProvider) CultureInfo.InvariantCulture) || this.mLoopMinutes.Text != (this.mRecording.LoopTime / 60 % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture) || (this.mLoopSeconds.Text != (this.mRecording.LoopTime % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture) || this.mLoopCountTextBox.Text != this.mRecording.LoopNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture)) || (this.mLoopIntervalMinsTextBox.Text != (this.mRecording.LoopInterval / 60).ToString((IFormatProvider) CultureInfo.InvariantCulture) || this.mLoopIntervalSecondsTextBox.Text != (this.mRecording.LoopInterval % 60).ToString((IFormatProvider) CultureInfo.InvariantCulture)))
        return true;
      bool? isChecked1 = this.mRestartPlayerCheckBox.IsChecked;
      bool restartPlayer = this.mRecording.RestartPlayer;
      if (!(isChecked1.GetValueOrDefault() == restartPlayer & isChecked1.HasValue))
        return true;
      bool? isChecked2 = this.mPlayOnStartCheckBox.IsChecked;
      bool playOnStart = this.mRecording.PlayOnStart;
      if (!(isChecked2.GetValueOrDefault() == playOnStart & isChecked2.HasValue))
        return true;
      bool? isChecked3 = this.mDonotShowWindowOnFinishCheckBox.IsChecked;
      bool showWindowOnFinish = this.mRecording.DonotShowWindowOnFinish;
      return !(isChecked3.GetValueOrDefault() == showWindowOnFinish & isChecked3.HasValue) || (this.mRestartPlayerIntervalTextBox.Text != this.mRecording.RestartPlayerAfterMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture) || this.FindLoopType() != this.mRecording.LoopType || this.mAccelerationCombobox.SelectedIndex != 1 && this.mRecording.Acceleration == 0.0 || this.mAccelerationCombobox.SelectedIndex != (int) (this.mRecording.Acceleration / 0.5 - 1.0));
    }

    private void RestartTextBlock_ToolTipOpening(object sender, ToolTipEventArgs e)
    {
      this.mRestartTextBlock.ToolTip = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_AFTER", "") + " " + this.mRestartPlayerIntervalTextBox.Text + " " + LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", ""));
    }

    private void RepeatAction_Checked(object sender, RoutedEventArgs e)
    {
      switch (this.FindLoopType())
      {
        case OperationsLoopType.TillLoopNumber:
          this.mRepeatActionTimePanelGrid.IsEnabled = false;
          this.mRepeatActionInSessionGrid.IsEnabled = true;
          this.mRepeatActionInSession.IsChecked = new bool?(true);
          break;
        case OperationsLoopType.TillTime:
          this.mRepeatActionInSessionGrid.IsEnabled = false;
          this.mRepeatActionTimePanelGrid.IsEnabled = true;
          this.mRepeatActionTime.IsChecked = new bool?(true);
          break;
        case OperationsLoopType.UntilStopped:
          this.mRepeatActionTimePanelGrid.IsEnabled = false;
          this.mRepeatActionInSessionGrid.IsEnabled = false;
          this.mRepeatSessionInfinite.IsChecked = new bool?(true);
          break;
      }
    }

    private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      CustomTextBox customTextBox = sender as CustomTextBox;
      customTextBox.InputTextValidity = string.IsNullOrEmpty(customTextBox.Text) || Convert.ToInt32(this.mLoopCountTextBox.Text, (IFormatProvider) CultureInfo.InvariantCulture) == 0 ? TextValidityOptions.Error : TextValidityOptions.Success;
      this.mErrorNamePopup.IsOpen = customTextBox.InputTextValidity == TextValidityOptions.Error;
      this.mSaveButton.IsEnabled = customTextBox.InputTextValidity == TextValidityOptions.Success;
    }

    private void LoopCountTextBox_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mLoopCountTextBox.InputTextValidity == TextValidityOptions.Error)
      {
        this.mErrorNamePopup.IsOpen = true;
        this.mErrorNamePopup.StaysOpen = true;
      }
      else
        this.mErrorNamePopup.IsOpen = false;
    }

    private void LoopCountTextBox_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mErrorNamePopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrosettingswindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMaskBorder = (Border) target;
          break;
        case 2:
          this.mSettingsHeaderText = (TextBlock) target;
          this.mSettingsHeaderText.MouseDown += new MouseButtonEventHandler(this.TopBar_MouseDown);
          break;
        case 3:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 4:
          this.mRepeactActionPanel = (StackPanel) target;
          break;
        case 5:
          this.mRepeatActionInSession = (CustomRadioButton) target;
          this.mRepeatActionInSession.Checked += new RoutedEventHandler(this.RepeatAction_Checked);
          break;
        case 6:
          this.mRepeatActionInSessionGrid = (Grid) target;
          break;
        case 7:
          this.mLoopCountTextBox = (CustomTextBox) target;
          this.mLoopCountTextBox.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopCountTextBox.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopCountTextBox.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          this.mLoopCountTextBox.MouseEnter += new MouseEventHandler(this.LoopCountTextBox_MouseEnter);
          this.mLoopCountTextBox.MouseLeave += new MouseEventHandler(this.LoopCountTextBox_MouseLeave);
          break;
        case 8:
          this.mErrorNamePopup = (BlueStacks.Common.CustomPopUp) target;
          break;
        case 9:
          this.mMaskBorder1 = (Border) target;
          break;
        case 10:
          this.mErrorText = (TextBlock) target;
          break;
        case 11:
          this.mDownArrow = (Path) target;
          break;
        case 12:
          this.mRepeatActionTimePanel = (StackPanel) target;
          break;
        case 13:
          this.mRepeatActionTime = (CustomRadioButton) target;
          this.mRepeatActionTime.Checked += new RoutedEventHandler(this.RepeatAction_Checked);
          break;
        case 14:
          this.mRepeatActionTimePanelGrid = (Grid) target;
          break;
        case 15:
          this.mLoopHours = (CustomTextBox) target;
          this.mLoopHours.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopHours.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopHours.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 16:
          this.mLoopMinutes = (CustomTextBox) target;
          this.mLoopMinutes.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopMinutes.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopMinutes.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 17:
          this.mLoopSeconds = (CustomTextBox) target;
          this.mLoopSeconds.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopSeconds.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopSeconds.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 18:
          this.mRepeatSessionInfinitePanel = (StackPanel) target;
          break;
        case 19:
          this.mRepeatSessionInfinite = (CustomRadioButton) target;
          this.mRepeatSessionInfinite.Checked += new RoutedEventHandler(this.RepeatAction_Checked);
          break;
        case 20:
          this.mLoopIntervalMinsTextBox = (CustomTextBox) target;
          this.mLoopIntervalMinsTextBox.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopIntervalMinsTextBox.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopIntervalMinsTextBox.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 21:
          this.mLoopIntervalSecondsTextBox = (CustomTextBox) target;
          this.mLoopIntervalSecondsTextBox.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mLoopIntervalSecondsTextBox.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mLoopIntervalSecondsTextBox.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 22:
          this.mAccelerationCombobox = (CustomComboBox) target;
          break;
        case 23:
          this.mPlayOnStartCheckBox = (CustomCheckbox) target;
          break;
        case 24:
          this.mRestartPlayerCheckBox = (CustomCheckbox) target;
          break;
        case 25:
          this.mRestartPlayerIntervalTextBox = (CustomTextBox) target;
          this.mRestartPlayerIntervalTextBox.PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          this.mRestartPlayerIntervalTextBox.AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          this.mRestartPlayerIntervalTextBox.PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 26:
          this.mRestartTextBlock = (TextBlock) target;
          this.mRestartTextBlock.ToolTipOpening += new ToolTipEventHandler(this.RestartTextBlock_ToolTipOpening);
          break;
        case 27:
          this.mDonotShowWindowOnFinishCheckBox = (CustomCheckbox) target;
          break;
        case 28:
          this.mSaveButton = (CustomButton) target;
          this.mSaveButton.Click += new RoutedEventHandler(this.SaveButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
