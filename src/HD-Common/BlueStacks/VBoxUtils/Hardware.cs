// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.Hardware
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "Hardware", Namespace = "http://www.virtualbox.org/")]
  public class Hardware
  {
    [XmlElement(ElementName = "CPU", Namespace = "http://www.virtualbox.org/")]
    public CPU CPU { get; set; }

    [XmlElement(ElementName = "Memory", Namespace = "http://www.virtualbox.org/")]
    public Memory Memory { get; set; }

    [XmlElement(ElementName = "HID", Namespace = "http://www.virtualbox.org/")]
    public HID HID { get; set; }

    [XmlElement(ElementName = "HPET", Namespace = "http://www.virtualbox.org/")]
    public HPET HPET { get; set; }

    [XmlElement(ElementName = "Boot", Namespace = "http://www.virtualbox.org/")]
    public Boot Boot { get; set; }

    [XmlElement(ElementName = "Display", Namespace = "http://www.virtualbox.org/")]
    public Display Display { get; set; }

    [XmlElement(ElementName = "RemoteDisplay", Namespace = "http://www.virtualbox.org/")]
    public RemoteDisplay RemoteDisplay { get; set; }

    [XmlElement(ElementName = "BIOS", Namespace = "http://www.virtualbox.org/")]
    public BIOS BIOS { get; set; }

    [XmlElement(ElementName = "USB", Namespace = "http://www.virtualbox.org/")]
    public USB USB { get; set; }

    [XmlElement(ElementName = "Network", Namespace = "http://www.virtualbox.org/")]
    public Network Network { get; set; }

    [XmlElement(ElementName = "LPT", Namespace = "http://www.virtualbox.org/")]
    public LPT LPT { get; set; }

    [XmlElement(ElementName = "AudioAdapter", Namespace = "http://www.virtualbox.org/")]
    public AudioAdapter AudioAdapter { get; set; }

    [XmlElement(ElementName = "RTC", Namespace = "http://www.virtualbox.org/")]
    public RTC RTC { get; set; }

    [XmlElement(ElementName = "SharedFolders", Namespace = "http://www.virtualbox.org/")]
    public SharedFolders SharedFolders { get; set; }
  }
}
