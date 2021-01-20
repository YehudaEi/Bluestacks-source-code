// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InstallerErrorHandling
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Linq;

namespace BlueStacks.Common
{
  public static class InstallerErrorHandling
  {
    public static string AssignErrorStringForInstallerExitCodes(
      int mInstallFailedErrorCode,
      string prefixKey)
    {
      string str1 = LocaleStrings.GetLocalizedString(prefixKey, "");
      string str2 = ((InstallerCodes) mInstallFailedErrorCode).ToString();
      string str3 = string.Empty;
      bool flag = true;
      if (prefixKey != "STRING_ROLLBACK_FAILED_SORRY_MESSAGE")
      {
        switch (mInstallFailedErrorCode)
        {
          case -59:
          case -58:
            str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}", (object) LocaleStrings.GetLocalizedString("STRING_OLD_INSTALLATION_INTERFERING", ""), (object) LocaleStrings.GetLocalizedString("STRING_TRY_RESTARTING_MACHINE", ""));
            break;
          case -55:
          case -54:
          case -53:
          case -52:
            str3 = LocaleStrings.GetLocalizedString("STRING_COULDNT_RESTORE_UNUSABLE", "");
            break;
          case -51:
          case -49:
          case -43:
          case -42:
          case -41:
          case -40:
          case -39:
          case -38:
          case -37:
          case -36:
          case -35:
          case -33:
            str3 = LocaleStrings.GetLocalizedString("STRING_ERROR_OCCURED_DEPLOYING_FILES", "");
            break;
          case -46:
          case -45:
          case -44:
          case -32:
            str3 = LocaleStrings.GetLocalizedString("STRING_FAILED_TO_RESTORE_OLD_DATA", "");
            break;
          case -30:
            str3 = LocaleStrings.GetLocalizedString("STRING_OLD_INSTALLATION_INTERFERING", "");
            break;
          case -19:
            str3 = LocaleStrings.GetLocalizedString("STRING_HYPERV_DISABLED_WARNING", "");
            flag = false;
            str1 = string.Empty;
            break;
          case -18:
            str3 = LocaleStrings.GetLocalizedString("STRING_HYPERV_INSTALLER_WARNING", "");
            flag = false;
            str1 = string.Empty;
            break;
          case -17:
            str3 = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT", "");
            flag = false;
            str1 = string.Empty;
            break;
          case -14:
            str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_INSUFFICIENT_DISKSPACE", ""), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}GB", (object) 5L));
            break;
          default:
            string id = "STRING_" + str2;
            str3 = LocaleStrings.GetLocalizedString(id, "");
            if (str3.Equals(id, StringComparison.InvariantCultureIgnoreCase))
            {
              str3 = LocaleStrings.GetLocalizedString("STRING_ERROR_OCCURED_DEPLOYING_FILES", "");
              break;
            }
            break;
        }
      }
      if (Enumerable.Range(-30, 20).Contains<int>(mInstallFailedErrorCode) & flag)
        str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_PREINSTALL_FAIL", ""), (object) str3);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n\n{2} {3}", (object) str1, (object) str3, (object) LocaleStrings.GetLocalizedString("STRING_ERROR_CODE_COLON", ""), (object) str2).TrimStart('\n');
    }
  }
}
