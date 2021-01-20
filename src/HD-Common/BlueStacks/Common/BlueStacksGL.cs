// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BlueStacksGL
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
  public static class BlueStacksGL
  {
    private static string BinaryName = "HD-GLCheck.exe";
    private static string BinaryPath = Path.Combine(Directory.GetCurrentDirectory(), BlueStacksGL.BinaryName);

    public static int GLCheckInstallation(
      GLRenderer rendererToCheck,
      GLMode modeToCheck,
      out string glVendor,
      out string glRenderer,
      out string glVersion)
    {
      Logger.Info("Checking for GLRenderer: {0}, GLMode: {1}", (object) rendererToCheck, (object) modeToCheck);
      string str1 = "";
      string str2 = "";
      string str3 = "";
      glVendor = str1;
      glRenderer = str2;
      glVersion = str3;
      return BlueStacksGL.Run(BlueStacksGL.BinaryPath, BlueStacksGL.GetArgs(rendererToCheck, modeToCheck), true, true, out glVendor, out glRenderer, out glVersion, 10000);
    }

    private static string GetArgs(GLRenderer rendererToCheck, GLMode modeToCheck)
    {
      return string.Format("{0} {1}", (object) (int) rendererToCheck, (object) (int) modeToCheck);
    }

    private static int Run(
      string prog,
      string args,
      bool logOutput,
      bool getGLValues,
      out string glVendor,
      out string glRenderer,
      out string glVersion,
      int timeout = 10000)
    {
      int num1 = -1;
      string vendor = "";
      string renderer = "";
      string version = "";
      glVendor = vendor;
      glRenderer = renderer;
      glVersion = version;
      try
      {
        using (Process process = new Process())
        {
          process.StartInfo.FileName = prog;
          process.StartInfo.Arguments = args;
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;
          if (getGLValues | logOutput)
          {
            if (logOutput && !getGLValues)
            {
              process.StartInfo.RedirectStandardOutput = true;
              process.OutputDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
              {
                try
                {
                  string str = outLine.Data != null ? outLine.Data : "";
                  if (!logOutput)
                    return;
                  Logger.Info("OUT: " + str);
                }
                catch (Exception ex)
                {
                  Logger.Error("A crash occured in the GLCheck delegate");
                  Logger.Error(ex.ToString());
                }
              });
            }
            else
            {
              process.StartInfo.RedirectStandardOutput = true;
              process.OutputDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
              {
                try
                {
                  string str = outLine.Data != null ? outLine.Data : "";
                  if (logOutput)
                    Logger.Info("OUT: " + str);
                  if (str.Contains("GL_VENDOR ="))
                  {
                    int num2 = str.IndexOf('=');
                    vendor = str.Substring(num2 + 1).Trim();
                    vendor = vendor.Replace(";", ";;");
                  }
                  if (str.Contains("GL_RENDERER ="))
                  {
                    int num2 = str.IndexOf('=');
                    renderer = str.Substring(num2 + 1).Trim();
                    renderer = renderer.Replace(";", ";;");
                  }
                  if (!str.Contains("GL_VERSION ="))
                    return;
                  int num3 = str.IndexOf('=');
                  version = str.Substring(num3 + 1).Trim();
                  version = version.Replace(";", ";;");
                }
                catch (Exception ex)
                {
                  Logger.Error("A crash occured in the GLCheck delegate");
                  Logger.Error(ex.ToString());
                }
              });
            }
          }
          Logger.Info("{0}: {1}", (object) prog, (object) args);
          process.Start();
          if (getGLValues | logOutput)
            process.BeginOutputReadLine();
          int num4 = process.WaitForExit(timeout) ? 1 : 0;
          glVendor = vendor;
          glRenderer = renderer;
          glVersion = version;
          if (num4 != 0)
          {
            Logger.Info(process.Id.ToString() + " EXIT: " + process.ExitCode.ToString());
            num1 = process.ExitCode;
          }
          else
            Logger.Fatal("Process {0} killed after timeout: {1}s", (object) process.StartInfo.FileName, (object) (timeout / 1000));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while running GLCheck. Ex: {0}", (object) ex);
      }
      return num1;
    }
  }
}
