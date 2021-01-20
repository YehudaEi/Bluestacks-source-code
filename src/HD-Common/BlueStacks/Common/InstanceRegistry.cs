// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InstanceRegistry
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System;

namespace BlueStacks.Common
{
  public class InstanceRegistry
  {
    private string mBaseKeyPath = "";
    private string mBlockDeviceKeyPath = "";
    private string mBlockDevice0KeyPath = "";
    private string mBlockDevice1KeyPath = "";
    private string mBlockDevice2KeyPath = "";
    private string mBlockDevice3KeyPath = "";
    private string mBlockDevice4KeyPath = "";
    private string mVmConfigKeyPath = "";
    private string mFrameBufferKeyPath = "";
    private string mFrameBuffer0KeyPath = "";
    private string mNetworkKeyPath = "";
    private string mNetwork0KeyPath = "";
    private string mNetworkRedirectKeyPath = "";
    private string mSharedFolderKeyPath = "";
    private string mSharedFolder0KeyPath = "";
    private string mSharedFolder1KeyPath = "";
    private string mSharedFolder2KeyPath = "";
    private string mSharedFolder3KeyPath = "";
    private string mSharedFolder4KeyPath = "";
    private string mSharedFolder5KeyPath = "";
    private string mVmId;

    public InstanceRegistry(string vmId, string oem = "bgp")
    {
      this.mVmId = vmId;
      if (oem == null)
        oem = "bgp";
      this.Init(oem);
    }

    private void Init(string oem = "bgp")
    {
      this.mBaseKeyPath = "Software\\BlueStacks" + (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem) + RegistryManager.UPGRADE_TAG;
      this.AndroidKeyPath = this.mBaseKeyPath + "\\Guests\\" + this.mVmId;
      this.mBlockDeviceKeyPath = this.AndroidKeyPath + "\\BlockDevice";
      this.mBlockDevice0KeyPath = this.AndroidKeyPath + "\\BlockDevice\\0";
      this.mBlockDevice1KeyPath = this.AndroidKeyPath + "\\BlockDevice\\1";
      this.mBlockDevice2KeyPath = this.AndroidKeyPath + "\\BlockDevice\\2";
      this.mBlockDevice3KeyPath = this.AndroidKeyPath + "\\BlockDevice\\3";
      this.mBlockDevice4KeyPath = this.AndroidKeyPath + "\\BlockDevice\\4";
      this.mVmConfigKeyPath = this.AndroidKeyPath + "\\Config";
      this.mFrameBufferKeyPath = this.AndroidKeyPath + "\\FrameBuffer";
      this.mFrameBuffer0KeyPath = this.AndroidKeyPath + "\\FrameBuffer\\0";
      this.mNetworkKeyPath = this.AndroidKeyPath + "\\Network";
      this.mNetwork0KeyPath = this.AndroidKeyPath + "\\Network\\0";
      this.mNetworkRedirectKeyPath = this.AndroidKeyPath + "\\Network\\Redirect";
      this.mSharedFolderKeyPath = this.AndroidKeyPath + "\\SharedFolder";
      this.mSharedFolder0KeyPath = this.AndroidKeyPath + "\\SharedFolder\\0";
      this.mSharedFolder1KeyPath = this.AndroidKeyPath + "\\SharedFolder\\1";
      this.mSharedFolder2KeyPath = this.AndroidKeyPath + "\\SharedFolder\\2";
      this.mSharedFolder3KeyPath = this.AndroidKeyPath + "\\SharedFolder\\3";
      this.mSharedFolder4KeyPath = this.AndroidKeyPath + "\\SharedFolder\\4";
      this.mSharedFolder5KeyPath = this.AndroidKeyPath + "\\SharedFolder\\5";
      RegistryUtils.InitKey(this.mBlockDevice0KeyPath);
      RegistryUtils.InitKey(this.mBlockDevice1KeyPath);
      RegistryUtils.InitKey(this.mBlockDevice2KeyPath);
      RegistryUtils.InitKey(this.mBlockDevice3KeyPath);
      RegistryUtils.InitKey(this.mBlockDevice4KeyPath);
      RegistryUtils.InitKey(this.mVmConfigKeyPath);
      RegistryUtils.InitKey(this.mFrameBuffer0KeyPath);
      RegistryUtils.InitKey(this.mNetwork0KeyPath);
      RegistryUtils.InitKey(this.mNetworkRedirectKeyPath);
      RegistryUtils.InitKey(this.mSharedFolder0KeyPath);
      RegistryUtils.InitKey(this.mSharedFolder1KeyPath);
      RegistryUtils.InitKey(this.mSharedFolder2KeyPath);
      RegistryUtils.InitKey(this.mSharedFolder3KeyPath);
      RegistryUtils.InitKey(this.mSharedFolder4KeyPath);
      RegistryUtils.InitKey(this.mSharedFolder5KeyPath);
    }

    public string AndroidKeyPath { get; private set; } = "";

