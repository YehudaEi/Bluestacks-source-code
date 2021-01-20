// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BlueStacksUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace BlueStacks.Common
{
  public static class BlueStacksUtils
  {
    public static bool IsSignedByBlueStacks(string filePath)
    {
      Logger.Info("Checking if {0} is signed", (object) filePath);
      bool flag = false;
      try
      {
        X509Certificate2 x509Certificate2 = new X509Certificate2(X509Certificate.CreateFromSignedFile(filePath));
        Logger.Debug("Certificate issuer name is: " + x509Certificate2.IssuerName.Name);
        string name = x509Certificate2.SubjectName.Name;
        Logger.Debug("Certificate issued by: " + name);
        if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(name, "Bluestack Systems, Inc.", CompareOptions.IgnoreCase) >= 0)
        {
          Logger.Info("File signed by BlueStacks");
          if (x509Certificate2.Verify())
          {
            Logger.Info("Certificate verified");
            flag = true;
          }
          else
            Logger.Warning("Certificate not verified");
        }
        else
          Logger.Warning("File not signed by BlueStacks. Signed by {0}", (object) name);
      }
      catch (Exception ex)
      {
        Logger.Error("File not signed");
        Logger.Error(ex.ToString());
      }
      return flag;
    }
  }
}
