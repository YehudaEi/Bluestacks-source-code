// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.CustomAlert
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.Agent
{
  internal class CustomAlert
  {
    private static int s_numAlerts = 0;
    public static Rectangle s_screenSize = Screen.PrimaryScreen.WorkingArea;
    private static string s_FontName = Utils.GetSystemFontName();

    private static Image ResizeImage(Image src)
    {
      Image image = (Image) new Bitmap(64, 64);
      Graphics graphics = Graphics.FromImage(image);
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphics.DrawImage(src, 0, 0, image.Width, image.Height);
      src.Dispose();
      return image;
    }

    private void AlertFormClosing(object sender, FormClosingEventArgs e)
    {
      --CustomAlert.s_numAlerts;
    }

    public static void ShowAlert(
      string imagePath,
      string title,
      string displayMsg,
      bool autoClose,
      MouseButtonEventHandler clickHandler,
      bool hideMute,
      string vmName,
      bool isCloudNotification,
      string package = "",
      string id = "0")
    {
      new Thread((ThreadStart) (() => NotificationWindow.Instance.AddAlert(imagePath, title, displayMsg, autoClose, 7000, clickHandler, hideMute, vmName, isCloudNotification, package, false, id, false)))
      {
        IsBackground = true
      }.Start();
    }

    public static void ShowCloudAnnouncement(
      string imagePath,
      string title,
      string displayMsg,
      bool autoClose,
      MouseButtonEventHandler clickHandler,
      string vmName)
    {
      CustomAlert.ShowAlert(imagePath, title, displayMsg, autoClose, clickHandler, true, vmName, true, "", "0");
    }
  }
}
