// Decompiled with JetBrains decompiler
// Type: BlueStacks.DataManager.DataManagerUtils
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using BlueStacks.Common;
using System;
using System.Windows;

namespace BlueStacks.DataManager
{
  public class DataManagerUtils
  {
    public static void ShowErrorMsg(string message, string captionTitle, ProgressWindow progress)
    {
      Logger.Info("Showing Error Message prompt");
      if (!App.sOpt.s)
        progress.Dispatcher.Invoke((Delegate) (() =>
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString(captionTitle, "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString(message, "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_EXIT", new EventHandler(DataManagerUtils.DataManagerUtils_ExitBtnHandler), (string) null, false, (object) null, true);
          customMessageWindow.CloseButtonHandle(new EventHandler(DataManagerUtils.DataManagerUtils_ExitBtnHandler), (object) null);
          customMessageWindow.Owner = (Window) progress;
          customMessageWindow.ShowInTaskbar = true;
          customMessageWindow.Title = Strings.ProductDisplayName;
          progress.Hide();
          customMessageWindow.ShowDialog();
        }));
      else
        DataManagerUtils.DataManagerUtils_ExitBtnHandler((object) null, (EventArgs) null);
    }

    private static void DataManagerUtils_ExitBtnHandler(object sender, EventArgs e)
    {
    }

    public static void ShowWarningMsg(
      string message,
      string captionTitle,
      ProgressWindow progress,
      EventHandler ContinueBtnhandler)
    {
      Logger.Info("Showing Warning Message prompt");
      if (!App.sOpt.s)
        progress.Dispatcher.Invoke((Delegate) (() =>
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString(captionTitle, "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString(message, "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CONTINUE", ContinueBtnhandler, (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", new EventHandler(DataManagerUtils.DataManagerUtils_CancelBtnHandler), (string) null, false, (object) null, true);
          customMessageWindow.CloseButtonHandle(new EventHandler(DataManagerUtils.DataManagerUtils_CancelBtnHandler), (object) null);
          customMessageWindow.Owner = (Window) progress;
          customMessageWindow.ShowDialog();
        }));
      else
        ContinueBtnhandler((object) null, (EventArgs) null);
    }

    private static void DataManagerUtils_CancelBtnHandler(object sender, EventArgs e)
    {
    }
  }
}
