// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppSuggestionPromotion
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
  [JsonObject(MemberSerialization.OptIn)]
  public class AppSuggestionPromotion
  {
    [JsonProperty("app_pkg", NullValueHandling = NullValueHandling.Ignore)]
    public string AppPackage { get; set; } = string.Empty;

    [JsonProperty("app_activity", NullValueHandling = NullValueHandling.Ignore)]
    public string AppActivity { get; set; }

    [JsonProperty("show_red_dot", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsShowRedDot { get; set; }

    [JsonProperty("app_name", NullValueHandling = NullValueHandling.Ignore)]
    public string AppName { get; set; }

    [JsonProperty("app_icon", NullValueHandling = NullValueHandling.Ignore)]
    public string AppIcon { get; set; }

    [JsonProperty("app_icon_id", NullValueHandling = NullValueHandling.Ignore)]
    public string AppIconId { get; set; }

    [JsonProperty("tooltip", NullValueHandling = NullValueHandling.Ignore)]
    public string ToolTip { get; set; }

    [JsonProperty("cross_promotion_pkg", NullValueHandling = NullValueHandling.Ignore)]
    public string CrossPromotionPackage { get; set; }

    [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
    public string AppLocation { get; set; } = string.Empty;

    [JsonProperty("is_email_required", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsEmailRequired { get; set; }

    public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

    [JsonProperty("is_animation", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsAnimation { get; set; }

    [JsonProperty("animation_time", NullValueHandling = NullValueHandling.Ignore)]
    public int AnimationTime { get; set; }

    [JsonProperty("is_icon_border", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsIconBorder { get; set; }

    [JsonProperty("icon_border_url", NullValueHandling = NullValueHandling.Ignore)]
    public string IconBorderUrl { get; set; }

    [JsonProperty("icon_border_click_url", NullValueHandling = NullValueHandling.Ignore)]
    public string IconBorderClickUrl { get; set; }

    [JsonProperty("icon_border_id", NullValueHandling = NullValueHandling.Ignore)]
    public string IconBorderId { get; set; }

    [JsonProperty("icon_border_hover_url", NullValueHandling = NullValueHandling.Ignore)]
    public string IconBorderHoverUrl { get; set; }

    public string AppIconPath { get; set; }

    [JsonProperty("app_icon_width", NullValueHandling = NullValueHandling.Ignore)]
    public double IconWidth { get; set; }

    [JsonProperty("app_icon_height", NullValueHandling = NullValueHandling.Ignore)]
    public double IconHeight { get; set; }
  }
}
