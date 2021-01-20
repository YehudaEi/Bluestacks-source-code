// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.LogCollectorUtils
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BlueStacks.LogCollector
{
  internal class LogCollectorUtils
  {
    private static string sVmName = MultiInstanceStrings.VmName;
    internal static string[] sProblemCategories = (string[]) null;
    internal static Dictionary<string, Dictionary<string, string>> sCategorySubcategoryMapping = (Dictionary<string, Dictionary<string, string>>) null;
    internal static Dictionary<string, Dictionary<string, string>> sCategorySubcategoryMappingWithDropdown = (Dictionary<string, Dictionary<string, string>>) null;
    internal static Hashtable sStringConversions = (Hashtable) null;
    internal static Dictionary<string, string> sCategoryShowDropdownMapping = (Dictionary<string, string>) null;
    internal static string sRedColorHex = "#FF402F";
    internal static string sDefaultBorderColorHex = "#99A9CF";

    internal static void GetCategoriesInBackground()
    {
      using (BackgroundWorker backgroundWorker = new BackgroundWorker()
      {
        WorkerSupportsCancellation = true
      })
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(LogCollectorUtils.GetCategoryJSON);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MainWindow.ShowCategories);
        backgroundWorker.RunWorkerAsync();
      }
    }

    internal static void GetCategoryJSON(object sender, DoWorkEventArgs doWorkEventArgs)
    {
      Logger.Info("Getting categories");
      string api = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}?oem={1}&version={2}&locale={3}", (object) "/app_settings/problem_categories", (object) Oem.Instance.OEM, (object) "4.250.0.1070", (object) LocaleStrings.GetLocaleName(MultiInstanceStrings.VmName, false));
      string jsonString = "";
      try
      {
        jsonString = HTTPUtils.SendRequestToCloud(api, (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, false);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed in fetching problem categories... Err : " + ex.ToString());
      }
      LogCollectorUtils.sStringConversions = new Hashtable();
      LogCollectorUtils.sCategoryShowDropdownMapping = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(jsonString))
      {
        LogCollectorUtils.SetDefaultCategoriesFromJson();
      }
      else
      {
        try
        {
          LogCollectorUtils.SetConversions(jsonString);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed in parsing json string received from cloud... Err : " + ex.ToString());
          Logger.Error("setting default categories/subcategories");
          LogCollectorUtils.SetDefaultCategoriesFromJson();
        }
      }
      if (LogCollectorUtils.sProblemCategories == null)
        return;
      foreach (string sProblemCategory in LogCollectorUtils.sProblemCategories)
        Logger.Info(sProblemCategory);
      MainWindow.Instance.Dispatcher.Invoke((Delegate) (() => MainWindow.Instance.AddCategories()));
    }

    internal static void SetDefaultCategoriesFromJson()
    {
      Logger.Info("Setting default categories");
      ProblemCategory forProblemCategory = LogCollectorUtils.GetLocalizationForProblemCategory(LogCollectorUtils.sVmName);
      if (forProblemCategory == null || forProblemCategory.Category == null || forProblemCategory.Category.Count <= 0)
        return;
      List<Category> categoryList = new List<Category>((IEnumerable<Category>) forProblemCategory.Category);
      LogCollectorUtils.sProblemCategories = new string[categoryList.Count];
      LogCollectorUtils.sCategorySubcategoryMapping = new Dictionary<string, Dictionary<string, string>>();
      LogCollectorUtils.sCategorySubcategoryMappingWithDropdown = new Dictionary<string, Dictionary<string, string>>();
      for (int index1 = 0; index1 < categoryList.Count; ++index1)
      {
        Category category = categoryList[index1];
        LogCollectorUtils.sProblemCategories[index1] = category.categoryValue;
        LogCollectorUtils.sStringConversions.Add((object) category.categoryValue, (object) category.categoryId);
        LogCollectorUtils.sCategoryShowDropdownMapping.Add(category.categoryId, category.showdropdown);
        if (category.Subcategory != null && category.Subcategory.Count > 0)
        {
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
          List<Subcategory> subcategoryList = new List<Subcategory>((IEnumerable<Subcategory>) category.Subcategory);
          for (int index2 = 0; index2 < subcategoryList.Count; ++index2)
          {
            dictionary1.Add(subcategoryList[index2].subcategoryId, subcategoryList[index2].subcategoryValue);
            dictionary2.Add(subcategoryList[index2].subcategoryId, subcategoryList[index2].showdropdown);
          }
          LogCollectorUtils.sCategorySubcategoryMapping.Add(category.categoryId, dictionary1);
          LogCollectorUtils.sCategorySubcategoryMappingWithDropdown.Add(category.categoryId, dictionary2);
        }
      }
    }

    private static void SetConversions(string jsonString)
    {
      Logger.Info("Downloaded Problem Category Json");
      JObject jobject1 = JObject.Parse(jsonString);
      foreach (KeyValuePair<string, JArray> serializable in (Dictionary<string, JArray>) jobject1["content"].ToSerializableDictionary<JArray>())
      {
        int num = 0;
        string key1 = serializable.Key;
        int count = serializable.Value.Count;
        if (count == 0)
        {
          LogCollectorUtils.sProblemCategories = (string[]) null;
          return;
        }
        LogCollectorUtils.sProblemCategories = new string[count];
        LogCollectorUtils.sCategorySubcategoryMapping = new Dictionary<string, Dictionary<string, string>>();
        LogCollectorUtils.sCategorySubcategoryMappingWithDropdown = new Dictionary<string, Dictionary<string, string>>();
        foreach (JObject jobject2 in serializable.Value)
        {
          string key2 = jobject2["id"].ToString();
          string str1 = jobject2["value"].ToString();
          if (jobject2.ContainsKey("showdropdown"))
          {
            string str2 = jobject2["showdropdown"].ToString();
            LogCollectorUtils.sCategoryShowDropdownMapping.Add(key2, str2);
          }
          LogCollectorUtils.sStringConversions.Add((object) str1, (object) key2);
          LogCollectorUtils.sProblemCategories[num++] = str1;
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
          if (jobject2.ContainsKey("subcategory"))
          {
            foreach (JObject jobject3 in JArray.Parse(jobject2["subcategory"].ToString()))
            {
              string key3 = jobject3["id"].ToString();
              string str2 = jobject3["value"].ToString();
              if (jobject3.ContainsKey("showdropdown"))
              {
                string str3 = jobject3["showdropdown"].ToString();
                dictionary2.Add(key3, str3);
              }
              dictionary1.Add(key3, str2);
            }
          }
          LogCollectorUtils.sCategorySubcategoryMapping.Add(key2, dictionary1);
          LogCollectorUtils.sCategorySubcategoryMappingWithDropdown.Add(key2, dictionary2);
        }
      }
      if (LogCollectorUtils.sStringConversions.Count != 0)
        return;
      LogCollectorUtils.sProblemCategories = (string[]) null;
    }

    public static ProblemCategory GetLocalizationForProblemCategory(string vmName)
    {
      ProblemCategory problemCategory = new ProblemCategory();
      string str = Thread.CurrentThread.CurrentCulture.Name;
      string locale = RegistryManager.Instance.Guest[vmName].Locale;
      if (!string.IsNullOrEmpty(locale))
        str = locale;
      if (string.Compare(str, "en-US", StringComparison.Ordinal) != 0 && LogCollectorUtils.PopulateLocaleProblemCategories(str, problemCategory))
        Logger.Info("Successfully populated localized strings for Problem Categories for locale: " + str);
      else if (LogCollectorUtils.PopulateLocaleProblemCategories("en-US", problemCategory))
        Logger.Info("Successfully populated English strings for Problem Categories");
      return problemCategory;
    }

    private static bool PopulateLocaleProblemCategories(
      string locale,
      ProblemCategory problemCategory)
    {
      try
      {
        string path = Path.Combine(Path.Combine(RegistryStrings.UserDefinedDir, "Locales\\ProblemCategories"), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "ReportProblemCategories.{0}.Json", (object) locale));
        if (!File.Exists(path))
        {
          Logger.Info(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "File does not exist for Problem Categories: {0}", (object) path));
          return false;
        }
        string json = File.ReadAllText(path);
        if (string.IsNullOrEmpty(json))
        {
          Logger.Info("Invalid json");
          return false;
        }
        Logger.Info("Found Json: " + json);
        foreach (KeyValuePair<string, JArray> serializable in (Dictionary<string, JArray>) JObject.Parse(json).ToSerializableDictionary<JArray>())
        {
          string key = serializable.Key;
          if (serializable.Value.Count == 0)
          {
            Logger.Info("No Categories found in Json");
            return false;
          }
          problemCategory.Category.Clear();
          foreach (JObject jobject1 in serializable.Value)
          {
            Category category = new Category()
            {
              categoryId = jobject1["id"].ToString(),
              categoryValue = jobject1["value"].ToString(),
              showdropdown = jobject1["showdropdown"].ToString()
            };
            if (jobject1.ContainsKey("subcategory"))
            {
              foreach (JObject jobject2 in JArray.Parse(jobject1["subcategory"].ToString()))
              {
                Subcategory subcategory = new Subcategory()
                {
                  subcategoryId = jobject2["id"].ToString(),
                  subcategoryValue = jobject2["value"].ToString(),
                  showdropdown = jobject2["showdropdown"].ToString()
                };
                category.Subcategory.Add(subcategory);
              }
            }
            else
              Logger.Info("No Subcategories found in Category: " + category.categoryId);
            problemCategory.Category.Add(category);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Could not populate localizes strings for Problem Categories. Error: " + ex.ToString());
        return false;
      }
    }
  }
}
