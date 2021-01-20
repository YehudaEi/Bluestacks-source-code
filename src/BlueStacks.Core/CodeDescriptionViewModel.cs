// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.CodeDescriptionViewModel
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using GalaSoft.MvvmLight;

namespace BlueStacks.Core
{
  public class CodeDescriptionViewModel : ViewModelBase, ICodeDescriptionViewModel
  {
    private string mCode = string.Empty;
    private string mDescription = string.Empty;

    public string Code
    {
      get
      {
        return this.mCode;
      }
      set
      {
        if (!(this.mCode != value))
          return;
        this.mCode = value;
        this.RaisePropertyChanged(nameof (Code));
      }
    }

    public string Description
    {
      get
      {
        return this.mDescription;
      }
      set
      {
        if (!(this.mDescription != value))
          return;
        this.mDescription = value;
        this.RaisePropertyChanged(nameof (Description));
      }
    }
  }
}
