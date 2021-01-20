// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Evaluators.RegistryKeyValueEvaluator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlueStacks.BlueStacksUI.Grm.Evaluators
{
  internal class RegistryKeyValueEvaluator : IRequirementEvaluator
  {
    public GrmOperand EvaluatorForOperandType
    {
      get
      {
        return GrmOperand.RegistryKeyValue;
      }
    }

    public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
    {
      string strA = "RegistryManager";
      Type objType = (Type) null;
      TypeCode typeCode = TypeCode.String;
      if (!(JsonConvert.DeserializeObject(context.ContextJson, Utils.GetSerializerSettings()) is JObject jobject))
        throw new ArgumentNullException("RegistryKeyValueEvaluator requires contextjson" + context.ContextJson);
      if (!jobject.ContainsKey("propertyName"))
        throw new ArgumentException("propertyName required in context json for RegisryKeyValueEvaluator");
      string str = jobject["propertyName"].Value<string>();
      if (jobject.ContainsKey("location") && !string.IsNullOrEmpty(jobject["location"].Value<string>()))
        strA = jobject["location"].Value<string>();
      object o;
      if (string.Compare(strA, "registryManager", StringComparison.OrdinalIgnoreCase) == 0)
        o = RegistryManager.Instance.GetPropValue(str, out objType);
      else if (string.Compare(strA, "instanceManager", StringComparison.OrdinalIgnoreCase) == 0)
      {
        o = RegistryManager.Instance.Guest[context.VmName].GetPropValue(str, out objType);
      }
      else
      {
        if (!jobject.ContainsKey("propertyPath"))
          throw new ArgumentException("propertyPath required in context json for RegisryKeyValueEvaluator");
        o = RegistryUtils.GetRegistryValue(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\{1}", (object) Strings.GetOemTag(), (object) jobject["propertyPath"].Value<string>().Replace("vmName", context.VmName)), str, (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
        typeCode = EnumHelper.Parse<TypeCode>(jobject["propertyTypeCode"].Value<string>(), TypeCode.String);
      }
      if (o == null)
        throw new MissingMemberException("Cannot find " + str);
      if (o.IsList())
        return GrmComparer<List<string>>.Evaluate(this.EvaluatorForOperandType, grmOperator, (List<string>) o, rightOperand, context);
      if ((object) objType != null)
        typeCode = Type.GetTypeCode(objType);
      switch (typeCode)
      {
        case TypeCode.Boolean:
          return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, (bool) o, rightOperand, context);
        case TypeCode.Int32:
          return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, (int) o, rightOperand, context);
        case TypeCode.Int64:
          return GrmComparer<long>.Evaluate(this.EvaluatorForOperandType, grmOperator, (long) o, rightOperand, context);
        case TypeCode.Double:
          return GrmComparer<double>.Evaluate(this.EvaluatorForOperandType, grmOperator, (double) o, rightOperand, context);
        case TypeCode.Decimal:
          return GrmComparer<Decimal>.Evaluate(this.EvaluatorForOperandType, grmOperator, (Decimal) o, rightOperand, context);
        case TypeCode.DateTime:
          return GrmComparer<DateTime>.Evaluate(this.EvaluatorForOperandType, grmOperator, (DateTime) o, rightOperand, context);
        case TypeCode.String:
          return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, (string) o, rightOperand, context);
        default:
          throw new Exception("Type of property is not known " + str);
      }
    }
  }
}
