// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmComparerFactory`1
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.Grm.Comparers;
using System;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  internal class GrmComparerFactory<T>
  {
    public static IGrmOperatorComparer<T> GetComparerForOperand(
      GrmOperand operand)
    {
      switch (operand)
      {
        case GrmOperand.AppVersionCode:
        case GrmOperand.Ram:
        case GrmOperand.PhysicalRam:
        case GrmOperand.CpuCoresAllocated:
        case GrmOperand.PhysicalCoresAvailable:
        case GrmOperand.Dpi:
        case GrmOperand.Fps:
        case GrmOperand.AppRunningCountAcrossInstances:
          return (IGrmOperatorComparer<T>) new GenericComparer<int>();
        case GrmOperand.ProductVersion:
          return (IGrmOperatorComparer<T>) new VersionComparer();
        case GrmOperand.Geo:
        case GrmOperand.Gpu:
        case GrmOperand.GlMode:
        case GrmOperand.EngineMode:
        case GrmOperand.Resolution:
        case GrmOperand.GuestOs:
        case GrmOperand.Oem:
        case GrmOperand.BootParam:
        case GrmOperand.DeviceProfile:
        case GrmOperand.ASTCTexture:
        case GrmOperand.ABIMode:
          return (IGrmOperatorComparer<T>) new GrmStringComparer();
        case GrmOperand.Is64Bit:
        case GrmOperand.CustomKeyMappingExists:
          return (IGrmOperatorComparer<T>) new BooleanComparer();
        case GrmOperand.InstalledOems:
          return (IGrmOperatorComparer<T>) new StringListComparer();
        default:
          if (typeof (T).IsAssignableFrom(typeof (int)))
            return (IGrmOperatorComparer<T>) new GenericComparer<int>();
          if (typeof (T).IsAssignableFrom(typeof (long)))
            return (IGrmOperatorComparer<T>) new GenericComparer<long>();
          if (typeof (T).IsAssignableFrom(typeof (bool)))
            return (IGrmOperatorComparer<T>) new BooleanComparer();
          if (typeof (T).IsAssignableFrom(typeof (string)))
            return (IGrmOperatorComparer<T>) new GrmStringComparer();
          if (typeof (T).IsAssignableFrom(typeof (double)))
            return (IGrmOperatorComparer<T>) new GenericComparer<double>();
          if (typeof (T).IsAssignableFrom(typeof (Decimal)))
            return (IGrmOperatorComparer<T>) new GenericComparer<Decimal>();
          if (typeof (T).IsAssignableFrom(typeof (DateTime)))
            return (IGrmOperatorComparer<T>) new GenericComparer<DateTime>();
          if (typeof (T).IsAssignableFrom(typeof (Version)))
            return (IGrmOperatorComparer<T>) new VersionComparer();
          if (typeof (T).IsAssignableFrom(typeof (List<string>)))
            return (IGrmOperatorComparer<T>) new StringListComparer();
          throw new ArgumentException("No comparer found for operand " + operand.ToString());
      }
    }
  }
}
