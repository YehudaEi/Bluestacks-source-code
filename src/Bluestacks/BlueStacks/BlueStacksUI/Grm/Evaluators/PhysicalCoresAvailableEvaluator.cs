// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.PhysicalCoresAvailableEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class PhysicalCoresAvailableEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.PhysicalCoresAvailable;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      int left = 1;
      if (RegistryManager.Instance.CurrentEngine != "raw")
        left = Environment.ProcessorCount > 8 ? 8 : Environment.ProcessorCount;
      return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
