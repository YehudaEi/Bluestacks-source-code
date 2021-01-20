// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.NativePipe
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System.Runtime.InteropServices;

namespace DiscordRPC.IO
{
  internal static class NativePipe
  {
    [DllImport("DiscordRPC.Native.dll", EntryPoint = "isConnected", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsConnected();

    [DllImport("DiscordRPC.Native.dll", EntryPoint = "readFrame", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ReadFrame(byte[] buffer, int length);

    [DllImport("DiscordRPC.Native.dll", EntryPoint = "writeFrame", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool WriteFrame(byte[] buffer, int length);

    [DllImport("DiscordRPC.Native.dll", EntryPoint = "close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Close();

    [DllImport("DiscordRPC.Native.dll", EntryPoint = "open", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint Open(string pipename);

    public enum PipeReadError
    {
      FailedToPeek = -5, // 0xFFFFFFFB
      ReadEmptyMessage = -4, // 0xFFFFFFFC
      FailedToRead = -3, // 0xFFFFFFFD
      PipeNotConnected = -2, // 0xFFFFFFFE
      BufferZeroSized = -1, // 0xFFFFFFFF
      None = 0,
    }
  }
}
