// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.GeoEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class GeoEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.Geo;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      string geo = RegistryManager.Instance.Geo;
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, geo, rightOperand, context);
    }
  }
}
