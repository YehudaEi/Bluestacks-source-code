// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppPlayerClosingEventArgs
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
  public class AppPlayerClosingEventArgs : BrowserEventArgs
  {
    public AppPlayerClosingEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
      : base(tag, vmName, extraData)
    {
    }
  }
}
