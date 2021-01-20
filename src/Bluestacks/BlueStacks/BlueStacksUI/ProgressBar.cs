// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ProgressBar
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
  public class ProgressBar : UserControl, IDimOverlayControl, IComponentConnector
  {
    internal ProgressBar mProgressBar;
    internal CustomPictureBox mLoadingImage;
    internal TextBlock mLabel;
    private bool _contentLoaded;

    public string ProgressText
    {
      get
      {
        return this.mLabel.Text;
      }
      set
      {
        BlueStacksUIBinding.Bind(this.mLabel, value, "");
        if (!string.IsNullOrEmpty(this.mLabel.Text))
          return;
        this.mLabel.Visibility = Visibility.Collapsed;
      }
    }

    public bool IsCloseOnOverLayClick
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowTransparentWindow { get; set; }

    public ProgressBar()
    {
      this.InitializeComponent();
    }

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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/progressbar.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mProgressBar = (ProgressBar) target;
          break;
        case 2:
          this.mLoadingImage = (CustomPictureBox) target;
          break;
        case 3:
          this.mLabel = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
