// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SchemeBookmarkControl
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
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class SchemeBookmarkControl : UserControl, IComponentConnector
  {
    private MainWindow mParentWindow;
    internal TextBlock mSchemeName;
    internal CustomPictureBox mPictureBox;
    private bool _contentLoaded;

    public SchemeBookmarkControl(IMControlScheme scheme, MainWindow window, double maxWidth)
    {
      this.InitializeComponent();
      this.mParentWindow = window;
      this.mSchemeName.Text = scheme?.Name;
      this.mSchemeName.ToolTip = (object) this.mSchemeName.Text;
      if (scheme.Selected)
        this.mPictureBox.ImageName = "radio_selected";
      this.mSchemeName.MaxWidth = maxWidth - 20.0;
    }

    private void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
    }

    private void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mParentWindow.SelectedConfig.SelectedControlScheme == null)
        return;
      int num = this.mSchemeName.Text != this.mParentWindow.SelectedConfig.SelectedControlScheme.Name ? 1 : 0;
    }

    private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!(this.mPictureBox.ImageName == "radio_unselected"))
        return;
      foreach (SchemeBookmarkControl child in this.mParentWindow.mSidebar.mBookmarkedSchemesStackPanel.Children)
        child.mPictureBox.ImageName = "radio_unselected";
      this.mPictureBox.ImageName = "radio_selected";
      KMManager.SelectSchemeIfPresent(this.mParentWindow, this.mSchemeName.Text, "bookmark", true);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/schemebookmarkcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.UserControl_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.UserControl_MouseLeave);
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.UserControl_PreviewMouseDown);
          break;
        case 2:
          this.mSchemeName = (TextBlock) target;
          break;
        case 3:
          this.mPictureBox = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
