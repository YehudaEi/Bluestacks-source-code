// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.PhysicalRamEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  public class PhysicalRamEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.PhysicalRam;
      }
    }

    public static string RAM
    {
      get
      {
        int num = 0;
        try
        {
          num = (int) (new ComputerInfo().TotalPhysicalMemory / 1048576UL);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when finding ram");
          Logger.Error(ex.ToString());
        }
        return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      int result;
      int.TryParse(PhysicalRamEvaluator.RAM, out result);
      int left = (int) ((double) result * 0.5);
      if (left >= 4096)
        left = 4096;
      if (RegistryManager.Instance.CurrentEngine == "raw" && left >= 3072)
        left = 3072;
      return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, left, rightOperand, context);
    }
  }
}
