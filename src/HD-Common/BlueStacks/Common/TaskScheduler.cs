// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.TaskScheduler
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Reflection;

namespace BlueStacks.Common
{
  public static class TaskScheduler
  {
    private static string BinaryName = "schtasks.exe";

    public static int CreateTask(
      string taskName,
      string binaryToRun,
      Tasks.Frequency frequency,
      int modifierOrIdleTime,
      DateTime timeToStart)
    {
      if (string.IsNullOrEmpty(binaryToRun))
        binaryToRun = Assembly.GetEntryAssembly().Location;
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("en-US", false).DateTimeFormat;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/" + Tasks.Parameter.Create.ToString() + " /SC " + frequency.ToString() + " /TN " + taskName + string.Format((IFormatProvider) CultureInfo.InvariantCulture, " /TR \"{0}\"", (object) binaryToRun) + " /F");
      string args;
      if (frequency != Tasks.Frequency.ONIDLE)
        args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} /MO {1}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} /ST {1}", (object) str, (object) timeToStart.ToString("HH:mm", (IFormatProvider) dateTimeFormat)), (object) modifierOrIdleTime.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} /I " + modifierOrIdleTime.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) str);
      int num = TaskScheduler.RunSchedulerCommand(args);
      if (num != 0)
        Logger.Error("An error occured while creating the task, exit code: {0}", (object) num);
      return num;
    }

    public static int DeleteTask(string taskName)
    {
      int num = TaskScheduler.RunSchedulerCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/" + Tasks.Parameter.Delete.ToString() + " /TN " + taskName + " /F"));
      if (num != 0)
        Logger.Error("An error occured while deleting the task, exit code: {0}", (object) num);
      return num;
    }

    private static string QueryTaskArguments(string taskName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/" + Tasks.Parameter.Query.ToString() + " /FO LIST /V  /TN " + taskName);
    }

    public static int QueryTask(string taskName)
    {
      int num = TaskScheduler.RunSchedulerCommand(TaskScheduler.QueryTaskArguments(taskName));
      if (num != 0)
        Logger.Error("An error occured while querying the task, exit code: {0}", (object) num);
      return num;
    }

    private static int RunSchedulerCommand(string args)
    {
      return RunCommand.RunCmd(TaskScheduler.BinaryName, args, true, true, false, 0).ExitCode;
    }

    public static CmdRes GetTaskQueryCommandOutput(string taskName)
    {
      return RunCommand.RunCmd(TaskScheduler.BinaryName, TaskScheduler.QueryTaskArguments(taskName), false, true, false, 0);
    }
  }
}
