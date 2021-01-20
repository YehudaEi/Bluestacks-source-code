// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomPictureBox
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
  public class CustomPictureBox : Image
  {
    public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register(nameof (ImageName), typeof (string), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) "", new PropertyChangedCallback(CustomPictureBox.ImageNameChanged)));
    public static readonly DependencyProperty IsImageHoverProperty = DependencyProperty.Register(nameof (IsImageHover), typeof (bool), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CustomPictureBox.IsImageHoverChanged)));
    public static readonly DependencyProperty IsAlwaysHalfSizeProperty = DependencyProperty.Register(nameof (IsAlwaysHalfSize), typeof (bool), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CustomPictureBox.IsAlwaysHalfSizeChanged)));
    public static readonly Dictionary<string, Tuple<BitmapImage, bool>> sImageAssetsDict = new Dictionary<string, Tuple<BitmapImage, bool>>();
    public static readonly DependencyProperty AllowClickThroughProperty = DependencyProperty.Register(nameof (AllowClickThrough), typeof (bool), typeof (CustomPictureBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private Point maxSize;
    internal BitmapImage BitmapImage;
    internal DoubleAnimation animation;
    private Storyboard mStoryBoard;
    private bool mIsImageToBeRotated;

    public CustomPictureBox.State ButtonState { get; set; }

    public bool IsFullImagePath { get; set; }

    public string ImageName
    {
      get
      {
        return (string) this.GetValue(CustomPictureBox.ImageNameProperty);
      }
      set
      {
        this.SetValue(CustomPictureBox.ImageNameProperty, (object) value);
      }
    }

    public bool IsImageHover
    {
      get
      {
        return (bool) this.GetValue(CustomPictureBox.IsImageHoverProperty);
      }
      set
      {
        this.SetValue(CustomPictureBox.IsImageHoverProperty, (object) value);
      }
    }

    public bool IsAlwaysHalfSize
    {
      get
      {
        return (bool) this.GetValue(CustomPictureBox.IsAlwaysHalfSizeProperty);
      }
      set
      {
        this.SetValue(CustomPictureBox.IsAlwaysHalfSizeProperty, (object) value);
      }
    }

    public static event EventHandler SourceUpdatedEvent;

    public bool IsImageToBeRotated
    {
      get
      {
        return this.mIsImageToBeRotated;
      }
      set
      {
        this.mIsImageToBeRotated = value;
        if (value)
        {
          this.SizeChanged -= new SizeChangedEventHandler(this.CustomPictureBox_SizeChanged);
          this.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(this.CustomPictureBox_IsVisibleChanged);
          this.SizeChanged += new SizeChangedEventHandler(this.CustomPictureBox_SizeChanged);
          this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.CustomPictureBox_IsVisibleChanged);
        }
        else
        {
          if (this.mStoryBoard != null)
            this.mStoryBoard.Stop();
          this.SizeChanged -= new SizeChangedEventHandler(this.CustomPictureBox_SizeChanged);
          this.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(this.CustomPictureBox_IsVisibleChanged);
        }
      }
    }

    public void SetDisabledState()
    {
      this.ButtonState = CustomPictureBox.State.disabled;
      this.Opacity = 0.4;
      this.SetDefaultImage();
    }

    public void SetNormalState()
    {
      this.ButtonState = CustomPictureBox.State.normal;
      this.Opacity = 1.0;
    }

    private string AppendStringToImageName(string appendText)
    {
      if (!this.ImageName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) && !this.ImageName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) && (!this.ImageName.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) && !this.ImageName.EndsWith(".ico", StringComparison.InvariantCultureIgnoreCase)))
        return this.ImageName + appendText;
      string extension = Path.GetExtension(this.ImageName);
      return Path.GetDirectoryName(this.ImageName) + Path.DirectorySeparatorChar.ToString() + Path.GetFileNameWithoutExtension(this.ImageName) + appendText + extension;
    }

    private string HoverImage
    {
      get
      {
        return this.AppendStringToImageName("_hover");
      }
    }

    private string ClickImage
    {
      get
      {
        return this.AppendStringToImageName("_click");
      }
    }

    private string DisabledImage
    {
      get
      {
        return this.AppendStringToImageName("_dis");
      }
    }

    public string SelectedImage
    {
      get
      {
        return this.AppendStringToImageName("_selected");
      }
    }

    public static string AssetsDir
    {
      get
      {
        return Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.ClientThemeName);
      }
    }

    public CustomPictureBox()
    {
      this.MouseEnter += new MouseEventHandler(this.PictureBox_MouseEnter);
      this.MouseLeave += new MouseEventHandler(this.PictureBox_MouseLeave);
      this.MouseDown += new MouseButtonEventHandler(this.PictureBox_MouseDown);
      this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.PictureBox_IsEnabledChanged);
      this.MouseUp += new MouseButtonEventHandler(this.PictureBox_MouseUp);
      RenderOptions.SetBitmapScalingMode((DependencyObject) this, BitmapScalingMode.HighQuality);
    }

    public static void UpdateImagesFromNewDirectory(string path = "")
    {
      foreach (Tuple<string, bool> tuple in CustomPictureBox.sImageAssetsDict.Select<KeyValuePair<string, Tuple<BitmapImage, bool>>, Tuple<string, bool>>((Func<KeyValuePair<string, Tuple<BitmapImage, bool>>, Tuple<string, bool>>) (_ => new Tuple<string, bool>(_.Key, _.Value.Item2))).ToList<Tuple<string, bool>>())
      {
        if (tuple.Item1.IndexOfAny(new char[2]
        {
          Path.AltDirectorySeparatorChar,
          Path.DirectorySeparatorChar
        }) == -1)
        {
          CustomPictureBox.sImageAssetsDict.Remove(tuple.Item1);
          CustomPictureBox.GetBitmapImage(tuple.Item1, path, tuple.Item2);
        }
      }
      CustomPictureBox.NotifyUIElements();
    }

    internal static void NotifyUIElements()
    {
      if (CustomPictureBox.SourceUpdatedEvent == null)
        return;
      CustomPictureBox.SourceUpdatedEvent((object) null, (EventArgs) null);
    }

    private static void ImageNameChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      CustomPictureBox customPictureBox = source as CustomPictureBox;
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) customPictureBox))
        return;
      customPictureBox.SetDefaultImage();
    }

    private static void IsImageHoverChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      CustomPictureBox customPictureBox = source as CustomPictureBox;
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) customPictureBox))
        return;
      if (customPictureBox.IsImageHover)
        customPictureBox.SetHoverImage();
      else
        customPictureBox.SetDefaultImage();
    }

    private static void IsAlwaysHalfSizeChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      CustomPictureBox customPictureBox = source as CustomPictureBox;
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) customPictureBox))
        return;
      customPictureBox.SetDefaultImage();
    }

    private void PictureBox_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.ButtonState != CustomPictureBox.State.normal || this.IsFullImagePath)
        return;
      this.SetHoverImage();
    }

    private void PictureBox_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.IsFullImagePath)
        return;
      this.SetDefaultImage();
    }

    private void PictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.ButtonState != CustomPictureBox.State.normal || this.IsFullImagePath)
        return;
      this.SetClickedImage();
    }

    private void PictureBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (this.IsFullImagePath)
        return;
      if (this.IsMouseOver && this.ButtonState == CustomPictureBox.State.normal)
        this.SetHoverImage();
      else
        this.SetDefaultImage();
    }

    public void SetHoverImage()
    {
      try
      {
        if (this.IsFullImagePath)
          return;
        CustomPictureBox.SetBitmapImage((Image) this, this.HoverImage, false);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetClickedImage()
    {
      try
      {
        if (this.IsFullImagePath)
          return;
        CustomPictureBox.SetBitmapImage((Image) this, this.ClickImage, false);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetSelectedImage()
    {
      try
      {
        if (this.IsFullImagePath)
          return;
        CustomPictureBox.SetBitmapImage((Image) this, this.SelectedImage, false);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetDisabledImage()
    {
      try
      {
        if (this.IsFullImagePath)
          return;
        CustomPictureBox.SetBitmapImage((Image) this, this.DisabledImage, false);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetDefaultImage()
    {
      try
      {
        CustomPictureBox.SetBitmapImage((Image) this, this.ImageName, this.IsFullImagePath);
      }
      catch
      {
      }
    }

    public static BitmapImage GetBitmapImage(
      string fileName,
      string assetDirectory = "",
      bool isFullImagePath = false)
    {
      if (string.IsNullOrEmpty(fileName))
        return (BitmapImage) null;
      if (CustomPictureBox.sImageAssetsDict.ContainsKey(fileName))
        return CustomPictureBox.sImageAssetsDict[fileName].Item1;
      BitmapImage bitmapImage = (BitmapImage) null;
      if (fileName.IndexOfAny(new char[2]
      {
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
      }) != -1)
      {
        if (!isFullImagePath)
          Logger.Warning("Full image path not marked false for image: " + fileName);
        bitmapImage = CustomPictureBox.BitmapFromPath(fileName);
      }
      else if (isFullImagePath)
        Logger.Warning("Full image path marked true for image: " + fileName);
      if (bitmapImage == null)
      {
        if (string.IsNullOrEmpty(assetDirectory))
          assetDirectory = CustomPictureBox.AssetsDir;
        bitmapImage = CustomPictureBox.BitmapFromPath(Path.Combine(assetDirectory, Path.GetFileNameWithoutExtension(fileName) + ".png")) ?? CustomPictureBox.BitmapFromPath(Path.Combine(assetDirectory, fileName)) ?? CustomPictureBox.BitmapFromPath(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets"), Path.GetFileNameWithoutExtension(fileName) + ".png")) ?? CustomPictureBox.BitmapFromPath(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), Path.GetFileNameWithoutExtension(fileName) + ".png"));
      }
      CustomPictureBox.sImageAssetsDict.Add(fileName, new Tuple<BitmapImage, bool>(bitmapImage, isFullImagePath));
      if (bitmapImage == null)
        Logger.Warning("Returning a null image for {0}", (object) fileName);
      return bitmapImage;
    }

    private static BitmapImage BitmapFromPath(string path)
    {
      BitmapImage bitmapImage = (BitmapImage) null;
      if (File.Exists(path))
      {
        bitmapImage = new BitmapImage();
        FileStream fileStream = File.OpenRead(path);
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = (Stream) fileStream;
        bitmapImage.EndInit();
        fileStream.Close();
        fileStream.Dispose();
      }
      return bitmapImage;
    }

    public static void SetBitmapImage(Image image, string fileName, bool isFullImagePath = false)
    {
      BitmapImage bitmapImage = CustomPictureBox.GetBitmapImage(fileName, "", isFullImagePath);
      if (bitmapImage != null)
      {
        bitmapImage.Freeze();
        BlueStacksUIBinding.Bind(image, Image.SourceProperty, fileName);
        if (image is CustomPictureBox)
        {
          CustomPictureBox customPictureBox = image as CustomPictureBox;
          customPictureBox.BitmapImage = bitmapImage;
          if (customPictureBox.IsAlwaysHalfSize)
          {
            customPictureBox.maxSize = new Point(customPictureBox.MaxWidth, customPictureBox.MaxHeight);
            customPictureBox.MaxWidth = bitmapImage.Width / 2.0;
            customPictureBox.MaxHeight = bitmapImage.Height / 2.0;
          }
          else if (customPictureBox.maxSize != new Point())
          {
            customPictureBox.MaxWidth = customPictureBox.maxSize.X;
            customPictureBox.MaxHeight = customPictureBox.maxSize.Y;
          }
        }
      }
    }

    private void CustomPictureBox_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.RenderTransform == null || !(this.RenderTransform is RotateTransform renderTransform))
        return;
      renderTransform.CenterX = this.ActualWidth / 2.0;
      renderTransform.CenterY = this.ActualHeight / 2.0;
    }

    private void CustomPictureBox_IsVisibleChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (this.IsVisible && this.mIsImageToBeRotated)
      {
        if (this.mStoryBoard == null)
        {
          this.mStoryBoard = new Storyboard();
          DoubleAnimation doubleAnimation = new DoubleAnimation();
          doubleAnimation.From = new double?(0.0);
          doubleAnimation.To = new double?(360.0);
          doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
          doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));
          this.animation = doubleAnimation;
          this.RenderTransform = (Transform) new RotateTransform()
          {
            CenterX = (this.ActualWidth / 2.0),
            CenterY = (this.ActualHeight / 2.0)
          };
          Storyboard.SetTarget((DependencyObject) this.animation, (DependencyObject) this);
          Storyboard.SetTargetProperty((DependencyObject) this.animation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)", new object[0]));
          this.mStoryBoard.Children.Add((Timeline) this.animation);
        }
        this.mStoryBoard.Begin();
      }
      else
        this.mStoryBoard?.Pause();
    }

    public bool IsDisabled
    {
      set
      {
        if (!value)
          return;
        this.MouseEnter -= new MouseEventHandler(this.PictureBox_MouseEnter);
        this.MouseLeave -= new MouseEventHandler(this.PictureBox_MouseLeave);
        this.MouseDown -= new MouseButtonEventHandler(this.PictureBox_MouseDown);
        this.MouseUp -= new MouseButtonEventHandler(this.PictureBox_MouseUp);
        this.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(this.PictureBox_IsEnabledChanged);
        this.Opacity = 0.5;
      }
    }

    private void PictureBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is CustomPictureBox customPictureBox) || !(e.NewValue is bool newValue))
        return;
      if (newValue)
        customPictureBox.SetDefaultImage();
      else
        customPictureBox.SetDisabledImage();
    }

    public void ReloadImages()
    {
      CustomPictureBox.sImageAssetsDict.Remove(this.ClickImage);
      CustomPictureBox.sImageAssetsDict.Remove(this.ImageName);
      CustomPictureBox.sImageAssetsDict.Remove(this.HoverImage);
      CustomPictureBox.sImageAssetsDict.Remove(this.SelectedImage);
      CustomPictureBox.sImageAssetsDict.Remove(this.DisabledImage);
      this.SetDefaultImage();
      CustomPictureBox.GCCollectAsync();
    }

    private static void GCCollectAsync()
    {
      new Thread((ThreadStart) (() => GC.Collect()))
      {
        IsBackground = true
      }.Start();
    }

    public bool AllowClickThrough
    {
      get
      {
        return (bool) this.GetValue(CustomPictureBox.AllowClickThroughProperty);
      }
      set
      {
        this.SetValue(CustomPictureBox.AllowClickThroughProperty, (object) value);
      }
    }

    protected override HitTestResult HitTestCore(
      PointHitTestParameters hitTestParameters)
    {
      try
      {
        if (hitTestParameters != null)
        {
          if (this.AllowClickThrough)
          {
            Point position = Mouse.GetPosition((IInputElement) this);
            int pixelWidth = ((BitmapSource) this.Source).PixelWidth;
            int pixelHeight = ((BitmapSource) this.Source).PixelHeight;
            double num1 = position.X * (double) pixelWidth / this.ActualWidth;
            double num2 = position.Y * (double) pixelHeight / this.ActualHeight;
            byte[] numArray = new byte[4];
            try
            {
              new CroppedBitmap((BitmapSource) this.Source, new Int32Rect((int) num1, (int) num2, 1, 1)).CopyPixels((Array) numArray, 4, 0);
              if ((int) numArray[3] < RegistryManager.Instance.AdvancedControlTransparencyLevel)
              {
                Logger.Info(string.Format("HitTestCore pixel density at Image location- (X:{0} Y:{1}) is (R:{2} B:{3} G{4} A{5})", (object) num1, (object) num2, (object) numArray[0], (object) numArray[1], (object) numArray[2], (object) numArray[3]));
                return (HitTestResult) null;
              }
            }
            catch (Exception ex)
            {
              Logger.Info(string.Format("Unable to get HitTestCore pixel density at Image location- X:{0} Y:{1}", (object) num1, (object) num2));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("HitTestCore: " + ex.Message);
      }
      return base.HitTestCore(hitTestParameters);
    }

    public enum State
    {
      normal,
      disabled,
    }
  }
}
