// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceData
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class GuidanceData
  {
    private Dictionary<string, Dictionary<Type, Dictionary<string, string>>> mViewIgnoreList = new Dictionary<string, Dictionary<Type, Dictionary<string, string>>>()
    {
      {
        "KeyMap",
        new Dictionary<Type, Dictionary<string, string>>()
        {
          {
            typeof (Dpad),
            new Dictionary<string, string>()
            {
              {
                "KeyUp",
                "DpadTitle"
              },
              {
                "KeyLeft",
                "DpadTitle"
              },
              {
                "KeyDown",
                "DpadTitle"
              },
              {
                "KeyRight",
                "DpadTitle"
              }
            }
          }
        }
      },
      {
        "GamePad",
        new Dictionary<Type, Dictionary<string, string>>()
      }
    };
    private Dictionary<string, Dictionary<Type, List<string>>> mEditIgnoreList = new Dictionary<string, Dictionary<Type, List<string>>>()
    {
      {
        "KeyMap",
        new Dictionary<Type, List<string>>()
        {
          {
            typeof (Dpad),
            new List<string>() { "DpadTitle" }
          }
        }
      },
      {
        "GamePad",
        new Dictionary<Type, List<string>>()
      }
    };
    private ObservableCollection<GuidanceCategoryEditModel> mKeymapEditGuidanceCloned;
    private ObservableCollection<GuidanceCategoryEditModel> mGamepadEditGuidanceCloned;

    public ObservableCollection<GuidanceCategoryViewModel> KeymapViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

    public ObservableCollection<GuidanceCategoryViewModel> GamepadViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

    public ObservableCollection<GuidanceCategoryEditModel> KeymapEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

    public ObservableCollection<GuidanceCategoryEditModel> GamepadEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

    public void Clear()
    {
      this.KeymapViewGuidance.Clear();
      this.GamepadViewGuidance.Clear();
      this.KeymapEditGuidance.Clear();
      this.GamepadEditGuidance.Clear();
      this.mKeymapEditGuidanceCloned = (ObservableCollection<GuidanceCategoryEditModel>) null;
      this.mGamepadEditGuidanceCloned = (ObservableCollection<GuidanceCategoryEditModel>) null;
    }

    public void SaveOriginalData()
    {
      this.mKeymapEditGuidanceCloned = this.KeymapEditGuidance.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
      this.mGamepadEditGuidanceCloned = this.GamepadEditGuidance.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
    }

    public void Reset()
    {
      this.KeymapEditGuidance = this.mKeymapEditGuidanceCloned.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
      this.GamepadEditGuidance = this.mGamepadEditGuidanceCloned.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
    }

    public void AddGuidance(
      bool isGamePad,
      string category,
      string guidanceText,
      string guidanceKey,
      string actionItem,
      IMAction imAction)
    {
      if (imAction == null || string.IsNullOrEmpty(guidanceKey) || string.IsNullOrEmpty(actionItem))
        return;
      Type propertyType = IMAction.DictPropertyInfo[imAction.Type][actionItem].PropertyType;
      double result;
      if ((object) propertyType == (object) typeof (double) && double.TryParse(guidanceKey, out result))
      {
        guidanceKey = result.ToString((IFormatProvider) CultureInfo.CurrentCulture);
        if (actionItem.Equals("SensitivityRatioY", StringComparison.InvariantCultureIgnoreCase))
          guidanceKey = Convert.ToDouble(imAction["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture) == 0.0 ? Convert.ToDouble(imAction["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture) : (Convert.ToDouble(imAction["Sensitivity"], (IFormatProvider) CultureInfo.InvariantCulture) * Convert.ToDouble(imAction["SensitivityRatioY"], (IFormatProvider) CultureInfo.InvariantCulture)).ToString("0.##", (IFormatProvider) CultureInfo.CurrentCulture);
      }
      if (imAction is EdgeScroll && actionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
        guidanceKey = Convert.ToBoolean(guidanceKey, (IFormatProvider) CultureInfo.InvariantCulture) ? "ON" : "OFF";
      string index = isGamePad ? "GamePad" : "KeyMap";
      if (!this.mViewIgnoreList[index].ContainsKey(imAction.GetType()) || !this.mViewIgnoreList[index][imAction.GetType()].ContainsKey(actionItem) || !imAction.Guidance.ContainsKey(this.mViewIgnoreList[index][imAction.GetType()][actionItem]))
      {
        ObservableCollection<GuidanceCategoryViewModel> source = isGamePad ? this.GamepadViewGuidance : this.KeymapViewGuidance;
        GuidanceCategoryViewModel categoryViewModel1 = source.Where<GuidanceCategoryViewModel>((Func<GuidanceCategoryViewModel, bool>) (guide => string.Equals(guide.Category, category, StringComparison.InvariantCulture))).FirstOrDefault<GuidanceCategoryViewModel>();
        if (categoryViewModel1 == null)
        {
          GuidanceViewModel guidanceViewModel = new GuidanceViewModel()
          {
            PropertyType = propertyType
          };
          guidanceViewModel.GuidanceTexts.Add(guidanceText);
          guidanceViewModel.GuidanceKeys.Add(guidanceKey);
          GuidanceCategoryViewModel categoryViewModel2 = new GuidanceCategoryViewModel()
          {
            Category = category
          };
          categoryViewModel2.GuidanceViewModels.Add(guidanceViewModel);
          source.Add(categoryViewModel2);
        }
        else
        {
          if ((object) propertyType != (object) typeof (double))
          {
            GuidanceViewModel guidanceViewModel = categoryViewModel1.GuidanceViewModels.Where<GuidanceViewModel>((Func<GuidanceViewModel, bool>) (guide => (object) guide.PropertyType != (object) typeof (double) && guide.GuidanceTexts.Count == 1 && guide.GuidanceTexts.Contains(guidanceText))).FirstOrDefault<GuidanceViewModel>();
            if (guidanceViewModel != null)
            {
              guidanceViewModel.GuidanceKeys.AddIfNotContain<string>(guidanceKey);
              goto label_16;
            }
          }
          if ((object) propertyType != (object) typeof (double))
          {
            GuidanceViewModel guidanceViewModel = categoryViewModel1.GuidanceViewModels.Where<GuidanceViewModel>((Func<GuidanceViewModel, bool>) (guide => (object) guide.PropertyType != (object) typeof (double) && guide.GuidanceKeys.Count == 1 && guide.GuidanceKeys.Contains(guidanceKey))).FirstOrDefault<GuidanceViewModel>();
            if (guidanceViewModel != null)
            {
              guidanceViewModel.GuidanceTexts.AddIfNotContain<string>(guidanceText);
              goto label_16;
            }
          }
          GuidanceViewModel guidanceViewModel1 = new GuidanceViewModel()
          {
            PropertyType = propertyType
          };
          guidanceViewModel1.GuidanceTexts.Add(guidanceText);
          guidanceViewModel1.GuidanceKeys.Add(guidanceKey);
          categoryViewModel1.GuidanceViewModels.Add(guidanceViewModel1);
        }
      }
label_16:
      if (this.mEditIgnoreList[index].ContainsKey(imAction.GetType()) && this.mEditIgnoreList[index][imAction.GetType()].Contains(actionItem))
        return;
      ObservableCollection<GuidanceCategoryEditModel> source1 = isGamePad ? this.GamepadEditGuidance : this.KeymapEditGuidance;
      GuidanceCategoryEditModel categoryEditModel = source1.Where<GuidanceCategoryEditModel>((Func<GuidanceCategoryEditModel, bool>) (guide => string.Equals(guide.Category, category, StringComparison.InvariantCulture))).FirstOrDefault<GuidanceCategoryEditModel>();
      if (categoryEditModel == null)
      {
        categoryEditModel = new GuidanceCategoryEditModel()
        {
          Category = category
        };
        source1.Add(categoryEditModel);
      }
      GuidanceEditModel guidanceEditModel = (GuidanceEditModel) null;
      if ((object) propertyType == (object) typeof (string))
        guidanceEditModel = categoryEditModel.GuidanceEditModels.Where<GuidanceEditModel>((Func<GuidanceEditModel, bool>) (gem => gem.ActionType == imAction.Type && (object) gem.PropertyType == (object) propertyType && string.Equals(gem.GuidanceText, guidanceText, StringComparison.InvariantCultureIgnoreCase) && string.Equals(gem.GuidanceKey, guidanceKey, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<GuidanceEditModel>();
      if (guidanceEditModel == null)
      {
        guidanceEditModel = (object) propertyType == (object) typeof (string) || (object) propertyType == (object) typeof (bool) ? (GuidanceEditModel) new GuidanceEditTextModel() : (GuidanceEditModel) new GuidanceEditDecimalModel();
        guidanceEditModel.GuidanceText = guidanceText;
        guidanceEditModel.OriginalGuidanceKey = guidanceKey;
        guidanceEditModel.ActionType = imAction.Type;
        guidanceEditModel.PropertyType = propertyType;
        guidanceEditModel.IsEnabled = !string.Equals(actionItem, "KeyAction", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(actionItem, "KeyMove", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(actionItem, "KeyWheel", StringComparison.InvariantCultureIgnoreCase);
        categoryEditModel.GuidanceEditModels.Add(guidanceEditModel);
      }
      if (string.Equals(actionItem, "Speed", StringComparison.InvariantCultureIgnoreCase))
        guidanceEditModel.MaxValue = 1000.0;
      guidanceEditModel.IMActionItems.Add(new IMActionItem()
      {
        ActionItem = actionItem,
        IMAction = imAction
      });
    }
  }
}
