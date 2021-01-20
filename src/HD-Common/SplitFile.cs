// Decompiled with JetBrains decompiler
// Type: SplitFile
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

internal class SplitFile
{
  public static void Split(string path, int size, SplitFile.ProgressCb progressCb)
  {
    byte[] buffer = new byte[16384];
    using (Stream stream1 = (Stream) File.OpenRead(path))
    {
      int num = 0;
      string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.manifest", (object) path);
      while (stream1.Position < stream1.Length)
      {
        string path1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_part_{1}", (object) path, (object) num);
        using (Stream stream2 = (Stream) File.Create(path1))
        {
          int val1 = size;
          int count;
          for (; val1 > 0; val1 -= count)
          {
            count = stream1.Read(buffer, 0, Math.Min(val1, 16384));
            if (count != 0)
              stream2.Write(buffer, 0, count);
            else
              break;
          }
        }
        string manifest = (string) null;
        using (Stream stream2 = (Stream) File.OpenRead(path1))
        {
          string str = SplitFile.CheckSum(stream2);
          long length = stream2.Length;
          manifest = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) Path.GetFileName(path1), (object) length, (object) str);
        }
        progressCb(manifest);
        ++num;
      }
    }
  }

  public static string CheckSum(Stream stream)
  {
    using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
    {
      byte[] hash = cryptoServiceProvider.ComputeHash(stream);
      StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);
      foreach (byte num in hash)
        stringBuilder.AppendFormat("{0:x2}", (object) num);
      return stringBuilder.ToString();
    }
  }

  public delegate void ProgressCb(string manifest);
}
