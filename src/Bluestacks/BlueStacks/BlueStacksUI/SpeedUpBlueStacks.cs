// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SpeedUpBlueStacks
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class SpeedUpBlueStacks : UserControl, IComponentConnector
  {
    internal CustomPictureBox CloseBtn;
    internal SpeedUpBluestacksUserControl mEnableVt;
    internal SpeedUpBluestacksUserControl mConfigureAntivirus;
    internal SpeedUpBluestacksUserControl mDiasbleHyperV;
    internal SpeedUpBluestacksUserControl mPowerPlan;
    internal SpeedUpBluestacksUserControl mUpgradeComputer;
    private bool _contentLoaded;

    public SpeedUpBlueStacks()
    {
      this.InitializeComponent();
      this.SetUrl();
      this.SetContent();
    }

    private void SetUrl()
    {
      if (Oem.IsOEMDmm)
      {
        this.mEnableVt.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
        this.mUpgradeComputer.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
        this.mConfigureAntivirus.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
        this.mDiasbleHyperV.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
        this.mPowerPlan.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
      }
      else
      {
        string str = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=";
        this.mEnableVt.mHyperLink.NavigateUri = new Uri(str + "enable_virtualization");
        this.mUpgradeComputer.mHyperLink.NavigateUri = new Uri(str + "bs3_nougat_min_requirements");
        this.mConfigureAntivirus.mHyperLink.NavigateUri = new Uri(str + "disable_antivirus");
        this.mDiasbleHyperV.mHyperLink.NavigateUri = new Uri(str + "disable_hypervisor");
        this.mPowerPlan.mHyperLink.NavigateUri = new Uri(str + "change_powerplan");
      }
    }

    private void SetContent()
    {
      BlueStacksUIBinding.Bind(this.mEnableVt.mTitleText, "STRING_ENABLE_VIRT", "");
      BlueStacksUIBinding.Bind(this.mEnableVt.mBodyText, "STRING_ENABLE_VIRT_BODY", "");
      this.mEnableVt.mHyperLink.Inlines.Clear();
      this.mEnableVt.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_ENABLE_VIRT_HYPERLINK", ""));
      this.mEnableVt.mImage.ImageName = "virtualization";
      BlueStacksUIBinding.Bind(this.mDiasbleHyperV.mTitleText, "STRING_DISABLE_HYPERV", "");
      BlueStacksUIBinding.Bind(this.mDiasbleHyperV.mBodyText, "STRING_DISABLE_HYPERV_BODY", "");
      this.mDiasbleHyperV.mHyperLink.Inlines.Clear();
      this.mDiasbleHyperV.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_DISABLE_HYPERV_HYPERLINK", ""));
      this.mDiasbleHyperV.mImage.ImageName = "hypervisor";
      BlueStacksUIBinding.Bind(this.mConfigureAntivirus.mTitleText, "STRING_CONFIGURE_ANTIVIRUS", "");
      BlueStacksUIBinding.Bind(this.mConfigureAntivirus.mBodyText, "STRING_CONFIGURE_ANTIVIRUS_BODY", "");
      this.mConfigureAntivirus.mHyperLink.Inlines.Clear();
      this.mConfigureAntivirus.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_CONFIGURE_ANTIVIRUS_HYPERLINK", ""));
      this.mConfigureAntivirus.mImage.ImageName = "antivirus";
      BlueStacksUIBinding.Bind(this.mPowerPlan.mTitleText, "STRING_POWER_PLAN", "");
      BlueStacksUIBinding.Bind(this.mPowerPlan.mBodyText, "STRING_POWER_PLAN_BODY", "");
      this.mPowerPlan.mHyperLink.Inlines.Clear();
      this.mPowerPlan.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_POWER_PLAN_HYPERLINK", ""));
      this.mPowerPlan.mImage.ImageName = "powerplan";
      BlueStacksUIBinding.Bind(this.mUpgradeComputer.mTitleText, "STRING_UPGRADE_SYSTEM", "");
      BlueStacksUIBinding.Bind(this.mUpgradeComputer.mBodyText, "STRING_UPGRADE_SYSTEM_BODY", "");
      this.mUpgradeComputer.mHyperLink.Inlines.Clear();
      this.mUpgradeComputer.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_UPGRADE_SYSTEM_HYPERLINK", ""));
      this.mUpgradeComputer.mImage.ImageName = "upgrade";
    }

    private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked close button speedUpBluestacks");
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/speedupbluestacks.xaml", UriKind.Relative));
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
          this.CloseBtn = (CustomPictureBox) target;
          this.CloseBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_PreviewMouseLeftButtonUp);
          break;
        case 2:
          this.mEnableVt = (SpeedUpBluestacksUserControl) target;
          break;
        case 3:
          this.mConfigureAntivirus = (SpeedUpBluestacksUserControl) target;
          break;
        case 4:
          this.mDiasbleHyperV = (SpeedUpBluestacksUserControl) target;
          break;
        case 5:
          this.mPowerPlan = (SpeedUpBluestacksUserControl) target;
          break;
        case 6:
          this.mUpgradeComputer = (SpeedUpBluestacksUserControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
