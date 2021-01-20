// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.FileImporter
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class FileImporter
  {
    public static DragEventHandler MakeDragDropHandler()
    {
      return (DragEventHandler) ((obj, evt) => new Thread((ThreadStart) (() => FileImporter.HandleDragDropAsync(evt)))
      {
        IsBackground = true
      }.Start());
    }

    private static bool IsSharedFolderEnabled()
    {
      if (RegistryManager.Instance.DefaultGuest.FileSystem != 0)
        return true;
      Logger.Info("Shared folders disabled");
      return false;
    }

    private static void HandleDragDropAsync(DragEventArgs evt)
    {
      string vmName = MultiInstanceStrings.VmName;
      if (!FileImporter.IsSharedFolderEnabled())
        return;
      try
      {
        Array data1 = (Array) evt.Data.GetData(DataFormats.FileDrop);
        List<string> stringList = new List<string>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int index = 0; index < data1.Length; ++index)
        {
          string path = data1.GetValue(index).ToString();
          string fileName = Path.GetFileName(path);
          if (string.Equals(Path.GetExtension(path), ".apk", StringComparison.InvariantCultureIgnoreCase) || string.Equals(Path.GetExtension(path), ".xapk", StringComparison.InvariantCultureIgnoreCase))
            stringList.Add(path);
          else
            dictionary.Add(fileName, path);
        }
        string sharedFolderDir = RegistryStrings.SharedFolderDir;
        if (dictionary.Count > 0)
        {
          string randomBstSharedFolder = Utils.CreateRandomBstSharedFolder(sharedFolderDir);
          string path1 = Path.Combine(RegistryStrings.SharedFolderDir, randomBstSharedFolder);
          Logger.Info("Shared Folder path : " + path1);
          foreach (KeyValuePair<string, string> keyValuePair in dictionary)
          {
            Logger.Info("DragDrop File: {0}", (object) keyValuePair.Key);
            string str = Path.Combine(path1, keyValuePair.Key);
            try
            {
              FileSystem.CopyFile(keyValuePair.Value, str, UIOption.AllDialogs);
              File.SetAttributes(str, FileAttributes.Normal);
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in copying file : " + keyValuePair.Value + "... Err : " + ex.ToString());
            }
          }
          JArray jarray1 = new JArray();
          JObject jobject = new JObject();
          jobject.Add((object) new JProperty("foldername", (object) randomBstSharedFolder));
          jarray1.Add((JToken) jobject);
          JArray jarray2 = jarray1;
          Dictionary<string, string> data2 = new Dictionary<string, string>()
          {
            {
              "data",
              jarray2.ToString(Formatting.None)
            }
          };
          Logger.Info("Sending drag drop request: " + jarray2.ToString());
          try
          {
            HTTPUtils.SendRequestToGuest("fileDrop", data2, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in sending FileDrop request. Err: " + ex.Message);
          }
        }
        if (stringList.Count <= 0)
          return;
        foreach (string str in stringList)
        {
          string apkPath = str;
          if (Oem.Instance.IsOEMWithBGPClient)
          {
            if (!Oem.IsOEMDmm)
            {
              try
              {
                HTTPUtils.SendRequestToClient("dragDropInstall", new Dictionary<string, string>()
                {
                  {
                    "filePath",
                    apkPath
                  }
                }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                continue;
              }
              catch (Exception ex)
              {
                Logger.Error("Exception in send drag drop install... Err : " + ex.ToString());
                continue;
              }
            }
          }
          new Thread((ThreadStart) (() => Utils.CallApkInstaller(apkPath, false, vmName)))
          {
            IsBackground = true
          }.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleDragDropAsync function. Err : " + ex.Message);
      }
    }

    public static void HandleDragEnter(object obj, DragEventArgs evt)
    {
      if (evt.Data.GetDataPresent(DataFormats.FileDrop))
      {
        evt.Effect = DragDropEffects.Copy;
      }
      else
      {
        Logger.Debug("FileDrop DataFormat not supported");
        string[] formats = evt.Data.GetFormats();
        Logger.Debug("Supported formats:");
        foreach (string msg in formats)
          Logger.Debug(msg);
        evt.Effect = DragDropEffects.None;
      }
    }
  }
}
