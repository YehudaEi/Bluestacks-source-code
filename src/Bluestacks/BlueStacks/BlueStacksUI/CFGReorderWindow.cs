// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CFGReorderWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class CFGReorderWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<string, IMConfig> mLoadedCFGDict = new Dictionary<string, IMConfig>();
    private Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>> mSchemeTreeMapping = new Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>>();
    private IMConfig mCurrentlySelectedCFG;
    private IMControlScheme mCurrentlySelectedScheme;
    private IList<IMControlScheme> mSchemesList;
    private const string NO_GUIDANCE = "NO_GUIDANCE";
    private const string CFG_MODIFIED_SUFFIX = "* (modified)";
    private static CFGReorderWindow mInstance;
    private Point _dragStartPoint;
    internal System.Windows.Controls.ListView mLoadedCFGsListView;
    internal StackPanel mCurrentlyLoadedStackPanel;
    internal TextBlock mCurrentlyLoadedCFGTextBlock;
    internal System.Windows.Controls.ListView mSchemesListView;
    internal System.Windows.Controls.TreeView mIMActionsTreeView;
    internal Grid mActionJsonGrid;
    internal System.Windows.Controls.TextBox mActionTextBox;
    internal System.Windows.Controls.Button mEditButton;
    internal System.Windows.Controls.Button mSaveButton;
    private bool _contentLoaded;

    public static CFGReorderWindow Instance
    {
      get
      {
        if (CFGReorderWindow.mInstance == null)
          CFGReorderWindow.mInstance = new CFGReorderWindow();
        return CFGReorderWindow.mInstance;
      }
    }

    public CFGReorderWindow()
    {
      this.InitializeComponent();
      this.Closing += new CancelEventHandler(this.CFGReorderWindow_Closing);
      this.Owner = (Window) BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName];
      this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void ClearState()
    {
      this.mCurrentlySelectedCFG = (IMConfig) null;
      this.mLoadedCFGDict.Clear();
      this.mLoadedCFGsListView.Items.Clear();
      this.mSchemeTreeMapping.Clear();
      this.ClearIMLists();
      this.mLoadedCFGsListView.Visibility = Visibility.Collapsed;
      this.mSchemesListView.Visibility = Visibility.Collapsed;
      this.mIMActionsTreeView.Visibility = Visibility.Collapsed;
    }

    private void ClearIMLists()
    {
      this.mSchemesList = (IList<IMControlScheme>) new ObservableCollection<IMControlScheme>();
      this.ClearIMActionsTree();
    }

    private void ClearIMActionsTree()
    {
      this.mIMActionsTreeView.Items.Clear();
    }

    private void CFGReorderWindow_Closing(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
      this.ClearState();
    }

    private void LoadCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ClearState();
      List<string> stringList = new List<string>();
      string path = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
      if (!Directory.Exists(path) || Directory.GetFileSystemEntries(path).Length == 0)
        path = RegistryStrings.InputMapperFolder;
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.Filter = "BlueStacks keyboard controls (*.cfg)|*.cfg";
      openFileDialog1.InitialDirectory = path;
      openFileDialog1.Multiselect = true;
      openFileDialog1.RestoreDirectory = true;
      using (OpenFileDialog openFileDialog2 = openFileDialog1)
      {
        if (openFileDialog2.ShowDialog() != DialogResult.OK)
          return;
        foreach (string fileName in openFileDialog2.FileNames)
        {
          if (!this.CheckValidCFGAndLoad(fileName))
            stringList.Add(Path.GetFileNameWithoutExtension(fileName));
        }
        if (stringList.Count > 0)
        {
          int num = (int) System.Windows.MessageBox.Show("The following CFG files could not be loaded.\n" + string.Join("\n", stringList.ToArray()));
        }
        if (this.mLoadedCFGDict.Count <= 0)
          return;
        this.InitCFGList();
        this.mLoadedCFGsListView.Visibility = Visibility.Visible;
        this.mLoadedCFGsListView.SelectedIndex = 0;
      }
    }

    private bool CheckValidCFGAndLoad(string filePath)
    {
      bool flag = false;
      try
      {
        IMConfig imConfig = JsonConvert.DeserializeObject<IMConfig>(File.ReadAllText(filePath), Utils.GetSerializerSettings());
        flag = true;
        this.mLoadedCFGDict.Add(filePath, imConfig);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to read cfg file... filepath: " + filePath + " Err : " + ex.Message);
      }
      return flag;
    }

    private List<IMAction> GetFinalListOfActions(Dictionary<string, List<IMAction>> dict)
    {
      List<IMAction> imActionList = new List<IMAction>();
      foreach (string key in dict.Keys)
      {
        foreach (IMAction imAction in dict[key])
        {
          imAction.GuidanceCategory = key;
          imActionList.Add(imAction);
        }
      }
      return imActionList;
    }

    private void SaveCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      foreach (System.Windows.Controls.ListViewItem listViewItem in (IEnumerable) this.mLoadedCFGsListView.Items)
      {
        if (listViewItem.Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
        {
          try
          {
            string fullFilePath = listViewItem.Tag.ToString();
            IMConfig index = this.mLoadedCFGDict[fullFilePath];
            List<IMControlScheme> imControlSchemeList = new List<IMControlScheme>();
            foreach (IMControlScheme controlScheme in index.ControlSchemes)
            {
              IMControlScheme imControlScheme = controlScheme.DeepCopy();
              imControlScheme.SetGameControls(this.GetFinalListOfActions(this.mSchemeTreeMapping[index][controlScheme]));
              imControlSchemeList.Add(imControlScheme);
            }
            index.ControlSchemes = imControlSchemeList;
            JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            this.WriteFile(fullFilePath, JsonConvert.SerializeObject((object) index, serializerSettings));
            listViewItem.Content = (object) listViewItem.Content.ToString().TrimEnd("* (modified)".ToCharArray());
          }
          catch (Exception ex)
          {
            string str = string.Format("Couldn't write to file: {0}, Ex: {1}", (object) listViewItem.Tag.ToString(), (object) ex);
            Logger.Error(str);
            int num = (int) System.Windows.MessageBox.Show(str);
          }
        }
      }
    }

    private void WriteFile(string fullFilePath, string output)
    {
      string str = fullFilePath + ".tmp";
      if (File.Exists(str))
        File.Delete(str);
      File.WriteAllText(str, output);
      if (File.Exists(fullFilePath))
        File.Delete(fullFilePath);
      File.Move(str, fullFilePath);
    }

    private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(child);
      if (parent == null)
        return default (T);
      return parent is T obj ? obj : this.FindVisualParent<T>(parent);
    }

    private void MapTreeViewFromDict(Dictionary<string, List<IMAction>> dict)
    {
      foreach (string key in dict.Keys)
      {
        TreeViewItem treeViewItem1 = new TreeViewItem();
        treeViewItem1.Header = (object) key;
        foreach (IMAction imAction in dict[key])
        {
          TreeViewItem treeViewItem2 = new TreeViewItem();
          treeViewItem2.Header = (object) CFGReorderWindow.GetGuidanceFromIMAction(imAction.Guidance.Values);
          treeViewItem2.Tag = (object) imAction;
          treeViewItem1.Items.Add((object) treeViewItem2);
        }
        this.mIMActionsTreeView.Items.Add((object) treeViewItem1);
      }
    }

    private void BuildIMActionsTree()
    {
      if (this.mSchemeTreeMapping.ContainsKey(this.mCurrentlySelectedCFG) && this.mSchemeTreeMapping[this.mCurrentlySelectedCFG].ContainsKey(this.mCurrentlySelectedScheme))
      {
        this.MapTreeViewFromDict(this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][this.mCurrentlySelectedScheme]);
      }
      else
      {
        foreach (IMControlScheme controlScheme in this.mCurrentlySelectedCFG.ControlSchemes)
        {
          Dictionary<string, List<IMAction>> dict = new Dictionary<string, List<IMAction>>();
          foreach (IMAction gameControl in controlScheme.GameControls)
          {
            if (!dict.ContainsKey(gameControl.GuidanceCategory))
              dict[gameControl.GuidanceCategory] = new List<IMAction>();
            dict[gameControl.GuidanceCategory].Add(gameControl);
          }
          if (!this.mSchemeTreeMapping.ContainsKey(this.mCurrentlySelectedCFG))
            this.mSchemeTreeMapping[this.mCurrentlySelectedCFG] = new Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>();
          this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][controlScheme] = dict;
          if (controlScheme == this.mCurrentlySelectedScheme)
            this.MapTreeViewFromDict(dict);
        }
      }
    }

    private static string GetGuidanceFromIMAction(
      Dictionary<string, string>.ValueCollection valuePairs)
    {
      if (valuePairs.Count == 0)
        return "NO_GUIDANCE";
      string str = "";
      foreach (string valuePair in valuePairs)
        str = str + valuePair + " / ";
      if (str.Length > 5)
        str = str.Substring(0, str.Length - 3);
      return str;
    }

    private void InitCFGList()
    {
      foreach (string key in this.mLoadedCFGDict.Keys)
      {
        System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem();
        listViewItem.Content = (object) Path.GetFileNameWithoutExtension(key);
        listViewItem.Tag = (object) key;
        this.mLoadedCFGsListView.Items.Add((object) listViewItem);
      }
    }

    private void GenerateTreeView()
    {
      this.mIMActionsTreeView.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.TreeView_PreviewMouseMove);
      this.mIMActionsTreeView.ItemContainerStyle = this.GetIMActionsListStyle();
    }

    private void GenerateSchemesListView()
    {
      this.mSchemesListView.DisplayMemberPath = "Name";
      this.mSchemesListView.ItemsSource = (IEnumerable) this.mSchemesList;
      this.mSchemesListView.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.ListView_PreviewMouseMove);
      this.mSchemesListView.ItemContainerStyle = this.GetSchemesListStyle();
    }

    private Style GetSchemesListStyle()
    {
      Style style = new Style(typeof (System.Windows.Controls.ListViewItem));
      style.Setters.Add((SetterBase) new Setter(UIElement.AllowDropProperty, (object) true));
      style.Setters.Add((SetterBase) new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.AnyItem_PreviewMouseLeftButtonDown)));
      style.Setters.Add((SetterBase) new EventSetter(UIElement.DropEvent, (Delegate) new System.Windows.DragEventHandler(this.SchemeItem_Drop)));
      return style;
    }

    private Style GetIMActionsListStyle()
    {
      Style style = new Style(typeof (TreeViewItem));
      style.Setters.Add((SetterBase) new Setter(UIElement.AllowDropProperty, (object) true));
      style.Setters.Add((SetterBase) new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.AnyItem_PreviewMouseLeftButtonDown)));
      style.Setters.Add((SetterBase) new EventSetter(UIElement.DropEvent, (Delegate) new System.Windows.DragEventHandler(this.IMActionItem_Drop)));
      return style;
    }

    private void ListView_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      Vector vector = this._dragStartPoint - e.GetPosition((IInputElement) null);
      if (e.LeftButton != MouseButtonState.Pressed || Math.Abs(vector.X) <= SystemParameters.MinimumHorizontalDragDistance && Math.Abs(vector.Y) <= SystemParameters.MinimumVerticalDragDistance)
        return;
      System.Windows.Controls.ListViewItem visualParent = this.FindVisualParent<System.Windows.Controls.ListViewItem>((DependencyObject) e.OriginalSource);
      if (visualParent == null)
        return;
      int num = (int) DragDrop.DoDragDrop((DependencyObject) visualParent, visualParent.DataContext, System.Windows.DragDropEffects.Move);
    }

    private void TreeView_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      Vector vector = this._dragStartPoint - e.GetPosition((IInputElement) null);
      if (e.LeftButton != MouseButtonState.Pressed || Math.Abs(vector.X) <= SystemParameters.MinimumHorizontalDragDistance && Math.Abs(vector.Y) <= SystemParameters.MinimumVerticalDragDistance)
        return;
      TreeViewItem visualParent = this.FindVisualParent<TreeViewItem>((DependencyObject) e.OriginalSource);
      if (visualParent == null)
        return;
      int num = (int) DragDrop.DoDragDrop((DependencyObject) visualParent, (object) visualParent, System.Windows.DragDropEffects.Move);
    }

    private void AnyItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this._dragStartPoint = e.GetPosition((IInputElement) null);
    }

    private void SchemeItem_Drop(object sender, System.Windows.DragEventArgs e)
    {
      IMControlScheme data = e.Data.GetData(typeof (IMControlScheme)) as IMControlScheme;
      IMControlScheme dataContext = ((FrameworkElement) sender).DataContext as IMControlScheme;
      int sourceIndex = this.mSchemesListView.Items.IndexOf((object) data);
      int targetIndex = this.mSchemesListView.Items.IndexOf((object) dataContext);
      if (sourceIndex == -1 || targetIndex == -1)
        return;
      this.MoveItem(data, sourceIndex, targetIndex);
      this.mSchemesListView.Items.Refresh();
      this.MarkCurrentCFGModified();
    }

    private void MarkCurrentCFGModified()
    {
      ListBoxItem selectedItem = this.mLoadedCFGsListView.SelectedItem as ListBoxItem;
      if (selectedItem.Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
        return;
      ListBoxItem listBoxItem = selectedItem;
      listBoxItem.Content = (object) (listBoxItem.Content?.ToString() + "* (modified)");
    }

    public static ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
    {
      DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject) item);
      while (true)
      {
        switch (parent)
        {
          case TreeViewItem _:
          case System.Windows.Controls.TreeView _:
            goto label_3;
          default:
            parent = VisualTreeHelper.GetParent(parent);
            continue;
        }
      }
