// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InstallerStatsEvent
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public static class InstallerStatsEvent
  {
    public const string InstallStarted = "install_launched";
    public const string InstallAborted = "install_aborted_by_user";
    public const string InstallLicenseAgreed = "install_license_agreed";
    public const string InstallChecksPassed = "install_checks_passed";
    public const string InstallCompleted = "install_completed";
    public const string InstallFailed = "install_failed";
    public const string UpgradeLaunched = "upgrade_launched";
    public const string BackupCancel = "backup_cancel";
    public const string BackupContinue = "backup_continue";
    public const string BackupCross = "backup_cross";
    public const string UpgradeStart = "upgrade_start";
    public const string UpgradeAborted = "upgrade_aborted_by_user";
    public const string UpgradeCleaned = "upgrade_cleaned";
    public const string UpgradeChecksPassed = "upgrade_checks_passed";
    public const string UpgradeCompleted = "upgrade_completed";
    public const string UpgradeFailed = "upgrade_failed";
    public const string SysprepStarted = "install_sysprep_started";
    public const string SilentBootCompleted = "install_silentboot_completed";
    public const string SysprepCompleted = "install_sysprep_completed";
    public const string MiLaunched = "mi_launched";
    public const string MiUacPrompted = "mi_uac_prompted";
    public const string MiUacPromptRetried = "mi_uac_prompt_retried";
    public const string MiAdminLaunched = "mi_admin_launched";
    public const string MiBackupContinue = "mi_backup_continue";
    public const string MiBackupCancel = "mi_backup_cancel";
    public const string MiBackupCross = "mi_backup_cross";
    public const string MiInstallLicenseAgreed = "mi_license_agreed";
    public const string MiLowDiskSpaceRetried = "mi_low_disk_space_retried";
    public const string MiChecksPassed = "mi_checks_passed";
    public const string MiDownloadStarted = "mi_download_started";
    public const string MiDownloadFailed = "mi_download_failed";
    public const string MiDownloadRetried = "mi_download_retried";
    public const string MiDownloadCompleted = "mi_download_completed";
    public const string MiMinimizePopupInit = "mi_minimizepopup_init";
    public const string MiMinimizePopupYes = "mi_minimizepopup_yes";
    public const string MiMinimizePopupNo = "mi_minimizepopup_no";
    public const string MiClosed = "mi_closed";
    public const string MiFailed = "mi_failed";
    public const string MiFullInstallerLaunched = "mi_full_installer_launched";
    public const string MiAdminProcCompleted = "mi_admin_proc_completed";
    public const string MiRegistryNotFound = "mi_registry_not_found";
    public const string MiClientLaunchFailed = "mi_client_launch_failed";
    public const string MiClientLaunched = "mi_client_launched";
    public const string DeviceProvisioned = "device_provisioned";
    public const string GoogleLoginCompleted = "google_login_completed";
    public const string BlueStacksLoginCompleted = "bluestacks_login_completed";
  }
}
