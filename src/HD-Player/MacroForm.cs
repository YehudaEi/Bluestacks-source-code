// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MacroForm
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class MacroForm : Form
  {
    internal static bool sIsRecording = false;
    internal static Macro mMacro = new Macro();
    internal static MacroForm mInstance = (MacroForm) null;
    private string editComboText = "Editing-New-Combo";
    private static Thread t;
    internal static DateTime mBaseDateTime;
    private IContainer components;
    private DataGridView dataGridView1;
    private SplitContainer splitContainer1;
    private Button bntStartRecord;
    private ComboBox cmbMacros;
    private Button btnStop;
    private Button btnPlay;
    private Button btnSave;
    private Button btnStopRecord;
    private Button btnAdd;
    private ComboBox cmbRepeatBehaviour;
    private Button btnReload;

    internal static MacroForm Instance
    {
      get
      {
        if (MacroForm.mInstance == null)
          MacroForm.mInstance = new MacroForm();
        return MacroForm.mInstance;
      }
    }

    private MacroForm()
    {
      this.InitializeComponent();
      this.cmbRepeatBehaviour.DataSource = (object) Enum.GetValues(typeof (RepeatBehaviour));
      this.UpdateUIForNewFile();
    }

    private void btnReload_Click(object sender, EventArgs e)
    {
      this.UpdateUIForNewFile();
    }

    private void UpdateUIForNewFile()
    {
      this.Text = "MacroForm - " + InputMapper.Instance.GetMacroFileName(false, InputMapper.Instance.GetPackage());
      List<string> list = MacroData.Instance.DictMacros.Keys.ToList<string>();
      list.Add(this.editComboText);
      this.cmbMacros.DataSource = (object) list;
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
      MacroForm.PlayMacro(this.cmbMacros.SelectedValue.ToString());
    }

    public static void PlayMacro(string MacroName)
    {
      Macro macro = string.IsNullOrEmpty(MacroName) || !MacroData.Instance.DictMacros.ContainsKey(MacroName) ? MacroData.Instance.DictMacros.FirstOrDefault<KeyValuePair<string, Macro>>().Value : MacroData.Instance.DictMacros[MacroName];
      int windowWidth = VMWindow.Instance.Width;
      int windowHeight = VMWindow.Instance.Height;
      if (MacroForm.t != null && MacroForm.t.IsAlive)
        MacroForm.AbortReroll();
      MacroForm.t = new Thread((ThreadStart) (() =>
      {
        try
        {
          DateTime dateTime = DateTime.Now;
          switch (macro.RepeatBehaiour)
          {
            case RepeatBehaviour.Once:
              dateTime = DateTime.Now.AddSeconds(2.0);
              break;
            case RepeatBehaviour.For5Minutes:
              dateTime = DateTime.Now.AddMinutes(5.0);
              break;
            case RepeatBehaviour.For1hour:
              dateTime = DateTime.Now.AddHours(1.0);
              break;
            case RepeatBehaviour.For8Hour:
              dateTime = DateTime.Now.AddHours(8.0);
              break;
            case RepeatBehaviour.Forever:
              dateTime = DateTime.Now.AddYears(5);
              break;
          }
          bool flag = true;
          while (DateTime.Now < dateTime)
          {
            if (flag)
            {
              flag = (uint) macro.AndroidCommandRepeatMode > 0U;
              foreach (KeyValuePair<string, SerializableDictionary<string, string>> dictAndroidCommand in (Dictionary<string, SerializableDictionary<string, string>>) macro.DictAndroidCommands)
              {
                try
                {
                  JsonParser jsonParser = new JsonParser(MultiInstanceStrings.VmName);
                  if (dictAndroidCommand.Key.StartsWith("MacroSleep"))
                    Thread.Sleep(Convert.ToInt32(dictAndroidCommand.Value.Keys.ElementAt<string>(0)));
                  else if (dictAndroidCommand.Key.Equals("runex") && jsonParser.GetAppInfoFromPackageName(MacroData.Instance.mPackageName) != null)
                    VmCmdHandler.RunCommand(string.Format("runex {0}/{1}", (object) MacroData.Instance.mPackageName, (object) jsonParser.GetAppInfoFromPackageName(MacroData.Instance.mPackageName).Activity), MultiInstanceStrings.VmName, "bgp");
                  else
                    MacroForm.RunCommand(dictAndroidCommand);
                }
                catch
                {
                }
              }
            }
            foreach (KeyValuePair<int, MacroAction> dictMacroAction in (Dictionary<int, MacroAction>) macro.DictMacroActions)
            {
              MacroAction macroAction = dictMacroAction.Value;
              Thread.Sleep((int) macroAction.DelayFromLastAction);
              switch (macroAction.ActionType)
              {
                case ActionType.MouseDown:
                  InputMapper.Instance.HandleMouseDown((object) null, new MouseEventArgs(macroAction.MouseButton, 0, (int) (macroAction.ActionPointX * (double) windowWidth), (int) (macroAction.ActionPointY * (double) windowHeight), 0));
                  continue;
                case ActionType.MouseUp:
                  InputMapper.Instance.HandleMouseUp((object) null, new MouseEventArgs(macroAction.MouseButton, 0, (int) (macroAction.ActionPointX * (double) windowWidth), (int) (macroAction.ActionPointY * (double) windowHeight), 0));
                  continue;
                default:
                  continue;
              }
            }
          }
          MacroForm.t = (Thread) null;
          HTTPUtils.SendRequestToClient("macroCompleted", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (ThreadAbortException ex)
        {
          Logger.Error("Error running reroll", (object) ex.ToString());
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in running reroll. Err : ", (object) ex.ToString());
          HTTPUtils.SendRequestToClient("macroCompleted", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
      }))
      {
        IsBackground = true
      };
      MacroForm.t.Start();
    }

    internal static void RunCommand(
      KeyValuePair<string, SerializableDictionary<string, string>> item)
    {
      try
      {
        string key = item.Key;
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) item.Value)
          data.Add(keyValuePair.Key, keyValuePair.Value);
        string url = string.Format("http://127.0.0.1:{0}/{1}", (object) Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName), (object) key);
        Logger.Info("The url being hit is {0}", (object) url);
        Logger.Info("Resp: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, MultiInstanceStrings.VmName, 500, 1, 0, false, "bgp"));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendKeymappingFiledownloadRequest. Err : " + ex.ToString());
      }
    }

    internal void btnStop_Click(object sender, EventArgs e)
    {
      MacroForm.AbortReroll();
    }

    internal static void AbortReroll()
    {
      try
      {
        if (MacroForm.t == null)
          return;
        MacroForm.t.Abort();
      }
      catch
      {
      }
    }

    private void bntStartRecord_Click(object sender, EventArgs e)
    {
      this.btnStopRecord.Enabled = true;
      this.btnAdd.Enabled = false;
      MacroForm.mMacro = new Macro();
      MacroForm.mBaseDateTime = DateTime.Now;
      MacroForm.sIsRecording = true;
      this.cmbMacros.SelectedItem = (object) this.editComboText;
      this.cmbMacros.Enabled = false;
    }

    private void btnStopRecord_Click(object sender, EventArgs e)
    {
      this.cmbMacros.Enabled = true;
      this.btnStopRecord.Enabled = false;
      if (MacroForm.mMacro.DictMacroActions.Count > 0)
        this.btnAdd.Enabled = true;
      MacroForm.sIsRecording = false;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      string key = Interaction.InputBox("Please enter a name for macro ", string.Format("{0} Macro", (object) BlueStacks.Common.Strings.ProductDisplayName), "", -1, -1);
      if (MacroData.Instance.DictMacros.ContainsKey(key) || string.IsNullOrEmpty(key))
      {
        int num = (int) MessageBox.Show("Macro name already exists");
      }
      else
      {
        this.btnAdd.Enabled = false;
        MacroForm.mMacro.RepeatBehaiour = (RepeatBehaviour) Enum.Parse(typeof (RepeatBehaviour), this.cmbRepeatBehaviour.SelectedValue.ToString(), true);
        MacroData.Instance.DictMacros.Add(key, MacroForm.mMacro);
        this.UpdateUIForNewFile();
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      MacroForm.mMacro = new Macro();
      this.UpdateGrid();
      MacroData.Instance.SaveMacroData();
    }

    private void cmbMacros_SelectedValueChanged(object sender, EventArgs e)
    {
      this.UpdateGrid();
    }

    private void UpdateGrid()
    {
      if (this.cmbMacros.SelectedValue.ToString() == this.editComboText)
        this.dataGridView1.DataSource = (object) MacroForm.mMacro.DictMacroActions.Values.ToList<MacroAction>();
      else
        this.dataGridView1.DataSource = (object) MacroData.Instance.DictMacros[this.cmbMacros.SelectedValue.ToString()].DictMacroActions.Values.ToList<MacroAction>();
    }

    internal static void RecordMouse(
      double x,
      double y,
      double width,
      double height,
      MouseButtons button,
      ActionType type)
    {
      if (!MacroForm.sIsRecording)
        return;
      double totalMilliseconds = (DateTime.Now - MacroForm.mBaseDateTime).TotalMilliseconds;
      MacroForm.mBaseDateTime = DateTime.Now;
      MacroForm.AddActionInDictionary(new MacroAction()
      {
        ActionType = type,
        MouseButton = button,
        ActionPointX = x / width,
        ActionPointY = y / height,
        DelayFromLastAction = totalMilliseconds
      });
    }

    internal static void RecordKeys(Keys keyCode, ActionType type)
    {
      if (!MacroForm.sIsRecording)
        return;
      double totalMilliseconds = (DateTime.Now - MacroForm.mBaseDateTime).TotalMilliseconds;
      MacroForm.mBaseDateTime = DateTime.Now;
      MacroForm.AddActionInDictionary(new MacroAction()
      {
        ActionType = type,
        ActionKey = keyCode,
        DelayFromLastAction = totalMilliseconds
      });
    }

    private static void AddActionInDictionary(MacroAction ma)
    {
      MacroForm.mMacro.DictMacroActions.Add(MacroForm.mMacro.DictMacroActions.Count, ma);
      MacroForm.Instance.UpdateGrid();
    }

    private void MacroForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.dataGridView1 = new DataGridView();
      this.splitContainer1 = new SplitContainer();
      this.btnReload = new Button();
      this.btnAdd = new Button();
      this.cmbRepeatBehaviour = new ComboBox();
      this.btnSave = new Button();
      this.btnStopRecord = new Button();
      this.bntStartRecord = new Button();
      this.cmbMacros = new ComboBox();
      this.btnStop = new Button();
      this.btnPlay = new Button();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Dock = DockStyle.Fill;
      this.dataGridView1.Location = new Point(0, 0);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new Size(1152, 722);
      this.dataGridView1.TabIndex = 0;
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.Location = new Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = Orientation.Horizontal;
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnReload);
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnAdd);
      this.splitContainer1.Panel1.Controls.Add((Control) this.cmbRepeatBehaviour);
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnSave);
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnStopRecord);
      this.splitContainer1.Panel1.Controls.Add((Control) this.bntStartRecord);
      this.splitContainer1.Panel1.Controls.Add((Control) this.cmbMacros);
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnStop);
      this.splitContainer1.Panel1.Controls.Add((Control) this.btnPlay);
      this.splitContainer1.Panel2.Controls.Add((Control) this.dataGridView1);
      this.splitContainer1.Size = new Size(1152, 790);
      this.splitContainer1.SplitterDistance = 64;
      this.splitContainer1.TabIndex = 1;
      this.btnReload.Location = new Point(1026, 17);
      this.btnReload.Name = "btnReload";
      this.btnReload.Size = new Size(92, 30);
      this.btnReload.TabIndex = 1;
      this.btnReload.Text = "Reload";
      this.btnReload.UseVisualStyleBackColor = true;
      this.btnReload.Click += new EventHandler(this.btnReload_Click);
      this.btnAdd.Enabled = false;
      this.btnAdd.Location = new Point(734, 18);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new Size(92, 30);
      this.btnAdd.TabIndex = 7;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
      this.cmbRepeatBehaviour.FormattingEnabled = true;
      this.cmbRepeatBehaviour.Location = new Point(607, 24);
      this.cmbRepeatBehaviour.Name = "cmbRepeatBehaviour";
      this.cmbRepeatBehaviour.Size = new Size(121, 21);
      this.cmbRepeatBehaviour.TabIndex = 6;
      this.btnSave.Location = new Point(928, 17);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(92, 30);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.btnStopRecord.Enabled = false;
      this.btnStopRecord.Location = new Point(509, 18);
      this.btnStopRecord.Name = "btnStopRecord";
      this.btnStopRecord.Size = new Size(92, 30);
      this.btnStopRecord.TabIndex = 4;
      this.btnStopRecord.Text = "Stop Record";
      this.btnStopRecord.UseVisualStyleBackColor = true;
      this.btnStopRecord.Click += new EventHandler(this.btnStopRecord_Click);
      this.bntStartRecord.Location = new Point(411, 18);
      this.bntStartRecord.Name = "bntStartRecord";
      this.bntStartRecord.Size = new Size(92, 30);
      this.bntStartRecord.TabIndex = 3;
      this.bntStartRecord.Text = "StartRecord";
      this.bntStartRecord.UseVisualStyleBackColor = true;
      this.bntStartRecord.Click += new EventHandler(this.bntStartRecord_Click);
      this.cmbMacros.FormattingEnabled = true;
      this.cmbMacros.Location = new Point(32, 23);
      this.cmbMacros.Name = "cmbMacros";
      this.cmbMacros.Size = new Size(121, 21);
      this.cmbMacros.TabIndex = 2;
      this.cmbMacros.SelectedValueChanged += new EventHandler(this.cmbMacros_SelectedValueChanged);
      this.btnStop.Enabled = false;
      this.btnStop.Location = new Point(268, 17);
      this.btnStop.Name = "btnStop";
      this.btnStop.Size = new Size(92, 30);
      this.btnStop.TabIndex = 1;
      this.btnStop.Text = "Stop";
      this.btnStop.UseVisualStyleBackColor = true;
      this.btnStop.Click += new EventHandler(this.btnStop_Click);
      this.btnPlay.Location = new Point(159, 17);
      this.btnPlay.Name = "btnPlay";
      this.btnPlay.Size = new Size(92, 30);
      this.btnPlay.TabIndex = 0;
      this.btnPlay.Text = "Play";
      this.btnPlay.UseVisualStyleBackColor = true;
      this.btnPlay.Click += new EventHandler(this.btnPlay_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(1152, 790);
      this.Controls.Add((Control) this.splitContainer1);
      this.Name = nameof (MacroForm);
      this.Text = nameof (MacroForm);
      this.FormClosing += new FormClosingEventHandler(this.MacroForm_FormClosing);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
