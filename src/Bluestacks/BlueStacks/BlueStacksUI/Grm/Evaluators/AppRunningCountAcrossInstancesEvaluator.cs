// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.AppRunningCountAcrossInstancesEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class AppRunningCountAcrossInstancesEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.AppRunningCountAcrossInstances;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      if (!(JsonConvert.DeserializeObject(context.ContextJson, Utils.GetSerializerSettings()) is JObject jobject) || !jobject.ContainsKey("pkg"))
        throw new ArgumentException("AppRunningCountAcrossInstancesEvaluator requires contextjson with pkg key.");
      string pkg = jobject["pkg"].Value<string>();
      int left = BlueStacksUIUtils.DictWindows.Values.Sum<MainWindow>((Func<MainWindow, int>) (window => !window.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(pkg) ? 0 : 1)) + 1;
      return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
