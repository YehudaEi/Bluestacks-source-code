// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GroupByGrid
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace BlueStacks.BlueStacksUI
{
  public class GroupByGrid : DataGridView
  {
    internal List<int> ColumnsToBeGrouped = new List<int>();

    protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
    {
      base.OnCellFormatting(args);
      if (args == null || args.RowIndex == 0 || !this.IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
        return;
      args.Value = (object) string.Empty;
      args.FormattingApplied = true;
    }

    private bool IsRepeatedCellValue(int rowIndex, int colIndex)
    {
      DataGridViewCell cell1 = this.Rows[rowIndex].Cells[colIndex];
      DataGridViewCell cell2 = this.Rows[rowIndex - 1].Cells[colIndex];
      return this.ColumnsToBeGrouped.Contains(colIndex) && (cell1.Value == cell2.Value || cell1.Value != null && cell2.Value != null && cell1.Value.ToString() == cell2.Value.ToString());
    }

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs args)
    {
      base.OnCellPainting(args);
      if (args == null)
        return;
      args.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
      if (args.RowIndex < 1 || args.ColumnIndex < 0)
        return;
      if (this.IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
        args.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
      else
        args.AdvancedBorderStyle.Top = this.AdvancedCellBorderStyle.Top;
    }
  }
}
