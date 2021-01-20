// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.BstCursor
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class BstCursor
  {
    private const int COUNT_MAX = 4;
    private const int INITIAL_X = 128;
    private const int INITIAL_Y = 128;
    private BstCursor.State[] mCursors;
    private static BstCursor mBstCursor;

    internal static BstCursor Instance
    {
      get
      {
        if (BstCursor.mBstCursor == null)
          BstCursor.mBstCursor = new BstCursor();
        return BstCursor.mBstCursor;
      }
    }

    public BstCursor()
    {
      this.mCursors = new BstCursor.State[4];
      for (int slotId = 0; slotId < 4; ++slotId)
      {
        Bitmap primaryImage = new Bitmap(string.Format("{0}\\CursorPrimary.png", (object) VMWindow.Instance.InstallDir));
        Bitmap secondaryImage = new Bitmap(string.Format("{0}\\CursorSecondary.png", (object) VMWindow.Instance.InstallDir));
        this.mCursors[slotId] = new BstCursor.State(slotId, primaryImage, secondaryImage);
      }
    }

    public void Attach(int identity)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          this.InternalAttach(identity);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }));
    }

    public void Detach(int identity)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          this.InternalDetach(identity);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }));
    }

    public void Move(int identity, float x, float y, bool absolute)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          this.InternalMove(identity, x, y, absolute);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }));
    }

    public void Click(int identity, bool down)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          this.InternalClick(identity, down);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }));
    }

    public void RaiseFocusChange()
    {
      bool flag = false;
      if (Utils.IsForegroundApplication())
        flag = true;
      for (int index = 0; index < 4; ++index)
      {
        BstCursor.State mCursor = this.mCursors[index];
        if (mCursor.Pointer != null)
        {
          if (flag)
          {
            VMWindow.Instance.Focus();
            mCursor.Pointer.Show();
          }
          else
            mCursor.Pointer.Hide();
        }
      }
    }

    public void GetNormalizedPosition(int identity, out float x, out float y)
    {
      BstCursor.State state = this.LookupCursor(identity);
      if (state == null)
      {
        x = 0.0f;
        y = 0.0f;
      }
      else
      {
        Rectangle scaledDisplayArea = LayoutManager.mScaledDisplayArea;
        x = (float) state.Position.X / (float) scaledDisplayArea.Width;
        y = (float) state.Position.Y / (float) scaledDisplayArea.Height;
      }
    }

    private void InternalAttach(int identity)
    {
      Logger.Info("Cursor.Attach({0})", (object) identity);
      BstCursor.State state = this.LookupCursor(identity);
      if (state == null)
        Logger.Warning("Cannot find cursor slot for identity {0}", (object) identity);
      else if (state.Pointer != null)
      {
        Logger.Warning("Cursor slot ID %d already has a pointer", (object) state.SlotId);
      }
      else
      {
        Logger.Info("Cursor using slot {0}", (object) state.SlotId);
        state.Position.X = 128;
        state.Position.Y = 128;
        state.Clicked = false;
        state.Pointer = new BstCursor.Pointer();
        state.Pointer.SetBitmap(state.PrimaryImage);
        this.InternalMove(identity, 0.0f, 0.0f, false);
        VMWindow.Instance.Focus();
      }
    }

    private void InternalDetach(int identity)
    {
      Logger.Info("Cursor.Detach({0})", (object) identity);
      BstCursor.State state = this.LookupCursor(identity);
      if (state == null)
      {
        Logger.Warning("Cannot find cursor slot for identity {0}", (object) identity);
      }
      else
      {
        state.Pointer.Close();
        state.Pointer = (BstCursor.Pointer) null;
        state.Position.X = 0;
        state.Position.Y = 0;
        state.Clicked = false;
      }
    }

    private void InternalMove(int identity, float x, float y, bool absolute)
    {
      if (!Utils.IsForegroundApplication())
        return;
      BstCursor.State state = this.LookupCursor(identity);
      if (state == null)
      {
        Logger.Warning("Cannot find cursor slot for identity {0}", (object) identity);
      }
      else
      {
        Rectangle scaledDisplayArea = LayoutManager.mScaledDisplayArea;
        state.Position.X += (int) x;
        if (state.Position.X < 0)
          state.Position.X = 0;
        else if (state.Position.X > scaledDisplayArea.Width)
          state.Position.X = scaledDisplayArea.Width;
        state.Position.Y += (int) y;
        if (state.Position.Y < 0)
          state.Position.Y = 0;
        else if (state.Position.Y > scaledDisplayArea.Height)
          state.Position.Y = scaledDisplayArea.Height;
        if (VMWindow.Instance.Visible)
        {
          Rectangle screen = VMWindow.Instance.RectangleToScreen(scaledDisplayArea);
          int x1 = state.Position.X + screen.Left - state.Pointer.GetBitmap().Width / 2;
          int y1 = state.Position.Y + screen.Top - state.Pointer.GetBitmap().Height / 2;
          state.Pointer.Update(x1, y1);
          state.Pointer.Show();
        }
        else
          state.Pointer.Hide();
        InputMapper.TouchPoint[] points = new InputMapper.TouchPoint[1];
        points[0].X = (float) state.Position.X / (float) scaledDisplayArea.Width;
        points[0].Y = (float) state.Position.Y / (float) scaledDisplayArea.Height;
        points[0].Down = state.Clicked;
        InputMapper.Instance.TouchHandlerImpl(points, state.SlotId * 4, false);
      }
    }

    private void InternalClick(int identity, bool down)
    {
      if (!Utils.IsForegroundApplication())
        return;
      BstCursor.State state = this.LookupCursor(identity);
      if (state == null)
      {
        Logger.Warning("Cannot find cursor slot for identity {0}", (object) identity);
      }
      else
      {
        state.Clicked = down;
        if (!down)
          state.Pointer.SetBitmap(state.PrimaryImage);
        else
          state.Pointer.SetBitmap(state.SecondaryImage);
        this.InternalMove(identity, 0.0f, 0.0f, false);
      }
    }

    private BstCursor.State LookupCursor(int identity)
    {
      int index = -1;
      if (identity >= 0 && identity < 4)
        index = 3 - identity;
      else if (identity >= 16)
        index = identity - 16;
      return index >= 0 && index < 4 ? this.mCursors[index] : (BstCursor.State) null;
    }

    private class State
    {
      public int SlotId;
      public Bitmap PrimaryImage;
      public Bitmap SecondaryImage;
      public BstCursor.Pointer Pointer;
      public Point Position;
      public bool Clicked;

      public State(int slotId, Bitmap primaryImage, Bitmap secondaryImage)
      {
        this.SlotId = slotId;
        this.PrimaryImage = primaryImage;
        this.SecondaryImage = secondaryImage;
      }
    }

    private class Pointer : Form
    {
      private const int WS_EX_TRANSPARENT = 32;
      private const int WS_EX_TOOLWINDOW = 128;
      private const int WS_EX_LAYERED = 524288;
      private const byte AC_SRC_OVER = 0;
      private const byte AC_SRC_ALPHA = 1;
      private const int ULW_ALPHA = 2;
      private Bitmap mBitmap;

      [DllImport("user32.dll", SetLastError = true)]
      private static extern bool UpdateLayeredWindow(
        IntPtr hwnd,
        IntPtr hdcDst,
        ref BstCursor.Pointer.Win32Point pptDst,
        ref BstCursor.Pointer.Win32Size psize,
        IntPtr hdcSrc,
        ref BstCursor.Pointer.Win32Point pprSrc,
        int crKey,
        ref BstCursor.Pointer.BLENDFUNCTION pblend,
        int dwFlags);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern IntPtr GetDC(IntPtr hWnd);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern bool DeleteDC(IntPtr hdc);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern bool DeleteObject(IntPtr hObject);

      public Pointer()
      {
        this.SuspendLayout();
        this.ShowInTaskbar = false;
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.ResumeLayout();
      }

      protected override CreateParams CreateParams
      {
        get
        {
          CreateParams createParams = base.CreateParams;
          createParams.ExStyle |= 32;
          createParams.ExStyle |= 128;
          createParams.ExStyle |= 524288;
          return createParams;
        }
      }

      public void SetBitmap(Bitmap bitmap)
      {
        if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
          throw new ApplicationException("Bad bitmap");
        this.mBitmap = bitmap;
      }

      public Bitmap GetBitmap()
      {
        return this.mBitmap;
      }

      public void Update(int x, int y)
      {
        IntPtr dc = BstCursor.Pointer.GetDC(IntPtr.Zero);
        IntPtr compatibleDc = BstCursor.Pointer.CreateCompatibleDC(dc);
        IntPtr hObject1 = IntPtr.Zero;
        IntPtr hObject2 = IntPtr.Zero;
        try
        {
          hObject1 = this.mBitmap.GetHbitmap(Color.FromArgb(0));
          hObject2 = BstCursor.Pointer.SelectObject(compatibleDc, hObject1);
          BstCursor.Pointer.Win32Size psize = new BstCursor.Pointer.Win32Size(this.mBitmap.Width, this.mBitmap.Height);
          BstCursor.Pointer.Win32Point pprSrc = new BstCursor.Pointer.Win32Point(0, 0);
          BstCursor.Pointer.Win32Point pptDst = new BstCursor.Pointer.Win32Point(x, y);
          BstCursor.Pointer.BLENDFUNCTION pblend = new BstCursor.Pointer.BLENDFUNCTION()
          {
            BlendOp = 0,
            BlendFlags = 0,
            SourceConstantAlpha = byte.MaxValue,
            AlphaFormat = 1
          };
          if (BstCursor.Pointer.UpdateLayeredWindow(this.Handle, dc, ref pptDst, ref psize, compatibleDc, ref pprSrc, 0, ref pblend, 2))
            return;
          CommonError.ThrowLastWin32Error("Cannot update layered window");
        }
        finally
        {
          BstCursor.Pointer.ReleaseDC(IntPtr.Zero, dc);
          if (hObject1 != IntPtr.Zero)
          {
            BstCursor.Pointer.SelectObject(compatibleDc, hObject2);
            BstCursor.Pointer.DeleteObject(hObject1);
          }
          BstCursor.Pointer.DeleteDC(compatibleDc);
        }
      }

      private struct Win32Point
      {
        public int X;
        public int Y;

        public Win32Point(int x, int y)
        {
          this.X = x;
          this.Y = y;
        }
      }

      private struct Win32Size
      {
        public int Width;
        public int Height;

        public Win32Size(int width, int height)
        {
          this.Width = width;
          this.Height = height;
        }
      }

      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      private struct BLENDFUNCTION
      {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
      }
    }
  }
}
