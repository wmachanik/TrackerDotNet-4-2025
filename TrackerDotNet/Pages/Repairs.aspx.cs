// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.Repairs
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.Pages
{

    public class Repairs : Page
    {
        private const string CONST_WHERECLAUSE_SESSIONVAR = "CustomerRepairWhereFilter";
        private const int CONST_GVCOL_CONTACTNAME = 4;
        private const int CONST_GVCOL_JOBCARD = 5;
        private const int CONST_GVCOL_EQUIPMENT = 6;
        private const int CONST_GVCOL_MACHINESN = 7;
        private const int CONST_GVCOL_FAULT = 8;
        private const int CONST_GVCOL_FAULTDESC = 9;
        private const int CONST_GVCOL_ROID = 10;
        protected ToolkitScriptManager smRepairsSummary;
        protected UpdateProgress uprgRepairsSummary;
        protected UpdatePanel upnlSelection;
        protected DropDownList ddlFilterBy;
        protected TextBox tbxFilterBy;
        protected Button btnGo;
        protected Button btnReset;
        protected DropDownList ddlRepairStatus;
        protected HyperLink hlAddRepair;
        protected UpdatePanel upnlRepairsSummary;
        protected GridView gvRepairs;
        protected ObjectDataSource odsRepairs;
        protected ObjectDataSource odsRepairsStatuses;
        protected Label lblFilter;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            bool flag = new CheckBrowser().fBrowserIsMobile();
            this.Session["RunningOnMoble"] = (object)flag;
            if (flag)
                this.MasterPageFile = "~/MobileSite.master";
            else
                this.MasterPageFile = "~/Site.master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack || !(bool)this.Session["RunningOnMoble"])
                return;
            this.tbxFilterBy.Width = new Unit(8.0, UnitType.Em);
        }

        public string GetCompanyName(long pCompanyID)
        {
            return pCompanyID > 0L ? new CompanyNames().GetCompanyNameByCompanyID(pCompanyID) : string.Empty;
        }

        public string GetMachineDesc(int pEquipID)
        {
            return pEquipID > 0 ? new EquipTypeTbl().GetEquipName(pEquipID) : string.Empty;
        }

        public string GetRepairFaultDesc(int pRepairFaultID)
        {
            return pRepairFaultID > 0 ? new RepairFaultsTbl().GetRepairFaultDesc(pRepairFaultID) : string.Empty;
        }

        public string GetRepairStatusDesc(int pRepairStatusID)
        {
            return pRepairStatusID > 0 ? new RepairStatusesTbl().GetRepairStatusDesc(pRepairStatusID) : string.Empty;
        }

        protected void gvRepairs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!(bool)this.Session["RunningOnMoble"])
                return;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[6].Visible = false;
            e.Row.Cells[8].Visible = false;
            e.Row.Cells[9].Visible = false;
            e.Row.Cells[10].Visible = false;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (!(this.ddlFilterBy.SelectedValue != "0"))
                return;
            string.IsNullOrWhiteSpace(this.tbxFilterBy.Text);
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            this.Session["CustomerRepairWhereFilter"] = (object)"";
            this.ddlFilterBy.SelectedIndex = 0;
            this.tbxFilterBy.Text = "";
            this.odsRepairs.DataBind();
        }

        protected void tbxFilterBy_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbxFilterBy.Text) || this.ddlFilterBy.SelectedIndex != 0)
                return;
            this.ddlFilterBy.SelectedIndex = 1;
            this.upnlSelection.Update();
        }

        protected void ddlRepairStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.odsRepairs.DataBind();
            this.upnlRepairsSummary.Update();
        }
    }
}

