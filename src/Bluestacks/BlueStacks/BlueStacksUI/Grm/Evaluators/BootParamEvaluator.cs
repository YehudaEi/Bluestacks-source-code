// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.BootParamEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class BootParamEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.BootParam;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      if (!(JsonConvert.DeserializeObject(context.ContextJson, Utils.GetSerializerSettings()) is JObject jobject) || !jobject.ContainsKey("param"))
        throw new ArgumentException("BootParamEvaluator requires contextjson with param key.");
      string valueInBootParams = Utils.GetValueInBootParams(jobject["param"].Value<string>(), context.VmName, "", "bgp");
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, valueInBootParams, rightOperand, context);
    }
  }
}
