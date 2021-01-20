// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DataModificationTracker
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlueStacks.Common
{
  [Serializable]
  public class DataModificationTracker
  {
    private object _PreviousObject;
    private List<string> _IgnoreList;
    private bool _IsRecursive;

    public IList<string> ChangedProperties { get; } = (IList<string>) new List<string>();

    public void Lock(object previousObject, List<string> ignoreList = null, bool isRecursive = false)
    {
      this._PreviousObject = previousObject;
      this._IgnoreList = ignoreList == null ? new List<string>() : ignoreList;
      this._IgnoreList.Add(nameof (DataModificationTracker));
      this._IsRecursive = isRecursive;
    }

    public bool HasChanged(object currentObject)
    {
      this.ChangedProperties.Clear();
      return !this.AreObjectsEqual(this._PreviousObject, currentObject) || this.ChangedProperties.Count > 0;
    }

    private bool AreObjectsEqual(object objectA, object objectB)
    {
      bool flag = true;
      if (objectA == null || objectB == null)
      {
        flag = object.Equals(objectA, objectB);
      }
      else
      {
        Type type = objectA.GetType();
        foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.CanRead && !this._IgnoreList.Contains(prop.Name))))
        {
          object valueA = propertyInfo.GetValue(objectA, (object[]) null);
          object valueB = propertyInfo.GetValue(objectB, (object[]) null);
          if (DataModificationTracker.CanDirectlyCompare(propertyInfo.PropertyType))
          {
            if (!DataModificationTracker.AreValuesEqual(valueA, valueB))
            {
              this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
              flag = false;
            }
          }
          else if (typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
          {
            if (valueA == null && valueB != null || valueA != null && valueB == null)
            {
              this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
              flag = false;
            }
            else if (valueA != null && valueB != null)
            {
              IEnumerable<object> source1 = ((IEnumerable) valueA).Cast<object>();
              IEnumerable<object> source2 = ((IEnumerable) valueB).Cast<object>();
              if (source1.Count<object>() != source2.Count<object>())
              {
                this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
                flag = false;
              }
              else
              {
                for (int index = 0; index < source1.Count<object>(); ++index)
                {
                  object obj1 = source1.ElementAt<object>(index);
                  object obj2 = source2.ElementAt<object>(index);
                  if (DataModificationTracker.CanDirectlyCompare(obj1.GetType()))
                  {
                    if (!DataModificationTracker.AreValuesEqual(obj1, obj2))
                    {
                      this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
                      flag = false;
                    }
                  }
                  else if (!this.AreObjectsEqual(obj1, obj2))
                  {
                    this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
                    flag = false;
                  }
                }
              }
            }
          }
          else if (propertyInfo.PropertyType.IsClass && this._IsRecursive)
          {
            if (!this.AreObjectsEqual(propertyInfo.GetValue(objectA, (object[]) null), propertyInfo.GetValue(objectB, (object[]) null)))
            {
              this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
              flag = false;
            }
          }
          else
          {
            this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
            flag = false;
          }
        }
      }
      return flag;
    }

    private static bool CanDirectlyCompare(Type type)
    {
      return typeof (IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
    }

    private static bool AreValuesEqual(object valueA, object valueB)
    {
      IComparable comparable = valueA as IComparable;
      return (valueA != null || valueB == null) && (valueA == null || valueB != null) && ((comparable == null || comparable.CompareTo(valueB) == 0) && object.Equals(valueA, valueB));
    }
  }
}
