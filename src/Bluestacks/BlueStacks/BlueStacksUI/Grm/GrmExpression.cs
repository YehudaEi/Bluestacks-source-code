// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmExpression
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class GrmExpression
  {
    private static List<string> _rulesetsWithException = new List<string>();

    [JsonProperty(PropertyName = "leftOperand")]
    public string LeftOperand { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "operator")]
    public string Operator { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "rightOperand")]
    public string RightOperand { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "contextJson")]
    public string ContextJson { get; set; } = string.Empty;

    public bool EvaluateExpression(GrmRuleSetContext context)
    {
      try
      {
        if (context != null)
          context.ContextJson = this.ContextJson;
        int num = (int) Enum.Parse(typeof (GrmOperand), this.LeftOperand, true);
        GrmOperator grmOperator = (GrmOperator) Enum.Parse(typeof (GrmOperator), this.Operator, true);
        return EvaluatorFactory.CreateandReturnEvaluator((GrmOperand) num).Evaluate(context, grmOperator, this.RightOperand);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while parsing operand for grmrule. operand: {0} operator: {1} rulesetid:{2} exception: {3}", (object) this.LeftOperand, (object) this.Operator, (object) context?.RuleSetId, (object) ex.Message);
        if (!GrmExpression._rulesetsWithException.Contains(context?.RuleSetId))
        {
          GrmExpression._rulesetsWithException.Add(context?.RuleSetId);
          ClientStats.SendMiscellaneousStatsAsync("grm_evaluation_error", RegistryManager.Instance.UserGuid, context?.RuleSetId, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp", context?.PackageName, ex.Message, (string) null, context?.VmName);
        }
        return false;
      }
    }
  }
}
