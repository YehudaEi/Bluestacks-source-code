// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ImportMacroScriptsControl
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
  public class ImportMacroScriptsControl : UserControl, IComponentConnector
  {
    internal ImportMacroWindow mImportMacroWindow;
    internal MainWindow ParentWindow;
    private static int mIdCount;
    internal Grid mMainGrid;
    internal CustomCheckbox mContent;
    internal TextBlock mSingleMacroRecordTextblock;
    internal Grid mBlock;
    internal TextBlock mMacroImportedAsTextBlock;
    internal StackPanel mConflictingMacroOptionsPanel;
    internal CustomRadioButton mReplaceExistingBtn;
    internal CustomRadioButton mRenameBtn;
    internal CustomTextBox mImportName;
    internal TextBlock mWarningMsg;
    internal TextBlock mDependentScriptsMsg;
    internal StackPanel mDependentScriptsPanel;
    private bool _contentLoaded;

    public ImportMacroScriptsControl(ImportMacroWindow importMacroWindow, MainWindow window)
    {
      this.InitializeComponent();
      this.mImportMacroWindow = importMacroWindow;
      this.ParentWindow = window;
      ++ImportMacroScriptsControl.mIdCount;
    }

    private void Box_Checked(object sender, RoutedEventArgs e)
    {
      this.mImportMacroWindow.Box_Checked(sender, e);
    }

    private void Box_Unchecked(object sender, RoutedEventArgs e)
    {
      this.mImportMacroWindow.Box_Unchecked(sender, e);
    }

    private void ImportName_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.mImportMacroWindow.TextChanged(sender, e);
    }

    internal void Init(string macroName, bool isSingleRecording)
    {
      this.mRenameBtn.ApplyTemplate();
      this.mReplaceExistingBtn.ApplyTemplate();
      this.mRenameBtn.RadioBtnImage.Width = 14.0;
      this.mRenameBtn.RadioBtnImage.Height = 14.0;
      this.mRenameBtn.GroupName = string.Format("MacroConflictAction_{0}{1}", (object) macroName, (object) ImportMacroScriptsControl.mIdCount);
      this.mReplaceExistingBtn.RadioBtnImage.Width = 14.0;
      this.mReplaceExistingBtn.RadioBtnImage.Height = 14.0;
      this.mReplaceExistingBtn.GroupName = string.Format("MacroConflictAction_{0}{1}", (object) macroName, (object) ImportMacroScriptsControl.mIdCount);
      this.mReplaceExistingBtn.IsChecked = new bool?(true);
      this.mReplaceExistingBtn.Checked += new RoutedEventHandler(this.ConflictingMacroHandlingRadioBtn_Checked);
      this.mContent.Content = (object) macroName;
      if (isSingleRecording)
      {
        this.mContent.Visibility = Visibility.Collapsed;
        this.mBlock.Margin = new Thickness(0.0);
        this.mMainGrid.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
        this.mSingleMacroRecordTextblock.Visibility = Visibility.Visible;
        this.mSingleMacroRecordTextblock.Text = macroName;
        this.mWarningMsg.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
      }
      else
        this.mMainGrid.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
    }

    private void ConflictingMacroHandlingRadioBtn_Checked(object sender, RoutedEventArgs e)
    {
      if (!(sender is CustomRadioButton customRadioButton) || this.mImportName == null)
        return;
      if (customRadioButton == this.mRenameBtn)
        this.mImportName.Visibility = Visibility.Visible;
      else
        this.mImportName.Visibility = Visibility.Collapsed;
    }

    internal bool IsScriptInRenameMode()
    {
      return this.mRenameBtn.IsChecked.HasValue && this.mRenameBtn.IsChecked.Value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/importmacroscriptscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMainGrid = (Grid) target;
          break;
        case 2:
          this.mContent = (CustomCheckbox) target;
          this.mContent.Checked += new RoutedEventHandler(this.Box_Checked);
          this.mContent.Unchecked += new RoutedEventHandler(this.Box_Unchecked);
          break;
        case 3:
          this.mSingleMacroRecordTextblock = (TextBlock) target;
          break;
        case 4:
          this.mBlock = (Grid) target;
          break;
        case 5:
          this.mMacroImportedAsTextBlock = (TextBlock) target;
          break;
        case 6:
          this.mConflictingMacroOptionsPanel = (StackPanel) target;
          break;
        case 7:
          this.mReplaceExistingBtn = (CustomRadioButton) target;
          break;
        case 8:
          this.mRenameBtn = (CustomRadioButton) target;
          this.mRenameBtn.Checked += new RoutedEventHandler(this.ConflictingMacroHandlingRadioBtn_Checked);
          break;
        case 9:
          this.mImportName = (CustomTextBox) target;
          this.mImportName.TextChanged += new TextChangedEventHandler(this.ImportName_TextChanged);
          break;
        case 10:
          this.mWarningMsg = (TextBlock) target;
          break;
        case 11:
          this.mDependentScriptsMsg = (TextBlock) target;
          break;
        case 12:
          this.mDependentScriptsPanel = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
