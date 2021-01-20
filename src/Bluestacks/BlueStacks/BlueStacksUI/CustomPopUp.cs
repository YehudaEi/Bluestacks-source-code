// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CustomPopUp
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class CustomPopUp : Popup
  {
    public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register(nameof (IsTopmost), typeof (bool), typeof (CustomPopUp), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CustomPopUp.OnIsTopmostChanged)));
    public static readonly DependencyProperty IsPopupEventTransparentProperty = DependencyProperty.Register(nameof (IsPopupEventTransparent), typeof (bool), typeof (CustomPopUp), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CustomPopUp.OnIsPopupEventTransparentPropertyChanged)));
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private static readonly IntPtr HWND_TOP = new IntPtr(0);
    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private bool? mAppliedTopMost;
    private bool mAlreadyLoaded;
    private Window mParentWindow;
    private const uint SWP_NOSIZE = 1;
    private const uint SWP_NOMOVE = 2;
    private const uint SWP_NOREDRAW = 8;
    private const uint SWP_NOACTIVATE = 16;
    private const uint SWP_NOOWNERZORDER = 512;
    private const uint SWP_NOSENDCHANGING = 1024;
    private const uint TOPMOST_FLAGS = 1563;

    public bool IsFocusOnMouseClick { get; set; }

    public bool IsTopmost
    {
      get
      {
        return (bool) this.GetValue(CustomPopUp.IsTopmostProperty);
      }
      set
      {
        this.SetValue(CustomPopUp.IsTopmostProperty, (object) value);
      }
    }

    public bool IsPopupEventTransparent
    {
      get
      {
        return (bool) this.GetValue(CustomPopUp.IsPopupEventTransparentProperty);
      }
      set
      {
        this.SetValue(CustomPopUp.IsPopupEventTransparentProperty, (object) value);
      }
    }

    public CustomPopUp()
    {
      this.Loaded += new RoutedEventHandler(this.OnPopupLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnPopupUnloaded);
      this.Opened += new EventHandler(this.CustomPopUp_Initialized);
      this.PreviewMouseDown += new MouseButtonEventHandler(this.CustomPopUp_PreviewMouseDown);
    }

    private void CustomPopUp_Initialized(object sender, EventArgs e)
    {
      RenderHelper.ChangeRenderModeToSoftware(sender);
    }

    private void CustomPopUp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.IsFocusOnMouseClick)
        return;
      try
      {
        if (!(PresentationSource.FromVisual((Visual) this.Child) is HwndSource hwndSource))
          return;
        InteropWindow.SetForegroundWindow(hwndSource.Handle);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in setting popup as foreground window: {0}", (object) ex);
      }
    }

    private void OnPopupLoaded(object sender, RoutedEventArgs e)
    {
      if (this.mAlreadyLoaded)
        return;
      this.mAlreadyLoaded = true;
      this.Child?.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.OnChildPreviewMouseLeftButtonDown), true);
      this.mParentWindow = Window.GetWindow((DependencyObject) this);
      if (this.mParentWindow == null)
        return;
      this.mParentWindow.Activated += new EventHandler(this.OnParentWindowActivated);
      this.mParentWindow.Deactivated += new EventHandler(this.OnParentWindowDeactivated);
    }

    private void OnPopupUnloaded(object sender, RoutedEventArgs e)
    {
      if (this.mParentWindow == null)
        return;
      this.mParentWindow.Activated -= new EventHandler(this.OnParentWindowActivated);
      this.mParentWindow.Deactivated -= new EventHandler(this.OnParentWindowDeactivated);
    }

    private void OnParentWindowActivated(object sender, EventArgs e)
    {
      this.SetTopmostState(true);
    }

    private void OnParentWindowDeactivated(object sender, EventArgs e)
    {
      if (this.IsTopmost)
        return;
      this.SetTopmostState(this.IsTopmost);
    }

    private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Logger.Debug("Child Mouse Left Button Down");
      this.SetTopmostState(true);
      if (this.mParentWindow == null || this.mParentWindow.IsActive || this.IsTopmost)
        return;
      this.mParentWindow.Activate();
      Logger.Debug("Activating Parent from child Left Button Down");
    }

    private static void OnIsTopmostChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs _)
    {
      CustomPopUp customPopUp = (CustomPopUp) obj;
      customPopUp.SetTopmostState(customPopUp.IsTopmost);
    }

    protected override void OnOpened(EventArgs e)
    {
      this.mParentWindow = Window.GetWindow((DependencyObject) this);
      this.SetTopmostState(this.IsTopmost);
      base.OnOpened(e);
    }

    private void SetTopmostState(bool isTop)
    {
      try
      {
        if (this.mParentWindow != null && !isTop && InteropWindow.GetTopmostOwnerWindow(this.mParentWindow).Topmost)
          isTop = true;
        if (this.mAppliedTopMost.HasValue)
        {
          bool? mAppliedTopMost = this.mAppliedTopMost;
          bool flag = isTop;
          if (mAppliedTopMost.GetValueOrDefault() == flag & mAppliedTopMost.HasValue)
            return;
        }
        if (this.Child == null || !(PresentationSource.FromVisual((Visual) this.Child) is HwndSource hwndSource))
          return;
        IntPtr handle = hwndSource.Handle;
        RECT lpRect;
        if (!NativeMethods.GetWindowRect(handle, out lpRect))
          return;
        Logger.Debug("setting z-order " + isTop.ToString());
        if (isTop)
        {
          NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_TOPMOST, lpRect.Left, lpRect.Top, (int) this.Width, (int) this.Height, 1563U);
        }
        else
        {
          NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_BOTTOM, lpRect.Left, lpRect.Top, (int) this.Width, (int) this.Height, 1563U);
          NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_TOP, lpRect.Left, lpRect.Top, (int) this.Width, (int) this.Height, 1563U);
          NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_NOTOPMOST, lpRect.Left, lpRect.Top, (int) this.Width, (int) this.Height, 1563U);
        }
        this.mAppliedTopMost = new bool?(isTop);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set topmost state in custom popup: {0}", (object) ex);
      }
    }

    private static void OnIsPopupEventTransparentPropertyChanged(
      DependencyObject target,
      DependencyPropertyChangedEventArgs e)
    {
      try
      {
        if (!(target is Popup popup))
          return;
        if ((bool) e.NewValue)
        {
          HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) popup.Child);
          InteropWindow.SetWindowLong(hwndSource.Handle, -20, InteropWindow.GetWindowLong(hwndSource.Handle, -20) | 32);
        }
        else
        {
          HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) popup.Child);
          InteropWindow.SetWindowLong(hwndSource.Handle, -20, InteropWindow.GetWindowLong(hwndSource.Handle, -20) & -33);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in setting popup event transparent: " + ex.ToString());
      }
    }
  }
}
