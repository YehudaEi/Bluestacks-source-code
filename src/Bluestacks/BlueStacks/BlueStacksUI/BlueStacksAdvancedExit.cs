// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BlueStacksAdvancedExit
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

namespace BlueStacks.BlueStacksUI
{
  public class BlueStacksAdvancedExit : UserControl, IDimOverlayControl, IComponentConnector
  {
    private string mCurrentGlobalDefault = RegistryManager.Instance.QuitDefaultOption;
    private MainWindow ParentWindow;
    internal CustomPictureBox mCrossButtonPictureBox;
    internal Grid mParentGrid;
    internal Grid mTitleGrid;
    internal TextBlock mTitleText;
    internal Grid mOptionsGrid;
    internal StackPanel mOptionsStackPanel;
    internal Grid mFooterGrid;
    internal CustomButton mNoButton;
    internal CustomButton mYesButton;
    private bool _contentLoaded;

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow { get; set; } = true;

    public bool ShowTransparentWindow { get; set; }

    bool IDimOverlayControl.Close()
    {
      this.Close();
      return true;
    }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    public BlueStacksAdvancedExit(MainWindow window)
    {
      this.ParentWindow = window;
      this.InitializeComponent();
      this.AddOptions();
    }

    private void AddOptions()
    {
      this.GenerateOptions("STRING_QUIT_BLUESTACKS", LocaleStringsConstants.ExitOptions);
      this.AddLineSeperator();
      this.GenerateOptions("STRING_RESTART", LocaleStringsConstants.RestartOptions);
      this.AddLineSeperator();
      this.GenerateCheckBox();
    }

    private void AddLineSeperator()
    {
      Border border1 = new Border();
      border1.Opacity = 0.5;
      border1.Height = 1.0;
      border1.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
      Border border2 = border1;
      BlueStacksUIBinding.BindColor((DependencyObject) border2, Border.BackgroundProperty, "SettingsWindowTabMenuItemForeground");
      this.mOptionsStackPanel.Children.Add((UIElement) border2);
    }

    private void GenerateCheckBox()
    {
      CustomCheckbox customCheckbox = new CustomCheckbox();
      BlueStacksUIBinding.Bind((ToggleButton) customCheckbox, "STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04");
      if (customCheckbox.Image != null)
      {
        customCheckbox.Image.Height = 14.0;
        customCheckbox.Image.Width = 14.0;
      }
      customCheckbox.Height = 20.0;
      customCheckbox.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
      customCheckbox.IsChecked = new bool?(false);
      customCheckbox.Checked += new RoutedEventHandler(this.DontShowAgainCB_Checked);
      customCheckbox.Unchecked += new RoutedEventHandler(this.DontShowAgainCB_Unchecked);
      this.mOptionsStackPanel.Children.Add((UIElement) customCheckbox);
    }

    private void DontShowAgainCB_Checked(object sender, RoutedEventArgs e)
    {
      RegistryManager.Instance.IsQuitOptionSaved = true;
    }

    private void DontShowAgainCB_Unchecked(object sender, RoutedEventArgs e)
    {
      RegistryManager.Instance.IsQuitOptionSaved = false;
    }

    private void GenerateOptions(string title, string[] childrenKeys)
    {
      TextBlock tb1 = new TextBlock();
      BlueStacksUIBinding.Bind(tb1, title, "");
      tb1.Padding = new Thickness(0.0);
      tb1.FontSize = 16.0;
      tb1.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
      BlueStacksUIBinding.BindColor((DependencyObject) tb1, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
      tb1.FontWeight = FontWeights.Normal;
      tb1.HorizontalAlignment = HorizontalAlignment.Left;
      tb1.VerticalAlignment = VerticalAlignment.Center;
      this.mOptionsStackPanel.Children.Add((UIElement) tb1);
      foreach (string childrenKey in childrenKeys)
      {
        CustomRadioButton tb2 = new CustomRadioButton();
        tb2.Checked += new RoutedEventHandler(this.Btn_Checked);
        tb2.HorizontalAlignment = HorizontalAlignment.Left;
        BlueStacksUIBinding.Bind(tb2, childrenKey);
        tb2.Tag = (object) childrenKey;
        tb2.Margin = new Thickness(0.0, 10.0, 0.0, 5.0);
        this.mOptionsStackPanel.Children.Add((UIElement) tb2);
        if (childrenKey == this.mCurrentGlobalDefault)
          tb2.IsChecked = new bool?(true);
      }
    }

    private void Btn_Checked(object sender, RoutedEventArgs e)
    {
      RegistryManager.Instance.QuitDefaultOption = (sender as CustomRadioButton).Tag.ToString();
    }

    public CustomButton YesButton
    {
      get
      {
        return this.mYesButton;
      }
    }

    public CustomButton NoButton
    {
      get
      {
        return this.mNoButton;
      }
    }

    public CustomPictureBox CrossButton
    {
      get
      {
        return this.mCrossButtonPictureBox;
      }
    }

    internal bool Close()
    {
      try
      {
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        this.ParentWindow.HideDimOverlay();
        this.Visibility = Visibility.Hidden;
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close the advanced exit from dimoverlay " + ex.ToString());
      }
      return false;
    }

    private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    private void MYesButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    private void MNoButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/bluestacksadvancedexit.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mCrossButtonPictureBox = (CustomPictureBox) target;
          this.mCrossButtonPictureBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Close_PreviewMouseLeftButtonUp);
          break;
        case 2:
          this.mParentGrid = (Grid) target;
          break;
        case 3:
          this.mTitleGrid = (Grid) target;
          break;
        case 4:
          this.mTitleText = (TextBlock) target;
          break;
        case 5:
          this.mOptionsGrid = (Grid) target;
          break;
        case 6:
          this.mOptionsStackPanel = (StackPanel) target;
          break;
        case 7:
          this.mFooterGrid = (Grid) target;
          break;
        case 8:
          this.mNoButton = (CustomButton) target;
          this.mNoButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MNoButton_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mYesButton = (CustomButton) target;
          this.mYesButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MYesButton_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
