// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.RenderHelper
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Core
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
      if (visual == null || !(PresentationSource.FromVisual(visual) is HwndSource hwndSource))
        return;
      hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
    }
  }
}
