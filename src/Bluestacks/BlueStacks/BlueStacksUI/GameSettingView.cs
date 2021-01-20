// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GameSettingView
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

namespace BlueStacks.BlueStacksUI
{
  public class GameSettingView : UserControl, IComponentConnector
  {
    internal StepperTextBox mXSensitivity;
    internal StepperTextBox mYSensitivity;
    internal CustomButton mGuideBtn;
    internal Hyperlink mKnowMoreLink;
    private bool _contentLoaded;

    public GameSettingView()
    {
      this.InitializeComponent();
      this.mKnowMoreLink.Inlines.Clear();
      this.mKnowMoreLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_KNOW_MORE", ""));
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      BluestacksUIColor.ScrollBarScrollChanged(sender, e);
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
        Utils.OpenUrl(e.Uri.AbsoluteUri);
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/gamesettingview.xaml", UriKind.Relative));
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
          ((ScrollViewer) target).ScrollChanged += new ScrollChangedEventHandler(this.ScrollViewer_ScrollChanged);
          break;
        case 2:
          this.mXSensitivity = (StepperTextBox) target;
          break;
        case 3:
          this.mYSensitivity = (StepperTextBox) target;
          break;
        case 4:
          this.mGuideBtn = (CustomButton) target;
          break;
        case 5:
          this.mKnowMoreLink = (Hyperlink) target;
          this.mKnowMoreLink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
