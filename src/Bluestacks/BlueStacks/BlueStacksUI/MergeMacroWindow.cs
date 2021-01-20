// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MergeMacroWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class MergeMacroWindow : CustomWindow, IComponentConnector
  {
    private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();
    private MacroRecorderWindow mMacroRecorderWindow;
    private MainWindow ParentWindow;
    private MacroRecording mOriginalMacroRecording;
    internal int mAddedMacroTag;
    private MacroSettingsWindow mMacroSettingsWindow;
    private SingleMacroControl mSingleMacroControl;
    internal Border mMaskBorder;
    internal TextBlock mMergeMacroWindowHeading;
    internal CustomButton mUnifyButton;
    internal StackPanel mCurrentMacroScripts;
    internal Line mLineSeperator;
    internal TextBlock mMergedMacrosHeader;
    internal CustomPictureBox mHelpCenterImage;
    internal MacroAddedDragControl mMacroDragControl;
    internal StackPanel mMergedMacrosFooter;
    internal StackPanel mMacroNameStackPanel;
    internal CustomTextBox MacroName;
    internal CustomButton mMacroSettings;
    internal CustomButton mMergeButton;
    internal CustomPopUp mErrorNamePopup;
    internal Border mMaskBorder1;
    internal TextBlock mErrorText;
    internal System.Windows.Shapes.Path mDownArrow;
    private bool _contentLoaded;

    internal MacroRecording MergedMacroRecording { get; set; }

    public MergeMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
    {
      this.InitializeComponent();
      this.mMacroRecorderWindow = window;
      this.ParentWindow = mainWindow;
    }

    internal void Init(MacroRecording mergedMacro = null, SingleMacroControl singleMacroControl = null)
    {
      try
      {
        this.mMacroNameStackPanel.Visibility = mergedMacro == null ? Visibility.Visible : Visibility.Collapsed;
        this.mOriginalMacroRecording = mergedMacro;
        int num = 0;
        foreach (MacroRecording macroRecording in MacroGraph.Instance.Vertices.Cast<MacroRecording>().OrderBy<MacroRecording, DateTime>((Func<MacroRecording, DateTime>) (o => DateTime.ParseExact(o.TimeCreated, "yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal))).ToList<MacroRecording>())
        {
          MacroRecording record = macroRecording;
          this.ParentWindow.mIsScriptsPresent = true;
          if (!record.Equals(mergedMacro) && (mergedMacro == null || !MacroGraph.Instance.DoesParentExist(MacroGraph.Instance.Vertices.Where<BiDirectionalVertex<MacroRecording>>((Func<BiDirectionalVertex<MacroRecording>, bool>) (macro => macro.Equals((object) mergedMacro))).FirstOrDefault<BiDirectionalVertex<MacroRecording>>(), MacroGraph.Instance.Vertices.Where<BiDirectionalVertex<MacroRecording>>((Func<BiDirectionalVertex<MacroRecording>, bool>) (macro => macro.Equals((object) record))).FirstOrDefault<BiDirectionalVertex<MacroRecording>>())))
          {
            MacroToAdd macroToAdd = new MacroToAdd(this, record.Name);
            if (num % 2 == 0)
              BlueStacksUIBinding.BindColor((DependencyObject) macroToAdd, Control.BackgroundProperty, "DarkBandingColor");
            else
              BlueStacksUIBinding.BindColor((DependencyObject) macroToAdd, Control.BackgroundProperty, "LightBandingColor");
            this.mCurrentMacroScripts.Children.Add((UIElement) macroToAdd);
            ++num;
          }
        }
        if (singleMacroControl != null)
          this.mSingleMacroControl = singleMacroControl;
        if (mergedMacro == null)
        {
          string str = DateTime.Now.ToString("yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture);
          this.MergedMacroRecording = new MacroRecording()
          {
            Name = CommonHandlers.GetMacroName("Macro"),
            TimeCreated = str,
            MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>()
          };
          this.mUnifyButton.Visibility = Visibility.Collapsed;
          BlueStacksUIBinding.Bind((Button) this.mMergeButton, "STRING_MERGE");
          BlueStacksUIBinding.Bind(this.mMergeMacroWindowHeading, "STRING_MERGE_MACROS", "");
        }
        else
        {
          this.MergedMacroRecording = mergedMacro.DeepCopy<MacroRecording>();
          BlueStacksUIBinding.Bind((Button) this.mMergeButton, "STRING_UPDATE_SETTING");
          BlueStacksUIBinding.Bind(this.mMergeMacroWindowHeading, "STRING_EDIT_MERGED_MACRO", "");
          this.mUnifyButton.Visibility = Visibility.Visible;
        }
        this.MacroName.Text = this.MergedMacroRecording.Name;
        this.mMacroDragControl.Init();
        this.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
        this.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
        this.Items_CollectionChanged((object) null, (NotifyCollectionChangedEventArgs) null);
        this.DataModificationTracker.Lock((object) this.mOriginalMacroRecording, new List<string>()
        {
          "IsGroupButtonVisible",
          "IsUnGroupButtonVisible",
          "IsSettingsVisible",
          "IsFirstListBoxItem",
          "IsLastListBoxItem",
          "Parents",
          "Childs",
          "IsVisited"
        }, true);
        this.CheckIfCanSave();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in export window init err: " + ex.ToString());
      }
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (args != null)
      {
        if (args.OldItems != null)
        {
          foreach (INotifyPropertyChanged oldItem in (IEnumerable) args.OldItems)
            oldItem.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
        }
        if (args.NewItems != null)
        {
          foreach (INotifyPropertyChanged newItem in (IEnumerable) args.NewItems)
            newItem.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
        }
      }
      else
      {
        foreach (MergedMacroConfiguration macroConfiguration in (Collection<MergedMacroConfiguration>) this.MergedMacroRecording.MergedMacroConfigurations)
          macroConfiguration.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
      }
      foreach (MergedMacroConfiguration macroConfiguration in (Collection<MergedMacroConfiguration>) this.MergedMacroRecording.MergedMacroConfigurations)
      {
        macroConfiguration.IsGroupButtonVisible = true;
        macroConfiguration.IsFirstListBoxItem = false;
        macroConfiguration.IsLastListBoxItem = false;
        macroConfiguration.IsUnGroupButtonVisible = macroConfiguration.MacrosToRun.Count > 1;
      }
      if (this.MergedMacroRecording.MergedMacroConfigurations.Count > 0)
      {
        this.MergedMacroRecording.MergedMacroConfigurations[0].IsGroupButtonVisible = false;
        this.MergedMacroRecording.MergedMacroConfigurations[0].IsFirstListBoxItem = true;
        this.MergedMacroRecording.MergedMacroConfigurations[this.MergedMacroRecording.MergedMacroConfigurations.Count - 1].IsLastListBoxItem = true;
        this.MergedMacroRecording.MergedMacroConfigurations[this.MergedMacroRecording.MergedMacroConfigurations.Count - 1].DelayNextScript = 0;
        this.mMergedMacrosHeader.Visibility = Visibility.Visible;
        this.mHelpCenterImage.Visibility = Visibility.Visible;
        this.mMergedMacrosFooter.IsEnabled = true;
      }
      else
      {
        this.mMergedMacrosHeader.Visibility = Visibility.Collapsed;
        this.mHelpCenterImage.Visibility = Visibility.Collapsed;
        this.mMergedMacrosFooter.IsEnabled = false;
      }
      this.CheckIfCanSave();
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      this.CheckIfCanSave();
    }

    private void CheckIfCanSave()
    {
      bool flag = this.MergedMacroRecording.MergedMacroConfigurations.Count > 0 && (this.MergedMacroRecording.MergedMacroConfigurations.Count > 1 || this.MergedMacroRecording.MergedMacroConfigurations[0].MacrosToRun.Count > 1);
      this.mMergeButton.IsEnabled = ((this.mMacroNameStackPanel.Visibility == Visibility.Collapsed ? 1 : (this.MacroName.InputTextValidity == TextValidityOptions.Success ? 1 : 0)) & (flag ? 1 : 0)) != 0 && this.MergedMacroRecording.MergedMacroConfigurations.All<MergedMacroConfiguration>((Func<MergedMacroConfiguration, bool>) (macro => macro.LoopCount > 0)) && this.DataModificationTracker.HasChanged((object) this.MergedMacroRecording);
      this.mUnifyButton.IsEnabled = flag;
      this.mMacroSettings.IsEnabled = flag;
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_close", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      this.Close();
      this.mMacroRecorderWindow.mMergeMacroWindow = (MergeMacroWindow) null;
      this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Collapsed;
    }

    private void MergeButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.mOriginalMacroRecording == null)
        this.mOriginalMacroRecording = new MacroRecording();
      this.mOriginalMacroRecording.CopyFrom(this.MergedMacroRecording);
      this.mMacroRecorderWindow.SaveMacroRecord(this.mOriginalMacroRecording);
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_success", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      this.CloseWindow();
    }

    private void MacroName_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.mMacroNameStackPanel.Visibility != Visibility.Visible)
        return;
      if (string.IsNullOrEmpty(this.MacroName.Text.Trim()))
      {
        this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", "");
        this.MacroName.InputTextValidity = TextValidityOptions.Error;
      }
      else if (this.MacroName.Text.Trim().IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
      {
        this.mErrorText.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""), (object) Environment.NewLine, (object) "\\ / : * ? \" < > |");
        this.MacroName.InputTextValidity = TextValidityOptions.Error;
      }
      else if (((IEnumerable<string>) Constants.ReservedFileNamesList).Contains<string>(this.MacroName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
      {
        this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
        this.MacroName.InputTextValidity = TextValidityOptions.Error;
      }
      else if (MacroGraph.Instance.Vertices.Cast<MacroRecording>().Any<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macro.Name, this.MacroName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))))
      {
        this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", "");
        this.MacroName.InputTextValidity = TextValidityOptions.Error;
      }
      else
        this.MacroName.InputTextValidity = TextValidityOptions.Success;
      this.mErrorNamePopup.IsOpen = this.MacroName.InputTextValidity == TextValidityOptions.Error;
      this.MergedMacroRecording.Name = this.MacroName.Text;
      this.CheckIfCanSave();
    }

    private void MacroSettings_Click(object sender, RoutedEventArgs e1)
    {
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_settings", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      if (this.mMacroSettingsWindow == null || this.mMacroSettingsWindow.IsClosed)
      {
        this.mMacroSettingsWindow = new MacroSettingsWindow(this.ParentWindow, this.MergedMacroRecording, this.mMacroRecorderWindow);
        this.mMacroSettingsWindow.Closed += (EventHandler) ((o, e2) => this.CheckIfCanSave());
      }
      this.mMacroSettingsWindow.ShowDialog();
    }

    private void UnifyButton_Click(object sender, RoutedEventArgs e1)
    {
      if (this.mOriginalMacroRecording == null)
        this.mOriginalMacroRecording = new MacroRecording();
      this.mOriginalMacroRecording.CopyFrom(this.MergedMacroRecording);
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNIFY_0", ""), (object) this.mOriginalMacroRecording.Name);
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
      bool closeWindow = false;
      customMessageWindow.AddButton(ButtonColors.Blue, string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), (object) "").Trim(), (EventHandler) ((o, evt) =>
      {
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
        this.mMacroRecorderWindow.FlattenRecording(this.mOriginalMacroRecording, false);
        CommonHandlers.SaveMacroJson(this.mOriginalMacroRecording, this.mOriginalMacroRecording.Name + ".json");
        CommonHandlers.RefreshAllMacroRecorderWindow();
        closeWindow = true;
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) ((o, evt) => ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", (string) null, (string) null, (string) null, (string) null, (string) null, "Android")), (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((EventHandler) ((o, e2) => ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", (string) null, (string) null, (string) null, (string) null, (string) null, "Android")), (object) null);
      customMessageWindow.Owner = (Window) this;
      customMessageWindow.ShowDialog();
      if (!closeWindow)
        return;
      this.CloseWindow();
    }

    private void MacroName_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.MacroName.InputTextValidity == TextValidityOptions.Error)
      {
        this.mErrorNamePopup.IsOpen = true;
        this.mErrorNamePopup.StaysOpen = true;
      }
      else
        this.mErrorNamePopup.IsOpen = false;
    }

    private void MacroName_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mErrorNamePopup.IsOpen = false;
    }

    private void mHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=MergeMacro_Help");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/mergemacrowindow.xaml", UriKind.Relative));
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
          this.mMaskBorder = (Border) target;
          break;
        case 2:
          this.mMergeMacroWindowHeading = (TextBlock) target;
          break;
        case 3:
          this.mUnifyButton = (CustomButton) target;
          this.mUnifyButton.Click += new RoutedEventHandler(this.UnifyButton_Click);
          break;
        case 4:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 5:
          this.mCurrentMacroScripts = (StackPanel) target;
          break;
        case 6:
          this.mLineSeperator = (Line) target;
          break;
        case 7:
          this.mMergedMacrosHeader = (TextBlock) target;
          break;
        case 8:
          this.mHelpCenterImage = (CustomPictureBox) target;
          this.mHelpCenterImage.MouseDown += new MouseButtonEventHandler(this.mHelpCenterImage_MouseDown);
          break;
        case 9:
          this.mMacroDragControl = (MacroAddedDragControl) target;
          break;
        case 10:
          this.mMergedMacrosFooter = (StackPanel) target;
          break;
        case 11:
          this.mMacroNameStackPanel = (StackPanel) target;
          break;
        case 12:
          this.MacroName = (CustomTextBox) target;
          this.MacroName.MouseEnter += new MouseEventHandler(this.MacroName_MouseEnter);
          this.MacroName.MouseLeave += new MouseEventHandler(this.MacroName_MouseLeave);
          this.MacroName.TextChanged += new TextChangedEventHandler(this.MacroName_TextChanged);
          break;
        case 13:
          this.mMacroSettings = (CustomButton) target;
          this.mMacroSettings.Click += new RoutedEventHandler(this.MacroSettings_Click);
          break;
        case 14:
          this.mMergeButton = (CustomButton) target;
          this.mMergeButton.Click += new RoutedEventHandler(this.MergeButton_Click);
          break;
        case 15:
          this.mErrorNamePopup = (CustomPopUp) target;
          break;
        case 16:
          this.mMaskBorder1 = (Border) target;
          break;
        case 17:
          this.mErrorText = (TextBlock) target;
          break;
        case 18:
          this.mDownArrow = (System.Windows.Shapes.Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
