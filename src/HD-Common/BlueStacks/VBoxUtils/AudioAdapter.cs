// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.AudioAdapter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "AudioAdapter", Namespace = "http://www.virtualbox.org/")]
  public class AudioAdapter
  {
    [XmlAttribute(AttributeName = "driver")]
    public string Driver { get; set; }

    [XmlAttribute(AttributeName = "enabled")]
    public string Enabled { get; set; }
  }
}
