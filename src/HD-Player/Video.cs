// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Video
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;

namespace BlueStacks.Player
{
  public class Video
  {
    private const uint OFFSET_MAGIC = 0;
    private const uint OFFSET_LENGTH = 4;
    private const uint OFFSET_OFFSET = 8;
    private const uint OFFSET_MODE = 12;
    private const uint OFFSET_STRIDE = 16;
    private const uint OFFSET_DIRTY = 20;
    private IntPtr addr;
    private unsafe byte* raw;

    public unsafe Video(IntPtr addr)
    {
      this.addr = addr;
      this.raw = (byte*) (void*) addr;
    }

    public void CheckMagic()
    {
      uint magic = 0;
      if (!HDPlusModule.VideoCheckMagic(this.addr, ref magic))
        throw new SystemException("Bad magic 0x" + magic.ToString("x"));
    }

    public Video.Mode GetMode()
    {
      uint width = 0;
      uint height = 0;
      uint depth = 0;
      HDPlusModule.VideoGetMode(this.addr, ref width, ref height, ref depth);
      return new Video.Mode((int) width, (int) height, (int) depth);
    }

    public bool GetAndClearDirty()
    {
      return HDPlusModule.VideoGetAndClearDirty(this.addr);
    }

    public unsafe uint GetStride()
    {
      return (uint) *(ushort*) (this.raw + 16);
    }

    public unsafe IntPtr GetBufferAddr()
    {
      return (IntPtr) (void*) (this.raw + *(uint*) (this.raw + 8));
    }

    public unsafe IntPtr GetBufferEnd()
    {
      return (IntPtr) (void*) (this.raw + *(uint*) (this.raw + 4));
    }

    public uint GetBufferSize()
    {
      int num = (int) this.GetBufferEnd() - (int) this.GetBufferAddr();
      if (num >= 0)
        return (uint) num;
      throw new SystemException("Buffer size is negative");
    }

    public class Mode
    {
      public int width;
      public int height;
      public int depth;

      public Mode(int width, int height, int depth)
      {
        this.width = width;
        this.height = height;
        this.depth = depth;
      }

      public int Width
      {
        get
        {
          return this.width;
        }
      }

      public int Height
      {
        get
        {
          return this.height;
        }
      }

      public int Depth
      {
        get
        {
          return this.depth;
        }
      }
    }
  }
}
