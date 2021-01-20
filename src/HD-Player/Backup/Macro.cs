// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Macro
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.Player
{
  [Serializable]
  public class Macro
  {
    private SerializableDictionary<int, MacroAction> mDictMacroActions = new SerializableDictionary<int, MacroAction>();
    private SerializableDictionary<string, SerializableDictionary<string, string>> mDictAndroidCommands = new SerializableDictionary<string, SerializableDictionary<string, string>>();
    private string mMacroName;
    private RepeatBehaviour mAndroidCommandRepeatMode;
    private RepeatBehaviour mRepeatBehaiour;

    public string MacroName
    {
      get
      {
        return this.mMacroName;
      }
      set
      {
        this.mMacroName = value;
      }
    }

    public RepeatBehaviour AndroidCommandRepeatMode
    {
      get
      {
        return this.mAndroidCommandRepeatMode;
      }
      set
      {
        this.mAndroidCommandRepeatMode = value;
      }
    }

    public RepeatBehaviour RepeatBehaiour
    {
      get
      {
        return this.mRepeatBehaiour;
      }
      set
      {
        this.mRepeatBehaiour = value;
      }
    }

    public SerializableDictionary<int, MacroAction> DictMacroActions
    {
      get
      {
        return this.mDictMacroActions;
      }
      set
      {
        this.mDictMacroActions = value;
      }
    }

    public SerializableDictionary<string, SerializableDictionary<string, string>> DictAndroidCommands
    {
      get
      {
        return this.mDictAndroidCommands;
      }
      set
      {
        this.mDictAndroidCommands = value;
      }
    }
  }
}
