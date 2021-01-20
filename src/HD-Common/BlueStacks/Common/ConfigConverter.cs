// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ConfigConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BlueStacks.Common
{
  public static class ConfigConverter
  {
    private static string DEFAULT_PROFILE_NAME = "Default";
    private static int mScriptRow = 0;
    private static int mScriptCol = 0;
    private static int mTiltCol = 0;
    private static int mTiltRow = 0;
    internal static string sImagesVersion = "Version 1";
    private const int THRESHOLD_VERSION = 13;
    public const int IMAGES_VERSION1_PARSER_VERSION = 16;
    public const string METADATA = "MetaData";
    public const string PARSER_VERSION = "ParserVersion";
    public const string COMMENT = "Comment";
    public const string COMMENT_VALUE = "Generated automatically from ver {0}";
    private const string PRIMITIVES = "Primitives";
    private const string SCHEMES = "Schemes";
    private const string TAGS = "Tags";
    private const string TAG = "Tag";
    public const string CONTROL_SCHEMES = "ControlSchemes";
    private const string NAME = "Name";
    private const string BUILT_IN = "BuiltIn";
    private const string SELECTED = "Selected";
    private const string IS_BOOKMARKED = "IsBookMarked";
    private const string KEYBOARD_LAYOUT = "KeyboardLayout";
    public const string IMAGES = "Images";
    private const string GAME_CONTROLS = "GameControls";
    private const string STRINGS = "Strings";
    private const string TYPE = "Type";
    private const string TYPE_ = "$type";
    private const string COMBO = "Combo";
    private const string STATE = "State";
    private const string TILT = "Tilt";
    private const string SCRIPT = "Script";
    private const string SCRIPT_ = "Script, Bluestacks";
    private const string DESCRIPTION = "Description";
    private const string X = "X";
    private const string Y = "Y";
    private const string EVENTS = "Events";
    private const string EVENT_TYPE = "EventType";
    private const string TIMESTAMP = "Timestamp";
    private const string KEY_NAME = "KeyName";
    private const string MSG = "Msg";
    private const string DELTA = "Delta";
    private const string COMMANDS = "Commands";
    public const string OVERLAY = "IsVisibleInOverlay";
    public const string SHOW_OVERLAY = "ShowOnOverlay";
    private const int mMaxRowCol = 8;
    private const int mTiltMaxRowCol = 2;

    public static bool Convert(
      string oldConfigPath,
      string newConfigPath,
      string newVersion,
      bool isBuiltIn,
      bool useCustomName)
    {
      try
      {
        JObject jobject1 = JObject.Parse(File.ReadAllText(oldConfigPath));
        string str;
        if (oldConfigPath == null)
          str = (string) null;
        else
          str = ((IEnumerable<string>) oldConfigPath.Split('\\')).Last<string>();
        str.Remove(str.Length - 4, 4);
        int? configVersion = ConfigConverter.GetConfigVersion(jobject1);
        int num1 = 13;
        if (configVersion.GetValueOrDefault() <= num1 & configVersion.HasValue)
        {
          JObject jobject2 = ConfigConverter.Convert(jobject1, newVersion, isBuiltIn, useCustomName);
          if (jobject2 != null)
          {
            File.WriteAllText(newConfigPath, jobject2.ToString());
            return true;
          }
        }
        else
        {
          configVersion = ConfigConverter.GetConfigVersion(jobject1);
          int num2 = 16;
          if (configVersion.GetValueOrDefault() < num2 & configVersion.HasValue && Utils.CheckIfImagesArrayPresentInCfg(jobject1))
          {
            JObject jobject2 = jobject1;
            foreach (JObject scheme in (IEnumerable<JToken>) jobject1["ControlSchemes"])
              scheme["Images"] = (JToken) ConfigConverter.ConvertImagesArrayForPV16(scheme);
            jobject2["MetaData"][(object) "Comment"] = (JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generated automatically from ver {0}", (object) (int) jobject2["MetaData"][(object) "ParserVersion"]);
            jobject2["MetaData"][(object) "ParserVersion"] = (JToken) 16;
            if (jobject2 != null)
            {
              File.WriteAllText(newConfigPath, jobject2.ToString());
              return true;
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while parsing config file {0}", (object) oldConfigPath), (object) ex.Message);
        return false;
      }
    }

    public static JObject Convert(
      JObject oldConfigJson,
      string newVersion,
      bool isBuiltIn,
      bool useCustomName)
    {
      if (useCustomName)
        ConfigConverter.DEFAULT_PROFILE_NAME = "Custom";
      List<string> source1 = new List<string>();
      if (oldConfigJson?["Primitives"] is JArray jarray)
      {
        foreach (JToken jtoken1 in jarray)
        {
          if (jtoken1[(object) "Tags"] is JArray jarray)
          {
            foreach (JToken jtoken2 in jarray)
              source1.Add(jtoken2.ToString());
          }
        }
      }
      List<string> list = source1.Distinct<string>().ToList<string>();
      string empty = string.Empty;
      JArray source2 = new JArray();
      if (!list.Any<string>())
      {
        source2.Add((JToken) new JObject()
        {
          {
            "Name",
            (JToken) ConfigConverter.DEFAULT_PROFILE_NAME
          },
          {
            "BuiltIn",
            (JToken) isBuiltIn
          },
          {
            "Selected",
            (JToken) true
          },
          {
            "IsBookMarked",
            (JToken) false
          },
          {
            "KeyboardLayout",
            oldConfigJson["MetaData"][(object) "KeyboardLayout"]
          },
          {
            "GameControls",
            (JToken) new JArray()
          },
          {
            "Images",
            (JToken) new JArray()
          }
        });
      }
      else
      {
        if (oldConfigJson["Schemes"] != null && oldConfigJson["Schemes"] is JArray source3 && source3.Any<JToken>())
        {
          JToken jtoken = oldConfigJson["Schemes"].Where<JToken>((Func<JToken, bool>) (scheme => bool.Parse(scheme[(object) "Selected"].ToString()))).FirstOrDefault<JToken>();
          if (jtoken != null && jtoken[(object) "Tag"] != null)
            empty = jtoken[(object) "Tag"].ToString();
        }
        foreach (string str in list)
          source2.Add((JToken) new JObject()
          {
            {
              "Name",
              (JToken) str.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            },
            {
              "BuiltIn",
              (JToken) isBuiltIn
            },
            {
              "Selected",
              (JToken) string.Equals(empty, str.ToString((IFormatProvider) CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
            },
            {
              "IsBookMarked",
              (JToken) false
            },
            {
              "KeyboardLayout",
              oldConfigJson["MetaData"][(object) "KeyboardLayout"]
            },
            {
              "GameControls",
              (JToken) new JArray()
            },
            {
              "Images",
              (JToken) new JArray()
            }
          });
        if (string.IsNullOrEmpty(empty))
          source2[0][(object) "Selected"] = (JToken) true;
      }
      foreach (JToken jtoken in (IEnumerable<JToken>) oldConfigJson["Primitives"])
      {
        if (jtoken.DeepClone() is JObject primitive)
        {
          List<string> tags = new List<string>();
          if (primitive["Tags"] != null)
          {
            primitive["Tags"].ToList<JToken>().ForEach((System.Action<JToken>) (x => tags.Add(x.ToString())));
            primitive["Tags"].Parent.Remove();
          }
          ConfigConverter.ConvertComboSequences(primitive);
          ConfigConverter.UpdateTiltAndStatePrimitives(primitive);
          if (!tags.Any<string>())
            ConfigConverter.AddPrimitiveToGameControls((IEnumerable<JToken>) source2, (JToken) primitive);
          else
            ConfigConverter.AddPrimitiveToGameControls(source2.ToList<JToken>().Where<JToken>((Func<JToken, bool>) (scheme => tags.Contains(scheme[(object) "Name"].ToString()))), (JToken) primitive);
        }
      }
      if (!string.IsNullOrEmpty(empty) && !list.Contains(empty))
        source2.Add((JToken) new JObject()
        {
          {
            "Name",
            (JToken) empty
          },
          {
            "BuiltIn",
            (JToken) isBuiltIn
          },
          {
            "Selected",
            (JToken) true
          },
          {
            "IsBookMarked",
            (JToken) false
          },
          {
            "KeyboardLayout",
            oldConfigJson["MetaData"][(object) "KeyboardLayout"]
          },
          {
            "GameControls",
            (JToken) new JArray()
          },
          {
            "Images",
            (JToken) new JArray()
          }
        });
      return new JObject()
      {
        {
          "MetaData",
          (JToken) ConfigConverter.GetMetadata(oldConfigJson["MetaData"], newVersion)
        },
        {
          "ControlSchemes",
          (JToken) source2
        },
        {
          "Strings",
          oldConfigJson["Strings"].DeepClone()
        }
      };
    }

    public static int? GetConfigVersion(string config)
    {
      try
      {
        return ConfigConverter.GetConfigVersion(JObject.Parse(File.ReadAllText(config)));
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while parsing config file {0}", (object) config), (object) ex.Message);
        return new int?();
      }
    }

    public static int? GetConfigVersion(JObject configJson)
    {
      int result;
      return configJson != null && configJson["MetaData"] != null && (configJson["MetaData"][(object) "ParserVersion"] != null && int.TryParse(configJson["MetaData"][(object) "ParserVersion"].ToString(), out result)) ? new int?(result) : new int?();
    }

    private static void AddPrimitiveToGameControls(
      IEnumerable<JToken> controlSchemes,
      JToken primitiveCopy)
    {
      controlSchemes.ToList<JToken>().ForEach((System.Action<JToken>) (scheme =>
      {
        if (!(scheme[(object) "GameControls"] is JArray jarray))
          return;
        jarray.Add(primitiveCopy);
      }));
    }

    private static JObject GetMetadata(JToken oldMetadata, string newVersion)
    {
      return new JObject()
      {
        {
          "ParserVersion",
          (JToken) newVersion
        },
        {
          "Comment",
          (JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generated automatically from ver {0}", (object) oldMetadata[(object) "ParserVersion"])
        }
      };
    }

    private static void ConvertComboSequences(JObject primitive)
    {
      if (primitive["Type"] == null || !string.Equals(primitive["Type"].ToString(), "Combo", StringComparison.OrdinalIgnoreCase))
        return;
      primitive.Add("X", (JToken) ConfigConverter.GetLocationForPoint(ConfigConverter.mScriptCol, 8));
      primitive.Add("Y", (JToken) ConfigConverter.GetLocationForPoint(ConfigConverter.mScriptRow, 8));
      ++ConfigConverter.mScriptCol;
      if (ConfigConverter.mScriptCol == 8)
      {
        ++ConfigConverter.mScriptRow;
        ConfigConverter.mScriptRow %= 8;
      }
      ConfigConverter.mScriptCol %= 8;
      primitive["Type"] = (JToken) "Script";
      primitive["$type"] = (JToken) "Script, Bluestacks";
      primitive["IsVisibleInOverlay"] = (JToken) true;
      primitive["ShowOnOverlay"] = (JToken) true;
      primitive.Add("Comment", primitive["Description"]);
      primitive["Description"].Parent.Remove();
      if (primitive["Events"] == null || !(primitive["Events"] is JArray jarray))
        return;
      JArray jarray1 = new JArray();
      int num1 = 0;
      foreach (JToken jtoken in jarray)
      {
        int result1;
        if (int.TryParse(jtoken[(object) "Timestamp"].ToString(), out result1))
        {
          int num2 = result1 - num1;
          jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "wait {0}", (object) num2));
          ConfigConverter.ComboEventType result2;
          if (jtoken[(object) "EventType"] != null && EnumHelper.TryParse<ConfigConverter.ComboEventType>(jtoken[(object) "EventType"].ToString(), out result2))
          {
            switch (result2)
            {
              case ConfigConverter.ComboEventType.MouseDown:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "mouseDown {0} {1}", (object) jtoken[(object) "X"].ToString(), (object) jtoken[(object) "Y"].ToString()));
                break;
              case ConfigConverter.ComboEventType.MouseUp:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "mouseUp {0} {1}", (object) jtoken[(object) "X"].ToString(), (object) jtoken[(object) "Y"].ToString()));
                break;
              case ConfigConverter.ComboEventType.MouseMove:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "mouseMove {0} {1}", (object) jtoken[(object) "X"].ToString(), (object) jtoken[(object) "Y"].ToString()));
                break;
              case ConfigConverter.ComboEventType.MouseWheel:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "mouseWheel {0} {1} {2}", (object) jtoken[(object) "X"].ToString(), (object) jtoken[(object) "Y"].ToString(), (object) jtoken[(object) "Delta"].ToString()));
                break;
              case ConfigConverter.ComboEventType.KeyDown:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "keyDown {0}", (object) jtoken[(object) "KeyName"].ToString()));
                break;
              case ConfigConverter.ComboEventType.KeyUp:
                jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "keyUp {0}", (object) jtoken[(object) "KeyName"].ToString()));
                break;
              case ConfigConverter.ComboEventType.IME:
                string[] strArray = jtoken[(object) "Msg"].ToString().Split(' ');
                string a = strArray[1].Split('=')[1];
                if (!string.Equals(a, "0", StringComparison.OrdinalIgnoreCase))
                  jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "text backspace {0}", (object) a));
                if (!string.IsNullOrEmpty(strArray[0].Split('_')[1]))
                {
                  jarray1.Add((JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "text {0}", (object) strArray[0].Split('_')[1]));
                  break;
                }
                break;
            }
          }
        }
        num1 = result1;
      }
      primitive.Add("Commands", (JToken) jarray1);
      primitive["Events"].Parent.Remove();
    }

    private static void UpdateTiltAndStatePrimitives(JObject primitive)
    {
      if ((primitive["Type"] == null || !string.Equals(primitive["Type"].ToString(), "State", StringComparison.OrdinalIgnoreCase)) && !string.Equals(primitive["Type"].ToString(), "Tilt", StringComparison.OrdinalIgnoreCase))
        return;
      primitive.Add("X", (JToken) ConfigConverter.GetLocationForPoint(ConfigConverter.mTiltCol, 2));
      primitive.Add("Y", (JToken) ConfigConverter.GetLocationForPoint(ConfigConverter.mTiltRow, 2));
      ++ConfigConverter.mTiltCol;
      if (ConfigConverter.mTiltCol == 2)
      {
        ++ConfigConverter.mTiltRow;
        ConfigConverter.mTiltRow %= 2;
      }
      ConfigConverter.mTiltCol %= 2;
    }

    private static int GetLocationForPoint(int _location, int _maxCol)
    {
      int num = 100 / _maxCol;
      return num / 2 + _location * num;
    }

    public static JArray ConvertImagesArrayForPV16(JObject scheme)
    {
      JArray jarray = new JArray();
      if (scheme != null && scheme["Images"] != null && ((JContainer) scheme["Images"]).Count > 0)
      {
        foreach (JToken jtoken in (IEnumerable<JToken>) scheme["Images"])
        {
          JObject jobject = new JObject();
          jobject.Add("ImageId", jtoken[(object) "ImageId"]);
          jobject.Add("ImageType", (JToken) ConfigConverter.sImagesVersion);
          if (jtoken[(object) "ImageType"] != null)
          {
            jobject.Add("TextureCRC", jtoken[(object) "TextureCRC"]);
            jobject.Add("TextureIndex", jtoken[(object) "TextureIndex"]);
            jobject.Add("TextureCoord", jtoken[(object) "TextureCoord"]);
            jobject.Add("VerticalIndex", jtoken[(object) "VerticalIndex"]);
            jobject.Add("VertexRect", (JToken) "VertexRect");
          }
          else
          {
            jobject.Add("TextureCRC", jtoken[(object) "Texture"]?[(object) "CRC"]);
            jobject.Add("TextureIndex", jtoken[(object) "VarState"]?[(object) 0]?[(object) 0]?[(object) "Index"]);
            jobject.Add("TextureCoord", jtoken[(object) "VarState"]?[(object) 0]?[(object) 0]?[(object) "Buffer"]);
            jobject.Add("VerticalIndex", (JToken) 0);
            jobject.Add("VertexRect", (JToken) new JArray());
          }
          jarray.Add((JToken) jobject);
        }
      }
      return jarray;
    }

    private enum ComboEventType
    {
      None,
      MouseDown,
      MouseUp,
      MouseMove,
      MouseWheel,
      KeyDown,
      KeyUp,
      IME,
    }
  }
}
