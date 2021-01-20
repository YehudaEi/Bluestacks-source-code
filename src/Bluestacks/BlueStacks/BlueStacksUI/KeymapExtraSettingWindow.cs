// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.KeymapExtraSettingWindow
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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class KeymapExtraSettingWindow : CustomPopUp, IComponentConnector
  {
    internal Dictionary<string, GroupBox> mDictGroupBox = new Dictionary<string, GroupBox>();
    internal Dictionary<string, DualTextBlockControl> mDictDualTextBox = new Dictionary<string, DualTextBlockControl>();
    private List<string> mListSuggestions = new List<string>();
    internal CanvasElement mCanvasElement;
    private StackPanel mStackPanel;
    private MainWindow ParentWindow;
    private DualTextBlockControl mLookAroundXDTB;
    private DualTextBlockControl mLookAroundYDTB;
    private DualTextBlockControl mLookAroundDTB;
    private DualTextBlockControl mLookAroundXExprDTB;
    private DualTextBlockControl mLookAroundYExprDTB;
    private DualTextBlockControl mLookAroundXOffsetDTB;
    private DualTextBlockControl mLookAroundYOffsetDTB;
    private DualTextBlockControl mLookAroundShowOnOverlayDTB;
    private DualTextBlockControl mShootXDTB;
    private DualTextBlockControl mShootYDTB;
    private DualTextBlockControl mShootDTB;
    private DualTextBlockControl mShootXExprDTB;
    private DualTextBlockControl mShootYExprDTB;
    private DualTextBlockControl mShootXOffsetDTB;
    private DualTextBlockControl mShootYOffsetDTB;
    private DualTextBlockControl mShootShowOnOverlayDTB;
    private DualTextBlockControl mMOBASkillCancelDTB;
    private DualTextBlockControl mMOBASkillCancelXExprDTB;
    private DualTextBlockControl mMOBASkillCancelYExprDTB;
    private DualTextBlockControl mMOBASkillCancelXOffsetDTB;
    private DualTextBlockControl mMOBASkillCancelYOffsetDTB;
    private DualTextBlockControl mMOBASkillCancelShowOnOverlayDTB;
    private DualTextBlockControl mEnableConditionTB;
    private DualTextBlockControl mStartConditionTB;
    private DualTextBlockControl mNoteTB;
    internal Grid mHeaderGrid;
    internal TextBlock mHeader;
    internal CustomScrollViewer mScrollBar;
    internal CustomButton mDeleteButton;
    internal Grid mDummyGrid;
    internal GroupBox mMOBAGB;
    internal StackPanel mMOBAPanel;
    internal CustomPictureBox mMOBAPB;
    internal GroupBox mGuidanceCategory;
    internal AutoCompleteComboBox mGuidanceCategoryComboBox;
    internal GroupBox mTabsGrid;
    internal Border mKeyboardTabBorder;
    internal TextBlock keyboardBtn;
    internal Border mGamepadTabBorder;
    internal TextBlock gamepadBtn;
    internal GroupBox mMOBASkillCancelGB;
    internal StackPanel mMOBASkillCancelGBPanel;
    internal CustomCheckbox mMOBASkillCancelCB;
    internal CustomPictureBox mMOBASkillCancelPB;
    internal GroupBox mLookAroundGB;
    internal StackPanel mLookAroundPanel;
    internal CustomCheckbox mLookAroundCB;
    internal CustomPictureBox mLookAroundPB;
    internal GroupBox mShootGB;
    internal StackPanel mShootGBPanel;
    internal CustomCheckbox mShootCB;
    internal CustomPictureBox mShootPB;
    internal GroupBox mSchemesGB;
    internal GroupBox mEnableConditionGB;
    internal GroupBox mNoteGB;
    internal GroupBox mStartConditionGB;
    internal GroupBox mOverlayGB;
    internal CustomCheckbox mOverlayCB;
    internal Canvas mCanvas;
    private bool _contentLoaded;

    public List<IMAction> ListAction { get; } = new List<IMAction>();

    public KeymapExtraSettingWindow(MainWindow window)
    {
      this.InitializeComponent();
      this.IsFocusOnMouseClick = true;
      this.ParentWindow = window;
      this.mStackPanel = this.mScrollBar.Content as StackPanel;
      this.AddGuidanceCategories();
      this.AddDualTextBlockControl();
      this.SetPopupDraggableProperty();
    }

    private void AddDualTextBlockControl()
    {
      this.AddDualTextBlockControlToLookAroundPanel();
      this.AddDualTextBlockControlToShootGBPanel();
      this.AddDualTextBlockControlToMOBASkillCancelGBPanel();
      this.AddDualTextBlockControlToGroupBox();
    }

    private void AddDualTextBlockControlToGroupBox()
    {
      DualTextBlockControl textBlockControl1 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl1.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      textBlockControl1.VerticalAlignment = VerticalAlignment.Top;
      textBlockControl1.HorizontalAlignment = HorizontalAlignment.Stretch;
      textBlockControl1.Height = 32.0;
      this.mEnableConditionTB = textBlockControl1;
      this.mEnableConditionGB.Content = (object) this.mEnableConditionTB;
      DualTextBlockControl textBlockControl2 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl2.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      textBlockControl2.VerticalAlignment = VerticalAlignment.Top;
      textBlockControl2.HorizontalAlignment = HorizontalAlignment.Stretch;
      textBlockControl2.Height = 32.0;
      this.mNoteTB = textBlockControl2;
      this.mNoteGB.Content = (object) this.mNoteTB;
      DualTextBlockControl textBlockControl3 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl3.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      textBlockControl3.VerticalAlignment = VerticalAlignment.Top;
      textBlockControl3.HorizontalAlignment = HorizontalAlignment.Stretch;
      textBlockControl3.Height = 32.0;
      this.mStartConditionTB = textBlockControl3;
      this.mStartConditionGB.Content = (object) this.mStartConditionTB;
    }

    internal void AddDualTextBlockControlToMOBASkillCancelGBPanel()
    {
      DualTextBlockControl textBlockControl1 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl1.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelDTB = textBlockControl1;
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelDTB);
      if (!KMManager.sIsDeveloperModeOn)
        return;
      DualTextBlockControl textBlockControl2 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl2.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelXExprDTB = textBlockControl2;
      DualTextBlockControl textBlockControl3 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl3.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelYExprDTB = textBlockControl3;
      DualTextBlockControl textBlockControl4 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl4.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelXOffsetDTB = textBlockControl4;
      DualTextBlockControl textBlockControl5 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl5.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelYOffsetDTB = textBlockControl5;
      DualTextBlockControl textBlockControl6 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl6.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mMOBASkillCancelShowOnOverlayDTB = textBlockControl6;
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelXExprDTB);
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelYExprDTB);
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelXOffsetDTB);
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelYOffsetDTB);
      this.mMOBASkillCancelGBPanel.Children.Add((UIElement) this.mMOBASkillCancelShowOnOverlayDTB);
    }

    private void AddDualTextBlockControlToShootGBPanel()
    {
      DualTextBlockControl textBlockControl1 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl1.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootXDTB = textBlockControl1;
      DualTextBlockControl textBlockControl2 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl2.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootYDTB = textBlockControl2;
      DualTextBlockControl textBlockControl3 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl3.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootDTB = textBlockControl3;
      this.mShootGBPanel.Children.Add((UIElement) this.mShootXDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootYDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootDTB);
      if (!KMManager.sIsDeveloperModeOn)
        return;
      DualTextBlockControl textBlockControl4 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl4.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootXExprDTB = textBlockControl4;
      DualTextBlockControl textBlockControl5 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl5.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootYExprDTB = textBlockControl5;
      DualTextBlockControl textBlockControl6 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl6.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootXOffsetDTB = textBlockControl6;
      DualTextBlockControl textBlockControl7 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl7.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootYOffsetDTB = textBlockControl7;
      DualTextBlockControl textBlockControl8 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl8.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mShootShowOnOverlayDTB = textBlockControl8;
      this.mShootGBPanel.Children.Add((UIElement) this.mShootXExprDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootYExprDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootXOffsetDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootYOffsetDTB);
      this.mShootGBPanel.Children.Add((UIElement) this.mShootShowOnOverlayDTB);
    }

    private void AddDualTextBlockControlToLookAroundPanel()
    {
      DualTextBlockControl textBlockControl1 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl1.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundXDTB = textBlockControl1;
      DualTextBlockControl textBlockControl2 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl2.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundYDTB = textBlockControl2;
      DualTextBlockControl textBlockControl3 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl3.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundDTB = textBlockControl3;
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundXDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundYDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundDTB);
      if (!KMManager.sIsDeveloperModeOn)
        return;
      DualTextBlockControl textBlockControl4 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl4.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundXExprDTB = textBlockControl4;
      DualTextBlockControl textBlockControl5 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl5.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundYExprDTB = textBlockControl5;
      DualTextBlockControl textBlockControl6 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl6.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundXOffsetDTB = textBlockControl6;
      DualTextBlockControl textBlockControl7 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl7.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundYOffsetDTB = textBlockControl7;
      DualTextBlockControl textBlockControl8 = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl8.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      this.mLookAroundShowOnOverlayDTB = textBlockControl8;
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundXExprDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundYExprDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundXOffsetDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundYOffsetDTB);
      this.mLookAroundPanel.Children.Add((UIElement) this.mLookAroundShowOnOverlayDTB);
    }

    private void AddControls(DualTextBlockControl control, StackPanel panel)
    {
      DualTextBlockControl textBlockControl = new DualTextBlockControl(this.ParentWindow, (KeymapExtraSettingWindow) null);
      textBlockControl.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      control = textBlockControl;
      panel.Children.Add((UIElement) control);
    }

    private void AddGuidanceCategories()
    {
      this.mListSuggestions.Clear();
      foreach (IMAction gameControl in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
      {
        string str = !string.Equals(gameControl.GuidanceCategory, "MISC", StringComparison.InvariantCulture) ? this.ParentWindow.SelectedConfig.GetUIString(gameControl.GuidanceCategory) : LocaleStrings.GetLocalizedString("STRING_" + gameControl.GuidanceCategory, "");
        if (!this.mListSuggestions.Contains(str))
          this.mListSuggestions.Add(str);
      }
    }

    private void AddListOfSuggestions()
    {
      this.mGuidanceCategoryComboBox.AddSuggestions(this.mListSuggestions);
    }

    private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.IsOpen = false;
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      KeymapCanvasWindow.sIsDirty = true;
      foreach (IMAction imAction in this.ListAction)
        this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(imAction);
      (this.mCanvasElement.Parent as Canvas).Children.Remove((UIElement) this.mCanvasElement);
      this.IsOpen = false;
      foreach (KeyValuePair<IMAction, CanvasElement> keyValuePair in KMManager.CanvasWindow.dictCanvasElement)
      {
        if (keyValuePair.Key.ParentAction == this.ListAction.First<IMAction>())
        {
          keyValuePair.Value.RemoveAction("");
          this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(keyValuePair.Value.ListActionItem.First<IMAction>());
        }
      }
    }

    private void SetPopupDraggableProperty()
    {
      try
      {
        Thumb thumb = new Thumb();
        thumb.Width = 0.0;
        thumb.Height = 0.0;
        Thumb Thumb = thumb;
        this.mHeaderGrid.Children.Add((UIElement) Thumb);
        this.mHeaderGrid.MouseLeftButtonDown -= new MouseButtonEventHandler(mouseDownHandler);
        this.mHeaderGrid.MouseLeftButtonDown += new MouseButtonEventHandler(mouseDownHandler);
        // ISSUE: method pointer
        Thumb.DragDelta -= new DragDeltaEventHandler((object) this, __methodptr(\u003CSetPopupDraggableProperty\u003Eg__deltaEventHandler\u007C45_1));
        // ISSUE: method pointer
        Thumb.DragDelta += new DragDeltaEventHandler((object) this, __methodptr(\u003CSetPopupDraggableProperty\u003Eg__deltaEventHandler\u007C45_1));

        void mouseDownHandler(object o, MouseButtonEventArgs e)
        {
          Thumb.RaiseEvent((RoutedEventArgs) e);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in draggable popup: " + ex.ToString());
      }
    }

    internal void Init(bool isGamepadTabSelected = false)
    {
      bool sIsDirty = KeymapCanvasWindow.sIsDirty;
      this.mDictGroupBox.Clear();
      this.mDummyGrid.Children.Clear();
      this.mStackPanel.Children.Clear();
      this.mDictDualTextBox.Clear();
      BlueStacksUIBinding.Bind(this.mHeader, Constants.ImapLocaleStringsConstant + this.ListAction.First<IMAction>().Type.ToString() + "_Settings", "");
      if (KMManager.sIsDeveloperModeOn)
      {
        this.mEnableConditionTB.Visibility = Visibility.Visible;
        this.mEnableConditionGB.Visibility = Visibility.Visible;
        this.mEnableConditionTB.ActionItemProperty = "EnableCondition";
        this.mStartConditionTB.Visibility = Visibility.Visible;
        this.mStartConditionGB.Visibility = Visibility.Visible;
        this.mStartConditionTB.ActionItemProperty = "StartCondition";
        this.mNoteTB.Visibility = Visibility.Visible;
        this.mNoteGB.Visibility = Visibility.Visible;
        this.mNoteTB.ActionItemProperty = "Note";
      }
      this.AddListOfSuggestions();
      this.mStackPanel.Children.Add((UIElement) this.mGuidanceCategory);
      this.mStackPanel.Children.Add((UIElement) this.mTabsGrid);
      if (isGamepadTabSelected)
      {
        this.mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
        this.mKeyboardTabBorder.Background = (Brush) Brushes.Transparent;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
        this.mGamepadTabBorder.BorderThickness = new Thickness(0.0);
      }
      else
      {
        this.mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
        this.mGamepadTabBorder.Background = (Brush) Brushes.Transparent;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
        this.mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
      }
      foreach (IMAction action in this.ListAction)
      {
        if (string.Equals(action.GuidanceCategory, "MISC", StringComparison.InvariantCulture))
          this.mGuidanceCategoryComboBox.mAutoComboBox.Text = LocaleStrings.GetLocalizedString("STRING_" + action.GuidanceCategory, "");
        else
          this.mGuidanceCategoryComboBox.mAutoComboBox.Text = this.ParentWindow.SelectedConfig.GetUIString(action.GuidanceCategory);
        if (KMManager.sIsDeveloperModeOn)
        {
          this.mEnableConditionGB.Visibility = Visibility.Visible;
          this.mEnableConditionTB.Visibility = Visibility.Visible;
          this.mEnableConditionTB.AddActionItem(action);
          this.mNoteGB.Visibility = Visibility.Visible;
          this.mNoteTB.Visibility = Visibility.Visible;
          this.mNoteTB.AddActionItem(action);
          this.mStartConditionGB.Visibility = Visibility.Visible;
          this.mStartConditionTB.Visibility = Visibility.Visible;
          this.mStartConditionTB.AddActionItem(action);
        }
        else
        {
          this.mEnableConditionGB.Visibility = Visibility.Collapsed;
          this.mStartConditionGB.Visibility = Visibility.Collapsed;
          this.mNoteGB.Visibility = Visibility.Collapsed;
        }
        foreach (KeyValuePair<string, PropertyInfo> keyValuePair in IMAction.DictPopUpUIElements[action.Type])
        {
          if (string.Equals(keyValuePair.Key, "IsCancelSkillEnabled", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mStackPanel.Children.Add((UIElement) this.mMOBASkillCancelGB);
            this.mMOBASkillCancelCB.IsChecked = new bool?(Convert.ToBoolean(action[keyValuePair.Key], (IFormatProvider) CultureInfo.InvariantCulture));
            bool flag = this.mMOBASkillCancelCB.IsChecked.Value;
            this.mMOBASkillCancelDTB.IsEnabled = flag;
            this.mMOBASkillCancelDTB.ActionItemProperty = !isGamepadTabSelected ? "KeyCancel" : "KeyCancel_alt1";
            this.mMOBASkillCancelDTB.AddActionItem(action);
            if (KMManager.sIsDeveloperModeOn)
            {
              this.mMOBASkillCancelXExprDTB.IsEnabled = flag;
              this.mMOBASkillCancelXExprDTB.ActionItemProperty = "CancelXExpr";
              this.mMOBASkillCancelXExprDTB.AddActionItem(action);
              this.mMOBASkillCancelYExprDTB.IsEnabled = flag;
              this.mMOBASkillCancelYExprDTB.ActionItemProperty = "CancelYExpr";
              this.mMOBASkillCancelYExprDTB.AddActionItem(action);
              this.mMOBASkillCancelXOffsetDTB.IsEnabled = flag;
              this.mMOBASkillCancelXOffsetDTB.ActionItemProperty = "CancelXOverlayOffset";
              this.mMOBASkillCancelXOffsetDTB.AddActionItem(action);
              this.mMOBASkillCancelYOffsetDTB.IsEnabled = flag;
              this.mMOBASkillCancelYOffsetDTB.ActionItemProperty = "CancelYOverlayOffset";
              this.mMOBASkillCancelYOffsetDTB.AddActionItem(action);
              this.mMOBASkillCancelShowOnOverlayDTB.IsEnabled = flag;
              this.mMOBASkillCancelShowOnOverlayDTB.ActionItemProperty = "CancelShowOnOverlayExpr";
              this.mMOBASkillCancelShowOnOverlayDTB.AddActionItem(action);
            }
          }
          else if (string.Equals(keyValuePair.Key, "IsLookAroundEnabled", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mStackPanel.Children.Add((UIElement) this.mLookAroundGB);
            this.mLookAroundCB.IsChecked = new bool?(Convert.ToBoolean(action[keyValuePair.Key], (IFormatProvider) CultureInfo.InvariantCulture));
            bool isEnabled = this.mLookAroundCB.IsChecked.Value;
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundDTB, isEnabled, "KeyLookAround");
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundXDTB, isEnabled, "LookAroundX");
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundYDTB, isEnabled, "LookAroundY");
            if (KMManager.sIsDeveloperModeOn)
            {
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundXExprDTB, isEnabled, "LookAroundXExpr");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundYExprDTB, isEnabled, "LookAroundYExpr");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundXOffsetDTB, isEnabled, "LookAroundXOverlayOffset");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundYOffsetDTB, isEnabled, "LookAroundYOverlayOffset");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mLookAroundShowOnOverlayDTB, isEnabled, "LookAroundShowOnOverlayExpr");
            }
          }
          else if (string.Equals(keyValuePair.Key, "IsShootOnClickEnabled", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mStackPanel.Children.Add((UIElement) this.mShootGB);
            this.mShootCB.IsChecked = new bool?(Convert.ToBoolean(action[keyValuePair.Key], (IFormatProvider) CultureInfo.InvariantCulture));
            bool isEnabled = this.mShootCB.IsChecked.Value;
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootDTB, isEnabled, "KeyAction");
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootXDTB, isEnabled, "LButtonX");
            KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootYDTB, isEnabled, "LButtonY");
            if (KMManager.sIsDeveloperModeOn)
            {
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootXExprDTB, isEnabled, "LButtonXExpr");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootYExprDTB, isEnabled, "LButtonYExpr");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootXOffsetDTB, isEnabled, "LButtonXOverlayOffset");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootYOffsetDTB, isEnabled, "LButtonYOverlayOffset");
              KeymapExtraSettingWindow.SetChildControlsValues(action, this.mShootShowOnOverlayDTB, isEnabled, "LButtonShowOnOverlayExpr");
            }
          }
          else if (string.Equals(keyValuePair.Key, "ShowOnOverlay", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mOverlayCB.IsChecked = new bool?(Convert.ToBoolean(action[keyValuePair.Key], (IFormatProvider) CultureInfo.InvariantCulture));
            this.mOverlayCB.Tag = (object) keyValuePair.Key;
            if (!this.mStackPanel.Children.Contains((UIElement) this.mOverlayGB))
              this.mStackPanel.Children.Add((UIElement) this.mOverlayGB);
          }
          else if (action.Type == KeyActionType.FreeLook && (string.Equals(keyValuePair.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase)))
          {
            if (((FreeLook) action).DeviceType == 0)
            {
              if (string.Equals(keyValuePair.Key, "Speed", StringComparison.InvariantCultureIgnoreCase))
                this.AddFields(keyValuePair, action);
            }
            else if (string.Equals(keyValuePair.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase))
              this.AddFields(keyValuePair, action);
          }
          else if (isGamepadTabSelected)
          {
            if (keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture) || (keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals("KeyWheel", StringComparison.InvariantCultureIgnoreCase) || keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals("KeyMove", StringComparison.InvariantCultureIgnoreCase)))
              this.AddFields(keyValuePair, action);
          }
          else if (!keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
            this.AddFields(keyValuePair, action);
        }
        if (KMManager.sIsDeveloperModeOn)
        {
          foreach (KeyValuePair<string, PropertyInfo> keyValuePair in IMAction.sDictDevModeUIElements[action.Type])
          {
            if (isGamepadTabSelected)
            {
              if (keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture))
                this.AddFields(keyValuePair, action);
            }
            else if (!keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
              this.AddFields(keyValuePair, action);
          }
        }
      }
      if (this.mDictDualTextBox.ContainsKey("Fields~Sensitivity") && this.mDictDualTextBox.ContainsKey("Fields~SensitivityRatioY"))
      {
        GroupBox groupBox = this.GetGroupBox("Fields");
        (groupBox.Content as StackPanel).Children.Remove((UIElement) this.mDictDualTextBox["Fields~SensitivityRatioY"]);
        (groupBox.Content as StackPanel).Children.Insert((groupBox.Content as StackPanel).Children.IndexOf((UIElement) this.mDictDualTextBox["Fields~Sensitivity"]) + 1, (UIElement) this.mDictDualTextBox["Fields~SensitivityRatioY"]);
      }
      if (this.ListAction.First<IMAction>().Type == KeyActionType.MOBADpad)
        this.mStackPanel.Children.Insert(this.mStackPanel.Children.IndexOf((UIElement) this.GetGroupBox("Fields")) + 1, (UIElement) this.mMOBAGB);
      if (KMManager.sIsDeveloperModeOn)
      {
        this.mStackPanel.Children.Add((UIElement) this.mEnableConditionGB);
        this.mStackPanel.Children.Add((UIElement) this.mNoteGB);
        this.mStackPanel.Children.Add((UIElement) this.mStartConditionGB);
      }
      KeymapCanvasWindow.sIsDirty = sIsDirty;
    }

    private static void SetChildControlsValues(
      IMAction action,
      DualTextBlockControl control,
      bool isEnabled,
      string actionItemProperty)
    {
      control.mKeyPropertyNameTextBox.IsEnabled = isEnabled;
      control.ActionItemProperty = actionItemProperty;
      control.AddActionItem(action);
    }

    private void AddFields(KeyValuePair<string, PropertyInfo> item, IMAction action)
    {
      bool flag = false;
      CategoryAttribute customAttribute = item.Value.GetCustomAttributes(typeof (CategoryAttribute), true)[0] as CategoryAttribute;
      string category = customAttribute.Category;
      string key = customAttribute.Category + "~" + item.Key;
      object[] customAttributes = item.Value.GetCustomAttributes(typeof (DescriptionAttribute), true);
      if (customAttributes.Length != 0 && (customAttributes[0] as DescriptionAttribute).Description.Contains("NotCommon"))
      {
        key = key + "~" + action.Direction.ToString();
        flag = true;
      }
      GroupBox groupBox = this.GetGroupBox(category);
      if (this.mDictDualTextBox.ContainsKey(key))
      {
        this.mDictDualTextBox[key].AddActionItem(action);
      }
      else
      {
        DualTextBlockControl textBlockControl = new DualTextBlockControl(this.ParentWindow, this)
        {
          IsAddDirectionAttribute = flag,
          ActionItemProperty = item.Key
        };
        textBlockControl.AddActionItem(action);
        if (string.Equals(item.Key, "GuidanceCategory", StringComparison.InvariantCultureIgnoreCase))
        {
          textBlockControl.mKeyPropertyNameTextBox.IsEnabled = false;
          this.mStackPanel.Children.Remove((UIElement) groupBox);
          this.mStackPanel.Children.Insert(0, (UIElement) groupBox);
        }
        (groupBox.Content as StackPanel).Children.Add((UIElement) textBlockControl);
        this.mDictDualTextBox[key] = textBlockControl;
      }
    }

    private GroupBox GetGroupBox(string category)
    {
      GroupBox groupBox;
      if (this.mDictGroupBox.ContainsKey(category))
      {
        groupBox = this.mDictGroupBox[category];
      }
      else
      {
        groupBox = new GroupBox();
        this.mDictGroupBox.Add(category, groupBox);
        groupBox.Header = (object) LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + category, "");
        this.mStackPanel.Children.Add((UIElement) groupBox);
        groupBox.Content = (object) new StackPanel();
      }
      return groupBox;
    }

    private void CustomPictureBox_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
    }

    private void CustomPictureBox_MouseLeave(object sender, MouseEventArgs e)
    {
      if (KMManager.CanvasWindow.mCanvasElement != null)
        return;
      this.Cursor = Cursors.Arrow;
    }

    private void MOBAHeroPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.IsOpen = false;
      MOBADpad action = this.ListAction.First<IMAction>() as MOBADpad;
      if (action.mMOBAHeroDummy != null && KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) action.mMOBAHeroDummy))
      {
        CanvasElement element = KMManager.CanvasWindow.dictCanvasElement[(IMAction) action.mMOBAHeroDummy];
        KMManager.CanvasWindow.StartMoving(element, new Point(Canvas.GetLeft((UIElement) element), Canvas.GetTop((UIElement) element)));
      }
      else
      {
        KMManager.CheckAndCreateNewScheme();
        action.mMOBAHeroDummy = new MOBAHeroDummy(action);
        this.AddUIInCanvas((IMAction) action.mMOBAHeroDummy);
      }
    }

    private void MOBASkillCancelCB_CheckedChanged(object sender, RoutedEventArgs e)
    {
      bool? isChecked;
      if (this.ListAction.Count > 0)
      {
        KMManager.CheckAndCreateNewScheme();
        DualTextBlockControl mobaSkillCancelDtb = this.mMOBASkillCancelDTB;
        isChecked = this.mMOBASkillCancelCB.IsChecked;
        int num1 = isChecked.Value ? 1 : 0;
        mobaSkillCancelDtb.IsEnabled = num1 != 0;
        if (KMManager.sIsDeveloperModeOn)
        {
          DualTextBlockControl skillCancelXexprDtb = this.mMOBASkillCancelXExprDTB;
          DualTextBlockControl skillCancelYexprDtb1 = this.mMOBASkillCancelYExprDTB;
          DualTextBlockControl skillCancelYexprDtb2 = this.mMOBASkillCancelYExprDTB;
          DualTextBlockControl cancelYoffsetDtb = this.mMOBASkillCancelYOffsetDTB;
          DualTextBlockControl showOnOverlayDtb = this.mMOBASkillCancelShowOnOverlayDTB;
          isChecked = this.mMOBASkillCancelCB.IsChecked;
          int num2;
          bool flag1 = (num2 = isChecked.Value ? 1 : 0) != 0;
          showOnOverlayDtb.IsEnabled = num2 != 0;
          int num3;
          bool flag2 = (num3 = flag1 ? 1 : 0) != 0;
          cancelYoffsetDtb.IsEnabled = num3 != 0;
          int num4;
          bool flag3 = (num4 = flag2 ? 1 : 0) != 0;
          skillCancelYexprDtb2.IsEnabled = num4 != 0;
          int num5;
          bool flag4 = (num5 = flag3 ? 1 : 0) != 0;
          skillCancelYexprDtb1.IsEnabled = num5 != 0;
          int num6 = flag4 ? 1 : 0;
          skillCancelXexprDtb.IsEnabled = num6 != 0;
        }
        MOBASkill action = this.ListAction.First<IMAction>() as MOBASkill;
        isChecked = this.mMOBASkillCancelCB.IsChecked;
        if (isChecked.Value)
          action.mMOBASkillCancel = new MOBASkillCancel(action);
        else if (action.IsCancelSkillEnabled)
        {
          if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) action.mMOBASkillCancel))
          {
            KMManager.CanvasWindow.dictCanvasElement[(IMAction) action.mMOBASkillCancel].RemoveAction("KeyCancel");
            KMManager.CanvasWindow.dictCanvasElement.Remove((IMAction) action.mMOBASkillCancel);
            this.mMOBASkillCancelDTB.mKeyPropertyNameTextBox.Text = string.Empty;
          }
          action.CancelX = action.CancelY = -1.0;
        }
        KeymapCanvasWindow.sIsDirty = true;
      }
      if (this.mMOBASkillCancelPB == null)
        return;
      CustomPictureBox mobaSkillCancelPb = this.mMOBASkillCancelPB;
      isChecked = this.mMOBASkillCancelCB.IsChecked;
      int num = isChecked.Value ? 1 : 0;
      mobaSkillCancelPb.IsEnabled = num != 0;
    }

    private void MOBASkillCancelPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.IsOpen = false;
      MOBASkill mobaSkill = this.ListAction.First<IMAction>() as MOBASkill;
      if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) mobaSkill.mMOBASkillCancel))
      {
        CanvasElement element = KMManager.CanvasWindow.dictCanvasElement[(IMAction) mobaSkill.mMOBASkillCancel];
        KMManager.CanvasWindow.StartMoving(element, new Point(Canvas.GetLeft((UIElement) element), Canvas.GetTop((UIElement) element)));
      }
      else
        this.AddUIInCanvas((IMAction) mobaSkill.mMOBASkillCancel);
    }

    private void LookAroundCB_CheckedChanged(object sender, RoutedEventArgs e)
    {
      bool? isChecked1;
      if (this.ListAction.Count > 0)
      {
        KMManager.CheckAndCreateNewScheme();
        DualTextBlockControl mLookAroundXdtb = this.mLookAroundXDTB;
        DualTextBlockControl mLookAroundYdtb = this.mLookAroundYDTB;
        DualTextBlockControl mLookAroundDtb = this.mLookAroundDTB;
        bool? isChecked2 = this.mLookAroundCB.IsChecked;
        int num1;
        bool flag1 = (num1 = isChecked2.Value ? 1 : 0) != 0;
        mLookAroundDtb.IsEnabled = num1 != 0;
        int num2;
        bool flag2 = (num2 = flag1 ? 1 : 0) != 0;
        mLookAroundYdtb.IsEnabled = num2 != 0;
        int num3 = flag2 ? 1 : 0;
        mLookAroundXdtb.IsEnabled = num3 != 0;
        if (KMManager.sIsDeveloperModeOn)
        {
          DualTextBlockControl lookAroundXexprDtb = this.mLookAroundXExprDTB;
          DualTextBlockControl lookAroundYexprDtb = this.mLookAroundYExprDTB;
          DualTextBlockControl aroundXoffsetDtb = this.mLookAroundXOffsetDTB;
          DualTextBlockControl aroundYoffsetDtb = this.mLookAroundYOffsetDTB;
          DualTextBlockControl showOnOverlayDtb = this.mLookAroundShowOnOverlayDTB;
          bool? isChecked3 = this.mLookAroundCB.IsChecked;
          int num4;
          bool flag3 = (num4 = isChecked3.Value ? 1 : 0) != 0;
          showOnOverlayDtb.IsEnabled = num4 != 0;
          int num5;
          bool flag4 = (num5 = flag3 ? 1 : 0) != 0;
          aroundYoffsetDtb.IsEnabled = num5 != 0;
          int num6;
          bool flag5 = (num6 = flag4 ? 1 : 0) != 0;
          aroundXoffsetDtb.IsEnabled = num6 != 0;
          int num7;
          bool flag6 = (num7 = flag5 ? 1 : 0) != 0;
          lookAroundYexprDtb.IsEnabled = num7 != 0;
          int num8 = flag6 ? 1 : 0;
          lookAroundXexprDtb.IsEnabled = num8 != 0;
        }
        Pan action = this.ListAction.First<IMAction>() as Pan;
        isChecked1 = this.mLookAroundCB.IsChecked;
        if (isChecked1.Value)
          action.mLookAround = new LookAround(action);
        else if (action.IsLookAroundEnabled)
        {
          if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) action.mLookAround))
          {
            KMManager.CanvasWindow.dictCanvasElement[(IMAction) action.mLookAround].RemoveAction("KeyLookAround");
            KMManager.CanvasWindow.dictCanvasElement.Remove((IMAction) action.mLookAround);
            this.mLookAroundDTB.mKeyPropertyNameTextBox.Text = string.Empty;
          }
          action.LookAroundX = action.LookAroundY = -1.0;
        }
        KeymapCanvasWindow.sIsDirty = true;
      }
      if (this.mLookAroundPB == null)
        return;
      CustomPictureBox mLookAroundPb = this.mLookAroundPB;
      isChecked1 = this.mLookAroundCB.IsChecked;
      int num = isChecked1.Value ? 1 : 0;
      mLookAroundPb.IsEnabled = num != 0;
    }

    private void LookAroundPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.IsOpen = false;
      Pan pan = this.ListAction.First<IMAction>() as Pan;
      if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) pan.mLookAround))
      {
        CanvasElement element = KMManager.CanvasWindow.dictCanvasElement[(IMAction) pan.mLookAround];
        KMManager.CanvasWindow.StartMoving(element, new Point(Canvas.GetLeft((UIElement) element), Canvas.GetTop((UIElement) element)));
      }
      else
        this.AddUIInCanvas((IMAction) pan.mLookAround);
    }

    private void ShootCB_CheckedChanged(object sender, RoutedEventArgs e)
    {
      bool? isChecked1;
      if (this.ListAction.Count > 0)
      {
        KMManager.CheckAndCreateNewScheme();
        DualTextBlockControl mShootXdtb = this.mShootXDTB;
        DualTextBlockControl mShootYdtb = this.mShootYDTB;
        DualTextBlockControl mShootDtb = this.mShootDTB;
        bool? isChecked2 = this.mLookAroundCB.IsChecked;
        int num1;
        bool flag1 = (num1 = isChecked2.Value ? 1 : 0) != 0;
        mShootDtb.IsEnabled = num1 != 0;
        int num2;
        bool flag2 = (num2 = flag1 ? 1 : 0) != 0;
        mShootYdtb.IsEnabled = num2 != 0;
        int num3 = flag2 ? 1 : 0;
        mShootXdtb.IsEnabled = num3 != 0;
        if (KMManager.sIsDeveloperModeOn)
        {
          DualTextBlockControl mShootXexprDtb = this.mShootXExprDTB;
          DualTextBlockControl mShootYexprDtb = this.mShootYExprDTB;
          DualTextBlockControl mShootXoffsetDtb = this.mShootXOffsetDTB;
          DualTextBlockControl mShootYoffsetDtb = this.mShootYOffsetDTB;
          DualTextBlockControl showOnOverlayDtb = this.mShootShowOnOverlayDTB;
          bool? isChecked3 = this.mShootCB.IsChecked;
          int num4;
          bool flag3 = (num4 = isChecked3.Value ? 1 : 0) != 0;
          showOnOverlayDtb.IsEnabled = num4 != 0;
          int num5;
          bool flag4 = (num5 = flag3 ? 1 : 0) != 0;
          mShootYoffsetDtb.IsEnabled = num5 != 0;
          int num6;
          bool flag5 = (num6 = flag4 ? 1 : 0) != 0;
          mShootXoffsetDtb.IsEnabled = num6 != 0;
          int num7;
          bool flag6 = (num7 = flag5 ? 1 : 0) != 0;
          mShootYexprDtb.IsEnabled = num7 != 0;
          int num8 = flag6 ? 1 : 0;
          mShootXexprDtb.IsEnabled = num8 != 0;
        }
        Pan action = this.ListAction.First<IMAction>() as Pan;
        isChecked1 = this.mShootCB.IsChecked;
        if (isChecked1.Value)
          action.mPanShoot = new PanShoot(action);
        else if (action.IsShootOnClickEnabled)
        {
          if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) action.mPanShoot))
          {
            KMManager.CanvasWindow.dictCanvasElement[(IMAction) action.mPanShoot].RemoveAction("KeyAction");
            KMManager.CanvasWindow.dictCanvasElement.Remove((IMAction) action.mPanShoot);
            this.mShootDTB.mKeyPropertyNameTextBox.Text = string.Empty;
          }
          action.LButtonX = action.LButtonY = -1.0;
        }
        KeymapCanvasWindow.sIsDirty = true;
      }
      if (this.mShootPB == null)
        return;
      CustomPictureBox mShootPb = this.mShootPB;
      isChecked1 = this.mShootCB.IsChecked;
      int num = isChecked1.Value ? 1 : 0;
      mShootPb.IsEnabled = num != 0;
    }

    private void ShootPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.IsOpen = false;
      Pan pan = this.ListAction.First<IMAction>() as Pan;
      if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey((IMAction) pan.mPanShoot))
      {
        CanvasElement element = KMManager.CanvasWindow.dictCanvasElement[(IMAction) pan.mPanShoot];
        KMManager.CanvasWindow.StartMoving(element, new Point(Canvas.GetLeft((UIElement) element), Canvas.GetTop((UIElement) element)));
      }
      else
        this.AddUIInCanvas((IMAction) pan.mPanShoot);
    }

    private void mOverlayCB_Checked(object sender, RoutedEventArgs e)
    {
      if (this.ListAction.Count <= 0)
        return;
      KMManager.CheckAndCreateNewScheme();
      foreach (IMAction imAction in this.ListAction)
        imAction.IsVisibleInOverlay = true;
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void mOverlayCB_Unchecked(object sender, RoutedEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      if (this.ListAction.Count > 0)
      {
        foreach (IMAction imAction in this.ListAction)
          imAction.IsVisibleInOverlay = false;
      }
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void AddUIInCanvas(IMAction hero)
    {
      this.Cursor = Cursors.Hand;
      KMManager.GetCanvasElement(this.ParentWindow, hero, this.mCanvas, true);
    }

    private void mCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      KMManager.RepositionCanvasElement();
    }

    private void mCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Cursor = Cursors.Arrow;
      KMManager.ClearElement();
    }

    private void mGamepadTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        KMManager.CheckAndCreateNewScheme();
        foreach (IMAction imAction in this.ListAction)
        {
          if (!string.Equals(imAction.GuidanceCategory, this.mGuidanceCategoryComboBox.mAutoComboBox.Text, StringComparison.InvariantCulture))
          {
            imAction.GuidanceCategory = this.mGuidanceCategoryComboBox.mAutoComboBox.Text;
            KeymapCanvasWindow.sIsDirty = true;
            this.ParentWindow.SelectedConfig.AddString(imAction.GuidanceCategory);
          }
        }
        this.mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
        this.mGamepadTabBorder.Background = (Brush) Brushes.Transparent;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
        this.mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
        this.AddGuidanceCategories();
        this.Init(true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in switching to Gamepad tab: " + ex.ToString());
      }
    }

    private void mKeyboardTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        KMManager.CheckAndCreateNewScheme();
        foreach (IMAction imAction in this.ListAction)
        {
          if (!string.Equals(imAction.GuidanceCategory, this.mGuidanceCategoryComboBox.mAutoComboBox.Text, StringComparison.InvariantCulture))
          {
            imAction.GuidanceCategory = this.mGuidanceCategoryComboBox.mAutoComboBox.Text;
            KeymapCanvasWindow.sIsDirty = true;
            this.ParentWindow.SelectedConfig.AddString(imAction.GuidanceCategory);
          }
        }
        this.AddGuidanceCategories();
        this.mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
        this.mKeyboardTabBorder.Background = (Brush) Brushes.Transparent;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
        this.mGamepadTabBorder.BorderThickness = new Thickness(0.0);
        this.Init(false);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in switching to Keyboard tab: " + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/keymapextrasettingwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mHeaderGrid = (Grid) target;
          break;
        case 2:
          this.mHeader = (TextBlock) target;
          break;
        case 3:
          ((UIElement) target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonDown);
          break;
        case 4:
          this.mScrollBar = (CustomScrollViewer) target;
          break;
        case 5:
          this.mDeleteButton = (CustomButton) target;
          this.mDeleteButton.Click += new RoutedEventHandler(this.DeleteButton_Click);
          break;
        case 6:
          this.mDummyGrid = (Grid) target;
          break;
        case 7:
          this.mMOBAGB = (GroupBox) target;
          break;
        case 8:
          this.mMOBAPanel = (StackPanel) target;
          break;
        case 9:
          this.mMOBAPB = (CustomPictureBox) target;
          this.mMOBAPB.MouseEnter += new MouseEventHandler(this.CustomPictureBox_MouseEnter);
          this.mMOBAPB.MouseLeave += new MouseEventHandler(this.CustomPictureBox_MouseLeave);
          this.mMOBAPB.MouseDown += new MouseButtonEventHandler(this.MOBAHeroPictureBox_MouseDown);
          break;
        case 10:
          this.mGuidanceCategory = (GroupBox) target;
          break;
        case 11:
          this.mGuidanceCategoryComboBox = (AutoCompleteComboBox) target;
          break;
        case 12:
          this.mTabsGrid = (GroupBox) target;
          break;
        case 13:
          this.mKeyboardTabBorder = (Border) target;
          this.mKeyboardTabBorder.MouseLeftButtonUp += new MouseButtonEventHandler(this.mKeyboardTabBorder_MouseLeftButtonUp);
          break;
        case 14:
          this.keyboardBtn = (TextBlock) target;
          break;
        case 15:
          this.mGamepadTabBorder = (Border) target;
          this.mGamepadTabBorder.MouseLeftButtonUp += new MouseButtonEventHandler(this.mGamepadTabBorder_MouseLeftButtonUp);
          break;
        case 16:
          this.gamepadBtn = (TextBlock) target;
          break;
        case 17:
          this.mMOBASkillCancelGB = (GroupBox) target;
          break;
        case 18:
          this.mMOBASkillCancelGBPanel = (StackPanel) target;
          break;
        case 19:
          this.mMOBASkillCancelCB = (CustomCheckbox) target;
          this.mMOBASkillCancelCB.Checked += new RoutedEventHandler(this.MOBASkillCancelCB_CheckedChanged);
          this.mMOBASkillCancelCB.Unchecked += new RoutedEventHandler(this.MOBASkillCancelCB_CheckedChanged);
          break;
        case 20:
          this.mMOBASkillCancelPB = (CustomPictureBox) target;
          this.mMOBASkillCancelPB.MouseEnter += new MouseEventHandler(this.CustomPictureBox_MouseEnter);
          this.mMOBASkillCancelPB.MouseLeave += new MouseEventHandler(this.CustomPictureBox_MouseLeave);
          this.mMOBASkillCancelPB.MouseDown += new MouseButtonEventHandler(this.MOBASkillCancelPictureBox_MouseDown);
          break;
        case 21:
          this.mLookAroundGB = (GroupBox) target;
          break;
        case 22:
          this.mLookAroundPanel = (StackPanel) target;
          break;
        case 23:
          this.mLookAroundCB = (CustomCheckbox) target;
          this.mLookAroundCB.Checked += new RoutedEventHandler(this.LookAroundCB_CheckedChanged);
          this.mLookAroundCB.Unchecked += new RoutedEventHandler(this.LookAroundCB_CheckedChanged);
          break;
        case 24:
          this.mLookAroundPB = (CustomPictureBox) target;
          this.mLookAroundPB.MouseEnter += new MouseEventHandler(this.CustomPictureBox_MouseEnter);
          this.mLookAroundPB.MouseLeave += new MouseEventHandler(this.CustomPictureBox_MouseLeave);
          this.mLookAroundPB.MouseDown += new MouseButtonEventHandler(this.LookAroundPictureBox_MouseDown);
          break;
        case 25:
          this.mShootGB = (GroupBox) target;
          break;
        case 26:
          this.mShootGBPanel = (StackPanel) target;
          break;
        case 27:
          this.mShootCB = (CustomCheckbox) target;
          this.mShootCB.Checked += new RoutedEventHandler(this.ShootCB_CheckedChanged);
          this.mShootCB.Unchecked += new RoutedEventHandler(this.ShootCB_CheckedChanged);
          break;
        case 28:
          this.mShootPB = (CustomPictureBox) target;
          this.mShootPB.MouseEnter += new MouseEventHandler(this.CustomPictureBox_MouseEnter);
          this.mShootPB.MouseLeave += new MouseEventHandler(this.CustomPictureBox_MouseLeave);
          this.mShootPB.MouseDown += new MouseButtonEventHandler(this.ShootPictureBox_MouseDown);
          break;
        case 29:
          this.mSchemesGB = (GroupBox) target;
          break;
        case 30:
          this.mEnableConditionGB = (GroupBox) target;
          break;
        case 31:
          this.mNoteGB = (GroupBox) target;
          break;
        case 32:
          this.mStartConditionGB = (GroupBox) target;
          break;
        case 33:
          this.mOverlayGB = (GroupBox) target;
          break;
        case 34:
          this.mOverlayCB = (CustomCheckbox) target;
          this.mOverlayCB.Checked += new RoutedEventHandler(this.mOverlayCB_Checked);
          this.mOverlayCB.Unchecked += new RoutedEventHandler(this.mOverlayCB_Unchecked);
          break;
        case 35:
          this.mCanvas = (Canvas) target;
          this.mCanvas.PreviewMouseMove += new MouseEventHandler(this.mCanvas_PreviewMouseMove);
          this.mCanvas.MouseUp += new MouseButtonEventHandler(this.mCanvas_MouseUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
