﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ReturnCodes
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public enum ReturnCodes
  {
    SUCCESS,
    INVALID_CODE,
    PROCESS_ALREADY_RUNNING,
    INSTALL_FAILED_SERVER_ERROR,
    USER_EXITED,
    INSTALL_FAILED_ERROR_AT_GUEST,
    GUEST_NOT_READY_YET,
    APK_INSTALLATION_IN_PROGRESS,
    INSTALL_FAILED_APKHANDLER_EXCEPTION,
    INSTALL_FAILED_APKHANDLER_WEBEXCEPTION,
    ANDROID_BOOT_FAILURE,
    AGENT_SERVER_NOT_RUNNING,
    FRONTEND_NOT_RUNNING,
    ANDROID_SERVICE_NOT_RUNNING,
    FRONTEND_AND_ANDROID_SERVICE_NOT_RUNNING,
    APK_FILE_NOT_FOUND,
    FRONTEND_NOT_STARTING,
    CONFIG_NOT_SYNCED,
    INSTALL_FAILED_INSUFFICIENT_STORAGE_HOST,
    APP_UNINSTALLATION_IN_PROGRESS,
    INSTALL_FAILED_UPLOAD_APK_ERROR,
    INSTALL_APK_TIMEOUT,
    INSTALL_APK_CONNECTION_TERMINATED,
    VM_DOES_NOT_EXIST,
    UNKNOWN_ERROR,
  }
}