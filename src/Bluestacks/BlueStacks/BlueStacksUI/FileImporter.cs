// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FileImporter
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public static class FileImporter
  {
    internal static void Init(MainWindow window)
    {
      window.AllowDrop = true;
      window.DragEnter += new DragEventHandler(FileImporter.HandleDragEnter);
      window.Drop += new DragEventHandler(FileImporter.HandleDragDrop);
    }

    private static void HandleDragDrop(object sender, DragEventArgs e)
    {
      new Thread((ThreadStart) (() => FileImporter.HandleDragDropAsync(e, sender as MainWindow)))
      {
        IsBackground = true
      }.Start();
    }

    private static bool IsSharedFolderEnabled(int fileSystem)
    {
      if (fileSystem != 0)
        return true;
      Logger.Info("Shared folders disabled");
      return false;
    }

    private static void HandleDragDropAsync(DragEventArgs evt, MainWindow window)
    {
      string mVmName = window.mVmName;
      if (!FileImporter.IsSharedFolderEnabled(window.EngineInstanceRegistry.FileSystem))
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
              Logger.Error("Failed to copy file : " + keyValuePair.Value + "...Err : " + ex.ToString());
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
            HTTPUtils.SendRequestToGuest("fileDrop", data2, mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to send FileDrop request. err: " + ex.ToString());
          }
        }
        if (stringList.Count <= 0)
          return;
        foreach (string str in stringList)
        {
          try
          {
            HTTPUtils.SendRequestToClient("dragDropInstall", new Dictionary<string, string>()
            {
              {
                "filePath",
                str
              }
            }, mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Warning("Failed to send drag drop install. Err: " + ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in DragDrop function: " + ex.Message);
      }
    }

    public static void HandleDragEnter(object obj, DragEventArgs evt)
    {
      if (evt == null)
        return;
      if (evt.Data.GetDataPresent(DataFormats.FileDrop))
      {
        evt.Effects = DragDropEffects.Copy;
      }
      else
      {
        Logger.Debug("FileDrop DataFormat not supported");
        string[] formats = evt.Data.GetFormats();
        Logger.Debug("Supported formats:");
        foreach (string msg in formats)
          Logger.Debug(msg);
        evt.Effects = DragDropEffects.None;
      }
    }

    public static string GetMimeFromFile(string filename)
    {
      string str = "";
      if (!File.Exists(filename))
        return str;
      byte[] numArray = new byte[256];
      using (FileStream fileStream = new FileStream(filename, FileMode.Open))
      {
        if (fileStream.Length >= 256L)
          fileStream.Read(numArray, 0, 256);
        else
          fileStream.Read(numArray, 0, (int) fileStream.Length);
      }
      try
      {
        uint ppwzMimeOut;
        int mimeFromData = (int) NativeMethods.FindMimeFromData(0U, (string) null, numArray, 256U, (string) null, 0U, out ppwzMimeOut, 0U);
        IntPtr ptr = new IntPtr((long) ppwzMimeOut);
        str = Marshal.PtrToStringUni(ptr);
        Marshal.FreeCoTaskMem(ptr);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get mime type. err: " + ex.Message);
      }
      return str;
    }
  }
}
