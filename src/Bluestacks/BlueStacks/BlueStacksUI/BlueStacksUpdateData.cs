// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BlueStacksUpdateData
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  internal class BlueStacksUpdateData
  {
    public string ClientVersion { get; set; } = "";

    public string EngineVersion { get; set; } = "";

    public bool IsFullInstaller { get; set; }

    public string Md5 { get; set; } = "";

    public string UpdateType { get; set; } = "";

    public string DownloadUrl { get; set; } = "";

    public List<string> UpdateDescrption { get; set; } = new List<string>();

    public bool IsUpdateAvailble { get; set; }

    public string UpdateDownloadLocation { get; set; } = "";

    public string DetailedChangeLogsUrl { get; set; } = "";

    public bool IsTryAgain { get; set; }
  }
}
