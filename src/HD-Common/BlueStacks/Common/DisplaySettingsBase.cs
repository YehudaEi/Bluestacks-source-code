// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DisplaySettingsBase
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class DisplaySettingsBase : System.Windows.Controls.UserControl, INotifyPropertyChanged, IComponentConnector
  {
    private DisplaySettingsBaseModel mCurrentDisplaySettingsModel;
    private CustomToastPopupControl mToastPopup;
    internal CustomComboBox mOrientation;
    internal Grid mRadioButtons;
    internal CustomRadioButton mResolution960x540;
    internal CustomRadioButton mResolution1280x720;
    internal CustomRadioButton mResolution1600x900;
    internal CustomRadioButton mResolution1920x1080;
    internal CustomRadioButton mResolution2560x1440;
    internal Grid CustomResolutionTextBoxes;
    internal CustomTextBox CustomResolutionWidth;
    internal CustomTextBox CustomResolutionHeight;
    internal CustomRadioButton mDpi160;
    internal CustomRadioButton mDpi240;
    internal CustomRadioButton mDpi320;
    internal CustomPictureBox mInfoIcon;
    internal CustomButton mSaveButton;
    private bool _contentLoaded;

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged != null)
        propertyChanged((object) this, new PropertyChangedEventArgs(name));
      CommandManager.InvalidateRequerySuggested();
    }

    public DisplaySettingsBaseModel InitialDisplaySettingsModel { get; set; }

    public DisplaySettingsBaseModel CurrentDisplaySettingsModel
    {
      get
      {
        return this.mCurrentDisplaySettingsModel;
      }
      set
      {
        this.mCurrentDisplaySettingsModel = value;
        this.OnPropertyChanged(nameof (CurrentDisplaySettingsModel));
      }
    }

    public bool IsOpenedFromMultiInstance { get; set; }

    public string VmName { get; set; } = "Android";

    public ICommand SaveCommand { get; set; }

    public int MinResolutionWidth { get; set; } = 540;

    public int MinResolutionHeight { get; set; } = 540;

    public int MaxResolutionWidth { get; set; } = 2560;

    public int MaxResolutionHeight { get; set; } = 2560;

    protected virtual void Save(object param)
    {
    }

    public Window Owner { get; private set; }

    public string OEM { get; private set; } = "bgp";

    public DisplaySettingsBase(Window owner, string vmName, string oem = "")
    {
      this.Owner = owner;
      this.OEM = string.IsNullOrEmpty(oem) ? "bgp" : oem;
      this.VmName = vmName;
      this.Owner = owner;
      this.Init();
      this.LoadViewFromUri("/HD-Common;component/Settings/DisplaySettingBase/DisplaySettingsBase.xaml");
      this.Visibility = Visibility.Hidden;
    }

    public void Init()
    {
      this.InitialDisplaySettingsModel = new DisplaySettingsBaseModel(Utils.GetDpiFromBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters), RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth, RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight);
      this.CurrentDisplaySettingsModel = this.InitialDisplaySettingsModel.DeepCopy<DisplaySettingsBaseModel>();
      this.SaveCommand = (ICommand) new RelayCommand2(new Func<object, bool>(this.CanSave), new System.Action<object>(this.Save));
      this.MaxResolutionWidth = Math.Max(this.MaxResolutionWidth, Screen.PrimaryScreen.Bounds.Width);
      this.MaxResolutionHeight = Math.Max(this.MaxResolutionHeight, Screen.PrimaryScreen.Bounds.Height);
    }

    private bool CanSave(object _1)
    {
      return this.IsDirty() && this.IsValid();
    }

    public bool IsDirty()
    {
      return this.InitialDisplaySettingsModel.ResolutionType.ResolutionWidth != this.CurrentDisplaySettingsModel.ResolutionType.ResolutionWidth || this.InitialDisplaySettingsModel.ResolutionType.ResolutionHeight != this.CurrentDisplaySettingsModel.ResolutionType.ResolutionHeight || this.CurrentDisplaySettingsModel.Dpi != this.InitialDisplaySettingsModel.Dpi;
    }

    public bool IsValid()
    {
      return !Validation.GetHasError((DependencyObject) this.CustomResolutionHeight) && !Validation.GetHasError((DependencyObject) this.CustomResolutionWidth);
    }

    protected void SaveDisplaySetting()
    {
      Logger.Info("Saving Display setting");
      Utils.SetDPIInBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters, this.CurrentDisplaySettingsModel.Dpi, this.VmName, this.OEM);
      RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth = this.CurrentDisplaySettingsModel.ResolutionType.ResolutionWidth;
      RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight = this.CurrentDisplaySettingsModel.ResolutionType.ResolutionHeight;
      Stats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Display-Settings", "", (string) null, this.VmName, (string) null, (string) null, "Android", 0);
      this.InitialDisplaySettingsModel.InitDisplaySettingsBaseModel(Utils.GetDpiFromBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters), RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth, RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight);
    }

    public void DiscardCurrentChangingModel()
    {
      this.CurrentDisplaySettingsModel = this.InitialDisplaySettingsModel.DeepCopy<DisplaySettingsBaseModel>();
    }

    protected void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl(this.Owner);
        this.mToastPopup.Init(this.Owner, message, (Brush) null, (Brush) null, System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/HD-Common;component/settings/displaysettingbase/displaysettingsbase.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          this.mOrientation = (CustomComboBox) target;
          break;
        case 2:
          this.mRadioButtons = (Grid) target;
          break;
        case 3:
          this.mResolution960x540 = (CustomRadioButton) target;
          break;
        case 4:
          this.mResolution1280x720 = (CustomRadioButton) target;
          break;
        case 5:
          this.mResolution1600x900 = (CustomRadioButton) target;
          break;
        case 6:
          this.mResolution1920x1080 = (CustomRadioButton) target;
          break;
        case 7:
          this.mResolution2560x1440 = (CustomRadioButton) target;
          break;
        case 8:
          this.CustomResolutionTextBoxes = (Grid) target;
          break;
        case 9:
          this.CustomResolutionWidth = (CustomTextBox) target;
          break;
        case 10:
          this.CustomResolutionHeight = (CustomTextBox) target;
          break;
        case 11:
          this.mDpi160 = (CustomRadioButton) target;
          break;
        case 12:
          this.mDpi240 = (CustomRadioButton) target;
          break;
        case 13:
          this.mDpi320 = (CustomRadioButton) target;
          break;
        case 14:
          this.mInfoIcon = (CustomPictureBox) target;
          break;
        case 15:
          this.mSaveButton = (CustomButton) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
