// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.NewOrderDetail
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public partial class NewOrderDetail : Page
    {
        public const string CONST_ZZNAME_DEFAULTID = "9";
        private const string CONST_DELIVERY_DEFAULT = "SQ";
        private const string CONST_UPDATELINES = "UpdateOrderLines";
        private const string CONST_LINESADDED = "OrderLinesAdded";
        private const string CONST_ORDERLINEIDS = "OrderLineIDS";
        private const string CONST_ORDERLINEITEMIDS = "OrderLineItemIDS";
        private const string CONST_WATERFILTER = "8ClarFltr";
        private const string CONST_BLUEWATERFILTER = "8ClarBlue";
        private const int CONST_ORDERIDCOL = 4;
        private const int CONST_NOTEITEMTIMEID = 100;
        public const string CONST_URL_REQUEST_CustomerID = "CoID";
        public const string CONST_URL_REQUEST_NAME = "Name";
        public const string CONST_URL_REQUEST_COMPANYNAME = "CoName";
        public const string CONST_URL_REQUEST_EMAIL = "EMail";
        public const string CONST_URL_REQUEST_LASTORDER = "LastOrder";
        public const string CONST_URL_REQUEST_SKU1 = "SKU1";
        protected ScriptManager smgrOrderDetails;
        protected UpdatePanel upnlNewOrder;
        protected UpdatePanel upnlOrderSummary;
        protected HyperLink IDCustomerHdr;
        protected DropDownList ddlContacts;
        protected TextBox tbxOrderDate;
        protected CalendarExtender tbxOrderDate_CalendarExtender;
        protected TextBox tbxRoastDate;
        protected CalendarExtender tbxRoastDate_CalendarExtender;
        protected DropDownList ddlToBeDeliveredBy;
        protected TextBox tbxRequiredByDate;
        protected CalendarExtender tbxRequiredByDate_CalendarExtender;
        protected TextBox tbxPurchaseOrder;
        protected CheckBox cbxConfirmed;
        protected CheckBox cbxInvoiceDone;
        protected CheckBox cbxDone;
        protected TextBox tbxNotes;
        protected Button btnUpdate;
        protected UpdatePanel upnlOrderLines;
        protected GridView gvOrderLines;
        protected UpdatePanel upnlNewOrderItem;
        protected Button btnNewItem;
        protected Literal ltrlStatus;
        protected Panel pnlNewItem;
        protected DropDownList ddlNewItemDesc;
        protected TextBox tbxNewQuantityOrdered;
        protected DropDownList ddlNewPackaging;
        protected Button btnAdd;
        protected Button btnCancel;
        protected UpdatePanel updtButtonPanel;
        protected Button btnAddLastOrder;
        protected Button btnDeliverySheet;
        protected Button btnCheckDetails;
        protected Button btnRefreshDetails;
        protected Button btnCancelled;
        protected UpdateProgress updprgOrderDetail;
        protected SqlDataSource sdsCompanys;
        protected ObjectDataSource odsOrderDetail;
        protected SqlDataSource sdsDeliveryBy;
        protected SqlDataSource sdsItems;
        protected SqlDataSource sdsPackagingTypes;
        protected Label lblCustomerID;
        protected Label lblDeliveryDate;
        protected Timer tmrOrderItem;

        protected void SetContactByID(string pCoNameID)
        {
            if (string.IsNullOrEmpty(pCoNameID))
                return;
            if (this.ddlContacts.Items.FindByValue(pCoNameID) != null)
            {
                this.ddlContacts.SelectedValue = pCoNameID;
                TrackerTools.ContactPreferedItems contactPreferedItems = new TrackerTools().RetrieveCustomerPrefs(Convert.ToInt32(pCoNameID));
                if (this.ddlToBeDeliveredBy.Items.FindByValue(contactPreferedItems.PreferredDeliveryByID.ToString()) == null)
                    return;
                this.ddlToBeDeliveredBy.SelectedValue = contactPreferedItems.PreferredDeliveryByID.ToString();
            }
            else
            {
                this.ddlContacts.SelectedValue = "9";
                TextBox tbxNotes = this.tbxNotes;
                tbxNotes.Text = $"{tbxNotes.Text}ID note found: {pCoNameID}: ";
            }
        }

        protected void SetContactValue(string pCoName, string pName, string pEmail)
        {
            int index1 = 0;
            if (pCoName == null)
                pCoName = "";
            if (pName == null)
                pName = "";
            while (index1 < this.ddlContacts.Items.Count && !pCoName.Equals(this.ddlContacts.Items[index1].Text, StringComparison.OrdinalIgnoreCase))
                ++index1;
            if (index1 < this.ddlContacts.Items.Count && pCoName.Equals(this.ddlContacts.Items[index1].Text, StringComparison.OrdinalIgnoreCase))
            {
                this.ddlContacts.SelectedValue = this.ddlContacts.Items[index1].Value;
            }
            else
            {
                pCoName = $"{pCoName}_{pCoName}";
                while (index1 < this.ddlContacts.Items.Count && !pCoName.Equals(this.ddlContacts.Items[index1].Text, StringComparison.OrdinalIgnoreCase))
                    ++index1;
                if (index1 < this.ddlContacts.Items.Count && pCoName.Equals(this.ddlContacts.Items[index1].Text, StringComparison.OrdinalIgnoreCase))
                    this.ddlContacts.SelectedValue = this.ddlContacts.Items[index1].Value;
                else if (pCoName != pName)
                {
                    int index2 = 0;
                    while (index2 < this.ddlContacts.Items.Count && !pName.Equals(this.ddlContacts.Items[index2].Text, StringComparison.OrdinalIgnoreCase))
                        ++index2;
                    if (index2 < this.ddlContacts.Items.Count && pName.Equals(this.ddlContacts.Items[index2].Text, StringComparison.OrdinalIgnoreCase))
                    {
                        this.ddlContacts.SelectedValue = this.ddlContacts.Items[index2].Value;
                    }
                    else
                    {
                        List<CustomersTbl> customerWithEmailLike = new CustomersTbl().GetAllCustomerWithEmailLIKE(pEmail);
                        if (customerWithEmailLike.Count > 0)
                        {
                            this.SetContactByID(customerWithEmailLike[0].CustomerID.ToString());
                        }
                        else
                        {
                            this.ddlContacts.SelectedValue = "9";
                            if (string.IsNullOrEmpty(pCoName))
                            {
                                TextBox tbxNotes = this.tbxNotes;
                                tbxNotes.Text = $"{tbxNotes.Text}{pName}: ";
                            }
                            else
                            {
                                TextBox tbxNotes = this.tbxNotes;
                                tbxNotes.Text = $"{tbxNotes.Text}{pCoName}, {pName}: ";
                            }
                            if (!string.IsNullOrEmpty(pEmail))
                            {
                                TextBox tbxNotes = this.tbxNotes;
                                tbxNotes.Text = $"{tbxNotes.Text} [#{pEmail}#]";
                            }
                        }
                    }
                }
                else
                {
                    this.ddlContacts.SelectedValue = "9";
                    TextBox tbxNotes = this.tbxNotes;
                    tbxNotes.Text = $"{tbxNotes.Text}{pCoName}: ";
                }
            }
            if (this.ddlContacts.SelectedIndex <= 0 || !(this.ddlContacts.SelectedValue != "9"))
                return;
            TrackerTools.ContactPreferedItems contactPreferedItems = new TrackerTools().RetrieveCustomerPrefs(Convert.ToInt32(this.ddlContacts.SelectedValue));
            if (this.ddlToBeDeliveredBy.Items.FindByValue(contactPreferedItems.PreferredDeliveryByID.ToString()) == null)
                return;
            this.ddlToBeDeliveredBy.SelectedValue = contactPreferedItems.PreferredDeliveryByID.ToString();
        }

        protected void SetButtonState(bool pState)
        {
            this.btnCheckDetails.Enabled = pState;
            this.updtButtonPanel.Update();
        }

        protected bool AddLastOrder() => this.AddLastOrder(false);

        protected bool AddLastOrder(bool pSetDates)
        {
            bool flag = this.Session["OrderLinesAdded"] != null && (bool)this.Session["OrderLinesAdded"];
            if (this.ddlContacts.SelectedValue != null)
            {
                this.SetUpdateBools();
                long int64 = Convert.ToInt32(this.ddlContacts.SelectedValue);
                if (pSetDates)
                    this.SetPrepAndDeliveryValues(int64);
                List<ItemUsageTbl> lastItemsUsed = new ItemUsageTbl().GetLastItemsUsed(int64, 2);
                if (lastItemsUsed.Count > 0)
                {
                    foreach (ItemUsageTbl itemUsageTbl in lastItemsUsed)
                    {
                        if (itemUsageTbl.ItemProvidedID > 0)
                        {
                            flag = this.AddNewOrderLine(itemUsageTbl.ItemProvidedID, itemUsageTbl.AmountProvided, itemUsageTbl.PackagingID) || flag;
                            if (!string.IsNullOrEmpty(itemUsageTbl.Notes))
                                this.tbxNotes.Text += $"{(this.tbxNotes.Text.Length > 0 ? (object)"; " : (object)"")}last order used a group item, so next item in group selected.";
                        }
                    }
                }
                else
                {
                    TrackerTools.ContactPreferedItems contactPreferedItems = new TrackerTools().RetrieveCustomerPrefs(int64);
                    flag = this.AddNewOrderLine(contactPreferedItems.PreferedItem, contactPreferedItems.PreferedQty, contactPreferedItems.PrefPackagingID) || flag;
                }
                this.Session["OrderLinesAdded"] = (object)flag;
            }
            return flag;
        }

        protected OrderDetailData GetNewOrderItemFromSKU(string pSKU, double pSKUQTY)
        {
            string strSQL = "SELECT ItemTypeID, SKU, ItemEnabled FROM ItemTypeTbl WHERE (SKU = ?)";
            OrderDetailData orderItemFromSku = (OrderDetailData)null;
            if (this.ddlContacts.SelectedValue != null)
            {
                TrackerDb trackerDb = new TrackerDb();
                if (pSKU == "8ClarBlue")
                    trackerDb.AddWhereParams((object)"8ClarFltr", DbType.String);
                else
                    trackerDb.AddWhereParams((object)pSKU, DbType.String);
                IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
                if (dataReader != null)
                {
                    if (dataReader.Read() && dataReader["ItemTypeID"] != null)
                    {
                        string str = dataReader["ItemTypeID"].ToString();
                        int num = pSKUQTY < 1.0 ? 2 : 0;
                        if (!num.Equals(0) && !this.tbxNotes.Text.Contains("Please check packing setting"))
                            this.tbxNotes.Text += this.tbxNotes.Text.Length > 0 ? " " : "Please check packing setting";
                        switch (pSKU)
                        {
                            case "8ClarBlue":
                                num = 9;
                                break;
                            case "8ClarFltr":
                                num = 8;
                                break;
                        }
                        orderItemFromSku = new OrderDetailData()
                        {
                            ItemTypeID = Convert.ToInt32(str),
                            QuantityOrdered = pSKUQTY,
                            PackagingID = num
                        };
                    }
                    else
                    {
                        TextBox tbxNotes = this.tbxNotes;
                        tbxNotes.Text = $"{tbxNotes.Text}SKU Not Found: {pSKU} QTY: {pSKUQTY.ToString()}";
                    }
                    dataReader.Dispose();
                }
                trackerDb.Close();
            }
            return orderItemFromSku;
        }

        private void BindRowQueryParameters()
        {
            this.Session["BoundCustomerID"] = (object)Convert.ToInt32(this.ddlContacts.SelectedValue);
            this.Session["BoundDeliveryDate"] = (object)Convert.ToDateTime(this.tbxRequiredByDate.Text).Date.Date;
            this.Session["BoundNotes"] = (object)this.tbxNotes.Text;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            DateTime dateTime = DateTime.Now.Date;
            this.tbxOrderDate.Text = dateTime.ToShortDateString();
            dateTime = DateTime.Now.Date;
            int num = dateTime.DayOfWeek <= DayOfWeek.Tuesday || dateTime.DayOfWeek >= DayOfWeek.Friday ? (dateTime.DayOfWeek >= DayOfWeek.Wednesday ? (dateTime.DayOfWeek >= DayOfWeek.Friday ? (int)(8 - dateTime.DayOfWeek) : (int)(3 - dateTime.DayOfWeek)) : (int)(1 - dateTime.DayOfWeek)) : (int)(3 - dateTime.DayOfWeek);
            dateTime = dateTime.AddDays((double)num);
            this.tbxRoastDate.Text = dateTime.ToShortDateString();
            this.tbxRequiredByDate.Text = dateTime.DayOfWeek >= DayOfWeek.Friday ? dateTime.AddDays(3.0).ToShortDateString() : dateTime.AddDays(1.0).ToShortDateString();
            bool flag1 = false;
            bool flag2 = false;
            this.Session["UpdateOrderLines"] = (object)flag1;
            this.Session["OrderLinesAdded"] = (object)flag2;
            this.BindRowQueryParameters();
        }

        private void Page_Unload(object sender, EventArgs e)
        {
        }

        private void RedirectToOrderDetail()
        {
            this.Response.Redirect($"{this.ResolveUrl("~/Pages/OrderDetail.aspx")}?{$"CustomerID={HttpContext.Current.Server.UrlEncode(this.ddlContacts.SelectedValue)}&DeliveryDate={Convert.ToDateTime(this.tbxRequiredByDate.Text):d}&Notes={HttpContext.Current.Server.UrlEncode(this.tbxNotes.Text)}"}");
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (this.IsPostBack || this.Request.QueryString.Count <= 0)
                return;
            if (this.Request.QueryString["CoID"] != null)
                this.SetContactByID(this.Request.QueryString["CoID"]);
            else if (this.Request.QueryString["Name"] != null)
                this.SetContactValue(this.Request.QueryString["CoName"], this.Request.QueryString["Name"], this.Request.QueryString["EMail"]);
            if (this.Request.QueryString["LastOrder"] != null && this.Request.QueryString["LastOrder"] == "Y" && this.AddLastOrder(true))
                this.RedirectToOrderDetail();
            if (this.Request.QueryString["SKU1"] == null)
                return;
            if (this.ddlContacts != null && Convert.ToInt32(this.ddlContacts.SelectedValue) > 0L)
                this.SetPrepAndDeliveryValues(Convert.ToInt32(this.ddlContacts.SelectedValue));
            List<OrderDetailData> orderDetailDataList = new List<OrderDetailData>();
            bool flag = false;
            int num;
            for (num = 1; this.Request.QueryString["SKU" + num.ToString()] != null; ++num)
            {
                OrderDetailData orderItemFromSku = this.GetNewOrderItemFromSKU(this.Request.QueryString["SKU" + (object)num], Convert.ToDouble(this.Request.QueryString["SKUQTY" + (object)num]));
                if (orderItemFromSku != null)
                    orderDetailDataList.Add(orderItemFromSku);
                else if (!flag)
                {
                    flag = true;
                    orderDetailDataList.Add(new OrderDetailData()
                    {
                        ItemTypeID = 100,
                        QuantityOrdered = 1.0,
                        PackagingID = 0
                    });
                }
            }
            this.tbxNotes.Text += $">{num - 1} items added";
            foreach (OrderDetailData orderDetailData in orderDetailDataList)
                this.AddNewOrderLine(orderDetailData.ItemTypeID, orderDetailData.QuantityOrdered, orderDetailData.PackagingID);
            this.Session["OrderLinesAdded"] = (object)true;
            this.RedirectToOrderDetail();
        }

        public void DeleteOrderItem(string pOrderID)
        {
            string str = new OrderTbl().DeleteOrderById(Convert.ToInt32(pOrderID));
            this.ltrlStatus.Text = str.Length == 0 ? "Item deleted" : str;
        }

        protected void gvOrderLines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("DeleteItem"))
                return;
            Convert.ToInt32(e.CommandArgument);
            this.DeleteOrderItem(e.CommandArgument.ToString());
            this.gvOrderLines.DataBind();
        }

        protected void btnNewItem_Click(object sender, EventArgs e)
        {
            this.btnAdd.Visible = true;
            this.btnCancel.Visible = true;
            this.pnlNewItem.Visible = true;
            this.btnNewItem.Visible = false;
            this.upnlNewOrderItem.Update();
        }

        private void UpdateDataDisplay()
        {
            this.gvOrderLines.DataBind();
            this.upnlNewOrderItem.Update();
            this.upnlOrderLines.Update();
        }

        private void HideNewOrderItemPanel()
        {
            this.btnAdd.Visible = false;
            this.btnCancel.Visible = false;
            this.pnlNewItem.Visible = false;
            this.btnNewItem.Visible = true;
            this.upnlNewOrderItem.Update();
            this.UpdateDataDisplay();
        }

        protected bool AddNewOrderLine(int pNewItemID, double pNewQuantityOrdered, int pNewPackagingID)
        {
            OrderTblData pOrderData = new OrderTblData();
            OrderTbl orderTbl = new OrderTbl();
            pOrderData.CustomerID = Convert.ToInt32(this.ddlContacts.SelectedValue);
            pOrderData.OrderDate = Convert.ToDateTime(this.tbxOrderDate.Text).Date;
            pOrderData.RoastDate = Convert.ToDateTime(this.tbxRoastDate.Text).Date;
            pOrderData.RequiredByDate = Convert.ToDateTime(this.tbxRequiredByDate.Text).Date;
            pOrderData.ToBeDeliveredBy = Convert.ToInt32(this.ddlToBeDeliveredBy.SelectedValue);
            pOrderData.PurchaseOrder = this.tbxPurchaseOrder.Text;
            pOrderData.Confirmed = Convert.ToBoolean(this.cbxConfirmed.Checked);
            pOrderData.InvoiceDone = this.cbxInvoiceDone.Checked;
            pOrderData.Done = Convert.ToBoolean(this.cbxDone.Checked);
            pOrderData.Notes = this.tbxNotes.Text;
            this.Session["BoundOldDeliveryDate"] = (object)pOrderData.RequiredByDate.Date;
            TrackerTools trackerTools = new TrackerTools();
            pOrderData.ItemTypeID = trackerTools.ChangeItemIfGroupToNextItemInGroup(pOrderData.CustomerID, pNewItemID, pOrderData.RequiredByDate);
            pOrderData.QuantityOrdered = pNewQuantityOrdered;
            pOrderData.PackagingID = pNewPackagingID;
            string str = orderTbl.InsertNewOrderLine(pOrderData);
            this.BindRowQueryParameters();
            return string.IsNullOrEmpty(str);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int int32 = Convert.ToInt32(this.ddlNewItemDesc.SelectedValue);
            bool flag = this.AddNewOrderLine(int32, Convert.ToDouble(this.tbxNewQuantityOrdered.Text), Convert.ToInt32(this.ddlNewPackaging.SelectedValue));
            if (int32.Equals(36) && !this.tbxNotes.Text.Contains("Should this not rather be a repair?"))
                this.tbxNotes.Text += this.tbxNotes.Text.Length > 0 ? " " : "Should this not rather be a repair?";
            this.SetButtonState(flag || this.btnCancel.Enabled);
            this.Session["OrderLinesAdded"] = (object)flag;
            this.ltrlStatus.Text = flag ? "Item Added" : "Error adding item";
            this.HideNewOrderItemPanel();
        }

        protected void btnCancel_Click(object sender, EventArgs e) => this.HideNewOrderItemPanel();

        protected virtual void dvOrderHeader_OnItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            this.UpdateDataDisplay();
        }

        protected void ddlToBeDeliveredBy_OnDataBound(object sender, EventArgs e)
        {
            int index = 0;
            while (index < this.ddlToBeDeliveredBy.Items.Count && this.ddlToBeDeliveredBy.Items[index].Text != "SQ")
                ++index;
            if (index >= this.ddlToBeDeliveredBy.Items.Count)
                return;
            this.ddlToBeDeliveredBy.SelectedValue = this.ddlToBeDeliveredBy.Items[index].Value;
        }

        protected void btnAddLastOrder_Click(object sender, EventArgs e)
        {
            this.AddLastOrder();
            this.SetButtonState(true);
            this.UpdateDataDisplay();
        }

        protected void btnCancelled_Click(object sender, EventArgs e)
        {
            if (!(Membership.GetUser().UserName.ToLower() == "warren"))
                return;
            foreach (Control row in this.gvOrderLines.Rows)
                this.DeleteOrderItem(((Label)row.FindControl("lblOrderID")).Text);
            this.Response.Redirect("DeliverySheet.aspx");
        }

        protected void tbxNotes_TextChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void gvOrderLines_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void btnRefreshDetails_Click(object sender, EventArgs e)
        {
            this.UpdateDataDisplay();
            this.ddlContacts.DataBind();
        }

        protected void tmrOrderItem_OnTick(object sender, EventArgs e)
        {
            if (this.Session["OrderLinesAdded"] != null && (bool)this.Session["OrderLinesAdded"])
                this.UpdateDataDisplay();
            this.tmrOrderItem.Enabled = false;
        }

        protected void SetUpdateBools()
        {
            if (this.ddlContacts.SelectedIndex > 0)
            {
                if (this.ddlContacts.SelectedValue.Equals("9") && string.IsNullOrEmpty(this.tbxNotes.Text))
                {
                    this.btnNewItem.Enabled = false;
                    this.upnlNewOrderItem.Update();
                }
                else
                {
                    if (!this.btnNewItem.Enabled)
                    {
                        this.btnNewItem.Enabled = true;
                        this.upnlNewOrderItem.Update();
                    }
                    if (!this.btnAddLastOrder.Enabled)
                    {
                        this.btnAddLastOrder.Enabled = true;
                        this.updtButtonPanel.Update();
                    }
                }
            }
            bool flag1 = this.Session["OrderLinesAdded"] != null && (bool)this.Session["OrderLinesAdded"];
            bool flag2 = this.Session["UpdateOrderLines"] != null && (bool)this.Session["UpdateOrderLines"];
            if (!flag1 || flag2)
                return;
            bool flag3 = true;
            this.btnUpdate.Visible = flag3;
            this.upnlOrderSummary.Update();
            this.Session["UpdateOrderLines"] = (object)flag3;
            if (this.btnRefreshDetails.Enabled)
                return;
            this.btnRefreshDetails.Enabled = true;
            this.updtButtonPanel.Update();
        }

        protected void DoHeaderUpdate()
        {
            List<string> pOrders = (List<string>)this.Session["OrderLineIDS"];
            List<int> intList = (List<int>)this.Session["OrderLineItemIDS"];
            if (pOrders.Count > 0)
            {
                OrderDataControl orderDataControl = new OrderDataControl();
                OrderHeaderData pOrderHeader = new OrderHeaderData();
                pOrderHeader.CustomerID = Convert.ToInt32(this.ddlContacts.SelectedValue);
                pOrderHeader.OrderDate = Convert.ToDateTime(this.tbxOrderDate.Text);
                pOrderHeader.RoastDate = Convert.ToDateTime(this.tbxRoastDate.Text);
                pOrderHeader.ToBeDeliveredBy = Convert.ToInt32(this.ddlToBeDeliveredBy.SelectedValue);
                pOrderHeader.RequiredByDate = Convert.ToDateTime(this.tbxRequiredByDate.Text);
                pOrderHeader.Confirmed = this.cbxConfirmed.Checked;
                pOrderHeader.Done = this.cbxDone.Checked;
                pOrderHeader.InvoiceDone = this.cbxInvoiceDone.Checked;
                pOrderHeader.PurchaseOrder = this.tbxPurchaseOrder.Text;
                pOrderHeader.Notes = this.tbxNotes.Text;
                orderDataControl.UpdateOrderHeader(pOrderHeader, pOrders);
                DateTime pOldDeliveryDate = pOrderHeader.RequiredByDate;
                if (this.Session["BoundOldDeliveryDate"] != null)
                    pOldDeliveryDate = ((DateTime)this.Session["BoundOldDeliveryDate"]).Date;
                if (!pOldDeliveryDate.Equals(pOrderHeader.RequiredByDate))
                {
                    UsedItemGroupTbl usedItemGroupTbl = new UsedItemGroupTbl();
                    foreach (int pItemTypeID in intList)
                        usedItemGroupTbl.UpdateIfGroupItem(pOrderHeader.CustomerID, pItemTypeID, pOldDeliveryDate, pOrderHeader.RequiredByDate);
                    this.Session["BoundOldDeliveryDate"] = (object)pOrderHeader.RequiredByDate.Date;
                }
            }
            this.BindRowQueryParameters();
        }

        private TrackerTools.ContactPreferedItems SetPrepAndDeliveryValues(long pCustomerID)
        {
            DateTime date = DateTime.Now.Date;
            TrackerTools trackerTools = new TrackerTools();
            DateTime dateByCustomerID = trackerTools.GetNextRoastDateByCustomerID(pCustomerID, ref date);
            TrackerTools.ContactPreferedItems contactPreferedItems = trackerTools.RetrieveCustomerPrefs(pCustomerID);
            this.tbxRoastDate.Text = $"{dateByCustomerID:d}";
            this.tbxRequiredByDate.Text = $"{date:d}";
            int num = new PersonsTbl().IsNormalDeliveryDoW(contactPreferedItems.PreferredDeliveryByID, (int)(date.DayOfWeek + 1)) ? contactPreferedItems.PreferredDeliveryByID : 3;
            if (!num.Equals(contactPreferedItems.PreferredDeliveryByID))
            {
                TextBox tbxNotes = this.tbxNotes;
                tbxNotes.Text = $"{tbxNotes.Text}{(this.tbxNotes.Text.Length > 0 ? " " : "")}!!!default delivery person changed due to DoW calculation";
            }
            if (this.ddlToBeDeliveredBy.Items.FindByValue(num.ToString()) != null)
                this.ddlToBeDeliveredBy.SelectedValue = num.ToString();
            if (contactPreferedItems.RequiresPurchOrder)
                this.tbxPurchaseOrder.Text = "!!!PO required!!!";
            return contactPreferedItems;
        }

        protected void ddlContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TrackerTools.ContactPreferedItems contactPreferedItems = this.SetPrepAndDeliveryValues(Convert.ToInt32(((ListControl)sender).SelectedValue));
            if (this.ddlNewItemDesc.SelectedIndex == -1)
            {
                if (this.ddlNewItemDesc.Items.FindByValue(contactPreferedItems.PreferedItem.ToString()) != null)
                    this.ddlNewItemDesc.SelectedValue = contactPreferedItems.PreferedItem.ToString();
                this.tbxNewQuantityOrdered.Text = contactPreferedItems.PreferedQty.ToString();
            }
            this.upnlOrderSummary.Update();
            this.SetUpdateBools();
        }

        protected void tbxOrderDate_TextChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void tbxRoastDate_TextChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void ddlToBeDeliveredBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetUpdateBools();
        }

        protected void tbxRequiredByDate_TextChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void tbxPurchaseOrder_TextChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void cbxConfirmed_CheckedChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void cbxInvoiceDone_CheckedChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void cbxDone_CheckedChanged(object sender, EventArgs e) => this.SetUpdateBools();

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.DoHeaderUpdate();
            this.btnUpdate.Visible = false;
            this.Session["UpdateOrderLines"] = (object)false;
        }

        protected void gvOrderLines_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            this.gvOrderLines.DataBind();
            this.upnlOrderLines.Update();
        }

        protected void gvOrderLines_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                List<string> stringList = new List<string>();
                List<int> intList = new List<int>();
                this.Session["OrderLineIDS"] = (object)stringList;
                this.Session["OrderLineItemIDS"] = (object)intList;
            }
            else
            {
                if (e.Row.RowType != DataControlRowType.DataRow)
                    return;
                List<string> stringList = (List<string>)this.Session["OrderLineIDS"] ?? new List<string>();
                Label control1 = (Label)e.Row.FindControl("lblOrderId");
                stringList.Add(control1.Text);
                this.Session["OrderLineIDS"] = (object)stringList;
                List<int> intList = (List<int>)this.Session["OrderLineItemIDS"] ?? new List<int>();
                DropDownList control2 = (DropDownList)e.Row.FindControl("ddlItemDesc");
                intList.Add(Convert.ToInt32(control2.SelectedValue));
                this.Session["OrderLineItemIDS"] = (object)intList;
            }
        }

        protected void btnCheckDetails_Click(object sender, EventArgs e) => this.RedirectToOrderDetail();
    }
}