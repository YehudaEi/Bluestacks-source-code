// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AdvancedGameControlWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class AdvancedGameControlWindow : CustomWindow, IComponentConnector
  {
    internal Dictionary<string, string> mScriptModeDictionary = new Dictionary<string, string>();
    private readonly DispatcherTimer configUpdatedToastTimer = new DispatcherTimer();
    private readonly DispatcherTimer updateSchemeTimeoutTimer = new DispatcherTimer();
    private string appVersion = string.Empty;
    private MainWindow ParentWindow;
    internal KeymapCanvasWindow CanvasWindow;
    private CustomToastPopupControl mToastPopup;
    internal ExportSchemesWindow mExportSchemesWindow;
    internal ImportSchemesWindow mImportSchemesWindow;
    internal double mLastSliderValue;
    internal double mLastSavedSliderValue;
    internal List<IMAction> mLastScriptActionItem;
    private int updateSchemeTimerTicks;
    internal Border mAdvancedGameControlBorder;
    internal Grid PrimaryGrid;
    internal CustomPictureBox mCloseSideBarWindow;
    internal TextBlock mProfileHeader;
    internal CustomPictureBox mUpdate;
    internal CustomPictureBox mImport;
    internal CustomPictureBox mExport;
    internal CustomPictureBox mOpenFolder;
    internal SchemeComboBox mSchemeComboBox;
    internal CustomPictureBox mBrowserHelp;
    internal WrapPanel mPrimitivesPanel;
    internal AdvancedSettingsItemPanel mTapPrimitive;
    internal AdvancedSettingsItemPanel mTapRepeatPrimitive;
    internal AdvancedSettingsItemPanel mDpadPrimitive;
    internal AdvancedSettingsItemPanel mPanPrimitive;
    internal AdvancedSettingsItemPanel mZoomPrimitive;
    internal AdvancedSettingsItemPanel mMOBASkillPrimitive;
    internal AdvancedSettingsItemPanel mSwipePrimitive;
    internal AdvancedSettingsItemPanel mFreeLookPrimitive;
    internal AdvancedSettingsItemPanel mTiltPrimitive;
    internal AdvancedSettingsItemPanel mStatePrimitive;
    internal AdvancedSettingsItemPanel mScriptPrimitive;
    internal AdvancedSettingsItemPanel mMouseZoomPrimitive;
    internal AdvancedSettingsItemPanel mRotatePrimitive;
    internal AdvancedSettingsItemPanel mScrollPrimitive;
    internal AdvancedSettingsItemPanel mEdgeScrollPrimitive;
    internal AdvancedSettingsItemPanel mMobaDpadPrimitive;
    internal Grid mNCTransparencySlider;
    internal CustomTextBox mNCTransparencyLevel;
    internal CustomPictureBox mNCTranslucentControlsSliderButton;
    internal Slider mNCTransSlider;
    internal StackPanel mButtonsGrid;
    internal CustomButton mRevertBtn;
    internal CustomButton mUndoBtn;
    internal CustomButton mSaveBtn;
    internal Canvas mCanvas;
    internal Grid mOverlayGrid;
    internal Grid KeySequenceScriptGrid;
    internal Grid mScriptHeaderGrid;
    internal TextBlock mHeaderText;
    internal CustomPictureBox mCloseScriptWindow;
    internal TextBlock mSubheadingText;
    internal CustomTextBox mScriptText;
    internal TextBlock mXYCurrentCoordinatesText;
    internal TextBlock mShowHelpHyperlink;
    internal Grid mFooterGrid;
    internal TextBlock mFooterText;
    internal CustomButton mKeySeqDoneButton;
    private bool _contentLoaded;

    internal AdvancedGameControlWindow(MainWindow window)
    {
      this.ParentWindow = window;
      this.InitializeComponent();
      if (KMManager.sIsDeveloperModeOn)
      {
        this.mStatePrimitive.Visibility = Visibility.Visible;
        this.mMouseZoomPrimitive.Visibility = Visibility.Visible;
      }
      else
      {
        this.mStatePrimitive.Visibility = Visibility.Collapsed;
        this.mMouseZoomPrimitive.Visibility = Visibility.Collapsed;
      }
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        this.mBrowserHelp.Visibility = Visibility.Collapsed;
      this.Width = 0.0;
      this.Height = 0.0;
      BlueStacksUIBinding.Bind(this.mShowHelpHyperlink, "STRING_SCRIPT_GUIDE", "");
      this.mTapPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mTapRepeatPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mDpadPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mZoomPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mFreeLookPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mPanPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mMOBASkillPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mSwipePrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mTiltPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mStatePrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mScriptPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mMouseZoomPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mRotatePrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mScrollPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mEdgeScrollPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mMobaDpadPrimitive.MouseDragStart += new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart);
      this.mTapPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mTapRepeatPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mDpadPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mZoomPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mFreeLookPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mPanPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mMOBASkillPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mSwipePrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mTiltPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mStatePrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mScriptPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mMouseZoomPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mRotatePrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mScrollPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mEdgeScrollPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.mMobaDpadPrimitive.Tap += new EventHandler(this.AdvancedSettingsItemPanel_Tap);
      this.configUpdatedToastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
      this.configUpdatedToastTimer.Tick += new EventHandler(this.ConfigUpdatedToastTimer_Tick);
      this.updateSchemeTimeoutTimer.Interval = TimeSpan.FromMilliseconds(500.0);
      this.updateSchemeTimeoutTimer.Tick += new EventHandler(this.UpdateSchemeTimeoutTimer_Tick);
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      KMManager.sIsDeveloperModeOn = false;
      this.Close();
    }

    internal void ToggleAGCWindowVisiblity(bool isScriptModeWindow)
    {
      KMManager.sIsInScriptEditingMode = isScriptModeWindow;
      this.mScriptModeDictionary["isInScriptMode"] = isScriptModeWindow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.BindOrUnbindMouseEvents(isScriptModeWindow);
      if (isScriptModeWindow)
      {
        this.PrimaryGrid.Visibility = Visibility.Collapsed;
        this.KeySequenceScriptGrid.Visibility = Visibility.Visible;
        this.Owner = (Window) this.ParentWindow;
        this.PopulateScriptTextBox();
        this.CanvasWindow.Hide();
        this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(false);
      }
      else
      {
        this.PrimaryGrid.Visibility = Visibility.Visible;
        this.KeySequenceScriptGrid.Visibility = Visibility.Collapsed;
        this.CanvasWindow.Show();
        this.Owner = (Window) this.CanvasWindow;
        this.Activate();
        this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(true);
      }
      HTTPUtils.SendRequestToEngineAsync("scriptEditingModeEntered", this.mScriptModeDictionary, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void BindOrUnbindMouseEvents(bool bind)
    {
      this.MouseEnter -= new System.Windows.Input.MouseEventHandler(this.AdvancedGameControlWindow_MouseEnter);
      this.MouseLeave -= new System.Windows.Input.MouseEventHandler(this.AdvancedGameControlWindow_MouseLeave);
      if (!bind)
        return;
      this.MouseEnter += new System.Windows.Input.MouseEventHandler(this.AdvancedGameControlWindow_MouseEnter);
      this.MouseLeave += new System.Windows.Input.MouseEventHandler(this.AdvancedGameControlWindow_MouseLeave);
    }

    private void AdvancedGameControlWindow_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.ParentWindow.IsActive)
        return;
      this.ParentWindow.mFrontendHandler.ShowGLWindow();
    }

    private void AdvancedGameControlWindow_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
    }

    private void AdvancedGameControlWindow_Closing(object sender, CancelEventArgs evt)
    {
      if (KeymapCanvasWindow.sIsDirty)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), (EventHandler) ((o, e) => KMManager.SaveIMActions(this.ParentWindow, false, false)), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), (EventHandler) ((o, e) =>
        {
          KMManager.LoadIMActions(this.ParentWindow, KMManager.sPackageName);
          KeymapCanvasWindow.sIsDirty = false;
        }), (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((EventHandler) ((o, e) =>
        {
          this.CanvasWindow.mIsClosing = false;
          evt.Cancel = true;
        }), (object) null);
        customMessageWindow.Owner = (Window) this.CanvasWindow;
        customMessageWindow.ShowDialog();
      }
      if (this.updateSchemeTimeoutTimer.IsEnabled)
        this.updateSchemeTimeoutTimer.Stop();
      this.CanvasWindow.SidebarWindowLeft = this.Left;
      this.CanvasWindow.SidebarWindowTop = this.Top;
      this.ParentWindow.Activate();
      this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(true);
    }

    private void AdvancedGameControlWindow_Closed(object sender, EventArgs e)
    {
      this.CanvasWindow.SidebarWindow = (AdvancedGameControlWindow) null;
      if (KeymapCanvasWindow.sWasMaximized)
        this.ParentWindow.MaximizeWindow();
      else
        this.ParentWindow.ChangeHeightWidthTopLeft(this.CanvasWindow.mParentWindowWidth, this.CanvasWindow.mParentWindowHeight, this.CanvasWindow.mParentWindowTop, this.CanvasWindow.mParentWindowLeft);
      KeymapCanvasWindow.sWasMaximized = false;
      if (this.CanvasWindow.IsLoaded && !this.CanvasWindow.mIsClosing)
        this.CanvasWindow.Close();
      if (!RegistryManager.Instance.ShowKeyControlsOverlay)
        return;
      KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
    }

    internal void Init(KeymapCanvasWindow window)
    {
      this.CanvasWindow = window;
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.mNCTransSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
        this.mLastSavedSliderValue = this.mNCTransSlider.Value;
        this.mNCTransparencyLevel.Text = ((int) (this.mNCTransSlider.Value * 100.0)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
          this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
        this.ParentWindow.mCommonHandler.OverlayStateChangedEvent += new CommonHandlers.OverlayStateChanged(this.ParentWindow_OverlayStateChangedEvent);
      }
      else
        this.mNCTransparencySlider.Visibility = Visibility.Collapsed;
      this.FillProfileCombo();
      this.ProfileChanged();
      this.mSaveBtn.IsEnabled = false;
      this.mUndoBtn.IsEnabled = false;
      AppInfo infoFromPackageName = new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromPackageName(KMManager.sPackageName);
      if (infoFromPackageName == null || string.IsNullOrEmpty(infoFromPackageName.Version))
        return;
      this.appVersion = infoFromPackageName.Version;
    }

    internal void InsertXYInScript(double x, double y)
    {
      string str = " " + x.ToString("00.00", (IFormatProvider) CultureInfo.InvariantCulture) + " " + y.ToString("00.00", (IFormatProvider) CultureInfo.InvariantCulture);
      int num = this.mScriptText.SelectionStart + str.Length;
      this.mScriptText.Text = this.mScriptText.Text.Insert(this.mScriptText.SelectionStart, str);
      this.mScriptText.SelectionStart = num;
    }

    internal void OrderingControlSchemes()
    {
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      this.ParentWindow.SelectedConfig.ControlSchemes.Sort(new Comparison<IMControlScheme>(this.CompareSchemesAlphabetically));
      foreach (IMControlScheme imControlScheme in new List<IMControlScheme>((IEnumerable<IMControlScheme>) this.ParentWindow.SelectedConfig.ControlSchemes))
      {
        if (imControlScheme.BuiltIn)
        {
          if (imControlScheme.IsBookMarked)
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
            this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index3, imControlScheme);
            ++index3;
            ++index2;
            ++index1;
          }
          else
          {
            this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
            this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index2, imControlScheme);
            ++index2;
            ++index1;
          }
        }
        else if (imControlScheme.IsBookMarked)
        {
          this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imControlScheme);
          this.ParentWindow.SelectedConfig.ControlSchemes.Insert(index1, imControlScheme);
          ++index1;
        }
      }
    }

    private int CompareSchemesAlphabetically(IMControlScheme x, IMControlScheme y)
    {
      string strA = x.Name.ToLower(CultureInfo.InvariantCulture).Trim();
      string strB = y.Name.ToLower(CultureInfo.InvariantCulture).Trim();
      return strA.Contains(strB) || !strB.Contains(strA) && string.CompareOrdinal(strA, strB) >= 0 ? 1 : -1;
    }

    public void FillProfileCombo()
    {
      this.OrderingControlSchemes();
      ComboBoxSchemeControl boxSchemeControl1 = (ComboBoxSchemeControl) null;
      this.mSchemeComboBox.Items.Children.Clear();
      if (this.ParentWindow.SelectedConfig.ControlSchemes != null && this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
      {
        this.mProfileHeader.Visibility = Visibility.Visible;
        foreach (IMControlScheme imControlScheme in this.ParentWindow.SelectedConfig.ControlSchemesDict.Values)
        {
          IMControlScheme item = imControlScheme;
          ComboBoxSchemeControl boxSchemeControl2 = new ComboBoxSchemeControl(this.CanvasWindow, this.ParentWindow);
          boxSchemeControl2.mSchemeName.Text = item.Name;
          boxSchemeControl2.IsEnabled = true;
          if (item.Selected)
          {
            boxSchemeControl1 = boxSchemeControl2;
            BlueStacksUIBinding.BindColor((DependencyObject) boxSchemeControl2, System.Windows.Controls.Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
          }
          if (item.BuiltIn || this.ParentWindow.SelectedConfig.ControlSchemes.Count<IMControlScheme>((Func<IMControlScheme, bool>) (x => string.Equals(x.Name, item.Name, StringComparison.InvariantCulture))) == 2)
          {
            boxSchemeControl2.mEditImg.Visibility = Visibility.Hidden;
            boxSchemeControl2.mDeleteImg.Visibility = Visibility.Hidden;
          }
          if (item.IsBookMarked)
            boxSchemeControl2.mBookmarkImg.ImageName = "bookmarked";
          this.mSchemeComboBox.Items.Children.Add((UIElement) boxSchemeControl2);
        }
        if (boxSchemeControl1 == null)
        {
          this.ParentWindow.SelectedConfig.ControlSchemesDict[(this.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl).mSchemeName.Text].Selected = true;
        }
        else
        {
          this.mSchemeComboBox.SelectedItem = boxSchemeControl1.mSchemeName.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeComboBox.SelectedItem];
          this.mSchemeComboBox.mName.Text = this.mSchemeComboBox.SelectedItem;
        }
      }
      else
        BlueStacksUIBinding.Bind(this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
      if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes != null && this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
        this.mExport.IsEnabled = true;
      else
        this.mExport.IsEnabled = false;
      this.mRevertBtn.IsEnabled = this.ParentWindow.SelectedConfig.ControlSchemes.Count<IMControlScheme>((Func<IMControlScheme, bool>) (x => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))) == 2;
    }

    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
      this.CloseWindow();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      KMManager.SaveIMActions(this.ParentWindow, false, false);
      this.mLastSavedSliderValue = this.mNCTransSlider.Value;
      this.FillProfileCombo();
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
    }

    internal void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((Window) this);
        this.mToastPopup.Init((Window) this, message, (System.Windows.Media.Brush) null, (System.Windows.Media.Brush) null, System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    private void AdvancedSettingsItemPanel_Tap(object sender, EventArgs e)
    {
      this.AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel, true);
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void AdvancedSettingsItemPanel_MouseDragStart(object sender, EventArgs e)
    {
      this.AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel, false);
      this.Cursor = System.Windows.Input.Cursors.Arrow;
    }

    private void AddAdvancedControlToCanvas(AdvancedSettingsItemPanel sender, bool isTap = false)
    {
      if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
        KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, (IMControlScheme) null, false);
      KMManager.CheckAndCreateNewScheme();
      if (!isTap)
      {
        this.Focus();
        this.Cursor = System.Windows.Input.Cursors.Hand;
      }
      KMManager.GetCanvasElement(this.ParentWindow, Assembly.GetExecutingAssembly().CreateInstance(sender.ActionType.ToString()) as IMAction, this.mCanvas, false);
      if (isTap)
      {
        this.CanvasWindow.AddNewCanvasElement(KMManager.ClearElement(), true);
        KMManager.ClearElement();
      }
      sender.ReatchedMouseMove();
    }

    private void mCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      KMManager.RepositionCanvasElement();
    }

    private void mCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
      KMManager.ClearElement();
    }

    private void mButtonsGrid_Loaded(object sender, RoutedEventArgs e)
    {
      this.MaxWidth = this.mButtonsGrid.ActualWidth < 320.0 ? 320.0 : this.mButtonsGrid.ActualWidth;
      this.MinWidth = this.MaxWidth;
      this.Left = this.CanvasWindow.SidebarWindowLeft == -1.0 ? this.ParentWindow.Left + this.ParentWindow.ActualWidth - (this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? 60.0 : 0.0) : this.CanvasWindow.SidebarWindowLeft;
      this.Top = this.CanvasWindow.SidebarWindowTop == -1.0 ? this.ParentWindow.Top : this.CanvasWindow.SidebarWindowTop;
      this.Height = this.ParentWindow.ActualHeight;
      Screen screen = Screen.FromHandle(new WindowInteropHelper((Window) this).Handle);
      double sScalingFactor = MainWindow.sScalingFactor;
      Rectangle rectangle;
      ref Rectangle local = ref rectangle;
      int x = (int) ((double) screen.WorkingArea.X / sScalingFactor);
      Rectangle workingArea = screen.WorkingArea;
      int y = (int) ((double) workingArea.Y / sScalingFactor);
      workingArea = screen.WorkingArea;
      int width = (int) ((double) workingArea.Width / sScalingFactor);
      workingArea = screen.WorkingArea;
      int height = (int) ((double) workingArea.Height / sScalingFactor);
      local = new Rectangle(x, y, width, height);
      Rectangle rect = new Rectangle(new System.Drawing.Point((int) this.Left, (int) this.Top), new System.Drawing.Size((int) this.ActualWidth, (int) this.ActualHeight));
      if (rectangle.Contains(rect))
        return;
      this.Left = (double) rectangle.Width - this.Width;
    }

    public void ProfileChanged()
    {
      if (this.mSchemeComboBox.SelectedItem == null)
        return;
      string selectedItem = this.mSchemeComboBox.SelectedItem;
      if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(selectedItem))
      {
        if (!this.ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem].Selected)
        {
          this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
          foreach (ComboBoxSchemeControl child in this.mSchemeComboBox.Items.Children)
          {
            if (child.mSchemeName.Text == this.ParentWindow.SelectedConfig.SelectedControlScheme.Name)
            {
              BlueStacksUIBinding.BindColor((DependencyObject) child, System.Windows.Controls.Control.BackgroundProperty, "ComboBoxBackgroundColor");
              break;
            }
          }
          this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem];
          this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
          KeymapCanvasWindow.sIsDirty = true;
        }
        this.CanvasWindow.Init();
      }
      this.mRevertBtn.IsEnabled = this.ParentWindow.SelectedConfig.ControlSchemes.Count<IMControlScheme>((Func<IMControlScheme, bool>) (x => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))) == 2;
    }

    private void AdvancedGameControlWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Activate();
    }

    private void AdvancedGameControlWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      string b = string.Empty;
      if (e.Key != Key.None)
      {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
          b = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
          b = b + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
          b = b + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
        b += IMAPKeys.GetStringForFile(e.Key);
      }
      Logger.Debug("SHORTCUT: KeyPressed.." + b);
      if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance == null)
        return;
      foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
      {
        if (string.Equals(shortcutKeys.ShortcutKey, b, StringComparison.InvariantCulture) && string.Equals(shortcutKeys.ShortcutName, "STRING_CONTROLS_EDITOR", StringComparison.InvariantCulture))
          KMManager.CloseWindows();
      }
    }

    private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        using (Process process = new Process())
        {
          process.StartInfo.UseShellExecute = true;
          process.StartInfo.FileName = Directory.Exists(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles")) ? Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles") : RegistryStrings.InputMapperFolder;
          process.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error in Open folder err: " + ex.ToString());
      }
    }

    private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("ExportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail, (string) null, (string) null, "Android");
      if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
      {
        this.mOverlayGrid.Visibility = Visibility.Visible;
        if (this.mExportSchemesWindow != null)
          return;
        ExportSchemesWindow exportSchemesWindow = new ExportSchemesWindow(this.CanvasWindow, this.ParentWindow);
        exportSchemesWindow.Owner = (Window) this;
        this.mExportSchemesWindow = exportSchemesWindow;
        this.mExportSchemesWindow.Init();
        this.mExportSchemesWindow.Show();
      }
      else
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_AVAILABLE", ""), 1.3, false);
    }

    private void ImportBtn_Click(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("ImportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail, (string) null, (string) null, "Android");
      this.mOverlayGrid.Visibility = Visibility.Visible;
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.Multiselect = true;
      openFileDialog1.Filter = "Cfg files (*.cfg)|*.cfg";
      using (OpenFileDialog openFileDialog2 = openFileDialog1)
      {
        if (openFileDialog2.ShowDialog() == DialogResult.OK)
        {
          ImportSchemesWindow importSchemesWindow = new ImportSchemesWindow(this.CanvasWindow, this.ParentWindow);
          importSchemesWindow.Owner = (Window) this;
          this.mImportSchemesWindow = importSchemesWindow;
          this.mImportSchemesWindow.Init(openFileDialog2.FileName);
          this.mImportSchemesWindow.Show();
        }
        else
        {
          this.mOverlayGrid.Visibility = Visibility.Hidden;
          this.mImportSchemesWindow = (ImportSchemesWindow) null;
          this.Focus();
        }
      }
    }

    private void mCloseScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ToggleAGCWindowVisiblity(false);
      this.PopulateScriptTextBox();
      ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_close_click");
    }

    private void PopulateScriptTextBox()
    {
      if (this.mLastScriptActionItem == null)
        return;
      IMAction imAction = this.mLastScriptActionItem.First<IMAction>();
      if (imAction.Type != KeyActionType.Script)
        return;
      this.mScriptText.Text = string.Join(Environment.NewLine, (imAction as Script).Commands.ToArray());
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void ShowHelpHyperlink_Click(object sender, RoutedEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/help_articles", (string) null, (string) null, (string) null) + "&article=keymapping_script_faq");
    }

    private void mDoneScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mLastScriptActionItem != null)
      {
        IMAction imAction = this.mLastScriptActionItem.First<IMAction>();
        if (imAction.Type == KeyActionType.Script)
        {
          string[] scriptCmds = this.mScriptText.Text.Split(new string[1]
          {
            Environment.NewLine
          }, StringSplitOptions.None);
          if (this.CheckIfScriptValid(scriptCmds))
          {
            (imAction as Script).Commands.ClearAddRange<string>(((IEnumerable<string>) scriptCmds).ToList<string>());
          }
          else
          {
            this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_INVALID_SCRIPT_COMMANDS", ""));
            return;
          }
        }
      }
      this.ToggleAGCWindowVisiblity(false);
      ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_done_click");
    }

    private void NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      if (this.mNCTransSlider.Value == 0.0)
      {
        this.mNCTransSlider.Value = this.mLastSliderValue;
        this.mNCTransparencyLevel.Text = ((int) (this.mNCTransSlider.Value * 100.0)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (this.mLastSliderValue > 0.0)
          this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
        RegistryManager.Instance.ShowKeyControlsOverlay = true;
      }
      else
      {
        this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
        double num = this.mNCTransSlider.Value;
        this.mNCTransSlider.Value = 0.0;
        this.mNCTransparencyLevel.Text = ((int) (this.mNCTransSlider.Value * 100.0)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.mLastSliderValue = num;
        RegistryManager.Instance.ShowKeyControlsOverlay = false;
      }
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void NCTransparencySlider_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      KMManager.ChangeTransparency(this.ParentWindow, this.mNCTransSlider.Value);
      if (this.mNCTransSlider.Value == 0.0)
        this.ParentWindow_OverlayStateChangedEvent(false);
      else
        this.ParentWindow_OverlayStateChangedEvent(true);
      this.mLastSliderValue = this.mNCTransSlider.Value;
      this.mNCTransparencyLevel.Text = ((int) (this.mNCTransSlider.Value * 100.0)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (this.mNCTransSlider.Value == RegistryManager.Instance.TranslucentControlsTransparency)
        return;
      KeymapCanvasWindow.sIsDirty = true;
    }

    public void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
    {
      if (isEnabled)
      {
        this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
        if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0 && this.mLastSliderValue == 0.0)
        {
          RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
          this.mNCTransSlider.Value = 0.5;
          this.mNCTransparencyLevel.Text = ((int) (this.mNCTransSlider.Value * 100.0)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
        else
          RegistryManager.Instance.TranslucentControlsTransparency = this.mNCTransSlider.Value != 0.0 ? this.mNCTransSlider.Value : this.mLastSliderValue;
        RegistryManager.Instance.ShowKeyControlsOverlay = true;
      }
      else
      {
        this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
        RegistryManager.Instance.TranslucentControlsTransparency = 0.0;
        double num = this.mNCTransSlider.Value;
        this.mNCTransSlider.Value = 0.0;
        this.mNCTransparencyLevel.Text = "0";
        this.mLastSliderValue = num;
        RegistryManager.Instance.ShowKeyControlsOverlay = false;
      }
    }

    private void BrowserHelp_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Hand;
    }

    private void BrowserHelp_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
    }

    private void Export_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.mExport.IsEnabled)
        this.mExport.Opacity = 1.0;
      else
        this.mExport.Opacity = 0.4;
    }

    private void KeySequenceScriptGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.KeySequenceScriptGrid.Visibility != Visibility.Visible || this.mScriptText.IsMouseOver)
        return;
      this.mAdvancedGameControlBorder.Focus();
    }

    private void BrowserHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=advanced_game_control");
    }

    private void RevertBtn_Click(object sender, RoutedEventArgs e1)
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESET", (EventHandler) ((o, e2) =>
      {
        string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
        bool isBookMarked = this.ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
        this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
        IMControlScheme imControlScheme = this.ParentWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture))).FirstOrDefault<IMControlScheme>();
        if (imControlScheme == null)
          return;
        imControlScheme.Selected = true;
        this.ParentWindow.SelectedConfig.SelectedControlScheme = imControlScheme;
        this.ParentWindow.SelectedConfig.ControlSchemesDict[imControlScheme.Name] = imControlScheme;
        imControlScheme.IsBookMarked = isBookMarked;
        this.FillProfileCombo();
        this.ProfileChanged();
        this.mSaveBtn.IsEnabled = false;
        this.mUndoBtn.IsEnabled = false;
        KeymapCanvasWindow.sIsDirty = true;
        KMManager.SaveIMActions(this.ParentWindow, false, false);
        ClientStats.SendKeyMappingUIStatsAsync("advancedcontrols_reset", KMManager.sPackageName, "");
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) ((o, e2) => {}), (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
    }

    private bool CheckIfScriptValid(string[] scriptCmds)
    {
      bool flag = false;
      try
      {
        flag = JObject.Parse((JToken.Parse(HTTPUtils.SendRequestToEngine("validateScriptCommands", new Dictionary<string, string>()
        {
          {
            "script",
            new JObject()
            {
              {
                "Commands",
                (JToken) JArray.FromObject((object) ((IEnumerable<string>) scriptCmds).ToList<string>())
              }
            }.ToString(Formatting.None)
          }
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp")) as JArray)[0].ToString())["success"].ToObject<bool>();
      }
      catch
      {
      }
      return flag;
    }

    private void UpdateBtn_Click(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("update_controls", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_UPDATE_CONTROL_SCHEMES", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_UPDATE_CONTROL_SCHEME_WARNING", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_UPDATE_SETTING", (EventHandler) ((o, evt) =>
      {
        ClientStats.SendMiscellaneousStatsAsync("update_controls_confirm", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        this.UpdateConfig();
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) ((o, evt) => ClientStats.SendMiscellaneousStatsAsync("update_controls_cancel", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android")), (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) => ClientStats.SendMiscellaneousStatsAsync("update_controls_close", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android")), (object) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
    }

    private void UpdateConfig()
    {
      this.ParentWindow.mIsManualCheck = true;
      this.mUpdate.Visibility = Visibility.Hidden;
      this.mUpdate.IsImageToBeRotated = true;
      this.mUpdate.ImageName = "updating";
      this.mUpdate.Visibility = Visibility.Visible;
      this.mUpdate.IsEnabled = false;
      this.updateSchemeTimerTicks = 0;
      this.ParentWindow.mKeymappingFilesDownloaded = false;
      this.updateSchemeTimeoutTimer.Start();
      Utils.SendKeymappingFiledownloadRequest(KMManager.sPackageName, this.ParentWindow.mVmName);
    }

    internal void ConfigUpdated()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        BlueStacksUIUtils.RefreshKeyMap(KMManager.sPackageName, (MainWindow) null);
        this.FillProfileCombo();
        this.ProfileChanged();
        this.ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(true);
      }));
      this.ShowPopupAndResetIcon(LocaleStrings.GetLocalizedString("STRING_CFG_UPDATED", ""), true);
      ClientStats.SendMiscellaneousStatsAsync("update_controls_success", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void ConfigUpdatedToastTimer_Tick(object sender, EventArgs e)
    {
      this.configUpdatedToastTimer.Stop();
      this.ParentWindow.mConfigUpdatedPopup.IsOpen = false;
    }

    private void ShowPopupAndResetIcon(string message, bool state)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          this.mUpdate.ImageName = "refresh_from_cloud";
          this.mUpdate.IsImageToBeRotated = false;
          this.mUpdate.IsEnabled = true;
          this.ParentWindow.mConfigUpdatedControl.Init((Window) this, message, (System.Windows.Media.Brush) null, (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, (byte) 168, (byte) 168, (byte) 168)), System.Windows.HorizontalAlignment.Right, VerticalAlignment.Top, new Thickness?(), 1, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
          if (state)
            this.ParentWindow.mConfigUpdatedControl.AddImage("toast_checked", 23.0, 23.0, new Thickness?(new Thickness(0.0, 0.0, 10.0, 0.0)));
          else
            this.ParentWindow.mConfigUpdatedControl.AddImage("toast_error", 23.0, 23.0, new Thickness?(new Thickness(0.0, 0.0, 10.0, 0.0)));
          this.ParentWindow.mConfigUpdatedPopup.Visibility = Visibility.Visible;
          this.ParentWindow.mConfigUpdatedPopup.IsOpen = true;
          this.ParentWindow.mConfigUpdatedCanvas.Width = this.ParentWindow.mConfigUpdatedControl.ActualWidth;
          this.ParentWindow.mConfigUpdatedCanvas.Height = this.ParentWindow.mConfigUpdatedControl.ActualHeight;
          this.ParentWindow.mConfigUpdatedPopup.VerticalOffset = this.ParentWindow.mTopBar.ActualHeight + 25.0;
          if (this.ParentWindow.mSidebar.Visibility == Visibility.Visible)
            this.ParentWindow.mConfigUpdatedPopup.HorizontalOffset = -1.0 * (this.ParentWindow.mConfigUpdatedControl.ActualWidth + this.ParentWindow.mSidebar.ActualWidth) - 22.0;
          else
            this.ParentWindow.mConfigUpdatedPopup.HorizontalOffset = -1.0 * this.ParentWindow.mConfigUpdatedControl.ActualWidth - 22.0;
          if (this.configUpdatedToastTimer.IsEnabled)
            this.configUpdatedToastTimer.Stop();
          this.configUpdatedToastTimer.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing toast popup for config updated : " + ex.ToString());
        }
      }));
    }

    private void UpdateSchemeTimeoutTimer_Tick(object sender, EventArgs e)
    {
      if (this.updateSchemeTimerTicks >= 30)
      {
        this.updateSchemeTimeoutTimer.Stop();
        this.ShowPopupAndResetIcon(LocaleStrings.GetLocalizedString("STRING_UNEXPECTED_ERROR", ""), false);
        ClientStats.SendMiscellaneousStatsAsync("update_controls_error", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      }
      else
      {
        ++this.updateSchemeTimerTicks;
        if (!this.ParentWindow.mKeymappingFilesDownloaded)
          return;
        this.updateSchemeTimeoutTimer.Stop();
      }
    }

    internal void ConfigNotAvailable()
    {
      this.updateSchemeTimeoutTimer.Stop();
      this.ShowPopupAndResetIcon(LocaleStrings.GetLocalizedString("STRING_CFG_NOT_AVAILABLE", ""), false);
      ClientStats.SendMiscellaneousStatsAsync("update_controls_nocfg", KMManager.sPackageName, this.appVersion, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/advancedgamecontrolwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.AdvancedGameControlWindow_Loaded);
          ((Window) target).Closing += new CancelEventHandler(this.AdvancedGameControlWindow_Closing);
          ((Window) target).Closed += new EventHandler(this.AdvancedGameControlWindow_Closed);
          ((UIElement) target).KeyDown += new System.Windows.Input.KeyEventHandler(this.AdvancedGameControlWindow_KeyDown);
          break;
        case 2:
          this.mAdvancedGameControlBorder = (Border) target;
          this.mAdvancedGameControlBorder.PreviewMouseDown += new MouseButtonEventHandler(this.KeySequenceScriptGrid_PreviewMouseDown);
          break;
        case 3:
          this.PrimaryGrid = (Grid) target;
          break;
        case 4:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.TopBar_MouseLeftButtonDown);
          break;
        case 5:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.TopBar_MouseLeftButtonDown);
          break;
        case 6:
          this.mCloseSideBarWindow = (CustomPictureBox) target;
          this.mCloseSideBarWindow.MouseDown += new MouseButtonEventHandler(this.CustomPictureBox_MouseDown);
          this.mCloseSideBarWindow.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 7:
          this.mProfileHeader = (TextBlock) target;
          break;
        case 8:
          this.mUpdate = (CustomPictureBox) target;
          this.mUpdate.MouseLeftButtonUp += new MouseButtonEventHandler(this.UpdateBtn_Click);
          break;
        case 9:
          this.mImport = (CustomPictureBox) target;
          this.mImport.MouseLeftButtonUp += new MouseButtonEventHandler(this.ImportBtn_Click);
          break;
        case 10:
          this.mExport = (CustomPictureBox) target;
          this.mExport.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.Export_IsEnabledChanged);
          this.mExport.MouseLeftButtonUp += new MouseButtonEventHandler(this.ExportBtn_Click);
          break;
        case 11:
          this.mOpenFolder = (CustomPictureBox) target;
          this.mOpenFolder.MouseLeftButtonUp += new MouseButtonEventHandler(this.OpenFolder_MouseLeftButtonUp);
          break;
        case 12:
          this.mSchemeComboBox = (SchemeComboBox) target;
          break;
        case 13:
          this.mBrowserHelp = (CustomPictureBox) target;
          this.mBrowserHelp.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BrowserHelp_MouseEnter);
          this.mBrowserHelp.MouseLeave += new System.Windows.Input.MouseEventHandler(this.BrowserHelp_MouseLeave);
          this.mBrowserHelp.MouseLeftButtonUp += new MouseButtonEventHandler(this.BrowserHelp_MouseLeftButtonUp);
          break;
        case 14:
          this.mPrimitivesPanel = (WrapPanel) target;
          break;
        case 15:
          this.mTapPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 16:
          this.mTapRepeatPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 17:
          this.mDpadPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 18:
          this.mPanPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 19:
          this.mZoomPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 20:
          this.mMOBASkillPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 21:
          this.mSwipePrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 22:
          this.mFreeLookPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 23:
          this.mTiltPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 24:
          this.mStatePrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 25:
          this.mScriptPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 26:
          this.mMouseZoomPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 27:
          this.mRotatePrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 28:
          this.mScrollPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 29:
          this.mEdgeScrollPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 30:
          this.mMobaDpadPrimitive = (AdvancedSettingsItemPanel) target;
          break;
        case 31:
          this.mNCTransparencySlider = (Grid) target;
          break;
        case 32:
          this.mNCTransparencyLevel = (CustomTextBox) target;
          break;
        case 33:
          this.mNCTranslucentControlsSliderButton = (CustomPictureBox) target;
          this.mNCTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
          break;
        case 34:
          this.mNCTransSlider = (Slider) target;
          this.mNCTransSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.NCTransparencySlider_ValueChanged);
          break;
        case 35:
          this.mButtonsGrid = (StackPanel) target;
          this.mButtonsGrid.Loaded += new RoutedEventHandler(this.mButtonsGrid_Loaded);
          break;
        case 36:
          this.mRevertBtn = (CustomButton) target;
          this.mRevertBtn.Click += new RoutedEventHandler(this.RevertBtn_Click);
          break;
        case 37:
          this.mUndoBtn = (CustomButton) target;
          this.mUndoBtn.Click += new RoutedEventHandler(this.UndoButton_Click);
          break;
        case 38:
          this.mSaveBtn = (CustomButton) target;
          this.mSaveBtn.Click += new RoutedEventHandler(this.SaveButton_Click);
          break;
        case 39:
          this.mCanvas = (Canvas) target;
          this.mCanvas.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.mCanvas_PreviewMouseMove);
          this.mCanvas.PreviewMouseUp += new MouseButtonEventHandler(this.mCanvas_MouseUp);
          break;
        case 40:
          this.mOverlayGrid = (Grid) target;
          break;
        case 41:
          this.KeySequenceScriptGrid = (Grid) target;
          break;
        case 42:
          this.mScriptHeaderGrid = (Grid) target;
          this.mScriptHeaderGrid.MouseLeftButtonDown += new MouseButtonEventHandler(this.TopBar_MouseLeftButtonDown);
          break;
        case 43:
          this.mHeaderText = (TextBlock) target;
          break;
        case 44:
          this.mCloseScriptWindow = (CustomPictureBox) target;
          this.mCloseScriptWindow.MouseDown += new MouseButtonEventHandler(this.CustomPictureBox_MouseDown);
          this.mCloseScriptWindow.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseScriptButton_MouseLeftButtonUp);
          break;
        case 45:
          this.mSubheadingText = (TextBlock) target;
          break;
        case 46:
          this.mScriptText = (CustomTextBox) target;
          break;
        case 47:
          this.mXYCurrentCoordinatesText = (TextBlock) target;
          break;
        case 48:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.ShowHelpHyperlink_Click);
          break;
        case 49:
          this.mShowHelpHyperlink = (TextBlock) target;
          break;
        case 50:
          this.mFooterGrid = (Grid) target;
          break;
        case 51:
          this.mFooterText = (TextBlock) target;
          break;
        case 52:
          this.mKeySeqDoneButton = (CustomButton) target;
          this.mKeySeqDoneButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mDoneScriptButton_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
