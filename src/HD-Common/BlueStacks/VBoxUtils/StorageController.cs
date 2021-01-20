// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.StorageController
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "StorageController", Namespace = "http://www.virtualbox.org/")]
  public class StorageController
  {
    [XmlElement(ElementName = "AttachedDevice", Namespace = "http://www.virtualbox.org/")]
    public List<BlueStacks.VBoxUtils.AttachedDevice> AttachedDevice { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "PortCount")]
    public string PortCount { get; set; }

    [XmlAttribute(AttributeName = "useHostIOCache")]
    public string UseHostIOCache { get; set; }

    [XmlAttribute(AttributeName = "Bootable")]
    public string Bootable { get; set; }

    [XmlAttribute(AttributeName = "IDE0MasterEmulationPort")]
    public string IDE0MasterEmulationPort { get; set; }

    [XmlAttribute(AttributeName = "IDE0SlaveEmulationPort")]
    public string IDE0SlaveEmulationPort { get; set; }

    [XmlAttribute(AttributeName = "IDE1MasterEmulationPort")]
    public string IDE1MasterEmulationPort { get; set; }

    [XmlAttribute(AttributeName = "IDE1SlaveEmulationPort")]
    public string IDE1SlaveEmulationPort { get; set; }
  }
}
