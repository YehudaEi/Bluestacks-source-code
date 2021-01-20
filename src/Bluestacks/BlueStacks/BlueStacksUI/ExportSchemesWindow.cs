// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ExportSchemesWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
  public class ExportSchemesWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();
    private KeymapCanvasWindow CanvasWindow;
    private MainWindow ParentWindow;
    internal StackPanel mSchemesStackPanel;
    internal int mNumberOfSchemesSelectedForExport;
    internal Border mMaskBorder;
    internal ScrollViewer mSchemesListScrollbar;
    internal CustomCheckbox mSelectAllBtn;
    internal CustomButton mExportBtn;
    internal ProgressBar mLoadingGrid;
    private bool _contentLoaded;

    public ExportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
    {
      this.InitializeComponent();
      this.CanvasWindow = window;
      this.ParentWindow = mainWindow;
      this.mSchemesStackPanel = this.mSchemesListScrollbar.Content as StackPanel;
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      this.Close();
      this.CanvasWindow.SidebarWindow.mExportSchemesWindow = (ExportSchemesWindow) null;
      this.CanvasWindow.SidebarWindow.mOverlayGrid.Visibility = Visibility.Hidden;
      this.CanvasWindow.SidebarWindow.Focus();
    }

    internal void Init()
    {
      try
      {
        this.mNumberOfSchemesSelectedForExport = 0;
        this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.BuiltIn)).ToList<IMControlScheme>().ForEach((System.Action<IMControlScheme>) (scheme => AddSchemeToExportCheckbox(scheme)));
        this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn)).ToList<IMControlScheme>().ForEach((System.Action<IMControlScheme>) (scheme =>
        {
          if (this.dict.Keys.Contains<string>(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
          {
            scheme.Name += " (Edited)";
            scheme.Name = KMManager.GetUniqueName(scheme.Name, (IEnumerable<string>) this.ParentWindow.OriginalLoadedConfig.ControlSchemesDict.Keys);
          }
          AddSchemeToExportCheckbox(scheme);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in export window init err: " + ex.ToString());
      }

      void AddSchemeToExportCheckbox(IMControlScheme scheme)
      {
        this.dict.Add(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim(), scheme);
        CustomCheckbox customCheckbox1 = new CustomCheckbox();
        customCheckbox1.Content = (object) scheme.Name;
        customCheckbox1.TextFontSize = 14.0;
        customCheckbox1.ImageMargin = new Thickness(2.0);
        customCheckbox1.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
        customCheckbox1.MaxHeight = 20.0;
        CustomCheckbox customCheckbox2 = customCheckbox1;
        customCheckbox2.Checked += new RoutedEventHandler(this.Box_Checked);
        customCheckbox2.Unchecked += new RoutedEventHandler(this.Box_Unchecked);
        this.mSchemesStackPanel.Children.Add((UIElement) customCheckbox2);
      }
    }

    private void Box_Unchecked(object sender, RoutedEventArgs e)
    {
      --this.mNumberOfSchemesSelectedForExport;
      if (this.mNumberOfSchemesSelectedForExport == this.mSchemesStackPanel.Children.Count - 1)
        this.mSelectAllBtn.IsChecked = new bool?(false);
      if (this.mNumberOfSchemesSelectedForExport != 0)
        return;
      this.mExportBtn.IsEnabled = false;
    }

    private void Box_Checked(object sender, RoutedEventArgs e)
    {
      ++this.mNumberOfSchemesSelectedForExport;
      if (this.mNumberOfSchemesSelectedForExport == this.mSchemesStackPanel.Children.Count)
        this.mSelectAllBtn.IsChecked = new bool?(true);
      if (this.mNumberOfSchemesSelectedForExport != 1)
        return;
      this.mExportBtn.IsEnabled = true;
    }

    private void ExportBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        int index = 0;
        List<IMControlScheme> imControlSchemeList = new List<IMControlScheme>();
        foreach (object child in this.mSchemesStackPanel.Children)
        {
          bool? isChecked = (child as CustomCheckbox).IsChecked;
          bool flag = true;
          if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
            imControlSchemeList.Add(this.dict.ElementAt<KeyValuePair<string, IMControlScheme>>(index).Value);
          ++index;
        }
        if (imControlSchemeList.Count != 0)
        {
          SaveFileDialog saveFileDialog1 = new SaveFileDialog();
          saveFileDialog1.AddExtension = true;
          saveFileDialog1.DefaultExt = ".cfg";
          saveFileDialog1.Filter = "Cfg files(*.cfg) | *.cfg";
          saveFileDialog1.FileName = this.ParentWindow.StaticComponents.mSelectedTabButton.AppName;
          using (SaveFileDialog saveFileDialog2 = saveFileDialog1)
          {
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
              using (BackgroundWorker backgroundWorker = new BackgroundWorker())
              {
                backgroundWorker.DoWork += new DoWorkEventHandler(this.BgExport_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BgExport_RunWorkerCompleted);
                this.ShowLoadingGrid(true);
                backgroundWorker.RunWorkerAsync((object) new List<object>()
                {
                  (object) saveFileDialog2.FileName,
                  (object) imControlSchemeList
                });
              }
            }
            else
              this.ToggleCheckBoxForExport();
          }
        }
        else
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""), 1.3, false);
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
      foreach (object child in this.mSchemesStackPanel.Children)
        (child as CustomCheckbox).IsChecked = new bool?(false);
    }

    private void BgExport_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        List<object> objectList = e.Argument as List<object>;
        string path = objectList[0] as string;
        List<IMControlScheme> imControlSchemeList = objectList[1] as List<IMControlScheme>;
        IMConfig imConfig = new IMConfig();
        imConfig.Strings = this.ParentWindow.OriginalLoadedConfig.Strings.DeepCopy<Dictionary<string, Dictionary<string, string>>>();
        imConfig.ControlSchemes = imControlSchemeList;
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Formatting = Formatting.Indented;
        string contents = JsonConvert.SerializeObject((object) imConfig, serializerSettings);
        File.WriteAllText(path, contents);
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_EXPORTED", ""), 1.3, false);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating exported file " + e.ToString());
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
        foreach (object child in this.mSchemesStackPanel.Children)
          (child as CustomCheckbox).IsChecked = new bool?(true);
      }
      else
      {
        foreach (object child in this.mSchemesStackPanel.Children)
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
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/exportschemeswindow.xaml", UriKind.Relative));
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
          this.mSchemesListScrollbar = (ScrollViewer) target;
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
