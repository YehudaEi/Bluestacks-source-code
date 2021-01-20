// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ListExtensions
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  public static class ListExtensions
  {
    public static void ClearSync<T>(this List<T> list)
    {
      if (list == null)
        return;
      lock (((ICollection) list).SyncRoot)
        list.Clear();
    }

    public static void ClearAddRange<T>(this List<T> list, List<T> listToAdd)
    {
      if (list == null)
        return;
      lock (((ICollection) list).SyncRoot)
      {
        list.Clear();
        // ISSUE: explicit non-virtual call
        if (listToAdd == null || __nonvirtual (listToAdd.Count) <= 0)
          return;
        list.AddRange((IEnumerable<T>) listToAdd);
      }
    }
  }
}
