﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.Category
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using System.Collections.Generic;

namespace BlueStacks.LogCollector
{
  public class Category
  {
    public List<BlueStacks.LogCollector.Subcategory> Subcategory { get; } = new List<BlueStacks.LogCollector.Subcategory>();

    public string categoryId { get; set; }

    public string categoryValue { get; set; }

    public string showdropdown { get; set; }
  }
}
