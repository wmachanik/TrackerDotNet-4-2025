// Decompiled with JetBrains decompiler
// Type: GridViewHelper
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

// #nullable disable --- not for this version of C#
public class GridViewHelper
{
  private const string USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY = "Use adequate method to register a summary with custom operation.";
  private const string GROUP_NOT_FOUND = "Group {0} not found. Please register the group before the summary.";
  private const string INVALID_SUMMARY = "Invalid summary.";
  private const string SUPPRESS_GROUP_ALREADY_DEFINED = "A suppress group is already defined. You can't define suppress AND summary groups simultaneously";
  private const string ONE_GROUP_ALREADY_REGISTERED = "At least a group is already defined. A suppress group can't coexist with other groups";
  private GridView mGrid;
  private GridViewSummaryList mGeneralSummaries;
  private GridViewGroupList mGroups;
  private bool useFooter;
  private SortDirection groupSortDir;

  public GridViewGroupList Groups => this.mGroups;

  public GridViewSummaryList GeneralSummaries => this.mGeneralSummaries;

  public event GroupEvent GroupStart;

  public event GroupEvent GroupEnd;

  public event GroupEvent GroupHeader;

  public event GroupEvent GroupSummary;

  public event FooterEvent GeneralSummary;

  public event FooterEvent FooterDataBound;

  public GridViewHelper(GridView grd)
    : this(grd, false, SortDirection.Ascending)
  {
  }

