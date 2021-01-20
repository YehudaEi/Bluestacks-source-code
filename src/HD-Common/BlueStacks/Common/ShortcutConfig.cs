// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ShortcutConfig
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace BlueStacks.Common
{
  [Serializable]
  public class ShortcutConfig
  {
    public List<ShortcutKeys> Shortcut { get; set; }

    public string DefaultModifier { get; set; } = IMAPKeys.GetStringForFile(Key.LeftCtrl) + "," + IMAPKeys.GetStringForFile(Key.LeftShift);

    public static ShortcutConfig LoadShortcutsConfig()
    {
      try
      {
        string empty = string.Empty;
        string str;
        if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
        {
          str = RegistryManager.Instance.UserDefinedShortcuts;
        }
        else
        {
          if (!string.IsNullOrEmpty(empty) || string.IsNullOrEmpty(RegistryManager.Instance.DefaultShortcuts))
            throw new Exception("Shortcuts registry entry not found.");
          str = RegistryManager.Instance.DefaultShortcuts;
        }
        return JsonConvert.DeserializeObject<ShortcutConfig>(str, Utils.GetSerializerSettings());
      }
      catch (Exception ex)
      {
        Logger.Error("SHORTCUT: Exception in loading shortcuts config: " + ex.ToString());
      }
      return (ShortcutConfig) null;
    }
  }
}
