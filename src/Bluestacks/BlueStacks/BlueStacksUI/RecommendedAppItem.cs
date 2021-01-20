// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RecommendedAppItem
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

namespace BlueStacks.BlueStacksUI
{
  public class RecommendedAppItem : UserControl, IComponentConnector
  {
    private MainWindow mMainWindow;
    internal CustomPictureBox recomIcon;
    internal TextBlock appNameTextBlock;
    internal CustomButton installButton;
    private bool _contentLoaded;

    public RecommendedAppItem()
    {
      this.InitializeComponent();
    }

    internal SearchRecommendation SearchRecomendation { get; set; }

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    internal void Populate(MainWindow parentWindow, SearchRecommendation recom)
    {
      this.mMainWindow = parentWindow;
      this.recomIcon.IsFullImagePath = true;
      this.recomIcon.ImageName = recom.ImagePath;
      this.installButton.ButtonColor = ButtonColors.Green;
      this.installButton.Content = this.ParentWindow.mAppHandler.IsAppInstalled(recom.ExtraPayload["click_action_packagename"]) ? (object) LocaleStrings.GetLocalizedString("STRING_PLAY", "") : (object) LocaleStrings.GetLocalizedString("STRING_INSTALL", "");
      this.appNameTextBlock.Text = recom.ExtraPayload["click_action_title"];
      this.SearchRecomendation = recom;
    }

    private void Recommendation_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ClientStats.SendFrontendClickStats("search_suggestion_click", "", this.SearchRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN" ? "cdn" : "gplay", this.SearchRecomendation.ExtraPayload["click_action_packagename"], this.ParentWindow.mAppHandler.IsAppInstalled(this.SearchRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false", (string) null, (string) null, (string) null);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while sending stats to cloud for search_suggestion_click " + ex.ToString());
      }
      this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.SearchRecomendation.ExtraPayload, "search_suggestion", "");
    }

    private void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
    }

    private void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Background = (Brush) Brushes.Transparent;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/recommendedappitem.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).MouseUp += new MouseButtonEventHandler(this.Recommendation_Click);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.UserControl_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.UserControl_MouseLeave);
          break;
        case 2:
          this.recomIcon = (CustomPictureBox) target;
          break;
        case 3:
          this.appNameTextBlock = (TextBlock) target;
          break;
        case 4:
          this.installButton = (CustomButton) target;
          this.installButton.Click += new RoutedEventHandler(this.Recommendation_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
