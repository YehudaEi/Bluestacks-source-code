// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.JsonParser
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BlueStacks.Common
{
  public class JsonParser
  {
    private string mAppsDotJsonFile;
    private AppInfo[] mOriginalJson;

    public string VmName { get; set; }

    public JsonParser(string vmName)
    {
      if (string.IsNullOrEmpty(vmName))
      {
        this.VmName = "";
        this.mAppsDotJsonFile = Path.Combine(RegistryStrings.GadgetDir, "systemApps.json");
      }
      else
      {
        this.VmName = vmName;
        this.mAppsDotJsonFile = Path.Combine(RegistryStrings.GadgetDir, "apps_" + vmName + ".json");
      }
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
      {
        if (!mutex.WaitOne())
          return;
        try
        {
          JsonParser.DeleteIfInvalidJsonFile(this.mAppsDotJsonFile);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to delete invalid json file... Err : " + ex.ToString());
        }
        finally
        {
          mutex.ReleaseMutex();
        }
      }
    }

    public static List<string> GetInstalledAppsList(string vmName)
    {
      List<string> stringList = new List<string>();
      AppInfo[] appList = new JsonParser(vmName).GetAppList();
      for (int index = 0; index < appList.Length; ++index)
      {
        if (appList[index] != null && appList[index].Package != null)
          stringList.Add(appList[index].Package);
      }
      return stringList;
    }

    public AppInfo[] GetAppList()
    {
      string json = "[]";
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
      {
        if (mutex.WaitOne())
        {
          try
          {
            if (!File.Exists(this.mAppsDotJsonFile))
            {
              using (StreamWriter streamWriter = new StreamWriter(this.mAppsDotJsonFile, true))
              {
                streamWriter.Write("[");
                streamWriter.WriteLine();
                streamWriter.Write("]");
              }
            }
            StreamReader streamReader = new StreamReader(this.mAppsDotJsonFile);
            json = streamReader.ReadToEnd();
            streamReader.Close();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to create empty app json... Err : " + ex.ToString());
          }
          finally
          {
            mutex.ReleaseMutex();
          }
        }
      }
      this.GetOriginalJson(JArray.Parse(json));
      return this.mOriginalJson;
    }

    private void GetOriginalJson(JArray input)
    {
      this.mOriginalJson = new AppInfo[input.Count];
      for (int index = 0; index < input.Count; ++index)
        this.mOriginalJson[index] = JsonConvert.DeserializeObject<AppInfo>(input[index].ToString());
    }

    public int GetInstalledAppCount()
    {
      this.GetAppList();
      int num = 0;
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (string.Compare(this.mOriginalJson[index].Activity, ".Main", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(this.mOriginalJson[index].Appstore, "yes", StringComparison.OrdinalIgnoreCase) != 0)
          ++num;
      }
      return num;
    }

    public bool GetAppInfoFromAppName(
      string appName,
      out string packageName,
      out string imageName,
      out string activityName)
    {
      packageName = (string) null;
      imageName = (string) null;
      activityName = (string) null;
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Name == appName)
        {
          packageName = this.mOriginalJson[index].Package;
          imageName = this.mOriginalJson[index].Img;
          activityName = this.mOriginalJson[index].Activity;
          return true;
        }
      }
      return false;
    }

    public bool GetAppInfoFromPackageName(
      string packageName,
      out string appName,
      out string imageName,
      out string activityName,
      out string appstore)
    {
      appName = "";
      imageName = "";
      activityName = "";
      appstore = "";
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
        {
          appName = this.mOriginalJson[index].Name;
          imageName = this.mOriginalJson[index].Img;
          activityName = this.mOriginalJson[index].Activity;
          appstore = this.mOriginalJson[index].Appstore;
          return true;
        }
      }
      return false;
    }

    public AppInfo GetAppInfoFromPackageName(string packageName)
    {
      AppInfo appInfo = (AppInfo) null;
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
          appInfo = this.mOriginalJson[index];
      }
      return appInfo;
    }

    public string GetPackageNameFromAppName(string appName)
    {
      AppInfo appInfo = (AppInfo) null;
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Name == appName)
          appInfo = this.mOriginalJson[index];
      }
      return appInfo == null ? string.Empty : appInfo.Package;
    }

    public string GetAppNameFromPackageActivity(string packageName, string activityName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName && this.mOriginalJson[index].Activity == activityName)
          return this.mOriginalJson[index].Name;
      }
      return string.Empty;
    }

    public string GetAppNameFromPackage(string packageName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
          return this.mOriginalJson[index].Name;
      }
      return string.Empty;
    }

    public static bool GetGl3RequirementFromPackage(AppInfo[] appJson, string packageName)
    {
      if (appJson != null)
      {
        for (int index = 0; index < appJson.Length; ++index)
        {
          if (appJson[index].Package == packageName)
            return appJson[index].Gl3Required;
        }
      }
      return false;
    }

    public static bool GetVideoPresentRequirementFromPackage(AppInfo[] appJson, string packageName)
    {
      if (appJson != null)
      {
        for (int index = 0; index < appJson.Length; ++index)
        {
          if (appJson[index].Package == packageName)
            return appJson[index].VideoPresent;
        }
      }
      return false;
    }

    public string GetPackageNameFromActivityName(string activityName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Activity == activityName)
          return this.mOriginalJson[index].Package;
      }
      return string.Empty;
    }

    public string GetActivityNameFromPackageName(string packageName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
          return this.mOriginalJson[index].Activity;
      }
      return string.Empty;
    }

    public bool IsPackageNameSystemApp(string packageName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
          return this.mOriginalJson[index].System == "1";
      }
      return false;
    }

    public bool IsAppNameSystemApp(string appName)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Name == appName)
          return this.mOriginalJson[index].System == "1";
      }
      return false;
    }

    public bool IsAppInstalled(string packageName)
    {
      return this.IsAppInstalled(packageName, out string _);
    }

    public bool IsAppInstalled(string packageName, out string version)
    {
      this.GetAppList();
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == packageName)
        {
          version = this.mOriginalJson[index].Version;
          return true;
        }
      }
      version = "NA";
      return false;
    }

    public bool GetAppData(string package, string activity, out string name, out string img)
    {
      this.GetAppList();
      name = "";
      img = "";
      for (int index = 0; index < this.mOriginalJson.Length; ++index)
      {
        if (this.mOriginalJson[index].Package == package && this.mOriginalJson[index].Activity == activity)
        {
          name = this.mOriginalJson[index].Name;
          img = this.mOriginalJson[index].Img;
          Logger.Info("Got AppName: {0} and AppIcon: {1}", (object) name, (object) img);
          return true;
        }
      }
      return false;
    }

    public void WriteJson(AppInfo[] json)
    {
      JArray jarray = new JArray();
      Logger.Info("JsonParser: Writing json object array to json writer");
      if (json != null)
      {
        for (int index = 0; index < json.Length; ++index)
        {
          JObject jobject = new JObject()
          {
            {
              "img",
              (JToken) json[index].Img
            },
            {
              "name",
              (JToken) json[index].Name
            },
            {
              "system",
              (JToken) json[index].System
            },
            {
              "package",
              (JToken) json[index].Package
            },
            {
              "appstore",
              (JToken) json[index].Appstore
            },
            {
              "activity",
              (JToken) json[index].Activity
            },
            {
              "version",
              (JToken) json[index].Version
            },
            {
              "versionName",
              (JToken) json[index].VersionName
            },
            {
              "gl3required",
              (JToken) json[index].Gl3Required
            },
            {
              "videopresent",
              (JToken) json[index].VideoPresent
            },
            {
              "isgamepadcompatible",
              (JToken) json[index].IsGamepadCompatible
            }
          };
          if (json[index].Url != null)
            jobject.Add("url", (JToken) json[index].Url);
          jarray.Add((JToken) jobject);
        }
      }
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
      {
        if (!mutex.WaitOne())
          return;
        try
        {
          StreamWriter streamWriter = new StreamWriter(this.mAppsDotJsonFile + ".tmp");
          streamWriter.Write(jarray.ToString(Formatting.None));
          streamWriter.Close();
          File.Copy(this.mAppsDotJsonFile + ".tmp", this.mAppsDotJsonFile + ".bak", true);
          File.Delete(this.mAppsDotJsonFile);
          int num = 10;
          while (File.Exists(this.mAppsDotJsonFile) && num > 0)
          {
            --num;
            Thread.Sleep(100);
          }
          File.Move(this.mAppsDotJsonFile + ".tmp", this.mAppsDotJsonFile);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to write in apps json file... Err : " + ex.ToString());
        }
        finally
        {
          mutex.ReleaseMutex();
        }
      }
    }

    public int AddToJson(AppInfo json)
    {
      this.GetAppList();
      Logger.Info("Adding to Json");
      AppInfo[] json1 = new AppInfo[this.mOriginalJson.Length + 1];
      int index;
      for (index = 0; index < this.mOriginalJson.Length; ++index)
        json1[index] = this.mOriginalJson[index];
      json1[index] = json;
      this.WriteJson(json1);
      return this.mOriginalJson.Length;
    }

    public static void DeleteIfInvalidJsonFile(string fileName)
    {
      try
      {
        if (JsonParser.IsValidJsonFile(fileName))
          return;
        File.Delete(fileName);
      }
      catch (Exception ex)
      {
        Logger.Error("Some error in deleting file, ex: " + ex.Message);
      }
    }

    private static bool IsValidJsonFile(string fileName)
    {
      try
      {
        JArray.Parse(File.ReadAllText(fileName));
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Invalid JSon file: " + fileName);
        Logger.Error(ex.Message);
        return false;
      }
    }
  }
}
