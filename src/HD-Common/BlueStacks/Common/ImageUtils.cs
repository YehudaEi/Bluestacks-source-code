﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ImageUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
  public static class ImageUtils
  {
    public static BitmapImage BitmapFromPath(string path)
    {
      BitmapImage bitmapImage = (BitmapImage) null;
      if (File.Exists(path))
      {
        bitmapImage = new BitmapImage();
        try
        {
          using (FileStream fileStream = File.OpenRead(path))
          {
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = (Stream) fileStream;
            bitmapImage.EndInit();
          }
        }
        catch
        {
        }
      }
      return bitmapImage;
    }

    public static BitmapImage BitmapFromUri(string uri)
    {
      BitmapImage bitmapImage = (BitmapImage) null;
      try
      {
        bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.UriSource = new Uri(uri);
        bitmapImage.EndInit();
      }
      catch
      {
      }
      return bitmapImage;
    }

    public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();
        bmpBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource) bitmapImage));
        bmpBitmapEncoder.Save((Stream) memoryStream);
        return new Bitmap((Stream) memoryStream);
      }
    }

    public static BitmapImage ByteArrayToImage(byte[] dataArray)
    {
      using (MemoryStream memoryStream = new MemoryStream(dataArray))
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = (Stream) memoryStream;
        bitmapImage.EndInit();
        return bitmapImage;
      }
    }
  }
}
