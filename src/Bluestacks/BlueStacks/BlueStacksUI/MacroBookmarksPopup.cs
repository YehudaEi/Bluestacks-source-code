// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroBookmarksPopup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class MacroBookmarksPopup : UserControl, IComponentConnector
  {
    internal MacroBookmarksPopup mMacroBookmarksPopup;
    internal Grid mGrid;
    internal StackPanel mMainStackPanel;
    private bool _contentLoaded;

    private MainWindow ParentWindow { get; set; }

    public MacroBookmarksPopup()
    {
      this.InitializeComponent();
      this.InitList();
    }

    public void SetParentWindowAndBindEvents(MainWindow window)
    {
      this.ParentWindow = window;
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.mCommonHandler.MacroBookmarkChangedEvent += new CommonHandlers.MacroBookmarkChanged(this.ParentWindow_MacroBookmarkChanged);
      this.ParentWindow.mCommonHandler.MacroSettingChangedEvent += new CommonHandlers.MacroSettingsChanged(this.ParentWindow_MacroSettingChangedEvent);
      this.ParentWindow.mCommonHandler.MacroDeletedEvent += new CommonHandlers.MacroDeleted(this.ParentWindow_MacroDeletedEvent);
    }

    private void ParentWindow_MacroDeletedEvent(string fileName)
    {
      Grid gridByTag = this.GetGridByTag(fileName);
      if (gridByTag == null)
        return;
      this.mMainStackPanel.Children.Remove((UIElement) gridByTag);
    }

    private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
    {
      try
      {
        this.mMainStackPanel.Children.Clear();
        this.InitList();
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't update name: {0}", (object) ex.Message);
      }
    }

    private void ParentWindow_MacroBookmarkChanged(string fileName, bool wasBookmarked)
    {
      if (wasBookmarked)
      {
        this.mMainStackPanel.Children.Add((UIElement) this.CreateGrid(fileName));
      }
      else
      {
        foreach (Grid child in this.mMainStackPanel.Children)
        {
          if ((string) child.Tag == fileName)
          {
            this.mMainStackPanel.Children.Remove((UIElement) child);
            break;
          }
        }
      }
    }

    private Grid GetGridByTag(string tag)
    {
      foreach (Grid child in this.mMainStackPanel.Children)
      {
        if ((string) child.Tag == tag)
          return child;
      }
      return (Grid) null;
    }

    private void InitList()
    {
      foreach (string bookmarkedScript in RegistryManager.Instance.BookmarkedScriptList)
      {
        if (!string.IsNullOrEmpty(bookmarkedScript))
          this.mMainStackPanel.Children.Add((UIElement) this.CreateGrid(bookmarkedScript));
      }
    }

    private void MMacroBookmarksPopup_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private Grid CreateGrid(string fileName)
    {
      Grid grid = new Grid();
      grid.MouseEnter += new MouseEventHandler(this.GridElement_MouseEnter);
      grid.MouseLeave += new MouseEventHandler(this.GridElement_MouseLeave);
      grid.MouseLeftButtonUp += new MouseButtonEventHandler(this.GridElement_MouseLeftButtonUp);
      grid.Background = (Brush) Brushes.Transparent;
      grid.Tag = (object) fileName;
      TextBlock textBlock1 = new TextBlock();
      textBlock1.FontSize = 12.0;
      textBlock1.TextTrimming = TextTrimming.CharacterEllipsis;
      textBlock1.Margin = new Thickness(10.0, 5.0, 10.0, 5.0);
      TextBlock textBlock2 = textBlock1;
      BlueStacksUIBinding.BindColor((DependencyObject) textBlock2, TextBlock.ForegroundProperty, "GuidanceTextColorForeground");
      string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, fileName);
      if (File.Exists(path))
      {
        try
        {
          MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), Utils.GetSerializerSettings());
          textBlock2.Text = macroRecording.Name;
          textBlock2.ToolTip = (object) macroRecording.Name;
        }
        catch
        {
        }
      }
      else
      {
        textBlock2.Text = fileName;
        textBlock2.ToolTip = (object) fileName;
      }
      grid.Children.Add((UIElement) textBlock2);
      return grid;
    }

    private void GridElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow.mIsMacroRecorderActive)
        return;
      string macroFileName = (string) (sender as Grid).Tag;
      MacroRecording record = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, macroFileName, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
      if (record == null)
      {
        this.mMainStackPanel.Children.Remove((UIElement) (sender as Grid));
      }
      else
      {
        try
        {
          if (!this.ParentWindow.mIsMacroPlaying)
          {
            this.ParentWindow.mCommonHandler.FullMacroScriptPlayHandler(record);
            ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "bookmark", record.RecordingType.ToString(), string.IsNullOrEmpty(record.MacroId) ? "local" : "community", (string) null, (string) null, "Android");
          }
          else
          {
            CustomMessageWindow customMessageWindow = new CustomMessageWindow();
            BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
            BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
            customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
            customMessageWindow.Owner = (Window) this.ParentWindow;
            customMessageWindow.ShowDialog();
          }
          if (this.ParentWindow.mSidebar == null)
            return;
          this.ParentWindow.mSidebar.mMacroButtonPopup.IsOpen = false;
          this.ParentWindow.mSidebar.ToggleSidebarVisibilityInFullscreen(false);
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show(ex.ToString());
        }
      }
    }

    private void GridElement_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void GridElement_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrobookmarkspopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMacroBookmarksPopup = (MacroBookmarksPopup) target;
          this.mMacroBookmarksPopup.Loaded += new RoutedEventHandler(this.MMacroBookmarksPopup_Loaded);
          break;
        case 2:
          this.mGrid = (Grid) target;
          break;
        case 3:
          this.mMainStackPanel = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
