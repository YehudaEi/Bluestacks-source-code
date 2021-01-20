// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.MessageModel.Classes.ToastPopupMessage
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

namespace MultiInstanceManagerMVVM.MessageModel.Classes
{
  public class ToastPopupMessage
  {
    public string Message { get; set; }

    public double Duration { get; set; } = 1.3;

    public bool IsShowCloseImage { get; set; } = false;
  }
}
