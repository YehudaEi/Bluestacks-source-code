// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.AttachedDevice
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "AttachedDevice", Namespace = "http://www.virtualbox.org/")]
  public class AttachedDevice
  {
    [XmlElement(ElementName = "Image", Namespace = "http://www.virtualbox.org/")]
    public Image Image { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "port")]
    public string Port { get; set; }

    [XmlAttribute(AttributeName = "device")]
    public string Device { get; set; }
  }
}