    public int EmulatePortraitMode
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (EmulatePortraitMode), (object) -1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (EmulatePortraitMode), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int Depth
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (Depth), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (Depth), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int HideBootProgress
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (HideBootProgress), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (HideBootProgress), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int WindowWidth
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (WindowWidth), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (WindowWidth), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int WindowHeight
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (WindowHeight), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (WindowHeight), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GuestWidth
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (GuestWidth), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (GuestWidth), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GuestHeight
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, nameof (GuestHeight), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, nameof (GuestHeight), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int Memory
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (Memory), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (Memory), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsSidebarVisible
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (IsSidebarVisible), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (IsSidebarVisible), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsTopbarVisible
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (IsTopbarVisible), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (IsTopbarVisible), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsSidebarInDefaultState
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (IsSidebarInDefaultState), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (IsSidebarInDefaultState), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string Kernel
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (Kernel), (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (Kernel), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string Initrd
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (Initrd), (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (Initrd), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int DisableRobustness
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (DisableRobustness), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (DisableRobustness), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string VirtType
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (VirtType), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (VirtType), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BootParameters
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (BootParameters), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (BootParameters), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool ShowSidebarInFullScreen
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ShowSidebarInFullScreen), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ShowSidebarInFullScreen), (object) (!value ? 0 : 1), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice0Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice0KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice0KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice0Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice0KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice0KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice1Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice1KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice1KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice1Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice1KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice1KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice2Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice2KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice2KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice2Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice2KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice2KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice4Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice4KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice4KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string BlockDevice4Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mBlockDevice4KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mBlockDevice4KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string Locale
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (Locale), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (Locale), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int VCPUs
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (VCPUs), (object) Utils.GetRecommendedVCPUCount(this.mVmId == "Android"), RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (VCPUs), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string EnableConsoleAccess
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (EnableConsoleAccess), (object) string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (EnableConsoleAccess), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GlRenderMode
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GlRenderMode), (object) -1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GlRenderMode), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int FPS
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FPS), (object) 60, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FPS), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int ShowFPS
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ShowFPS), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ShowFPS), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int EnableHighFPS
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (EnableHighFPS), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (EnableHighFPS), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int EnableVSync
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (EnableVSync), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (EnableVSync), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GlMode
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GlMode), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GlMode), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int Camera
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (Camera), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (Camera), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int ConfigSynced
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ConfigSynced), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ConfigSynced), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int HScroll
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (HScroll), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (HScroll), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GpsMode
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GpsMode), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GpsMode), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int FileSystem
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FileSystem), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FileSystem), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int StopZygoteOnClose
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (StopZygoteOnClose), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (StopZygoteOnClose), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int FenceSyncType
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FenceSyncType), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FenceSyncType), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool CanAccessWindowsFolder
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (CanAccessWindowsFolder), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) == 1;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (CanAccessWindowsFolder), (object) (!value ? 0 : 1), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string EcoModeFPS
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (EcoModeFPS), (object) "5", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (EcoModeFPS), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int FrontendNoClose
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FrontendNoClose), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FrontendNoClose), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GpsSource
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GpsSource), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GpsSource), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string GpsLatitude
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GpsLatitude), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GpsLatitude), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string GpsLongitude
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GpsLongitude), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GpsLongitude), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GlPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GlPort), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GlPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string GamingResolutionPubg
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GamingResolutionPubg), (object) "1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GamingResolutionPubg), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string DisplayQualityPubg
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayQualityPubg), (object) "-1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayQualityPubg), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string GamingResolutionCOD
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GamingResolutionCOD), (object) "720", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GamingResolutionCOD), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string DisplayQualityCOD
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayQualityCOD), (object) "-1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayQualityCOD), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int HostSensorPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (HostSensorPort), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (HostSensorPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SoftControlBarHeightLandscape
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (SoftControlBarHeightLandscape), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (SoftControlBarHeightLandscape), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SoftControlBarHeightPortrait
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (SoftControlBarHeightPortrait), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (SoftControlBarHeightPortrait), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GrabKeyboard
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GrabKeyboard), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GrabKeyboard), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int DisableDWM
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (DisableDWM), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (DisableDWM), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int DisablePcIme
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (DisablePcIme), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (DisablePcIme), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int EnableBSTVC
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (EnableBSTVC), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (EnableBSTVC), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int ForceVMLegacyMode
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ForceVMLegacyMode), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ForceVMLegacyMode), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int FrontendServerPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FrontendServerPort), (object) 2881, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FrontendServerPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int BstAndroidPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (BstAndroidPort), (object) 9999, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (BstAndroidPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int BstAdbPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (BstAdbPort), (object) 5555, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (BstAdbPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int TriggerMemoryTrimThreshold
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (TriggerMemoryTrimThreshold), (object) 700, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (TriggerMemoryTrimThreshold), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int TriggerMemoryTrimTimerInterval
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (TriggerMemoryTrimTimerInterval), (object) 60000, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (TriggerMemoryTrimTimerInterval), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int UpdatedVersion
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (UpdatedVersion), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (UpdatedVersion), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int GPSAvailable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GPSAvailable), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GPSAvailable), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string OpenSensorDeviceId
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (OpenSensorDeviceId), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (OpenSensorDeviceId), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int HostForwardSensorPort
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (HostForwardSensorPort), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (HostForwardSensorPort), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string ImeSelected
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ImeSelected), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ImeSelected), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int RunAppProcessId
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (RunAppProcessId), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (RunAppProcessId), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string DisplayName
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayName), (object) string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (DisplayName), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string LastBootDate
    {
      get
      {
        string mVmConfigKeyPath = this.mVmConfigKeyPath;
        DateTime dateTime = DateTime.Now;
        dateTime = dateTime.Date;
        string shortDateString = dateTime.ToShortDateString();
        return (string) RegistryUtils.GetRegistryValue(mVmConfigKeyPath, nameof (LastBootDate), (object) shortDateString, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (LastBootDate), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsOneTimeSetupDone
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsOneTimeSetupDone), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsOneTimeSetupDone), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsMuted
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsMuted), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsMuted), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int Volume
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (Volume), (object) 5, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (Volume), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool FixVboxConfig
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (FixVboxConfig), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (FixVboxConfig), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string WindowPlacement
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (WindowPlacement), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (WindowPlacement), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsGoogleSigninDone
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsGoogleSigninDone), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsGoogleSigninDone), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsGoogleSigninPopupShown
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsGoogleSigninPopupShown), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsGoogleSigninPopupShown), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string[] GrmDonotShowRuleList
    {
      get
      {
        return (string[]) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (GrmDonotShowRuleList), (object) new string[0], RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (GrmDonotShowRuleList), (object) value, RegistryValueKind.MultiString, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string GoogleAId
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstVmAId", (object) string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstVmAId", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string AndroidId
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstVmId", (object) string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstVmId", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool ShowMacroDeletePopup
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ShowMacroDeletePopup), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ShowMacroDeletePopup), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool ShowSchemeDeletePopup
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ShowSchemeDeletePopup), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ShowSchemeDeletePopup), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool TouchSoundEnabled
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (TouchSoundEnabled), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) == 1;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (TouchSoundEnabled), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool ShowBlueHighlighter
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (ShowBlueHighlighter), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) == 1;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (ShowBlueHighlighter), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsFreeFireInGameSettingsCustomized
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsFreeFireInGameSettingsCustomized), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsFreeFireInGameSettingsCustomized), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsClientOnTop
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsClientOnTop), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsClientOnTop), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public ASTCOption ASTCOption
    {
      get
      {
        return (ASTCOption) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (ASTCOption), (object) (ASTCOption) (FeatureManager.Instance.IsCustomUIForNCSoft ? 2 : 0), RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (ASTCOption), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsHardwareAstcSupported
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.AndroidKeyPath, nameof (IsHardwareAstcSupported), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.AndroidKeyPath, nameof (IsHardwareAstcSupported), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public NativeGamepadState NativeGamepadState
    {
      get
      {
        return (NativeGamepadState) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (NativeGamepadState), (object) NativeGamepadState.Auto, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (NativeGamepadState), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsShowMinimizeBlueStacksPopupOnClose
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsShowMinimizeBlueStacksPopupOnClose), (object) 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsShowMinimizeBlueStacksPopupOnClose), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public bool IsMinimizeSelectedOnReceiveGameNotificationPopup
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (IsMinimizeSelectedOnReceiveGameNotificationPopup), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (IsMinimizeSelectedOnReceiveGameNotificationPopup), (object) (value ? 1 : 0), RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NotificationModePopupShownCount
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (NotificationModePopupShownCount), (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (NotificationModePopupShownCount), (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string LastNotificationEnabledAppLaunched
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, nameof (LastNotificationEnabledAppLaunched), (object) string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, nameof (LastNotificationEnabledAppLaunched), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string[] NetworkInboundRules
    {
      get
      {
        return (string[]) RegistryUtils.GetRegistryValue(this.mNetwork0KeyPath, "InboundRules", (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetwork0KeyPath, "InboundRules", (object) value, RegistryValueKind.MultiString, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string AllowRemoteAccess
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mNetwork0KeyPath, nameof (AllowRemoteAccess), (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetwork0KeyPath, nameof (AllowRemoteAccess), (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NetworkRedirectTcp5555
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/5555", (object) 5555, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/5555", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NetworkRedirectTcp6666
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/6666", (object) 6666, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/6666", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NetworkRedirectTcp7777
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/7777", (object) 7777, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/7777", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NetworkRedirectTcp9999
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/9999", (object) 8888, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/9999", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int NetworkRedirectUdp12000
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "udp/12000", (object) 12000, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "udp/12000", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder0Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder0Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder0Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder1Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder1Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder1Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder2Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder2Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder2Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder3Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder3Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder3Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder4Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder4Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder4Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder5Name
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Name", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Name", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public string SharedFolder5Path
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Path", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Path", (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    public int SharedFolder5Writable
    {
      get
      {
        return (int) RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Writable", (object) 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
      set
      {
        RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Writable", (object) value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }
  }
}
