// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FeatureBitHelper
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public static class FeatureBitHelper
  {
    public static bool IsFeatureEnabled(ulong featureMask, ulong feature)
    {
      return ((long) feature & (long) featureMask) != 0L;
    }

    public static ulong EnableFeature(ulong featureMask, ulong feature)
    {
      return ((long) feature & (long) featureMask) != 0L ? feature : (feature |= featureMask);
    }

    public static ulong DisableFeature(ulong featureMask, ulong feature)
    {
      return ((long) feature & (long) featureMask) == 0L ? feature : feature & ~featureMask;
    }

    public static ulong ToggleFeature(ulong featureMask, ulong feature)
    {
      return !FeatureBitHelper.IsFeatureEnabled(featureMask, feature) ? FeatureBitHelper.EnableFeature(featureMask, feature) : FeatureBitHelper.DisableFeature(featureMask, feature);
    }

    public static bool WasFeatureChanged(
      ulong featureMask,
      ulong newFeature,
      ulong originalFeature,
      out bool isEnabled)
    {
      int num1 = FeatureBitHelper.IsFeatureEnabled(featureMask, originalFeature) ? 1 : 0;
      isEnabled = FeatureBitHelper.IsFeatureEnabled(featureMask, newFeature);
      int num2 = isEnabled ? 1 : 0;
      return num1 != num2;
    }
  }
}
