// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.UpdateDownloadProgress
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class UpdateDownloadProgress : CustomWindow, IComponentConnector
  {
    internal UpdateDownloadProgress mUpdateDownloadProgressUserControl;
    internal Border mMaskBorder;
    internal TextBlock titleLabel;
    internal CustomPictureBox mCloseBtn;
    internal Hyperlink mDetailedChangeLogs;
    internal BlueProgressBar mUpdateDownloadProgressBar;
    internal Label mUpdateDownloadProgressPercentage;
    internal CustomButton mHideBtn;
    private bool _contentLoaded;

    public UpdateDownloadProgress()
    {
      this.InitializeComponent();
      this.IsShowGLWindow = true;
      this.mDetailedChangeLogs.Inlines.Clear();
      this.mDetailedChangeLogs.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
      this.mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
    }

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void HideBtn_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
    }

    private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Hide();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/updatedownloadprogress.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mUpdateDownloadProgressUserControl = (UpdateDownloadProgress) target;
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.Grid_MouseLeftButtonDown);
          break;
        case 4:
          this.titleLabel = (TextBlock) target;
          break;
        case 5:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseBtn_PreviewMouseLeftButtonUp);
          break;
        case 6:
          this.mDetailedChangeLogs = (Hyperlink) target;
          this.mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 7:
          this.mUpdateDownloadProgressBar = (BlueProgressBar) target;
          break;
        case 8:
          this.mUpdateDownloadProgressPercentage = (Label) target;
          break;
        case 9:
          this.mHideBtn = (CustomButton) target;
          this.mHideBtn.Click += new RoutedEventHandler(this.HideBtn_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
