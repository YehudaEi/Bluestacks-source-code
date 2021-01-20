// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ShortcutKeysControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class ShortcutKeysControl : UserControl, IComponentConnector
  {
    private Dictionary<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> mShortcutUIElements = new Dictionary<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>();
    private MainWindow ParentWindow;
    private SettingsWindow ParentSettingsWindow;
    private StackPanel mShortcutKeyPanel;
    private CustomToastPopupControl mToastPopup;
    internal ScrollViewer mShortcutKeyScrollBar;
    internal CustomButton mRevertBtn;
    internal CustomButton mSaveBtn;
    private bool _contentLoaded;

    public ShortcutKeysControl(MainWindow window, SettingsWindow settingsWindow)
    {
      this.InitializeComponent();
      this.Visibility = Visibility.Hidden;
      this.ParentWindow = window;
      this.ParentSettingsWindow = settingsWindow;
      this.mShortcutKeyPanel = this.mShortcutKeyScrollBar.Content as StackPanel;
      if (this.ParentWindow != null)
      {
        if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
          this.AddShortcutKeyElements();
        if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
          this.mRevertBtn.IsEnabled = true;
        this.ParentWindow.mCommonHandler.ShortcutKeysChangedEvent += new CommonHandlers.ShortcutKeysChanged(this.ShortcutKeysChangedEvent);
        this.ParentWindow.mCommonHandler.ShortcutKeysRefreshEvent += new CommonHandlers.ShortcutKeysRefresh(this.ShortcutKeysRefreshEvent);
      }
      this.mShortcutKeyScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
    }

    private void ShortcutKeysRefreshEvent()
    {
      IEnumerable<ShortcutKeyControlElement> keyControlElements = this.mShortcutUIElements.SelectMany<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>, ShortcutKeyControlElement>((Func<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>, IEnumerable<ShortcutKeyControlElement>>) (x => (IEnumerable<ShortcutKeyControlElement>) x.Value.Item2)).GroupBy<ShortcutKeyControlElement, string>((Func<ShortcutKeyControlElement, string>) (ele => ele.mShortcutKeyTextBox.Text)).Where<IGrouping<string, ShortcutKeyControlElement>>((Func<IGrouping<string, ShortcutKeyControlElement>, bool>) (grp => grp.Count<ShortcutKeyControlElement>() == 1)).SelectMany<IGrouping<string, ShortcutKeyControlElement>, ShortcutKeyControlElement>((Func<IGrouping<string, ShortcutKeyControlElement>, IEnumerable<ShortcutKeyControlElement>>) (grp => (IEnumerable<ShortcutKeyControlElement>) grp)).Where<ShortcutKeyControlElement>((Func<ShortcutKeyControlElement, bool>) (ele => !string.IsNullOrEmpty(ele.mShortcutKeyTextBox.Text)));
      int num = 0;
      foreach (ShortcutKeyControlElement keyControlElement in keyControlElements)
      {
        if (!keyControlElement.mIsShortcutSameAsMacroShortcut)
        {
          keyControlElement.mKeyInfoPopup.IsOpen = false;
          keyControlElement.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Success;
          ++num;
        }
      }
      if (num == this.mShortcutUIElements.SelectMany<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>, ShortcutKeyControlElement>((Func<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>, IEnumerable<ShortcutKeyControlElement>>) (x => (IEnumerable<ShortcutKeyControlElement>) x.Value.Item2)).Where<ShortcutKeyControlElement>((Func<ShortcutKeyControlElement, bool>) (ele => !string.IsNullOrEmpty(ele.mShortcutKeyTextBox.Text))).Count<ShortcutKeyControlElement>())
        this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(true);
      else
        this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(false);
    }

    private void ShortcutKeysChangedEvent(bool isEnabled)
    {
      this.mSaveBtn.IsEnabled = isEnabled;
    }

    private void AddShortcutKeyElements()
    {
      try
      {
        List<ShortcutKeys> shortcutKeysList = new List<ShortcutKeys>();
        this.mShortcutKeyPanel.Children.Clear();
        this.mShortcutUIElements.Clear();
        foreach (ShortcutKeys ele in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
        {
          this.CreateShortcutCategory(ele.ShortcutCategory);
          this.AddElement(ele);
        }
        foreach (KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> shortcutUiElement in this.mShortcutUIElements)
        {
          this.mShortcutKeyPanel.Children.Add((UIElement) shortcutUiElement.Value.Item1);
          foreach (FrameworkElement frameworkElement in shortcutUiElement.Value.Item2)
            (shortcutUiElement.Value.Item1.Content as StackPanel).Children.Add((UIElement) frameworkElement);
        }
        this.mShortcutUIElements.First<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>>().Value.Item1.Margin = new Thickness(0.0);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in adding shortcut elements: " + ex.ToString());
      }
    }

    private void AddElement(ShortcutKeys ele)
    {
      ShortcutKeyControlElement keyControlElement = new ShortcutKeyControlElement(this.ParentWindow, this.ParentSettingsWindow);
      BlueStacksUIBinding.Bind(keyControlElement.mShortcutNameTextBlock, ele.ShortcutName, "");
      string[] strArray = ele.ShortcutKey.Split(new char[2]
      {
        '+',
        ' '
      }, StringSplitOptions.RemoveEmptyEntries);
      string str = string.Empty;
      foreach (string key in strArray)
        str = str + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(key), "") + " + ";
      this.mShortcutUIElements[ele.ShortcutCategory].Item2.Add(keyControlElement);
      if (!string.IsNullOrEmpty(str))
        keyControlElement.mShortcutKeyTextBox.Text = str.Substring(0, str.Length - 3);
      keyControlElement.mUserDefinedConfigList = new List<ShortcutKeys>()
      {
        ele
      };
      if (!ele.ReadOnlyTextbox)
        return;
      keyControlElement.mShortcutKeyTextBox.IsEnabled = false;
    }

    private void CreateShortcutCategory(string categoryName)
    {
      if (this.mShortcutUIElements.ContainsKey(categoryName))
        return;
      string localizedString = LocaleStrings.GetLocalizedString(categoryName, "");
      GroupBox groupBox1 = new GroupBox();
      groupBox1.Content = (object) new StackPanel();
      groupBox1.Header = (object) localizedString;
      groupBox1.Tag = (object) categoryName;
      groupBox1.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
      groupBox1.FontSize = 16.0;
      GroupBox groupBox2 = groupBox1;
      BlueStacksUIBinding.BindColor((DependencyObject) groupBox2, Control.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
      groupBox2.BorderThickness = new Thickness(0.0);
      TextBlock textBlock = new TextBlock();
      textBlock.Text = localizedString;
      textBlock.Tag = (object) categoryName;
      textBlock.FontStretch = FontStretches.ExtraExpanded;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
      textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
      BlueStacksUIBinding.BindColor((DependencyObject) textBlock, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
      this.mShortcutUIElements.Add(categoryName, new BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>(groupBox2, new List<ShortcutKeyControlElement>()));
    }

    private void SaveBtnClick(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      this.ParentSettingsWindow.mIsShortcutEdited = false;
      this.mSaveBtn.IsEnabled = false;
      this.mRevertBtn.IsEnabled = true;
      if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut != null)
        this.RefreshShortcutConfigForUI();
      this.ParentWindow.mTopbarOptions?.SetLabel();
      ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Shortcut-Settings", "", (string) null, this.ParentWindow.mVmName, (string) null, (string) null, "Android");
    }

    private void RefreshShortcutConfigForUI()
    {
      foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
      {
        foreach (ShortcutKeyControlElement keyControlElement1 in this.mShortcutUIElements[shortcutKeys.ShortcutCategory].Item2)
        {
          if (keyControlElement1 is ShortcutKeyControlElement keyControlElement && string.Equals(keyControlElement.mShortcutNameTextBlock.Text, LocaleStrings.GetLocalizedString(shortcutKeys.ShortcutName, ""), StringComparison.InvariantCulture))
            keyControlElement.mUserDefinedConfigList = new List<ShortcutKeys>()
            {
              shortcutKeys
            };
        }
      }
    }

    private void RevertBtnClick(object sender, RoutedEventArgs e)
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_DEFAULTS", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_SHORTCUTS", "");
      customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_RESTORE_BUTTON", ""), (EventHandler) ((o, evt) =>
      {
        this.RestoreDefaultShortcuts();
        this.mRevertBtn.IsEnabled = false;
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.Owner = (Window) this.ParentWindow;
      customMessageWindow.ShowDialog();
    }

    private void RestoreDefaultShortcuts()
    {
      RegistryManager.Instance.UserDefinedShortcuts = string.Empty;
      this.ParentSettingsWindow.mIsShortcutEdited = false;
      CommonHandlers.ReloadShortcutsForAllInstances();
      if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
        this.AddShortcutKeyElements();
      this.mSaveBtn.IsEnabled = false;
      this.ParentWindow.mTopbarOptions?.SetLabel();
      BlueStacks.Common.Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_restore_default", (string) null, (string) null, (string) null, (string) null, (string) null, "Android", 0);
    }

    private void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((UserControl) this);
        this.mToastPopup.Init((Window) this.ParentWindow, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeyscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mShortcutKeyScrollBar = (ScrollViewer) target;
          break;
        case 2:
          this.mRevertBtn = (CustomButton) target;
          this.mRevertBtn.Click += new RoutedEventHandler(this.RevertBtnClick);
          break;
        case 3:
          this.mSaveBtn = (CustomButton) target;
          this.mSaveBtn.Click += new RoutedEventHandler(this.SaveBtnClick);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
