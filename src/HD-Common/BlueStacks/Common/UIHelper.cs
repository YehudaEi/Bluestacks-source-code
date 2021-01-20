// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.UIHelper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  public static class UIHelper
  {
    private static UIHelper.dispatcher obj;

    public static void SetDispatcher(UIHelper.dispatcher gameManagerWindowDispatcher)
    {
      UIHelper.obj = gameManagerWindowDispatcher;
    }

    public static void RunOnUIThread(System.Windows.Forms.Control control, UIHelper.Action action)
    {
      if (UIHelper.obj == null)
      {
        if (control != null && control.InvokeRequired)
        {
          control.Invoke((Delegate) action);
        }
        else
        {
          if (action == null)
            return;
          action();
        }
      }
      else
      {
        if (control == null || !control.InvokeRequired)
          return;
        object obj = UIHelper.obj((Delegate) action, new object[0]);
      }
    }

    public static void AssertUIThread(System.Windows.Forms.Control control)
    {
      if (control != null && control.InvokeRequired)
        throw new ApplicationException("Not running on UI thread");
    }

    public static void AssertUIThread(System.Windows.Controls.Control control)
    {
      if (control != null && !control.Dispatcher.CheckAccess())
        throw new ApplicationException("Not running on UI thread");
    }

    public delegate object dispatcher(Delegate method, params object[] args);

    public delegate void Action();
  }
}
