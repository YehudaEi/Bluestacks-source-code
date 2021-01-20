// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.AppRequirement
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class AppRequirement
  {
    [JsonProperty(PropertyName = "pkgName")]
    public string PackageName { get; set; }

    [JsonProperty(PropertyName = "RuleSets")]
    public List<GrmRuleSet> GrmRuleSets { get; set; } = new List<GrmRuleSet>();

    public GrmRuleSet EvaluateRequirement(string packageName, string vmName)
    {
      GrmRuleSetContext context = new GrmRuleSetContext()
      {
        PackageName = packageName,
        VmName = vmName
      };
      foreach (GrmRuleSet grmRuleSet in this.GrmRuleSets)
      {
        if (!((IEnumerable<string>) RegistryManager.Instance.Guest[vmName].GrmDonotShowRuleList).Contains<string>(grmRuleSet.RuleId))
        {
          context.RuleSetId = grmRuleSet.RuleId;
          bool flag1 = false;
          foreach (GrmRule rule in grmRuleSet.Rules)
          {
            bool flag2 = true;
            foreach (GrmExpression expression in rule.Expressions)
            {
              flag2 = flag2 && expression.EvaluateExpression(context);
              if (!flag2)
                break;
            }
            if (flag2)
            {
              flag1 = true;
              break;
            }
          }
          if (flag1)
            return grmRuleSet;
        }
      }
      return (GrmRuleSet) null;
    }
  }
}
