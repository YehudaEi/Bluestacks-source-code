// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.JSonTemplates
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
  internal class JSonTemplates
  {
    private static string mSuccessArrayJsonString = string.Empty;
    private static string mFailedArrayJsonString = string.Empty;
    private static string mSuccessJsonString = string.Empty;
    private static string mFailedJsonString = string.Empty;

    public static string SuccessArrayJSonTemplate
    {
      get
      {
        if (string.IsNullOrEmpty(JSonTemplates.mSuccessArrayJsonString))
          JSonTemplates.mSuccessArrayJsonString = new JArray()
          {
            (JToken) new JObject()
            {
              {
                "success",
                (JToken) true
              },
              {
                "reason",
                (JToken) ""
              }
            }
          }.ToString(Formatting.None);
        return JSonTemplates.mSuccessArrayJsonString;
      }
    }

    public static string FailedArrayJSonTemplate
    {
      get
      {
        if (string.IsNullOrEmpty(JSonTemplates.mFailedArrayJsonString))
          JSonTemplates.mFailedArrayJsonString = new JArray()
          {
            (JToken) new JObject()
            {
              {
                "success",
                (JToken) false
              },
              {
                "reason",
                (JToken) ""
              }
            }
          }.ToString(Formatting.None);
        return JSonTemplates.mFailedArrayJsonString;
      }
    }

    public static string SuccessJSonTemplate
    {
      get
      {
        if (string.IsNullOrEmpty(JSonTemplates.mSuccessJsonString))
          JSonTemplates.mSuccessJsonString = new JObject()
          {
            {
              "success",
              (JToken) true
            },
            {
              "reason",
              (JToken) ""
            }
          }.ToString(Formatting.None);
        return JSonTemplates.mSuccessJsonString;
      }
    }

    public static string FailedJSonTemplate
    {
      get
      {
        if (string.IsNullOrEmpty(JSonTemplates.mFailedJsonString))
          JSonTemplates.mFailedJsonString = new JObject()
          {
            {
              "success",
              (JToken) false
            },
            {
              "reason",
              (JToken) ""
            }
          }.ToString(Formatting.None);
        return JSonTemplates.mFailedJsonString;
      }
    }
  }
}
