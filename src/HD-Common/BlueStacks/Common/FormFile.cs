// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FormFile
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.IO;

namespace BlueStacks.Common
{
  public class FormFile
  {
    public string Name { get; set; }

    public string ContentType { get; set; }

    public string FilePath { get; set; }

    public Stream Stream { get; set; }
  }
}
