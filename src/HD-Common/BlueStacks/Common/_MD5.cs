// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common._MD5
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;
using System.Text;

namespace BlueStacks.Common
{
  public class _MD5
  {
    private static readonly uint[] T = new uint[64]
    {
      3614090360U,
      3905402710U,
      606105819U,
      3250441966U,
      4118548399U,
      1200080426U,
      2821735955U,
      4249261313U,
      1770035416U,
      2336552879U,
      4294925233U,
      2304563134U,
      1804603682U,
      4254626195U,
      2792965006U,
      1236535329U,
      4129170786U,
      3225465664U,
      643717713U,
      3921069994U,
      3593408605U,
      38016083U,
      3634488961U,
      3889429448U,
      568446438U,
      3275163606U,
      4107603335U,
      1163531501U,
      2850285829U,
      4243563512U,
      1735328473U,
      2368359562U,
      4294588738U,
      2272392833U,
      1839030562U,
      4259657740U,
      2763975236U,
      1272893353U,
      4139469664U,
      3200236656U,
      681279174U,
      3936430074U,
      3572445317U,
      76029189U,
      3654602809U,
      3873151461U,
      530742520U,
      3299628645U,
      4096336452U,
      1126891415U,
      2878612391U,
      4237533241U,
      1700485571U,
      2399980690U,
      4293915773U,
      2240044497U,
      1873313359U,
      4264355552U,
      2734768916U,
      1309151649U,
      4149444226U,
      3174756917U,
      718787259U,
      3951481745U
    };
    private Digest dg;

    protected uint[] X { get; set; } = new uint[16];

    protected byte[] m_byteInput { get; set; }

    public string Value
    {
      set
      {
        this.ValueAsByte = Encoding.ASCII.GetBytes(value);
      }
    }

    public byte[] ValueAsByte
    {
      set
      {
        if (value == null)
          return;
        this.m_byteInput = new byte[value.Length];
        for (int index = 0; index < value.Length; ++index)
          this.m_byteInput[index] = value[index];
        this.CalculateMD5Value();
      }
    }

    public string FingerPrint
    {
      get
      {
        return this.dg.GetString();
      }
    }

    public byte[] FingerPrintBytes
    {
      get
      {
        byte[] numArray = new byte[16];
        BitConverter.GetBytes(this.dg.A).CopyTo((Array) numArray, 0);
        BitConverter.GetBytes(this.dg.B).CopyTo((Array) numArray, 4);
        BitConverter.GetBytes(this.dg.C).CopyTo((Array) numArray, 8);
        BitConverter.GetBytes(this.dg.D).CopyTo((Array) numArray, 12);
        return numArray;
      }
    }

    public _MD5()
    {
      this.dg = new Digest();
    }

    public string ValueAsFile
    {
      set
      {
        string path = value;
        int count = 65536;
        byte[] numArray1 = new byte[count];
        uint num = 0;
        byte[] numArray2 = new byte[0];
        using (Stream stream = (Stream) File.OpenRead(path))
        {
          int length;
          while ((length = stream.Read(numArray1, 0, count)) == count)
          {
            ++num;
            this.TransformBlock(numArray1);
          }
          byte[] buffer = new byte[0];
          if (length != 0)
          {
            buffer = new byte[length];
            for (int index = 0; index < length; ++index)
              buffer[index] = numArray1[index];
          }
          this.TransformBlock(_MD5.GetLastPaddedBuffer((ulong) num * (ulong) count + (ulong) length, buffer));
        }
      }
    }

    public void TransformBlock(byte[] block)
    {
      uint num = 0;
      if (block != null)
        num = (uint) (block.Length * 8) / 32U;
      for (uint block1 = 0; block1 < num / 16U; ++block1)
      {
        this.CopyBlock(block, block1);
        uint a = this.dg.A;
        uint b = this.dg.B;
        uint c = this.dg.C;
        uint d = this.dg.D;
        this.PerformTransformation(ref a, ref b, ref c, ref d);
        this.dg.A = a;
        this.dg.B = b;
        this.dg.C = c;
        this.dg.D = d;
      }
    }

    protected static byte[] GetLastPaddedBuffer(ulong totalSize, byte[] buffer)
    {
      uint num1 = (uint) (((buffer == null ? 448 : 448 - buffer.Length * 8 % 512) + 512) % 512);
      if (num1 == 0U)
        num1 = 512U;
      uint num2 = buffer == null ? num1 / 8U + 8U : (uint) ((ulong) buffer.Length + (ulong) (num1 / 8U) + 8UL);
      ulong num3 = totalSize * 8UL;
      byte[] numArray = new byte[(int) num2];
      if (buffer != null)
      {
        for (int index = 0; index < buffer.Length; ++index)
          numArray[index] = buffer[index];
        numArray[buffer.Length] |= (byte) 128;
      }
      for (int index = 8; index > 0; --index)
        numArray[(long) num2 - (long) index] = (byte) (num3 >> (8 - index) * 8 & (ulong) byte.MaxValue);
      return numArray;
    }

