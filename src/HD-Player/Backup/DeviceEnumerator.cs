// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.DeviceEnumerator
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace BlueStacks.Player
{
  public class DeviceEnumerator : IDisposable
  {
    private IMoniker m_Moniker;
    private string m_FriendlyName;

    public static List<DeviceEnumerator> ListDevices(Guid filterType)
    {
      List<DeviceEnumerator> deviceEnumeratorList = new List<DeviceEnumerator>();
      IEnumMoniker ppEnumMoniker;
      ErrorHandler classEnumerator = (ErrorHandler) ((ICreateDevEnum) new CreateDevEnum()).CreateClassEnumerator(filterType, out ppEnumMoniker, 0);
      if (ppEnumMoniker != null)
      {
        try
        {
          IMoniker[] rgelt = (IMoniker[]) null;
          try
          {
            while (true)
            {
              DeviceEnumerator deviceEnumerator;
              string property;
              do
              {
                rgelt = new IMoniker[1];
                if (ppEnumMoniker.Next(1, rgelt, IntPtr.Zero) != 0)
                {
                  Logger.Info("Breaking out of loop..");
                  goto label_11;
                }
                else
                {
                  deviceEnumerator = new DeviceEnumerator()
                  {
                    m_Moniker = rgelt[0]
                  };
                  deviceEnumerator.m_FriendlyName = deviceEnumerator.getProperty("FriendlyName");
                  property = deviceEnumerator.getProperty("DevicePath");
                }
              }
              while (deviceEnumerator.m_FriendlyName == null || property == null || !property.Contains("\\usb#vid") && !property.Contains("\\pci#ven"));
              deviceEnumeratorList.Add(deviceEnumerator);
              Logger.Info("Camera device {0}", (object) deviceEnumerator.m_FriendlyName);
            }
          }
          catch (Exception ex)
          {
            if (rgelt != null)
              Marshal.ReleaseComObject((object) rgelt[0]);
            deviceEnumeratorList = (List<DeviceEnumerator>) null;
            Logger.Error("Failed to enumerate Video input devices: {0}", (object) ex.ToString());
            throw;
          }
        }
        finally
        {
          Marshal.ReleaseComObject((object) ppEnumMoniker);
        }
      }
      else
      {
        Logger.Error("Cannot enumerate the device");
        deviceEnumeratorList = (List<DeviceEnumerator>) null;
      }
label_11:
      return deviceEnumeratorList;
    }

    public void Dispose()
    {
      if (this.m_Moniker != null)
      {
        Marshal.ReleaseComObject((object) this.m_Moniker);
        this.m_Moniker = (IMoniker) null;
      }
      this.m_FriendlyName = (string) null;
    }

    public string FriendlyName
    {
      get
      {
        return this.m_FriendlyName;
      }
    }

    public IMoniker Moniker
    {
      get
      {
        return this.m_Moniker;
      }
    }

    public Guid ClassGUID
    {
      get
      {
        Guid pClassID;
        this.m_Moniker.GetClassID(out pClassID);
        return pClassID;
      }
    }

    public string GetDisplayName
    {
      get
      {
        string ppszDisplayName = (string) null;
        try
        {
          this.m_Moniker.GetDisplayName((IBindCtx) null, (IMoniker) null, out ppszDisplayName);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
        return ppszDisplayName;
      }
    }

    public string getProperty(string sProperty)
    {
      object ppvObj = (object) null;
      IPropertyBag propertyBag = (IPropertyBag) null;
      string str = (string) null;
      Guid guid = typeof (IPropertyBag).GUID;
      try
      {
        this.m_Moniker.BindToStorage((IBindCtx) null, (IMoniker) null, ref guid, out ppvObj);
        propertyBag = (IPropertyBag) ppvObj;
        object pVar;
        ErrorHandler errorHandler = (ErrorHandler) propertyBag.Read(sProperty, out pVar, (IErrorLog) null);
        str = (string) pVar;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in fetch Property.  {0} Err :", (object) sProperty, (object) ex.ToString());
      }
      finally
      {
        if (propertyBag != null)
          Marshal.ReleaseComObject((object) propertyBag);
      }
      return str;
    }
  }
}
