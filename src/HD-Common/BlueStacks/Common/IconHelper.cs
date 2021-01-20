// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.IconHelper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BlueStacks.Common
{
  public static class IconHelper
  {
    public static bool ConvertToIcon(
      Stream input,
      Stream output,
      int size = 16,
      bool preserveAspectRatio = false)
    {
      Bitmap bitmap1 = (Bitmap) Image.FromStream(input);
      if (bitmap1 == null)
        return false;
      float num1 = (float) size;
      float num2 = (float) size;
      if (preserveAspectRatio)
      {
        if (bitmap1.Width > bitmap1.Height)
          num2 = (float) bitmap1.Height / (float) bitmap1.Width * (float) size;
        else
          num1 = (float) bitmap1.Width / (float) bitmap1.Height * (float) size;
      }
      using (Bitmap bitmap2 = new Bitmap((Image) bitmap1, new Size((int) num1, (int) num2)))
      {
        if (bitmap2 == null)
          return false;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          bitmap2.Save((Stream) memoryStream, ImageFormat.Png);
          using (BinaryWriter binaryWriter = new BinaryWriter(output))
          {
            if (output == null || binaryWriter == null)
              return false;
            binaryWriter.Write((byte) 0);
            binaryWriter.Write((byte) 0);
            binaryWriter.Write((short) 1);
            binaryWriter.Write((short) 1);
            binaryWriter.Write((byte) num1);
            binaryWriter.Write((byte) num2);
            binaryWriter.Write((byte) 0);
            binaryWriter.Write((byte) 0);
            binaryWriter.Write((short) 0);
            binaryWriter.Write((short) 32);
            binaryWriter.Write((int) memoryStream.Length);
            binaryWriter.Write(22);
            binaryWriter.Write(memoryStream.ToArray());
            binaryWriter.Flush();
            binaryWriter.Close();
          }
        }
        return true;
      }
    }

    public static bool ConvertToIcon(
      string inputPath,
      string outputPath,
      int size = 16,
      bool preserveAspectRatio = false)
    {
      using (FileStream fileStream1 = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
      {
        using (FileStream fileStream2 = new FileStream(outputPath, FileMode.OpenOrCreate))
          return IconHelper.ConvertToIcon((Stream) fileStream1, (Stream) fileStream2, size, preserveAspectRatio);
      }
    }

    public static byte[] ConvertToIcon(Image image, bool preserveAspectRatio = false)
    {
      using (MemoryStream memoryStream1 = new MemoryStream())
      {
        image?.Save((Stream) memoryStream1, ImageFormat.Png);
        memoryStream1.Seek(0L, SeekOrigin.Begin);
        using (MemoryStream memoryStream2 = new MemoryStream())
        {
          int width = image.Size.Width;
          return !IconHelper.ConvertToIcon((Stream) memoryStream1, (Stream) memoryStream2, width, preserveAspectRatio) ? (byte[]) null : memoryStream2.ToArray();
        }
      }
    }
  }
}
