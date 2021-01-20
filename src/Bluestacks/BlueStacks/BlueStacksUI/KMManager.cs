// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.KMManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace BlueStacks.BlueStacksUI
{
  internal static class KMManager
  {
    internal static KeymapCanvasWindow CanvasWindow = (KeymapCanvasWindow) null;
    internal static GuidanceWindow sGuidanceWindow = (GuidanceWindow) null;
    internal static string mComboEvents = string.Empty;
    internal static bool sIsCancelComboClicked = false;
    internal static bool sIsSaveComboClicked = false;
    internal static bool sIsComboRecordingOn = false;
    internal static DualTextBlockControl sGamepadDualTextbox = (DualTextBlockControl) null;
    internal static IMapTextBox CurrentIMapTextBox = (IMapTextBox) null;
    internal static string sShootingModeKey = "F1";
    public static Dictionary<string, bool> pressedGamepadKeyList = new Dictionary<string, bool>();
    public static string sGameControlsEnabledDisabledArray = string.Empty;
    public static string sOldGameControlsEnabledDisabledArray = string.Empty;
    public static List<List<CanvasElement>> listCanvasElement = new List<List<CanvasElement>>();
    internal static bool sIsInScriptEditingMode = false;
    internal static Dictionary<MainWindow, KeymapCanvasWindow> dictOverlayWindow = new Dictionary<MainWindow, KeymapCanvasWindow>();
    public static string ParserVersion = "17";
    public static string MinParserVersion = "14";
    internal static string sPackageName = string.Empty;
    internal static GuidanceVideoType sVideoMode = GuidanceVideoType.Default;
    internal static bool sIsDeveloperModeOn = false;
    internal static int mOnboardingCounter = 1;
    internal static bool mIsEnabledStateChanged = false;
    internal static List<OnBoardingPopupWindow> onBoardingPopupWindows = new List<OnBoardingPopupWindow>();
    internal static CanvasElement sDragCanvasElement;

    public static bool IsDragging
    {
      get
      {
        return KMManager.sDragCanvasElement != null;
      }
    }

    internal static void GetCurrentParserVersion(MainWindow window)
    {
      try
      {
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          try
          {
            JObject jobject = JObject.Parse(window.mFrontendHandler.SendFrontendRequest("getkeymappingparserversion", (Dictionary<string, string>) null));
            if (!jobject["success"].ToObject<bool>())
              return;
            KMManager.ParserVersion = jobject["parserversion"].ToString();
            KMManager.MinParserVersion = jobject["minparserversion"].ToString();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to get/parse result for getkeymappingparserversion");
            Logger.Error(ex.ToString());
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in KMManager init: " + ex.ToString());
      }
    }

    internal static void CheckAndCreateNewScheme()
    {
      if (!BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
        return;
      bool isBookMarked = BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
      IMControlScheme builtInScheme = BlueStacksUIUtils.LastActivatedWindow.OriginalLoadedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.BuiltIn && string.Equals(scheme.Name, BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))).FirstOrDefault<IMControlScheme>();
      if (builtInScheme == null)
        return;
      KMManager.AddNewControlSchemeAndSelectImap(BlueStacksUIUtils.LastActivatedWindow, builtInScheme);
      BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked = isBookMarked;
    }

    internal static void UpdateUIForGamepadEvent(string text, bool isDown)
    {
      if (text.Contains("GamepadStart", StringComparison.InvariantCultureIgnoreCase) || text.Contains("GamepadBack", StringComparison.InvariantCultureIgnoreCase))
        return;
      string str1 = string.Empty;
      string str2 = ".";
      if (text.Contains(str2))
      {
        str1 = text.Substring(text.IndexOf(str2, StringComparison.InvariantCultureIgnoreCase));
        text = text.Substring(0, text.IndexOf(str2, StringComparison.InvariantCultureIgnoreCase));
      }
      if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.IsVisible && KMManager.sGamepadDualTextbox != null)
      {
        if (string.Equals(KMManager.sGamepadDualTextbox.ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase))
          text = KMManager.CheckForAnalogEvent(text);
        if (string.IsNullOrEmpty(text))
          return;
        if (KMManager.sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.Tap || KMManager.sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.TapRepeat || KMManager.sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.Script)
        {
          KMManager.CheckItemToAddInList(text, isDown);
          if (KMManager.pressedGamepadKeyList.Count > 2)
          {
            KMManager.pressedGamepadKeyList.Clear();
            KMManager.sGamepadDualTextbox.mKeyTextBox.Text = string.Empty;
            KMManager.sGamepadDualTextbox.Setvalue(string.Empty);
            KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = (object) string.Empty;
          }
          else if (KMManager.pressedGamepadKeyList.Count == 2)
          {
            string str3 = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0)) + " + " + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(1));
            KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(1)), "");
            KMManager.sGamepadDualTextbox.Setvalue(str3 + str1);
            KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = (object) KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
            KMManager.pressedGamepadKeyList.Clear();
          }
          else
          {
            if (KMManager.pressedGamepadKeyList.Count != 1)
              return;
            string stringForUi = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0));
            KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUi, "");
            KMManager.sGamepadDualTextbox.Setvalue(stringForUi + str1);
            KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = (object) KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
          }
        }
        else
        {
          KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + text, "");
          KMManager.sGamepadDualTextbox.Setvalue(text + str1);
          KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = (object) KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
        }
      }
      else
      {
        if (KMManager.sGuidanceWindow == null || !KMManager.sGuidanceWindow.IsVisible || (KMManager.CurrentIMapTextBox == null || KMManager.CurrentIMapTextBox.IMActionItems == null) || !KMManager.CurrentIMapTextBox.IMActionItems.Any<IMActionItem>())
          return;
        KMManager.CheckAndCreateNewScheme();
        GuidanceWindow.sIsDirty = true;
        if (KMManager.CurrentIMapTextBox.IMActionItems != null && KMManager.CurrentIMapTextBox.IMActionItems.Any<IMActionItem>((Func<IMActionItem, bool>) (item => string.Equals(item.ActionItem, "GamepadStick", StringComparison.InvariantCultureIgnoreCase))))
          text = KMManager.CheckForAnalogEvent(text);
        if (string.IsNullOrEmpty(text))
          return;
        if (KMManager.CurrentIMapTextBox.ActionType == KeyActionType.Tap || KMManager.CurrentIMapTextBox.ActionType == KeyActionType.TapRepeat || KMManager.CurrentIMapTextBox.ActionType == KeyActionType.Script)
        {
          KMManager.CheckItemToAddInList(text, isDown);
          if (KMManager.pressedGamepadKeyList.Count > 2)
          {
            KMManager.pressedGamepadKeyList.Clear();
            KMManager.CurrentIMapTextBox.Tag = (object) string.Empty;
            KMManager.CurrentIMapTextBox.Text = string.Empty;
            foreach (IMActionItem imActionItem in (Collection<IMActionItem>) KMManager.CurrentIMapTextBox.IMActionItems)
              IMapTextBox.Setvalue(imActionItem, string.Empty);
          }
          else if (KMManager.pressedGamepadKeyList.Count == 2)
          {
            string str3 = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0)) + " + " + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(1));
            KMManager.CurrentIMapTextBox.Tag = (object) (str3 + str1);
            KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(1)), "");
            foreach (IMActionItem imActionItem in (Collection<IMActionItem>) KMManager.CurrentIMapTextBox.IMActionItems)
              IMapTextBox.Setvalue(imActionItem, KMManager.CurrentIMapTextBox.Tag.ToString());
            KMManager.pressedGamepadKeyList.Clear();
          }
          else
          {
            if (KMManager.pressedGamepadKeyList.Count != 1)
              return;
            string stringForUi = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt<string>(0));
            KMManager.CurrentIMapTextBox.Tag = (object) (stringForUi + str1);
            KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUi, "");
            foreach (IMActionItem imActionItem in (Collection<IMActionItem>) KMManager.CurrentIMapTextBox.IMActionItems)
              IMapTextBox.Setvalue(imActionItem, KMManager.CurrentIMapTextBox.Tag.ToString());
          }
        }
        else
        {
          KMManager.CurrentIMapTextBox.Tag = (object) (text + str1);
          KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text), "");
          foreach (IMActionItem imActionItem in (Collection<IMActionItem>) KMManager.CurrentIMapTextBox.IMActionItems)
            IMapTextBox.Setvalue(imActionItem, KMManager.CurrentIMapTextBox.Tag.ToString());
        }
      }
    }

    private static void CheckItemToAddInList(string text, bool isDown)
    {
      if (KMManager.pressedGamepadKeyList.ContainsKey(text) && KMManager.pressedGamepadKeyList[text] && !isDown)
        KMManager.pressedGamepadKeyList.Remove(text);
      if (!(!KMManager.pressedGamepadKeyList.ContainsKey(text) & isDown))
        return;
      KMManager.pressedGamepadKeyList.Add(text, isDown);
    }

    private static string CheckForAnalogEvent(string text)
    {
      string str = string.Empty;
      if (string.Equals(text, "GamepadLStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickDown", StringComparison.InvariantCultureIgnoreCase) || (string.Equals(text, "GamepadLStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickRight", StringComparison.InvariantCultureIgnoreCase)))
        str = "LeftStick";
      else if (string.Equals(text, "GamepadRStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickDown", StringComparison.InvariantCultureIgnoreCase) || (string.Equals(text, "GamepadRStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickRight", StringComparison.InvariantCultureIgnoreCase)))
        str = "RightStick";
      return str;
    }

    internal static bool KeyMappingFilesAvailable(string packageName)
    {
      return !string.IsNullOrEmpty(Utils.GetInputmapperFile(packageName));
    }

    internal static bool IsSelectedSchemeSmart(MainWindow mainWindow)
    {
      if (mainWindow == null)
        return false;
      int? count = mainWindow.SelectedConfig?.SelectedControlScheme?.Images?.Count;
      int num = 0;
      return count.GetValueOrDefault() > num & count.HasValue;
    }

    internal static bool IsShowShootingModeTooltip(MainWindow mainWindow)
    {
      bool flag = false;
      foreach (IMAction gameControl in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
      {
        if (gameControl is Pan pan)
        {
          if ((pan.Tweaks & 32) != 0)
            return false;
          flag = true;
        }
      }
      return flag;
    }

    internal static void HandleInputMapperWindow(MainWindow mainWindow, string isSelectedTab = "")
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft && mainWindow.mDimOverlay != null && (mainWindow.mDimOverlay.Control != null && (object) mainWindow.mDimOverlay.Control.GetType() == (object) mainWindow.ScreenLockInstance.GetType()) && mainWindow.ScreenLockInstance.IsVisible || mainWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab || KMManager.CanvasWindow?.SidebarWindow != null)
        return;
      if (mainWindow.SelectedConfig != null && mainWindow.SelectedConfig.SelectedControlScheme != null && mainWindow.SelectedConfig.ControlSchemes.Any<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !string.IsNullOrEmpty(scheme.Name))))
      {
        mainWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = true;
        mainWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide_active");
        KMManager.sGuidanceWindow = new GuidanceWindow(mainWindow);
        KMManager.sGuidanceWindow.GuidanceWindowTabSelected(isSelectedTab);
        KMManager.sGuidanceWindow.Show();
        mainWindow.Focus();
      }
      else
      {
        if (!string.Equals(isSelectedTab, "gamepad", StringComparison.InvariantCultureIgnoreCase))
          return;
        KMManager.ShowAdvancedSettings(mainWindow);
      }
    }

    internal static void ResizeMainWindow(MainWindow window)
    {
      Screen screen = Screen.FromHandle(new WindowInteropHelper((Window) window).Handle);
      double sScalingFactor = MainWindow.sScalingFactor;
      Rectangle rectangle;
      ref Rectangle local = ref rectangle;
      Rectangle workingArea = screen.WorkingArea;
      int x = (int) ((double) workingArea.X / sScalingFactor);
      workingArea = screen.WorkingArea;
      int y = (int) ((double) workingArea.Y / sScalingFactor);
      workingArea = screen.WorkingArea;
      int width = (int) ((double) workingArea.Width / sScalingFactor);
      workingArea = screen.WorkingArea;
      int height = (int) ((double) workingArea.Height / sScalingFactor);
      local = new Rectangle(x, y, width, height);
      if (window.Top + window.ActualHeight > (double) rectangle.Height)
        window.Top = ((double) rectangle.Height - window.ActualHeight) / 2.0;
      if (window.Left >= 0.0 && window.Left + window.ActualWidth <= (double) rectangle.Width)
        return;
      window.Left = ((double) rectangle.Width - window.ActualWidth) / 2.0;
    }

    internal static void ShowAdvancedSettings(MainWindow mainWindow)
    {
      if (mainWindow.WindowState != WindowState.Normal)
      {
        mainWindow.RestoreWindows(false);
        KeymapCanvasWindow.sWasMaximized = true;
      }
      KMManager.CloseWindows();
      if (KMManager.sGuidanceWindow != null)
        return;
      KeymapCanvasWindow keymapCanvasWindow = new KeymapCanvasWindow(mainWindow);
      keymapCanvasWindow.Owner = (Window) mainWindow;
      KMManager.CanvasWindow = keymapCanvasWindow;
      KMManager.CanvasWindow.InitLayout();
      KMManager.ShowOverlayWindow(mainWindow, false, false);
      KMManager.CanvasWindow.ShowDialog();
      if (!RegistryManager.Instance.ShowKeyControlsOverlay)
        return;
      KMManager.ShowOverlayWindow(mainWindow, true, false);
    }

    internal static void ShowDynamicOverlay(
      MainWindow mainWindow,
      bool isShow,
      bool isReload = false,
      string data = "")
    {
      DynamicOverlayConfigControls.Init(data);
      KMManager.ShowOverlayWindow(mainWindow, isShow, isReload);
    }

    internal static void HandleCallbackControl(MainWindow mainWindow, string data = "")
    {
      DynamicOverlayConfigControls.Init(data);
      if (!KMManager.sIsInScriptEditingMode && KMManager.CanvasWindow?.SidebarWindow != null || (KMManager.listCanvasElement == null || DynamicOverlayConfigControls.Instance.GameControls == null))
        return;
      if (KMManager.listCanvasElement.Count != DynamicOverlayConfigControls.Instance.GameControls.Count)
        return;
      try
      {
        for (int index = 0; index < DynamicOverlayConfigControls.Instance.GameControls.Count; ++index)
        {
          DynamicOverlayConfig gameControl = DynamicOverlayConfigControls.Instance.GameControls[index];
          CanvasElement canvasElement = KMManager.listCanvasElement[index][0];
          if (canvasElement != null && canvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
          {
            Logger.Info("Callback: IsEnabled1 : " + gameControl.Enabled);
            IMAction imAction = canvasElement.ListActionItem.First<IMAction>();
            KMManager.HandleCallbackPrimitive(mainWindow, gameControl, (imAction as Callback).Action, (imAction as Callback).Id);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("ERROR : GameControl not found in canvas elements. " + ex?.ToString());
      }
    }

    internal static void ShowOverlayWindow(MainWindow mainWindow, bool isShow, bool isreload = false)
    {
      if (mainWindow == null)
        return;
      if (isShow && mainWindow.IsVisible)
      {
        if (!KMManager.dictOverlayWindow.ContainsKey(mainWindow) && mainWindow != null && (mainWindow.mTopBar?.mAppTabButtons?.SelectedTab?.PackageName != null && mainWindow != null))
        {
          TabType? mTabType = mainWindow.mTopBar?.mAppTabButtons?.SelectedTab.mTabType;
          TabType tabType = TabType.AppTab;
          if (mTabType.GetValueOrDefault() == tabType & mTabType.HasValue)
          {
            if (FeatureManager.Instance.IsCustomUIForNCSoft && mainWindow.mDimOverlay != null && (mainWindow.mDimOverlay.Control != null && (object) mainWindow.mDimOverlay.Control.GetType() == (object) mainWindow.ScreenLockInstance.GetType()) && mainWindow.ScreenLockInstance.IsVisible)
              return;
            KeymapCanvasWindow keymapCanvasWindow = new KeymapCanvasWindow(mainWindow);
            KMManager.dictOverlayWindow[mainWindow] = keymapCanvasWindow;
            keymapCanvasWindow.IsInOverlayMode = true;
            keymapCanvasWindow.Owner = (Window) mainWindow;
            keymapCanvasWindow.InitLayout();
            if (mainWindow.mFrontendHandler.mFrontendHandle == IntPtr.Zero)
            {
              mainWindow.mFrontendHandler.ReparentingCompletedAction = new System.Action<MainWindow>(KMManager.ShowOverlayWindowAfterReparenting);
              goto label_13;
            }
            else
            {
              KMManager.ShowOverlayWindowAfterReparenting(mainWindow);
              goto label_13;
            }
          }
        }
        if (mainWindow != null && KMManager.dictOverlayWindow.ContainsKey(mainWindow))
        {
          if (isreload)
            KMManager.dictOverlayWindow[mainWindow].ReloadCanvasWindow();
          KMManager.dictOverlayWindow[mainWindow].UpdateSize();
        }
label_13:
        if (KMManager.dictOverlayWindow.ContainsKey(mainWindow))
          KMManager.dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(0.0, 0.0, false);
      }
      else if (KMManager.dictOverlayWindow.ContainsKey(mainWindow) && !KMManager.dictOverlayWindow[mainWindow].mIsClosing)
        KMManager.dictOverlayWindow[mainWindow].Close();
      KMManager.ToggleOverlayVisibility(mainWindow);
    }

    private static void ToggleOverlayVisibility(MainWindow mainWindow)
    {
      int count = KMManager.listCanvasElement.Count;
      string str1 = count.ToString();
      count = DynamicOverlayConfigControls.Instance.GameControls.Count;
      string str2 = count.ToString();
      Logger.Debug("OverlayCount : " + str1 + "    " + str2);
      if (KMManager.CanvasWindow?.SidebarWindow != null && !KMManager.sIsInScriptEditingMode)
        return;
      List<List<CanvasElement>> list = new List<List<CanvasElement>>();
      list.ClearAddRange<List<CanvasElement>>(KMManager.listCanvasElement.Where<List<CanvasElement>>((Func<List<CanvasElement>, bool>) (x => x.Count == 0 || x.First<CanvasElement>().ActionType != KeyActionType.MOBAHeroDummy)).ToList<List<CanvasElement>>());
      if (list == null || DynamicOverlayConfigControls.Instance.GameControls == null || list.Count != DynamicOverlayConfigControls.Instance.GameControls.Count && !KMManager.sIsInScriptEditingMode)
        return;
      try
      {
        for (int index = 0; index < DynamicOverlayConfigControls.Instance.GameControls.Count; ++index)
        {
          if (mainWindow.SelectedConfig.SelectedControlScheme.GameControls[index].IsVisibleInOverlay)
          {
            DynamicOverlayConfig gameControl = DynamicOverlayConfigControls.Instance.GameControls[index];
            if (!gameControl.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
              list[index].ForEach((System.Action<CanvasElement>) (element => element.Visibility = Visibility.Hidden));
            }
            else
            {
              foreach (CanvasElement canvasElement in list[index])
              {
                IMAction imAction1 = canvasElement.ListActionItem.First<IMAction>();
                switch (imAction1.Type)
                {
                  case KeyActionType.Pan:
                    if (((Pan) imAction1).IsLookAroundEnabled || ((Pan) imAction1).IsShootOnClickEnabled)
                    {
                      canvasElement.Visibility = Visibility.Hidden;
                      continue;
                    }
                    break;
                  case KeyActionType.Callback:
                    Logger.Info("Callback: IsEnabled2 : " + gameControl.Enabled);
                    KMManager.HandleCallbackPrimitive(mainWindow, gameControl, (imAction1 as Callback).Action, (imAction1 as Callback).Id);
                    continue;
                  case KeyActionType.LookAround:
                    Logger.Info("Position: " + gameControl.Type + " " + gameControl.LookAroundX.ToString() + " " + gameControl.LookAroundY.ToString());
                    if (gameControl.LookAroundShowOnOverlay && imAction1 is LookAround lookAround)
                    {
                      double num1 = string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                      double num2 = string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                      canvasElement.SetElementLayout(true, gameControl.LookAroundX + num1, gameControl.LookAroundY + num2);
                      canvasElement.mXPosition = gameControl.LookAroundX + num1;
                      canvasElement.mYPosition = gameControl.LookAroundY + num2;
                      canvasElement.Visibility = Visibility.Visible;
                      continue;
                    }
                    canvasElement.Visibility = Visibility.Hidden;
                    continue;
                  case KeyActionType.PanShoot:
                    Logger.Info("Position: " + gameControl.Type + " " + gameControl.LButtonX.ToString() + " " + gameControl.LButtonY.ToString());
                    if (gameControl.LButtonShowOnOverlay && imAction1 is PanShoot panShoot)
                    {
                      double num1 = string.IsNullOrEmpty(panShoot.LButtonXOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonXOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                      double num2 = string.IsNullOrEmpty(panShoot.LButtonYOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonYOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                      canvasElement.SetElementLayout(true, gameControl.LButtonX + num1, gameControl.LButtonY + num2);
                      canvasElement.mXPosition = gameControl.LButtonX + num1;
                      canvasElement.mYPosition = gameControl.LButtonY + num2;
                      canvasElement.Visibility = Visibility.Visible;
                      continue;
                    }
                    canvasElement.Visibility = Visibility.Hidden;
                    continue;
                  case KeyActionType.MOBASkillCancel:
                    Logger.Info("Position: " + gameControl.Type + " " + gameControl.CancelX.ToString() + " " + gameControl.CancelY.ToString());
                    if (gameControl.CancelShowOnOverlay && imAction1 is MOBASkillCancel mobaSkillCancel)
                    {
                      double num1 = string.IsNullOrEmpty(mobaSkillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mobaSkillCancel.MOBASkillCancelOffsetX, (IFormatProvider) CultureInfo.InvariantCulture);
                      double num2 = string.IsNullOrEmpty(mobaSkillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mobaSkillCancel.MOBASkillCancelOffsetX, (IFormatProvider) CultureInfo.InvariantCulture);
                      canvasElement.SetElementLayout(true, gameControl.CancelX + num1, gameControl.CancelY + num2);
                      canvasElement.mXPosition = gameControl.CancelX + num1;
                      canvasElement.mYPosition = gameControl.CancelY + num2;
                      canvasElement.Visibility = Visibility.Visible;
                      continue;
                    }
                    canvasElement.Visibility = Visibility.Hidden;
                    continue;
                }
                Logger.Info("Position: " + gameControl.Type + " " + gameControl.X.ToString() + " " + gameControl.Y.ToString());
                IMAction imAction2 = imAction1;
                double num3 = string.IsNullOrEmpty(imAction2.XOverlayOffset) ? 0.0 : Convert.ToDouble(imAction2.XOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                double num4 = string.IsNullOrEmpty(imAction2.YOverlayOffset) ? 0.0 : Convert.ToDouble(imAction2.YOverlayOffset, (IFormatProvider) CultureInfo.InvariantCulture);
                canvasElement.SetElementLayout(true, gameControl.X + num3, gameControl.Y + num4);
                canvasElement.mXPosition = gameControl.X + num3;
                canvasElement.mYPosition = gameControl.Y + num4;
                canvasElement.Visibility = Visibility.Visible;
              }
            }
          }
          else
          {
            DynamicOverlayConfig gameControl = DynamicOverlayConfigControls.Instance.GameControls[index];
            if (gameControl.Type.Equals("Callback", StringComparison.InvariantCultureIgnoreCase))
            {
              CanvasElement canvasElement = list[index][0];
              if (canvasElement != null && canvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
              {
                Logger.Info("Callback: IsEnabledValue : " + gameControl.Enabled);
                IMAction imAction = canvasElement.ListActionItem.First<IMAction>();
                canvasElement.mXPosition = gameControl.X;
                canvasElement.mYPosition = gameControl.Y;
                KMManager.HandleCallbackPrimitive(mainWindow, gameControl, (imAction as Callback).Action, (imAction as Callback).Id);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("ERROR : GameControl not found in canvas elements. " + ex?.ToString());
      }
    }

    private static void HandleCallbackPrimitive(
      MainWindow mainWindow,
      DynamicOverlayConfig item,
      string action,
      string id)
    {
      switch (action)
      {
        case "Api":
          mainWindow.mCallbackEnabled = item.Enabled.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          Logger.Info("Callback: HandleCallbackPrimitive(): " + mainWindow.mCallbackEnabled);
          break;
        case "Onboarding":
          if (item.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
          {
            if (!KMManager.mIsEnabledStateChanged && id.Equals("Step2", StringComparison.InvariantCultureIgnoreCase))
            {
              ++KMManager.mOnboardingCounter;
              KMManager.mIsEnabledStateChanged = true;
            }
            KMManager.dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(item.X, item.Y, true);
            break;
          }
          if (id.Equals("Step1", StringComparison.InvariantCultureIgnoreCase) || KMManager.mOnboardingCounter <= 1)
            break;
          KMManager.mIsEnabledStateChanged = false;
          KMManager.dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(item.X, item.Y, false);
          break;
      }
    }

    private static void ShowOverlayWindowAfterReparenting(MainWindow window)
    {
      window.Dispatcher.Invoke((Delegate) (() =>
      {
        if (window == null)
          return;
        window.mFrontendHandler.ShowGLWindow();
        if (!KMManager.dictOverlayWindow.ContainsKey(window))
          return;
        KeymapCanvasWindow keymapCanvasWindow = KMManager.dictOverlayWindow[window];
        if (keymapCanvasWindow == null || keymapCanvasWindow.mIsClosing)
          return;
        keymapCanvasWindow.mCanvas.Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
        if (!(window.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero))
          return;
        keymapCanvasWindow.Show();
        if (!KMManager.sIsInScriptEditingMode || KMManager.CanvasWindow.SidebarWindow == null)
          return;
        KMManager.CanvasWindow.SidebarWindow.Owner = (Window) keymapCanvasWindow;
        KMManager.CanvasWindow.SidebarWindow.Activate();
      }));
    }

    internal static void ChangeTransparency(MainWindow window, double value)
    {
      if (window != null && KMManager.dictOverlayWindow.ContainsKey(window))
        KMManager.dictOverlayWindow[window].mCanvas.Opacity = value;
      RegistryManager.Instance.TranslucentControlsTransparency = value;
    }

    internal static void CloseWindows()
    {
      if (KMManager.sGuidanceWindow != null)
      {
        if (!KMManager.sGuidanceWindow.IsClosed)
        {
          try
          {
            KMManager.sGuidanceWindow.Close();
          }
          catch (Exception ex)
          {
            Logger.Error("exception closing GameControlWindow " + ex.ToString());
          }
        }
      }
      if (KMManager.CanvasWindow == null)
        return;
      if (KMManager.CanvasWindow.mIsClosing)
        return;
      try
      {
        KMManager.CanvasWindow.SidebarWindow.Close();
      }
      catch (Exception ex)
      {
        Logger.Error("exception closing GameControlWindow " + ex.ToString());
      }
    }

    internal static void LoadIMActions(MainWindow mainWindow, string packageName)
    {
      Logger.Debug("Extralog: LoadImAction called. vmName:" + mainWindow.mVmName + "..." + Environment.StackTrace);
      KMManager.sPackageName = packageName;
      string inputmapperFile = Utils.GetInputmapperFile(KMManager.sPackageName);
      try
      {
        KMManager.ClearConfig(mainWindow);
        if (File.Exists(inputmapperFile))
        {
          mainWindow.SelectedConfig = KMManager.GetDeserializedIMConfigObject(inputmapperFile, true);
          if (mainWindow.SelectedConfig.ControlSchemes.Any<IMControlScheme>())
          {
            foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
            {
              if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(controlScheme.Name))
              {
                if (mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name].BuiltIn)
                  mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name] = controlScheme;
              }
              else
                mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name] = controlScheme;
            }
            IMControlScheme imControlScheme = mainWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.Selected)).FirstOrDefault<IMControlScheme>();
            mainWindow.SelectedConfig.SelectedControlScheme = imControlScheme ?? mainWindow.SelectedConfig.ControlSchemes[0];
          }
          mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
        }
        KMManager.CheckForShootingModeTooltip(mainWindow);
        if (!AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName].ContainsKey(packageName))
          AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName] = new AppSettings();
        if (AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded)
          return;
        ClientStats.SendMiscellaneousStatsAsync("DefaultScheme", RegistryManager.Instance.UserGuid, packageName, mainWindow.SelectedConfig?.SelectedControlScheme?.Name, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error parsing file " + inputmapperFile + ex.ToString());
      }
    }

    internal static bool CheckForGamepadKeymapping(MainWindow mainWindow)
    {
      JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
      serializerSettings.Formatting = Formatting.Indented;
      string str = JsonConvert.SerializeObject((object) mainWindow.SelectedConfig.SelectedControlScheme.GameControls, serializerSettings);
      foreach (string imapGamepadEvent in Constants.ImapGamepadEvents)
      {
        if (str.Contains(imapGamepadEvent))
        {
          mainWindow.mTopBar.mAppTabButtons.SelectedTab.mDictGamepadEligibility[KMManager.sPackageName] = true;
          return true;
        }
        mainWindow.mTopBar.mAppTabButtons.SelectedTab.mDictGamepadEligibility[KMManager.sPackageName] = false;
      }
      return false;
    }

    internal static void SendSchemeChangedStats(MainWindow window, string source = "")
    {
      ClientStats.SendMiscellaneousStatsAsync("SchemeChanged", RegistryManager.Instance.UserGuid, KMManager.sPackageName, window.SelectedConfig.SelectedControlScheme.Name, source, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    internal static MOBADpad GetMOBADPad(MainWindow mainWindow)
    {
      foreach (IMAction gameControl in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
      {
        if (gameControl.Type == KeyActionType.MOBADpad)
        {
          MOBADpad mobaDpad = gameControl as MOBADpad;
          if (mobaDpad.OriginX != -1.0 && mobaDpad.OriginY != -1.0)
            return mobaDpad;
        }
      }
      return (MOBADpad) null;
    }

    internal static IMConfig GetDeserializedIMConfigObject(
      string fileName,
      bool isFileNameUsed = true)
    {
      IMConfig config;
      if (!isFileNameUsed)
      {
        config = JsonConvert.DeserializeObject<IMConfig>(fileName, Utils.GetSerializerSettings());
      }
      else
      {
        bool flag = false;
        string str = "";
        using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
        {
          if (mutex.WaitOne())
          {
            try
            {
              str = File.ReadAllText(fileName);
              flag = true;
            }
            catch (Exception ex)
            {
              Logger.Error(string.Format("Failed to read cfg file... filepath: {0} Err : {1}", (object) fileName, (object) ex));
            }
            finally
            {
              mutex.ReleaseMutex();
            }
          }
        }
        if (!flag)
          throw new Exception("Could not read file " + fileName);
        config = JsonConvert.DeserializeObject<IMConfig>(str, Utils.GetSerializerSettings());
      }
      return KMManager.RemoveParentDpadsOfMobaDpads(config);
    }

    private static IMConfig RemoveParentDpadsOfMobaDpads(IMConfig config)
    {
      bool flag = false;
      foreach (IMControlScheme controlScheme in config.ControlSchemes)
      {
        for (int index = 0; index < controlScheme.GameControls.Count; ++index)
        {
          IMAction gameControl1 = controlScheme.GameControls[index];
          if (gameControl1.Type == KeyActionType.Dpad)
          {
            Dpad dpad = gameControl1 as Dpad;
            foreach (IMAction gameControl2 in controlScheme.GameControls)
            {
              if (gameControl2.Type == KeyActionType.MOBADpad && gameControl2.PositionX == gameControl1.PositionX && (gameControl2.PositionY == gameControl1.PositionY && string.IsNullOrEmpty(dpad.KeyUp)) && (string.IsNullOrEmpty(dpad.KeyDown) && string.IsNullOrEmpty(dpad.KeyLeft) && (string.IsNullOrEmpty(dpad.KeyRight) && string.IsNullOrEmpty(dpad.KeyUp_alt1))) && (string.IsNullOrEmpty(dpad.KeyDown_alt1) && string.IsNullOrEmpty(dpad.KeyLeft_alt1) && (string.IsNullOrEmpty(dpad.KeyRight_alt1) && string.IsNullOrEmpty(dpad.GamepadStick)) && string.IsNullOrEmpty(dpad.DpadTitle)))
              {
                gameControl2.IsVisibleInOverlay = gameControl1.IsVisibleInOverlay;
                controlScheme.GameControls.Remove(gameControl1);
                --index;
                flag = true;
                break;
              }
            }
          }
        }
      }
      if (flag)
        KMManager.SaveConfigToFile(Utils.GetInputmapperFile(KMManager.sPackageName), config);
      return config;
    }

    internal static IMControlScheme GetNewControlSchemes(string name)
    {
      return new IMControlScheme() { Name = name };
    }

    internal static ComboBoxSchemeControl GetComboBoxSchemeControlFromName(
      string schemeName)
    {
      foreach (ComboBoxSchemeControl child in KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
      {
        if (child.mSchemeName.Text == schemeName)
          return child;
      }
      return (ComboBoxSchemeControl) null;
    }

    internal static void AddNewControlSchemeAndSelect(
      MainWindow mainWindow,
      IMControlScheme toCopyFromScheme = null,
      bool isCopyOrNew = false)
    {
      bool flag1 = false;
      if (toCopyFromScheme != null && toCopyFromScheme.Selected && string.Equals(mainWindow.SelectedConfig?.SelectedControlScheme.Name, toCopyFromScheme.Name, StringComparison.InvariantCulture))
        flag1 = true;
      IMControlScheme imControlScheme;
      if (toCopyFromScheme != null)
      {
        imControlScheme = toCopyFromScheme.DeepCopy();
        if (flag1)
        {
          List<IMAction> gameControls = toCopyFromScheme.GameControls;
          toCopyFromScheme.SetGameControls(imControlScheme.GameControls);
          imControlScheme.SetGameControls(gameControls);
        }
      }
      else
        imControlScheme = new IMControlScheme();
      imControlScheme.Name = KMManager.GetNewSchemeName(mainWindow, toCopyFromScheme, isCopyOrNew);
      imControlScheme.Selected = true;
      imControlScheme.BuiltIn = false;
      mainWindow.SelectedConfig.ControlSchemes.Add(imControlScheme);
      bool flag2 = false;
      if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(imControlScheme.Name))
      {
        if (mainWindow.SelectedConfig.ControlSchemesDict[imControlScheme.Name].BuiltIn)
        {
          flag2 = mainWindow.SelectedConfig.ControlSchemesDict[imControlScheme.Name].IsBookMarked;
          mainWindow.SelectedConfig.ControlSchemesDict[imControlScheme.Name] = imControlScheme;
        }
      }
      else
        mainWindow.SelectedConfig.ControlSchemesDict.Add(imControlScheme.Name, imControlScheme);
      imControlScheme.IsBookMarked = flag2;
      if (isCopyOrNew && KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
      {
        KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = imControlScheme.Name;
        ComboBoxSchemeControl boxSchemeControl = new ComboBoxSchemeControl(KMManager.CanvasWindow, mainWindow);
        boxSchemeControl.mSchemeName.Text = LocaleStrings.GetLocalizedString(imControlScheme.Name, "");
        boxSchemeControl.IsEnabled = true;
        BlueStacksUIBinding.BindColor((DependencyObject) boxSchemeControl, System.Windows.Controls.Control.BackgroundProperty, "AdvancedGameControlButtonGridBackground");
        KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Add((UIElement) boxSchemeControl);
      }
      if (mainWindow.SelectedConfig.SelectedControlScheme != null)
      {
        if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
        {
          ComboBoxSchemeControl schemeControlFromName = KMManager.GetComboBoxSchemeControlFromName(mainWindow.SelectedConfig.SelectedControlScheme.Name);
          if (schemeControlFromName != null)
            BlueStacksUIBinding.BindColor((DependencyObject) schemeControlFromName, System.Windows.Controls.Control.BackgroundProperty, "ComboBoxBackgroundColor");
        }
        mainWindow.SelectedConfig.SelectedControlScheme.Selected = false;
      }
      mainWindow.SelectedConfig.SelectedControlScheme = imControlScheme;
      KeymapCanvasWindow.sIsDirty = true;
      if (!flag1 && KMManager.CanvasWindow != null)
        KMManager.CanvasWindow.Init();
      if (!isCopyOrNew || KMManager.CanvasWindow == null || KMManager.CanvasWindow.SidebarWindow == null)
        return;
      KMManager.CanvasWindow.SidebarWindow.FillProfileCombo();
    }

    private static void AddNewControlSchemeAndSelectImap(
      MainWindow mainWindow,
      IMControlScheme builtInScheme)
    {
      IMControlScheme selectedControlScheme = mainWindow.SelectedConfig?.SelectedControlScheme;
      int? nullable1 = mainWindow?.SelectedConfig?.ControlSchemes.IndexOf(selectedControlScheme);
      if (nullable1.HasValue)
      {
        int? nullable2 = nullable1;
        int num = -1;
        if (nullable2.GetValueOrDefault() > num & nullable2.HasValue)
        {
          mainWindow.SelectedConfig.ControlSchemes[nullable1.Value] = builtInScheme.DeepCopy();
          mainWindow.SelectedConfig.ControlSchemes[nullable1.Value].IsBookMarked = false;
        }
      }
      selectedControlScheme.BuiltIn = false;
      foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
        controlScheme.Selected = false;
      mainWindow.SelectedConfig.ControlSchemes.Add(selectedControlScheme);
      if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(selectedControlScheme.Name))
        mainWindow.SelectedConfig.ControlSchemesDict[selectedControlScheme.Name] = selectedControlScheme;
      mainWindow.SelectedConfig.SelectedControlScheme = selectedControlScheme;
      selectedControlScheme.Selected = true;
    }

    internal static string GetNewSchemeName(
      MainWindow mainWindow,
      IMControlScheme builtInScheme,
      bool isCopyOrNew)
    {
      string baseName;
      if (builtInScheme == null)
      {
        baseName = "Custom";
      }
      else
      {
        baseName = builtInScheme.Name;
        if (builtInScheme.BuiltIn && !isCopyOrNew)
          return baseName;
      }
      List<string> stringList = new List<string>();
      foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
        stringList.Add(controlScheme.Name);
      return KMManager.GetUniqueName(baseName, (IEnumerable<string>) stringList);
    }

    internal static string GetUniqueName(string baseName, IEnumerable<string> nameCollection)
    {
      int length = baseName.Length;
      int num1 = 0;
      bool flag = false;
      foreach (string name in nameCollection)
      {
        if (string.Compare(baseName, 0, name, 0, length, StringComparison.OrdinalIgnoreCase) == 0)
        {
          flag = true;
          if (name.Length > length + 3 && name[length] == ' ' && name[length + 1] == '(')
          {
            if (name[name.Length - 1] == ')')
            {
              int num2;
              try
              {
                num2 = int.Parse(name.Substring(length + 2, name.Length - length - 3), (IFormatProvider) CultureInfo.InvariantCulture);
              }
              catch (Exception ex)
              {
                continue;
              }
              if (num2 > num1)
                num1 = num2;
            }
          }
        }
      }
      if (!flag)
        return baseName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) baseName, (object) (num1 + 1));
    }

    internal static bool IsValidCfg(string fileName)
    {
      try
      {
        return JsonConvert.DeserializeObject(File.ReadAllText(fileName)) != null;
      }
      catch (Exception ex)
      {
        Logger.Error("invalid cfg file: {0}", (object) fileName);
        return false;
      }
    }

    private static void CheckForShootingModeTooltip(MainWindow window)
    {
      try
      {
        foreach (IMAction gameControl in window.SelectedConfig.SelectedControlScheme.GameControls)
        {
          if (gameControl.Type == KeyActionType.Pan)
            KMManager.sShootingModeKey = ((Pan) gameControl).KeyStartStop.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing shooting mode tooltip: " + ex.ToString());
      }
    }

    internal static void ClearConfig(MainWindow mainWindow)
    {
      MOBADpad.sListMOBADpad.Clear();
      Dpad.sListDpad.Clear();
      mainWindow.SelectedConfig = (IMConfig) null;
      mainWindow.OriginalLoadedConfig = (IMConfig) null;
    }

    internal static void GetCanvasElement(
      MainWindow mainWindow,
      IMAction action,
      Canvas canvas,
      bool addToCanvas = true)
    {
      KMManager.sDragCanvasElement = new CanvasElement(KMManager.CanvasWindow, mainWindow);
      KMManager.sDragCanvasElement.AddAction(action);
      KMManager.sDragCanvasElement.Opacity = 0.1;
      if (addToCanvas)
        canvas.Children.Add((UIElement) KMManager.sDragCanvasElement);
      if (action.Type != KeyActionType.Swipe)
        return;
      KMManager.AssignSwapValues(action);
      List<Direction> list = Enum.GetValues(typeof (Direction)).Cast<Direction>().ToList<Direction>();
      list.Remove(action.Direction);
      foreach (Direction direction in list)
      {
        IMAction action1 = action.DeepCopy<IMAction>();
        action1.Direction = direction;
        action1.RadiusProperty = action.RadiusProperty;
        KMManager.AssignSwapValues(action1);
        KMManager.sDragCanvasElement.AddAction(action1);
      }
      action.RadiusProperty = action.RadiusProperty;
    }

    private static void AssignSwapValues(IMAction action)
    {
      if (action.Direction == Direction.Up)
        (action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Up);
      else if (action.Direction == Direction.Down)
        (action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Down);
      else if (action.Direction == Direction.Left)
        (action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Left);
      else
        (action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Right);
    }

    internal static List<IMAction> ClearElement()
    {
      List<IMAction> imActionList = (List<IMAction>) null;
      if (KMManager.sDragCanvasElement != null)
      {
        imActionList = KMManager.sDragCanvasElement.ListActionItem;
        if (KMManager.sDragCanvasElement.Parent is System.Windows.Controls.Panel parent)
          parent.Children.Remove((UIElement) KMManager.sDragCanvasElement);
        KMManager.sDragCanvasElement = (CanvasElement) null;
      }
      return imActionList;
    }

    internal static void RepositionCanvasElement()
    {
      if (KMManager.sDragCanvasElement == null)
        return;
      System.Windows.Point position = Mouse.GetPosition(KMManager.sDragCanvasElement.Parent as IInputElement);
      Canvas.SetTop((UIElement) KMManager.sDragCanvasElement, position.Y - KMManager.sDragCanvasElement.ActualHeight / 2.0);
      Canvas.SetLeft((UIElement) KMManager.sDragCanvasElement, position.X - KMManager.sDragCanvasElement.ActualWidth / 2.0);
    }

    internal static void SaveIMActions(
      MainWindow mainWindow,
      bool isSavedFromGameControlWindow,
      bool isdDeleteIfEmpty = false)
    {
      Logger.Debug(string.Format("ExtraLog:Calling SaveIMActions, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) mainWindow.mVmName, (object) mainWindow.SelectedConfig.SelectedControlScheme.Name, (object) mainWindow.SelectedConfig.ControlSchemes.Count));
      if (!KeymapCanvasWindow.sIsDirty && !GuidanceWindow.sIsDirty && !isdDeleteIfEmpty)
      {
        Logger.Info("No changes were made in config file. Not saving");
      }
      else
      {
        KMManager.sPackageName = mainWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
        KeymapCanvasWindow.sIsDirty = false;
        GuidanceWindow.sIsDirty = false;
        KMManager.sGamepadDualTextbox = (DualTextBlockControl) null;
        string inputmapperUserFilePath = Utils.GetInputmapperUserFilePath(KMManager.sPackageName);
        KMManager.CheckForShootingModeTooltip(mainWindow);
        try
        {
          string directoryName = Path.GetDirectoryName(inputmapperUserFilePath);
          if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
          KMManager.CleanupGuidanceAccordingToSchemes(mainWindow.SelectedConfig.ControlSchemes, mainWindow.SelectedConfig.Strings);
          KMManager.SaveAndUpdateKeymapUI(mainWindow, isSavedFromGameControlWindow, inputmapperUserFilePath);
        }
        catch (Exception ex)
        {
          Logger.Error("Error saving file  for " + inputmapperUserFilePath + Environment.NewLine + ex.ToString());
        }
      }
    }

    private static void SaveAndUpdateKeymapUI(
      MainWindow mainWindow,
      bool isSavedFromGameControlWindow,
      string path)
    {
      Logger.Debug(string.Format("ExtraLog:Calling SaveAndUpdateKeymapUI, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) mainWindow.mVmName, (object) mainWindow.SelectedConfig.SelectedControlScheme.Name, (object) mainWindow.SelectedConfig.ControlSchemes.Count));
      try
      {
        mainWindow.SelectedConfig.MetaData.ParserVersion = KMManager.ParserVersion;
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Culture = CultureInfo.InvariantCulture;
        serializerSettings.Formatting = Formatting.Indented;
        serializerSettings.Converters.Add((JsonConverter) new JsonFormattingConverter());
        string contents = JsonConvert.SerializeObject((object) mainWindow.SelectedConfig, serializerSettings);
        bool callUpdateGrm = false;
        if (!File.Exists(path))
          callUpdateGrm = true;
        using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
        {
          if (mutex.WaitOne())
          {
            try
            {
              Logger.Debug(string.Format("ExtraLog:Calling WriteAllText, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) mainWindow.mVmName, (object) mainWindow.SelectedConfig.SelectedControlScheme.Name, (object) mainWindow.SelectedConfig.ControlSchemes.Count));
              File.WriteAllText(path, contents);
            }
            catch (Exception ex)
            {
              Logger.Error(string.Format("Failed to write cfg path: {0} Err : {1}", (object) path, (object) ex));
            }
            finally
            {
              mutex.ReleaseMutex();
            }
          }
        }
        Logger.Debug(string.Format("ExtraLog:Updating Original Config, VmName:{0}, Scheme:{1}, SchemeCount:{2}", (object) mainWindow.mVmName, (object) mainWindow.SelectedConfig.SelectedControlScheme.Name, (object) mainWindow.SelectedConfig.ControlSchemes.Count));
        mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
        bool isEnabled = false;
        isEnabled = mainWindow.OriginalLoadedConfig.ControlSchemes != null && mainWindow.OriginalLoadedConfig.ControlSchemes.Count > 0;
        mainWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
            KMManager.CanvasWindow.SidebarWindow.mExport.IsEnabled = isEnabled;
          mainWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(isEnabled);
          ClientStats.SendKeyMappingUIStatsAsync("cfg_saved", KMManager.sPackageName, isSavedFromGameControlWindow ? "edit_keys" : "advanced");
          if (callUpdateGrm)
            GrmHandler.RequirementConfigUpdated(mainWindow.mVmName);
          BlueStacksUIUtils.RefreshKeyMap(KMManager.sPackageName, (MainWindow) null);
          if (KMManager.CanvasWindow != null && !KMManager.CanvasWindow.IsInOverlayMode)
            KMManager.CanvasWindow.Init();
          KMManager.CheckForGamepadKeymapping(mainWindow);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SaveAndUpdateKeymapUI.." + ex.ToString());
      }
    }

    internal static void SaveConfigToFile(string path, IMConfig config)
    {
      config.MetaData.ParserVersion = KMManager.ParserVersion;
      JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
      serializerSettings.Formatting = Formatting.Indented;
      string contents = JsonConvert.SerializeObject((object) config, serializerSettings);
      using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
      {
        if (!mutex.WaitOne())
          return;
        try
        {
          File.WriteAllText(path, contents);
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format("Failed to write cfg path: {0} Err : {1}", (object) path, (object) ex));
        }
        finally
        {
          mutex.ReleaseMutex();
        }
      }
    }

    internal static bool CheckIfKeymappingWindowVisible(bool checkForGuidanceWindow = false)
    {
      bool isVisible = false;
      bool guidanceWindowVisible = false;
      try
      {
        BlueStacksUIUtils.LastActivatedWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          if (KMManager.sGuidanceWindow != null && (KMManager.sGuidanceWindow.IsActive || KMManager.sGuidanceWindow.IsVisible))
            guidanceWindowVisible = true;
          if (KMManager.CanvasWindow == null || !KMManager.CanvasWindow.IsActive)
            return;
          isVisible = true;
        }));
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in checkifkeymappingwindowvisible: " + ex.ToString());
      }
      return checkForGuidanceWindow ? guidanceWindowVisible : isVisible;
    }

    internal static void CallGamepadHandler(MainWindow mainWindow, string isEnable = "true")
    {
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "enable",
          isEnable
        }
      };
      mainWindow.mFrontendHandler.SendFrontendRequestAsync("toggleGamepadButton", data);
    }

    internal static string GetStringsToShowInUI(string text)
    {
      string[] strArray = text.ToString((IFormatProvider) CultureInfo.InvariantCulture).Split(new char[1]
      {
        '+'
      }, StringSplitOptions.RemoveEmptyEntries);
      string str = string.Empty;
      if (strArray.Length == 2)
      {
        string stringForUi1 = IMAPKeys.GetStringForUI(strArray[0].Trim());
        string stringForUi2 = IMAPKeys.GetStringForUI(strArray[1].Trim());
        str = Constants.ImapLocaleStringsConstant + stringForUi1 + " + " + stringForUi2;
      }
      else if (strArray.Length == 1)
      {
        string stringForUi = IMAPKeys.GetStringForUI(strArray[0].Trim());
        str = Constants.ImapLocaleStringsConstant + stringForUi;
      }
      return str;
    }

    internal static Dictionary<string, Dictionary<string, string>> CleanupGuidanceAccordingToSchemes(
      List<IMControlScheme> schemes,
      Dictionary<string, Dictionary<string, string>> locales)
    {
      HashSet<string> guidanceInUse = new HashSet<string>();
      foreach (IMControlScheme scheme in schemes)
      {
        foreach (IMAction gameControl in scheme.GameControls)
        {
          guidanceInUse.UnionWith((IEnumerable<string>) gameControl.Guidance.Values);
          guidanceInUse.Add(gameControl.GuidanceCategory);
        }
      }
      foreach (string key in locales.Keys)
      {
        foreach (KeyValuePair<string, string> keyValuePair in locales[key].Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kv => !guidanceInUse.Contains(kv.Key))).ToList<KeyValuePair<string, string>>())
          locales[key].Remove(keyValuePair.Key);
      }
      return locales;
    }

    public static string GetPackageFromCfgFile(string cfgFileName)
    {
      string str = string.Empty;
      if (!string.IsNullOrEmpty(cfgFileName))
        str = Path.GetFileNameWithoutExtension(cfgFileName);
      return str;
    }

    public static void MergeConfig(string pdPath)
    {
      Logger.Info("In MergeConfig");
      try
      {
        string str1 = Path.Combine(pdPath, "Engine\\UserData\\InputMapper\\UserFiles");
        foreach (string file in Directory.GetFiles(str1))
        {
          FileInfo fileInfo = new FileInfo(file);
          string path1 = Path.Combine(pdPath, "Engine\\UserData\\InputMapper");
          string str2 = Path.Combine(path1, fileInfo.Name);
          string str3 = Path.Combine(str1, fileInfo.Name);
          IMConfig deserializedImConfigObject = KMManager.GetDeserializedIMConfigObject(str3, true);
          if (deserializedImConfigObject.ControlSchemes.Count == 1)
            deserializedImConfigObject.ControlSchemes[0].Selected = true;
          if (!File.Exists(str2))
            KMManager.SaveConfigToFile(Path.Combine(path1, "UserFiles\\" + fileInfo.Name), deserializedImConfigObject);
          else
            KMManager.ControlSchemesHandling(KMManager.GetPackageFromCfgFile(fileInfo.Name), str3, str2);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in merging cfg. err: " + ex.ToString());
      }
    }

    internal static void MergeConflictingGuidanceStrings(
      IMConfig newConfig,
      List<IMControlScheme> toCopyFromSchemes,
      Dictionary<string, Dictionary<string, string>> stringsToImport)
    {
      HashSet<string> stringSet1 = new HashSet<string>();
      HashSet<string> stringSet2 = new HashSet<string>();
      foreach (string key1 in stringsToImport.Keys)
      {
        stringSet2.UnionWith((IEnumerable<string>) stringsToImport[key1].Keys);
        if (newConfig.Strings.Keys.Contains<string>(key1))
        {
          stringSet2.UnionWith((IEnumerable<string>) newConfig.Strings[key1].Keys);
          foreach (string key2 in stringsToImport[key1].Keys)
          {
            if (newConfig.Strings[key1].Keys.Contains<string>(key2) && stringsToImport[key1][key2] != newConfig.Strings[key1][key2])
              stringSet1.Add(key2);
          }
        }
      }
      foreach (string index in stringSet1)
      {
        string uniqueName = KMManager.GetUniqueName(index, (IEnumerable<string>) stringSet2);
        foreach (IMControlScheme toCopyFromScheme in toCopyFromSchemes)
        {
          foreach (IMAction gameControl in toCopyFromScheme.GameControls)
          {
            if (gameControl.GuidanceCategory == index)
              gameControl.GuidanceCategory = uniqueName;
            foreach (string key in gameControl.Guidance.Keys)
            {
              if (gameControl.Guidance[key] == index)
              {
                gameControl.Guidance[key] = uniqueName;
                break;
              }
            }
          }
        }
        foreach (Dictionary<string, string> dictionary in stringsToImport.Values)
        {
          if (dictionary.ContainsKey(index))
          {
            dictionary[uniqueName] = dictionary[index];
            dictionary.Remove(index);
          }
        }
      }
      foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair1 in stringsToImport)
      {
        if (newConfig.Strings.ContainsKey(keyValuePair1.Key))
        {
          foreach (KeyValuePair<string, string> keyValuePair2 in keyValuePair1.Value)
            newConfig.Strings[keyValuePair1.Key][keyValuePair2.Key] = keyValuePair2.Value;
        }
        else
          newConfig.Strings[keyValuePair1.Key] = keyValuePair1.Value;
      }
    }

    public static void ControlSchemesHandlingWhileCfgUpdateFromCloud(
      MainWindow window,
      string package)
    {
      bool isAutoUpdateRequired = false;
      if (window.mIsManualCheck)
        window.mKeymappingFilesDownloaded = true;
      string userFilesCfgPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), package) + ".cfg");
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Path.Combine(RegistryStrings.InputMapperFolder, package) + ".cfg");
      if (File.Exists(str))
      {
        string md5HashFromFile = Utils.GetMD5HashFromFile(str);
        string vmName = string.Empty;
        if (AppConfigurationManager.Instance.CheckIfTrueInAnyVm(package, (Predicate<AppSettings>) (appSettings => appSettings.CfgStored != null), out vmName))
          isAutoUpdateRequired = !string.Equals(AppConfigurationManager.Instance.VmAppConfig[vmName][package].CfgStored, md5HashFromFile, StringComparison.InvariantCultureIgnoreCase);
        if (!string.IsNullOrEmpty(vmName))
          AppConfigurationManager.Instance.VmAppConfig[vmName][package].CfgStored = md5HashFromFile;
        KMManager.ControlSchemesHandlingFromCloud(window, userFilesCfgPath, str, package, isAutoUpdateRequired);
      }
      else
        KMManager.CanvasWindow?.SidebarWindow?.ConfigNotAvailable();
    }

    private static void ControlSchemesHandling(
      string package,
      string userFilesCfgPath,
      string inputMapperCfgPath)
    {
      try
      {
        if (!File.Exists(userFilesCfgPath))
          return;
        IMConfig deserializedImConfigObject1 = KMManager.GetDeserializedIMConfigObject(inputMapperCfgPath, true);
        IMConfig deserializedImConfigObject2 = KMManager.GetDeserializedIMConfigObject(userFilesCfgPath, true);
        KMManager.MergeConflictingGuidanceStrings(deserializedImConfigObject1, deserializedImConfigObject2.ControlSchemes, deserializedImConfigObject2.Strings);
        deserializedImConfigObject2.Strings = deserializedImConfigObject1.Strings;
        List<IMControlScheme> source1 = new List<IMControlScheme>();
        foreach (IMControlScheme controlScheme in deserializedImConfigObject1.ControlSchemes)
        {
          if (controlScheme.BuiltIn)
            source1.Add(controlScheme);
        }
        IMControlScheme selectedScheme;
        bool flag1 = KMManager.IsBuiltInSchemeSelected(deserializedImConfigObject2, out selectedScheme);
        List<IMControlScheme> imControlSchemeList = new List<IMControlScheme>();
        foreach (IMControlScheme controlScheme in deserializedImConfigObject2.ControlSchemes)
        {
          if (controlScheme.BuiltIn)
            imControlSchemeList.Add(controlScheme);
        }
        foreach (IMControlScheme imControlScheme in imControlSchemeList)
          deserializedImConfigObject2.ControlSchemes.Remove(imControlScheme);
        List<string> list1 = source1.Select<IMControlScheme, string>((Func<IMControlScheme, string>) (scheme => scheme.Name)).ToList<string>();
        List<string> list2 = deserializedImConfigObject2.ControlSchemes.Select<IMControlScheme, string>((Func<IMControlScheme, string>) (scheme => scheme.Name)).ToList<string>();
        list2.AddRange((IEnumerable<string>) list1);
        string str = " (Custom)";
        foreach (IMControlScheme controlScheme in deserializedImConfigObject2.ControlSchemes)
        {
          if (list1.Contains(controlScheme.Name))
          {
            controlScheme.Name = KMManager.GetUniqueName(controlScheme.Name + str, (IEnumerable<string>) list2);
            list2.Add(controlScheme.Name);
          }
        }
        foreach (IMControlScheme imControlScheme in source1)
          deserializedImConfigObject2.ControlSchemes.Add(imControlScheme);
        if (!flag1)
        {
          foreach (IMControlScheme controlScheme in deserializedImConfigObject2.ControlSchemes)
          {
            if (controlScheme.BuiltIn)
            {
              if (!Opt.Instance.isUpgradeFromImap13 || !string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
                controlScheme.Selected = false;
            }
            else if (Opt.Instance.isUpgradeFromImap13 && string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
              controlScheme.Selected = false;
          }
        }
        else if (selectedScheme != null)
        {
          IMControlScheme imControlScheme = (IMControlScheme) null;
          bool flag2 = false;
          foreach (IMControlScheme controlScheme in deserializedImConfigObject2.ControlSchemes)
          {
            if (controlScheme.BuiltIn)
            {
              if (controlScheme.Name == selectedScheme.Name)
              {
                controlScheme.Selected = true;
                flag2 = true;
              }
              else if (controlScheme.Selected)
                imControlScheme = controlScheme;
            }
          }
          if (imControlScheme != null && flag2)
            imControlScheme.Selected = false;
        }
        if (deserializedImConfigObject2.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.Images.Count > 0)).FirstOrDefault<IMControlScheme>() != null)
        {
          Dictionary<string, IMControlScheme> source2 = new Dictionary<string, IMControlScheme>();
          foreach (IMControlScheme imControlScheme in imControlSchemeList)
          {
            if (imControlScheme.Images != null && imControlScheme.Images.Count > 0)
            {
              string key = JsonConvert.SerializeObject((object) imControlScheme.Images, Utils.GetSerializerSettings());
              source2.Add(key, imControlScheme);
            }
          }
          foreach (IMControlScheme controlScheme in deserializedImConfigObject2.ControlSchemes)
          {
            if (controlScheme.Images != null && !controlScheme.BuiltIn && controlScheme.Images.Count > 0)
            {
              string images = JsonConvert.SerializeObject((object) controlScheme.Images, Utils.GetSerializerSettings());
              IEnumerable<KeyValuePair<string, IMControlScheme>> prevSchemesMatchingImages = source2.Where<KeyValuePair<string, IMControlScheme>>((Func<KeyValuePair<string, IMControlScheme>, bool>) (kvp => string.Compare(kvp.Key, images, StringComparison.InvariantCultureIgnoreCase) == 0));
              if (prevSchemesMatchingImages.Any<KeyValuePair<string, IMControlScheme>>())
              {
                IEnumerable<IMControlScheme> source3 = source1.Where<IMControlScheme>((Func<IMControlScheme, bool>) (newScheme => string.Compare(newScheme.Name, prevSchemesMatchingImages.First<KeyValuePair<string, IMControlScheme>>().Value.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
                if (source3.Any<IMControlScheme>() && source3.First<IMControlScheme>().Images != null && source3.First<IMControlScheme>().Images.Any<JObject>())
                  controlScheme.SetImages(source3.First<IMControlScheme>().Images);
              }
            }
          }
        }
        KMManager.SaveConfigToFile(userFilesCfgPath, deserializedImConfigObject2);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in updating control schemes err: " + ex.ToString());
      }
    }

    internal static void SelectSchemeIfPresent(
      MainWindow window,
      string schemeNameToSelect,
      string statSource,
      bool forceSave)
    {
      IEnumerable<IMControlScheme> source = window.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, schemeNameToSelect, StringComparison.InvariantCulture)));
      bool flag = true;
      if (source.Any<IMControlScheme>())
      {
        IMControlScheme imControlScheme = source.Count<IMControlScheme>() != 1 ? source.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn)).FirstOrDefault<IMControlScheme>() : source.FirstOrDefault<IMControlScheme>();
        if (imControlScheme == null || imControlScheme.Name == window.SelectedConfig.SelectedControlScheme.Name && !forceSave)
        {
          flag = false;
        }
        else
        {
          window.SelectedConfig.SelectedControlScheme.Selected = false;
          imControlScheme.Selected = true;
          window.SelectedConfig.SelectedControlScheme = imControlScheme;
        }
      }
      if (!flag)
        return;
      KeymapCanvasWindow.sIsDirty = true;
      KMManager.SaveIMActions(window, false, false);
      if (KMManager.dictOverlayWindow.ContainsKey(window) && KMManager.dictOverlayWindow[window] != null && RegistryManager.Instance.ShowKeyControlsOverlay)
        KMManager.ShowOverlayWindow(window, true, true);
      BlueStacksUIUtils.RefreshKeyMap(KMManager.sPackageName, (MainWindow) null);
      KMManager.SendSchemeChangedStats(window, statSource);
    }

    private static bool IsBuiltInSchemeSelected(
      IMConfig prevConfig,
      out IMControlScheme selectedScheme)
    {
      selectedScheme = (IMControlScheme) null;
      foreach (IMControlScheme controlScheme in prevConfig.ControlSchemes)
      {
        if (controlScheme.Selected && controlScheme.BuiltIn)
        {
          selectedScheme = controlScheme;
          return true;
        }
      }
      return false;
    }

    private static void ControlSchemesHandlingFromCloud(
      MainWindow window,
      string userFilesCfgPath,
      string inputMapperCfgPath,
      string package,
      bool isAutoUpdateRequired = false)
    {
      try
      {
        if (File.Exists(userFilesCfgPath))
        {
          IMConfig deserializedImConfigObject1 = KMManager.GetDeserializedIMConfigObject(inputMapperCfgPath, true);
          IMConfig deserializedImConfigObject2 = KMManager.GetDeserializedIMConfigObject(userFilesCfgPath, true);
          KMManager.MergeConflictingGuidanceStrings(deserializedImConfigObject1, deserializedImConfigObject2.ControlSchemes, deserializedImConfigObject2.Strings);
          IEnumerable<IMControlScheme> source1 = deserializedImConfigObject2.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn));
          IEnumerable<IMControlScheme> source2 = deserializedImConfigObject2.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.BuiltIn));
          List<string> stringList = new List<string>();
          foreach (IMControlScheme imControlScheme in source2)
            stringList.Add(imControlScheme.Name);
          if (source1.Any<IMControlScheme>())
          {
            foreach (IMControlScheme imControlScheme in source1)
            {
              if (!window.mIsManualCheck || !stringList.Contains(imControlScheme.Name))
                deserializedImConfigObject1.ControlSchemes.Add(imControlScheme);
            }
          }
          string selectedSchemeName = string.Empty;
          IMControlScheme imControlScheme1 = deserializedImConfigObject2.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.Selected)).FirstOrDefault<IMControlScheme>();
          if (imControlScheme1 != null)
            selectedSchemeName = imControlScheme1.Name;
          if (imControlScheme1 != null)
          {
            List<IMControlScheme> list = deserializedImConfigObject1.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, selectedSchemeName, StringComparison.InvariantCultureIgnoreCase))).ToList<IMControlScheme>();
            if (list.Count == 1)
            {
              deserializedImConfigObject1.ControlSchemes.ForEach((System.Action<IMControlScheme>) (scheme => scheme.Selected = false));
              list[0].Selected = true;
            }
            else if (list.Count == 2)
            {
              IMControlScheme imControlScheme2 = list.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn)).FirstOrDefault<IMControlScheme>();
              if (imControlScheme2 != null)
              {
                deserializedImConfigObject1.ControlSchemes.ForEach((System.Action<IMControlScheme>) (scheme => scheme.Selected = false));
                imControlScheme2.Selected = true;
              }
            }
          }
          if (source2.Any<IMControlScheme>())
          {
            foreach (IMControlScheme imControlScheme2 in source2)
            {
              IMControlScheme userScheme = imControlScheme2;
              IMControlScheme imControlScheme3 = deserializedImConfigObject1.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.BuiltIn && string.Equals(scheme.Name, userScheme.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<IMControlScheme>();
              if (imControlScheme3 != null)
                imControlScheme3.IsBookMarked = userScheme.IsBookMarked;
            }
            if (deserializedImConfigObject2.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.Images.Count > 0)).FirstOrDefault<IMControlScheme>() != null)
            {
              Dictionary<string, IMControlScheme> dictionary = new Dictionary<string, IMControlScheme>();
              foreach (IMControlScheme imControlScheme2 in source2)
              {
                if (imControlScheme2.Images != null && imControlScheme2.Images.Count > 0)
                {
                  string key = JsonConvert.SerializeObject((object) imControlScheme2.Images, Utils.GetSerializerSettings());
                  dictionary.Add(key, imControlScheme2);
                }
              }
              foreach (IMControlScheme imControlScheme2 in deserializedImConfigObject1.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn)))
              {
                if (imControlScheme2.Images != null && imControlScheme2.Images.Count > 0)
                {
                  string key = JsonConvert.SerializeObject((object) imControlScheme2.Images, Utils.GetSerializerSettings());
                  if (dictionary.ContainsKey(key))
                  {
                    IMControlScheme userSchemeMatchingBuiltInImage = dictionary[key];
                    IMControlScheme imControlScheme3 = deserializedImConfigObject1.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (cloudScheme => cloudScheme.BuiltIn && string.Equals(cloudScheme.Name, userSchemeMatchingBuiltInImage.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<IMControlScheme>();
                    if (imControlScheme3 != null && imControlScheme3.Images != null && imControlScheme3.Images.Any<JObject>())
                      imControlScheme2.SetImages(imControlScheme3.Images);
                  }
                }
              }
            }
          }
          KMManager.SaveConfigToFile(userFilesCfgPath, deserializedImConfigObject1);
        }
        if (window.mIsManualCheck)
        {
          KMManager.CanvasWindow?.SidebarWindow?.ConfigUpdated();
          window.mIsManualCheck = false;
        }
        if (!isAutoUpdateRequired || !KMManager.sPackageName.Equals(package, StringComparison.InvariantCultureIgnoreCase) || !GuidanceCloudInfoManager.GetApplyCfgUpdateSettingForPackage(package))
          return;
        Logger.Info("AUTO_CFG_UPDATE Called.");
        KMManager.LoadIMActions(window, package);
        window.Dispatcher.Invoke((Delegate) (() =>
        {
          if (!KMManager.dictOverlayWindow.ContainsKey(window) || KMManager.dictOverlayWindow[window] == null)
            return;
          KMManager.dictOverlayWindow[window].Init();
          window.mCommonHandler.OnGameGuideButtonVisibilityChanged(true);
        }));
        BlueStacksUIUtils.RefreshKeyMap(package, window);
        KMManager.sGuidanceWindow?.RefreshGameGuide();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in updating control schemes err: " + ex.ToString());
      }
    }

    internal static string CheckForGamepadSuffix(string text)
    {
      if (text.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase) || text.Contains("LeftStick", StringComparison.InvariantCultureIgnoreCase) || text.Contains("RightStick", StringComparison.InvariantCultureIgnoreCase))
      {
        string str = ".";
        if (text.Contains(str))
          text = text.Substring(0, text.IndexOf(str, StringComparison.InvariantCultureIgnoreCase));
      }
      return text;
    }

    internal static string GetKeyUIValue(string text)
    {
      return string.Join(" + ", ((IEnumerable<string>) text.Split('+')).ToList<string>().Select<string, string>((Func<string, string>) (singleItem => LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.CheckForGamepadSuffix(singleItem.Trim())), ""))).ToArray<string>());
    }

    internal static void ShowShootingModeTooltip(MainWindow mainWindow, string package)
    {
      if (KMManager.CheckIfKeymappingWindowVisible(false) || !KMManager.KeyMappingFilesAvailable(package) || (mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed || KMManager.IsSelectedSchemeSmart(mainWindow)) || !KMManager.IsShowShootingModeTooltip(mainWindow))
        return;
      string[] strArray = LocaleStrings.GetLocalizedString("STRING_PRESS_TO_AIM_AND_SHOOT", "").Split('{', '}');
      mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = true;
      mainWindow.ToggleFullScreenToastVisibility(true, strArray[0], IMAPKeys.GetStringForUI(KMManager.sShootingModeKey), strArray[2]);
      mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed = true;
      mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = false;
    }

    internal static void AssignEdgeScrollMode(string keyValue, System.Windows.Controls.TextBox keyTextBox)
    {
      string str = Convert.ToBoolean(keyValue, (IFormatProvider) CultureInfo.InvariantCulture) ? "ON" : "OFF";
      BlueStacksUIBinding.Bind(keyTextBox, Constants.ImapLocaleStringsConstant + str);
    }
  }
}
