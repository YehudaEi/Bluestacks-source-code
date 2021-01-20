// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ScreenLockControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class ScreenLockControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    internal CustomPictureBox mScreenLockImage;
    internal TextBlock mScreenLockText;
    private bool _contentLoaded;

    public ScreenLockControl()
    {
      this.InitializeComponent();
    }

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow { get; set; }

    public bool ShowTransparentWindow { get; set; }

    public bool Close()
    {
      this.Visibility = Visibility.Hidden;
      return true;
    }

    public bool Show()
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/screenlockcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mScreenLockText = (TextBlock) target;
        else
          this._contentLoaded = true;
      }
      else
        this.mScreenLockImage = (CustomPictureBox) target;
    }
  }
}
