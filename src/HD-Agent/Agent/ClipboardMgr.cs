// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.ClipboardMgr
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Agent
{
  public class ClipboardMgr : Form
  {
    private const int WM_DRAWCLIPBOARD = 776;
    private const int WM_CHANGECBCHAIN = 781;
    private const int WM_CLIPBOARDUPDATE = 797;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    [DllImport("User32.dll")]
    private static extern int SetClipboardViewer(int hWndNewViewer);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    public ClipboardMgr()
    {
      this.WindowState = FormWindowState.Minimized;
      this.Load += new EventHandler(this.OnLoad);
    }

    protected override void OnActivated(EventArgs args)
    {
      this.Hide();
    }

    private void OnLoad(object sender, EventArgs e)
    {
      this.Hide();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      ClipboardMgr.AddClipboardFormatListener(this.Handle);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
      ClipboardMgr.RemoveClipboardFormatListener(this.Handle);
      base.OnHandleDestroyed(e);
    }

    private void ProcessClipboardData()
    {
      string clipboardText = "";
      string str = "";
      bool flag = false;
      for (int index = 0; index < 10; ++index)
      {
        try
        {
          clipboardText = System.Windows.Clipboard.GetText();
          flag = true;
          break;
        }
        catch (COMException ex)
        {
          str = ex.ToString();
        }
        Thread.Sleep(20);
      }
      if (!flag)
        Logger.Error("Handled com error in clipboard...Err : " + str);
      else
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          foreach (string vm in RegistryManager.Instance.VmList)
          {
            try
            {
              HTTPUtils.SendRequestToGuest("clipboard", new Dictionary<string, string>()
              {
                {
                  "text",
                  clipboardText
                }
              }, vm, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in Sending ClipboardCommand {0}", (object) ex.ToString());
            }
          }
        }));
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 797)
        this.ProcessClipboardData();
      else
        base.WndProc(ref m);
    }
  }
}
