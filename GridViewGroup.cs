// Decompiled with JetBrains decompiler
// Type: GridViewGroup
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Web.UI;

// #nullable disable --- not for this version of C#
public class GridViewGroup
{
  private string[] _columns;
  private object[] _actualValues;
  private int _quantity;
  private bool _automatic;
  private bool _hideGroupColumns;
  private bool _isSuppressGroup;
  private bool _generateAllCellsOnSummaryRow;
  private GridViewSummaryList mSummaries;

  public string[] Columns => this._columns;

  public object[] ActualValues => this._actualValues;

  public int Quantity => this._quantity;

  public bool Automatic
  {
    get => this._automatic;
    set => this._automatic = value;
  }

  public bool HideGroupColumns
  {
    get => this._hideGroupColumns;
    set => this._hideGroupColumns = value;
  }

  public bool IsSuppressGroup => this._isSuppressGroup;

  public bool GenerateAllCellsOnSummaryRow
  {
    get => this._generateAllCellsOnSummaryRow;
    set => this._generateAllCellsOnSummaryRow = value;
  }

  public string Name => string.Join("+", this._columns);

  public GridViewSummaryList Summaries => this.mSummaries;

  public GridViewGroup(
    string[] cols,
    bool isSuppressGroup,
    bool auto,
    bool hideGroupColumns,
    bool generateAllCellsOnSummaryRow)
  {
    this.mSummaries = new GridViewSummaryList();
    this._actualValues = (object[]) null;
    this._quantity = 0;
    this._columns = cols;
    this._isSuppressGroup = isSuppressGroup;
    this._automatic = auto;
    this._hideGroupColumns = hideGroupColumns;
    this._generateAllCellsOnSummaryRow = generateAllCellsOnSummaryRow;
  }

  public GridViewGroup(
    string[] cols,
    bool auto,
    bool hideGroupColumns,
    bool generateAllCellsOnSummaryRow)
    : this(cols, false, auto, hideGroupColumns, generateAllCellsOnSummaryRow)
  {
  }

  public GridViewGroup(string[] cols, bool auto, bool hideGroupColumns)
    : this(cols, auto, hideGroupColumns, false)
  {
  }

  internal void SetActualValues(object[] values) => this._actualValues = values;

  public bool ContainsSummary(GridViewSummary s) => this.mSummaries.Contains(s);

  public void AddSummary(GridViewSummary s)
  {
    if (this.ContainsSummary(s))
      throw new Exception("Summary already exists in this group.");
    if (!s.Validate())
      throw new Exception("Invalid summary.");
    s.SetGroup(this);
    this.mSummaries.Add(s);
  }

  public void Reset()
  {
    this._quantity = 0;
    foreach (GridViewSummary mSummary in (List<GridViewSummary>) this.mSummaries)
      mSummary.Reset();
  }

  public void AddValueToSummaries(object dataitem)
  {
    ++this._quantity;
    foreach (GridViewSummary mSummary in (List<GridViewSummary>) this.mSummaries)
      mSummary.AddValue(DataBinder.Eval(dataitem, mSummary.Column));
  }

  public void CalculateSummaries()
  {
    foreach (GridViewSummary mSummary in (List<GridViewSummary>) this.mSummaries)
      mSummary.Calculate();
  }
}
