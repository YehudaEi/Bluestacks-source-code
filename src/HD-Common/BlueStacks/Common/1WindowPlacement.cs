// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.WindowPlacement
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
  public static class WindowPlacement
  {
    private static Encoding encoding = (Encoding) new UTF8Encoding();
    private static XmlSerializer serializer = new XmlSerializer(typeof (WINDOWPLACEMENT));
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private static RECT PlaceOnScreen(RECT monitorRect, RECT windowRect)
    {
      int num1 = monitorRect.Right - monitorRect.Left;
      int num2 = monitorRect.Bottom - monitorRect.Top;
      if (windowRect.Left < monitorRect.Left)
      {
        int num3 = windowRect.Right - windowRect.Left;
        if (num3 > num1)
          num3 = num1;
        windowRect.Left = monitorRect.Left;
        windowRect.Right = windowRect.Left + num3;
      }
      else if (windowRect.Right > monitorRect.Right)
      {
        int num3 = windowRect.Right - windowRect.Left;
        if (num3 > num1)
          num3 = num1;
        windowRect.Right = monitorRect.Right;
        windowRect.Left = windowRect.Right - num3;
      }
      if (windowRect.Top < monitorRect.Top)
      {
        int num3 = windowRect.Bottom - windowRect.Top;
        if (num3 > num2)
          num3 = num2;
        windowRect.Top = monitorRect.Top;
        windowRect.Bottom = windowRect.Top + num3;
      }
      else if (windowRect.Bottom > monitorRect.Bottom)
      {
        int num3 = windowRect.Bottom - windowRect.Top;
        if (num3 > num2)
          num3 = num2;
        windowRect.Bottom = monitorRect.Bottom;
        windowRect.Top = windowRect.Bottom - num3;
      }
      return windowRect;
    }

    private static RECT PlaceOnScreenIfEntirelyOutside(RECT monitorRect, RECT windowRect)
    {
      int num1 = monitorRect.Right - monitorRect.Left;
      int num2 = monitorRect.Bottom - monitorRect.Top;
      if (windowRect.Right < monitorRect.Left)
      {
        int num3 = windowRect.Right - windowRect.Left;
        if (num3 > num1)
          num3 = num1;
        windowRect.Left = monitorRect.Left;
        windowRect.Right = windowRect.Left + num3;
      }
      else if (windowRect.Left > monitorRect.Right)
      {
        int num3 = windowRect.Right - windowRect.Left;
        if (num3 > num1)
          num3 = num1;
        windowRect.Right = monitorRect.Right;
        windowRect.Left = windowRect.Right - num3;
      }
      if (windowRect.Bottom < monitorRect.Top)
      {
        int num3 = windowRect.Bottom - windowRect.Top;
        if (num3 > num2)
          num3 = num2;
        windowRect.Top = monitorRect.Top;
        windowRect.Bottom = windowRect.Top + num3;
      }
      else if (windowRect.Top > monitorRect.Bottom)
      {
        int num3 = windowRect.Bottom - windowRect.Top;
        if (num3 > num2)
          num3 = num2;
        windowRect.Bottom = monitorRect.Bottom;
        windowRect.Top = windowRect.Bottom - num3;
      }
      return windowRect;
    }

    private static bool RectangleEntirelyInside(RECT parent, RECT child)
    {
      return child.Left >= parent.Left && child.Right <= parent.Right && (child.Top <= parent.Top && child.Bottom >= parent.Bottom);
    }

    private static bool RectanglesIntersect(RECT a, RECT b)
    {
      return a.Left <= b.Right && a.Right >= b.Left && (a.Top <= b.Bottom && a.Bottom >= b.Top);
    }

    public static void SetPlacement(IntPtr windowHandle, RECT placementRect)
    {
      try
      {
        WINDOWPLACEMENT lpwndpl;
        using (XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(WindowPlacement.encoding.GetBytes(WindowPlacement.GetPlacement(windowHandle)))))
          lpwndpl = (WINDOWPLACEMENT) WindowPlacement.serializer.Deserialize(xmlReader);
        lpwndpl.length = Marshal.SizeOf(typeof (WINDOWPLACEMENT));
        lpwndpl.flags = 0;
        lpwndpl.showCmd = lpwndpl.showCmd == 2 ? 1 : lpwndpl.showCmd;
        IntPtr hMonitor = WindowPlacement.MonitorFromRect(ref placementRect, 2U);
        MONITORINFO monitorinfo = new MONITORINFO()
        {
          cbSize = Marshal.SizeOf(typeof (MONITORINFO))
        };
        ref MONITORINFO local = ref monitorinfo;
        if (WindowPlacement.GetMonitorInfo(hMonitor, ref local) && !WindowPlacement.RectangleEntirelyInside(monitorinfo.rcMonitor, placementRect))
          lpwndpl.normalPosition = WindowPlacement.PlaceOnScreen(monitorinfo.rcMonitor, placementRect);
        WindowPlacement.SetWindowPlacement(windowHandle, ref lpwndpl);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in SetPlacement. Exception: " + ex.ToString());
      }
    }

    public static System.Windows.Size GetMaxWidthAndHeightOfMonitor(IntPtr handle)
    {
      Screen screen = Screen.FromHandle(handle);
      Rectangle bounds = screen.Bounds;
      double width = (double) bounds.Width;
      bounds = screen.Bounds;
      double height = (double) bounds.Height;
      return new System.Windows.Size(width, height);
    }

    public static void SetPlacement(IntPtr windowHandle, string placementXml)
    {
      if (string.IsNullOrEmpty(placementXml))
        return;
      byte[] bytes = WindowPlacement.encoding.GetBytes(placementXml);
      try
      {
        WINDOWPLACEMENT lpwndpl;
        using (XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(bytes)))
          lpwndpl = (WINDOWPLACEMENT) WindowPlacement.serializer.Deserialize(xmlReader);
        lpwndpl.length = Marshal.SizeOf(typeof (WINDOWPLACEMENT));
        lpwndpl.flags = 0;
        lpwndpl.showCmd = lpwndpl.showCmd == 2 ? 1 : lpwndpl.showCmd;
        RECT normalPosition = lpwndpl.normalPosition;
        IntPtr hMonitor = WindowPlacement.MonitorFromRect(ref normalPosition, 2U);
        MONITORINFO monitorinfo = new MONITORINFO()
        {
          cbSize = Marshal.SizeOf(typeof (MONITORINFO))
        };
        ref MONITORINFO local = ref monitorinfo;
        if (WindowPlacement.GetMonitorInfo(hMonitor, ref local) && !WindowPlacement.RectangleEntirelyInside(monitorinfo.rcMonitor, lpwndpl.normalPosition))
          lpwndpl.normalPosition = WindowPlacement.PlaceOnScreen(monitorinfo.rcMonitor, lpwndpl.normalPosition);
        WindowPlacement.SetWindowPlacement(windowHandle, ref lpwndpl);
      }
      catch (InvalidOperationException ex)
      {
      }
    }

    public static string GetPlacement(IntPtr windowHandle)
    {
      WINDOWPLACEMENT lpwndpl = new WINDOWPLACEMENT();
      WindowPlacement.GetWindowPlacement(windowHandle, out lpwndpl);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter((Stream) memoryStream, Encoding.UTF8))
        {
          WindowPlacement.serializer.Serialize((XmlWriter) xmlTextWriter, (object) lpwndpl);
          byte[] array = memoryStream.ToArray();
          return WindowPlacement.encoding.GetString(array);
        }
      }
    }
  }
}
