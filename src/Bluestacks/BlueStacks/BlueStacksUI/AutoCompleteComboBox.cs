// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AutoCompleteComboBox
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class AutoCompleteComboBox : UserControl, IComponentConnector
  {
    private List<string> mListData = new List<string>();
    internal CustomComboBox mAutoComboBox;
    private bool _contentLoaded;

    public AutoCompleteComboBox()
    {
      this.InitializeComponent();
      this.mAutoComboBox.IsDropDownOpen = false;
      this.mAutoComboBox.Loaded += (RoutedEventHandler) ((_param1, _param2) =>
      {
        if (!(this.mAutoComboBox.Template.FindName("PART_EditableTextBox", (FrameworkElement) this.mAutoComboBox) is TextBox name))
          return;
        name.TextChanged += new TextChangedEventHandler(this.EditTextBox_TextChanged);
      });
      this.mAutoComboBox.DropDownOpened += new EventHandler(this.MAutoComboBox_DropDownOpened);
      EventManager.RegisterClassHandler(typeof (TextBox), UIElement.KeyUpEvent, (Delegate) new RoutedEventHandler(this.DeselectText));
    }

    private void DeselectText(object sender, RoutedEventArgs e)
    {
      if (!(e.OriginalSource is TextBox originalSource) || originalSource.Text.Length >= 2)
        return;
      originalSource.SelectionLength = 0;
      originalSource.SelectionStart = 1;
    }

    private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.Down))
        e.Handled = true;
      else
        this.mAutoComboBox_TextChanged((sender as TextBox).Text);
    }

    private void MAutoComboBox_DropDownOpened(object sender, EventArgs e)
    {
      this.mAutoComboBox.SelectedItem = (object) null;
    }

    public void AddItems(string key)
    {
      ComboBoxItem comboBoxItem = new ComboBoxItem();
      comboBoxItem.Content = (object) key;
      this.mAutoComboBox.Items.Add((object) comboBoxItem);
    }

    public void AddSuggestions(List<string> listOfSuggestions)
    {
      this.mListData.Clear();
      this.mListData = listOfSuggestions;
    }

    private void mAutoComboBox_TextChanged(string msg)
    {
      string str = msg;
      bool flag = false;
      if (string.IsNullOrEmpty(str))
        this.mAutoComboBox.IsDropDownOpen = false;
      this.mAutoComboBox.Items.Clear();
      foreach (string key in this.mListData)
      {
        if (key.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
        {
          this.AddItems(key);
          flag = true;
        }
      }
      if (flag)
        this.mAutoComboBox.IsDropDownOpen = true;
      else
        this.mAutoComboBox.IsDropDownOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/autocompletecombobox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.mAutoComboBox = (CustomComboBox) target;
      else
        this._contentLoaded = true;
    }
  }
}
