// Decompiled with JetBrains decompiler
// Type: Manifest
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

public class Manifest
{
  private List<FilePart> m_FileParts;
  private string m_FilePath;

  public Manifest(string filePath)
  {
    this.m_FileParts = new List<FilePart>();
    this.m_FilePath = filePath;
  }

  public bool Check()
  {
    int num = 0;
    foreach (FilePart filePart in this.m_FileParts)
    {
      if (!filePart.Check())
      {
        Logger.Error("Check failed for part " + num.ToString());
        return false;
      }
      ++num;
    }
    return true;
  }

  public void Build()
  {
    using (StreamReader streamReader = new StreamReader((Stream) File.OpenRead(this.m_FilePath)))
    {
      string str1;
      while ((str1 = streamReader.ReadLine()) != null)
      {
        string[] strArray = str1.Split(' ');
        string str2 = strArray[0];
        long int64 = Convert.ToInt64(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
        string sha1 = strArray[2];
        string path = Path.Combine(Path.GetDirectoryName(this.m_FilePath), str2);
        FilePart filePart = new FilePart(str2, int64, sha1, path);
        if (filePart.Check())
          filePart.DownloadedSize = filePart.Size;
        this.m_FileParts.Add(filePart);
        this.FileSize += int64;
      }
    }
  }

  public void Dump()
  {
    foreach (FilePart filePart in this.m_FileParts)
      Logger.Info("{0} {1} {2}", (object) filePart.Name, (object) filePart.Size, (object) filePart.SHA1);
  }

  public long Count
  {
    get
    {
      return (long) this.m_FileParts.Count;
    }
  }

  public FilePart this[int i]
  {
    get
    {
      return this.m_FileParts[i];
    }
  }

  [DllImport("kernel32", SetLastError = true)]
  private static extern bool FlushFileBuffers(IntPtr handle);

  public string MakeFile()
  {
    int count1 = 16384;
    byte[] buffer = new byte[count1];
    string path = Path.Combine(Path.GetDirectoryName(this.m_FilePath), Path.GetFileNameWithoutExtension(this.m_FilePath));
    using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
    {
      foreach (FilePart filePart in this.m_FileParts)
      {
        using (Stream stream = (Stream) new FileStream(filePart.Path, FileMode.Open, FileAccess.Read))
        {
          int count2;
          while ((count2 = stream.Read(buffer, 0, count1)) > 0)
            fileStream.Write(buffer, 0, count2);
        }
      }
      fileStream.Flush();
      if (!Manifest.FlushFileBuffers(fileStream.Handle))
        throw new SystemException("Win32 FlushFileBuffers failed for " + path, (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }
    return path;
  }

  public void DeleteFileParts()
  {
    foreach (FilePart filePart in this.m_FileParts)
      File.Delete(filePart.Path);
  }

  public void DeleteManifest()
  {
    File.Delete(this.m_FilePath);
  }

  public long DownloadedSize
  {
    get
    {
      long num = 0;
      foreach (FilePart filePart in this.m_FileParts)
        num += filePart.DownloadedSize;
      return num;
    }
  }

  public long FileSize { get; private set; }

  public float PercentDownloaded()
  {
    return (float) Math.Round((double) this.DownloadedSize * 100.0 / (double) this.FileSize, 1);
  }
}
