// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ShortcutKeyControlElement
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class ShortcutKeyControlElement : UserControl, IComponentConnector
  {
    internal string mDefaultModifierForUI = string.Empty;
    internal string mDefaultModifierForFile = string.Empty;
    internal MainWindow ParentWindow;
    internal SettingsWindow ParentSettingsWindow;
    private bool mErrorMessageShown;
    internal bool mIsShortcutSameAsMacroShortcut;
    private CustomToastPopupControl mToastPopup;
    internal TextBlock mShortcutNameTextBlock;
    internal CustomTextBox mShortcutKeyTextBox;
    internal CustomPopUp mKeyInfoPopup;
    internal Border mMaskBorder;
    internal TextBlock mKeyInfoText;
    internal Path mDownArrow;
    private bool _contentLoaded;

    internal List<ShortcutKeys> mUserDefinedConfigList { get; set; }

    public ShortcutKeyControlElement(MainWindow window, SettingsWindow settingsWindow)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.ParentSettingsWindow = settingsWindow;
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.mShortcutKeyTextBox, false);
      MainWindow parentWindow = this.ParentWindow;
      string[] strArray;
      if (parentWindow == null)
        strArray = (string[]) null;
      else
        strArray = parentWindow.mCommonHandler.mShortcutsConfigInstance.DefaultModifier.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string str in strArray)
      {
        Key key = (Key) Enum.Parse(typeof (Key), str);
        this.mDefaultModifierForUI = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(key) + " + ";
        this.mDefaultModifierForFile = this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(key) + " + ";
      }
    }

    private static bool IsValid(Key key)
    {
      return key != Key.LeftAlt && key != Key.RightAlt && (key != Key.LeftShift && key != Key.RightShift) && (key != Key.LeftCtrl && key != Key.RightCtrl && (key != Key.None && key != Key.System));
    }

    private void ShortcutKeyTextBoxKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Snapshot && e.SystemKey != Key.Snapshot)
        return;
      this.HandleShortcutKeyDown(e);
    }

    private void ShortcutKeyTextBoxKeyDown(object sender, KeyEventArgs e)
    {
      this.HandleShortcutKeyDown(e);
    }

    private void HandleShortcutKeyDown(KeyEventArgs e)
    {
      Logger.Debug("SHORTCUT: PrintKey............" + e.Key.ToString());
      Logger.Debug("SHORTCUT: PrintSystemKey............" + e.SystemKey.ToString());
      if ((!IMAPKeys.mDictKeys.ContainsKey(e.Key) && !IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || !ShortcutKeyControlElement.IsValid(e.Key) && !ShortcutKeyControlElement.IsValid(e.SystemKey)) && (e.Key != Key.Back && e.Key != Key.Delete))
        return;
      string str1 = string.Empty;
      string str2 = string.Empty;
      string str3 = string.Empty;
      this.mShortcutKeyTextBox.Tag = (object) string.Empty;
      if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
      {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
          str1 = IMAPKeys.GetStringForUI(Key.LeftCtrl) + " + ";
          this.mShortcutKeyTextBox.Tag = (object) (IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ");
        }
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
        {
          str2 = IMAPKeys.GetStringForUI(Key.LeftAlt) + " + ";
          CustomTextBox shortcutKeyTextBox = this.mShortcutKeyTextBox;
          shortcutKeyTextBox.Tag = (object) (shortcutKeyTextBox.Tag?.ToString() + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ");
        }
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
        {
          str3 = IMAPKeys.GetStringForUI(Key.LeftShift) + " + ";
          CustomTextBox shortcutKeyTextBox = this.mShortcutKeyTextBox;
          shortcutKeyTextBox.Tag = (object) (shortcutKeyTextBox.Tag?.ToString() + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ");
        }
        if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2) || e.SystemKey == Key.F10)
        {
          this.mShortcutKeyTextBox.Text = str1 + str2 + str3 + IMAPKeys.GetStringForUI(e.SystemKey);
          CustomTextBox shortcutKeyTextBox = this.mShortcutKeyTextBox;
          shortcutKeyTextBox.Tag = (object) (shortcutKeyTextBox.Tag?.ToString() + IMAPKeys.GetStringForFile(e.SystemKey));
        }
        else
        {
          this.mShortcutKeyTextBox.Text = str1 + str2 + str3 + IMAPKeys.GetStringForUI(e.Key);
          CustomTextBox shortcutKeyTextBox = this.mShortcutKeyTextBox;
          shortcutKeyTextBox.Tag = (object) (shortcutKeyTextBox.Tag?.ToString() + IMAPKeys.GetStringForFile(e.Key));
        }
      }
      else if (e.Key == Key.Back || e.Key == Key.Delete)
      {
        this.mShortcutKeyTextBox.Text = string.Empty;
        this.mShortcutKeyTextBox.Tag = (object) string.Empty;
        if (this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
          this.ParentSettingsWindow.mDuplicateShortcutsList.Remove(this.mShortcutNameTextBlock.Text);
        this.SetSaveButtonState(this.ParentSettingsWindow.mIsShortcutEdited);
      }
      else if (e.Key == Key.Escape)
      {
        if (string.Equals(this.mDefaultModifierForFile, "Shift + ", StringComparison.InvariantCulture))
        {
          this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
          this.mShortcutKeyTextBox.Tag = (object) (this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key));
        }
      }
      else if ((e.Key == Key.D0 || e.SystemKey == Key.D0) && string.Equals(this.mDefaultModifierForUI, "Ctrl + Shift + ", StringComparison.InvariantCulture))
      {
        this.mShortcutKeyTextBox.Text = string.Empty;
        this.mShortcutKeyTextBox.Tag = (object) string.Empty;
        this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
      }
      else if (e.Key == Key.System)
      {
        this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.SystemKey);
        this.mShortcutKeyTextBox.Tag = (object) (this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.SystemKey));
      }
      else
      {
        this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
        this.mShortcutKeyTextBox.Tag = (object) (this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key));
      }
      e.Handled = true;
      this.mShortcutKeyTextBox.CaretIndex = this.mShortcutKeyTextBox.Text.Length;
      this.mIsShortcutSameAsMacroShortcut = false;
      if ((MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.Key)) || MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.SystemKey))) && (string.Equals(this.mShortcutKeyTextBox.Text, str1 + str2 + IMAPKeys.GetStringForUI(e.Key), StringComparison.InvariantCulture) || string.Equals(this.mShortcutKeyTextBox.Text, str1 + str2 + IMAPKeys.GetStringForUI(e.SystemKey), StringComparison.InvariantCulture)))
        this.mIsShortcutSameAsMacroShortcut = true;
      if (string.Equals(this.mShortcutKeyTextBox.Text, "Alt + F4", StringComparison.InvariantCulture))
      {
        this.mShortcutKeyTextBox.Text = string.Empty;
        this.mShortcutKeyTextBox.Tag = (object) string.Empty;
        this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
      }
      foreach (ShortcutKeys userDefinedConfig in this.mUserDefinedConfigList)
      {
        this.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Success;
        this.mKeyInfoPopup.IsOpen = false;
        this.ParentSettingsWindow.mIsShortcutEdited = true;
        this.CheckIfShortcutAlreadyUsed();
        this.ParentWindow.mCommonHandler.OnShortcutKeysRefresh();
        if (string.Equals(LocaleStrings.GetLocalizedString(userDefinedConfig.ShortcutName, ""), this.mShortcutNameTextBlock.Text, StringComparison.InvariantCulture) && !string.Equals(userDefinedConfig.ShortcutKey, this.mShortcutKeyTextBox.Text, StringComparison.InvariantCulture))
        {
          userDefinedConfig.ShortcutKey = this.mShortcutKeyTextBox.Tag.ToString();
          BlueStacks.Common.Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_edit", this.mShortcutNameTextBlock.Text, (string) null, (string) null, (string) null, (string) null, "Android", 0);
        }
      }
    }

    private void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((UserControl) this.ParentSettingsWindow);
        this.mToastPopup.Init((UserControl) this.ParentSettingsWindow, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Top, new Thickness?(), 12, new Thickness?(), (Brush) null);
        this.mToastPopup.Margin = new Thickness(20.0, 30.0, 0.0, 0.0);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    private void CheckIfShortcutAlreadyUsed()
    {
      this.mErrorMessageShown = false;
      foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
      {
        if (!string.IsNullOrEmpty(shortcutKeys.ShortcutKey) && string.Equals(shortcutKeys.ShortcutKey, this.mShortcutKeyTextBox.Tag.ToString(), StringComparison.InvariantCulture) && !string.Equals(LocaleStrings.GetLocalizedString(shortcutKeys.ShortcutName, ""), this.mShortcutNameTextBlock.Text, StringComparison.InvariantCulture) || this.mIsShortcutSameAsMacroShortcut)
        {
          this.mKeyInfoPopup.PlacementTarget = (UIElement) this.mShortcutKeyTextBox;
          this.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Error;
          this.mKeyInfoPopup.IsOpen = true;
          this.mErrorMessageShown = true;
          if (!this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
            this.ParentSettingsWindow.mDuplicateShortcutsList.Add(this.mShortcutNameTextBlock.Text);
        }
      }
      if (!this.mErrorMessageShown && this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
        this.ParentSettingsWindow.mDuplicateShortcutsList.Remove(this.mShortcutNameTextBlock.Text);
      this.SetSaveButtonState(this.ParentSettingsWindow.mIsShortcutEdited);
    }

    private void SetSaveButtonState(bool isEdited)
    {
      if (this.ParentSettingsWindow.mDuplicateShortcutsList.Count == 0 & isEdited)
      {
        this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(true);
        this.ParentSettingsWindow.mIsShortcutSaveBtnEnabled = true;
      }
      else
      {
        this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(false);
        this.ParentSettingsWindow.mIsShortcutSaveBtnEnabled = false;
      }
    }

    private void ShortcutKeyTextBoxMouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mShortcutKeyTextBox.InputTextValidity == TextValidityOptions.Error)
      {
        this.mKeyInfoPopup.PlacementTarget = (UIElement) this.mShortcutKeyTextBox;
        this.mKeyInfoPopup.IsOpen = true;
        this.mKeyInfoPopup.StaysOpen = true;
      }
      else
        this.mKeyInfoPopup.IsOpen = false;
    }

    private void ShortcutKeyTextBoxMouseLeave(object sender, MouseEventArgs e)
    {
      this.mKeyInfoPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeycontrolelement.xaml", UriKind.Relative));
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
          this.mShortcutNameTextBlock = (TextBlock) target;
          break;
        case 2:
          this.mShortcutKeyTextBox = (CustomTextBox) target;
          this.mShortcutKeyTextBox.PreviewKeyDown += new KeyEventHandler(this.ShortcutKeyTextBoxKeyDown);
          this.mShortcutKeyTextBox.MouseEnter += new MouseEventHandler(this.ShortcutKeyTextBoxMouseEnter);
          this.mShortcutKeyTextBox.MouseLeave += new MouseEventHandler(this.ShortcutKeyTextBoxMouseLeave);
          this.mShortcutKeyTextBox.PreviewKeyUp += new KeyEventHandler(this.ShortcutKeyTextBoxKeyUp);
          break;
        case 3:
          this.mKeyInfoPopup = (CustomPopUp) target;
          break;
        case 4:
          this.mMaskBorder = (Border) target;
          break;
        case 5:
          this.mKeyInfoText = (TextBlock) target;
          break;
        case 6:
          this.mDownArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
