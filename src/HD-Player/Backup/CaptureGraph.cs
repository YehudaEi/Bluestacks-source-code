// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.CaptureGraph
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace BlueStacks.Player
{
  public class CaptureGraph : ISampleGrabberCB, IDisposable
  {
    private IntPtr m_Buffer = IntPtr.Zero;
    private int m_Unit;
    private int m_Width;
    private int m_Height;
    private int m_FrameRate;
    private int m_Stride;
    private int m_DroppedFrame;
    private ManualResetEvent m_Evt;
    private bool m_bGraphRunning;
    private volatile bool m_bGrabFrame;
    private SupportedColorFormat m_color;
    private IFilterGraph2 m_FilterGraph;
    private IMediaControl m_mediaCtrl;

    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

    public CaptureGraph(
      int unit,
      int width,
      int height,
      int framerate,
      SupportedColorFormat color)
    {
      this.m_Unit = unit;
      this.m_Width = width;
      this.m_Height = height;
      this.m_FrameRate = framerate;
      this.m_DroppedFrame = 0;
      this.m_color = color;
      this.m_Evt = new ManualResetEvent(false);
      this.m_bGraphRunning = false;
      Logger.Info("Building graph");
      try
      {
        this.BuildGraph();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in build graph. Err : {0}", (object) ex.ToString());
        this.Dispose();
        throw;
      }
    }

    ~CaptureGraph()
    {
      if (this.m_Buffer != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.m_Buffer);
        this.m_Buffer = IntPtr.Zero;
      }
      this.Dispose();
    }

    public void BuildGraph()
    {
      ICaptureGraphBuilder2 captureGraphBuilder2 = (ICaptureGraphBuilder2) null;
      IBaseFilter ppFilter = (IBaseFilter) null;
      ISampleGrabber sampleGrabber = (ISampleGrabber) null;
      List<DeviceEnumerator> deviceEnumeratorList = (List<DeviceEnumerator>) null;
      try
      {
        Logger.Info("Creating List of devices");
        deviceEnumeratorList = DeviceEnumerator.ListDevices(Guids.VideoInputDeviceCategory);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in finding Video device. Err : {0}", (object) ex.ToString());
      }
      if (deviceEnumeratorList != null)
      {
        if (deviceEnumeratorList.Count != 0)
        {
          try
          {
            Logger.Info("found {0} Camera, Opening {1}", (object) deviceEnumeratorList.Count, (object) this.m_Unit);
            DeviceEnumerator deviceEnumerator = this.m_Unit >= deviceEnumeratorList.Count ? deviceEnumeratorList[0] : deviceEnumeratorList[this.m_Unit];
            this.m_FilterGraph = (IFilterGraph2) new FilterGraph();
            this.m_mediaCtrl = this.m_FilterGraph as IMediaControl;
            captureGraphBuilder2 = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();
            sampleGrabber = (ISampleGrabber) new SampleGrabber();
            ErrorHandler errorHandler1 = (ErrorHandler) captureGraphBuilder2.SetFiltergraph((IGraphBuilder) this.m_FilterGraph);
            if (errorHandler1.GetError() != 0)
              Logger.Error("SetFiltergraph failed with {0:X}..", (object) errorHandler1.GetError());
            ErrorHandler errorHandler2 = (ErrorHandler) this.m_FilterGraph.AddSourceFilterForMoniker(deviceEnumerator.Moniker, (IBindCtx) null, "Video input", out ppFilter);
            if (errorHandler2.GetError() != 0)
              Logger.Error("AddSourceFilterForMoniker failed with {0:X}", (object) errorHandler2.GetError());
            AMMediaType pmt = new AMMediaType()
            {
              majorType = Guids.MediaTypeVideo
            };
            if (this.m_color == SupportedColorFormat.YUV2)
            {
              pmt.subType = Guids.MediaSubtypeYUY2;
            }
            else
            {
              if (this.m_color != SupportedColorFormat.RGB24)
                throw new Exception("Unsupported color format");
              pmt.subType = Guids.MediaSubtypeRGB24;
            }
            pmt.formatType = Guids.FormatTypesVideoInfo;
            ErrorHandler errorHandler3 = (ErrorHandler) sampleGrabber.SetMediaType(pmt);
            this.FreeAMMedia(pmt);
            ErrorHandler errorHandler4 = (ErrorHandler) sampleGrabber.SetCallback((ISampleGrabberCB) this, 1);
            if (errorHandler4.GetError() != 0)
              Logger.Error("Grabber setcallback failed with {0:X}", (object) errorHandler4.GetError());
            IBaseFilter baseFilter = (IBaseFilter) sampleGrabber;
            ErrorHandler errorHandler5 = (ErrorHandler) this.m_FilterGraph.AddFilter(baseFilter, "FrameGrabber");
            if (errorHandler5.GetError() != 0)
              Logger.Error("AddFilter failed with {0:X}", (object) errorHandler5.GetError());
            object ppint;
            ErrorHandler errorHandler6 = (ErrorHandler) captureGraphBuilder2.FindInterface(Guids.PinCategoryCapture, Guids.MediaTypeVideo, ppFilter, typeof (IAMStreamConfig).GUID, out ppint);
            if (errorHandler6.GetError() != 0)
              Logger.Error("FindInterface failed with {0:X}", (object) errorHandler6.GetError());
            if (!(ppint is IAMStreamConfig amStreamConfig))
              throw new Exception("Stream config Error");
            errorHandler3 = (ErrorHandler) amStreamConfig.GetFormat(out pmt);
            VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure(pmt.pbFormat, (object) videoInfoHeader);
            videoInfoHeader.AvgTimePerFrame = (long) (10000000 / this.m_FrameRate);
            videoInfoHeader.BmiHeader.Width = this.m_Width;
            videoInfoHeader.BmiHeader.Height = this.m_Height;
            Marshal.StructureToPtr((object) videoInfoHeader, pmt.pbFormat, false);
            ErrorHandler errorHandler7 = (ErrorHandler) amStreamConfig.SetFormat(pmt);
            if (errorHandler7.GetError() != 0)
              Logger.Error("conf.setformat failed with {0:X}", (object) errorHandler7.GetError());
            this.FreeAMMedia(pmt);
            ErrorHandler errorHandler8 = (ErrorHandler) captureGraphBuilder2.RenderStream(Guids.PinCategoryCapture, Guids.MediaTypeVideo, (object) ppFilter, (IBaseFilter) null, baseFilter);
            if (errorHandler8.GetError() != 0)
              Logger.Error("RenderStream failed with {0:X}", (object) errorHandler8.GetError());
            AMMediaType amMediaType = new AMMediaType();
            errorHandler3 = (ErrorHandler) sampleGrabber.GetConnectedMediaType(amMediaType);
            if (amMediaType.formatType != Guids.FormatTypesVideoInfo)
              throw new ColorFormatNotSupported("Not able to connect to Video Media");
            if (amMediaType.pbFormat == IntPtr.Zero)
              throw new Exception("Format Array is null");
            VideoInfoHeader structure = (VideoInfoHeader) Marshal.PtrToStructure(amMediaType.pbFormat, typeof (VideoInfoHeader));
            this.m_Width = structure.BmiHeader.Width;
            this.m_Height = structure.BmiHeader.Height;
            this.m_Stride = this.m_Width * ((int) structure.BmiHeader.BitCount / 8);
            if (this.m_Buffer == IntPtr.Zero)
              this.m_Buffer = Marshal.AllocCoTaskMem(this.m_Stride * this.m_Height);
            this.FreeAMMedia(amMediaType);
            return;
          }
          catch
          {
            throw;
          }
          finally
          {
            if (ppFilter != null)
              Marshal.ReleaseComObject((object) ppFilter);
            if (sampleGrabber != null)
              Marshal.ReleaseComObject((object) sampleGrabber);
            if (captureGraphBuilder2 != null)
              Marshal.ReleaseComObject((object) captureGraphBuilder2);
          }
        }
      }
      Logger.Info("CAMERA: Could not find a camera device!");
    }

    public void Dispose()
    {
      this.TearDownCom();
      if (this.m_Evt == null)
        return;
      this.m_Evt.Close();
      this.m_Evt = (ManualResetEvent) null;
    }

    private void FreeAMMedia(AMMediaType m)
    {
      if (m != null)
      {
        if (m.cbFormat != 0)
        {
          Marshal.FreeCoTaskMem(m.pbFormat);
          m.cbFormat = 0;
          m.pbFormat = IntPtr.Zero;
        }
        if (m.pUnk != IntPtr.Zero)
        {
          Marshal.Release(m.pUnk);
          m.pUnk = IntPtr.Zero;
        }
      }
      m = (AMMediaType) null;
    }

    int ISampleGrabberCB.SampleCB(double time, IMediaSample pSample)
    {
      Marshal.ReleaseComObject((object) pSample);
      return 0;
    }

    int ISampleGrabberCB.BufferCB(double time, IntPtr pBuffer, int len)
    {
      if (this.m_bGrabFrame)
      {
        if (len <= this.m_Stride * this.m_Height)
          CaptureGraph.CopyMemory(this.m_Buffer, pBuffer, len);
        this.m_bGrabFrame = false;
        this.m_Evt.Set();
      }
      else
        ++this.m_DroppedFrame;
      return 0;
    }

    public void Run()
    {
      if (this.m_bGraphRunning || this.m_mediaCtrl == null)
        return;
      ErrorHandler errorHandler = (ErrorHandler) this.m_mediaCtrl.Run();
      this.m_bGraphRunning = true;
    }

    public void Pause()
    {
      if (!this.m_bGraphRunning)
        return;
      ErrorHandler errorHandler = (ErrorHandler) this.m_mediaCtrl.Pause();
      this.m_bGraphRunning = false;
    }

    public IntPtr getSignleFrame()
    {
      try
      {
        this.m_Evt.Reset();
        this.m_bGrabFrame = true;
        this.Run();
        if (!this.m_Evt.WaitOne(5000, false))
          Logger.Info("GetSingleFrame Timed out");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting single frame. Err : " + ex.ToString());
        Marshal.FreeCoTaskMem(this.m_Buffer);
        this.m_Buffer = IntPtr.Zero;
      }
      return this.m_Buffer;
    }

    public int Width
    {
      get
      {
        return this.m_Width;
      }
    }

    public int Height
    {
      get
      {
        return this.m_Height;
      }
    }

    public int Stride
    {
      get
      {
        return this.m_Stride;
      }
    }

    private void TearDownCom()
    {
      try
      {
        if (this.m_mediaCtrl != null && this.m_bGraphRunning)
        {
          ErrorHandler errorHandler = (ErrorHandler) this.m_mediaCtrl.Stop();
          this.m_bGraphRunning = false;
        }
        if (this.m_mediaCtrl != null)
        {
          Marshal.ReleaseComObject((object) this.m_mediaCtrl);
          this.m_mediaCtrl = (IMediaControl) null;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Stop Graph. Err : {0}", (object) ex.ToString());
      }
      if (this.m_FilterGraph == null)
        return;
      Marshal.ReleaseComObject((object) this.m_FilterGraph);
      this.m_FilterGraph = (IFilterGraph2) null;
    }
  }
}
