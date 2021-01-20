// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.AppVersionEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class AppVersionEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.AppVersionCode;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      int left = int.Parse(new JsonParser(context.VmName).GetAppInfoFromPackageName(context.PackageName).Version, (IFormatProvider) CultureInfo.InvariantCulture);
      return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
