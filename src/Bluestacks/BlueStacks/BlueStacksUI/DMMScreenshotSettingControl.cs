// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DMMScreenshotSettingControl
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
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class DMMScreenshotSettingControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal TextBlock mChooseFolderTextBlock;
    internal CustomButton mChangePathBtn;
    private bool _contentLoaded;

    public DMMScreenshotSettingControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Visibility = Visibility.Hidden;
      this.mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
    }

    private void ChooseScreenshotFolder_MouseLeftButtonUp(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.mCommonHandler.DMMScreenshotHandler();
      this.mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/dmmscreenshotsettingcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
        {
          this.mChangePathBtn = (CustomButton) target;
          this.mChangePathBtn.Click += new RoutedEventHandler(this.ChooseScreenshotFolder_MouseLeftButtonUp);
        }
        else
          this._contentLoaded = true;
      }
      else
        this.mChooseFolderTextBlock = (TextBlock) target;
    }
  }
}
