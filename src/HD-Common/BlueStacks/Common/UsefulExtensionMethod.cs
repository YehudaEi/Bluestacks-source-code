// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.UsefulExtensionMethod
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace BlueStacks.Common
{
  public static class UsefulExtensionMethod
  {
    public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
      return "{" + string.Join(",", dictionary.Select<KeyValuePair<TKey, TValue>, string>((Func<KeyValuePair<TKey, TValue>, string>) (kv => kv.Key?.ToString() + "=" + kv.Value?.ToString())).ToArray<string>()) + "}";
    }

    public static void AddIfNotContain<T>(this IList<T> list, T item)
    {
      if (list == null || list.Contains(item))
        return;
      list.Add(item);
    }

    public static void AddIfNotContain<T>(this IList<T> list, IList<T> itemList)
    {
      if (itemList == null)
        return;
      foreach (T obj in (IEnumerable<T>) itemList)
        list.AddIfNotContain<T>(obj);
    }

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
      return enumerable.RandomElementUsing<T>(new Random());
    }

    public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
    {
      int index = 0;
      if (rand != null)
        index = rand.Next(0, enumerable.Count<T>());
      return enumerable.ElementAt<T>(index);
    }

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
      return source != null && source.IndexOf(toCheck, comp) >= 0;
    }

    public static string GetDescription(this Enum value)
    {
      Enum @enum = value;
      DescriptionAttribute customAttribute = (DescriptionAttribute) Attribute.GetCustomAttribute(@enum != null ? (MemberInfo) ((IEnumerable<FieldInfo>) @enum.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)).Single<FieldInfo>((Func<FieldInfo, bool>) (x => x.GetValue((object) null).Equals((object) value))) : (MemberInfo) null, typeof (DescriptionAttribute));
      return customAttribute != null ? customAttribute.Description : value.ToString();
    }

    public static void LoadViewFromUri(this UserControl userControl, string baseUri)
    {
      Uri relativeUri = new Uri(baseUri, UriKind.Relative);
      Stream stream = ((PackagePart) typeof (Application).GetMethod("GetResourceOrContentPart", BindingFlags.Static | BindingFlags.NonPublic).Invoke((object) null, new object[1]
      {
        (object) relativeUri
      })).GetStream();
      Uri uri = new Uri((Uri) typeof (BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null, (object[]) null), relativeUri);
      ParserContext parserContext = new ParserContext()
      {
        BaseUri = uri
      };
      typeof (XamlReader).GetMethod("LoadBaml", BindingFlags.Static | BindingFlags.NonPublic).Invoke((object) null, new object[4]
      {
        (object) stream,
        (object) parserContext,
        (object) userControl,
        (object) true
      });
    }

    public static T GetObjectOfType<T>(this string val, T defaultValue)
    {
      return !string.IsNullOrEmpty(val) ? (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromString(val) : defaultValue;
    }

    public static T DeepCopy<T>(this T other)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize((Stream) memoryStream, (object) other);
        memoryStream.Position = 0L;
        return (T) binaryFormatter.Deserialize((Stream) memoryStream);
      }
    }

    public static void SetPlacement(this Window window, string placementXml)
    {
      try
      {
        WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetPlacement.Exception: " + ex.ToString());
      }
    }

    public static void SetPlacement(this Window window, double scalingFactor)
    {
      try
      {
        if (window == null)
          return;
        RECT placementRect = new RECT((int) Math.Floor(window.Left * scalingFactor), (int) Math.Floor(window.Top * scalingFactor), (int) Math.Floor((window.Left + window.ActualWidth) * scalingFactor), (int) Math.Floor((window.Top + window.ActualHeight) * scalingFactor));
        WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementRect);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in SetPlacement. " + ex?.ToString());
      }
    }

    public static string GetPlacement(this Window window)
    {
      return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
    }

    public static object GetPropValue(this object obj, string name, out Type objType)
    {
      objType = typeof (string);
      if (name != null)
      {
        string str = name;
        char[] chArray = new char[1]{ '.' };
        foreach (string name1 in str.Split(chArray))
        {
          if (obj == null)
            return (object) null;
          PropertyInfo property = obj.GetType().GetProperty(name1);
          if (property == null)
            return (object) null;
          obj = property.GetValue(obj, (object[]) null);
          objType = property.PropertyType;
        }
      }
      return obj;
    }

    public static T GetPropValue<T>(this object obj, string name)
    {
      object propValue = obj.GetPropValue(name, out Type _);
      return propValue == null ? default (T) : (T) propValue;
    }

    public static object ChangeType(this object obj, Type type)
    {
      if (!obj.IsList())
        return Convert.ChangeType(obj, type, (IFormatProvider) CultureInfo.InvariantCulture);
      List<object> list = ((IEnumerable) obj).Cast<object>().ToList<object>();
      Type containedType = type != null ? ((IEnumerable<Type>) type.GetGenericArguments()).First<Type>() : (Type) null;
      Func<object, object> selector = (Func<object, object>) (item => Convert.ChangeType(item, containedType, (IFormatProvider) CultureInfo.InvariantCulture));
      return (object) list.Select<object, object>(selector).ToList<object>();
    }

    public static bool IsList(this object o)
    {
      return o != null && o is IList && o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof (List<>));
    }

    public static bool? SetTextblockTooltip(this TextBlock textBlock)
    {
      if (textBlock == null)
        return new bool?();
      if (textBlock.IsTextTrimmed())
      {
        ToolTipService.SetIsEnabled((DependencyObject) textBlock, true);
        return new bool?(true);
      }
      ToolTipService.SetIsEnabled((DependencyObject) textBlock, false);
      return new bool?(false);
    }

    public static bool IsTextTrimmed(this CustomComboBox comboBox, string text)
    {
      if (comboBox == null)
        return false;
      Typeface typeface = new Typeface(comboBox.FontFamily, comboBox.FontStyle, comboBox.FontWeight, comboBox.FontStretch);
      return new FormattedText(text, Thread.CurrentThread.CurrentCulture, comboBox.FlowDirection, typeface, comboBox.FontSize, comboBox.Foreground)
      {
        MaxTextWidth = comboBox.ActualWidth,
        Trimming = TextTrimming.None
      }.Height > comboBox.ActualHeight;
    }

    public static bool IsTextTrimmed(this TextBlock textBlock)
    {
      if (textBlock == null)
        return false;
      Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
      return new FormattedText(textBlock.Text, Thread.CurrentThread.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground)
      {
        MaxTextWidth = textBlock.ActualWidth,
        Trimming = TextTrimming.None
      }.Height > textBlock.ActualHeight;
    }

    public static void SaveUserDefinedShortcuts(this ShortcutConfig mShortcutsConfigInstance)
    {
      if (mShortcutsConfigInstance == null)
        return;
      JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
      serializerSettings.Formatting = Formatting.Indented;
      RegistryManager.Instance.UserDefinedShortcuts = JsonConvert.SerializeObject((object) mShortcutsConfigInstance, serializerSettings);
    }
  }
}
