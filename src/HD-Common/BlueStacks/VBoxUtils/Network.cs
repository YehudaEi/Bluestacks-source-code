// Decompiled with JetBrains decompiler
// Type: BlueStacks.VBoxUtils.Network
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
  [XmlRoot(ElementName = "Network", Namespace = "http://www.virtualbox.org/")]
  public class Network
  {
    [XmlElement(ElementName = "Adapter", Namespace = "http://www.virtualbox.org/")]
    public List<BlueStacks.VBoxUtils.Adapter> Adapter { get; set; }
  }
}
