// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.OtsFeedbackControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class OtsFeedbackControl : UserControl, IComponentConnector
  {
    private static List<string> m_Phone_Patterns = new List<string>()
    {
      "^[\\d\\s-\\+]{5,15}$"
    };
    internal CustomPictureBox mCloseBtn;
    internal TextBox txtDescIssue;
    internal Border txtEmailBorder;
    internal TextBox txtEmail;
    internal Border txtPhoneBorder;
    internal TextBox txtPhone;
    private bool _contentLoaded;

    public MainWindow ParentWindow { get; set; }

    public OtsFeedbackControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (!this.TestEmail(this.txtEmail.Text) || !this.TestPhone(this.txtPhone.Text))
          return;
        ClientStats.SendMiscellaneousStatsAsync("OTSFeedback", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.txtDescIssue.Text, this.txtEmail.Text, this.txtPhone.Text, (string) null, (string) null, (string) null, "Android");
        new Thread((ThreadStart) (() =>
        {
          try
          {
            new Process()
            {
              StartInfo = {
                Arguments = "-silent",
                FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe")
              }
            }.Start();
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in starting HD-logCollector.exe: " + ex.ToString());
          }
        }))
        {
          IsBackground = true
        }.Start();
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.ImageName = "help";
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_THANK_YOU", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_APPRECIATE_FEEDBACK", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this.ParentWindow;
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Submitting ots feedback " + ex.ToString());
      }
    }

    private bool TestEmail(string text)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.txtEmailBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
      if (Regex.IsMatch(text, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase))
        return true;
      this.txtEmailBorder.BorderBrush = (Brush) Brushes.Red;
      return false;
    }

    private bool TestPhone(string text)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.txtPhoneBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
      if (Regex.IsMatch(text, OtsFeedbackControl.MakeCombinedPattern((IEnumerable<string>) OtsFeedbackControl.m_Phone_Patterns), RegexOptions.IgnoreCase))
        return true;
      this.txtPhoneBorder.BorderBrush = (Brush) Brushes.Red;
      return false;
    }

    private static string MakeCombinedPattern(IEnumerable<string> patterns)
    {
      return string.Join("|", patterns.Select<string, string>((Func<string, string>) (item => "(" + item + ")")).ToArray<string>());
    }

    private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.TestEmail(this.txtEmail.Text);
    }

    private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.TestPhone(this.txtPhone.Text);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/otsfeedbackcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        case 2:
          this.txtDescIssue = (TextBox) target;
          break;
        case 3:
          this.txtEmailBorder = (Border) target;
          break;
        case 4:
          this.txtEmail = (TextBox) target;
          this.txtEmail.TextChanged += new TextChangedEventHandler(this.txtEmail_TextChanged);
          break;
        case 5:
          this.txtPhoneBorder = (Border) target;
          break;
        case 6:
          this.txtPhone = (TextBox) target;
          this.txtPhone.TextChanged += new TextChangedEventHandler(this.txtPhone_TextChanged);
          break;
        case 7:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.SubmitButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
