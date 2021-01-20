// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FormFile
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
