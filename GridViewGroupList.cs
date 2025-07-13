// Decompiled with JetBrains decompiler
// Type: GridViewGroupList
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System.Collections.Generic;

// #nullable disable --- not for this version of C#
public class GridViewGroupList : List<GridViewGroup>
{
  public GridViewGroup this[string name] => this.FindGroupByName(name);

  public GridViewGroup FindGroupByName(string name)
  {
    foreach (GridViewGroup groupByName in (List<GridViewGroup>) this)
    {
      if (groupByName.Name.ToLower() == name.ToLower())
        return groupByName;
    }
    return (GridViewGroup) null;
  }
}
