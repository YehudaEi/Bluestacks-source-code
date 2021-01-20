// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SidebarPopup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class SidebarPopup : UserControl, IComponentConnector
  {
    private const int NumElementsPerRow = 3;
    internal Grid mGrid;
    internal StackPanel mMainStackPanel;
    private bool _contentLoaded;

    private int NumColumns
    {
      get
      {
        return this.mMainStackPanel.Children.Count;
      }
    }

    public SidebarPopup()
    {
      this.InitializeComponent();
    }

    public void AddElement(SidebarElement element)
    {
      if (element == null)
        return;
      SidebarPopup.RemoveParentIfExists(element);
      if (this.NumColumns == 0)
      {
        this.AddToNewPanel(element);
      }
      else
      {
        StackPanel child = this.mMainStackPanel.Children[this.NumColumns - 1] as StackPanel;
        if (child.Children.Count == 3)
          this.AddToNewPanel(element);
        else
          child.Children.Add((UIElement) element);
      }
    }

    private static void RemoveParentIfExists(SidebarElement element)
    {
      if (!(element.Parent is StackPanel parent))
        return;
      parent.Children.Remove((UIElement) element);
    }

    private void AddToNewPanel(SidebarElement element)
    {
      this.CreateStackPanel().Children.Add((UIElement) element);
    }

    public SidebarElement PopElement()
    {
      StackPanel child1 = this.mMainStackPanel.Children[this.NumColumns - 1] as StackPanel;
      SidebarElement child2 = child1.Children[child1.Children.Count - 1] as SidebarElement;
      child1.Children.Remove((UIElement) child2);
      if (child1.Children.Count == 0)
        this.mMainStackPanel.Children.Remove((UIElement) child1);
      return child2;
    }

    private StackPanel CreateStackPanel()
    {
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.Margin = new Thickness(2.0, 0.0, 2.0, 0.0);
      stackPanel1.Orientation = Orientation.Vertical;
      StackPanel stackPanel2 = stackPanel1;
      this.mMainStackPanel.Children.Add((UIElement) stackPanel2);
      return stackPanel2;
    }

    private void SidebarPopup_Loaded(object sender, RoutedEventArgs e)
    {
    }

    internal void InitAllElements(IEnumerable<SidebarElement> listOfHiddenElements)
    {
      foreach (SidebarElement listOfHiddenElement in listOfHiddenElements)
      {
        if (listOfHiddenElement.Visibility == Visibility.Visible)
        {
          listOfHiddenElement.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
          this.AddElement(listOfHiddenElement);
        }
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/sidebarpopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.SidebarPopup_Loaded);
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
