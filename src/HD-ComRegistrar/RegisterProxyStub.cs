// Decompiled with JetBrains decompiler
// Type: BlueStacks.ComRegistrar.RegisterProxyStub
// Assembly: HD-ComRegistrar, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: E05F62B1-3170-42C6-BFA0-DC982106896F
// Assembly location: C:\Program Files\BlueStacks\HD-ComRegistrar.exe

using System.Runtime.InteropServices;

namespace BlueStacks.ComRegistrar
{
  public class RegisterProxyStub
  {
    private const string PROXY_STUB_DLL = "BstkProxyStub.dll";

    [DllImport("BstkProxyStub.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern ulong DllRegisterServer();

    [DllImport("BstkProxyStub.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern ulong DllUnregisterServer();
  }
}
