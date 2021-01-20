// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tasks
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common
{
  public static class Tasks
  {
    public enum Parameter
    {
      Create,
      Delete,
      Query,
      Run,
      End,
    }

    public enum Frequency
    {
      MINUTE,
      HOURLY,
      DAILY,
      WEEKLY,
      MONTHLY,
      ONCE,
      ONSTART,
      ONLOGON,
      ONIDLE,
      ONEVENT,
    }

    public enum Modifiers
    {
      MON,
      TUE,
      WED,
      THU,
      FRI,
      SAT,
      SUN,
    }

    public enum Days
    {
      MON,
      TUE,
      WED,
      THU,
      FRI,
      SAT,
      SUN,
    }
  }
}
