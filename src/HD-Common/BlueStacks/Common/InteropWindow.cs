// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InteropWindow
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public static class InteropWindow
  {
    public static readonly IntPtr HWND_TOP = IntPtr.Zero;
    public const int WM_CREATE = 1;
    public const int WM_CLOSE = 16;
    public const int WM_INPUT = 255;
    public const int WM_USER = 1024;
    public const int WM_USER_SHOW_WINDOW = 1025;
    public const int WM_USER_SWITCH_TO_LAUNCHER = 1026;
    public const int WM_USER_RESIZE_WINDOW = 1027;
    public const int WM_USER_FE_STATE_CHANGE = 1028;
    public const int WM_USER_FE_APP_DISPLAYED = 1029;
    public const int WM_USER_FE_ORIENTATION_CHANGE = 1030;
    public const int WM_USER_FE_RESIZE = 1031;
    public const int WM_USER_INSTALL_COMPLETED = 1032;
    public const int WM_USER_UNINSTALL_COMPLETED = 1033;
    public const int WM_USER_APP_CRASHED = 1034;
    public const int WM_USER_EXE_CRASHED = 1035;
    public const int WM_USER_UPGRADE_FAILED = 1036;
    public const int WM_USER_BOOT_FAILURE = 1037;
    public const int WM_USER_FE_SHOOTMODE_STATE = 1038;
    public const int WM_USER_TOGGLE_FULLSCREEN = 1056;
    public const int WM_USER_GO_BACK = 1057;
    public const int WM_USER_SHOW_GUIDANCE = 1058;
    public const int WM_USER_AUDIO_MUTE = 1059;
    public const int WM_USER_AUDIO_UNMUTE = 1060;
    public const int WM_USER_AT_HOME = 1061;
    public const int WM_USER_ACTIVATE = 1062;
    public const int WM_USER_HIDE_WINDOW = 1063;
    public const int WM_USER_VMX_BIT_ON = 1064;
    public const int WM_USER_DEACTIVATE = 1065;
    public const int WM_USER_LOGS_REPORTING = 1072;
    public const int WM_NCHITTEST = 132;
    public const int WM_MOUSEMOVE = 512;
    public const int WM_MOUSEWHEEL = 522;
    public const int WM_RBUTTONDOWN = 516;
    public const int WM_RBUTTONUP = 517;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int WM_MBUTTONDOWN = 519;
    public const int WM_MBUTTONUP = 520;
    public const int WM_XBUTTONDOWN = 523;
    public const int WM_XBUTTONUP = 524;
    public const int WM_LBUTTONDBLCLK = 515;
    public const int WM_DISPLAYCHANGE = 126;
    public const int WM_INPUTLANGCHANGEREQUEST = 80;
    public const int WM_IME_ENDCOMPOSITION = 270;
    public const int WM_IME_COMPOSITION = 271;
    public const int WM_IME_CHAR = 646;
    public const int WM_CHAR = 258;
    public const int WM_IME_NOTIFY = 642;
    public const int WM_NCLBUTTONDOWN = 161;
    public const int HT_CAPTION = 2;
    public const int WM_IME_SETCONTEXT = 641;
    public const int WM_USER_TROUBLESHOOT_STUCK_AT_LOADING = 1088;
    public const int WM_USER_TROUBLESHOOT_BLACK_SCREEN = 1089;
    public const int WM_USER_TROUBLESHOOT_RPC = 1090;
    public const int WM_SYSKEYDOWN = 260;
    public const int WM_SYSCHAR = 262;
    public const int VK_MENU = 18;
    public const int VK_F10 = 121;
    public const int VK_SPACE = 32;
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOOLWINDOW = 128;
    public const int WS_EX_APPWINDOW = 262144;
    public const int WS_EX_TOPMOST = 8;
    public const int CHINESE_SIMPLIFIED_LANG_DECIMALVALUE = 2052;
    private const int GCS_COMPSTR = 8;
    public const int WM_COPYDATA = 74;
    public const int SC_KEYMENU = 61696;
    public const int SC_MAXIMIZE = 61488;
    public const int SC_MAXIMIZE2 = 61490;
    public const int SC_RESTORE = 61728;
    public const int SC_RESTORE2 = 61730;
    public const int WM_SYSCOMMAND = 274;
    public const int WM_ERASEBKGND = 20;
    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;
    public const int SWP_ASYNCWINDOWPOS = 16384;
    public const int SWP_DEFERERASE = 8192;
    public const int SWP_DRAWFRAME = 32;
    public const int SWP_FRAMECHANGED = 32;
    public const int SWP_HIDEWINDOW = 128;
    public const int SWP_NOACTIVATE = 16;
    public const int SWP_NOCOPYBITS = 256;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOOWNERZORDER = 512;
    public const int SWP_NOREDRAW = 8;
    public const int SWP_NOREPOSITION = 512;
    public const int SWP_NOSENDCHANGING = 1024;
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOZORDER = 4;
    public const int SWP_SHOWWINDOW = 64;
    public const int WS_OVERLAPPED = 0;
    public const int WS_CAPTION = 12582912;
    public const int WS_SYSMENU = 524288;
    public const int WS_THICKFRAME = 262144;
    public const int WS_MINIMIZEBOX = 131072;
    public const int WS_MAXIMIZEBOX = 65536;
    public const int WS_OVERLAPPEDWINDOW = 13565952;
    public const int WS_EX_TRANSPARENT = 32;
    private const int LOGPIXELSX = 88;
    public const int WM_SETREDRAW = 11;
    public const int SW_HIDE = 0;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWNORMAL = 1;
    public const int GWL_STYLE = -16;
    public const uint WS_POPUP = 2147483648;
    public const uint WS_CHILD = 1073741824;
    public const uint WS_DISABLED = 134217728;
    private const int KL_NAMELENGTH = 9;

    [DllImport("user32.dll")]
    public static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    public static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll")]
    public static extern int ToUnicodeEx(
      uint wVirtKey,
      uint wScanCode,
      byte[] lpKeyState,
      [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pwszBuff,
      int cchBuff,
      uint wFlags,
      IntPtr dwhkl);

    public static IntPtr GetHwnd(Popup popup)
    {
      return ((HwndSource) PresentationSource.FromVisual((Visual) popup?.Child)).Handle;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("imm32.dll")]
    public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

    [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr ImmGetContext(IntPtr hWnd);

    [DllImport("Imm32.dll")]
    public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

    [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
    private static extern int ImmGetCompositionStringW(
      IntPtr hIMC,
      int dwIndex,
      byte[] lpBuf,
      int dwBufLen);

    [DllImport("imm32.dll")]
    public static extern bool ImmSetCompositionWindow(IntPtr hIMC, out COMPOSITIONFORM lpptPos);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int which);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
      IntPtr hwnd,
      IntPtr hwndInsertAfter,
      int x,
      int y,
      int w,
      int h,
      uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool AdjustWindowRect(out RECT lpRect, int dwStyle, bool bMenu);

    [DllImport("user32.dll")]
    public static extern bool MoveWindow(
      IntPtr hWnd,
      int X,
      int Y,
      int nWidth,
      int nHeight,
      bool bRepaint);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateDC(
      string driver,
      string name,
      string output,
      IntPtr mode);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int index);

    public static int ScreenWidth
    {
      get
      {
        return InteropWindow.GetSystemMetrics(0);
      }
    }

    public static int ScreenHeight
    {
      get
      {
        return InteropWindow.GetSystemMetrics(1);
      }
    }

    [DllImport("user32.dll")]
    public static extern bool HideCaret(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(
      IntPtr hWnd,
      uint Msg,
      IntPtr wParam,
      ref COPYGAMEPADDATASTRUCT cds);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr FindWindow(string cls, string name);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr handle);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr handle, int cmd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetParent(IntPtr handle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(
      IntPtr hwndParent,
      IntPtr hwndChildAfter,
      string lpszClass,
      string lpszWindow);

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint ProcessId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool EnableWindow(IntPtr hwnd, bool enable);

    public static IntPtr MinimizeWindow(string name)
    {
      IntPtr window = InteropWindow.FindWindow((string) null, name);
      if (window == IntPtr.Zero)
        throw new SystemException("Cannot find window '" + name + "'", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      InteropWindow.ShowWindow(window, 6);
      return window;
    }

    public static IntPtr BringWindowToFront(
      string name,
      bool _,
      bool isRetoreMinimizedWindow = false)
    {
      IntPtr window = InteropWindow.FindWindow((string) null, name);
      if (window == IntPtr.Zero)
        throw new SystemException("Cannot find window '" + name + "'", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      if (!InteropWindow.SetForegroundWindow(window))
        throw new SystemException("Cannot set foreground window", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      if (isRetoreMinimizedWindow)
        InteropWindow.ShowWindow(window, 9);
      else
        InteropWindow.ShowWindow(window, 5);
      return window;
    }

    public static void RemoveWindowFromAltTabUI(IntPtr handle)
    {
      int windowLong = InteropWindow.GetWindowLong(handle, -20);
      InteropWindow.SetWindowLong(handle, -20, windowLong | 128);
    }

    public static IntPtr GetWindowHandle(string name)
    {
      IntPtr window = InteropWindow.FindWindow((string) null, name);
      if (!(window == IntPtr.Zero))
        return window;
      throw new SystemException("Cannot find window '" + name + "'", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }

    public static bool ForceSetForegroundWindow(IntPtr h)
    {
      if (h == IntPtr.Zero)
        return false;
      IntPtr foregroundWindow = InteropWindow.GetForegroundWindow();
      if (foregroundWindow == IntPtr.Zero)
        return InteropWindow.SetForegroundWindow(h);
      if (h == foregroundWindow)
        return true;
      uint ProcessId = 0;
      uint windowThreadProcessId = InteropWindow.GetWindowThreadProcessId(foregroundWindow, ref ProcessId);
      uint currentThreadId = InteropWindow.GetCurrentThreadId();
      if ((int) currentThreadId == (int) windowThreadProcessId)
        return InteropWindow.SetForegroundWindow(h);
      if (windowThreadProcessId != 0U)
      {
        if (!InteropWindow.AttachThreadInput(currentThreadId, windowThreadProcessId, true))
          return false;
        if (!InteropWindow.SetForegroundWindow(h))
        {
          InteropWindow.AttachThreadInput(currentThreadId, windowThreadProcessId, false);
          return false;
        }
        InteropWindow.AttachThreadInput(currentThreadId, windowThreadProcessId, false);
      }
      return true;
    }

    public static int GetScreenDpi()
    {
      IntPtr dc = InteropWindow.CreateDC("DISPLAY", (string) null, (string) null, IntPtr.Zero);
      if (dc == IntPtr.Zero)
        return -1;
      int num = InteropWindow.GetDeviceCaps(dc, 88);
      if (num == 0)
        num = 96;
      InteropWindow.DeleteDC(dc);
      return num;
    }

    public static void SetFullScreen(IntPtr hwnd)
    {
      InteropWindow.SetFullScreen(hwnd, 0, 0, InteropWindow.ScreenWidth, InteropWindow.ScreenHeight);
    }

    public static void SetFullScreen(IntPtr hwnd, int X, int Y, int cx, int cy)
    {
      if (!InteropWindow.SetWindowPos(hwnd, InteropWindow.HWND_TOP, X, Y, cx, cy, 64U))
        throw new SystemException("Cannot call SetWindowPos()", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }

    public static string CurrentCompStr(IntPtr handle)
    {
      int dwIndex = 8;
      IntPtr context = InteropWindow.ImmGetContext(handle);
      try
      {
        int compositionStringW = InteropWindow.ImmGetCompositionStringW(context, dwIndex, (byte[]) null, 0);
        if (compositionStringW <= 0)
          return string.Empty;
        byte[] numArray = new byte[compositionStringW];
        InteropWindow.ImmGetCompositionStringW(context, dwIndex, numArray, compositionStringW);
        return Encoding.Unicode.GetString(numArray);
      }
      finally
      {
        InteropWindow.ImmReleaseContext(handle, context);
      }
    }

    public static IntPtr GetWindowHandle(Window window)
    {
      HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) window);
      return hwndSource != null ? hwndSource.Handle : IntPtr.Zero;
    }

    public static bool IsWindowTopMost(IntPtr hWnd)
    {
      return (InteropWindow.GetWindowLong(hWnd, -20) & 8) == 8;
    }

    public static Window GetTopmostOwnerWindow(Window window)
    {
      while (window.Owner != null)
        window = window.Owner;
      return window;
    }

    public static int GetAForegroundApplicationProcessId()
    {
      IntPtr foregroundWindow = InteropWindow.GetForegroundWindow();
      if (foregroundWindow == IntPtr.Zero)
        return 0;
      uint ProcessId = 0;
      int windowThreadProcessId = (int) InteropWindow.GetWindowThreadProcessId(foregroundWindow, ref ProcessId);
      return (int) ProcessId;
    }

    [DllImport("user32.dll")]
    private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

    private static string GetLayoutCode()
    {
      StringBuilder pwszKLID = new StringBuilder(9);
      InteropWindow.GetKeyboardLayoutName(pwszKLID);
      return pwszKLID.ToString();
    }

    public static string MapLayoutName(string code = null)
    {
      if (code == null)
        code = InteropWindow.GetLayoutCode();
      string str;
      switch (code)
      {
        case "00000401":
          str = "Arabic (101)";
          break;
        case "00000402":
          str = "Bulgarian(typewriter)";
          break;
        case "00000404":
          str = "Chinese (traditional) - us keyboard";
          break;
        case "00000405":
          str = "Czech";
          break;
        case "00000406":
          str = "Danish";
          break;
        case "00000407":
          str = "German";
          break;
        case "00000408":
          str = "Greek";
          break;
        case "00000409":
          str = "United States";
          break;
        case "0000040A":
          str = "Spanish";
          break;
        case "0000040B":
          str = "Finnish";
          break;
        case "0000040C":
          str = "French";
          break;
        case "0000040D":
          str = "Hebrew";
          break;
        case "0000040E":
          str = "Hungarian";
          break;
        case "0000040F":
          str = "Icelandic";
          break;
        case "00000410":
          str = "Italian";
          break;
        case "00000411":
          str = "Japanese";
          break;
        case "00000412":
          str = "Korean";
          break;
        case "00000413":
          str = "Dutch";
          break;
        case "00000414":
          str = "Norwegian";
          break;
        case "00000415":
          str = "Polish (programmers)";
          break;
        case "00000416":
          str = "Portuguese (brazillian abnt)";
          break;
        case "00000418":
          str = "Romanian (legacy)";
          break;
        case "00000419":
          str = "Russian";
          break;
        case "0000041A":
          str = "Croatian";
          break;
        case "0000041B":
          str = "Slovak";
          break;
        case "0000041C":
          str = "Albanian";
          break;
        case "0000041D":
          str = "Swedish";
          break;
        case "0000041E":
          str = "Thai Kedmanee";
          break;
        case "0000041F":
          str = "Turkish Q";
          break;
        case "00000420":
          str = "Urdu";
          break;
        case "00000422":
          str = "Ukrainian";
          break;
        case "00000423":
          str = "Belarusian";
          break;
        case "00000424":
          str = "Slovenian";
          break;
        case "00000425":
          str = "Estonian";
          break;
        case "00000426":
          str = "Latvian";
          break;
        case "00000427":
          str = "Lithuanian ibm";
          break;
        case "00000428":
          str = "Tajik";
          break;
        case "00000429":
          str = "Persian";
          break;
        case "0000042A":
          str = "Vietnamese";
          break;
        case "0000042B":
          str = "Armenian eastern";
          break;
        case "0000042C":
          str = "Azeri Latin";
          break;
        case "0000042E":
          str = "Sorbian standard (legacy)";
          break;
        case "0000042F":
          str = "Macedonian (fyrom)";
          break;
        case "00000432":
          str = "Setswana";
          break;
        case "00000437":
          str = "Georgian";
          break;
        case "00000438":
          str = "Faeroese";
          break;
        case "00000439":
          str = "Deanagari - inscript";
          break;
        case "0000043A":
          str = "Maltese 47-key";
          break;
        case "0000043B":
          str = "Norwegian with sami";
          break;
        case "0000043F":
          str = "Kazakh";
          break;
        case "00000440":
          str = "Kyrgyz cyrillic";
          break;
        case "00000442":
          str = "Turkmen";
          break;
        case "00000444":
          str = "Tatar";
          break;
        case "00000445":
          str = "Bengali";
          break;
        case "00000446":
          str = "Punjabi";
          break;
        case "00000447":
          str = "Gujarati";
          break;
        case "00000448":
          str = "Oriya";
          break;
        case "00000449":
          str = "Tamil";
          break;
        case "0000044A":
          str = "Telugu";
          break;
        case "0000044B":
          str = "Kannada";
          break;
        case "0000044C":
          str = "Malayalam";
          break;
        case "0000044D":
          str = "Assamese - inscript";
          break;
        case "0000044E":
          str = "Marathi";
          break;
        case "00000450":
          str = "Mongolian cyrillic";
          break;
        case "00000451":
          str = "Tibetan (prc)";
          break;
        case "00000452":
          str = "United Kingdom Extended";
          break;
        case "00000453":
          str = "Khmer";
          break;
        case "00000454":
          str = "Lao";
          break;
        case "0000045A":
          str = "Syriac";
          break;
        case "0000045B":
          str = "Sinhala";
          break;
        case "00000461":
          str = "Nepali";
          break;
        case "00000463":
          str = "Pashto (afghanistan)";
          break;
        case "00000465":
          str = "Divehi phonetic";
          break;
        case "00000468":
          str = "Hausa";
          break;
        case "0000046A":
          str = "Yoruba";
          break;
        case "0000046C":
          str = "Sesotho sa Leboa";
          break;
        case "0000046D":
          str = "Bashkir";
          break;
        case "0000046E":
          str = "Luxembourgish";
          break;
        case "0000046F":
          str = "Greenlandic";
          break;
        case "00000470":
          str = "Igbo";
          break;
        case "00000480":
          str = "Uyghur (legacy)";
          break;
        case "00000481":
          str = "Maroi";
          break;
        case "00000485":
          str = "Yakut";
          break;
        case "00000488":
          str = "Wolof";
          break;
        case "00000804":
          str = "Chinese (simplified) -us keyboard";
          break;
        case "00000807":
          str = "Swiss german";
          break;
        case "00000809":
          str = "United Kingdom";
          break;
        case "0000080A":
          str = "Latin america";
          break;
        case "0000080C":
          str = "Belgian French";
          break;
        case "00000813":
          str = "Belgian (period)";
          break;
        case "00000816":
          str = "Portuguese";
          break;
        case "0000081A":
          str = "Serbian (latin)";
          break;
        case "0000082C":
          str = "Azeri Cyrillic";
          break;
        case "0000083B":
          str = "Swedish with sami";
          break;
        case "00000843":
          str = "Uzbek cyrillic";
          break;
        case "00000850":
          str = "Mongolian (mongolian script)";
          break;
        case "0000085D":
          str = "Inuktitut - latin";
          break;
        case "00000C04":
          str = "Chinese (traditional, hong kong s.a.r.) - us keyboard";
          break;
        case "00000C0C":
          str = "Canada French (legacy)";
          break;
        case "00000C1A":
          str = "Serbian (cyrillic)";
          break;
        case "00001004":
          str = "Chinese (simplified, singapore) - us keyboard";
          break;
        case "00001009":
          str = "Canada French";
          break;
        case "0000100C":
          str = "Swiss french";
          break;
        case "00001404":
          str = "Chinese (traditional, macao s.a.r.) - us keyboard";
          break;
        case "00001809":
          str = "Irish";
          break;
        case "0000201A":
          str = "Bosnian (cyrillic)";
          break;
        case "00004009":
          str = "United States - india";
          break;
        case "00010401":
          str = "Arabic (102)";
          break;
        case "00010402":
          str = "Bulgarian (latin)";
          break;
        case "00010405":
          str = "Czech (qwerty)";
          break;
        case "00010407":
          str = "German (ibm)";
          break;
        case "00010408":
          str = "Greek (220)";
          break;
        case "00010409":
          str = "United States - dvorak";
          break;
        case "0001040A":
          str = "Spanish variation";
          break;
        case "0001040E":
          str = "Hungarian 101 key";
          break;
        case "00010410":
          str = "Italian (142)";
          break;
        case "00010415":
          str = "Polish (214)";
          break;
        case "00010416":
          str = "Portuguese (brazillian abnt2)";
          break;
        case "00010418":
          str = "Romanian (standard)";
          break;
        case "00010419":
          str = "Russian (typewriter)";
          break;
        case "0001041B":
          str = "Slovak (qwerty)";
          break;
        case "0001041E":
          str = "Thai Pattachote";
          break;
        case "0001041F":
          str = "Turkish F";
          break;
        case "00010426":
          str = "Latvian (qwerty)";
          break;
        case "00010427":
          str = "Lithuanian";
          break;
        case "0001042B":
          str = "Armenian Western";
          break;
        case "0001042E":
          str = "Sorbian extended";
          break;
        case "0001042F":
          str = "Macedonian (fyrom) - standard";
          break;
        case "00010437":
          str = "Georgian (qwerty)";
          break;
        case "00010439":
          str = "Hindi traditional";
          break;
        case "0001043A":
          str = "Maltese 48-key";
          break;
        case "0001043B":
          str = "Sami extended norway";
          break;
        case "00010445":
          str = "Bengali - inscript (legacy)";
          break;
        case "0001045A":
          str = "Syriac phonetic";
          break;
        case "0001045B":
          str = "Sinhala -Wij 9";
          break;
        case "0001045D":
          str = "Inuktitut - naqittaut";
          break;
        case "00010465":
          str = "Divehi typewriter";
          break;
        case "00010480":
          str = "Uyghur";
          break;
        case "0001080C":
          str = "Belgian (comma)";
          break;
        case "0001083B":
          str = "Finnish with sami";
          break;
        case "00011009":
          str = "Canada Multilingual";
          break;
        case "00011809":
          str = "Gaelic";
          break;
        case "00020401":
          str = "Arabic (102) Azerty";
          break;
        case "00020402":
          str = "Bulgarian (phonetic)";
          break;
        case "00020405":
          str = "Czech programmers";
          break;
        case "00020408":
          str = "Greek (319)";
          break;
        case "00020409":
          str = "United States - international";
          break;
        case "00020418":
          str = "Romanian (programmers)";
          break;
        case "0002041E":
          str = "Thai Kedmanee (non-shiftlock)";
          break;
        case "00020422":
          str = "Ukrainian (enhanced)";
          break;
        case "00020427":
          str = "Lithuanian standard";
          break;
        case "0002042E":
          str = "Sorbian standard";
          break;
        case "00020437":
          str = "Georgian (ergonomic)";
          break;
        case "00020445":
          str = "Bengali - inscript";
          break;
        case "0002083B":
          str = "Sami extended finland-sweden";
          break;
        case "00030402":
          str = "Bulgarian";
          break;
        case "00030408":
          str = "Greek (220) latin";
          break;
        case "00030409":
          str = "United States - dvorak left hand";
          break;
        case "0003041E":
          str = "Thai Pattachote (non-shiftlock)";
          break;
        case "00040402":
          str = "Bulgarian (phonetic traditional)";
          break;
        case "00040408":
          str = "Greek (319) latin";
          break;
        case "00050408":
          str = "Greek latin";
          break;
        case "00050409":
          str = "United States - dvorak right hand";
          break;
        case "00060408":
          str = "Greek polyonic";
          break;
        default:
          str = code;
          break;
      }
      return str;
    }

    public static WindowState FindMainWindowState(Window window)
    {
      return window?.Owner == null ? window.WindowState : InteropWindow.FindMainWindowState(window.Owner);
    }

    [DllImport("User32.dll", CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr LoadCursorFromFile(string str);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

    [DllImport("user32.dll")]
    public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

    public static Cursor CreateCursor(
      Bitmap bmp,
      int xHotSpot,
      int yHotSpot,
      float scalingFactor)
    {
      int width = (int) (32.0 * (double) scalingFactor);
      int height = (int) (32.0 * (double) scalingFactor);
      using (Bitmap bitmap = new Bitmap((Image) bmp, width, height))
      {
        IntPtr hicon = bitmap.GetHicon();
        IconInfo icon = new IconInfo();
        ref IconInfo local = ref icon;
        InteropWindow.GetIconInfo(hicon, ref local);
        icon.xHotspot = (int) ((double) xHotSpot * (double) scalingFactor * ((double) width / (double) bmp?.Width.Value));
        icon.yHotspot = (int) ((double) yHotSpot * (double) scalingFactor * ((double) height / (double) bmp?.Height.Value));
        icon.fIcon = false;
        return new Cursor(InteropWindow.CreateIconIndirect(ref icon));
      }
    }

    public static Cursor LoadCustomCursor(string path)
    {
      IntPtr handle = InteropWindow.LoadCursorFromFile(path);
      if (handle == IntPtr.Zero)
        throw new Win32Exception();
      Cursor cursor = new Cursor(handle);
      typeof (Cursor).GetField("ownHandle", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((object) cursor, (object) true);
      return cursor;
    }
  }
}
