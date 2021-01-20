// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CanvasElement
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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class CanvasElement : UserControl, IComponentConnector
  {
    internal static Dictionary<string, CanvasElement> dictPoints = new Dictionary<string, CanvasElement>();
    private readonly double mMaxSenstivity = 10.0;
    private List<Key> mKeyList = new List<Key>();
    internal Dictionary<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>> dictTextElemets = new Dictionary<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>();
    private readonly double mMinSenstivity;
    internal bool mIsLoadingfromFile;
    internal Point Center;
    private Point? mMousePointForTap;
    private KeymapCanvasWindow mParentWindow;
    private MainWindow ParentWindow;
    private KeyActionType mType;
    internal double TopOnClick;
    internal double LeftOnClick;
    internal static object sFocusedTextBox;
    internal double mXPosition;
    internal double mYPosition;
    private MOBASkillSettingsPopup mMOBASkillSettingsPopup;
    private MOBAOtherSettingsMoreInfoPopup mMOBAOtherSettingsMoreInfoPopup;
    private SkillIconToolTipPopup mSkillIconToolTipPopup;
    private MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;
    internal CanvasElement mCanvasElement;
    internal Grid mToggleModeGrid;
    internal TextBlock mToggleMode1;
    internal CustomPictureBox mToggleImage;
    internal TextBlock mToggleMode2;
    internal Grid mXSensitivityGrid;
    internal TextBlock mXSensitivity;
    internal Grid mKeyRepeatGrid;
    internal TextBlock mCountText;
    internal Grid mCanvasGrid;
    internal CustomPictureBox mActionIcon;
    internal CustomPictureBox mActionIcon2;
    internal CustomPictureBox mCloseIcon;
    internal CustomPictureBox mResizeIcon;
    internal CustomPictureBox mSkillImage;
    internal CustomPictureBox mSettingsIcon;
    internal Grid mGrid;
    internal ColumnDefinition mColumn0;
    internal ColumnDefinition mColumn1;
    internal ColumnDefinition mColumn2;
    internal ColumnDefinition mColumn3;
    internal ColumnDefinition mColumn4;
    internal RowDefinition mRow0;
    internal RowDefinition mRow1;
    internal RowDefinition mRow2;
    internal RowDefinition mRow3;
    internal RowDefinition mRow4;
    internal Grid mYSensitivityGrid;
    internal TextBlock mYSensitivity;
    private bool _contentLoaded;

    public List<IMAction> ListActionItem { get; } = new List<IMAction>();

    public KeyActionType ActionType
    {
      get
      {
        return this.mType;
      }
      set
      {
        this.mType = value;
        this.SetActiveImage(false);
      }
    }

    internal MOBASkillSettingsPopup MOBASkillSettingsPopup
    {
      get
      {
        if (this.mMOBASkillSettingsPopup == null)
          this.mMOBASkillSettingsPopup = new MOBASkillSettingsPopup(this);
        return this.mMOBASkillSettingsPopup;
      }
    }

    internal MOBAOtherSettingsMoreInfoPopup MOBAOtherSettingsMoreInfoPopup
    {
      get
      {
        if (this.mMOBAOtherSettingsMoreInfoPopup == null)
          this.mMOBAOtherSettingsMoreInfoPopup = new MOBAOtherSettingsMoreInfoPopup(this);
        return this.mMOBAOtherSettingsMoreInfoPopup;
      }
    }

    private SkillIconToolTipPopup SkillIconToolTipPopup
    {
      get
      {
        if (this.mSkillIconToolTipPopup == null)
          this.mSkillIconToolTipPopup = new SkillIconToolTipPopup(this);
        return this.mSkillIconToolTipPopup;
      }
    }

    internal MOBASkillSettingsMoreInfoPopup MOBASkillSettingsMoreInfoPopup
    {
      get
      {
        if (this.mMOBASkillSettingsMoreInfoPopup == null)
          this.mMOBASkillSettingsMoreInfoPopup = new MOBASkillSettingsMoreInfoPopup(this);
        return this.mMOBASkillSettingsMoreInfoPopup;
      }
    }

    public bool IsRemoveIfEmpty { get; internal set; }

    public CanvasElement(KeymapCanvasWindow window, MainWindow parentWindow)
    {
      this.mParentWindow = window;
      this.ParentWindow = parentWindow;
      this.InitializeComponent();
    }

    internal void AddAction(IMAction action)
    {
      this.ListActionItem.Add(action);
      this.ActionType = (KeyActionType) Enum.Parse(typeof (KeyActionType), action.GetType().ToString());
      this.SetKeysForActions(this.ListActionItem);
      this.SetSize(action);
      this.SetElementLayout(true, 0.0, 0.0);
      if (this.mType == KeyActionType.Dpad || this.mType == KeyActionType.MOBADpad || (this.mType == KeyActionType.Rotate || this.mType == KeyActionType.Swipe) || (this.mType == KeyActionType.Zoom || this.mType == KeyActionType.Tilt || this.mType == KeyActionType.FreeLook && (action as FreeLook).DeviceType == 0))
      {
        this.mSettingsIcon.Visibility = Visibility.Visible;
        this.mCloseIcon.Visibility = Visibility.Visible;
      }
      else if (this.mType == KeyActionType.MOBASkill)
      {
        this.mActionIcon.Visibility = Visibility.Collapsed;
        this.mCloseIcon.Visibility = Visibility.Collapsed;
        this.mSkillImage.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.mSettingsIcon.Visibility = Visibility.Collapsed;
        this.mCloseIcon.Visibility = Visibility.Collapsed;
        this.mSkillImage.Visibility = Visibility.Collapsed;
      }
      if (this.mType != KeyActionType.MOBAHeroDummy && this.mType != KeyActionType.Pan)
        return;
      this.mActionIcon.AllowClickThrough = false;
    }

    private void SetKeysForActions(List<IMAction> lst)
    {
      foreach (KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>> dictTextElemet in this.dictTextElemets)
        dictTextElemet.Value.Item3.Visibility = Visibility.Collapsed;
      foreach (IMAction action in lst)
        this.SetKeysForAction(action);
    }

    private void SetKeysForAction(IMAction action)
    {
      bool isGamepadEnabled = this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName) && this.mParentWindow.IsInOverlayMode;
      switch (action.Type)
      {
        case KeyActionType.Tap:
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.Swipe:
          this.mColumn0.Width = new GridLength(0.0, GridUnitType.Star);
          this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
          this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn4.Width = new GridLength(0.0, GridUnitType.Star);
          this.mRow0.Height = new GridLength(5.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow2.Height = new GridLength(50.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
          string str1 = isGamepadEnabled ? "Key_alt1" : "Key";
          if (action.Direction == Direction.Left)
          {
            this.SetKeys(action, "", str1, "", "", "");
            break;
          }
          if (action.Direction == Direction.Right)
          {
            this.SetKeys(action, "", "", "", str1, "");
            break;
          }
          if (action.Direction == Direction.Up)
          {
            this.SetKeys(action, "", "", str1, "", "");
            break;
          }
          if (action.Direction != Direction.Down)
            break;
          this.SetKeys(action, "", "", "", "", str1);
          break;
        case KeyActionType.Dpad:
          Dpad dpad = action as Dpad;
          if ((isGamepadEnabled ? (string.IsNullOrEmpty(dpad.GamepadStick) ? 1 : 0) : 1) != 0)
          {
            this.mColumn0.Width = new GridLength(10.0, GridUnitType.Star);
            this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
            this.mColumn2.Width = new GridLength(20.0, GridUnitType.Star);
            this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
            this.mColumn4.Width = new GridLength(10.0, GridUnitType.Star);
            this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
            this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
            this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
            this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
            this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
          }
          if (!isGamepadEnabled)
          {
            this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
            break;
          }
          if (!string.IsNullOrEmpty(dpad.GamepadStick))
          {
            this.SetKeys(action, "GamepadStick", "", "", "", "");
            break;
          }
          this.SetKeys(action, "", "KeyLeft_alt1", "KeyUp_alt1", "KeyRight_alt1", "KeyDown_alt1");
          break;
        case KeyActionType.Zoom:
          this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
          this.mColumn1.Width = new GridLength(72.0);
          this.mColumn2.Width = new GridLength(70.0, GridUnitType.Star);
          this.mColumn3.Width = new GridLength(30.0);
          this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
          this.mRow0.Height = new GridLength(80.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(35.0);
          this.mRow2.Height = new GridLength(35.0);
          this.mRow3.Height = new GridLength(0.0);
          this.mRow4.Height = new GridLength(80.0, GridUnitType.Star);
          this.SetKeys(action, "", "KeyOut", "KeyIn", "", "");
          break;
        case KeyActionType.Tilt:
          this.mColumn0.Width = new GridLength(0.0, GridUnitType.Star);
          this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
          this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn4.Width = new GridLength(0.0, GridUnitType.Star);
          this.mRow0.Height = new GridLength(5.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow2.Height = new GridLength(50.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
          this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
          break;
        case KeyActionType.Pan:
          Pan pan = action as Pan;
          if (isGamepadEnabled)
            this.SetKeys(action, "KeyStartStop_alt1", "", "", "", "");
          else
            this.SetKeys(action, "KeyStartStop", "", "", "", "");
          if (!this.mParentWindow.IsInOverlayMode)
          {
            this.mXSensitivityGrid.Visibility = Visibility.Visible;
            this.mYSensitivityGrid.Visibility = Visibility.Visible;
            TextBlock mXsensitivity = this.mXSensitivity;
            double num = pan.Sensitivity;
            string str2 = num.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
            mXsensitivity.Text = str2;
            double sensitivityRatioY = pan.SensitivityRatioY;
            double sensitivity = pan.Sensitivity;
            if (sensitivityRatioY != 0.0 && sensitivity != 0.0)
            {
              TextBlock mYsensitivity = this.mYSensitivity;
              num = sensitivity * sensitivityRatioY;
              string str3 = num.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
              mYsensitivity.Text = str3;
            }
            else
              this.mYSensitivity.Text = sensitivityRatioY == 0.0 ? 0.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture) : sensitivityRatioY.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
          }
          if (!this.mParentWindow.dictCanvasElement.ContainsKey(action) || !pan.IsLookAroundEnabled || !this.mParentWindow.dictCanvasElement.ContainsKey((IMAction) pan.mLookAround))
            break;
          this.mParentWindow.dictCanvasElement[(IMAction) pan.mLookAround].SetKeysForAction((IMAction) pan.mLookAround);
          break;
        case KeyActionType.MOBADpad:
          if (this.mParentWindow.IsInOverlayMode)
          {
            this.SetKeys(action, "KeyMove", "", "", "", "");
            break;
          }
          this.SetKeys(action, "", "", "", "", "");
          break;
        case KeyActionType.MOBASkill:
          MOBASkill mobaSkill = action as MOBASkill;
          if (isGamepadEnabled)
            this.SetKeys(action, "KeyActivate_alt1", "", "", "", "");
          else
            this.SetKeys(action, "KeyActivate", "", "", "", "");
          if (!this.mParentWindow.dictCanvasElement.ContainsKey(action) || !mobaSkill.IsCancelSkillEnabled || !this.mParentWindow.dictCanvasElement.ContainsKey((IMAction) mobaSkill.mMOBASkillCancel))
            break;
          this.mParentWindow.dictCanvasElement[(IMAction) mobaSkill.mMOBASkillCancel].SetKeysForAction((IMAction) mobaSkill.mMOBASkillCancel);
          break;
        case KeyActionType.Script:
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.TapRepeat:
          this.mRow1.Height = new GridLength(0.0);
          this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(4.0, GridUnitType.Star);
          this.mCountText.Text = ((TapRepeat) action).Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          if (!this.mParentWindow.IsInOverlayMode)
            this.mKeyRepeatGrid.Visibility = Visibility.Visible;
          this.SetToggleModeValues(action, isGamepadEnabled);
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.Rotate:
          this.mColumn0.Width = new GridLength(0.0);
          this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
          this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
          this.mColumn4.Width = new GridLength(0.0);
          this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "", "KeyAntiClock_alt1", "", "KeyClock_alt1", "");
            break;
          }
          this.SetKeys(action, "", "KeyAntiClock", "", "KeyClock", "");
          break;
        case KeyActionType.State:
          if (!KMManager.sIsDeveloperModeOn)
            break;
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.FreeLook:
          this.SetToggleModeValues(action, isGamepadEnabled);
          break;
        case KeyActionType.MouseZoom:
          if (!KMManager.sIsDeveloperModeOn)
            break;
          this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
          this.mColumn1.Width = new GridLength(72.0);
          this.mColumn2.Width = new GridLength(70.0, GridUnitType.Star);
          this.mColumn3.Width = new GridLength(30.0);
          this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
          this.mRow0.Height = new GridLength(80.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(35.0);
          this.mRow2.Height = new GridLength(35.0);
          this.mRow3.Height = new GridLength(0.0);
          this.mRow4.Height = new GridLength(80.0, GridUnitType.Star);
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.EdgeScroll:
          this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
          this.mActionIcon.MinHeight = 60.0;
          this.mActionIcon.MinWidth = 60.0;
          this.SetKeys(action, "", "", "", "", "");
          break;
        case KeyActionType.Callback:
          this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
          this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
          this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
          this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
          this.mActionIcon.MinHeight = 60.0;
          this.mActionIcon.MinWidth = 60.0;
          this.SetKeys(action, "Id", "", "", "", "");
          break;
        case KeyActionType.LookAround:
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.PanShoot:
          if (!this.mParentWindow.IsInOverlayMode)
            break;
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
        case KeyActionType.MOBASkillCancel:
          if (isGamepadEnabled)
          {
            this.SetKeys(action, "Key_alt1", "", "", "", "");
            break;
          }
          this.SetKeys(action, "Key", "", "", "", "");
          break;
      }
    }

    internal void SetToggleModeValues(IMAction action, bool isGamepadEnabled = false)
    {
      this.mToggleModeGrid.Visibility = Visibility.Visible;
      switch (action.Type)
      {
        case KeyActionType.TapRepeat:
          this.MinHeight = 92.0;
          BlueStacksUIBinding.Bind(this.mToggleMode1, "STRING_TAP_MODE", "");
          BlueStacksUIBinding.Bind(this.mToggleMode2, "STRING_LONG_PRESS_MODE", "");
          if (((TapRepeat) action).RepeatUntilKeyUp)
          {
            this.mToggleImage.ImageName = "right_switch";
            break;
          }
          this.mToggleImage.ImageName = "left_switch";
          break;
        case KeyActionType.FreeLook:
          BlueStacksUIBinding.Bind(this.mToggleMode1, "STRING_KEYBOARD_MODE", "");
          BlueStacksUIBinding.Bind(this.mToggleMode2, "STRING_MOUSE_MODE", "");
          if (((FreeLook) action).DeviceType == 0)
          {
            this.MinHeight = 182.0;
            this.mToggleImage.ImageName = "left_switch";
            this.mColumn0.Width = new GridLength(0.0);
            this.mColumn1.Width = new GridLength(25.0, GridUnitType.Star);
            this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
            this.mColumn3.Width = new GridLength(25.0, GridUnitType.Star);
            this.mColumn4.Width = new GridLength(0.0);
            this.mRow0.Height = new GridLength(10.0, GridUnitType.Star);
            this.mRow1.Height = new GridLength(25.0, GridUnitType.Star);
            this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
            this.mRow3.Height = new GridLength(25.0, GridUnitType.Star);
            this.mRow4.Height = new GridLength(10.0, GridUnitType.Star);
            if (isGamepadEnabled)
              this.SetKeys(action, "", "KeyLeft_alt1", "KeyUp_alt1", "KeyRight_alt1", "KeyDown_alt1");
            else
              this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
          }
          else if (((FreeLook) action).DeviceType == 1)
          {
            this.MinHeight = 117.0;
            this.mToggleImage.ImageName = "right_switch";
            if (isGamepadEnabled)
              this.SetKeys(action, "Key_alt1", "", "", "", "");
            else
              this.SetKeys(action, "Key", "", "", "", "");
          }
          this.SetActiveImage(true);
          break;
      }
    }

    private void InsertScriptSettingsClickGrid()
    {
      Grid grid1 = new Grid();
      grid1.Height = 19.0;
      grid1.Width = 19.0;
      grid1.HorizontalAlignment = HorizontalAlignment.Center;
      grid1.VerticalAlignment = VerticalAlignment.Bottom;
      grid1.Margin = new Thickness(30.0, 0.0, 0.0, 3.0);
      grid1.Background = (Brush) Brushes.Black;
      grid1.Opacity = 0.0001;
      Grid grid2 = grid1;
      Grid.SetRow((UIElement) grid2, 3);
      Grid.SetRowSpan((UIElement) grid2, 2);
      Grid.SetColumn((UIElement) grid2, 3);
      grid2.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScriptSettingsGrid_MouseLeftButtonUp);
      grid2.MouseEnter += new MouseEventHandler(this.ScriptSettingsGrid_MouseEnter);
      grid2.MouseLeave += new MouseEventHandler(this.ScriptSettingsGrid_MouseLeave);
      this.mGrid.Children.Add((UIElement) grid2);
    }

    private void ScriptSettingsGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mActionIcon.ImageName = this.ActionType.ToString() + "_canvas";
    }

    private void ScriptSettingsGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mActionIcon.ImageName = this.ActionType.ToString() + "_canvas_hover";
    }

    private static bool CheckForOffsetValueInGameControl(IMAction action)
    {
      bool flag = true;
      switch (action.Type)
      {
        case KeyActionType.LookAround:
          if (string.IsNullOrEmpty(((LookAround) action).LookAroundXOverlayOffset) && string.IsNullOrEmpty(((LookAround) action).LookAroundYOverlayOffset))
          {
            flag = false;
            break;
          }
          break;
        case KeyActionType.PanShoot:
          if (string.IsNullOrEmpty(((PanShoot) action).LButtonXOverlayOffset) && string.IsNullOrEmpty(((PanShoot) action).LButtonXOverlayOffset))
          {
            flag = false;
            break;
          }
          break;
        case KeyActionType.MOBASkillCancel:
          if (string.IsNullOrEmpty(((MOBASkillCancel) action).MOBASkillCancelOffsetX) && string.IsNullOrEmpty(((MOBASkillCancel) action).MOBASkillCancelOffsetY))
          {
            flag = false;
            break;
          }
          break;
        default:
          if (string.IsNullOrEmpty(action.XOverlayOffset) && string.IsNullOrEmpty(action.YOverlayOffset))
            return false;
          break;
      }
      return flag;
    }

    private void SetKeys(
      IMAction action,
      string center,
      string left,
      string up,
      string right,
      string down)
    {
      if (this.mParentWindow.IsInOverlayMode && action.Type == KeyActionType.FreeLook)
        this.ShowOverlayKeysOnImage(center, action, 0, 0);
      if (!this.mParentWindow.IsInOverlayMode && action.Type == KeyActionType.Script)
      {
        this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
        this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
        this.mColumn0.Width = new GridLength(2.0, GridUnitType.Star);
        this.InsertScriptSettingsClickGrid();
      }
      string text = string.Empty;
      bool flag = this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName) && this.mParentWindow.IsInOverlayMode && (action.Type == KeyActionType.Dpad && !string.IsNullOrEmpty(action is Dpad dpad ? dpad.GamepadStick : (string) null));
      if (!string.IsNullOrEmpty(center))
      {
        if (!string.IsNullOrEmpty(action[center].ToString()))
          text = KMManager.GetKeyUIValue(action[center].ToString());
        if (this.mParentWindow.IsInOverlayMode)
        {
          this.MinHeight = 50.0;
          this.MinWidth = 50.0;
          if (!string.IsNullOrEmpty(action[center].ToString()) && !((IEnumerable<string>) Constants.ImapGameControlsHiddenInOverlayList).Contains<string>(action.Type.ToString()))
          {
            if (CanvasElement.CheckForOffsetValueInGameControl(action))
            {
              if (action.Type == KeyActionType.Script)
                this.GetLabelsForOverlay(Positions.Center, text, action, 3, 3, center);
              else
                this.GetLabelsForOverlay(Positions.Center, text, action, 2, 2, center);
            }
            else
            {
              if (action.Type == KeyActionType.Tap | flag || action.Type == KeyActionType.MOBADpad)
              {
                this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
                this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
                this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
              }
              else if (action.Type == KeyActionType.TapRepeat)
              {
                this.MinWidth = 50.0;
                this.MinHeight = 50.0;
                this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
                this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
                this.mRow1.Height = new GridLength(1.0, GridUnitType.Auto);
                this.mRow2.Height = new GridLength(3.0, GridUnitType.Star);
                this.mRow3.Height = new GridLength(1.0, GridUnitType.Auto);
                this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
              }
              else if (action.Type == KeyActionType.MOBASkill)
              {
                this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
                this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
                this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
              }
              else if (action.Type == KeyActionType.MOBASkillCancel || action.Type == KeyActionType.Script)
              {
                this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
                this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
                this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
              }
              else if (action.Type == KeyActionType.Pan || action.Type == KeyActionType.PanShoot || action.Type == KeyActionType.LookAround)
              {
                this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
                this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
                this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
              }
              if (action.Type != KeyActionType.FreeLook)
                this.GetLabelsForOverlay(Positions.Center, text, action, 4, 4, center);
            }
          }
        }
        else
        {
          TextBlock newTextBlock = this.GetNewTextBlock(Positions.Center, center, action);
          if (action.Type == KeyActionType.Script)
          {
            Grid.SetColumn((UIElement) newTextBlock, 3);
            Grid.SetRow((UIElement) newTextBlock, 3);
          }
          else
          {
            Grid.SetColumn((UIElement) newTextBlock, 2);
            Grid.SetRow((UIElement) newTextBlock, 2);
          }
          BlueStacksUIBinding.Bind(newTextBlock, KMManager.GetStringsToShowInUI(text.ToString((IFormatProvider) CultureInfo.InvariantCulture)), "");
          if (action.Type != KeyActionType.MouseZoom && action.Type != KeyActionType.Callback)
          {
            newTextBlock.Visibility = Visibility.Visible;
            newTextBlock.ToolTip = (object) newTextBlock.Text;
          }
          else
            newTextBlock.Visibility = Visibility.Collapsed;
        }
      }
      if (!string.IsNullOrEmpty(left))
      {
        if (!string.IsNullOrEmpty(action[left].ToString()))
          text = KMManager.GetKeyUIValue(action[left].ToString());
        if (this.mParentWindow.IsInOverlayMode)
        {
          if (!string.IsNullOrEmpty(action[left].ToString()) && !((IEnumerable<string>) Constants.ImapGameControlsHiddenInOverlayList).Contains<string>(action.Type.ToString()))
          {
            if (CanvasElement.CheckForOffsetValueInGameControl(action))
            {
              this.GetLabelsForOverlay(Positions.Center, text, action, 2, 1, left);
            }
            else
            {
              if (action.Type == KeyActionType.Dpad)
                this.mColumn1.Width = new GridLength(50.0, GridUnitType.Star);
              else if (action.Type == KeyActionType.Rotate)
              {
                this.mColumn1.Width = new GridLength(60.0, GridUnitType.Star);
                this.ShowRotateControlOverlay(2, 1, 16.0, "overlay_left_arrow");
              }
              if (action.Type == KeyActionType.FreeLook)
              {
                this.mColumn2.Width = new GridLength(30.0, GridUnitType.Star);
                this.mColumn3.Width = new GridLength(40.0, GridUnitType.Star);
                this.GetLabelsForOverlay(Positions.Left, text, action, 2, 2, left);
              }
              else
                this.GetLabelsForOverlay(Positions.Left, text, action, 2, 1, left);
            }
          }
        }
        else
        {
          TextBlock newTextBlock = this.GetNewTextBlock(Positions.Left, left, action);
          if (action.Type == KeyActionType.Zoom)
          {
            Grid.SetColumn((UIElement) newTextBlock, 2);
            Grid.SetRow((UIElement) newTextBlock, 2);
          }
          else
          {
            Grid.SetColumn((UIElement) newTextBlock, 1);
            Grid.SetRow((UIElement) newTextBlock, 2);
          }
          BlueStacksUIBinding.Bind(newTextBlock, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[left].ToString()), "");
          newTextBlock.Visibility = Visibility.Visible;
          newTextBlock.ToolTip = (object) newTextBlock.Text;
          if (action.Type == KeyActionType.Zoom)
            BlueStacksUIBinding.BindColor((DependencyObject) newTextBlock, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
        }
      }
      if (!string.IsNullOrEmpty(up))
      {
        if (!string.IsNullOrEmpty(action[up].ToString()))
          text = KMManager.GetKeyUIValue(action[up].ToString());
        if (this.mParentWindow.IsInOverlayMode)
        {
          if (!string.IsNullOrEmpty(action[up].ToString()) && !((IEnumerable<string>) Constants.ImapGameControlsHiddenInOverlayList).Contains<string>(action.Type.ToString()))
          {
            if (CanvasElement.CheckForOffsetValueInGameControl(action))
            {
              this.GetLabelsForOverlay(Positions.Center, text, action, 1, 2, up);
            }
            else
            {
              if (action.Type == KeyActionType.Dpad)
                this.mColumn2.Width = new GridLength(50.0, GridUnitType.Star);
              if (action.Type == KeyActionType.FreeLook)
                this.GetLabelsForOverlay(Positions.Up, text, action, 1, 3, up);
              else
                this.GetLabelsForOverlay(Positions.Up, text, action, 1, 2, up);
            }
          }
        }
        else
        {
          TextBlock newTextBlock = this.GetNewTextBlock(Positions.Up, up, action);
          Grid.SetColumn((UIElement) newTextBlock, 2);
          Grid.SetRow((UIElement) newTextBlock, 1);
          BlueStacksUIBinding.Bind(newTextBlock, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[up].ToString()), "");
          newTextBlock.Visibility = Visibility.Visible;
          newTextBlock.ToolTip = (object) newTextBlock.Text;
          if (action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom)
            BlueStacksUIBinding.BindColor((DependencyObject) newTextBlock, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
        }
      }
      if (!string.IsNullOrEmpty(right))
      {
        if (!string.IsNullOrEmpty(action[right].ToString()))
          text = KMManager.GetKeyUIValue(action[right].ToString());
        if (this.mParentWindow.IsInOverlayMode)
        {
          if (!string.IsNullOrEmpty(action[right].ToString()) && !((IEnumerable<string>) Constants.ImapGameControlsHiddenInOverlayList).Contains<string>(action.Type.ToString()))
          {
            if (CanvasElement.CheckForOffsetValueInGameControl(action))
            {
              this.GetLabelsForOverlay(Positions.Center, text, action, 2, 3, right);
            }
            else
            {
              if (action.Type == KeyActionType.Dpad)
                this.mColumn3.Width = new GridLength(50.0, GridUnitType.Star);
              else if (action.Type == KeyActionType.Rotate)
              {
                this.mColumn3.Width = new GridLength(60.0, GridUnitType.Star);
                this.ShowRotateControlOverlay(2, 3, -16.0, "overlay_right_arrow");
              }
              if (action.Type == KeyActionType.FreeLook)
              {
                this.mColumn3.Width = new GridLength(70.0);
                this.mColumn4.Width = new GridLength(30.0, GridUnitType.Star);
                this.GetLabelsForOverlay(Positions.Right, text, action, 2, 4, right);
              }
              else
                this.GetLabelsForOverlay(Positions.Right, text, action, 2, 3, right);
            }
          }
        }
        else
        {
          TextBlock newTextBlock = this.GetNewTextBlock(Positions.Right, right, action);
          Grid.SetColumn((UIElement) newTextBlock, 3);
          Grid.SetRow((UIElement) newTextBlock, 2);
          BlueStacksUIBinding.Bind(newTextBlock, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[right].ToString()), "");
          newTextBlock.ToolTip = (object) newTextBlock.Text;
          newTextBlock.Visibility = Visibility.Visible;
        }
      }
      if (!string.IsNullOrEmpty(down))
      {
        if (!string.IsNullOrEmpty(action[down].ToString()))
          text = KMManager.GetKeyUIValue(action[down].ToString());
        if (this.mParentWindow.IsInOverlayMode)
        {
          if (!string.IsNullOrEmpty(action[down].ToString()) && !((IEnumerable<string>) Constants.ImapGameControlsHiddenInOverlayList).Contains<string>(action.Type.ToString()))
          {
            if (CanvasElement.CheckForOffsetValueInGameControl(action))
            {
              this.GetLabelsForOverlay(Positions.Center, text, action, 3, 2, down);
            }
            else
            {
              if (action.Type == KeyActionType.Dpad)
                this.mColumn2.Width = new GridLength(50.0, GridUnitType.Star);
              if (action.Type == KeyActionType.FreeLook)
                this.GetLabelsForOverlay(Positions.Down, text, action, 3, 3, down);
              else
                this.GetLabelsForOverlay(Positions.Down, text, action, 3, 2, down);
            }
          }
        }
        else
        {
          TextBlock newTextBlock = this.GetNewTextBlock(Positions.Down, down, action);
          Grid.SetColumn((UIElement) newTextBlock, 2);
          Grid.SetRow((UIElement) newTextBlock, 3);
          BlueStacksUIBinding.Bind(newTextBlock, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[down].ToString()), "");
          newTextBlock.ToolTip = (object) newTextBlock.Text;
          newTextBlock.Visibility = Visibility.Visible;
        }
      }
      if (this.mParentWindow.IsInOverlayMode)
      {
        this.mCanvasGrid.Visibility = Visibility.Collapsed;
        this.mToggleModeGrid.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.mCanvasGrid.Visibility = Visibility.Visible;
        if (action.Type != KeyActionType.TapRepeat && action.Type != KeyActionType.FreeLook)
          return;
        this.mToggleModeGrid.Visibility = Visibility.Visible;
      }
    }

    private void ShowRotateControlOverlay(int row, int column, double margin, string imageName)
    {
      Grid grid1 = new Grid();
      grid1.MinHeight = 50.0;
      grid1.MinWidth = 50.0;
      grid1.Visibility = Visibility.Visible;
      grid1.HorizontalAlignment = HorizontalAlignment.Stretch;
      grid1.VerticalAlignment = VerticalAlignment.Center;
      Grid grid2 = grid1;
      CustomPictureBox customPictureBox1 = new CustomPictureBox();
      customPictureBox1.Visibility = Visibility.Visible;
      customPictureBox1.HorizontalAlignment = HorizontalAlignment.Center;
      customPictureBox1.VerticalAlignment = VerticalAlignment.Center;
      CustomPictureBox customPictureBox2 = customPictureBox1;
      customPictureBox2.ImageName = imageName;
      grid2.Children.Add((UIElement) customPictureBox2);
      grid2.Margin = new Thickness(margin, 0.0, 0.0, 0.0);
      Grid.SetRow((UIElement) grid2, row);
      Grid.SetColumn((UIElement) grid2, column);
      this.mGrid.Children.Add((UIElement) grid2);
    }

    private void ShowOverlayKeysOnImage(string s, IMAction action, int row, int column)
    {
      Grid grid1 = new Grid();
      grid1.MinHeight = 50.0;
      grid1.MinWidth = 60.0;
      grid1.Visibility = Visibility.Visible;
      grid1.HorizontalAlignment = HorizontalAlignment.Stretch;
      grid1.VerticalAlignment = VerticalAlignment.Center;
      Grid grid2 = grid1;
      CustomPictureBox customPictureBox1 = new CustomPictureBox();
      customPictureBox1.Visibility = Visibility.Visible;
      customPictureBox1.HorizontalAlignment = HorizontalAlignment.Center;
      customPictureBox1.VerticalAlignment = VerticalAlignment.Center;
      CustomPictureBox customPictureBox2 = customPictureBox1;
      if (((FreeLook) action).DeviceType == 0)
      {
        customPictureBox2.Height = 196.0;
        customPictureBox2.Width = 98.0;
        customPictureBox2.ImageName = "overlay_keyboard";
        customPictureBox2.Margin = new Thickness(0.0, 6.0, 0.0, 0.0);
        grid2.Children.Add((UIElement) customPictureBox2);
        Grid.SetRow((UIElement) grid2, row);
        Grid.SetColumn((UIElement) grid2, 1);
        Grid.SetRowSpan((UIElement) grid2, 5);
        Grid.SetColumnSpan((UIElement) grid2, 5);
      }
      else
      {
        customPictureBox2.ImageName = "overlay_mouse";
        customPictureBox2.HorizontalAlignment = HorizontalAlignment.Stretch;
        Label label = new Label();
        BlueStacksUIBinding.Bind(label, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[s].ToString()));
        label.Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
        label.Padding = new Thickness(2.0);
        label.FontSize = 11.0;
        label.Foreground = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFFFF"));
        label.FontWeight = FontWeights.DemiBold;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        Typeface typeface = new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch);
        FormattedText formattedText = new FormattedText(label.Content.ToString(), Thread.CurrentThread.CurrentCulture, label.FlowDirection, typeface, label.FontSize, label.Foreground);
        customPictureBox2.Width = formattedText.WidthIncludingTrailingWhitespace + 10.0 > 40.0 ? formattedText.WidthIncludingTrailingWhitespace + 10.0 : 40.0;
        customPictureBox2.Height = customPictureBox2.Width > 50.0 ? 50.0 : customPictureBox2.Width;
        grid2.Children.Add((UIElement) customPictureBox2);
        grid2.Children.Add((UIElement) label);
        Grid.SetRow((UIElement) grid2, 2);
        Grid.SetRowSpan((UIElement) grid2, 4);
        Grid.SetColumn((UIElement) grid2, column);
      }
      this.mGrid.Children.Add((UIElement) grid2);
    }

    private void GetLabelsForOverlay(
      Positions p,
      string text,
      IMAction action,
      int row,
      int column,
      string key)
    {
      bool flag = this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName) && this.mParentWindow.IsInOverlayMode && (action.Type == KeyActionType.Dpad && !string.IsNullOrEmpty(action is Dpad dpad ? dpad.GamepadStick : (string) null));
      if (ShowImagesOnOverlay.ListShowImagesForKeys.Contains(action[key].ToString()))
      {
        Grid grid1 = new Grid();
        grid1.MinHeight = 29.0;
        grid1.MinWidth = 29.0;
        grid1.Visibility = Visibility.Visible;
        grid1.HorizontalAlignment = HorizontalAlignment.Center;
        grid1.VerticalAlignment = VerticalAlignment.Center;
        Grid grid2 = grid1;
        CustomPictureBox customPictureBox1 = new CustomPictureBox();
        customPictureBox1.Height = 27.0;
        customPictureBox1.Width = 27.0;
        customPictureBox1.Visibility = Visibility.Visible;
        customPictureBox1.HorizontalAlignment = HorizontalAlignment.Center;
        customPictureBox1.VerticalAlignment = VerticalAlignment.Center;
        customPictureBox1.ImageName = "Imap_" + action[key].ToString();
        CustomPictureBox customPictureBox2 = customPictureBox1;
        grid2.Children.Add((UIElement) customPictureBox2);
        this.mGrid.Children.Add((UIElement) grid2);
        Grid.SetRow((UIElement) grid2, row);
        Grid.SetColumn((UIElement) grid2, column);
      }
      else if (action.Type == KeyActionType.Dpad && !flag || (action.Type == KeyActionType.MOBASkill || action.Type == KeyActionType.FreeLook) || action.Type == KeyActionType.Rotate)
      {
        if (action.Type == KeyActionType.Dpad)
        {
          Grid labelGrid = this.GetLabelGrid(text);
          Grid.SetRow((UIElement) labelGrid, row);
          Grid.SetColumn((UIElement) labelGrid, column);
        }
        else
        {
          Grid labelGrid = this.GetLabelGrid(text);
          Grid.SetRow((UIElement) labelGrid, row);
          Grid.SetColumn((UIElement) labelGrid, column);
          if (p.Equals((object) Positions.Up))
            labelGrid.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
          if (!p.Equals((object) Positions.Down))
            return;
          labelGrid.Margin = new Thickness(0.0, -20.0, 0.0, 0.0);
        }
      }
      else
      {
        OverlayLabelControl overlayLabelControl = new OverlayLabelControl();
        BlueStacksUIBinding.Bind(overlayLabelControl.lbl, KMManager.GetStringsToShowInUI(text));
        overlayLabelControl.MinHeight = 27.0;
        overlayLabelControl.MinWidth = 27.0;
        overlayLabelControl.lbl.HorizontalAlignment = HorizontalAlignment.Center;
        overlayLabelControl.lbl.VerticalAlignment = VerticalAlignment.Center;
        overlayLabelControl.lbl.Margin = new Thickness(3.0, 0.0, 3.0, 1.0);
        overlayLabelControl.lbl.Padding = new Thickness(5.0);
        overlayLabelControl.lbl.FontSize = 11.0;
        this.mGrid.Children.Add((UIElement) overlayLabelControl);
        Grid.SetRow((UIElement) overlayLabelControl, row);
        Grid.SetColumn((UIElement) overlayLabelControl, column);
      }
    }

    private Grid GetLabelGrid(string text)
    {
      Grid grid1 = new Grid();
      grid1.MinHeight = 28.0;
      grid1.MinWidth = 28.0;
      grid1.Visibility = Visibility.Visible;
      grid1.HorizontalAlignment = HorizontalAlignment.Center;
      grid1.VerticalAlignment = VerticalAlignment.Center;
      Grid grid2 = grid1;
      Border border = new Border();
      Label label = new Label();
      BlueStacksUIBinding.BindColor((DependencyObject) border, Border.BorderBrushProperty, "OverlayLabelBorderColor");
      RenderOptions.SetEdgeMode((DependencyObject) border, EdgeMode.Unspecified);
      BlueStacksUIBinding.BindColor((DependencyObject) border, Border.BackgroundProperty, "OverlayLabelBackgroundColor");
      border.SnapsToDevicePixels = false;
      border.ClipToBounds = false;
      border.CornerRadius = new CornerRadius(14.0);
      border.BorderThickness = new Thickness(1.5);
      BlueStacksUIBinding.Bind(label, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text));
      label.Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
      label.Padding = new Thickness(1.0);
      label.FontSize = 11.0;
      label.Foreground = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFFFFFFF"));
      label.FontWeight = FontWeights.Bold;
      label.HorizontalAlignment = HorizontalAlignment.Center;
      label.VerticalAlignment = VerticalAlignment.Center;
      grid2.Children.Add((UIElement) border);
      grid2.Children.Add((UIElement) label);
      this.mGrid.Children.Add((UIElement) grid2);
      return grid2;
    }

    private TextBlock GetNewTextBlock(Positions p, string s, IMAction action)
    {
      TextBlock textBlock1;
      if (this.dictTextElemets.ContainsKey(p))
      {
        this.dictTextElemets[p].Item4.Add(action);
        textBlock1 = this.dictTextElemets[p].Item3;
      }
      else
      {
        TextBlock textBlock2 = new TextBlock();
        textBlock2.FontSize = 14.0;
        textBlock2.FontWeight = FontWeights.Bold;
        textBlock2.Background = (Brush) Brushes.Transparent;
        textBlock2.TextTrimming = TextTrimming.CharacterEllipsis;
        textBlock2.Padding = new Thickness(5.0, 2.0, 5.0, 2.0);
        textBlock2.Foreground = (Brush) Brushes.Black;
        textBlock2.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock2.VerticalAlignment = VerticalAlignment.Center;
        textBlock1 = textBlock2;
        textBlock1.PreviewMouseUp += new MouseButtonEventHandler(this.TxtBlock_PreviewMouseUp);
        textBlock1.Name = p.ToString();
        TextBox textBox1 = new TextBox();
        textBox1.FontSize = 14.0;
        textBox1.FontWeight = FontWeights.Bold;
        TextBox textBox2 = textBox1;
        BlueStacksUIBinding.BindColor((DependencyObject) textBox2, Control.BackgroundProperty, "ComboBoxBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) textBox2, Control.BorderBrushProperty, "ComboBoxBorderColor");
        BlueStacksUIBinding.BindColor((DependencyObject) textBox2, Control.ForegroundProperty, "ComboBoxForegroundColor");
        textBox2.Padding = new Thickness(0.0, 1.0, 0.0, 1.0);
        textBox2.HorizontalAlignment = HorizontalAlignment.Center;
        textBox2.VerticalAlignment = VerticalAlignment.Center;
        textBox2.TextAlignment = TextAlignment.Center;
        textBox2.LostFocus += new RoutedEventHandler(this.TxtBox_LostFocus);
        textBox2.GotFocus += new RoutedEventHandler(this.TxtBox_GotFocus);
        textBox2.MinWidth = 24.0;
        textBox2.TextWrapping = TextWrapping.Wrap;
        InputMethod.SetIsInputMethodEnabled((DependencyObject) textBox2, false);
        textBox2.Name = p.ToString();
        textBox2.Visibility = Visibility.Collapsed;
        textBox2.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.SelectivelyIgnoreMouseButton);
        textBox2.PreviewMouseDown += new MouseButtonEventHandler(this.TxtBox_PreviewMouseDown);
        textBox2.PreviewKeyDown += new KeyEventHandler(this.TxtBox_PreviewKeyDown);
        textBox2.KeyUp += new KeyEventHandler(this.TxtBox_KeyUp);
        textBox2.TextChanged += new TextChangedEventHandler(this.TxtBox_TextChanged);
        textBox2.PreviewMouseWheel += new MouseWheelEventHandler(this.TxtBox_PreviewMouseWheel);
        this.mGrid.Children.Add((UIElement) textBlock1);
        this.mGrid.Children.Add((UIElement) textBox2);
        this.dictTextElemets.Add(p, new BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>(s, textBox2, textBlock1, new List<IMAction>()
        {
          action
        }));
      }
      return textBlock1;
    }

    private void TxtBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      if (e.Delta > 0)
      {
        e.Handled = true;
        textBox.Tag = (object) "MouseWheelUp";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
      }
      else
      {
        if (e.Delta >= 0)
          return;
        e.Handled = true;
        textBox.Tag = (object) "MouseWheelDown";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
      }
    }

    private void TxtBox_KeyUp(object sender, KeyEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      if (this.ActionType == KeyActionType.Tap || this.ListActionItem[0].Type == KeyActionType.TapRepeat || this.ListActionItem[0].Type == KeyActionType.Script)
      {
        if (this.mKeyList.Count >= 2)
        {
          string str1 = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1));
          string str2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(this.mKeyList.Count - 1));
          textBox.Text = str1;
          textBox.Tag = (object) str2;
          this.SetValueHandling(sender);
        }
        else if (this.mKeyList.Count == 1)
        {
          string stringForUi = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt<Key>(0));
          string stringForFile = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt<Key>(0));
          textBox.Text = stringForUi;
          textBox.Tag = (object) stringForFile;
          this.SetValueHandling(sender);
        }
        textBox.CaretIndex = textBox.Text.Length;
        this.mKeyList.Clear();
      }
      else
        e.Handled = true;
    }

    internal void SetMousePoint(Point mousePoint)
    {
      this.mMousePointForTap = new Point?(mousePoint);
    }

    private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.SetValueHandling(sender);
    }

    private void SetValueHandling(object sender)
    {
      TextBox mKeyTextBox = sender as TextBox;
      Positions index1 = EnumHelper.Parse<Positions>(mKeyTextBox.Name.ToString((IFormatProvider) CultureInfo.InvariantCulture), Positions.Center);
      string index2 = this.dictTextElemets[index1].Item1;
      string str = this.dictTextElemets[index1].Item4[0][index2].ToString();
      if (mKeyTextBox.Tag != null)
        str = mKeyTextBox.Tag.ToString();
      this.Setvalue(mKeyTextBox, str);
    }

    private void Setvalue(TextBox mKeyTextBox, string value)
    {
      Positions index1 = EnumHelper.Parse<Positions>(mKeyTextBox.Name.ToString((IFormatProvider) CultureInfo.InvariantCulture), Positions.Center);
      string index2 = this.dictTextElemets[index1].Item1;
      foreach (IMAction imAction in this.dictTextElemets[index1].Item4)
      {
        if (!string.Equals(imAction[index2].ToString(), value, StringComparison.InvariantCulture))
        {
          imAction[index2] = (object) value;
          KeymapCanvasWindow.sIsDirty = true;
        }
      }
      if (!index2.StartsWith("Key", StringComparison.InvariantCulture))
        return;
      mKeyTextBox.Text = mKeyTextBox.Text;
    }

    private void TxtBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        return;
      KMManager.CheckAndCreateNewScheme();
      if (this.IsRemoveIfEmpty)
      {
        this.IsRemoveIfEmpty = false;
        this.UpdatePosition(Canvas.GetTop((UIElement) this), Canvas.GetLeft((UIElement) this));
      }
      TextBox textBox = sender as TextBox;
      if (this.ActionType == KeyActionType.Tap || this.ActionType == KeyActionType.TapRepeat || this.ActionType == KeyActionType.Script)
      {
        if (e.Key == Key.Back || e.SystemKey == Key.Back)
        {
          textBox.Tag = (object) string.Empty;
          BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + textBox.Tag?.ToString());
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
      else if (e.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
      {
        textBox.Tag = (object) IMAPKeys.GetStringForFile(e.SystemKey);
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
      }
      else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
      {
        textBox.Tag = (object) IMAPKeys.GetStringForFile(e.Key);
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
      }
      else if (e.Key == Key.Back)
      {
        textBox.Tag = (object) string.Empty;
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + textBox.Tag?.ToString());
      }
      e.Handled = true;
      textBox.Focus();
      textBox.SelectAll();
    }

    private void TxtBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.IsRemoveIfEmpty)
      {
        this.IsRemoveIfEmpty = false;
        this.UpdatePosition(Canvas.GetTop((UIElement) this), Canvas.GetLeft((UIElement) this));
      }
      TextBox textBox = sender as TextBox;
      if (e.MiddleButton == MouseButtonState.Pressed)
      {
        e.Handled = true;
        KMManager.CheckAndCreateNewScheme();
        textBox.Tag = (object) "MouseMButton";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseMButton");
      }
      else if (e.RightButton == MouseButtonState.Pressed)
      {
        e.Handled = true;
        KMManager.CheckAndCreateNewScheme();
        textBox.Tag = (object) "MouseRButton";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseRButton");
      }
      else if (e.XButton1 == MouseButtonState.Pressed)
      {
        e.Handled = true;
        KMManager.CheckAndCreateNewScheme();
        textBox.Tag = (object) "MouseXButton1";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseXButton1");
      }
      else if (e.XButton2 == MouseButtonState.Pressed)
      {
        e.Handled = true;
        KMManager.CheckAndCreateNewScheme();
        textBox.Tag = (object) "MouseXButton2";
        BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseXButton2");
      }
      if (e.LeftButton == MouseButtonState.Pressed && textBox.IsKeyboardFocusWithin)
        e.Handled = true;
      textBox.Focus();
      textBox.SelectAll();
    }

    private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is TextBox textBox) || textBox.IsKeyboardFocusWithin)
        return;
      e.Handled = true;
      textBox.Focus();
    }

    private void TxtBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (Math.Abs(this.TopOnClick - Canvas.GetTop((UIElement) this)) >= 2.0 || Math.Abs(this.LeftOnClick - Canvas.GetLeft((UIElement) this)) >= 2.0)
        return;
      this.ShowTextBox(sender);
    }

    internal void ShowTextBox(object sender)
    {
      IMAction imAction = this.ListActionItem.FirstOrDefault<IMAction>();
      Positions index = EnumHelper.Parse<Positions>((sender as TextBlock).Name.ToString((IFormatProvider) CultureInfo.InvariantCulture), Positions.Center);
      if (imAction != null && imAction.Type != KeyActionType.MouseZoom)
        this.dictTextElemets[index].Item2.Visibility = Visibility.Visible;
      this.dictTextElemets[index].Item3.Visibility = Visibility.Collapsed;
      this.dictTextElemets[index].Item2.Text = this.dictTextElemets[index].Item3.Text;
      Grid.SetColumn((UIElement) this.dictTextElemets[index].Item2, Grid.GetColumn((UIElement) this.dictTextElemets[index].Item3));
      Grid.SetRow((UIElement) this.dictTextElemets[index].Item2, Grid.GetRow((UIElement) this.dictTextElemets[index].Item3));
      MiscUtils.SetFocusAsync((UIElement) this.dictTextElemets[index].Item2, 100);
      if (CanvasElement.sFocusedTextBox == null)
        CanvasElement.sFocusedTextBox = (object) this.dictTextElemets[index].Item2;
      if (imAction == null || imAction.Type != KeyActionType.MOBASkill)
        return;
      this.mActionIcon.Visibility = Visibility.Visible;
    }

    private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
    {
      CanvasElement.sFocusedTextBox = sender;
      this.SetActiveImage(true);
    }

    internal void TxtBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if ((sender as TextBox).Visibility != Visibility.Visible)
        return;
      CanvasElement.sFocusedTextBox = (object) null;
      Positions index = EnumHelper.Parse<Positions>((sender as TextBox).Name.ToString((IFormatProvider) CultureInfo.InvariantCulture), Positions.Center);
      this.dictTextElemets[index].Item3.Visibility = Visibility.Visible;
      this.dictTextElemets[index].Item2.Visibility = Visibility.Collapsed;
      this.dictTextElemets[index].Item3.Text = this.dictTextElemets[index].Item2.Text;
      this.SetActiveImage(false);
      if (this.IsRemoveIfEmpty && !this.mParentWindow.mIsExtraSettingsPopupOpened)
      {
        Logger.Debug("DELETE_TAP: Empty canvas element deleted");
        this.DeleteElement();
      }
      if (this.ListActionItem.First<IMAction>().Type != KeyActionType.MOBASkill)
        return;
      this.mActionIcon.Visibility = Visibility.Collapsed;
      this.mSkillImage.Visibility = Visibility.Collapsed;
      this.mCloseIcon.Visibility = Visibility.Collapsed;
    }

    internal void ShowOtherIcons(bool isShow = true)
    {
      IMAction imAction = this.ListActionItem.FirstOrDefault<IMAction>();
      if (isShow)
      {
        if (imAction == null || imAction.RadiusProperty == -1.0)
          return;
        if (KMManager.sIsDeveloperModeOn)
        {
          this.mResizeIcon.Visibility = Visibility.Visible;
        }
        else
        {
          if (imAction.Type == KeyActionType.MouseZoom)
            return;
          this.mResizeIcon.Visibility = Visibility.Visible;
        }
      }
      else
      {
        this.mSkillImage.Visibility = Visibility.Hidden;
        this.mResizeIcon.Visibility = Visibility.Hidden;
      }
    }

    private void SetMOBASkillSettingsContent()
    {
      if (!((MOBASkill) this.ListActionItem.First<IMAction>()).AdvancedMode && !((MOBASkill) this.ListActionItem.First<IMAction>()).AutocastEnabled)
        this.MOBASkillSettingsPopup.mManualSkill.IsChecked = new bool?(true);
      else if (((MOBASkill) this.ListActionItem.First<IMAction>()).AdvancedMode && !((MOBASkill) this.ListActionItem.First<IMAction>()).AutocastEnabled)
        this.MOBASkillSettingsPopup.mAutoSkill.IsChecked = new bool?(true);
      else if (((MOBASkill) this.ListActionItem.First<IMAction>()).AdvancedMode && ((MOBASkill) this.ListActionItem.First<IMAction>()).AutocastEnabled)
        this.MOBASkillSettingsPopup.mQuickSkill.IsChecked = new bool?(true);
      this.MOBASkillSettingsPopup.mStopMovementCheckbox.IsChecked = new bool?(((MOBASkill) this.ListActionItem.First<IMAction>()).StopMOBADpad);
      this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.Inlines.Clear();
      this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
      this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.NavigateUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=moba_stop_movement_help");
      this.MOBASkillSettingsMoreInfoPopup.mHyperLink.Inlines.Clear();
      this.MOBASkillSettingsMoreInfoPopup.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
      this.MOBASkillSettingsMoreInfoPopup.mHyperLink.NavigateUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=" + "moba_skill_settings_help");
    }

    private void SetSize(IMAction action)
    {
      if (!string.IsNullOrEmpty(IMAction.sRadiusPropertyName[action.Type]))
        return;
      this.mActionIcon.IsAlwaysHalfSize = true;
      this.MaxHeight = this.mActionIcon.MaxHeight;
    }

    internal void UpdatePosition(double top, double left)
    {
      Canvas mCanvas = this.mParentWindow.mCanvas;
      double num1 = (left + this.ActualWidth / 2.0) / mCanvas.ActualWidth * 100.0;
      double num2 = (top + this.ActualHeight / 2.0) / mCanvas.ActualHeight * 100.0;
      double num3 = this.ActualWidth / mCanvas.ActualWidth * 50.0;
      foreach (IMAction imAction in this.ListActionItem)
      {
        imAction.PositionX = Math.Round(num1, 2);
        imAction.PositionY = Math.Round(num2, 2);
        imAction.RadiusProperty = Math.Round(num3, 2);
        if (this.ListActionItem.First<IMAction>().Type == KeyActionType.Swipe || this.ListActionItem.First<IMAction>().Type == KeyActionType.Tilt)
        {
          this.mSettingsIcon.Margin = new Thickness(this.ActualWidth / 9.0, 0.0, 0.0, this.ActualWidth / 9.0);
          this.mCloseIcon.Margin = new Thickness(0.0, this.ActualWidth / 9.0, this.ActualWidth / 9.0, 0.0);
          this.mResizeIcon.Margin = new Thickness(0.0, 0.0, this.ActualWidth / 9.0, this.ActualWidth / 9.0);
        }
      }
    }

    internal static List<CanvasElement> GetCanvasElement(
      IMAction action,
      KeymapCanvasWindow window,
      MainWindow mainWindow)
    {
      List<CanvasElement> canvasElementList = new List<CanvasElement>();
      object[] customAttributes = action.GetType().GetCustomAttributes(typeof (DescriptionAttribute), true);
      if (customAttributes.Length != 0 && customAttributes[0] is DescriptionAttribute descriptionAttribute)
      {
        if (descriptionAttribute.Description.Contains("Dependent"))
        {
          if (CanvasElement.dictPoints.ContainsKey(action.PositionX.ToString() + "~" + action.PositionY.ToString()))
          {
            CanvasElement dictPoint = CanvasElement.dictPoints[action.PositionX.ToString() + "~" + action.PositionY.ToString()];
            dictPoint.AddAction(action);
            canvasElementList.Add(dictPoint);
          }
          else
          {
            CanvasElement canvasElement = new CanvasElement(window, mainWindow);
            canvasElement.AddAction(action);
            canvasElement.ShowOtherIcons(true);
            CanvasElement.dictPoints.Add(action.PositionX.ToString() + "~" + action.PositionY.ToString(), canvasElement);
            canvasElementList.Add(canvasElement);
          }
        }
        else if (descriptionAttribute.Description.Contains("ParentElement"))
        {
          CanvasElement canvasElement = new CanvasElement(window, mainWindow);
          canvasElement.AddAction(action);
          canvasElement.ShowOtherIcons(true);
          canvasElementList.Add(canvasElement);
          switch (action)
          {
            case Pan _:
              Pan pan = action as Pan;
              if (pan.mLookAround != null)
                canvasElementList.Add(CanvasElement.GetCanvasElement((IMAction) pan.mLookAround, window, mainWindow).First<CanvasElement>());
              if (pan.mPanShoot != null)
              {
                canvasElementList.Add(CanvasElement.GetCanvasElement((IMAction) pan.mPanShoot, window, mainWindow).First<CanvasElement>());
                break;
              }
              break;
            case MOBASkill _:
              MOBASkill mobaSkill = action as MOBASkill;
              if (mobaSkill.mMOBASkillCancel != null)
              {
                canvasElementList.Add(CanvasElement.GetCanvasElement((IMAction) mobaSkill.mMOBASkillCancel, window, mainWindow).First<CanvasElement>());
                break;
              }
              break;
          }
        }
        else
        {
          CanvasElement canvasElement = new CanvasElement(window, mainWindow);
          canvasElement.AddAction(action);
          canvasElement.ShowOtherIcons(true);
          canvasElementList.Add(canvasElement);
        }
      }
      return canvasElementList;
    }

    internal void RemoveAction(string actionItemProperty = "")
    {
      if (this.ListActionItem.Count > 1)
      {
        this.ListActionItem.RemoveAt(0);
      }
      else
      {
        if (this.Parent == null)
          return;
        (this.Parent as Canvas).Children.Remove((UIElement) this);
        IMAction parentAction = this.ListActionItem.First<IMAction>().ParentAction;
        if (!parentAction.Guidance.ContainsKey(actionItemProperty))
          return;
        parentAction.Guidance.Remove(actionItemProperty);
      }
    }

    private void CanvasElement_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.MOBASkillSettingsPopup.IsOpen || this.MOBAOtherSettingsMoreInfoPopup.IsOpen || this.MOBASkillSettingsMoreInfoPopup.IsOpen)
        return;
      this.OpenPopup();
    }

    internal void OpenPopup()
    {
      List<IMAction> imActionList = new List<IMAction>();
      List<IMAction> listToAdd;
      if (this.ListActionItem.First<IMAction>().IsChildAction)
      {
        listToAdd = new List<IMAction>()
        {
          this.ListActionItem.First<IMAction>().ParentAction
        };
        this.SetActiveImage(true);
      }
      else
        listToAdd = this.ListActionItem;
      if (listToAdd.Count == 0)
        return;
      KeymapExtraSettingWindow extraSettingWindow = new KeymapExtraSettingWindow(this.ParentWindow);
      extraSettingWindow.ListAction.ClearAddRange<IMAction>(listToAdd);
      extraSettingWindow.mCanvasElement = this;
      extraSettingWindow.Placement = PlacementMode.Relative;
      extraSettingWindow.PlacementTarget = (UIElement) this;
      extraSettingWindow.StaysOpen = false;
      extraSettingWindow.Init(false);
      this.mParentWindow.mIsExtraSettingsPopupOpened = true;
      this.SetActiveImage(false);
      extraSettingWindow.IsTopmost = true;
      extraSettingWindow.IsOpen = true;
      Point position = Mouse.GetPosition((IInputElement) this);
      extraSettingWindow.HorizontalOffset = position.X;
      extraSettingWindow.VerticalOffset = position.Y;
      extraSettingWindow.Closed += new EventHandler(this.ExtraSettingPopup_Closed);
    }

    private void SetActiveImage(bool isActive = true)
    {
      string str = this.mType.ToString();
      if (this.mType == KeyActionType.MOBASkill && this.mParentWindow.IsInOverlayMode)
        str = KeyActionType.Tap.ToString();
      if (this.mType == KeyActionType.Dpad)
        this.mGrid.Visibility = Visibility.Visible;
      if (this.mType == KeyActionType.FreeLook)
      {
        if ((this.ListActionItem.First<IMAction>() as FreeLook).DeviceType == 0)
        {
          str += "_keyboard";
          this.mSettingsIcon.Visibility = Visibility.Visible;
          this.mCloseIcon.Visibility = Visibility.Visible;
        }
        else
        {
          str += "_mouse";
          this.mActionIcon.AllowClickThrough = false;
        }
      }
      if (isActive)
      {
        this.mActionIcon.ImageName = str + "_canvas_active";
        this.mActionIcon2.ImageName = this.mActionIcon.ImageName + "_2";
      }
      else
      {
        this.mActionIcon.ImageName = str + "_canvas";
        this.mActionIcon2.ImageName = this.mActionIcon.ImageName + "_2";
      }
      this.mActionIcon.Visibility = Visibility.Visible;
      this.mActionIcon2.Visibility = Visibility.Visible;
      if ((this.mType == KeyActionType.State || this.mType == KeyActionType.MouseZoom || this.mType == KeyActionType.Callback) && !KMManager.sIsDeveloperModeOn)
      {
        this.mCloseIcon.Visibility = Visibility.Collapsed;
        this.mResizeIcon.Visibility = Visibility.Collapsed;
        this.mActionIcon.Visibility = Visibility.Collapsed;
        this.mActionIcon2.Visibility = Visibility.Collapsed;
        this.mSettingsIcon.Visibility = Visibility.Collapsed;
      }
      if (this.mType == KeyActionType.Zoom || this.mType == KeyActionType.MouseZoom)
      {
        this.mCloseIcon.Margin = new Thickness(-20.0, 20.0, 20.0, -20.0);
        this.mActionIcon2.Margin = new Thickness(-55.0, 0.0, 0.0, 0.0);
        this.mResizeIcon.Margin = new Thickness(-20.0, -20.0, 20.0, 20.0);
        this.mSettingsIcon.Margin = new Thickness(20.0, -20.0, -20.0, 20.0);
      }
      if (this.mType == KeyActionType.MOBASkill && CanvasElement.sFocusedTextBox == null)
      {
        this.mActionIcon.Visibility = Visibility.Collapsed;
        this.mSkillImage.Visibility = Visibility.Collapsed;
        this.mCloseIcon.Visibility = Visibility.Collapsed;
      }
      if (this.ListActionItem.Count == 0 || !this.ListActionItem.First<IMAction>().IsChildAction || !this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>().ParentAction))
        return;
      this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>().ParentAction].SetActiveImage(isActive);
    }

    internal void ExtraSettingPopup_Closed(object sender, EventArgs e)
    {
      if (this.mParentWindow.mIsClosing)
        return;
      KeymapExtraSettingWindow extraSettingWindow = sender as KeymapExtraSettingWindow;
      string text = extraSettingWindow.mGuidanceCategoryComboBox.mAutoComboBox.Text;
      foreach (IMAction imAction in extraSettingWindow.ListAction)
      {
        if (!string.Equals(imAction.GuidanceCategory, text, StringComparison.InvariantCulture))
        {
          imAction.GuidanceCategory = text;
          KeymapCanvasWindow.sIsDirty = true;
          this.ParentWindow.SelectedConfig.AddString(imAction.GuidanceCategory);
        }
      }
      this.SetKeysForActions(this.ListActionItem);
      this.SetActiveImage(false);
      if (this.ListActionItem.First<IMAction>().Type == KeyActionType.Zoom || this.ListActionItem.First<IMAction>().Type == KeyActionType.MouseZoom)
        this.ListActionItem.First<IMAction>().RadiusProperty = this.ListActionItem.First<IMAction>().RadiusProperty;
      if (this.ParentWindow.SelectedConfig.ControlSchemes == null)
        this.ParentWindow.SelectedConfig.ControlSchemes = new List<IMControlScheme>();
      KMManager.sGamepadDualTextbox = (DualTextBlockControl) null;
      KMManager.CallGamepadHandler(this.ParentWindow, "false");
      this.SetElementLayout(false, 0.0, 0.0);
      this.mParentWindow.mIsExtraSettingsPopupOpened = false;
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.ListActionItem.Count != 0 && this.mParentWindow != null)
      {
        if (!this.mParentWindow.IsInOverlayMode)
        {
          this.SetElementLayout(true, 0.0, 0.0);
        }
        else
        {
          this.SetElementLayout(true, this.mXPosition, this.mYPosition);
          if (this.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
            KMManager.CanvasWindow.SetOnboardingControlPosition(this.mXPosition, this.mYPosition);
        }
      }
      this.mIsLoadingfromFile = false;
      this.mMousePointForTap = new Point?();
    }

    internal void SetElementLayout(bool isLoaded = false, double xPos = 0.0, double yPos = 0.0)
    {
      IMAction imAction = this.ListActionItem.First<IMAction>();
      bool flag = this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName) && this.mParentWindow.IsInOverlayMode && (imAction.Type == KeyActionType.Dpad && !string.IsNullOrEmpty(imAction is Dpad dpad ? dpad.GamepadStick : (string) null));
      if (xPos <= 0.0)
        xPos = imAction.PositionX;
      if (yPos <= 0.0)
        yPos = imAction.PositionY;
      double num1;
      if (((imAction.RadiusProperty == -1.0 ? 1 : (!this.mParentWindow.IsInOverlayMode ? 0 : (imAction.Type == KeyActionType.MOBASkill ? 1 : (imAction.Type == KeyActionType.MOBADpad ? 1 : 0)))) | (flag ? 1 : 0)) != 0)
      {
        num1 = this.ActualWidth;
      }
      else
      {
        num1 = imAction.RadiusProperty * 2.0 / 100.0 * this.mParentWindow.mCanvas.ActualWidth;
        this.Width = num1;
      }
      if (imAction.Type == KeyActionType.Pan && !this.mParentWindow.IsInOverlayMode)
        this.Height = 56.0;
      else
        this.Height = num1;
      if (flag && imAction.Type == KeyActionType.Dpad)
        this.MaxHeight = 48.0;
      if (imAction.PositionX == -1.0)
      {
        Point position;
        if (this.mMousePointForTap.HasValue)
        {
          position = this.mMousePointForTap.Value;
          if (imAction.Type == KeyActionType.Tap)
            this.UpdatePosition(position.Y, position.X);
        }
        else
          position = Mouse.GetPosition(this.Parent as IInputElement);
        if (!isLoaded && (imAction.Type == KeyActionType.Tilt || imAction.Type == KeyActionType.State || imAction.Type == KeyActionType.MouseZoom))
          return;
        if (isLoaded && this.mIsLoadingfromFile)
        {
          if (imAction.Type == KeyActionType.Tilt || (imAction.Type == KeyActionType.State || imAction.Type == KeyActionType.MouseZoom) && KMManager.sIsDeveloperModeOn)
          {
            Canvas.SetTop((UIElement) this, 0.0);
            Canvas.SetLeft((UIElement) this, 0.0);
          }
        }
        else
        {
          Canvas.SetTop((UIElement) this, position.Y - num1 / 2.0);
          Canvas.SetLeft((UIElement) this, position.X - num1 / 2.0);
        }
      }
      else
      {
        double num2 = xPos / 100.0 * this.mParentWindow.mCanvas.ActualWidth;
        double num3 = yPos / 100.0 * this.mParentWindow.mCanvas.ActualHeight;
        double num4 = num2 < 0.0 ? 0.0 : num2;
        double num5 = num3 < 0.0 ? 0.0 : num3;
        if (this.mParentWindow.IsInOverlayMode)
        {
          double num6 = imAction.Type != KeyActionType.Dpad || flag ? 30.0 : this.ActualWidth - this.ActualWidth * 0.4;
          double length1 = num4 <= this.mParentWindow.mCanvas.ActualWidth - num6 ? num4 - num1 / 2.0 : this.mParentWindow.mCanvas.ActualWidth - this.ActualWidth;
          double length2 = num5 <= this.mParentWindow.mCanvas.ActualHeight - num6 ? num5 - num1 / 2.0 : this.mParentWindow.mCanvas.ActualHeight - this.ActualHeight;
          Canvas.SetLeft((UIElement) this, length1);
          Canvas.SetTop((UIElement) this, length2);
        }
        else
        {
          double num6 = num4 > this.mParentWindow.mCanvas.ActualWidth ? this.mParentWindow.mCanvas.ActualWidth : num4;
          double num7 = num5 > this.mParentWindow.mCanvas.ActualHeight ? this.mParentWindow.mCanvas.ActualHeight : num5;
          double length1 = num6 - num1 / 2.0;
          double length2 = num7 - this.Height / 2.0;
          Canvas.SetLeft((UIElement) this, length1);
          Canvas.SetTop((UIElement) this, length2);
        }
      }
      if (this.ListActionItem.First<IMAction>().Type != KeyActionType.Swipe && this.ListActionItem.First<IMAction>().Type != KeyActionType.Tilt)
        return;
      this.mSettingsIcon.Margin = new Thickness(this.ActualWidth / 9.0, 0.0, 0.0, this.ActualWidth / 9.0);
      this.mCloseIcon.Margin = new Thickness(0.0, this.ActualWidth / 9.0, this.ActualWidth / 9.0, 0.0);
      this.mResizeIcon.Margin = new Thickness(0.0, 0.0, this.ActualWidth / 9.0, this.ActualWidth / 9.0);
    }

    private void MoveIcon_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!this.mResizeIcon.IsMouseOver)
        this.Cursor = Cursors.Hand;
      if (this.mType == KeyActionType.MOBASkill)
        this.mSkillImage.Visibility = Visibility.Visible;
      else
        this.mSettingsIcon.Visibility = Visibility.Visible;
      this.mCloseIcon.Visibility = Visibility.Visible;
    }

    private void MoveIcon_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mParentWindow.mCanvasElement == null)
        this.Cursor = Cursors.Arrow;
      if (this.mType == KeyActionType.Dpad || this.mType == KeyActionType.MOBADpad || (this.mType == KeyActionType.Rotate || this.mType == KeyActionType.Swipe) || (this.mType == KeyActionType.Zoom || this.mType == KeyActionType.Tilt || this.mType == KeyActionType.FreeLook && (this.ListActionItem.First<IMAction>() as FreeLook).DeviceType == 0))
      {
        this.mSettingsIcon.Visibility = Visibility.Visible;
        this.mCloseIcon.Visibility = Visibility.Visible;
      }
      else if (this.mType == KeyActionType.MOBASkill)
      {
        this.mActionIcon.Visibility = Visibility.Collapsed;
        this.mSkillImage.Visibility = Visibility.Collapsed;
        this.mCloseIcon.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.mSettingsIcon.Visibility = Visibility.Hidden;
        this.mCloseIcon.Visibility = Visibility.Hidden;
        this.mSkillImage.Visibility = Visibility.Hidden;
      }
    }

    private void ResizeIcon_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNWSE;
      e.Handled = true;
    }

    private void ResizeIcon_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mParentWindow.mCanvasElement != null)
        return;
      this.Cursor = Cursors.Arrow;
      e.Handled = true;
      if (!this.IsMouseOver)
        return;
      this.Cursor = Cursors.Hand;
    }

    private void DeleteIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
      KeymapCanvasWindow.sIsDirty = true;
      KMManager.CheckAndCreateNewScheme();
      this.DeleteElement();
    }

    private void DeleteElement()
    {
      switch (this.ListActionItem.First<IMAction>().Type)
      {
        case KeyActionType.MOBADpad:
          if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
          {
            this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("");
            this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
            if ((this.ListActionItem.First<IMAction>() as MOBADpad).mMOBAHeroDummy != null)
            {
              this.mParentWindow.dictCanvasElement[(IMAction) (this.ListActionItem.First<IMAction>() as MOBADpad).mMOBAHeroDummy].RemoveAction("");
              this.mParentWindow.dictCanvasElement.Remove((IMAction) (this.ListActionItem.First<IMAction>() as MOBADpad).mMOBAHeroDummy);
            }
          }
          this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(this.ListActionItem.First<IMAction>());
          break;
        case KeyActionType.LookAround:
          if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
          {
            this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyLookAround");
            this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
          }
          Pan parentAction1 = (this.ListActionItem.First<IMAction>() as LookAround).ParentAction as Pan;
          double num1;
          double num2 = num1 = -1.0;
          parentAction1.LookAroundY = num1;
          parentAction1.LookAroundX = num2;
          break;
        case KeyActionType.PanShoot:
          if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
          {
            this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyAction");
            this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
          }
          Pan parentAction2 = (this.ListActionItem.First<IMAction>() as PanShoot).ParentAction as Pan;
          double num3;
          double num4 = num3 = -1.0;
          parentAction2.LButtonY = num3;
          parentAction2.LButtonX = num4;
          break;
        case KeyActionType.MOBASkillCancel:
          if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
          {
            this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyCancel");
            this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
          }
          MOBASkill parentAction3 = (this.ListActionItem.First<IMAction>() as MOBASkillCancel).ParentAction as MOBASkill;
          double num5;
          double num6 = num5 = -1.0;
          parentAction3.CancelY = num5;
          parentAction3.CancelX = num6;
          break;
        case KeyActionType.MOBAHeroDummy:
          if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
          {
            this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyAction");
            this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
          }
          MOBADpad mobaDpad = this.ListActionItem.First<IMAction>() is MOBAHeroDummy mobaHeroDummy ? mobaHeroDummy.mMOBADpad : (MOBADpad) null;
          if (mobaDpad == null)
            break;
          mobaDpad.mMOBAHeroDummy = (MOBAHeroDummy) null;
          mobaDpad.OriginX = -1.0;
          mobaDpad.OriginY = -1.0;
          break;
        default:
          foreach (IMAction imAction in this.ListActionItem)
            this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(imAction);
          if (this.Parent == null)
            break;
          (this.Parent as Canvas).Children.Remove((UIElement) this);
          using (Dictionary<IMAction, CanvasElement>.Enumerator enumerator = this.mParentWindow.dictCanvasElement.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<IMAction, CanvasElement> current = enumerator.Current;
              if (current.Key.ParentAction == this.ListActionItem.First<IMAction>())
              {
                current.Value.RemoveAction("");
                this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(current.Value.ListActionItem.First<IMAction>());
              }
            }
            break;
          }
      }
    }

    private void UpArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      int num = Convert.ToInt32(this.mCountText.Text, (IFormatProvider) CultureInfo.InvariantCulture) + 1;
      this.mCountText.Text = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (this.ListActionItem.First<IMAction>().Type != KeyActionType.TapRepeat)
        return;
      ((TapRepeat) this.ListActionItem.First<IMAction>()).Count = num;
      ((TapRepeat) this.ListActionItem.First<IMAction>()).Delay = 1000 / (2 * num);
    }

    private void DownArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      int num = Convert.ToInt32(this.mCountText.Text, (IFormatProvider) CultureInfo.InvariantCulture) - 1;
      this.mCountText.Text = num > 1 ? num.ToString((IFormatProvider) CultureInfo.InvariantCulture) : "1";
      if (this.ListActionItem.First<IMAction>().Type != KeyActionType.TapRepeat)
        return;
      ((TapRepeat) this.ListActionItem.First<IMAction>()).Count = Convert.ToInt32(this.mCountText.Text, (IFormatProvider) CultureInfo.InvariantCulture);
      ((TapRepeat) this.ListActionItem.First<IMAction>()).Delay = 1000 / (2 * ((TapRepeat) this.ListActionItem.First<IMAction>()).Count);
    }

    private void mToggleImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (CanvasElement.sFocusedTextBox != null)
          WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
        if (string.Equals(this.mToggleImage.ImageName, "right_switch", StringComparison.InvariantCulture))
        {
          this.mToggleImage.ImageName = "left_switch";
          if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
          {
            ((TapRepeat) this.ListActionItem.First<IMAction>()).RepeatUntilKeyUp = false;
          }
          else
          {
            if (this.ListActionItem.First<IMAction>().Type != KeyActionType.FreeLook)
              return;
            ((FreeLook) this.ListActionItem.First<IMAction>()).DeviceType = 0;
            this.SetKeysForActions(this.ListActionItem);
            this.mSettingsIcon.Visibility = Visibility.Visible;
            this.mCloseIcon.Visibility = Visibility.Visible;
          }
        }
        else
        {
          this.mToggleImage.ImageName = "right_switch";
          if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
          {
            ((TapRepeat) this.ListActionItem.First<IMAction>()).RepeatUntilKeyUp = true;
          }
          else
          {
            if (this.ListActionItem.First<IMAction>().Type != KeyActionType.FreeLook)
              return;
            ((FreeLook) this.ListActionItem.First<IMAction>()).DeviceType = 1;
            this.SetKeysForActions(this.ListActionItem);
            this.mSettingsIcon.Visibility = Visibility.Collapsed;
            this.mCloseIcon.Visibility = Visibility.Collapsed;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in toggleMode: " + ex.ToString());
      }
    }

    private void MSkillImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.SetMOBASkillSettingsContent();
      this.SkillIconToolTipPopup.IsOpen = false;
      this.MOBASkillSettingsPopup.IsOpen = true;
      this.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      this.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
      ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, "moba_skill_settings_clicked", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void MSkillImage_MouseEnter(object sender, MouseEventArgs e)
    {
      this.SkillIconToolTipPopup.IsOpen = true;
      this.SkillIconToolTipPopup.StaysOpen = true;
    }

    private void MSkillImage_MouseLeave(object sender, MouseEventArgs e)
    {
      this.SkillIconToolTipPopup.IsOpen = false;
    }

    private void ScriptSettingsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mParentWindow.SidebarWindow.mLastScriptActionItem = this.ListActionItem;
      this.mParentWindow.SidebarWindow.ToggleAGCWindowVisiblity(true);
      ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_edit");
    }

    internal void SendMOBAStats(string action, string skillName = "")
    {
      try
      {
        string key = this.dictTextElemets[Positions.Center].Item1;
        string str = "";
        if (this.ListActionItem.First<IMAction>().Guidance.ContainsKey(key))
          str = this.ListActionItem.First<IMAction>().Guidance[key];
        ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, action, str, skillName, (string) null, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending MOBA stats: " + ex.ToString());
      }
    }

    private void SettingsIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.MOBASkillSettingsPopup.IsOpen || this.MOBAOtherSettingsMoreInfoPopup.IsOpen || this.MOBASkillSettingsMoreInfoPopup.IsOpen)
        return;
      this.OpenPopup();
    }

    private void XSensitivity_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      double result;
      if (double.TryParse(this.mXSensitivity.Text.Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) && this.CanIncreaseSenstivity(result, 0.05, this.mMaxSenstivity))
      {
        result += 0.05;
        this.mXSensitivity.Text = result.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
        KMManager.CheckAndCreateNewScheme();
      }
      Pan pan = (Pan) this.ListActionItem.First<IMAction>();
      double num1 = Convert.ToDouble((object) pan.SensitivityRatioY, (IFormatProvider) CultureInfo.InvariantCulture);
      double num2 = Convert.ToDouble((object) pan.Sensitivity, (IFormatProvider) CultureInfo.InvariantCulture);
      double num3 = Convert.ToDouble((object) result, (IFormatProvider) CultureInfo.InvariantCulture);
      double num4 = num2 == 0.0 ? num1 : num2 * num1;
      if (num1 != 0.0)
        pan.SensitivityRatioY = num3 == 0.0 || num4 == 0.0 ? num4 : num4 / num3;
      pan.Sensitivity = result;
    }

    private bool CanIncreaseSenstivity(double doubleVal, double diff, double maxSenstivity)
    {
      return doubleVal + diff <= maxSenstivity;
    }

    private bool CanDecreaseSenstivity(double doubleVal, double diff, double minSenstivity)
    {
      return minSenstivity <= doubleVal - diff;
    }

    private void XSensitivity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      double result;
      if (double.TryParse(this.mXSensitivity.Text.Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) && this.CanDecreaseSenstivity(result, 0.05, this.mMinSenstivity))
      {
        result -= 0.05;
        this.mXSensitivity.Text = result.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
        KMManager.CheckAndCreateNewScheme();
      }
      Pan pan = (Pan) this.ListActionItem.First<IMAction>();
      double num1 = Convert.ToDouble((object) pan.SensitivityRatioY, (IFormatProvider) CultureInfo.InvariantCulture);
      double num2 = Convert.ToDouble((object) pan.Sensitivity, (IFormatProvider) CultureInfo.InvariantCulture);
      double num3 = Convert.ToDouble((object) result, (IFormatProvider) CultureInfo.InvariantCulture);
      double num4 = num2 == 0.0 ? num1 : num2 * num1;
      if (num1 != 0.0)
        pan.SensitivityRatioY = num3 == 0.0 || num4 == 0.0 ? num4 : num4 / num3;
      pan.Sensitivity = result;
    }

    private void YSensitivity_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      double result;
      if (double.TryParse(this.mYSensitivity.Text.Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) && this.CanIncreaseSenstivity(result, 0.05, this.mMaxSenstivity))
      {
        result += 0.05;
        this.mYSensitivity.Text = result.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
        KMManager.CheckAndCreateNewScheme();
      }
      Pan pan = (Pan) this.ListActionItem.First<IMAction>();
      double num = result;
      double sensitivity = pan.Sensitivity;
      if (num != 0.0)
      {
        if (sensitivity != 0.0)
          pan.SensitivityRatioY = num / sensitivity;
        else
          pan.SensitivityRatioY = num;
      }
      else
        pan.SensitivityRatioY = 0.0;
    }

    private void YSensitivity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      double result;
      if (double.TryParse(this.mYSensitivity.Text.Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) && this.CanDecreaseSenstivity(result, 0.05, this.mMinSenstivity))
      {
        result -= 0.05;
        this.mYSensitivity.Text = result.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
        KMManager.CheckAndCreateNewScheme();
      }
      Pan pan = (Pan) this.ListActionItem.First<IMAction>();
      double num = result;
      double sensitivity = pan.Sensitivity;
      if (num != 0.0)
      {
        if (sensitivity != 0.0)
          pan.SensitivityRatioY = num / sensitivity;
        else
          pan.SensitivityRatioY = num;
      }
      else
        pan.SensitivityRatioY = 0.0;
    }

    internal void AssignMobaSkill(bool advancedMode, bool autoCastEnabled)
    {
      KMManager.CheckAndCreateNewScheme();
      if (this.ListActionItem.First<IMAction>().Type != KeyActionType.MOBASkill)
        return;
      ((MOBASkill) this.ListActionItem.First<IMAction>()).AdvancedMode = advancedMode;
      ((MOBASkill) this.ListActionItem.First<IMAction>()).AutocastEnabled = autoCastEnabled;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/canvaselement.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mCanvasElement = (CanvasElement) target;
          this.mCanvasElement.MouseEnter += new MouseEventHandler(this.MoveIcon_MouseEnter);
          this.mCanvasElement.MouseLeave += new MouseEventHandler(this.MoveIcon_MouseLeave);
          this.mCanvasElement.PreviewMouseRightButtonUp += new MouseButtonEventHandler(this.CanvasElement_PreviewMouseRightButtonUp);
          break;
        case 2:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Grid_Loaded);
          break;
        case 3:
          this.mToggleModeGrid = (Grid) target;
          break;
        case 4:
          this.mToggleMode1 = (TextBlock) target;
          break;
        case 5:
          this.mToggleImage = (CustomPictureBox) target;
          this.mToggleImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mToggleImage_PreviewMouseLeftButtonUp);
          break;
        case 6:
          this.mToggleMode2 = (TextBlock) target;
          break;
        case 7:
          this.mXSensitivityGrid = (Grid) target;
          break;
        case 8:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.XSensitivity_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mXSensitivity = (TextBlock) target;
          break;
        case 10:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.XSensitivity_PreviewMouseLeftButtonDown);
          break;
        case 11:
          this.mKeyRepeatGrid = (Grid) target;
          break;
        case 12:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.UpArrow_PreviewMouseDown);
          break;
        case 13:
          this.mCountText = (TextBlock) target;
          break;
        case 14:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.DownArrow_PreviewMouseDown);
          break;
        case 15:
          this.mCanvasGrid = (Grid) target;
          break;
        case 16:
          this.mActionIcon = (CustomPictureBox) target;
          break;
        case 17:
          this.mActionIcon2 = (CustomPictureBox) target;
          break;
        case 18:
          this.mCloseIcon = (CustomPictureBox) target;
          this.mCloseIcon.PreviewMouseDown += new MouseButtonEventHandler(this.DeleteIcon_PreviewMouseDown);
          break;
        case 19:
          this.mResizeIcon = (CustomPictureBox) target;
          this.mResizeIcon.MouseEnter += new MouseEventHandler(this.ResizeIcon_MouseEnter);
          this.mResizeIcon.MouseLeave += new MouseEventHandler(this.ResizeIcon_MouseLeave);
          break;
        case 20:
          this.mSkillImage = (CustomPictureBox) target;
          this.mSkillImage.MouseLeftButtonUp += new MouseButtonEventHandler(this.MSkillImage_MouseLeftButtonUp);
          this.mSkillImage.MouseEnter += new MouseEventHandler(this.MSkillImage_MouseEnter);
          this.mSkillImage.MouseLeave += new MouseEventHandler(this.MSkillImage_MouseLeave);
          break;
        case 21:
          this.mSettingsIcon = (CustomPictureBox) target;
          this.mSettingsIcon.MouseLeftButtonUp += new MouseButtonEventHandler(this.SettingsIcon_MouseLeftButtonUp);
          this.mSettingsIcon.MouseEnter += new MouseEventHandler(this.MSkillImage_MouseEnter);
          this.mSettingsIcon.MouseLeave += new MouseEventHandler(this.MSkillImage_MouseLeave);
          break;
        case 22:
          this.mGrid = (Grid) target;
          break;
        case 23:
          this.mColumn0 = (ColumnDefinition) target;
          break;
        case 24:
          this.mColumn1 = (ColumnDefinition) target;
          break;
        case 25:
          this.mColumn2 = (ColumnDefinition) target;
          break;
        case 26:
          this.mColumn3 = (ColumnDefinition) target;
          break;
        case 27:
          this.mColumn4 = (ColumnDefinition) target;
          break;
        case 28:
          this.mRow0 = (RowDefinition) target;
          break;
        case 29:
          this.mRow1 = (RowDefinition) target;
          break;
        case 30:
          this.mRow2 = (RowDefinition) target;
          break;
        case 31:
          this.mRow3 = (RowDefinition) target;
          break;
        case 32:
          this.mRow4 = (RowDefinition) target;
          break;
        case 33:
          this.mYSensitivityGrid = (Grid) target;
          break;
        case 34:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.YSensitivity_PreviewMouseLeftButtonUp);
          break;
        case 35:
          this.mYSensitivity = (TextBlock) target;
          break;
        case 36:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.YSensitivity_PreviewMouseLeftButtonDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
