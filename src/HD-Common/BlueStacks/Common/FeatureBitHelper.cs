// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FeatureBitHelper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
