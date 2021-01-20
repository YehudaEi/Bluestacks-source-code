// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.Adapter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "Adapter", Namespace = "http://www.virtualbox.org/")]
  public class Adapter
  {
    [XmlElement(ElementName = "DisabledModes", Namespace = "http://www.virtualbox.org/")]
    public DisabledModes DisabledModes { get; set; }

    [XmlElement(ElementName = "NAT", Namespace = "http://www.virtualbox.org/")]
    public string NAT { get; set; }

    [XmlAttribute(AttributeName = "slot")]
    public string Slot { get; set; }

    [XmlAttribute(AttributeName = "enabled")]
    public string Enabled { get; set; }

    [XmlAttribute(AttributeName = "MACAddress")]
    public string MACAddress { get; set; }

    [XmlAttribute(AttributeName = "cable")]
    public string Cable { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
  }
}
