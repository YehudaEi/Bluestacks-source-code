// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SchemeComboBox
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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class SchemeComboBox : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (string), typeof (ComboBoxSchemeControl), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty));
    internal SchemeComboBox _this;
    internal Grid mGrid;
    internal ToggleButton TogglePopupButton;
    internal TextBlock mName;
    internal Path Arrow;
    internal CustomPopUp mItems;
    internal ScrollViewer mSchemesListScrollbar;
    internal StackPanel Items;
    internal Grid NewProfile;
    private bool _contentLoaded;

    public string mSelectedItem { get; set; }

    public string SelectedItem
    {
      get
      {
        return this.mSelectedItem;
      }
      set
      {
        this.mSelectedItem = value;
      }
    }

    public SchemeComboBox()
    {
      this.InitializeComponent();
    }

    private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
        return;
      e.Handled = true;
    }

    private void ComboBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
    {
      ((ComboBoxSchemeControl) sender).mBookmarkImg.Visibility = Visibility.Collapsed;
    }

    private void NewProfile_MouseDown(object sender, MouseButtonEventArgs e)
    {
      KMManager.AddNewControlSchemeAndSelect(BlueStacksUIUtils.LastActivatedWindow, (IMControlScheme) null, true);
      KMManager.CanvasWindow.ClearWindow();
    }

    private void NewProfile_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.NewProfile, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void NewProfile_MouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.NewProfile, Panel.BackgroundProperty, "ComboBoxBackgroundColor");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/schemecombobox.xaml", UriKind.Relative));
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
          this._this = (SchemeComboBox) target;
          break;
        case 2:
          this.mGrid = (Grid) target;
          break;
        case 3:
          this.TogglePopupButton = (ToggleButton) target;
          break;
        case 4:
          this.mName = (TextBlock) target;
          break;
        case 5:
          this.Arrow = (Path) target;
          break;
        case 6:
          this.mItems = (CustomPopUp) target;
          break;
        case 7:
          this.mSchemesListScrollbar = (ScrollViewer) target;
          break;
        case 8:
          this.Items = (StackPanel) target;
          break;
        case 9:
          this.NewProfile = (Grid) target;
          this.NewProfile.MouseDown += new MouseButtonEventHandler(this.NewProfile_MouseDown);
          this.NewProfile.MouseEnter += new MouseEventHandler(this.NewProfile_MouseEnter);
          this.NewProfile.MouseLeave += new MouseEventHandler(this.NewProfile_MouseLeave);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
