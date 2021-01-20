// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RequestData
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Specialized;

namespace BlueStacks.Common
{
  public class RequestData
  {
    public NameValueCollection Headers { get; set; }

    public NameValueCollection QueryString { get; set; }

    public NameValueCollection Data { get; set; }

    public NameValueCollection Files { get; set; }

    public string RequestVmName { get; set; }

    public int RequestVmId { get; set; }

    public string Oem { get; set; }

    public RequestData()
    {
      this.Headers = new NameValueCollection();
      this.QueryString = new NameValueCollection();
      this.Data = new NameValueCollection();
      this.Files = new NameValueCollection();
    }
  }
}
