// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ComboBoxSchemeControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
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
  public class ComboBoxSchemeControl : UserControl, IComponentConnector
  {
    private KeymapCanvasWindow CanvasWindow;
    private MainWindow ParentWindow;
    private CustomMessageWindow mDeleteScriptMessageWindow;
    internal string mOldSchemeName;
    internal Grid mSchemeControl;
    internal CustomPictureBox mBookmarkImg;
    internal CustomTextBox mSchemeName;
    internal CustomPictureBox mEditImg;
    internal CustomPictureBox mSaveImg;
    internal CustomPictureBox mCopyImg;
    internal CustomPictureBox mDeleteImg;
    private bool _contentLoaded;

    public ComboBoxSchemeControl(KeymapCanvasWindow window, MainWindow mainWindow)
    {
      this.CanvasWindow = window;
      this.ParentWindow = mainWindow;
      this.InitializeComponent();
    }

    private void Bookmark_img_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.mSchemeName.IsReadOnly)
        this.HandleNameEdit(this);
      if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(this.mSchemeName.Text))
      {
        IMControlScheme imControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text];
        if (imControlScheme.IsBookMarked)
        {
          imControlScheme.IsBookMarked = false;
          this.mBookmarkImg.ImageName = "bookmark";
        }
        else
        {
          List<IMControlScheme> controlSchemes = this.ParentWindow.SelectedConfig.ControlSchemes;
          if ((controlSchemes != null ? (controlSchemes.Count<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.IsBookMarked)) < 5 ? 1 : 0) : 0) != 0)
          {
            imControlScheme.IsBookMarked = true;
            this.mBookmarkImg.ImageName = "bookmarked";
          }
          else
            this.CanvasWindow.SidebarWindow.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_BOOKMARK_SCHEMES_WARNING", ""));
        }
        this.CanvasWindow.SidebarWindow.FillProfileCombo();
        KeymapCanvasWindow.sIsDirty = true;
      }
      e.Handled = true;
    }

    private void EditImg_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.mEditImg.Visibility = Visibility.Collapsed;
      this.mSaveImg.Visibility = Visibility.Visible;
      this.mOldSchemeName = this.mSchemeName.Text;
      this.mSchemeName.Focusable = true;
      this.mSchemeName.IsReadOnly = false;
      this.mSchemeName.CaretIndex = this.mSchemeName.Text.Length;
      this.mSchemeName.Focus();
      e.Handled = true;
    }

    private void SaveImg_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.HandleNameEdit(this);
      e.Handled = true;
    }

    private bool EditedNameIsAllowed(string text, ComboBoxSchemeControl toBeRenamedControl)
    {
      if (string.IsNullOrEmpty(text.Trim()))
      {
        this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), 1.3, false);
        return false;
      }
      foreach (ComboBoxSchemeControl child in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
      {
        if (child.mSchemeName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && child != toBeRenamedControl)
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), 1.3, false);
          return false;
        }
        if (child.mSchemeName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
          this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.CanvasWindow.SidebarWindow, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) LocaleStrings.GetLocalizedString("STRING_SCHEME_INVALID_CHARACTERS", ""), (object) Environment.NewLine, (object) "\\ / : * ? \" < > |"), 3.0, false);
          return false;
        }
      }
      return true;
    }

    private void CopyImg_MouseDown(object sender, MouseButtonEventArgs e)
    {
      bool flag = false;
      foreach (ComboBoxSchemeControl child in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
      {
        if (!child.mSchemeName.IsReadOnly)
        {
          this.HandleNameEdit(child);
          flag = true;
          e.Handled = true;
          break;
        }
      }
      if (!flag)
        KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text], true);
      e.Handled = true;
    }

    private void DeleteImg_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.mSchemeName.IsReadOnly)
        this.HandleNameEdit(this);
      if (!this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup)
      {
        this.DeleteControlScheme();
        e.Handled = true;
      }
      else
      {
        this.mDeleteScriptMessageWindow = new CustomMessageWindow();
        this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME", "");
        this.mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME_CONFIRMATION", "");
        this.mDeleteScriptMessageWindow.CheckBox.Content = (object) LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
        this.mDeleteScriptMessageWindow.CheckBox.Visibility = Visibility.Visible;
        this.mDeleteScriptMessageWindow.CheckBox.IsChecked = new bool?(false);
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), new EventHandler(this.UpdateSettingsAndDeleteScheme), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler) ((o, evt) =>
        {
          KeymapCanvasWindow.sIsDirty = false;
          GuidanceWindow.sIsDirty = false;
        }), (string) null, false, (object) null, true);
        this.mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler) ((o, evt) => {}), (object) null);
        this.mDeleteScriptMessageWindow.Owner = (Window) this.CanvasWindow;
        this.mDeleteScriptMessageWindow.ShowDialog();
        e.Handled = true;
      }
    }

    private void UpdateSettingsAndDeleteScheme(object sender, EventArgs e)
    {
      this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value;
      this.mDeleteScriptMessageWindow = (CustomMessageWindow) null;
      this.DeleteControlScheme();
    }

    private void DeleteControlScheme()
    {
      if (!this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(this.mSchemeName.Text) || this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].BuiltIn)
        return;
      if (this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected)
      {
        this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected = false;
        if (this.ParentWindow.SelectedConfig.ControlSchemes.Count > 1)
        {
          this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = !(this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl).mSchemeName.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture)) ? (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl).mSchemeName.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[1] as ComboBoxSchemeControl).mSchemeName.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = true;
          this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem;
          this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem];
          this.CanvasWindow.SidebarWindow.ProfileChanged();
        }
        else
        {
          this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = (string) null;
          BlueStacksUIBinding.Bind(this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
        }
      }
      this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text]);
      this.ParentWindow.SelectedConfig.ControlSchemesDict.Remove(this.mSchemeName.Text);
      ComboBoxSchemeControl schemeControlFromName = KMManager.GetComboBoxSchemeControlFromName(this.mSchemeName.Text);
      if (schemeControlFromName != null)
        this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Remove((UIElement) schemeControlFromName);
      KeymapCanvasWindow.sIsDirty = true;
      this.CanvasWindow.SidebarWindow.FillProfileCombo();
      if (this.ParentWindow.SelectedConfig.ControlSchemes.Count != 0)
        return;
      this.CanvasWindow.ClearWindow();
    }

    private void ComboBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      bool flag = false;
      foreach (ComboBoxSchemeControl child in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
      {
        if (!child.mSchemeName.IsReadOnly)
        {
          this.HandleNameEdit(child);
          flag = true;
          e.Handled = true;
          break;
        }
      }
      if (flag)
        return;
      if (this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == this.mSchemeName.Text)
      {
        this.CanvasWindow.SidebarWindow.mSchemeComboBox.mItems.IsOpen = false;
      }
      else
      {
        if (this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem != null)
          this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = false;
        this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected = true;
        this.ParentWindow.SelectedConfig.ControlSchemesDict[this.ParentWindow.SelectedConfig.SelectedControlScheme.Name].Selected = false;
        this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text];
        this.CanvasWindow.SidebarWindow.FillProfileCombo();
        this.CanvasWindow.SidebarWindow.ProfileChanged();
        this.CanvasWindow.SidebarWindow.mSchemeComboBox.mItems.IsOpen = false;
        KeymapCanvasWindow.sIsDirty = true;
        KMManager.SendSchemeChangedStats(this.ParentWindow, "controls_editor");
      }
    }

    private void ComboBoxItem_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void ComboBoxItem_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mSchemeName.Text != this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem)
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "ComboBoxBackgroundColor");
      else
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
    }

    private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.mSchemeName.Focusable)
        this.HandleNameEdit(this);
      e.Handled = true;
    }

    private void HandleNameEdit(ComboBoxSchemeControl control)
    {
      control.mEditImg.Visibility = Visibility.Visible;
      control.mSaveImg.Visibility = Visibility.Collapsed;
      if (this.EditedNameIsAllowed(control.mSchemeName.Text, control))
      {
        if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(control.mOldSchemeName))
        {
          IMControlScheme imControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[control.mOldSchemeName];
          imControlScheme.Name = control.mSchemeName.Text.Trim();
          this.ParentWindow.SelectedConfig.ControlSchemesDict.Remove(control.mOldSchemeName);
          this.ParentWindow.SelectedConfig.ControlSchemesDict.Add(imControlScheme.Name, imControlScheme);
          this.CanvasWindow.SidebarWindow.FillProfileCombo();
          KeymapCanvasWindow.sIsDirty = true;
        }
      }
      else
        control.mSchemeName.Text = control.mOldSchemeName;
      control.mSchemeName.Focusable = false;
      control.mSchemeName.IsReadOnly = true;
    }

    private void MSchemeName_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Return)
        return;
      this.HandleNameEdit(this);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/comboboxschemecontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.ComboBoxItem_MouseDown);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.ComboBoxItem_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.ComboBoxItem_MouseLeave);
          ((UIElement) target).LostFocus += new RoutedEventHandler(this.ComboBoxItem_LostFocus);
          break;
        case 2:
          this.mSchemeControl = (Grid) target;
          break;
        case 3:
          this.mBookmarkImg = (CustomPictureBox) target;
          this.mBookmarkImg.MouseDown += new MouseButtonEventHandler(this.Bookmark_img_MouseDown);
          break;
        case 4:
          this.mSchemeName = (CustomTextBox) target;
          this.mSchemeName.KeyUp += new KeyEventHandler(this.MSchemeName_KeyUp);
          break;
        case 5:
          this.mEditImg = (CustomPictureBox) target;
          this.mEditImg.MouseDown += new MouseButtonEventHandler(this.EditImg_MouseDown);
          break;
        case 6:
          this.mSaveImg = (CustomPictureBox) target;
          this.mSaveImg.MouseDown += new MouseButtonEventHandler(this.SaveImg_MouseDown);
          break;
        case 7:
          this.mCopyImg = (CustomPictureBox) target;
          this.mCopyImg.MouseDown += new MouseButtonEventHandler(this.CopyImg_MouseDown);
          break;
        case 8:
          this.mDeleteImg = (CustomPictureBox) target;
          this.mDeleteImg.MouseDown += new MouseButtonEventHandler(this.DeleteImg_MouseDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
