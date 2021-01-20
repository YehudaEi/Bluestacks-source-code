// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ArgAttribute
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  [AttributeUsage(AttributeTargets.Field)]
  public class ArgAttribute : Attribute
  {
    public string Name { get; set; }

    public object Value { get; set; }

    public string Description { get; set; }
  }
}
