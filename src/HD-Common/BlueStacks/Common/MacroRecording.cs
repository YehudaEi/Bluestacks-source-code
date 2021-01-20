// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MacroRecording
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlueStacks.Common
{
  [Serializable]
  public class MacroRecording : BiDirectionalVertex<MacroRecording>, INotifyPropertyChanged, IEquatable<MacroRecording>
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    [JsonProperty("TimeCreated")]
    public string TimeCreated { get; set; }

    [JsonProperty("FileName")]
    public string FileName { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("Events", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
    public List<MacroEvents> Events { get; set; }

    [JsonProperty("SourceRecordings", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> SourceRecordings { get; set; }

    [JsonProperty("MergedMacroConfigurations", NullValueHandling = NullValueHandling.Ignore)]
    public ObservableCollection<MergedMacroConfiguration> MergedMacroConfigurations { get; set; }

    [JsonProperty("LoopType")]
    public OperationsLoopType LoopType { get; set; }

    [JsonIgnore]
    public RecordingTypes RecordingType
    {
      get
      {
        return this.Events != null ? RecordingTypes.SingleRecording : RecordingTypes.MultiRecording;
      }
    }

    [JsonProperty("LoopNumber")]
    public int LoopNumber { get; set; } = 1;

    [JsonProperty("LoopTime")]
    public int LoopTime { get; set; }

    [JsonProperty("LoopInterval")]
    public int LoopInterval { get; set; }

    [JsonProperty("Acceleration")]
    public double Acceleration { get; set; } = 1.0;

    [JsonProperty("PlayOnStart")]
    public bool PlayOnStart { get; set; }

    [JsonProperty("DonotShowWindowOnFinish")]
    public bool DonotShowWindowOnFinish { get; set; }

    [JsonProperty("RestartPlayer")]
    public bool RestartPlayer { get; set; }

    [JsonProperty("RestartPlayerAfterMinutes")]
    public int RestartPlayerAfterMinutes { get; set; } = 60;

    [JsonProperty("ShortCut")]
    public string Shortcut { get; set; } = string.Empty;

    [JsonProperty("UserName", NullValueHandling = NullValueHandling.Ignore)]
    public string User { get; set; } = string.Empty;

    [JsonProperty("AuthorPageUrl", NullValueHandling = NullValueHandling.Ignore)]
    public Uri AuthorPageUrl { get; set; }

    [JsonProperty("MacroId", NullValueHandling = NullValueHandling.Ignore)]
    public string MacroId { get; set; } = string.Empty;

    [JsonProperty("MacroPageUrl", NullValueHandling = NullValueHandling.Ignore)]
    public Uri MacroPageUrl { get; set; }

    public bool Equals(MacroRecording other)
    {
      return other != null && string.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as MacroRecording);
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public void CopyFrom(MacroRecording previous)
    {
      if (previous == null)
        return;
      this.TimeCreated = previous.TimeCreated;
      this.Name = previous.Name;
      List<MacroEvents> events = previous.Events;
      this.Events = events != null ? events.DeepCopy<List<MacroEvents>>() : (List<MacroEvents>) null;
      List<string> sourceRecordings = previous.SourceRecordings;
      this.SourceRecordings = sourceRecordings != null ? sourceRecordings.DeepCopy<List<string>>() : (List<string>) null;
      this.MergedMacroConfigurations = JsonConvert.DeserializeObject<ObservableCollection<MergedMacroConfiguration>>(JsonConvert.SerializeObject((object) previous.MergedMacroConfigurations, Utils.GetSerializerSettings()), Utils.GetSerializerSettings());
      this.LoopType = previous.LoopType;
      this.LoopNumber = previous.LoopNumber;
      this.LoopTime = previous.LoopTime;
      this.LoopInterval = previous.LoopInterval;
      this.Acceleration = previous.Acceleration;
      this.PlayOnStart = previous.PlayOnStart;
      this.RestartPlayer = previous.RestartPlayer;
      this.RestartPlayerAfterMinutes = previous.RestartPlayerAfterMinutes;
      this.DonotShowWindowOnFinish = previous.DonotShowWindowOnFinish;
      this.Shortcut = previous.Shortcut;
      MacroGraph.Instance.DeLinkMacroChild((BiDirectionalVertex<MacroRecording>) this);
    }
  }
}
