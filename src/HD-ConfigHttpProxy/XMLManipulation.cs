// Decompiled with JetBrains decompiler
// Type: BlueStacks.ConfigHttpProxy.XMLManipulation
// Assembly: HD-ConfigHttpProxy, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 12C81935-F2EE-4FEC-B10B-2FD5D0D8A1FF
// Assembly location: C:\Program Files\BlueStacks\HD-ConfigHttpProxy.exe

using System;
using System.Linq;
using System.Xml.Linq;

namespace BlueStacks.ConfigHttpProxy
{
  internal class XMLManipulation
  {
    internal static string xmlFilePath = BlueStacks.ConfigHttpProxy.ConfigHttpProxy.TEMP_XML_FILE_PATH;

    public static void AppendToXMLFile(string name, string id, string cmdArg)
    {
      try
      {
        XDocument xdocument = XDocument.Load(XMLManipulation.xmlFilePath);
        xdocument.Descendants((XName) "setting").Select(node => new
        {
          node = node,
          attr = node.Attribute((XName) nameof (name))
        }).Where(_param1 => _param1.attr != null && _param1.attr.Value == name).Select(_param1 => _param1.node).ToList<XElement>().ForEach((Action<XElement>) (x => x.Remove()));
        XElement xelement = new XElement((XName) "setting");
        XAttribute xattribute1 = new XAttribute((XName) nameof (id), (object) id);
        XAttribute xattribute2 = new XAttribute((XName) nameof (name), (object) name);
        XAttribute xattribute3 = new XAttribute((XName) "value", (object) cmdArg);
        XAttribute xattribute4 = new XAttribute((XName) "package", (object) "android");
        xelement.Add((object) xattribute1);
        xelement.Add((object) xattribute2);
        xelement.Add((object) xattribute3);
        xelement.Add((object) xattribute4);
        xdocument.Element((XName) "settings").Add((object) xelement);
        xdocument.Save(XMLManipulation.xmlFilePath);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Could not configure proxy parameters. {0}", (object) ex.Message);
        Environment.Exit(1);
      }
    }

    public static void DeleteFromXMLFile(string name)
    {
      try
      {
        XDocument xdocument = XDocument.Load(XMLManipulation.xmlFilePath);
        xdocument.Descendants((XName) "setting").Select(node => new
        {
          node = node,
          attr = node.Attribute((XName) nameof (name))
        }).Where(_param1 => _param1.attr != null && _param1.attr.Value == name).Select(_param1 => _param1.node).ToList<XElement>().ForEach((Action<XElement>) (x => x.Remove()));
        xdocument.Save(XMLManipulation.xmlFilePath);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Could not reset proxy. {0}", (object) ex.Message);
        Environment.Exit(1);
      }
    }
  }
}
