// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BiDirectionalVertex`1
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public abstract class BiDirectionalVertex<T>
  {
    [JsonIgnore]
    public List<BiDirectionalVertex<T>> Parents { get; } = new List<BiDirectionalVertex<T>>();

    [JsonIgnore]
    public List<BiDirectionalVertex<T>> Childs { get; } = new List<BiDirectionalVertex<T>>();

    [JsonIgnore]
    public bool IsVisited { get; set; }

    public void AddChild(BiDirectionalVertex<T> newChild)
    {
      this.Childs.Add(newChild);
    }

    public void RemoveChild(BiDirectionalVertex<T> newChild)
    {
      this.Childs.Remove(newChild);
    }

    public void AddParent(BiDirectionalVertex<T> newParent)
    {
      this.Parents.Add(newParent);
    }

    public void RemoveParent(BiDirectionalVertex<T> newParent)
    {
      this.Parents.Remove(newParent);
    }
  }
}
