// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.EvaluatorFactory
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.Grm.Evaluators;

namespace BlueStacks.BlueStacksUI.Grm
{
  internal class EvaluatorFactory
  {
    public static IRequirementEvaluator CreateandReturnEvaluator(
      GrmOperand operand)
    {
      switch (operand)
      {
        case GrmOperand.AppVersionCode:
          return (IRequirementEvaluator) new AppVersionEvaluator();
        case GrmOperand.ProductVersion:
          return (IRequirementEvaluator) new ProductVersionEvaluator();
        case GrmOperand.Geo:
          return (IRequirementEvaluator) new GeoEvaluator();
        case GrmOperand.Gpu:
          return (IRequirementEvaluator) new GpuEvaluator();
        case GrmOperand.Ram:
          return (IRequirementEvaluator) new RamEvaluator();
        case GrmOperand.PhysicalRam:
          return (IRequirementEvaluator) new PhysicalRamEvaluator();
        case GrmOperand.GlMode:
          return (IRequirementEvaluator) new GlModeEvaluator();
        case GrmOperand.EngineMode:
          return (IRequirementEvaluator) new EngineModeEvaluator();
        case GrmOperand.Is64Bit:
          return (IRequirementEvaluator) new Is64BitEvaluator();
        case GrmOperand.CpuCoresAllocated:
          return (IRequirementEvaluator) new CpuCoresAllocatedEvaluator();
        case GrmOperand.PhysicalCoresAvailable:
          return (IRequirementEvaluator) new PhysicalCoresAvailableEvaluator();
        case GrmOperand.Dpi:
          return (IRequirementEvaluator) new DpiEvaluator();
        case GrmOperand.Fps:
          return (IRequirementEvaluator) new FpsEvaluator();
        case GrmOperand.Resolution:
          return (IRequirementEvaluator) new ResolutionEvaluator();
        case GrmOperand.GuestOs:
          return (IRequirementEvaluator) new GuestOsEvaluator();
        case GrmOperand.Oem:
          return (IRequirementEvaluator) new OemEvaluator();
        case GrmOperand.InstalledOems:
          return (IRequirementEvaluator) new InstalledOemEvaluator();
        case GrmOperand.CustomKeyMappingExists:
          return (IRequirementEvaluator) new CustomKeyMappingExistsEvaluator();
        case GrmOperand.RegistryKeyValue:
          return (IRequirementEvaluator) new RegistryKeyValueEvaluator();
        case GrmOperand.BootParam:
          return (IRequirementEvaluator) new BootParamEvaluator();
        case GrmOperand.DeviceProfile:
          return (IRequirementEvaluator) new DeviceProfileEvaluator();
        case GrmOperand.ASTCTexture:
          return (IRequirementEvaluator) new ASTCTextureEvaluator();
        case GrmOperand.ABIMode:
          return (IRequirementEvaluator) new ABIModeEvaluator();
        case GrmOperand.AppRunningCountAcrossInstances:
          return (IRequirementEvaluator) new AppRunningCountAcrossInstancesEvaluator();
        default:
          return (IRequirementEvaluator) null;
      }
    }
  }
}
