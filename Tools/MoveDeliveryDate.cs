// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Tools.MoveDeliveryDate
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.control;

#nullable disable
namespace TrackerDotNet.Tools;

public class MoveDeliveryDate : Page
{
  protected ToolkitScriptManager scrmMoveDeliveryDate;
  protected UpdateProgress UpdateProgress1;
  protected UpdatePanel upnlMoveDeliveryDate;
  protected DropDownList OldDeliveryDateDDL;
  protected ObjectDataSource odsCityDeliveryDates;
  protected TextBox NewDeliveryDateTextBox;
  protected CalendarExtender NewDeliveryDateTextBox_CalendarExtender;
  protected Button btnMove;
  protected Button btnCancel;
  protected Literal StatusLiteral;
  protected GridView gvPrepData;
  protected SqlDataSource sdsCityPrepDates;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    this.NewDeliveryDateTextBox.Text = $"{DateTime.Now.AddDays(1.0).Date:d}";
  }

  protected void btnMove_Click(object sender, EventArgs e)
  {
    DateTime dateTime1 = Convert.ToDateTime(this.OldDeliveryDateDDL.SelectedValue);
    DateTime dateTime2 = Convert.ToDateTime(this.NewDeliveryDateTextBox.Text);
    if (dateTime1 == DateTime.MinValue || dateTime2 == DateTime.MinValue)
    {
      this.StatusLiteral.Text = "new and old dates must be valid";
    }
    else
    {
      NextRoastDateByCityTbl roastDateByCityTbl = new NextRoastDateByCityTbl();
      int pNumRecs = 0;
      string str = roastDateByCityTbl.MoveDeliveryDate(dateTime1, dateTime2, ref pNumRecs);
      if (string.IsNullOrEmpty(str))
      {
        foreach (int pNextRoastID in roastDateByCityTbl.GetAllIDsByDate(dateTime1))
        {
          str += roastDateByCityTbl.UpdateDeliveryDateByID(pNextRoastID, dateTime1);
          ++pNumRecs;
        }
        this.StatusLiteral.Text = string.IsNullOrEmpty(str) ? $"Move done: {pNumRecs}record(s) updated." : "ERROR: " + str;
        this.gvPrepData.DataBind();
        this.OldDeliveryDateDDL.DataBind();
      }
      else
        this.StatusLiteral.Text = "ERROR: " + str;
    }
  }
}
