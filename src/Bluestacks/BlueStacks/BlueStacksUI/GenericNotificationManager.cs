// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GenericNotificationManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.BlueStacksUI
{
  internal sealed class GenericNotificationManager
  {
    private static object syncRoot = new object();
    private static object syncNotificationsReadWrite = new object();
    private static volatile GenericNotificationManager sInstance;

    private GenericNotificationManager()
    {
    }

    public static GenericNotificationManager Instance
    {
      get
      {
        if (GenericNotificationManager.sInstance == null)
        {
          lock (GenericNotificationManager.syncRoot)
          {
            if (GenericNotificationManager.sInstance == null)
              GenericNotificationManager.sInstance = new GenericNotificationManager();
          }
        }
        return GenericNotificationManager.sInstance;
      }
    }

    internal static string GenericNotificationFilePath
    {
      get
      {
        return Path.Combine(RegistryStrings.PromotionDirectory, "bst_genericNotification");
      }
    }

    public static void AddNewNotification(
      GenericNotificationItem notificationItem,
      bool dontOverwrite = false)
    {
      lock (GenericNotificationManager.syncNotificationsReadWrite)
      {
        try
        {
          SerializableDictionary<string, GenericNotificationItem> savedNotifications = GenericNotificationManager.GetSavedNotifications();
          if (!dontOverwrite)
          {
            savedNotifications[notificationItem.Id] = notificationItem;
            GenericNotificationManager.SaveNotifications(savedNotifications);
          }
          else
          {
            if (savedNotifications.ContainsKey(notificationItem.Id))
              return;
            savedNotifications[notificationItem.Id] = notificationItem;
            GenericNotificationManager.SaveNotifications(savedNotifications);
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to add notification id : {0} titled : {1} and msg : {2}... Err : {3}", (object) notificationItem.Id, (object) notificationItem.Title, (object) notificationItem.Message, (object) ex.ToString());
        }
      }
    }

    private static void SaveNotifications(
      SerializableDictionary<string, GenericNotificationItem> lstItem)
    {
      using (XmlTextWriter xmlTextWriter = new XmlTextWriter(GenericNotificationManager.GenericNotificationFilePath, Encoding.UTF8)
      {
        Formatting = Formatting.Indented
      })
      {
        SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
        foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in (Dictionary<string, GenericNotificationItem>) lstItem)
        {
          if (!keyValuePair.Value.IsDeleted)
            serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
        new XmlSerializer(typeof (SerializableDictionary<string, GenericNotificationItem>)).Serialize((XmlWriter) xmlTextWriter, (object) serializableDictionary);
        xmlTextWriter.Flush();
      }
    }

    private static SerializableDictionary<string, GenericNotificationItem> GetSavedNotifications()
    {
      SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
      if (File.Exists(GenericNotificationManager.GenericNotificationFilePath))
      {
        int num = 3;
        while (num > 0)
        {
          --num;
          try
          {
            string notificationFilePath = GenericNotificationManager.GenericNotificationFilePath;
            using (XmlReader xmlReader = XmlReader.Create(notificationFilePath, new XmlReaderSettings()
            {
              ProhibitDtd = true
            }))
            {
              serializableDictionary = (SerializableDictionary<string, GenericNotificationItem>) new XmlSerializer(typeof (SerializableDictionary<string, GenericNotificationItem>)).Deserialize(xmlReader);
              break;
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Exception when reading saved notifications." + ex.ToString());
          }
        }
      }
      return serializableDictionary;
    }

    internal GenericNotificationItem GetNotificationItem(string id)
    {
      return GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (_ => _.Id == id)).FirstOrDefault<KeyValuePair<string, GenericNotificationItem>>().Value;
    }

    public static SerializableDictionary<string, GenericNotificationItem> MarkNotification(
      IEnumerable<string> ids,
      System.Action<GenericNotificationItem> setter)
    {
      lock (GenericNotificationManager.syncNotificationsReadWrite)
      {
        SerializableDictionary<string, GenericNotificationItem> lstItem = new SerializableDictionary<string, GenericNotificationItem>();
        try
        {
          lstItem = GenericNotificationManager.GetSavedNotifications();
          foreach (string index in ids.Where<string>((Func<string, bool>) (id => id != null && lstItem.ContainsKey(id))))
            setter(lstItem[index]);
          GenericNotificationManager.SaveNotifications(lstItem);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to mark notification... Err : " + ex.ToString());
        }
        return lstItem;
      }
    }

    public static SerializableDictionary<string, GenericNotificationItem> GetNotificationItems(
      Predicate<GenericNotificationItem> getter)
    {
      lock (GenericNotificationManager.syncNotificationsReadWrite)
      {
        SerializableDictionary<string, GenericNotificationItem> savedNotifications = GenericNotificationManager.GetSavedNotifications();
        SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
        Func<KeyValuePair<string, GenericNotificationItem>, bool> predicate = (Func<KeyValuePair<string, GenericNotificationItem>, bool>) (item => getter(item.Value));
        foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in savedNotifications.Where<KeyValuePair<string, GenericNotificationItem>>(predicate))
          serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        return serializableDictionary;
      }
    }
  }
}
