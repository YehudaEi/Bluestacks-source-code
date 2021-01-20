// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.KeyboardSend
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Common
{
  public static class KeyboardSend
  {
    private const int KEYEVENTF_EXTENDEDKEY = 1;
    private const int KEYEVENTF_KEYUP = 2;

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public static void KeyDown(Keys vKey)
    {
      KeyboardSend.keybd_event((byte) vKey, (byte) 0, 1, 0);
    }

    public static void KeyUp(Keys vKey)
    {
      KeyboardSend.keybd_event((byte) vKey, (byte) 0, 3, 0);
    }
  }
}
