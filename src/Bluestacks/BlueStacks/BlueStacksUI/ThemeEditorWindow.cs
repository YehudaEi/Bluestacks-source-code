// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ThemeEditorWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueStacks.BlueStacksUI
{
  public class ThemeEditorWindow : CustomWindow, IComponentConnector
  {
    private string selectedItem = string.Empty;
    private bool isCreateDraftDirectory = true;
    private bool ignore = true;
    private string DraftDirectory = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Drafts");
    internal const string THUMBNAIL_ICON = "ThemeThumbnail.png";
    private static ThemeEditorWindow mInstance;
    private const string DraftFolderName = "Drafts";
    internal Slider sliderR;
    internal Slider sliderG;
    internal Slider sliderB;
    internal Slider sliderA;
    internal System.Windows.Controls.Label labelA;
    internal System.Windows.Controls.Label labelR;
    internal System.Windows.Controls.Label labelG;
    internal System.Windows.Controls.Label labelB;
    internal Slider sliderX;
    internal Slider sliderY;
    internal System.Windows.Controls.Label labelX;
    internal System.Windows.Controls.Label labelY;
    internal System.Windows.Controls.Label AppIcon;
    internal Slider tabangleX;
    internal System.Windows.Controls.Label AngleX;
    internal Slider tabangleY;
    internal System.Windows.Controls.Label AngleY;
    internal Slider topleftCornerRadius;
    internal System.Windows.Controls.Label top;
    internal Slider toprightcornerradius;
    internal System.Windows.Controls.Label left;
    internal Slider bottomleftCornerRadius;
    internal System.Windows.Controls.Label right;
    internal Slider bottomrightcornerradius;
    internal System.Windows.Controls.Label bottom;
    internal System.Windows.Controls.GroupBox groupBox1;
    internal System.Windows.Controls.RadioButton SearchTextBoxCurvature;
    internal System.Windows.Controls.RadioButton TabTransFormPortrait;
    internal System.Windows.Controls.RadioButton TabTransFormLandscape;
    internal System.Windows.Controls.TextBox textBox;
    internal Grid gridColor;
    internal System.Windows.Controls.Button btnLoad;
    internal System.Windows.Controls.Button btnSave;
    internal GroupByGrid dataGrid;
    internal GroupByGrid dataGrid1;
    internal System.Windows.Controls.ListBox ListView2;
    internal Image pictureBox;
    private bool _contentLoaded;

    public static ThemeEditorWindow Instance
    {
      get
      {
        if (ThemeEditorWindow.mInstance == null)
          ThemeEditorWindow.mInstance = new ThemeEditorWindow();
        return ThemeEditorWindow.mInstance;
      }
      set
      {
        ThemeEditorWindow.mInstance = value;
      }
    }

    public ThemeEditorWindow()
    {
      this.InitializeComponent();
      this.Closing += new CancelEventHandler(this.ThemeEditorWindow_Closing);
      this.Activated += new EventHandler(this.ThemeEditorWindow_Activated);
      this.sliderX.Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX;
      this.sliderY.Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY;
      this.TabTransFormLandscape.IsChecked = new bool?(true);
      this.ListView2.ItemsSource = (IEnumerable) BlueStacksUIBinding.Instance.ImageModel.Keys.ToList<string>();
      using (DataTable dataTable = new DataTable())
      {
        dataTable.Columns.Add(new DataColumn("Category", typeof (string)));
        dataTable.Columns.Add(new DataColumn("Name", typeof (string)));
        dataTable.Columns.Add(new DataColumn("Brush", typeof (Brush)));
        foreach (KeyValuePair<string, Brush> keyValuePair in (Dictionary<string, Brush>) BlueStacksUIColorManager.AppliedTheme.DictBrush)
        {
          DataRow row = dataTable.NewRow();
          row["Name"] = (object) keyValuePair.Key;
          row["Brush"] = (object) keyValuePair.Value;
          if (BlueStacksUIColorManager.AppliedTheme.DictCategory.ContainsKey(keyValuePair.Key))
            row["Category"] = (object) BlueStacksUIColorManager.AppliedTheme.DictCategory[keyValuePair.Key];
          dataTable.Rows.Add(row);
        }
        DataView defaultView = dataTable.DefaultView;
        defaultView.Sort = "Category asc";
        DataTable table = defaultView.ToTable();
        this.dataGrid.ColumnsToBeGrouped.Add(0);
        this.dataGrid.DataSource = (object) table;
        this.dataGrid.CellClick += new DataGridViewCellEventHandler(this.DataGrid_CellClick);
        this.dataGrid.CellValueChanged += new DataGridViewCellEventHandler(this.DataGrid_CellValueChanged);
        this.dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        this.dataGrid.Columns["Brush"].Visible = false;
      }
      using (DataTable dataTable = new DataTable())
      {
        dataTable.Columns.Add(new DataColumn("Name", typeof (string)));
        dataTable.Columns.Add(new DataColumn("CornerRadius", typeof (CornerRadius)));
        foreach (KeyValuePair<string, CornerRadius> dictCornerRadiu in (Dictionary<string, CornerRadius>) BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)
        {
          DataRow row = dataTable.NewRow();
          row["Name"] = (object) dictCornerRadiu.Key;
          row["CornerRadius"] = (object) dictCornerRadiu.Value;
          dataTable.Rows.Add(row);
        }
        this.dataGrid1.DataSource = (object) dataTable;
        this.dataGrid1.CellClick += new DataGridViewCellEventHandler(this.DataGrid1_CellClick);
        this.dataGrid1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
      }
      this.ignore = false;
    }

    private void DataGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      this.ignore = true;
      try
      {
        this.topleftCornerRadius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].TopLeft;
        this.toprightcornerradius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].TopRight;
        this.bottomleftCornerRadius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].BottomLeft;
        this.bottomrightcornerradius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].BottomRight;
      }
      catch (Exception ex)
      {
        Console.WriteLine("exception:" + ex.ToString());
      }
      this.ignore = false;
    }

    private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (!(this.dataGrid.Columns[e.ColumnIndex].Name == "Category"))
        return;
      BlueStacksUIColorManager.AppliedTheme.DictCategory[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] = this.dataGrid.Rows[e.RowIndex].Cells["Category"].Value.ToString();
      this.dataGrid.Sort(this.dataGrid.Columns[e.ColumnIndex], ListSortDirection.Ascending);
    }

    private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      this.ignore = true;
      try
      {
        this.sliderA.Value = (double) (BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.A;
        this.sliderR.Value = (double) (BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.R;
        this.sliderG.Value = (double) (BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.G;
        this.sliderB.Value = (double) (BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.B;
        this.textBox.Text = this.dataGrid.Rows[e.RowIndex].Cells["Brush"].Value.ToString();
      }
      catch (Exception ex)
      {
        Console.WriteLine("exception:" + ex.ToString());
      }
      this.ignore = false;
    }

    private void ThemeEditorWindow_Activated(object sender, EventArgs e)
    {
      if (!this.isCreateDraftDirectory)
        return;
      this.ListView2.ItemsSource = (IEnumerable) BlueStacksUIBinding.Instance.ImageModel.Keys.ToList<string>();
      this.isCreateDraftDirectory = false;
      ThemeEditorWindow.CopyEverything(CustomPictureBox.AssetsDir, this.DraftDirectory);
      File.Delete(Path.Combine(this.DraftDirectory, "ThemeThumbnail.png"));
    }

    private void ThemeEditorWindow_Closing(object sender, CancelEventArgs e)
    {
      this.isCreateDraftDirectory = true;
      e.Cancel = true;
      this.Hide();
    }

    private void Color_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      try
      {
        int num1 = (int) (byte) this.sliderA.Value;
        byte num2 = (byte) this.sliderR.Value;
        byte num3 = (byte) this.sliderG.Value;
        byte num4 = (byte) this.sliderB.Value;
        int num5 = (int) num2;
        int num6 = (int) num3;
        int num7 = (int) num4;
        System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb((byte) num1, (byte) num5, (byte) num6, (byte) num7);
        this.gridColor.Background = (Brush) new SolidColorBrush(color);
        if (this.ignore)
          return;
        this.textBox.Text = color.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.CurrentRow.Cells["Name"].Value.ToString()] = (Brush) new SolidColorBrush(new ColorUtils(color).WPFColor);
        if (this.dataGrid.CurrentRow.Cells["Category"].Value.ToString().Equals("*MainColors*", StringComparison.OrdinalIgnoreCase))
          BlueStacksUIColorManager.AppliedTheme.CalculateAndNotify(true);
        else
          BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
      }
      catch (Exception ex)
      {
      }
    }

    private void textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        if (this.ignore)
          return;
        SolidColorBrush solidColorBrush = new SolidColorBrush((System.Windows.Media.Color) ColorConverter.ConvertFromString(this.textBox.Text));
        this.sliderA.Value = (double) solidColorBrush.Color.A;
        this.sliderR.Value = (double) solidColorBrush.Color.R;
        this.sliderG.Value = (double) solidColorBrush.Color.G;
        this.sliderB.Value = (double) solidColorBrush.Color.B;
      }
      catch
      {
      }
    }

    private void Curve_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (this.ignore)
        return;
      BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX = this.sliderX.Value;
      BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY = this.sliderY.Value;
      BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
      {
        SelectedPath = RegistryManager.Instance.ClientInstallDir
      })
      {
        if (folderBrowserDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        string selectedPath = folderBrowserDialog.SelectedPath;
        if (File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "ThemeFile")))
        {
          string fileName = Path.GetFileName(folderBrowserDialog.SelectedPath);
          if (!folderBrowserDialog.SelectedPath.Contains(RegistryManager.Instance.ClientInstallDir))
          {
            string str = Path.Combine(RegistryManager.Instance.ClientInstallDir, fileName);
            if (Directory.Exists(str))
            {
              int num = (int) System.Windows.MessageBox.Show("Theme with this name already exists. Please rename the folder an try again");
            }
            else
            {
              Directory.CreateDirectory(str);
              ThemeEditorWindow.CopyEverything(folderBrowserDialog.SelectedPath, str);
            }
          }
          BlueStacksUIColorManager.ReloadAppliedTheme(fileName);
        }
        else
        {
          int num1 = (int) System.Windows.MessageBox.Show("Please select theme folder");
        }
      }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
      string str = Interaction.InputBox("Theme name", "BlueStacks Theme Editor Tool", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0:F}", (object) DateTime.Now), 0, 0);
      if (Directory.Exists(Path.Combine(RegistryManager.Instance.ClientInstallDir, str)))
      {
        int num = (int) System.Windows.MessageBox.Show("Already Exists. Please retry");
      }
      else
      {
        Directory.CreateDirectory(Path.Combine(RegistryManager.Instance.ClientInstallDir, str));
        ThemeEditorWindow.CopyEverything(this.DraftDirectory, Path.Combine(RegistryManager.Instance.ClientInstallDir, str));
        RegistryManager.Instance.SetClientThemeNameInRegistry(str);
        Window w = (Window) BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0];
        w.Dispatcher.Invoke((Delegate) (() =>
        {
          RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int) w.ActualWidth, (int) w.ActualHeight, 0.0, 0.0, PixelFormats.Pbgra32);
          renderTargetBitmap.Render((Visual) BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0]);
          PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
          pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource) renderTargetBitmap));
          using (Stream stream = (Stream) File.Create(Path.Combine(CustomPictureBox.AssetsDir, "ThemeThumbnail.png")))
            pngBitmapEncoder.Save(stream);
        }));
        BlueStacksUIColorManager.AppliedTheme.Save(BlueStacksUIColorManager.GetThemeFilePath(RegistryManager.ClientThemeName));
        CustomPictureBox.UpdateImagesFromNewDirectory("");
      }
    }

    private static void CopyEverything(string SourcePath, string DestinationPath)
    {
      foreach (string directory in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
        Directory.CreateDirectory(directory.Replace(SourcePath, DestinationPath));
      foreach (string file in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
        File.Copy(file, file.Replace(SourcePath, DestinationPath), true);
    }

    private void tabangle_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (this.ignore)
        return;
      if (this.SearchTextBoxCurvature.IsChecked.Value)
      {
        BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
        BlueStacksUIColorManager.AppliedTheme.TextBoxAntiTransForm = new SkewTransform(this.tabangleX.Value * -1.0, this.tabangleY.Value * -1.0);
        BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
      }
      else if (this.TabTransFormLandscape.IsChecked.Value)
      {
        BlueStacksUIColorManager.AppliedTheme.TabTransform = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
        BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
      }
      else
      {
        BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
        BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
      }
    }

    private void TabTransFormCheckedPortrait(object sender, RoutedEventArgs e)
    {
      this.ignore = true;
      this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX;
      this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY;
      this.ignore = false;
    }

    private void SearchTextBoxCurvatureChecked(object sender, RoutedEventArgs e)
    {
      this.ignore = true;
      this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleX;
      this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleY;
      this.ignore = false;
    }

    private void TabTransFormCheckedLandscape(object sender, RoutedEventArgs e)
    {
      this.ignore = true;
      this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX;
      this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY;
      this.ignore = false;
    }

    private void cornerRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (this.ignore)
        return;
      BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.CurrentRow.Cells["Name"].Value.ToString()] = new CornerRadius(this.topleftCornerRadius.Value, this.toprightcornerradius.Value, this.bottomrightcornerradius.Value, this.bottomleftCornerRadius.Value);
      try
      {
        this.dataGrid1[1, this.dataGrid1.CurrentRow.Index].Value = (object) new CornerRadius(this.topleftCornerRadius.Value, this.toprightcornerradius.Value, this.bottomrightcornerradius.Value, this.bottomleftCornerRadius.Value);
      }
      catch (Exception ex)
      {
      }
      BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.ListView2.SelectedItem == null)
        return;
      this.selectedItem = this.ListView2.SelectedItem.ToString();
      CustomPictureBox.SetBitmapImage(this.pictureBox, this.ListView2.SelectedItem.ToString(), false);
    }

    private void pictureBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (string.IsNullOrEmpty(this.selectedItem))
        return;
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      bool? nullable = openFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      string fileName = openFileDialog.FileName;
      string path = Path.Combine(this.DraftDirectory, this.selectedItem.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!File.Exists(path))
        path += ".png";
      string destFileName = path;
      File.Copy(fileName, destFileName, true);
      CustomPictureBox.UpdateImagesFromNewDirectory(this.DraftDirectory);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/themeeditorwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.sliderR = (Slider) target;
          this.sliderR.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Color_Changed);
          break;
        case 2:
          this.sliderG = (Slider) target;
          this.sliderG.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Color_Changed);
          break;
        case 3:
          this.sliderB = (Slider) target;
          this.sliderB.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Color_Changed);
          break;
        case 4:
          this.sliderA = (Slider) target;
          this.sliderA.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Color_Changed);
          break;
        case 5:
          this.labelA = (System.Windows.Controls.Label) target;
          break;
        case 6:
          this.labelR = (System.Windows.Controls.Label) target;
          break;
        case 7:
          this.labelG = (System.Windows.Controls.Label) target;
          break;
        case 8:
          this.labelB = (System.Windows.Controls.Label) target;
          break;
        case 9:
          this.sliderX = (Slider) target;
          this.sliderX.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Curve_Changed);
          break;
        case 10:
          this.sliderY = (Slider) target;
          this.sliderY.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Curve_Changed);
          break;
        case 11:
          this.labelX = (System.Windows.Controls.Label) target;
          break;
        case 12:
          this.labelY = (System.Windows.Controls.Label) target;
          break;
        case 13:
          this.AppIcon = (System.Windows.Controls.Label) target;
          break;
        case 14:
          this.tabangleX = (Slider) target;
          this.tabangleX.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.tabangle_Changed);
          break;
        case 15:
          this.AngleX = (System.Windows.Controls.Label) target;
          break;
        case 16:
          this.tabangleY = (Slider) target;
          this.tabangleY.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.tabangle_Changed);
          break;
        case 17:
          this.AngleY = (System.Windows.Controls.Label) target;
          break;
        case 18:
          this.topleftCornerRadius = (Slider) target;
          this.topleftCornerRadius.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.cornerRadiusChanged);
          break;
        case 19:
          this.top = (System.Windows.Controls.Label) target;
          break;
        case 20:
          this.toprightcornerradius = (Slider) target;
          this.toprightcornerradius.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.cornerRadiusChanged);
          break;
        case 21:
          this.left = (System.Windows.Controls.Label) target;
          break;
        case 22:
          this.bottomleftCornerRadius = (Slider) target;
          this.bottomleftCornerRadius.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.cornerRadiusChanged);
          break;
        case 23:
          this.right = (System.Windows.Controls.Label) target;
          break;
        case 24:
          this.bottomrightcornerradius = (Slider) target;
          this.bottomrightcornerradius.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.cornerRadiusChanged);
          break;
        case 25:
          this.bottom = (System.Windows.Controls.Label) target;
          break;
        case 26:
          this.groupBox1 = (System.Windows.Controls.GroupBox) target;
          break;
        case 27:
          this.SearchTextBoxCurvature = (System.Windows.Controls.RadioButton) target;
          this.SearchTextBoxCurvature.Checked += new RoutedEventHandler(this.SearchTextBoxCurvatureChecked);
          break;
        case 28:
          this.TabTransFormPortrait = (System.Windows.Controls.RadioButton) target;
          this.TabTransFormPortrait.Checked += new RoutedEventHandler(this.TabTransFormCheckedPortrait);
          break;
        case 29:
          this.TabTransFormLandscape = (System.Windows.Controls.RadioButton) target;
          this.TabTransFormLandscape.Checked += new RoutedEventHandler(this.TabTransFormCheckedLandscape);
          break;
        case 30:
          this.textBox = (System.Windows.Controls.TextBox) target;
          this.textBox.TextChanged += new TextChangedEventHandler(this.textBox_TextChanged);
          break;
        case 31:
          this.gridColor = (Grid) target;
          break;
        case 32:
          this.btnLoad = (System.Windows.Controls.Button) target;
          this.btnLoad.Click += new RoutedEventHandler(this.Load_Click);
          break;
        case 33:
          this.btnSave = (System.Windows.Controls.Button) target;
          this.btnSave.Click += new RoutedEventHandler(this.Save_Click);
          break;
        case 34:
          this.dataGrid = (GroupByGrid) target;
          break;
        case 35:
          this.dataGrid1 = (GroupByGrid) target;
          break;
        case 36:
          this.ListView2 = (System.Windows.Controls.ListBox) target;
          this.ListView2.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ListViewItem_PreviewMouseLeftButtonDown);
          break;
        case 37:
          this.pictureBox = (Image) target;
          this.pictureBox.MouseDown += new MouseButtonEventHandler(this.pictureBox_MouseDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
