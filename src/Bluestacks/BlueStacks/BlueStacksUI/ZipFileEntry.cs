// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ZipFileEntry
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.BlueStacksUI
{
  public struct ZipFileEntry : IEquatable<ZipFileEntry>
  {
    public Compression Method { [IsReadOnly] get; set; }

    public string FilenameInZip { [IsReadOnly] get; set; }

    public uint FileSize { [IsReadOnly] get; set; }

    public uint CompressedSize { [IsReadOnly] get; set; }

    public uint HeaderOffset { [IsReadOnly] get; set; }

    public uint FileOffset { [IsReadOnly] get; set; }

    public uint HeaderSize { [IsReadOnly] get; set; }

    public uint Crc32 { [IsReadOnly] get; set; }

    public DateTime ModifyTime { [IsReadOnly] get; set; }

    public string Comment { [IsReadOnly] get; set; }

    public bool EncodeUTF8 { [IsReadOnly] get; set; }

    public override bool Equals(object obj)
    {
      return obj is ZipFileEntry other && this.Equals(other);
    }

    public bool Equals(ZipFileEntry other)
    {
      return this.Method == other.Method && this.FilenameInZip == other.FilenameInZip && ((int) this.FileSize == (int) other.FileSize && (int) this.CompressedSize == (int) other.CompressedSize) && ((int) this.HeaderOffset == (int) other.HeaderOffset && (int) this.FileOffset == (int) other.FileOffset && ((int) this.HeaderSize == (int) other.HeaderSize && (int) this.Crc32 == (int) other.Crc32)) && (this.ModifyTime == other.ModifyTime && this.Comment == other.Comment) && this.EncodeUTF8 == other.EncodeUTF8;
    }

    public override string ToString()
    {
      return this.FilenameInZip;
    }

    public static bool operator ==(ZipFileEntry left, ZipFileEntry right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(ZipFileEntry left, ZipFileEntry right)
    {
      return !(left == right);
    }

    public override int GetHashCode()
    {
      return this.Method.GetHashCode() ^ this.FilenameInZip.GetHashCode() ^ this.FileSize.GetHashCode() ^ this.CompressedSize.GetHashCode() ^ this.HeaderOffset.GetHashCode() ^ this.FileOffset.GetHashCode() ^ this.HeaderSize.GetHashCode() ^ this.Crc32.GetHashCode() ^ this.ModifyTime.GetHashCode() ^ this.Comment.GetHashCode() ^ this.EncodeUTF8.GetHashCode();
    }
  }
}
