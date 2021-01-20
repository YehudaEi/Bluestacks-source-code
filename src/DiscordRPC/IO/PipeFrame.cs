// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.PipeFrame
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace DiscordRPC.IO
{
  public struct PipeFrame
  {
    public static readonly int MAX_SIZE = 16384;

    public Opcode Opcode { get; set; }

    public uint Length
    {
      get
      {
        return (uint) this.Data.Length;
      }
    }

    public byte[] Data { get; set; }

    public string Message
    {
      get
      {
        return this.GetMessage();
      }
      set
      {
        this.SetMessage(value);
      }
    }

    public PipeFrame(Opcode opcode, object data)
    {
      this.Opcode = opcode;
      this.Data = (byte[]) null;
      this.SetObject(data);
    }

    public Encoding MessageEncoding
    {
      get
      {
        return Encoding.UTF8;
      }
    }

    private void SetMessage(string str)
    {
      this.Data = this.MessageEncoding.GetBytes(str);
    }

    private string GetMessage()
    {
      return this.MessageEncoding.GetString(this.Data);
    }

    public void SetObject(object obj)
    {
      this.SetMessage(JsonConvert.SerializeObject(obj));
    }

    public void SetObject(Opcode opcode, object obj)
    {
      this.Opcode = opcode;
      this.SetObject(obj);
    }

    public T GetObject<T>()
    {
      return JsonConvert.DeserializeObject<T>(this.GetMessage());
    }

    public bool ReadStream(Stream stream)
    {
      uint num;
      uint b1;
      if (!this.TryReadUInt32(stream, out num) || !this.TryReadUInt32(stream, out b1))
        return false;
      uint b2 = b1;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        byte[] buffer = new byte[this.Min(2048, b1)];
        int count;
        while ((count = stream.Read(buffer, 0, this.Min(buffer.Length, b2))) > 0)
        {
          b2 -= b1;
          memoryStream.Write(buffer, 0, count);
        }
        byte[] array = memoryStream.ToArray();
        if ((long) array.Length != (long) b1)
          return false;
        this.Opcode = (Opcode) num;
        this.Data = array;
        return true;
      }
    }

    private int Min(int a, uint b)
    {
      return (long) b >= (long) a ? a : (int) b;
    }

    private bool TryReadUInt32(Stream stream, out uint value)
    {
      byte[] buffer = new byte[4];
      if (stream.Read(buffer, 0, buffer.Length) != 4)
      {
        value = 0U;
        return false;
      }
      value = BitConverter.ToUInt32(buffer, 0);
      return true;
    }

    public void WriteStream(Stream stream)
    {
      byte[] bytes1 = BitConverter.GetBytes((uint) this.Opcode);
      byte[] bytes2 = BitConverter.GetBytes(this.Length);
      byte[] buffer = new byte[bytes1.Length + bytes2.Length + this.Data.Length];
      bytes1.CopyTo((Array) buffer, 0);
      bytes2.CopyTo((Array) buffer, bytes1.Length);
      this.Data.CopyTo((Array) buffer, bytes1.Length + bytes2.Length);
      stream.Write(buffer, 0, buffer.Length);
    }
  }
}
