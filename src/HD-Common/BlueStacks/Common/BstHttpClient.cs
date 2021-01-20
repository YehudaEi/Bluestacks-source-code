// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BstHttpClient
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace BlueStacks.Common
{
  public static class BstHttpClient
  {
    public static string Get(
      string url,
      Dictionary<string, string> headers,
      bool gzip,
      string vmName = "Android",
      int timeout = 0,
      int retries = 1,
      int sleepTimeMSec = 0,
      bool isOnUIThreadOnPurpose = false,
      string oem = "bgp")
    {
      try
      {
        if (oem == null)
          oem = "bgp";
        return BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
      }
      catch (Exception ex)
      {
        if (url == null)
          throw new Exception("url cannot be  null");
        if (oem == null)
          oem = "bgp";
        if (url.Contains(RegistryManager.Instance.Host))
        {
          Logger.Error("GET failed: {0}", (object) ex.Message);
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
          return BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
        }
        if (url.Contains(RegistryManager.Instance.Host2))
        {
          Logger.Error("GET failed: {0}", (object) ex.Message);
          url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
          return BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
        }
        throw;
      }
    }

    private static string GetInternal(
      string url,
      Dictionary<string, string> headers,
      bool gzip,
      int retries,
      int sleepTimeMSec,
      int timeout,
      string vmName,
      bool isOnUIThreadOnPurpose,
      string oem = "bgp")
    {
      if (Thread.CurrentThread.ManagedThreadId == 1 && !isOnUIThreadOnPurpose)
        Logger.Warning("WARNING: This network call is from the UI thread. StackTrace: {0}", (object) new StackTrace());
      NameValueCollection headerCollection = HTTPUtils.GetRequestHeaderCollection(vmName);
      Uri uri = new Uri(url);
      if (uri.Host.Contains("localhost") || uri.Host.Contains("127.0.0.1"))
      {
        RegistryKey registryKey = RegistryUtils.InitKeyWithSecurityCheck("Software\\BlueStacks" + (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem));
        headerCollection.Add("x_api_token", (string) registryKey.GetValue("ApiToken", (object) ""));
      }
      else
        headerCollection.Remove("x_api_token");
      return HTTP.Get(url, headers, gzip, retries, sleepTimeMSec, timeout, headerCollection, Utils.GetUserAgent(oem));
    }

    public static string Post(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers = null,
      bool gzip = false,
      string vmName = "Android",
      int timeout = 0,
      int retries = 1,
      int sleepTimeMSec = 0,
      bool isOnUIThreadOnPurpose = false,
      string oem = "bgp")
    {
      try
      {
        if (oem == null)
          oem = "bgp";
        if (url != null && Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
        return BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
      }
      catch (Exception ex)
      {
        if (url == null)
          throw new Exception("url cannot be  null");
        if (oem == null)
          oem = "bgp";
        if (url.Contains(RegistryManager.Instance.Host))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
          return BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
        }
        if (url.Contains(RegistryManager.Instance.Host2))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
          return BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
        }
        throw;
      }
    }

    private static string PostInternal(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers,
      bool gzip,
      int retries,
      int sleepTimeMSecs,
      int timeout,
      string vmName,
      bool isOnUIThreadOnPurpose,
      string oem = "bgp")
    {
      if (Thread.CurrentThread.ManagedThreadId == 1 && !isOnUIThreadOnPurpose)
        Logger.Warning("WARNING: This network call is from UI Thread and its stack trace is {0}", (object) new StackTrace().ToString());
      NameValueCollection headerCollection = HTTPUtils.GetRequestHeaderCollection(vmName);
      if (data == null)
        data = new Dictionary<string, string>();
      Uri uri = new Uri(url);
      if (uri.Host.Contains("localhost") || uri.Host.Contains("127.0.0.1"))
      {
        RegistryKey registryKey = RegistryUtils.InitKeyWithSecurityCheck("Software\\BlueStacks" + (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem));
        headerCollection.Add("x_api_token", (string) registryKey.GetValue("ApiToken", (object) ""));
      }
      else
      {
        headerCollection.Remove("x_api_token");
        data = Utils.AddCommonData(data);
      }
      return HTTP.Post(url, data, headers, gzip, retries, sleepTimeMSecs, timeout, headerCollection, Utils.GetUserAgent(oem));
    }

    public static string HTTPGaeFileUploader(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers,
      string filepath,
      string contentType,
      bool gzip,
      string vmName)
    {
      if (data == null)
        data = new Dictionary<string, string>();
      try
      {
        if (url != null && Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
        return BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
      }
      catch (Exception ex)
      {
        if (url == null)
          throw new Exception("url cannot be  null");
        if (url.Contains(RegistryManager.Instance.Host))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
          return BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
        }
        if (url.Contains(RegistryManager.Instance.Host2))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
          return BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
        }
        throw;
      }
    }

    private static string HTTPGaeFileUploaderInternal(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers,
      string filepath,
      string contentType,
      bool gzip,
      string vmName)
    {
      if (data == null)
        data = new Dictionary<string, string>();
      if (filepath == null || !System.IO.File.Exists(filepath))
        return BstHttpClient.Post(url, data, headers, gzip, vmName, 0, 1, 0, false, "bgp");
      JObject jobject = JObject.Parse(BstHttpClient.Get(url, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
      string url1 = (string) null;
      string str1 = (string) null;
      string str2 = "";
      if (jobject["success"].ToObject<bool>())
      {
        url1 = jobject[nameof (url)].ToString();
        try
        {
          str1 = jobject["country"].ToString();
        }
        catch
        {
          try
          {
            str1 = new RegionInfo(CultureInfo.CurrentCulture.Name).TwoLetterISORegionName;
          }
          catch
          {
            str1 = "US";
          }
        }
      }
      data.Add("country", str1);
      if (Oem.Instance.IsOEMWithBGPClient)
        str2 = RegistryManager.Instance.ClientVersion;
      data.Add("client_ver", str2);
      return BstHttpClient.HttpUploadFile(url1, filepath, "file", contentType, headers, data);
    }

    private static string HttpUploadFile(
      string url,
      string file,
      string paramName,
      string contentType,
      Dictionary<string, string> headers,
      Dictionary<string, string> data)
    {
      try
      {
        if (Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
        return BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
      }
      catch (Exception ex)
      {
        if (url.Contains(RegistryManager.Instance.Host))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
          return BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
        }
        if (url.Contains(RegistryManager.Instance.Host2))
        {
          Logger.Error(ex.Message);
          url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
          return BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
        }
        throw;
      }
    }

    private static string HttpUploadFileInternal(
      string url,
      string file,
      string paramName,
      string contentType,
      Dictionary<string, string> headers,
      Dictionary<string, string> data)
    {
      Logger.Info("Uploading {0} to {1}", (object) file, (object) url);
      string str1 = "---------------------------" + DateTime.Now.Ticks.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture);
      byte[] bytes1 = Encoding.ASCII.GetBytes("\r\n--" + str1 + "\r\n");
      Uri destination = new Uri(url);
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.ContentType = "multipart/form-data; boundary=" + str1;
      httpWebRequest.Method = "POST";
      httpWebRequest.KeepAlive = true;
      httpWebRequest.Timeout = 300000;
      httpWebRequest.UserAgent = Utils.GetUserAgent("bgp");
      if (!destination.Host.Contains("localhost") && !destination.Host.Contains("127.0.0.1"))
      {
        Uri proxy = httpWebRequest.Proxy.GetProxy(destination);
        Logger.Debug("URI of proxy = " + ((object) proxy != null ? proxy.ToString() : (string) null));
      }
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
          httpWebRequest.Headers.Set(StringUtils.GetControlCharFreeString(header.Key), StringUtils.GetControlCharFreeString(header.Value));
      }
      httpWebRequest.Headers.Add(HTTPUtils.GetRequestHeaderCollection(""));
      if (data == null)
        data = new Dictionary<string, string>();
      Stream requestStream = httpWebRequest.GetRequestStream();
      string format = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
      foreach (KeyValuePair<string, string> keyValuePair in data)
      {
        requestStream.Write(bytes1, 0, bytes1.Length);
        byte[] bytes2 = Encoding.UTF8.GetBytes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) keyValuePair.Key, (object) keyValuePair.Value));
        requestStream.Write(bytes2, 0, bytes2.Length);
      }
      requestStream.Write(bytes1, 0, bytes1.Length);
      byte[] bytes3 = Encoding.UTF8.GetBytes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", (object) paramName, (object) file, (object) contentType));
      requestStream.Write(bytes3, 0, bytes3.Length);
      string str2 = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Path.GetFileName(file)) + "_bst";
      System.IO.File.Copy(file, str2);
      if (contentType.Equals("text/plain", StringComparison.InvariantCultureIgnoreCase))
      {
        string s = System.IO.File.ReadAllText(str2);
        byte[] numArray = new byte[1048576];
        byte[] bytes2 = Encoding.UTF8.GetBytes(s);
        requestStream.Write(bytes2, 0, bytes2.Length);
      }
      else
      {
        FileStream fileStream = new FileStream(str2, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[4096];
        int count;
        while ((count = fileStream.Read(buffer, 0, buffer.Length)) != 0)
          requestStream.Write(buffer, 0, count);
        fileStream.Close();
      }
      System.IO.File.Delete(str2);
      byte[] bytes4 = Encoding.ASCII.GetBytes("\r\n--" + str1 + "--\r\n");
      requestStream.Write(bytes4, 0, bytes4.Length);
      requestStream.Close();
      string str3 = (string) null;
      WebResponse webResponse = (WebResponse) null;
      try
      {
        webResponse = httpWebRequest.GetResponse();
        using (Stream responseStream = webResponse.GetResponseStream())
        {
          using (StreamReader streamReader = new StreamReader(responseStream))
          {
            str3 = streamReader.ReadToEnd();
            Logger.Info("File uploaded, server response is: {0}", (object) str3);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error uploading file", (object) ex);
        webResponse?.Close();
        throw;
      }
      finally
      {
      }
      return str3;
    }

    public static string PostMultipart(
      string url,
      Dictionary<string, object> parameters,
      out byte[] dataArray)
    {
      string str1 = "---------------------------" + DateTime.Now.Ticks.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture);
      byte[] bytes1 = Encoding.ASCII.GetBytes("\r\n--" + str1 + "\r\n");
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.ContentType = "multipart/form-data; boundary=" + str1;
      httpWebRequest.Method = "POST";
      httpWebRequest.KeepAlive = true;
      httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
      if (parameters != null && parameters.Count > 0)
      {
        using (Stream requestStream = httpWebRequest.GetRequestStream())
        {
          foreach (KeyValuePair<string, object> parameter in parameters)
          {
            requestStream.Write(bytes1, 0, bytes1.Length);
            if (parameter.Value is FormFile)
            {
              FormFile formFile = parameter.Value as FormFile;
              byte[] bytes2 = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + parameter.Key + "\"; filename=\"" + formFile.Name + "\"\r\nContent-Type: " + formFile.ContentType + "\r\n\r\n");
              requestStream.Write(bytes2, 0, bytes2.Length);
              byte[] buffer = new byte[32768];
              if (formFile.Stream == null)
              {
                using (FileStream fileStream = System.IO.File.OpenRead(formFile.FilePath))
                {
                  int count;
                  while ((count = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, count);
                  fileStream.Close();
                }
              }
              else
              {
                int count;
                while ((count = formFile.Stream.Read(buffer, 0, buffer.Length)) != 0)
                  requestStream.Write(buffer, 0, count);
              }
            }
            else
            {
              byte[] bytes2 = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + parameter.Key + "\"\r\n\r\n" + parameter.Value?.ToString());
              requestStream.Write(bytes2, 0, bytes2.Length);
            }
          }
          byte[] bytes3 = Encoding.ASCII.GetBytes("\r\n--" + str1 + "--\r\n");
          requestStream.Write(bytes3, 0, bytes3.Length);
          requestStream.Close();
        }
      }
      HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
      string str2 = ((HttpWebResponse) httpWebRequest.GetResponse()).ToString();
      int result;
      using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
      {
        httpStatusCode = response.StatusCode;
        if (int.TryParse(response.Headers.Get("Content-Length"), out result))
          Logger.Info("content lenght.." + result.ToString());
        using (BinaryReader binaryReader = new BinaryReader(response.GetResponseStream()))
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            for (byte[] buffer = binaryReader.ReadBytes(16384); buffer.Length != 0; buffer = binaryReader.ReadBytes(16384))
              memoryStream.Write(buffer, 0, buffer.Length);
            dataArray = new byte[(int) memoryStream.Length];
            memoryStream.Position = 0L;
            memoryStream.Read(dataArray, 0, dataArray.Length);
          }
        }
      }
      if (httpStatusCode != HttpStatusCode.OK || result < 2000)
        str2 = "error";
      return str2;
    }
  }
}
