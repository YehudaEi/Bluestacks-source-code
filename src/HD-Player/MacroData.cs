// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MacroData
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.IO;
using System.Xml.Serialization;

namespace BlueStacks.Player
{
  [Serializable]
  public class MacroData
  {
    private SerializableDictionary<string, Macro> mDictMacros = new SerializableDictionary<string, Macro>();
    public string mPackageName;
    private static MacroData sInstance;

    internal static MacroData Instance
    {
      get
      {
        if (MacroData.sInstance == null)
          MacroData.sInstance = new MacroData();
        return MacroData.sInstance;
      }
    }

    private MacroData()
    {
    }

    internal void LoadMacroData(string packageName)
    {
      this.mPackageName = packageName;
      string macroFileName = InputMapper.Instance.GetMacroFileName(false, this.mPackageName);
      if (File.Exists(macroFileName))
        this.mDictMacros = (SerializableDictionary<string, Macro>) new XmlSerializer(typeof (SerializableDictionary<string, Macro>)).Deserialize((TextReader) new StringReader(File.ReadAllText(macroFileName)));
      else
        this.mDictMacros = new SerializableDictionary<string, Macro>();
    }

    internal void SaveMacroData()
    {
      string macroFileName = InputMapper.Instance.GetMacroFileName(true, InputMapper.Instance.GetPackage());
      StringWriter stringWriter = new StringWriter();
      new XmlSerializer(typeof (SerializableDictionary<string, Macro>)).Serialize((TextWriter) stringWriter, (object) this.mDictMacros);
      string contents = stringWriter.ToString();
      File.WriteAllText(macroFileName, contents);
    }

    internal SerializableDictionary<string, Macro> DictMacros
    {
      get
      {
        return this.mDictMacros;
      }
      set
      {
        this.mDictMacros = value;
      }
    }
  }
}