    protected void CalculateMD5Value()
    {
      byte[] paddedBuffer = this.CreatePaddedBuffer();
      uint num = (uint) (paddedBuffer.Length * 8) / 32U;
      for (uint block = 0; block < num / 16U; ++block)
      {
        this.CopyBlock(paddedBuffer, block);
        uint a = this.dg.A;
        uint b = this.dg.B;
        uint c = this.dg.C;
        uint d = this.dg.D;
        this.PerformTransformation(ref a, ref b, ref c, ref d);
        this.dg.A = a;
        this.dg.B = b;
        this.dg.C = c;
        this.dg.D = d;
      }
    }

    protected void TransF(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + _MD5.RotateLeft(a + (uint) ((int) b & (int) c | ~(int) b & (int) d) + this.X[(int) k] + _MD5.T[(int) i - 1], s);
    }

    protected void TransG(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + _MD5.RotateLeft(a + (uint) ((int) b & (int) d | (int) c & ~(int) d) + this.X[(int) k] + _MD5.T[(int) i - 1], s);
    }

    protected void TransH(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + _MD5.RotateLeft(a + (b ^ c ^ d) + this.X[(int) k] + _MD5.T[(int) i - 1], s);
    }

    protected void TransI(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + _MD5.RotateLeft(a + (c ^ (b | ~d)) + this.X[(int) k] + _MD5.T[(int) i - 1], s);
    }

