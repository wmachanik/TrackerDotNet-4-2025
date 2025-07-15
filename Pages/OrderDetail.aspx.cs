// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.OrderDetail
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;
using TrackerDotNet.BusinessLogic;
//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public partial class OrderDetail : Page
    {
        public const string CONST_EMAILDELIMITERSTART = "[#";
        public const string CONST_EMAILDELIMITEREND = "#]";
        public const string CONST_QRYSTR_CustomerID = "CustomerID";
        public const string CONST_QRYSTR_DELIVERYDATE = "DeliveryDate";
        public const string CONST_QRYSTR_NOTES = "Notes";
        public const string CONST_QRYSTR_DELIVERED = "Delivered";
        public const string CONST_QRYSTR_INVOICED = "Invoiced";
        private const string CONST_FROMEMAIL = "orders@quaffee.co.za";
        private const string CONST_DELIVERYTYPEISCOLLECTION = "Cllct";
        private const string CONST_DELIVERYTYPEISCOURIER = "Cour";
        private const string CONST_ORDERHEADERVALUES = "OrderHeaderValues";
        protected ScriptManager scrmOrderDetail;
        protected UpdateProgress udtpOrderDetail;
        protected UpdatePanel pnlOrderHeader;
        protected DetailsView dvOrderHeader;
        protected UpdatePanel upnlOrderLines;
        protected GridView gvOrderLines;
        protected UpdatePanel upnlNewOrderItem;
        protected Button btnNewItem;
        protected Panel pnlNewItem;
        protected DropDownList ddlNewItemDesc;
        protected TextBox tbxNewQuantityOrdered;
        protected DropDownList ddlNewPackaging;
        protected Button btnAdd;
        protected Button btnCancel;
        protected Literal ltrlStatus;
        protected UpdatePanel updtButtonPanel;
        protected Button btnNewOrder;
        protected Button btnConfirmOrder;
        protected Button btnDlSheet;
        protected Button btnOrderCancelled;
        protected Button btnUnDoDone;
        protected Button btnOrderDelivered;
        protected ObjectDataSource odsOrderSummary;
        protected SqlDataSource sdsCompanys;
        protected ObjectDataSource odsOrderDetail;
        protected SqlDataSource sdsDeliveryBy;
        protected ObjectDataSource odsItemTypes;
        protected SqlDataSource sdsPackagingTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                long num = 1;
                DateTime date = TimeZoneUtils.Now().Date;
                string empty = string.Empty;
                if (this.Request.QueryString["CustomerID"] != null)
                    num = (long)Convert.ToInt32(this.Request.QueryString["CustomerID"].ToString());
                if (this.Request.QueryString["DeliveryDate"] != null)
                    date = Convert.ToDateTime(this.Request.QueryString["DeliveryDate"]).Date;
                if (this.Request.QueryString["Notes"] != null)
                    empty = this.Request.QueryString["Notes"].ToString();
                this.Session["BoundCustomerID"] = (object)num;
                this.Session["BoundDeliveryDate"] = (object)date.Date;
                this.Session["BoundNotes"] = (object)empty;
                this.Session["OrderHeaderValues"] = (object)null;
                OrderItemTbl orderItemTbl = new OrderItemTbl();
                this.btnOrderCancelled.Enabled = Membership.GetUser().UserName.ToLower() == "warren";
                this.btnNewItem.Enabled = this.User.IsInRole("Administrators") || this.User.IsInRole("AgentManager") || this.User.IsInRole("Agents");
                new TrackerTools().SetTrackerSessionErrorString(string.Empty);
                if (this.Request.QueryString["Invoiced"] != null)
                {
                    if (!this.Request.QueryString["Invoiced"].Equals("Y"))
                        return;
                    this.MarkItemAsInvoiced();
                }
                else
                {
                    if (this.Request.QueryString["Delivered"] == null || !this.Request.QueryString["Delivered"].Equals("Y"))
                        return;
                    this.btnOrderDelivered_Click(sender, e);
                }
            }
            else
            {
                TrackerTools trackerTools = new TrackerTools();
                string sessionErrorString = trackerTools.GetTrackerSessionErrorString();
                if (string.IsNullOrEmpty(sessionErrorString))
                    return;
                showMessageBox showMessageBox = new showMessageBox(this.Page, "Tracker Error", "ERROR: " + sessionErrorString);
                trackerTools.SetTrackerSessionErrorString(string.Empty);
            }
        }

        private string GetOrderHeaderRequiredByDateStr()
        {
            string empty = string.Empty;
            return this.dvOrderHeader.CurrentMode != DetailsViewMode.Edit ? this.dvOrderHeaderGetLabelValue("lblRequiredByDate") : this.dvOrderHeaderGetTextBoxValue("tbxRequiredByDate");
        }

        private string GetOrderHeaderNotes()
        {
            string empty = string.Empty;
            return this.dvOrderHeader.CurrentMode != DetailsViewMode.Edit ? this.dvOrderHeaderGetLabelValue("lblNotes") : this.dvOrderHeaderGetTextBoxValue("tbxNotes");
        }

        private void BindRowQueryParameters()
        {
            string controlSelectedValue = this.dvOrderHeaderGetDDLControlSelectedValue("ddlContacts");
            this.Session["BoundCustomerID"] = (object)Convert.ToInt32(controlSelectedValue);
            DateTime date = Convert.ToDateTime(this.GetOrderHeaderRequiredByDateStr()).Date;
            this.Session["BoundDeliveryDate"] = (object)date.Date;
            string orderHeaderNotes = this.GetOrderHeaderNotes();
            this.Session["BoundNotes"] = (object)orderHeaderNotes;
            UriBuilder uriBuilder = new UriBuilder(this.Request.Url);
            NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryString.Set("CustomerID", controlSelectedValue);
            queryString.Set("DeliveryDate", $"{date:yyyy-MM-dd}");
            queryString.Set("Notes", orderHeaderNotes);
            uriBuilder.Query = queryString.ToString();
            string script = $"ChangeUrl('OrderDetail','{uriBuilder.Uri.ToString()}')";
            System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Order Detail", script, true);
        }

        protected void btnNewItem_Click(object sender, EventArgs e)
        {
            this.btnAdd.Visible = true;
            this.btnCancel.Visible = true;
            this.pnlNewItem.Visible = true;
            this.btnNewItem.Visible = false;
            this.upnlNewOrderItem.Update();
        }

        private void HideNewOrderItemPanel()
        {
            this.btnAdd.Visible = false;
            this.btnCancel.Visible = false;
            this.pnlNewItem.Visible = false;
            this.btnNewItem.Visible = true;
            this.upnlNewOrderItem.Update();
            this.odsOrderDetail.DataBind();
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }

        private string dvOrderHeaderGetDDLControlSelectedValue(string pDDLControlName)
        {
            DropDownList control = (DropDownList)this.dvOrderHeader.FindControl(pDDLControlName);
            return control.SelectedValue == null ? "0" : control.SelectedValue;
        }

        private string dvOrderHeaderGetTextBoxValue(string pTextBoxControlName)
        {
            TextBox control = (TextBox)this.dvOrderHeader.FindControl(pTextBoxControlName);
            return control != null ? control.Text : string.Empty;
        }

        private string dvOrderHeaderGetLabelValue(string pTextBoxControlName)
        {
            Label control = (Label)this.dvOrderHeader.FindControl(pTextBoxControlName);
            return control != null ? control.Text : string.Empty;
        }

        private bool dvOrderHeaderGetCheckBoxValue(string pCheckBoxControlName)
        {
            CheckBox control = (CheckBox)this.dvOrderHeader.FindControl(pCheckBoxControlName);
            return control != null && control.Checked;
        }

        private OrderHeaderData Get_dvOrderHeaderData(bool pInEditMode)
        {
            OrderHeaderData dvOrderHeaderData = new OrderHeaderData();
            dvOrderHeaderData.CustomerID = Convert.ToInt32(this.dvOrderHeaderGetDDLControlSelectedValue("ddlContacts"));
            dvOrderHeaderData.ToBeDeliveredBy = Convert.ToInt32(this.dvOrderHeaderGetDDLControlSelectedValue("ddlToBeDeliveredBy"));
            dvOrderHeaderData.Confirmed = this.dvOrderHeaderGetCheckBoxValue("cbxConfirmed");
            dvOrderHeaderData.Done = this.dvOrderHeaderGetCheckBoxValue("cbxDone");
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            string str1;
            string str2;
            string str3;
            if (pInEditMode)
            {
                str1 = this.dvOrderHeaderGetTextBoxValue("tbxOrderDate");
                str2 = this.dvOrderHeaderGetTextBoxValue("tbxRoastDate");
                str3 = this.dvOrderHeaderGetTextBoxValue("tbxRequiredByDate");
                dvOrderHeaderData.Notes = this.dvOrderHeaderGetTextBoxValue("tbxNotes");
            }
            else
            {
                str3 = this.dvOrderHeaderGetLabelValue("lblRequiredByDate");
                str1 = this.dvOrderHeaderGetLabelValue("lblOrderDate");
                str2 = this.dvOrderHeaderGetLabelValue("lblRoastDate");
                dvOrderHeaderData.Notes = this.dvOrderHeaderGetLabelValue("lblNotes");
            }
            dvOrderHeaderData.RequiredByDate = string.IsNullOrEmpty(str3) ? DateTime.MinValue : Convert.ToDateTime(str3).Date;
            dvOrderHeaderData.OrderDate = string.IsNullOrEmpty(str1) ? DateTime.MinValue : Convert.ToDateTime(str1).Date;
            dvOrderHeaderData.RoastDate = string.IsNullOrEmpty(str2) ? DateTime.MinValue : Convert.ToDateTime(str2).Date;
            return dvOrderHeaderData;
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            OrderHeaderData headerData = this.Get_dvOrderHeaderData(false);
            OrderTblData orderData = new OrderTblData
            {
                CustomerID = headerData.CustomerID,
                OrderDate = headerData.OrderDate,
                RoastDate = headerData.RoastDate,
                RequiredByDate = headerData.RequiredByDate,
                ToBeDeliveredBy = Convert.ToInt32(headerData.ToBeDeliveredBy),
                PurchaseOrder = headerData.PurchaseOrder,
                Confirmed = headerData.Confirmed,
                InvoiceDone = headerData.InvoiceDone,
                Done = headerData.Done,
                Notes = headerData.Notes,
                ItemTypeID = Convert.ToInt32(this.ddlNewItemDesc.SelectedValue),
                QuantityOrdered = Convert.ToDouble(this.tbxNewQuantityOrdered.Text),
                PackagingID = Convert.ToInt32(this.ddlNewPackaging.SelectedValue)
            };

            var manager = new TrackerDotNet.BusinessLogic.OrderManager();
            string result = manager.AddOrderLine(headerData, orderData);

            this.ltrlStatus.Text = string.IsNullOrWhiteSpace(result) ? "Item Added" : "Error adding item: " + result;
            this.HideNewOrderItemPanel();
        }
        /*
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            OrderTbl orderTbl = new OrderTbl();
            OrderHeaderData dvOrderHeaderData = this.Get_dvOrderHeaderData(false);
            OrderTblData pOrderData = new OrderTblData()
            {
                CustomerID = dvOrderHeaderData.CustomerID,
                OrderDate = dvOrderHeaderData.OrderDate,
                RoastDate = dvOrderHeaderData.RoastDate,
                RequiredByDate = dvOrderHeaderData.RequiredByDate,
                ToBeDeliveredBy = Convert.ToInt32(dvOrderHeaderData.ToBeDeliveredBy),
                PurchaseOrder = dvOrderHeaderData.PurchaseOrder,
                Confirmed = dvOrderHeaderData.Confirmed,
                InvoiceDone = dvOrderHeaderData.InvoiceDone,
                Done = dvOrderHeaderData.Done
            };
            pOrderData.PurchaseOrder = dvOrderHeaderData.PurchaseOrder;
            pOrderData.Notes = dvOrderHeaderData.Notes;
            TrackerTools trackerTools = new TrackerTools();
            pOrderData.ItemTypeID = Convert.ToInt32(this.ddlNewItemDesc.SelectedValue);
            pOrderData.ItemTypeID = trackerTools.ChangeItemIfGroupToNextItemInGroup(pOrderData.CustomerID, pOrderData.ItemTypeID, pOrderData.RequiredByDate);
            pOrderData.QuantityOrdered = Convert.ToDouble(this.tbxNewQuantityOrdered.Text);
            pOrderData.PackagingID = Convert.ToInt32(this.ddlNewPackaging.SelectedValue);
            string str = orderTbl.InsertNewOrderLine(pOrderData);
            this.ltrlStatus.Text = string.IsNullOrWhiteSpace(str) ? "Item Added" : "Error adding item: " + str;
            this.HideNewOrderItemPanel();
        }
        */
        protected void btnCancel_Click(object sender, EventArgs e) => this.HideNewOrderItemPanel();

        protected void Page_Unload(object sender, EventArgs e)
        {
        }
        protected void gvOrderLines_OnItemDelete(object sender, EventArgs e)
        {
            string pDataValue = ((CommandEventArgs)e).CommandArgument.ToString();
            var manager = new TrackerDotNet.BusinessLogic.OrderManager();
            string result = manager.DeleteOrderItem(Convert.ToInt32(pDataValue));
            this.ltrlStatus.Text = string.IsNullOrEmpty(result) ? "Item Deleted" : "Error deleting item: " + result;
        }
        /*
         
        protected void gvOrderLines_OnItemDelete(object sender, EventArgs e)
        {
            string pDataValue = ((CommandEventArgs)e).CommandArgument.ToString();
            string strSQL = "DELETE FROM OrdersTbl WHERE (OrderID = ?)";
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pDataValue, DbType.String, "@OrderID");
            string str = trackerDb.ExecuteNonQuerySQL(strSQL);
            trackerDb.Close();
            this.ltrlStatus.Text = string.IsNullOrEmpty(str) ? "Item Deleted" : "Error deleting item: " + str;
        }
        */
        protected void gvOrderLines_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            string empty = string.Empty;
            LogTbl logTbl = new LogTbl();
            logTbl.AddToWhatsChanged("ItemTypeID", e.OldValues[(object)"ItemTypeID"].ToString(), e.NewValues[(object)"ItemTypeID"].ToString(), ref empty);
            logTbl.AddToWhatsChanged("Qty", e.OldValues[(object)"QuantityOrdered"].ToString(), e.NewValues[(object)"QuantityOrdered"].ToString(), ref empty);
            logTbl.AddToWhatsChanged("PackagingID", e.OldValues[(object)"PackagingID"].ToString(), e.NewValues[(object)"PackagingID"].ToString(), ref empty);
            logTbl.InsertLogItem(Membership.GetUser().UserName, 1, 2, Convert.ToInt32(this.dvOrderHeaderGetDDLControlSelectedValue("ddlContacts")), empty, "Order Detail");
            this.odsOrderDetail.DataBind();
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }

        protected virtual void dvOrderHeader_OnModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            if (e.NewMode == DetailsViewMode.Edit)
            {
                this.Session["OrderHeaderValues"] = (object)this.Get_dvOrderHeaderData(false);
            }
            else
            {
                if (e.NewMode != DetailsViewMode.ReadOnly || this.Session["OrderHeaderValues"] == null)
                    return;
                this.dvOrderHeader.DataBind();
            }
        }

        protected virtual void dvOrderHeader_OnItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            this.upnlOrderLines.Update();
        }

        protected virtual void dvOrderHeader_OnDataBound(object sender, EventArgs e)
        {
            if (this.dvOrderHeader.CurrentMode == DetailsViewMode.ReadOnly)
            {
                if (this.Session["OrderHeaderValues"] != null)
                {
                    OrderHeaderData orderHeaderData = (OrderHeaderData)this.Session["OrderHeaderValues"];
                    if (!this.GetOrderHeaderRequiredByDateStr().Equals(string.Empty))
                    {
                        DateTime date = Convert.ToDateTime(this.GetOrderHeaderRequiredByDateStr()).Date;
                        if (orderHeaderData.OrderDate.Date != date.Date)
                        {
                            UsedItemGroupTbl usedItemGroupTbl = new UsedItemGroupTbl();
                            foreach (GridViewRow row in this.gvOrderLines.Rows)
                            {
                                DropDownList control = (DropDownList)row.FindControl("ddlItemDesc");
                                usedItemGroupTbl.UpdateIfGroupItem(orderHeaderData.CustomerID, Convert.ToInt32(control.SelectedValue), orderHeaderData.RequiredByDate, date);
                            }
                        }
                    }
                }
                Label control1 = (Label)this.dvOrderHeader.FindControl("lblPurchaseOrder");
                if (control1 != null && control1.Text.Equals("!!!PO required!!!"))
                {
                    control1.BackColor = Color.Red;
                    control1.ForeColor = Color.White;
                }
            }
            CheckBox control2 = (CheckBox)this.dvOrderHeader.FindControl("cbxDone");
            if (control2 != null)
                this.btnOrderDelivered.Enabled = this.btnOrderCancelled.Enabled = !control2.Checked;
            this.updtButtonPanel.Update();
        }

        public void DeleteOrderItem(string pOrderID)
        {
            string str = new OrderTbl().DeleteOrderById(Convert.ToInt32(pOrderID));
            this.ltrlStatus.Text = str.Length == 0 ? "Item deleted" : str;
        }

        private string UnDoneOrderItem(string pOrderID)
        {
            return new OrderTbl().UpdateSetDoneByID(false, Convert.ToInt32(pOrderID));
        }

        protected void btnCancelled_Click(object sender, EventArgs e)
        {
            if (!(Membership.GetUser().UserName.ToLower() == "warren"))
                return;
            foreach (TableRow row in this.gvOrderLines.Rows)
                this.DeleteOrderItem(((Label)row.Cells[4].FindControl("lblOrderID")).Text);
            this.Response.Redirect("DeliverySheet.aspx");
        }

        private ContactEmailDetails GetEmailAddressFromNote()
        {
            ContactEmailDetails emailAddressFromNote = (ContactEmailDetails)null;
            string labelValue = this.dvOrderHeaderGetLabelValue("lblNotes");
            int num = labelValue.IndexOf("[#");
            if (num >= 0)
            {
                emailAddressFromNote = new ContactEmailDetails();
                emailAddressFromNote.EmailAddress = labelValue.Substring(num + 2, labelValue.IndexOf("#]") - num - 2);
            }
            return emailAddressFromNote;
        }

        private ContactEmailDetails GetEmailDetails(string pContactsID)
        {
            ContactEmailDetails contactEmailDetails = new ContactEmailDetails();
            return !pContactsID.Equals("9") ? contactEmailDetails.GetContactsEmailDetails(Convert.ToInt32(pContactsID)) : this.GetEmailAddressFromNote();
        }

        private string AddUnitsToQty(string pItemTypeID, string pQty)
        {
            string itemUoM = this.GetItemUoM(Convert.ToInt32(pItemTypeID));
            if (string.IsNullOrEmpty(itemUoM))
                return pQty;
            double num = Convert.ToDouble(pQty);
            return $"{pQty} {(num == 1.0 ? itemUoM : itemUoM + "s")}";
        }

        private int GetItemSortOrderID(string pItemTypeID)
        {
            return new ItemTypeTbl().GetItemSortOrder(Convert.ToInt32(pItemTypeID));
        }

        private string UpCaseFirstLetter(string pString)
        {
            return string.IsNullOrEmpty(pString) ? string.Empty : char.ToUpper(pString[0]).ToString() + pString.Substring(1);
        }

        private string ResolveRecipientEmail(ContactEmailDetails details)
        {
            return !string.IsNullOrWhiteSpace(details.EmailAddress)
                ? details.EmailAddress
                : details.altEmailAddress;
        }

        private void AppendOrderItemsToEmailBody(EmailMailKitCls email)
        {
            email.AddToBody("<ul>");

            foreach (GridViewRow row in gvOrderLines.Rows)
            {
                var itemDDL = (DropDownList)row.FindControl("ddlItemDesc");
                var qtyLbl = (Label)row.FindControl("lblQuantityOrdered");
                var packagingDDL = (DropDownList)row.FindControl("ddlPackaging");

                string itemId = itemDDL.SelectedValue;
                string itemName = itemDDL.SelectedItem.Text;
                string qty = AddUnitsToQty(itemId, qtyLbl.Text);

                if (GetItemSortOrderID(itemId) == 10)
                {
                    string notes = EmailUtils.CleanNoteText(dvOrderHeaderGetLabelValue("lblNotes"));
                    email.AddFormatToBody("<li>{0}</li>", notes);
                }
                else
                {
                    if (packagingDDL.SelectedIndex == 0)
                        email.AddFormatToBody(MessageProvider.Get("OrderItemFormatBasic"), qty, itemName);
                    else
                        email.AddFormatToBody(MessageProvider.Get("OrderItemFormatWithPrep"), qty, itemName, packagingDDL.SelectedItem.Text);
                }
            }

            email.AddToBody("</ul>");
        }

        private void AppendOrderDetails(EmailMailKitCls email)
        {
            string poRef = dvOrderHeaderGetLabelValue("lblPurchaseOrder");
            if (!string.IsNullOrWhiteSpace(poRef))
            {
                string poMsg = poRef.EndsWith("!!!PO required!!!")
                    ? MessageProvider.Get("OrderConfirmationPORequired")
                    : MessageProvider.Format("OrderConfirmationPOReceived", poRef);
                email.AddStrAndNewLineToBody(poMsg);
            }

            var deliveryDDL = (DropDownList)this.dvOrderHeader.FindControl("ddlToBeDeliveredBy");
            string deliveryDate = dvOrderHeaderGetLabelValue("lblRequiredByDate");
            string deliveryOption = deliveryDDL.SelectedItem.Text;

            string deliveryMsg = "";

            if (deliveryOption == "Cllct")
                deliveryMsg = MessageProvider.Format("OrderCollectionNote", deliveryDate);
            else if (deliveryOption == "Cour")
                deliveryMsg = MessageProvider.Format("OrderCourierNote", deliveryDate);
            else
                deliveryMsg = MessageProvider.Format("OrderDeliveryNote", deliveryDate);

            email.AddStrAndNewLineToBody(deliveryMsg);
        }

        private string GetSenderInfo()
        {
            MembershipUser user = Membership.GetUser();
            return string.IsNullOrWhiteSpace(user?.UserName)
                ? "The Quaffee Team"
                : $"The Quaffee Team ({UpCaseFirstLetter(user.UserName)})";
        }
        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            // Gather data from UI
            // Get ContactEmailDetails from the selected contact in the DetailsView
            var contactDDL = (DropDownList)this.dvOrderHeader.FindControl("ddlContacts");
            ContactEmailDetails contact = GetEmailDetails(contactDDL.SelectedValue);

            // Get OrderHeaderData using your existing helper
            OrderHeaderData header = this.Get_dvOrderHeaderData(false);

            // Build List<OrderLineData> from the GridView
            var orderLines = new List<TrackerDotNet.Managers.OrderLineData>();
            foreach (GridViewRow row in this.gvOrderLines.Rows)
            {
                var itemDDL = (DropDownList)row.FindControl("ddlItemDesc");
                var qtyLbl = (Label)row.FindControl("lblQuantityOrdered");
                var packagingDDL = (DropDownList)row.FindControl("ddlPackaging");

                var line = new TrackerDotNet.Managers.OrderLineData
                {
                    ItemID = Convert.ToInt32(itemDDL.SelectedValue),
                    ItemName = itemDDL.SelectedItem.Text,
                    Qty = Convert.ToDouble(qtyLbl.Text),
                    PackagingID = packagingDDL.SelectedIndex > 0 ? Convert.ToInt32(packagingDDL.SelectedValue) : 0,
                    PackagingName = packagingDDL.SelectedIndex > 0 ? packagingDDL.SelectedItem.Text : string.Empty
                };
                orderLines.Add(line);
            }

            // Get notes from the order header
            string notes = this.GetOrderHeaderNotes();

            var emailManager = new TrackerDotNet.Managers.OrderDetailManager();
            string statusMsg;
            bool success = emailManager.SendOrderConfirmation(contact, header, orderLines, notes, out statusMsg);

            ltrlStatus.Text = statusMsg;
            // Show message box, update UI, etc.
            new showMessageBox(this.Page, "Order Confirmation", statusMsg);
            upnlNewOrderItem.Update();
        }
        /*
        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            var contactDDL = (DropDownList)this.dvOrderHeader.FindControl("ddlContacts");
            ContactEmailDetails details = GetEmailDetails(contactDDL.SelectedValue);

            string recipientEmail = ResolveRecipientEmail(details);

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                ltrlStatus.Text = "No email address found.";
                new showMessageBox(this.Page, "Email FAILED", ltrlStatus.Text);
                upnlNewOrderItem.Update();
                AppLogger.WriteLog("email", ltrlStatus.Text);
                return;
            }

            var emailSettings = new EmailSettings();
            emailSettings.SetRecipient(recipientEmail);

            var email = new EmailMailKitCls(emailSettings);
            email.IsTestMode = true;
            email.AddCCFromAddress();
            email.SetEmailSubject(MessageProvider.Get("OrderConfirmationSubject"));

            string contactName = EmailUtils.GetFriendlyContactName(details);
            email.AddFormatToBody(MessageProvider.Get("OrderConfirmationIntro"), contactName);

            if (contactDDL.SelectedValue.Equals("9"))
                email.AddStrAndNewLineToBody(MessageProvider.Get("OrderConfirmationLineIntro"));

            AppendOrderItemsToEmailBody(email);
            AppendOrderDetails(email);

            string senderInfo = GetSenderInfo();
            email.AddFormatToBody(MessageProvider.Get("OrderEmailFooter"), senderInfo);
            email.AddToBody(MessageProvider.Get("DefaultEmailSignature"));

            bool success = email.SendEmail();

            string statusMsg = success
                ? $"Email sent to {contactName}"
                : $"Error sending email: {email.myResults.sResult}";

            ltrlStatus.Text = statusMsg;
            new showMessageBox(this.Page, "Order Confirmation", statusMsg);
            upnlNewOrderItem.Update();
            AppLogger.WriteLog("email", statusMsg);
        }

        /*
        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            var contactDDL = (DropDownList)this.dvOrderHeader.FindControl("ddlContacts");
            ContactEmailDetails emailDetails = this.GetEmailDetails(contactDDL.SelectedItem.Value);
            if (emailDetails == null)
                return;

            string recipientEmail = !string.IsNullOrEmpty(emailDetails.EmailAddress)
                ? emailDetails.EmailAddress
                : emailDetails.altEmailAddress;

            if (string.IsNullOrEmpty(recipientEmail))
            {
                ltrlStatus.Text = "No email address found.";
                new showMessageBox(this.Page, "Email FAILED", ltrlStatus.Text);
                upnlNewOrderItem.Update();
                return;
            }

            var emailSettings = new EmailSettings();
            emailSettings.SetRecipient(recipientEmail);

            var email = new EmailMailKitCls(emailSettings);
            email.SetEmailSubject(MessageProvider.Get("OrderConfirmationSubject"));
            email.IsTestMode = true;
            email.AddCCFromAddress();

            string contactName = EmailUtils.GetFriendlyContactName(emailDetails);
            email.AddFormatToBody(MessageProvider.Get("OrderConfirmationIntro"), contactName);

            if (contactDDL.SelectedValue.Equals("9"))
                email.AddStrAndNewLineToBody("We confirm your order below:");

            email.AddToBody("<ul>");

            foreach (GridViewRow row in gvOrderLines.Rows)
            {
                var itemDDL = (DropDownList)row.FindControl("ddlItemDesc");
                var qtyLbl = (Label)row.FindControl("lblQuantityOrdered");
                var packagingDDL = (DropDownList)row.FindControl("ddlPackaging");

                string itemId = itemDDL.SelectedValue;
                string itemName = itemDDL.SelectedItem.Text;
                string qty = AddUnitsToQty(itemId, qtyLbl.Text);

                if (GetItemSortOrderID(itemId) == 10)
                {
                    string notes = EmailUtils.CleanNoteText(dvOrderHeaderGetLabelValue("lblNotes"));
                    email.AddFormatToBody("<li>{0}</li>", notes);
                }
                else
                {
                    if (packagingDDL.SelectedIndex == 0)
                        email.AddFormatToBody("<li>{0} of {1}</li>", qty, itemName);
                    else
                        email.AddFormatToBody("<li>{0} of {1} - Preparation note: {2}</li>", qty, itemName, packagingDDL.SelectedItem.Text);
                }
            }

            email.AddToBody("</ul>");

            string poRef = dvOrderHeaderGetLabelValue("lblPurchaseOrder");
            if (!string.IsNullOrEmpty(poRef))
            {
                string poMsg = poRef.EndsWith("!!!PO required!!!")
                    ? MessageProvider.Get("OrderConfirmationPORequired")
                    : MessageProvider.Format("OrderConfirmationPOReceived", poRef);
                email.AddStrAndNewLineToBody(poMsg);
            }

            var deliveryDDL = (DropDownList)this.dvOrderHeader.FindControl("ddlToBeDeliveredBy");
            string deliveryDate = dvOrderHeaderGetLabelValue("lblRequiredByDate");
            string deliveryOption = deliveryDDL.SelectedItem.Text;

            string deliveryMsg = "";

            if (deliveryOption == "Cllct")
                deliveryMsg = MessageProvider.Format("OrderCollectionNote", deliveryDate);
            else if (deliveryOption == "Cour")
                deliveryMsg = MessageProvider.Format("OrderCourierNote", deliveryDate);
            else
                deliveryMsg = MessageProvider.Format("OrderDeliveryNote", deliveryDate);

            email.AddStrAndNewLineToBody(deliveryMsg);

            MembershipUser user = Membership.GetUser();
            string senderInfo = string.IsNullOrEmpty(user?.UserName)
                ? "The Quaffee Team"
                : $"The Quaffee Team ({UpCaseFirstLetter(user.UserName)})";

            email.AddFormatToBody(MessageProvider.Get("OrderEmailFooter"), senderInfo);

            string statusMsg = email.SendEmail()
                ? $"Email sent to {contactName}"
                : $"Error sending email: {email.myResults.sResult}";

            ltrlStatus.Text = statusMsg;
            new showMessageBox(this.Page, "Order Confirmation", statusMsg);
            upnlNewOrderItem.Update();
        }
        */


        /*
        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            DropDownList control1 = (DropDownList)this.dvOrderHeader.FindControl("ddlContacts");
            ContactEmailDetails emailDetails = this.GetEmailDetails(control1.SelectedItem.Value);
            if (emailDetails == null)
                return;
            EmailCls emailCls = new EmailCls();
            DropDownList control2 = (DropDownList)this.dvOrderHeader.FindControl("ddlToBeDeliveredBy");
            string labelValue1 = this.dvOrderHeaderGetLabelValue("lblRequiredByDate");
            string labelValue2 = this.dvOrderHeaderGetLabelValue("lblPurchaseOrder");
            string pObj1 = this.dvOrderHeaderGetLabelValue("lblNotes");
            if (emailDetails.EmailAddress != "")
            {
                emailCls.SetLegacyEmailFromTo("orders@quaffee.co.za", emailDetails.EmailAddress);
                if (emailDetails.altEmailAddress != "")
                    emailCls.SetLegacyEmailCC(emailDetails.altEmailAddress);
            }
            else if (emailDetails.altEmailAddress != "")
            {
                emailCls.SetLegacyEmailFromTo("orders@quaffee.co.za", emailDetails.altEmailAddress);
            }
            else
            {
                this.ltrlStatus.Text = "no email address found";
                showMessageBox showMessageBox = new showMessageBox(this.Page, "Email FAILED: ", this.ltrlStatus.Text);
                this.upnlNewOrderItem.Update();
                return;
            }
            emailCls.SetLegacyEmailBCC("orders@quaffee.co.za");
            emailCls.SetLegacyEmailSubject("Order Confirmation");
            string str1 = "Coffee Lover";
            if (emailDetails.FirstName != "")
            {
                str1 = emailDetails.FirstName.Trim();
                if (emailDetails.altFirstName != "")
                    str1 = $"{str1} and {emailDetails.altFirstName.Trim()}";
            }
            else if (emailDetails.altFirstName != "")
                str1 = emailDetails.altFirstName.Trim();
            emailCls.AddStrAndNewLineToLegacyEmailBody($"Dear {str1},<br />");
            if (control1.SelectedValue.Equals("9"))
                emailCls.AddStrAndNewLineToLegacyEmailBody("We confirm you order below:");
            else
                emailCls.AddStrAndNewLineToLegacyEmailBody($"We confirm the following order for {control1.SelectedItem.Text}:");
            emailCls.AddToLegacyEmailBody("<ul>");
            foreach (GridViewRow row in this.gvOrderLines.Rows)
            {
                DropDownList control3 = (DropDownList)row.FindControl("ddlItemDesc");
                Label control4 = (Label)row.FindControl("lblQuantityOrdered");
                DropDownList control5 = (DropDownList)row.FindControl("ddlPackaging");
                if (this.GetItemSortOrderID(control3.SelectedValue) == 10)
                {
                    if (pObj1.Contains(":"))
                        pObj1 = pObj1.Substring(pObj1.IndexOf(":") + 1).Trim();
                    int length = pObj1.IndexOf("[#");
                    if (length >= 0)
                    {
                        int num = pObj1.IndexOf("#]");
                        if (num >= 0)
                            pObj1 = $"{pObj1.Substring(0, length)};{pObj1.Substring(num + 2)}";
                    }
                    emailCls.AddFormatToLegacyEmailBody("<li>{0}</li>", (object)pObj1);
                }
                else
                {
                    string qty = this.AddUnitsToQty(control3.SelectedValue, control4.Text);
                    if (control5.SelectedIndex == 0)
                        emailCls.AddFormatToLegacyEmailBody("<li>{0} of {1}</li>", (object)qty, (object)control3.SelectedItem.Text);
                    else
                        emailCls.AddFormatToLegacyEmailBody("<li>{0} of {1} - Preperation note: {2}</li>", (object)qty, (object)control3.SelectedItem.Text, (object)control5.SelectedItem.Text);
                }
            }
            emailCls.AddStrAndNewLineToLegacyEmailBody("</ul>");
            if (!string.IsNullOrEmpty(labelValue2))
            {
                if (labelValue2.EndsWith("!!!PO required!!!"))
                    emailCls.AddStrAndNewLineToLegacyEmailBody("<b>NOTE</b>: We are still waiting for a Purchase Order number from you.<br />");
                else
                    emailCls.AddStrAndNewLineToLegacyEmailBody($"This order has purchase order: {labelValue2}, allocated to it.<br />");
            }
            if (control2.SelectedItem.Text == "Cllct")
                emailCls.AddStrAndNewLineToLegacyEmailBody("The order will be ready for collection on: " + labelValue1);
            else if (control2.SelectedItem.Text == "Cour")
                emailCls.AddStrAndNewLineToLegacyEmailBody($"The order will be dispatched on: {labelValue1}.");
            else
                emailCls.AddStrAndNewLineToLegacyEmailBody($"The order will be delivered on: {labelValue1}.");
            MembershipUser user = Membership.GetUser();
            string str2 = string.IsNullOrEmpty(user.UserName) ? "the Quaffee Team" : $" from the Quaffee Team ({this.UpCaseFirstLetter(user.UserName)})";
            emailCls.AddStrAndNewLineToLegacyEmailBody($"<br />Sent automatically by Quaffee's order and tracking System.<br /><br />Sincerely {str2} (orders@quaffee.co.za)");
            if (emailCls.SendLegacyEmail())
            {
                this.ltrlStatus.Text = "Email Sent to: " + str1;
            }
            else
            {
                showMessageBox showMessageBox = new showMessageBox(this.Page, "error", "error sending email: " + (object)emailCls.myResults);
                this.ltrlStatus.Text = "Email was not sent!";
            }
            showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Email Confirmation", this.ltrlStatus.Text);
            this.upnlNewOrderItem.Update();
        }
        */

        protected void OnDataBinding_ddlToBeDeliveredBy(object sender, EventArgs e)
        {
        }

        public string GetToBeDeliveredBy(object pToBeDeliveredBy)
        {
            return pToBeDeliveredBy != null ? pToBeDeliveredBy.ToString() : "0";
        }

        public string GetItemUoMObj(object pItemID)
        {
            return pItemID == null ? string.Empty : this.GetItemUoM(Convert.ToInt32(pItemID.ToString()));
        }

        public string GetItemUoM(int pItemID)
        {
            return pItemID > 0 ? new ItemTypeTbl().GetItemUnitOfMeasure(pItemID) : string.Empty;
        }

        protected void odsOrderSummary_OnUpdated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if ((bool)e.ReturnValue)
            {
                DropDownList control1 = (DropDownList)this.dvOrderHeader.FindControl("ddlContacts");
                TextBox control2 = (TextBox)this.dvOrderHeader.FindControl("tbxRequiredByDate");
                TextBox control3 = (TextBox)this.dvOrderHeader.FindControl("tbxNotes");
                if (control1 != null && control2 != null && control3 != null)
                    this.BindRowQueryParameters();
            }
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }
        protected void MarkItemAsInvoiced()
        {
            var manager = new TrackerDotNet.BusinessLogic.OrderManager();
            manager.MarkItemAsInvoiced(
                (long)this.Session["BoundCustomerID"],
                ((DateTime)this.Session["BoundDeliveryDate"]).Date,
                (string)this.Session["BoundNotes"]);
            this.pnlOrderHeader.Update();
        }
        /*
         protected void MarkItemAsInvoiced()
        {
            new OrderTbl().UpdateSetInvoiced(true, (long)this.Session["BoundCustomerID"], ((DateTime)this.Session["BoundDeliveryDate"]).Date, (string)this.Session["BoundNotes"]);
            this.pnlOrderHeader.Update();
        }
        */
        protected void btnOrderDelivered_Click(object sender, EventArgs e)
        {
            OrderHeaderData headerData = this.Get_dvOrderHeaderData(false);
            var orderLines = new List<OrderManager.TempOrderLineData>();
            ItemTypeTbl itemTypeTbl = new ItemTypeTbl();

            foreach (GridViewRow row in this.gvOrderLines.Rows)
            {
                var itemDDL = (DropDownList)row.FindControl("ddlItemDesc");
                var qtyLbl = (Label)row.FindControl("lblQuantityOrdered");
                var packagingDDL = (DropDownList)row.FindControl("ddlPackaging");
                var orderIdLbl = (Label)row.FindControl("lblOrderID");

                var line = new OrderManager.TempOrderLineData
                {
                    ItemID = Convert.ToInt32(itemDDL.SelectedValue),
                    Qty = Convert.ToDouble(qtyLbl.Text),
                    PackagingID = Convert.ToInt32(packagingDDL.SelectedValue),
                    ServiceTypeID = itemTypeTbl.GetServiceID(Convert.ToInt32(itemDDL.SelectedValue)),
                    OriginalOrderID = Convert.ToInt32(orderIdLbl.Text)
                };
                orderLines.Add(line);
            }

            var manager = new TrackerDotNet.BusinessLogic.OrderManager();
            bool success = manager.CompleteOrderDelivery(headerData, orderLines);

            if (!success)
                this.ltrlStatus.Text = "Error deleting Temp Table";
            else
                this.Response.Redirect("OrderDone.aspx");
        }
        /*
        protected void btnOrderDelivered_Click(object sender, EventArgs e)
        {
            OrderHeaderData dvOrderHeaderData = this.Get_dvOrderHeaderData(false);
            TempOrdersData pTempOrder = new TempOrdersData();
            TempOrdersDAL tempOrdersDal = new TempOrdersDAL();
            if (!tempOrdersDal.KillTempOrdersData())
                this.ltrlStatus.Text = "Error deleting Temp Table";
            pTempOrder.HeaderData.CustomerID = dvOrderHeaderData.CustomerID;
            pTempOrder.HeaderData.OrderDate = dvOrderHeaderData.OrderDate;
            pTempOrder.HeaderData.RoastDate = dvOrderHeaderData.RoastDate;
            pTempOrder.HeaderData.RequiredByDate = dvOrderHeaderData.RequiredByDate;
            pTempOrder.HeaderData.ToBeDeliveredByID = Convert.ToInt32(dvOrderHeaderData.ToBeDeliveredBy);
            pTempOrder.HeaderData.Confirmed = dvOrderHeaderData.Confirmed;
            pTempOrder.HeaderData.Done = dvOrderHeaderData.Done;
            pTempOrder.HeaderData.Notes = dvOrderHeaderData.Notes;
            ItemTypeTbl itemTypeTbl = new ItemTypeTbl();
            foreach (GridViewRow row in this.gvOrderLines.Rows)
            {
                TempOrdersLinesTbl tempOrdersLinesTbl = new TempOrdersLinesTbl();
                DropDownList control1 = (DropDownList)row.FindControl("ddlItemDesc");
                Label control2 = (Label)row.FindControl("lblQuantityOrdered");
                DropDownList control3 = (DropDownList)row.FindControl("ddlPackaging");
                Label control4 = (Label)row.FindControl("lblOrderID");
                tempOrdersLinesTbl.ItemID = Convert.ToInt32(control1.SelectedValue);
                tempOrdersLinesTbl.Qty = Convert.ToDouble(control2.Text);
                tempOrdersLinesTbl.PackagingID = Convert.ToInt32(control3.SelectedValue);
                tempOrdersLinesTbl.ServiceTypeID = itemTypeTbl.GetServiceID(tempOrdersLinesTbl.ItemID);
                tempOrdersLinesTbl.OriginalOrderID = Convert.ToInt32(control4.Text);
                pTempOrder.OrdersLines.Add(tempOrdersLinesTbl);
            }
            tempOrdersDal.Insert(pTempOrder);
            this.Response.Redirect("OrderDone.aspx");
        }
        */
        protected void btnUnDoDone_Click(object sender, EventArgs e)
        {
            var manager = new TrackerDotNet.BusinessLogic.OrderManager();
            string empty = string.Empty;
            foreach (TableRow row in this.gvOrderLines.Rows)
            {
                Label control = (Label)row.Cells[4].FindControl("lblOrderID");
                empty += manager.UnDoOrderItem(Convert.ToInt32(control.Text));
            }
            this.ltrlStatus.Text = empty;
            this.dvOrderHeader.DataBind();
            this.pnlOrderHeader.Update();
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }
        /*
        protected void btnUnDoDone_Click(object sender, EventArgs e)
        {
            string empty = string.Empty;
            foreach (TableRow row in this.gvOrderLines.Rows)
            {
                Label control = (Label)row.Cells[4].FindControl("lblOrderID");
                empty += this.UnDoneOrderItem(control.Text);
            }
            this.ltrlStatus.Text = empty;
            this.dvOrderHeader.DataBind();
            this.pnlOrderHeader.Update();
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }
        */
        protected void gvOrderLines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool flag = false;
            if (e.CommandName == "MoveOneDayOn")
            {
                flag = true;
                Label control = (Label)this.gvOrderLines.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("lblOrderID");
                DateTime pNewDate = Convert.ToDateTime(this.dvOrderHeaderGetLabelValue("lblRequiredByDate")).Date;
                if (pNewDate.DayOfWeek < DayOfWeek.Friday)
                {
                    pNewDate = pNewDate.AddDays(1.0);
                }
                else
                {
                    int num = (int)(1 - pNewDate.DayOfWeek + 7) % 7;
                    pNewDate = pNewDate.AddDays((double)num);
                }
                new OrderTbl().UpdateOrderDeliveryDate(pNewDate, Convert.ToInt32(control.Text));
            }
            else if (e.CommandName == "DeleteOrder")
            {
                flag = true;
                this.DeleteOrderItem(e.CommandArgument.ToString());
            }
            if (!flag)
                return;
            this.dvOrderHeader.DataBind();
            this.pnlOrderHeader.Update();
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }

        protected void ddlItemDesc_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}