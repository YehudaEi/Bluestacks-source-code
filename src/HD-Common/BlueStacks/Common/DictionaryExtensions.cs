// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DictionaryExtensions
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  public static class DictionaryExtensions
  {
    public static void ClearSync<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
      if (dic == null)
        return;
      lock (((ICollection) dic).SyncRoot)
        dic.Clear();
    }

    public static void ClearAddRange<TKey, TValue>(
      this Dictionary<TKey, TValue> dic,
      Dictionary<TKey, TValue> dicToAdd)
    {
      if (dic == null)
        return;
      lock (((ICollection) dic).SyncRoot)
      {
        dic.Clear();
        // ISSUE: explicit non-virtual call
        if (dicToAdd == null || __nonvirtual (dicToAdd.Count) <= 0)
          return;
        foreach (KeyValuePair<TKey, TValue> keyValuePair in dicToAdd)
          dic.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }
  }
}
