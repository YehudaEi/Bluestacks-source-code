// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RenderHelper
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public static class RenderHelper
  {
    private static bool? mSoftwareOnly;

    private static bool SoftwareOnly
    {
      get
      {
        if (!RenderHelper.mSoftwareOnly.HasValue)
          RenderHelper.mSoftwareOnly = new bool?(false);
        return RenderHelper.mSoftwareOnly.Value;
      }
    }

    public static void ChangeRenderModeToSoftware(object sender)
    {
      if (!RenderHelper.SoftwareOnly)
        return;
      Visual visual = (Visual) sender;
      if (visual == null)
        return;
      if (PresentationSource.FromVisual(visual) is HwndSource hwndSource)
      {
        Logger.Info("hwnd Source :" + sender.ToString());
        hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
      }
      else
        Logger.Info("sender = " + sender.ToString());
    }
  }
}
