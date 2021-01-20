// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Writer
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BlueStacks.Common
{
  public class Writer : TextWriter
  {
    private Writer.WriteFunc writeFunc;

    public Writer(Writer.WriteFunc writeFunc)
    {
      this.writeFunc = writeFunc;
    }

    public override Encoding Encoding
    {
      get
      {
        return Encoding.UTF8;
      }
    }

    public override void WriteLine(string msg)
    {
      this.writeFunc(msg);
    }

    public override void WriteLine(string fmt, object obj)
    {
      this.writeFunc(string.Format((IFormatProvider) CultureInfo.InvariantCulture, fmt, obj));
    }

    public override void WriteLine(string fmt, params object[] objs)
    {
      this.writeFunc(string.Format((IFormatProvider) CultureInfo.InvariantCulture, fmt, objs));
    }

    public delegate void WriteFunc(string msg);
  }
}
