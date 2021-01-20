// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.DeviceProfileEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class DeviceProfileEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.DeviceProfile;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      string valueInBootParams = Utils.GetValueInBootParams("pcode", context.VmName, "", "bgp");
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, valueInBootParams, rightOperand, context);
    }
  }
}
