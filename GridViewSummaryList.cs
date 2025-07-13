// Decompiled with JetBrains decompiler
// Type: GridViewSummaryList
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System.Collections.Generic;

// #nullable disable --- not for this version of C#
public class GridViewSummaryList : List<GridViewSummary>
{
  public GridViewSummary this[string name] => this.FindSummaryByColumn(name);

  public GridViewSummary FindSummaryByColumn(string columnName)
  {
    foreach (GridViewSummary summaryByColumn in (List<GridViewSummary>) this)
    {
      if (summaryByColumn.Column.ToLower() == columnName.ToLower())
        return summaryByColumn;
    }
    return (GridViewSummary) null;
  }
}
