// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Gl3CustomWindow
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
  public class Gl3CustomWindow : CustomWindow, IComponentConnector
  {
    private MainWindow mParentWindow;
    internal Border mMaskBorder;
    internal Grid mParentGrid;
    internal Grid mTextBlockGrid;
    internal CustomPictureBox mCustomMessageBoxCloseButton;
    internal TextBlock mTitleText;
    internal CustomPictureBox mTitleIcon;
    internal TextBlock mBodyTextBlock;
    internal Grid mHintGrid;
    internal TextBlock mHintTextBlock;
    internal Grid mHintGrid1;
    internal TextBlock mHintTextBlock1;
    internal CustomButton mButton;
    private bool _contentLoaded;

    public Gl3CustomWindow(MainWindow parentWindow)
    {
      this.mParentWindow = parentWindow;
      this.InitializeComponent();
    }

    private void mGetButton_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("Clicked Restart to opengl button");
      if (RegistryManager.Instance.GLES3 && this.mParentWindow.EngineInstanceRegistry.GlRenderMode != 1)
      {
        this.mParentWindow.EngineInstanceRegistry.GlRenderMode = 1;
        BlueStacksUIUtils.RestartInstance(this.mParentWindow.mVmName, false);
      }
      else
        this.Close();
    }

    private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked Gl3 custom window close button");
      this.Close();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/gl3customwindow.xaml", UriKind.Relative));
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
          this.mParentGrid = (Grid) target;
          break;
        case 3:
          this.mTextBlockGrid = (Grid) target;
          break;
        case 4:
          this.mCustomMessageBoxCloseButton = (CustomPictureBox) target;
          this.mCustomMessageBoxCloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Close_PreviewMouseLeftButtonUp);
          break;
        case 5:
          this.mTitleText = (TextBlock) target;
          break;
        case 6:
          this.mTitleIcon = (CustomPictureBox) target;
          break;
        case 7:
          this.mBodyTextBlock = (TextBlock) target;
          break;
        case 8:
          this.mHintGrid = (Grid) target;
          break;
        case 9:
          this.mHintTextBlock = (TextBlock) target;
          break;
        case 10:
          this.mHintGrid1 = (Grid) target;
          break;
        case 11:
          this.mHintTextBlock1 = (TextBlock) target;
          break;
        case 12:
          this.mButton = (CustomButton) target;
          this.mButton.Click += new RoutedEventHandler(this.mGetButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
