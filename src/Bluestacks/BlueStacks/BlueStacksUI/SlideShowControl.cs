// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SlideShowControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class SlideShowControl : System.Windows.Controls.UserControl, IDisposable, IComponentConnector
  {
    private static string[] ValidImageExtensions = new string[5]
    {
      ".png",
      ".jpg",
      ".jpeg",
      ".bmp",
      ".gif"
    };
    public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register(nameof (TransitionType), typeof (SlideShowControl.SlideAnimationType), typeof (SlideShowControl), new PropertyMetadata((object) SlideShowControl.SlideAnimationType.Fade));
    public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register(nameof (TextVerticalAlignment), typeof (VerticalAlignment), typeof (SlideShowControl), new PropertyMetadata((object) VerticalAlignment.Bottom));
    public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register(nameof (TextHorizontalAlignment), typeof (System.Windows.HorizontalAlignment), typeof (SlideShowControl), new PropertyMetadata((object) System.Windows.HorizontalAlignment.Center));
    public static readonly DependencyProperty IsAutoPlayProperty = DependencyProperty.Register(nameof (IsAutoPlay), typeof (bool), typeof (SlideShowControl), new PropertyMetadata((object) false));
    public static readonly DependencyProperty HideArrowOnLeaveProperty = DependencyProperty.Register(nameof (HideArrowOnLeave), typeof (bool), typeof (SlideShowControl), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsArrowVisibleProperty = DependencyProperty.Register(nameof (IsArrowVisible), typeof (bool), typeof (SlideShowControl), new PropertyMetadata((object) true));
    public static readonly DependencyProperty SlideDelayProperty = DependencyProperty.Register(nameof (SlideDelay), typeof (int), typeof (SlideShowControl), new PropertyMetadata((object) 5));
    public static readonly DependencyProperty ImagesFolderPathProperty = DependencyProperty.Register(nameof (ImagesFolderPath), typeof (string), typeof (SlideShowControl), new PropertyMetadata((object) ""));
    private SortedDictionary<int, SlideShowControl.SlideShowContext> mSlideShowDict = new SortedDictionary<int, SlideShowControl.SlideShowContext>();
    private int _slide;
    private Timer timer;
    private bool disposedValue;
    internal SlideShowControl slideControl;
    internal Grid SlideshowGrid;
    internal CustomPictureBox image1;
    internal CustomPictureBox mPrevBtn;
    internal CustomPictureBox mNextBtn;
    internal TextBlock SlideshowName;
    private bool _contentLoaded;

    public SlideShowControl()
    {
      this.InitializeComponent();
      this.image1_MouseEnter((object) null, (System.Windows.Input.MouseEventArgs) null);
    }

    internal void AddOrUpdateSlide(SlideShowControl.SlideShowContext slideContext)
    {
      if (this.mSlideShowDict.ContainsKey(slideContext.Key))
        this.mSlideShowDict[slideContext.Key] = slideContext;
      else
        this.mSlideShowDict.Add(slideContext.Key == 0 ? this.mSlideShowDict.Count : slideContext.Key, slideContext);
    }

    public System.Windows.HorizontalAlignment TextHorizontalAlignment
    {
      get
      {
        return (System.Windows.HorizontalAlignment) this.GetValue(SlideShowControl.TextHorizontalAlignmentProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.TextHorizontalAlignmentProperty, (object) value);
      }
    }

    public VerticalAlignment TextVerticalAlignment
    {
      get
      {
        return (VerticalAlignment) this.GetValue(SlideShowControl.TextVerticalAlignmentProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.TextVerticalAlignmentProperty, (object) value);
      }
    }

    public string ImagesFolderPath
    {
      get
      {
        return (string) this.GetValue(SlideShowControl.ImagesFolderPathProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.ImagesFolderPathProperty, (object) value);
      }
    }

    public bool IsArrowVisible
    {
      get
      {
        return (bool) this.GetValue(SlideShowControl.IsArrowVisibleProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.IsArrowVisibleProperty, (object) value);
      }
    }

    public bool HideArrowOnLeave
    {
      get
      {
        return (bool) this.GetValue(SlideShowControl.HideArrowOnLeaveProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.HideArrowOnLeaveProperty, (object) value);
      }
    }

    public bool IsAutoPlay
    {
      get
      {
        return (bool) this.GetValue(SlideShowControl.IsAutoPlayProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.IsAutoPlayProperty, (object) value);
      }
    }

    public int SlideDelay
    {
      get
      {
        return (int) this.GetValue(SlideShowControl.SlideDelayProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.SlideDelayProperty, (object) value);
      }
    }

    public SlideShowControl.SlideAnimationType TransitionType
    {
      get
      {
        return (SlideShowControl.SlideAnimationType) this.GetValue(SlideShowControl.TransitionTypeProperty);
      }
      set
      {
        this.SetValue(SlideShowControl.TransitionTypeProperty, (object) value);
      }
    }

    internal void PlaySlideShow()
    {
      this.IsAutoPlay = true;
      this.SlideShowLoop(false);
      this.StartImageTransition(this._slide + 1);
    }

    internal void StopSlideShow()
    {
      this.IsAutoPlay = false;
    }

    internal void LoadImagesFromFolder(string folderPath)
    {
      if (!Path.IsPathRooted(folderPath))
        folderPath = Path.Combine(CustomPictureBox.AssetsDir, folderPath);
      if (!Directory.Exists(folderPath))
        return;
      try
      {
        string path = Path.Combine(folderPath, "slides.json");
        if (File.Exists(path))
        {
          IEnumerable<SlideShowControl.SlideShowContext> slideShowContexts = JObject.Parse(File.ReadAllText(path)).ToObject<IEnumerable<SlideShowControl.SlideShowContext>>();
          if (slideShowContexts != null)
          {
            foreach (SlideShowControl.SlideShowContext slideContext in slideShowContexts)
            {
              if (!string.IsNullOrEmpty(slideContext.Description))
                slideContext.Description = LocaleStrings.GetLocalizedString(slideContext.Description, "");
              this.AddOrUpdateSlide(slideContext);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error while trying to read slides.json from " + folderPath + "." + ex.ToString());
        this.mSlideShowDict.Clear();
      }
      if (this.mSlideShowDict.Count == 0)
      {
        FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();
        int num = 0;
        for (int index = 0; index < files.Length; ++index)
        {
          if (((IEnumerable<string>) SlideShowControl.ValidImageExtensions).Contains<string>(files[index].Extension, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          {
            this.AddOrUpdateSlide(new SlideShowControl.SlideShowContext()
            {
              Key = num,
              ImageName = files[index].FullName
            });
            ++num;
          }
        }
      }
      this.StartImageTransition(0);
    }

    private void SlideShowLoop(bool forceStart = false)
    {
      if (forceStart && this.timer != null)
        this.timer.Enabled = false;
      if (this.timer != null && !this.timer.Enabled)
        this.timer.Dispose();
      if (!this.IsAutoPlay || this.mSlideShowDict.Count <= 1)
        return;
      this.timer = new Timer()
      {
        Interval = this.SlideDelay * 1000
      };
      this.timer.Tick += new EventHandler(this.Timer_Tick);
      this.timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      if (this.timer.Enabled && this.IsAutoPlay && (this.mSlideShowDict.Count > 1 && sender == this.timer))
        this.StartImageTransition(this._slide + 1);
      else
        ((Timer) sender).Enabled = false;
    }

    private void StartImageTransition(int i)
    {
      if (this.mSlideShowDict.Count <= 0)
        return;
      if (this._slide == i)
      {
        this.image1.ImageName = this.mSlideShowDict[this._slide].ImageName;
        this.SlideshowName.Text = this.mSlideShowDict[this._slide].Description;
        this.SlideShowLoop(false);
      }
      else if (i >= this.mSlideShowDict.Count)
        this.UnloadImage(0);
      else if (i < 0)
        this.UnloadImage(this.mSlideShowDict.Count - 1);
      else
        this.UnloadImage(i);
    }

    private void UnloadImage(int imageToShow)
    {
      Storyboard storyboard = (this.Resources[(object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}Out", (object) this.TransitionType.ToString())] as Storyboard).Clone();
      storyboard.Completed += (EventHandler) ((o, e) =>
      {
        this.image1.ImageName = this.mSlideShowDict[imageToShow].ImageName;
        this.LoadImage(imageToShow);
      });
      Storyboard.SetTarget((DependencyObject) storyboard, (DependencyObject) this.SlideshowGrid);
      storyboard.Begin();
    }

    private void LoadImage(int imageToShow)
    {
      this._slide = imageToShow;
      this.SlideshowName.Text = this.mSlideShowDict[imageToShow].Description;
      Storyboard resource = this.Resources[(object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}In", (object) this.TransitionType.ToString())] as Storyboard;
      Storyboard.SetTarget((DependencyObject) resource, (DependencyObject) this.SlideshowGrid);
      resource.Begin();
    }

    private void mPrevBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.SlideShowLoop(true);
      this.StartImageTransition(this._slide - 1);
    }

    private void mNextBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.SlideShowLoop(true);
      this.StartImageTransition(this._slide + 1);
    }

    private void SlideShowControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(this.ImagesFolderPath))
        return;
      this.LoadImagesFromFolder(this.ImagesFolderPath);
    }

    private void image1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.IsArrowVisible || this.mSlideShowDict.Count < 2)
      {
        this.image1_MouseLeave(sender, e);
      }
      else
      {
        if (!this.HideArrowOnLeave)
          return;
        if (this.image1.IsMouseOver)
        {
          this.mPrevBtn.Visibility = Visibility.Visible;
          this.mNextBtn.Visibility = Visibility.Visible;
        }
        else
          this.image1_MouseLeave(sender, e);
      }
    }

    private void image1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.IsArrowVisible || this.mSlideShowDict.Count < 2)
      {
        this.mPrevBtn.Visibility = Visibility.Hidden;
        this.mNextBtn.Visibility = Visibility.Hidden;
      }
      else
      {
        if (!this.HideArrowOnLeave || this.mPrevBtn.IsMouseOver || (this.mNextBtn.IsMouseOver || this.image1.IsMouseOver))
          return;
        this.mPrevBtn.Visibility = Visibility.Hidden;
        this.mNextBtn.Visibility = Visibility.Hidden;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.timer != null)
      {
        this.timer.Tick += new EventHandler(this.Timer_Tick);
        this.timer.Dispose();
      }
      this.disposedValue = true;
    }

    ~SlideShowControl()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/slideshowcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.slideControl = (SlideShowControl) target;
          this.slideControl.Loaded += new RoutedEventHandler(this.SlideShowControl_Loaded);
          break;
        case 2:
          this.SlideshowGrid = (Grid) target;
          break;
        case 3:
          this.image1 = (CustomPictureBox) target;
          this.image1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.image1_MouseEnter);
          this.image1.MouseLeave += new System.Windows.Input.MouseEventHandler(this.image1_MouseLeave);
          break;
        case 4:
          this.mPrevBtn = (CustomPictureBox) target;
          this.mPrevBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mPrevBtn_MouseLeftButtonUp);
          this.mPrevBtn.MouseLeave += new System.Windows.Input.MouseEventHandler(this.image1_MouseLeave);
          break;
        case 5:
          this.mNextBtn = (CustomPictureBox) target;
          this.mNextBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mNextBtn_MouseLeftButtonUp);
          this.mNextBtn.MouseLeave += new System.Windows.Input.MouseEventHandler(this.image1_MouseLeave);
          break;
        case 6:
          this.SlideshowName = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class SlideShowContext
    {
      [JsonProperty("key")]
      internal int Key;
      [JsonProperty("imagename")]
      internal string ImageName;
      [JsonProperty("description")]
      internal string Description;
    }

    public enum SlideAnimationType
    {
      Fade,
      Slide,
    }
  }
}
