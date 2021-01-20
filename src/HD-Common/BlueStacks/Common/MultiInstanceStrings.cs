// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MultiInstanceStrings
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  public static class MultiInstanceStrings
  {
    private static string sVmName;
    private static int sVmId;

    public static string VmName
    {
      get
      {
        return MultiInstanceStrings.sVmName == null ? "Android" : MultiInstanceStrings.sVmName;
      }
      set
      {
        if (MultiInstanceStrings.sVmName != null)
          throw new Exception("VmName can be set only once");
        MultiInstanceStrings.sVmName = value;
        if (MultiInstanceStrings.sVmName == "Android")
        {
          MultiInstanceStrings.sVmId = 0;
        }
        else
        {
          string sVmName = MultiInstanceStrings.sVmName;
          string s;
          if (sVmName == null)
            s = (string) null;
          else
            s = sVmName.Split('_')[1];
          ref int local = ref MultiInstanceStrings.sVmId;
          if (!int.TryParse(s, out local))
            throw new Exception("Invalid VM: " + MultiInstanceStrings.sVmName);
        }
      }
    }

    public static ushort BstServerPort
    {
      get
      {
        return (ushort) RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAndroidPort;
      }
      set
      {
        RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAndroidPort = (int) value;
      }
    }

    public static int VmId
    {
      get
      {
        return MultiInstanceStrings.sVmId;
      }
    }
  }
}
