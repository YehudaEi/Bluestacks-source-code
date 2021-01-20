// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.HTTP
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace BlueStacks.Common
{
  public static class HTTP
  {
    public static string Get(
      string url,
      Dictionary<string, string> headers,
      bool gzip,
      int tries,
      int sleepTimeMSec,
      int timeout,
      NameValueCollection headerCollection = null,
      string userAgent = "")
    {
      string str = (string) null;
      int num = tries;
      while (num > 0)
      {
        try
        {
          str = HTTP.GetInternal(url, headers, gzip, timeout, headerCollection, userAgent);
          break;
        }
        catch (Exception ex)
        {
          if (num == 1)
            throw;
          else
            Logger.Warning("Exception when GET to url: {0}, Ex: {1}", (object) url, (object) ex.Message);
        }
        --num;
        Thread.Sleep(sleepTimeMSec);
      }
      return str;
    }

    private static string GetInternal(
      string url,
      Dictionary<string, string> headers,
      bool gzip,
      int timeout,
      NameValueCollection headerCollection = null,
      string userAgent = "")
    {
      HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
      httpWebRequest.Method = "GET";
      if (timeout != 0)
        httpWebRequest.Timeout = timeout;
      if (gzip)
      {
        httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
        httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, nameof (gzip));
      }
      if (headerCollection != null)
        httpWebRequest.Headers.Add(headerCollection);
      if (!string.IsNullOrEmpty(userAgent))
        httpWebRequest.UserAgent = userAgent;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
          httpWebRequest.Headers.Set(StringUtils.GetControlCharFreeString(header.Key), StringUtils.GetControlCharFreeString(header.Value));
      }
      using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
      {
        Logger.Debug("Response StatusCode:" + response.StatusCode.ToString());
        using (Stream responseStream = response.GetResponseStream())
        {
          using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
            return streamReader.ReadToEnd();
        }
      }
    }

    public static string Post(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers,
      bool gzip,
      int retries,
      int sleepTimeMSecs,
      int timeout,
      NameValueCollection headerCollection,
      string userAgent)
    {
      string str = (string) null;
      int num = retries;
      while (num > 0)
      {
        try
        {
          str = HTTP.PostInternal(url, data, headers, gzip, timeout, headerCollection, userAgent);
          break;
        }
        catch (Exception ex)
        {
          if (num == 1)
            throw;
          else
            Logger.Warning("Exception when posting to url: {0}, Ex: {1}", (object) url, (object) ex.Message);
        }
        --num;
        Thread.Sleep(sleepTimeMSecs);
      }
      return str;
    }

    private static string PostInternal(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers,
      bool gzip,
      int timeout,
      NameValueCollection headerCollection,
      string userAgent)
    {
      HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
      httpWebRequest.Method = "POST";
      if (timeout != 0)
        httpWebRequest.Timeout = timeout;
      if (gzip)
      {
        httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
        httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, nameof (gzip));
      }
      if (headerCollection != null)
        httpWebRequest.Headers.Add(headerCollection);
      if (!string.IsNullOrEmpty(userAgent))
        httpWebRequest.UserAgent = userAgent;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
          httpWebRequest.Headers.Set(StringUtils.GetControlCharFreeString(header.Key), StringUtils.GetControlCharFreeString(header.Value));
      }
      if (data == null)
        data = new Dictionary<string, string>();
      byte[] bytes = Encoding.UTF8.GetBytes(StringUtils.Encode(data));
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.ContentLength = (long) bytes.Length;
      using (Stream requestStream = httpWebRequest.GetRequestStream())
      {
        requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          Logger.Debug("Response StatusCode:" + response.StatusCode.ToString());
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
              return streamReader.ReadToEnd();
          }
        }
      }
    }
  }
}
