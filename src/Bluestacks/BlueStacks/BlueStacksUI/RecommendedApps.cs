// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RecommendedApps
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class RecommendedApps : UserControl, IComponentConnector
  {
    private MainWindow mMainWindow;
    internal Grid mMainGrid;
    internal CustomPictureBox recomIcon;
    internal TextBlock appNameTextBlock;
    internal TextBlock appGenreTextBlock;
    internal CustomButton installButton;
    private bool _contentLoaded;

    public RecommendedApps()
    {
      this.InitializeComponent();
    }

    internal AppRecommendation AppRecomendation { get; set; }

    internal int RecommendedAppPosition { get; set; }

    internal int RecommendedAppRank { get; set; }

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    internal void Populate(AppRecommendation recom, int appPosition, int appRank)
    {
      this.AppRecomendation = recom;
      this.recomIcon.IsFullImagePath = true;
      this.recomIcon.ImageName = recom.ImagePath;
      this.appNameTextBlock.Text = recom.ExtraPayload["click_action_title"];
      this.appGenreTextBlock.Text = recom.GameGenre;
      this.RecommendedAppPosition = appPosition;
      this.RecommendedAppRank = appRank;
    }

    private void Recommendation_Click(object sender, MouseButtonEventArgs e)
    {
      try
      {
        ClientStats.SendFrontendClickStats("apps_recommendation", "click", (string) null, this.AppRecomendation.ExtraPayload["click_action_packagename"], (string) null, (string) null, (string) null, new JArray()
        {
          (JToken) new JObject()
          {
            {
              "app_loc",
              (JToken) (this.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN" ? "cdn" : "gplay")
            },
            {
              "app_pkg",
              (JToken) this.AppRecomendation.ExtraPayload["click_action_packagename"]
            },
            {
              "is_installed",
              (JToken) (this.ParentWindow.mAppHandler.IsAppInstalled(this.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false")
            },
            {
              "app_position",
              (JToken) this.RecommendedAppPosition.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            },
            {
              "app_rank",
              (JToken) this.RecommendedAppRank.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }
          }
        }.ToString(Formatting.None));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while sending stats to cloud for apps_recommendation_click " + ex.ToString());
      }
      this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.AppRecomendation.ExtraPayload, "search_suggestion", "");
    }

    private void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.mMainGrid, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
    }

    private void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mMainGrid.Background = (Brush) Brushes.Transparent;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/recommendedapps.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMainGrid = (Grid) target;
          this.mMainGrid.MouseEnter += new MouseEventHandler(this.UserControl_MouseEnter);
          this.mMainGrid.MouseLeave += new MouseEventHandler(this.UserControl_MouseLeave);
          this.mMainGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Recommendation_Click);
          break;
        case 2:
          this.recomIcon = (CustomPictureBox) target;
          break;
        case 3:
          this.appNameTextBlock = (TextBlock) target;
          break;
        case 4:
          this.appGenreTextBlock = (TextBlock) target;
          break;
        case 5:
          this.installButton = (CustomButton) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
