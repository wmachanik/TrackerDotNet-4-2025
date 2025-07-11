// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.OrderDone
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public class OrderDone : Page
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
        }

        private int AddItemsToClientUsageTbl(
          long pCustomerID,
          bool pIsActual,
          long pCupCount,
          double pStock,
          DateTime pDeliveryDate)
        {
            List<ClientUsageFromTempOrder> all = new ClientUsageFromTempOrder().GetAll(pCustomerID);
            List<ItemUsageTbl> itemUsageTblList = new List<ItemUsageTbl>();
            List<ClientUsageLinesTbl> clientUsageLinesTblList = new List<ClientUsageLinesTbl>();
            int index1 = 0;
            string str = pIsActual ? "actual count" : "estimate count";
            if (pStock > 0.0)
            {
                pCupCount -= Convert.ToInt32(Math.Round(pStock * 100.0, 0));
                str = $"{str}; Stock of: {pCupCount.ToString()}";
            }
            while (all.Count > index1)
            {
                ClientUsageLinesTbl clientUsageLinesTbl = new ClientUsageLinesTbl();
                clientUsageLinesTbl.CustomerID = all[index1].CustomerID;
                clientUsageLinesTbl.LineDate = pDeliveryDate;
                clientUsageLinesTbl.ServiceTypeID = all[index1].ServiceTypeID;
                clientUsageLinesTbl.Qty = 0.0;
                clientUsageLinesTbl.CupCount = pCupCount;
                clientUsageLinesTbl.Notes = str;
                do
                {
                    clientUsageLinesTbl.Qty += all[index1].Qty * all[index1].UnitsPerQty;
                    itemUsageTblList.Add(new ItemUsageTbl()
                    {
                        CustomerID = all[index1].CustomerID,
                        ItemDate = pDeliveryDate,
                        ItemProvidedID = all[index1].ItemID,
                        AmountProvided = all[index1].Qty,
                        PackagingID = all[index1].PackagingID,
                        Notes = str
                    });
                    ++index1;
                }
                while (all.Count > index1 && clientUsageLinesTbl.ServiceTypeID == all[index1].ServiceTypeID);
                clientUsageLinesTblList.Add(clientUsageLinesTbl);
            }
            for (int index2 = 0; index2 < clientUsageLinesTblList.Count; ++index2)
                clientUsageLinesTblList[index2].InsertItemsUsed(clientUsageLinesTblList[index2]);
            for (int index3 = 0; index3 < itemUsageTblList.Count; ++index3)
                itemUsageTblList[index3].InsertItemsUsed(itemUsageTblList[index3]);
            return pCupCount;
        }

        protected void ShowResults(
          string CustomerName,
          long pCustomerId,
          ClientUsageTbl pOriginalUsageData)
        {
            this.pnlOrderDetails.Visible = false;
            this.tbxCustomerName.Text = CustomerName;
            List<ClientUsageTbl> clientUsageTblList = new List<ClientUsageTbl>();
            clientUsageTblList.Add(pOriginalUsageData);
            clientUsageTblList.Add(new ClientUsageTbl().GetUsageData(pCustomerId));
            this.dgCustomerUsage.AutoGenerateColumns = false;
            this.dgCustomerUsage.DataSource = (object)clientUsageTblList;
            this.dgCustomerUsage.DataBind();
            this.pnlCustomerDetailsUpdated.Visible = true;
        }

        private bool SendDeliveredEmail(long pCustomerID, string pMessage)
        {
            bool flag = false;
            CustomersTbl customersByCustomerId = new CustomersTbl().GetCustomersByCustomerID(pCustomerID);
            if (customersByCustomerId.EmailAddress.Contains("@") || customersByCustomerId.AltEmailAddress.Contains("@"))
            {
                string empty = string.Empty;
                EmailCls emailCls = new EmailCls();
                emailCls.SetEmailSubject("Confirmation email");
                string pObj1;
                if (customersByCustomerId.EmailAddress.Contains("@"))
                {
                    emailCls.SetEmailTo(customersByCustomerId.EmailAddress);
                    if (customersByCustomerId.AltEmailAddress.Contains("@"))
                    {
                        emailCls.SetEmailCC(customersByCustomerId.AltEmailAddress);
                        string str = !string.IsNullOrEmpty(customersByCustomerId.ContactFirstName) ? customersByCustomerId.ContactFirstName : string.Empty;
                        if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName))
                            str += " and ";
                        pObj1 = str + (string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName) ? customersByCustomerId.ContactAltFirstName : string.Empty);
                    }
                    else
                    {
                        emailCls.SetEmailTo(customersByCustomerId.AltEmailAddress);
                        pObj1 = empty + (string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName) ? customersByCustomerId.ContactAltFirstName : string.Empty);
                    }
                }
                else
                {
                    emailCls.SetEmailTo(customersByCustomerId.AltEmailAddress);
                    pObj1 = empty + (string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName) ? customersByCustomerId.ContactAltFirstName : string.Empty);
                }
                if (string.IsNullOrEmpty(pObj1))
                    pObj1 = "coffee lover";
                emailCls.AddFormatToBody("To {0},<br />,<br />", (object)pObj1);
                emailCls.AddStrAndNewLineToBody($"Just a quick note to notify you that Quaffee has {pMessage}<br />");
                emailCls.AddStrAndNewLineToBody("Thank you for your support.<br />");
                emailCls.AddStrAndNewLineToBody("Sincerely Quaffee Team (orders@quaffee.co.za)");
                flag = emailCls.SendEmail();
            }
            return flag;
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            TrackerTools trackerTools = new TrackerTools();
            trackerTools.SetTrackerSessionErrorString(string.Empty);
            Label control1 = (Label)this.fvOrderDone.FindControl("CustomerIDLabel");
            Label control2 = (Label)this.fvOrderDone.FindControl("CompanyNameLabel");
            long int64 = Convert.ToInt32(control1.Text);
            DateTime dateTime = Convert.ToDateTime(((TextBox)this.fvOrderDone.FindControl("ByDateTextBox")).Text);
            ClientUsageTbl clientUsageTbl1 = new ClientUsageTbl();
            ClientUsageTbl usageData = clientUsageTbl1.GetUsageData(int64);
            if (!string.IsNullOrEmpty(trackerTools.GetTrackerSessionErrorString()))
                this.Response.Write(trackerTools.GetTrackerSessionErrorString());
            TempOrdersDAL tempOrdersDal = new TempOrdersDAL();
            bool flag = tempOrdersDal.HasCoffeeInTempOrder();
            if (!string.IsNullOrEmpty(trackerTools.GetTrackerSessionErrorString()))
                this.Response.Write(trackerTools.GetTrackerSessionErrorString());
            if (this.tbxStock.Text.Length > 0 && Convert.ToDouble(this.tbxStock.Text) > 50.0)
            {
                showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Stock seems high", "The stock quantity appears very high please check that you have enterred the correct value in kilograms.</b>");
            }
            else
            {
                double pStock = string.IsNullOrEmpty(this.tbxStock.Text) ? 0.0 : Math.Round(Convert.ToDouble(this.tbxStock.Text), 2);
                this.ltrlStatus.Text = "Calculating the latest cup count";
                GeneralTrackerDbTools generalTrackerDbTools = new GeneralTrackerDbTools();
                GeneralTrackerDbTools.LineUsageData latestUsageData = generalTrackerDbTools.GetLatestUsageData(int64, 2);
                if (!string.IsNullOrEmpty(trackerTools.GetTrackerSessionErrorString()))
                {
                    showMessageBox showMessageBox2 = new showMessageBox(this.Page, "Tracker Session Error", trackerTools.GetTrackerSessionErrorString());
                }
                bool pIsActual = this.tbxCount.MaxLength > 0;
                long pCupCount = 0;
                if (this.tbxCount.MaxLength > 0)
                    pCupCount = Convert.ToInt32(this.tbxCount.Text);
                if (pCupCount < 1L || pCupCount < latestUsageData.LastCount)
                {
                    this.ltrlStatus.Text = "Calculating the latest est cup count";
                    pCupCount = generalTrackerDbTools.CalcEstCupCount(int64, latestUsageData, flag);
                    pIsActual = false;
                }
                new RepairsTbl().SetStatusDoneByTempOrder();
                long clientUsageTbl2 = this.AddItemsToClientUsageTbl(int64, pIsActual, pCupCount, pStock, dateTime);
                if (!clientUsageTbl1.UpdateUsageCupCount(int64, clientUsageTbl2))
                {
                    showMessageBox showMessageBox3 = new showMessageBox(this.Page, "Error", "Error updating last count");
                    this.ltrlStatus.Text = "error updating last count";
                }
                generalTrackerDbTools.UpdatePredictions(int64, clientUsageTbl2);
                tempOrdersDal.MarkTempOrdersItemsAsDone();
                generalTrackerDbTools.ResetCustomerReminderCount(int64, flag);
                if (flag)
                    generalTrackerDbTools.SetClientCoffeeOnlyIfInfo(int64);
                switch (this.rbtnSendConfirm.SelectedValue)
                {
                    case "postbox":
                        this.SendDeliveredEmail(int64, "placed your order in the your post box.");
                        break;
                    case "dispatched":
                        this.SendDeliveredEmail(int64, "dispatched you order.");
                        break;
                    case "done":
                        this.SendDeliveredEmail(int64, "delivered your order and it has signed for.");
                        break;
                }
                tempOrdersDal.KillTempOrdersData();
                this.ShowResults(control2.Text, int64, usageData);
            }
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