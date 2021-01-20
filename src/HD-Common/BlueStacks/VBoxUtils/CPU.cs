// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.CPU
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "CPU", Namespace = "http://www.virtualbox.org/")]
  public class CPU
  {
    [XmlElement(ElementName = "PAE", Namespace = "http://www.virtualbox.org/")]
    public PAE PAE { get; set; }

    [XmlElement(ElementName = "LongMode", Namespace = "http://www.virtualbox.org/")]
    public LongMode LongMode { get; set; }

    [XmlElement(ElementName = "HardwareVirtExLargePages", Namespace = "http://www.virtualbox.org/")]
    public HardwareVirtExLargePages HardwareVirtExLargePages { get; set; }

    [XmlAttribute(AttributeName = "count")]
    public string Count { get; set; }
  }
}
