// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Controls.ChangeThemeWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI.Controls
{
  public class ChangeThemeWindow : UserControl, IComponentConnector
  {
    private WrapPanel ThemesDrawer;
    private MainWindow ParentWindow;
    internal Grid mGrid;
    internal CustomPictureBox mChangeThemeWindowIcon;
    internal TextBlock mLblBlueStacksChangeTheme;
    internal CustomPictureBox mCrossButton;
    internal ScrollViewer mThemesDrawerScrollBar;
    private bool _contentLoaded;

    public ChangeThemeWindow(MainWindow parentWindow)
    {
      this.InitializeComponent();
      this.ParentWindow = parentWindow;
      this.ThemesDrawer = this.mThemesDrawerScrollBar.Content as WrapPanel;
      this.AddSkinImages();
    }

    public void AddSkinImages()
    {
      try
      {
        this.ThemesDrawer.Children.Clear();
        foreach (string directory in Directory.GetDirectories(RegistryManager.Instance.ClientInstallDir))
        {
          if (File.Exists(Path.Combine(directory, "ThemeThumbnail.png")))
          {
            string themeName = BlueStacksUIColorManager.GetThemeName(directory);
            SkinSelectorControl skinSelectorControl1 = new SkinSelectorControl();
            skinSelectorControl1.Visibility = Visibility.Visible;
            skinSelectorControl1.HorizontalAlignment = HorizontalAlignment.Center;
            skinSelectorControl1.VerticalAlignment = VerticalAlignment.Top;
            SkinSelectorControl skinSelectorControl2 = skinSelectorControl1;
            skinSelectorControl2.mThemeImage.Visibility = Visibility.Visible;
            skinSelectorControl2.mThemeName.Visibility = Visibility.Visible;
            skinSelectorControl2.mThemeImage.IsFullImagePath = true;
            skinSelectorControl2.mThemeImage.ImageName = Path.Combine(directory, "ThemeThumbnail.png");
            skinSelectorControl2.mThemeCheckButton.Height = 30.0;
            skinSelectorControl2.mThemeName.ToolTip = (object) themeName;
            skinSelectorControl2.mThemeName.Width = double.NaN;
            skinSelectorControl2.mThemeCheckButton.Width = double.NaN;
            skinSelectorControl2.mThemeName.Text = themeName;
            BlueStacksUIBinding.BindColor((DependencyObject) skinSelectorControl2.mThemeName, TextBlock.ForegroundProperty, "ContextMenuItemForegroundColor");
            skinSelectorControl2.mThemeCheckButton.Tag = (object) Path.GetFileName(directory);
            skinSelectorControl2.mThemeCheckButton.Click += new RoutedEventHandler(this.ThemeApplyButton_Click);
            if (string.Compare(RegistryManager.ClientThemeName, Path.GetFileName(directory), StringComparison.OrdinalIgnoreCase) == 0)
            {
              skinSelectorControl2.mThemeAppliedText.Text = LocaleStrings.GetLocalizedString("STRING_APPLIED", "");
              skinSelectorControl2.mThemeAppliedText.Visibility = Visibility.Visible;
              skinSelectorControl2.mThemeAppliedText.Margin = new Thickness(0.0, 3.0, 4.0, 0.0);
            }
            else
            {
              skinSelectorControl2.mThemeCheckButton.ButtonColor = ButtonColors.Blue;
              skinSelectorControl2.mThemeCheckButton.IsEnabled = true;
              skinSelectorControl2.mThemeCheckButton.Content = (object) LocaleStrings.GetLocalizedString("STRING_APPLY", "");
              skinSelectorControl2.mThemeCheckButton.Visibility = Visibility.Visible;
            }
            this.ThemesDrawer.Children.Add((UIElement) skinSelectorControl2);
            this.mThemesDrawerScrollBar.Visibility = Visibility.Visible;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in populating themes in skin widget " + ex.ToString());
      }
    }

    private void ThemeApplyButton_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("Clicked theme apply button");
      this.ParentWindow.Utils.ApplyTheme((sender as CustomButton).Tag.ToString());
      this.AddSkinImages();
      this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
    }

    private void mCrossButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    private void mCrossButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/changethemewindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mGrid = (Grid) target;
          break;
        case 2:
          this.mChangeThemeWindowIcon = (CustomPictureBox) target;
          break;
        case 3:
          this.mLblBlueStacksChangeTheme = (TextBlock) target;
          break;
        case 4:
          this.mCrossButton = (CustomPictureBox) target;
          this.mCrossButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCrossButton_MouseLeftButtonUp);
          this.mCrossButton.PreviewMouseDown += new MouseButtonEventHandler(this.mCrossButton_PreviewMouseDown);
          break;
        case 5:
          this.mThemesDrawerScrollBar = (ScrollViewer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
