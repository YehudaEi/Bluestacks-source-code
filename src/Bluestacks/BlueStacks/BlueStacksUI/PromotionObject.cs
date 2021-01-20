// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PromotionObject
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.BlueStacksUI
{
  public class PromotionObject
  {
    private static bool mIsPromotionLoading = true;
    internal static volatile bool mIsBootPromotionLoading = true;
    internal static PromotionObject Instance = (PromotionObject) null;
    private const string sPromotionFilename = "bst_promotion";

    private static event EventHandler mBootPromotionHandler;

    internal static EventHandler BootPromotionHandler { get; set; }

    internal void SetDefaultMoreAppsOrder(bool overwrite = true)
    {
      if (!(this.MoreAppsDockOrder.Count == 0 | overwrite))
        return;
      SerializableDictionary<string, int> moreAppsDockOrder = this.MoreAppsDockOrder;
      SerializableDictionary<string, int> serializableDictionary = new SerializableDictionary<string, int>();
      serializableDictionary.Add("com.android.chrome", 2);
      serializableDictionary.Add("com.android.camera2", 2);
      serializableDictionary.Add("com.bluestacks.settings", 3);
      serializableDictionary.Add("com.bluestacks.filemanager", 4);
      serializableDictionary.Add("instance_manager", 5);
      serializableDictionary.Add("help_center", 6);
      moreAppsDockOrder.ClearAddRange<string, int>((Dictionary<string, int>) serializableDictionary);
    }

    internal void SetDefaultDockOrder(bool overwrite = true)
    {
      if (!(this.DockOrder.Count == 0 | overwrite))
        return;
      SerializableDictionary<string, int> dockOrder = this.DockOrder;
      SerializableDictionary<string, int> serializableDictionary = new SerializableDictionary<string, int>();
      serializableDictionary.Add("appcenter", 1);
      serializableDictionary.Add("pikaworld", 2);
      dockOrder.ClearAddRange<string, int>((Dictionary<string, int>) serializableDictionary);
    }

    internal void SetDefaultMyAppsOrder(bool overwrite = true)
    {
      if (!(this.MyAppsOrder.Count == 0 | overwrite))
        return;
      SerializableDictionary<string, int> myAppsOrder = this.MyAppsOrder;
      SerializableDictionary<string, int> serializableDictionary = new SerializableDictionary<string, int>();
      serializableDictionary.Add("com.android.vending", 1);
      myAppsOrder.ClearAddRange<string, int>((Dictionary<string, int>) serializableDictionary);
    }

    internal void SetDefaultOrder(bool overwrite = true)
    {
      this.SetDefaultMyAppsOrder(overwrite);
      this.SetDefaultDockOrder(overwrite);
      this.SetDefaultMoreAppsOrder(overwrite);
    }

    private static event EventHandler mBackgroundPromotionHandler;

    internal static EventHandler BackgroundPromotionHandler
    {
      get
      {
        return PromotionObject.mBackgroundPromotionHandler;
      }
      set
      {
        PromotionObject.mBackgroundPromotionHandler = value;
        if (PromotionObject.mIsPromotionLoading)
          return;
        PromotionObject.mBackgroundPromotionHandler((object) PromotionObject.Instance, new EventArgs());
      }
    }

    private static event EventHandler mPromotionHandler;

    internal static EventHandler PromotionHandler
    {
      get
      {
        return PromotionObject.mPromotionHandler;
      }
      set
      {
        PromotionObject.mPromotionHandler = value;
        if (PromotionObject.mIsPromotionLoading)
          return;
        PromotionObject.mPromotionHandler((object) PromotionObject.Instance, new EventArgs());
      }
    }

    private static event EventHandler mAppSpecificRulesHandler;

    internal static EventHandler AppSpecificRulesHandler
    {
      get
      {
        return PromotionObject.mAppSpecificRulesHandler;
      }
      set
      {
        PromotionObject.mAppSpecificRulesHandler = value;
        if (PromotionObject.mIsPromotionLoading)
          return;
        PromotionObject.mAppSpecificRulesHandler((object) PromotionObject.Instance, new EventArgs());
      }
    }

    private static event System.Action<bool> mAppSuggestionHandler;

    internal static System.Action<bool> AppSuggestionHandler { get; set; }

    private static event System.Action<bool> mAppRecommendationHandler;

    internal static System.Action<bool> AppRecommendationHandler { get; set; }

    private static event System.Action mQuestHandler;

    internal static System.Action QuestHandler { get; set; }

    private static string FilePath
    {
      get
      {
        return Path.Combine(RegistryStrings.PromotionDirectory, "bst_promotion");
      }
    }

    internal static void LoadDataFromFile()
    {
      try
      {
        if (!File.Exists(PromotionObject.FilePath))
          return;
        string filePath = PromotionObject.FilePath;
        using (XmlReader xmlReader = XmlReader.Create(filePath, new XmlReaderSettings()
        {
          ProhibitDtd = true
        }))
        {
          Logger.Info("vikramTest: Loading PromotionObject Settings from " + PromotionObject.FilePath);
          PromotionObject.Instance = (PromotionObject) new XmlSerializer(typeof (PromotionObject)).Deserialize(xmlReader);
          Logger.Info("vikramTest: Done loading promotionObject.");
          PromotionObject.Instance.QuestHdPlayerRules.ClearSync<string, long>();
          PromotionObject.Instance.QuestRules.ClearSync<QuestRule>();
          PromotionObject.Instance.ResetQuestRules.ClearSync<string, long[]>();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error Loading PromotionObject Settings " + ex.ToString());
      }
      finally
      {
        if (PromotionObject.Instance == null)
          PromotionObject.Instance = new PromotionObject();
        if (PromotionObject.Instance.DockOrder.Count == 0)
          PromotionObject.Instance.SetDefaultDockOrder(true);
        PromotionObject.CacheOldBootPromotions();
      }
    }

    private static void CacheOldBootPromotions()
    {
      PromotionObject.Instance.DictOldBootPromotions.ClearAddRange<string, BootPromotion>((Dictionary<string, BootPromotion>) PromotionObject.Instance.DictBootPromotions);
    }

    internal static void Save()
    {
      try
      {
        if (!Directory.Exists(Directory.GetParent(PromotionObject.FilePath).FullName))
          Directory.CreateDirectory(Directory.GetParent(PromotionObject.FilePath).FullName);
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(PromotionObject.FilePath, Encoding.UTF8)
        {
          Formatting = Formatting.Indented
        })
        {
          new XmlSerializer(typeof (PromotionObject)).Serialize((XmlWriter) xmlTextWriter, (object) PromotionObject.Instance);
          xmlTextWriter.Flush();
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    internal void PromotionLoaded()
    {
      PromotionObject.mIsPromotionLoading = false;
      EventHandler promotionHandler1 = PromotionObject.mBootPromotionHandler;
      if (promotionHandler1 != null)
        promotionHandler1((object) this, new EventArgs());
      EventHandler promotionHandler2 = PromotionObject.mBackgroundPromotionHandler;
      if (promotionHandler2 != null)
        promotionHandler2((object) this, new EventArgs());
      System.Action mQuestHandler = PromotionObject.mQuestHandler;
      if (mQuestHandler != null)
        mQuestHandler();
      System.Action<bool> suggestionHandler = PromotionObject.mAppSuggestionHandler;
      if (suggestionHandler != null)
        suggestionHandler(true);
      System.Action<bool> recommendationHandler = PromotionObject.mAppRecommendationHandler;
      if (recommendationHandler != null)
        recommendationHandler(true);
      EventHandler promotionHandler3 = PromotionObject.mPromotionHandler;
      if (promotionHandler3 != null)
        promotionHandler3((object) this, new EventArgs());
      EventHandler specificRulesHandler = PromotionObject.mAppSpecificRulesHandler;
      if (specificRulesHandler == null)
        return;
      specificRulesHandler((object) this, new EventArgs());
    }

    [XmlIgnore]
    public List<string> AppSpecificRulesList { get; }

    public List<string> CustomCursorExcludedAppsList { get; }

    [XmlIgnore]
    public bool IsRootAccessEnabled { get; set; }

    public string MyAppsPromotionID { get; set; }

    public string MyAppsCrossPromotionID { get; set; }

    public string BackgroundPromotionID { get; set; }

    public string BackgroundPromotionImagePath { get; set; }

    public SerializableDictionary<string, AppIconPromotionObject> DictAppsPromotions { get; set; }

    public string QuestName { get; set; }

    public string QuestActionType { get; set; }

    public List<QuestRule> QuestRules { get; }

    public SerializableDictionary<string, long[]> ResetQuestRules { get; set; }

    public SerializableDictionary<string, long> QuestHdPlayerRules { get; set; }

    public SerializableDictionary<string, int> MyAppsOrder { get; set; }

    public SerializableDictionary<string, int> DockOrder { get; set; }

    public SerializableDictionary<string, int> MoreAppsDockOrder { get; set; }

    internal SerializableDictionary<string, BootPromotion> DictOldBootPromotions { get; set; }

    public int BootPromoDisplaytime { get; set; }

    public SerializableDictionary<string, BootPromotion> DictBootPromotions { get; set; }

    public SerializableDictionary<string, SearchRecommendation> SearchRecommendations { get; set; }

    public AppRecommendationSection AppRecommendations { get; set; }

    public List<AppSuggestionPromotion> AppSuggestionList { get; }

    public List<string> BlackListedApplicationsList { get; }

    public SerializableDictionary<string, string> StartupTab { get; set; }

    public bool IsShowOtsFeedback { get; set; }

    public string DiscordClientID { get; set; }

    public bool IsSecurityMetricsEnable { get; set; }

    public PromotionObject()
    {
      SerializableDictionary<string, int> serializableDictionary1 = new SerializableDictionary<string, int>();
      serializableDictionary1.Add("appcenter", 1);
      serializableDictionary1.Add("com.android.vending", 2);
      serializableDictionary1.Add("pikaworld", 3);
      serializableDictionary1.Add("macro_recorder", 4);
      serializableDictionary1.Add("instance_manager", 5);
      serializableDictionary1.Add("help_center", 6);
      // ISSUE: reference to a compiler-generated field
      this.\u003CDockOrder\u003Ek__BackingField = serializableDictionary1;
      SerializableDictionary<string, int> serializableDictionary2 = new SerializableDictionary<string, int>();
      serializableDictionary2.Add("appcenter", 1);
      serializableDictionary2.Add("com.android.vending", 2);
      serializableDictionary2.Add("pikaworld", 3);
      serializableDictionary2.Add("macro_recorder", 4);
      serializableDictionary2.Add("instance_manager", 5);
      serializableDictionary2.Add("help_center", 6);
      // ISSUE: reference to a compiler-generated field
      this.\u003CMoreAppsDockOrder\u003Ek__BackingField = serializableDictionary2;
      // ISSUE: reference to a compiler-generated field
      this.\u003CDictOldBootPromotions\u003Ek__BackingField = new SerializableDictionary<string, BootPromotion>();
      // ISSUE: reference to a compiler-generated field
      this.\u003CBootPromoDisplaytime\u003Ek__BackingField = 4000;
      // ISSUE: reference to a compiler-generated field
      this.\u003CDictBootPromotions\u003Ek__BackingField = new SerializableDictionary<string, BootPromotion>();
      // ISSUE: reference to a compiler-generated field
      this.\u003CSearchRecommendations\u003Ek__BackingField = new SerializableDictionary<string, SearchRecommendation>();
      // ISSUE: reference to a compiler-generated field
      this.\u003CAppRecommendations\u003Ek__BackingField = new AppRecommendationSection();
      this.AppSuggestionList = new List<AppSuggestionPromotion>();
      this.BlackListedApplicationsList = new List<string>();
      // ISSUE: reference to a compiler-generated field
      this.\u003CStartupTab\u003Ek__BackingField = new SerializableDictionary<string, string>();
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }
  }
}
