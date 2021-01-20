// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SingleMacroControl
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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class SingleMacroControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal MacroRecorderWindow mMacroRecorderWindow;
    internal MacroRecording mRecording;
    internal MacroSettingsWindow mMacroSettingsWindow;
    private CustomMessageWindow mDeleteScriptMessageWindow;
    private bool mIsBookmarked;
    private string mLastScriptName;
    internal Grid mGrid;
    internal CustomPictureBox mBookmarkImg;
    internal Grid mScriptNameGrid;
    internal CustomTextBox mScriptName;
    internal TextBlock mUserNameTextblock;
    internal Hyperlink mUserNameHyperlink;
    internal CustomPictureBox mEditNameImg;
    internal TextBlock mTimestamp;
    internal TextBlock mPrefix;
    internal CustomTextBox mMacroShortcutTextBox;
    internal StackPanel mScriptPlayPanel;
    internal CustomPictureBox mAutorunImage;
    internal CustomPictureBox mCommunityMacroImage;
    internal CustomPictureBox mPlayScriptImg;
    internal CustomPictureBox mScriptSettingsImg;
    internal CustomPictureBox mMergeScriptSettingsImg;
    internal CustomPictureBox mDeleteScriptImg;
    internal StackPanel mScriptRunningPanel;
    internal CustomPictureBox mStopScriptImg;
    internal TextBlock mRunning;
    private bool _contentLoaded;

    public bool IsBookmarked
    {
      get
      {
        return this.mIsBookmarked;
      }
      set
      {
        this.mIsBookmarked = value;
        this.ToggleBookmarkIcon(value);
      }
    }

    private void ToggleBookmarkIcon(bool isBookmarked)
    {
      if (isBookmarked)
        this.mBookmarkImg.ImageName = "bookmarked";
      else
        this.mBookmarkImg.ImageName = "bookmark";
    }

    internal SingleMacroControl(
      MainWindow parentWindow,
      MacroRecording record,
      MacroRecorderWindow recorderWindow)
    {
      this.InitializeComponent();
      this.mRecording = record;
      this.ParentWindow = parentWindow;
      this.mMacroRecorderWindow = recorderWindow;
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mMacroShortcutTextBox, false);
      this.mTimestamp.Text = DateTime.ParseExact(this.mRecording.TimeCreated, "yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).ToString("yyyy.MM.dd HH.mm.ss", (IFormatProvider) CultureInfo.InvariantCulture);
      this.mScriptName.Text = this.mRecording.Name;
      this.mMacroShortcutTextBox.Text = IMAPKeys.GetStringForUI(this.mRecording.Shortcut);
      this.mScriptName.ToolTip = (object) this.mScriptName.Text;
      if (record.RecordingType == RecordingTypes.MultiRecording)
      {
        this.mScriptSettingsImg.Visibility = Visibility.Collapsed;
        this.mMergeScriptSettingsImg.Visibility = Visibility.Visible;
      }
      if (!string.IsNullOrEmpty(this.mRecording.Shortcut))
      {
        this.mMacroShortcutTextBox.Tag = (object) IMAPKeys.GetStringForFile(IMAPKeys.mDictKeys.FirstOrDefault<KeyValuePair<Key, string>>((Func<KeyValuePair<Key, string>, bool>) (x => x.Value == this.mRecording.Shortcut)).Key);
        MainWindow.sMacroMapping[this.mMacroShortcutTextBox.Tag.ToString()] = this.mScriptName.Text;
      }
      else
        this.mMacroShortcutTextBox.Tag = (object) "";
      this.IsBookmarked = BlueStacksUIUtils.CheckIfMacroScriptBookmarked(this.mRecording.Name);
      if (record.PlayOnStart)
        this.mAutorunImage.Visibility = Visibility.Visible;
      if (this.ParentWindow.mIsMacroPlaying && string.Equals(this.mRecording.Name, this.ParentWindow.mMacroPlaying, StringComparison.InvariantCulture))
        this.ToggleScriptPlayPauseUi(true);
      else
        this.ToggleScriptPlayPauseUi(false);
    }

    public void UpdateMacroRecordingObject(MacroRecording record)
    {
      this.mRecording = record;
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

    public bool AddScriptNameToBookmarkedScriptListIfNotPresent(string fileName)
    {
      if (((IEnumerable<string>) RegistryManager.Instance.BookmarkedScriptList).Contains<string>(this.mRecording.Name))
        return false;
      RegistryManager.Instance.BookmarkedScriptList = new List<string>((IEnumerable<string>) RegistryManager.Instance.BookmarkedScriptList)
      {
        fileName
      }.ToArray();
      return true;
    }

    private void UpdateMacroDeleteWindowSettings()
    {
      this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value;
      this.mDeleteScriptMessageWindow = (CustomMessageWindow) null;
    }

    private void DeleteMacroScript()
    {
      string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mRecording.Name + ".json");
      if (File.Exists(path))
        File.Delete(path);
      if (this.mRecording.Shortcut != null && MainWindow.sMacroMapping.ContainsKey(this.mRecording.Shortcut))
        MainWindow.sMacroMapping.Remove(this.mRecording.Shortcut);
      SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(this.mRecording.Name);
      MacroGraph.Instance.RemoveVertex((BiDirectionalVertex<MacroRecording>) MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>());
      if (this.ParentWindow.mAutoRunMacro != null && this.ParentWindow.mAutoRunMacro.Name.ToLower(CultureInfo.InvariantCulture).Trim() == this.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
        this.ParentWindow.mAutoRunMacro = (MacroRecording) null;
      CommonHandlers.RefreshAllMacroRecorderWindow();
      CommonHandlers.OnMacroDeleted(this.mRecording.Name);
    }

    private void SingleMacroControl_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.mGrid, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
      this.mEditNameImg.Visibility = Visibility.Visible;
    }

    private void SingleMacroControl_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mGrid.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.mEditNameImg.Visibility = Visibility.Hidden;
    }

    private void PauseScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private void StopScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ToggleScriptPlayPauseUi(false);
      this.ParentWindow.mCommonHandler.StopMacroScriptHandling();
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", (string) null, this.mRecording.RecordingType.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    internal void ToggleScriptPlayPauseUi(bool isScriptRunning)
    {
      if (isScriptRunning)
      {
        this.mScriptPlayPanel.Visibility = Visibility.Collapsed;
        this.mScriptRunningPanel.Visibility = Visibility.Visible;
      }
      else
      {
        this.mScriptPlayPanel.Visibility = Visibility.Visible;
        this.mScriptRunningPanel.Visibility = Visibility.Collapsed;
      }
    }

    private void ScriptSettingsImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Visible;
      MacroRecording macroRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => macro.Equals(this.mRecording))).FirstOrDefault<MacroRecording>();
      MacroRecording mRecording = this.mRecording;
      if ((mRecording != null ? (mRecording.RecordingType == RecordingTypes.MultiRecording ? 1 : 0) : 0) != 0)
      {
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_edit", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        if (this.mMacroRecorderWindow.mMergeMacroWindow == null)
        {
          MacroRecorderWindow macroRecorderWindow = this.mMacroRecorderWindow;
          MergeMacroWindow mergeMacroWindow = new MergeMacroWindow(this.mMacroRecorderWindow, this.ParentWindow);
          mergeMacroWindow.Owner = (Window) this.ParentWindow;
          macroRecorderWindow.mMergeMacroWindow = mergeMacroWindow;
        }
        this.mMacroRecorderWindow.mMergeMacroWindow.Init(macroRecording, this);
        this.mMacroRecorderWindow.mMergeMacroWindow.Show();
      }
      else
      {
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_settings", (string) null, this.mRecording.RecordingType.ToString(), (string) null, (string) null, (string) null, "Android");
        if (this.mMacroSettingsWindow == null || this.mMacroSettingsWindow.IsClosed)
          this.mMacroSettingsWindow = new MacroSettingsWindow(this.ParentWindow, macroRecording, this.mMacroRecorderWindow);
        this.mMacroSettingsWindow.ShowDialog();
      }
    }

    private void BookMarkScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.IsBookmarked)
      {
        this.IsBookmarked = false;
        SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(this.mRecording.Name);
        this.ParentWindow.mCommonHandler.OnMacroBookmarkChanged(this.mRecording.Name, false);
      }
      else if (RegistryManager.Instance.BookmarkedScriptList.Length < 5)
      {
        this.IsBookmarked = true;
        this.AddScriptNameToBookmarkedScriptListIfNotPresent(this.mRecording.Name);
        this.ParentWindow.mCommonHandler.OnMacroBookmarkChanged(this.mRecording.Name, true);
      }
      else
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_BOOKMARK_MACRO_WARNING", ""), 2.5, true);
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_bookmark", (string) null, this.mRecording.RecordingType.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    private void DeleteScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      MacroRecording currentRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
      if (currentRecording == null)
        return;
      if (currentRecording.Parents.Count > 0)
      {
        this.mDeleteScriptMessageWindow = new CustomMessageWindow();
        this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_DEPENDENT_MACRO", "");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(LocaleStrings.GetLocalizedString("STRING_MACRO_IN_USE_BY_OTHER_MACROS", ""));
        stringBuilder.Append(" ");
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_MACRO_LOSE_CONFIGURABILITY", ""), (object) this.mRecording.Name));
        this.mDeleteScriptMessageWindow.BodyTextBlock.Text = stringBuilder.ToString();
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DELETE_ANYWAY", ""), (EventHandler) ((o, evt) =>
        {
          for (int i = currentRecording.Parents.Count - 1; i >= 0; i--)
          {
            MacroRecording macroRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => macro.Equals((object) currentRecording.Parents[i]))).FirstOrDefault<MacroRecording>();
            this.mMacroRecorderWindow.FlattenRecording(currentRecording.Parents[i] as MacroRecording, false);
            CommonHandlers.SaveMacroJson(currentRecording.Parents[i] as MacroRecording, (currentRecording.Parents[i] as MacroRecording).Name + ".json");
            foreach (SingleMacroControl child in this.mMacroRecorderWindow.mScriptsStackPanel.Children)
            {
              if (child.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
                child.mScriptSettingsImg.ImageName = "macro_settings";
            }
            MacroGraph.Instance.DeLinkMacroChild((BiDirectionalVertex<MacroRecording>) (currentRecording.Parents[i] as MacroRecording));
          }
          this.DeleteMacroScript();
          CommonHandlers.RefreshAllMacroRecorderWindow();
        }), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_DONT_DELETE", ""), (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) => {}), (object) null);
        this.mDeleteScriptMessageWindow.Owner = (Window) this.ParentWindow;
        this.mDeleteScriptMessageWindow.ShowDialog();
      }
      else if (!this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup)
      {
        this.DeleteMacroScript();
      }
      else
      {
        this.mDeleteScriptMessageWindow = new CustomMessageWindow();
        this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_MACRO", "");
        this.mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCRIPT", "");
        this.mDeleteScriptMessageWindow.CheckBox.Content = (object) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
        this.mDeleteScriptMessageWindow.CheckBox.Visibility = Visibility.Visible;
        this.mDeleteScriptMessageWindow.CheckBox.IsChecked = new bool?(false);
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), new EventHandler(this.FlattenTargetMacrosAndDeleteSourceMacro), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) ((o, evt) => this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) => {}), (object) null);
        this.mDeleteScriptMessageWindow.Owner = (Window) this.ParentWindow;
        this.mDeleteScriptMessageWindow.ShowDialog();
      }
    }

    private void FlattenTargetMacrosAndDeleteSourceMacro(object sender, EventArgs e)
    {
      this.UpdateMacroDeleteWindowSettings();
      this.DeleteMacroScript();
    }

    private void PlayScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!this.ParentWindow.mIsMacroPlaying)
      {
        if (MacroGraph.CheckIfDependentMacrosAreAvailable(this.mRecording))
        {
          this.ToggleScriptPlayPauseUi(true);
          this.ParentWindow.mCommonHandler.PlayMacroScript(this.mRecording);
          ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "macro_popup", this.mRecording.RecordingType.ToString(), string.IsNullOrEmpty(this.mRecording.MacroId) ? "local" : "community", (string) null, (string) null, "Android");
          this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
        }
        else
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = (Window) this.mMacroRecorderWindow;
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ERROR_IN_MERGE_MACRO", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
      }
      else
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_STOP_THE_SCRIPT", ""), 4.0, true);
    }

    private void EditMacroName_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (string.Equals(this.mEditNameImg.ImageName, "edit_icon", StringComparison.InvariantCulture))
      {
        e.Handled = true;
        this.mScriptName.IsEnabled = true;
        this.mEditNameImg.ImageName = "macro_name_save";
        this.mScriptName.Width = this.mScriptNameGrid.ActualWidth - this.mEditNameImg.ActualWidth - this.mTimestamp.ActualWidth - this.mPrefix.ActualWidth - 30.0;
        this.mLastScriptName = this.mScriptName.Text;
        this.mScriptName.Focusable = true;
        this.mScriptName.IsReadOnly = false;
        this.mScriptName.Focus();
        BlueStacksUIBinding.Bind((Image) this.mEditNameImg, "STRING_SAVE");
        this.mScriptName.CaretIndex = this.mScriptName.Text.Length;
      }
      else
        this.PerformSaveMacroNameOperations();
    }

    private void PerformSaveMacroNameOperations()
    {
      this.mScriptName.IsEnabled = false;
      this.mScriptName.Focusable = false;
      this.mScriptName.IsReadOnly = true;
      this.mScriptName.BorderThickness = new Thickness(0.0);
      this.mEditNameImg.ImageName = "edit_icon";
      BlueStacksUIBinding.Bind((Image) this.mEditNameImg, "STRING_RENAME");
      this.SaveMacroName();
    }

    private void SaveMacroName()
    {
      if (!string.IsNullOrEmpty(this.mScriptName.Text.Trim()))
      {
        if (!MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name.ToLower(CultureInfo.InvariantCulture))).Contains<string>(this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim()) || this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == this.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
        {
          if (this.mScriptName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
          {
            this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""), (object) Environment.NewLine, (object) "\\ / : * ? \" < > |"), 4.0, true);
            this.mScriptName.Text = this.mLastScriptName;
          }
          else if (((IEnumerable<string>) Constants.ReservedFileNamesList).Contains<string>(this.mScriptName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
          {
            this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", ""), 4.0, true);
            this.mScriptName.Text = this.mLastScriptName;
          }
          else
            this.SaveScriptSettings();
        }
        else
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", ""), 4.0, true);
          this.mScriptName.Text = this.mLastScriptName;
        }
      }
      else
      {
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), 4.0, true);
        this.mScriptName.Text = this.mLastScriptName;
      }
    }

    private void SaveScriptSettings()
    {
      try
      {
        if (string.Equals(this.mRecording.Shortcut, this.mMacroShortcutTextBox.Tag.ToString(), StringComparison.InvariantCulture) && string.Equals(this.mRecording.Name.Trim(), this.mScriptName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
          return;
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Formatting = Formatting.Indented;
        string str1 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mRecording.Name + ".json");
        if (this.mRecording.Shortcut != this.mMacroShortcutTextBox.Tag.ToString())
        {
          if (!string.IsNullOrEmpty(this.mRecording.Shortcut) && MainWindow.sMacroMapping.ContainsKey(this.mRecording.Shortcut))
            MainWindow.sMacroMapping.Remove(this.mRecording.Shortcut);
          if (this.mMacroShortcutTextBox.Tag != null && !string.IsNullOrEmpty(this.mMacroShortcutTextBox.Tag.ToString()))
            MainWindow.sMacroMapping[this.mMacroShortcutTextBox.Tag.ToString()] = this.mScriptName.Text;
          if (this.mRecording.Shortcut != null && this.mMacroShortcutTextBox.Tag != null && !string.Equals(this.mRecording.Shortcut, this.mMacroShortcutTextBox.Tag.ToString(), StringComparison.InvariantCulture))
            ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_shortcutkey", (string) null, this.mRecording.RecordingType.ToString(), (string) null, (string) null, (string) null, "Android");
          if (this.mMacroShortcutTextBox.Tag != null)
            this.mRecording.Shortcut = this.mMacroShortcutTextBox.Tag.ToString();
          if (this.mRecording.PlayOnStart)
            this.ParentWindow.mAutoRunMacro = this.mRecording;
          string contents = JsonConvert.SerializeObject((object) this.mRecording, serializerSettings);
          File.WriteAllText(str1, contents);
        }
        if (!string.Equals(this.mRecording.Name.Trim(), this.mScriptName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
        {
          string oldMacroName = this.mRecording.Name;
          MacroRecording macroRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
          macroRecording.Name = this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim();
          this.mRecording.Name = this.mScriptName.Text.Trim();
          if (this.mRecording.PlayOnStart)
            this.ParentWindow.mAutoRunMacro = this.mRecording;
          string contents = JsonConvert.SerializeObject((object) this.mRecording, serializerSettings);
          File.WriteAllText(str1, contents);
          string destFileName = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mScriptName.Text.Trim() + ".json");
          File.Move(str1, destFileName);
          foreach (MacroRecording record in macroRecording.Parents.Cast<MacroRecording>())
          {
            foreach (MergedMacroConfiguration macroConfiguration in (Collection<MergedMacroConfiguration>) record.MergedMacroConfigurations)
            {
              List<string> stringList = new List<string>();
              foreach (string str2 in macroConfiguration.MacrosToRun.Select<string, string>((Func<string, string>) (macroToRun => !string.Equals(oldMacroName, macroToRun, StringComparison.CurrentCultureIgnoreCase) ? macroToRun : macroToRun.Replace(macroToRun, this.mRecording.Name))))
                stringList.Add(str2);
              macroConfiguration.MacrosToRun.Clear();
              foreach (string str2 in stringList)
                macroConfiguration.MacrosToRun.Add(str2);
            }
            CommonHandlers.SaveMacroJson(record, CommonHandlers.GetCompleteMacroRecordingPath(record.Name));
            CommonHandlers.OnMacroSettingChanged(record);
          }
          if (this.IsBookmarked)
          {
            SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(oldMacroName);
            this.AddScriptNameToBookmarkedScriptListIfNotPresent(this.mRecording.Name);
          }
        }
        CommonHandlers.OnMacroSettingChanged(this.mRecording);
        CommonHandlers.RefreshAllMacroRecorderWindow();
        CommonHandlers.ReloadMacroShortcutsForAllInstances();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in saving macro settings: " + ex.ToString());
      }
    }

    private void NoSelection_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mScriptName.SelectionLength = 0;
    }

    private bool Valid(Key key)
    {
      if (key == Key.Escape || key == Key.LeftAlt || (key == Key.RightAlt || key == Key.LeftCtrl) || (key == Key.RightCtrl || key == Key.RightShift || (key == Key.LeftShift || key == Key.Capital)) || (key == Key.Return || key == Key.Space || key == Key.Delete))
        return false;
      if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
      {
        string b = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} + {1} + {2}", (object) IMAPKeys.GetStringForFile(Key.LeftCtrl), (object) IMAPKeys.GetStringForFile(Key.LeftAlt), (object) IMAPKeys.GetStringForFile(key));
        foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
        {
          if (string.Equals(shortcutKeys.ShortcutKey, b, StringComparison.InvariantCulture))
          {
            this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, true);
            return false;
          }
        }
      }
      if (!MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForFile(key)) || !(MainWindow.sMacroMapping[IMAPKeys.GetStringForFile(key)] != this.mRecording.Name))
        return true;
      this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, true);
      return false;
    }

    private void MacroShortcutPreviewKeyDown(object sender, KeyEventArgs e)
    {
      this.mMacroShortcutTextBox.Text = "";
      this.mMacroShortcutTextBox.Tag = (object) "";
      Key key = e.Key == Key.System ? e.SystemKey : e.Key;
      if (IMAPKeys.mDictKeys.ContainsKey(key) && this.Valid(key))
      {
        this.mMacroShortcutTextBox.Text = IMAPKeys.GetStringForUI(key);
        this.mMacroShortcutTextBox.Tag = (object) IMAPKeys.GetStringForFile(key);
      }
      else
      {
        this.mMacroShortcutTextBox.Text = "";
        this.mMacroShortcutTextBox.Tag = (object) "";
      }
      e.Handled = true;
      this.SaveScriptSettings();
    }

    private void ScriptName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Return)
        return;
      this.PerformSaveMacroNameOperations();
    }

    private void ScriptName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.PerformSaveMacroNameOperations();
    }

    private void ScriptName_LostFocus(object sender, RoutedEventArgs e)
    {
      this.PerformSaveMacroNameOperations();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      this.mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(this.mRecording.AuthorPageUrl.ToString());
      e.Handled = true;
    }

    private void UserNameHyperlink_Loaded(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(this.mRecording.User))
        return;
      this.mUserNameHyperlink.Inlines.Clear();
      this.mUserNameHyperlink.Inlines.Add(this.mRecording.User);
      if (this.mRecording.AuthorPageUrl != (Uri) null && !string.IsNullOrEmpty(this.mRecording.AuthorPageUrl.ToString()))
        this.mUserNameHyperlink.NavigateUri = this.mRecording.AuthorPageUrl;
      this.mScriptName.FontSize = 13.0;
      this.mUserNameTextblock.Visibility = Visibility.Visible;
    }

    private void CommunityMacroPage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(this.mRecording.MacroPageUrl.ToString());
    }

    private void ScriptControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(this.mRecording.User))
        return;
      this.mCommunityMacroImage.Visibility = Visibility.Visible;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/singlemacrocontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.SingleMacroControl_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.SingleMacroControl_MouseLeave);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.ScriptControl_Loaded);
          break;
        case 2:
          this.mGrid = (Grid) target;
          break;
        case 3:
          this.mBookmarkImg = (CustomPictureBox) target;
          this.mBookmarkImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BookMarkScriptImg_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mScriptNameGrid = (Grid) target;
          break;
        case 5:
          this.mScriptName = (CustomTextBox) target;
          this.mScriptName.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.ScriptName_LostKeyboardFocus);
          this.mScriptName.LostFocus += new RoutedEventHandler(this.ScriptName_LostFocus);
          this.mScriptName.MouseLeftButtonUp += new MouseButtonEventHandler(this.NoSelection_MouseUp);
          this.mScriptName.KeyDown += new KeyEventHandler(this.ScriptName_KeyDown);
          break;
        case 6:
          this.mUserNameTextblock = (TextBlock) target;
          break;
        case 7:
          this.mUserNameHyperlink = (Hyperlink) target;
          this.mUserNameHyperlink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          this.mUserNameHyperlink.Loaded += new RoutedEventHandler(this.UserNameHyperlink_Loaded);
          break;
        case 8:
          this.mEditNameImg = (CustomPictureBox) target;
          this.mEditNameImg.MouseLeftButtonDown += new MouseButtonEventHandler(this.EditMacroName_MouseDown);
          break;
        case 9:
          this.mTimestamp = (TextBlock) target;
          break;
        case 10:
          this.mPrefix = (TextBlock) target;
          break;
        case 11:
          this.mMacroShortcutTextBox = (CustomTextBox) target;
          this.mMacroShortcutTextBox.PreviewKeyDown += new KeyEventHandler(this.MacroShortcutPreviewKeyDown);
          break;
        case 12:
          this.mScriptPlayPanel = (StackPanel) target;
          break;
        case 13:
          this.mAutorunImage = (CustomPictureBox) target;
          this.mAutorunImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BookMarkScriptImg_PreviewMouseLeftButtonUp);
          break;
        case 14:
          this.mCommunityMacroImage = (CustomPictureBox) target;
          this.mCommunityMacroImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CommunityMacroPage_PreviewMouseLeftButtonUp);
          break;
        case 15:
          this.mPlayScriptImg = (CustomPictureBox) target;
          this.mPlayScriptImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.PlayScriptImg_PreviewMouseLeftButtonUp);
          break;
        case 16:
          this.mScriptSettingsImg = (CustomPictureBox) target;
          this.mScriptSettingsImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScriptSettingsImg_PreviewMouseLeftButtonUp);
          break;
        case 17:
          this.mMergeScriptSettingsImg = (CustomPictureBox) target;
          this.mMergeScriptSettingsImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScriptSettingsImg_PreviewMouseLeftButtonUp);
          break;
        case 18:
          this.mDeleteScriptImg = (CustomPictureBox) target;
          this.mDeleteScriptImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.DeleteScriptImg_PreviewMouseLeftButtonUp);
          break;
        case 19:
          this.mScriptRunningPanel = (StackPanel) target;
          break;
        case 20:
          this.mStopScriptImg = (CustomPictureBox) target;
          this.mStopScriptImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopScriptImg_PreviewMouseLeftButtonUp);
          break;
        case 21:
          this.mRunning = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
