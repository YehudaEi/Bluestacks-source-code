// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ImportSchemesWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class ImportSchemesWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();
    private KeymapCanvasWindow CanvasWindow;
    private MainWindow ParentWindow;
    internal StackPanel mSchemesStackPanel;
    internal int mNumberOfSchemesSelectedForImport;
    private Dictionary<string, Dictionary<string, string>> mStringsToImport;
    internal Border mMaskBorder;
    internal ScrollViewer mSchemesListScrollbar;
    internal CustomCheckbox mSelectAllBtn;
    internal CustomButton mImportBtn;
    internal ProgressBar mLoadingGrid;
    private bool _contentLoaded;

    public ImportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
    {
      this.InitializeComponent();
      this.CanvasWindow = window;
      this.ParentWindow = mainWindow;
      this.mSchemesStackPanel = this.mSchemesListScrollbar.Content as StackPanel;
    }

    private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      this.Close();
      this.CanvasWindow.SidebarWindow.mImportSchemesWindow = (ImportSchemesWindow) null;
      this.CanvasWindow.SidebarWindow.mOverlayGrid.Visibility = Visibility.Hidden;
      this.CanvasWindow.SidebarWindow.Focus();
    }

    internal void Init(string fileName)
    {
      try
      {
        List<string> schemeNames = new List<string>();
        foreach (IMControlScheme controlScheme in this.ParentWindow.SelectedConfig.ControlSchemes)
          schemeNames.Add(controlScheme.Name);
        this.mSchemesStackPanel.Children.Clear();
        JObject jobject1 = JObject.Parse(File.ReadAllText(fileName));
        int? configVersion = ConfigConverter.GetConfigVersion(jobject1);
        int num1 = 14;
        IMConfig deserializedImConfigObject;
        if (configVersion.GetValueOrDefault() < num1 & configVersion.HasValue)
        {
          JObject jobject2 = ConfigConverter.Convert(jobject1, "14", false, true);
          JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
          serializerSettings.Formatting = Formatting.Indented;
          JsonSerializerSettings settings = serializerSettings;
          deserializedImConfigObject = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject((object) jobject2, settings), false);
        }
        else
        {
          configVersion = ConfigConverter.GetConfigVersion(jobject1);
          int num2 = 16;
          if (configVersion.GetValueOrDefault() < num2 & configVersion.HasValue && Utils.CheckIfImagesArrayPresentInCfg(jobject1))
          {
            JObject jobject2 = jobject1;
            foreach (JObject scheme in (IEnumerable<JToken>) jobject1["ControlSchemes"])
              scheme["Images"] = (JToken) ConfigConverter.ConvertImagesArrayForPV16(scheme);
            jobject2["MetaData"][(object) "Comment"] = (JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generated automatically from ver {0}", (object) (int) jobject2["MetaData"][(object) "ParserVersion"]);
            jobject2["MetaData"][(object) "ParserVersion"] = (JToken) 16;
            JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            deserializedImConfigObject = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject((object) jobject2, serializerSettings), false);
          }
          else
            deserializedImConfigObject = KMManager.GetDeserializedIMConfigObject(fileName, true);
        }
        this.mStringsToImport = deserializedImConfigObject.Strings;
        this.mNumberOfSchemesSelectedForImport = 0;
        deserializedImConfigObject.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.BuiltIn)).ToList<IMControlScheme>().ForEach((System.Action<IMControlScheme>) (scheme => AddSchemeToImportCheckbox(scheme)));
        deserializedImConfigObject.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => !scheme.BuiltIn)).ToList<IMControlScheme>().ForEach((System.Action<IMControlScheme>) (scheme =>
        {
          if (this.dict.Keys.Contains<string>(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
          {
            scheme.Name += " (Edited)";
            scheme.Name = KMManager.GetUniqueName(scheme.Name, (IEnumerable<string>) schemeNames);
          }
          AddSchemeToImportCheckbox(scheme);
        }));

        void AddSchemeToImportCheckbox(IMControlScheme scheme)
        {
          this.dict.Add(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim(), scheme);
          ImportSchemesWindowControl schemesWindowControl1 = new ImportSchemesWindowControl(this, this.ParentWindow);
          schemesWindowControl1.Width = this.mSchemesStackPanel.Width;
          ImportSchemesWindowControl schemesWindowControl2 = schemesWindowControl1;
          schemesWindowControl2.mContent.Content = (object) scheme.Name;
          schemesWindowControl2.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
          foreach (string key in this.ParentWindow.SelectedConfig.ControlSchemesDict.Keys)
          {
            if (string.Equals(key.Trim(), schemesWindowControl2.mContent.Content.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
              schemesWindowControl2.mBlock.Visibility = Visibility.Visible;
              schemesWindowControl2.mImportName.Text = KMManager.GetUniqueName(schemesWindowControl2.mContent.Content.ToString().Trim(), (IEnumerable<string>) schemeNames);
              break;
            }
          }
          this.mSchemesStackPanel.Children.Add((UIElement) schemesWindowControl2);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in import window init err: " + ex.ToString());
      }
    }

    internal void Box_Unchecked(object sender, RoutedEventArgs e)
    {
      --this.mNumberOfSchemesSelectedForImport;
      if (this.mNumberOfSchemesSelectedForImport == this.mSchemesStackPanel.Children.Count - 1)
        this.mSelectAllBtn.IsChecked = new bool?(false);
      if (this.mNumberOfSchemesSelectedForImport != 0)
        return;
      this.mImportBtn.IsEnabled = false;
    }

    internal void Box_Checked(object sender, RoutedEventArgs e)
    {
      ++this.mNumberOfSchemesSelectedForImport;
      if (this.mNumberOfSchemesSelectedForImport == this.mSchemesStackPanel.Children.Count)
        this.mSelectAllBtn.IsChecked = new bool?(true);
      if (this.mNumberOfSchemesSelectedForImport != 1)
        return;
      this.mImportBtn.IsEnabled = true;
    }

    private bool EditedNameIsAllowed(string text, ImportSchemesWindowControl item)
    {
      if (string.IsNullOrEmpty(text.Trim()))
      {
        BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
        return false;
      }
      if (text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
      {
        BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
        return false;
      }
      foreach (IMControlScheme controlScheme in this.ParentWindow.SelectedConfig.ControlSchemes)
      {
        if (controlScheme.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
          return false;
      }
      foreach (ImportSchemesWindowControl child in this.mSchemesStackPanel.Children)
      {
        bool? isChecked = child.mContent.IsChecked;
        bool flag1 = true;
        if (isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue && child.mBlock.Visibility == Visibility.Visible && (child.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && child.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) != item.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture)))
          return false;
        isChecked = child.mContent.IsChecked;
        bool flag2 = true;
        if (isChecked.GetValueOrDefault() == flag2 & isChecked.HasValue && child.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
          return false;
      }
      return true;
    }

    private void ImportBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        int index = 0;
        bool flag1 = true;
        List<IMControlScheme> imControlSchemeList = new List<IMControlScheme>();
        foreach (ImportSchemesWindowControl child in this.mSchemesStackPanel.Children)
        {
          bool? isChecked = child.mContent.IsChecked;
          bool flag2 = true;
          if (isChecked.GetValueOrDefault() == flag2 & isChecked.HasValue)
          {
            imControlSchemeList.Add(this.dict.ElementAt<KeyValuePair<string, IMControlScheme>>(index).Value);
            if (this.ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select<string, string>((Func<string, string>) (key => key.ToLower(CultureInfo.InvariantCulture).Trim())).Contains<string>(child.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
            {
              if (!this.EditedNameIsAllowed(child.mImportName.Text, child))
              {
                child.mImportName.InputTextValidity = TextValidityOptions.Error;
                if (!string.IsNullOrEmpty(child.mImportName.Text) && child.mImportName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                  BlueStacksUIBinding.Bind(child.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_SCHEME_NAME_WARNING", ""), "");
                child.mWarningMsg.Visibility = Visibility.Visible;
                flag1 = false;
              }
              else
              {
                child.mImportName.InputTextValidity = TextValidityOptions.Success;
                child.mWarningMsg.Visibility = Visibility.Collapsed;
              }
            }
          }
          ++index;
        }
        if (imControlSchemeList.Count == 0)
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""), 1.3, false);
        }
        else
        {
          if (!flag1)
            return;
          foreach (IMControlScheme scheme in imControlSchemeList)
          {
            ImportSchemesWindowControl controlFromScheme = this.GetControlFromScheme(scheme);
            if (this.ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select<string, string>((Func<string, string>) (key => key.ToLower(CultureInfo.InvariantCulture))).Contains<string>(controlFromScheme.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
              scheme.Name = controlFromScheme.mImportName.Text.Trim();
          }
          this.mStringsToImport = KMManager.CleanupGuidanceAccordingToSchemes(imControlSchemeList, this.mStringsToImport);
          this.ImportSchemes(imControlSchemeList, this.mStringsToImport);
          KeymapCanvasWindow.sIsDirty = true;
          KMManager.SaveIMActions(this.ParentWindow, false, false);
          this.CanvasWindow.SidebarWindow.FillProfileCombo();
          this.CanvasWindow.SidebarWindow.ProfileChanged();
          KMManager.SendSchemeChangedStats(this.ParentWindow, "import_scheme");
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_IMPORTED", ""), 1.3, false);
          this.CloseWindow();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error while importing script. err:" + ex.ToString());
      }
    }

    private ImportSchemesWindowControl GetControlFromScheme(
      IMControlScheme scheme)
    {
      foreach (ImportSchemesWindowControl child in this.mSchemesStackPanel.Children)
      {
        if (child.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) == scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim())
          return child;
      }
      return (ImportSchemesWindowControl) null;
    }

    private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
    {
      if (this.mSelectAllBtn.IsChecked.Value)
      {
        foreach (ImportSchemesWindowControl child in this.mSchemesStackPanel.Children)
          child.mContent.IsChecked = new bool?(true);
      }
      else
      {
        foreach (ImportSchemesWindowControl child in this.mSchemesStackPanel.Children)
          child.mContent.IsChecked = new bool?(false);
      }
    }

    internal void ImportSchemes(
      List<IMControlScheme> toCopyFromSchemes,
      Dictionary<string, Dictionary<string, string>> stringsToImport)
    {
      bool flag1 = false;
      bool flag2 = false;
      KMManager.MergeConflictingGuidanceStrings(this.ParentWindow.SelectedConfig, toCopyFromSchemes, stringsToImport);
      if (this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
        flag1 = true;
      foreach (IMControlScheme toCopyFromScheme in toCopyFromSchemes)
      {
        IMControlScheme imControlScheme = toCopyFromScheme.DeepCopy();
        if (flag1)
          imControlScheme.Selected = false;
        imControlScheme.BuiltIn = false;
        imControlScheme.IsBookMarked = false;
        this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = imControlScheme.Name;
        this.ParentWindow.SelectedConfig.ControlSchemes.Add(imControlScheme);
        this.ParentWindow.SelectedConfig.ControlSchemesDict.Add(imControlScheme.Name, imControlScheme);
        ComboBoxSchemeControl boxSchemeControl = new ComboBoxSchemeControl(this.CanvasWindow, this.ParentWindow);
        boxSchemeControl.mSchemeName.Text = LocaleStrings.GetLocalizedString(imControlScheme.Name, "");
        boxSchemeControl.IsEnabled = true;
        BlueStacksUIBinding.BindColor((DependencyObject) boxSchemeControl, Control.BackgroundProperty, "ComboBoxBackgroundColor");
        this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Add((UIElement) boxSchemeControl);
      }
      if (flag1)
        return;
      foreach (IMControlScheme controlScheme in this.ParentWindow.SelectedConfig.ControlSchemes)
      {
        if (controlScheme.Selected)
        {
          flag2 = true;
          break;
        }
      }
      if (flag2)
        return;
      this.ParentWindow.SelectedConfig.ControlSchemes[0].Selected = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/importschemeswindow.xaml", UriKind.Relative));
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
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.Close_MouseLeftButtonUp);
          break;
        case 3:
          this.mSchemesListScrollbar = (ScrollViewer) target;
          break;
        case 4:
          this.mSelectAllBtn = (CustomCheckbox) target;
          this.mSelectAllBtn.Click += new RoutedEventHandler(this.SelectAllBtn_Click);
          break;
        case 5:
          this.mImportBtn = (CustomButton) target;
          this.mImportBtn.Click += new RoutedEventHandler(this.ImportBtn_Click);
          break;
        case 6:
          this.mLoadingGrid = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
