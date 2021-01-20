// Decompiled with JetBrains decompiler
// Type: BlueStacks.GuestCommandRunner.VMCommand
// Assembly: HD-GuestCommandRunner, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 899CB498-70B0-44E2-A8EB-5E2DBF0FFF50
// Assembly location: C:\Program Files\BlueStacks\HD-GuestCommandRunner.exe

namespace BlueStacks.GuestCommandRunner
{
  public class VMCommand
  {
    public static string[] COMMAND = new string[2]
    {
      string.Format("clearappdata {0}", (object) "com.google.android.gms"),
      string.Format("clearappdata {0}", (object) "com.android.vending")
    };
  }
}
