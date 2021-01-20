// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GlobalKeyBoardMouseHooks
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.BlueStacksUI
{
  internal class GlobalKeyBoardMouseHooks
  {
    private static bool mIsHidden = false;
    private static readonly GlobalKeyBoardMouseHooks.LowLevelKeyboardProc mKeyboardProc = new GlobalKeyBoardMouseHooks.LowLevelKeyboardProc(GlobalKeyBoardMouseHooks.KeyboardHookCallback);
    private static IntPtr mKeyboardHookID = IntPtr.Zero;
    private static string sKey = (string) null;
    private static bool sIsControlUsedInBossKey = false;
    private static bool sIsAltUsedInBossKey = false;
    private static bool sIsShiftUsedInBossKey = false;
    internal static bool sIsEnableKeyboardHookLogging = false;
    private static GlobalKeyBoardMouseHooks.LowLevelMouseProc mMouseProc = new GlobalKeyBoardMouseHooks.LowLevelMouseProc(GlobalKeyBoardMouseHooks.MouseHookCallback);
    private static IntPtr mMouseHookId = IntPtr.Zero;
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 256;
    private const int WM_KEYUP = 257;
    private const int WM_SYSKEYDOWN = 260;
    private const int WH_MOUSE_LL = 14;

    private static IntPtr SetHook(GlobalKeyBoardMouseHooks.LowLevelMouseProc proc)
    {
      using (Process currentProcess = Process.GetCurrentProcess())
      {
        using (ProcessModule mainModule = currentProcess.MainModule)
          return NativeMethods.SetWindowsHookEx(14, proc, NativeMethods.GetModuleHandle(mainModule.ModuleName), 0U);
      }
    }

    private static IntPtr SetHook(GlobalKeyBoardMouseHooks.LowLevelKeyboardProc proc)
    {
      using (Process currentProcess = Process.GetCurrentProcess())
      {
        using (ProcessModule mainModule = currentProcess.MainModule)
          return NativeMethods.SetWindowsHookEx(13, proc, NativeMethods.GetModuleHandle(mainModule.ModuleName), 0U);
      }
    }

    internal static void SetMouseMoveHook()
    {
      try
      {
        if (!(GlobalKeyBoardMouseHooks.mMouseHookId == IntPtr.Zero))
          return;
        GlobalKeyBoardMouseHooks.mMouseHookId = GlobalKeyBoardMouseHooks.SetHook(GlobalKeyBoardMouseHooks.mMouseProc);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception setting global mouse hook" + ex.ToString());
      }
    }

    internal static void SetBossKeyHook()
    {
      try
      {
        GlobalKeyBoardMouseHooks.SetKey(RegistryManager.Instance.BossKey);
        if (!(GlobalKeyBoardMouseHooks.mKeyboardHookID == IntPtr.Zero))
          return;
        GlobalKeyBoardMouseHooks.mKeyboardHookID = GlobalKeyBoardMouseHooks.SetHook(GlobalKeyBoardMouseHooks.mKeyboardProc);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception setting global hook" + ex.ToString());
      }
    }

    internal static void SetKey(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        Logger.Warning("Cannot set an empty key");
      }
      else
      {
        string[] strArray = key.Split(new char[2]
        {
          '+',
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        string bossKey = strArray[strArray.Length - 1];
        GlobalKeyBoardMouseHooks.sKey = IMAPKeys.mDictKeys.First<KeyValuePair<Key, string>>((Func<KeyValuePair<Key, string>, bool>) (x => x.Value == bossKey)).Key.ToString();
        int num;
        GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = (num = 0) != 0;
        GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = num != 0;
        GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = num != 0;
        foreach (string a in strArray)
        {
          if (string.Equals(a, "Ctrl", StringComparison.InvariantCulture))
            GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = true;
          else if (string.Equals(a, "Alt", StringComparison.InvariantCulture))
            GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = true;
          else if (string.Equals(a, "Shift", StringComparison.InvariantCulture))
            GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = true;
        }
      }
    }

    internal static void UnsetKey()
    {
      GlobalKeyBoardMouseHooks.sKey = string.Empty;
      GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = false;
      GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = false;
      GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = false;
    }

    internal static void UnHookGlobalHooks()
    {
      NativeMethods.UnhookWindowsHookEx(GlobalKeyBoardMouseHooks.mKeyboardHookID);
      GlobalKeyBoardMouseHooks.mKeyboardHookID = IntPtr.Zero;
      GlobalKeyBoardMouseHooks.UnhookGlobalMouseHooks();
    }

    internal static void UnhookGlobalMouseHooks()
    {
      if (!(GlobalKeyBoardMouseHooks.mMouseHookId != IntPtr.Zero))
        return;
      NativeMethods.UnhookWindowsHookEx(GlobalKeyBoardMouseHooks.mMouseHookId);
      GlobalKeyBoardMouseHooks.mMouseHookId = IntPtr.Zero;
    }

    private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      try
      {
        if (GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging)
          Logger.Info("Keyboard hook .." + nCode.ToString() + ".." + wParam.ToString() + ".." + lParam.ToString());
        MainWindow window = BlueStacksUIUtils.ActivatedWindow;
        if (nCode >= 0)
        {
          if (!(wParam == (IntPtr) 256) && !(wParam == (IntPtr) 260))
          {
            if (!(wParam == (IntPtr) 257))
              goto label_19;
          }
          int virtualKey = Marshal.ReadInt32(lParam);
          Logger.Debug("Keyboard hook .." + virtualKey.ToString() + ".." + GlobalKeyBoardMouseHooks.sKey);
          if (!(wParam == (IntPtr) 256))
          {
            if (!(wParam == (IntPtr) 260))
              goto label_19;
          }
          if (!string.IsNullOrEmpty(GlobalKeyBoardMouseHooks.sKey) && (Keys) virtualKey == (Keys) Enum.Parse(typeof (Keys), GlobalKeyBoardMouseHooks.sKey, false) && (GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey ? 1 : 0) == (Keyboard.IsKeyDown(Key.LeftCtrl) ? 1 : (Keyboard.IsKeyDown(Key.RightCtrl) ? 1 : 0)) && ((GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey ? 1 : 0) == (Keyboard.IsKeyDown(Key.LeftAlt) ? 1 : (Keyboard.IsKeyDown(Key.RightAlt) ? 1 : 0)) && (GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey ? 1 : 0) == (Keyboard.IsKeyDown(Key.LeftShift) ? 1 : (Keyboard.IsKeyDown(Key.RightShift) ? 1 : 0))))
          {
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
            {
              if (BlueStacksUIUtils.DictWindows.Values.Count <= 0)
                return;
              MainWindow mainWindow = BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0];
              mainWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                try
                {
                  if (mainWindow.OwnedWindows.OfType<OnBoardingPopupWindow>().Any<OnBoardingPopupWindow>() || mainWindow.OwnedWindows.OfType<GameOnboardingControl>().Any<GameOnboardingControl>())
                    return;
                  GlobalKeyBoardMouseHooks.mIsHidden = !GlobalKeyBoardMouseHooks.mIsHidden;
                  BlueStacksUIUtils.HideUnhideBlueStacks(GlobalKeyBoardMouseHooks.mIsHidden);
                }
                catch
                {
                }
              }));
            }));
            return (IntPtr) 1;
          }
          if (window != null)
          {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
              if (!Keyboard.IsKeyDown(Key.RightCtrl))
                goto label_19;
            }
            if (!Keyboard.IsKeyDown(Key.LeftAlt))
            {
              if (!Keyboard.IsKeyDown(Key.RightAlt))
                goto label_19;
            }
            if (virtualKey >= 96 && virtualKey <= 105)
              virtualKey -= 48;
            string vkString = IMAPKeys.GetStringForFile(KeyInterop.KeyFromVirtualKey(virtualKey));
            if (MainWindow.sMacroMapping.Keys.Contains<string>(vkString))
              ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
              {
                try
                {
                  window.Dispatcher.Invoke((Delegate) (() =>
                  {
                    if (window.mSidebar.GetElementFromTag("sidebar_macro") != null && window.mSidebar.GetElementFromTag("sidebar_macro").Visibility == Visibility.Visible && window.mSidebar.GetElementFromTag("sidebar_macro").IsEnabled)
                    {
                      if (window.mIsMacroRecorderActive)
                        window.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
                      else if (window.mIsMacroPlaying)
                      {
                        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
                        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
                        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
                        customMessageWindow.Owner = (Window) window;
                        customMessageWindow.ShowDialog();
                      }
                      else
                      {
                        try
                        {
                          string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, MainWindow.sMacroMapping[vkString] + ".json");
                          if (!File.Exists(path))
                            return;
                          MacroRecording record = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), Utils.GetSerializerSettings());
                          record.Name = MainWindow.sMacroMapping[vkString];
                          window.mCommonHandler.FullMacroScriptPlayHandler(record);
                          ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "shortcut_keys", record.RecordingType.ToString(), string.IsNullOrEmpty(record.MacroId) ? "local" : "community", (string) null, (string) null, "Android");
                        }
                        catch (Exception ex)
                        {
                          Logger.Error("Exception in macro play with shortcut: " + ex.ToString());
                        }
                      }
                    }
                    else
                      Logger.Info("Macro not enabled for the current package: " + window.StaticComponents.mSelectedTabButton.PackageName);
                  }));
                }
                catch
                {
                }
              }));
          }
        }
      }
      catch
      {
      }
label_19:
      return NativeMethods.CallNextHookEx(GlobalKeyBoardMouseHooks.mKeyboardHookID, nCode, wParam, lParam);
    }

    private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      return NativeMethods.CallNextHookEx(GlobalKeyBoardMouseHooks.mMouseHookId, nCode, wParam, lParam);
    }

    internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private enum MouseMessages
    {
      WM_MOUSEMOVE = 512, // 0x00000200
      WM_LBUTTONDOWN = 513, // 0x00000201
      WM_LBUTTONUP = 514, // 0x00000202
      WM_RBUTTONDOWN = 516, // 0x00000204
      WM_RBUTTONUP = 517, // 0x00000205
      WM_MOUSEWHEEL = 522, // 0x0000020A
    }

    private struct MSLLHOOKSTRUCT
    {
      public BlueStacks.Common.POINT pt;
      public uint mouseData;
      public uint flags;
      public uint time;
      public IntPtr dwExtraInfo;
    }
  }
}
