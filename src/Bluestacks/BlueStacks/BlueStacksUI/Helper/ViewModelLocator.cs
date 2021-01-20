// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Helper.ViewModelLocator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using GalaSoft.MvvmLight.Ioc;

namespace BlueStacks.BlueStacksUI.Helper
{
  public class ViewModelLocator
  {
    static ViewModelLocator()
    {
      SimpleIoc.Default.Register<MinimizeBlueStacksOnCloseView>();
    }
  }
}
