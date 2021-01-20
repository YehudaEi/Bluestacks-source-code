// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ErrorHandler
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

namespace BlueStacks.Player
{
  public class ErrorHandler
  {
    private int hr;

    public ErrorHandler(int hr)
    {
      this.hr = hr;
      CameraError.GetCameraErrorString(hr);
    }

    public ErrorHandler(ErrorHandler err)
    {
      CameraError.ThrowCameraError(err.hr);
    }

    public static implicit operator ErrorHandler(int hr)
    {
      return new ErrorHandler(hr);
    }

    public int GetError()
    {
      return this.hr;
    }
  }
}
