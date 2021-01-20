// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SerializableDictionary`2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
  [XmlRoot("dictionary")]
  [Serializable]
  public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
  {
    public SerializableDictionary()
    {
    }

    public SerializableDictionary(IEqualityComparer<TKey> comparer)
      : base(comparer)
    {
    }

    public SerializableDictionary(
      IDictionary<TKey, TValue> dictionary,
      IEqualityComparer<TKey> comparer)
      : base(dictionary, comparer)
    {
    }

    protected SerializableDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public XmlSchema GetSchema()
    {
      return (XmlSchema) null;
    }

    public void ReadXml(XmlReader reader)
    {
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (TKey));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (TValue));
      bool flag = true;
      if (reader != null)
      {
        flag = reader.IsEmptyElement;
        reader.Read();
      }
      if (flag)
        return;
      while ((reader != null ? (reader.NodeType != XmlNodeType.EndElement ? 1 : 0) : 1) != 0)
      {
        reader.ReadStartElement("item");
        reader.ReadStartElement("key");
        TKey key = (TKey) xmlSerializer1.Deserialize(reader);
        reader.ReadEndElement();
        reader.ReadStartElement("value");
        TValue obj = (TValue) xmlSerializer2.Deserialize(reader);
        reader.ReadEndElement();
        this.Add(key, obj);
        reader.ReadEndElement();
        int content = (int) reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
      if (writer == null)
        return;
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (TKey));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (TValue));
      foreach (TKey key in this.Keys)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        xmlSerializer1.Serialize(writer, (object) key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        TValue obj = this[key];
        xmlSerializer2.Serialize(writer, (object) obj);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    public virtual object Clone()
    {
      this.GetType();
      IFormatter formatter = (IFormatter) new BinaryFormatter();
      using (Stream serializationStream = (Stream) new MemoryStream())
      {
        formatter.Serialize(serializationStream, (object) this);
        serializationStream.Seek(0L, SeekOrigin.Begin);
        return formatter.Deserialize(serializationStream);
      }
    }
  }
}
