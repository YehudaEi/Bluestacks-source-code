// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.OldWindow
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Windows.Forms;

namespace BlueStacks.Common
{
  internal class OldWindow : IWin32Window
  {
    private readonly IntPtr _handle;

    public OldWindow(IntPtr handle)
    {
      this._handle = handle;
    }

    IntPtr IWin32Window.Handle
    {
      get
      {
        return this._handle;
      }
    }
  }
}
