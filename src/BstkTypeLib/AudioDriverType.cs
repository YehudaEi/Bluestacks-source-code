// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.AudioDriverType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("4BCC3D73-C2FE-40DB-B72F-0C2CA9D68496")]
  public enum AudioDriverType
  {
    AudioDriverType_Null,
    AudioDriverType_WinMM,
    AudioDriverType_OSS,
    AudioDriverType_ALSA,
    AudioDriverType_DirectSound,
    AudioDriverType_CoreAudio,
    AudioDriverType_MMPM,
    AudioDriverType_Pulse,
    AudioDriverType_SolAudio,
  }
}
