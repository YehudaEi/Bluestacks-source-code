// Decompiled with JetBrains decompiler
// Type: PayloadInfo
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

public class PayloadInfo
{
  public PayloadInfo(bool supportsRangeRequest, long size, bool invalidStatusCode = false)
  {
    this.SupportsRangeRequest = supportsRangeRequest;
    this.Size = size;
    this.InvalidHTTPStatusCode = invalidStatusCode;
  }

  public long Size { get; }

  public bool SupportsRangeRequest { get; }

  public bool InvalidHTTPStatusCode { get; }
}
