// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.HardDisk
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "HardDisk", Namespace = "http://www.virtualbox.org/")]
  public class HardDisk
  {
    [XmlAttribute(AttributeName = "uuid")]
    public string Uuid { get; set; }

    [XmlAttribute(AttributeName = "location")]
    public string Location { get; set; }

    [XmlAttribute(AttributeName = "format")]
    public string Format { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlElement(ElementName = "HardDisk", Namespace = "http://www.virtualbox.org/")]
    public List<HardDisk> HardDisk1 { get; set; }
  }
}
