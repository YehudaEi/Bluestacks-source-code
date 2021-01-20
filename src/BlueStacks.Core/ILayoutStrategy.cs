// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.ILayoutStrategy
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;

namespace BlueStacks.Core
{
  public interface ILayoutStrategy
  {
    Size ResultSize { get; }

    void Calculate(Size availableSize, Size[] sizes);

    Rect GetPosition(int index);

    int GetIndex(Point position);
  }
}
