// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RecommendedAppsSection
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class RecommendedAppsSection : UserControl, IComponentConnector
  {
    internal TextBlock mSectionHeader;
    internal StackPanel mAppRecommendationsPanel;
    private bool _contentLoaded;

    public RecommendedAppsSection(string header)
    {
      this.InitializeComponent();
      this.mSectionHeader.Text = header;
    }

    internal void AddSuggestedApps(
      MainWindow ParentWindow,
      List<AppRecommendation> suggestedApps,
      int clientShowCount)
    {
      int appPosition = 1;
      int appRank = 1;
      ParentWindow.mWelcomeTab.mHomeAppManager.ClearAppRecommendationPool();
      foreach (AppRecommendation suggestedApp in suggestedApps)
      {
        if (!ParentWindow.mAppHandler.IsAppInstalled(suggestedApp.ExtraPayload["click_action_packagename"]))
        {
          RecommendedApps recomApp = new RecommendedApps();
          recomApp.Populate(suggestedApp, appPosition, appRank);
          if (appPosition <= clientShowCount)
          {
            this.mAppRecommendationsPanel.Children.Add((UIElement) recomApp);
            ++appPosition;
          }
          else
            ParentWindow.mWelcomeTab.mHomeAppManager.AddToAppRecommendationPool(recomApp);
        }
        ++appRank;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/recommendedappssection.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mAppRecommendationsPanel = (StackPanel) target;
        else
          this._contentLoaded = true;
      }
      else
        this.mSectionHeader = (TextBlock) target;
    }
  }
}
