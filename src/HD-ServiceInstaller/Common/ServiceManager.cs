// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ServiceManager
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace BlueStacks.Common
{
  public static class ServiceManager
  {
    private const int SERVICE_WIN32_OWN_PROCESS = 16;
    private const int SERVICE_KERNEL_DRIVER = 1;

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr OpenSCManager(
      string lpMachineName,
      string lpDatabaseName,
      ServiceManager.ServiceManagerRights dwDesiredAccess);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr OpenService(
      IntPtr hSCManager,
      string lpServiceName,
      ServiceManager.ServiceRights dwDesiredAccess);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CreateService(
      IntPtr hSCManager,
      string lpServiceName,
      string lpDisplayName,
      ServiceManager.ServiceRights dwDesiredAccess,
      int dwServiceType,
      ServiceManager.ServiceBootFlag dwStartType,
      ServiceManager.ServiceError dwErrorControl,
      string lpBinaryPathName,
      string lpLoadOrderGroup,
      IntPtr lpdwTagId,
      string lpDependencies,
      string lp,
      string lpPassword);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int CloseServiceHandle(IntPtr hSCObject);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int QueryServiceStatus(
      IntPtr hService,
      ServiceManager.SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int DeleteService(IntPtr hService);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int ControlService(
      IntPtr hService,
      ServiceManager.ServiceControl dwControl,
      ServiceManager.SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int StartService(
      IntPtr hService,
      int dwNumServiceArgs,
      int lpServiceArgVectors);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool QueryServiceObjectSecurity(
      SafeHandle serviceHandle,
      SecurityInfos secInfo,
      byte[] lpSecDesrBuf,
      uint bufSize,
      out uint bufSizeNeeded);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SetServiceObjectSecurity(
      SafeHandle serviceHandle,
      SecurityInfos secInfos,
      byte[] lpSecDesrBuf);

    public static bool UninstallService(string serviceName, bool isKernelDriverService = false)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) serviceName);
      IntPtr num1 = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
      bool flag = true;
      try
      {
        IntPtr num2 = ServiceManager.OpenService(num1, serviceName, ServiceManager.ServiceRights.StandardRightsRequired | ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Stop);
        if (num2 == IntPtr.Zero)
        {
          Logger.Info("Service " + serviceName + " is not installed or inaccessible.");
          return true;
        }
        try
        {
          ServiceManager.StopService(num2, isKernelDriverService);
          if (ServiceManager.DeleteService(num2) == 0)
            throw new Exception("Could not delete service " + Marshal.GetLastWin32Error().ToString());
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to uninstall service... Err : " + ex.ToString());
          flag = false;
        }
        finally
        {
          ServiceManager.CloseServiceHandle(num2);
        }
      }
      finally
      {
        ServiceManager.CloseServiceHandle(num1);
      }
      return flag;
    }

    public static bool ServiceIsInstalled(string serviceName)
    {
      IntPtr num = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
      try
      {
        IntPtr hSCObject = ServiceManager.OpenService(num, serviceName, ServiceManager.ServiceRights.QueryStatus);
        if (hSCObject == IntPtr.Zero)
          return false;
        ServiceManager.CloseServiceHandle(hSCObject);
        return true;
      }
      finally
      {
        ServiceManager.CloseServiceHandle(num);
      }
    }

    public static void InstallKernelDriver(string serviceName, string displayName, string fileName)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) serviceName);
      IntPtr num = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.AllAccess);
      try
      {
        if (ServiceManager.OpenService(num, serviceName, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start) != IntPtr.Zero)
        {
          Logger.Info("service is already installed...uninstalling it first");
          ServiceManager.UninstallService(serviceName, true);
        }
        IntPtr service = ServiceManager.CreateService(num, serviceName, displayName, ServiceManager.ServiceRights.AllAccess, 1, ServiceManager.ServiceBootFlag.AutoStart, ServiceManager.ServiceError.Normal, fileName, (string) null, IntPtr.Zero, (string) null, (string) null, (string) null);
        int lastWin32Error = Marshal.GetLastWin32Error();
        IntPtr zero = IntPtr.Zero;
        if (service == zero)
        {
          Logger.Info("Error in creating kernel driver service...last win32 error = " + lastWin32Error.ToString());
          throw new Exception("Failed to create service.");
        }
        Logger.Info("Successfully created service = " + serviceName + "...setting DACL now");
        ServiceManager.SetServicePermissions(serviceName);
        Logger.Info("Successfully set DACL");
      }
      catch (Exception ex)
      {
        ServiceManager.CloseServiceHandle(num);
        Logger.Error("Failed to install kernel driver... Err : " + ex.ToString());
        throw new Exception(ex.Message);
      }
    }

    public static void Install(string serviceName, string displayName, string fileName)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) serviceName);
      IntPtr num = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.AllAccess);
      try
      {
        if (ServiceManager.OpenService(num, serviceName, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start) != IntPtr.Zero)
        {
          Logger.Info("service is already installed...uninstalling it first");
          ServiceManager.UninstallService(serviceName, false);
        }
        IntPtr service = ServiceManager.CreateService(num, serviceName, displayName, ServiceManager.ServiceRights.AllAccess, 16, ServiceManager.ServiceBootFlag.AutoStart, ServiceManager.ServiceError.Normal, fileName, (string) null, IntPtr.Zero, (string) null, (string) null, (string) null);
        Logger.Info("Successfully created service = " + serviceName + "...setting DACL now");
        ServiceManager.SetServicePermissions(serviceName);
        Logger.Info("Successfully set DACL");
        IntPtr zero = IntPtr.Zero;
        if (service == zero)
          throw new Exception("Failed to install service.");
      }
      catch (Exception ex)
      {
        ServiceManager.CloseServiceHandle(num);
        Logger.Error("Failed to install kernel driver... Err : " + ex.ToString());
        throw new Exception(ex.Message);
      }
    }

    public static void StartService(string name, bool isKernelDriverService = false)
    {
      IntPtr num1 = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
      try
      {
        IntPtr num2 = ServiceManager.OpenService(num1, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start);
        if (num2 == IntPtr.Zero)
          throw new Exception("Could not open service.");
        try
        {
          ServiceManager.StartService(num2, isKernelDriverService);
        }
        finally
        {
          ServiceManager.CloseServiceHandle(num2);
        }
      }
      finally
      {
        ServiceManager.CloseServiceHandle(num1);
      }
    }

    public static void StopService(string name, bool isKernelDriverService = false)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) name);
      IntPtr num1 = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
      try
      {
        IntPtr num2 = ServiceManager.OpenService(num1, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Stop);
        if (num2 == IntPtr.Zero)
          return;
        try
        {
          ServiceManager.StopService(num2, isKernelDriverService);
        }
        finally
        {
          ServiceManager.CloseServiceHandle(num2);
        }
      }
      finally
      {
        ServiceManager.CloseServiceHandle(num1);
      }
    }

    private static void StartService(IntPtr hService, bool isKernelDriverService = false)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) Convert.ToString((object) hService, (IFormatProvider) CultureInfo.InvariantCulture));
      int num = ServiceManager.StartService(hService, 0, 0);
      if (num == 0)
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        Logger.Warning("Error in starting service, StartService ret: {0}, Last win32 error: {1}", (object) num, (object) lastWin32Error);
      }
      ServiceManager.SERVICE_STATUS ssStatus = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
      ServiceManager.WaitForServiceStatus(hService, ServiceManager.ServiceState.StartPending, ServiceManager.ServiceState.Running, ssStatus);
    }

    private static void StopService(IntPtr hService, bool isKernelDriverService = false)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) Convert.ToString((object) hService, (IFormatProvider) CultureInfo.InvariantCulture));
      ServiceManager.SERVICE_STATUS serviceStatus = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
      ServiceManager.ControlService(hService, ServiceManager.ServiceControl.Stop, serviceStatus);
      ServiceManager.WaitForServiceStatus(hService, ServiceManager.ServiceState.StopPending, ServiceManager.ServiceState.Stopped, serviceStatus);
    }

    private static bool WaitForServiceStatus(
      IntPtr hService,
      ServiceManager.ServiceState waitStatus,
      ServiceManager.ServiceState desiredStatus,
      ServiceManager.SERVICE_STATUS ssStatus)
    {
      ServiceManager.QueryServiceStatus(hService, ssStatus);
      if (ssStatus.dwCurrentState == desiredStatus)
        return true;
      int tickCount = Environment.TickCount;
      int dwCheckPoint = ssStatus.dwCheckPoint;
      while (ssStatus.dwCurrentState == waitStatus)
      {
        int millisecondsTimeout = ssStatus.dwWaitHint / 10;
        if (millisecondsTimeout < 1000)
          millisecondsTimeout = 1000;
        else if (millisecondsTimeout > 10000)
          millisecondsTimeout = 10000;
        Thread.Sleep(millisecondsTimeout);
        if (ServiceManager.QueryServiceStatus(hService, ssStatus) != 0)
        {
          if (ssStatus.dwCheckPoint > dwCheckPoint)
          {
            tickCount = Environment.TickCount;
            dwCheckPoint = ssStatus.dwCheckPoint;
          }
          else if (Environment.TickCount - tickCount > ssStatus.dwWaitHint)
            break;
        }
        else
          break;
      }
      return ssStatus.dwCurrentState == desiredStatus;
    }

    private static IntPtr OpenSCManager(ServiceManager.ServiceManagerRights rights)
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      IntPtr num = ServiceManager.OpenSCManager((string) null, (string) null, rights);
      if (!(num == IntPtr.Zero))
        return num;
      throw new Exception("Could not connect to service control manager.");
    }

    public static void SetServicePermissions(string serviceName)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) serviceName);
      using (ServiceController serviceController = new ServiceController(serviceName, "."))
      {
        int status = (int) serviceController.Status;
        byte[] numArray1 = new byte[0];
        uint bufSizeNeeded;
        bool flag = ServiceManager.QueryServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, numArray1, 0U, out bufSizeNeeded);
        if (!flag)
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          switch (lastWin32Error)
          {
            case 0:
            case 122:
              numArray1 = new byte[(int) bufSizeNeeded];
              flag = ServiceManager.QueryServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, numArray1, bufSizeNeeded, out bufSizeNeeded);
              break;
            default:
              throw new Exception("error calling QueryServiceObjectSecurity() to get DACL : error code=" + lastWin32Error.ToString());
          }
        }
        if (!flag)
          throw new Exception("error calling QueryServiceObjectSecurity(2) to get DACL : error code=" + Marshal.GetLastWin32Error().ToString());
        RawSecurityDescriptor securityDescriptor = new RawSecurityDescriptor(numArray1, 0);
        DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(false, false, securityDescriptor.DiscretionaryAcl);
        discretionaryAcl.AddAccess(AccessControlType.Allow, new SecurityIdentifier(WellKnownSidType.InteractiveSid, (SecurityIdentifier) null), 983551, InheritanceFlags.None, PropagationFlags.None);
        byte[] binaryForm = new byte[discretionaryAcl.BinaryLength];
        discretionaryAcl.GetBinaryForm(binaryForm, 0);
        securityDescriptor.DiscretionaryAcl = new RawAcl(binaryForm, 0);
        byte[] numArray2 = new byte[securityDescriptor.BinaryLength];
        securityDescriptor.GetBinaryForm(numArray2, 0);
        if (!ServiceManager.SetServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, numArray2))
          throw new Exception("error calling SetServiceObjectSecurity(); error code=" + Marshal.GetLastWin32Error().ToString());
      }
    }

    public static int InstallBstkDrv(string installDir, string driverName = "")
    {
      Logger.Info("InstallService start");
      try
      {
        int num = ServiceManager.InstallPlusDriver(installDir, driverName);
        if (num != 0)
          return num;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in installing BstkDrv, Err: " + ex.Message);
      }
      return 0;
    }

    private static int CheckStatusAndReturnResult(int result)
    {
      try
      {
        Logger.Info("Install failed due to: {0}", (object) (InstallerCodes) result);
      }
      catch
      {
      }
      return result;
    }

    private static int InstallPlusDriver(string installDir, string driverName = "")
    {
      Logger.Info("Installing driver");
      ServiceController[] devices = ServiceController.GetDevices();
      string str1 = Strings.BlueStacksDriverName;
      if (!string.IsNullOrEmpty(driverName))
        str1 = driverName;
      string driverDisplayName = Strings.BlueStacksDriverDisplayName;
      string driverPath = Path.Combine(installDir, Strings.BlueStacksDriverFileName);
      Logger.Info("Registering driver with params: file path : {0}, DriverName {1}, DisplayName: {2}", (object) driverPath, (object) str1, (object) driverDisplayName);
      if (ServiceManager.IsServiceAlreadyExists(devices, str1) && !ServiceManager.GetImagePathOfService(str1).Equals(driverPath, StringComparison.InvariantCultureIgnoreCase))
      {
        Logger.Info("Image path of driver is not same");
        if (ServiceManager.QueryDriverStatus(str1, true) != 1)
        {
          Logger.Info("InstallPlusDriver-> UNABLE_TO_STOP_SERVICE_BEFORE_UNINSTALLING");
          return -59;
        }
        try
        {
          string str2 = "sc.exe delete " + str1;
          using (Process p = new Process())
          {
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c \"" + str2 + "\"";
            Countdown countdown = new Countdown(2);
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            Logger.Info("Deleting service: {0}", (object) str1);
            p.OutputDataReceived += (DataReceivedEventHandler) ((o, e) =>
            {
              if (e.Data != null)
                Logger.Info(string.Format("{0} {1}", (object) p.Id, (object) e.Data));
              else
                countdown.Signal();
            });
            p.ErrorDataReceived += (DataReceivedEventHandler) ((o, e) =>
            {
              if (e.Data != null)
                Logger.Error(string.Format("{0} {1}", (object) p.Id, (object) e.Data));
              else
                countdown.Signal();
            });
            Logger.Info("Calling {0} {1}", (object) p.StartInfo.FileName, (object) p.StartInfo.Arguments);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            int exitCode = p.ExitCode;
            countdown.Wait();
            Logger.Info(string.Format("{0} {1} Exit Code:{2}", (object) p.StartInfo.FileName, (object) p.StartInfo.Arguments, (object) exitCode));
            if (exitCode != 0)
              return -58;
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Some error while running sc delete. Ex: {0}", (object) ex);
        }
        int num = ServiceManager.CheckForBlueStacksServicesMarkForDeletion(new List<string>()
        {
          Strings.BlueStacksDriverName
        });
        Logger.Info(string.Format("CheckForBlueStacksServicesMarkForDeletion Exit Code:{0}", (object) num));
        if (num != 0)
          return -58;
      }
      if (!ServiceManager.InstallDriver(str1, driverPath, driverDisplayName))
      {
        Logger.Error("Failed to install driver");
        return -40;
      }
      Logger.Info("Successfully Installed Driver");
      return 0;
    }

    public static int QueryDriverStatus(string name, bool isKernelDriverService = false)
    {
      Logger.Info("{0} {1}", (object) MethodBase.GetCurrentMethod().Name, (object) name);
      IntPtr num1 = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
      IntPtr num2 = IntPtr.Zero;
      try
      {
        num2 = ServiceManager.OpenService(num1, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Stop);
        if (num2 == IntPtr.Zero)
        {
          Logger.Info("service handle not created");
          return -1;
        }
        ServiceManager.SERVICE_STATUS lpServiceStatus = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
        if (ServiceManager.QueryServiceStatus(num2, lpServiceStatus) != 0)
        {
          Logger.Info("current service state is: {0} for service: {1}", (object) lpServiceStatus.dwCurrentState, (object) name);
          return (int) lpServiceStatus.dwCurrentState;
        }
        Logger.Info("Error in getting service status.." + Marshal.GetLastWin32Error().ToString());
        return -1;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in querying driver status err: " + ex.ToString());
        return -1;
      }
      finally
      {
        ServiceManager.CloseServiceHandle(num2);
        ServiceManager.CloseServiceHandle(num1);
      }
    }

    private static bool InstallDriver(
      string driverName,
      string driverPath,
      string driverDisplayName)
    {
      try
      {
        ServiceManager.StopService(driverName, false);
        ServiceManager.UninstallService(driverName, true);
      }
      catch (Exception ex)
      {
        Logger.Info("Ignore Error, when stopping and uninstalling driver ex : {0}", (object) ex.ToString());
      }
      try
      {
        ServiceManager.InstallKernelDriver(driverName, driverDisplayName, driverPath);
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
        return false;
      }
    }

    private static string GetImagePathOfService(string serviceName)
    {
      RegistryKey registryKey = (RegistryKey) null;
      try
      {
        Logger.Info("In GetImagePathOfService {0}", (object) serviceName);
        string name = Path.Combine("System\\CurrentControlSet\\Services", serviceName);
        registryKey = Registry.LocalMachine.OpenSubKey(name);
        string str = (string) registryKey.GetValue("ImagePath");
        registryKey.Close();
        return str;
      }
      catch (Exception ex)
      {
        Logger.Error("Could not get the image path for service {0}, ex: {1}", (object) serviceName, (object) ex.ToString());
        return (string) null;
      }
      finally
      {
        registryKey?.Close();
      }
    }

    private static bool CheckIfInstalledServicePathAndInstallDirPathMatch(
      string serviceName,
      string installDir)
    {
      Logger.Info("Checking file path for {0}", (object) serviceName);
      string imagePathOfService = ServiceManager.GetImagePathOfService(serviceName);
      if (string.IsNullOrEmpty(imagePathOfService))
      {
        Logger.Error("The code checking image path of service returned null");
        return false;
      }
      string installDirOfService = ServiceManager.GetInstallDirOfService(imagePathOfService);
      if (!(installDirOfService != installDir))
        return true;
      Logger.Error("Service {0} is already installed but at incorrect path {1}, required path is {2}", (object) serviceName, (object) installDirOfService, (object) installDir);
      return false;
    }

    private static string GetInstallDirOfService(string servicePath)
    {
      int length = servicePath.IndexOf(".sys", StringComparison.OrdinalIgnoreCase);
      if (length != -1)
        return Path.GetDirectoryName(servicePath.Substring(4, length));
      int num = servicePath.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
      return num == -1 ? (string) null : Path.GetDirectoryName(servicePath.Substring(0, num + 4).Replace("\"", ""));
    }

    private static bool IsServiceAlreadyExists(ServiceController[] services, string serviceName)
    {
      Logger.Info("Checking if service {0} exists on user's machine", (object) serviceName);
      try
      {
        foreach (ServiceController service in services)
        {
          if (service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
          {
            Logger.Info("Found service: " + serviceName);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in checking if service {0} is installed ex: {1}", (object) serviceName, (object) ex.ToString());
      }
      return false;
    }

    public static int CheckForBlueStacksServicesMarkForDeletion(List<string> servicesName)
    {
      if (servicesName != null)
      {
        foreach (string serviceName in servicesName)
        {
          if (ServiceManager.CheckIfServiceHasBeenMarkedForDeletion(serviceName))
            return -30;
        }
      }
      return 0;
    }

    private static bool CheckIfServiceHasBeenMarkedForDeletion(string serviceName)
    {
      try
      {
        Logger.Info("checking for marked for deletion flag in service {0}", (object) serviceName);
        string name = Path.Combine("system\\CurrentControlSet\\services", serviceName);
        int num1 = 10;
        while (num1 > 0)
        {
          int num2 = (int) Registry.LocalMachine.OpenSubKey(name).GetValue("DeleteFlag");
          --num1;
          Logger.Info("delete flag : " + num2.ToString() + " and retry number = " + (10 - num1).ToString());
          if (num2 == 1)
          {
            if (num1 == 0)
            {
              Logger.Warning("the  service {0} has been marked for deletion.", (object) serviceName);
              return true;
            }
            Thread.Sleep(1000);
          }
          else
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Could not check for service marked for deletion. should be safe to ignore in most cases.");
      }
      return false;
    }

    [System.Flags]
    public enum ServiceManagerRights
    {
      Connect = 1,
      CreateService = 2,
      EnumerateService = 4,
      Lock = 8,
      QueryLockStatus = 16, // 0x00000010
      ModifyBootConfig = 32, // 0x00000020
      StandardRightsRequired = 983040, // 0x000F0000
      AllAccess = StandardRightsRequired | ModifyBootConfig | QueryLockStatus | Lock | EnumerateService | CreateService | Connect, // 0x000F003F
    }

    [System.Flags]
    public enum ServiceRights
    {
      QueryConfig = 1,
      ChangeConfig = 2,
      QueryStatus = 4,
      EnumerateDependants = 8,
      Start = 16, // 0x00000010
      Stop = 32, // 0x00000020
      PauseContinue = 64, // 0x00000040
      Interrogate = 128, // 0x00000080
      UserDefinedControl = 256, // 0x00000100
      Delete = 65536, // 0x00010000
      StandardRightsRequired = 983040, // 0x000F0000
      ReadControl = 131072, // 0x00020000
      AllAccess = 983551, // 0x000F01FF
    }

    public enum ServiceBootFlag
    {
      BootStart,
      SystemStart,
      AutoStart,
      DemandStart,
      Disabled,
    }

    public enum ServiceControl
    {
      Stop = 1,
      Pause = 2,
      Continue = 3,
      Interrogate = 4,
      Shutdown = 5,
      ParamChange = 6,
      NetBindAdd = 7,
      NetBindRemove = 8,
      NetBindEnable = 9,
      NetBindDisable = 10, // 0x0000000A
    }

    public enum ServiceError
    {
      Ignore,
      Normal,
      Severe,
      Critical,
    }

    public enum ServiceState
    {
      Unknown = -1, // 0xFFFFFFFF
      NotFound = 0,
      Stopped = 1,
      StartPending = 2,
      StopPending = 3,
      Running = 4,
      ContinuePending = 5,
      PausePending = 6,
      Paused = 7,
    }

    [StructLayout(LayoutKind.Sequential)]
    private class SERVICE_STATUS
    {
      public int dwServiceType = 16;
      public ServiceManager.ServiceState dwCurrentState;
      public int dwControlsAccepted;
      public int dwWin32ExitCode;
      public int dwServiceSpecificExitCode;
      public int dwCheckPoint;
      public int dwWaitHint;

      public SERVICE_STATUS(bool isKernelDriver)
      {
        if (!isKernelDriver)
          return;
        this.dwServiceType = 1;
      }
    }
  }
}
