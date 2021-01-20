// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.MainWindow
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueStacks.LogCollector
{
  public partial class MainWindow : Window, IComponentConnector, IStyleConnector
  {
    private static string[] sNotAllowedAttachmentExtensions = new string[2]
    {
      ".bat",
      ".exe"
    };
    internal static MainWindow Instance = (MainWindow) null;
    private ObservableCollection<string> AttachedFiles = new ObservableCollection<string>();
    private List<string> InvalidExtensionFiles = new List<string>();
    private const long TOTAL_ATTACHMENT_MAX_SIZE = 26214400;
    private CustomToastPopupControl mToastPopup;
    private bool mQuickLogs;
    internal Border mBorder;
    internal CustomPictureBox CloseBtn;
    internal TextBlock label;
    internal StackPanel mGifGrid;
    internal Image mLoadingImage;
    internal StackPanel mCategoryStackPanel;
    internal TextBlock mCategoryTextBlock;
    internal CustomComboBox mCategoryComboBox;
    internal StackPanel mSubCategoryStackPanel;
    internal TextBlock mSubCategoryTextBlock;
    internal CustomComboBox mSubCategoryCombobox;
    internal TextBlock mAppDetailsTextBlock;
    internal TextBlock mAppNameTitle;
    internal CustomComboBox mAppListComboBox;
    internal Grid mAppNameGrid;
    internal CustomTextBox mAppNameTextBox;
    internal TextBlock mDescribeProblemTextBlock;
    internal CustomTextBox mDescribeProblemTextBox;
    internal CustomButton mChooseButton;
    internal ListBox mAttachedFilesListBox;
    internal TextBlock mEmailTextBlock;
    internal CustomTextBox mEmailTextBox;
    internal CustomCheckbox mAlIOemsCheckbox;
    internal CustomPictureBox mHelpIcon;
    internal CustomCheckbox mStartAllOemsCheckbox;
    internal CustomButton mBtn;
    private bool _contentLoaded;

    public MainWindow(bool quickLogs)
    {
      this.mQuickLogs = quickLogs;
      MainWindow.Instance = this;
      this.InitializeComponent();
      this.Title = LocaleStrings.GetLocalizedString("STRING_BST_SUPPORT_UTILITY", "");
      this.mEmailTextBox.Text = RegistryManager.Instance.RegisteredEmail;
      this.AddAsteriskToMandatoryFields();
      try
      {
        ImageBehavior.SetAnimatedSource(this.mLoadingImage, (ImageSource) new BitmapImage(new Uri(Path.Combine(RegistryStrings.InstallDir, "loadingCircles.gif"))));
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't set loading GIF, Ex: {0}", (object) ex);
      }
      LogCollectorUtils.GetCategoriesInBackground();
      this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
      this.Closing += new CancelEventHandler(this.MainWindow_Closing);
      this.mAlIOemsCheckbox.Visibility = this.mHelpIcon.Visibility = App.sOemApplicableForLogCollection.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void AddAsteriskToMandatoryFields()
    {
      this.mEmailTextBlock.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}*", (object) this.mEmailTextBlock.Text);
      this.mDescribeProblemTextBlock.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}*", (object) this.mDescribeProblemTextBlock.Text);
      if (this.mQuickLogs)
        return;
      this.mCategoryTextBlock.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}*", (object) this.mCategoryTextBlock.Text);
      this.mSubCategoryTextBlock.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}*", (object) this.mSubCategoryTextBlock.Text);
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.mSubCategoryStackPanel.Visibility = Visibility.Hidden;
      this.mAppListComboBox.Visibility = Visibility.Collapsed;
      if (LogCollectorUtils.sProblemCategories != null)
      {
        this.AddCategories();
      }
      else
      {
        this.mCategoryStackPanel.Visibility = Visibility.Collapsed;
        this.mGifGrid.Visibility = Visibility.Visible;
      }
    }

    public void AddCategories()
    {
      Logger.Info("Setting categories");
      this.mCategoryComboBox.Items.Clear();
      ComboBoxItem comboBoxItem1 = new ComboBoxItem();
      comboBoxItem1.Content = (object) LocaleStrings.GetLocalizedString("STRING_CATEGORY", "");
      this.mCategoryComboBox.Items.Add((object) comboBoxItem1);
      foreach (string sProblemCategory in LogCollectorUtils.sProblemCategories)
      {
        ComboBoxItem comboBoxItem2 = new ComboBoxItem();
        comboBoxItem2.Content = (object) sProblemCategory;
        this.mCategoryComboBox.Items.Add((object) comboBoxItem2);
      }
      this.mCategoryComboBox.SelectedIndex = 0;
    }

    internal static void ShowCategories(
      object sender,
      RunWorkerCompletedEventArgs workCompletedEventArgs)
    {
      MainWindow.Instance.Dispatcher.Invoke((Delegate) (() =>
      {
        MainWindow.Instance.mGifGrid.Visibility = Visibility.Collapsed;
        MainWindow.Instance.mCategoryStackPanel.Visibility = Visibility.Visible;
      }));
    }

    private void CategoryChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        Logger.Info("Category changed");
        ComboBox comboBox = (ComboBox) sender;
        string stringConversion = (string) LogCollectorUtils.sStringConversions[(object) ((ContentControl) comboBox.SelectedItem).Content.ToString()];
        if (string.IsNullOrEmpty(stringConversion))
        {
          this.mSubCategoryCombobox.Items.Clear();
          this.mSubCategoryStackPanel.Visibility = Visibility.Hidden;
        }
        else
        {
          if (LogCollectorUtils.sCategorySubcategoryMapping.ContainsKey(stringConversion))
          {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            if (LogCollectorUtils.sCategorySubcategoryMapping[stringConversion] != null)
            {
              Dictionary<string, string> dictionary3 = LogCollectorUtils.sCategorySubcategoryMapping[stringConversion];
              Dictionary<string, string> dictionary4 = LogCollectorUtils.sCategorySubcategoryMappingWithDropdown[stringConversion];
              string[] subCategories = new string[dictionary3.Count];
              int num = 0;
              foreach (string str in dictionary3.Values)
                subCategories[num++] = str;
              if (subCategories.Length != 0)
              {
                this.AddSubcategories(subCategories);
              }
              else
              {
                this.mSubCategoryCombobox.Items.Clear();
                this.mSubCategoryStackPanel.Visibility = Visibility.Hidden;
              }
            }
          }
          string str1 = LogCollectorUtils.sCategoryShowDropdownMapping[stringConversion];
          if (!string.IsNullOrEmpty(str1) && str1 == "1")
            this.AbleDisableAppName(true);
          else
            this.AbleDisableAppName(false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occured, Err: {0}", (object) ex.ToString());
      }
    }

    private void SubCategoryChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        Logger.Info("Subcategory changed");
        string str1 = ((ContentControl) this.mCategoryComboBox.SelectedItem).Content.ToString();
        string stringConversion = (string) LogCollectorUtils.sStringConversions[(object) str1];
        string str2 = LogCollectorUtils.sCategoryShowDropdownMapping[stringConversion];
        string key = ((ContentControl) this.mSubCategoryCombobox.SelectedItem)?.Content?.ToString();
        if (string.IsNullOrEmpty(key) || this.mSubCategoryCombobox.SelectedIndex < 1)
          return;
        if (this.mCategoryComboBox.SelectedIndex > 0 && (string.IsNullOrEmpty(str2) || str2 == "0"))
        {
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          Dictionary<string, string> HT = LogCollectorUtils.sCategorySubcategoryMapping[stringConversion];
          key = MainWindow.FindKey(key, HT);
          Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
          string str3 = LogCollectorUtils.sCategorySubcategoryMappingWithDropdown[stringConversion][key];
          if (!string.IsNullOrEmpty(str3) && str3 == "1")
            this.AbleDisableAppName(true);
          else
            this.AbleDisableAppName(false);
        }
        this.TroubleshootIfPossible(key);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    public static string FindKey(string value, Dictionary<string, string> HT)
    {
      if (HT != null)
      {
        IEnumerable<KeyValuePair<string, string>> source = HT.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (item => string.Equals(item.Value, value, StringComparison.Ordinal)));
        if (source.Any<KeyValuePair<string, string>>())
          return source.First<KeyValuePair<string, string>>().Key;
      }
      return string.Empty;
    }

    private void TroubleshootIfPossible(string subCategory)
    {
      Logger.Info("In method TroubleshootIfPossible");
      if (subCategory.Contains("RPCError"))
      {
        Logger.Info("RPC Error detected");
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RPC_FORM", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_TROUBLESHOOTER", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_RESOLVE_AUTOMATIC", ""), new EventHandler(this.TroubleshootRPCConfirmationHandler), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this;
        customMessageWindow.ShowDialog();
      }
      else
      {
        if (!subCategory.Contains("StuckOnLoading") && !subCategory.Contains("GreyScreen"))
          return;
        Logger.Info("Stuck on loading detected");
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_STUCK_AT_INITIALIZING_FORM", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_TROUBLESHOOTER", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_RESOLVE_AUTOMATIC", ""), new EventHandler(this.TroubleshootStuckConfirmationHandler), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this;
        customMessageWindow.ShowDialog();
      }
    }

    private void TroubleshootRPCConfirmationHandler(object sender, EventArgs e)
    {
      this.ShowProgress(LocaleStrings.GetLocalizedString("STRING_PROGRESS", ""));
      this.RunBinary("HD-GuestCommandRunner.exe", "", LocaleStrings.GetLocalizedString("STRING_WORK_DONE", ""), LocaleStrings.GetLocalizedString("STRING_RPC_FORM", ""));
    }

    private void TroubleshootStuckConfirmationHandler(object sender, EventArgs e)
    {
      Logger.Info("RegistryManager.Instance.VmName " + MultiInstanceStrings.VmName);
      this.Hide();
      HTTPUtils.SendRequestToClient("restartFrontend", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      Process.GetCurrentProcess().Kill();
    }

    private void RunBinary(string fileName, string args, string text, string title)
    {
      try
      {
        Logger.Info("In method RunBinary");
        string str = Path.Combine(RegistryStrings.InstallDir, fileName);
        using (Process process = new Process())
        {
          process.StartInfo.FileName = str;
          process.StartInfo.Arguments = args;
          process.EnableRaisingEvents = true;
          process.Disposed += (EventHandler) ((sender, e) => this.Dispatcher.Invoke((Delegate) (() =>
          {
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            if (!Oem.Instance.IsHideMessageBoxIconInTaskBar)
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              customMessageWindow.TitleTextBlock.Text = title;
              customMessageWindow.BodyTextBlock.Text = text;
              customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
              customMessageWindow.ShowDialog();
            }
            this.Close();
          })));
          process.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occured, Err: {0}", (object) ex.ToString());
      }
    }

    private void ShowProgress(string _)
    {
      this.Hide();
    }

    private void AddSubcategories(string[] subCategories)
    {
      Logger.Info("In method AddSubCategories");
      this.mSubCategoryCombobox.Items.Clear();
      ComboBoxItem comboBoxItem1 = new ComboBoxItem();
      comboBoxItem1.Content = (object) LocaleStrings.GetLocalizedString("STRING_SUBCATEGORY", "");
      this.mSubCategoryCombobox.Items.Add((object) comboBoxItem1);
      foreach (string subCategory in subCategories)
      {
        ComboBoxItem comboBoxItem2 = new ComboBoxItem();
        comboBoxItem2.Content = (object) subCategory;
        this.mSubCategoryCombobox.Items.Add((object) comboBoxItem2);
      }
      this.mSubCategoryCombobox.SelectedIndex = 0;
      this.mSubCategoryStackPanel.Visibility = Visibility.Visible;
    }

    private void AbleDisableAppName(bool showDropdown)
    {
      Logger.Info("In Method AbleDisableAppName");
      Logger.Info("Show apps list dropdown: " + showDropdown.ToString());
      if (showDropdown)
      {
        this.mAppListComboBox.Items.Clear();
        if (!this.AddAppNames())
        {
          Logger.Info("AddAppNames returns false");
          this.mAppNameGrid.Visibility = Visibility.Visible;
          this.mAppListComboBox.Visibility = Visibility.Collapsed;
          this.mAppNameTitle.Text = LocaleStrings.GetLocalizedString("STRING_APP_NAME", "");
        }
        else
        {
          Logger.Info("AddAppNames returns true");
          this.mAppNameGrid.Visibility = Visibility.Collapsed;
          this.mAppListComboBox.Visibility = Visibility.Visible;
          this.mAppNameTitle.Text = LocaleStrings.GetLocalizedString("STRING_SELECT_APP_NAME", "");
          this.mAppListComboBox.SelectedIndex = 0;
        }
      }
      else
      {
        Logger.Info("User not selected App not working");
        this.mAppListComboBox.Visibility = Visibility.Collapsed;
        this.mAppNameGrid.Visibility = Visibility.Visible;
      }
    }

    private bool AddAppNames()
    {
      try
      {
        Logger.Info("In Method AddAppNames");
        Logger.Info("Getting installed apps");
        Logger.Info("Requesting Agent");
        new Dictionary<string, string>()
        {
          {
            "vmname",
            MultiInstanceStrings.VmName
          }
        };
        List<BlueStacks.Common.AppInfo> list = ((IEnumerable<BlueStacks.Common.AppInfo>) new JsonParser(BlueStacks.Common.Strings.CurrentDefaultVmName).GetAppList()).ToList<BlueStacks.Common.AppInfo>();
        int count = list.Count;
        AppInfo[] array = new AppInfo[count];
        if (array == null || array.Length == 0)
        {
          Logger.Info("AppInfo null");
          return false;
        }
        for (int index = 0; index < count; ++index)
        {
          array[index] = new AppInfo(list[index].Name, list[index].Package, list[index].Version);
          array[index].name = Regex.Replace(array[index].name, "\\t|\\n|\\r", string.Empty);
        }
        foreach (AppInfo appInfo in array)
        {
          if (string.IsNullOrEmpty(appInfo.name) || string.IsNullOrEmpty(appInfo.package))
          {
            Logger.Info("Empty app name or package");
            return false;
          }
          Logger.Info("App Package Name: " + appInfo.package + " App Name: " + appInfo.name);
        }
        Array.Sort<AppInfo>(array, (Comparison<AppInfo>) ((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal)));
        ComboBoxItem comboBoxItem1 = new ComboBoxItem();
        comboBoxItem1.Content = (object) LocaleStrings.GetLocalizedString("STRING_SELECT", "");
        comboBoxItem1.Tag = (object) null;
        this.mAppListComboBox.Items.Add((object) comboBoxItem1);
        foreach (AppInfo appInfo in array)
        {
          ComboBoxItem comboBoxItem2 = new ComboBoxItem();
          comboBoxItem2.Content = (object) Regex.Replace(appInfo.name, "\\t|\\n|\\r", string.Empty);
          comboBoxItem2.Tag = (object) Regex.Replace(appInfo.package, "\\t|\\n|\\r", string.Empty);
          this.mAppListComboBox.Items.Add((object) comboBoxItem2);
        }
        ComboBoxItem comboBoxItem3 = new ComboBoxItem();
        comboBoxItem3.Content = (object) "Other";
        comboBoxItem3.Tag = (object) "Other";
        this.mAppListComboBox.Items.Add((object) comboBoxItem3);
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
        return false;
      }
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    private void mChooseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "All Files (*.*)|*.*";
      openFileDialog.RestoreDirectory = true;
      openFileDialog.FileOk += new CancelEventHandler(this.BrowseForAttachmentOkEvent);
      openFileDialog.Multiselect = true;
      openFileDialog.ShowDialog();
      if (this.InvalidExtensionFiles.Count <= 0)
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INVALID_FILE", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ATTACHMENT_UNALLOWED_EXTENSION_BAT_EXE", "");
      customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.ShowDialog();
      this.InvalidExtensionFiles.Clear();
    }

    private void BrowseForAttachmentOkEvent(object sender, CancelEventArgs e)
    {
      long num = 0;
      foreach (string fileName in ((FileDialog) sender).FileNames)
      {
        FileInfo fileInfo = new FileInfo(fileName);
        num += fileInfo.Length;
        string lower = Path.GetExtension(fileName).ToLower(CultureInfo.CurrentCulture);
        if (this.AttachedFiles.Contains(fileName))
          this.AttachedFiles.Remove(fileName);
        this.AttachedFiles.Add(fileName);
        if (((IEnumerable<string>) MainWindow.sNotAllowedAttachmentExtensions).Contains<string>(lower))
        {
          this.InvalidExtensionFiles.Add(fileName);
          this.AttachedFiles.Remove(fileName);
        }
      }
      if (this.AttachedFiles.Count > 10)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ATTACHMENT_COUNT_EXCEEDED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_TOTAL_ATTACHMENT_COUNT_EXCEEDED_LIMIT", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.ShowDialog();
        this.AttachedFiles.Clear();
        this.InvalidExtensionFiles.Clear();
        e.Cancel = true;
      }
      else if (num > 26214400L)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ATTACHMENT_SIZE_EXCEEDED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_TOTAL_ATTACHMENT_SIZE_EXCEEDED_LIMIT", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.ShowDialog();
        this.AttachedFiles.Clear();
        this.InvalidExtensionFiles.Clear();
        e.Cancel = true;
      }
      this.mAttachedFilesListBox.ItemsSource = (IEnumerable) this.AttachedFiles;
    }

    private void mSubmitBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        string text1 = this.mEmailTextBox.Text;
        string text2 = this.mDescribeProblemTextBox.Text;
        ObservableCollection<string> attachedFiles = this.AttachedFiles;
        string str1 = "";
        string str2 = (string) null;
        int num1 = 0;
        string index = (string) null;
        string subcategory = "";
        int num2 = this.mSubCategoryCombobox != null ? this.mSubCategoryCombobox.SelectedIndex : 0;
        if (LogCollectorUtils.sProblemCategories != null)
        {
          num1 = this.mCategoryComboBox.SelectedIndex;
          index = ((ContentControl) this.mCategoryComboBox.SelectedItem).Content.ToString();
        }
        bool flag = true;
        if (this.mAppListComboBox.Visibility == Visibility.Visible)
        {
          if (!this.mQuickLogs && this.mAppListComboBox.SelectedIndex == 0)
          {
            flag = false;
            this.HighlightComboBoxColor(this.mAppListComboBox);
          }
          else
          {
            ComboBoxItem selectedItem = (ComboBoxItem) this.mAppListComboBox.SelectedItem;
            str1 = selectedItem.Content.ToString();
            str2 = selectedItem.Tag?.ToString();
            Logger.Info("Selected App Name from combo box: " + str1 + " and App Package: " + str2);
          }
        }
        string appName = this.mAppListComboBox.Visibility == Visibility.Visible ? str1 : this.mAppNameTextBox.Text;
        if (appName.Equals(this.mAppNameTextBox.WatermarkText, StringComparison.InvariantCultureIgnoreCase))
          appName = string.Empty;
        string appPkgName = this.mAppListComboBox.Visibility == Visibility.Visible ? str2 : (string) null;
        Logger.Info("Selected App Name: " + appName + " and App Package Name: " + appPkgName);
        if (!this.ValidateEmail(text1))
        {
          flag = false;
          this.mEmailTextBox.InputTextValidity = TextValidityOptions.Error;
        }
        if (!this.mQuickLogs && LogCollectorUtils.sProblemCategories != null && num1 == 0)
        {
          flag = false;
          this.HighlightComboBoxColor(this.mCategoryComboBox);
        }
        if (!this.mQuickLogs && this.mSubCategoryCombobox != null && (this.mSubCategoryCombobox.Visibility == Visibility.Visible && num2 == 0))
        {
          flag = false;
          this.HighlightComboBoxColor(this.mSubCategoryCombobox);
        }
        if (string.IsNullOrEmpty(text2.Trim()) || this.mDescribeProblemTextBox.WatermarkText.Equals(text2.Trim(), StringComparison.InvariantCultureIgnoreCase) || text2.Length <= 10)
        {
          flag = false;
          this.mDescribeProblemTextBox.InputTextValidity = TextValidityOptions.Error;
          this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_LOGCOLLECTOR_DESCRIPTION_WARNING", ""));
        }
        if (!flag)
          return;
        try
        {
          if (LogCollectorUtils.sProblemCategories != null)
            index = (string) LogCollectorUtils.sStringConversions[(object) index];
          if (!this.mQuickLogs || index != null && LogCollectorUtils.sCategorySubcategoryMapping.ContainsKey(index) && (this.mSubCategoryCombobox.Visibility == Visibility.Visible && this.mSubCategoryCombobox.SelectedItem != null))
          {
            string str3 = ((ContentControl) this.mSubCategoryCombobox.SelectedItem).Content.ToString();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Dictionary<string, string> HT = LogCollectorUtils.sCategorySubcategoryMapping[index];
            subcategory = MainWindow.FindKey(str3, HT);
          }
          this.ShowInTaskbar = false;
          this.Visibility = Visibility.Hidden;
          using (new CollectLogs(text1, index, appName, appPkgName, text2, subcategory, (ICollection<string>) attachedFiles, this.mAlIOemsCheckbox.IsChecked.GetValueOrDefault(), this.mStartAllOemsCheckbox.IsChecked.GetValueOrDefault()))
          {
            CollectLogs.sProgressWindow.Owner = (Window) this;
            CollectLogs.sProgressWindow.ShowDialog();
            Application.Current.Shutdown();
          }
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured after areMandatoryFieldsCorrect, Err : {0}", (object) ex.ToString()));
          Application.Current.Shutdown();
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured, Err : {0}", (object) ex.ToString()));
        Application.Current.Shutdown();
      }
    }

    private void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((Window) this);
        this.mToastPopup.Init((Window) this, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    private bool ValidateEmail(string email)
    {
      return new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*").IsMatch(email);
    }

    private void mCategoryComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mCategoryComboBox.BorderBrush = (Brush) new BrushConverter().ConvertFrom((object) LogCollectorUtils.sDefaultBorderColorHex);
    }

    private void mSubCategoryCombobox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mSubCategoryCombobox.BorderBrush = (Brush) new BrushConverter().ConvertFrom((object) LogCollectorUtils.sDefaultBorderColorHex);
    }

    private void mAppListComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mAppListComboBox.BorderBrush = (Brush) new BrushConverter().ConvertFrom((object) LogCollectorUtils.sDefaultBorderColorHex);
    }

    private void HighlightComboBoxColor(CustomComboBox cmbBox)
    {
      cmbBox.BorderBrush = (Brush) new BrushConverter().ConvertFrom((object) "Red");
    }

    private void mRemoveSSBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.AttachedFiles.Remove((sender is CustomPictureBox customPictureBox ? customPictureBox.DataContext : (object) null) as string);
    }

    private void DescribeProblemTextBox_GotKeyboardFocus(
      object sender,
      KeyboardFocusChangedEventArgs e)
    {
      this.mDescribeProblemTextBox.InputTextValidity = TextValidityOptions.Success;
    }

    private void EmailTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.mEmailTextBox.InputTextValidity = TextValidityOptions.Success;
    }

    private void TitleMouseButonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType() == typeof (CustomPictureBox))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void mAllInstancesCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
      this.mStartAllOemsCheckbox.IsChecked = new bool?(false);
    }

    private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetHelpArticleURL("log_collector_all_instances_help"));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-LogCollector;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          this.mBorder = (Border) target;
          break;
        case 3:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.TitleMouseButonDown);
          break;
        case 4:
          this.CloseBtn = (CustomPictureBox) target;
          this.CloseBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        case 5:
          this.label = (TextBlock) target;
          break;
        case 6:
          this.mGifGrid = (StackPanel) target;
          break;
        case 7:
          this.mLoadingImage = (Image) target;
          break;
        case 8:
          this.mCategoryStackPanel = (StackPanel) target;
          break;
        case 9:
          this.mCategoryTextBlock = (TextBlock) target;
          break;
        case 10:
          this.mCategoryComboBox = (CustomComboBox) target;
          this.mCategoryComboBox.PreviewMouseDown += new MouseButtonEventHandler(this.mCategoryComboBox_PreviewMouseDown);
          this.mCategoryComboBox.SelectionChanged += new SelectionChangedEventHandler(this.CategoryChanged);
          break;
        case 11:
          this.mSubCategoryStackPanel = (StackPanel) target;
          break;
        case 12:
          this.mSubCategoryTextBlock = (TextBlock) target;
          break;
        case 13:
          this.mSubCategoryCombobox = (CustomComboBox) target;
          this.mSubCategoryCombobox.PreviewMouseDown += new MouseButtonEventHandler(this.mSubCategoryCombobox_PreviewMouseDown);
          this.mSubCategoryCombobox.SelectionChanged += new SelectionChangedEventHandler(this.SubCategoryChanged);
          break;
        case 14:
          this.mAppDetailsTextBlock = (TextBlock) target;
          break;
        case 15:
          this.mAppNameTitle = (TextBlock) target;
          break;
        case 16:
          this.mAppListComboBox = (CustomComboBox) target;
          this.mAppListComboBox.PreviewMouseDown += new MouseButtonEventHandler(this.mAppListComboBox_PreviewMouseDown);
          break;
        case 17:
          this.mAppNameGrid = (Grid) target;
          break;
        case 18:
          this.mAppNameTextBox = (CustomTextBox) target;
          break;
        case 19:
          this.mDescribeProblemTextBlock = (TextBlock) target;
          break;
        case 20:
          this.mDescribeProblemTextBox = (CustomTextBox) target;
          this.mDescribeProblemTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.DescribeProblemTextBox_GotKeyboardFocus);
          break;
        case 21:
          this.mChooseButton = (CustomButton) target;
          this.mChooseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mChooseButton_PreviewMouseLeftButtonUp);
          break;
        case 22:
          this.mAttachedFilesListBox = (ListBox) target;
          break;
        case 23:
          this.mEmailTextBlock = (TextBlock) target;
          break;
        case 24:
          this.mEmailTextBox = (CustomTextBox) target;
          this.mEmailTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.EmailTextBox_GotKeyboardFocus);
          break;
        case 25:
          this.mAlIOemsCheckbox = (CustomCheckbox) target;
          this.mAlIOemsCheckbox.Unchecked += new RoutedEventHandler(this.mAllInstancesCheckbox_Unchecked);
          break;
        case 26:
          this.mHelpIcon = (CustomPictureBox) target;
          this.mHelpIcon.MouseDown += new MouseButtonEventHandler(this.CustomPictureBox_MouseDown);
          break;
        case 27:
          this.mStartAllOemsCheckbox = (CustomCheckbox) target;
          break;
        case 28:
          this.mBtn = (CustomButton) target;
          this.mBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mSubmitBtn_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
        return;
      ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mRemoveSSBtn_PreviewMouseLeftButtonUp);
    }
  }
}
