// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ConsoleControl
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public static class ConsoleControl
  {
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(ConsoleControl.Handler handler, bool Add);

    public static void SetHandler(ConsoleControl.Handler handler)
    {
      ConsoleControl.SetConsoleCtrlHandler(handler, true);
    }

    public delegate bool Handler(CtrlType ctrlType);
  }
}
