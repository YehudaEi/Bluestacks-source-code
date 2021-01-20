// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallFeedback
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Uninstaller
{
  public partial class UninstallFeedback : UserControl, IComponentConnector
  {
    internal TextWrapCustomCheckBox mInstallEngineFail;
    internal TextWrapCustomCheckBox mInstallGameFail;
    internal TextWrapCustomCheckBox mConflictWithOthers;
    internal TextWrapCustomCheckBox mStartEngineFail;
    internal TextWrapCustomCheckBox mGameLag;
    internal TextWrapCustomCheckBox mBlackScreen;
    internal TextWrapCustomCheckBox mCantFindGame;
    internal TextWrapCustomCheckBox mAppCrash;
    internal TextWrapCustomCheckBox mExeCrash;
    internal CustomTextBox mOtherReasonTextBox;
    private bool _contentLoaded;

    public UninstallFeedback()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/uninstallfeedback.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mInstallEngineFail = (TextWrapCustomCheckBox) target;
          break;
        case 2:
          this.mInstallGameFail = (TextWrapCustomCheckBox) target;
          break;
        case 3:
          this.mConflictWithOthers = (TextWrapCustomCheckBox) target;
          break;
        case 4:
          this.mStartEngineFail = (TextWrapCustomCheckBox) target;
          break;
        case 5:
          this.mGameLag = (TextWrapCustomCheckBox) target;
          break;
        case 6:
          this.mBlackScreen = (TextWrapCustomCheckBox) target;
          break;
        case 7:
          this.mCantFindGame = (TextWrapCustomCheckBox) target;
          break;
        case 8:
          this.mAppCrash = (TextWrapCustomCheckBox) target;
          break;
        case 9:
          this.mExeCrash = (TextWrapCustomCheckBox) target;
          break;
        case 10:
          this.mOtherReasonTextBox = (CustomTextBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
