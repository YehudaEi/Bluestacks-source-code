// Decompiled with JetBrains decompiler
// Type: System.Windows.Interop.WindowInteropHelperExtensions
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Globalization;
using System.Reflection;

namespace System.Windows.Interop
{
  public static class WindowInteropHelperExtensions
  {
    public static IntPtr EnsureHandle(this WindowInteropHelper helper)
    {
      if (helper == null)
        throw new ArgumentNullException(nameof (helper));
      if (helper.Handle == IntPtr.Zero)
      {
        Window window = (Window) typeof (WindowInteropHelper).InvokeMember("_window", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, (Binder) null, (object) helper, (object[]) null, CultureInfo.InvariantCulture);
        try
        {
          typeof (Window).InvokeMember("SafeCreateWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, (object) window, (object[]) null, CultureInfo.InvariantCulture);
        }
        catch (MissingMethodException ex)
        {
          typeof (Window).InvokeMember("CreateSourceWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, (object) window, new object[1]
          {
            (object) false
          }, CultureInfo.InvariantCulture);
        }
      }
      return helper.Handle;
    }
  }
}
