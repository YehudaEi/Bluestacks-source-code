// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.QuitActionElement
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
  public class QuitActionElement : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty ActionElementProperty = DependencyProperty.Register(nameof (ActionElement), typeof (QuitActionItem), typeof (QuitActionElement), new PropertyMetadata((object) QuitActionItem.None, new PropertyChangedCallback(QuitActionElement.ActionElementPropertyChangedCallback)));
    private string mQuitActionValue = string.Empty;
    private string mCTAEventName = string.Empty;
    private MainWindow ParentWindow;
    private QuitPopupControl ParentQuitPopup;
    private QuitActionItemCTA mCallToAction;
    internal Border maskBorder;
    internal Grid mParentGrid;
    internal CustomPictureBox mExternalLinkImage;
    internal CustomPictureBox mMainImage;
    internal TextBlock mBodyTextBlock;
    internal TextBlock mHyperlinkTextBlock;
    private bool _contentLoaded;

    public QuitActionElement(MainWindow window, QuitPopupControl qpc)
    {
      this.ParentWindow = window;
      this.ParentQuitPopup = qpc;
      this.InitializeComponent();
    }

    public string ParentPopupTag { get; set; } = string.Empty;

    private void SetProperties(QuitActionItem item)
    {
      string str = QuitActionCollection.Actions[item][QuitActionItemProperty.CallToAction];
      this.mBodyTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.BodyText];
      this.mMainImage.ImageName = QuitActionCollection.Actions[item][QuitActionItemProperty.ImageName];
      this.mHyperlinkTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionText];
      this.mQuitActionValue = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionValue];
      this.mCTAEventName = QuitActionCollection.Actions[item][QuitActionItemProperty.StatEventName];
      this.mCallToAction = (QuitActionItemCTA) Enum.Parse(typeof (QuitActionItemCTA), str, true);
    }

    public QuitActionItem ActionElement
    {
      get
      {
        return (QuitActionItem) this.GetValue(QuitActionElement.ActionElementProperty);
      }
      set
      {
        this.SetValue(QuitActionElement.ActionElementProperty, (object) value);
      }
    }

    private static void ActionElementPropertyChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      QuitActionElement quitActionElement = d as QuitActionElement;
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) quitActionElement))
        return;
      quitActionElement.SetProperties((QuitActionItem) e.NewValue);
    }

    private void QAE_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        switch (this.mCallToAction)
        {
          case QuitActionItemCTA.OpenLinkInBrowser:
            if (!string.IsNullOrEmpty(this.mQuitActionValue))
              BlueStacksUIUtils.OpenUrl(this.mQuitActionValue);
            this.SendCTAStat();
            break;
          case QuitActionItemCTA.OpenAppCenter:
            this.OpenAppCenter();
            this.ParentQuitPopup.Close();
            this.SendCTAStat();
            break;
          case QuitActionItemCTA.OpenApplication:
            if (!string.IsNullOrEmpty(this.mQuitActionValue))
              Process.Start(this.mQuitActionValue);
            this.SendCTAStat();
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Some error while CallToAction of QuitPopup. Ex: {0}", (object) ex);
      }
    }

    private void OpenAppCenter()
    {
      try
      {
        this.ParentWindow?.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl((string) null), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter", false, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't open app center. Ex: {0}", (object) ex);
      }
    }

    private void SendCTAStat()
    {
      ClientStats.SendLocalQuitPopupStatsAsync(this.ParentPopupTag, this.mCTAEventName);
    }

    private void QAE_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mExternalLinkImage.Visibility = Visibility.Hidden;
      BlueStacksUIBinding.BindColor((DependencyObject) this.maskBorder, Border.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
      this.Cursor = Cursors.Hand;
    }

    private void QAE_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mExternalLinkImage.Visibility = Visibility.Hidden;
      BlueStacksUIBinding.BindColor((DependencyObject) this.maskBorder, Border.BackgroundProperty, "LightBandingColor");
      this.Cursor = Cursors.Arrow;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/quitactionelement.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.QAE_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.QAE_MouseLeave);
          ((UIElement) target).PreviewMouseUp += new MouseButtonEventHandler(this.QAE_PreviewMouseUp);
          break;
        case 2:
          this.maskBorder = (Border) target;
          break;
        case 3:
          this.mParentGrid = (Grid) target;
          break;
        case 4:
          this.mExternalLinkImage = (CustomPictureBox) target;
          break;
        case 5:
          this.mMainImage = (CustomPictureBox) target;
          break;
        case 6:
          this.mBodyTextBlock = (TextBlock) target;
          break;
        case 7:
          this.mHyperlinkTextBlock = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