label_3:
      return parent as ItemsControl;
    }

    private void IMActionItem_Drop(object sender, System.Windows.DragEventArgs e)
    {
      this.MoveItem2(new CFGReorderWindow.TreeItemDrop(e.Data.GetData(typeof (TreeViewItem)) as TreeViewItem, e.Source as TreeViewItem, this.mIMActionsTreeView));
      this.mIMActionsTreeView.Items.Refresh();
      this.UpdateTreeDictionary();
      this.MarkCurrentCFGModified();
    }

    private void UpdateTreeDictionary()
    {
      Dictionary<string, List<IMAction>> dictionary = new Dictionary<string, List<IMAction>>();
      foreach (TreeViewItem treeViewItem1 in (IEnumerable) this.mIMActionsTreeView.Items)
      {
        List<IMAction> imActionList = new List<IMAction>();
        foreach (TreeViewItem treeViewItem2 in (IEnumerable) treeViewItem1.Items)
          imActionList.Add((IMAction) treeViewItem2.Tag);
        dictionary[(string) treeViewItem1.Header] = imActionList;
      }
      this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][this.mCurrentlySelectedScheme] = dictionary;
    }

    private void MoveItem2(CFGReorderWindow.TreeItemDrop item)
    {
      if (item.IsTargetCategory != item.IsSourceCategory)
        return;
      if (item.AreSourceAndTargetCategories)
      {
        if (this.mIMActionsTreeView.Items.Count <= item.SourceIndex)
          return;
        this.mIMActionsTreeView.Items.RemoveAt(item.SourceIndex);
        this.mIMActionsTreeView.Items.Insert(item.TargetIndex, (object) item.Source);
      }
      else
      {
        if (item.SourceParent.Items.Count <= item.SourceIndex || item.TargetParent.Items.Count <= item.TargetIndex)
          return;
        item.SourceParent.Items.RemoveAt(item.SourceIndex);
        item.TargetParent.Items.Insert(item.TargetIndex, (object) item.Source);
      }
    }

    private void MoveItem(IMControlScheme source, int sourceIndex, int targetIndex)
    {
      if (sourceIndex < targetIndex)
      {
        this.mSchemesList.Insert(targetIndex + 1, source);
        this.mSchemesList.RemoveAt(sourceIndex);
      }
      else
      {
        int index = sourceIndex + 1;
        if (this.mSchemesList.Count + 1 <= index)
          return;
        this.mSchemesList.Insert(targetIndex, source);
        this.mSchemesList.RemoveAt(index);
      }
    }

    private void LoadedCFGsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.mLoadedCFGsListView.SelectedItem == null)
        return;
      this.ClearIMLists();
      this.mCurrentlySelectedCFG = this.mLoadedCFGDict[(string) (this.mLoadedCFGsListView.SelectedItem as System.Windows.Controls.ListViewItem).Tag];
      this.mSchemesList = (IList<IMControlScheme>) this.mCurrentlySelectedCFG.ControlSchemes;
      this.GenerateSchemesListView();
      this.mIMActionsTreeView.Items.Clear();
      if (this.mSchemesListView.Items.Count <= 0)
        return;
      this.mSchemesListView.Visibility = Visibility.Visible;
    }

    private void mSchemesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      int selectedIndex = this.mSchemesListView.SelectedIndex;
      if (selectedIndex == -1)
        return;
      this.mCurrentlySelectedScheme = this.mSchemesList[selectedIndex];
      this.ClearIMActionsTree();
      this.BuildIMActionsTree();
      this.GenerateTreeView();
      if (this.mIMActionsTreeView.Items.Count <= 0)
        return;
      this.mIMActionsTreeView.Visibility = Visibility.Visible;
    }

    private void mIMActionsTreeView_SelectedItemChanged(
      object sender,
      RoutedPropertyChangedEventArgs<object> e)
    {
      this.mActionTextBox.IsEnabled = false;
      this.mActionTextBox.ScrollToLine(0);
      try
      {
        IMAction tag = (IMAction) (this.mIMActionsTreeView.SelectedItem as TreeViewItem).Tag;
        if (tag != null)
        {
          JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
          serializerSettings.Formatting = Formatting.Indented;
          this.mActionTextBox.Text = JsonConvert.SerializeObject((object) tag, serializerSettings);
          this.mActionJsonGrid.Visibility = Visibility.Visible;
        }
        else
        {
          this.mActionTextBox.Text = "";
          this.mActionJsonGrid.Visibility = Visibility.Collapsed;
        }
      }
      catch
      {
        this.mActionTextBox.Text = "";
        this.mActionJsonGrid.Visibility = Visibility.Collapsed;
      }
    }

    private void EditButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      int num = (int) System.Windows.MessageBox.Show("Not implemented");
      this.mActionTextBox.IsEnabled = true;
    }

    private void SaveButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      int num = (int) System.Windows.MessageBox.Show("Not implemented");
    }

    private void mLoadedCFGsListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/cfgreorderwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mLoadedCFGsListView = (System.Windows.Controls.ListView) target;
          this.mLoadedCFGsListView.SelectionChanged += new SelectionChangedEventHandler(this.LoadedCFGsListView_SelectionChanged);
          break;
        case 2:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.LoadCFGButton_PreviewMouseLeftButtonUp);
          break;
        case 3:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SaveCFGButton_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mCurrentlyLoadedStackPanel = (StackPanel) target;
          break;
        case 5:
          this.mCurrentlyLoadedCFGTextBlock = (TextBlock) target;
          break;
        case 6:
          this.mSchemesListView = (System.Windows.Controls.ListView) target;
          this.mSchemesListView.SelectionChanged += new SelectionChangedEventHandler(this.mSchemesListView_SelectionChanged);
          break;
        case 7:
          this.mIMActionsTreeView = (System.Windows.Controls.TreeView) target;
          this.mIMActionsTreeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(this.mIMActionsTreeView_SelectedItemChanged);
          break;
        case 8:
          this.mActionJsonGrid = (Grid) target;
          break;
        case 9:
          this.mActionTextBox = (System.Windows.Controls.TextBox) target;
          break;
        case 10:
          this.mEditButton = (System.Windows.Controls.Button) target;
          this.mEditButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.EditButton_PreviewMouseLeftButtonUp);
          break;
        case 11:
          this.mSaveButton = (System.Windows.Controls.Button) target;
          this.mSaveButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SaveButton_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class TreeItemDrop
    {
      public TreeViewItem Source { get; set; }

      public TreeViewItem Target { get; set; }

      public bool IsSourceCategory { get; set; }

      public bool IsTargetCategory { get; set; }

      public TreeViewItem SourceParent { get; set; }

      public TreeViewItem TargetParent { get; set; }

      public int SourceIndex { get; set; } = -1;

      public int TargetIndex { get; set; } = -1;

      public bool AreSourceAndTargetCategories { get; set; }

      public TreeItemDrop(TreeViewItem sourceItem, TreeViewItem targetItem, System.Windows.Controls.TreeView currentTree)
      {
        this.Source = sourceItem;
        this.Target = targetItem;
        this.SourceParent = CFGReorderWindow.GetSelectedTreeViewItemParent(sourceItem) as TreeViewItem;
        if (this.SourceParent != null)
        {
          this.SourceIndex = this.SourceParent.Items.IndexOf((object) this.Source);
        }
        else
        {
          this.IsSourceCategory = true;
          this.SourceIndex = currentTree.Items.IndexOf((object) this.Source);
        }
        this.TargetParent = CFGReorderWindow.GetSelectedTreeViewItemParent(targetItem) as TreeViewItem;
        if (this.TargetParent != null)
        {
          this.TargetIndex = this.TargetParent.Items.IndexOf((object) this.Target);
        }
        else
        {
          this.IsTargetCategory = true;
          this.TargetIndex = currentTree.Items.IndexOf((object) this.Target);
        }
        this.AreSourceAndTargetCategories = this.SourceParent == null;
      }
    }
  }
}
