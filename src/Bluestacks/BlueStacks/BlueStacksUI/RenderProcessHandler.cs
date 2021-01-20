// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RenderProcessHandler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
  internal sealed class RenderProcessHandler : CefRenderProcessHandler
  {
    private MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();

    protected override void OnWebKitInitialized()
    {
      CefRuntime.RegisterExtension("MessageEvent", "var gmApi = function(jsonArg) {\r\n                    native function MyNativeFunction(jsonArg);\r\n                    return MyNativeFunction(jsonArg);\r\n                };", (CefV8Handler) this.myCefV8Handler);
      base.OnWebKitInitialized();
    }

    protected override bool OnProcessMessageReceived(
      CefBrowser browser,
      CefProcessId sourceProcess,
      CefProcessMessage message)
    {
      this.myCefV8Handler.OnProcessMessageReceived(message);
      return base.OnProcessMessageReceived(browser, sourceProcess, message);
    }
  }
}
