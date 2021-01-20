// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PostBootCloudInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  public class PostBootCloudInfo
  {
    public NotificationModeInfo GameNotificationAppPackages { get; set; } = new NotificationModeInfo();

    public OnBoardingInfo OnBoardingInfo { get; set; } = new OnBoardingInfo();

    public AppSpecificCustomCursorInfo AppSpecificCustomCursorInfo { get; set; } = new AppSpecificCustomCursorInfo();

    public List<string> IgnoredActivitiesForTabs { get; } = new List<string>();

    public GameAwareOnboardingInfo GameAwareOnboardingInfo { get; set; } = new GameAwareOnboardingInfo();

    public UtcConverterInfo UtcConverterInfo { get; set; } = new UtcConverterInfo();

    public DesktopNotificationsInfo DesktopNotificationsChatPackages { get; set; } = new DesktopNotificationsInfo();
  }
}
