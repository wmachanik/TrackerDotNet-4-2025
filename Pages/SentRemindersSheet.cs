// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.SentRemindersSheet
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.control;

#nullable disable
namespace TrackerDotNet.Pages;

public class SentRemindersSheet : Page
{
  private const string CONST_URL_REQUEST_LASTSENTDATE = "LastSentDate";
  protected ToolkitScriptManager smSentRemindersSummary;
  protected UpdateProgress uprgSentRemindersSummary;
  protected UpdatePanel upnlSelection;
  protected DropDownList ddlFilterByDate;
  protected UpdatePanel upnlSentRemindersList;
  protected GridView gvSentReminders;
  protected ObjectDataSource odsSentRemindersSummarys;
  protected ObjectDataSource odsDatesSentReminder;
  protected UpdatePanel UpdatePanel1;
  protected Label lblFilter;

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void Page_PreRenderComplete(object sender, EventArgs e)
  {
    if (this.IsPostBack || this.Request.QueryString.Count <= 0 || this.Request.QueryString["LastSentDate"] == null)
      return;
    string str = $"{Convert.ToDateTime(this.Request.QueryString["LastSentDate"]):d}";
    if (this.ddlFilterByDate.Items.FindByValue(str) == null)
      return;
    this.ddlFilterByDate.SelectedValue = str;
    this.gvSentReminders.DataBind();
    this.upnlSentRemindersList.Update();
  }

  public string GetCompanyName(long pCompanyID)
  {
    return pCompanyID > 0L ? new CompanyNames().GetCompanyNameByCompanyID(pCompanyID) : string.Empty;
  }
}
