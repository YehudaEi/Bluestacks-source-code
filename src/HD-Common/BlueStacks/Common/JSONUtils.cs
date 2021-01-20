// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.JSONUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlueStacks.Common
{
  public static class JSONUtils
  {
    public static string GetJSONArrayString(Dictionary<string, string> dict)
    {
      StringBuilder sb = new StringBuilder();
      using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
      {
        jsonWriter.WriteStartArray();
        jsonWriter.WriteStartObject();
        if (dict != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in dict)
          {
            jsonWriter.WritePropertyName(keyValuePair.Key);
            jsonWriter.WriteValue(keyValuePair.Value);
          }
        }
        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndArray();
      }
      return sb.ToString();
    }

    public static string GetJSONObjectString<T>(Dictionary<string, T> dict)
    {
      StringBuilder sb = new StringBuilder();
      using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
      {
        jsonWriter.WriteStartObject();
        if (dict != null)
        {
          foreach (KeyValuePair<string, T> keyValuePair in dict)
          {
            jsonWriter.WritePropertyName(keyValuePair.Key);
            jsonWriter.WriteValue((object) keyValuePair.Value);
          }
        }
        jsonWriter.WriteEndObject();
      }
      return sb.ToString();
    }

    public static string GetJSONObjectString(Dictionary<string, Dictionary<string, long>> dict)
    {
      StringBuilder sb = new StringBuilder();
      using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
      {
        jsonWriter.WriteStartObject();
        if (dict != null)
        {
          foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in dict)
          {
            jsonWriter.WritePropertyName(keyValuePair.Key);
            jsonWriter.WriteValue(JSONUtils.GetJSONObjectString<long>(keyValuePair.Value));
          }
        }
        jsonWriter.WriteEndObject();
      }
      return sb.ToString();
    }
  }
}
