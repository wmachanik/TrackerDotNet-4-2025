// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.OrderDone
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.BusinessLogic;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public partial class OrderDone : Page
    {
        protected ScriptManager smOrderDone;
        protected UpdateProgress updtprgOrderDone;
        protected UpdatePanel updtpnlOrderDone;
        protected Panel pnlOrderDetails;
        protected FormView fvOrderDone;
        protected GridView gvOrderDoeLines;
        protected Button btnDone;
        protected Button btnCancel;
        protected TextBox tbxStock;
        protected RadioButtonList rbtnSendConfirm;
        protected TextBox tbxCount;
        protected Literal ltrlStatus;
        protected Panel pnlCustomerDetailsUpdated;
        protected Label tbxCustomerName;
        protected DataGrid dgCustomerUsage;
        protected Button btnReturnToDeliveres;
        protected SqlDataSource sdsOrderDoneHeader;
        protected SqlDataSource sdsOrderDoneLines;
        protected SqlDataSource sdsItemTypes;
        protected SqlDataSource sdsPackagingTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sdsOrderDoneHeader.Select(DataSourceSelectArguments.Empty);
                fvOrderDone.ChangeMode(FormViewMode.Edit);
            }
        }
        private bool TempOrderExists(string customerID)
        {
            string query = "SELECT COUNT(*) FROM TempOrdersHeaderTbl WHERE CustomerID = @CustomerID";

            using (var conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString))
            using (var cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        protected void ShowResults(string CustomerName, long pCustomerID, ClientUsageTbl pOriginalUsageData) 
        {
            this.pnlOrderDetails.Visible = false;
            this.tbxCustomerName.Text = CustomerName;
            List<ClientUsageTbl> clientUsageTblList = new List<ClientUsageTbl>();
            clientUsageTblList.Add(pOriginalUsageData);
            clientUsageTblList.Add(new ClientUsageTbl().GetUsageData(pCustomerID));
            this.dgCustomerUsage.AutoGenerateColumns = false;
            this.dgCustomerUsage.DataSource = (object)clientUsageTblList;
            this.dgCustomerUsage.DataBind();
            this.pnlCustomerDetailsUpdated.Visible = true;
        }

        //private bool SendDeliveredEmail(long pCustomerID, string pMessage)
        //{
        //    bool flag = false;
        //    CustomersTbl customersByCustomerID = new CustomersTbl().GetCustomerByCustomerID(pCustomerID);
        //    if (customersByCustomerID.EmailAddress.Contains("@") || customersByCustomerID.AltEmailAddress.Contains("@"))
        //    {
        //        string empty = string.Empty;
        //        EmailCls emailCls = new EmailCls();
        //        emailCls.SetLegacyEmailSubject("Confirmation email");
        //        string pObj1;
        //        if (customersByCustomerID.EmailAddress.Contains("@"))
        //        {
        //            emailCls.SetLegacyEmailTo(customersByCustomerID.EmailAddress);
        //            if (customersByCustomerID.AltEmailAddress.Contains("@"))
        //            {
        //                emailCls.SetLegacyEmailCC(customersByCustomerID.AltEmailAddress);
        //                string str = !string.IsNullOrEmpty(customersByCustomerID.ContactFirstName) ? customersByCustomerID.ContactFirstName : string.Empty;
        //                if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(customersByCustomerID.ContactAltFirstName))
        //                    str += " and ";
        //                pObj1 = str + (string.IsNullOrEmpty(customersByCustomerID.ContactAltFirstName) ? customersByCustomerID.ContactAltFirstName : string.Empty);
        //            }
        //            else
        //            {
        //                emailCls.SetLegacyEmailTo(customersByCustomerID.AltEmailAddress);
        //                pObj1 = empty + (string.IsNullOrEmpty(customersByCustomerID.ContactAltFirstName) ? customersByCustomerID.ContactAltFirstName : string.Empty);
        //            }
        //        }
        //        else
        //        {
        //            emailCls.SetLegacyEmailTo(customersByCustomerID.AltEmailAddress);
        //            pObj1 = empty + (string.IsNullOrEmpty(customersByCustomerID.ContactAltFirstName) ? customersByCustomerID.ContactAltFirstName : string.Empty);
        //        }
        //        if (string.IsNullOrEmpty(pObj1))
        //            pObj1 = "coffee lover";
        //        emailCls.AddFormatToLegacyEmailBody("To {0},<br />,<br />", (object)pObj1);
        //        emailCls.AddStrAndNewLineToLegacyEmailBody($"Just a quick note to notify you that Quaffee has {pMessage}<br />");
        //        emailCls.AddStrAndNewLineToLegacyEmailBody("Thank you for your support.<br />");
        //        emailCls.AddStrAndNewLineToLegacyEmailBody("Sincerely Quaffee Team (orders@quaffee.co.za)");
        //        flag = emailCls.SendLegacyEmail();
        //    }
        //    return flag;
        //}

        protected void btnDone_Click(object sender, EventArgs e)
        {
            Label customerID = (Label)this.fvOrderDone.FindControl("CustomerIDLabel");
            Label companyName = (Label)this.fvOrderDone.FindControl("CompanyNameLabel");
            int customerId = Convert.ToInt32(customerID.Text);
            DateTime deliveryDate = Convert.ToDateTime(((TextBox)this.fvOrderDone.FindControl("ByDateTextBox")).Text);

            string statusKey = null;
            switch (this.rbtnSendConfirm.SelectedValue)
            {
                case "postbox":
                    statusKey = MessageKeys.Order.StatusPostbox;
                    break;
                case "dispatched":
                    statusKey = MessageKeys.Order.StatusDispatched;
                    break;
                case "collected":
                    statusKey = MessageKeys.Order.StatusCollected;
                    break;
                case "done":
                    statusKey = MessageKeys.Order.StatusDelivered;
                    break;
            }

            var result = OrderDoneManager.CompleteOrder(
                customerId,
                deliveryDate,
                this.tbxStock.Text,
                this.tbxCount.Text,
                statusKey
            );

            if (result.Success)
            {
                new showMessageBox(this.Page, 
                    MessageProvider.Get(MessageKeys.Order.CompletedTitle),
                    MessageProvider.Format(MessageKeys.Order.CompletedSuccess, companyName.Text));
            }
            else
            {
                new showMessageBox(this.Page,
                    MessageProvider.Get(MessageKeys.Order.CompletedFailed),
                    result.Message);
            }

            this.ShowResults(companyName.Text, customerId, result.OriginalUsage);
        }

        protected void btnReturnToDeliveres_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("DeliverySheet.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("DeliverySheet.aspx");
        }
    }
}