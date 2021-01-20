// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ArgAttribute
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
