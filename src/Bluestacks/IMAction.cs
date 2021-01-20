// Decompiled with JetBrains decompiler
// Type: IMAction
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

[Serializable]
public abstract class IMAction
{
  internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> sDictDevModeUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
  internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPropertyInfo = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
  internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPopUpUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
  private static Dictionary<KeyActionType, string> sPositionXPropertyName = new Dictionary<KeyActionType, string>();
  private static Dictionary<KeyActionType, string> sPositionYPropertyName = new Dictionary<KeyActionType, string>();
  internal static Dictionary<KeyActionType, string> sRadiusPropertyName = new Dictionary<KeyActionType, string>();
  private string mGuidanceCategory = "MISC";
  internal Direction Direction;
  internal IMAction ParentAction;
  internal bool IsChildAction;

  public IMAction()
  {
    this.GetPropertyInfo(nameof (Type));
    if (this.ParentAction != null)
      return;
    this.ParentAction = this;
  }

  public KeyActionType Type { get; set; }

  public Dictionary<string, string> Guidance { get; } = new Dictionary<string, string>();

  public string GuidanceCategory
  {
    get
    {
      return this.IsChildAction && this.ParentAction != this ? this.ParentAction.GuidanceCategory : this.GetCurrentGuidanceCategory();
    }
    set
    {
      this.mGuidanceCategory = value == null || string.IsNullOrEmpty(value.Trim()) ? "MISC" : value.Trim();
    }
  }

  private string GetCurrentGuidanceCategory()
  {
    if (string.IsNullOrEmpty(this.mGuidanceCategory))
      this.mGuidanceCategory = "MISC";
    return this.mGuidanceCategory;
  }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public bool Exclusive { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int ExclusiveDelay { get; set; } = 200;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string XExpr { get; set; } = string.Empty;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string YExpr { get; set; } = string.Empty;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string XOverlayOffset { get; set; } = string.Empty;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string YOverlayOffset { get; set; } = string.Empty;

  public string EnableCondition { get; set; } = string.Empty;

  public string StartCondition { get; set; }

  public string Note { get; set; }

  public string Comment { get; set; }

  internal double PositionX
  {
    get
    {
      double result;
      if (!double.TryParse(this[IMAction.sPositionXPropertyName[this.Type]].ToString(), out result))
        result = -1.0;
      return result;
    }
    set
    {
      this[IMAction.sPositionXPropertyName[this.Type]] = (object) value;
    }
  }

  internal double PositionY
  {
    get
    {
      double result;
      if (!double.TryParse(this[IMAction.sPositionYPropertyName[this.Type]].ToString(), out result))
        result = -1.0;
      return result;
    }
    set
    {
      this[IMAction.sPositionYPropertyName[this.Type]] = (object) value;
    }
  }

  internal double RadiusProperty
  {
    get
    {
      double result;
      if (!double.TryParse(this[IMAction.sRadiusPropertyName[this.Type]].ToString(), out result))
        result = -1.0;
      return result;
    }
    set
    {
      this[IMAction.sRadiusPropertyName[this.Type]] = (object) value;
    }
  }

  public bool IsVisibleInOverlay
  {
    get
    {
      bool result;
      if (!bool.TryParse(this["ShowOnOverlay"].ToString(), out result))
        result = false;
      return result;
    }
    set
    {
      this["ShowOnOverlay"] = (object) value;
    }
  }

  public object this[string propertyName]
  {
    get
    {
      object obj = (object) null;
      if ((object) this.GetPropertyInfo(propertyName) != null)
        obj = this.GetPropertyInfo(propertyName).GetValue((object) this, (object[]) null);
      return obj ?? (object) string.Empty;
    }
    set
    {
      try
      {
        PropertyInfo propertyInfo = this.GetPropertyInfo(propertyName);
        if ((object) propertyInfo == null)
          return;
        propertyInfo.SetValue((object) this, Convert.ChangeType(value, this.GetPropertyInfo(propertyName).PropertyType, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
      }
      catch (Exception ex)
      {
        Logger.Error(Constants.ImapLocaleStringsConstant + " error parsing variable set " + ex.ToString());
      }
    }
  }

  private PropertyInfo GetPropertyInfo(string propertyName)
  {
    PropertyInfo propertyInfo = (PropertyInfo) null;
    KeyActionType key = EnumHelper.Parse<KeyActionType>(this.GetType().Name, KeyActionType.Alias);
    this.Type = key;
    if (!IMAction.DictPropertyInfo.ContainsKey(key))
    {
      IMAction.DictPropertyInfo[key] = new Dictionary<string, PropertyInfo>();
      IMAction.DictPopUpUIElements[key] = new Dictionary<string, PropertyInfo>();
      IMAction.sDictDevModeUIElements[key] = new Dictionary<string, PropertyInfo>();
      IMAction.sPositionXPropertyName[key] = string.Empty;
      IMAction.sPositionYPropertyName[key] = string.Empty;
      IMAction.sRadiusPropertyName[key] = string.Empty;
      foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        IMAction.DictPropertyInfo[key].Add(property.Name, property);
        object[] customAttributes = property.GetCustomAttributes(typeof (DescriptionAttribute), true);
        if (customAttributes.Length != 0)
        {
          DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
          if (descriptionAttribute.Description.Contains("IMAP_CanvasElementY"))
            IMAction.sPositionXPropertyName[key] = property.Name;
          if (descriptionAttribute.Description.Contains("IMAP_CanvasElementX"))
            IMAction.sPositionYPropertyName[key] = property.Name;
          if (descriptionAttribute.Description.Contains("IMAP_CanvasElementRadius"))
            IMAction.sRadiusPropertyName[key] = property.Name;
          if (descriptionAttribute.Description.Contains("IMAP_PopupUIElement"))
            IMAction.DictPopUpUIElements[key].Add(property.Name, property);
          if (descriptionAttribute.Description.Contains("IMAP_DeveloperModeUIElemnt"))
            IMAction.sDictDevModeUIElements[key].Add(property.Name, property);
        }
      }
    }
    if (!string.IsNullOrEmpty(propertyName) && IMAction.DictPropertyInfo[key].ContainsKey(propertyName))
      propertyInfo = IMAction.DictPropertyInfo[key][propertyName];
    return propertyInfo;
  }

  internal List<BlueStacks.Common.Tuple<string, IMAction>> GetListGuidanceElements()
  {
    List<string> stringList = new List<string>();
    List<BlueStacks.Common.Tuple<string, IMAction>> tupleList = new List<BlueStacks.Common.Tuple<string, IMAction>>();
    foreach (KeyValuePair<string, string> keyValuePair in this.Guidance)
    {
      if (keyValuePair.Key.StartsWith("Key", StringComparison.InvariantCulture))
      {
        tupleList.Add(new BlueStacks.Common.Tuple<string, IMAction>(keyValuePair.Key, this));
        stringList.Add(keyValuePair.Key);
      }
    }
    foreach (KeyValuePair<string, PropertyInfo> keyValuePair in IMAction.DictPropertyInfo[this.Type])
    {
      if (!stringList.Contains(keyValuePair.Key) && (keyValuePair.Key.StartsWith("Key", StringComparison.InvariantCulture) || keyValuePair.Key.StartsWith("Sensitivity", StringComparison.InvariantCulture) || (keyValuePair.Key.StartsWith("MouseAcceleration", StringComparison.InvariantCulture) || keyValuePair.Key.StartsWith("EdgeScrollEnabled", StringComparison.InvariantCulture))))
        tupleList.Add(new BlueStacks.Common.Tuple<string, IMAction>(keyValuePair.Key, this));
    }
    return tupleList;
  }
}
