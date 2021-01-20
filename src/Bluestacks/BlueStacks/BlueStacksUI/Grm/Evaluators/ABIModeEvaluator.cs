// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.ABIModeEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class ABIModeEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.ABIMode;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      string valueInBootParams = Utils.GetValueInBootParams("abivalue", context.VmName, "", "bgp");
      string left;
      if (string.IsNullOrEmpty(valueInBootParams))
        left = ABISetting.Auto.ToString();
      else if (EngineSettingBaseViewModel.Is64BitABIValuesValid("bgp"))
      {
        string str;
        if (valueInBootParams != null)
        {
          if (!(valueInBootParams == "7"))
          {
            if (valueInBootParams == "15")
            {
              str = ABISetting.ARM.ToString();
              goto label_9;
            }
          }
          else
          {
            str = ABISetting.Auto.ToString();
            goto label_9;
          }
        }
        str = ABISetting.Custom.ToString();
label_9:
        left = str;
      }
      else
      {
        string str;
        if (valueInBootParams != null)
        {
          if (!(valueInBootParams == "15"))
          {
            if (valueInBootParams == "4")
            {
              str = ABISetting.ARM.ToString();
              goto label_16;
            }
          }
          else
          {
            str = ABISetting.Auto.ToString();
            goto label_16;
          }
        }
        str = ABISetting.Custom.ToString();
label_16:
        left = str;
      }
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
