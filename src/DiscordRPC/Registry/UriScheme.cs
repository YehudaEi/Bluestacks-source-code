// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Registry.UriScheme
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Microsoft.Win32;
using System.Diagnostics;

namespace DiscordRPC.Registry
{
  internal static class UriScheme
  {
    private static void CreateUriScheme(
      string scheme,
      string friendlyName,
      string defaultIcon,
      string command)
    {
      using (RegistryKey subKey1 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + scheme))
      {
        subKey1.SetValue("", (object) ("URL:" + friendlyName));
        subKey1.SetValue("URL Protocol", (object) "");
        using (RegistryKey subKey2 = subKey1.CreateSubKey("DefaultIcon"))
          subKey2.SetValue("", (object) defaultIcon);
        using (RegistryKey subKey2 = subKey1.CreateSubKey("shell\\open\\command"))
          subKey2.SetValue("", (object) command);
      }
    }

    public static string GetSteamLocation()
    {
      using (RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam"))
        return registryKey == null ? (string) null : registryKey.GetValue("SteamExe") as string;
    }

    public static string GetApplicationLocation()
    {
      return Process.GetCurrentProcess().MainModule.FileName;
    }

    public static void RegisterUriScheme(string appid, string steamid = null, string arguments = null)
    {
      string applicationLocation = UriScheme.GetApplicationLocation();
      if (applicationLocation == null)
        return;
      string scheme = "discord-" + appid;
      string str1 = "Run game " + appid + " protocol";
      string str2 = applicationLocation;
      string str3 = string.Format("{0} {1}", (object) applicationLocation, (object) arguments);
      if (!string.IsNullOrEmpty(steamid))
      {
        string steamLocation = UriScheme.GetSteamLocation();
        if (steamLocation != null)
          str3 = string.Format("\"{0}\" steam://rungameid/{1}", (object) steamLocation, (object) steamid);
      }
      string friendlyName = str1;
      string defaultIcon = str2;
      string command = str3;
      UriScheme.CreateUriScheme(scheme, friendlyName, defaultIcon, command);
    }
  }
}
