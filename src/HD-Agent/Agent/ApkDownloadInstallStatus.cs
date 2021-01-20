// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.ApkDownloadInstallStatus
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

namespace BlueStacks.Agent
{
  public class ApkDownloadInstallStatus
  {
    public DownloadInstallStatus status = DownloadInstallStatus.None;
    public string vmName;
    public string packageName;
    public long downloadedSize;
    public long payloadSize;
    public bool isUpgrade;
    public uint downloadFailedCode;
    public uint installFailedCode;
  }
}
