// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.PayloadInfo
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Net;

namespace BlueStacks.Common
{
  public class PayloadInfo
  {
    public HttpStatusCode StatusCode { get; private set; }

    public bool SupportsRangeRequest { get; set; }

    public long Size { get; set; }

    public long FileSize { get; set; }

    public PayloadInfo(
      HttpStatusCode statusCode,
      long size,
      long fileSize = 0,
      bool supportsRangeRequest = false)
    {
      this.SupportsRangeRequest = supportsRangeRequest;
      this.Size = size;
      this.StatusCode = statusCode;
      this.FileSize = fileSize == 0L ? size : fileSize;
    }
  }
}
