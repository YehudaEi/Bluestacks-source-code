// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.GpuEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class GpuEvaluator : IRequirementEvaluator
  {
    private static Dictionary<string, string> mVmNameGpu = new Dictionary<string, string>();

    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.Gpu;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      if (!GpuEvaluator.mVmNameGpu.ContainsKey(context.VmName) || string.IsNullOrEmpty(GpuEvaluator.mVmNameGpu[context.VmName]))
      {
        string glVendor;
        string glRenderer;
        Utils.GetCurrentGraphicsInfo(RegistryManager.Instance.Guest[context.VmName].GlRenderMode.ToString() + " " + RegistryManager.Instance.Guest[context.VmName].GlMode.ToString(), out glVendor, out glRenderer, out string _);
        string str = glVendor + " " + glRenderer;
        GpuEvaluator.mVmNameGpu[context.VmName] = str;
        Logger.Info("GpuEvaluator " + str);
      }
      return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, GpuEvaluator.mVmNameGpu[context.VmName], rightOperand, context);
    }
  }
}
