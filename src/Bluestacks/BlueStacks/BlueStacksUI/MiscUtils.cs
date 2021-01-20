// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MiscUtils
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BlueStacks.BlueStacksUI
{
  public static class MiscUtils
  {
    private const int TextBoxFoxusAttemts = 10;
    private static SerialWorkQueue sFocusWorker;

    private static SerialWorkQueue FocusWorker
    {
      get
      {
        if (MiscUtils.sFocusWorker == null)
        {
          MiscUtils.sFocusWorker = new SerialWorkQueue();
          MiscUtils.sFocusWorker.Start();
        }
        return MiscUtils.sFocusWorker;
      }
    }

    public static void SetFocusAsync(UIElement control, int delay = 0)
    {
      MiscUtils.FocusWorker.Enqueue((SerialWorkQueue.Work) (() =>
      {
        try
        {
          int i = 0;
          if (delay > 0)
            Thread.Sleep(delay);
          while (10 > i)
          {
            control.Dispatcher.Invoke((Delegate) (() =>
            {
              if (!control.Focus())
                return;
              i = 11;
            }));
            i++;
            Thread.Sleep(10);
          }
        }
        catch (Exception ex)
        {
          Logger.Info("Error setting focus on control" + ex.ToString());
        }
      }));
    }

    public static void GetWindowWidthAndHeight(out int width, out int height)
    {
      int width1 = Screen.PrimaryScreen.Bounds.Width;
      int height1 = Screen.PrimaryScreen.Bounds.Height;
      if (width1 > 2560 && height1 > 1440)
      {
        width = 2560;
        height = 1440;
      }
      else if (width1 > 1920 && height1 > 1080)
      {
        width = 1920;
        height = 1080;
      }
      else if (width1 > 1600 && height1 > 900)
      {
        width = 1600;
        height = 900;
      }
      else if (width1 > 1280 && height1 > 720)
      {
        width = 1280;
        height = 720;
      }
      else
      {
        width = 960;
        height = 540;
      }
    }

    private static bool IsParametersValid(Window window)
    {
      try
      {
        if (window.Left < 0.0 || window.Left > SystemParameters.VirtualScreenWidth || (window.Top < 0.0 || window.Top > SystemParameters.VirtualScreenHeight) || (SystemParameters.VirtualScreenWidth - window.Left < window.Width / 10.0 || SystemParameters.VirtualScreenHeight - window.Top < window.Height / 10.0))
          return false;
        int screenWidth = RegistryManager.Instance.ScreenWidth;
        int screenHeight = RegistryManager.Instance.ScreenHeight;
        if (Math.Abs((double) screenWidth - SystemParameters.VirtualScreenWidth) <= 100.0)
        {
          if (Math.Abs((double) screenHeight - SystemParameters.VirtualScreenHeight) <= 100.0)
            goto label_6;
        }
        return false;
      }
      catch (Exception ex)
      {
        Logger.Info("Exception calculating size" + ex.ToString());
        return false;
      }
label_6:
      return true;
    }

    private static void SaveControlSize(double width, double height, string prefix)
    {
      RegistryKey subKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
      subKey.SetValue(prefix + "Width", (object) width, RegistryValueKind.DWord);
      subKey.SetValue(prefix + "Height", (object) height, RegistryValueKind.DWord);
      subKey.Close();
    }

    public static void SetWindowSizeAndLocation(Window window, string prefix, bool isGMWindow = false)
    {
      if (window == null)
        return;
      try
      {
        double aspectRatio = 16.0 / 9.0;
        bool flag = true;
        RegistryKey subKey1 = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
        if ((int) subKey1.GetValue(prefix + "Width", (object) int.MinValue) != int.MinValue)
        {
          try
          {
            window.Width = (double) (int) subKey1.GetValue(prefix + "Width");
            window.Height = (double) (int) subKey1.GetValue(prefix + "Height");
            RegistryKey subKey2 = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
            window.Left = (double) (int) subKey2.GetValue(prefix + "Left");
            window.Top = (double) (int) subKey2.GetValue(prefix + "Top");
            flag = false;
            if (!MiscUtils.IsParametersValid(window))
              flag = true;
          }
          catch (Exception ex)
          {
            Logger.Info("Exception in geting value from reg" + ex.ToString());
            flag = true;
          }
        }
        if (!flag)
          return;
        double width;
        double height;
        double left;
        WpfUtils.GetDefaultSize(out width, out height, out left, aspectRatio, isGMWindow);
        double num1 = left + width;
        double num2 = (double) ((int) (SystemParameters.PrimaryScreenHeight - height) / 2);
        if (isGMWindow)
        {
          window.Left = left;
          window.Top = num2;
          window.Height = height;
          window.Width = width;
          MiscUtils.SaveControlSize(width, height, "DefaultGM");
        }
        else
        {
          window.Left = num1;
          window.Top = num2;
          window.Height = height;
          window.Width = (window.Height - 33.0) / 27.0 * 16.0;
          if (window.Left + window.Width <= SystemParameters.PrimaryScreenWidth)
            return;
          window.Left = SystemParameters.PrimaryScreenWidth - window.Width - 20.0;
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Exception getting size" + ex.ToString());
      }
    }

    public static int Extract7Zip(string zipFilePath, string extractDirectory)
    {
      string cmd = Path.Combine(RegistryStrings.InstallDir, "7zr.exe");
      if (!Directory.Exists(extractDirectory))
        Directory.CreateDirectory(extractDirectory);
      string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "x \"{0}\" -o\"{1}\" -aoa", (object) zipFilePath, (object) extractDirectory);
      return RunCommand.RunCmd(cmd, args, false, true, false, 0).ExitCode;
    }

    public static void GetStreamWidthAndHeight(
      int sWidth,
      int sHeight,
      out int width,
      out int height)
    {
      height = Utils.GetInt(RegistryManager.Instance.FrontendHeight, sHeight);
      width = Utils.GetInt(RegistryManager.Instance.FrontendWidth, sWidth);
    }
  }
}
