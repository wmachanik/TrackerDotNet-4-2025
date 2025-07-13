using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackerDotNet.Tools
{
  public partial class SystemData : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {

      

    }
    protected void dvSystemData_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
      if (e.CommandName.Equals("Update"))
      {
        DetailsViewRow _EdittedRow = dvSystemData.Rows[dvSystemData.DataItemIndex];

        CheckBox _DoReoccuringOrdersCheckBox = (CheckBox)_EdittedRow.FindControl("DoReoccuringOrdersCheckBox");
        TextBox _LastReoccurringDateTextBox = (TextBox)_EdittedRow.FindControl("LastReoccurringDateTextBox");
        TextBox _DateLastPrepDateCalcdTextBox = (TextBox)_EdittedRow.FindControl("DateLastPrepDateCalcdTextBox");
        TextBox _MinReminderDateTextBox = (TextBox)_EdittedRow.FindControl("MinReminderDateTextBox");
        TextBox _GroupItemTypeIDTextBox = (TextBox)_EdittedRow.FindControl("GroupItemTypeIDTextBox");
        Label _IDLabel = (Label)_EdittedRow.FindControl("IDLabel");

        control.SysDataTbl _SysData = new control.SysDataTbl();
        _SysData.ID = Convert.ToInt32(_IDLabel.Text);
        _SysData.DoReoccuringOrders = _DoReoccuringOrdersCheckBox.Checked;
        _SysData.LastReoccurringDate = Convert.ToDateTime(_LastReoccurringDateTextBox.Text);
        _SysData.DateLastPrepDateCalcd = Convert.ToDateTime(_DateLastPrepDateCalcdTextBox.Text);
        _SysData.MinReminderDate = Convert.ToDateTime(_MinReminderDateTextBox.Text);
        _SysData.GroupItemTypeID = Convert.ToInt32(_GroupItemTypeIDTextBox.Text);
        _SysData.Update(_SysData);

        dvSystemData.DataBind();
      }
    }
  }
}