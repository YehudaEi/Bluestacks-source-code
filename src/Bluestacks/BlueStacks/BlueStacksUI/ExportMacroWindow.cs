// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ExportMacroWindow
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class ExportMacroWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<string, MacroRecording> mNameRecordingDict = new Dictionary<string, MacroRecording>();
    private MacroRecorderWindow mOperationWindow;
    private MainWindow ParentWindow;
    internal StackPanel mScriptsStackPanel;
    internal int mNumberOfFilesSelectedForExport;
    internal Border mMaskBorder;
    internal ScrollViewer mScriptsListScrollbar;
    internal CustomCheckbox mSelectAllBtn;
    internal CustomButton mExportBtn;
    internal ProgressBar mLoadingGrid;
    private bool _contentLoaded;

    public ExportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
    {
      this.InitializeComponent();
      this.mOperationWindow = window;
      this.ParentWindow = mainWindow;
      this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      this.Close();
      this.mOperationWindow.mExportMacroWindow = (ExportMacroWindow) null;
      this.mOperationWindow.mOverlayGrid.Visibility = Visibility.Hidden;
      this.mOperationWindow.Focus();
    }

    internal void Init()
    {
      try
      {
        foreach (MacroRecording vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.Instance.Vertices)
        {
          this.ParentWindow.mIsScriptsPresent = true;
          if (!this.mNameRecordingDict.ContainsKey(vertex.Name.ToLower(CultureInfo.InvariantCulture)))
          {
            this.mNameRecordingDict.Add(vertex.Name.ToLower(CultureInfo.InvariantCulture), vertex);
            CustomCheckbox customCheckbox1 = new CustomCheckbox();
            customCheckbox1.Content = (object) vertex.Name;
            customCheckbox1.TextFontSize = 12.0;
            customCheckbox1.Margin = new Thickness(0.0, 6.0, 0.0, 6.0);
            CustomCheckbox customCheckbox2 = customCheckbox1;
            customCheckbox2.Checked += new RoutedEventHandler(this.Box_Checked);
            customCheckbox2.Unchecked += new RoutedEventHandler(this.Box_Unchecked);
            customCheckbox2.ImageMargin = new Thickness(2.0);
            customCheckbox2.MaxHeight = 20.0;
            this.mScriptsStackPanel.Children.Add((UIElement) customCheckbox2);
          }
        }
        this.mNumberOfFilesSelectedForExport = 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in export window init err: " + ex.ToString());
      }
    }

    private void Box_Unchecked(object sender, RoutedEventArgs e)
    {
      --this.mNumberOfFilesSelectedForExport;
      if (this.mNumberOfFilesSelectedForExport == 0)
        this.mExportBtn.IsEnabled = false;
      if (this.mNumberOfFilesSelectedForExport != this.mScriptsStackPanel.Children.Count - 1)
        return;
      this.mSelectAllBtn.IsChecked = new bool?(false);
    }

    private void Box_Checked(object sender, RoutedEventArgs e)
    {
      ++this.mNumberOfFilesSelectedForExport;
      if (this.mNumberOfFilesSelectedForExport == 1)
        this.mExportBtn.IsEnabled = true;
      if (this.mNumberOfFilesSelectedForExport != this.mScriptsStackPanel.Children.Count)
        return;
      this.mSelectAllBtn.IsChecked = new bool?(true);
    }

    private void ExportBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        int index = 0;
        List<MacroRecording> macroRecordingList = new List<MacroRecording>();
        foreach (object child in this.mScriptsStackPanel.Children)
        {
          bool? isChecked = (child as CustomCheckbox).IsChecked;
          bool flag = true;
          if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
            macroRecordingList.Add(this.mNameRecordingDict.ElementAt<KeyValuePair<string, MacroRecording>>(index).Value);
          ++index;
        }
        if (macroRecordingList.Count != 0)
        {
          using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
          {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
              using (BackgroundWorker backgroundWorker = new BackgroundWorker())
              {
                backgroundWorker.DoWork += new DoWorkEventHandler(this.BgExport_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BgExport_RunWorkerCompleted);
                this.ShowLoadingGrid(true);
                backgroundWorker.RunWorkerAsync((object) new List<object>()
                {
                  (object) folderBrowserDialog.SelectedPath,
                  (object) macroRecordingList
                });
              }
            }
            else
              this.ToggleCheckBoxForExport();
          }
        }
        else
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_SELECTED", ""), 4.0, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Error while exporting script. err:" + e.ToString());
      }
    }

    private void BgExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.ShowLoadingGrid(false);
      this.ToggleCheckBoxForExport();
      this.CloseWindow();
    }

    private void ToggleCheckBoxForExport()
    {
      foreach (object child in this.mScriptsStackPanel.Children)
        (child as CustomCheckbox).IsChecked = new bool?(false);
    }

    private void BgExport_DoWork(object sender, DoWorkEventArgs e)
    {
      List<object> objectList = e.Argument as List<object>;
      string path1 = objectList[0] as string;
      foreach (MacroRecording macroRecording1 in objectList[1] as List<MacroRecording>)
      {
        string name = macroRecording1.Name;
        string str1 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
        string str2 = Path.Combine(path1, macroRecording1.Name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
        if (macroRecording1.RecordingType == RecordingTypes.SingleRecording)
        {
          File.Copy(str1, str2, true);
        }
        else
        {
          try
          {
            Logger.Info("Saving multi-macro");
            List<string> stringList = new List<string>();
            foreach (MacroRecording allChild in MacroGraph.Instance.GetAllChilds((BiDirectionalVertex<MacroRecording>) macroRecording1))
              stringList.Add(File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, allChild.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
            MacroRecording macroRecording2 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(str1), Utils.GetSerializerSettings());
            macroRecording2.SourceRecordings = stringList;
            JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            string contents = JsonConvert.SerializeObject((object) macroRecording2, serializerSettings);
            File.WriteAllText(str2, contents);
          }
          catch (Exception ex)
          {
            Logger.Error("Coulnd't take backup of script {0}, Ex: {1}", (object) name, (object) ex);
          }
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
          this.mLoadingGrid.Visibility = Visibility.Hidden;
      }));
    }

    private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
    {
      if (this.mSelectAllBtn.IsChecked.Value)
      {
        foreach (object child in this.mScriptsStackPanel.Children)
          (child as CustomCheckbox).IsChecked = new bool?(true);
      }
      else
      {
        foreach (object child in this.mScriptsStackPanel.Children)
          (child as CustomCheckbox).IsChecked = new bool?(false);
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/exportmacrowindow.xaml", UriKind.Relative));
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
          this.mMaskBorder = (Border) target;
          break;
        case 2:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 3:
          this.mScriptsListScrollbar = (ScrollViewer) target;
          break;
        case 4:
          this.mSelectAllBtn = (CustomCheckbox) target;
          this.mSelectAllBtn.Click += new RoutedEventHandler(this.SelectAllBtn_Click);
          break;
        case 5:
          this.mExportBtn = (CustomButton) target;
          this.mExportBtn.Click += new RoutedEventHandler(this.ExportBtn_Click);
          break;
        case 6:
          this.mLoadingGrid = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
