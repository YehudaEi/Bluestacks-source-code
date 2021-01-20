// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ZipStorer
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BlueStacks.BlueStacksUI
{
  public class ZipStorer : IDisposable
  {
    private List<ZipFileEntry> Files = new List<ZipFileEntry>();
    private string Comment = "";
    private static uint[] CrcTable = new uint[256];
    private static Encoding DefaultEncoding = Encoding.GetEncoding(437);
    private string FileName;
    private Stream ZipFileStream;
    private byte[] CentralDirImage;
    private ushort ExistingFiles;
    private FileAccess Access;
    private bool disposedValue;

    public bool EncodeUTF8 { get; set; }

    public bool ForceDeflating { get; set; }

    static ZipStorer()
    {
      for (int index1 = 0; index1 < ZipStorer.CrcTable.Length; ++index1)
      {
        uint num = (uint) index1;
        for (int index2 = 0; index2 < 8; ++index2)
        {
          if (((int) num & 1) != 0)
            num = 3988292384U ^ num >> 1;
          else
            num >>= 1;
        }
        ZipStorer.CrcTable[index1] = num;
      }
    }

    public static ZipStorer Create(string _filename, string _comment)
    {
      ZipStorer zipStorer = ZipStorer.Create((Stream) new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite), _comment);
      zipStorer.Comment = _comment;
      zipStorer.FileName = _filename;
      return zipStorer;
    }

    public static ZipStorer Create(Stream _stream, string _comment)
    {
      return new ZipStorer()
      {
        Comment = _comment,
        ZipFileStream = _stream,
        Access = FileAccess.Write
      };
    }

    public static ZipStorer Open(string _filename, FileAccess _access)
    {
      ZipStorer zipStorer = ZipStorer.Open((Stream) new FileStream(_filename, FileMode.Open, _access == FileAccess.Read ? FileAccess.Read : FileAccess.ReadWrite), _access);
      zipStorer.FileName = _filename;
      return zipStorer;
    }

    public static ZipStorer Open(Stream _stream, FileAccess _access)
    {
      if (_stream != null)
      {
        if (!_stream.CanSeek && _access != FileAccess.Read)
          throw new InvalidOperationException("Stream cannot seek");
        ZipStorer zipStorer = new ZipStorer()
        {
          ZipFileStream = _stream,
          Access = _access
        };
        if (zipStorer.ReadFileInfo())
          return zipStorer;
      }
      throw new InvalidDataException();
    }

    public void AddFile(
      BlueStacks.BlueStacksUI.Compression _method,
      string _pathname,
      string _filenameInZip,
      string _comment)
    {
      if (this.Access == FileAccess.Read)
        throw new InvalidOperationException("Writing is not alowed");
      FileStream fileStream = new FileStream(_pathname, FileMode.Open, FileAccess.Read);
      this.AddStream(_method, _filenameInZip, (Stream) fileStream, File.GetLastWriteTime(_pathname), _comment);
      fileStream.Close();
    }

    public void AddStream(
      BlueStacks.BlueStacksUI.Compression _method,
      string _filenameInZip,
      Stream _source,
      DateTime _modTime,
      string _comment)
    {
      if (this.Access == FileAccess.Read || _source == null || string.IsNullOrEmpty(_filenameInZip))
        throw new InvalidOperationException("Writing is not alowed");
      ZipFileEntry _zfe = new ZipFileEntry()
      {
        Method = _method,
        EncodeUTF8 = this.EncodeUTF8,
        FilenameInZip = ZipStorer.NormalizedFilename(_filenameInZip),
        Comment = _comment ?? "",
        Crc32 = 0,
        HeaderOffset = (uint) this.ZipFileStream.Position,
        ModifyTime = _modTime
      };
      this.WriteLocalHeader(ref _zfe);
      _zfe.FileOffset = (uint) this.ZipFileStream.Position;
      this.Store(ref _zfe, _source);
      _source.Close();
      this.UpdateCrcAndSizes(ref _zfe);
      this.Files.Add(_zfe);
    }

    public void Close()
    {
      if (this.Access != FileAccess.Read)
      {
        uint position1 = (uint) this.ZipFileStream.Position;
        uint _size = 0;
        if (this.CentralDirImage != null)
          this.ZipFileStream.Write(this.CentralDirImage, 0, this.CentralDirImage.Length);
        for (int index = 0; index < this.Files.Count; ++index)
        {
          long position2 = this.ZipFileStream.Position;
          this.WriteCentralDirRecord(this.Files[index]);
          _size += (uint) (this.ZipFileStream.Position - position2);
        }
        if (this.CentralDirImage != null)
          this.WriteEndRecord(_size + (uint) this.CentralDirImage.Length, position1);
        else
          this.WriteEndRecord(_size, position1);
      }
      if (this.ZipFileStream == null)
        return;
      this.ZipFileStream.Flush();
      this.ZipFileStream.Dispose();
      this.ZipFileStream = (Stream) null;
    }

    public List<ZipFileEntry> ReadCentralDir()
    {
      if (this.CentralDirImage == null)
        throw new InvalidOperationException("Central directory currently does not exist");
      List<ZipFileEntry> zipFileEntryList = new List<ZipFileEntry>();
      ushort uint16_1;
      ushort uint16_2;
      ushort uint16_3;
      for (int startIndex = 0; startIndex < this.CentralDirImage.Length && BitConverter.ToUInt32(this.CentralDirImage, startIndex) == 33639248U; startIndex += 46 + (int) uint16_1 + (int) uint16_2 + (int) uint16_3)
      {
        int num1 = ((uint) BitConverter.ToUInt16(this.CentralDirImage, startIndex + 8) & 2048U) > 0U ? 1 : 0;
        ushort uint16_4 = BitConverter.ToUInt16(this.CentralDirImage, startIndex + 10);
        uint uint32_1 = BitConverter.ToUInt32(this.CentralDirImage, startIndex + 16);
        uint uint32_2 = BitConverter.ToUInt32(this.CentralDirImage, startIndex + 20);
        uint uint32_3 = BitConverter.ToUInt32(this.CentralDirImage, startIndex + 24);
        uint16_1 = BitConverter.ToUInt16(this.CentralDirImage, startIndex + 28);
        uint16_2 = BitConverter.ToUInt16(this.CentralDirImage, startIndex + 30);
        uint16_3 = BitConverter.ToUInt16(this.CentralDirImage, startIndex + 32);
        uint uint32_4 = BitConverter.ToUInt32(this.CentralDirImage, startIndex + 42);
        uint num2 = 46U + (uint) uint16_1 + (uint) uint16_2 + (uint) uint16_3;
        Encoding encoding = num1 != 0 ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
        ZipFileEntry zipFileEntry = new ZipFileEntry()
        {
          Method = (BlueStacks.BlueStacksUI.Compression) uint16_4,
          FilenameInZip = encoding.GetString(this.CentralDirImage, startIndex + 46, (int) uint16_1),
          FileOffset = this.GetFileOffset(uint32_4),
          FileSize = uint32_3,
          CompressedSize = uint32_2,
          HeaderOffset = uint32_4,
          HeaderSize = num2,
          Crc32 = uint32_1,
          ModifyTime = DateTime.Now
        };
        if (uint16_3 > (ushort) 0)
          zipFileEntry.Comment = encoding.GetString(this.CentralDirImage, startIndex + 46 + (int) uint16_1 + (int) uint16_2, (int) uint16_3);
        zipFileEntryList.Add(zipFileEntry);
      }
      return zipFileEntryList;
    }

    public bool ExtractFile(ZipFileEntry _zfe, string _filename)
    {
      string directoryName = Path.GetDirectoryName(_filename);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      if (Directory.Exists(_filename))
        return true;
      bool file;
      using (Stream _stream = (Stream) new FileStream(_filename, FileMode.Create, FileAccess.Write))
        file = this.ExtractFile(_zfe, _stream);
      File.SetCreationTime(_filename, _zfe.ModifyTime);
      File.SetLastWriteTime(_filename, _zfe.ModifyTime);
      return file;
    }

    public bool ExtractFile(ZipFileEntry _zfe, Stream _stream)
    {
      if (_stream == null || !_stream.CanWrite)
        throw new InvalidOperationException("Stream cannot be written");
      byte[] buffer1 = new byte[4];
      this.ZipFileStream.Seek((long) _zfe.HeaderOffset, SeekOrigin.Begin);
      this.ZipFileStream.Read(buffer1, 0, 4);
      if (BitConverter.ToUInt32(buffer1, 0) != 67324752U)
        return false;
      Stream stream;
      if (_zfe.Method == BlueStacks.BlueStacksUI.Compression.Store)
      {
        stream = this.ZipFileStream;
      }
      else
      {
        if (_zfe.Method != BlueStacks.BlueStacksUI.Compression.Deflate)
          return false;
        stream = (Stream) new DeflateStream(this.ZipFileStream, CompressionMode.Decompress, true);
      }
      byte[] buffer2 = new byte[16384];
      this.ZipFileStream.Seek((long) _zfe.FileOffset, SeekOrigin.Begin);
      int count;
      for (uint fileSize = _zfe.FileSize; fileSize > 0U; fileSize -= (uint) count)
      {
        count = stream.Read(buffer2, 0, (int) Math.Min((long) fileSize, (long) buffer2.Length));
        _stream.Write(buffer2, 0, count);
      }
      _stream.Flush();
      if (_zfe.Method == BlueStacks.BlueStacksUI.Compression.Deflate)
        stream.Dispose();
      return true;
    }

    public static bool RemoveEntries(ref ZipStorer _zip, List<ZipFileEntry> _zfes)
    {
      if (_zip == null || !(_zip.ZipFileStream is FileStream))
        throw new InvalidOperationException("RemoveEntries is allowed just over streams of type FileStream");
      List<ZipFileEntry> zipFileEntryList = _zip.ReadCentralDir();
      string tempFileName1 = Path.GetTempFileName();
      string tempFileName2 = Path.GetTempFileName();
      try
      {
        ZipStorer zipStorer = ZipStorer.Create(tempFileName1, string.Empty);
        foreach (ZipFileEntry _zfe in zipFileEntryList)
        {
          if (_zfes != null && !_zfes.Contains(_zfe) && _zip.ExtractFile(_zfe, tempFileName2))
            zipStorer.AddFile(_zfe.Method, tempFileName2, _zfe.FilenameInZip, _zfe.Comment);
        }
        _zip.Close();
        zipStorer.Close();
        File.Delete(_zip.FileName);
        File.Move(tempFileName1, _zip.FileName);
        _zip = ZipStorer.Open(_zip.FileName, _zip.Access);
      }
      catch
      {
        return false;
      }
      finally
      {
        if (File.Exists(tempFileName1))
          File.Delete(tempFileName1);
        if (File.Exists(tempFileName2))
          File.Delete(tempFileName2);
      }
      return true;
    }

    private uint GetFileOffset(uint _headerOffset)
    {
      byte[] buffer = new byte[2];
      this.ZipFileStream.Seek((long) (_headerOffset + 26U), SeekOrigin.Begin);
      this.ZipFileStream.Read(buffer, 0, 2);
      ushort uint16_1 = BitConverter.ToUInt16(buffer, 0);
      this.ZipFileStream.Read(buffer, 0, 2);
      ushort uint16_2 = BitConverter.ToUInt16(buffer, 0);
      return (uint) ((ulong) (30 + (int) uint16_1 + (int) uint16_2) + (ulong) _headerOffset);
    }

    private void WriteLocalHeader(ref ZipFileEntry _zfe)
    {
      long position = this.ZipFileStream.Position;
      byte[] bytes = (_zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding).GetBytes(_zfe.FilenameInZip);
      this.ZipFileStream.Write(new byte[6]
      {
        (byte) 80,
        (byte) 75,
        (byte) 3,
        (byte) 4,
        (byte) 20,
        (byte) 0
      }, 0, 6);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? (ushort) 2048 : (ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) _zfe.Method), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
      this.ZipFileStream.Write(new byte[12], 0, 12);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) bytes.Length), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      this.ZipFileStream.Write(bytes, 0, bytes.Length);
      _zfe.HeaderSize = (uint) (this.ZipFileStream.Position - position);
    }

    private void WriteCentralDirRecord(ZipFileEntry _zfe)
    {
      Encoding encoding = _zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
      byte[] bytes1 = encoding.GetBytes(_zfe.FilenameInZip);
      byte[] bytes2 = encoding.GetBytes(_zfe.Comment);
      this.ZipFileStream.Write(new byte[8]
      {
        (byte) 80,
        (byte) 75,
        (byte) 1,
        (byte) 2,
        (byte) 23,
        (byte) 11,
        (byte) 20,
        (byte) 0
      }, 0, 8);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? (ushort) 2048 : (ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) _zfe.Method), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) bytes1.Length), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) bytes2.Length), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) 33024), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);
      this.ZipFileStream.Write(bytes1, 0, bytes1.Length);
      this.ZipFileStream.Write(bytes2, 0, bytes2.Length);
    }

    private void WriteEndRecord(uint _size, uint _offset)
    {
      byte[] bytes = (this.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding).GetBytes(this.Comment);
      this.ZipFileStream.Write(new byte[8]
      {
        (byte) 80,
        (byte) 75,
        (byte) 5,
        (byte) 6,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, 0, 8);
      this.ZipFileStream.Write(BitConverter.GetBytes((int) (ushort) this.Files.Count + (int) this.ExistingFiles), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes((int) (ushort) this.Files.Count + (int) this.ExistingFiles), 0, 2);
      this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) bytes.Length), 0, 2);
      this.ZipFileStream.Write(bytes, 0, bytes.Length);
    }

    private void Store(ref ZipFileEntry _zfe, Stream _source)
    {
      byte[] buffer = new byte[16384];
      uint num = 0;
      Stream stream = (Stream) null;
      long position1 = this.ZipFileStream.Position;
      long position2 = _source.Position;
      if (_zfe.Method == BlueStacks.BlueStacksUI.Compression.Store)
        stream = this.ZipFileStream;
      else if (_zfe.Method == BlueStacks.BlueStacksUI.Compression.Deflate)
        stream = (Stream) new DeflateStream(this.ZipFileStream, CompressionMode.Compress, true);
      _zfe.Crc32 = uint.MaxValue;
      int count;
      do
      {
        count = _source.Read(buffer, 0, buffer.Length);
        num += (uint) count;
        if (count > 0)
        {
          stream?.Write(buffer, 0, count);
          for (uint index = 0; (long) index < (long) count; ++index)
            _zfe.Crc32 = ZipStorer.CrcTable[((int) _zfe.Crc32 ^ (int) buffer[(int) index]) & (int) byte.MaxValue] ^ _zfe.Crc32 >> 8;
        }
      }
      while (count == buffer.Length);
      stream.Flush();
      if (_zfe.Method == BlueStacks.BlueStacksUI.Compression.Deflate)
        stream.Dispose();
      _zfe.Crc32 ^= uint.MaxValue;
      _zfe.FileSize = num;
      _zfe.CompressedSize = (uint) (this.ZipFileStream.Position - position1);
      if (_zfe.Method != BlueStacks.BlueStacksUI.Compression.Deflate || this.ForceDeflating || (!_source.CanSeek || _zfe.CompressedSize <= _zfe.FileSize))
        return;
      _zfe.Method = BlueStacks.BlueStacksUI.Compression.Store;
      this.ZipFileStream.Position = position1;
      this.ZipFileStream.SetLength(position1);
      _source.Position = position2;
      this.Store(ref _zfe, _source);
    }

    private static uint DateTimeToDosTime(DateTime _dt)
    {
      return (uint) (_dt.Second / 2 | _dt.Minute << 5 | _dt.Hour << 11 | _dt.Day << 16 | _dt.Month << 21 | _dt.Year - 1980 << 25);
    }

    private void UpdateCrcAndSizes(ref ZipFileEntry _zfe)
    {
      long position = this.ZipFileStream.Position;
      this.ZipFileStream.Position = (long) (_zfe.HeaderOffset + 8U);
      this.ZipFileStream.Write(BitConverter.GetBytes((ushort) _zfe.Method), 0, 2);
      this.ZipFileStream.Position = (long) (_zfe.HeaderOffset + 14U);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
      this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
      this.ZipFileStream.Position = position;
    }

    private static string NormalizedFilename(string _filename)
    {
      string str = _filename.Replace('\\', '/');
      int num = str.IndexOf(':');
      if (num >= 0)
        str = str.Remove(0, num + 1);
      return str.Trim('/');
    }

    private bool ReadFileInfo()
    {
      if (this.ZipFileStream.Length < 22L)
        return false;
      try
      {
        this.ZipFileStream.Seek(-17L, SeekOrigin.End);
        using (BinaryReader binaryReader = new BinaryReader(this.ZipFileStream))
        {
          do
          {
            this.ZipFileStream.Seek(-5L, SeekOrigin.Current);
            if (binaryReader.ReadUInt32() == 101010256U)
            {
              this.ZipFileStream.Seek(6L, SeekOrigin.Current);
              ushort num1 = binaryReader.ReadUInt16();
              int count = binaryReader.ReadInt32();
              uint num2 = binaryReader.ReadUInt32();
              if (this.ZipFileStream.Position + (long) binaryReader.ReadUInt16() != this.ZipFileStream.Length)
                return false;
              this.ExistingFiles = num1;
              this.CentralDirImage = new byte[count];
              this.ZipFileStream.Seek((long) num2, SeekOrigin.Begin);
              this.ZipFileStream.Read(this.CentralDirImage, 0, count);
              this.ZipFileStream.Seek((long) num2, SeekOrigin.Begin);
              return true;
            }
          }
          while (this.ZipFileStream.Position > 0L);
        }
      }
      catch
      {
      }
      return false;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.Close();
      this.disposedValue = true;
    }

    ~ZipStorer()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
