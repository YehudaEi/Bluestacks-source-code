// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SpeedUpBluestacksUserControl
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
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class SpeedUpBluestacksUserControl : UserControl, IComponentConnector
  {
    internal TextBlock mTitleText;
    internal TextBlock mBodyText;
    internal CustomPictureBox mImage;
    internal Hyperlink mHyperLink;
    private bool _contentLoaded;

    public SpeedUpBluestacksUserControl()
    {
      this.InitializeComponent();
      if (!Oem.IsOEMDmm)
        return;
      this.mHyperLink.Foreground = (Brush) new BrushConverter().ConvertFrom((object) "#FF328CF2");
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
        BlueStacksUIUtils.OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in opening url" + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/speedupbluestacksusercontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mTitleText = (TextBlock) target;
          break;
        case 2:
          this.mBodyText = (TextBlock) target;
          break;
        case 3:
          this.mImage = (CustomPictureBox) target;
          break;
        case 4:
          this.mHyperLink = (Hyperlink) target;
          this.mHyperLink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
