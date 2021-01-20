// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomPopUp
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Windows.Controls.Primitives;

namespace BlueStacks.Common
{
  public class CustomPopUp : Popup
  {
    public CustomPopUp()
    {
      this.Opened += new EventHandler(this.CustomPopUp_Initialized);
    }

    private void CustomPopUp_Initialized(object sender, EventArgs e)
    {
      RenderHelper.ChangeRenderModeToSoftware(sender);
    }
  }
}
