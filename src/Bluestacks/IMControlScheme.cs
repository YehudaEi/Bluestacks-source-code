// Decompiled with JetBrains decompiler
// Type: IMControlScheme
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

[Serializable]
public class IMControlScheme
{
  public List<IMAction> GameControls { get; private set; } = new List<IMAction>();

  public List<JObject> Images { get; private set; } = new List<JObject>();

  public string Name { get; set; }

  public bool BuiltIn { get; set; }

  public bool Selected { get; set; }

  public bool IsBookMarked { get; set; }

  public bool IsCategoryVisible { get; set; } = true;

  public string KeyboardLayout { get; set; } = InteropWindow.MapLayoutName((string) null);

  public IMControlScheme DeepCopy()
  {
    IMControlScheme imControlScheme = (IMControlScheme) this.MemberwiseClone();
    List<IMAction> gameControls = this.GameControls;
    imControlScheme.SetGameControls(gameControls != null ? gameControls.DeepCopy<List<IMAction>>() : (List<IMAction>) null);
    imControlScheme.SetImages(this.Images.ConvertAll<JObject>((Converter<JObject, JObject>) (jt => (JObject) jt?.DeepClone())));
    return imControlScheme;
  }

  public void SetGameControls(List<IMAction> gameControls)
  {
    this.GameControls = gameControls;
  }

  public void SetImages(List<JObject> images)
  {
    this.Images = images?.ConvertAll<JObject>((Converter<JObject, JObject>) (jt => (JObject) jt?.DeepClone()));
  }
}
