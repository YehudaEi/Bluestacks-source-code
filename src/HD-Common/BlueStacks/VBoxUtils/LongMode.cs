// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.LongMode
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "LongMode", Namespace = "http://www.virtualbox.org/")]
  public class LongMode
  {
    [XmlAttribute(AttributeName = "enabled")]
    public string Enabled { get; set; }
  }
}
