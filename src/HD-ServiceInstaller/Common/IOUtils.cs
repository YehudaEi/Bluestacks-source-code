// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.IOUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common
{
  public static class IOUtils
  {
    public static readonly char[] DisallowedCharsInDirs = new char[6]
    {
      '&',
      '<',
      '>',
      '"',
      '\'',
      '^'
    };

    public static void DeleteIfExists(IEnumerable<string> filesToDelete)
    {
      if (filesToDelete == null)
        return;
      foreach (string path in filesToDelete)
      {
        try
        {
          if (File.Exists(path))
            File.Delete(path);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while deleting file " + path + ex.ToString());
        }
      }
    }

    public static long GetAvailableDiskSpaceOfDrive(string path)
    {
      return new DriveInfo(path).AvailableFreeSpace;
    }

    public static string GetPartitionNameFromPath(string path)
    {
      return new DriveInfo(path).Name;
    }

    public static long GetDirectorySize(string dirPath)
    {
      if (!Directory.Exists(dirPath))
        return 0;
      DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
      long num = 0;
      foreach (FileInfo file in directoryInfo.GetFiles())
        num += file.Length;
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        num += IOUtils.GetDirectorySize(directory.FullName);
      return num;
    }

    public static bool IfPathExists(string path)
    {
      return new DirectoryInfo(path).Exists || new FileInfo(path).Exists;
    }

    public static string GetFileOrFolderName(string path)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      if (directoryInfo.Exists)
        return directoryInfo.Name;
      FileInfo fileInfo = new FileInfo(path);
      if (fileInfo.Exists)
        return fileInfo.Name;
      throw new IOException("File or folder name does not exist");
    }

    public static bool IsDirectoryEmpty(string dir)
    {
      bool flag = true;
      if (!Directory.Exists(dir))
      {
        Logger.Info(dir + " does not exist");
        return flag;
      }
      if (Directory.GetFiles(dir).Length == 0)
        Logger.Info(dir + " is empty");
      else
        flag = false;
      foreach (string directory in Directory.GetDirectories(dir))
      {
        Directory.GetFiles(directory);
        if (!IOUtils.IsDirectoryEmpty(directory))
          flag = false;
      }
      return flag;
    }
  }
}
