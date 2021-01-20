// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.CustomKeyMappingExistsEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System.IO;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class CustomKeyMappingExistsEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.CustomKeyMappingExists;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      bool left = File.Exists(Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), context.PackageName + ".cfg"));
      return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
