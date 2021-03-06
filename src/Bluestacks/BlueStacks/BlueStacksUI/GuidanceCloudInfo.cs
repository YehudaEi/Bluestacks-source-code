﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceCloudInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  internal class GuidanceCloudInfo
  {
    public Dictionary<string, CustomThumbnail> CustomThumbnails { get; } = new Dictionary<string, CustomThumbnail>();

    public Dictionary<GuidanceVideoType, VideoThumbnailInfo> DefaultThumbnails { get; } = new Dictionary<GuidanceVideoType, VideoThumbnailInfo>();

    public Dictionary<string, HelpArticle> HelpArticles { get; } = new Dictionary<string, HelpArticle>();

    public List<GameSetting> GameSettings { get; } = new List<GameSetting>();
  }
}
