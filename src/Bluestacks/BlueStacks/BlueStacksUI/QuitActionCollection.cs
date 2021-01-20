// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.QuitActionCollection
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.BlueStacksUI
{
  public static class QuitActionCollection
  {
    private static Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> sQuitActionCollection = (Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>>) null;
    private static readonly object syncRoot = new object();

    public static Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> Actions
    {
      get
      {
        if (QuitActionCollection.sQuitActionCollection == null)
        {
          lock (QuitActionCollection.syncRoot)
          {
            if (QuitActionCollection.sQuitActionCollection == null)
              QuitActionCollection.InitDictionary();
          }
        }
        return QuitActionCollection.sQuitActionCollection;
      }
    }

    private static void InitDictionary()
    {
      Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> dictionary1 = new Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>>();
      foreach (QuitActionItem index in Enum.GetValues(typeof (QuitActionItem)))
      {
        Dictionary<QuitActionItemProperty, string> dictionary2 = new Dictionary<QuitActionItemProperty, string>();
        QuitActionItemCTA quitActionItemCta;
        switch (index)
        {
          case QuitActionItem.None:
            continue;
          case QuitActionItem.StuckAtBoot:
            dictionary2[QuitActionItemProperty.ImageName] = "clock_icon";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_STUCK_AT_BOOT_SCREEN", "");
            Dictionary<QuitActionItemProperty, string> dictionary3 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenLinkInBrowser;
            string str1 = quitActionItemCta.ToString();
            dictionary3[QuitActionItemProperty.CallToAction] = str1;
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
            dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/stuck_at_boot", (string) null, (string) null, (string) null);
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_stuck_at_boot";
            break;
          case QuitActionItem.SomethingElseWrong:
            dictionary2[QuitActionItemProperty.ImageName] = "support_icon";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_SOMETHING_ELSE_WENT_WRONG", "");
            Dictionary<QuitActionItemProperty, string> dictionary4 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenApplication;
            string str2 = quitActionItemCta.ToString();
            dictionary4[QuitActionItemProperty.CallToAction] = str2;
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_REPORT_A_PROBLEM", "");
            dictionary2[QuitActionItemProperty.ActionValue] = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_support";
            break;
          case QuitActionItem.SlowPerformance:
            dictionary2[QuitActionItemProperty.ImageName] = "rocket";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_SLOW_PERFORMANCE", "");
            Dictionary<QuitActionItemProperty, string> dictionary5 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenLinkInBrowser;
            string str3 = quitActionItemCta.ToString();
            dictionary5[QuitActionItemProperty.CallToAction] = str3;
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
            dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/enhance_performance", (string) null, (string) null, (string) null);
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_enhance_performance";
            break;
          case QuitActionItem.WhyGoogleAccount:
            dictionary2[QuitActionItemProperty.ImageName] = "google_white_icon";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_WHY_GOOGLE_ACCOUNT", "");
            Dictionary<QuitActionItemProperty, string> dictionary6 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenLinkInBrowser;
            string str4 = quitActionItemCta.ToString();
            dictionary6[QuitActionItemProperty.CallToAction] = str4;
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
            dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/why_google", (string) null, (string) null, (string) null);
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_why_google";
            break;
          case QuitActionItem.TroubleSigningIn:
            dictionary2[QuitActionItemProperty.ImageName] = "performance_icon";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_TROUBLE_SIGNING_IN", "");
            Dictionary<QuitActionItemProperty, string> dictionary7 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenLinkInBrowser;
            string str5 = quitActionItemCta.ToString();
            dictionary7[QuitActionItemProperty.CallToAction] = str5;
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
            dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/trouble_signing", (string) null, (string) null, (string) null);
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_trouble_signing";
            break;
          case QuitActionItem.UnsureWhereStart:
            dictionary2[QuitActionItemProperty.ImageName] = "exit_star";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_NOT_SURE_WHERE_START", "");
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_VIEW_TOP_GAMES", "");
            Dictionary<QuitActionItemProperty, string> dictionary8 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenAppCenter;
            string str6 = quitActionItemCta.ToString();
            dictionary8[QuitActionItemProperty.CallToAction] = str6;
            dictionary2[QuitActionItemProperty.ActionValue] = "";
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_unsure_where_start";
            break;
          case QuitActionItem.IssueInstallingGame:
            dictionary2[QuitActionItemProperty.ImageName] = "exit_exclamation";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_ISSUES_INSTALLING_A_GAME", "");
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_READ_SOLUTION", "");
            Dictionary<QuitActionItemProperty, string> dictionary9 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenLinkInBrowser;
            string str7 = quitActionItemCta.ToString();
            dictionary9[QuitActionItemProperty.CallToAction] = str7;
            dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetHelpArticleURL("trouble_installing_running_game");
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_issue_installing_game";
            break;
          case QuitActionItem.FacingOtherTroubles:
            dictionary2[QuitActionItemProperty.ImageName] = "exit_question";
            dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_FACING_OTHER_TROUBLE", "");
            dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_REPORT_A_PROBLEM", "");
            Dictionary<QuitActionItemProperty, string> dictionary10 = dictionary2;
            quitActionItemCta = QuitActionItemCTA.OpenApplication;
            string str8 = quitActionItemCta.ToString();
            dictionary10[QuitActionItemProperty.CallToAction] = str8;
            dictionary2[QuitActionItemProperty.ActionValue] = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
            dictionary2[QuitActionItemProperty.StatEventName] = "click_action_facing_other_troubles";
            break;
        }
        dictionary1[index] = dictionary2;
      }
      QuitActionCollection.sQuitActionCollection = dictionary1;
    }
  }
}
