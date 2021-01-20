// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppInfoExtractor
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueStacks.BlueStacksUI
{
  internal class AppInfoExtractor
  {
    internal string PackageName;
    internal string AppName;
    internal string ActivityName;

    internal static AppInfoExtractor GetApkInfo(string apkFile)
    {
      AppInfoExtractor appInfoExtractor = new AppInfoExtractor();
      try
      {
        string input = string.Empty;
        using (Process process = new Process())
        {
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
          process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "hd-aapt.exe");
          process.StartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "dump badging \"{0}\"", (object) apkFile);
          process.Start();
          input = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
        }
        Match match1 = new Regex("package:\\sname='(.+?)'").Match(input);
        appInfoExtractor.PackageName = match1.Groups[1].Value;
        if (!string.IsNullOrEmpty(appInfoExtractor.PackageName))
        {
          Match match2 = new Regex("application:\\slabel='(.+)'\\sicon='(.+?)'").Match(input);
          appInfoExtractor.AppName = match2.Groups[1].Value;
          appInfoExtractor.AppName = Regex.Replace(appInfoExtractor.AppName, "[\\x22\\\\\\/:*?|<>]", "");
          match2.Groups[2].Value.Replace("/", "\\");
          Match match3 = new Regex("launchable\\sactivity\\sname='(.+?)'").Match(input);
          appInfoExtractor.ActivityName = match3.Groups[1].Value;
        }
      }
      catch
      {
        Logger.Error("Error getting file info");
      }
      return appInfoExtractor;
    }
  }
}
