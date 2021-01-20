// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NotificationManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
  public sealed class NotificationManager
  {
    private static object syncRoot = new object();
    internal string mNotificationFilePath = string.Empty;
    private string mShowNotificationText = LocaleStrings.GetLocalizedString("STRING_SHOW_NOTIFICATIONS", "");
    private static volatile NotificationManager mInstance;

    public SerializableDictionary<string, NotificationItem> DictNotificationItems { get; set; } = new SerializableDictionary<string, NotificationItem>();

    public SerializableDictionary<string, CloudNotificationItem> DictNotifications { get; set; } = new SerializableDictionary<string, CloudNotificationItem>();

    public AppPackageListObject ChatApplications { get; set; } = new AppPackageListObject();

    public string ShowNotificationText
    {
      get
      {
        return this.mShowNotificationText;
      }
      private set
      {
        this.mShowNotificationText = value;
      }
    }

    public static NotificationManager Instance
    {
      get
      {
        if (NotificationManager.mInstance == null)
        {
          lock (NotificationManager.syncRoot)
          {
            if (NotificationManager.mInstance == null)
              NotificationManager.mInstance = new NotificationManager();
          }
        }
        return NotificationManager.mInstance;
      }
    }

    private NotificationManager()
    {
      this.ReloadNotificationDetails();
      this.mNotificationFilePath = Path.Combine(RegistryStrings.BstUserDataDir, "Notifications.txt");
    }

    public void ReloadNotificationDetails()
    {
      if (string.IsNullOrEmpty(RegistryManager.Instance.NotificationData))
      {
        this.DictNotificationItems = new SerializableDictionary<string, NotificationItem>();
      }
      else
      {
        try
        {
          using (XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(RegistryManager.Instance.NotificationData)))
            this.DictNotificationItems = (SerializableDictionary<string, NotificationItem>) new XmlSerializer(typeof (SerializableDictionary<string, NotificationItem>)).Deserialize(xmlReader);
        }
        catch (Exception ex)
        {
          if (ex != null && ex is XmlException)
          {
            RegistryManager.Instance.NotificationData = string.Empty;
          }
          else
          {
            Exception innerException = ex.InnerException;
            if (innerException == null || !(innerException is XmlException))
              return;
            RegistryManager.Instance.NotificationData = string.Empty;
          }
        }
      }
    }

    public void UpdateNotificationsSettings()
    {
      try
      {
        using (StringWriter stringWriter = new StringWriter())
        {
          new XmlSerializer(typeof (SerializableDictionary<string, NotificationItem>)).Serialize((TextWriter) stringWriter, (object) this.DictNotificationItems);
          RegistryManager.Instance.NotificationData = stringWriter.ToString();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to update notification... Err : " + ex.ToString());
      }
    }

    public MuteState IsShowNotificationForKey(string title, string vmName)
    {
      this.ReloadNotificationDetails();
      return this.IsNotificationMutedForKey(title, vmName);
    }

    public MuteState IsNotificationMutedForKey(string title, string vmName = "Android")
    {
      MuteState state = MuteState.AutoHide;
      if (this.DictNotificationItems.ContainsKey(title))
      {
        if (this.DictNotificationItems[title].MuteState != MuteState.AutoHide)
        {
          if (this.DictNotificationItems[title].MuteState == MuteState.MutedForever)
            state = MuteState.MutedForever;
          else if (this.DictNotificationItems[title].MuteState == MuteState.NotMuted)
            state = MuteState.NotMuted;
          else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Hour)
          {
            if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Hours < 1)
            {
              state = MuteState.MutedForever;
            }
            else
            {
              this.DictNotificationItems.Remove(title);
              this.UpdateNotificationsSettings();
              state = MuteState.AutoHide;
            }
          }
          else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Day)
          {
            if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Days < 1)
            {
              state = MuteState.MutedForever;
            }
            else
            {
              this.DictNotificationItems.Remove(title);
              this.UpdateNotificationsSettings();
              state = MuteState.AutoHide;
            }
          }
          else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Week)
          {
            if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Days < 7)
            {
              state = MuteState.MutedForever;
            }
            else
            {
              this.DictNotificationItems.Remove(title);
              this.UpdateNotificationsSettings();
              state = MuteState.AutoHide;
            }
          }
        }
      }
      else
      {
        if (this.DictNotificationItems.ContainsKey(this.ShowNotificationText))
        {
          state = this.GetDefaultState(vmName);
        }
        else
        {
          state = MuteState.NotMuted;
          this.DictNotificationItems.Add(this.ShowNotificationText, new NotificationItem(this.ShowNotificationText, state, DateTime.Now, false));
        }
        string packageName;
        bool appInfoFromAppName = new JsonParser(vmName).GetAppInfoFromAppName(title, out packageName, out string _, out string _);
        if (!this.DictNotificationItems.ContainsKey(title))
          this.DictNotificationItems.Add(title, new NotificationItem(title, state, DateTime.Now, appInfoFromAppName && NotificationManager.Instance.ChatApplications.IsPackageAvailable(packageName)));
      }
      if (string.Equals(title, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
      {
        state = MuteState.NotMuted;
        this.UpdateMuteState(state, title, vmName);
      }
      else
        this.UpdateNotificationsSettings();
      return state;
    }

    public void SetDefaultState(string packageName, string vmName)
    {
      MuteState state;
      if (this.DictNotificationItems.ContainsKey(this.ShowNotificationText))
      {
        state = this.GetDefaultState(vmName);
      }
      else
      {
        state = MuteState.NotMuted;
        this.DictNotificationItems.Add(this.ShowNotificationText, new NotificationItem(this.ShowNotificationText, state, DateTime.Now, false));
      }
      string appNameFromPackage = new JsonParser(vmName).GetAppNameFromPackage(packageName);
      if (!this.DictNotificationItems.ContainsKey(appNameFromPackage))
      {
        if (NotificationManager.Instance.ChatApplications == null)
        {
          Logger.Warning("Chat applications instance null");
          this.DictNotificationItems.Add(appNameFromPackage, new NotificationItem(appNameFromPackage, state, DateTime.Now, false));
        }
        else
          this.DictNotificationItems.Add(appNameFromPackage, new NotificationItem(appNameFromPackage, state, DateTime.Now, NotificationManager.Instance.ChatApplications.IsPackageAvailable(packageName)));
      }
      this.UpdateNotificationsSettings();
    }

    public void UpdateMuteState(MuteState state, string key, string vmName)
    {
      if (this.DictNotificationItems.ContainsKey(key))
      {
        this.DictNotificationItems[key].MuteState = state;
        this.DictNotificationItems[key].MuteTime = DateTime.Now;
      }
      else
      {
        string packageName;
        bool appInfoFromAppName = new JsonParser(vmName).GetAppInfoFromAppName(key, out packageName, out string _, out string _);
        this.DictNotificationItems.Add(key, new NotificationItem(key, state, DateTime.Now, appInfoFromAppName && NotificationManager.Instance.ChatApplications.IsPackageAvailable(packageName)));
      }
      if (string.Equals(key, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
      {
        this.DictNotificationItems[key].MuteState = MuteState.NotMuted;
        this.DictNotificationItems[key].ShowDesktopNotifications = false;
      }
      this.UpdateNotificationsSettings();
    }

    internal void DeleteMuteState(string key)
    {
      if (this.DictNotificationItems.ContainsKey(key))
        this.DictNotificationItems.Remove(key);
      this.UpdateNotificationsSettings();
    }

    public void AddNewNotification(
      string imagePath,
      int id,
      string title,
      string msg,
      string url)
    {
      int num = 3;
      while (num > 0)
      {
        --num;
        try
        {
          CloudNotificationItem notificationItem = new CloudNotificationItem(title, msg, imagePath, url);
          SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
          savedNotifications[id.ToString((IFormatProvider) CultureInfo.InvariantCulture)] = notificationItem;
          this.SaveNotifications(savedNotifications);
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to add notification titled : {0} and msg : {1}... Err : {2}", (object) title, (object) msg, (object) ex.ToString());
        }
      }
    }

    private void SaveNotifications(
      SerializableDictionary<string, CloudNotificationItem> lstItem)
    {
      using (XmlTextWriter xmlTextWriter = new XmlTextWriter(this.mNotificationFilePath, Encoding.UTF8))
      {
        xmlTextWriter.Formatting = Formatting.Indented;
        new XmlSerializer(typeof (SerializableDictionary<string, CloudNotificationItem>)).Serialize((XmlWriter) xmlTextWriter, (object) lstItem);
        xmlTextWriter.Flush();
      }
    }

    private SerializableDictionary<string, CloudNotificationItem> GetSavedNotifications()
    {
      SerializableDictionary<string, CloudNotificationItem> serializableDictionary = new SerializableDictionary<string, CloudNotificationItem>();
      if (File.Exists(this.mNotificationFilePath))
      {
        using (XmlReader xmlReader = XmlReader.Create((Stream) File.OpenRead(this.mNotificationFilePath)))
          serializableDictionary = (SerializableDictionary<string, CloudNotificationItem>) new XmlSerializer(typeof (SerializableDictionary<string, CloudNotificationItem>)).Deserialize(xmlReader);
      }
      return serializableDictionary;
    }

    public void RemoveNotification(string key)
    {
      int num = 3;
      while (num > 0)
      {
        --num;
        try
        {
          SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
          if (!savedNotifications.ContainsKey(key))
            break;
          savedNotifications.Remove(key);
          this.SaveNotifications(savedNotifications);
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to remove notification... Err : " + ex.ToString());
        }
      }
    }

    public void MarkReadNotification(string key)
    {
      int num = 3;
      while (num > 0)
      {
        --num;
        try
        {
          SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
          if (key == null || !savedNotifications.ContainsKey(key))
            break;
          savedNotifications[key].IsRead = true;
          this.SaveNotifications(savedNotifications);
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to mark read notification... Err : " + ex.ToString());
        }
      }
    }

    public void UpdateDictionary()
    {
      int num = 3;
      while (num > 0)
      {
        --num;
        try
        {
          this.DictNotifications = NotificationManager.Instance.GetSavedNotifications();
          break;
        }
        catch (Exception ex)
        {
          Logger.Info("Failed to update notification dictionary... Err : " + ex.ToString());
        }
      }
    }

    public MuteState GetDefaultState(string vmName)
    {
      return this.IsNotificationMutedForKey(this.ShowNotificationText, vmName);
    }

    public void RemoveNotificationItem(string title, string package)
    {
      if (this.DictNotificationItems == null || !this.DictNotificationItems.ContainsKey(title))
        return;
      string[] vmList = RegistryManager.Instance.VmList;
      List<string> list = new List<string>();
      try
      {
        foreach (string vmName in vmList)
        {
          foreach (AppInfo app in new JsonParser(vmName).GetAppList())
            list.AddIfNotContain<string>(app.Package);
        }
        if (list.Contains(package))
          return;
        this.DictNotificationItems.Remove(title);
        this.UpdateNotificationsSettings();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting all installed apps from all Vms: {0}", (object) ex.ToString());
      }
    }

    public bool IsDesktopNotificationToBeShown(string key)
    {
      if (!this.DictNotificationItems.ContainsKey(key) || !this.DictNotificationItems[key].ShowDesktopNotifications)
        return false;
      switch (this.DictNotificationItems[key].MuteState)
      {
        case MuteState.MutedFor1Hour:
        case MuteState.MutedFor1Day:
        case MuteState.MutedFor1Week:
          return false;
        default:
          return true;
      }
    }
  }
}
