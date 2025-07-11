// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.Log
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public class Log : Page
    {
        private const string CONST_WHERECLAUSE_SESSIONVAR = "CustomerLogWhereFilter";
        private const int CONST_GVCOL_CONTACTNAME = 4;
        private const int CONST_GVCOL_JOBCARD = 5;
        private const int CONST_GVCOL_EQUIPMENT = 6;
        private const int CONST_GVCOL_MACHINESN = 7;
        private const int CONST_GVCOL_FAULT = 8;
        private const int CONST_GVCOL_FAULTDESC = 9;
        private const int CONST_GVCOL_ROID = 10;
        protected ScriptManager smLogSummary;
        protected UpdateProgress uprgLogSummary;
        protected UpdatePanel upnlSelection;
        protected DropDownList ddlFilterBy;
        protected TextBox tbxFilterBy;
        protected Button btnGo;
        protected Button btnReset;
        protected UpdatePanel upnlLogSummary;
        protected GridView gvLog;
        protected ObjectDataSource odsLogTbl;
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

        public string GetSectionDescription(int pSectionTypeID)
        {
            return pSectionTypeID > 0 ? new SectionTypesTbl().GetSectionTypeByID(pSectionTypeID) : string.Empty;
        }

        public string GetTransactionDescription(int pTransactionTypeID)
        {
            return pTransactionTypeID > 0 ? new TransactionTypesTbl().GetTransactionTypeByID(pTransactionTypeID) : string.Empty;
        }

        protected void gvLog_RowDataBound(object sender, GridViewRowEventArgs e)
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
            this.Session["CustomerLogWhereFilter"] = (object)"";
            this.ddlFilterBy.SelectedIndex = 0;
            this.tbxFilterBy.Text = "";
            this.odsLogTbl.DataBind();
        }

        protected void tbxFilterBy_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbxFilterBy.Text) || this.ddlFilterBy.SelectedIndex != 0)
                return;
            this.ddlFilterBy.SelectedIndex = 1;
            this.upnlSelection.Update();
        }

        public string GetPersonsNameFromID(string pPersonsID)
        {
            string personsNameFromId = "anon";
            int result = 0;
            if (!int.TryParse(pPersonsID, out result))
                result = 0;
            PersonsTbl personsTbl = new PersonsTbl();
            if (result > 0)
                personsNameFromId = personsTbl.PersonsNameFromID(result);
            return personsNameFromId;
        }

        public string GetSectionFromID(string pSectionID)
        {
            string sectionFromId = "n/a";
            int result = 0;
            if (!int.TryParse(pSectionID, out result))
                result = 0;
            SectionTypesTbl sectionTypesTbl = new SectionTypesTbl();
            if (result > 0)
                sectionFromId = sectionTypesTbl.GetSectionTypeByID(result);
            return sectionFromId;
        }

        public string GetTransactionFromID(string pTransactionID)
        {
            string transactionFromId = "n/a";
            int result = 0;
            if (!int.TryParse(pTransactionID, out result))
                result = 0;
            TransactionTypesTbl transactionTypesTbl = new TransactionTypesTbl();
            if (result > 0)
                transactionFromId = transactionTypesTbl.GetTransactionTypeByID(result);
            return transactionFromId;
        }

        public string GetCustomerFromID(string pCustomerID)
        {
            string customerFromId = "n/a";
            int result = 0;
            if (!int.TryParse(pCustomerID, out result))
                result = 0;
            CustomersTbl customersTbl = new CustomersTbl();
            if (result > 0)
                customersTbl.GetCustomersByCustomerID((long)result);
            return customerFromId;
        }
    }
}