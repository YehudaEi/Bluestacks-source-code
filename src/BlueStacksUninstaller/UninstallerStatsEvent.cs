// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallerStatsEvent
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

namespace BlueStacks.Uninstaller
{
  public class UninstallerStatsEvent
  {
    public static string UninstallStarted
    {
      get
      {
        return "uninstall_launched";
      }
    }

    public static string UninstallCompleted
    {
      get
      {
        return "uninstall_completed";
      }
    }

    public static string UninstallFailed
    {
      get
      {
        return "uninstall_failed";
      }
    }
  }
}