  public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries)
    : this(grd, useFooterForGeneralSummaries, SortDirection.Ascending)
  {
  }

  public GridViewHelper(
    GridView grd,
    bool useFooterForGeneralSummaries,
    SortDirection groupSortDirection)
  {
    this.mGrid = grd;
    this.useFooter = useFooterForGeneralSummaries;
    this.groupSortDir = groupSortDirection;
    this.mGeneralSummaries = new GridViewSummaryList();
    this.mGroups = new GridViewGroupList();
    this.mGrid.RowDataBound += new GridViewRowEventHandler(this.RowDataBoundHandler);
  }

  public GridViewSummary RegisterSummary(string column, SummaryOperation operation)
  {
    return this.RegisterSummary(column, string.Empty, operation);
  }

  public GridViewSummary RegisterSummary(
    string column,
    string formatString,
    SummaryOperation operation)
  {
    if (operation == SummaryOperation.Custom)
      throw new Exception("Use adequate method to register a summary with custom operation.");
    GridViewSummary gridViewSummary = new GridViewSummary(column, formatString, operation, (GridViewGroup) null);
    this.mGeneralSummaries.Add(gridViewSummary);
    if (this.useFooter)
      this.mGrid.ShowFooter = true;
    return gridViewSummary;
  }

  public GridViewSummary RegisterSummary(
    string column,
    SummaryOperation operation,
    string groupName)
  {
    return this.RegisterSummary(column, string.Empty, operation, groupName);
  }

  public GridViewSummary RegisterSummary(
    string column,
    string formatString,
    SummaryOperation operation,
    string groupName)
  {
    if (operation == SummaryOperation.Custom)
      throw new Exception("Use adequate method to register a summary with custom operation.");
    GridViewGroup mGroup = this.mGroups[groupName];
    if (mGroup == null)
      throw new Exception($"Group {groupName} not found. Please register the group before the summary.");
    GridViewSummary s = new GridViewSummary(column, formatString, operation, mGroup);
    mGroup.AddSummary(s);
    return s;
  }

  public GridViewSummary RegisterSummary(
    string column,
    CustomSummaryOperation operation,
    SummaryResultMethod getResult)
  {
    return this.RegisterSummary(column, string.Empty, operation, getResult);
  }

  public GridViewSummary RegisterSummary(
    string column,
    string formatString,
    CustomSummaryOperation operation,
    SummaryResultMethod getResult)
  {
    GridViewSummary gridViewSummary = new GridViewSummary(column, formatString, operation, getResult, (GridViewGroup) null);
    this.mGeneralSummaries.Add(gridViewSummary);
    if (this.useFooter)
      this.mGrid.ShowFooter = true;
    return gridViewSummary;
  }

  public GridViewSummary RegisterSummary(
    string column,
    CustomSummaryOperation operation,
    SummaryResultMethod getResult,
    string groupName)
  {
    return this.RegisterSummary(column, string.Empty, operation, getResult, groupName);
  }

  public GridViewSummary RegisterSummary(
    string column,
    string formatString,
    CustomSummaryOperation operation,
    SummaryResultMethod getResult,
    string groupName)
  {
    GridViewGroup mGroup = this.mGroups[groupName];
    if (mGroup == null)
      throw new Exception($"Group {groupName} not found. Please register the group before the summary.");
    GridViewSummary s = new GridViewSummary(column, formatString, operation, getResult, mGroup);
    mGroup.AddSummary(s);
    return s;
  }

  public GridViewSummary RegisterSummary(GridViewSummary s)
  {
    if (!s.Validate())
      throw new Exception("Invalid summary.");
    if (s.Group == null)
    {
      if (this.useFooter)
        this.mGrid.ShowFooter = true;
      this.mGeneralSummaries.Add(s);
    }
    else if (!s.Group.ContainsSummary(s))
      s.Group.AddSummary(s);
    return s;
  }

  public GridViewGroup RegisterGroup(string column, bool auto, bool hideGroupColumns)
  {
    return this.RegisterGroup(new string[1]{ column }, auto, hideGroupColumns);
  }

  public GridViewGroup RegisterGroup(string[] columns, bool auto, bool hideGroupColumns)
  {
    if (this.HasSuppressGroup())
      throw new Exception("A suppress group is already defined. You can't define suppress AND summary groups simultaneously");
    GridViewGroup gridViewGroup = new GridViewGroup(columns, auto, hideGroupColumns);
    this.mGroups.Add(gridViewGroup);
    if (hideGroupColumns)
    {
      for (int index1 = 0; index1 < this.mGrid.Columns.Count; ++index1)
      {
        for (int index2 = 0; index2 < columns.Length; ++index2)
        {
          if (this.GetDataFieldName(this.mGrid.Columns[index1]).ToLower() == columns[index2].ToLower())
            this.mGrid.Columns[index1].Visible = false;
        }
      }
    }
    return gridViewGroup;
  }

  public GridViewGroup SetSuppressGroup(string column)
  {
    return this.SetSuppressGroup(new string[1]{ column });
  }

  public GridViewGroup SetSuppressGroup(string[] columns)
  {
    if (this.mGroups.Count > 0)
      throw new Exception("At least a group is already defined. A suppress group can't coexist with other groups");
    GridViewGroup gridViewGroup = new GridViewGroup(columns, true, false, false, false);
    this.mGroups.Add(gridViewGroup);
    this.mGrid.AllowPaging = false;
    return gridViewGroup;
  }

  private string GetSequentialGroupColumns()
  {
    string str = string.Empty;
    foreach (GridViewGroup mGroup in (List<GridViewGroup>) this.mGroups)
      str = $"{str}{mGroup.Name.Replace('+', ',')},";
    return str.Substring(0, str.Length - 1);
  }

  private bool EvaluateEquals(GridViewGroup g, object dataitem)
  {
    if (g.ActualValues == null)
      return false;
    for (int index = 0; index < g.Columns.Length; ++index)
    {
      if (g.ActualValues[index] == null && DataBinder.Eval(dataitem, g.Columns[index]) != null || g.ActualValues[index] != null && DataBinder.Eval(dataitem, g.Columns[index]) == null || !g.ActualValues[index].Equals(DataBinder.Eval(dataitem, g.Columns[index])))
        return false;
    }
    return true;
  }

  private bool HasSuppressGroup()
  {
    foreach (GridViewGroup mGroup in (List<GridViewGroup>) this.mGroups)
    {
      if (mGroup.IsSuppressGroup)
        return true;
    }
    return false;
  }

  private bool HasAutoSummary(List<GridViewSummary> list)
  {
    foreach (GridViewSummary gridViewSummary in list)
    {
      if (gridViewSummary.Automatic)
        return true;
    }
    return false;
  }

  private object[] GetGroupRowValues(GridViewGroup g, object dataitem)
  {
    object[] groupRowValues = new object[g.Columns.Length];
    for (int index = 0; index < g.Columns.Length; ++index)
      groupRowValues[index] = DataBinder.Eval(dataitem, g.Columns[index]);
    return groupRowValues;
  }

  private GridViewRow InsertGridRow(GridViewRow beforeRow, GridViewGroup g)
  {
    int visibleColumnCount = this.GetVisibleColumnCount();
    Table control = (Table) this.mGrid.Controls[0];
    int rowIndex = control.Rows.GetRowIndex((TableRow) beforeRow);
    GridViewRow child = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
    TableCell[] tableCellArray;
    if (g != null && (g.IsSuppressGroup || g.GenerateAllCellsOnSummaryRow))
    {
      tableCellArray = new TableCell[visibleColumnCount];
      for (int visibleIndex = 0; visibleIndex < visibleColumnCount; ++visibleIndex)
      {
        TableCell tableCell = new TableCell();
        tableCell.ApplyStyle((Style) this.mGrid.Columns[this.GetRealIndexFromVisibleColumnIndex(visibleIndex)].ItemStyle);
        tableCell.Text = "&nbsp;";
        tableCellArray[visibleIndex] = tableCell;
      }
    }
    else
    {
      int num = 0;
      List<TableCell> tableCellList = new List<TableCell>();
      for (int index = 0; index < this.mGrid.Columns.Count; ++index)
      {
        if (this.ColumnHasSummary(index, g))
        {
          if (num > 0)
          {
            tableCellList.Add(new TableCell()
            {
              Text = "&nbsp;",
              ColumnSpan = num
            });
            num = 0;
          }
          TableCell tableCell = new TableCell();
          tableCell.ApplyStyle((Style) this.mGrid.Columns[index].ItemStyle);
          tableCellList.Add(tableCell);
        }
        else if (this.mGrid.Columns[index].Visible)
          ++num;
      }
      if (num > 0)
        tableCellList.Add(new TableCell()
        {
          Text = "&nbsp;",
          ColumnSpan = num
        });
      tableCellArray = new TableCell[tableCellList.Count];
      tableCellList.CopyTo(tableCellArray);
    }
    child.Cells.AddRange(tableCellArray);
    control.Controls.AddAt(rowIndex, (Control) child);
    return child;
  }

  private void RowDataBoundHandler(object sender, GridViewRowEventArgs e)
  {
    foreach (GridViewGroup mGroup in (List<GridViewGroup>) this.mGroups)
    {
      if (e.Row.RowType == DataControlRowType.Footer)
      {
        mGroup.CalculateSummaries();
        this.GenerateGroupSummary(mGroup, e.Row);
        if (this.GroupEnd != null)
          this.GroupEnd(mGroup.Name, mGroup.ActualValues, e.Row);
      }
      else if (e.Row.RowType == DataControlRowType.DataRow)
      {
        this.ProcessGroup(mGroup, e);
        if (mGroup.IsSuppressGroup)
          e.Row.Visible = false;
      }
      else if (e.Row.RowType == DataControlRowType.Pager)
      {
        TableCell cell1 = e.Row.Cells[0];
        TableCell cell2 = new TableCell();
        cell2.Visible = false;
        e.Row.Cells.AddAt(0, cell2);
        cell1.ColumnSpan = this.GetVisibleColumnCount();
      }
    }
    foreach (GridViewSummary mGeneralSummary in (List<GridViewSummary>) this.mGeneralSummaries)
    {
      if (e.Row.RowType == DataControlRowType.Header)
        mGeneralSummary.Reset();
      else if (e.Row.RowType == DataControlRowType.DataRow)
        mGeneralSummary.AddValue(DataBinder.Eval(e.Row.DataItem, mGeneralSummary.Column));
      else if (e.Row.RowType == DataControlRowType.Footer)
        mGeneralSummary.Calculate();
    }
    if (e.Row.RowType != DataControlRowType.Footer)
      return;
    this.GenerateGeneralSummaries(e);
    if (this.FooterDataBound == null)
      return;
    this.FooterDataBound(e.Row);
  }

  private void ProcessGroup(GridViewGroup g, GridViewRowEventArgs e)
  {
    string empty = string.Empty;
    if (!this.EvaluateEquals(g, e.Row.DataItem))
    {
      if (g.ActualValues != null)
      {
        g.CalculateSummaries();
        this.GenerateGroupSummary(g, e.Row);
        if (this.GroupEnd != null)
          this.GroupEnd(g.Name, g.ActualValues, e.Row);
      }
      g.Reset();
      g.SetActualValues(this.GetGroupRowValues(g, e.Row.DataItem));
      if (g.Automatic)
      {
        for (int index = 0; index < g.ActualValues.Length; ++index)
        {
          if (g.ActualValues[index] != null)
          {
            empty += g.ActualValues[index].ToString();
            if (g.ActualValues.Length - index > 1)
              empty += " - ";
          }
        }
        GridViewRow row = this.InsertGridRow(e.Row);
        row.Cells[0].Text = empty;
        if (this.GroupHeader != null)
          this.GroupHeader(g.Name, g.ActualValues, row);
      }
      if (this.GroupStart != null)
        this.GroupStart(g.Name, g.ActualValues, e.Row);
    }
    g.AddValueToSummaries(e.Row.DataItem);
  }

  private string GetFormatedString(string preferredFormat, string secondFormat, object value)
  {
    string format = preferredFormat;
    if (format.Length == 0)
      format = secondFormat;
    return format.Length > 0 ? string.Format(format, value) : value.ToString();
  }

  private void GenerateGroupSummary(GridViewGroup g, GridViewRow row)
  {
    if (!this.HasAutoSummary((List<GridViewSummary>) g.Summaries) && !this.HasSuppressGroup())
      return;
    GridViewRow row1 = this.InsertGridRow(row, g);
    foreach (GridViewSummary summary in (List<GridViewSummary>) g.Summaries)
    {
      if (summary.Automatic)
      {
        int visibleColumnIndex = this.GetVisibleColumnIndex(summary.Column);
        int index = this.ResolveCellIndex(row1, visibleColumnIndex);
        row1.Cells[index].Text = this.GetFormatedString(summary.FormatString, this.GetColumnFormat(this.GetColumnIndex(summary.Column)), summary.Value);
      }
    }
    if (g.IsSuppressGroup)
    {
      for (int index1 = 0; index1 < g.Columns.Length; ++index1)
      {
        object actualValue = g.ActualValues[index1];
        if (actualValue != null)
        {
          int visibleColumnIndex = this.GetVisibleColumnIndex(g.Columns[index1]);
          int index2 = this.ResolveCellIndex(row1, visibleColumnIndex);
          row1.Cells[index2].Text = actualValue.ToString();
        }
      }
    }
    if (this.GroupSummary == null)
      return;
    this.GroupSummary(g.Name, g.ActualValues, row1);
  }

  private void GenerateGeneralSummaries(GridViewRowEventArgs e)
  {
    if (!this.HasAutoSummary((List<GridViewSummary>) this.mGeneralSummaries))
    {
      if (this.GeneralSummary == null)
        return;
      this.GeneralSummary(e.Row);
    }
    else
    {
      GridViewRow row = !this.useFooter ? this.InsertGridRow(e.Row, (GridViewGroup) null) : e.Row;
      foreach (GridViewSummary mGeneralSummary in (List<GridViewSummary>) this.mGeneralSummaries)
      {
        if (mGeneralSummary.Automatic)
        {
          int colIndex = !this.useFooter ? this.GetVisibleColumnIndex(mGeneralSummary.Column) : this.GetColumnIndex(mGeneralSummary.Column);
          int index = this.ResolveCellIndex(row, colIndex);
          row.Cells[index].Text = this.GetFormatedString(mGeneralSummary.FormatString, this.GetColumnFormat(this.GetColumnIndex(mGeneralSummary.Column)), mGeneralSummary.Value);
        }
      }
      if (this.GeneralSummary == null)
        return;
      this.GeneralSummary(row);
    }
  }

  private int ResolveCellIndex(GridViewRow row, int colIndex)
  {
    int num = 0;
    for (int index = 0; index < row.Cells.Count; ++index)
    {
      if (index + num == colIndex)
        return index;
      if (row.Cells[index].ColumnSpan > 1)
        num = num + row.Cells[index].ColumnSpan - 1;
    }
    return -1;
  }

  private bool ColumnHasSummary(int colindex, GridViewGroup g)
  {
    string dataFieldName = this.GetDataFieldName(this.mGrid.Columns[colindex]);
    foreach (GridViewSummary gridViewSummary in g != null ? (List<GridViewSummary>) g.Summaries : (List<GridViewSummary>) this.mGeneralSummaries)
    {
      if (dataFieldName.ToLower() == gridViewSummary.Column.ToLower())
        return true;
    }
    return false;
  }

  private bool ColumnHasSummary(string column, GridViewGroup g)
  {
    foreach (GridViewSummary gridViewSummary in g != null ? (List<GridViewSummary>) g.Summaries : (List<GridViewSummary>) this.mGeneralSummaries)
    {
      if (column.ToLower() == gridViewSummary.Column.ToLower())
        return true;
    }
    return false;
  }

  public int GetRealIndexFromVisibleColumnIndex(int visibleIndex)
  {
    int num = 0;
    for (int index = 0; index < this.mGrid.Columns.Count; ++index)
    {
      if (this.mGrid.Columns[index].Visible)
      {
        if (visibleIndex == num)
          return index;
        ++num;
      }
    }
    return -1;
  }

  public int GetVisibleColumnIndex(string columnName)
  {
    int visibleColumnIndex = 0;
    for (int index = 0; index < this.mGrid.Columns.Count; ++index)
    {
      if (this.GetDataFieldName(this.mGrid.Columns[index]).ToLower() == columnName.ToLower())
        return visibleColumnIndex;
      if (this.mGrid.Columns[index].Visible)
        ++visibleColumnIndex;
    }
    return -1;
  }

  public int GetColumnIndex(string columnName)
  {
    for (int index = 0; index < this.mGrid.Columns.Count; ++index)
    {
      if (this.GetDataFieldName(this.mGrid.Columns[index]).ToLower() == columnName.ToLower())
        return index;
    }
    return -1;
  }

  public string GetDataFieldName(DataControlField field)
  {
    return field is BoundField ? (field as BoundField).DataField : field.SortExpression;
  }

  public string GetColumnFormat(int colIndex)
  {
    return this.mGrid.Columns[colIndex] is BoundField ? (this.mGrid.Columns[colIndex] as BoundField).DataFormatString : string.Empty;
  }

  public int GetVisibleColumnCount()
  {
    int visibleColumnCount = 0;
    for (int index = 0; index < this.mGrid.Columns.Count; ++index)
    {
      if (this.mGrid.Columns[index].Visible)
        ++visibleColumnCount;
    }
    return visibleColumnCount;
  }

  public void HideColumnsWithoutGroupSummary()
  {
    foreach (DataControlField column in (StateManagedCollection) this.mGrid.Columns)
    {
      bool flag = false;
      string lower = this.GetDataFieldName(column).ToLower();
      foreach (GridViewGroup mGroup in (List<GridViewGroup>) this.mGroups)
      {
        for (int index = 0; index < mGroup.Columns.Length; ++index)
        {
          if (lower == mGroup.Columns[index].ToLower())
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          flag = this.ColumnHasSummary(lower, mGroup);
          if (flag)
            break;
        }
        else
          break;
      }
      if (!flag)
        column.Visible = false;
    }
  }

  public void SetInvisibleColumnsWithoutGroupSummary() => this.HideColumnsWithoutGroupSummary();

  public GridViewRow InsertGridRow(GridViewRow beforeRow)
  {
    int visibleColumnCount = this.GetVisibleColumnCount();
    Table control = (Table) this.mGrid.Controls[0];
    int rowIndex = control.Rows.GetRowIndex((TableRow) beforeRow);
    GridViewRow child = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
    child.Cells.Add(new TableCell());
    if (visibleColumnCount > 1)
      child.Cells[0].ColumnSpan = visibleColumnCount;
    control.Controls.AddAt(rowIndex, (Control) child);
    return child;
  }

  public void ApplyGroupSort()
  {
    this.mGrid.Sort(this.GetSequentialGroupColumns(), this.groupSortDir);
  }
}