    protected void PerformTransformation(ref uint A, ref uint B, ref uint C, ref uint D)
    {
      uint num1 = A;
      uint num2 = B;
      uint num3 = C;
      uint num4 = D;
      this.TransF(ref A, B, C, D, 0U, (ushort) 7, 1U);
      this.TransF(ref D, A, B, C, 1U, (ushort) 12, 2U);
      this.TransF(ref C, D, A, B, 2U, (ushort) 17, 3U);
      this.TransF(ref B, C, D, A, 3U, (ushort) 22, 4U);
      this.TransF(ref A, B, C, D, 4U, (ushort) 7, 5U);
      this.TransF(ref D, A, B, C, 5U, (ushort) 12, 6U);
      this.TransF(ref C, D, A, B, 6U, (ushort) 17, 7U);
      this.TransF(ref B, C, D, A, 7U, (ushort) 22, 8U);
      this.TransF(ref A, B, C, D, 8U, (ushort) 7, 9U);
      this.TransF(ref D, A, B, C, 9U, (ushort) 12, 10U);
      this.TransF(ref C, D, A, B, 10U, (ushort) 17, 11U);
      this.TransF(ref B, C, D, A, 11U, (ushort) 22, 12U);
      this.TransF(ref A, B, C, D, 12U, (ushort) 7, 13U);
      this.TransF(ref D, A, B, C, 13U, (ushort) 12, 14U);
      this.TransF(ref C, D, A, B, 14U, (ushort) 17, 15U);
      this.TransF(ref B, C, D, A, 15U, (ushort) 22, 16U);
      this.TransG(ref A, B, C, D, 1U, (ushort) 5, 17U);
      this.TransG(ref D, A, B, C, 6U, (ushort) 9, 18U);
      this.TransG(ref C, D, A, B, 11U, (ushort) 14, 19U);
      this.TransG(ref B, C, D, A, 0U, (ushort) 20, 20U);
      this.TransG(ref A, B, C, D, 5U, (ushort) 5, 21U);
      this.TransG(ref D, A, B, C, 10U, (ushort) 9, 22U);
      this.TransG(ref C, D, A, B, 15U, (ushort) 14, 23U);
      this.TransG(ref B, C, D, A, 4U, (ushort) 20, 24U);
      this.TransG(ref A, B, C, D, 9U, (ushort) 5, 25U);
      this.TransG(ref D, A, B, C, 14U, (ushort) 9, 26U);
      this.TransG(ref C, D, A, B, 3U, (ushort) 14, 27U);
      this.TransG(ref B, C, D, A, 8U, (ushort) 20, 28U);
      this.TransG(ref A, B, C, D, 13U, (ushort) 5, 29U);
      this.TransG(ref D, A, B, C, 2U, (ushort) 9, 30U);
      this.TransG(ref C, D, A, B, 7U, (ushort) 14, 31U);
      this.TransG(ref B, C, D, A, 12U, (ushort) 20, 32U);
      this.TransH(ref A, B, C, D, 5U, (ushort) 4, 33U);
      this.TransH(ref D, A, B, C, 8U, (ushort) 11, 34U);
      this.TransH(ref C, D, A, B, 11U, (ushort) 16, 35U);
      this.TransH(ref B, C, D, A, 14U, (ushort) 23, 36U);
      this.TransH(ref A, B, C, D, 1U, (ushort) 4, 37U);
      this.TransH(ref D, A, B, C, 4U, (ushort) 11, 38U);
      this.TransH(ref C, D, A, B, 7U, (ushort) 16, 39U);
      this.TransH(ref B, C, D, A, 10U, (ushort) 23, 40U);
      this.TransH(ref A, B, C, D, 13U, (ushort) 4, 41U);
      this.TransH(ref D, A, B, C, 0U, (ushort) 11, 42U);
      this.TransH(ref C, D, A, B, 3U, (ushort) 16, 43U);
      this.TransH(ref B, C, D, A, 6U, (ushort) 23, 44U);
      this.TransH(ref A, B, C, D, 9U, (ushort) 4, 45U);
      this.TransH(ref D, A, B, C, 12U, (ushort) 11, 46U);
      this.TransH(ref C, D, A, B, 15U, (ushort) 16, 47U);
      this.TransH(ref B, C, D, A, 2U, (ushort) 23, 48U);
      this.TransI(ref A, B, C, D, 0U, (ushort) 6, 49U);
      this.TransI(ref D, A, B, C, 7U, (ushort) 10, 50U);
      this.TransI(ref C, D, A, B, 14U, (ushort) 15, 51U);
      this.TransI(ref B, C, D, A, 5U, (ushort) 21, 52U);
      this.TransI(ref A, B, C, D, 12U, (ushort) 6, 53U);
      this.TransI(ref D, A, B, C, 3U, (ushort) 10, 54U);
      this.TransI(ref C, D, A, B, 10U, (ushort) 15, 55U);
      this.TransI(ref B, C, D, A, 1U, (ushort) 21, 56U);
      this.TransI(ref A, B, C, D, 8U, (ushort) 6, 57U);
      this.TransI(ref D, A, B, C, 15U, (ushort) 10, 58U);
      this.TransI(ref C, D, A, B, 6U, (ushort) 15, 59U);
      this.TransI(ref B, C, D, A, 13U, (ushort) 21, 60U);
      this.TransI(ref A, B, C, D, 4U, (ushort) 6, 61U);
      this.TransI(ref D, A, B, C, 11U, (ushort) 10, 62U);
      this.TransI(ref C, D, A, B, 2U, (ushort) 15, 63U);
      this.TransI(ref B, C, D, A, 9U, (ushort) 21, 64U);
      A += num1;
      B += num2;
      C += num3;
      D += num4;
    }

    protected byte[] CreatePaddedBuffer()
    {
      uint num1 = (uint) ((448 - this.m_byteInput.Length * 8 % 512 + 512) % 512);
      if (num1 == 0U)
        num1 = 512U;
      uint num2 = (uint) ((ulong) this.m_byteInput.Length + (ulong) (num1 / 8U) + 8UL);
      ulong num3 = (ulong) this.m_byteInput.Length * 8UL;
      byte[] numArray = new byte[(int) num2];
      for (int index = 0; index < this.m_byteInput.Length; ++index)
        numArray[index] = this.m_byteInput[index];
      numArray[this.m_byteInput.Length] |= (byte) 128;
      for (int index = 8; index > 0; --index)
        numArray[(long) num2 - (long) index] = (byte) (num3 >> (8 - index) * 8 & (ulong) byte.MaxValue);
      return numArray;
    }

    protected void CopyBlock(byte[] bMsg, uint block)
    {
      block <<= 6;
      if (bMsg == null)
        return;
      for (uint index = 0; index < 61U; index += 4U)
        this.X[(int) (index >> 2)] = (uint) ((int) bMsg[(int) block + ((int) index + 3)] << 24 | (int) bMsg[(int) block + ((int) index + 2)] << 16 | (int) bMsg[(int) block + ((int) index + 1)] << 8) | (uint) bMsg[(int) block + (int) index];
    }

    public static uint RotateLeft(uint uiNumber, ushort shift)
    {
      return uiNumber >> 32 - (int) shift | uiNumber << (int) shift;
    }
  }
}
