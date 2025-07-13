// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Tools.SystemData
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.control;

#nullable disable
namespace TrackerDotNet.Tools;

public class SystemData : Page
{
  protected DetailsView dvSystemData;
  protected ObjectDataSource odsSystemData;

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void dvSystemData_ItemCommand(object sender, DetailsViewCommandEventArgs e)
  {
    if (!e.CommandName.Equals("Update"))
      return;
    DetailsViewRow row = this.dvSystemData.Rows[this.dvSystemData.DataItemIndex];
    CheckBox control1 = (CheckBox) row.FindControl("DoReoccuringOrdersCheckBox");
    TextBox control2 = (TextBox) row.FindControl("LastReoccurringDateTextBox");
    TextBox control3 = (TextBox) row.FindControl("DateLastPrepDateCalcdTextBox");
    TextBox control4 = (TextBox) row.FindControl("MinReminderDateTextBox");
    TextBox control5 = (TextBox) row.FindControl("GroupItemTypeIDTextBox");
    Label control6 = (Label) row.FindControl("IDLabel");
    SysDataTbl SysDataItem = new SysDataTbl();
    SysDataItem.ID = Convert.ToInt32(control6.Text);
    SysDataItem.DoReoccuringOrders = control1.Checked;
    SysDataItem.LastReoccurringDate = Convert.ToDateTime(control2.Text);
    SysDataItem.DateLastPrepDateCalcd = Convert.ToDateTime(control3.Text);
    SysDataItem.MinReminderDate = Convert.ToDateTime(control4.Text);
    SysDataItem.GroupItemTypeID = Convert.ToInt32(control5.Text);
    SysDataItem.Update(SysDataItem);
    this.dvSystemData.DataBind();
  }
}
