// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MacroGraph
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace BlueStacks.Common
{
  public sealed class MacroGraph
  {
    private static BiDirectionalGraph<MacroRecording> mInstance = (BiDirectionalGraph<MacroRecording>) null;
    private static readonly object lockObj = new object();

    public static BiDirectionalGraph<MacroRecording> Instance
    {
      get
      {
        if (MacroGraph.mInstance == null)
        {
          lock (MacroGraph.lockObj)
          {
            if (MacroGraph.mInstance == null)
            {
              MacroGraph.mInstance = new BiDirectionalGraph<MacroRecording>((ObservableCollection<BiDirectionalVertex<MacroRecording>>) null);
              MacroGraph.CreateMacroGraphInstance();
            }
          }
        }
        return MacroGraph.mInstance;
      }
    }

    public static void ReCreateMacroGraphInstance()
    {
      if (MacroGraph.mInstance == null)
        MacroGraph.mInstance = new BiDirectionalGraph<MacroRecording>((ObservableCollection<BiDirectionalVertex<MacroRecording>>) null);
      MacroGraph.mInstance.Vertices.Clear();
      MacroGraph.CreateMacroGraphInstance();
    }

    private static void CreateMacroGraphInstance()
    {
      if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
        return;
      foreach (string file in Directory.GetFiles(RegistryStrings.MacroRecordingsFolderPath))
      {
        string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, file);
        if (File.Exists(path))
        {
          try
          {
            MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), Utils.GetSerializerSettings());
            if (macroRecording != null)
            {
              if (!string.IsNullOrEmpty(macroRecording.Name))
              {
                if (!string.IsNullOrEmpty(macroRecording.TimeCreated))
                {
                  if (macroRecording.Events == null)
                  {
                    ObservableCollection<MergedMacroConfiguration> macroConfigurations = macroRecording.MergedMacroConfigurations;
                    // ISSUE: explicit non-virtual call
                    if ((macroConfigurations != null ? (__nonvirtual (macroConfigurations.Count) > 0 ? 1 : 0) : 0) == 0)
                      continue;
                  }
                  MacroGraph.mInstance.AddVertex((BiDirectionalVertex<MacroRecording>) macroRecording);
                }
              }
            }
          }
          catch
          {
            Logger.Error("Unable to deserialize userscript.");
          }
        }
      }
      MacroGraph.DrawMacroGraph();
    }

    private static void DrawMacroGraph()
    {
      foreach (BiDirectionalVertex<MacroRecording> vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.mInstance.Vertices)
        MacroGraph.LinkMacroChilds(vertex as MacroRecording);
    }

    public static void LinkMacroChilds(MacroRecording macro)
    {
      if (macro?.MergedMacroConfigurations == null)
        return;
      foreach (string str in macro.MergedMacroConfigurations.SelectMany<MergedMacroConfiguration, string>((Func<MergedMacroConfiguration, IEnumerable<string>>) (macro_ => (IEnumerable<string>) macro_.MacrosToRun)))
      {
        string dependentMacro = str;
        MacroGraph.mInstance.AddParentChild((BiDirectionalVertex<MacroRecording>) macro, (BiDirectionalVertex<MacroRecording>) MacroGraph.mInstance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro_ => string.Equals(macro_.Name, dependentMacro, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>());
      }
    }

    public static bool CheckIfDependentMacrosAreAvailable(MacroRecording macro1)
    {
      if (macro1 == null)
        return false;
      if (macro1.RecordingType == RecordingTypes.SingleRecording)
        return true;
      return macro1.MergedMacroConfigurations.SelectMany<MergedMacroConfiguration, string>((Func<MergedMacroConfiguration, IEnumerable<string>>) (macro2 => (IEnumerable<string>) macro2.MacrosToRun)).Distinct<string>().Count<string>() == macro1.Childs.Count && macro1.Childs.Cast<MacroRecording>().All<MacroRecording>((Func<MacroRecording, bool>) (childMacro => MacroGraph.CheckIfDependentMacrosAreAvailable(childMacro)));
    }
  }
}
