// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.PayloadInfo
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
