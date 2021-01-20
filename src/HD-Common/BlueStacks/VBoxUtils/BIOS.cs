// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.BIOS
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "BIOS", Namespace = "http://www.virtualbox.org/")]
  public class BIOS
  {
    [XmlElement(ElementName = "IOAPIC", Namespace = "http://www.virtualbox.org/")]
    public IOAPIC IOAPIC { get; set; }

    [XmlElement(ElementName = "Logo", Namespace = "http://www.virtualbox.org/")]
    public Logo Logo { get; set; }

    [XmlElement(ElementName = "BootMenu", Namespace = "http://www.virtualbox.org/")]
    public BootMenu BootMenu { get; set; }
  }
}
