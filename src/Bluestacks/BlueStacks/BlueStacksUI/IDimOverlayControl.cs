﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.IDimOverlayControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

namespace BlueStacks.BlueStacksUI
{
  internal interface IDimOverlayControl
  {
    bool Close();

    bool IsCloseOnOverLayClick { get; set; }

    bool Show();

    bool ShowControlInSeparateWindow { get; set; }

    bool ShowTransparentWindow { get; set; }

    double Height { get; set; }

    double Width { get; set; }
  }
}
