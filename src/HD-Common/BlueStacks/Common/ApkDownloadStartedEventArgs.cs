// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ApkDownloadStartedEventArgs
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
  public class ApkDownloadStartedEventArgs : BrowserEventArgs
  {
    public ApkDownloadStartedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
      : base(tag, vmName, extraData)
    {
    }
  }
}
