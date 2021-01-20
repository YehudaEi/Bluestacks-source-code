// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Input
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  internal class Input
  {
    private const uint MOUSEEVENTF_FROMTOUCH = 4283520896;
    private const uint MOUSEEVENTF_FROMPEN = 4283520768;
    private const uint MOUSEEVENTF_MASK = 4294967168;
    private const int WM_KEYDOWN = 256;
    private const int WM_KEYUP = 256;
    private const int WM_SYSKEYDOWN = 260;
    private const int WM_SYSKEYUP = 261;
    private const int WH_KEYBOARD_LL = 13;
    private const int HC_ACTION = 0;
    public const int VK_LWIN = 91;
    private static int sHookHandle;
    private static Input.HookProc sHookProc;

    [DllImport("user32.dll")]
    private static extern uint GetMessageExtraInfo();

    public static bool IsEventFromTouch()
    {
      return ((int) Input.GetMessageExtraInfo() & (int) sbyte.MinValue) == -11446400;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern ushort GlobalAddAtom(string str);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetProp(IntPtr wind, string str, IntPtr data);

    public static void DisablePressAndHold(IntPtr hWnd)
    {
      try
      {
        string str = "MicrosoftTabletPenServiceProperty";
        if (Input.GlobalAddAtom(str) == (ushort) 0)
          throw new SystemException("Cannot add global atom", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
        if (!Input.SetProp(hWnd, str, (IntPtr) 1))
          throw new SystemException("Cannot set property", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not disable press and hold: {0}", (object) ex);
      }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowsHookEx(
      int type,
      Input.HookProc callback,
      IntPtr module,
      uint threadId);

    [DllImport("user32.dll")]
    private static extern int CallNextHookEx(int handle, int code, uint wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(int handle);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(IntPtr name);

    public static void HookKeyboard(Input.KeyboardCallback cb)
    {
      Input.sHookProc = (Input.HookProc) ((code, wparam, lparam) =>
      {
        if (code < 0 || wparam == 260U || wparam == 261U)
          return Input.CallNextHookEx(Input.sHookHandle, code, wparam, lparam);
        Input.HookData structure = (Input.HookData) Marshal.PtrToStructure(lparam, typeof (Input.HookData));
        return !cb(wparam == 256U, structure.vkCode) ? 1 : Input.CallNextHookEx(Input.sHookHandle, code, wparam, lparam);
      });
      if (Input.sHookHandle != 0)
        throw new SystemException("Keyboard hook is already set");
      IntPtr moduleHandle = Input.GetModuleHandle(IntPtr.Zero);
      Input.sHookHandle = Input.SetWindowsHookEx(13, Input.sHookProc, moduleHandle, 0U);
      if (Input.sHookHandle == 0)
        throw new SystemException("Cannot set hooks", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }

    public static void UnhookKeyboard()
    {
      if (Input.sHookHandle == 0)
        return;
      Input.UnhookWindowsHookEx(Input.sHookHandle);
      Input.sHookHandle = 0;
    }

    private struct HookData
    {
      public uint vkCode;
      public uint scanCode;
      public uint flags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    public delegate bool KeyboardCallback(bool pressed, uint key);

    private delegate int HookProc(int code, uint wparam, IntPtr lparam);
  }
}
