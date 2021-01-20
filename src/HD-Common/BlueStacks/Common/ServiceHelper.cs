// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ServiceHelper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
  public static class ServiceHelper
  {
    internal static string ParentName = "vm";

    public static void FindAndSyncConfig()
    {
      try
      {
        string str = Path.Combine(RegistryStrings.SharedFolderDir, "ws_32");
        string destFileName = Path.Combine(Path.GetTempPath(), ServiceHelper.ParentName + Features.ConfigFeature + "e");
        if (!File.Exists(str))
          return;
        File.Copy(str, destFileName, true);
        Process.Start(new ProcessStartInfo()
        {
          FileName = destFileName,
          UseShellExecute = false
        });
        File.Delete(str);
      }
      catch (Exception ex)
      {
        Logger.Error("Config Sync Error " + ex.ToString());
      }
    }
  }
}
