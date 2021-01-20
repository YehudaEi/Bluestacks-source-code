// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppPackageListObject
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public class AppPackageListObject
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "app_pkg_list")]
    public List<AppPackageObject> CloudPackageList { get; set; } = new List<AppPackageObject>();

    public AppPackageListObject()
    {
    }

    public AppPackageListObject(List<AppPackageObject> packageList)
    {
      if (packageList == null)
        return;
      this.CloudPackageList = packageList;
    }

    public AppPackageListObject(List<string> packageList)
    {
      if (packageList == null)
        return;
      List<AppPackageObject> appPackageObjectList = new List<AppPackageObject>();
      foreach (string package in packageList)
        appPackageObjectList.Add(new AppPackageObject()
        {
          Package = package
        });
      this.CloudPackageList = appPackageObjectList;
    }

    public bool IsPackageAvailable(string appPackage)
    {
      foreach (AppPackageObject cloudPackage in this.CloudPackageList)
      {
        string str = cloudPackage.Package;
        if (cloudPackage.Package.EndsWith("*", StringComparison.InvariantCulture))
          str = cloudPackage.Package.TrimEnd('*');
        if (str.StartsWith("~", StringComparison.InvariantCulture))
        {
          if (appPackage.StartsWith(str.Substring(1), StringComparison.InvariantCulture))
            return false;
        }
        else if (appPackage.StartsWith(str, StringComparison.InvariantCulture))
          return true;
      }
      return false;
    }

    public AppPackageObject GetAppPackageObject(string appPackage)
    {
      foreach (AppPackageObject cloudPackage in this.CloudPackageList)
      {
        string str = cloudPackage.Package;
        if (cloudPackage.Package.EndsWith("*", StringComparison.InvariantCulture))
          str = cloudPackage.Package.TrimEnd('*');
        if (str.StartsWith("~", StringComparison.InvariantCulture))
        {
          if (appPackage.StartsWith(str.Substring(1), StringComparison.InvariantCulture))
            return (AppPackageObject) null;
        }
        else if (appPackage.StartsWith(str, StringComparison.InvariantCulture))
          return cloudPackage;
      }
      return (AppPackageObject) null;
    }
  }
}
