// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ImageAnimationController
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.Common
{
  public class ImageAnimationController : IDisposable
  {
    private static readonly DependencyPropertyDescriptor _sourceDescriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof (Image));
    private readonly Image _image;
    private readonly ObjectAnimationUsingKeyFrames _animation;
    private readonly AnimationClock _clock;
    private readonly ClockController _clockController;

    internal ImageAnimationController(
      Image image,
      ObjectAnimationUsingKeyFrames animation,
      bool autoStart)
    {
      this._image = image;
      this._animation = animation;
      this._animation.Completed += new EventHandler(this.AnimationCompleted);
      this._clock = this._animation.CreateClock();
      this._clockController = this._clock.Controller;
      ImageAnimationController._sourceDescriptor.AddValueChanged((object) image, new EventHandler(this.ImageSourceChanged));
      this._clockController.Pause();
      this._image.ApplyAnimationClock(Image.SourceProperty, this._clock);
      if (!autoStart)
        return;
      this._clockController.Resume();
    }

    private void AnimationCompleted(object sender, EventArgs e)
    {
      this._image.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, (object) this._image));
    }

    private void ImageSourceChanged(object sender, EventArgs e)
    {
      this.OnCurrentFrameChanged();
    }

    public int FrameCount
    {
      get
      {
        return this._animation.KeyFrames.Count;
      }
    }

    public bool IsPaused
    {
      get
      {
        return this._clock.IsPaused;
      }
    }

    public bool IsComplete
    {
      get
      {
        return this._clock.CurrentState == ClockState.Filling;
      }
    }

    public void GotoFrame(int index)
    {
      this._clockController.Seek(this._animation.KeyFrames[index].KeyTime.TimeSpan, TimeSeekOrigin.BeginTime);
    }

    public int CurrentFrame
    {
      get
      {
        TimeSpan? time = this._clock.CurrentTime;
        var data = this._animation.KeyFrames.Cast<ObjectKeyFrame>().Select((f, i) => new
        {
          Time = f.KeyTime.TimeSpan,
          Index = i
        }).FirstOrDefault(fi =>
        {
          TimeSpan time1 = fi.Time;
          TimeSpan? nullable = time;
          return nullable.HasValue && time1 >= nullable.GetValueOrDefault();
        });
        return data != null ? data.Index : -1;
      }
    }

    public void Pause()
    {
      this._clockController.Pause();
    }

    public void Play()
    {
      this._clockController.Resume();
    }

    public event EventHandler CurrentFrameChanged;

    private void OnCurrentFrameChanged()
    {
      EventHandler currentFrameChanged = this.CurrentFrameChanged;
      if (currentFrameChanged == null)
        return;
      currentFrameChanged((object) this, EventArgs.Empty);
    }

    ~ImageAnimationController()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._image.BeginAnimation(Image.SourceProperty, (AnimationTimeline) null);
      this._animation.Completed -= new EventHandler(this.AnimationCompleted);
      ImageAnimationController._sourceDescriptor.RemoveValueChanged((object) this._image, new EventHandler(this.ImageSourceChanged));
      this._image.Source = (ImageSource) null;
    }
  }
}
