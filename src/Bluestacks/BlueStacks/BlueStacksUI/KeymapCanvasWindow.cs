// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.KeymapCanvasWindow
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class KeymapCanvasWindow : CustomWindow, IComponentConnector
  {
    private int mTapYMargin = 5;
    private int mTapXMargin = 5;
    private int mMaxElementPerRow = 10;
    internal bool mIsShowWindow = true;
    private int mSidebarWidth = 260;
    private Point startPoint = new Point(-1.0, -1.0);
    internal Dictionary<IMAction, CanvasElement> dictCanvasElement = new Dictionary<IMAction, CanvasElement>();
    internal double SidebarWindowLeft = -1.0;
    internal double SidebarWindowTop = -1.0;
    internal double CanvasWindowLeft = -1.0;
    internal double CanvasWindowTop = -1.0;
    private int mCurrentTapElementDisplayRow;
    private int mCurrentTapElementDisplayCol;
    private bool isNewElementAdded;
    internal AdvancedGameControlWindow SidebarWindow;
    private int zIndex;
    internal CanvasElement mCanvasElement;
    internal double mParentWindowHeight;
    internal double mParentWindowWidth;
    internal double mParentWindowTop;
    internal double mParentWindowLeft;
    internal static bool IsDirty;
    internal static bool sWasMaximized;
    internal bool mIsExtraSettingsPopupOpened;
    private int mOldControlSchemeHashCode;
    private bool mIsInOverlayMode;
    internal bool mIsClosing;
    private Point mMousePointForNewTap;
    private IntPtr Handle;
    internal Canvas mCanvas;
    internal CustomPictureBox mCanvasImage;
    internal Grid mGrid;
    internal Canvas mCanvas2;
    internal OnboardingOverlayControl mOnboardingControl;
    private bool _contentLoaded;

    public MainWindow ParentWindow { get; }

    public static bool sIsDirty
    {
      get
      {
        return KeymapCanvasWindow.IsDirty;
      }
      set
      {
        KeymapCanvasWindow.IsDirty = value;
        if (KMManager.CanvasWindow == null || KMManager.CanvasWindow.SidebarWindow == null)
          return;
        KMManager.CanvasWindow.SidebarWindow.mUndoBtn.IsEnabled = KeymapCanvasWindow.IsDirty;
        KMManager.CanvasWindow.SidebarWindow.mSaveBtn.IsEnabled = KeymapCanvasWindow.IsDirty;
      }
    }

    internal bool IsInOverlayMode
    {
      get
      {
        return this.mIsInOverlayMode;
      }
      set
      {
        this.mIsInOverlayMode = value;
        this.IsShowGLWindow = value;
      }
    }

    internal KeymapCanvasWindow(MainWindow window)
    {
      this.ParentWindow = window;
      this.InitializeComponent();
      this.mParentWindowHeight = this.ParentWindow.ActualHeight * MainWindow.sScalingFactor;
      this.mParentWindowWidth = this.ParentWindow.ActualWidth * MainWindow.sScalingFactor;
      this.mParentWindowTop = this.ParentWindow.Top * MainWindow.sScalingFactor;
      this.mParentWindowLeft = this.ParentWindow.Left * MainWindow.sScalingFactor;
    }

    internal void ClearWindow()
    {
      this.dictCanvasElement.Clear();
      KMManager.listCanvasElement.Clear();
      CanvasElement.dictPoints.Clear();
      this.mCanvas.Children.Clear();
    }

    private void Canvas_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!KMManager.IsDragging)
        return;
      this.isNewElementAdded = true;
      KeymapCanvasWindow.sIsDirty = true;
      this.AddNewCanvasElement(KMManager.ClearElement(), false);
      this.StartMoving(this.mCanvasElement, e.GetPosition((IInputElement) this));
    }

    public void AddNewCanvasElement(List<IMAction> lstAction, bool isTap = false)
    {
      if (lstAction == null)
        return;
      this.mCanvasElement = new CanvasElement(this, this.ParentWindow);
      double num1 = (this.ActualWidth - this.mCanvasElement.ActualWidth) * 100.0 / (this.ActualWidth * 3.0) + (double) (this.mTapXMargin * this.mCurrentTapElementDisplayCol);
      double num2 = (this.ActualHeight - this.mCanvasElement.ActualHeight) * 100.0 / (this.ActualHeight * 3.0) + (double) (this.mTapYMargin * this.mCurrentTapElementDisplayRow);
      foreach (IMAction imAction in lstAction)
      {
        if (isTap)
        {
          imAction.PositionX = Math.Round(num1, 2);
          imAction.PositionY = Math.Round(num2, 2);
        }
        this.mCanvasElement.AddAction(imAction);
        this.dictCanvasElement.Add(imAction, this.mCanvasElement);
        if (isTap && lstAction.First<IMAction>().Type != KeyActionType.EdgeScroll && (lstAction.First<IMAction>().Type != KeyActionType.Scroll && lstAction.First<IMAction>().Type != KeyActionType.MOBADpad))
          this.mCanvasElement.ShowTextBox((object) this.mCanvasElement.dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
        if (!imAction.IsChildAction)
          this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imAction);
      }
      ++this.mCurrentTapElementDisplayCol;
      if (this.mCurrentTapElementDisplayCol == this.mMaxElementPerRow)
      {
        this.mCurrentTapElementDisplayCol = 0;
        ++this.mCurrentTapElementDisplayRow;
      }
      this.mCanvasElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.MoveIcon_PreviewMouseDown);
      this.mCanvasElement.mResizeIcon.PreviewMouseDown += new MouseButtonEventHandler(this.ResizeIcon_PreviewMouseDown);
      this.mCanvas.Children.Add((UIElement) this.mCanvasElement);
    }

    private void ResizeIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.mCanvasElement = WpfUtils.FindVisualParent<CanvasElement>(sender as DependencyObject);
      this.mCanvasElement.mResizeIcon.Focus();
      this.startPoint = e.GetPosition((IInputElement) this);
      this.mCanvas.PreviewMouseMove += new MouseEventHandler(this.CanvasResizeExistingElement_MouseMove);
      KeymapCanvasWindow.sIsDirty = true;
      e.Handled = true;
      Mouse.Capture((IInputElement) this.mCanvas);
    }

    private void CanvasResizeExistingElement_MouseMove(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.SizeNWSE;
      Point position = e.GetPosition((IInputElement) this);
      double num1 = position.X - this.startPoint.X;
      double num2 = position.Y - this.startPoint.Y;
      double num3 = num2;
      if (Math.Abs(num1) > Math.Abs(num2))
        num3 = num1;
      double num4 = Math.Round(num3, 2);
      double num5 = this.mCanvasElement.ActualWidth + num4;
      if (num5 < 40.0)
      {
        num5 = 40.0;
        num4 = num5 - this.mCanvasElement.ActualWidth;
      }
      if (this.mCanvasElement.ActionType == KeyActionType.MOBASkill)
      {
        this.mCanvasElement.mSkillImage.Visibility = Visibility.Visible;
        this.mCanvasElement.mCloseIcon.Visibility = Visibility.Visible;
        this.mCanvasElement.mActionIcon.Visibility = Visibility.Visible;
      }
      double d1 = Canvas.GetTop((UIElement) this.mCanvasElement);
      double d2 = Canvas.GetLeft((UIElement) this.mCanvasElement);
      if (double.IsNaN(d1))
        d1 = 0.0;
      if (double.IsNaN(d2))
        d2 = 0.0;
      double num6 = d1 - num4 / 2.0;
      double num7 = d2 - num4 / 2.0;
      this.mCanvasElement.Width = num5;
      this.mCanvasElement.Height = num5;
      Canvas.SetLeft((UIElement) this.mCanvasElement, num7);
      Canvas.SetTop((UIElement) this.mCanvasElement, num6);
      this.mCanvasElement.UpdatePosition(num6, num7);
      this.startPoint = position;
    }

    internal void ReloadCanvasWindow()
    {
      this.mCurrentTapElementDisplayRow = 0;
      this.mCurrentTapElementDisplayCol = 0;
      if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null)
        KMManager.LoadIMActions(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      this.mCanvas.Children.Clear();
      this.Init();
    }

    private void MoveIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      CanvasElement element = sender as CanvasElement;
      if (element.MOBASkillSettingsPopup != null && element.MOBASkillSettingsPopup.IsOpen || (element.MOBAOtherSettingsMoreInfoPopup.IsOpen || element.MOBASkillSettingsMoreInfoPopup.IsOpen))
      {
        e.Handled = true;
      }
      else
      {
        element.TopOnClick = Canvas.GetTop((UIElement) element);
        element.LeftOnClick = Canvas.GetLeft((UIElement) element);
        Point position = e.GetPosition((IInputElement) this);
        if (element.mResizeIcon.IsMouseOver || element.mCloseIcon.IsMouseOver)
          return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
          if (element.ListActionItem.First<IMAction>().Type == KeyActionType.Swipe)
          {
            bool isNewScheme = true;
            foreach (IMAction originalAction in element.ListActionItem)
            {
              this.CreateGameControlCopy(originalAction, e.GetPosition((IInputElement) this), isNewScheme);
              isNewScheme = false;
            }
          }
          else if (!element.ListActionItem.First<IMAction>().IsChildAction)
            this.CreateGameControlCopy(element.ListActionItem.First<IMAction>(), e.GetPosition((IInputElement) this), true);
        }
        else
          this.StartMoving(element, position);
        e.Handled = true;
      }
    }

    private void CreateGameControlCopy(IMAction originalAction, Point point, bool isNewScheme = true)
    {
      IMAction imAction = originalAction.DeepCopy<IMAction>();
      imAction.PositionX = originalAction.PositionX + 1.0;
      List<CanvasElement> source = this.AddCanvasElementsForAction(imAction, false);
      if (isNewScheme)
        KMManager.CheckAndCreateNewScheme();
      this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imAction);
      this.StartMoving(source.First<CanvasElement>(), point);
    }

    private void CanvasMoveExistingElement_MouseMove(object sender, MouseEventArgs e)
    {
      KMManager.CheckAndCreateNewScheme();
      this.Focus();
      this.MoveElement(e.GetPosition((IInputElement) this));
    }

    internal void StartMoving(CanvasElement element, Point p)
    {
      if (this.mCanvasElement != null && this.mCanvasElement != element)
        return;
      if (!element.mSkillImage.IsMouseOver)
        KeymapCanvasWindow.sIsDirty = true;
      this.mCanvasElement = element;
      this.startPoint = p;
      this.mCanvas.PreviewMouseMove -= new MouseEventHandler(this.CanvasMoveExistingElement_MouseMove);
      this.mCanvas.PreviewMouseMove += new MouseEventHandler(this.CanvasMoveExistingElement_MouseMove);
    }

    internal void MoveElement(Point p1)
    {
      if (!this.mCanvasElement.IsLoaded)
        return;
      this.Cursor = Cursors.Hand;
      double d1 = Canvas.GetTop((UIElement) this.mCanvasElement);
      double d2 = Canvas.GetLeft((UIElement) this.mCanvasElement);
      if (double.IsNaN(d1))
        d1 = 0.0;
      if (double.IsNaN(d2))
        d2 = 0.0;
      double num1 = d2 + this.mCanvasElement.ActualWidth / 2.0;
      double num2 = d1 + this.mCanvasElement.ActualHeight / 2.0;
      double num3 = num1 + (p1.X - this.startPoint.X);
      double num4 = num2 + (p1.Y - this.startPoint.Y);
      double num5 = num3 < 0.0 ? 0.0 : num3;
      double num6 = num4 < 0.0 ? 0.0 : num4;
      double num7 = num5 > this.mCanvas.ActualWidth ? this.mCanvas.ActualWidth : num5;
      double num8 = num6 > this.mCanvas.ActualHeight ? this.mCanvas.ActualHeight : num6;
      double num9 = num7 - this.mCanvasElement.ActualWidth / 2.0;
      double num10 = num8 - this.mCanvasElement.ActualHeight / 2.0;
      Canvas.SetLeft((UIElement) this.mCanvasElement, num9);
      Canvas.SetTop((UIElement) this.mCanvasElement, num10);
      this.mCanvasElement.UpdatePosition(num10, num9);
      this.startPoint = p1;
    }

    internal void Init()
    {
      if (this.ParentWindow.SelectedConfig?.SelectedControlScheme == null)
        return;
      int num1 = this.ParentWindow.SelectedConfig?.SelectedControlScheme.GetHashCode().Value;
      if (this.mOldControlSchemeHashCode == num1)
        return;
      this.mOldControlSchemeHashCode = num1;
      this.ClearWindow();
      IMConfig selectedConfig = this.ParentWindow.SelectedConfig;
      int num2;
      if (selectedConfig == null)
      {
        num2 = 0;
      }
      else
      {
        int? count = selectedConfig.SelectedControlScheme?.GameControls.Count;
        int num3 = 0;
        num2 = count.GetValueOrDefault() > num3 & count.HasValue ? 1 : 0;
      }
      if (num2 == 0)
        return;
      foreach (IMAction gameControl in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
      {
        if (!this.IsInOverlayMode || gameControl.IsVisibleInOverlay)
        {
          this.AddCanvasElementsForAction(gameControl, true);
          if (gameControl.Type == KeyActionType.MOBADpad)
          {
            (gameControl as MOBADpad).mMOBAHeroDummy = new MOBAHeroDummy(gameControl as MOBADpad);
            this.AddCanvasElementsForAction((IMAction) (gameControl as MOBADpad).mMOBAHeroDummy, true);
          }
        }
        else if (!gameControl.IsVisibleInOverlay)
        {
          List<CanvasElement> canvasElement = CanvasElement.GetCanvasElement(gameControl, this, this.ParentWindow);
          foreach (UIElement uiElement in canvasElement)
            uiElement.Visibility = Visibility.Hidden;
          KMManager.listCanvasElement.Add(canvasElement);
        }
      }
    }

    internal void InitLayout()
    {
      MainWindow parentWindow = this.ParentWindow;
      Grid mFrontendGrid = parentWindow.mFrontendGrid;
      Point point1 = mFrontendGrid.TranslatePoint(new Point(), (UIElement) parentWindow);
      if (this.IsInOverlayMode)
      {
        this.Background = (Brush) Brushes.Transparent;
        this.mCanvas.Background = (Brush) Brushes.Transparent;
        this.mCanvas2.Background = (Brush) Brushes.Transparent;
        this.mCanvas.Margin = new Thickness();
        if (Oem.IsOEMDmm)
        {
          this.Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
        }
        else
        {
          string path = Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "ImapImages"), this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName + ".png");
          if (File.Exists(path))
          {
            this.mCanvasImage.ImageName = path;
            this.mCanvas.Opacity = 0.0;
          }
          else
            this.mCanvas.Opacity = 1.0;
        }
        this.Handle = WindowInteropHelperExtensions.EnsureHandle(new WindowInteropHelper((Window) this));
        InteropWindow.SetWindowLong(this.Handle, -16, 1207959552);
      }
      else
      {
        IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.ParentWindow.Handle, true);
        double width = (double) fullscreenMonitorSize.Width - (double) this.mSidebarWidth * MainWindow.sScalingFactor;
        if (!this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
          width -= this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor;
        double height = this.ParentWindow.GetHeightFromWidth(width, true, false);
        if (height > (double) fullscreenMonitorSize.Height)
        {
          height = (double) fullscreenMonitorSize.Height;
          width = this.ParentWindow.GetWidthFromHeight(height, true, false);
        }
        double top = this.ParentWindow.Top * MainWindow.sScalingFactor + height <= (double) (fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) ? this.ParentWindow.Top * MainWindow.sScalingFactor : (double) fullscreenMonitorSize.Y + ((double) fullscreenMonitorSize.Height - height) / 2.0;
        double left = this.ParentWindow.Left * MainWindow.sScalingFactor + width + (double) this.mSidebarWidth * MainWindow.sScalingFactor <= (double) (fullscreenMonitorSize.X + fullscreenMonitorSize.Width) ? this.ParentWindow.Left * MainWindow.sScalingFactor : (double) fullscreenMonitorSize.X + ((double) fullscreenMonitorSize.Width - width - (double) this.mSidebarWidth * MainWindow.sScalingFactor) / 2.0;
        this.ParentWindow.ChangeHeightWidthTopLeft(width, height, top, left);
        this.Width = this.ParentWindow.ActualWidth;
        this.Height = this.ParentWindow.ActualHeight;
        this.Top = this.ParentWindow.Top;
        this.Left = this.ParentWindow.Left;
        this.mCanvas.Width = this.ParentWindow.mFullScreenRestoredButNotSidebar ? mFrontendGrid.ActualWidth - this.ParentWindow.mSidebar.Width : mFrontendGrid.ActualWidth;
        this.mCanvas.Height = mFrontendGrid.ActualHeight;
        Point point2 = new Point(parentWindow.ActualWidth - (this.mCanvas.Width + point1.X), parentWindow.ActualHeight - (mFrontendGrid.ActualHeight + point1.Y));
        this.mCanvas.Margin = new Thickness(point1.X, point1.Y, point2.X, point2.Y);
      }
    }

    private List<CanvasElement> AddCanvasElementsForAction(
      IMAction item,
      bool isLoadingFromFile = false)
    {
      List<CanvasElement> canvasElement1 = CanvasElement.GetCanvasElement(item, this, this.ParentWindow);
      foreach (CanvasElement canvasElement2 in canvasElement1)
      {
        canvasElement2.mIsLoadingfromFile = isLoadingFromFile;
        foreach (IMAction index in canvasElement2.ListActionItem)
          this.dictCanvasElement[index] = canvasElement2;
        if (canvasElement2.Parent == null)
        {
          this.mCanvas.Children.Add((UIElement) canvasElement2);
          canvasElement2.MouseLeftButtonDown -= new MouseButtonEventHandler(this.MoveIcon_PreviewMouseDown);
          canvasElement2.mResizeIcon.PreviewMouseDown -= new MouseButtonEventHandler(this.ResizeIcon_PreviewMouseDown);
          canvasElement2.MouseLeftButtonDown += new MouseButtonEventHandler(this.MoveIcon_PreviewMouseDown);
          canvasElement2.mResizeIcon.PreviewMouseDown += new MouseButtonEventHandler(this.ResizeIcon_PreviewMouseDown);
        }
        if (this.SidebarWindow == null)
          canvasElement2.Visibility = Visibility.Hidden;
      }
      KMManager.listCanvasElement.Add(canvasElement1);
      return canvasElement1;
    }

    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Cursor = Cursors.Arrow;
      if (this.mCanvasElement != null)
      {
        Panel.SetZIndex((UIElement) this.mCanvasElement, this.zIndex++);
        this.mCanvasElement.ShowOtherIcons(true);
        if (this.mCanvasElement.IsMouseDirectlyOver)
          e.Handled = true;
        if (this.isNewElementAdded && this.mCanvasElement.dictTextElemets.Count > 0)
        {
          this.isNewElementAdded = false;
          this.mCanvasElement.ShowTextBox((object) this.mCanvasElement.dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
        }
      }
      this.startPoint = new Point(-1.0, -1.0);
      this.mCanvas.PreviewMouseMove -= new MouseEventHandler(this.CanvasMoveExistingElement_MouseMove);
      this.mCanvas.PreviewMouseMove -= new MouseEventHandler(this.CanvasResizeExistingElement_MouseMove);
      Mouse.Capture((IInputElement) null);
      this.mCanvasElement = (CanvasElement) null;
    }

    private void KeymapCanvasWindow_Closing(object sender, CancelEventArgs e)
    {
      this.mIsClosing = true;
      this.ParentWindow.Focus();
    }

    private void CustomWindow_Closed(object sender, EventArgs e)
    {
      if (!KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) || KMManager.dictOverlayWindow[this.ParentWindow] != this)
        return;
      KMManager.dictOverlayWindow.Remove(this.ParentWindow);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      InteropWindow.RemoveWindowFromAltTabUI(new WindowInteropHelper((Window) this).Handle);
      if (!this.IsInOverlayMode)
      {
        this.ShowSideBarWindow();
      }
      else
      {
        this.Handle = new WindowInteropHelper((Window) this).Handle;
        InteropWindow.SetWindowLong(this.Handle, -16, 1207959552);
        this.ParentWindow.mFrontendHandler.UpdateOverlaySizeStatus();
        this.ParentWindow.LocationChanged += new EventHandler(this.ParentWindow_LocationChanged);
        this.ParentWindow.Activated += new EventHandler(this.ParentWindow_Activated);
        this.ParentWindow.Deactivated += new EventHandler(this.ParentWindow_Deactivated);
        this.UpdateSize();
      }
      this.Init();
    }

    private void ParentWindow_Deactivated(object sender, EventArgs e)
    {
      if (this.mIsClosing || KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.IsActive && KMManager.sGuidanceWindow.ParentWindow == this.ParentWindow)
        return;
      this.Hide();
    }

    private void ParentWindow_Activated(object sender, EventArgs e)
    {
      if (this.mIsClosing)
        return;
      this.Show();
    }

    private void ParentWindow_LocationChanged(object sender, EventArgs e)
    {
      this.UpdateSize();
    }

    internal void UpdateSize()
    {
      if (!(this.ParentWindow.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero) || this.mIsClosing)
        return;
      if (this.mIsShowWindow)
      {
        this.mIsShowWindow = false;
        Logger.Debug("KMP KeymapCanvasWindow UpdateSize");
        this.ParentWindow.mFrontendHandler.DeactivateFrontend();
        this.Show();
      }
      else
      {
        RECT rect = new RECT();
        InteropWindow.GetWindowRect(this.ParentWindow.StaticComponents.mLastMappableWindowHandle, ref rect);
        InteropWindow.SetWindowPos(this.Handle, (IntPtr) 0, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 16448U);
        this.ParentWindow.mFrontendHandler.FocusFrontend();
        this.SetOnboardingControlPosition(0.0, 0.0);
      }
    }

    private Point GetCorrectCoordinateLocationForAndroid(Point p)
    {
      return new Point(p.X * 100.0 / this.ParentWindow.Width, p.Y * 100.0 / this.ParentWindow.Height);
    }

    private void ShowSideBarWindow()
    {
      if (this.SidebarWindow != null)
        return;
      this.SidebarWindow = new AdvancedGameControlWindow(this.ParentWindow);
      this.SidebarWindow.Init(this);
      this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
      this.SidebarWindow.Owner = (Window) this;
      this.SidebarWindow.Show();
      this.SidebarWindow.Activate();
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (CanvasElement.sFocusedTextBox != null)
      {
        WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
      }
      else
      {
        if (double.IsNaN(this.CanvasWindowLeft) && double.IsNaN(this.CanvasWindowTop))
        {
          this.CanvasWindowLeft = this.Left;
          this.CanvasWindowTop = this.Top;
          this.mMousePointForNewTap = Mouse.GetPosition((IInputElement) this.mCanvas);
        }
        KeymapCanvasWindow.sIsDirty = true;
        try
        {
          this.DragMove();
        }
        catch (Exception ex)
        {
        }
        if (Math.Abs(this.CanvasWindowLeft - this.Left) < 2.0 && Math.Abs(this.CanvasWindowTop - this.Top) < 2.0)
        {
          if (KMManager.sIsInScriptEditingMode && this.mIsExtraSettingsPopupOpened)
            return;
          Tap tap = new Tap();
          tap.Type = KeyActionType.Tap;
          IMAction imAction = (IMAction) tap;
          if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0 && CanvasElement.sFocusedTextBox != null)
          {
            WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
          }
          else
          {
            if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
              KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, (IMControlScheme) null, false);
            else if (this.ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
              KMManager.CheckAndCreateNewScheme();
            this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imAction);
            List<CanvasElement> source = this.AddCanvasElementsForAction(imAction, false);
            source.First<CanvasElement>().SetMousePoint(this.mMousePointForNewTap);
            source.First<CanvasElement>().IsRemoveIfEmpty = true;
            source.First<CanvasElement>().ShowTextBox((object) source.First<CanvasElement>().dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
          }
        }
        this.CanvasWindowLeft = double.NaN;
        this.CanvasWindowTop = double.NaN;
      }
    }

    private void CustomWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mMousePointForNewTap = Mouse.GetPosition((IInputElement) this.mCanvas);
      this.CanvasWindowLeft = this.Left;
      this.CanvasWindowTop = this.Top;
      try
      {
        this.DragMove();
      }
      catch (Exception ex)
      {
      }
    }

    private void CustomWindow_LocationChanged(object sender, EventArgs e)
    {
      if (this.IsInOverlayMode)
        return;
      this.ParentWindow.Top = this.Top;
      this.ParentWindow.Left = this.Left;
    }

    private void CustomWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      foreach (object child in this.mCanvas.Children)
      {
        if (this.IsInOverlayMode)
        {
          (child as CanvasElement).SetElementLayout(true, (child as CanvasElement).mXPosition, (child as CanvasElement).mYPosition);
          if ((child as CanvasElement).ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
            this.SetOnboardingControlPosition((child as CanvasElement).mXPosition, (child as CanvasElement).mYPosition);
        }
        else
          (child as CanvasElement).SetElementLayout(false, 0.0, 0.0);
      }
    }

    private void MCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      ToolTip toolTip = new ToolTip();
      if (!KMManager.sIsInScriptEditingMode)
        return;
      Point locationForAndroid = this.GetCorrectCoordinateLocationForAndroid(Mouse.GetPosition((IInputElement) this.mCanvas));
      if (toolTip.IsOpen)
        toolTip.IsOpen = false;
      toolTip.Content = (object) string.Format(" X: {0} Y: {1}", (object) locationForAndroid.X, (object) locationForAndroid.Y);
      toolTip.StaysOpen = true;
      toolTip.Placement = PlacementMode.Mouse;
      toolTip.IsOpen = true;
    }

    internal void ShowOnboardingOverlayControl(double left, double top, bool isVisible = true)
    {
      if (!isVisible || !File.Exists(Path.Combine(CustomPictureBox.AssetsDir, "onboarding_step_" + KMManager.mOnboardingCounter.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ".png")) || !PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName).GetValueOrDefault())
      {
        this.mOnboardingControl.Visibility = Visibility.Collapsed;
        this.mGrid.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.mCanvas2.Opacity = 1.0;
        this.mOnboardingControl.mOnboardingImg.ImageName = "onboarding_step_" + KMManager.mOnboardingCounter.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.mOnboardingControl.Visibility = Visibility.Visible;
        this.mOnboardingControl.mOnboardingImg.Visibility = Visibility.Visible;
        this.mGrid.Visibility = Visibility.Visible;
        this.SetOnboardingControlPosition(left, top);
      }
    }

    internal void SetOnboardingControlPosition(double left, double top)
    {
      if (left == 0.0 || top == 0.0)
      {
        left = 62.8;
        top = 15.6;
      }
      double num1 = left / 100.0 * this.mCanvas.ActualWidth;
      double num2 = top / 100.0 * this.mCanvas.ActualHeight;
      double num3 = num1 < 0.0 ? 0.0 : num1;
      double num4 = num2 < 0.0 ? 0.0 : num2;
      double num5 = num3 > this.ParentWindow.ActualWidth ? this.ParentWindow.ActualWidth : num3;
      double num6 = num4 > this.ParentWindow.ActualHeight ? this.ParentWindow.ActualHeight : num4;
      double num7 = 310.0;
      this.mOnboardingControl.mOnboardingImg.Height = 85.0 / 100.0 * this.mCanvas.ActualHeight * 0.2;
      this.mOnboardingControl.mOnboardingImg.Width = num7 / 100.0 * this.mCanvas.ActualWidth * 0.1;
      left = num5 - this.mOnboardingControl.mOnboardingImg.Width / 2.0;
      top = num6 - this.mOnboardingControl.mOnboardingImg.Height / 2.0;
      Canvas.SetLeft((UIElement) this.mOnboardingControl, left);
      Canvas.SetTop((UIElement) this.mOnboardingControl, top);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/keymapcanvaswindow.xaml", UriKind.Relative));
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
          ((Window) target).Closing += new CancelEventHandler(this.KeymapCanvasWindow_Closing);
          ((Window) target).Closed += new EventHandler(this.CustomWindow_Closed);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
          ((Window) target).LocationChanged += new EventHandler(this.CustomWindow_LocationChanged);
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.CustomWindow_MouseDown);
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.CustomWindow_SizeChanged);
          break;
        case 2:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.Canvas_MouseDown);
          break;
        case 3:
          this.mCanvas = (Canvas) target;
          this.mCanvas.MouseEnter += new MouseEventHandler(this.Canvas_MouseEnter);
          this.mCanvas.PreviewMouseUp += new MouseButtonEventHandler(this.Canvas_MouseUp);
          this.mCanvas.MouseDown += new MouseButtonEventHandler(this.CustomWindow_MouseDown);
          break;
        case 4:
          this.mCanvasImage = (CustomPictureBox) target;
          break;
        case 5:
          this.mGrid = (Grid) target;
          break;
        case 6:
          this.mCanvas2 = (Canvas) target;
          break;
        case 7:
          this.mOnboardingControl = (OnboardingOverlayControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
