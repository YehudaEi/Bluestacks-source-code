// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.JsonFormattingConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  public class JsonFormattingConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof (List<double>);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer?.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
    }
  }
}
