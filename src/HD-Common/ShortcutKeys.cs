// Decompiled with JetBrains decompiler
// Type: ShortcutKeys
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using BlueStacks.Common;
using System;

[Serializable]
public class ShortcutKeys : ShortcutConfig
{
  public string ShortcutCategory { get; set; }

  public string ShortcutName { get; set; }

  public string ShortcutKey { get; set; }

  public bool ReadOnlyTextbox { get; set; }
}
