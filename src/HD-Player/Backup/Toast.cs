// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Toast
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  internal class Toast : Form
  {
    private Font font = new Font(Utils.GetSystemFontName(), 12f);
    private const int WS_EX_TOOLWINDOW = 128;
    private const int WS_EX_NOACTIVATE = 134217728;
    private const int WS_CHILD = 1073741824;
    private SizeF stringSize;
    private string toastText;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetProcessDPIAware();

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateRoundRectRgn(
      int nLeftRect,
      int nTopRect,
      int nRightRect,
      int nBottomRect,
      int nWidthEllipse,
      int nHeightEllipse);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        createParams.Style = 1073741824;
        createParams.ExStyle |= 134217856;
        return createParams;
      }
    }

    public Toast(Control parent, string toastText)
    {
      this.toastText = toastText;
      this.stringSize = this.CreateGraphics().MeasureString(this.toastText, this.font);
      this.StartPosition = FormStartPosition.Manual;
      this.FormBorderStyle = FormBorderStyle.None;
      this.ShowInTaskbar = false;
      this.Paint += new PaintEventHandler(this.ShowToast);
      this.Width = (int) this.stringSize.Width + 20;
      this.Height = (int) this.stringSize.Height + 20;
      this.Location = new Point(parent.Left + (parent.Width - this.Width) / 2, parent.Top + 5);
      IntPtr roundRectRgn = Toast.CreateRoundRectRgn(0, 0, this.Width, this.Height, 5, 5);
      this.Region = Region.FromHrgn(roundRectRgn);
      Toast.DeleteObject(roundRectRgn);
    }

    private void ShowToast(object sender, PaintEventArgs e)
    {
      RectangleF rect = new RectangleF(0.0f, 0.0f, (float) this.Width, (float) this.Height);
      Pen pen = new Pen(Color.Black);
      e.Graphics.DrawRectangle(pen, 0, 0, this.Width, this.Height);
      SolidBrush solidBrush1 = new SolidBrush(Color.White);
      e.Graphics.FillRectangle((Brush) solidBrush1, rect);
      RectangleF layoutRectangle = new RectangleF((float) (((double) this.Width - (double) this.stringSize.Width) / 2.0), (float) (((double) this.Height - (double) this.stringSize.Height) / 2.0), this.stringSize.Width, this.stringSize.Height);
      SolidBrush solidBrush2 = new SolidBrush(Color.Black);
      e.Graphics.DrawString(this.toastText, this.font, (Brush) solidBrush2, layoutRectangle);
    }
  }
}
