// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroOverlay
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class MacroOverlay : UserControl, IDimOverlayControl, IComponentConnector
  {
    private bool mIsCloseOnOverLayClick;
    private bool _contentLoaded;

    public MainWindow ParentWindow { get; set; }

    public MacroOverlay()
    {
      this.InitializeComponent();
    }

    public MacroOverlay(MainWindow mainWindow)
    {
      this.ParentWindow = mainWindow;
    }

    private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.HideOverlay();
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("abortReroll", (Dictionary<string, string>) null);
    }

    private void HideOverlay()
    {
      this.ParentWindow.HideDimOverlay();
    }

    internal void ShowPromptAndHideOverlay()
    {
      if (this.Visibility != Visibility.Visible)
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_REROLL_COMPLETED", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_REROLL_COMPLETED_SUCCESS", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
      this.HideOverlay();
    }

    bool IDimOverlayControl.Close()
    {
      this.Visibility = Visibility.Hidden;
      return true;
    }

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return this.mIsCloseOnOverLayClick;
      }
      set
      {
        this.mIsCloseOnOverLayClick = value;
      }
    }

    public bool ShowControlInSeparateWindow { get; set; }

    public bool ShowTransparentWindow { get; set; }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrooverlay.xaml", UriKind.Relative));
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
      if (connectionId == 1)
        ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_PreviewMouseLeftButtonUp);
      else
        this._contentLoaded = true;
    }
  }
}
