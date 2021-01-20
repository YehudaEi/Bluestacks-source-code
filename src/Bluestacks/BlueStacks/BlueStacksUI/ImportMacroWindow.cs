// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ImportMacroWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class ImportMacroWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<ImportMacroScriptsControl, MacroRecording> mBoxToRecordingDict = new Dictionary<ImportMacroScriptsControl, MacroRecording>();
    private Dictionary<CustomTextBox, MacroRecording> mDependentRecordingDict = new Dictionary<CustomTextBox, MacroRecording>();
    private MacroRecorderWindow mOperationWindow;
    private MainWindow ParentWindow;
    internal StackPanel mScriptsStackPanel;
    internal int mNumberOfFilesSelectedForImport;
    private bool mInited;
    internal bool mIsInDependentFileFindingMode;
    internal Border mMaskBorder;
    internal ScrollViewer mScriptsListScrollbar;
    internal CustomCheckbox mSelectAllBtn;
    internal CustomButton mImportBtn;
    internal ProgressBar mLoadingGrid;
    private bool _contentLoaded;

    public ImportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
    {
      this.InitializeComponent();
      this.mOperationWindow = window;
      this.ParentWindow = mainWindow;
      this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
    }

    internal void TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!this.mInited)
        return;
      ImportMacroScriptsControl macroItemGrandchild = this.GetScriptControlFromMacroItemGrandchild((object) (sender as FrameworkElement).Parent);
      string text = (sender as CustomTextBox).Text;
      foreach (UIElement child in macroItemGrandchild.mDependentScriptsPanel.Children)
      {
        CustomTextBox index = child as CustomTextBox;
        index.Text = MacroRecorderWindow.GetDependentRecordingName(text, this.mDependentRecordingDict[index].Name);
      }
    }

    private void ImportMacroWindow_Closing(object sender, CancelEventArgs e)
    {
      this.CloseWindow();
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    private void CloseWindow()
    {
      this.mOperationWindow.mImportMacroWindow = (ImportMacroWindow) null;
      this.mOperationWindow.mOverlayGrid.Visibility = Visibility.Hidden;
      this.mOperationWindow.Focus();
    }

    private ImportMacroScriptsControl AddRecordingToStackPanelAndDict(
      MacroRecording record,
      bool isSingleRecording,
      out string suggestedName)
    {
      ImportMacroScriptsControl index = new ImportMacroScriptsControl(this, this.ParentWindow);
      index.Init(record.Name, isSingleRecording);
      suggestedName = !MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name.ToLower(CultureInfo.InvariantCulture))).Contains<string>(record.Name.ToLower(CultureInfo.InvariantCulture).Trim()) ? record.Name : CommonHandlers.GetMacroName(record.Name);
      index.mImportName.Text = this.ValidateSuggestedName(suggestedName);
      this.mScriptsStackPanel.Children.Add((UIElement) index);
      this.mBoxToRecordingDict[index] = record;
      return index;
    }

    internal void Init()
    {
      bool isSingleRecording = this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Count == 1;
      try
      {
        this.mInited = false;
        this.mScriptsStackPanel.Children.Clear();
        foreach (MacroRecording mRenamingMacros in this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList)
        {
          string suggestedName1;
          ImportMacroScriptsControl stackPanelAndDict = this.AddRecordingToStackPanelAndDict(mRenamingMacros, isSingleRecording, out suggestedName1);
          if (mRenamingMacros.RecordingType == RecordingTypes.MultiRecording)
          {
            bool? multiMacroAsUnified = this.mOperationWindow.mImportMultiMacroAsUnified;
            if (0 == (multiMacroAsUnified.GetValueOrDefault() ? 1 : 0) & multiMacroAsUnified.HasValue)
            {
              stackPanelAndDict.mDependentScriptsMsg.Visibility = Visibility.Visible;
              stackPanelAndDict.mDependentScriptsPanel.Visibility = Visibility.Visible;
              stackPanelAndDict.mDependentScriptsPanel.Children.Clear();
              foreach (string sourceRecording in mRenamingMacros.SourceRecordings)
              {
                MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
                string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(suggestedName1, macroRecording.Name);
                string suggestedName2 = !MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name)).Contains<string>(dependentRecordingName.ToLower(CultureInfo.InvariantCulture).Trim()) ? dependentRecordingName : CommonHandlers.GetMacroName(dependentRecordingName);
                CustomTextBox customTextBox = new CustomTextBox();
                customTextBox.Height = 24.0;
                customTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                customTextBox.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
                customTextBox.Text = this.ValidateSuggestedName(suggestedName2);
                customTextBox.Visibility = Visibility.Visible;
                customTextBox.IsEnabled = false;
                CustomTextBox index = customTextBox;
                stackPanelAndDict.mDependentScriptsPanel.Children.Add((UIElement) index);
                this.mDependentRecordingDict[index] = macroRecording;
              }
            }
          }
        }
        this.mNumberOfFilesSelectedForImport = 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in import window init err: " + ex.ToString());
      }
      this.mInited = true;
      if (isSingleRecording)
        this.mSelectAllBtn.Visibility = Visibility.Hidden;
      this.mSelectAllBtn.IsChecked = new bool?(true);
      this.SelectAllBtn_Click((object) null, (RoutedEventArgs) null);
    }

    private string ValidateSuggestedName(string suggestedName)
    {
      if (this.mBoxToRecordingDict.Keys.Any<ImportMacroScriptsControl>((Func<ImportMacroScriptsControl, bool>) (box => string.Equals(box.mImportName.Text.Trim(), suggestedName, StringComparison.InvariantCultureIgnoreCase))))
      {
        int startIndex = suggestedName.LastIndexOf('(') + 1;
        int num = suggestedName.LastIndexOf(')');
        int result;
        if (int.TryParse(suggestedName.Substring(startIndex, num - startIndex), out result))
        {
          suggestedName = suggestedName.Remove(startIndex, num - startIndex).Insert(startIndex, (result + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
          return this.ValidateSuggestedName(suggestedName);
        }
        Logger.Error("Error in ValidateSuggestedName: Could not get integer part in suggested name '{0}'", (object) suggestedName);
      }
      return suggestedName;
    }

    private bool CheckIfEditedMacroNameIsAllowed(string text, ImportMacroScriptsControl item)
    {
      if (string.IsNullOrEmpty(text.Trim()))
      {
        BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), "");
        return false;
      }
      foreach (MacroRecording vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.Instance.Vertices)
      {
        if (vertex.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
          return false;
      }
      foreach (ImportMacroScriptsControl child in this.mScriptsStackPanel.Children)
      {
        if (item != child)
        {
          bool? isChecked = child.mContent.IsChecked;
          bool flag = true;
          if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue && child.IsScriptInRenameMode() && child.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
            return false;
        }
      }
      return true;
    }

    private bool IsMacroItemDependentOfParent(ImportMacroScriptsControl item, string name)
    {
      return item.Tag != null && item.Tag.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase);
    }

    private ImportMacroScriptsControl GetScriptControlFromMacroItemGrandchild(
      object grandchild)
    {
      DependencyObject parent;
      for (; grandchild != null; grandchild = (object) parent)
      {
        parent = (grandchild as FrameworkElement).Parent;
        if (parent != null && parent is ImportMacroScriptsControl)
          return parent as ImportMacroScriptsControl;
      }
      return (ImportMacroScriptsControl) null;
    }

    internal void Box_Unchecked(object sender, RoutedEventArgs e)
    {
      if (this.mIsInDependentFileFindingMode)
        return;
      this.mIsInDependentFileFindingMode = true;
      --this.mNumberOfFilesSelectedForImport;
      if (this.mNumberOfFilesSelectedForImport == 0)
        this.mImportBtn.IsEnabled = false;
      if (this.mNumberOfFilesSelectedForImport < this.mScriptsStackPanel.Children.Count)
        this.mSelectAllBtn.IsChecked = new bool?(false);
      this.mIsInDependentFileFindingMode = false;
    }

    internal void Box_Checked(object sender, RoutedEventArgs e)
    {
      if (this.mIsInDependentFileFindingMode)
        return;
      this.mIsInDependentFileFindingMode = true;
      ++this.mNumberOfFilesSelectedForImport;
      if (this.mNumberOfFilesSelectedForImport > 0)
        this.mImportBtn.IsEnabled = true;
      if (this.mNumberOfFilesSelectedForImport == this.mScriptsStackPanel.Children.Count)
        this.mSelectAllBtn.IsChecked = new bool?(true);
      this.mIsInDependentFileFindingMode = false;
    }

    private static List<MacroEvents> GetRecordingEventsFromSourceRecording(
      MacroRecording srcRecording,
      double acceleration,
      long initialTime,
      ref long elapsedTime)
    {
      if (srcRecording == null)
        throw new Exception("Source recording now found in multiMacro");
      List<MacroEvents> macroEventsList = new List<MacroEvents>();
      foreach (MacroEvents macroEvents1 in srcRecording.Events)
      {
        MacroEvents macroEvents2 = macroEvents1;
        macroEvents2.Timestamp = (long) Math.Floor((double) macroEvents1.Timestamp / acceleration);
        macroEvents2.Timestamp += initialTime;
        elapsedTime = macroEvents2.Timestamp;
      }
      return macroEventsList;
    }

    private ImportMacroScriptsControl GetMacroItemFromTag(string tag)
    {
      foreach (ImportMacroScriptsControl child in this.mScriptsStackPanel.Children)
      {
        if (this.mBoxToRecordingDict[child].Name == tag)
          return child;
      }
      return (ImportMacroScriptsControl) null;
    }

    private void ImportBtn_Click(object sender, RoutedEventArgs e)
    {
      int num = 0;
      bool flag1 = false;
      bool flag2 = true;
      List<MacroRecording> newlyAddedMacro = new List<MacroRecording>();
      foreach (ImportMacroScriptsControl child in this.mScriptsStackPanel.Children)
      {
        bool? isChecked = child.mContent.IsChecked;
        bool flag3 = true;
        if (isChecked.GetValueOrDefault() == flag3 & isChecked.HasValue)
        {
          if (child.mImportName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
          {
            string path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""), (object) Environment.NewLine, (object) "\\ / : * ? \" < > |");
            BlueStacksUIBinding.Bind(child.mWarningMsg, path, "");
            child.mImportName.InputTextValidity = TextValidityOptions.Error;
            if (child.mImportName.IsEnabled)
              child.mWarningMsg.Visibility = Visibility.Visible;
            flag2 = false;
          }
          else if (((IEnumerable<string>) Constants.ReservedFileNamesList).Contains<string>(child.mImportName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
          {
            BlueStacksUIBinding.Bind(child.mWarningMsg, "STRING_MACRO_FILE_NAME_ERROR", "");
            child.mImportName.InputTextValidity = TextValidityOptions.Error;
            if (child.mImportName.IsEnabled)
              child.mWarningMsg.Visibility = Visibility.Visible;
            flag2 = false;
          }
          else if (!this.CheckIfEditedMacroNameIsAllowed(child.mImportName.Text, child) && child.IsScriptInRenameMode())
          {
            if (!string.IsNullOrEmpty(child.mImportName.Text.Trim()))
              BlueStacksUIBinding.Bind(child.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", ""), "");
            child.mImportName.InputTextValidity = TextValidityOptions.Error;
            if (child.mImportName.IsEnabled)
              child.mWarningMsg.Visibility = Visibility.Visible;
            flag2 = false;
          }
          else if (child.mDependentScriptsPanel.Visibility == Visibility.Visible && child.mDependentScriptsPanel.Children.Count > 0)
          {
            string path = this.CheckIfDependentScriptsHaveInvalidName(child);
            if (path != "TEXT_VALID")
            {
              BlueStacksUIBinding.Bind(child.mWarningMsg, path, "");
              child.mImportName.InputTextValidity = TextValidityOptions.Error;
              flag2 = false;
            }
            else
            {
              child.mImportName.InputTextValidity = TextValidityOptions.Success;
              child.mWarningMsg.Visibility = Visibility.Collapsed;
            }
          }
          else
          {
            child.mImportName.InputTextValidity = TextValidityOptions.Success;
            child.mWarningMsg.Visibility = Visibility.Collapsed;
          }
          flag1 = true;
        }
        ++num;
      }
      if (!flag1)
      {
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_IMPORT_MACRO_SELECTED", ""), 4.0, true);
      }
      else
      {
        if (!flag2)
          return;
        if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
          Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
        foreach (ImportMacroScriptsControl child1 in this.mScriptsStackPanel.Children)
        {
          bool? isChecked = child1.mContent.IsChecked;
          if (isChecked.GetValueOrDefault())
          {
            MacroRecording record = this.mBoxToRecordingDict[child1];
            isChecked = child1.mReplaceExistingBtn.IsChecked;
            string str;
            if (isChecked.HasValue)
            {
              isChecked = child1.mReplaceExistingBtn.IsChecked;
              if (isChecked.Value)
              {
                str = child1.mContent.Content.ToString();
                goto label_40;
              }
            }
            str = child1.mImportName.Text.Trim();
label_40:
            string newScript = str;
            MacroRecording existingMacro = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (m => string.Equals(m.Name, newScript, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
            if (existingMacro != null)
            {
              if (existingMacro.Parents.Count > 0)
              {
                for (int index = existingMacro.Parents.Count - 1; index >= 0; index--)
                {
                  MacroRecording macroRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => macro.Equals((object) existingMacro.Parents[index]))).FirstOrDefault<MacroRecording>();
                  this.mOperationWindow.FlattenRecording(existingMacro.Parents[index] as MacroRecording, false);
                  CommonHandlers.SaveMacroJson(existingMacro.Parents[index] as MacroRecording, (existingMacro.Parents[index] as MacroRecording).Name + ".json");
                  foreach (SingleMacroControl child2 in this.mOperationWindow.mScriptsStackPanel.Children)
                  {
                    if (child2.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
                      child2.mScriptSettingsImg.ImageName = "macro_settings";
                  }
                  MacroGraph.Instance.DeLinkMacroChild((BiDirectionalVertex<MacroRecording>) (existingMacro.Parents[index] as MacroRecording));
                }
              }
              this.DeleteMacroScript(existingMacro);
            }
            record.Name = newScript;
            if (record.RecordingType == RecordingTypes.MultiRecording)
            {
              this.mOperationWindow.ImportMultiMacro(record, this.mOperationWindow.mImportMultiMacroAsUnified.Value, newlyAddedMacro, this.GetDictionaryOfNewNamesForDependentRecordings(record.Name));
            }
            else
            {
              CommonHandlers.SaveMacroJson(record, record.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json");
              MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>) record);
              newlyAddedMacro.Add(record);
            }
          }
        }
        foreach (MacroRecording macro in newlyAddedMacro)
          MacroGraph.LinkMacroChilds(macro);
        this.mOperationWindow.mNewlyAddedMacrosList.AddRange((IEnumerable<MacroRecording>) newlyAddedMacro);
        this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
        this.Close();
      }
    }

    private void DeleteMacroScript(MacroRecording mRecording)
    {
      string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, mRecording.Name + ".json");
      if (File.Exists(path))
        File.Delete(path);
      if (mRecording.Shortcut != null && MainWindow.sMacroMapping.ContainsKey(mRecording.Shortcut))
        MainWindow.sMacroMapping.Remove(mRecording.Shortcut);
      ImportMacroWindow.DeleteScriptNameFromBookmarkedScriptListIfPresent(mRecording.Name);
      MacroGraph.Instance.RemoveVertex((BiDirectionalVertex<MacroRecording>) MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, mRecording.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>());
      if (this.ParentWindow.mAutoRunMacro != null && this.ParentWindow.mAutoRunMacro.Name.ToLower(CultureInfo.InvariantCulture).Trim() == mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
        this.ParentWindow.mAutoRunMacro = (MacroRecording) null;
      CommonHandlers.OnMacroDeleted(mRecording.Name);
    }

    public static bool DeleteScriptNameFromBookmarkedScriptListIfPresent(string fileName)
    {
      if (!((IEnumerable<string>) RegistryManager.Instance.BookmarkedScriptList).Contains<string>(fileName))
        return false;
      List<string> stringList = new List<string>((IEnumerable<string>) RegistryManager.Instance.BookmarkedScriptList);
      stringList.Remove(fileName);
      RegistryManager.Instance.BookmarkedScriptList = stringList.ToArray();
      return true;
    }

    private string CheckIfDependentScriptsHaveInvalidName(ImportMacroScriptsControl scriptControl)
    {
      string str = "TEXT_VALID";
      foreach (UIElement child1 in scriptControl.mDependentScriptsPanel.Children)
      {
        CustomTextBox customTextBox = child1 as CustomTextBox;
        string text = customTextBox.Text;
        if (text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
          customTextBox.InputTextValidity = TextValidityOptions.Error;
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""), (object) Environment.NewLine, (object) "\\ / : * ? \" < > |");
        }
        else if (((IEnumerable<string>) Constants.ReservedFileNamesList).Contains<string>(text.Trim().ToLower(CultureInfo.InvariantCulture)))
        {
          customTextBox.InputTextValidity = TextValidityOptions.Error;
          str = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
        }
        else if (scriptControl.IsScriptInRenameMode())
        {
          foreach (MacroRecording vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.Instance.Vertices)
          {
            if (vertex.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
            {
              customTextBox.InputTextValidity = TextValidityOptions.Error;
              return LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
            }
          }
          foreach (ImportMacroScriptsControl child2 in this.mScriptsStackPanel.Children)
          {
            if (child2 != scriptControl && scriptControl.IsScriptInRenameMode())
            {
              bool? isChecked = child2.mContent.IsChecked;
              bool flag = true;
              if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
              {
                if (child2.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
                {
                  customTextBox.InputTextValidity = TextValidityOptions.Error;
                  str = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
                }
                else
                {
                  foreach (UIElement child3 in child2.mDependentScriptsPanel.Children)
                  {
                    if ((child3 as CustomTextBox).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
                    {
                      customTextBox.InputTextValidity = TextValidityOptions.Error;
                      str = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
                      break;
                    }
                  }
                }
              }
            }
          }
        }
      }
      return str;
    }

    private Dictionary<string, string> GetDictionaryOfNewNamesForDependentRecordings(
      string parentMacroName)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (ImportMacroScriptsControl child in this.mScriptsStackPanel.Children)
      {
        if (child.Tag != null && child.Tag.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase) || child.mContent.Content.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase) && !child.mReplaceExistingBtn.IsChecked.GetValueOrDefault())
          dictionary.Add(child.mContent.Content.ToString(), child.mImportName.Text);
      }
      return dictionary;
    }

    private void ShowLoadingGrid(bool isShow)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isShow)
          this.mLoadingGrid.Visibility = Visibility.Visible;
        else
          this.mLoadingGrid.Visibility = Visibility.Hidden;
      }));
    }

    private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
    {
      if (this.mSelectAllBtn.IsChecked.Value)
      {
        foreach (object child in this.mScriptsStackPanel.Children)
          (child as ImportMacroScriptsControl).mContent.IsChecked = new bool?(true);
      }
      else
      {
        foreach (object child in this.mScriptsStackPanel.Children)
          (child as ImportMacroScriptsControl).mContent.IsChecked = new bool?(false);
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/importmacrowindow.xaml", UriKind.Relative));
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
          ((Window) target).Closing += new CancelEventHandler(this.ImportMacroWindow_Closing);
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 4:
          this.mScriptsListScrollbar = (ScrollViewer) target;
          break;
        case 5:
          this.mSelectAllBtn = (CustomCheckbox) target;
          this.mSelectAllBtn.Click += new RoutedEventHandler(this.SelectAllBtn_Click);
          break;
        case 6:
          this.mImportBtn = (CustomButton) target;
          this.mImportBtn.Click += new RoutedEventHandler(this.ImportBtn_Click);
          break;
        case 7:
          this.mLoadingGrid = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
