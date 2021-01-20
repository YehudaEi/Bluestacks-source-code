// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.CameraError
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BlueStacks.Player
{
  public static class CameraError
  {
    [SuppressUnmanagedCodeSecurity]
    [DllImport("quartz.dll", EntryPoint = "AMGetErrorTextW", CharSet = CharSet.Unicode)]
    public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);

    public static string GetCameraErrorString(int hr)
    {
      StringBuilder buf = new StringBuilder(256, 256);
      return CameraError.AMGetErrorText(hr, buf, 256) > 0 ? buf.ToString() : (string) null;
    }

    public static void ThrowCameraError(int hr)
    {
      if (hr >= 0)
        return;
      string cameraErrorString = CameraError.GetCameraErrorString(hr);
      if (cameraErrorString != null)
        throw new COMException(cameraErrorString, hr);
      Marshal.ThrowExceptionForHR(hr);
    }
  }
}
