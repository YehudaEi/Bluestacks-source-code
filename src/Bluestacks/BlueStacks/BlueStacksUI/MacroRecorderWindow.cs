// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroRecorderWindow
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
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class MacroRecorderWindow : CustomWindow, IDisposable, IComponentConnector
  {
    internal List<MacroRecording> mRenamingMacrosList = new List<MacroRecording>();
    internal List<MacroRecording> mNewlyAddedMacrosList = new List<MacroRecording>();
    private MainWindow ParentWindow;
    internal string mMacroOnRestart;
    internal StackPanel mScriptsStackPanel;
    private System.Timers.Timer mMacroLoopTimer;
    internal ExportMacroWindow mExportMacroWindow;
    internal MergeMacroWindow mMergeMacroWindow;
    internal ImportMacroWindow mImportMacroWindow;
    internal bool? mImportMultiMacroAsUnified;
    private bool mAlternateBackgroundColor;
    internal BackgroundWorker mBGMacroPlaybackWorker;
    private bool disposedValue;
    internal Border mOperationRecorderBorder;
    internal Border mMaskBorder;
    internal CustomPictureBox mMerge;
    internal CustomPictureBox mImport;
    internal CustomPictureBox mExport;
    internal CustomPictureBox mOpenFolder;
    internal CustomButton mStartMacroRecordingBtn;
    internal CustomButton mStopMacroRecordingBtn;
    internal CustomButton mGetMacroBtn;
    internal Border mNoScriptsGrid;
    internal Grid mScriptsGrid;
    internal ScrollViewer mScriptsListScrollbar;
    internal CustomButton mOpenCommunityBtn;
    internal ProgressBar mLoadingGrid;
    internal Grid mOverlayGrid;
    private bool _contentLoaded;

    public MacroRecorderWindow(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Owner = (Window) this.ParentWindow;
      this.IsShowGLWindow = true;
      this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
      this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      if (window != null)
        window.mCommonHandler.MacroSettingChangedEvent += new CommonHandlers.MacroSettingsChanged(this.ParentWindow_MacroSettingChangedEvent);
      this.Init();
      if (this.ParentWindow == null)
        return;
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent -= new MacroTopBarPlayControl.ScriptPlayDelegate(this.ParentWindow_ScriptPlayEvent);
        this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent -= new MacroTopBarPlayControl.ScriptStopDelegate(this.MacroPlayControl_ScriptStopEvent);
        this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent += new MacroTopBarPlayControl.ScriptPlayDelegate(this.ParentWindow_ScriptPlayEvent);
        this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent += new MacroTopBarPlayControl.ScriptStopDelegate(this.MacroPlayControl_ScriptStopEvent);
      }
      else
      {
        this.ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent -= new MacroTopBarPlayControl.ScriptPlayDelegate(this.ParentWindow_ScriptPlayEvent);
        this.ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent -= new MacroTopBarPlayControl.ScriptStopDelegate(this.MacroPlayControl_ScriptStopEvent);
        this.ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent += new MacroTopBarPlayControl.ScriptPlayDelegate(this.ParentWindow_ScriptPlayEvent);
        this.ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent += new MacroTopBarPlayControl.ScriptStopDelegate(this.MacroPlayControl_ScriptStopEvent);
      }
    }

    public void ShowAtCenter()
    {
      this.Show();
      BlueStacks.Common.RECT lpRect1;
      NativeMethods.GetWindowRect((this.Owner as MainWindow).Handle, out lpRect1);
      BlueStacks.Common.RECT lpRect2;
      NativeMethods.GetWindowRect(new WindowInteropHelper((Window) this).Handle, out lpRect2);
      BlueStacks.Common.RECT placementRect = new BlueStacks.Common.RECT()
      {
        Left = (lpRect1.Right - lpRect1.Left - lpRect2.Right + lpRect2.Left) / 2 + lpRect1.Left,
        Top = (lpRect1.Bottom - lpRect1.Top - lpRect2.Bottom + lpRect2.Top) / 2 + lpRect1.Top
      };
      placementRect.Right = placementRect.Left + lpRect2.Right - lpRect2.Left;
      placementRect.Bottom = placementRect.Top + lpRect2.Bottom - lpRect2.Top;
      WindowPlacement.SetPlacement(new WindowInteropHelper((Window) this).Handle, placementRect);
    }

    private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
    {
      if (!record.PlayOnStart)
        return;
      foreach (SingleMacroControl child in this.mScriptsStackPanel.Children)
      {
        if (child.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == record.Name.ToLower(CultureInfo.InvariantCulture).Trim())
        {
          this.ChangeAutorunImageVisibility(child.mAutorunImage, Visibility.Visible);
        }
        else
        {
          this.ChangeAutorunImageVisibility(child.mAutorunImage, Visibility.Hidden);
          if (child.mRecording.PlayOnStart)
          {
            child.mRecording.PlayOnStart = false;
            if (child.mMacroSettingsWindow != null)
              child.mMacroSettingsWindow.mPlayOnStartCheckBox.IsChecked = new bool?(false);
            JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            string contents = JsonConvert.SerializeObject((object) child.mRecording, serializerSettings);
            File.WriteAllText(CommonHandlers.GetCompleteMacroRecordingPath(child.mRecording.Name), contents);
          }
        }
      }
    }

    private void ChangeAutorunImageVisibility(CustomPictureBox cpb, Visibility visibility)
    {
      this.Dispatcher.Invoke((Delegate) (() => cpb.Visibility = visibility));
    }

    private void MacroPlayControl_ScriptStopEvent(string tag)
    {
      SingleMacroControl controlFromTag = this.GetControlFromTag(tag);
      controlFromTag?.ToggleScriptPlayPauseUi(false);
      if (controlFromTag != null && controlFromTag.mRecording.DonotShowWindowOnFinish || this.ParentWindow.IsClosed)
        return;
      this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
    }

    private void ParentWindow_ScriptPlayEvent(string tag)
    {
      this.GetControlFromTag(tag)?.ToggleScriptPlayPauseUi(true);
    }

    public void Init()
    {
      this.ParentWindow.mIsScriptsPresent = false;
      this.mAlternateBackgroundColor = false;
      this.AddScriptsToStackPanel();
      if (!this.ParentWindow.mIsScriptsPresent)
      {
        this.mNoScriptsGrid.Visibility = Visibility.Visible;
        this.mExport.IsEnabled = false;
        this.mExport.Opacity = 0.4;
      }
      else
      {
        this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
        this.mExport.IsEnabled = true;
        this.mExport.Opacity = 1.0;
      }
      if (this.ParentWindow.mIsMacroRecorderActive)
      {
        this.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
        this.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
      }
      else
      {
        this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
        this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
      }
      this.ToggleUI(this.ParentWindow.mIsMacroRecorderActive);
      if (this.ParentWindow.mIsMacroPlaying)
      {
        this.mStartMacroRecordingBtn.Visibility = Visibility.Hidden;
        this.mStopMacroRecordingBtn.Visibility = Visibility.Hidden;
      }
      this.ShowLoadingGrid(false);
    }

    private void AddScriptsToStackPanel()
    {
      foreach (MacroRecording record in (IEnumerable<MacroRecording>) MacroGraph.Instance.Vertices.Cast<MacroRecording>().OrderBy<MacroRecording, DateTime>((Func<MacroRecording, DateTime>) (macro => DateTime.ParseExact(macro.TimeCreated, "yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal))))
      {
        if (record != null && !string.IsNullOrEmpty(record.Name) && !string.IsNullOrEmpty(record.TimeCreated))
        {
          if (record.Events == null)
          {
            ObservableCollection<MergedMacroConfiguration> macroConfigurations = record.MergedMacroConfigurations;
            // ISSUE: explicit non-virtual call
            if ((macroConfigurations != null ? (__nonvirtual (macroConfigurations.Count) > 0 ? 1 : 0) : 0) == 0)
              continue;
          }
          this.ParentWindow.mIsScriptsPresent = true;
          SingleMacroControl singleMacroControl = new SingleMacroControl(this.ParentWindow, record, this);
          singleMacroControl.Tag = (object) record.Name;
          SingleMacroControl mScriptControl = singleMacroControl;
          if (this.ParentWindow.mIsMacroPlaying && !string.Equals(this.ParentWindow.mMacroPlaying, record.Name, StringComparison.InvariantCulture))
            CommonHandlers.DisableScriptControl(mScriptControl);
          else if (this.ParentWindow.mIsMacroPlaying)
            mScriptControl.mEditNameImg.IsEnabled = false;
          else if (this.ParentWindow.mIsMacroRecorderActive)
            CommonHandlers.DisableScriptControl(mScriptControl);
          if (record.PlayOnStart)
            mScriptControl.mAutorunImage.Visibility = Visibility.Visible;
          if (this.mAlternateBackgroundColor)
            BlueStacksUIBinding.BindColor((DependencyObject) mScriptControl, System.Windows.Controls.Control.BackgroundProperty, "DarkBandingColor");
          else
            BlueStacksUIBinding.BindColor((DependencyObject) mScriptControl, System.Windows.Controls.Control.BackgroundProperty, "LightBandingColor");
          this.mAlternateBackgroundColor = !this.mAlternateBackgroundColor;
          this.mScriptsStackPanel.Children.Add((UIElement) mScriptControl);
        }
      }
    }

    private SingleMacroControl GetControlFromTag(string tag)
    {
      foreach (SingleMacroControl child in this.mScriptsStackPanel.Children)
      {
        if ((string) child.Tag == tag)
          return child;
      }
      return (SingleMacroControl) null;
    }

    private void OpenScriptFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
        return;
      using (Process process = new Process())
      {
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
        process.Start();
      }
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
    }

    private void mStartMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.mCommonHandler.StartMacroRecording();
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "new_macro_record", (string) null, RecordingTypes.SingleRecording.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    private void mStopMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.mCommonHandler.StopMacroRecording();
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", (string) null, RecordingTypes.SingleRecording.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    internal void PerformStopMacroAfterSave()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.mTopBar.HideRecordingIcons();
        this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
        this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
        this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
        this.ParentWindow.mIsMacroRecorderActive = false;
      }));
    }

    internal void SaveOperation(string events)
    {
      try
      {
        if (!string.Equals(events, "[]", StringComparison.InvariantCulture))
        {
          string recordingsFolderPath = RegistryStrings.MacroRecordingsFolderPath;
          this.SaveMacroRecord(new MacroRecording()
          {
            TimeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture),
            Name = CommonHandlers.GetMacroName("Macro"),
            Events = JsonConvert.DeserializeObject<List<MacroEvents>>(events, Utils.GetSerializerSettings())
          });
        }
        else
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_OPERATION_MESSAGE", ""), 4.0, true);
        this.PerformStopMacroAfterSave();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SaveOperations. Exception: " + ex.ToString());
      }
    }

    internal void SaveMacroRecord(MacroRecording record)
    {
      CommonHandlers.SaveMacroJson(record, record.Name + ".json");
      MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>) record);
      MacroGraph.LinkMacroChilds(record);
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.mIsMacroRecorderActive = false;
        foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
        {
          if (dictWindow.Value.MacroRecorderWindow != null)
          {
            new SingleMacroControl(dictWindow.Value, record, this).Tag = (object) record.Name;
            dictWindow.Value.MacroRecorderWindow.mNoScriptsGrid.Visibility = Visibility.Collapsed;
            this.mExport.IsEnabled = true;
            this.mExport.Opacity = 1.0;
            if (!dictWindow.Value.mIsScriptsPresent)
              dictWindow.Value.mIsScriptsPresent = true;
            dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
            dictWindow.Value.MacroRecorderWindow.Init();
            dictWindow.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
            int index = dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Count - 1;
            SingleMacroControl child = dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel.Children[index] as SingleMacroControl;
            BlueStacksUIBinding.BindColor((DependencyObject) child.mGrid, System.Windows.Controls.Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
            BlueStacksUIBinding.BindColor((DependencyObject) child.mScriptName, TextBlock.ForegroundProperty, "WhiteMouseOutBorderBackground");
            BlueStacksUIBinding.BindColor((DependencyObject) child.mMacroShortcutTextBox, TextBlock.ForegroundProperty, "DualTextBlockForeground");
          }
        }
      }));
    }

    private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    internal void ToggleUI(bool isRecording)
    {
      if (isRecording)
      {
        this.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
        this.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
        this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
        this.mScriptsGrid.Visibility = Visibility.Visible;
      }
      else
      {
        this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
        this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
        if (this.ParentWindow.mIsScriptsPresent)
        {
          this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
          this.mScriptsGrid.Visibility = Visibility.Visible;
        }
        else
        {
          this.mNoScriptsGrid.Visibility = Visibility.Visible;
          this.mScriptsGrid.Visibility = Visibility.Collapsed;
        }
      }
    }

    private void ShowLoadingGrid(bool isShow)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isShow)
          this.mLoadingGrid.Visibility = Visibility.Visible;
        else
          this.mLoadingGrid.Visibility = Visibility.Collapsed;
      }));
    }

    private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_export", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      if (this.ParentWindow.mIsScriptsPresent)
      {
        this.mOverlayGrid.Visibility = Visibility.Visible;
        if (this.mExportMacroWindow != null)
          return;
        ExportMacroWindow exportMacroWindow = new ExportMacroWindow(this, this.ParentWindow);
        exportMacroWindow.Owner = (Window) this;
        this.mExportMacroWindow = exportMacroWindow;
        this.mExportMacroWindow.Init();
        this.mExportMacroWindow.ShowDialog();
      }
      else
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, true);
    }

    private void MergeMacroBtn_Click(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_icon", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      if (this.ParentWindow.mIsScriptsPresent)
      {
        this.mOverlayGrid.Visibility = Visibility.Visible;
        if (this.mMergeMacroWindow != null)
          return;
        MergeMacroWindow mergeMacroWindow = new MergeMacroWindow(this, this.ParentWindow);
        mergeMacroWindow.Owner = (Window) this.ParentWindow;
        this.mMergeMacroWindow = mergeMacroWindow;
        this.mMergeMacroWindow.Init((MacroRecording) null, (SingleMacroControl) null);
        this.mMergeMacroWindow.Show();
      }
      else
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, true);
    }

    private void ImportBtn_Click(object sender, MouseButtonEventArgs e1)
    {
      try
      {
        if (this.ParentWindow.mIsMacroPlaying)
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_IMPORT_MACRO_WARNING", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_WARNING", "");
          customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_STOP_IMPORT", ""), (EventHandler) ((o, e2) =>
          {
            this.ParentWindow.mTopBar.mMacroPlayControl.StopMacro();
            this.ImportMacro();
          }), (string) null, false, (object) null, true);
          customMessageWindow.Owner = (Window) this;
          customMessageWindow.ShowDialog();
        }
        else
          this.ImportMacro();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Importing file. err: " + ex.ToString());
        this.ShowLoadingGrid(false);
      }
    }

    private void ImportMacro()
    {
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_import", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.Multiselect = true;
      openFileDialog1.Filter = "Json files (*.json)|*.json";
      using (OpenFileDialog openFileDialog2 = openFileDialog1)
      {
        if (openFileDialog2.ShowDialog() != DialogResult.OK || openFileDialog2.FileNames.Length == 0)
          return;
        if (string.Equals(Path.GetDirectoryName(openFileDialog2.FileNames[0]), RegistryStrings.MacroRecordingsFolderPath, StringComparison.InvariantCultureIgnoreCase))
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = (Window) this;
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SAME_MACRO_EXISTS", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
        else
          this.RunImportMacroScriptBacgroundWorker(((IEnumerable<string>) openFileDialog2.FileNames).ToList<string>());
      }
    }

    internal void RunImportMacroScriptBacgroundWorker(List<string> fileNames)
    {
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(this.BgImport_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BgImport_RunWorkerCompleted);
        this.ShowLoadingGrid(true);
        backgroundWorker.RunWorkerAsync((object) fileNames);
      }
    }

    private void BgImport_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        e.Result = (object) this.CopyMacroScriptIfFileFormatSupported(e.Argument as List<string>);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Importing file. err: " + ex.ToString());
        e.Result = (object) true;
      }
    }

    private void BgImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.ValidateReturnCode((int) e.Result);
      if ((int) e.Result != 0)
        return;
      CommonHandlers.RefreshAllMacroWindowWithScroll();
    }

    internal void ValidateReturnCode(int retCode)
    {
      this.ShowLoadingGrid(false);
      switch (retCode)
      {
        case 0:
          if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
            Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
          this.ShowMacroImportSuccessPopup();
          break;
        case 1:
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_IMPORTING_CANCELLED", ""), 4.0, true);
          break;
        case 2:
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_FILE_FORMAT_NOT_SUPPORTED", ""), 4.0, true);
          break;
        case 3:
          this.ShowMacroImportWizard();
          if (this.mNewlyAddedMacrosList.Count <= 0)
            break;
          CommonHandlers.RefreshAllMacroWindowWithScroll();
          this.ShowMacroImportSuccessPopup();
          break;
      }
    }

    internal int CopyMacroScriptIfFileFormatSupported(List<string> selectedFileNames)
    {
      try
      {
        this.mRenamingMacrosList.Clear();
        bool isShowRenameWizard = false;
        List<string> stringList = new List<string>();
        List<MacroRecording> macroRecordingList = new List<MacroRecording>();
        foreach (string selectedFileName in selectedFileNames)
        {
          MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(selectedFileName), Utils.GetSerializerSettings());
          if (macroRecording == null || string.IsNullOrEmpty(macroRecording.Name) || string.IsNullOrEmpty(macroRecording.TimeCreated) || macroRecording.RecordingType == RecordingTypes.MultiRecording && (macroRecording.SourceRecordings == null || !macroRecording.SourceRecordings.Any<string>()))
            stringList.Add(Path.GetFileNameWithoutExtension(selectedFileName));
          else
            macroRecordingList.Add(macroRecording);
        }
        if (macroRecordingList.Any<MacroRecording>((Func<MacroRecording, bool>) (x => x.RecordingType == RecordingTypes.MultiRecording)))
        {
          this.AskUserHowToImportMultiMacro();
          if (!this.mImportMultiMacroAsUnified.HasValue)
            return 1;
        }
        int num = this.ImportMacroRecordings(macroRecordingList, ref isShowRenameWizard);
        if (num != 0)
          return num;
        if (stringList.Count > 0)
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_INVALID_FILES_LIST", ""), (object) string.Join(", ", stringList.ToArray())), 4.0, true);
          if (macroRecordingList.Count <= 0)
            return 4;
        }
        return isShowRenameWizard ? 3 : 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Wrong file format wont import. err:" + ex.ToString());
        return 2;
      }
    }

    internal int ImportMacroRecordings(
      List<MacroRecording> recordingsToImport,
      ref bool isShowRenameWizard)
    {
      try
      {
        this.mNewlyAddedMacrosList.Clear();
        foreach (MacroRecording record in recordingsToImport)
        {
          record.Shortcut = string.Empty;
          record.PlayOnStart = false;
          if (MacroRecorderWindow.CheckIfDuplicateMacroInImport(record.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
          {
            isShowRenameWizard = true;
            this.mRenamingMacrosList.Add(record);
          }
          if (record.RecordingType == RecordingTypes.MultiRecording)
          {
            bool? multiMacroAsUnified = this.mImportMultiMacroAsUnified;
            if (0 == (multiMacroAsUnified.GetValueOrDefault() ? 1 : 0) & multiMacroAsUnified.HasValue && !this.mRenamingMacrosList.Contains(record))
            {
              List<string> stringList = new List<string>();
              if (MacroRecorderWindow.CheckIfDuplicateMacroInImport(record.Name, record.MergedMacroConfigurations.SelectMany<MergedMacroConfiguration, string>((Func<MergedMacroConfiguration, IEnumerable<string>>) (macro => (IEnumerable<string>) macro.MacrosToRun))))
              {
                isShowRenameWizard = true;
                this.mRenamingMacrosList.AddIfNotContain<MacroRecording>(record);
              }
            }
          }
          if (!this.mRenamingMacrosList.Contains(record))
          {
            record.Name = record.Name.Trim();
            if (record.RecordingType == RecordingTypes.SingleRecording)
            {
              MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>) record);
              this.mNewlyAddedMacrosList.Add(record);
              CommonHandlers.SaveMacroJson(record, record.Name + ".json");
            }
            else
              this.ImportMultiMacro(record, this.mImportMultiMacroAsUnified.Value, this.mNewlyAddedMacrosList, (Dictionary<string, string>) null);
          }
        }
        foreach (MacroRecording newlyAddedMacros in this.mNewlyAddedMacrosList)
          MacroGraph.LinkMacroChilds(newlyAddedMacros);
      }
      catch
      {
        throw;
      }
      return 0;
    }

    private void AskUserHowToImportMultiMacro()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_IMPORT_DEPENDENT_MACRO", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_IMPORT_ALL_MACROS", (EventHandler) ((o, evt) =>
        {
          this.mImportMultiMacroAsUnified = new bool?(false);
          ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_all", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_IMPORT_UNIFIED", (EventHandler) ((o, evt) =>
        {
          this.mImportMultiMacroAsUnified = new bool?(true);
          ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_unify", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        }), (string) null, false, (object) null, true);
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_IMPORT_DEPENDENT_MACRO_UNIFIED", "");
        customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
        customMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) =>
        {
          ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_cancel", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
          this.mImportMultiMacroAsUnified = new bool?();
        }), (object) null);
        customMessageWindow.Owner = (Window) this;
        customMessageWindow.ShowDialog();
      }));
    }

    private List<MacroEvents> GetFlattenedEventsFromMultiRecording(
      MacroRecording srcRecording,
      long initialTime,
      out long elapsedTime,
      bool isExternalMacro = false)
    {
      List<MacroEvents> macroEventsList = new List<MacroEvents>();
      elapsedTime = initialTime;
      List<MacroRecording> list = MacroGraph.Instance.Vertices.Cast<MacroRecording>().ToList<MacroRecording>();
      if (isExternalMacro)
      {
        list.Clear();
        foreach (string sourceRecording in srcRecording.SourceRecordings)
        {
          MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
          list.Add(macroRecording);
        }
      }
      try
      {
        foreach (MergedMacroConfiguration macroConfiguration in (Collection<MergedMacroConfiguration>) srcRecording.MergedMacroConfigurations)
        {
          for (int index = 0; index < macroConfiguration.LoopCount; ++index)
          {
            foreach (string str in (Collection<string>) macroConfiguration.MacrosToRun)
            {
              string gMacro = str;
              MacroRecording srcRecording1 = list.Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(gMacro, macro.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
              if (srcRecording1.RecordingType == RecordingTypes.SingleRecording)
                macroEventsList.AddRange((IEnumerable<MacroEvents>) MacroRecorderWindow.GetRecordingEventsFromSourceRecording(srcRecording1, macroConfiguration.Acceleration, elapsedTime, ref elapsedTime));
              else
                macroEventsList.AddRange((IEnumerable<MacroEvents>) this.GetFlattenedEventsFromMultiRecording(srcRecording1, elapsedTime, out elapsedTime, false));
              elapsedTime += (long) (macroConfiguration.LoopInterval * 1000);
            }
            elapsedTime += (long) (macroConfiguration.DelayNextScript * 1000);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't get flattened events. Ex: {0}", (object) ex);
      }
      return macroEventsList;
    }

    internal void FlattenRecording(MacroRecording srcRecording, bool isExternalMacro = false)
    {
      Logger.Info("Will attempt to flatten {0}", (object) srcRecording.Name);
      srcRecording.Events = this.GetFlattenedEventsFromMultiRecording(srcRecording, 0L, out long _, isExternalMacro);
      srcRecording.SourceRecordings = (List<string>) null;
      srcRecording.MergedMacroConfigurations = (ObservableCollection<MergedMacroConfiguration>) null;
    }

    private static MacroRecording GetFixedMultiMacroSourceRecording(
      MacroRecording record,
      string baseChildRecordingName)
    {
      try
      {
        MacroRecording macroRecording = record.DeepCopy<MacroRecording>();
        macroRecording.Name = MacroRecorderWindow.GetDependentRecordingName(baseChildRecordingName, macroRecording.Name);
        macroRecording.MergedMacroConfigurations.Clear();
        foreach (MergedMacroConfiguration macroConfiguration1 in (Collection<MergedMacroConfiguration>) record.MergedMacroConfigurations)
        {
          MergedMacroConfiguration macroConfiguration2 = macroConfiguration1.DeepCopy<MergedMacroConfiguration>();
          macroConfiguration2.MacrosToRun.Clear();
          foreach (string dependentMacroName in (Collection<string>) macroConfiguration1.MacrosToRun)
            macroConfiguration2.MacrosToRun.Add(MacroRecorderWindow.GetDependentRecordingName(baseChildRecordingName, dependentMacroName));
          macroRecording.MergedMacroConfigurations.Add(macroConfiguration2);
        }
        record = macroRecording;
        record.SourceRecordings = (List<string>) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Some error occured while fixing dependent source multi macro: Ex: {0}", (object) ex);
      }
      return record;
    }

    internal void ImportMultiMacro(
      MacroRecording record,
      bool flatten,
      List<MacroRecording> newlyAddedMacro,
      Dictionary<string, string> dependentMacroNewNamesDict = null)
    {
      if (flatten)
      {
        this.FlattenRecording(record, true);
        if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
          record.Name = dependentMacroNewNamesDict[record.Name];
      }
      else
      {
        try
        {
          MacroRecording macroRecording = record.DeepCopy<MacroRecording>();
          string name1 = record.Name;
          if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
          {
            name1 = dependentMacroNewNamesDict[record.Name];
            macroRecording.Name = name1;
          }
          foreach (string sourceRecording in record.SourceRecordings)
          {
            MacroRecording record1 = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
            if (record1.RecordingType == RecordingTypes.MultiRecording)
            {
              record1 = MacroRecorderWindow.GetFixedMultiMacroSourceRecording(record1, name1);
            }
            else
            {
              string name2 = record1.Name;
              string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(name1, name2);
              if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(name2))
                dependentRecordingName = dependentMacroNewNamesDict[name2];
              record1.Name = dependentRecordingName;
            }
            MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>) record1);
            newlyAddedMacro.Add(record1);
            CommonHandlers.SaveMacroJson(record1, record1.Name + ".json");
          }
          macroRecording.MergedMacroConfigurations.Clear();
          foreach (MergedMacroConfiguration macroConfiguration1 in (Collection<MergedMacroConfiguration>) record.MergedMacroConfigurations)
          {
            MergedMacroConfiguration macroConfiguration2 = macroConfiguration1.DeepCopy<MergedMacroConfiguration>();
            macroConfiguration2.MacrosToRun.Clear();
            ObservableCollection<string> observableCollection = new ObservableCollection<string>();
            foreach (string index in (Collection<string>) macroConfiguration1.MacrosToRun)
            {
              string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(name1, index);
              if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(index))
                dependentRecordingName = dependentMacroNewNamesDict[index];
              macroConfiguration2.MacrosToRun.Add(dependentRecordingName);
            }
            macroRecording.MergedMacroConfigurations.Add(macroConfiguration2);
          }
          record = macroRecording;
        }
        catch (Exception ex)
        {
          Logger.Error("Some error occured: Ex: {0}", (object) ex);
        }
        record.SourceRecordings = (List<string>) null;
      }
      MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>) record);
      newlyAddedMacro.Add(record);
      CommonHandlers.SaveMacroJson(record, record.Name + ".json");
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
      foreach (MacroEvents other in srcRecording.Events)
      {
        MacroEvents macroEvents = other.DeepCopy<MacroEvents>();
        macroEvents.Timestamp = (long) Math.Floor((double) other.Timestamp / acceleration);
        macroEvents.Timestamp += initialTime;
        elapsedTime = macroEvents.Timestamp;
        macroEventsList.Add(macroEvents);
      }
      return macroEventsList;
    }

    private static bool CheckIfDuplicateMacroInImport(
      string origMacroName,
      IEnumerable<string> lsMacros)
    {
      foreach (string lsMacro in lsMacros)
      {
        if (MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name.ToLower(CultureInfo.InvariantCulture))).Contains<string>(MacroRecorderWindow.GetDependentRecordingName(lsMacro, origMacroName).ToLower(CultureInfo.InvariantCulture)))
          return true;
      }
      return false;
    }

    internal static string GetDependentRecordingName(
      string originalMacroName,
      string dependentMacroName)
    {
      return originalMacroName + "-" + dependentMacroName;
    }

    private static bool CheckIfDuplicateMacroInImport(string macroName)
    {
      return MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name.ToLower(CultureInfo.InvariantCulture))).Contains<string>(macroName.ToLower(CultureInfo.InvariantCulture));
    }

    private void ShowMacroImportWizard()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mOverlayGrid.Visibility = Visibility.Visible;
        if (this.mImportMacroWindow != null)
          return;
        this.mImportMacroWindow = new ImportMacroWindow(this, this.ParentWindow)
        {
          Owner = (Window) this
        };
        this.mImportMacroWindow.Init();
        this.mImportMacroWindow.ShowDialog();
      }));
    }

    private void MImportBtn_Click(object sender, RoutedEventArgs e)
    {
      CommonHandlers.RefreshAllMacroWindowWithScroll();
    }

    private void mBGMacroPlaybackWorker_DoWork(BackgroundWorker bg, MacroRecording record)
    {
      if (!File.Exists(CommonHandlers.GetCompleteMacroRecordingPath(record.Name)) || this.ParentWindow.mIsMacroPlaying)
        return;
      Logger.Debug("Macro Playback started");
      this.ParentWindow.mIsMacroPlaying = true;
      this.ParentWindow.mFrontendHandler.SendFrontendRequest("initMacroPlayback", new Dictionary<string, string>()
      {
        {
          "scriptFilePath",
          CommonHandlers.GetCompleteMacroRecordingPath(record.Name)
        }
      });
      switch (record.LoopType)
      {
        case OperationsLoopType.TillLoopNumber:
          this.HandleMacroPlaybackTillLoopNumber(bg, record);
          break;
        case OperationsLoopType.TillTime:
          this.HandleMacroPlaybackTillTime(bg, record);
          break;
        case OperationsLoopType.UntilStopped:
          this.HandleMacroPlaybackUntillStopped(bg, record);
          break;
      }
    }

    internal void RunMacroOperation(MacroRecording record)
    {
      BackgroundWorker bg = new BackgroundWorker()
      {
        WorkerSupportsCancellation = true
      };
      bg.DoWork += (DoWorkEventHandler) ((obj, e) => this.mBGMacroPlaybackWorker_DoWork(bg, record));
      this.mBGMacroPlaybackWorker = bg;
      bg.RunWorkerAsync();
    }

    private void HandleMacroPlaybackUntillStopped(BackgroundWorker bg, MacroRecording record)
    {
      try
      {
        EventWaitHandle eventWaitHandle = (EventWaitHandle) null;
        string playbackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
        this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
        int i = 1;
        this.UpdateMacroPlayBackUI(i, record);
        while (this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
        {
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", (Dictionary<string, string>) null);
          if (eventWaitHandle == null)
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, playbackEventName);
          this.UpdateMacroPlayBackUI(i, record);
          ++i;
          eventWaitHandle.WaitOne();
          Thread.Sleep(record.LoopInterval * 1000);
        }
        eventWaitHandle.Close();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in macroplaybackuntil stopped. err:" + ex.ToString());
      }
    }

    private void HandleMacroPlaybackTillTime(BackgroundWorker bg, MacroRecording record)
    {
      try
      {
        if (record.LoopTime > 0)
        {
          EventWaitHandle eventWaitHandle = (EventWaitHandle) null;
          string playbackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
          this.mMacroLoopTimer = new System.Timers.Timer((double) record.LoopTime)
          {
            Interval = (double) (record.LoopTime * 1000)
          };
          this.mMacroLoopTimer.Elapsed += (ElapsedEventHandler) ((sender, e) => this.MacroLoopTimer_Elapsed(record.Name));
          DateTime now = DateTime.Now;
          TimeSpan timeSpan = DateTime.Now - now;
          this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
          this.mMacroLoopTimer.Enabled = true;
          this.UpdateMacroPlayBackUI(1, record);
          for (; timeSpan.TotalSeconds < (double) record.LoopTime && this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending; timeSpan = DateTime.Now - now)
          {
            this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", (Dictionary<string, string>) null);
            if (eventWaitHandle == null)
              eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, playbackEventName);
            eventWaitHandle.WaitOne();
            Thread.Sleep(record.LoopInterval * 1000);
          }
          eventWaitHandle.Close();
        }
        else
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_TIMER_SET", ""), 4.0, true);
          Logger.Debug("Macro timer set to ZERO");
          this.SendStopMacroEventsAndUpdateUi(record.Name);
        }
        if (this.mMacroLoopTimer == null)
          return;
        this.mMacroLoopTimer.Enabled = false;
        this.mMacroLoopTimer = (System.Timers.Timer) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in MacroPlaybacktillTime. err:" + ex.ToString());
      }
    }

    private void MacroLoopTimer_Elapsed(string fileName)
    {
      Logger.Debug("Macro timer finished");
      this.SendStopMacroEventsAndUpdateUi(fileName);
    }

    private void SendStopMacroEventsAndUpdateUi(string fileName)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.mCommonHandler.StopMacroScriptHandling();
        if (FeatureManager.Instance.IsCustomUIForNCSoft)
          this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
        else
          this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
      }));
    }

    private void HandleMacroPlaybackTillLoopNumber(BackgroundWorker bg, MacroRecording record)
    {
      try
      {
        string playbackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
        if (record.LoopNumber >= 1)
        {
          this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
        }
        else
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_LOOP_ITERATION_SET", ""), 4.0, true);
          Logger.Debug("Macro loop iterations set to ZERO");
        }
        EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, playbackEventName);
        for (int i = 1; i <= record.LoopNumber && this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending; ++i)
        {
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", (Dictionary<string, string>) null);
          this.UpdateMacroPlayBackUI(i, record);
          eventWaitHandle.WaitOne();
          if (i != record.LoopNumber)
            Thread.Sleep(record.LoopInterval * 1000);
        }
        eventWaitHandle.Close();
        if (bg.CancellationPending || !this.ParentWindow.mIsMacroPlaying)
          return;
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          this.ParentWindow.mCommonHandler.StopMacroPlaybackOperation();
          if (FeatureManager.Instance.IsCustomUIForNCSoft)
            this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
          else
            this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception err: " + ex.ToString());
      }
    }

    private void UpdateMacroPlayBackUI(int i, MacroRecording record)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (record.LoopType == OperationsLoopType.TillLoopNumber)
        {
          if (FeatureManager.Instance.IsCustomUIForNCSoft)
          {
            this.ParentWindow.mNCTopBar.mMacroPlayControl.mTimerDisplay.Visibility = Visibility.Collapsed;
            this.ParentWindow.mNCTopBar.mMacroPlayControl.mRunningIterations.Visibility = Visibility.Visible;
            this.ParentWindow.mNCTopBar.mMacroPlayControl.IncreaseIteration(i);
          }
          else
          {
            this.ParentWindow.mTopBar.mMacroPlayControl.mTimerDisplay.Visibility = Visibility.Collapsed;
            this.ParentWindow.mTopBar.mMacroPlayControl.mRunningIterations.Visibility = Visibility.Visible;
            this.ParentWindow.mTopBar.mMacroPlayControl.IncreaseIteration(i);
          }
        }
        else if (record.LoopType == OperationsLoopType.TillTime)
        {
          if (FeatureManager.Instance.IsCustomUIForNCSoft)
            this.ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
          else
            this.ParentWindow.mTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
        }
        else if (FeatureManager.Instance.IsCustomUIForNCSoft)
          this.ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
        else
          this.ParentWindow.mTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
      }));
    }

    private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
          Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
        using (Process process = new Process())
        {
          process.StartInfo.UseShellExecute = true;
          process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
          process.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error in Open folder err: " + ex.ToString());
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.mBGMacroPlaybackWorker?.Dispose();
      this.mMacroLoopTimer?.Dispose();
      this.disposedValue = true;
    }

    ~MacroRecorderWindow()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void OpenCommunityBtn_Click(object sender, RoutedEventArgs e)
    {
      if (string.Equals((sender as CustomButton).Name, "mOpenCommunityBtn", StringComparison.InvariantCultureIgnoreCase))
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Open_Community_Button", (string) null, (string) null, (string) null, (string) null, "Android");
      else
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Get_Macro_Button", (string) null, (string) null, (string) null, (string) null, "Android");
      this.OpenCommunityAndCloseMacroWindow(BlueStacksUIUtils.GetMacroCommunityUrl(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName));
    }

    internal void OpenCommunityAndCloseMacroWindow(string url)
    {
      this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(url ?? BlueStacksUIUtils.GetMacroCommunityUrl(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName), "STRING_MACRO_COMMUNITY", "community_big", true, "STRING_MACRO_COMMUNITY", false);
      this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
      this.ParentWindow.Focus();
    }

    internal void ShowMacroImportSuccessPopup()
    {
      this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_MACRO_IMPORT_SUCCESS", ""), 2.0, true);
    }

    private void CustomWindow_Closing(object sender, CancelEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      e.Cancel = true;
    }

    private void MacroTouchPointsTextBlock_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      try
      {
        string url = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=" + "macro_touch_points_help";
        Logger.Debug("Opening url: " + url);
        Utils.OpenUrl(url);
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macroRecorder_touchPoint_articleClicked", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
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
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrorecorderwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          ((Window) target).Closing += new CancelEventHandler(this.CustomWindow_Closing);
          break;
        case 2:
          this.mOperationRecorderBorder = (Border) target;
          break;
        case 3:
          this.mMaskBorder = (Border) target;
          break;
        case 4:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.Topbar_MouseDown);
          break;
        case 5:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 6:
          this.mMerge = (CustomPictureBox) target;
          this.mMerge.MouseLeftButtonUp += new MouseButtonEventHandler(this.MergeMacroBtn_Click);
          break;
        case 7:
          this.mImport = (CustomPictureBox) target;
          this.mImport.MouseLeftButtonUp += new MouseButtonEventHandler(this.ImportBtn_Click);
          break;
        case 8:
          this.mExport = (CustomPictureBox) target;
          this.mExport.MouseLeftButtonUp += new MouseButtonEventHandler(this.ExportBtn_Click);
          break;
        case 9:
          this.mOpenFolder = (CustomPictureBox) target;
          this.mOpenFolder.MouseLeftButtonUp += new MouseButtonEventHandler(this.OpenFolder_MouseLeftButtonUp);
          break;
        case 10:
          this.mStartMacroRecordingBtn = (CustomButton) target;
          this.mStartMacroRecordingBtn.Click += new RoutedEventHandler(this.mStartMacroRecordingBtn_Click);
          break;
        case 11:
          this.mStopMacroRecordingBtn = (CustomButton) target;
          this.mStopMacroRecordingBtn.Click += new RoutedEventHandler(this.mStopMacroRecordingBtn_Click);
          break;
        case 12:
          this.mGetMacroBtn = (CustomButton) target;
          this.mGetMacroBtn.Click += new RoutedEventHandler(this.OpenCommunityBtn_Click);
          break;
        case 13:
          this.mNoScriptsGrid = (Border) target;
          break;
        case 14:
          this.mScriptsGrid = (Grid) target;
          break;
        case 15:
          this.mScriptsListScrollbar = (ScrollViewer) target;
          break;
        case 16:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MacroTouchPointsTextBlock_PreviewMouseLeftButtonUp);
          break;
        case 17:
          this.mOpenCommunityBtn = (CustomButton) target;
          this.mOpenCommunityBtn.Click += new RoutedEventHandler(this.OpenCommunityBtn_Click);
          break;
        case 18:
          this.mLoadingGrid = (ProgressBar) target;
          break;
        case 19:
          this.mOverlayGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
