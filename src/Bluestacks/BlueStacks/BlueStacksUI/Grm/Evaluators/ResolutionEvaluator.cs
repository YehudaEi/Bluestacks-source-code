// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.ResolutionEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class ResolutionEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.Resolution;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      string left = RegistryManager.Instance.Guest[context.VmName].GuestWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x" + RegistryManager.Instance.Guest[context.VmName].GuestHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      rightOperand = rightOperand.Replace(" ", string.Empty);
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
