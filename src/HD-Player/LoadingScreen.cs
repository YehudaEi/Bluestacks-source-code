// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.LoadingScreen
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  internal class LoadingScreen : UserControl
  {
    private Label appNameText = (Label) new LoadingScreen.AppNameText();
    private Image splashLogoImage;
    private Image whiteLogoImage;
    private Image closeButtonImage;
    private Image fullScreenButtonImage;
    private LoadingScreen.NewProgressBar progressBar;
    private Label statusText;
    private Label closeButton;
    private Label fullScreenButton;
    private Label appLogo;
    private Label whiteLogo;
    private string installDir;
    private string imageDir;
    private System.Windows.Forms.Timer progressTimer;
    private bool isFullScreen;
    private List<string> mLstDynamicString;
    internal static LoadingScreen mLoadingScreen;

    public LoadingScreen(
      Point loadingScreenLocation,
      Size loadingScreenSize,
      string barType,
      bool isFullScreenButtonVisible)
    {
      Logger.Info("LoadingScreen({0})", (object) loadingScreenSize);
      this.SetImageDir();
      this.LoadImages();
      this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      this.Location = loadingScreenLocation;
      this.Size = loadingScreenSize;
      this.BackColor = Color.FromArgb(35, 147, 213);
      this.appLogo = new Label();
      Logger.Info("Using splash logo");
      this.appLogo.BackgroundImage = this.splashLogoImage;
      this.appLogo.Width = this.appLogo.BackgroundImage.Width;
      this.appLogo.Height = this.appLogo.BackgroundImage.Height;
      this.appLogo.BackColor = Color.Transparent;
      this.installDir = RegistryStrings.InstallDir;
      string str = "LoadingScreenAppTitle";
      if (str.StartsWith("DynamicText"))
      {
        this.mLstDynamicString = new List<string>((IEnumerable<string>) str.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries));
        if (this.mLstDynamicString.Count > 0)
        {
          this.mLstDynamicString.Remove("DynamicText");
          System.Timers.Timer timer = new System.Timers.Timer(5000.0);
          timer.Elapsed += new ElapsedEventHandler(this.TimerElapsed);
          str = this.mLstDynamicString[0];
          timer.Start();
        }
      }
      this.appNameText.Text = str;
      this.appNameText.TextAlign = ContentAlignment.MiddleCenter;
      this.appNameText.Width = loadingScreenSize.Width;
      this.appNameText.Height = 50;
      this.appNameText.UseMnemonic = false;
      this.appNameText.ForeColor = Color.White;
      this.appNameText.BackColor = Color.Transparent;
      LoadingScreen.StatusText statusText = new LoadingScreen.StatusText();
      statusText.TextAlign = ContentAlignment.MiddleCenter;
      statusText.Width = loadingScreenSize.Width;
      statusText.Height = 40;
      statusText.UseMnemonic = false;
      statusText.ForeColor = Color.White;
      statusText.BackColor = Color.Transparent;
      this.statusText = (Label) statusText;
      LoadingScreen.NewProgressBar newProgressBar = new LoadingScreen.NewProgressBar(barType);
      newProgressBar.Width = 336;
      newProgressBar.Height = 10;
      newProgressBar.Value = 0;
      this.progressBar = newProgressBar;
      if (barType == "Marquee")
      {
        this.progressTimer = new System.Windows.Forms.Timer() { Interval = 50 };
        this.progressTimer.Tick += (EventHandler) ((obj, evt) => this.progressBar.Invalidate());
        this.progressTimer.Start();
      }
      Label label1 = new Label();
      label1.BackgroundImage = this.whiteLogoImage;
      label1.BackgroundImageLayout = ImageLayout.Stretch;
      label1.Width = 48;
      label1.Height = 44;
      label1.BackColor = Color.Transparent;
      this.whiteLogo = label1;
      Label label2 = new Label();
      label2.BackgroundImage = this.closeButtonImage;
      label2.BackgroundImageLayout = ImageLayout.Stretch;
      label2.Width = 24;
      label2.Height = 24;
      label2.BackColor = Color.Transparent;
      this.closeButton = label2;
      this.closeButton.Click += (EventHandler) ((obj, evt) => VMWindow.Instance.Close());
      this.closeButton.Visible = false;
      Label label3 = new Label();
      label3.BackgroundImage = this.fullScreenButtonImage;
      label3.BackgroundImageLayout = ImageLayout.Stretch;
      label3.Width = 24;
      label3.Height = 24;
      label3.BackColor = Color.Transparent;
      this.fullScreenButton = label3;
      this.fullScreenButton.Click += (EventHandler) ((obj, evt) =>
      {
        LayoutManager.ToggleFullScreen();
        this.FullScreenToggled();
      });
      if (!isFullScreenButtonVisible)
        this.fullScreenButton.Visible = false;
      this.SetLocations();
      this.Controls.Add((Control) this.appLogo);
      this.Controls.Add((Control) this.appNameText);
      this.Controls.Add((Control) this.progressBar);
      this.Controls.Add((Control) this.statusText);
      this.Controls.Add((Control) this.whiteLogo);
      this.Controls.Add((Control) this.closeButton);
      this.Controls.Add((Control) this.fullScreenButton);
      this.whiteLogo.Visible = false;
    }

    private void SetLocations()
    {
      int num1 = this.Size.Width / 2;
      int num2 = this.Size.Height / 2;
      Logger.Info("centerX: {0}, centerY: {1}", (object) num1, (object) num2);
      int num3 = this.appLogo.Height + 30 + this.appNameText.Height + 50 + this.progressBar.Height + 20 + this.statusText.Height;
      int y = num2 - num3 / 2;
      this.appLogo.Location = new Point(num1 - this.appLogo.Width / 2, y);
      this.appNameText.Location = new Point(0, this.appLogo.Bottom + 30);
      this.progressBar.Location = new Point(num1 - this.progressBar.Width / 2, this.appNameText.Bottom + 50);
      this.statusText.Location = new Point(0, this.progressBar.Bottom + 20);
      this.whiteLogo.Location = new Point(num1 - this.whiteLogo.Width / 2, this.Height - this.whiteLogo.Height - 20);
      this.closeButton.Location = new Point(this.Width - this.closeButton.Width - 30, 30);
      this.fullScreenButton.Location = new Point(this.closeButton.Left - 10 - this.fullScreenButton.Width, 30);
    }

    private void FullScreenToggled()
    {
      this.isFullScreen = !this.isFullScreen;
      this.closeButton.Visible = !this.closeButton.Visible;
      this.Size = VMWindow.Instance.ClientSize;
      if (this.isFullScreen)
      {
        double num1 = (double) VMWindow.Instance.ClientSize.Width / (double) LayoutManager.mConfiguredDisplaySize.Width;
        double num2 = (double) VMWindow.Instance.ClientSize.Height / (double) LayoutManager.mConfiguredDisplaySize.Height;
        this.appLogo.Width = (int) ((double) this.appLogo.Width * num1);
        this.appLogo.Height = (int) ((double) this.appLogo.Height * num1);
        this.splashLogoImage = (Image) new Bitmap((Image) new Bitmap(RegistryStrings.ProductImageCompletePath), new Size(this.appLogo.Width, this.appLogo.Height));
        this.appLogo.BackgroundImage = this.splashLogoImage;
        this.appNameText.Width = LoadingScreen.mLoadingScreen.Width;
        this.appNameText.Height = (int) ((double) this.appNameText.Height * num2);
        this.statusText.Width = LoadingScreen.mLoadingScreen.Width;
        this.statusText.Height = (int) ((double) this.statusText.Height * num2);
        this.progressBar.Width = (int) ((double) this.progressBar.Width * num1);
        this.progressBar.Height = (int) ((double) this.progressBar.Height * num2);
        this.whiteLogo.Width = (int) ((double) this.whiteLogo.Width * num1);
        this.whiteLogo.Height = (int) ((double) this.whiteLogo.Height * num2);
        this.fullScreenButton.Width = (int) ((double) this.fullScreenButton.Width * num1);
        this.fullScreenButton.Height = (int) ((double) this.fullScreenButton.Height * num2);
        this.closeButton.Width = (int) ((double) this.closeButton.Width * num1);
        this.closeButton.Height = (int) ((double) this.closeButton.Height * num2);
      }
      else
      {
        this.splashLogoImage = (Image) new Bitmap((Image) new Bitmap(RegistryStrings.ProductImageCompletePath), new Size(128, 128));
        this.appLogo.BackgroundImage = this.splashLogoImage;
        this.appLogo.Width = this.splashLogoImage.Width;
        this.appLogo.Height = this.splashLogoImage.Height;
        this.appNameText.Width = LoadingScreen.mLoadingScreen.Width;
        this.appNameText.Height = 50;
        this.statusText.Width = LoadingScreen.mLoadingScreen.Width;
        this.statusText.Height = 40;
        this.progressBar.Width = 336;
        this.progressBar.Height = 10;
        this.whiteLogo.Width = 48;
        this.whiteLogo.Height = 44;
        this.fullScreenButton.Width = 24;
        this.fullScreenButton.Height = 24;
        this.closeButton.Width = 24;
        this.closeButton.Height = 24;
      }
      this.SetLocations();
    }

    internal static void AddLoadingScreen(string barType = "Marquee")
    {
      Logger.Info("AddLoadingScreen: " + barType);
      if (LoadingScreen.mLoadingScreen != null && VMWindow.Instance.Controls.Contains((Control) LoadingScreen.mLoadingScreen))
      {
        Logger.Info("Already added");
      }
      else
      {
        Logger.Info("In Loading Screen");
        LoadingScreen.mLoadingScreen = new LoadingScreen(new Point(0, 0), new Size()
        {
          Height = VMWindow.Instance.ClientSize.Height,
          Width = VMWindow.Instance.ClientSize.Width
        }, barType, false);
        VMWindow.Instance.SuspendLayout();
        VMWindow.Instance.Controls.Add((Control) LoadingScreen.mLoadingScreen);
        LoadingScreen.mLoadingScreen.SetStatusText(LocaleStrings.GetLocalizedString("STRING_INITIALIZING", ""));
        VMWindow.Instance.ResumeLayout();
      }
    }

    internal static void RemoveLoadingScreen()
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        VMWindow.Instance.SuspendLayout();
        LoadingScreen.mLoadingScreen.progressTimer.Stop();
        LoadingScreen.mLoadingScreen.Hide();
        while (LoadingScreen.mLoadingScreen.Controls.Count > 0)
          LoadingScreen.mLoadingScreen.Controls[0].Dispose();
        VMWindow.Instance.Controls.Remove((Control) LoadingScreen.mLoadingScreen);
        LoadingScreen.mLoadingScreen.Dispose();
        LoadingScreen.mLoadingScreen = (LoadingScreen) null;
        VMWindow.Instance.ResumeLayout();
      }));
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          if (this.mLstDynamicString == null || this.mLstDynamicString.Count <= 0)
            return;
          int index = 0;
          if (this.mLstDynamicString.Contains(this.appNameText.Text))
            index = this.mLstDynamicString.IndexOf(this.appNameText.Text) + 1;
          if (index == this.mLstDynamicString.Count)
            index = 0;
          this.appNameText.Text = this.mLstDynamicString[index];
        }));
      }
      catch (Exception ex)
      {
        Logger.Info(ex.Message);
      }
    }

    public void SetStatusText(string text)
    {
      this.statusText.Text = text;
    }

    private void SetImageDir()
    {
      this.imageDir = RegistryStrings.InstallDir;
    }

    private void LoadImages()
    {
      Logger.Info("imageDir = " + this.imageDir);
      this.splashLogoImage = (Image) new Bitmap((Image) new Bitmap(RegistryStrings.ProductImageCompletePath), new Size(128, 128));
      this.whiteLogoImage = (Image) new Bitmap(Path.Combine(this.imageDir, "WhiteLogo.png"));
      this.closeButtonImage = (Image) new Bitmap(Path.Combine(this.imageDir, "XButton.png"));
      this.fullScreenButtonImage = (Image) new Bitmap(Path.Combine(this.imageDir, "WhiteFullScreen.png"));
    }

    public void UpdateProgressBar(int val)
    {
      this.progressBar.Value = val;
    }

    public class NewProgressBar : ProgressBar
    {
      internal LoadingScreen.NewProgressBar.BarType barType;
      private SolidBrush baseBrush;
      private SolidBrush backBrush;
      private SolidBrush foreBrush;
      private int marqueeStart;

      public NewProgressBar(string type)
      {
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.DoubleBuffer, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.barType = (LoadingScreen.NewProgressBar.BarType) Enum.Parse(typeof (LoadingScreen.NewProgressBar.BarType), type, true);
        this.baseBrush = new SolidBrush(Color.FromArgb(35, 147, 213));
        this.backBrush = new SolidBrush(Color.FromArgb(195, 195, 193));
        this.foreBrush = new SolidBrush(Color.FromArgb(21, 83, 120));
      }

      protected override void OnPaint(PaintEventArgs e)
      {
        Rectangle clipRectangle = e.ClipRectangle;
        this.FillRectangle(e, (Brush) this.baseBrush, 0, 0, clipRectangle.Width, 1);
        this.FillRectangle(e, (Brush) this.backBrush, 0, 1, clipRectangle.Width, clipRectangle.Height - 2);
        this.FillRectangle(e, (Brush) this.baseBrush, 0, clipRectangle.Height - 1, clipRectangle.Width, 1);
        switch (this.barType)
        {
          case LoadingScreen.NewProgressBar.BarType.Progress:
            int width = (int) ((double) clipRectangle.Width * ((double) this.Value / (double) this.Maximum));
            this.FillRectangle(e, (Brush) this.foreBrush, 0, 0, width, clipRectangle.Height);
            break;
          case LoadingScreen.NewProgressBar.BarType.Marquee:
            this.FillRectangle(e, (Brush) this.foreBrush, this.marqueeStart, 0, 96, clipRectangle.Height);
            this.marqueeStart += 10;
            if (this.marqueeStart < clipRectangle.Width - 20)
              break;
            this.marqueeStart = 0;
            break;
        }
      }

      private void FillRectangle(
        PaintEventArgs e,
        Brush brush,
        int x,
        int y,
        int width,
        int height)
      {
        e.Graphics.FillRectangle(brush, x, y, width, height);
      }

      internal enum BarType
      {
        Progress,
        Marquee,
      }
    }

    public class AppNameText : Label
    {
      public AppNameText()
      {
        this.Font = new Font(Utils.GetSystemFontName(), 18f, FontStyle.Bold);
        this.Height = 36;
      }

      protected override void OnPaint(PaintEventArgs evt)
      {
        evt.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        base.OnPaint(evt);
      }
    }

    public class StatusText : Label
    {
      public StatusText()
      {
        this.Font = new Font(Utils.GetSystemFontName(), 12f, FontStyle.Regular);
        this.Height = 24;
      }

      protected override void OnPaint(PaintEventArgs evt)
      {
        evt.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        base.OnPaint(evt);
      }
    }
  }
}
