// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ImageBehavior
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using BlueStacks.Common.Decoding;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace BlueStacks.Common
{
  public static class ImageBehavior
  {
    public static readonly DependencyProperty AnimatedSourceProperty = DependencyProperty.RegisterAttached("AnimatedSource", typeof (ImageSource), typeof (ImageBehavior), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(ImageBehavior.AnimatedSourceChanged)));
    public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.RegisterAttached("RepeatBehavior", typeof (RepeatBehavior), typeof (ImageBehavior), (PropertyMetadata) new UIPropertyMetadata((object) new RepeatBehavior(), new PropertyChangedCallback(ImageBehavior.RepeatBehaviorChanged)));
    public static readonly DependencyProperty AnimateInDesignModeProperty = DependencyProperty.RegisterAttached("AnimateInDesignMode", typeof (bool), typeof (ImageBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(ImageBehavior.AnimateInDesignModeChanged)));
    public static readonly DependencyProperty AutoStartProperty = DependencyProperty.RegisterAttached("AutoStart", typeof (bool), typeof (ImageBehavior), new PropertyMetadata((object) true));
    private static readonly DependencyPropertyKey AnimationControllerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AnimationController", typeof (ImageAnimationController), typeof (ImageBehavior), new PropertyMetadata((PropertyChangedCallback) null));
    private static readonly DependencyPropertyKey IsAnimationLoadedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsAnimationLoaded", typeof (bool), typeof (ImageBehavior), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsAnimationLoadedProperty = ImageBehavior.IsAnimationLoadedPropertyKey.DependencyProperty;
    public static readonly RoutedEvent AnimationLoadedEvent = EventManager.RegisterRoutedEvent("AnimationLoaded", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ImageBehavior));
    public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ImageBehavior));

    [AttachedPropertyBrowsableForType(typeof (Image))]
    public static ImageSource GetAnimatedSource(Image obj)
    {
      return (ImageSource) obj?.GetValue(ImageBehavior.AnimatedSourceProperty);
    }

    public static void SetAnimatedSource(Image obj, ImageSource value)
    {
      obj?.SetValue(ImageBehavior.AnimatedSourceProperty, (object) value);
    }

    [AttachedPropertyBrowsableForType(typeof (Image))]
    public static RepeatBehavior GetRepeatBehavior(Image obj)
    {
      return (RepeatBehavior) obj?.GetValue(ImageBehavior.RepeatBehaviorProperty);
    }

    public static void SetRepeatBehavior(Image obj, RepeatBehavior value)
    {
      obj?.SetValue(ImageBehavior.RepeatBehaviorProperty, (object) value);
    }

    public static bool GetAnimateInDesignMode(DependencyObject obj)
    {
      return (bool) obj?.GetValue(ImageBehavior.AnimateInDesignModeProperty);
    }

    public static void SetAnimateInDesignMode(DependencyObject obj, bool value)
    {
      obj?.SetValue(ImageBehavior.AnimateInDesignModeProperty, (object) value);
    }

    [AttachedPropertyBrowsableForType(typeof (Image))]
    public static bool GetAutoStart(Image obj)
    {
      return (bool) obj?.GetValue(ImageBehavior.AutoStartProperty);
    }

    public static void SetAutoStart(Image obj, bool value)
    {
      obj?.SetValue(ImageBehavior.AutoStartProperty, (object) value);
    }

    public static ImageAnimationController GetAnimationController(
      Image imageControl)
    {
      return (ImageAnimationController) imageControl?.GetValue(ImageBehavior.AnimationControllerPropertyKey.DependencyProperty);
    }

    private static void SetAnimationController(DependencyObject obj, ImageAnimationController value)
    {
      obj?.SetValue(ImageBehavior.AnimationControllerPropertyKey, (object) value);
    }

    public static bool GetIsAnimationLoaded(Image image)
    {
      return (bool) image?.GetValue(ImageBehavior.IsAnimationLoadedProperty);
    }

    private static void SetIsAnimationLoaded(Image image, bool value)
    {
      image.SetValue(ImageBehavior.IsAnimationLoadedPropertyKey, (object) value);
    }

    public static void AddAnimationLoadedHandler(Image image, RoutedEventHandler handler)
    {
      if (image == null)
        throw new ArgumentNullException(nameof (image));
      if (handler == null)
        throw new ArgumentNullException(nameof (handler));
      image.AddHandler(ImageBehavior.AnimationLoadedEvent, (Delegate) handler);
    }

    public static void RemoveAnimationLoadedHandler(Image image, RoutedEventHandler handler)
    {
      if (image == null)
        throw new ArgumentNullException(nameof (image));
      if (handler == null)
        throw new ArgumentNullException(nameof (handler));
      image.RemoveHandler(ImageBehavior.AnimationLoadedEvent, (Delegate) handler);
    }

    public static void AddAnimationCompletedHandler(Image d, RoutedEventHandler handler)
    {
      d?.AddHandler(ImageBehavior.AnimationCompletedEvent, (Delegate) handler);
    }

    public static void RemoveAnimationCompletedHandler(Image d, RoutedEventHandler handler)
    {
      d?.RemoveHandler(ImageBehavior.AnimationCompletedEvent, (Delegate) handler);
    }

    private static void AnimatedSourceChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(o is Image imageControl))
        return;
      ImageSource oldValue = e.OldValue as ImageSource;
      ImageSource newValue = e.NewValue as ImageSource;
      if (oldValue == newValue)
        return;
      if (oldValue != null)
      {
        imageControl.Loaded -= new RoutedEventHandler(ImageBehavior.ImageControlLoaded);
        imageControl.Unloaded -= new RoutedEventHandler(ImageBehavior.ImageControlUnloaded);
        AnimationCache.DecrementReferenceCount(oldValue, ImageBehavior.GetRepeatBehavior(imageControl));
        ImageBehavior.GetAnimationController(imageControl)?.Dispose();
        imageControl.Source = (ImageSource) null;
      }
      if (newValue == null)
        return;
      imageControl.Loaded += new RoutedEventHandler(ImageBehavior.ImageControlLoaded);
      imageControl.Unloaded += new RoutedEventHandler(ImageBehavior.ImageControlUnloaded);
      if (!imageControl.IsLoaded)
        return;
      ImageBehavior.InitAnimationOrImage(imageControl);
    }

    private static void ImageControlLoaded(object sender, RoutedEventArgs e)
    {
      if (!(sender is Image imageControl))
        return;
      ImageBehavior.InitAnimationOrImage(imageControl);
    }

    private static void ImageControlUnloaded(object sender, RoutedEventArgs e)
    {
      if (!(sender is Image imageControl))
        return;
      ImageSource animatedSource = ImageBehavior.GetAnimatedSource(imageControl);
      if (animatedSource != null)
        AnimationCache.DecrementReferenceCount(animatedSource, ImageBehavior.GetRepeatBehavior(imageControl));
      ImageBehavior.GetAnimationController(imageControl)?.Dispose();
    }

    private static void RepeatBehaviorChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(o is Image imageControl))
        return;
      ImageSource animatedSource = ImageBehavior.GetAnimatedSource(imageControl);
      if (animatedSource == null)
        return;
      if (!object.Equals(e.OldValue, e.NewValue))
        AnimationCache.DecrementReferenceCount(animatedSource, (RepeatBehavior) e.OldValue);
      if (!imageControl.IsLoaded)
        return;
      ImageBehavior.InitAnimationOrImage(imageControl);
    }

    private static void AnimateInDesignModeChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(o is Image imageControl))
        return;
      bool newValue = (bool) e.NewValue;
      if (ImageBehavior.GetAnimatedSource(imageControl) == null || !imageControl.IsLoaded)
        return;
      if (newValue)
        ImageBehavior.InitAnimationOrImage(imageControl);
      else
        imageControl.BeginAnimation(Image.SourceProperty, (AnimationTimeline) null);
    }

    private static void InitAnimationOrImage(Image imageControl)
    {
      ImageBehavior.SetAnimationController((DependencyObject) imageControl, (ImageAnimationController) null);
      ImageBehavior.SetIsAnimationLoaded(imageControl, false);
      BitmapSource source = ImageBehavior.GetAnimatedSource(imageControl) as BitmapSource;
      bool flag1 = (DesignerProperties.GetIsInDesignMode((DependencyObject) imageControl) ? 1 : 0) == 0 | ImageBehavior.GetAnimateInDesignMode((DependencyObject) imageControl);
      bool flag2 = ImageBehavior.IsLoadingDeferred(source);
      if (source != null & flag1 && !flag2)
      {
        if (source.IsDownloading)
        {
          EventHandler handler = (EventHandler) null;
          handler = (EventHandler) ((sender, args) =>
          {
            source.DownloadCompleted -= handler;
            ImageBehavior.InitAnimationOrImage(imageControl);
          });
          source.DownloadCompleted += handler;
          imageControl.Source = (ImageSource) source;
          return;
        }
        ObjectAnimationUsingKeyFrames animation = ImageBehavior.GetAnimation(imageControl, source);
        if (animation != null)
        {
          if (animation.KeyFrames.Count > 0)
            ImageBehavior.TryTwice((System.Action) (() => imageControl.Source = (ImageSource) animation.KeyFrames[0].Value));
          else
            imageControl.Source = (ImageSource) source;
          ImageAnimationController animationController = new ImageAnimationController(imageControl, animation, ImageBehavior.GetAutoStart(imageControl));
          ImageBehavior.SetAnimationController((DependencyObject) imageControl, animationController);
          ImageBehavior.SetIsAnimationLoaded(imageControl, true);
          imageControl.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationLoadedEvent, (object) imageControl));
          return;
        }
      }
      imageControl.Source = (ImageSource) source;
      if (source == null)
        return;
      ImageBehavior.SetIsAnimationLoaded(imageControl, true);
      imageControl.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationLoadedEvent, (object) imageControl));
    }

    private static ObjectAnimationUsingKeyFrames GetAnimation(
      Image imageControl,
      BitmapSource source)
    {
      ObjectAnimationUsingKeyFrames animation1 = AnimationCache.GetAnimation((ImageSource) source, ImageBehavior.GetRepeatBehavior(imageControl));
      if (animation1 != null)
        return animation1;
      GifFile gifFile;
      if (!(ImageBehavior.GetDecoder(source, out gifFile) is GifBitmapDecoder decoder) || decoder.Frames.Count <= 1)
        return (ObjectAnimationUsingKeyFrames) null;
      ImageBehavior.Int32Size fullSize = ImageBehavior.GetFullSize((BitmapDecoder) decoder, gifFile);
      int frameIndex = 0;
      ObjectAnimationUsingKeyFrames animation2 = new ObjectAnimationUsingKeyFrames();
      TimeSpan zero = TimeSpan.Zero;
      BitmapSource baseFrame = (BitmapSource) null;
      foreach (BitmapFrame frame1 in decoder.Frames)
      {
        ImageBehavior.FrameMetadata frameMetadata = ImageBehavior.GetFrameMetadata((BitmapDecoder) decoder, gifFile, frameIndex);
        BitmapSource frame2 = ImageBehavior.MakeFrame(fullSize, (BitmapSource) frame1, frameMetadata, baseFrame);
        DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame((object) frame2, (KeyTime) zero);
        animation2.KeyFrames.Add((ObjectKeyFrame) discreteObjectKeyFrame);
        zero += frameMetadata.Delay;
        switch (frameMetadata.DisposalMethod)
        {
          case ImageBehavior.FrameDisposalMethod.None:
          case ImageBehavior.FrameDisposalMethod.DoNotDispose:
            baseFrame = frame2;
            break;
          case ImageBehavior.FrameDisposalMethod.RestoreBackground:
            baseFrame = !ImageBehavior.IsFullFrame(frameMetadata, fullSize) ? ImageBehavior.ClearArea(frame2, frameMetadata) : (BitmapSource) null;
            break;
        }
        ++frameIndex;
      }
      animation2.Duration = (Duration) zero;
      animation2.RepeatBehavior = ImageBehavior.GetActualRepeatBehavior(imageControl, (BitmapDecoder) decoder, gifFile);
      AnimationCache.AddAnimation((ImageSource) source, ImageBehavior.GetRepeatBehavior(imageControl), animation2);
      AnimationCache.IncrementReferenceCount((ImageSource) source, ImageBehavior.GetRepeatBehavior(imageControl));
      return animation2;
    }

    private static BitmapSource ClearArea(
      BitmapSource frame,
      ImageBehavior.FrameMetadata metadata)
    {
      DrawingVisual drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
      {
        Rect rect1 = new Rect(0.0, 0.0, (double) frame.PixelWidth, (double) frame.PixelHeight);
        Rect rect2 = new Rect((double) metadata.Left, (double) metadata.Top, (double) metadata.Width, (double) metadata.Height);
        PathGeometry pathGeometry = Geometry.Combine((Geometry) new RectangleGeometry(rect1), (Geometry) new RectangleGeometry(rect2), GeometryCombineMode.Exclude, (Transform) null);
        drawingContext.PushClip((Geometry) pathGeometry);
        drawingContext.DrawImage((ImageSource) frame, rect1);
      }
      RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
      renderTargetBitmap.Render((Visual) drawingVisual);
      if (renderTargetBitmap.CanFreeze && !renderTargetBitmap.IsFrozen)
        renderTargetBitmap.Freeze();
      return (BitmapSource) renderTargetBitmap;
    }

    private static void TryTwice(System.Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        action();
      }
    }

    private static bool IsLoadingDeferred(BitmapSource source)
    {
      return source is BitmapImage bitmapImage && bitmapImage.UriSource != (Uri) null && !bitmapImage.UriSource.IsAbsoluteUri && bitmapImage.BaseUri == (Uri) null;
    }

    private static BitmapDecoder GetDecoder(BitmapSource image, out GifFile gifFile)
    {
      gifFile = (GifFile) null;
      BitmapDecoder decoder = (BitmapDecoder) null;
      Stream stream = (Stream) null;
      Uri result = (Uri) null;
      BitmapCreateOptions createOptions = BitmapCreateOptions.None;
      if (image is BitmapImage bitmapImage)
      {
        createOptions = bitmapImage.CreateOptions;
        if (bitmapImage.StreamSource != null)
          stream = bitmapImage.StreamSource;
        else if (bitmapImage.UriSource != (Uri) null)
        {
          result = bitmapImage.UriSource;
          if (bitmapImage.BaseUri != (Uri) null && !result.IsAbsoluteUri)
            result = new Uri(bitmapImage.BaseUri, result);
        }
      }
      else if (image is BitmapFrame bitmapFrame)
      {
        decoder = bitmapFrame.Decoder;
        Uri.TryCreate(bitmapFrame.BaseUri, bitmapFrame.ToString((IFormatProvider) CultureInfo.InvariantCulture), out result);
      }
      if (decoder == null)
      {
        if (stream != null)
        {
          stream.Position = 0L;
          decoder = BitmapDecoder.Create(stream, createOptions, BitmapCacheOption.OnLoad);
        }
        else if (result != (Uri) null && result.IsAbsoluteUri)
          decoder = BitmapDecoder.Create(result, createOptions, BitmapCacheOption.OnLoad);
      }
      if (decoder is GifBitmapDecoder && !ImageBehavior.CanReadNativeMetadata(decoder))
      {
        if (stream != null)
        {
          stream.Position = 0L;
          gifFile = GifFile.ReadGifFile(stream, true);
        }
        else
        {
          if (!(result != (Uri) null))
            throw new InvalidOperationException("Can't get URI or Stream from the source. AnimatedSource should be either a BitmapImage, or a BitmapFrame constructed from a URI.");
          gifFile = ImageBehavior.DecodeGifFile(result);
        }
      }
      if (decoder == null)
        throw new InvalidOperationException("Can't get a decoder from the source. AnimatedSource should be either a BitmapImage or a BitmapFrame.");
      return decoder;
    }

    private static bool CanReadNativeMetadata(BitmapDecoder decoder)
    {
      try
      {
        return decoder.Metadata != null;
      }
      catch
      {
        return false;
      }
    }

    private static GifFile DecodeGifFile(Uri uri)
    {
      Stream stream = (Stream) null;
      if (uri.Scheme == PackUriHelper.UriSchemePack)
      {
        StreamResourceInfo streamResourceInfo = !(uri.Authority == "siteoforigin:,,,") ? Application.GetResourceStream(uri) : Application.GetRemoteStream(uri);
        if (streamResourceInfo != null)
          stream = streamResourceInfo.Stream;
      }
      else
      {
        using (WebClient webClient = new WebClient())
          stream = webClient.OpenRead(uri);
      }
      if (stream == null)
        return (GifFile) null;
      using (stream)
        return GifFile.ReadGifFile(stream, true);
    }

    private static bool IsFullFrame(
      ImageBehavior.FrameMetadata metadata,
      ImageBehavior.Int32Size fullSize)
    {
      return metadata.Left == 0 && metadata.Top == 0 && metadata.Width == fullSize.Width && metadata.Height == fullSize.Height;
    }

    private static BitmapSource MakeFrame(
      ImageBehavior.Int32Size fullSize,
      BitmapSource rawFrame,
      ImageBehavior.FrameMetadata metadata,
      BitmapSource baseFrame)
    {
      if (baseFrame == null && ImageBehavior.IsFullFrame(metadata, fullSize))
        return rawFrame;
      DrawingVisual drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
      {
        if (baseFrame != null)
        {
          Rect rectangle = new Rect(0.0, 0.0, (double) fullSize.Width, (double) fullSize.Height);
          drawingContext.DrawImage((ImageSource) baseFrame, rectangle);
        }
        Rect rectangle1 = new Rect((double) metadata.Left, (double) metadata.Top, (double) metadata.Width, (double) metadata.Height);
        drawingContext.DrawImage((ImageSource) rawFrame, rectangle1);
      }
      RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(fullSize.Width, fullSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
      renderTargetBitmap.Render((Visual) drawingVisual);
      if (renderTargetBitmap.CanFreeze && !renderTargetBitmap.IsFrozen)
        renderTargetBitmap.Freeze();
      return (BitmapSource) renderTargetBitmap;
    }

    private static RepeatBehavior GetActualRepeatBehavior(
      Image imageControl,
      BitmapDecoder decoder,
      GifFile gifMetadata)
    {
      RepeatBehavior repeatBehavior = ImageBehavior.GetRepeatBehavior(imageControl);
      if (repeatBehavior != new RepeatBehavior())
        return repeatBehavior;
      int num = gifMetadata == null ? ImageBehavior.GetRepeatCount(decoder) : (int) gifMetadata.RepeatCount;
      return num == 0 ? RepeatBehavior.Forever : new RepeatBehavior((double) num);
    }

    private static int GetRepeatCount(BitmapDecoder decoder)
    {
      BitmapMetadata applicationExtension = ImageBehavior.GetApplicationExtension(decoder, "NETSCAPE2.0");
      if (applicationExtension != null)
      {
        byte[] queryOrNull = applicationExtension.GetQueryOrNull<byte[]>("/Data");
        if (queryOrNull != null && queryOrNull.Length >= 4)
          return (int) BitConverter.ToUInt16(queryOrNull, 2);
      }
      return 1;
    }

    private static BitmapMetadata GetApplicationExtension(
      BitmapDecoder decoder,
      string application)
    {
      int num = 0;
      string query1 = "/appext";
      string query2;
      for (BitmapMetadata queryOrNull1 = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query1); queryOrNull1 != null; queryOrNull1 = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query2))
      {
        byte[] queryOrNull2 = queryOrNull1.GetQueryOrNull<byte[]>("/Application");
        if (queryOrNull2 != null && Encoding.ASCII.GetString(queryOrNull2) == application)
          return queryOrNull1;
        query2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/[{0}]appext", (object) ++num);
      }
      return (BitmapMetadata) null;
    }

    private static ImageBehavior.FrameMetadata GetFrameMetadata(
      BitmapDecoder decoder,
      GifFile gifMetadata,
      int frameIndex)
    {
      return gifMetadata != null && gifMetadata.Frames.Count > frameIndex ? ImageBehavior.GetFrameMetadata(gifMetadata.Frames[frameIndex]) : ImageBehavior.GetFrameMetadata(decoder.Frames[frameIndex]);
    }

    private static ImageBehavior.FrameMetadata GetFrameMetadata(BitmapFrame frame)
    {
      BitmapMetadata metadata = (BitmapMetadata) frame.Metadata;
      TimeSpan timeSpan = TimeSpan.FromMilliseconds(100.0);
      int queryOrDefault1 = metadata.GetQueryOrDefault<int>("/grctlext/Delay", 10);
      if (queryOrDefault1 != 0)
        timeSpan = TimeSpan.FromMilliseconds((double) (queryOrDefault1 * 10));
      ImageBehavior.FrameDisposalMethod queryOrDefault2 = (ImageBehavior.FrameDisposalMethod) metadata.GetQueryOrDefault<int>("/grctlext/Disposal", 0);
      return new ImageBehavior.FrameMetadata()
      {
        Left = metadata.GetQueryOrDefault<int>("/imgdesc/Left", 0),
        Top = metadata.GetQueryOrDefault<int>("/imgdesc/Top", 0),
        Width = metadata.GetQueryOrDefault<int>("/imgdesc/Width", frame.PixelWidth),
        Height = metadata.GetQueryOrDefault<int>("/imgdesc/Height", frame.PixelHeight),
        Delay = timeSpan,
        DisposalMethod = queryOrDefault2
      };
    }

    private static ImageBehavior.FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
    {
      GifImageDescriptor descriptor = gifMetadata.Descriptor;
      ImageBehavior.FrameMetadata frameMetadata = new ImageBehavior.FrameMetadata()
      {
        Left = descriptor.Left,
        Top = descriptor.Top,
        Width = descriptor.Width,
        Height = descriptor.Height,
        Delay = TimeSpan.FromMilliseconds(100.0),
        DisposalMethod = ImageBehavior.FrameDisposalMethod.None
      };
      GifGraphicControlExtension controlExtension = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault<GifGraphicControlExtension>();
      if (controlExtension != null)
      {
        if (controlExtension.Delay != 0)
          frameMetadata.Delay = TimeSpan.FromMilliseconds((double) controlExtension.Delay);
        frameMetadata.DisposalMethod = (ImageBehavior.FrameDisposalMethod) controlExtension.DisposalMethod;
      }
      return frameMetadata;
    }

    private static ImageBehavior.Int32Size GetFullSize(
      BitmapDecoder decoder,
      GifFile gifMetadata)
    {
      if (gifMetadata == null)
        return new ImageBehavior.Int32Size(decoder.Metadata.GetQueryOrDefault<int>("/logscrdesc/Width", 0), decoder.Metadata.GetQueryOrDefault<int>("/logscrdesc/Height", 0));
      GifLogicalScreenDescriptor screenDescriptor = gifMetadata.Header.LogicalScreenDescriptor;
      return new ImageBehavior.Int32Size(screenDescriptor.Width, screenDescriptor.Height);
    }

    private static T GetQueryOrDefault<T>(
      this BitmapMetadata metadata,
      string query,
      T defaultValue)
    {
      return metadata.ContainsQuery(query) ? (T) Convert.ChangeType(metadata.GetQuery(query), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture) : defaultValue;
    }

    private static T GetQueryOrNull<T>(this BitmapMetadata metadata, string query) where T : class
    {
      return metadata.ContainsQuery(query) ? metadata.GetQuery(query) as T : default (T);
    }

    private struct Int32Size
    {
      public Int32Size(int width, int height)
        : this()
      {
        this.Width = width;
        this.Height = height;
      }

      public int Width { [IsReadOnly] get; private set; }

      public int Height { [IsReadOnly] get; private set; }
    }

    private class FrameMetadata
    {
      public int Left { get; set; }

      public int Top { get; set; }

      public int Width { get; set; }

      public int Height { get; set; }

      public TimeSpan Delay { get; set; }

      public ImageBehavior.FrameDisposalMethod DisposalMethod { get; set; }
    }

    private enum FrameDisposalMethod
    {
      None,
      DoNotDispose,
      RestoreBackground,
      RestorePrevious,
    }
  }
}
