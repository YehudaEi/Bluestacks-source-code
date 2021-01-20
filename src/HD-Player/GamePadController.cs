// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.GamePadController
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  internal class GamePadController
  {
    private GamePadController.LoggerCallback mLoggerCallback;
    private GamePadController.AttachCallback mAttachCallback;
    private GamePadController.DetachCallback mDetachCallback;
    private GamePadController.UpdateCallback mUpdateCallback;

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    public void Setup(IntPtr windowHandle, IntPtr vmwHandle)
    {
      Logger.Info("GamePad.Setup()");
      Logger.Debug(windowHandle.ToString() + " " + Environment.StackTrace);
      this.mLoggerCallback = (GamePadController.LoggerCallback) (msg => Logger.Info("GamePad: " + msg));
      this.mAttachCallback = (GamePadController.AttachCallback) ((identity, vendor, product) => {});
      this.mDetachCallback = (GamePadController.DetachCallback) (identity => {});
      this.mUpdateCallback = (GamePadController.UpdateCallback) ((int identity, ref GamePad gamepad) => {});
      string dllToLoad = "HD-Frontend-Native.dll";
      IntPtr hModule = GamePadController.LoadLibrary(dllToLoad);
      if (hModule == IntPtr.Zero)
      {
        Logger.Info("Failed to {0} dll", (object) dllToLoad);
      }
      else
      {
        IntPtr procAddress = GamePadController.GetProcAddress(hModule, "GamePadSetup");
        if (procAddress == IntPtr.Zero)
          Logger.Info("function pointer is null");
        else
          ((GamePadController.GamePadSetup) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (GamePadController.GamePadSetup)))(this.mLoggerCallback, this.mAttachCallback, this.mDetachCallback, this.mUpdateCallback, windowHandle, vmwHandle);
      }
    }

    private delegate void LoggerCallback(string msg);

    private delegate void AttachCallback(int identity, int vendor, int product);

    private delegate void DetachCallback(int identity);

    private delegate void UpdateCallback(int identity, ref GamePad gamepad);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void GamePadSetup(
      GamePadController.LoggerCallback logger,
      GamePadController.AttachCallback attach,
      GamePadController.DetachCallback detach,
      GamePadController.UpdateCallback update,
      IntPtr windowHandle,
      IntPtr vmwHandle);
  }
}
