// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.EngineSettingBase
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.Common
{
  public class EngineSettingBase : UserControl, IComponentConnector
  {
    internal CustomRadioButton mCompatibilityMode;
    internal CustomRadioButton mPerformanceMode;
    internal CustomRadioButton mDirectX;
    internal CustomRadioButton mGlMode;
    internal CustomRadioButton softwareDecoding;
    internal CustomPictureBox mVSyncHelp;
    internal CustomPictureBox mHelpCenterImage;
    private bool _contentLoaded;

    public EngineSettingBase()
    {
      this.InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
        Utils.OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in opening url" + ex.ToString());
      }
    }

    private void ASTCHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=ASTC_Help");
    }

    private void mHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=ABI_Help");
    }

    private void GPUHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=GPU_Setting_Help");
    }

    private void DirectXRadioButton_Click(object sender, RoutedEventArgs e)
    {
      if (!(this.DataContext is EngineSettingBaseViewModel dataContext))
        return;
      dataContext.SetGraphicMode(GraphicsMode.DirectX);
    }

    internal void SetAdvancedGraphicMode(bool useAdvancedGraphicEngine)
    {
      if (useAdvancedGraphicEngine)
      {
        if (!this.mCompatibilityMode.IsChecked.GetValueOrDefault())
          this.mCompatibilityMode.IsChecked = new bool?(true);
        if (!this.mPerformanceMode.IsChecked.GetValueOrDefault())
          return;
        this.mPerformanceMode.IsChecked = new bool?(false);
      }
      else
      {
        if (this.mCompatibilityMode.IsChecked.GetValueOrDefault())
          this.mCompatibilityMode.IsChecked = new bool?(false);
        if (this.mPerformanceMode.IsChecked.GetValueOrDefault())
          return;
        this.mPerformanceMode.IsChecked = new bool?(true);
      }
    }

    private void OpenGlRadioButton_Click(object sender, RoutedEventArgs e)
    {
      if (!(this.DataContext is EngineSettingBaseViewModel dataContext))
        return;
      dataContext.SetGraphicMode(GraphicsMode.OpenGL);
    }

    public void SetGraphicMode(GraphicsMode mode)
    {
      this.mDirectX.IsChecked = new bool?(mode == GraphicsMode.DirectX);
      this.mGlMode.IsChecked = new bool?(mode == GraphicsMode.OpenGL);
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      BluestacksUIColor.ScrollBarScrollChanged(sender, e);
    }

    private void PerformanceMode_Click(object sender, RoutedEventArgs e)
    {
      if (!(this.DataContext is EngineSettingBaseViewModel dataContext))
        return;
      dataContext.UseAdvancedGraphicEngine = false;
    }

    private void CompatibilityMode_Click(object sender, RoutedEventArgs e)
    {
      if (!(this.DataContext is EngineSettingBaseViewModel dataContext))
        return;
      dataContext.UseAdvancedGraphicEngine = true;
    }

    private void VSyncHelp_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=VSync_Help");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/settings/enginesettingbase/enginesettingbase.xaml", UriKind.Relative));
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
          ((ScrollViewer) target).ScrollChanged += new ScrollChangedEventHandler(this.ScrollViewer_ScrollChanged);
          break;
        case 2:
          this.mCompatibilityMode = (CustomRadioButton) target;
          break;
        case 3:
          this.mPerformanceMode = (CustomRadioButton) target;
          break;
        case 4:
          this.mDirectX = (CustomRadioButton) target;
          break;
        case 5:
          this.mGlMode = (CustomRadioButton) target;
          break;
        case 6:
          this.softwareDecoding = (CustomRadioButton) target;
          break;
        case 7:
          this.mVSyncHelp = (CustomPictureBox) target;
          break;
        case 8:
          this.mHelpCenterImage = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
