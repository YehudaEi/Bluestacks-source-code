// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.SharedFolder
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "SharedFolder", Namespace = "http://www.virtualbox.org/")]
  public class SharedFolder
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "hostPath")]
    public string HostPath { get; set; }

    [XmlAttribute(AttributeName = "writable")]
    public string Writable { get; set; }

    [XmlAttribute(AttributeName = "autoMount")]
    public string AutoMount { get; set; }
  }
}
