// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppUsageTimer
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlueStacks.BlueStacksUI
{
  public static class AppUsageTimer
  {
    internal static Dictionary<string, Dictionary<string, long>> sDictAppUsageInfo = new Dictionary<string, Dictionary<string, long>>();
    private static Dictionary<string, long> sResetQuestDict = new Dictionary<string, long>();
    private static Stopwatch sStopwatch = new Stopwatch();
    private static string sLastAppPackage = (string) null;
    private static string sLastVMName = (string) null;
    private static SessionSwitchEventHandler sessionSwitchHandler = new SessionSwitchEventHandler(AppUsageTimer.SystemEvents_SessionSwitch);
    private static readonly byte[] bytes = new byte[13]
    {
      (byte) 73,
      (byte) 118,
      (byte) 97,
      (byte) 110,
      (byte) 32,
      (byte) 77,
      (byte) 101,
      (byte) 100,
      (byte) 118,
      (byte) 101,
      (byte) 100,
      (byte) 101,
      (byte) 118
    };

    internal static void StartTimer(string vmName, string packageName)
    {
      AppUsageTimer.StopTimer();
      AppUsageTimer.sLastAppPackage = packageName;
      AppUsageTimer.sLastVMName = vmName;
      AppUsageTimer.sStopwatch.Reset();
      AppUsageTimer.sStopwatch.Start();
    }

    internal static void StopTimer()
    {
      if (!AppUsageTimer.sStopwatch.IsRunning || string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage))
        return;
      AppUsageTimer.sStopwatch.Stop();
      long totalSeconds = (long) AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
      if (AppUsageTimer.sDictAppUsageInfo.ContainsKey(AppUsageTimer.sLastVMName))
      {
        if (AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].ContainsKey(AppUsageTimer.sLastAppPackage))
          AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName][AppUsageTimer.sLastAppPackage] += totalSeconds;
        else
          AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, totalSeconds);
        AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName]["TotalUsage"] += totalSeconds;
      }
      else
      {
        AppUsageTimer.sDictAppUsageInfo.Add(AppUsageTimer.sLastVMName, new Dictionary<string, long>()
        {
          {
            "TotalUsage",
            totalSeconds
          }
        });
        AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, totalSeconds);
      }
      AppUsageTimer.sLastAppPackage = string.Empty;
    }

    internal static Dictionary<string, Dictionary<string, long>> GetRealtimeDictionary()
    {
      if (!AppUsageTimer.sStopwatch.IsRunning || string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage))
        return AppUsageTimer.sDictAppUsageInfo;
      Dictionary<string, Dictionary<string, long>> dictionary = new Dictionary<string, Dictionary<string, long>>();
      foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
        dictionary.Add(keyValuePair.Key, keyValuePair.Value.ToDictionary<KeyValuePair<string, long>, string, long>((Func<KeyValuePair<string, long>, string>) (_ => _.Key), (Func<KeyValuePair<string, long>, long>) (_ => _.Value)));
      long totalSeconds = (long) AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
      if (dictionary.ContainsKey(AppUsageTimer.sLastVMName))
      {
        if (dictionary[AppUsageTimer.sLastVMName].ContainsKey(AppUsageTimer.sLastAppPackage))
          dictionary[AppUsageTimer.sLastVMName][AppUsageTimer.sLastAppPackage] += totalSeconds;
        else
          dictionary[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, totalSeconds);
        dictionary[AppUsageTimer.sLastVMName]["TotalUsage"] += totalSeconds;
      }
      else
      {
        dictionary.Add(AppUsageTimer.sLastVMName, new Dictionary<string, long>()
        {
          {
            "TotalUsage",
            totalSeconds
          }
        });
        dictionary[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, totalSeconds);
      }
      return dictionary;
    }

    internal static long GetTotalTimeForPackageAcrossInstances(string packageName)
    {
      long num = 0;
      try
      {
        foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
        {
          IEnumerable<KeyValuePair<string, long>> source = keyValuePair.Value.Where<KeyValuePair<string, long>>((Func<KeyValuePair<string, long>, bool>) (_ => string.Equals(_.Key, packageName, StringComparison.OrdinalIgnoreCase)));
          if (source.Any<KeyValuePair<string, long>>())
            num += source.First<KeyValuePair<string, long>>().Value;
        }
        if (!string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage) && string.Compare(AppUsageTimer.sLastAppPackage, packageName, StringComparison.OrdinalIgnoreCase) == 0)
          num += (long) AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
        Logger.Debug("Total time for package " + packageName + " " + num.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetTotalTimeForPackageAcrossInstances. Err : " + ex.ToString());
      }
      return num;
    }

    internal static long GetTotalTimeForAllPackages()
    {
      long num1 = 0;
      try
      {
        foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
        {
          long num2 = 0;
          IEnumerable<KeyValuePair<string, long>> source1 = keyValuePair.Value.Where<KeyValuePair<string, long>>((Func<KeyValuePair<string, long>, bool>) (_ => string.Compare(_.Key, "Home", StringComparison.OrdinalIgnoreCase) == 0));
          if (source1.Any<KeyValuePair<string, long>>())
            num2 += source1.First<KeyValuePair<string, long>>().Value;
          IEnumerable<KeyValuePair<string, long>> source2 = keyValuePair.Value.Where<KeyValuePair<string, long>>((Func<KeyValuePair<string, long>, bool>) (_ => string.Compare(_.Key, "TotalUsage", StringComparison.OrdinalIgnoreCase) == 0));
          if (source2.Any<KeyValuePair<string, long>>())
          {
            num1 += source2.First<KeyValuePair<string, long>>().Value;
            num1 -= num2;
          }
        }
        if (!string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage) && !string.Equals(AppUsageTimer.sLastAppPackage, "Home", StringComparison.InvariantCulture))
          num1 += (long) AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
        Logger.Debug("Total time for all packages " + num1.ToString());
        return num1 < 0L ? 0L : num1;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetTotalTimeForAllPackages " + ex.ToString());
      }
      return 0;
    }

    internal static long GetTotalTimeForPackageAfterReset(string packageName)
    {
      try
      {
        long packageAcrossInstances = AppUsageTimer.GetTotalTimeForPackageAcrossInstances(packageName);
        return packageAcrossInstances < 0L ? 0L : packageAcrossInstances;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetTotalTimeForPackageAfterReset. Err : " + ex.ToString());
      }
      return 0;
    }

    internal static void AddPackageForReset(string package, long time)
    {
      AppUsageTimer.sResetQuestDict[package] = time;
    }

    internal static void SessionEventHandler()
    {
      SystemEvents.SessionSwitch += AppUsageTimer.sessionSwitchHandler;
    }

    private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
      if (e.Reason == SessionSwitchReason.SessionLock)
      {
        AppUsageTimer.StopTimer();
      }
      else
      {
        if (e.Reason != SessionSwitchReason.SessionUnlock)
          return;
        AppUsageTimer.StartTimerAfterResume();
      }
    }

    internal static void DetachSessionEventHandler()
    {
      SystemEvents.SessionSwitch -= AppUsageTimer.sessionSwitchHandler;
    }

    private static void StartTimerAfterResume()
    {
      try
      {
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(AppUsageTimer.sLastVMName))
          return;
        MainWindow dictWindow = BlueStacksUIUtils.DictWindows[AppUsageTimer.sLastVMName];
        if (dictWindow == null || dictWindow.mTopBar.mAppTabButtons.SelectedTab == null)
          return;
        AppUsageTimer.StartTimer(AppUsageTimer.sLastVMName, dictWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in starting timer after sleep. Err : " + ex.ToString());
      }
    }

    internal static void SaveData()
    {
      AppUsageTimer.StopTimer();
      RegistryManager.Instance.AInfo = AppUsageTimer.EncryptString(JsonConvert.SerializeObject((object) AppUsageTimer.sDictAppUsageInfo));
    }

    internal static string EncryptString(string encryptString)
    {
      string userGuid = RegistryManager.Instance.UserGuid;
      byte[] bytes1 = Encoding.Unicode.GetBytes(encryptString);
      byte[] bytes2 = AppUsageTimer.bytes;
      Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, bytes2);
      using (Aes aes = Aes.Create())
      {
        aes.Key = rfc2898DeriveBytes.GetBytes(32);
        aes.IV = rfc2898DeriveBytes.GetBytes(16);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
          {
            cryptoStream.Write(bytes1, 0, bytes1.Length);
            cryptoStream.Close();
            encryptString = Convert.ToBase64String(memoryStream.ToArray());
            return encryptString;
          }
        }
      }
    }

    public static string DecryptString(string decryptString)
    {
      string userGuid = RegistryManager.Instance.UserGuid;
      decryptString = decryptString?.Replace(" ", "+");
      byte[] buffer = Convert.FromBase64String(decryptString);
      byte[] bytes = AppUsageTimer.bytes;
      Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, bytes);
      using (Aes aes = Aes.Create())
      {
        aes.Key = rfc2898DeriveBytes.GetBytes(32);
        aes.IV = rfc2898DeriveBytes.GetBytes(16);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
          {
            cryptoStream.Write(buffer, 0, buffer.Length);
            cryptoStream.Close();
            decryptString = Encoding.Unicode.GetString(memoryStream.ToArray());
            return decryptString;
          }
        }
      }
    }
  }
}
