// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Utils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using BlueStacks.Common.Interop;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BlueStacks.Common
{
  public static class Utils
  {
    private static bool sIsSyncAppJsonComplete = false;
    private static Dictionary<string, object> OemVmLockNamedata = new Dictionary<string, object>();
    public static readonly Dictionary<string, bool> sIsGuestBooted = new Dictionary<string, bool>();
    public static readonly Dictionary<string, bool> sIsGuestReady = new Dictionary<string, bool>();
    public static readonly Dictionary<string, bool> sIsSharedFolderMounted = new Dictionary<string, bool>();
    public static readonly List<string> sListIgnoredApps = new List<string>()
    {
      "tv.gamepop.home",
      "com.pop.store",
      "com.pop.store51",
      "com.bluestacks.s2p5105",
      "mpi.v23",
      "com.google.android.gms",
      "com.google.android.gsf.login",
      "com.android.deskclock",
      "me.onemobile.android",
      "me.onemobile.lite.android",
      "android.rk.RockVideoPlayer.RockVideoPlayer",
      "com.bluestacks.chartapp",
      "com.bluestacks.setupapp",
      "com.android.gallery3d",
      "com.bluestacks.keymappingtool",
      "com.baidu.appsearch",
      "com.bluestacks.s2p",
      "com.bluestacks.windowsfilemanager",
      "com.android.quicksearchbox",
      "com.bluestacks.setup",
      "com.bluestacks.appsettings",
      "mpi.v23",
      "com.bluestacks.setup",
      "com.bluestacks.gamepophome",
      "com.bluestacks.appfinder",
      "com.android.providers.downloads.ui"
    };
    private const int SM_TABLETPC = 86;
    public const int BTV_RIGHT_PANEL_WIDTH = 320;
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private const int PROC_KILL_TIMEOUT = 10000;
    private const int COMSERVER_EXIT_TIMEOUT = 5000;

    [DllImport("urlmon.dll", CharSet = CharSet.Auto)]
    private static extern uint FindMimeFromData(
      uint pBC,
      [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
      [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
      uint cbSize,
      [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
      uint dwMimeFlags,
      out uint ppwzMimeOut,
      uint dwReserverd);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint thread);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int smIndex);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetClassDevs(
      ref Guid lpGuid,
      IntPtr Enumerator,
      IntPtr hwndParent,
      ClassDevsFlags Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetClassDevs(
      IntPtr guid,
      IntPtr Enumerator,
      IntPtr hwndParent,
      ClassDevsFlags Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiEnumDeviceInfo(
      int DeviceInfoSet,
      int Index,
      ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiEnumDeviceInterfaces(
      int DeviceInfoSet,
      int DeviceInfoData,
      ref Guid lpHidGuid,
      int MemberIndex,
      ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetDeviceInterfaceDetail(
      int DeviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
      IntPtr aPtr,
      int detailSize,
      ref int requiredSize,
      IntPtr bPtr);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetDeviceInterfaceDetail(
      int DeviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
      ref PSP_DEVICE_INTERFACE_DETAIL_DATA myPSP_DEVICE_INTERFACE_DETAIL_DATA,
      int detailSize,
      ref int requiredSize,
      IntPtr bPtr);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetDeviceRegistryProperty(
      int DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      RegPropertyType Property,
      IntPtr PropertyRegDataType,
      IntPtr PropertyBuffer,
      int PropertyBufferSize,
      ref int RequiredSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiGetDeviceRegistryProperty(
      int DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      RegPropertyType Property,
      IntPtr PropertyRegDataType,
      ref DATA_BUFFER PropertyBuffer,
      int PropertyBufferSize,
      ref int RequiredSize);

    public static bool IsTargetForShortcut(string shortcutPath, string targetPath)
    {
      try
      {
        if (System.IO.File.Exists(shortcutPath))
        {
          string strA = ShortcutHelper.GetShortcutArguments(shortcutPath)?.ToLower(CultureInfo.InvariantCulture).Trim();
          targetPath = targetPath?.ToLower(CultureInfo.InvariantCulture).Trim();
          if (strA.Contains(targetPath))
          {
            if (string.Compare(strA, Path.Combine(RegistryStrings.InstallDir, targetPath), StringComparison.OrdinalIgnoreCase) == 0)
            {
              Logger.Info("{0} is a shortcut for target {1}", (object) shortcutPath, (object) targetPath);
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Ignoring exception while comparing TargetForShortcut: " + ex.Message);
      }
      return false;
    }

    public static bool IsShortcutArgumentContainsPackage(string shortcutPath, string packageName)
    {
      try
      {
        if (System.IO.File.Exists(shortcutPath))
        {
          ShellLink shellLink;
          ((IPersistFile) (shellLink = new ShellLink())).Load(shortcutPath, 0);
          StringBuilder pszArgs = new StringBuilder(1000);
          ((IShellLink) shellLink).GetArguments(pszArgs, pszArgs.Capacity);
          if (pszArgs.ToString().ToLower(CultureInfo.InvariantCulture).Contains(packageName?.ToLower(CultureInfo.InvariantCulture)))
            return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Ignoring exception " + ex.ToString());
      }
      return false;
    }

    public static void NotifyBootFailureToParentWindow(
      string className,
      string windowName,
      int exitCode,
      string vmName)
    {
      Logger.Info("Sending BOOT_FAILURE message to class = {0}, window = {1}", (object) className, (object) windowName);
      IntPtr window = InteropWindow.FindWindow(className, windowName);
      try
      {
        if (window == IntPtr.Zero)
        {
          Logger.Info("Unable to find window : {0}", (object) className);
        }
        else
        {
          uint num1;
          if (vmName == "Android")
          {
            num1 = 0U;
          }
          else
          {
            string s;
            if (vmName == null)
              s = (string) null;
            else
              s = vmName.Split('_')[1];
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            num1 = (uint) int.Parse(s, (IFormatProvider) invariantCulture);
          }
          uint num2 = (uint) exitCode;
          Logger.Info("Sending wparam : {0} and lparam : {1}", (object) num2, (object) num1);
          InteropWindow.SendMessage(window, 1037U, (IntPtr) (long) num2, (IntPtr) (long) num1);
          Logger.Info("Sent BOOT_FAILURE message");
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    public static bool IsDesktopPC()
    {
      return Utils.GetSystemMetrics(86) == 0;
    }

    public static bool CopyRecursive(string srcPath, string dstPath)
    {
      bool flag = true;
      try
      {
        Logger.Info("Copying {0} to {1}", (object) srcPath, (object) dstPath);
        if (!Directory.Exists(dstPath))
          Directory.CreateDirectory(dstPath);
        DirectoryInfo directoryInfo = new DirectoryInfo(srcPath);
        foreach (FileInfo file in directoryInfo.GetFiles())
          file.CopyTo(Path.Combine(dstPath, file.Name), true);
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
          if (!Utils.CopyRecursive(Path.Combine(srcPath, directory.Name), Path.Combine(dstPath, directory.Name)))
            flag = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Ignoring exception in copy recursive src:{0} dst:{1}, exception:{2}", (object) srcPath, (object) dstPath, (object) ex.Message);
        flag = false;
      }
      return flag;
    }

    public static bool IsNumberWithinRange(
      int number,
      int lowerLimit,
      int upperLimit,
      bool includeLowerLimit = true)
    {
      return includeLowerLimit ? number >= lowerLimit && number < upperLimit : number > lowerLimit && number < upperLimit;
    }

    public static int GetAndroidVMMemory(bool defaultInstance = true)
    {
      Logger.Info("Getting Android VM Memory");
      ulong num1 = 1048576;
      int num2 = 600;
      int lowerLimit = 3072;
      int num3 = 4096;
      int num4 = 5120;
      int num5 = 6144;
      int upperLimit = 8192;
      int num6;
      try
      {
        int number = (int) (SystemUtils.GetSystemTotalPhysicalMemory() / num1);
        Logger.Info("Total RAM = {0} MB", (object) number);
        num6 = !SystemUtils.IsOs64Bit() ? (number < lowerLimit || !defaultInstance ? num2 : 900) : (!defaultInstance ? (number <= 4000 ? 800 : 1024) : (number >= lowerLimit ? (!Utils.IsNumberWithinRange(number, lowerLimit, num3, true) ? (!Utils.IsNumberWithinRange(number, num3, num4, true) ? (!Utils.IsNumberWithinRange(number, num4, num5, true) ? (!Utils.IsNumberWithinRange(number, num5, upperLimit, true) ? 2048 : 1800) : 1500) : 1200) : 900) : num2));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to check physical memory. Err: " + ex.ToString());
        num6 = 1200;
      }
      Logger.Info("Using RAM: {0}MB", (object) num6);
      return num6;
    }

    public static void KillComServerSafe()
    {
      try
      {
        Logger.Info("KillComServerSafe()");
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher((ObjectQuery) new WqlObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'BstkSVC.exe'")))
        {
          if (managementObjectSearcher.Get().Count != 0)
          {
            Logger.Info("Found BstkSVC. Waiting for it to exit automatically...");
            Thread.Sleep(5000);
          }
          foreach (ManagementObject managementObject in managementObjectSearcher.Get())
          {
            Logger.Info("Considering " + managementObject["ProcessId"]?.ToString() + " -> " + managementObject["ExecutablePath"]?.ToString());
            Process processById = Process.GetProcessById((int) (uint) managementObject["ProcessId"]);
            string path1 = (string) managementObject["ExecutablePath"];
            if (!string.IsNullOrEmpty(path1))
            {
              string path2 = Directory.GetParent(path1).ToString();
              string installDir = RegistryStrings.InstallDir;
              if (string.Compare(Path.GetFullPath(installDir).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
              {
                Logger.Info("process BstkSVC not killed since the process Dir:{0} and Ignore Dir:{1} are same", (object) path2, (object) installDir);
                continue;
              }
            }
            Logger.Info("Trying to kill BstkSvc PID " + processById.Id.ToString());
            processById.Kill();
            if (!processById.WaitForExit(10000))
              Logger.Info("Timeout waiting for process to die");
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in KillComServerSafe :" + ex.ToString());
      }
    }

    public static bool CheckIfGuestReady(string vmName, int retries)
    {
      if (!Utils.sIsGuestReady.ContainsKey(vmName))
        Utils.sIsGuestReady.Add(vmName, false);
      if (!Utils.sIsGuestReady[vmName] && retries > 0)
      {
        while (retries > 0)
        {
          --retries;
          try
          {
            if (JObject.Parse(HTTPUtils.SendRequestToGuest("checkIfGuestReady", (Dictionary<string, string>) null, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"))["result"].ToString().Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
              Logger.Info("Guest is ready");
              Utils.sIsGuestReady[vmName] = true;
              return Utils.sIsGuestReady[vmName];
            }
            Thread.Sleep(1000);
          }
          catch (Exception ex)
          {
            Thread.Sleep(1000);
          }
        }
        Logger.Error("Guest is not ready now after all retries");
      }
      return Utils.sIsGuestReady[vmName];
    }

    public static List<string> GetRunningInstancesList()
    {
      List<string> stringList = new List<string>();
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if (Utils.IsAndroidPlayerRunning(vm, "bgp"))
          stringList.Add(vm);
      }
      return stringList;
    }

    private static bool CheckIfAndroidService(string serviceName)
    {
      return Regex.IsMatch(serviceName, "[bB]st[hH]d(Plus{1})?Android(_\\d+)?Svc");
    }

    public static string GetUserAgent(string oem = "bgp")
    {
      if (string.IsNullOrEmpty(oem))
        oem = "bgp";
      string s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2} gzip", (object) "BlueStacks", (object) "4.250.0.1070", (object) RegistryManager.RegistryManagers[oem].UserGuid);
      Logger.Debug("UserAgent = " + s);
      return Encoding.UTF8.GetString(Encoding.Default.GetBytes(s));
    }

    public static Process StartHiddenFrontend(string vmName, string oem = "bgp")
    {
      if (string.Equals(oem, "dmm", StringComparison.InvariantCultureIgnoreCase))
        return ProcessUtils.StartExe(RegistryManager.Instance.PartnerExePath, vmName + " -h", false);
      Process process;
      try
      {
        string str = Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "HD-Player.exe");
        process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = str;
        process.StartInfo.Arguments = vmName + " -h";
        Logger.Info("Sending vmName for vm calling {0}", (object) vmName);
        Logger.Info("Utils: Starting hidden Frontend");
        process.Start();
      }
      catch (Exception ex)
      {
        process = (Process) null;
        Logger.Error("Error starting process" + ex.ToString());
      }
      return process;
    }

    public static Process StartFrontend(string vmName)
    {
      string str = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
      Process process = new Process();
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = str;
      process.StartInfo.Arguments = "-vmname:" + vmName;
      Logger.Info("Utils: Starting Frontend");
      process.Start();
      return process;
    }

    public static string GetMD5HashFromFile(string fileName)
    {
      try
      {
        return new _MD5() { ValueAsFile = fileName }.FingerPrint.ToLower(CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating md5 hash: " + ex.ToString());
      }
      return string.Empty;
    }

    public static bool IsCheckSumValid(string md5Compare, string filePath)
    {
      Logger.Info("Checking for valid checksum");
      string md5HashFromFile = Utils.GetMD5HashFromFile(filePath);
      Logger.Info("Computed MD5: " + md5HashFromFile);
      return string.Equals(md5Compare, md5HashFromFile, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetSystemFontName()
    {
      try
      {
        using (new Font("Arial", 8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0))
          return "Arial";
      }
      catch (Exception ex1)
      {
        using (Label label = new Label())
        {
          try
          {
            using (new Font(label.Font.Name, 8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0))
              ;
          }
          catch (Exception ex2)
          {
            if (Oem.Instance.IsMessageBoxToBeDisplayed)
            {
              int num = (int) MessageBox.Show("Failed to load Font set.", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} instance failed.", (object) Strings.ProductDisplayName), MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            Environment.Exit(-1);
          }
          return label.Font.Name;
        }
      }
    }

    public static bool IsBlueStacksInstalled()
    {
      return !string.IsNullOrEmpty(RegistryManager.Instance.Version);
    }

    public static string GetLogoFile()
    {
      return Path.Combine(RegistryStrings.InstallDir, "ProductLogo.ico");
    }

    public static void AddUploadTextToImage(string inputImage, string outputImage)
    {
      Image image1 = Image.FromFile(inputImage);
      using (Bitmap bitmap = new Bitmap(image1.Width, image1.Height + 100))
      {
        Graphics graphics = Graphics.FromImage((Image) bitmap);
        graphics.DrawImage(image1, new Rectangle(0, 0, image1.Width, image1.Height), new Rectangle(0, 0, image1.Width, image1.Height), GraphicsUnit.Pixel);
        Image image2 = Image.FromFile(Utils.GetLogoFile());
        graphics.DrawImage(image2, new Rectangle(65, image1.Height, 40, 40), new Rectangle(80, 0, image2.Width, 40), GraphicsUnit.Pixel);
        using (SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.White))
        {
          float width = (float) image1.Width;
          float height = 80f;
          RectangleF layoutRectangle = new RectangleF(120f, (float) (image1.Height + 7), width, height);
          using (Pen pen = new Pen(System.Drawing.Color.Black))
          {
            graphics.DrawRectangle(pen, 120f, (float) image1.Height, width, height);
            string snapShotShareString = Oem.Instance.SnapShotShareString;
            using (Font font = new Font("Arial", 14f))
            {
              graphics.DrawString(snapShotShareString, font, (Brush) solidBrush, layoutRectangle);
              graphics.Save();
              image1.Dispose();
              bitmap.Save(outputImage, ImageFormat.Jpeg);
            }
          }
        }
      }
    }

    public static CmdRes RunCmd(string prog, string args, string outPath)
    {
      try
      {
        return Utils.RunCmdInternal(prog, args, outPath, true, false);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      return new CmdRes();
    }

    public static string RunCmdNoLog(string prog, string args, int timeout)
    {
      using (Process process = new Process())
      {
        CmdRes cmdRes = new CmdRes();
        string output = "";
        process.StartInfo.FileName = prog;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.OutputDataReceived += (DataReceivedEventHandler) ((obj, line) =>
        {
          string data = line.Data;
          string str;
          if (data == null || string.IsNullOrEmpty(str = data.Trim()))
            return;
          output = output + str + "\n";
        });
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit(timeout);
        return output;
      }
    }

    private static CmdRes RunCmdInternal(
      string prog,
      string args,
      string outPath,
      bool enableLog,
      bool append = false)
    {
      using (StreamWriter writer = string.IsNullOrEmpty(outPath) ? (StreamWriter) null : new StreamWriter(outPath, append))
      {
        using (Process proc = new Process())
        {
          Logger.Info("Running Command");
          Logger.Info("    prog: " + prog);
          Logger.Info("    args: " + args);
          Logger.Info("    out:  " + outPath);
          CmdRes res = new CmdRes();
          proc.StartInfo.FileName = prog;
          proc.StartInfo.Arguments = args;
          proc.StartInfo.UseShellExecute = false;
          proc.StartInfo.CreateNoWindow = true;
          proc.StartInfo.RedirectStandardInput = true;
          proc.StartInfo.RedirectStandardOutput = true;
          proc.StartInfo.RedirectStandardError = true;
          proc.OutputDataReceived += (DataReceivedEventHandler) ((obj, line) =>
          {
            if (outPath != null)
              writer.WriteLine(line.Data);
            string data = line.Data;
            string str;
            if (data == null || string.IsNullOrEmpty(str = data.Trim()))
              return;
            if (enableLog)
              Logger.Info(proc.Id.ToString() + " OUT: " + str);
            CmdRes cmdRes = res;
            cmdRes.StdOut = cmdRes.StdOut + str + "\n";
          });
          proc.ErrorDataReceived += (DataReceivedEventHandler) ((obj, line) =>
          {
            if (outPath != null)
              writer.WriteLine(line.Data);
            if (enableLog)
              Logger.Error(proc.Id.ToString() + " ERR: " + line.Data);
            CmdRes cmdRes = res;
            cmdRes.StdErr = cmdRes.StdErr + line.Data + "\n";
          });
          proc.Start();
          proc.BeginOutputReadLine();
          proc.BeginErrorReadLine();
          proc.WaitForExit();
          res.ExitCode = proc.ExitCode;
          if (enableLog)
          {
            int num = proc.Id;
            string str1 = num.ToString();
            num = proc.ExitCode;
            string str2 = num.ToString();
            Logger.Info(str1 + " ExitCode: " + str2);
          }
          if (outPath != null)
            writer.Close();
          return res;
        }
      }
    }

    public static void RunCmdAsync(string prog, string args)
    {
      try
      {
        Utils.RunCmdAsyncInternal(prog, args);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    private static void RunCmdAsyncInternal(string prog, string args)
    {
      Process process = new Process();
      Logger.Info("Running Command Async");
      Logger.Info("    prog: " + prog);
      Logger.Info("    args: " + args);
      process.StartInfo.FileName = prog;
      process.StartInfo.Arguments = args;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.Start();
    }

    public static string GetPartnerExecutablePath()
    {
      string str = RegistryManager.Instance.PartnerExePath;
      if (string.IsNullOrEmpty(str))
        str = Path.Combine(RegistryStrings.InstallDir, "BlueStacks.exe");
      return str;
    }

    public static Process StartPartnerExe(string vm = "Android")
    {
      Process process = new Process();
      process.StartInfo.FileName = Utils.GetPartnerExecutablePath();
      process.StartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-vmName={0}", (object) vm);
      process.StartInfo.UseShellExecute = false;
      process.Start();
      return process;
    }

    public static bool RestartBlueStacks()
    {
      int num = (int) MessageBox.Show(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Application restart required to use internet on {0}", (object) Strings.ProductDisplayName), Strings.ProductDisplayName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      return true;
    }

    public static void GetGuestWidthAndHeight(
      int sWidth,
      int sHeight,
      out int width,
      out int height)
    {
      if (Oem.Instance.OEM.Equals("yoozoo", StringComparison.OrdinalIgnoreCase))
      {
        width = 1280;
        height = 720;
      }
      else if (sWidth > 1920 && sHeight > 1080)
      {
        width = 1920;
        height = 1080;
      }
      else if (sWidth < 1280 && sHeight < 720)
      {
        width = 1280;
        height = 720;
      }
      else
      {
        width = sWidth;
        height = sHeight;
      }
    }

    public static void GetWindowWidthAndHeight(out int width, out int height)
    {
      int width1 = Screen.PrimaryScreen.Bounds.Width;
      int height1 = Screen.PrimaryScreen.Bounds.Height;
      if (width1 > 1920 && height1 > 1080)
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

    public static void AddMessagingSupport(out Dictionary<string, string[]> oemWindowMapper)
    {
      oemWindowMapper = new Dictionary<string, string[]>();
      if (string.IsNullOrEmpty(Oem.Instance.MsgWindowClassName) && string.IsNullOrEmpty(Oem.Instance.MsgWindowTitle))
        return;
      string[] strArray = new string[2]
      {
        Oem.Instance.MsgWindowClassName,
        Oem.Instance.MsgWindowTitle
      };
      oemWindowMapper.Add(Oem.Instance.OEM, strArray);
    }

    public static bool IsUIProcessAlive(string vmName, string oem = "bgp")
    {
      return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, oem)) || ProcessUtils.IsAlreadyRunning(Strings.GetBlueStacksUILockNameOem(oem));
    }

    public static bool IsAllUIProcessAlive(string vmName)
    {
      return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, "bgp")) && ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp");
    }

    public static bool IsAndroidPlayerRunning(string vmName, string oem = "bgp")
    {
      return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, oem));
    }

    public static bool IsFileNullOrMissing(string file)
    {
      if (!System.IO.File.Exists(file))
      {
        Logger.Info(file + " does not exist");
        return true;
      }
      if (new FileInfo(file).Length != 0L)
        return false;
      Logger.Info(file + " is null");
      return true;
    }

    public static string GetUserGUID()
    {
      string str1 = (string) null;
      string name = "Software\\\\BlueStacks";
      Logger.Info("Checking if guid present in HKCU");
      RegistryKey registryKey1;
      using (registryKey1 = Registry.CurrentUser.OpenSubKey(name))
      {
        if (registryKey1 != null)
        {
          str1 = (string) registryKey1.GetValue("USER_GUID", (object) null);
          if (str1 != null)
          {
            Logger.Info("Detected GUID in HKCU: " + str1);
            return str1;
          }
        }
      }
      Logger.Info("Checking if guid present in HKLM");
      RegistryKey registryKey2;
      using (registryKey2 = Registry.LocalMachine.OpenSubKey(name))
      {
        if (registryKey2 != null)
        {
          str1 = (string) registryKey2.GetValue("USER_GUID", (object) null);
          if (str1 != null)
          {
            Logger.Info("Detected User GUID in HKLM: " + str1);
            return str1;
          }
        }
      }
      try
      {
        Logger.Info("Checking if guid present in %temp%");
        string environmentVariable = Environment.GetEnvironmentVariable("TEMP");
        Logger.Info("%TEMP% = " + environmentVariable);
        string path = Path.Combine(environmentVariable, "Bst_Guid_Backup");
        if (System.IO.File.Exists(path))
        {
          string str2 = System.IO.File.ReadAllText(path);
          if (!string.IsNullOrEmpty(str2))
          {
            str1 = str2;
            Logger.Info("Detected User GUID %temp%: " + str1);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      return str1;
    }

    private static string GetOldPCode()
    {
      string path = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_PCode_Backup");
      string str = "";
      if (System.IO.File.Exists(path))
      {
        str = System.IO.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(str))
          Logger.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Old PCode = {0}", (object) str));
        try
        {
          System.IO.File.Delete(path);
        }
        catch (Exception ex)
        {
          Logger.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ignoring Error Occured, Err: {0}", (object) ex.ToString()));
        }
      }
      return str;
    }

    private static bool IsCACodeValid(string caCode)
    {
      string[] strArray = new string[8]
      {
        "4",
        "20",
        "5",
        "14",
        "8",
        "2",
        "9",
        "36"
      };
      foreach (string strA in strArray)
      {
        if (string.Compare(strA, caCode, StringComparison.OrdinalIgnoreCase) == 0)
          return false;
      }
      return true;
    }

    private static string GetOldCaCode()
    {
      string path = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_CaCode_Backup");
      string caCode = "";
      if (System.IO.File.Exists(path))
      {
        Logger.Info("the ca code temp file exists");
        caCode = System.IO.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(caCode))
          Logger.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Old CaCode = {0}", (object) caCode));
        try
        {
          System.IO.File.Delete(path);
        }
        catch (Exception ex)
        {
          Logger.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
        }
      }
      if (!Utils.IsCACodeValid(caCode))
        caCode = "";
      return caCode;
    }

    private static string GetOldCaSelector()
    {
      string path = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_CaSelector_Backup");
      string str = "";
      if (System.IO.File.Exists(path))
      {
        Logger.Info("the ca selector temp file exists");
        str = System.IO.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(str))
          Logger.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Old CaSelector = {0}", (object) str));
        try
        {
          System.IO.File.Delete(path);
        }
        catch (Exception ex)
        {
          Logger.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
        }
      }
      return str;
    }

    private static string GetRandomPCode()
    {
      string[] strArray = new string[8]
      {
        "madw",
        "mtox",
        "optr",
        "pxln",
        "ofpn",
        "snpe",
        "segn",
        "ptxg"
      };
      int index = new Random().Next(strArray.Length);
      return strArray[index];
    }

    public static JObject JSonResponseFromCloud(
      string locale,
      string vmName,
      string campaignHash,
      string guid)
    {
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "api/getcacode");
      if (string.IsNullOrEmpty(guid))
      {
        guid = RegistryManager.Instance.UserGuid;
        campaignHash = RegistryManager.Instance.CampaignMD5;
      }
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          nameof (locale),
          locale
        },
        {
          nameof (guid),
          guid
        },
        {
          "campaign_hash",
          campaignHash
        }
      };
      string json = "";
      try
      {
        json = BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while fetching info from cloud...Err : " + ex.ToString());
      }
      Logger.Info("Got resp: " + json);
      return JObject.Parse(json);
    }

    public static void GetCodesAndCountryInfo(
      out string code,
      out string pcode,
      out string country,
      out string caSelector,
      out string noChangesDroidG,
      out string pcodeFromCloud,
      out bool isCacodeValid,
      out string DNS,
      out string DNS2,
      out string abivalue,
      out string memAllocator,
      string locale,
      string upgradeDetected,
      string vmName,
      string campaignHash = "",
      string guid = "")
    {
      code = "";
      pcode = "";
      country = "";
      caSelector = "";
      abivalue = "15";
      memAllocator = string.Empty;
      noChangesDroidG = "";
      pcodeFromCloud = "";
      DNS = "";
      DNS2 = "";
      isCacodeValid = false;
      RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
      string str1 = (string) registryKey.GetValue("BstTestCA", (object) "");
      string str2 = (string) registryKey.GetValue("BstTestPCode", (object) "");
      string str3 = (string) registryKey.GetValue("BstTestCaSelector", (object) "");
      string str4 = (string) registryKey.GetValue("BstTestNoChangesDroidG", (object) "");
      if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
      {
        Logger.Info("Using test CA/P codes");
        code = str1;
        pcode = str2;
        caSelector = str3;
        noChangesDroidG = str4;
      }
      else
      {
        string oldPcode = Utils.GetOldPCode();
        string oldCaCode = Utils.GetOldCaCode();
        string oldCaSelector = Utils.GetOldCaSelector();
        if (!string.IsNullOrEmpty(oldCaCode))
        {
          Logger.Info("The CaCode taken from temp file");
          code = oldCaCode;
          pcode = oldPcode;
          caSelector = oldCaSelector;
          if (Oem.Instance.IsLoadCACodeFromCloud)
          {
            Logger.Info("noChangesDroidG requested from cloud");
            try
            {
              JObject jobject = Utils.JSonResponseFromCloud(locale, vmName, campaignHash, guid);
              if (jobject["success"].ToString().Trim() == "true")
              {
                if (jobject.ToString().Contains("p_code"))
                  pcodeFromCloud = jobject["p_code"].ToString().Trim();
                if (jobject.ToString().Contains("no_changes_droidg"))
                  noChangesDroidG = jobject["no_changes_droidg"].ToString().Trim();
                if (string.IsNullOrEmpty(caSelector) && string.IsNullOrEmpty(upgradeDetected) && jobject.ToString().Contains("ca_selector"))
                  caSelector = jobject["ca_selector"].ToString().Trim();
                if (jobject.ContainsKey("is_valid_code"))
                  isCacodeValid = jobject["is_valid_code"].ToObject<bool>();
                if (jobject.ContainsKey("dns"))
                  DNS = jobject["dns"].ToObject<string>();
                if (jobject.ContainsKey("dns2"))
                  DNS = jobject["dns2"].ToObject<string>();
                if (jobject.ContainsKey("abi_value"))
                  abivalue = jobject["abi_value"].ToObject<string>();
                if (jobject.ContainsKey("malloc_value"))
                  memAllocator = jobject["malloc_value"].ToObject<string>();
              }
            }
            catch (Exception ex)
            {
              Logger.Error(ex.Message);
            }
          }
        }
        else if (Oem.Instance.IsLoadCACodeFromCloud)
        {
          try
          {
            JObject jobject = Utils.JSonResponseFromCloud(locale, vmName, campaignHash, guid);
            if (jobject["success"].ToString().Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
              code = jobject[nameof (code)].ToString().Trim();
              if (jobject.ToString().Contains("p_code"))
                pcodeFromCloud = jobject["p_code"].ToString().Trim();
              pcode = !string.IsNullOrEmpty(upgradeDetected) ? "" : pcodeFromCloud;
              if (jobject.ToString().Contains("ca_selector"))
                caSelector = jobject["ca_selector"].ToString().Trim();
              if (jobject.ToString().Contains("no_changes_droidg"))
                noChangesDroidG = jobject["no_changes_droidg"].ToString().Trim();
              if (jobject.ContainsKey("is_valid_code"))
                isCacodeValid = jobject["is_valid_code"].ToObject<bool>();
              if (jobject.ContainsKey("abi_value"))
                abivalue = jobject["abi_value"].ToObject<string>();
              if (jobject.ContainsKey("malloc_value"))
                memAllocator = jobject["malloc_value"].ToObject<string>();
            }
            else
            {
              pcode = "ofpn";
              code = "840";
              caSelector = "se_310260";
              Logger.Info("Setting default pcode = {0} cacode = {1} caselector = {2} ", (object) pcode, (object) code, (object) caSelector);
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to get cacode, pcode etc from cloud");
            Logger.Error(ex.Message);
            pcode = "ofpn";
            code = "840";
            caSelector = "se_310260";
            Logger.Info("Setting default pcode = {0} cacode = {1} caselector = {2} ", (object) pcode, (object) code, (object) caSelector);
          }
        }
        else
        {
          pcode = !string.IsNullOrEmpty(upgradeDetected) ? "" : Utils.GetRandomPCode();
          code = "156";
          Logger.Info("cacode = {0} and pcode = {1}", (object) code, (object) pcode);
        }
      }
      if (Oem.Instance.IsCountryChina)
      {
        country = "CN";
        caSelector = "se_46000";
      }
      else
        country = Utils.GetUserCountry(vmName);
    }

    public static bool IsAndroidFeatureBitEnabled(uint featureBit, string vmName)
    {
      try
      {
        string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
        uint num = 0;
        char[] chArray1 = new char[1]{ ' ' };
        foreach (string str in bootParameters.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ '=' };
          string[] strArray = str.Split(chArray2);
          if (strArray[0] == "OEMFEATURES")
          {
            num = Convert.ToUInt32(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
            break;
          }
        }
        Logger.Info("the android oem feature bits are" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        if (((int) num & (int) featureBit) == 0)
          return false;
      }
      catch (Exception ex)
      {
        Logger.Error("Got error while checking for android bit, err:{0}", (object) ex.ToString());
        return false;
      }
      return true;
    }

    public static void SetImeSelectedInReg(string imeSelected, string vmName)
    {
      RegistryManager.Instance.Guest[vmName].ImeSelected = imeSelected;
    }

    public static bool IsLatinImeSelected(string vmName)
    {
      string imeSelected1 = RegistryManager.Instance.Guest[vmName].ImeSelected;
      if (imeSelected1.Equals("com.android.inputmethod.latin/.LatinIME", StringComparison.CurrentCultureIgnoreCase))
      {
        Logger.Info("LatinIme is selected");
        return true;
      }
      if (string.IsNullOrEmpty(imeSelected1))
      {
        try
        {
          Logger.Info("IME selected in registry is null, query currentImeId");
          string guest = HTTPUtils.SendRequestToGuest("getCurrentIMEID", (Dictionary<string, string>) null, vmName, 5000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          Logger.Debug("Response: {0}", (object) guest);
          string imeSelected2 = JObject.Parse(guest)["currentIme"].ToString();
          Logger.Info("The currentIme: {0}", (object) imeSelected2);
          if (imeSelected2.Equals("com.android.inputmethod.latin/.LatinIME", StringComparison.CurrentCultureIgnoreCase))
          {
            Utils.SetImeSelectedInReg(imeSelected2, vmName);
            return true;
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Got exception in checking CurrentImeSelected, ex : {0}", (object) ex.ToString());
        }
      }
      return false;
    }

    public static bool IsForcePcImeForLang(string locale)
    {
      if (locale == null || !locale.Equals("vi-VN", StringComparison.OrdinalIgnoreCase))
        return false;
      Logger.Info("the system locale is vi-vn, using pcime workflow");
      return true;
    }

    public static bool IsEastAsianLanguage(string lang)
    {
      return new List<string>()
      {
        "zh-CN",
        "ja-JP",
        "ko-KR"
      }.Contains(lang);
    }

    public static bool WaitForSyncConfig(string vmName)
    {
      int num = 240;
      while (num > 0)
      {
        --num;
        if (RegistryManager.Instance.Guest[vmName].ConfigSynced == 0)
        {
          Logger.Info("Config not sycned, wait 1 second and try again");
          Thread.Sleep(1000);
        }
        else
        {
          Logger.Info("Config is synced now");
          return true;
        }
      }
      return false;
    }

    public static bool WaitForFrontendPingResponse(string vmName)
    {
      Logger.Info("In method WaitForFrontendPingResponse for vmName: " + vmName);
      int num = 50;
      while (num > 0)
      {
        --num;
        try
        {
          string engine = HTTPUtils.SendRequestToEngine("pingVm", (Dictionary<string, string>) null, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
          Logger.Debug("Response: " + engine);
          if ((JArray.Parse(engine)[0] as JObject)["success"].ToObject<bool>())
          {
            Logger.Info("Frontend server running");
            return true;
          }
          Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
          Thread.Sleep(1000);
        }
      }
      Logger.Error("Frontend server not running after {0} retries", (object) num);
      return false;
    }

    public static bool WaitForAgentPingResponse(string vmName, string oem = "bgp")
    {
      Logger.Info("In WaitForAgentPingResponse");
      int num = 50;
      while (num > 0)
      {
        --num;
        try
        {
          if ((JArray.Parse(HTTPUtils.SendRequestToAgent("ping", (Dictionary<string, string>) null, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, oem, false))[0] as JObject)["success"].ToObject<bool>())
          {
            Logger.Info("Agent server running");
            return true;
          }
          Thread.Sleep(200);
        }
        catch (Exception ex)
        {
          Thread.Sleep(200);
          if (num <= 40)
          {
            if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lockbgp"))
              return false;
          }
        }
      }
      Logger.Info("Agent server not running after {0} retries", (object) num);
      return false;
    }

    public static bool WaitForBootComplete(string vmName, string oem = "bgp")
    {
      return Utils.WaitForBootComplete(vmName, 180, oem);
    }

    public static bool WaitForBootComplete(string vmName, int retries, string oem = "bgp")
    {
      if (!Utils.OemVmLockNamedata.ContainsKey(vmName + "_" + oem))
        Utils.OemVmLockNamedata.Add(vmName + "_" + oem, new object());
      if (!Utils.sIsGuestBooted.ContainsKey(vmName + "_" + oem))
        Utils.sIsGuestBooted.Add(vmName + "_" + oem, false);
      lock (Utils.OemVmLockNamedata[vmName + "_" + oem])
      {
        Logger.Info("Checking if guest booted or not for {0} retries", (object) retries);
        while (retries > 0)
        {
          --retries;
          if (Utils.IsGuestBooted(vmName, oem))
            return true;
          Thread.Sleep(1000);
        }
        Logger.Info("Guest not booted after {0} retries", (object) retries);
      }
      return false;
    }

    public static bool IsSharedFolderMounted(string vmName)
    {
      try
      {
        if (!Utils.sIsSharedFolderMounted.ContainsKey(vmName))
          Utils.sIsSharedFolderMounted.Add(vmName, false);
        if (!Utils.sIsSharedFolderMounted[vmName])
        {
          if (JObject.Parse(HTTPUtils.SendRequestToGuest("isSharedFolderMounted", (Dictionary<string, string>) null, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"))["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
          {
            Utils.sIsSharedFolderMounted[vmName] = true;
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Info("shared folder not mounted yet." + ex.Message);
      }
      return Utils.sIsSharedFolderMounted[vmName];
    }

    public static bool SetCustomAppSize(string vmName, string package, ScreenMode mode)
    {
      string json = "";
      try
      {
        json = HTTPUtils.SendRequestToGuest("setcustomappsize", new Dictionary<string, string>()
        {
          {
            "d",
            new JObject()
            {
              {
                "package_name",
                (JToken) package
              },
              {
                "screen_mode",
                (JToken) mode.ToString()
              }
            }.ToString(Formatting.None)
          }
        }, vmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        if (JObject.Parse(json)["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in sending setCustomAppSize to android response: " + json + " " + Environment.NewLine + " message: " + ex.Message);
      }
      return false;
    }

    public static void SendKeymappingFiledownloadRequest(string packageName, string vmName)
    {
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string str = "{\"pkgName\":\"" + packageName + "\"}";
        data.Add("action", "com.bluestacks.DOWNLOAD_KEY_MAPPING_SERVICE");
        data.Add("extras", str);
        Logger.Info("Sending request to android for downloading keymapping file for pkg " + packageName);
        HTTPUtils.SendRequestToGuest("customStartService", data, vmName, 0, (Dictionary<string, string>) null, false, 10, 500, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendKeymappingFiledownloadRequest: {0}", (object) ex.Message);
      }
    }

    public static bool IsGuestBooted(string vmName, string oem = "bgp")
    {
      try
      {
        if (!Utils.sIsGuestBooted.ContainsKey(vmName + "_" + oem))
          Utils.sIsGuestBooted.Add(vmName + "_" + oem, false);
        if (!Utils.sIsGuestBooted[vmName + "_" + oem])
        {
          if ((bool) JArray.Parse(HTTPUtils.SendRequestToEngine("checkIfGuestBooted", (Dictionary<string, string>) null, vmName, 100, (Dictionary<string, string>) null, false, 1, 0, "", oem))[0][(object) "success"])
          {
            Utils.sIsGuestBooted[vmName + "_" + oem] = true;
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Guest not booted yet." + ex.Message);
      }
      return Utils.sIsGuestBooted[vmName + "_" + oem];
    }

    public static void ExtractImages(string targetDir, string resourceName)
    {
      try
      {
        Directory.Delete(targetDir, true);
      }
      catch (Exception ex)
      {
      }
      if (!Directory.Exists(targetDir))
        Directory.CreateDirectory(targetDir);
      ResourceManager resourceManager;
      try
      {
        resourceManager = new ResourceManager(resourceName, Assembly.GetExecutingAssembly());
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to extract resources. err: " + ex.ToString());
        return;
      }
      ((Image) resourceManager.GetObject("bg", CultureInfo.InvariantCulture)).Save(Path.Combine(targetDir, "bg.jpg"), ImageFormat.Jpeg);
      bool flag = true;
      try
      {
        ((Image) resourceManager.GetObject("HomeScreen", CultureInfo.InvariantCulture)).Save(Path.Combine(targetDir, "HomeScreen.jpg"), ImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
        flag = false;
      }
      try
      {
        ((Image) resourceManager.GetObject("ThankYouImage", CultureInfo.InvariantCulture)).Save(Path.Combine(targetDir, "ThankYouImage.jpg"), ImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
      }
      int num = 0;
      try
      {
        while (true)
        {
          Image image;
          do
          {
            ++num;
            image = (Image) resourceManager.GetObject("SetupImage" + Convert.ToString(num, (IFormatProvider) CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            image.Save(Path.Combine(targetDir, "SetupImage" + Convert.ToString(num, (IFormatProvider) CultureInfo.InvariantCulture) + ".jpg"), ImageFormat.Jpeg);
          }
          while (flag || num != 1);
          image.Save(Path.Combine(targetDir, "HomeScreen.jpg"), ImageFormat.Jpeg);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static string DownloadIcon(string package, string directory = "", bool isReDownload = false)
    {
      return Utils.TinyDownloader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://cloud.bluestacks.com/app/icon?pkg={0}&fallback=false", (object) package), package + ".png", directory, isReDownload);
    }

    public static string TinyDownloader(
      string url,
      string fileNameWithExtension,
      string directory = "",
      bool isReDownload = false)
    {
      string str = string.Empty;
      try
      {
        if (!string.IsNullOrEmpty(url))
        {
          if (!string.IsNullOrEmpty(fileNameWithExtension))
          {
            string path2 = Regex.Replace(fileNameWithExtension, "[\\x22\\\\\\/:*?|<>]", " ");
            if (string.IsNullOrEmpty(directory))
              directory = RegistryStrings.GadgetDir;
            str = Path.Combine(directory, path2);
            if (!Directory.Exists(Directory.GetParent(str).FullName))
              Directory.CreateDirectory(Directory.GetParent(str).FullName);
            if (!System.IO.File.Exists(str) | isReDownload)
            {
              using (WebClient webClient = new WebClient())
                webClient.DownloadFile(url, str);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot download icon file" + ex.ToString());
      }
      return str;
    }

    public static string GetDNS2Value(string oem)
    {
      string str = "8.8.8.8";
      if (string.Compare(oem, "tc_dt", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "china", StringComparison.OrdinalIgnoreCase) == 0 || (string.Compare(oem, "china_api", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "ucweb_dt", StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(oem, "4399", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "anquicafe", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "yy_dt", StringComparison.OrdinalIgnoreCase) == 0))
        str = "114.114.114.114";
      return str;
    }

    public static bool IsInstallOrUpgradeRequired()
    {
      if (!Utils.IsBlueStacksInstalled())
        return true;
      string version1 = RegistryManager.Instance.Version;
      if (string.IsNullOrEmpty(version1))
        return true;
      string version2 = version1.Substring(0, version1.LastIndexOf('.')) + ".0";
      string version3 = "4.250.0.1070".Substring(0, "4.250.0.1070".LastIndexOf('.')) + ".0";
      System.Version version4 = new System.Version(version2);
      System.Version version5 = new System.Version(version3);
      Logger.Info("Installed Version: {0}, new version: {1}", (object) version1, (object) "4.250.0.1070");
      if (!(version5 > version4))
        return false;
      Logger.Info("IMP: lower version: {0} is already installed. Forcing upgrade.", (object) version1);
      return true;
    }

    public static void SendBrowserVersionStats(string version, string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          string userGuid = RegistryManager.Instance.UserGuid;
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "ie_ver",
              version
            },
            {
              "guid",
              userGuid
            },
            {
              "prod_ver",
              "4.250.0.1070"
            }
          };
          Logger.Info("Sending browser version Stats");
          Logger.Info("Got browser version stat response: {0}", (object) BstHttpClient.Post("https://bluestacks-cloud.appspot.com/stats/ieversionstats", data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send app stats. error: " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static bool IsRemoteFilePresent(string url)
    {
      bool flag = true;
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.Method = "Head";
      try
      {
        HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse();
        if (response.StatusCode == HttpStatusCode.NotFound)
          flag = false;
        response.Close();
      }
      catch (Exception ex)
      {
        flag = false;
        Logger.Error("Could not make http request: " + ex.ToString());
      }
      return flag;
    }

    public static string ConvertToIco(string imagePath, string iconsDir)
    {
      Logger.Info("Converting {0}", (object) imagePath);
      string fileName = Path.GetFileName(imagePath);
      string path2 = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)) + ".ico";
      string outputPath = Path.Combine(iconsDir, path2);
      IconHelper.ConvertToIcon(imagePath, outputPath, 256, false);
      return outputPath;
    }

    public static void ResizeImage(string imagePath)
    {
      bool flag = false;
      using (FileStream fileStream = System.IO.File.OpenRead(imagePath))
      {
        using (Image image1 = Image.FromStream((Stream) fileStream))
        {
          int width = image1.Width;
          int height = image1.Height;
          if (width >= 256)
          {
            int num = 256;
            height = (int) ((double) height / ((double) width / (double) num));
            width = num;
            flag = true;
          }
          if (height >= 256)
          {
            int num = 256;
            width = (int) ((double) width / ((double) height / (double) num));
            height = num;
            flag = true;
          }
          if (width % 8 != 0)
          {
            width -= width % 8;
            flag = true;
          }
          if (height % 8 != 0)
          {
            height -= height % 8;
            flag = true;
          }
          if (!flag)
            return;
          using (Image image2 = (Image) new Bitmap(width, height))
          {
            Graphics graphics = Graphics.FromImage(image2);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image1, 0, 0, image2.Width, image2.Height);
            System.IO.File.Delete(imagePath);
            image2.Save(imagePath);
          }
        }
      }
    }

    public static int GetSystemHeight()
    {
      return Utils.GetSystemMetrics(1);
    }

    public static int GetSystemWidth()
    {
      return Utils.GetSystemMetrics(0);
    }

    public static int GetBstCommandProcessorPort(string vmName)
    {
      return RegistryManager.Instance.Guest[vmName].BstAndroidPort;
    }

    public static bool IsHomeApp(string appInfo)
    {
      return (appInfo != null ? (appInfo.IndexOf("com.bluestacks.appmart", StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0) : 1) != 0 || (appInfo != null ? (appInfo.IndexOf("com.android.launcher2", StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0) : 1) != 0 || ((appInfo != null ? (appInfo.IndexOf("com.uncube.launcher", StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0) : 1) != 0 || (appInfo != null ? (appInfo.IndexOf("com.bluestacks.gamepophome", StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0) : 1) != 0);
    }

    public static bool IsValidEmail(string email)
    {
      return new Regex("^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$").IsMatch(email);
    }

    public static string GetFileURI(string path)
    {
      return new Uri(path).AbsoluteUri;
    }

    public static string PostToBstCmdProcessorAfterServiceStart(
      string path,
      Dictionary<string, string> data,
      string vmName,
      bool isLaunchUI = true)
    {
      string str = (string) null;
      Stats.DMMEvent dmmEvent;
      if (!Utils.IsAllUIProcessAlive(vmName) & isLaunchUI)
      {
        Logger.Info("Starting Frontend in hidden mode.");
        Utils.StartHiddenFrontend(vmName, "bgp");
        dmmEvent = Stats.DMMEvent.client_launched;
        Stats.SendMiscellaneousStatsAsyncForDMM(dmmEvent.ToString(), path, (string) null, (string) null, (string) null, "Android", 0);
      }
      int retries = 300;
      if (!Utils.CheckIfGuestReady(vmName, retries))
      {
        dmmEvent = Stats.DMMEvent.boot_failed;
        Stats.SendMiscellaneousStatsAsyncForDMM(dmmEvent.ToString(), "checkIfGuestReady", (string) null, (string) null, (string) null, "Android", 0);
        return new JObject()
        {
          {
            "result",
            (JToken) "error"
          },
          {
            "reason",
            (JToken) "Guest boot failed"
          }
        }.ToString(Formatting.None);
      }
      dmmEvent = Stats.DMMEvent.boot_success;
      Stats.SendMiscellaneousStatsAsyncForDMM(dmmEvent.ToString(), "checkIfGuestReady", (string) null, (string) null, (string) null, "Android", 0);
      try
      {
        str = HTTPUtils.SendRequestToGuest(path, data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in PostAfterServiceStart");
        Logger.Error(ex.Message);
      }
      return str;
    }

    public static string GetToBstCmdProcessorAfterServiceStart(string path, string vmName)
    {
      string str = (string) null;
      if (!Utils.IsUIProcessAlive(vmName, "bgp"))
      {
        Logger.Info("Starting Frontend in hidden mode.");
        using (Process process = Utils.StartHiddenFrontend(vmName, "bgp"))
          process.WaitForExit(60000);
      }
      try
      {
        str = HTTPUtils.SendRequestToGuest(path, (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetToBstCmdProcessorAfterServiceStart");
        Logger.Error(ex.Message);
      }
      return str;
    }

    public static bool IsAppInstalled(string package, string vmName, out string version)
    {
      version = "";
      return Utils.IsAppInstalled(package, vmName, out version, out string _, true);
    }

    public static bool IsAppInstalled(
      string package,
      string vmName,
      out string version,
      out string failReason,
      bool isLaunchUI = true)
    {
      Logger.Info("Utils: IsAppInstalled Called for package {0}", (object) package);
      version = "";
      failReason = "App not installed";
      bool flag = false;
      try
      {
        string afterServiceStart = Utils.PostToBstCmdProcessorAfterServiceStart("isPackageInstalled", new Dictionary<string, string>()
        {
          {
            nameof (package),
            package
          }
        }, vmName, isLaunchUI);
        Logger.Info("Got response: {0}", (object) afterServiceStart);
        if (string.IsNullOrEmpty(afterServiceStart))
        {
          failReason = "The Api failed to get a response";
        }
        else
        {
          JObject jobject = JObject.Parse(afterServiceStart);
          string strA = jobject["result"].ToString().Trim();
          if (string.Compare(strA, "ok", StringComparison.OrdinalIgnoreCase) == 0)
          {
            flag = true;
            version = jobject[nameof (version)].ToString().Trim();
            Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.is_app_installed.ToString(), "success", package, version, (string) null, "Android", 0);
          }
          else if (string.Compare(strA, "error", StringComparison.OrdinalIgnoreCase) == 0)
          {
            failReason = jobject["reason"].ToString().Trim();
            Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.is_app_installed.ToString(), "failed", package, failReason, (string) null, "Android", 0);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
        failReason = ex.Message;
      }
      Logger.Info("Installed = {0}", (object) flag);
      return flag;
    }

    private static string FilterSystemApps(JArray packages, out bool isSamsungStorePresent)
    {
      isSamsungStorePresent = false;
      JArray jarray = new JArray();
      foreach (JObject child in packages.Children<JObject>())
      {
        if (child["package"].ToString().Trim().Equals("com.sec.android.app.samsungapps", StringComparison.Ordinal))
          isSamsungStorePresent = true;
        if (string.Compare(child["systemapp"].ToString().Trim(), "0", StringComparison.OrdinalIgnoreCase) == 0)
        {
          bool flag = true;
          for (int index = 0; index < Utils.sListIgnoredApps.Count; ++index)
          {
            if (string.Compare(child["package"].ToString().Trim(), Utils.sListIgnoredApps[index], StringComparison.OrdinalIgnoreCase) == 0)
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            JObject jobject = new JObject()
            {
              {
                "package",
                (JToken) child["package"].ToString().Trim()
              },
              {
                "version",
                (JToken) child["version"].ToString().Trim()
              },
              {
                "appname",
                (JToken) child["appname"].ToString().Trim()
              },
              {
                "gl3required",
                (JToken) child["gl3required"].ToString().Trim()
              }
            };
            jarray.Add((JToken) jobject);
          }
        }
      }
      return jarray.ToString(Formatting.None);
    }

    public static string GetInstalledPackages(
      string vmName,
      out string failReason,
      out bool isSamsungStorePresent,
      int count = 0)
    {
      Logger.Info("Utils: GetInstalledPackages Called for VM: {0}", (object) vmName);
      failReason = "Unable to get list of installed apps";
      isSamsungStorePresent = false;
      string str = "";
      try
      {
        string afterServiceStart = Utils.GetToBstCmdProcessorAfterServiceStart("installedPackages", vmName);
        Logger.Info("Got response: {0}", (object) afterServiceStart);
        if (string.IsNullOrEmpty(afterServiceStart))
        {
          failReason = "The Api failed to get a response";
        }
        else
        {
          JObject jobject = JObject.Parse(afterServiceStart);
          string strA = jobject["result"].ToString().Trim();
          if (string.Compare(strA, "ok", StringComparison.OrdinalIgnoreCase) == 0)
          {
            failReason = "";
            str = Utils.FilterSystemApps(jobject["installed_packages"] as JArray, out isSamsungStorePresent);
            Logger.Info("Filtered results: {0}", (object) str);
          }
          else if (string.Compare(strA, "error", StringComparison.OrdinalIgnoreCase) == 0)
          {
            failReason = jobject["reason"].ToString().Trim();
            if (string.Compare(failReason, "system not ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
              if (count < 6)
              {
                Thread.Sleep(500);
                return Utils.GetInstalledPackages(vmName, out failReason, out isSamsungStorePresent, count++);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occurred, Err: {0}", (object) ex.ToString()));
        failReason = ex.Message;
      }
      return str;
    }

    public static string GetInstalledPackagesFromAppsJSon(string vmName)
    {
      try
      {
        return string.Join(",", JsonParser.GetInstalledAppsList(vmName).ToArray());
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't get installed app list. Ex: {0}", (object) ex);
      }
      return string.Empty;
    }

    public static AppInfo GetPackageDetails(
      string vmName,
      string package,
      bool videoPresent,
      out string failReason)
    {
      AppInfo appInfo = (AppInfo) null;
      try
      {
        string afterServiceStart = Utils.PostToBstCmdProcessorAfterServiceStart("getPackageDetails", new Dictionary<string, string>()
        {
          {
            nameof (package),
            package
          }
        }, vmName, true);
        if (string.IsNullOrEmpty(afterServiceStart))
        {
          failReason = "The api failed to get a response";
        }
        else
        {
          JObject jobject = JObject.Parse(afterServiceStart);
          if (string.Compare(jobject["result"].ToString().Trim(), "ok", StringComparison.OrdinalIgnoreCase) == 0)
          {
            failReason = "";
            JArray jarray = JArray.Parse(jobject["activities"].ToString());
            appInfo = new AppInfo(jobject["name"].ToString().Trim(), jarray[0][(object) "img"].ToString().Trim(), jobject[nameof (package)].ToString().Trim(), (jarray[0] as JObject)["activity"].ToString().Trim(), "0", "no", jobject["version"].ToString().Trim(), jobject["gl3required"].ToObject<bool>(), videoPresent, jobject["versionName"].ToString().Trim(), false);
          }
          else
            failReason = "The api failed to get a response";
        }
      }
      catch (Exception ex)
      {
        failReason = ex.Message;
      }
      return appInfo;
    }

    public static void SyncAppJson(string vmName)
    {
      Logger.Info("In SyncAppJson");
      if (Utils.sIsSyncAppJsonComplete)
        return;
      try
      {
        string failReason;
        bool isSamsungStorePresent;
        string installedPackages = Utils.GetInstalledPackages(vmName, out failReason, out isSamsungStorePresent, 0);
        if (isSamsungStorePresent && !RegistryManager.Instance.IsSamsungStorePresent)
        {
          RegistryManager.Instance.IsSamsungStorePresent = true;
          HTTPUtils.SendRequestToClient("reloadPromotions", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        if (!string.IsNullOrEmpty(failReason))
          return;
        JArray jarray = JArray.Parse(installedPackages);
        JsonParser jsonParser = new JsonParser(vmName);
        AppInfo[] appList = jsonParser.GetAppList();
        List<AppInfo> source = ((System.Collections.Generic.IEnumerable<AppInfo>) appList).ToList<AppInfo>();
        bool flag1 = false;
        bool videoPresent = false;
        foreach (JObject child in jarray.Children<JObject>())
        {
          JObject installedAppsJsonObj = child;
          string package = installedAppsJsonObj["package"].ToString().Trim();
          if (jsonParser.GetAppInfoFromPackageName(package) != null)
          {
            if (!string.Equals(JsonParser.GetGl3RequirementFromPackage(appList, package).ToString((IFormatProvider) CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture).Trim(), installedAppsJsonObj["gl3required"].ToString().ToLower(CultureInfo.InvariantCulture).Trim(), StringComparison.OrdinalIgnoreCase))
            {
              flag1 = true;
              source.Where<AppInfo>((Func<AppInfo, bool>) (x => x.Package == package)).FirstOrDefault<AppInfo>().Gl3Required = installedAppsJsonObj["gl3required"].ToObject<bool>();
            }
            videoPresent = JsonParser.GetVideoPresentRequirementFromPackage(appList, package);
            AppInfo appInfo = source.Where<AppInfo>((Func<AppInfo, bool>) (x => x.Package == package)).FirstOrDefault<AppInfo>();
            try
            {
              appInfo.VideoPresent = installedAppsJsonObj["videopresent"].ToObject<bool>();
            }
            catch
            {
            }
          }
          if (!((System.Collections.Generic.IEnumerable<AppInfo>) appList).Any<AppInfo>((Func<AppInfo, bool>) (_ => string.Compare(_.Package.Trim(), installedAppsJsonObj["package"].ToString().Trim(), StringComparison.OrdinalIgnoreCase) == 0)))
          {
            flag1 = true;
            AppInfo packageDetails = Utils.GetPackageDetails(vmName, installedAppsJsonObj["package"].ToString().Trim(), videoPresent, out failReason);
            if (packageDetails != null)
              source.Add(packageDetails);
          }
        }
        if (jarray.Count != source.Count | flag1)
        {
          List<string> stringList = new List<string>();
          foreach (AppInfo appInfo1 in source)
          {
            AppInfo appInfo = appInfo1;
            if (!jarray.Children<JObject>().Any<JObject>((Func<JObject, bool>) (_ => string.Compare(_["package"].ToString().Trim(), appInfo.Package.Trim(), StringComparison.OrdinalIgnoreCase) == 0)))
            {
              stringList.Add(appInfo.Package);
              flag1 = true;
            }
          }
          foreach (string str in stringList)
          {
            string package = str;
            source.RemoveAll((Predicate<AppInfo>) (_ => _.Package == package));
          }
          if (appList.Length != source.Count)
          {
            flag1 = true;
            source = new List<AppInfo>(jarray.Count);
            foreach (JObject child in jarray.Children<JObject>())
            {
              AppInfo packageDetails = Utils.GetPackageDetails(vmName, child["package"].ToString().Trim(), false, out failReason);
              if (packageDetails != null)
                source.Add(packageDetails);
            }
            Logger.Info("Updating App Json from apps received from android. Details: " + installedPackages);
          }
        }
        foreach (AppInfo appInfo in source)
        {
          bool flag2 = Utils.CheckGamepadCompatible(appInfo.Package);
          if (appInfo.IsGamepadCompatible != flag2)
          {
            appInfo.IsGamepadCompatible = flag2;
            flag1 = true;
          }
        }
        if (flag1)
        {
          jsonParser.WriteJson(source.ToArray());
          try
          {
            HTTPUtils.SendRequestToClient("appJsonChanged", new Dictionary<string, string>(), vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Exception while sending appsync update to client: " + ex.ToString());
          }
        }
        Utils.sIsSyncAppJsonComplete = true;
      }
      catch (Exception ex)
      {
        Logger.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to sync app.json file for vm:{0}. " + ex.ToString(), (object) vmName));
      }
    }

    public static bool CheckGamepadCompatible(string packageName)
    {
      try
      {
        string inputmapperFile = Utils.GetInputmapperFile(packageName);
        bool flag = false;
        if (!string.IsNullOrEmpty(inputmapperFile))
        {
          string str = "";
          using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
          {
            if (mutex.WaitOne())
            {
              try
              {
                str = System.IO.File.ReadAllText(inputmapperFile);
                flag = true;
              }
              catch (Exception ex)
              {
                Logger.Error(string.Format("Failed to read cfg file... filepath: {0} Err : {1}", (object) inputmapperFile, (object) ex));
              }
              finally
              {
                mutex.ReleaseMutex();
              }
            }
          }
          if (flag)
          {
            foreach (string imapGamepadEvent in Constants.ImapGamepadEvents)
            {
              if (str.Contains(imapGamepadEvent))
                return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CheckGamepadCompatible: " + ex.ToString());
      }
      return false;
    }

    public static string GetInputmapperFile(string packageName = "")
    {
      string str = string.Empty;
      try
      {
        if (System.IO.File.Exists(Utils.GetInputmapperUserFilePath(packageName)))
          str = Utils.GetInputmapperUserFilePath(packageName);
        else if (System.IO.File.Exists(Utils.GetInputmapperDefaultFilePath(packageName)))
          str = Utils.GetInputmapperDefaultFilePath(packageName);
      }
      catch (Exception ex)
      {
        Logger.Error("Excpetion in GetInputMapper: " + ex.ToString());
      }
      return str;
    }

    public static string GetInputmapperUserFilePath(string packageName)
    {
      return Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), packageName + ".cfg");
    }

    public static string GetInputmapperDefaultFilePath(string packageName)
    {
      return Path.Combine(RegistryStrings.InputMapperFolder, packageName + ".cfg");
    }

    public static bool UnsupportedProcessor()
    {
      try
      {
        Logger.Info("Checking if Processor Unsupported");
        string[] strArray = new string[1]
        {
          "AMD64 Family 21 Model 16 Stepping 1 AuthenticAMD"
        };
        string str1 = Path.Combine(Path.GetTempPath(), "SystemInfo.txt");
        if (System.IO.File.Exists(str1))
          System.IO.File.Delete(str1);
        Utils.RunCmd("SystemInfo", (string) null, str1);
        string str2 = System.IO.File.ReadAllText(str1);
        foreach (string str3 in strArray)
        {
          if (str2.IndexOf(str3, StringComparison.OrdinalIgnoreCase) != -1)
            return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Checking if Processor Unsupported : {0}", (object) ex.ToString());
      }
      return false;
    }

    public static bool ReserveHTTPPorts()
    {
      bool flag1;
      try
      {
        string str = new SecurityIdentifier("S-1-1-0").Translate(typeof (NTAccount)).ToString();
        string cmd = "netsh.exe";
        int num1 = 2861;
        int num2 = 2971;
        Logger.Info("Reserving ports {0} - {1}", (object) num1, (object) num2);
        Logger.Info("---------------------------------------------------------------");
        bool flag2 = false;
        for (int index = num1; index < num2; ++index)
        {
          try
          {
            RunCommand.RunCmd(cmd, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http add urlacl url=http://*:{0}/ User=\\\"" + str + "\"", (object) index), (flag2 ? 1 : 0) != 0, (flag2 ? 1 : 0) != 0, false, 0);
          }
          catch (Exception ex)
          {
            Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error occured, Err: {0}", (object) ex.ToString()));
          }
          flag2 = index % 10 == 0;
        }
        flag1 = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in reserving HTTP ports: {0}", (object) ex.ToString());
        flag1 = false;
      }
      Logger.Info("---------------------------------------------------------------");
      return flag1;
    }

    public static void RestartService(string serviceName, int timeoutMilliseconds)
    {
      Logger.Info("Restarting {0} service", (object) serviceName);
      using (ServiceController serviceController = new ServiceController(serviceName))
      {
        try
        {
          int tickCount1 = Environment.TickCount;
          TimeSpan timeout1 = TimeSpan.FromMilliseconds((double) timeoutMilliseconds);
          serviceController.Stop();
          serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout1);
          int tickCount2 = Environment.TickCount;
          TimeSpan timeout2 = TimeSpan.FromMilliseconds((double) (timeoutMilliseconds - (tickCount2 - tickCount1)));
          serviceController.Start();
          serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout2);
        }
        catch (Exception ex)
        {
          Logger.Error("Error in restarting service " + ex.ToString());
        }
      }
    }

    public static bool CheckOpenGlSupport(
      out int glRenderMode,
      out string glVendor,
      out string glRenderer,
      out string glVersion,
      string blueStacksProgramFiles)
    {
      Logger.Info("In CheckSupportedGlRenderMode");
      glRenderMode = 4;
      glVersion = "";
      glRenderer = "";
      glVendor = "";
      Logger.Info("Running glcheck from folder : " + blueStacksProgramFiles);
      Logger.Info("Checking for glRenderMode 1");
      if (Utils.GetGraphicsInfo(Path.Combine(blueStacksProgramFiles, "HD-GLCheck.exe"), "1", out glVendor, out glRenderer, out glVersion) == 0)
      {
        glRenderMode = 1;
        return true;
      }
      Logger.Info("Opengl not supported.");
      return false;
    }

    public static int GetCurrentGraphicsInfo(
      string args,
      out string glVendor,
      out string glRenderer,
      out string glVersion)
    {
      return Utils.GetGraphicsInfo(Path.Combine(RegistryStrings.InstallDir, "HD-GLCheck.exe"), args, out glVendor, out glRenderer, out glVersion, true);
    }

    public static int GetGraphicsInfo(
      string prog,
      string args,
      out string glVendor,
      out string glRenderer,
      out string glVersion)
    {
      return Utils.GetGraphicsInfo(prog, args, out glVendor, out glRenderer, out glVersion, true);
    }

    public static int GetGraphicsInfo(
      string prog,
      string args,
      out string glVendor,
      out string glRenderer,
      out string glVersion,
      bool enableLogging)
    {
      Logger.Info("Will run " + prog + " with args " + args);
      string vendor = "";
      string renderer = "";
      string version = "";
      glVendor = vendor;
      glRenderer = renderer;
      glVersion = version;
      int num1 = -1;
      Environment.GetEnvironmentVariable("TEMP");
      try
      {
        using (Process proc = new Process())
        {
          proc.StartInfo.FileName = prog;
          proc.StartInfo.Arguments = args;
          proc.StartInfo.UseShellExecute = false;
          proc.StartInfo.CreateNoWindow = true;
          proc.StartInfo.RedirectStandardOutput = true;
          proc.OutputDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
          {
            try
            {
              string str = outLine.Data != null ? outLine.Data : "";
              if (enableLogging)
                Logger.Info(proc.Id.ToString() + " OUT: " + str);
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
          proc.Start();
          proc.BeginOutputReadLine();
          int milliseconds = 10000;
          int num4 = proc.WaitForExit(milliseconds) ? 1 : 0;
          glVendor = vendor;
          glRenderer = renderer;
          glVersion = version;
          if (num4 != 0)
          {
            int num2 = proc.Id;
            string str1 = num2.ToString();
            num2 = proc.ExitCode;
            string str2 = num2.ToString();
            Logger.Info(str1 + " EXIT: " + str2);
            num1 = proc.ExitCode;
          }
          else
            Logger.Error("Process killed after timeout: {0}s", (object) (milliseconds / 1000));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while running graphics check. Ex: {0}", (object) ex);
      }
      return num1;
    }

    public static int CheckSsse3Info(string prog, out string ssse3Supported)
    {
      Logger.Info("Will run " + prog);
      int num1 = -1;
      string ssse3value = "";
      try
      {
        using (Process process = new Process())
        {
          int milliseconds = 10000;
          process.StartInfo.FileName = prog;
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.RedirectStandardError = true;
          Countdown countDown = new Countdown(2);
          process.OutputDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
          {
            if (outLine.Data != null)
            {
              try
              {
                string data = outLine.Data;
                if (data.Contains("value ="))
                {
                  int num2 = data.IndexOf('=');
                  ssse3value = data.Substring(num2 + 1).Trim();
                }
              }
              catch (Exception ex)
              {
                Logger.Error("A crash occured in check cpu info delegate");
                Logger.Error(ex.ToString());
              }
              Logger.Info(Path.GetFileName(prog) + ": " + outLine.Data);
            }
            else
              countDown.Signal();
          });
          process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
          {
            if (outLine.Data != null)
              Logger.Error(Path.GetFileName(prog) + ": " + outLine.Data);
            else
              countDown.Signal();
          });
          process.Start();
          process.BeginOutputReadLine();
          process.BeginErrorReadLine();
          int num3 = process.WaitForExit(milliseconds) ? 1 : 0;
          countDown.Wait();
          if (num3 != 0)
          {
            Logger.Info(process.Id.ToString() + " EXIT: " + process.ExitCode.ToString());
            num1 = process.ExitCode;
          }
          else
            Logger.Error("Process killed after timeout: {0}s", (object) (milliseconds / 1000));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while running graphics check. Ex: {0}", (object) ex);
      }
      ssse3Supported = ssse3value == "1" || string.IsNullOrEmpty(ssse3value) ? "1" : "0";
      return num1;
    }

    public static bool CheckTwoCameraPresentOnDevice(ref bool bBothCamera)
    {
      bool flag;
      try
      {
        Guid lpGuid = new Guid("{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}");
        int classDevs = Utils.SetupDiGetClassDevs(ref lpGuid, IntPtr.Zero, IntPtr.Zero, ClassDevsFlags.DIGCF_PRESENT | ClassDevsFlags.DIGCF_ALLCLASSES);
        int num1 = -1;
        int Index = 0;
        int num2 = 0;
        while (num1 != 0)
        {
          SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
          DeviceInfoData.cbSize = Marshal.SizeOf((object) DeviceInfoData);
          num1 = Utils.SetupDiEnumDeviceInfo(classDevs, Index, ref DeviceInfoData);
          if (num1 == 1 && Utils.GetRegistryProperty(classDevs, ref DeviceInfoData, RegPropertyType.SPDRP_CLASSGUID).Equals("{6bdd1fc6-810f-11d0-bec7-08002be2092f}", StringComparison.OrdinalIgnoreCase))
          {
            ++num2;
            if (num2 == 2)
              bBothCamera = true;
          }
          ++Index;
          if (bBothCamera)
          {
            Logger.Info("Both Camera present on Device");
            break;
          }
        }
        flag = true;
      }
      catch (Exception ex)
      {
        flag = false;
        Logger.Info("Exception when trying to check Camera present on Device");
        Logger.Info(ex.ToString());
      }
      return flag;
    }

    private static string GetRegistryProperty(
      int PnPHandle,
      ref SP_DEVINFO_DATA DeviceInfoData,
      RegPropertyType Property)
    {
      int RequiredSize = 0;
      DATA_BUFFER PropertyBuffer = new DATA_BUFFER();
      Utils.SetupDiGetDeviceRegistryProperty(PnPHandle, ref DeviceInfoData, Property, IntPtr.Zero, ref PropertyBuffer, 1024, ref RequiredSize);
      return PropertyBuffer.Buffer;
    }

    public static int CallApkInstaller(string apkPath, bool isSilentInstall)
    {
      return Utils.CallApkInstaller(apkPath, isSilentInstall, (string) null);
    }

    public static int CallApkInstaller(string apkPath, bool isSilentInstall, string vmName)
    {
      Logger.Info("Installing apk :{0} vm name :{1} ", (object) apkPath, (object) vmName);
      if (vmName == null)
        vmName = "Android";
      int num = -1;
      try
      {
        string installDir = RegistryStrings.InstallDir;
        ProcessStartInfo startInfo = new ProcessStartInfo();
        if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
        {
          startInfo.FileName = Path.Combine(installDir, "HD-XapkHandler.exe");
          if (isSilentInstall)
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-xapk \"{0}\" -s -vmname {1}", (object) apkPath, (object) vmName);
          else
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-xapk \"{0}\" -vmname {1}", (object) apkPath, (object) vmName);
        }
        else
        {
          startInfo.FileName = Path.Combine(installDir, "HD-ApkHandler.exe");
          if (isSilentInstall)
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-apk \"{0}\" -s -vmname {1}", (object) apkPath, (object) vmName);
          else
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-apk \"{0}\" -vmname {1}", (object) apkPath, (object) vmName);
        }
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        Logger.Info("Console: installer path {0}", (object) startInfo.FileName);
        Process process = Process.Start(startInfo);
        process.WaitForExit();
        num = process.ExitCode;
        Logger.Info("Console: apk installer exit code: {0}", (object) process.ExitCode);
      }
      catch (Exception ex)
      {
        Logger.Info("Error Installing Apk : " + ex.ToString());
      }
      return num;
    }

    public static string GetInstallStatsUrl()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/bsinstallstats");
    }

    public static Dictionary<string, string> GetUserData()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string version = RegistryManager.Instance.Version;
      string registeredEmail = RegistryManager.Instance.RegisteredEmail;
      if (!string.IsNullOrEmpty(registeredEmail))
        dictionary.Add("email", registeredEmail);
      string str = ((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000L).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary.Add("user_time", str);
      return dictionary;
    }

    public static bool IsForegroundApplication()
    {
      bool flag = false;
      IntPtr foregroundWindow = InteropWindow.GetForegroundWindow();
      if (foregroundWindow != IntPtr.Zero)
      {
        uint ProcessId = 0;
        int windowThreadProcessId = (int) InteropWindow.GetWindowThreadProcessId(foregroundWindow, ref ProcessId);
        if ((long) ProcessId == (long) Process.GetCurrentProcess().Id)
          flag = true;
      }
      return flag;
    }

    public static bool CheckWritePermissionForFolder(string DirectoryPath)
    {
      if (string.IsNullOrEmpty(DirectoryPath))
        return false;
      try
      {
        using (System.IO.File.Create(Path.Combine(DirectoryPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
          ;
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static void UpdateRegistry(
      string registryKey,
      string name,
      object value,
      RegistryValueKind kind)
    {
      try
      {
        RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(registryKey, true);
        registryKey1.SetValue(name, value, kind);
        registryKey1.Close();
        registryKey1.Flush();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception occured in UpdateRegistry " + ex.ToString());
        throw;
      }
    }

    public static Icon GetApplicationIcon()
    {
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        return new Icon(Path.Combine(RegistryStrings.InstallDir, "app_icon.ico"));
      string iconCompletePath = RegistryStrings.ProductIconCompletePath;
      return System.IO.File.Exists(iconCompletePath) ? new Icon(iconCompletePath) : Icon.ExtractAssociatedIcon(Application.ExecutablePath);
    }

    public static bool IsHDPlusDebugMode()
    {
      return RegistryManager.Instance.PlusDebug != 0;
    }

    public static int GetGMStreamWindowWidth()
    {
      return 320 * SystemUtils.GetDPI() / 96;
    }

    public static void SetCurrentEngineStateAndGlTransportValue(EngineState state, string vmName)
    {
      Logger.Info("Setting CurrentEngineState: " + state.ToString());
      RegistryManager.Instance.CurrentEngine = state.ToString();
      string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
      string[] strArray = bootParameters.Split(' ');
      string str1 = "";
      string str2 = "GlTransport";
      int num = state != EngineState.legacy ? RegistryManager.Instance.GlPlusTransportConfig : RegistryManager.Instance.GlLegacyTransportConfig;
      Logger.Info("setting GlValue to {0}", (object) num);
      if (bootParameters.IndexOf(str2, StringComparison.OrdinalIgnoreCase) == -1)
      {
        str1 = bootParameters + " " + str2 + "=" + num.ToString();
      }
      else
      {
        foreach (string str3 in strArray)
        {
          if (str3.IndexOf(str2, StringComparison.OrdinalIgnoreCase) != -1)
          {
            if (!string.IsNullOrEmpty(str1))
              str1 += " ";
            str1 = str1 + str2 + "=" + num.ToString();
          }
          else
          {
            if (!string.IsNullOrEmpty(str1))
              str1 += " ";
            str1 += str3;
          }
        }
      }
      RegistryManager.Instance.Guest[vmName].BootParameters = str1;
    }

    public static bool RegisterComExe(string path, bool register)
    {
      try
      {
        return Utils.RunCmd(path, register ? "/RegServer" : "/UnregServer", (string) null).ExitCode == 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Command runner raised an exception: " + ex.ToString());
        return false;
      }
    }

    public static string GetCurrentKeyboardLayout()
    {
      try
      {
        return new CultureInfo(Utils.GetKeyboardLayout(Utils.GetWindowThreadProcessId(Utils.GetForegroundWindow(), IntPtr.Zero)).ToInt32() & (int) ushort.MaxValue).Name;
      }
      catch
      {
        return "en-US";
      }
    }

    public static bool IsEngineRaw()
    {
      bool flag = false;
      try
      {
        if (JObject.Parse(RegistryManager.Instance.DeviceCaps)["engine_enabled"].ToString().Trim() == EngineState.raw.ToString())
          flag = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error Occured, Err: " + ex.ToString());
      }
      Logger.Info("Engine mode Raw: " + flag.ToString());
      return flag;
    }

    public static string GetCampaignName()
    {
      string str = "";
      try
      {
        string campaignJson = RegistryManager.Instance.CampaignJson;
        if (string.IsNullOrEmpty(campaignJson))
          return str;
        JObject jobject = JObject.Parse(campaignJson);
        if (jobject != null)
          str = jobject["campaign_name"].ToString();
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to get campaign name.");
      }
      return str;
    }

    public static string GetUserCountry(string vmName)
    {
      try
      {
        string json = BstHttpClient.Get(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "api/getcountryforip"), (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp");
        Logger.Info("Got resp: " + json);
        return JObject.Parse(json)["country"].ToString().Trim();
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message);
        return "";
      }
    }

    public static void KillComServer()
    {
      Logger.Info("In KillComServer");
      string fullPath = Path.GetFullPath(RegistryStrings.InstallDir + "\\");
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher((ObjectQuery) new WqlObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'BstkSVC.exe'")))
      {
        foreach (ManagementObject managementObject in managementObjectSearcher.Get())
        {
          Logger.Info("Considering " + managementObject["ProcessId"]?.ToString() + " -> " + managementObject["ExecutablePath"]?.ToString());
          if (string.Compare(Path.GetFullPath(Path.GetDirectoryName((string) managementObject["ExecutablePath"]) + "\\"), fullPath, StringComparison.OrdinalIgnoreCase) == 0)
          {
            Process processById = Process.GetProcessById((int) (uint) managementObject["ProcessId"]);
            Logger.Info("Trying to kill PID " + processById.Id.ToString());
            processById.Kill();
            if (!processById.WaitForExit(10000))
              Logger.Info("Timeout waiting for process to die");
          }
        }
      }
    }

    public static void StopClientInstanceAsync(string vmName)
    {
      try
      {
        Logger.Info("Will send request stopInstance to " + vmName);
        List<string> stringList = new List<string>()
        {
          vmName
        };
        if (string.IsNullOrEmpty(vmName))
          stringList = ((System.Collections.Generic.IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>();
        foreach (string vmName1 in stringList)
        {
          try
          {
            HTTPUtils.SendRequestToClientAsync("stopInstance", new Dictionary<string, string>()
            {
              {
                nameof (vmName),
                vmName1
              }
            }, vmName1, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Warning("Exception in closing client for vm: {0} --> {1}", (object) vmName1, (object) ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing any frontend: " + ex.ToString());
      }
    }

    public static void StopFrontend(string vmName, bool isWaitForPlayerClosing = true)
    {
      try
      {
        Logger.Info("Will send request shutdown" + vmName);
        List<string> stringList = new List<string>()
        {
          vmName
        };
        if (string.IsNullOrEmpty(vmName))
          stringList = ((System.Collections.Generic.IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>();
        foreach (string vmName1 in stringList)
        {
          try
          {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
              {
                nameof (vmName),
                vmName1
              }
            };
            bool createdNew;
            using (Mutex mutex = new Mutex(true, Strings.GetPlayerLockName(vmName1, "bgp"), out createdNew))
            {
              if (!createdNew)
              {
                HTTPUtils.SendRequestToEngineAsync("shutdown", data, vmName1, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                if (isWaitForPlayerClosing)
                {
                  try
                  {
                    if (!mutex.WaitOne(60000))
                      HTTPUtils.SendRequestToEngine("forceShutdown", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
                  }
                  catch (AbandonedMutexException ex)
                  {
                    Logger.Info("Player closed: " + ex.Message);
                  }
                  catch (Exception ex)
                  {
                    Logger.Error("Could not check if player is running." + ex.Message);
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            Logger.Warning("Exception in closing any frontend for vm = " + vmName1 + " -->" + ex.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing any frontend: " + ex.ToString());
      }
    }

    public static bool CheckIfAndroidBstkExistAndValid(string vmName)
    {
      Logger.Info("Checking if android bstk exist and valid");
      string str = Path.Combine(Path.Combine(RegistryStrings.DataDir, vmName), vmName + ".bstk");
      if (!System.IO.File.Exists(str))
        return false;
      if (new FileInfo(str).Length == 0L)
        return false;
      try
      {
        XDocument xdocument = new XDocument();
        XDocument.Load(str);
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in parsing bstk file" + ex.ToString());
        return false;
      }
    }

    public static void CreateBstkFileFromPrev(string vmName)
    {
      Logger.Info("Creating Bstk file from Bstk-Prev file");
      string path1 = Path.Combine(RegistryStrings.DataDir, vmName);
      string destFileName = Path.Combine(path1, vmName + ".bstk");
      string str = Path.Combine(path1, vmName + ".bstk-prev");
      if (!System.IO.File.Exists(str))
        Logger.Info("android.bstk-prev file not exist");
      else
        System.IO.File.Copy(str, destFileName, true);
    }

    public static bool IsFirstVersionHigher(string firstVersion, string secondVersion)
    {
      string[] strArray1;
      if (firstVersion == null)
        strArray1 = (string[]) null;
      else
        strArray1 = firstVersion.Split('.');
      string[] strArray2 = strArray1;
      string[] strArray3;
      if (secondVersion == null)
        strArray3 = (string[]) null;
      else
        strArray3 = secondVersion.Split('.');
      string[] strArray4 = strArray3;
      bool flag = false;
      int index1 = 0;
      for (int index2 = Math.Min(strArray2.Length, strArray4.Length); index1 < index2; ++index1)
      {
        long num = Convert.ToInt64(strArray2[index1], (IFormatProvider) CultureInfo.InvariantCulture) - Convert.ToInt64(strArray4[index1], (IFormatProvider) CultureInfo.InvariantCulture);
        if (num > 0L)
        {
          flag = true;
          break;
        }
        if (num < 0L)
          break;
      }
      if (!flag && index1 < strArray2.Length && index1 == strArray4.Length)
        flag = true;
      return flag;
    }

    public static bool IsRunningInstanceClashWithAnotherInstance(string procName)
    {
      string installDir = RegistryStrings.InstallDir;
      string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
      if (string.IsNullOrEmpty(installDir) && string.IsNullOrEmpty(clientInstallDir))
        return false;
      procName = procName?.Replace(".exe", "");
      List<string> applicationPath = GetProcessExecutionPath.GetApplicationPath(Process.GetProcessesByName(procName));
      Logger.Debug("Number of running instances for the process {0} are {1} ", (object) procName, (object) applicationPath.Count);
      foreach (string path in applicationPath)
      {
        try
        {
          string directoryName = Path.GetDirectoryName(path);
          if (!directoryName.Equals(installDir.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
          {
            if (!directoryName.Equals(clientInstallDir.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
              return true;
          }
        }
        catch
        {
        }
      }
      return false;
    }

    public static int GetVideoControllersNum()
    {
      int num = 0;
      try
      {
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
        {
          ManagementObjectCollection objectCollection = managementObjectSearcher.Get();
          num = objectCollection.Count;
          Logger.Info("Win32_VideoController query count: ", (object) num);
          foreach (ManagementBaseObject managementBaseObject in objectCollection)
          {
            foreach (PropertyData property in managementBaseObject.Properties)
            {
              switch (property.Name)
              {
                case "Description":
                  Logger.Info("Description (Name): {0}", property.Value);
                  continue;
                case "DriverVersion":
                  Logger.Info("DriverVersion: {0}", property.Value);
                  continue;
                case "DriverDate":
                  Logger.Info("DriverDate: {0}", (object) ManagementDateTimeConverter.ToDateTime(property.Value.ToString()).ToUniversalTime().ToString("yyyy-MM-dd HH-mm-ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while runninq query. Ex: ", (object) ex);
        Logger.Info("Ignoring and continuing...");
      }
      return num;
    }

    public static void ParseGLVersion(string glVersion, out double version)
    {
      try
      {
        string s;
        if (glVersion != null && glVersion.StartsWith("OpenGL", StringComparison.OrdinalIgnoreCase))
        {
          s = glVersion.Split('(')[0].Trim().Split('S')[1].Trim();
        }
        else
        {
          string[] strArray = glVersion.Split(' ')[0].Trim().Split('.');
          s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) strArray[0], (object) strArray[1]);
        }
        version = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't parse for GL3 string: {0}", (object) glVersion);
        Logger.Error(ex.ToString());
        version = 0.0;
      }
    }

    public static string GetUpdatedBootParamsString(string var, string val, string oldBootParams)
    {
      Logger.Info("Attempting to update bootparam for {0}={1}", (object) var, (object) val);
      bool flag1 = false;
      if (string.IsNullOrEmpty(val))
        flag1 = true;
      List<string[]> strArrayList;
      if (oldBootParams == null)
        strArrayList = (List<string[]>) null;
      else
        strArrayList = ((System.Collections.Generic.IEnumerable<string>) oldBootParams.Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 1)).ToList<string[]>();
      List<string[]> source1 = strArrayList;
      Dictionary<string, string> dictionary;
      if (oldBootParams == null)
        dictionary = (Dictionary<string, string>) null;
      else
        dictionary = ((System.Collections.Generic.IEnumerable<string>) oldBootParams.Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      Dictionary<string, string> source2 = dictionary;
      if (flag1)
      {
        string[] strArray = new string[1]{ var };
        bool flag2 = false;
        foreach (System.Collections.Generic.IEnumerable<string> source3 in source1)
        {
          if (source3.Contains<string>(var))
            flag2 = true;
        }
        if (!flag2)
        {
          source1.Add(strArray);
          Logger.Info("BootParams added for {0}", (object) var, (object) val);
        }
        else
          Logger.Info("BootParam already present");
      }
      else
      {
        source2[var] = val;
        Logger.Info("BootParam added/updated");
      }
      List<string> list = source2.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)).ToList<string>();
      list.AddRange(source1.SelectMany<string[], string>((Func<string[], System.Collections.Generic.IEnumerable<string>>) (x => (System.Collections.Generic.IEnumerable<string>) x)));
      return string.Join(" ", list.ToArray());
    }

    private static string GetServiceImagePath(string svcName)
    {
      string name = "SYSTEM\\CurrentControlSet\\Services\\" + svcName;
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
      {
        if (registryKey != null)
          return Environment.ExpandEnvironmentVariables(registryKey.GetValue("ImagePath", (object) "").ToString());
      }
      return "";
    }

    public static bool IsRunningInstanceClashWithService(
      string[] servicePrefixes,
      out ServiceController runningSvc)
    {
      Logger.Info("In IsRunningInstanceClashWithService");
      runningSvc = (ServiceController) null;
      ServiceController[] devices = ServiceController.GetDevices();
      List<ServiceController> serviceControllerList = new List<ServiceController>();
      if (servicePrefixes != null)
      {
        foreach (ServiceController serviceController in devices)
        {
          foreach (string servicePrefix in servicePrefixes)
          {
            if (serviceController.ServiceName.Contains(servicePrefix))
              serviceControllerList.Add(serviceController);
          }
        }
      }
      string a = RegistryStrings.InstallDir.TrimEnd('\\');
      foreach (ServiceController serviceController in serviceControllerList)
      {
        string directoryName = Path.GetDirectoryName(Utils.GetServiceImagePath(serviceController.ServiceName));
        string b = directoryName.Substring(4, directoryName.Length - 4);
        if (!string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase) && serviceController.Status == ServiceControllerStatus.Running)
        {
          runningSvc = serviceController;
          return true;
        }
      }
      return false;
    }

    public static double RoundUp(double input, int places)
    {
      double num = Math.Pow(10.0, Convert.ToDouble(places));
      return Math.Ceiling(input * num) / num;
    }

    public static void UpdateBlueStacksSizeToRegistryASync()
    {
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(Utils.UpdateBlueStacksSizeToRegistry);
        backgroundWorker.RunWorkerAsync();
      }
    }

    private static void UpdateBlueStacksSizeToRegistry(object sender, DoWorkEventArgs e)
    {
      try
      {
        string empty = string.Empty;
        string name;
        if (SystemUtils.IsOs64Bit())
          name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\BlueStacks{1}", (object) "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall", (object) Strings.GetOemTag());
        else
          name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\BlueStacks{1}", (object) "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", (object) Strings.GetOemTag());
        long num = 0;
        foreach (string path2 in ((System.Collections.Generic.IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>())
        {
          string str = Path.Combine(RegistryStrings.DataDir, path2);
          if (Directory.Exists(str))
            num += IOUtils.GetDirectorySize(str);
        }
        int int32 = Convert.ToInt32(num / 1048576L + 1000L);
        Logger.Info("Updating {0}MB BlueStacks size to registry", (object) int32);
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true))
          registryKey?.SetValue("EstimatedSize", (object) (int32 * 1024), RegistryValueKind.DWord);
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't update size to registry, ignoring error. Ex: {0}", (object) ex.Message);
      }
    }

    public static object GetRegistryHKLMValue(string regPath, string key, object defaultValue)
    {
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regPath))
        {
          if (registryKey != null)
            return registryKey.GetValue(key, defaultValue);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting the reistry value " + ex.Message);
      }
      return defaultValue;
    }

    public static object GetRegistryHKCUValue(string regPath, string key, object defaultValue)
    {
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regPath))
        {
          if (registryKey != null)
            return registryKey.GetValue(key, defaultValue);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting the HKCU reistry value " + ex.Message);
      }
      return defaultValue;
    }

    public static void BackUpGuid(string userGUID)
    {
      try
      {
        StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_Guid_Backup"));
        streamWriter.Write(userGUID);
        streamWriter.Close();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to backup guid...ignoring...printing exception");
        Logger.Error(ex.ToString());
      }
    }

    public static void SetAttributesNormal(DirectoryInfo dir)
    {
      foreach (DirectoryInfo directory in dir?.GetDirectories("*", SearchOption.AllDirectories))
      {
        Utils.SetAttributesNormal(directory);
        directory.Attributes = FileAttributes.Normal;
      }
      foreach (FileSystemInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
        file.Attributes = FileAttributes.Normal;
    }

    public static string GetString(string currentValue, string defaultValue)
    {
      return string.IsNullOrEmpty(currentValue) ? defaultValue : currentValue;
    }

    public static int GetInt(int currentValue, int defaultValue)
    {
      return currentValue == 0 ? defaultValue : currentValue;
    }

    public static ulong GeneratePseudoRandomNumber()
    {
      DateTime now = DateTime.Now;
      return ((ulong) ((long) now.Month * 1000000000L + (long) now.DayOfWeek * 100000000L + (long) now.Day * 1000000L + (long) now.Hour * 1000L + (long) now.Minute * 100L) + (ulong) now.Second) * (ulong) now.Millisecond;
    }

    public static string CreateRandomBstSharedFolder(string bstSharedFolder)
    {
      try
      {
        ulong pseudoRandomNumber = Utils.GeneratePseudoRandomNumber();
        string path2;
        string path;
        while (true)
        {
          path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bst_{0}", (object) Convert.ToString(pseudoRandomNumber, (IFormatProvider) CultureInfo.InvariantCulture));
          path = Path.Combine(bstSharedFolder, path2);
          if (Directory.Exists(path))
            pseudoRandomNumber = Utils.GeneratePseudoRandomNumber();
          else
            break;
        }
        Directory.CreateDirectory(path);
        return path2;
      }
      catch (Exception ex)
      {
        Logger.Info("Failed to create random shared folder... Err : " + ex.ToString());
        throw new Exception("Failed to create Bst Shared Folder");
      }
    }

    public static string GetValueInBootParams(
      string name,
      string vmName,
      string bootparam = "",
      string oem = "bgp")
    {
      if (oem == null)
        oem = "bgp";
      string empty = string.Empty;
      string str = bootparam;
      if (string.IsNullOrEmpty(str))
        str = RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters;
      Dictionary<string, string> dictionary = ((System.Collections.Generic.IEnumerable<string>) str.Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary.ContainsKey(name))
        empty = dictionary[name];
      return empty;
    }

    public static string RemoveKeyFromBootParam(string key, string bootParam)
    {
      if (bootParam == null)
        return "";
      Dictionary<string, string> dictionary = ((System.Collections.Generic.IEnumerable<string>) bootParam.Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary.ContainsKey(key))
        dictionary.Remove(key);
      return string.Join(" ", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)).ToArray<string>());
    }

    public static void UpdateValueInBootParams(
      string name,
      string value,
      string vmName,
      bool addIfNotPresent = true,
      string oem = "bgp")
    {
      if (oem == null)
        oem = "bgp";
      Dictionary<string, string> dictionary = ((System.Collections.Generic.IEnumerable<string>) (!(oem != "bgp") ? RegistryManager.Instance.Guest[vmName].BootParameters : RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters).Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary.ContainsKey(name))
        dictionary[name] = value;
      else if (addIfNotPresent)
        dictionary.Add(name, value);
      string str = string.Join(" ", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)).ToList<string>().ToArray());
      if (oem != "bgp")
        RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = str;
      else
        RegistryManager.Instance.Guest[vmName].BootParameters = str;
    }

    public static string GetDisplayName(string vmName, string oem = "bgp")
    {
      if (oem == "bgp")
      {
        if ("Android".Equals(vmName, StringComparison.OrdinalIgnoreCase))
          return Strings.ProductTopBarDisplayName;
        if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
          return vmName?.Replace("Android_", Strings.ProductDisplayName + " ");
        if (!string.IsNullOrEmpty(RegistryManager.Instance.Guest[vmName].DisplayName))
          return RegistryManager.Instance.Guest[vmName].DisplayName;
        return vmName?.Replace("Android_", Strings.ProductDisplayName + " ");
      }
      if (!RegistryManager.RegistryManagers[oem].Guest.ContainsKey(vmName))
        return vmName?.Replace("Android_", Strings.ProductDisplayName + " ");
      if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers[oem].Guest[vmName].DisplayName))
        return RegistryManager.RegistryManagers[oem].Guest[vmName].DisplayName;
      return vmName?.Replace("Android_", Strings.ProductDisplayName + " ");
    }

    public static bool IsAnyItemEmptyInStringList(List<string> strList)
    {
      if (strList != null)
      {
        foreach (string str in strList)
        {
          if (string.IsNullOrEmpty(str))
            return false;
        }
      }
      return true;
    }

    private static void SaveFileInUnicode(string filePath)
    {
      string str = System.IO.File.ReadAllText(filePath);
      using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Unicode))
      {
        streamWriter.Write(str);
        streamWriter.Flush();
        streamWriter.Close();
      }
    }

    internal static Dictionary<string, string> AddCommonData(
      Dictionary<string, string> data)
    {
      if (!data.ContainsKey("install_id"))
        data.Add("install_id", RegistryManager.Instance.InstallID);
      if (!data.ContainsKey("launcher_version"))
        data.Add("launcher_version", RegistryManager.Instance.WebAppVersion);
      return data;
    }

    public static string GetVmIdFromVmName(string vmName)
    {
      if (vmName == "Android")
        return "0";
      return vmName?.Split('_')[1];
    }

    public static string GetAppRunAppJsonArg(string appName, string pkgName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-json \"{0}\"", (object) new JObject()
      {
        {
          "app_icon_url",
          (JToken) ""
        },
        {
          "app_name",
          (JToken) appName
        },
        {
          "app_url",
          (JToken) ""
        },
        {
          "app_pkg",
          (JToken) pkgName
        }
      }.ToString(Formatting.None).Replace("\"", "\\\""));
    }

    public static void DeleteFiles(List<string> listOfFiles)
    {
      if (listOfFiles == null)
        return;
      foreach (string listOfFile in listOfFiles)
      {
        if (!Utils.DeleteFile(listOfFile))
          Logger.Warning("Couldn't delete file: {0}", (object) listOfFile);
      }
    }

    public static bool DeleteFile(string filePath)
    {
      if (System.IO.File.Exists(filePath))
      {
        try
        {
          System.IO.File.Delete(filePath);
        }
        catch
        {
          return false;
        }
      }
      return true;
    }

    public static void RemoveGamingRelatedFiles()
    {
      Logger.Info("Removing gaming related files");
      Utils.DeleteFiles(new List<string>()
      {
        Path.Combine(RegistryManager.Instance.ClientInstallDir, "game_config.json")
      });
    }

    public static void UpgradeGamingRegistriesToFull()
    {
      Logger.Info("Setting registries for full edition");
      RegistryManager.Instance.InstallationType = InstallationTypes.FullEdition;
      RegistryManager.Instance.InstallerPkgName = string.Empty;
    }

    public static void UpgradeToFullVersionAndCreateBstShortcut(bool updateUninstallEntryToo = false)
    {
      Logger.Info("Upgrading to full version of BlueStacks");
      Utils.UpgradeGamingRegistriesToFull();
      Utils.RemoveGamingRelatedFiles();
      Utils.RemoveAdminRelatedRegistryAndFiles(updateUninstallEntryToo);
      CommonInstallUtils.CreateDesktopAndStartMenuShortcuts(Strings.ProductDisplayName, RegistryStrings.ProductIconCompletePath, Path.Combine(RegistryManager.Instance.InstallDir, "BlueStacks.exe"), "", "", "");
      Utils.SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
    }

    private static void RemoveAdminRelatedRegistryAndFiles(bool updateUninstallEntryToo)
    {
      Logger.Info("Removing admin related things");
      List<string> stringList = new List<string>()
      {
        Path.Combine(RegistryStrings.InstallDir, "game_config.json"),
        Path.Combine(RegistryStrings.InstallDir, "app_icon.ico")
      };
      string[] files = Directory.GetFiles(RegistryStrings.InstallDir, "gameinstaller_*.png", SearchOption.TopDirectoryOnly);
      stringList.AddRange((System.Collections.Generic.IEnumerable<string>) files);
      string path = Path.Combine(Path.GetTempPath(), "RemoveGamingFiles.bat");
      using (StreamWriter streamWriter = new StreamWriter(path))
      {
        if (updateUninstallEntryToo)
        {
          Logger.Info("Exporting uninstall registry");
          Logger.Info("Exporting result: {0}", (object) Utils.ExportUninstallEntry("\\BlueStacks" + Strings.GetOemTag(), Strings.UninstallRegistryExportedFilePath));
          Utils.UpdateUninstallRegistryFileForFullEdition(Strings.UninstallRegistryExportedFilePath);
          streamWriter.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "REG IMPORT \"{0}\"", (object) Strings.UninstallRegistryExportedFilePath));
        }
        foreach (string str in stringList)
          streamWriter.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DEL /F /Q \"{0}\"", (object) str));
        streamWriter.Close();
      }
      Logger.Info("Executing: {0}", (object) path);
      Process process = new Process();
      process.StartInfo.Verb = "runas";
      process.StartInfo.FileName = path;
      process.StartInfo.WorkingDirectory = Path.GetTempPath();
      process.StartInfo.CreateNoWindow = true;
      try
      {
        process.Start();
        process.WaitForExit();
      }
      catch (Win32Exception ex)
      {
        Logger.Error("User cancelled UAC: " + ex.Message);
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while executing the batch script: " + ex.ToString());
      }
      finally
      {
        process.Dispose();
      }
      Logger.Info("All done!");
    }

    public static void UpdateUninstallEntryForFullEdition()
    {
      try
      {
        Logger.Info("Exporting uninstall registry");
        string exportedFilePath = Strings.UninstallRegistryExportedFilePath;
        Logger.Info("Exporting result: {0}", (object) Utils.ExportUninstallEntry("\\BlueStacks" + Strings.GetOemTag(), exportedFilePath));
        Utils.UpdateUninstallRegistryFileForFullEdition(exportedFilePath);
        Logger.Info("Importing result: {0}", (object) Utils.ImportRegistryFile(exportedFilePath, true));
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't update uninstall entry");
        Logger.Warning(ex.ToString());
      }
    }

    private static void UpdateUninstallRegistryFileForFullEdition(string regFilePath)
    {
      Logger.Info("Updating exported file for full version");
      List<string> stringList = new List<string>();
      using (StreamReader streamReader = new StreamReader(regFilePath))
      {
        string str;
        while ((str = streamReader.ReadLine()) != null)
        {
          if (str.Contains("DisplayName"))
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"DisplayName\"=\"{0}\"", (object) Oem.Instance.ControlPanelDisplayName);
          else if (str.Contains("DisplayIcon"))
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"DisplayIcon\"=\"{0}\"", (object) RegistryStrings.ProductIconCompletePath);
          else if (str.Contains("Publisher"))
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"Publisher\"=\"{0}\"", (object) "BlueStack Systems, Inc.");
          stringList.Add(str);
        }
        streamReader.Close();
      }
      using (StreamWriter streamWriter = new StreamWriter(regFilePath))
      {
        foreach (string str in stringList)
          streamWriter.WriteLine(str);
        streamWriter.Close();
      }
    }

    public static int ExportUninstallEntry(string keyName, string destFilePath)
    {
      string cmd = "Reg.exe";
      string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EXPORT HKLM\\{0}{1} \"{2}\"", (object) "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", (object) keyName, (object) destFilePath);
      if (System.IO.File.Exists(destFilePath))
        System.IO.File.Delete(destFilePath);
      return RunCommand.RunCmd(cmd, args, true, true, false, 0).ExitCode;
    }

    public static int ImportRegistryFile(string regFilePath, bool requireAdminProc)
    {
      return RunCommand.RunCmd("Reg.exe", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IMPORT \"{0}\"", (object) regFilePath), true, true, requireAdminProc, 0).ExitCode;
    }

    public static string[] FixDuplicate7zArgs(string[] args)
    {
      string[] strArray;
      if ((args != null ? ((uint) args.Length > 0U ? 1 : 0) : 1) != 0)
      {
        if (args.Length == 1)
        {
          args[0] = args[0].Remove(args[0].Length / 2);
          strArray = args;
        }
        else
        {
          int length = args.Length / 2 + 1;
          strArray = new string[length];
          if (args[args.Length / 2].EndsWith(args[0], StringComparison.OrdinalIgnoreCase))
          {
            for (int index = 0; index <= length - 1; ++index)
              strArray[index] = args[index];
            strArray[length - 1] = strArray[length - 1].Remove(strArray[length - 1].LastIndexOf(args[0], StringComparison.OrdinalIgnoreCase));
          }
        }
      }
      else
        strArray = args;
      return strArray;
    }

    public static string GetMultiInstanceEventName(string vmName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) "BstClient", (object) vmName);
    }

    public static int GetVmIdToCreate(string oem = "bgp")
    {
      int vmId = RegistryManager.Instance.VmId;
      if (oem == null)
        oem = "bgp";
      if (oem != "bgp")
        vmId = RegistryManager.RegistryManagers[oem].VmId;
      while (true)
      {
        string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Android_{0}", (object) vmId);
        if (oem == "bgp")
        {
          if (Directory.Exists(Path.Combine(RegistryStrings.DataDir, path2)))
          {
            ++vmId;
            Logger.Info("Incrementing vmId: {0}", (object) vmId);
          }
          else
            break;
        }
        else if (Directory.Exists(Path.Combine(RegistryManager.RegistryManagers[oem].EngineDataDir, path2)))
        {
          ++vmId;
          Logger.Info("Incrementing vmId: {0}", (object) vmId);
        }
        else
          break;
      }
      if (oem != "bgp")
        ++RegistryManager.RegistryManagers[oem].VmId;
      else
        ++RegistryManager.Instance.VmId;
      return vmId;
    }

    public static JsonSerializerSettings GetSerializerSettings()
    {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
      serializerSettings.NullValueHandling = NullValueHandling.Ignore;
      serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
      serializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
      serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
      serializerSettings.Error += new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(Utils.JsonSerializer_Error);
      serializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
      return serializerSettings;
    }

    private static void JsonSerializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
    {
      e.ErrorContext.Handled = true;
      Logger.Error("Error loading JSON " + e.ErrorContext.Path + Environment.NewLine + e.ErrorContext.Error.ToString());
    }

    public static void OpenUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch (Win32Exception ex1)
      {
        try
        {
          Process.Start("IExplore.exe", url);
        }
        catch (Exception ex2)
        {
          Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex2.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex.ToString());
      }
    }

    public static string GetDpiFromBootParameters(string bootParameterString)
    {
      string[] strArray;
      if (bootParameterString == null)
        strArray = (string[]) null;
      else
        strArray = bootParameterString.Split(' ');
      string str1 = (string) null;
      foreach (string str2 in strArray)
      {
        if (str2.StartsWith("DPI=", StringComparison.OrdinalIgnoreCase))
        {
          str1 = str2.Split('=')[1];
          break;
        }
      }
      if (str1 == null)
        str1 = "240";
      return str1;
    }

    public static void ReplaceOldVirtualBoxNamespaceWithNew(string filePath)
    {
      Logger.Info("In ReplaceOldVirtualBoxNamespaceWithNew");
      string str = System.IO.File.ReadAllText(filePath);
      string oldValue = "http://www.innotek.de/VirtualBox-settings";
      string newValue = "http://www.virtualbox.org/";
      if (!str.Contains(oldValue))
        return;
      string contents = str.Replace(oldValue, newValue);
      System.IO.File.WriteAllText(filePath, contents);
    }

    public static void SetDPIInBootParameters(
      string bootParameterString,
      string updatedValue,
      string vmName,
      string oem = "bgp")
    {
      string[] strArray;
      if (bootParameterString == null)
        strArray = (string[]) null;
      else
        strArray = bootParameterString.Split(' ');
      string str1 = (string) null;
      foreach (string str2 in strArray)
      {
        if (str2.StartsWith("DPI=", StringComparison.OrdinalIgnoreCase))
        {
          str1 = str2.Split('=')[0];
          string str3 = str2.Split('=')[1];
          if (str3 != updatedValue)
          {
            string newValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DPI={0}", (object) updatedValue);
            string oldValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DPI={0}", (object) str3);
            string str4 = bootParameterString.Replace(oldValue, newValue);
            if (oem != "bgp")
              RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = str4;
            else
              RegistryManager.Instance.Guest[vmName].BootParameters = str4;
          }
        }
      }
      if (str1 != null)
        return;
      string str5 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DPI={0}", (object) updatedValue);
      string str6 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) bootParameterString, (object) str5);
      if (oem != "bgp")
        RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = str6;
      else
        RegistryManager.Instance.Guest[vmName].BootParameters = str6;
    }

    public static IntPtr BringToFront(string windowName)
    {
      Logger.Info("Window name is = " + windowName);
      IntPtr num = IntPtr.Zero;
      try
      {
        num = InteropWindow.BringWindowToFront(windowName, false, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in bringing existing window to the foreground Err : " + ex.ToString());
      }
      return num;
    }

    public static void SendChangeFPSToInstanceASync(string vmname, int newFps = 2147483647, string oem = "bgp")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "setfpsvalue?fps={0}", (object) (newFps == int.MaxValue ? RegistryManager.Instance.Guest[vmname].FPS : newFps)), vmname, oem);
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception in SendChangeFPSToInstanceASync. Error: " + ex?.ToString());
        }
      }));
    }

    public static void SendEnableVSyncToInstanceASync(bool enable, string vmname, string oem = "bgp")
    {
      HTTPUtils.SendRequestToEngineAsync("enableVSync", new Dictionary<string, string>()
      {
        {
          "enableVsync",
          enable.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      }, vmname, 0, (Dictionary<string, string>) null, true, 1, 0, oem);
    }

    public static void SendShowFPSToInstanceASync(string vmname, int isShowFPS)
    {
      HTTPUtils.SendRequestToEngineAsync("showFPS", new Dictionary<string, string>()
      {
        {
          "isshowfps",
          isShowFPS.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      }, vmname, 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
    }

    public static bool CheckMultiInstallBeforeRunQuitMultiInstall()
    {
      try
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
        int num = 0;
        foreach (string subKeyName in registryKey.GetSubKeyNames())
        {
          if (subKeyName.StartsWith("BlueStacks", StringComparison.OrdinalIgnoreCase) && !subKeyName.StartsWith("BlueStacksGP", StringComparison.OrdinalIgnoreCase) && !subKeyName.StartsWith("BlueStacksInstaller", StringComparison.OrdinalIgnoreCase))
            ++num;
        }
        if (num >= 2)
          return true;
      }
      catch (Exception ex)
      {
        Logger.Info("error at CheckMultiInstallBeforeRunQuitMultiInstall" + ex.ToString());
      }
      return false;
    }

    public static bool PingPartner(string oem, string vmName)
    {
      try
      {
        if (!string.IsNullOrEmpty(oem))
        {
          if (BstHttpClient.Get(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://127.0.0.1:{0}/ping", (object) RegistryManager.RegistryManagers[oem].PartnerServerPort), (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp").Contains("success"))
            return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to ping partner server. Exc: " + ex.ToString());
      }
      return false;
    }

    public static void WriteAgentPortInFile(int port)
    {
      new Thread((ThreadStart) (() =>
      {
        int num = 5;
        while (num > 0)
        {
          try
          {
            Utils.WriteToFile(Path.Combine(RegistryManager.Instance.UserDefinedDir, "bst_params.txt"), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "agentserverport={0}", (object) port), "agentserverport");
            break;
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to write agent port to bst_params.txt. Ex: " + ex.ToString());
          }
          Logger.Info("retrying..." + num.ToString());
          --num;
          Thread.Sleep(500);
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static void WriteToFile(string path, string text, string searchText = "")
    {
      bool flag = true;
      List<string> stringList = new List<string>();
      if (System.IO.File.Exists(path))
      {
        foreach (string readAllLine in System.IO.File.ReadAllLines(path))
        {
          if (readAllLine.Contains("="))
          {
            if (readAllLine.Contains(searchText))
            {
              stringList.Add(text);
              flag = false;
            }
            else
              stringList.Add(readAllLine);
          }
        }
      }
      if (flag)
      {
        using (TextWriter textWriter = (TextWriter) new StreamWriter(path, true))
        {
          textWriter.WriteLine(text);
          textWriter.Flush();
        }
      }
      else
      {
        using (TextWriter textWriter = (TextWriter) new StreamWriter(path, false))
        {
          foreach (string str in stringList)
            textWriter.WriteLine(str);
          textWriter.Flush();
        }
      }
    }

    public static int RunQuitMultiInstall()
    {
      string installDir = RegistryStrings.InstallDir;
      string path2 = "HD-QuitMultiInstall.exe";
      try
      {
        string str = Path.Combine(installDir, path2);
        Process process = new Process();
        process.StartInfo.Arguments = "-in";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = str;
        Logger.Info("Complete path to QuitMultiInstall: " + str);
        if (Environment.OSVersion.Version.Major <= 5)
          process.StartInfo.Verb = "runas";
        Logger.Info("Utils: Starting QuitMultiInstall with -in");
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured: {0}", (object) ex);
        Process process = new Process();
        process.StartInfo.Arguments = "-in";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = path2;
        process.StartInfo.WorkingDirectory = installDir;
        Logger.Info("Running {0} with WorkingDir {1}", (object) path2, (object) installDir);
        if (Environment.OSVersion.Version.Major <= 5)
          process.StartInfo.Verb = "runas";
        Logger.Info("Utils: Starting QuitMultiInstall with -in");
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
      }
    }

    public static bool WaitForBGPClientPing(int retries = 40)
    {
      while (retries > 0)
      {
        try
        {
          if (JArray.Parse(HTTPUtils.SendRequestToClient("ping", (Dictionary<string, string>) null, "Android", 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"))[0][(object) "success"].ToString().Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Debug("got ping response from client");
            return true;
          }
        }
        catch
        {
        }
        --retries;
        Thread.Sleep(500);
      }
      return false;
    }

    public static IWin32Window GetIWin32Window(IntPtr handle)
    {
      return (IWin32Window) new OldWindow(handle);
    }

    public static string GetPackageNameFromAPK(string apkFile)
    {
      string str = (string) null;
      try
      {
        using (Process process = new Process())
        {
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
          process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "hd-aapt.exe");
          process.StartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "dump badging \"{0}\"", (object) apkFile);
          process.Start();
          string end = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
          str = new Regex("package:\\sname='(.+?)'").Match(end).Groups[1].Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error getting apk name: {0}", (object) ex.Message);
      }
      return str;
    }

    public static string GetFileAssemblyVersion(string path)
    {
      string str = string.Empty;
      if (System.IO.File.Exists(path))
      {
        try
        {
          str = FileVersionInfo.GetVersionInfo(path).FileVersion;
        }
        catch (Exception ex)
        {
          Logger.Error("Error in parsing file version information: {0}", (object) ex.Message);
        }
      }
      return str;
    }

    public static string GetHelperInstalledPath()
    {
      return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Helper"), "BlueStacksHelper.exe");
    }

    public static string GetHelperTaskDetailsJSon()
    {
      try
      {
        CmdRes queryCommandOutput = TaskScheduler.GetTaskQueryCommandOutput("BlueStacksHelper");
        JObject jobject1 = new JObject();
        string[] strArray1 = queryCommandOutput.StdOut.Split('\n');
        string[] strArray2 = queryCommandOutput.StdErr.Split('\n');
        JObject jobject2 = new JObject();
        int num1 = 1;
        foreach (string str in strArray1)
        {
          if (!string.IsNullOrEmpty(str))
          {
            jobject2.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "line{0}", (object) num1), (JToken) str);
            ++num1;
          }
        }
        jobject1.Add("stdout", (JToken) jobject2.ToString());
        JObject jobject3 = new JObject();
        int num2 = 1;
        foreach (string str in strArray2)
        {
          if (!string.IsNullOrEmpty(str))
          {
            jobject3.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "line{0}", (object) num2), (JToken) str);
            ++num2;
          }
        }
        jobject1.Add("stderr", (JToken) jobject3.ToString());
        return jobject1.ToString(Formatting.None);
      }
      catch (Exception ex)
      {
        Logger.Error("Some error while creating json of the QueryTask: {0}", (object) ex);
      }
      return "";
    }

    public static bool HasOneDayPassed(DateTime srcTime)
    {
      try
      {
        if ((DateTime.Now - srcTime).TotalMinutes > 1440.0)
          return true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't check if the req time has passed. Ex: {0}", (object) ex.Message);
      }
      return false;
    }

    public static string GetAndroidIDFromAndroid(string vmName)
    {
      string empty = string.Empty;
      try
      {
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getAndroidID", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        if (jobject["result"].ToString() == "ok")
          empty = jobject["androidID"].ToString();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting Android ID: {0}", (object) ex.ToString());
      }
      return empty;
    }

    public static string GetGoogleAdIDFromAndroid(string vmName)
    {
      string empty = string.Empty;
      try
      {
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getGoogleAdID", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        if (jobject["result"].ToString() == "ok")
          empty = jobject["googleadid"].ToString();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting googleAd ID: {0}", (object) ex.ToString());
      }
      return empty;
    }

    public static void SetGoogleAdIdAndAndroidIdFromAndroid(string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          string googleAdIdFromAndroid = Utils.GetGoogleAdIDFromAndroid(vmName);
          if (!string.IsNullOrEmpty(googleAdIdFromAndroid))
            RegistryManager.Instance.Guest[vmName].GoogleAId = UUID.Base64Encode(googleAdIdFromAndroid);
          string androidIdFromAndroid = Utils.GetAndroidIDFromAndroid(vmName);
          if (string.IsNullOrEmpty(androidIdFromAndroid))
            return;
          RegistryManager.Instance.Guest[vmName].AndroidId = UUID.Base64Encode(androidIdFromAndroid);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while getting ids from android: {0}", (object) ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static string GetGoogleAdIdfromRegistry(string vmName)
    {
      string s = string.Empty;
      try
      {
        s = UUID.Base64Decode(RegistryManager.Instance.Guest[vmName].GoogleAId);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in decoding GoogleAid: {0}", (object) ex.ToString());
      }
      return StringUtils.GetControlCharFreeString(s);
    }

    public static string GetAndroidIdfromRegistry(string vmName)
    {
      string s = string.Empty;
      try
      {
        s = UUID.Base64Decode(RegistryManager.Instance.Guest[vmName].AndroidId);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in decoding AndroidID ID: {0}", (object) ex.ToString());
      }
      return StringUtils.GetControlCharFreeString(s);
    }

    public static void CreateMD5HashOfRootVdi()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          RegistryManager.Instance.RootVdiMd5Hash = Utils.GetMD5HashFromFile(RegistryManager.Instance.DefaultGuest.BlockDevice0Path);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while checking md5 hash of root.vdi: {0}", (object) ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static int GetMaxVmIdFromVmList(string[] vmList)
    {
      int num = 0;
      try
      {
        if (vmList != null)
        {
          foreach (string vm in vmList)
          {
            if (!(vm == "Android"))
            {
              int result;
              int.TryParse(vm.Split('_')[1], out result);
              if (num < result)
                num = result;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting max VmId to create err:", (object) ex.ToString());
      }
      return num + 1;
    }

    public static bool CheckIfUserIsPartOfGroup(string currentUser, string groupName)
    {
      Logger.Info("Inside CheckIfUserIsPartOfGroup- Args:" + currentUser + ", " + groupName);
      try
      {
        using (DirectoryEntry directoryEntry1 = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer"))
        {
          foreach (DirectoryEntry child in directoryEntry1.Children)
          {
            Logger.Info("\tInside CheckIfUserIsPartOfGroup - SchemeClassName: " + child.SchemaClassName);
            if (child.SchemaClassName == "User" && string.Equals(child.Name, currentUser, StringComparison.InvariantCultureIgnoreCase))
            {
              if (child.Invoke("Groups") is System.Collections.IEnumerable enumerable)
              {
                foreach (object adsObject in enumerable)
                {
                  try
                  {
                    DirectoryEntry directoryEntry2 = new DirectoryEntry(adsObject);
                    if (string.Equals(directoryEntry2.Name, groupName, StringComparison.InvariantCultureIgnoreCase))
                    {
                      directoryEntry2.Close();
                      return true;
                    }
                    directoryEntry2.Close();
                  }
                  catch (Exception ex)
                  {
                    Logger.Error("Could not create DirectoryEntry", (object) ex);
                  }
                }
              }
              else
                Logger.Info("\tCould not get childEntry.Invoke(\"Groups\") as IEnumerable");
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error while checking if " + currentUser + " is part of " + groupName + " group", (object) ex);
      }
      return false;
    }

    public static int GetRecommendedVCPUCount(bool isDefaultVm)
    {
      int num = 2;
      if (!isDefaultVm)
        num = 1;
      return num;
    }

    public static void SetTimeZoneInGuest(string vmName)
    {
      string str1 = TimeZone.CurrentTimeZone.StandardName;
      TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
      string str2 = utcOffset.ToString();
      string str3;
      if (str2[0] != '-')
        str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GMT+{0}", (object) str2);
      else
        str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GMT{0}", (object) str2);
      string str4 = TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string str5 = SystemUtils.GetSysInfo("Select DaylightBias from Win32_TimeZone");
      string str6;
      if (str4.Equals("True", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(str5))
      {
        string str7 = utcOffset.Add(new TimeSpan(0, Convert.ToInt32(str5, (IFormatProvider) CultureInfo.InvariantCulture), 0)).ToString();
        if (str7[0] != '-')
          str6 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GMT+{0}", (object) str7);
        else
          str6 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GMT{0}", (object) str7);
      }
      else
        str6 = str3;
      if (Features.IsFeatureEnabled(4194304UL))
      {
        str6 = "GMT+08:00:00";
        str4 = "False";
        str5 = "0";
        str3 = "GMT+08:00:00";
        str1 = "中国标准时间";
      }
      else if ("bgp".Equals("dmm", StringComparison.OrdinalIgnoreCase))
      {
        str6 = "GMT+09:00:00";
        str4 = "False";
        str5 = "0";
        str3 = "GMT+09:00:00";
        str1 = "日本の標準時";
      }
      VmCmdHandler.SendRequest("settz", new Dictionary<string, string>()
      {
        {
          "baseUtcOffset",
          str6
        },
        {
          "isDaylightSavingTime",
          str4
        },
        {
          "daylightBias",
          str5
        },
        {
          "utcOffset",
          str3
        },
        {
          "standardName",
          str1
        }
      }, vmName, out JObject _, "bgp");
    }

    public static void RunHDQuit(
      bool isWaitForExit = false,
      bool isFromClient = false,
      bool overrideIgnoreAgent = false,
      string oem = "bgp")
    {
      try
      {
        Logger.Info("Quit bluestacks called with args: {0}, {1}", (object) isWaitForExit, (object) isFromClient);
        string empty = string.Empty;
        string installDir;
        try
        {
          installDir = RegistryManager.RegistryManagers[oem].InstallDir;
        }
        catch (Exception ex)
        {
          Logger.Warning(string.Format("Failed to get RegistryManager.RegistryManagers[oem].InstallDir {0}", (object) ex));
          installDir = RegistryStrings.InstallDir;
        }
        string str = Path.Combine(installDir, "HD-Quit.exe");
        using (Process process = new Process())
        {
          process.StartInfo.FileName = str;
          if (!Oem.IsOEMDmm && !overrideIgnoreAgent)
            process.StartInfo.Arguments = "-ignoreAgent";
          if (isFromClient)
            process.StartInfo.Arguments += " -isFromClient";
          Logger.Debug("Quit Aguments = " + process.StartInfo.Arguments);
          process.Start();
          if (!isWaitForExit)
            return;
          process.WaitForExit();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Quit bluestacks failed: " + ex.ToString());
      }
    }

    public static JToken ExtractInfoFromXapk(string zipFilePath)
    {
      JToken jtoken = (JToken) null;
      string path1 = Path.Combine(Path.GetTempPath(), Path.GetFileName(zipFilePath));
      if (System.IO.File.Exists(Path.Combine(path1, "manifest.json")))
        jtoken = JToken.Parse(System.IO.File.ReadAllText(Path.Combine(path1, "manifest.json")));
      return jtoken;
    }

    public static bool CheckIfDeviceProfileChanged(
      JObject mCurrentDeviceProfile,
      JObject mChangedDeviceProfile)
    {
      return mCurrentDeviceProfile != null && mChangedDeviceProfile != null && (!string.Equals(mCurrentDeviceProfile["pcode"].ToString(), mChangedDeviceProfile["pcode"].ToString(), StringComparison.OrdinalIgnoreCase) || !string.Equals(mCurrentDeviceProfile["caSelector"].ToString(), mChangedDeviceProfile["caSelector"].ToString(), StringComparison.OrdinalIgnoreCase) || string.Equals(mCurrentDeviceProfile["pcode"].ToString(), "custom", StringComparison.OrdinalIgnoreCase) && (!string.Equals(mCurrentDeviceProfile["model"].ToString(), mChangedDeviceProfile["model"].ToString(), StringComparison.OrdinalIgnoreCase) || !string.Equals(mCurrentDeviceProfile["brand"].ToString(), mChangedDeviceProfile["brand"].ToString(), StringComparison.OrdinalIgnoreCase) || !string.Equals(mCurrentDeviceProfile["manufacturer"].ToString(), mChangedDeviceProfile["manufacturer"].ToString(), StringComparison.OrdinalIgnoreCase)));
    }

    public static bool CheckForInternetConnection()
    {
      try
      {
        using (WebClient webClient = new WebClient())
        {
          using (webClient.OpenRead("http://connectivitycheck.gstatic.com/generate_204"))
            return true;
        }
      }
      catch
      {
        return false;
      }
    }

    public static string[] AddVmNameInArgsIfNotPresent(string[] args)
    {
      if (!Utils.CheckIfVmNamePassedToArgs(args))
      {
        string str = Utils.CheckIfAnyVmRunning();
        List<string> list = ((System.Collections.Generic.IEnumerable<string>) args).ToList<string>();
        if (!string.IsNullOrEmpty(str))
        {
          list.Add("-vmname");
          list.Add(str);
        }
        args = list.ToArray();
      }
      return args;
    }

    private static string CheckIfAnyVmRunning()
    {
      foreach (object vm in (Array) RegistryManager.Instance.VmList)
      {
        string vmName = vm as string;
        if (Utils.CheckIfGuestReady(vmName, 1))
          return vmName;
      }
      return (string) null;
    }

    private static bool CheckIfVmNamePassedToArgs(string[] args)
    {
      return ((System.Collections.Generic.IEnumerable<string>) args).ToList<string>().Intersect<string>((System.Collections.Generic.IEnumerable<string>) new List<string>()
      {
        "--vmname",
        "-vmname",
        "vmname"
      }).Any<string>();
    }

    public static void SetAstcOption(string vmname, ASTCOption astcOption, string oem = "bgp")
    {
      if (oem == null)
        oem = "bgp";
      if (oem != "bgp")
        RegistryManager.RegistryManagers[oem].Guest[vmname].ASTCOption = astcOption;
      else
        RegistryManager.Instance.Guest[vmname].ASTCOption = astcOption;
      HTTPUtils.SendRequestToEngineAsync("setAstcOption", new Dictionary<string, string>()
      {
        {
          "AstcOption",
          ((int) astcOption).ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      }, vmname, 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
    }

    public static void KillCurrentOemProcessByName(string[] nameList, string clientInstallDir = null)
    {
      if (nameList == null)
        return;
      if (clientInstallDir == null)
        clientInstallDir = string.Empty;
      foreach (string name in nameList)
        Utils.KillCurrentOemProcessByName(name, clientInstallDir);
    }

    public static void KillCurrentOemProcessByName(string procName, string clientInstallDir = null)
    {
      Process[] processesByName = Process.GetProcessesByName(procName);
      string installDir = RegistryStrings.InstallDir;
      if (string.IsNullOrEmpty(clientInstallDir))
        clientInstallDir = RegistryManager.Instance.ClientInstallDir;
      foreach (Process proc in processesByName)
      {
        try
        {
          string directoryName = Path.GetDirectoryName(GetProcessExecutionPath.GetApplicationPathFromProcess(proc));
          if (!directoryName.Equals(installDir.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
          {
            if (!directoryName.Equals(clientInstallDir.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
            {
              if (!directoryName.Equals(RegistryStrings.ObsDir, StringComparison.InvariantCultureIgnoreCase))
                continue;
            }
          }
          Logger.Debug("Attempting to kill: {0}", (object) proc.ProcessName);
          proc.Kill();
          if (!proc.WaitForExit(5000))
            Logger.Info("Timeout waiting for process {0} to die", (object) proc.ProcessName);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in killing process " + ex.Message);
        }
      }
    }

    public static void EnableDisableApp(string appPackage, bool enable, string vmName)
    {
      try
      {
        HTTPUtils.SendRequestToGuestAsync("setapplicationstate", new Dictionary<string, string>()
        {
          {
            "d",
            new JObject()
            {
              {
                "packagename",
                (JToken) appPackage
              },
              {
                nameof (enable),
                (JToken) enable.ToString((IFormatProvider) CultureInfo.InvariantCulture)
              }
            }.ToString(Formatting.None)
          }
        }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableDisableApp :" + ex.ToString());
      }
    }

    public static string GetInstalledAppDataFromAllVms()
    {
      string[] vmList = RegistryManager.Instance.VmList;
      string empty = string.Empty;
      try
      {
        StringBuilder sb = new StringBuilder();
        JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) new StringWriter(sb));
        jsonTextWriter.Formatting = Formatting.Indented;
        using (JsonWriter jsonWriter = (JsonWriter) jsonTextWriter)
        {
          jsonWriter.WriteStartObject();
          foreach (string vmName in vmList)
          {
            jsonWriter.WritePropertyName("vm" + Utils.GetVmIdFromVmName(vmName));
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("google_aid");
            jsonWriter.WriteValue(Utils.GetGoogleAdIdfromRegistry(vmName));
            jsonWriter.WritePropertyName("android_id");
            jsonWriter.WriteValue(Utils.GetAndroidIdfromRegistry(vmName));
            jsonWriter.WritePropertyName("installed_apps");
            jsonWriter.WriteStartObject();
            foreach (AppInfo appInfo in ((System.Collections.Generic.IEnumerable<AppInfo>) new JsonParser(vmName).GetAppList()).ToList<AppInfo>())
            {
              string package = appInfo.Package;
              string name = appInfo.Name;
              jsonWriter.WritePropertyName(package);
              jsonWriter.WriteValue(name);
            }
            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
          }
          jsonWriter.WriteEndObject();
          empty = sb.ToString();
          Logger.Debug("json string of all apps : " + sb.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting all installed apps from all Vms: {0}", (object) ex.ToString());
      }
      return empty;
    }

    public static int GetTimeFromEpoch()
    {
      return (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static Dictionary<string, string> GetBootParamsDictFromBootParamString(
      string bootParams)
    {
      try
      {
        if (string.IsNullOrEmpty(bootParams))
          return (Dictionary<string, string>) null;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string str1 = bootParams;
        char[] chArray = new char[1]{ ' ' };
        foreach (string str2 in ((System.Collections.Generic.IEnumerable<string>) str1.Split(chArray)).ToList<string>())
          dictionary.Add(str2.Split('=')[0], str2.Split('=')[1]);
        return dictionary;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting bootParamsDict err: " + ex.ToString());
      }
      return (Dictionary<string, string>) null;
    }

    public static GlMode GetGlModeForVm(string vmName)
    {
      int glRenderMode = RegistryManager.Instance.Guest[vmName].GlRenderMode;
      int glMode = RegistryManager.Instance.Guest[vmName].GlMode;
      if (glRenderMode == 1 && glMode == 1)
        return GlMode.PGA_GL;
      if (glRenderMode == 1 && glMode == 2)
        return GlMode.AGA_GL;
      if (glMode == 1)
        return GlMode.PGA_DX;
      if (glMode == 2)
        return GlMode.AGA_DX;
      throw new Exception("Could not determine the engine mode for current build.");
    }

    public static bool CheckIfImagesArrayPresentInCfg(JObject oldConfig)
    {
      try
      {
        if (oldConfig != null)
        {
          foreach (JObject jobject in (System.Collections.Generic.IEnumerable<JToken>) oldConfig["ControlSchemes"])
          {
            if (jobject != null && jobject["Images"] != null && ((JContainer) jobject["Images"]).Count > 0)
              return true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CheckIfImagesArrayPresentInCfg: " + ex.ToString());
      }
      return false;
    }

    public static void ValidateScreenShotFolder()
    {
      try
      {
        if (StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
          return;
        Logger.Info("Media folder path: " + RegistryManager.Instance.ScreenShotsPath + " is invalid");
        if (!Directory.Exists(RegistryStrings.ScreenshotDefaultPath))
        {
          Logger.Info("ScreenshotDefaultPath: " + RegistryManager.Instance.ScreenShotsPath + " folder does not exist");
          Directory.CreateDirectory(RegistryStrings.ScreenshotDefaultPath);
        }
        RegistryManager.Instance.ScreenShotsPath = RegistryStrings.ScreenshotDefaultPath;
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("ValidateScreenShotFolder: {0}", (object) ex));
      }
    }
  }
}
