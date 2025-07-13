// Decompiled with JetBrains decompiler
// Type: GridViewSummary
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;

// #nullable disable --- not for this version of C#
public class GridViewSummary
{
  private string _column;
  private SummaryOperation _operation;
  private CustomSummaryOperation _customOperation;
  private SummaryResultMethod _getSummaryMethod;
  private GridViewGroup _group;
  private object _value;
  private string _formatString;
  private int _quantity;
  private bool _automatic;
  private bool _treatNullAsZero;

  public string Column => this._column;

  public SummaryOperation Operation => this._operation;

  public CustomSummaryOperation CustomOperation => this._customOperation;

  public SummaryResultMethod GetSummaryMethod => this._getSummaryMethod;

  public GridViewGroup Group => this._group;

  public object Value => this._value;

  public string FormatString
  {
    get => this._formatString;
    set => this._formatString = value;
  }

  public int Quantity => this._quantity;

  public bool Automatic
  {
    get => this._automatic;
    set => this._automatic = value;
  }

  public bool TreatNullAsZero
  {
    get => this._treatNullAsZero;
    set => this._treatNullAsZero = value;
  }

  private GridViewSummary(string col, GridViewGroup grp)
  {
    this._column = col;
    this._group = grp;
    this._value = (object) null;
    this._quantity = 0;
    this._automatic = true;
    this._treatNullAsZero = false;
  }

  public GridViewSummary(string col, string formatString, SummaryOperation op, GridViewGroup grp)
    : this(col, grp)
  {
    this._formatString = formatString;
    this._operation = op;
    this._customOperation = (CustomSummaryOperation) null;
    this._getSummaryMethod = (SummaryResultMethod) null;
  }

  public GridViewSummary(string col, SummaryOperation op, GridViewGroup grp)
    : this(col, string.Empty, op, grp)
  {
  }

  public GridViewSummary(
    string col,
    string formatString,
    CustomSummaryOperation op,
    SummaryResultMethod getResult,
    GridViewGroup grp)
    : this(col, grp)
  {
    this._formatString = formatString;
    this._operation = SummaryOperation.Custom;
    this._customOperation = op;
    this._getSummaryMethod = getResult;
  }

  public GridViewSummary(
    string col,
    CustomSummaryOperation op,
    SummaryResultMethod getResult,
    GridViewGroup grp)
    : this(col, string.Empty, op, getResult, grp)
  {
  }

  internal void SetGroup(GridViewGroup g) => this._group = g;

  public bool Validate()
  {
    return this._operation == SummaryOperation.Custom ? this._customOperation != null && this._getSummaryMethod != null : this._customOperation == null && this._getSummaryMethod == null;
  }

  public void Reset()
  {
    this._quantity = 0;
    this._value = (object) null;
  }

  public void AddValue(object newValue)
  {
    ++this._quantity;
    if (this._operation == SummaryOperation.Sum || this._operation == SummaryOperation.Avg)
    {
      if (this._value == null)
        this._value = newValue;
      else
        this._value = this.PerformSum(this._value, newValue);
    }
    else
    {
      if (this._customOperation == null)
        return;
      if (this._group != null)
        this._customOperation(this._column, this._group.Name, newValue);
      else
        this._customOperation(this._column, (string) null, newValue);
    }
  }

  public void Calculate()
  {
    if (this._operation == SummaryOperation.Avg)
      this._value = this.PerformDiv(this._value, this._quantity);
    if (this._operation == SummaryOperation.Count)
    {
      this._value = (object) this._quantity;
    }
    else
    {
      if (this._operation != SummaryOperation.Custom || this._getSummaryMethod == null)
        return;
      this._value = this._getSummaryMethod(this._column, (string) null);
    }
  }

  private object PerformSum(object a, object b)
  {
    if (a == null)
    {
      if (!this._treatNullAsZero)
        return (object) null;
      a = (object) 0;
    }
    if (b == null)
    {
      if (!this._treatNullAsZero)
        return (object) null;
      b = (object) 0;
    }
    switch (a.GetType().FullName)
    {
      case "System.Int16":
        return (object) ((int) Convert.ToInt16(a) + (int) Convert.ToInt16(b));
      case "System.Int32":
        return (object) (Convert.ToInt32(a) + Convert.ToInt32(b));
      case "System.Int64":
        return (object) (Convert.ToInt64(a) + Convert.ToInt64(b));
      case "System.UInt16":
        return (object) ((int) Convert.ToUInt16(a) + (int) Convert.ToUInt16(b));
      case "System.UInt32":
        return (object) (uint) ((int) Convert.ToUInt32(a) + (int) Convert.ToUInt32(b));
      case "System.UInt64":
        return (object) (ulong) ((long) Convert.ToUInt64(a) + (long) Convert.ToUInt64(b));
      case "System.Single":
        return (object) (float) ((double) Convert.ToSingle(a) + (double) Convert.ToSingle(b));
      case "System.Double":
        return (object) (Convert.ToDouble(a) + Convert.ToDouble(b));
      case "System.Decimal":
        return (object) (Convert.ToDecimal(a) + Convert.ToDecimal(b));
      case "System.Byte":
        return (object) ((int) Convert.ToByte(a) + (int) Convert.ToByte(b));
      case "System.String":
        return (object) (a.ToString() + b.ToString());
      default:
        return (object) null;
    }
  }

  private object PerformDiv(object a, int b)
  {
    object obj = (object) 0;
    if (a == null)
      return !this._treatNullAsZero ? (object) null : obj;
    if (b == 0)
      return (object) null;
    switch (a.GetType().FullName)
    {
      case "System.Int16":
        return (object) ((int) Convert.ToInt16(a) / b);
      case "System.Int32":
        return (object) (Convert.ToInt32(a) / b);
      case "System.Int64":
        return (object) (Convert.ToInt64(a) / (long) b);
      case "System.UInt16":
        return (object) ((int) Convert.ToUInt16(a) / b);
      case "System.UInt32":
        return (object) ((long) Convert.ToUInt32(a) / (long) b);
      case "System.Single":
        return (object) (float) ((double) Convert.ToSingle(a) / (double) b);
      case "System.Double":
        return (object) (Convert.ToDouble(a) / (double) b);
      case "System.Decimal":
        return (object) (Convert.ToDecimal(a) / (Decimal) b);
      case "System.Byte":
        return (object) ((int) Convert.ToByte(a) / b);
      default:
        return (object) null;
    }
  }
}
