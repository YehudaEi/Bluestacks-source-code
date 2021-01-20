// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppRequirementsParser
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.Grm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BlueStacks.Common
{
  public class AppRequirementsParser
  {
    private static object syncRoot = new object();
    private Dictionary<string, Dictionary<string, string>> mTranslations = new Dictionary<string, Dictionary<string, string>>();
    private static volatile AppRequirementsParser sInstance;
    private List<AppRequirement> mRequirements;
    private readonly string mRequirementsJsonFile;
    private readonly string mRequirementsTranslationsFile;

    public static AppRequirementsParser Instance
    {
      get
      {
        if (AppRequirementsParser.sInstance == null)
        {
          lock (AppRequirementsParser.syncRoot)
          {
            if (AppRequirementsParser.sInstance == null)
              AppRequirementsParser.sInstance = new AppRequirementsParser();
          }
        }
        return AppRequirementsParser.sInstance;
      }
    }

    public List<AppRequirement> Requirements
    {
      get
      {
        return this.mRequirements;
      }
      set
      {
        this.mRequirements = value;
        EventHandler requirementConfigUpdated = this.RequirementConfigUpdated;
        if (requirementConfigUpdated == null)
          return;
        requirementConfigUpdated((object) this, new EventArgs());
      }
    }

    public event EventHandler RequirementConfigUpdated;

    private AppRequirementsParser()
    {
      this.mRequirementsJsonFile = Path.Combine(RegistryStrings.GadgetDir, "requirements.json");
      this.mRequirementsTranslationsFile = Path.Combine(RegistryStrings.GadgetDir, "req_trans.json");
    }

    public void PopulateRequirementsFromFile()
    {
      string str1 = "[]";
      string str2 = "[]";
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppRequirementUpdate"))
      {
        if (mutex.WaitOne())
        {
          try
          {
            if (!File.Exists(this.mRequirementsJsonFile))
            {
              using (StreamWriter streamWriter = new StreamWriter(this.mRequirementsJsonFile, true))
              {
                streamWriter.Write("[");
                streamWriter.WriteLine();
                streamWriter.Write("]");
              }
            }
            using (StreamReader streamReader = new StreamReader((Stream) new FileStream(this.mRequirementsJsonFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
              str1 = streamReader.ReadToEnd();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to get app requirements");
            Logger.Error(ex.ToString());
          }
          try
          {
            if (!File.Exists(this.mRequirementsTranslationsFile))
            {
              using (StreamWriter streamWriter = new StreamWriter(this.mRequirementsTranslationsFile, true))
                streamWriter.Write("{}");
            }
            using (StreamReader streamReader = new StreamReader((Stream) new FileStream(this.mRequirementsTranslationsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
              str2 = streamReader.ReadToEnd();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to get app requirements translations from file");
            Logger.Error(ex.ToString());
          }
          finally
          {
            mutex.ReleaseMutex();
          }
        }
      }
      List<AppRequirement> appRequirementList = new List<AppRequirement>();
      try
      {
        appRequirementList = JsonConvert.DeserializeObject<List<AppRequirement>>(str1, Utils.GetSerializerSettings());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in parsing apprequirement json " + ex?.ToString());
      }
      Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
      try
      {
        dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(str2, Utils.GetSerializerSettings());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in parsing req translations json " + ex?.ToString());
      }
      this.Requirements = appRequirementList;
      if (dictionary == null || dictionary.Count <= 0)
        return;
      this.mTranslations = dictionary;
    }

    public void UpdateOverwriteRequirements(string fullJson, string translationJson)
    {
      List<AppRequirement> appRequirements = JsonConvert.DeserializeObject<List<AppRequirement>>(fullJson, Utils.GetSerializerSettings());
      Dictionary<string, Dictionary<string, string>> translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(translationJson, Utils.GetSerializerSettings());
      this.SaveRequirements(appRequirements, translations);
      if (translations.Count > 0)
        this.mTranslations = translations;
      this.Requirements = appRequirements;
    }

    public string GetLocalizedString(string key)
    {
      string closestMatchingLocale = Globalization.FindClosestMatchingLocale(RegistryManager.Instance.UserSelectedLocale);
      return this.mTranslations.ContainsKey(closestMatchingLocale) && this.mTranslations[closestMatchingLocale].ContainsKey(key) ? this.mTranslations[closestMatchingLocale][key] : key;
    }

    private void SaveRequirements(
      List<AppRequirement> appRequirements,
      Dictionary<string, Dictionary<string, string>> translations)
    {
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppRequirementUpdate"))
      {
        if (!mutex.WaitOne())
          return;
        try
        {
          FileInfo fileInfo = new FileInfo(this.mRequirementsJsonFile + ".tmp");
          using (TextWriter textWriter = (TextWriter) new StreamWriter((Stream) fileInfo.Open(FileMode.Create)))
          {
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter(textWriter))
              JsonSerializer.Create(Utils.GetSerializerSettings()).Serialize(jsonWriter, (object) appRequirements);
          }
          File.Copy(this.mRequirementsJsonFile, this.mRequirementsJsonFile + ".bak", true);
          File.Delete(this.mRequirementsJsonFile);
          int num = 10;
          while (File.Exists(this.mRequirementsJsonFile) && num > 0)
          {
            --num;
            Thread.Sleep(100);
          }
          File.Move(this.mRequirementsJsonFile + ".tmp", this.mRequirementsJsonFile);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to write/move requirements to apps json file");
          Logger.Error(ex.ToString());
        }
        try
        {
          FileInfo fileInfo = new FileInfo(this.mRequirementsTranslationsFile + ".tmp");
          using (TextWriter textWriter = (TextWriter) new StreamWriter((Stream) fileInfo.Open(FileMode.Create)))
          {
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter(textWriter))
              JsonSerializer.Create(Utils.GetSerializerSettings()).Serialize(jsonWriter, (object) translations);
          }
          File.Copy(this.mRequirementsTranslationsFile, this.mRequirementsTranslationsFile + ".bak", true);
          File.Delete(this.mRequirementsTranslationsFile);
          int num = 10;
          while (File.Exists(this.mRequirementsTranslationsFile) && num > 0)
          {
            --num;
            Thread.Sleep(100);
          }
          File.Move(this.mRequirementsTranslationsFile + ".tmp", this.mRequirementsTranslationsFile);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to write/move requirements translations to req translations json file");
          Logger.Error(ex.ToString());
        }
        finally
        {
          mutex.ReleaseMutex();
        }
      }
    }
  }
}
