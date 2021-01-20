// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Logger
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace BlueStacks.Common
{
  public static class Logger
  {
    private static int s_OpenCloseAfter = 300;
    private static int s_OpenCloseAfterCount = 0;
    private static object s_sync = new object();
    private static TextWriter sWriter = Console.Error;
    private static int s_logRotationTime = 30000;
    public static readonly int s_logFileSize = 10485760;
    public static readonly int s_totalLogFileNum = 5;
    private static string s_logFilePath = (string) null;
    private static bool s_loggerInited = false;
    private static int s_processId = -1;
    private static string s_processName = "Unknown";
    private static string s_logLevels = (string) null;
    private static string s_logDir = (string) null;
    private static string s_logStringFatal = "FATAL";
    private static string s_logStringError = "ERROR";
    private static string s_logStringWarning = "WARNING";
    private static string s_logStringInfo = "INFO";
    private static string s_logStringDebug = "DEBUG";
    private static string s_vmNameTextToLog = "";
    private const int HDLOG_PRIORITY_FATAL = 0;
    private const int HDLOG_PRIORITY_ERROR = 1;
    private const int HDLOG_PRIORITY_WARNING = 2;
    private const int HDLOG_PRIORITY_INFO = 3;
    private const int HDLOG_PRIORITY_DEBUG = 4;
    private static FileStream s_fileStream;
    private const string DEFAULT_FILE_NAME = "BlueStacksUsers";
    private static Logger.HdLoggerCallback s_HdLoggerCallback;

    public static void InitUserLog()
    {
      Logger.InitLog((string) null, (string) null, true);
    }

    private static string GetLogDir()
    {
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\BlueStacks{0}", (object) Strings.GetOemTag())))
      {
        if (registryKey != null)
          Logger.s_logDir = (string) registryKey.GetValue("LogDir", (object) "");
        if (string.IsNullOrEmpty(Logger.s_logDir))
        {
          Logger.s_logDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          Logger.s_logDir = Path.Combine(Logger.s_logDir, "Bluestacks\\Logs");
        }
      }
      return Logger.s_logDir;
    }

    private static void HdLogger(int prio, uint tid, string tag, string msg)
    {
      Logger.Print(Logger.GetLogFromLevel(prio), tag, "{0:X8}: {1}", (object) tid, (object) msg);
    }

    public static void InitLog(string logFileName, string tag, bool doLogRotation = true)
    {
      if (logFileName == null)
        logFileName = "BlueStacksUsers";
      string str;
      if (logFileName == "-")
      {
        str = "-";
      }
      else
      {
        str = Path.Combine(Logger.GetLogDir(), logFileName);
        if (!Path.HasExtension(str))
          str += ".log";
      }
      Logger.InitLogAtPath(str, tag, doLogRotation);
    }

    public static void InitLogAtPath(string logFilePath, string _, bool doLogRotation)
    {
      Logger.s_loggerInited = true;
      Logger.s_HdLoggerCallback = new Logger.HdLoggerCallback(Logger.HdLogger);
      Logger.s_processId = Process.GetCurrentProcess().Id;
      Logger.s_processName = Process.GetCurrentProcess().ProcessName;
      string directoryName = Path.GetDirectoryName(logFilePath);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      Logger.s_logFilePath = logFilePath;
      Logger.LogLevelsInit();
      Logger.Open();
      if (!doLogRotation)
        return;
      Thread thread = new Thread((ThreadStart) (() => Logger.DoLogRotation()));
      if (((int) RegistryUtils.GetRegistryValue(Path.Combine(Strings.RegistryBaseKeyPath, "Client"), "RotateLog", (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) == 0 ? 0 : 1) == 0)
        return;
      thread.IsBackground = true;
      thread.Start();
    }

    public static Logger.HdLoggerCallback GetHdLoggerCallback()
    {
      return Logger.s_HdLoggerCallback;
    }

    private static void LogLevelsInit()
    {
      Logger.s_logLevels = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "DebugLogs", (object) null, RegistryKeyKind.HKEY_CURRENT_USER);
      if (!string.IsNullOrEmpty(Logger.s_logLevels))
        return;
      Logger.s_logLevels = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "DebugLogs", (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      if (string.IsNullOrEmpty(Logger.s_logLevels))
        return;
      Logger.s_logLevels = Logger.s_logLevels.ToUpper(CultureInfo.InvariantCulture);
    }

    public static void EnableDebugLogs()
    {
      Logger.s_logLevels = "ALL";
    }

    private static bool IsLogLevelEnabled(string tag, string level)
    {
      if (Logger.s_logLevels == null)
        return false;
      return Logger.s_logLevels.StartsWith("ALL", StringComparison.OrdinalIgnoreCase) || Logger.s_logLevels.Contains((tag + ":" + level).ToUpper(CultureInfo.InvariantCulture));
    }

    private static void DoLogRotation()
    {
      while (true)
      {
        Thread.Sleep(Logger.s_logRotationTime);
        try
        {
          lock (Logger.s_sync)
          {
            if (new FileInfo(Logger.s_logFilePath).Length >= (long) Logger.s_logFileSize)
            {
              string destFileName1 = Logger.s_logFilePath + ".1";
              string path = Logger.s_logFilePath + "." + Logger.s_totalLogFileNum.ToString();
              if (File.Exists(path))
                File.Delete(path);
              for (int index = Logger.s_totalLogFileNum - 1; index >= 1; --index)
              {
                string str = Logger.s_logFilePath + "." + index.ToString();
                string destFileName2 = Logger.s_logFilePath + "." + (index + 1).ToString();
                if (File.Exists(str))
                  File.Move(str, destFileName2);
              }
              File.Move(Logger.s_logFilePath, destFileName1);
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static void Open()
    {
      Logger.s_fileStream = new FileStream(Logger.s_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
      Logger.sWriter = (TextWriter) new StreamWriter((Stream) Logger.s_fileStream, Encoding.UTF8);
    }

    private static void Close()
    {
      try
      {
        Logger.sWriter.Close();
        Logger.s_fileStream.Close();
        Logger.s_fileStream.Dispose();
        Logger.sWriter.Dispose();
      }
      catch (Exception ex)
      {
        Logger.Open();
      }
    }

    public static TextWriter GetWriter()
    {
      return (TextWriter) new Writer((Writer.WriteFunc) (msg => Logger.Print(msg)));
    }

    private static string GetLogFromLevel(int level)
    {
      string str;
      switch (level)
      {
        case 0:
          str = Logger.s_logStringFatal;
          break;
        case 1:
          str = Logger.s_logStringError;
          break;
        case 2:
          str = Logger.s_logStringWarning;
          break;
        case 3:
          str = Logger.s_logStringInfo;
          break;
        case 4:
          str = Logger.s_logStringDebug;
          break;
        default:
          str = "UNKNOWN";
          break;
      }
      return str;
    }

    private static void Print(string s, string tag, string fmt, params object[] args)
    {
      if (!Logger.s_loggerInited)
        Logger.InitLog("", tag, true);
      if (s == Logger.s_logStringDebug && !Logger.IsLogLevelEnabled(tag, s))
        return;
      lock (Logger.s_sync)
      {
        try
        {
          if (Logger.s_fileStream != null)
            Logger.s_fileStream.Seek(0L, SeekOrigin.End);
        }
        catch (Exception ex)
        {
          Logger.Close();
          Logger.s_OpenCloseAfterCount = 0;
          Logger.Open();
        }
        ++Logger.s_OpenCloseAfterCount;
        if (Logger.s_OpenCloseAfterCount > Logger.s_OpenCloseAfter)
        {
          Logger.Close();
          Logger.s_OpenCloseAfterCount = 0;
          Logger.Open();
        }
        Logger.sWriter.WriteLine(Logger.GetPrefix(tag, s) + Logger.s_vmNameTextToLog + fmt, args);
        Logger.sWriter.Flush();
      }
    }

    private static void Print(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringInfo, Logger.s_processName, fmt, args);
    }

    private static void Print(string msg)
    {
      Logger.Print("{0}", (object) msg);
    }

    public static void Fatal(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringFatal, Logger.s_processName, fmt, args);
    }

    public static void Fatal(string msg)
    {
      Logger.Fatal("{0}", (object) msg);
    }

    public static void Error(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringError, Logger.s_processName, fmt, args);
    }

    public static void Error(string msg)
    {
      Logger.Error("{0}", (object) msg);
    }

    public static void Warning(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringWarning, Logger.s_processName, fmt, args);
    }

    public static void Warning(string msg)
    {
      Logger.Warning("{0}", (object) msg);
    }

    public static void Info(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringInfo, Logger.s_processName, fmt, args);
    }

    public static void Info(string msg)
    {
      Logger.Info("{0}", (object) msg);
    }

    public static void Debug(string fmt, params object[] args)
    {
      Logger.Print(Logger.s_logStringDebug, Logger.s_processName, fmt, args);
    }

    public static void Debug(string msg)
    {
      Logger.Debug("{0}", (object) msg);
    }

    private static string GetPrefix(string tag, string logLevel)
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      DateTime now = DateTime.Now;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}.{6:D3} {7}:{8:X8} ({9}) {10}: ", (object) now.Year, (object) now.Month, (object) now.Day, (object) now.Hour, (object) now.Minute, (object) now.Second, (object) now.Millisecond, (object) Logger.s_processId, (object) managedThreadId, (object) tag, (object) logLevel);
    }

    public static void InitVmInstanceName(string vmName)
    {
      if (vmName == null || vmName.Equals("Android", StringComparison.OrdinalIgnoreCase))
        return;
      Logger.s_vmNameTextToLog = vmName + ": ";
    }

    public delegate void HdLoggerCallback(int prio, uint tid, string tag, string msg);
  }
}
