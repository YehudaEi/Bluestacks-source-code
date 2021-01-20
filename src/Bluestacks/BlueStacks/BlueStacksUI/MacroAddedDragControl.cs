// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroAddedDragControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class MacroAddedDragControl : UserControl, IComponentConnector, IStyleConnector
  {
    private MergeMacroWindow mMergeMacroWindow;
    private Point _dragStartPoint;
    internal Border mNoMergeMacroGrid;
    internal ListBox mListBox;
    private bool _contentLoaded;

    public MergeMacroWindow MergeMacroWindow
    {
      get
      {
        if (this.mMergeMacroWindow == null)
          this.mMergeMacroWindow = Window.GetWindow((DependencyObject) this) as MergeMacroWindow;
        return this.mMergeMacroWindow;
      }
    }

    public MacroAddedDragControl()
    {
      this.InitializeComponent();
    }

    internal void Init()
    {
      this.mListBox.DataContext = (object) this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
      this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
      this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
      this.Items_CollectionChanged((object) null, (NotifyCollectionChangedEventArgs) null);
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Count > 0)
      {
        this.mNoMergeMacroGrid.Visibility = Visibility.Collapsed;
        this.mListBox.Visibility = Visibility.Visible;
      }
      else
      {
        this.mNoMergeMacroGrid.Visibility = Visibility.Visible;
        this.mListBox.Visibility = Visibility.Collapsed;
      }
    }

    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      Vector vector = this._dragStartPoint - e.GetPosition((IInputElement) null);
      if (e.LeftButton != MouseButtonState.Pressed || Math.Abs(vector.X) <= SystemParameters.MinimumHorizontalDragDistance && Math.Abs(vector.Y) <= SystemParameters.MinimumVerticalDragDistance)
        return;
      ListBoxItem visualParent = WpfUtils.FindVisualParent<ListBoxItem>((DependencyObject) e.OriginalSource);
      if (visualParent == null)
        return;
      int num = (int) DragDrop.DoDragDrop((DependencyObject) visualParent, visualParent.DataContext, DragDropEffects.Move);
    }

    private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this._dragStartPoint = e.GetPosition((IInputElement) null);
    }

    private void ListBoxItem_Drop(object sender, DragEventArgs e)
    {
      this.UnsetMarginDuringDrag((ListBoxItem) null);
      if (!(sender is ListBoxItem))
        return;
      MergedMacroConfiguration data = e.Data.GetData(typeof (MergedMacroConfiguration)) as MergedMacroConfiguration;
      MergedMacroConfiguration dataContext = ((FrameworkElement) sender).DataContext as MergedMacroConfiguration;
      int sourceIndex = this.mListBox.Items.IndexOf((object) data);
      int targetIndex = this.mListBox.Items.IndexOf((object) dataContext);
      this.Move(data, sourceIndex, targetIndex);
    }

    private void ListBoxItem_DragOver(object sender, DragEventArgs e)
    {
      if (!(sender is ListBoxItem))
        return;
      MergedMacroConfiguration data = e.Data.GetData(typeof (MergedMacroConfiguration)) as MergedMacroConfiguration;
      ListBoxItem neglectItem = (ListBoxItem) sender;
      MergedMacroConfiguration dataContext = ((FrameworkElement) sender).DataContext as MergedMacroConfiguration;
      int num1 = this.mListBox.Items.IndexOf((object) data);
      int num2 = this.mListBox.Items.IndexOf((object) dataContext);
      if (num2 < num1)
        (neglectItem.Template.FindName("mMainGrid", (FrameworkElement) neglectItem) as Grid).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
      else if (num2 > num1)
        (neglectItem.Template.FindName("mMainGrid", (FrameworkElement) neglectItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 10.0);
      else
        (neglectItem.Template.FindName("mMainGrid", (FrameworkElement) neglectItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
      this.UnsetMarginDuringDrag(neglectItem);
    }

    private void UnsetMarginDuringDrag(ListBoxItem neglectItem = null)
    {
      foreach (object obj in (IEnumerable) this.mListBox.Items)
      {
        ListBoxItem listBoxItem = this.mListBox.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
        if (neglectItem == null || listBoxItem != neglectItem)
          (listBoxItem.Template.FindName("mMainGrid", (FrameworkElement) listBoxItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
      }
    }

    private void Move(MergedMacroConfiguration source, int sourceIndex, int targetIndex)
    {
      if (sourceIndex < targetIndex)
      {
        if (!(this.mListBox.DataContext is ObservableCollection<MergedMacroConfiguration> dataContext))
          return;
        dataContext.Insert(targetIndex + 1, source);
        dataContext.RemoveAt(sourceIndex);
      }
      else
      {
        if (!(this.mListBox.DataContext is ObservableCollection<MergedMacroConfiguration> dataContext))
          return;
        int index = sourceIndex + 1;
        if (dataContext.Count + 1 <= index)
          return;
        dataContext.Insert(targetIndex, source);
        dataContext.RemoveAt(index);
      }
    }

    private void ListBox_DragOver(object sender, DragEventArgs e)
    {
      ListBox listBox = sender as ListBox;
      ScrollViewer visualChild = WpfUtils.FindVisualChild<ScrollViewer>((DependencyObject) listBox);
      double num1 = 15.0;
      double y = e.GetPosition((IInputElement) listBox).Y;
      double num2 = 10.0;
      if (y < num1)
      {
        visualChild.ScrollToVerticalOffset(visualChild.VerticalOffset - num2);
      }
      else
      {
        if (y <= listBox.ActualHeight - num1)
          return;
        visualChild.ScrollToVerticalOffset(visualChild.VerticalOffset + num2);
      }
    }

    private void Group_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is CustomPictureBox customPictureBox))
        return;
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_group", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      int sourceIndex = this.mListBox.Items.IndexOf((object) (customPictureBox.DataContext as MergedMacroConfiguration));
      this.Merge(sourceIndex, sourceIndex - 1);
    }

    private void Merge(int sourceIndex, int targetIndex)
    {
      if (!(this.mListBox.DataContext is ObservableCollection<MergedMacroConfiguration> dataContext))
        return;
      foreach (string str in (Collection<string>) dataContext[sourceIndex].MacrosToRun)
        dataContext[targetIndex].MacrosToRun.Add(str);
      dataContext.RemoveAt(sourceIndex);
      MacroAddedDragControl.SetDefaultPropertiesForMergedMacroConfig(dataContext[targetIndex]);
    }

    private void UnGroup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is CustomPictureBox customPictureBox))
        return;
      MergedMacroConfiguration dataContext = customPictureBox.DataContext as MergedMacroConfiguration;
      int sourceIndex = this.mListBox.Items.IndexOf((object) dataContext);
      this.UnMerge(dataContext, sourceIndex);
    }

    private static void SetDefaultPropertiesForMergedMacroConfig(MergedMacroConfiguration config)
    {
      config.LoopCount = 1;
      config.LoopInterval = 0;
      config.Acceleration = 1.0;
      config.DelayNextScript = 0;
    }

    private void UnMerge(MergedMacroConfiguration source, int sourceIndex)
    {
      if (!(this.mListBox.DataContext is ObservableCollection<MergedMacroConfiguration> dataContext))
        return;
      MacroAddedDragControl.SetDefaultPropertiesForMergedMacroConfig(source);
      for (int index = 0; index < source.MacrosToRun.Count; ++index)
      {
        string str = source.MacrosToRun[index];
        MergedMacroConfiguration macroConfiguration = new MergedMacroConfiguration()
        {
          Tag = this.MergeMacroWindow.mAddedMacroTag++
        };
        macroConfiguration.MacrosToRun.Add(str);
        dataContext.Insert(sourceIndex + index + 1, macroConfiguration);
      }
      dataContext.RemoveAt(sourceIndex);
    }

    private void Remove_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (!(sender is CustomPictureBox customPictureBox))
          return;
        MergedMacroConfiguration dataContext = customPictureBox.DataContext as MergedMacroConfiguration;
        int index = this.mListBox.Items.IndexOf((object) dataContext);
        Logger.Info("Macro tag= " + dataContext?.Tag.ToString() + "and index= " + index.ToString());
        (this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>).RemoveAt(index);
        this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Remove(dataContext);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in removing the merged macro configuration : " + ex.ToString());
      }
    }

    private void Settings_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is CustomPictureBox customPictureBox))
        return;
      ListBoxItem visualParent = WpfUtils.FindVisualParent<ListBoxItem>((DependencyObject) customPictureBox);
      MergedMacroConfiguration dataContext = visualParent.DataContext as MergedMacroConfiguration;
      dataContext.IsSettingsVisible = !dataContext.IsSettingsVisible;
      (visualParent.Template.FindName("mMacroSettingsImage", (FrameworkElement) visualParent) as CustomPictureBox).ImageName = dataContext.IsSettingsVisible ? "outline_settings_collapse" : "outline_settings_expand";
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, dataContext.IsSettingsVisible ? "merge_dropdown_expand" : "merge_dropdown_collapse", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      foreach (object obj in (IEnumerable) this.mListBox.Items)
      {
        ListBoxItem listBoxItem = this.mListBox.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
        if (listBoxItem != visualParent)
        {
          (listBoxItem.DataContext as MergedMacroConfiguration).IsSettingsVisible = false;
          (listBoxItem.Template.FindName("mMacroSettingsImage", (FrameworkElement) listBoxItem) as CustomPictureBox).ImageName = "outline_settings_expand";
        }
      }
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = !this.IsTextAllowed(e.Text);
    }

    private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof (string)))
      {
        if (this.IsTextAllowed((string) e.DataObject.GetData(typeof (string))))
          return;
        e.CancelCommand();
      }
      else
        e.CancelCommand();
    }

    private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Space)
        return;
      e.Handled = true;
    }

    private bool IsTextAllowed(string text)
    {
      return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
    }

    private void MacroAddDragControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.mListBox.DataContext = (object) this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
    }

    private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      CustomTextBox customTextBox = sender as CustomTextBox;
      customTextBox.InputTextValidity = string.IsNullOrEmpty(customTextBox.Text) || customTextBox.Text == "0" ? TextValidityOptions.Error : TextValidityOptions.Success;
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macroaddeddragcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.MacroAddDragControl_Loaded);
          break;
        case 12:
          this.mNoMergeMacroGrid = (Border) target;
          break;
        case 13:
          this.mListBox = (ListBox) target;
          this.mListBox.DragOver += new DragEventHandler(this.ListBox_DragOver);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.MacroName_SizeChanged);
          break;
        case 3:
          ((Style) target).Setters.Add((SetterBase) new EventSetter()
          {
            Event = UIElement.DragOverEvent,
            Handler = (Delegate) new DragEventHandler(this.ListBoxItem_DragOver)
          });
          ((Style) target).Setters.Add((SetterBase) new EventSetter()
          {
            Event = UIElement.DropEvent,
            Handler = (Delegate) new DragEventHandler(this.ListBoxItem_Drop)
          });
          break;
        case 4:
          ((UIElement) target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.ListBoxItem_PreviewMouseLeftButtonDown);
          ((UIElement) target).PreviewMouseMove += new MouseEventHandler(this.ListBox_PreviewMouseMove);
          break;
        case 5:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.UnGroup_PreviewMouseLeftButtonUp);
          break;
        case 6:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Settings_PreviewMouseLeftButtonUp);
          break;
        case 7:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Remove_PreviewMouseLeftButtonUp);
          break;
        case 8:
          ((UIElement) target).PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          ((UIElement) target).AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          ((UIElement) target).PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          ((TextBoxBase) target).TextChanged += new TextChangedEventHandler(this.LoopCountTextBox_TextChanged);
          break;
        case 9:
          ((UIElement) target).PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          ((UIElement) target).AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          ((UIElement) target).PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 10:
          ((UIElement) target).PreviewTextInput += new TextCompositionEventHandler(this.NumericTextBox_PreviewTextInput);
          ((UIElement) target).AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
          ((UIElement) target).PreviewKeyDown += new KeyEventHandler(this.NumericTextBox_KeyDown);
          break;
        case 11:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Group_PreviewMouseLeftButtonUp);
          break;
      }
    }
  }
}
