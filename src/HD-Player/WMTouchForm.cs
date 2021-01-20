// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.WMTouchForm
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class WMTouchForm : Form
  {
    private const int WM_TOUCHMOVE = 576;
    private const int WM_TOUCHDOWN = 577;
    private const int WM_TOUCHUP = 578;
    private const int TOUCHEVENTF_MOVE = 1;
    private const int TOUCHEVENTF_DOWN = 2;
    private const int TOUCHEVENTF_UP = 4;
    private const int TOUCHEVENTF_INRANGE = 8;
    private const int TOUCHEVENTF_PRIMARY = 16;
    private const int TOUCHEVENTF_NOCOALESCE = 32;
    private const int TOUCHEVENTF_PEN = 64;
    private const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 1;
    private const int TOUCHINPUTMASKF_EXTRAINFO = 2;
    private const int TOUCHINPUTMASKF_CONTACTAREA = 4;
    private const int TWF_FINETOUCH = 1;
    private const int TWF_WANTPALM = 2;
    private WMTouchForm.TOUCHINPUT[] touchInputArray;
    private WMTouchForm.TouchPoint[] touchPointArray;
    private WMTouchForm.WMTouchEventArgs touchEventArgs;
    private int touchInputSize;

    [SecurityPermission(SecurityAction.Demand)]
    public WMTouchForm()
    {
      try
      {
        this.Load += new EventHandler(this.OnLoadHandler);
      }
      catch (Exception ex)
      {
        Logger.Info("Touch: ERROR: Could not add form load handler");
        Logger.Info("Touch: " + ex.ToString());
      }
      this.touchInputArray = new WMTouchForm.TOUCHINPUT[16];
      for (int index = 0; index < 16; ++index)
        this.touchInputArray[index] = new WMTouchForm.TOUCHINPUT();
      this.touchPointArray = new WMTouchForm.TouchPoint[16];
      for (int slot = 0; slot < 16; ++slot)
        this.touchPointArray[slot] = new WMTouchForm.TouchPoint(slot);
      this.touchEventArgs = new WMTouchForm.WMTouchEventArgs(this);
      this.touchInputSize = Marshal.SizeOf((object) new WMTouchForm.TOUCHINPUT());
    }

    protected event EventHandler<WMTouchForm.WMTouchEventArgs> TouchEvent;

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterTouchWindow(IntPtr hWnd, ulong ulFlags);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetTouchInputInfo(
      IntPtr hTouchInput,
      int cInputs,
      [In, Out] WMTouchForm.TOUCHINPUT[] pInputs,
      int cbSize);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern void CloseTouchInputHandle(IntPtr lParam);

    private void OnLoadHandler(object sender, EventArgs e)
    {
      ulong ulFlags = 2;
      try
      {
        if (WMTouchForm.RegisterTouchWindow(this.Handle, ulFlags))
          return;
        Logger.Info("Touch: ERROR: Could not register window for touch");
      }
      catch (Exception ex)
      {
        Logger.Info("Touch: ERROR: RegisterTouchWindow API not available");
        Logger.Info("Touch: " + ex.ToString());
      }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
      bool flag;
      switch (m.Msg)
      {
        case 576:
        case 577:
        case 578:
          flag = this.DecodeTouch(ref m);
          break;
        default:
          flag = false;
          break;
      }
      base.WndProc(ref m);
      if (!flag)
        return;
      try
      {
        m.Result = new IntPtr(1);
      }
      catch (Exception ex)
      {
        Logger.Info("Touch: ERROR: Could not allocate result ptr");
        Logger.Info("Touch: " + ex.ToString());
      }
    }

    private static int LoWord(int number)
    {
      return number & (int) ushort.MaxValue;
    }

    private bool DecodeTouch(ref Message m)
    {
      if (this.TouchEvent == null)
        return false;
      int cInputs = WMTouchForm.LoWord(m.WParam.ToInt32());
      if (cInputs > this.touchInputArray.Length)
        cInputs = this.touchInputArray.Length;
      if (!WMTouchForm.GetTouchInputInfo(m.LParam, cInputs, this.touchInputArray, this.touchInputSize))
        return false;
      for (int index = 0; index < this.touchPointArray.Length; ++index)
        this.touchPointArray[index].Clear();
      for (int index = 0; index < cInputs; ++index)
      {
        WMTouchForm.TOUCHINPUT touchInput = this.touchInputArray[index];
        WMTouchForm.TouchPoint touchPoint = this.touchPointArray[index];
        if ((touchInput.dwFlags & 2) != 0 || (touchInput.dwFlags & 1) != 0)
        {
          Point client = this.PointToClient(new Point(touchInput.x / 100, touchInput.y / 100));
          touchPoint.Id = touchInput.dwID;
          touchPoint.X = client.X;
          touchPoint.Y = client.Y;
        }
      }
      this.TouchEvent((object) this, this.touchEventArgs);
      WMTouchForm.CloseTouchInputHandle(m.LParam);
      return true;
    }

    public class TouchPoint
    {
      private int x;
      private int y;
      private int id;
      private int slot;

      public int X
      {
        get
        {
          return this.x;
        }
        set
        {
          this.x = value;
        }
      }

      public int Y
      {
        get
        {
          return this.y;
        }
        set
        {
          this.y = value;
        }
      }

      public int Id
      {
        get
        {
          return this.id;
        }
        set
        {
          this.id = value;
        }
      }

      public int Slot
      {
        get
        {
          return this.slot;
        }
      }

      public TouchPoint(int slot)
      {
        this.Clear();
        this.slot = slot;
      }

      public void Clear()
      {
        this.x = -1;
        this.y = -1;
        this.id = -1;
      }
    }

    public class WMTouchEventArgs : EventArgs
    {
      private WMTouchForm form;

      public int GetPointCount()
      {
        return this.form.touchPointArray.Length;
      }

      public WMTouchForm.TouchPoint GetPoint(int ndx)
      {
        return this.form.touchPointArray[ndx];
      }

      public WMTouchEventArgs(WMTouchForm form)
      {
        this.form = form;
      }
    }

    private struct TOUCHINPUT
    {
      public int x;
      public int y;
      public IntPtr hSource;
      public int dwID;
      public int dwFlags;
      public int dwMask;
      public int dwTime;
      public IntPtr dwExtraInfo;
      public int cxContact;
      public int cyContact;
    }

    private struct POINTS
    {
      public short x;
      public short y;
    }
  }
}
