// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.InputMapperForm
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class InputMapperForm : Form
  {
    private const int WIDTH = 400;
    private const int HEIGHT = 250;
    private const int PADDING = 10;
    private const int TEXT_HEIGHT = 50;
    private string mPackage;
    private InputMapperForm.EditHandler mEditHandler;
    private InputMapperForm.ManageHandler mManageHandler;

    public InputMapperForm(
      string package,
      InputMapperForm.EditHandler editHandler,
      InputMapperForm.ManageHandler manageHandler)
    {
      this.mPackage = package;
      this.mEditHandler = editHandler;
      this.mManageHandler = manageHandler;
      this.Text = "Input Mapper Tool";
      this.CreateLayout();
    }

    private void CreateLayout()
    {
      this.Size = new Size(400, 250);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MinimizeBox = false;
      this.MaximizeBox = false;
      Label label1 = new Label();
      label1.Text = "Current app: " + (this.mPackage != null ? this.mPackage : "none");
      label1.Location = new Point(10, 10);
      label1.Width = this.ClientSize.Width - 10;
      Label label2 = label1;
      Button button1 = new Button();
      button1.Text = "Edit";
      button1.Location = new Point(10, label2.Bottom + 10);
      Button button2 = button1;
      if (this.mPackage == null)
        button2.Enabled = false;
      button2.Click += (EventHandler) ((obj, evt) =>
      {
        this.mEditHandler(this.mPackage);
        this.Close();
      });
      Label label3 = new Label();
      label3.Text = "Edit the input mapper configuration for the current app.  If the current app does not yet have a configuration file, then create one from a template.";
      label3.Location = new Point(button2.Right + 10, button2.Top);
      Label label4 = label3;
      label4.Size = new Size(this.ClientSize.Width - label4.Left - 10, 50);
      Button button3 = new Button();
      button3.Text = "Manage";
      button3.Location = new Point(10, label4.Bottom + 10);
      Button button4 = button3;
      button4.Click += (EventHandler) ((obj, evt) =>
      {
        this.mManageHandler(this.mPackage);
        this.Close();
      });
      Label label5 = new Label();
      label5.Text = "Manage all the existing input mapper configurations.  Opens the input mapper folder in Windows Explorer.";
      label5.Location = new Point(button4.Right + 10, button4.Top);
      Label label6 = label5;
      label6.Size = new Size(this.ClientSize.Width - label6.Left - 10, 50);
      Button button5 = new Button();
      button5.Text = "Cancel";
      button5.Location = new Point(10, label6.Bottom + 10);
      Button button6 = button5;
      button6.Click += (EventHandler) ((obj, evt) => this.Close());
      Label label7 = new Label();
      label7.Text = "Close this window.";
      label7.Location = new Point(button6.Right + 10, button6.Top);
      Label label8 = label7;
      label8.Size = new Size(this.ClientSize.Width - label8.Left - 10, 50);
      this.Controls.AddRange(new Control[7]
      {
        (Control) label2,
        (Control) button2,
        (Control) label4,
        (Control) button4,
        (Control) label6,
        (Control) button6,
        (Control) label8
      });
    }

    public delegate void EditHandler(string package);

    public delegate void ManageHandler(string package);
  }
}
