// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.VBoxManager
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;
using BlueStacks.VBoxUtils;
using BstkTypeLib;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.VmManager
{
  public class VBoxManager
  {
    private const string s_defaultVmName = "Android";
    protected static IVirtualBoxClient sVirtualBoxClient;
    protected static IVirtualBox sVirtualBox;
    protected static Session mSession;
    protected static IConsole mConsole;
    protected static IMachine mMachine;
    protected static int mVmId;
    private static bool mIsSDCardPresent;

    public static bool IsSDCardPresent
    {
      get
      {
        return VBoxManager.mIsSDCardPresent;
      }
      set
      {
        VBoxManager.mIsSDCardPresent = value;
      }
    }

    public static bool Init()
    {
      Logger.Info("In method VBoxManager Init");
      try
      {
        if (VBoxManager.sVirtualBox == null)
        {
          VBoxManager.sVirtualBoxClient = (IVirtualBoxClient) new VirtualBoxClientClass();
          VBoxManager.sVirtualBox = (IVirtualBox) VBoxManager.sVirtualBoxClient.VirtualBox;
        }
      }
      catch (Exception ex1)
      {
        Logger.Info("Virtual box init failed, error : {0}", (object) ex1.ToString());
        try
        {
          ComRegistration.Register();
          Logger.Info("Retrying to init VBox");
          if (VBoxManager.sVirtualBox == null)
          {
            VBoxManager.sVirtualBoxClient = (IVirtualBoxClient) new VirtualBoxClientClass();
            VBoxManager.sVirtualBox = (IVirtualBox) VBoxManager.sVirtualBoxClient.VirtualBox;
          }
        }
        catch (Exception ex2)
        {
          Logger.Error("Virtual box init failed, error : {0}", (object) ex2.ToString());
          return false;
        }
      }
      return true;
    }

    public static void CheckForSdCardFilePresent()
    {
      Logger.Info("Check for SDCard file");
      VBoxManager.IsSDCardPresent = File.Exists(Path.Combine(Path.Combine(RegistryStrings.DataDir, "Android"), "SDCard.vdi"));
    }

    private static void StopInstance(string vmName)
    {
      Utils.StopFrontend(vmName, true);
    }

    public static int DeleteMachine(string vmName)
    {
      if (ProcessUtils.IsAlreadyRunning(BlueStacks.Common.Strings.GetPlayerLockName(vmName, "bgp")))
        VBoxManager.StopInstance(vmName);
      try
      {
        if (!VBoxManager.Init())
          return -12;
        IMachine machine = VBoxManager.sVirtualBox.FindMachine(vmName);
        IMedium[] aMedia = machine.Unregister(CleanupMode.CleanupMode_Full);
        IProgress progress1 = machine.DeleteConfig(aMedia);
        while (progress1.Completed == 0)
        {
          Thread.Sleep(100);
          Logger.Info(string.Format("Deleting : {0}%", (object) progress1.Percent));
        }
        foreach (IMedium medium in aMedia)
        {
          if (!medium.Name.Equals("fastboot.vdi", StringComparison.InvariantCultureIgnoreCase) && !medium.Name.Equals("Root.vdi", StringComparison.InvariantCultureIgnoreCase))
          {
            IProgress progress2 = medium.DeleteStorage();
            while (progress2.Completed == 0)
            {
              Thread.Sleep(100);
              Logger.Info(string.Format("Deleting : {0}%", (object) progress2.Percent));
            }
          }
        }
      }
      catch (COMException ex)
      {
        Logger.Error(ex.ToString());
      }
      Logger.Info("Deleting apps json");
      VBoxManager.DeleteAppsJson(vmName);
      Logger.Info("Deleting regsitry");
      RegistryManager.Instance.DeleteAndroidSubKey(vmName);
      VBoxManager.DeleteVmNameFromVmList(vmName);
      return 0;
    }

    private static void DeleteAppsJson(string vmName)
    {
      string path1 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\apps_" + vmName + ".json");
      if (File.Exists(path1))
        File.Delete(path1);
      string path2 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\apps_" + vmName + ".json.bak");
      if (File.Exists(path2))
        File.Delete(path2);
      string path3 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\requirements_" + vmName + ".json");
      if (File.Exists(path3))
        File.Delete(path3);
      string path4 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\requirements_" + vmName + ".json.bak");
      if (!File.Exists(path4))
        return;
      File.Delete(path4);
    }

    public static void AddVmNameToVmList(string vmName)
    {
      RegistryManager.Instance.VmList = new List<string>((IEnumerable<string>) RegistryManager.Instance.VmList)
      {
        vmName
      }.ToArray();
    }

    public static void AddAppConfigrationEntryForNewVm(string vmName, string cloneFromVm)
    {
      if (!AppConfigurationManager.Instance.VmAppConfig.ContainsKey(cloneFromVm))
        return;
      AppConfigurationManager.Instance.VmAppConfig[vmName] = new Dictionary<string, BlueStacks.Common.AppSettings>();
      foreach (KeyValuePair<string, BlueStacks.Common.AppSettings> keyValuePair in AppConfigurationManager.Instance.VmAppConfig[cloneFromVm])
      {
        string key = keyValuePair.Key;
        BlueStacks.Common.AppSettings appSettings = keyValuePair.Value.DeepCopy<BlueStacks.Common.AppSettings>();
        AppConfigurationManager.Instance.VmAppConfig[vmName][key] = appSettings;
      }
      AppConfigurationManager.Save();
    }

    public static void DeleteVmNameFromVmList(string vmName)
    {
      List<string> stringList = new List<string>((IEnumerable<string>) RegistryManager.Instance.VmList);
      stringList.Remove(vmName);
      RegistryManager.Instance.VmList = stringList.ToArray();
    }

    private static IMedium CloneMediumFull(string oldMediumPath, string newMediumPath)
    {
      IMedium medium1 = VBoxManager.sVirtualBox.OpenMedium(oldMediumPath, DeviceType.DeviceType_HardDisk, AccessMode.AccessMode_ReadOnly, 0);
      IMedium medium2 = VBoxManager.sVirtualBox.CreateMedium("vdi", newMediumPath, AccessMode.AccessMode_ReadWrite, DeviceType.DeviceType_HardDisk);
      Logger.Info("Old medium was of logical size {0} ", (object) medium1.LogicalSize);
      IProgress progress = medium1.CloneTo(medium2, medium1.Variant, (IMedium) null);
      while (progress.Completed == 0)
      {
        Thread.Sleep(100);
        Logger.Info(string.Format("Cloning {0}: {1}%", (object) oldMediumPath, (object) progress.Percent));
      }
      return medium2;
    }

    private static bool Clone(string cloneFromVm, string vmName, string vmType)
    {
      IMediumAttachment[] mediumAttachment = (IMediumAttachment[]) null;
      string controllerName = (string) null;
      try
      {
        string path = Path.Combine(RegistryStrings.DataDir, vmName);
        Path.Combine(RegistryStrings.DataDir, "Android");
        if (Directory.Exists(path))
        {
          Logger.Error("Virtual Machine directory exists!!");
          return false;
        }
        Directory.CreateDirectory(path);
        Logger.Info("Android directory {0} successfully created", (object) path);
        if (string.Equals(vmType, "fresh", StringComparison.InvariantCultureIgnoreCase))
        {
          IMachine machine = VBoxManager.sVirtualBox.FindMachine("Android");
          mediumAttachment = VBoxManager.GetMediumAttachments(machine, ref controllerName);
          IMediumAttachment fastbootAttachment = VBoxManager.GetFastbootAttachment(machine);
          VBoxManager.CreateNewVm(vmName, 0, mediumAttachment, fastbootAttachment);
        }
        else
        {
          VBoxManager.InternalOpenMachine(cloneFromVm, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
          mediumAttachment = VBoxManager.GetMediumAttachments(VBoxManager.mMachine, ref controllerName);
          IMediumAttachment fastbootAttachment = VBoxManager.GetFastbootAttachment(VBoxManager.mMachine);
          for (int index = 0; index < mediumAttachment.Length; ++index)
          {
            IMedium medium = mediumAttachment[index].Medium;
            if (mediumAttachment[index].Type == DeviceType.DeviceType_HardDisk)
            {
              Logger.Info("Skipping prebundled and root for detachment while clone");
              if (VBoxManager.IsSDCardPresent)
              {
                if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 3)
                  continue;
              }
              else if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 2)
                continue;
              int port = mediumAttachment[index].Port;
              int device = mediumAttachment[index].Device;
              VBoxManager.mMachine.DetachDevice(controllerName, port, device);
              VBoxManager.mMachine.SaveSettings();
              Logger.Debug("Detached media {0} from VM {1}", (object) medium.Name, (object) cloneFromVm);
            }
          }
          VBoxManager.InternalCloseMachine();
          VBoxManager.CreateNewVm(vmName, 1, mediumAttachment, fastbootAttachment);
          VBoxManager.InternalOpenMachine(cloneFromVm, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
          for (int index = 0; index < mediumAttachment.Length; ++index)
          {
            IMedium medium = mediumAttachment[index].Medium;
            if (mediumAttachment[index].Type == DeviceType.DeviceType_HardDisk)
            {
              Logger.Info("Skipping root for attachment while clone");
              if (VBoxManager.IsSDCardPresent)
              {
                if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 3)
                  continue;
              }
              else if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 2)
                continue;
              int port = mediumAttachment[index].Port;
              int device = mediumAttachment[index].Device;
              string mediumName;
              if (((IEnumerable<string>) medium.Name.Split('_')).Count<string>() != 2)
              {
                if (((IEnumerable<string>) medium.Name.Split('_')).Count<string>() != 2 || !VBoxManager.IsSDCardPresent)
                {
                  int num = int.Parse(medium.Name.Split('_')[2].Split('.')[0]) + 1;
                  mediumName = medium.Name.Substring(0, medium.Name.LastIndexOf('_') + 1) + num.ToString() + Path.GetExtension(medium.Name);
                  goto label_22;
                }
              }
              mediumName = Path.GetFileNameWithoutExtension(medium.Name) + "_1" + Path.GetExtension(medium.Name);
label_22:
              string targetLocation = Path.Combine(RegistryStrings.DataDir, cloneFromVm);
              IMedium aMedium = VBoxManager.CloneMedium(medium, targetLocation, mediumName);
              VBoxManager.mMachine.AttachDevice(controllerName, port, device, DeviceType.DeviceType_HardDisk, aMedium);
              VBoxManager.mMachine.SaveSettings();
              Logger.Info("Detached media {0} from VM {1}", (object) medium.Name, (object) cloneFromVm);
            }
          }
          VBoxManager.InternalCloseMachine();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Cloning Failed: exception {0}", (object) ex.ToString());
        VBoxManager.AttachMediumsWhenCloningFailed(cloneFromVm, mediumAttachment, controllerName);
        return false;
      }
      return true;
    }

    private static void AttachMediumsWhenCloningFailed(
      string vmName,
      IMediumAttachment[] mediumAttachment,
      string controllerName)
    {
      try
      {
        VBoxManager.InternalOpenMachine(vmName, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
        string targetLocation = Path.Combine(RegistryStrings.DataDir, "Android");
        for (int index = 0; index < mediumAttachment.Length; ++index)
        {
          IMedium medium = mediumAttachment[index].Medium;
          if (mediumAttachment[index].Type == DeviceType.DeviceType_HardDisk && mediumAttachment[index].Port != 0 && mediumAttachment[index].Port != 3)
          {
            int port = mediumAttachment[index].Port;
            int device = mediumAttachment[index].Device;
            string mediumName;
            if (medium.Name == "Data_0.vdi" || medium.Name == "SDCard_0.vdi" || VBoxManager.IsSDCardPresent)
            {
              mediumName = Path.GetFileNameWithoutExtension(medium.Name) + "_1" + Path.GetExtension(medium.Name);
            }
            else
            {
              int num = int.Parse(medium.Name.Split('_')[2].Split('.')[0]) + 1;
              mediumName = medium.Name.Substring(0, medium.Name.LastIndexOf('_') + 1) + num.ToString() + Path.GetExtension(medium.Name);
            }
            IMedium aMedium = VBoxManager.CloneMedium(medium, targetLocation, mediumName);
            VBoxManager.mMachine.AttachDevice(controllerName, port, device, DeviceType.DeviceType_HardDisk, aMedium);
            VBoxManager.mMachine.SaveSettings();
            Logger.Info("Attached media {0} from VM {1}", (object) medium.Name, (object) vmName);
          }
        }
        VBoxManager.InternalCloseMachine();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in attaching data and sdcard to main instance" + ex.ToString());
      }
    }

    private static bool PrepareVMConfig(int newVmId)
    {
      string str1 = "Android_" + newVmId.ToString();
      if (!VBoxManager.InstallVmConfig(str1))
        return false;
      string dataDir = RegistryStrings.DataDir;
      string path1 = Path.Combine(dataDir, str1);
      string str2 = Path.Combine(path1, str1 + ".bstk");
      Path.Combine(path1, str1 + ".bstk.in");
      Path.Combine(dataDir, str1);
      string xml = File.ReadAllText(str2);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
      nsmgr.AddNamespace("bstk", "http://www.virtualbox.org/");
      XmlNode xmlNode1 = xmlDocument.SelectSingleNode("descendant::bstk:Machine", nsmgr);
      Logger.Info("Value of m/c uuid is {0}", (object) xmlNode1.Attributes["uuid"].Value);
      string str3 = Guid.NewGuid().ToString();
      xmlNode1.Attributes["uuid"].Value = "{" + str3 + "}";
      xmlNode1.Attributes["name"].Value = str1;
      List<XmlNode> xmlNodeList = new List<XmlNode>();
      foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:MediaRegistry//bstk:HardDisks", nsmgr))
      {
        foreach (XmlNode childNode in selectNode.ChildNodes)
          xmlNodeList.Add(childNode);
        foreach (XmlNode oldChild in xmlNodeList)
          selectNode.RemoveChild(oldChild);
        xmlNodeList.Clear();
      }
      Logger.Info("Successfully deleted the media registries");
      foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:StorageControllers//bstk:StorageController", nsmgr))
      {
        if (selectNode.Attributes["name"].Value == "SATA")
        {
          Logger.Info("Childnode is {0}", (object) selectNode.Name);
          XmlNodeList childNodes = selectNode.ChildNodes;
          int count = childNodes.Count;
          for (int index = 0; index < count; ++index)
          {
            XmlNode xmlNode2 = childNodes[index];
            Logger.Info("Subchildnode is {0}, type is {1}", (object) xmlNode2.Name, (object) xmlNode2.Attributes["type"].Value);
            if (xmlNode2.Name.Equals("AttachedDevice"))
            {
              Logger.Info("Not removing entry of root from preparevmconfig");
              if (!xmlNode2.Attributes["port"].Value.Equals("0") && !xmlNode2.Attributes["port"].Value.Equals("2") && (xmlNode2.Attributes["type"] != null && xmlNode2.Attributes["type"].Value.Equals("HardDisk")))
                xmlNodeList.Add(xmlNode2);
            }
          }
          foreach (XmlNode oldChild in xmlNodeList)
            selectNode.RemoveChild(oldChild);
          xmlNodeList.Clear();
          if (VBoxManager.IsSDCardPresent)
          {
            foreach (XmlNode xmlNode2 in childNodes)
            {
              if (xmlNode2.Attributes["port"].Value.Equals("2"))
              {
                Logger.Info("Changing port of prebundled to 3");
                xmlNode2.Attributes["port"].Value = "3";
                break;
              }
            }
            selectNode.Attributes["PortCount"].Value = "4";
            break;
          }
          break;
        }
      }
      foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:Hardware//bstk:Memory", nsmgr))
      {
        if (selectNode.Name == "Memory")
          selectNode.Attributes["RAMSize"].Value = Utils.GetAndroidVMMemory(true).ToString();
      }
      xmlDocument.Save(str2);
      return true;
    }

    public static bool InstallVmConfig(string vmName)
    {
      Logger.Info("InstallVmConfig()");
      string installDir = RegistryStrings.InstallDir;
      string dataDir = RegistryStrings.DataDir;
      string path1_1 = Path.Combine(dataDir, vmName);
      string path1_2 = Path.Combine(dataDir, "Android");
      string filename = Path.Combine(path1_1, vmName + ".bstk");
      string path = Path.Combine(path1_2, "Android.bstk.in");
      Path.Combine(path1_1, vmName + ".bstk.in");
      string str1 = (string) null;
      try
      {
        using (StreamReader streamReader = new StreamReader(path))
          str1 = streamReader.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot read '" + path + "': " + ex?.ToString());
        return false;
      }
      string str2 = str1.Replace("@@HD_PLUS_DEVICES_DLL_PATH@@", SecurityElement.Escape(Path.Combine(installDir, "HD-Plus-Devices.dll")));
      string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      string str3;
      if (!string.IsNullOrEmpty(folderPath1))
      {
        string newValue = string.Format("<SharedFolder name=\"Documents\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", (object) SecurityElement.Escape(folderPath1));
        str3 = str2.Replace("@@USER_DOCUMENTS_FOLDER@@", newValue);
      }
      else
        str3 = str2.Replace("@@USER_DOCUMENTS_FOLDER@@", "");
      string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      string str4;
      if (!string.IsNullOrEmpty(folderPath2))
      {
        string newValue = string.Format("<SharedFolder name=\"Pictures\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", (object) SecurityElement.Escape(folderPath2));
        str4 = str3.Replace("@@USER_PICTURES_FOLDER@@", newValue);
      }
      else
        str4 = str3.Replace("@@USER_PICTURES_FOLDER@@", "");
      string xml = str4.Replace("@@INPUT_MAPPER_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\InputMapper"))).Replace("@@BST_SHARED_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\SharedFolder"))).Replace("@@BST_VM_MEMORY_SIZE@@", SecurityElement.Escape(Utils.GetAndroidVMMemory(true).ToString()));
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        xmlDocument.Save(filename);
      }
      catch (Exception ex)
      {
        Logger.Info("Cannot write '" + filename + "': " + ex?.ToString());
        return false;
      }
      return true;
    }

    private static void InternalOpenMachine(
      string vmName,
      IVirtualBox sVirtualBox,
      IVirtualBoxClient sVirtualBoxClient,
      ref Session mSession,
      ref IMachine mMachine,
      ref IConsole mConsole)
    {
      Logger.Debug("In InternalOpenMachine");
      mSession = sVirtualBoxClient.Session;
      sVirtualBox.FindMachine(vmName).LockMachine(mSession, LockType.LockType_VM);
      mConsole = mSession.Console;
      mMachine = mConsole.Machine;
    }

    private static void InternalCloseMachine()
    {
      Logger.Info("In InternalCloseMachine");
      if (VBoxManager.mSession != null)
        Logger.Debug("Session state is {0}", (object) VBoxManager.mSession.State);
      if (VBoxManager.mSession == null)
        return;
      if (VBoxManager.mSession.State == SessionState.SessionState_Unlocked)
        return;
      try
      {
        VBoxManager.mSession.UnlockMachine();
        Logger.Debug("Successfully Unlocked machine {0}", (object) VBoxManager.mMachine.Name);
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not unlock the machine due to exception {0}", (object) ex.ToString());
      }
    }

    private static IMedium GetFactoryDefaultMedium(IMedium srcMedium)
    {
      IMedium[] hardDisks = VBoxManager.sVirtualBox.HardDisks;
      try
      {
        IMedium[] children = srcMedium.Children;
        string str = Path.Combine(Path.GetDirectoryName(srcMedium.Location), Path.GetFileNameWithoutExtension(srcMedium.Name) + Path.GetExtension(srcMedium.Location));
        Logger.Info("Diff disk name is {0}", (object) str);
        for (int index = 0; index < children.Length; ++index)
        {
          Logger.Debug("Child {0} is {1}, location is {2}", (object) index, (object) children[index].Name, (object) children[index].Location);
          if (children[index].Location.Equals(str) && children[index].MachineIds == null)
            return children[index];
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Got exception wile trying to find the default medium");
        Logger.Info(ex.ToString());
      }
      return srcMedium.Type == MediumType.MediumType_Normal && srcMedium.Children.Length != 0 ? srcMedium : (IMedium) null;
    }

    private static IMedium CloneMedium(
      IMedium srcMedium,
      string targetLocation,
      string mediumName)
    {
      Logger.Info("In CloneMedium for medium {0}", (object) mediumName);
      MediumVariant[] aVariant = new MediumVariant[1]
      {
        MediumVariant.MediumVariant_Diff
      };
      IMedium medium = VBoxManager.sVirtualBox.CreateMedium("vdi", Path.Combine(targetLocation, mediumName), AccessMode.AccessMode_ReadWrite, DeviceType.DeviceType_HardDisk);
      IProgress diffStorage = srcMedium.CreateDiffStorage(medium, aVariant);
      while (diffStorage.Completed == 0)
        Thread.Sleep(10);
      Logger.Info("Able to create target medium {0}", (object) medium.Name);
      return medium;
    }

    private static IMediumAttachment[] GetMediumAttachments(
      IMachine machine,
      ref string controllerName)
    {
      IMediumAttachment[] attachmentsOfController;
      try
      {
        attachmentsOfController = machine.GetMediumAttachmentsOfController("SCSI");
        controllerName = "SCSI";
        if (attachmentsOfController == null || attachmentsOfController.Length == 0)
        {
          Logger.Debug("SCSI controller doesn't exist {0}. Trying to get attachments on SATA");
          attachmentsOfController = machine.GetMediumAttachmentsOfController("SATA");
          controllerName = "SATA";
        }
        else
        {
          Logger.Debug("Found SCSI controller");
          controllerName = "SCSI";
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Neither SCSI nor SATA controller exists {0}", (object) ex.ToString());
        throw;
      }
      return attachmentsOfController;
    }

    private static IMediumAttachment GetFastbootAttachment(IMachine machine)
    {
      try
      {
        return machine.GetMediumAttachment("IDE", 0, 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Could not find fastboot medium attachment", (object) ex.ToString());
        throw;
      }
    }

    private static void CreateNewVm(
      string vmName,
      int isClone,
      IMediumAttachment[] mediumAttachment,
      IMediumAttachment fastbootAttachment)
    {
      Logger.Info("In CreatenewVm");
      string dataDir = RegistryStrings.DataDir;
      Path.Combine(dataDir, "Android");
      string str = Path.Combine(dataDir, vmName);
      int int32 = Convert.ToInt32(vmName.Split('_')[1]);
      VBoxManager.mVmId = int32;
      VBoxManager.PrepareVMConfig(int32);
      try
      {
        IMachine aMachine = VBoxManager.sVirtualBox.OpenMachine(Path.Combine(str, vmName + ".bstk"));
        VBoxManager.sVirtualBox.RegisterMachine(aMachine);
        Logger.Info("Registering vm {0} successful", (object) vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Unable to register a VM {0}", (object) vmName);
        Logger.Error(ex.ToString());
        return;
      }
      VBoxManager.InternalOpenMachine(vmName, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
      try
      {
        for (int index = 0; index < mediumAttachment.Length; ++index)
        {
          IMedium medium = mediumAttachment[index].Medium;
          if (mediumAttachment[index].Type == DeviceType.DeviceType_HardDisk)
          {
            if (VBoxManager.IsSDCardPresent)
            {
              if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 3)
                continue;
            }
            else if (mediumAttachment[index].Port == 0 || mediumAttachment[index].Port == 2)
              continue;
            int port = mediumAttachment[index].Port;
            int device = mediumAttachment[index].Device;
            IMedium aMedium = isClone != 0 ? VBoxManager.CloneMedium(medium, str, Path.GetFileNameWithoutExtension(medium.Base.Name) + "_" + int32.ToString() + Path.GetExtension(medium.Base.Name)) : VBoxManager.CloneMedium(medium.Base, str, Path.GetFileNameWithoutExtension(medium.Base.Name) + "_" + int32.ToString() + Path.GetExtension(medium.Base.Name));
            VBoxManager.mMachine.AttachDevice(mediumAttachment[index].Controller, port, device, DeviceType.DeviceType_HardDisk, aMedium);
            VBoxManager.mMachine.SaveSettings();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed cloning...");
        Logger.Error(ex.ToString());
        VBoxManager.InternalCloseMachine();
        IMedium[] aMedia = VBoxManager.mMachine.Unregister(CleanupMode.CleanupMode_Full);
        VBoxManager.mMachine.DeleteConfig(aMedia);
        return;
      }
      VBoxManager.InternalCloseMachine();
    }

    private static void CopyAppsJson(string vmName, string cloneFromVm)
    {
      string str1 = Path.Combine(RegistryStrings.DataDir, string.Format("UserData\\Gadget\\apps_{0}.json", (object) cloneFromVm));
      string destFileName1 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\apps_" + vmName + ".json");
      if (File.Exists(str1))
        File.Copy(str1, destFileName1, true);
      string str2 = Path.Combine(RegistryStrings.DataDir, string.Format("UserData\\Gadget\\requirements_{0}.json", (object) cloneFromVm));
      string destFileName2 = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget\\requirements_" + vmName + ".json");
      if (!File.Exists(str2))
        return;
      File.Copy(str2, destFileName2, true);
    }

    private static void DeserializeBstkFile(
      string vboxFilePath,
      ref string oldDataVdiUUid,
      ref string oldSdCardVdiUUid)
    {
      FileStream fileStream = File.OpenRead(vboxFilePath);
      foreach (HardDisk hardDisk in ((BlueStacks.VBoxUtils.VirtualBox) new XmlSerializer(typeof (BlueStacks.VBoxUtils.VirtualBox)).Deserialize((Stream) fileStream)).Machine.MediaRegistry.HardDisks.HardDisk)
      {
        if (hardDisk.Location.Equals("Data.vdi", StringComparison.InvariantCultureIgnoreCase))
          oldDataVdiUUid = hardDisk.Uuid.Replace("{", "").Replace("}", "");
        else if (hardDisk.Location.Equals("SDCard.vdi", StringComparison.InvariantCultureIgnoreCase))
          oldSdCardVdiUUid = hardDisk.Uuid.Replace("{", "").Replace("}", "");
      }
      fileStream.Close();
    }

    private static void DeleteStorageIfPossible(IMedium medium, bool recurse = true)
    {
      Logger.Info("In DeleteStorageIfPossible");
      if (medium == null)
        return;
      try
      {
        if (medium.Children.Length == 0)
        {
          IMedium parent = medium.Parent;
          IProgress progress = medium.DeleteStorage();
          while (progress.Completed == 0)
            Thread.Sleep(5);
          Logger.Info("Successfully deleted storage");
          if (parent != null)
            Logger.Info("Now trying to delete storage {0}, UUID {1}", (object) parent.Name, (object) parent.Id);
          if (!recurse)
            return;
          VBoxManager.DeleteStorageIfPossible(parent, true);
        }
        else
          Logger.Info("Number of children {0}", (object) medium.Children.Length);
      }
      catch (Exception ex)
      {
        Logger.Warning("Got exception while deleting storage");
        Logger.Warning(ex.ToString());
      }
    }

    private static IMedium GetBaseMediumForFactoryReset(IMedium medium)
    {
      if (new List<string>()
      {
        "Root.vdi",
        "SDCard.vdi",
        "Data.vdi"
      }.Contains<string>(medium.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        return medium;
      IMedium[] hardDisks = VBoxManager.sVirtualBox.HardDisks;
      string str = medium.Name.Split('_')[0] + ".vdi";
      for (int index = 0; index < hardDisks.Length; ++index)
      {
        if (hardDisks[index].Name.Equals(str))
        {
          Logger.Info("Found a registered base disk for the disk {0}", (object) medium.Name);
          return hardDisks[index];
        }
      }
      return (IMedium) null;
    }

    private static void DoFactoryReset()
    {
      try
      {
        string controllerName = (string) null;
        IMediumAttachment[] mediumAttachments = VBoxManager.GetMediumAttachments(VBoxManager.mMachine, ref controllerName);
        for (int index = 0; index < mediumAttachments.Length; ++index)
        {
          IMedium medium = mediumAttachments[index].Medium;
          if (mediumAttachments[index].Type == DeviceType.DeviceType_HardDisk)
          {
            int port = mediumAttachments[index].Port;
            int device = mediumAttachments[index].Device;
            IMedium mediumForFactoryReset = VBoxManager.GetBaseMediumForFactoryReset(medium.Base);
            IMedium aMedium = (IMedium) null;
            if (mediumForFactoryReset != null)
              aMedium = VBoxManager.GetFactoryDefaultMedium(mediumForFactoryReset);
            else
              Logger.Info("Could not find the base medium for {0}", (object) medium.Base.Name);
            if (aMedium != null)
            {
              VBoxManager.mMachine.DetachDevice(controllerName, port, device);
              VBoxManager.mMachine.SaveSettings();
              VBoxManager.mMachine.AttachDevice(controllerName, port, device, DeviceType.DeviceType_HardDisk, aMedium);
              VBoxManager.mMachine.SaveSettings();
              Logger.Info("Successfully attached medium {0}", (object) aMedium.Name);
              VBoxManager.DeleteStorageIfPossible(medium, false);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in factory reset" + ex.ToString());
        throw;
      }
    }

    internal static int CloneForFactoryReset(string vmName)
    {
      try
      {
        if (!VBoxManager.Init())
          return -12;
        VBoxManager.InternalOpenMachine(vmName, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
        VBoxManager.DoFactoryReset();
      }
      catch (Exception ex)
      {
        Logger.Error("Cloning failed for factory reset" + ex.ToString());
        return -16;
      }
      return 0;
    }

    private static void RecursiveRegistryCopy(
      RegistryKey srcKey,
      RegistryKey desKey,
      string vmType)
    {
      foreach (string valueName in srcKey.GetValueNames())
      {
        object obj = srcKey.GetValue(valueName);
        Logger.Info(string.Format("Copying key: {0}, value: {1}", (object) valueName, obj));
        desKey.SetValue(valueName, obj, srcKey.GetValueKind(valueName));
      }
      foreach (string subKeyName in srcKey.GetSubKeyNames())
      {
        Logger.Info("Opening SubKey {0}", (object) subKeyName);
        using (RegistryKey srcKey1 = srcKey.OpenSubKey(subKeyName, false))
        {
          RegistryKey subKey = desKey.CreateSubKey(subKeyName);
          VBoxManager.RecursiveRegistryCopy(srcKey1, subKey, vmType);
          subKey.Close();
          srcKey1.Close();
        }
      }
    }

    private static bool CopyVMRegistry(
      string vmName,
      string vmType,
      string cloneFromVm,
      string engineSettings)
    {
      try
      {
        int int32 = Convert.ToInt32(vmName.Split('_')[1]);
        RegistryKey srcKey = Registry.LocalMachine.OpenSubKey(RegistryManager.Instance.Guest[cloneFromVm].AndroidKeyPath, false);
        RegistryKey subKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.BaseKeyPath + "\\Guests\\" + vmName);
        int androidVmMemory = Utils.GetAndroidVMMemory(false);
        int num1 = 0;
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        int num2 = 1280;
        int num3 = 720;
        if (!string.IsNullOrEmpty(engineSettings))
        {
          try
          {
            JObject jobject = JObject.Parse(engineSettings);
            Logger.Info("settings received are : " + jobject.ToString(Newtonsoft.Json.Formatting.None));
            if (jobject.ContainsKey("ram"))
              androidVmMemory = int.Parse(jobject["ram"].ToString());
            if (jobject.ContainsKey("cpu"))
              num1 = int.Parse(jobject["cpu"].ToString());
            if (jobject.ContainsKey("abi"))
              empty1 = jobject["abi"].ToString();
            if (jobject.ContainsKey("dpi"))
              empty2 = jobject["dpi"].ToString();
            if (jobject.ContainsKey("resolutionwidth"))
              num2 = int.Parse(jobject["resolutionwidth"].ToString());
            if (jobject.ContainsKey("resolutionheight"))
              num3 = int.Parse(jobject["resolutionheight"].ToString());
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to parse engine settings. Ex : " + ex.ToString());
          }
        }
        VBoxManager.RecursiveRegistryCopy(srcKey, subKey, vmType);
        subKey.SetValue("Memory", (object) androidVmMemory);
        if (vmType == "fresh")
        {
          if (androidVmMemory == 1024)
            subKey.SetValue("PerformanceSettings", (object) PerformanceSetting.Low);
          else
            subKey.SetValue("PerformanceSettings", (object) PerformanceSetting.Custom);
          subKey.DeleteValue("ASTCOption", false);
          subKey.DeleteValue("IsHardwareAstcSupported", false);
        }
        Logger.Info("Copying Done");
        int num4 = 3;
        if (VBoxManager.IsSDCardPresent)
          num4 = 4;
        for (int index = 0; index < num4; ++index)
        {
          Logger.Info("Open regex");
          RegistryKey registryKey = subKey.OpenSubKey("BlockDevice\\" + index.ToString(), true);
          string str = (string) registryKey.GetValue("Path", (object) "");
          if (str.Contains(cloneFromVm))
            str = str.Replace(cloneFromVm, vmName);
          Logger.Info("updating BlockDevice\\{0} key Path: {1}", (object) index, (object) str);
          registryKey.SetValue("Path", (object) str);
          registryKey.Close();
        }
        RegistryKey registryKey1 = subKey.OpenSubKey("FrameBuffer\\0", true);
        registryKey1.SetValue("GuestWidth", (object) num2);
        registryKey1.SetValue("GuestHeight", (object) num3);
        Logger.Info("Updating default Guest width x height to {0}x{1}", (object) num2, (object) num3);
        RegistryKey registryKey2 = subKey.OpenSubKey("Config", true);
        Logger.Info("Updating Config key FrontendServerPort: {0}", (object) (int) registryKey2.GetValue("FrontendServerPort", (object) 2881));
        registryKey2.SetValue("FrontendServerPort", (object) (int) registryKey2.GetValue("FrontendServerPort", (object) 2881));
        Logger.Info("updating Config key bstandroidport: {0}", (object) ((int) registryKey2.GetValue("bstandroidport", (object) 9999) + int32 * 10));
        registryKey2.SetValue("bstandroidport", (object) ((int) registryKey2.GetValue("bstandroidport", (object) 9999) + int32 * 10));
        Logger.Info("Updating Config key bstadbport : {0}", (object) ((int) registryKey2.GetValue("BstAdbPort", (object) 5555) + int32 * 10));
        registryKey2.SetValue("BstAdbPort", (object) ((int) registryKey2.GetValue("BstAdbPort", (object) 5555) + int32 * 10));
        Logger.Info("updating Config key GlPort: {0}", (object) ((int) registryKey2.GetValue("GlPort", (object) 0) + int32 * 10));
        registryKey2.SetValue("GlPort", (object) ((int) registryKey2.GetValue("GlPort", (object) 0) + int32 * 10));
        Logger.Info("updating Config key HostSensorPort: {0}", (object) ((int) registryKey2.GetValue("HostSensorPort", (object) 0) + int32 * 10));
        registryKey2.SetValue("HostSensorPort", (object) ((int) registryKey2.GetValue("HostSensorPort", (object) 0) + int32 * 10));
        Logger.Info("updating Config key DisplayName: ");
        registryKey2.SetValue("DisplayName", (object) string.Format("{0}_{1}", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) int32));
        if (string.Equals(vmType, "fresh", StringComparison.InvariantCultureIgnoreCase))
        {
          Logger.Info("Updating Config key Eco mode FPS: {0}", (object) 5);
          registryKey2.SetValue("EcoModeFPS", (object) "5");
          Logger.Info("Updating Config key CanAccessWindowsFolder: {0}", (object) 1);
          registryKey2.SetValue("CanAccessWindowsFolder", (object) 1);
          Logger.Info("updating Config key ConfigSynced: {0}", (object) 0);
          registryKey2.SetValue("ConfigSynced", (object) 0);
          registryKey2.SetValue("ShowSchemeDeletePopup", (object) 1);
          registryKey2.SetValue("TouchSoundEnabled", (object) 1);
          registryKey2.SetValue("ShowBlueHighlighter", (object) 1);
          registryKey2.SetValue("EnableVSync", (object) 0);
          Logger.Info("Updating google signin details and popup visibility for fresh instance");
          if (Oem.Instance.IsResetSigninRegistryForFreshVM)
          {
            registryKey2.SetValue("IsOneTimeSetupDone", (object) 0);
            registryKey2.SetValue("IsGoogleSigninDone", (object) 0);
            registryKey2.SetValue("IsGoogleSigninPopupShown", (object) 0);
          }
          Logger.Info("updating Config key FPS :{0}", (object) (int) registryKey2.GetValue("CommonFPS", (object) 60));
          registryKey2.SetValue("FPS", (object) RegistryManager.Instance.CommonFPS);
          Logger.Info("updating Config key IsFreeFireInGameSettingsCustomized");
          registryKey2.SetValue("IsFreeFireInGameSettingsCustomized", (object) 1);
          Logger.Info("Resetting PUBG Game Settings");
          registryKey2.SetValue("GamingResolutionPubg", (object) "1");
          registryKey2.SetValue("DisplayQualityPubg", (object) "-1");
          registryKey2.SetValue("PUBGLaunchedCount", (object) 0);
          Logger.Info("Updating config key NativeGamepaState: Auto");
          registryKey2.SetValue("NativeGamepadState", (object) 2);
        }
        if (string.Equals(vmType, "clone", StringComparison.InvariantCultureIgnoreCase) && (int) registryKey2.GetValue("IsGoogleSigninDone", (object) 0) == 0)
        {
          Logger.Info("Updating google signin popup visibility for clone instance");
          registryKey2.SetValue("IsGoogleSigninPopupShown", (object) 0);
        }
        Logger.Info("updating Config key FirstLaunchDateTime: {0}", (object) "");
        registryKey2.SetValue("FirstLaunchDateTime", (object) "");
        int num5 = (int) registryKey2.GetValue("FPS", (object) RegistryManager.Instance.CommonFPS);
        registryKey2.Close();
        RegistryKey registryKey3 = subKey.OpenSubKey("Network\\0", true);
        string[] strArray1 = (string[]) registryKey3.GetValue("InboundRules", (object) null);
        for (int index = 0; index < (strArray1 != null ? strArray1.Length : 0); ++index)
        {
          string[] strArray2 = strArray1[index].Split(':');
          if (strArray2.Length >= 3)
          {
            int num6 = int.Parse(strArray2[2]) + int32 * 10;
            strArray1[index] = string.Format("{0}:{1}:{2}", (object) strArray2[0], (object) strArray2[1], (object) num6);
          }
        }
        Logger.Info("updating network\\0 key inboundrules: {0}", (object[]) strArray1);
        registryKey3.SetValue("inboundrules", (object) strArray1);
        RegistryKey registryKey4 = subKey.OpenSubKey("Network\\Redirect", true);
        registryKey4.SetValue("tcp/5555", (object) ((int) registryKey4.GetValue("tcp/5555", (object) 5555) + int32 * 10), RegistryValueKind.DWord);
        registryKey4.SetValue("tcp/6666", (object) ((int) registryKey4.GetValue("tcp/6666", (object) 6666) + int32 * 10), RegistryValueKind.DWord);
        registryKey4.SetValue("tcp/7777", (object) ((int) registryKey4.GetValue("tcp/7777", (object) 7777) + int32 * 10), RegistryValueKind.DWord);
        registryKey4.SetValue("tcp/9999", (object) ((int) registryKey4.GetValue("tcp/9999", (object) 8888) + int32 * 10), RegistryValueKind.DWord);
        registryKey4.SetValue("udp/12000", (object) ((int) registryKey4.GetValue("udp/12000", (object) 12000) + int32 * 10), RegistryValueKind.DWord);
        registryKey4.Close();
        RegistryKey registryKey5 = subKey.OpenSubKey("Config", true);
        if (num1 == 0)
          registryKey5.DeleteValue("VCPUs", false);
        else
          registryKey5.SetValue("VCPUs", (object) num1, RegistryValueKind.DWord);
        registryKey5.Close();
        string str1 = (string) subKey.GetValue("BootParameters", (object) "");
        if (!str1.IsNullOrWhiteSpace())
        {
          Dictionary<string, string> dictionary = ((IEnumerable<string>) str1.Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
          if (dictionary.ContainsKey("fps"))
            dictionary["fps"] = num5.ToString();
          if (!string.IsNullOrEmpty(empty2))
            dictionary["DPI"] = empty2;
          if (dictionary.ContainsKey("pcode") && string.Equals(vmType, "fresh", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(RegistryManager.Instance.DeviceProfileFromCloud))
            dictionary["pcode"] = RegistryManager.Instance.DeviceProfileFromCloud;
          if (!string.IsNullOrEmpty(empty1))
            dictionary["abivalue"] = empty1;
          string str2 = string.Join(" ", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)).ToList<string>().ToArray());
          subKey.SetValue("BootParameters", (object) str2);
          if (string.Equals(vmType, "fresh", StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Info("updating Sidebar Visibilty key");
            subKey.SetValue("IsSidebarVisible", (object) 1);
          }
        }
        subKey.Close();
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Registry Copy: {0}", (object) ex.ToString());
        return false;
      }
    }

    private static IMedium GetMediumByName(string diskName)
    {
      IMedium[] hardDisks = VBoxManager.sVirtualBox.HardDisks;
      Logger.Info("Printing the length of disks attached for disk " + diskName + " " + hardDisks.Length.ToString());
      for (int index = 0; index < hardDisks.Length; ++index)
      {
        Logger.Info("Disk Name " + hardDisks[index].Name);
        if (hardDisks[index].Name.Equals(diskName, StringComparison.InvariantCultureIgnoreCase))
          return hardDisks[index];
      }
      return (IMedium) null;
    }

    public static int UpgradeMachine()
    {
      try
      {
        string dataDir = RegistryStrings.DataDir;
        string path1_1 = Path.Combine(dataDir, "Android");
        string aLocation1 = Path.Combine(path1_1, "fastboot.vdi");
        if (!VBoxManager.Init())
          return -12;
        foreach (string str1 in new List<string>((IEnumerable<string>) RegistryManager.Instance.VmList))
        {
          if (!str1.Equals("Android", StringComparison.OrdinalIgnoreCase))
          {
            VBoxManager.InternalOpenMachine(str1, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
            string controllerName = (string) null;
            VBoxManager.GetMediumAttachments(VBoxManager.mMachine, ref controllerName);
            string path1_2 = Path.Combine(dataDir, str1);
            Logger.Info("Cloning fastboot medium");
            string str2 = (string) null;
            IMedium aMedium = VBoxManager.CloneMediumFull(Path.Combine(path1_1, "fastboot.vdi"), Path.Combine(path1_2, "fastboot.vdi"));
            Logger.Info("Cloned fastboot image's uuid is {0} for {1}", (object) str2, (object) str1);
            VBoxManager.mMachine.AttachDevice("IDE", 0, 0, DeviceType.DeviceType_HardDisk, aMedium);
            VBoxManager.mMachine.SaveSettings();
            IMedium mediumByName = VBoxManager.GetMediumByName("Root.vdi");
            if (mediumByName == null)
            {
              Logger.Info("for Vm " + str1);
              return -14;
            }
            string aLocation2 = Path.Combine(Path.Combine(dataDir, str1), "Root.vdi");
            IMedium medium = VBoxManager.sVirtualBox.CreateMedium("vdi", aLocation2, AccessMode.AccessMode_ReadWrite, DeviceType.DeviceType_HardDisk);
            MediumVariant[] aVariant = new MediumVariant[1]
            {
              MediumVariant.MediumVariant_Diff
            };
            IProgress diffStorage = mediumByName.CreateDiffStorage(medium, aVariant);
            while (diffStorage.Completed == 0)
              Thread.Sleep(5);
            VBoxManager.mMachine.AttachDevice(controllerName, 0, 0, DeviceType.DeviceType_HardDisk, medium);
            VBoxManager.mMachine.SaveSettings();
            VBoxManager.mMachine.AttachDevice(controllerName, 3, 0, DeviceType.DeviceType_HardDisk, medium);
            VBoxManager.mMachine.SaveSettings();
            VBoxManager.InternalCloseMachine();
          }
        }
        IMedium mediumByName1 = VBoxManager.GetMediumByName("Root.vdi");
        string controllerName1 = (string) null;
        VBoxManager.InternalOpenMachine("Android", VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
        VBoxManager.GetMediumAttachments(VBoxManager.mMachine, ref controllerName1);
        if (mediumByName1 == null)
        {
          Logger.Info("for Android");
          return -14;
        }
        Logger.Info("Attaching Fastboot for Android");
        IMedium aMedium1 = VBoxManager.sVirtualBox.OpenMedium(aLocation1, DeviceType.DeviceType_HardDisk, AccessMode.AccessMode_ReadOnly, 0);
        VBoxManager.mMachine.AttachDevice("IDE", 0, 0, DeviceType.DeviceType_HardDisk, aMedium1);
        VBoxManager.mMachine.SaveSettings();
        Logger.Info("Attaching Root.vdi for Android");
        VBoxManager.mMachine.AttachDevice(controllerName1, 0, 0, DeviceType.DeviceType_HardDisk, mediumByName1);
        VBoxManager.mMachine.SaveSettings();
        VBoxManager.mMachine.SaveSettings();
        VBoxManager.InternalCloseMachine();
      }
      catch (Exception ex)
      {
        Logger.Error("Got exception while trying to upgrade instance. ERR: {0}", (object) ex.ToString());
        return -5;
      }
      return 0;
    }

    public static int CreateMachine(
      string vmName,
      string vmType,
      string cloneFromVm,
      string engineSettings)
    {
      try
      {
        if (!VBoxManager.Init())
          return -12;
        VBoxManager.CheckForSdCardFilePresent();
        if (!VBoxManager.Clone(cloneFromVm, vmName, vmType))
        {
          Logger.Error("Create Machine failed!!");
          return -2;
        }
        if (string.Equals(vmType, "clone", StringComparison.InvariantCultureIgnoreCase))
          VBoxManager.CopyAppsJson(vmName, cloneFromVm);
        if (!VBoxManager.CopyVMRegistry(vmName, vmType, cloneFromVm, engineSettings))
        {
          Logger.Error("Create Machine failed!!");
          return -3;
        }
        if (string.Equals(vmType, "clone", StringComparison.InvariantCultureIgnoreCase))
          VBoxManager.AddAppConfigrationEntryForNewVm(vmName, cloneFromVm);
        VBoxManager.AddVmNameToVmList(vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Create Machine failed!!, err :{0}", (object) ex.ToString());
        Logger.Info("Number of machines registered {0}", (object) VBoxManager.sVirtualBox.Machines.Length);
        for (int index = 0; index < VBoxManager.sVirtualBox.Machines.Length; ++index)
          Logger.Info("Machine {0} {1}", (object) index, (object) VBoxManager.sVirtualBox.Machines[index].Name);
        return -5;
      }
      VBoxManager.ResetSharedFoldersForFreshAndClone(vmName);
      return 0;
    }

    public static int ResetSharedFoldersForFreshAndClone(string vmName)
    {
      RegistryKey registryKey = (RegistryKey) null;
      try
      {
        registryKey = Registry.LocalMachine.OpenSubKey(RegistryManager.Instance.BaseKeyPath + "\\Guests\\" + vmName + "\\Config");
        bool canAccessSharedFolders = (int) registryKey.GetValue("CanAccessWindowsFolder", (object) 1) == 1;
        return VBoxManager.ResetSharedFolders(vmName, canAccessSharedFolders);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in ResetSharedFoldersForFreshAndClone: " + vmName + " : " + ex.Message);
        return -21;
      }
      finally
      {
        registryKey?.Close();
      }
    }

    internal static int RemoveDisk(string diskName)
    {
      Logger.Info("In RemoveDisk removing disk: " + diskName);
      try
      {
        if (!VBoxManager.Init())
          return -12;
        VBoxManager.InternalOpenMachine("Android", VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
        string controllerName = (string) null;
        IMediumAttachment[] mediumAttachments = VBoxManager.GetMediumAttachments(VBoxManager.mMachine, ref controllerName);
        for (int index = 0; index < mediumAttachments.Length; ++index)
        {
          IMedium medium = mediumAttachments[index].Medium;
          if (mediumAttachments[index].Type == DeviceType.DeviceType_HardDisk && medium.Name.Equals(diskName, StringComparison.InvariantCultureIgnoreCase))
          {
            int port = mediumAttachments[index].Port;
            int device = mediumAttachments[index].Device;
            VBoxManager.mMachine.DetachDevice(controllerName, port, device);
            VBoxManager.mMachine.SaveSettings();
            Logger.Info("Removed disk {0}", (object) diskName);
            break;
          }
        }
        VBoxManager.InternalCloseMachine();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in removing disk err: " + ex.ToString());
        return -20;
      }
      return 0;
    }

    public static int ResetSharedFolders(string vmName)
    {
      bool accessWindowsFolder = RegistryManager.Instance.Guest[vmName].CanAccessWindowsFolder;
      return VBoxManager.ResetSharedFolders(vmName, accessWindowsFolder);
    }

    private static int ResetSharedFolders(string vmName, bool canAccessSharedFolders)
    {
      try
      {
        Logger.Info("ResetSharedFolders: " + vmName);
        if (!VBoxManager.Init())
          return -12;
        VBoxManager.InternalOpenMachine(vmName, VBoxManager.sVirtualBox, VBoxManager.sVirtualBoxClient, ref VBoxManager.mSession, ref VBoxManager.mMachine, ref VBoxManager.mConsole);
        if (canAccessSharedFolders)
        {
          VBoxManager.mMachine.CreateSharedFolder("Documents", SecurityElement.Escape(Environment.GetFolderPath(Environment.SpecialFolder.Personal)), 1, 0);
          VBoxManager.mMachine.CreateSharedFolder("Pictures", SecurityElement.Escape(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)), 1, 0);
        }
        else
        {
          VBoxManager.mMachine.RemoveSharedFolder("Documents");
          VBoxManager.mMachine.RemoveSharedFolder("Pictures");
        }
        VBoxManager.mMachine.SaveSettings();
        return 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in ResetSharedFolders: " + vmName + " : " + ex.Message);
        return -21;
      }
      finally
      {
        try
        {
          VBoxManager.InternalCloseMachine();
        }
        catch (Exception ex)
        {
          Logger.Error("Error in ResetSharedFolders->InternalCloseMachine: " + vmName + " : " + ex.Message);
        }
      }
    }
  }
}
