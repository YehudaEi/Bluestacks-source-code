// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.Is64BitEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class Is64BitEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.Is64Bit;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      bool bitOperatingSystem = CommonInstallUtils.Is64BitOperatingSystem;
      return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, bitOperatingSystem, rightOperand, context);
    }
  }
}
