// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.IGrmOperatorComparer`1
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

namespace BlueStacks.BlueStacksUI.Grm
{
  internal interface IGrmOperatorComparer<T>
  {
    bool Contains(T left, string right);

    bool LessThan(T left, string right);

    bool GreaterThan(T left, string right);

    bool Equal(T left, string right);

    bool NotEqual(T left, string right);

    bool LessThanEqual(T left, string right);

    bool GreaterThanEqual(T left, string right);

    bool StartsWith(T left, string right, string contextJson);

    bool LikeRegex(T left, string right, string contextJson);

    bool In(T left, string right);

    bool NotIn(T left, string right);
  }
}
