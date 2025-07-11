// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.Customers
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public class Customers : Page
    {
        private const string CONST_WHERECLAUSE_SESSIONVAR = "CustomerSummaryWhereFilter";
        protected ScriptManager smCustomerSummary;
        protected UpdateProgress uprgCustomerSummary;
        protected UpdatePanel upnlSelection;
        protected DropDownList ddlFilterBy;
        protected TextBox tbxFilterBy;
        protected Button btnGon;
        protected Button btnReset;
        protected DropDownList ddlCustomerEnabled;
        protected UpdatePanel upnlCustomerSummary;
        protected GridView gvCustomers;
        protected ObjectDataSource odsCustomerSummarys;
        protected Label lblFilter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (this.Request.QueryString["CompanyName"] != null)
                {
                    this.tbxFilterBy.Text = this.Request.QueryString["CompanyName"].ToString();
                    this.ddlFilterBy.SelectedValue = "CompanyName";
                    this.Session["CustomerSummaryWhereFilter"] = (object)$"CompanyName LIKE '{this.tbxFilterBy.Text}%'";
                }
                else
                    this.Session["CustomerSummaryWhereFilter"] = (object)"";
                this.gvCustomers.Sort("CompanyName", SortDirection.Ascending);
            }
            else
            {
                if (this.Session["CustomerSummaryWhereFilter"] == null)
                    return;
                this.lblFilter.Text = this.Session["CustomerSummaryWhereFilter"].ToString();
            }
        }

        protected void btnGon_Click(object sender, EventArgs e)
        {
            if (!(this.ddlFilterBy.SelectedValue != "0") || string.IsNullOrWhiteSpace(this.tbxFilterBy.Text))
                return;
            this.Session["CustomerSummaryWhereFilter"] = (object)$"{this.ddlFilterBy.SelectedValue} LIKE '%{this.tbxFilterBy.Text}%'";
            this.odsCustomerSummarys.DataBind();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            this.Session["CustomerSummaryWhereFilter"] = (object)"";
            this.ddlFilterBy.SelectedIndex = 0;
            this.tbxFilterBy.Text = "";
            this.odsCustomerSummarys.DataBind();
        }

        protected void tbxFilterBy_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbxFilterBy.Text) || this.ddlFilterBy.SelectedIndex != 0)
                return;
            this.ddlFilterBy.SelectedIndex = 1;
            this.upnlSelection.Update();
        }
    }
}