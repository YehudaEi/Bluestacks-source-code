// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroToAdd
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class MacroToAdd : UserControl, IComponentConnector
  {
    private MergeMacroWindow mMergeMacroWindow;
    internal TextBlock mMacroName;
    internal CustomPictureBox mAddMacro;
    private bool _contentLoaded;

    public MacroToAdd(MergeMacroWindow window, string macroName)
    {
      this.InitializeComponent();
      this.mMergeMacroWindow = window;
      this.Tag = (object) macroName;
      this.mMacroName.Text = macroName;
    }

    private void AddMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      MergedMacroConfiguration macroConfiguration = new MergedMacroConfiguration();
      macroConfiguration.MacrosToRun.Add(this.mMacroName.Text);
      macroConfiguration.Tag = this.mMergeMacroWindow.mAddedMacroTag++;
      if (this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations == null)
        this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>();
      this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Add(macroConfiguration);
    }

    private void MacroName_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      (sender as TextBlock).SetTextblockTooltip();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrotoadd.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
        {
          this.mAddMacro = (CustomPictureBox) target;
          this.mAddMacro.MouseLeftButtonUp += new MouseButtonEventHandler(this.AddMacro_MouseLeftButtonUp);
        }
        else
          this._contentLoaded = true;
      }
      else
      {
        this.mMacroName = (TextBlock) target;
        this.mMacroName.SizeChanged += new SizeChangedEventHandler(this.MacroName_SizeChanged);
      }
    }
  }
}
