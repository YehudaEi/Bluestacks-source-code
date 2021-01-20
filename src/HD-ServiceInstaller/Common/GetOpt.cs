// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GetOpt
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BlueStacks.Common
{
  public class GetOpt
  {
    private ArrayList mInvalidArgs = new ArrayList();

    public void Parse(string[] args)
    {
      int index = 0;
      if (args == null)
        return;
      for (; index < args.Length; ++index)
      {
        int pos = this.OptionPos(args[index]);
        if (pos > 0)
        {
          if (this.GetOption(args, ref index, pos))
            ++this.Count;
          else
            this.InvalidOption(args[Math.Min(index, args.Length - 1)]);
        }
        else
        {
          if (this.Args == null)
            this.Args = new ArrayList();
          this.Args.Add((object) args[index]);
          if (!this.IsValidArg(args[index]))
            this.InvalidOption(args[index]);
        }
      }
    }

    public IList InvalidArgs
    {
      get
      {
        return (IList) this.mInvalidArgs;
      }
    }

    public bool NoArgs
    {
      get
      {
        return this.ArgCount == 0 && this.Count == 0;
      }
    }

    public int ArgCount
    {
      get
      {
        return this.Args != null ? this.Args.Count : 0;
      }
    }

    public bool IsInValid
    {
      get
      {
        return this.IsInvalid;
      }
    }

    protected virtual int OptionPos(string opt)
    {
      if (opt == null || opt.Length < 2)
        return 0;
      char[] charArray;
      if (opt.Length > 2)
      {
        charArray = opt.ToCharArray(0, 3);
        if (charArray[0] == '-' && charArray[1] == '-' && this.IsOptionNameChar(charArray[2]))
          return 2;
      }
      else
        charArray = opt.ToCharArray(0, 2);
      return charArray[0] == '-' && this.IsOptionNameChar(charArray[1]) ? 1 : 0;
    }

    protected virtual bool IsOptionNameChar(char c)
    {
      return char.IsLetterOrDigit(c) || c == '?';
    }

    protected virtual void InvalidOption(string name)
    {
      this.mInvalidArgs.Add((object) name);
      this.IsInvalid = true;
    }

    protected virtual bool IsValidArg(string arg)
    {
      return true;
    }

    protected virtual bool MatchName(MemberInfo field, string name)
    {
      foreach (ArgAttribute customAttribute in field?.GetCustomAttributes(typeof (ArgAttribute), true))
      {
        if (string.Compare(customAttribute.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
          return true;
      }
      return false;
    }

    protected virtual PropertyInfo GetMemberProperty(string name)
    {
      foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (string.Compare(property.Name, name, StringComparison.OrdinalIgnoreCase) == 0 || this.MatchName((MemberInfo) property, name))
          return property;
      }
      return (PropertyInfo) null;
    }

    protected virtual FieldInfo GetMemberField(string name)
    {
      foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
      {
        if (string.Compare(field.Name, name, StringComparison.OrdinalIgnoreCase) == 0 || this.MatchName((MemberInfo) field, name))
          return field;
      }
      return (FieldInfo) null;
    }

    protected virtual object GetOptionValue(MemberInfo field)
    {
      object[] customAttributes = field?.GetCustomAttributes(typeof (ArgAttribute), true);
      foreach (object obj in customAttributes)
        Console.WriteLine(obj);
      return customAttributes.Length != 0 ? ((ArgAttribute) customAttributes[0]).Value : (object) null;
    }

    protected virtual bool GetOption(string[] args, ref int index, int pos)
    {
      try
      {
        object val = (object) null;
        string opt = args?[index]?.Substring(pos, args[index].Length - pos);
        this.SplitOptionAndValue(ref opt, ref val);
        FieldInfo memberField = this.GetMemberField(opt);
        if (memberField != null)
        {
          object obj1 = this.GetOptionValue((MemberInfo) memberField);
          if (obj1 == null)
          {
            if (memberField.FieldType == typeof (bool))
              obj1 = (object) true;
            else if (memberField.FieldType == typeof (string))
            {
              object obj2 = val ?? (object) args[++index];
              memberField.SetValue((object) this, Convert.ChangeType(obj2, memberField.FieldType, (IFormatProvider) CultureInfo.InvariantCulture));
              switch ((string) obj2)
              {
                case "":
                case null:
                  return false;
                default:
                  return true;
              }
            }
            else
              obj1 = !memberField.FieldType.IsEnum ? val ?? (object) args[++index] : Enum.Parse(memberField.FieldType, (string) val, true);
          }
          memberField.SetValue((object) this, Convert.ChangeType(obj1, memberField.FieldType, (IFormatProvider) CultureInfo.InvariantCulture));
          return true;
        }
        PropertyInfo memberProperty = this.GetMemberProperty(opt);
        if (memberProperty != null)
        {
          object obj1 = this.GetOptionValue((MemberInfo) memberProperty);
          if (obj1 == null)
          {
            if (memberProperty.PropertyType == typeof (bool))
              obj1 = (object) true;
            else if (memberProperty.PropertyType == typeof (string))
            {
              object obj2 = val ?? (object) args[++index];
              memberProperty.SetValue((object) this, Convert.ChangeType(obj2, memberProperty.PropertyType, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
              switch ((string) obj2)
              {
                case "":
                case null:
                  return false;
                default:
                  return true;
              }
            }
            else
              obj1 = !memberProperty.PropertyType.IsEnum ? val ?? (object) args[++index] : Enum.Parse(memberProperty.PropertyType, (string) val, true);
          }
          memberProperty.SetValue((object) this, Convert.ChangeType(obj1, memberProperty.PropertyType, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    protected virtual void SplitOptionAndValue(ref string opt, ref object val)
    {
      int length = -1;
      if (opt != null)
        length = opt.IndexOfAny(new char[2]{ ':', '=' });
      if (length < 1)
        return;
      val = (object) opt.Substring(length + 1);
      opt = opt.Substring(0, length);
    }

    public virtual void Help()
    {
      Console.WriteLine(this.GetHelpText());
    }

    public virtual string GetHelpText()
    {
      StringBuilder stringBuilder = new StringBuilder();
      FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
      char ch = '-';
      foreach (FieldInfo fieldInfo in fields)
      {
        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof (ArgAttribute), true);
        if (customAttributes.Length != 0)
        {
          ArgAttribute argAttribute = (ArgAttribute) customAttributes[0];
          if (argAttribute.Description != null)
          {
            string str = "";
            if (argAttribute.Value == null)
            {
              if (fieldInfo.FieldType == typeof (int))
                str = "[Integer]";
              else if (fieldInfo.FieldType == typeof (float))
                str = "[Float]";
              else if (fieldInfo.FieldType == typeof (string))
                str = "[String]";
              else if (fieldInfo.FieldType == typeof (bool))
                str = "[Boolean]";
            }
            stringBuilder.AppendFormat("{0}{1,-20}\n\t{2}", (object) ch, (object) (fieldInfo.Name + str), (object) argAttribute.Description);
            if (argAttribute.Name != null)
              stringBuilder.AppendFormat(" (Name format: {0}{1}{2})", (object) ch, (object) argAttribute.Name, (object) str);
            stringBuilder.Append(Environment.NewLine);
          }
        }
      }
      return stringBuilder.ToString();
    }

    protected ArrayList Args { get; set; }

    protected bool IsInvalid { get; set; }

    public int Count { get; set; }
  }
}
