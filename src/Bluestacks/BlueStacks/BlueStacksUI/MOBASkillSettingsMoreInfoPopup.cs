// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MOBASkillSettingsMoreInfoPopup
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class MOBASkillSettingsMoreInfoPopup : CustomPopUp, IComponentConnector
  {
    private CanvasElement mCanvasElement;
    internal MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;
    internal Border mMaskBorder4;
    internal Hyperlink mHyperLink;
    internal Path LeftArrow;
    private bool _contentLoaded;

    public MOBASkillSettingsMoreInfoPopup(CanvasElement canvasElement)
    {
      this.mCanvasElement = canvasElement;
      this.InitializeComponent();
      this.PlacementTarget = (UIElement) this.mCanvasElement?.MOBASkillSettingsPopup.mHelpIcon;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
        this.mCanvasElement.SendMOBAStats("read_more_clicked", "");
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingsmoreinfopopup.xaml", UriKind.Relative));
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
          this.mMOBASkillSettingsMoreInfoPopup = (MOBASkillSettingsMoreInfoPopup) target;
          break;
        case 2:
          this.mMaskBorder4 = (Border) target;
          break;
        case 3:
          this.mHyperLink = (Hyperlink) target;
          this.mHyperLink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 4:
          this.LeftArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
