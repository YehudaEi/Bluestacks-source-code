// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SerializableKeyValuePair`2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlueStacks.Common
{
  [Serializable]
  public struct SerializableKeyValuePair<TKey, TValue>
  {
    public SerializableKeyValuePair(TKey key, TValue value)
    {
      this.Key = key;
      this.Value = value;
    }

    public TKey Key { [IsReadOnly] get; set; }

    public TValue Value { [IsReadOnly] get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('[');
      if ((object) this.Key != null)
        stringBuilder.Append(this.Key.ToString());
      stringBuilder.Append(", ");
      if ((object) this.Value != null)
        stringBuilder.Append(this.Value.ToString());
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }
  }
}
