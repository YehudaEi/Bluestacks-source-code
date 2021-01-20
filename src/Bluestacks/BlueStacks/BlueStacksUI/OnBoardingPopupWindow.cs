// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.OnBoardingPopupWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class OnBoardingPopupWindow : CustomWindow, INotifyPropertyChanged, IComponentConnector
  {
    private PopupArrowAlignment mPopArrowAlignment = PopupArrowAlignment.Top;
    private string mHeaderContent;
    private string mBodyContent;
    private bool mIsLastPopup;
    internal Border mMaskBorder;
    internal Grid ContentGrid;
    internal TextBlock headerTextBlock;
    internal TextBlock bodyTextBlock;
    internal BlurbMessageControl bodyContentBlurbControl;
    internal CustomButton OkayButton;
    internal Path RightArrow;
    private bool _contentLoaded;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    public event System.Action Startevent;

    public event System.Action Endevent;

    public string HeaderContent
    {
      get
      {
        return this.mHeaderContent;
      }
      set
      {
        if (!(this.mHeaderContent != value))
          return;
        this.mHeaderContent = value;
        this.OnPropertyChanged(nameof (HeaderContent));
      }
    }

    public string BodyContent
    {
      get
      {
        return this.mBodyContent;
      }
      set
      {
        if (!(this.mBodyContent != value))
          return;
        this.mBodyContent = value;
        this.OnPropertyChanged(nameof (BodyContent));
      }
    }

    public bool IsLastPopup
    {
      get
      {
        return this.mIsLastPopup;
      }
      set
      {
        if (this.mIsLastPopup == value)
          return;
        this.mIsLastPopup = value;
        this.OnPropertyChanged(nameof (IsLastPopup));
      }
    }

    public PopupArrowAlignment PopArrowAlignment
    {
      get
      {
        return this.mPopArrowAlignment;
      }
      set
      {
        if (this.mPopArrowAlignment == value)
          return;
        this.mPopArrowAlignment = value;
        this.OnPropertyChanged(nameof (PopArrowAlignment));
      }
    }

    public UIElement PlacementTarget { get; set; }

    public int LeftMargin { get; set; }

    public int TopMargin { get; set; }

    public bool IsBlurbRelatedToGuidance { get; set; }

    public string PackageName { get; set; }

    public MainWindow ParentWindow { get; set; }

    public OnBoardingPopupWindow(MainWindow mainWindow, string packageName)
    {
      this.PackageName = packageName;
      this.ParentWindow = mainWindow;
      this.InitializeComponent();
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.SizeChanged -= new SizeChangedEventHandler(this.Owner_SizeChanged);
      this.ParentWindow.LocationChanged -= new EventHandler(this.Owner_SizeChanged);
      this.ParentWindow.StateChanged -= new EventHandler(this.Owner_SizeChanged);
      this.ParentWindow.SizeChanged += new SizeChangedEventHandler(this.Owner_SizeChanged);
      this.ParentWindow.LocationChanged += new EventHandler(this.Owner_SizeChanged);
      this.ParentWindow.StateChanged += new EventHandler(this.Owner_SizeChanged);
    }

    private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
    {
      System.Action startevent = this.Startevent;
      if (startevent == null)
        return;
      startevent();
    }

    private void Owner_SizeChanged(object sender, EventArgs e)
    {
      if (this.PlacementTarget == null || !this.PlacementTarget.IsVisible)
        return;
      this.SetValue(Window.LeftProperty, (object) (this.PlacementTarget.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) this.LeftMargin));
      this.SetValue(Window.TopProperty, (object) (this.PlacementTarget.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) this.TopMargin));
    }

    public void CloseWindow()
    {
      System.Action endevent = this.Endevent;
      if (endevent != null)
        endevent();
      this.Close();
    }

    private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", this.ParentWindow.mVmName, this.PackageName, this.Title, "", "");
      this.CloseWindow();
    }

    private void CustomWindow_KeyDown(object sender, KeyEventArgs e1)
    {
      if (e1.Key == Key.System && e1.SystemKey == Key.F4)
      {
        e1.Handled = true;
      }
      else
      {
        string keyPressed = string.Empty;
        keyPressed = e1.Key != Key.System ? MainWindow.GetShortcutKey(e1.Key) : MainWindow.GetShortcutKey(e1.SystemKey);
        MainWindow owner = (MainWindow) this.Owner;
        if (!string.Equals(keyPressed, owner.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false), StringComparison.InvariantCulture) || !(this.Title == "FullScreenBlurb"))
          return;
        ClientHotKeys clienthotKey = (ClientHotKeys) Enum.Parse(typeof (ClientHotKeys), owner.mCommonHandler.mShortcutsConfigInstance.Shortcut.Where<ShortcutKeys>((Func<ShortcutKeys, bool>) (e2 => e2.ShortcutKey.Equals(keyPressed, StringComparison.InvariantCulture))).First<ShortcutKeys>().ShortcutName);
        owner.HandleClientHotKey(clienthotKey);
        this.CloseWindow();
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/onboardingpopupwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.CustomWindow_Loaded);
          ((UIElement) target).KeyDown += new KeyEventHandler(this.CustomWindow_KeyDown);
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          this.ContentGrid = (Grid) target;
          break;
        case 4:
          this.headerTextBlock = (TextBlock) target;
          break;
        case 5:
          this.bodyTextBlock = (TextBlock) target;
          break;
        case 6:
          this.bodyContentBlurbControl = (BlurbMessageControl) target;
          break;
        case 7:
          this.OkayButton = (CustomButton) target;
          this.OkayButton.Click += new RoutedEventHandler(this.OnBoardingPopupNext_Click);
          break;
        case 8:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnBoardingPopupNext_Click);
          break;
        case 9:
          this.RightArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
