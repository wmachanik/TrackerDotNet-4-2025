// Type: TrackerDotNet.Pages.DeliverySheet
// Type: TrackerDotNet.Pages.DeliverySheet
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public partial class DeliverySheet : Page
    {
        public const string CONST_SESSION_SHEETDATE = "DeliverySheetDate";
        public const string CONST_SESSION_DELIVERTBY = "DeliverySheetDeliveryBy";
        public const string CONST_SESSION_DDLSHEETDATE_SELECTED = "DeliverySheetDateItemSelected";
        public const string CONST_SESSION_DDLDELIVERTBY_SELECTED = "DeliverySheetDeliveryByItemSelected";
        public const string CONST_SESSION_SHEETISPRINTING = "SheetIsPrinting";
        private const int CONST_ONLYAFEWDELIVERIES = 9;
        private const int CONST_ALOTOFDELIVERIES = 21;
        private const string CONST_ZZNAME_PREFIX = "_*:";
        private const string CONST_IS_ZZNAME = "ZZ";
        protected ScriptManager smDelivery;
        protected Panel pnlDeliveryDate;
        protected UpdateProgress uprgDeliveryFilterBy;
        protected UpdatePanel upnlDeliveryFilterBy;
        protected DropDownList ddlActiveRoastDates;
        protected Button btnGo;
        protected Button btnRefresh;
        protected Label lblDeliveryBy;
        protected DropDownList ddlDeliveryBy;
        protected TextBox tbxFindClient;
        protected Button btnFind;
        protected Button btnPrint;
        protected HyperLink hlAddDeliveryItem;
        protected ObjectDataSource odsActiveRoastDates;
        protected UpdatePanel upnlDeliveryItems;
        protected Table tblDeliveries;
        protected TableHeaderCell thcReceivedBy;
        protected TableHeaderCell thcSignature;
        protected TableHeaderCell thcInStock;
        protected Table tblTotals;
        protected Label ltrlWhichDate;

        private void Page_PreInit(object sender, EventArgs e)
        {
            bool flag1 = false;
            bool flag2 = new CheckBrowser().fBrowserIsMobile();
            this.Session["RunningOnMoble"] = (object)flag2;
            if (this.Request.QueryString["Print"] != null)
                flag1 = this.Request.QueryString["Print"].ToString() == "Y";
            if (flag1)
            {
                this.MasterPageFile = "~/Print.master";
                this.Session["SheetIsPrinting"] = (object)"Y";
            }
            else
            {
                //this.Session["RunningOnMoble"] = (object)flag2;
                this.MasterPageFile = "~/Site.master";
                this.Session["SheetIsPrinting"] = (object)"N";
            }
        }

        protected void PageInitialize(bool pPrintForm)
        {
            this.btnPrint.Visible = !pPrintForm;
            this.pnlDeliveryDate.Visible = !pPrintForm;
            this.ltrlWhichDate.Visible = !pPrintForm;
            string pActiveDeliveryDate = this.Request.QueryString["DateValue"] == null ? "" : this.Request.QueryString["DateValue"];
            string pOnlyDeliveryBy = this.Request.QueryString["DeliveryBy"] == null ? "" : this.Request.QueryString["DeliveryBy"];
            if (string.IsNullOrEmpty(pActiveDeliveryDate) && this.Session["DeliverySheetDate"] != null)
            {
                pActiveDeliveryDate = (string)this.Session["DeliverySheetDate"];
                this.ltrlWhichDate.Text = pActiveDeliveryDate;
            }
            if (string.IsNullOrEmpty(pOnlyDeliveryBy) && this.Session["DeliverySheetDeliveryBy"] != null)
                pOnlyDeliveryBy = (string)this.Session["DeliverySheetDeliveryBy"];
            if (string.IsNullOrEmpty(pActiveDeliveryDate))
                return;
            this.BuildDeliverySheet(pPrintForm, pActiveDeliveryDate, pOnlyDeliveryBy);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool pPrintForm = this.Request.QueryString["Print"] != null && this.Request.QueryString["Print"].ToString() == "Y";
            if (this.Session["DeliverySheetDateItemSelected"] == null)
                this.Session["DeliverySheetDateItemSelected"] = (object)$"{TimeZoneUtils.Now().Date}";
            if (!this.IsPostBack)
            {
                Button control = (Button)this.pnlDeliveryDate.FindControl("btnFind");
                if (control != null)
                    this.Form.DefaultButton = control.UniqueID;
                this.PageInitialize(pPrintForm);
            }
            if (pPrintForm)
                return;
            this.tblDeliveries.Rows[0].Cells[2].Visible = false;
            this.tblDeliveries.Rows[0].Cells[3].Visible = false;
            this.tblDeliveries.Rows[0].Cells[5].Visible = false;
            if ((bool)this.Session["RunningOnMoble"])
                return;
            TableCellCollection cells = this.tblDeliveries.Rows[0].Cells;
            TableHeaderCell tableHeaderCell = new TableHeaderCell();
            tableHeaderCell.Text = "Action";
            TableHeaderCell cell = tableHeaderCell;
            cells.Add((TableCell)cell);
        }

        protected void BuildDeliverySheet()
        {
            if (this.ltrlWhichDate.Text.Length > 0)
            {
                string text = this.ltrlWhichDate.Text;
            }
            this.BuildDeliverySheet(false, this.ltrlWhichDate.Text.Length > 0 ? this.ltrlWhichDate.Text : "2012-01-01", this.ddlDeliveryBy.Items.Count <= 1 || this.ddlDeliveryBy.SelectedIndex <= 0 ? "" : this.ddlDeliveryBy.SelectedValue);
        }

        protected void BuildDeliverySheet(bool pPrintForm, string pActiveDeliveryDate, string pOnlyDeliveryBy)
        {
            this.Session["DeliverySheetDeliveryBy"] = pOnlyDeliveryBy;

            // Complete SQL with all columns
            string strSQL = @"SELECT DISTINCT OrdersTbl.OrderID, CustomersTbl.CompanyName AS CoName, OrdersTbl.CustomerID, " +
                              "OrdersTbl.OrderDate, OrdersTbl.RoastDate,OrdersTbl.ItemTypeID, ItemTypeTbl.ItemDesc, " +
                              "OrdersTbl.QuantityOrdered, ItemTypeTbl.ItemShortName, ItemTypeTbl.ItemEnabled, " +
                              "ItemTypeTbl.ReplacementID,  CityPrepDaysTbl.DeliveryOrder,  ItemTypeTbl.SortOrder, " +
                              "OrdersTbl.RequiredByDate, OrdersTbl.ToBeDeliveredBy, OrdersTbl.PurchaseOrder, OrdersTbl.Confirmed," +
                              "OrdersTbl.InvoiceDone, OrdersTbl.Done, OrdersTbl.Notes, PackagingTbl.Description AS PackDesc, " +
                              "PackagingTbl.BGColour, PersonsTbl.Abreviation " +
                              "FROM ( ( " +
                                       "( CityPrepDaysTbl RIGHT OUTER JOIN CustomersTbl ON CityPrepDaysTbl.CityID = CustomersTbl.City )" +
                                        "RIGHT OUTER JOIN " +
                                        "( OrdersTbl LEFT OUTER JOIN PersonsTbl ON OrdersTbl.ToBeDeliveredBy = PersonsTbl.PersonID)" +
                                        " ON CustomersTbl.CustomerID = OrdersTbl.CustomerID" +
                                      ") LEFT OUTER JOIN PackagingTbl ON OrdersTbl.PackagingID = PackagingTbl.PackagingID) " +
                                    " LEFT OUTER JOIN ItemTypeTbl ON OrdersTbl.ItemTypeID = ItemTypeTbl.ItemTypeID " +
                              "WHERE (OrdersTbl.RequiredByDate = ?)";

            // Add date parameter using utility method
            if (!DateTime.TryParse(pActiveDeliveryDate, out DateTime reqDate))
            {
                reqDate = DateTime.Today;
            }
            using (TrackerDb trackerDb = new TrackerDb())
            {

                trackerDb.AddWhereParams(reqDate, DbType.DateTime);

                // Add delivery person filter if specified
                if (!string.IsNullOrEmpty(pOnlyDeliveryBy))
                {
                    strSQL += " AND OrdersTbl.ToBeDeliveredBy = ?";

                    if (int.TryParse(pOnlyDeliveryBy, out int deliveryById))
                    {
                        trackerDb.AddWhereParams(deliveryById, DbType.Int32);
                    }
                    else
                    {
                        trackerDb.AddWhereParams(DBNull.Value, DbType.Int32);
                    }
                }

                // Complete SQL
                strSQL += @" ORDER BY OrdersTbl.RequiredByDate, OrdersTbl.ToBeDeliveredBy, CityPrepDaysTbl.DeliveryOrder," +
                            "CustomersTbl.CompanyName, ItemTypeTbl.SortOrder";

                // Execute the query
                using (IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL))
                {
                    this.BuildDeliveryTable(dataReader, pPrintForm);
                }
                // Explicit close still happens through TrackerDb.Dispose()
            }

            //IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            //this.BuildDeliveryTable(dataReader, pPrintForm);
            //dataReader.Close();
            //trackerDb.Close();
        }

        private string StripEmailOut(string pNotes)
        {
            int length = pNotes.IndexOf("[#");
            if (length >= 0)
            {
                int num = pNotes.IndexOf("#]");
                if (num >= 0)
                    pNotes = $"{pNotes.Substring(0, length)};{pNotes.Substring(num + 2)}";
            }
            return pNotes;
        }

        private void BuildDeliveryTable(IDataReader pDataReader, bool pPrintForm)
        {
            while (1 < this.tblDeliveries.Rows.Count)
                this.tblDeliveries.Rows.RemoveAt(1);
            this.tblTotals.Rows.Clear();
            List<DeliverySheet.deliveryItems> deliveryItemsList = new List<DeliverySheet.deliveryItems>();
            SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
            string str1 = "";
            Dictionary<string, DeliverySheet.ItemTotals> source = new Dictionary<string, DeliverySheet.ItemTotals>();
            string[] strArray = new string[8]
            {
      "",
      "dN",
      "d#",
      "g$",
      "cS",
      "s@",
      "!!",
      "??"
            };
            CustomersAccInfoTbl customersAccInfoTbl = new CustomersAccInfoTbl();
            int num1 = 0;
            while (pDataReader.Read())
            {
                bool flag = false;
                DeliverySheet.deliveryItems deliveryItems1 = new DeliverySheet.deliveryItems();
                deliveryItems1.ContactID = pDataReader["CustomerID"].ToString();
                deliveryItems1.ContactCompany = pDataReader["CoName"].ToString();
                if (deliveryItems1.ContactCompany.StartsWith("ZZName"))
                {
                    deliveryItems1.ContactID = "ZZ";
                    flag = true;
                    string pNotes = pDataReader["Notes"].ToString();
                    if (pNotes.Contains(":"))
                        pNotes = pNotes.Remove(pNotes.IndexOf(":")).Trim();
                    string str2 = this.StripEmailOut(pNotes);
                    deliveryItems1.ContactCompany = "_*: " + str2;
                }
                else if (deliveryItems1.ContactCompany.StartsWith("Stock"))
                    deliveryItems1.ContactCompany = "STK: " + pDataReader["Notes"].ToString();
                if (pDataReader["Notes"].ToString().StartsWith("+"))
                {
                    DeliverySheet.deliveryItems deliveryItems2 = deliveryItems1;
                    deliveryItems2.ContactCompany = $"{deliveryItems2.ContactCompany}[{pDataReader["Notes"].ToString()}]";
                }
                if (!deliveryItems1.ContactID.Equals("ZZ"))
                {
                    long result = 0;
                    if (long.TryParse(deliveryItems1.ContactID, out result))
                    {
                        int customersInvoiceType = customersAccInfoTbl.GetCustomersInvoiceType(result);
                        if (customersInvoiceType > 1)
                            deliveryItems1.ContactCompany = $"{strArray[customersInvoiceType - 1]}]> {deliveryItems1.ContactCompany}";
                    }
                }
                deliveryItems1.Done = pDataReader["Done"] != DBNull.Value && (bool)pDataReader["Done"];
                if (deliveryItems1.Done)
                    deliveryItems1.ContactCompany = "<b>DONE</b>-> " + deliveryItems1.ContactCompany;
                if (!pPrintForm)
                {
                    if (!sortedDictionary.ContainsKey(pDataReader["ToBeDeliveredBy"].ToString()))
                        sortedDictionary[pDataReader["ToBeDeliveredBy"].ToString()] = pDataReader["Abreviation"].ToString();
                    deliveryItems1.OrderDetailURL = $"{this.ResolveUrl("~/Pages/OrderDetail.aspx")}?{$"CustomerID={HttpContext.Current.Server.UrlEncode(pDataReader["CustomerID"].ToString())}&DeliveryDate={pDataReader["RequiredByDate"]:d}&Notes={HttpContext.Current.Server.UrlEncode(pDataReader["Notes"].ToString())}"}";
                }
                deliveryItems1.Details = $"{pDataReader["RequiredByDate"]:d}, {pDataReader["Abreviation"]}";
                deliveryItems1.InvoiceDone = pDataReader["InvoiceDone"] != DBNull.Value && (bool)pDataReader["InvoiceDone"];
                deliveryItems1.PurchaseOrder = pDataReader["PurchaseOrder"] == DBNull.Value ? string.Empty : pDataReader["PurchaseOrder"].ToString();
                string key = pDataReader["ItemTypeID"].ToString();
                string str3 = pDataReader["ItemShortName"].ToString().Length > 0 ? pDataReader["ItemShortName"].ToString() : pDataReader["ItemDesc"].ToString();
                string str4 = str3;
                if (!bool.Parse(pDataReader["ItemEnabled"].ToString()))
                {
                    str3 = "<span style='background-color: RED; color: WHITE'>SOLD OUT</span> " + str3;
                    str4 = $">{str4}<";
                }
                int num2 = pDataReader["SortOrder"] == DBNull.Value ? 0 : (int)pDataReader["SortOrder"];
                if (num2 == 10)
                {
                    string pNotes = pDataReader["Notes"].ToString();
                    if (flag && pNotes.Contains(":"))
                        pNotes = pNotes.Substring(pNotes.IndexOf(":") + 1).Trim();
                    string str5 = this.StripEmailOut(pNotes);
                    str3 = $"{str3}: {str5}";
                }
                if (pDataReader["PackDesc"].ToString().Length > 0)
                    deliveryItems1.Items += $"<span style='background-color:{pDataReader["BGColour"]}; padding-top: 1px; padding-bottom:2px'>{pDataReader["QuantityOrdered"]}X{str3} ({pDataReader["PackDesc"]})</span>";
                else
                    deliveryItems1.Items += $"<span style='background-color:{pDataReader["BGColour"]}'>{pDataReader["QuantityOrdered"]}X{str3}</span>";
                if (num2 != 10)
                {
                    if (source.ContainsKey(key))
                    {
                        source[key].TotalsQty += Convert.ToDouble(pDataReader["QuantityOrdered"]);
                    }
                    else
                    {
                        if (str3.Contains(":"))
                            str1 = str3.Remove(str3.IndexOf(":"));
                        source[key] = new DeliverySheet.ItemTotals()
                        {
                            ItemID = key,
                            ItemDesc = str4,
                            TotalsQty = Convert.ToDouble(pDataReader["QuantityOrdered"].ToString()),
                            ItemOrder = pDataReader["SortOrder"] == DBNull.Value ? 0 : Convert.ToInt32(pDataReader["SortOrder"].ToString())
                        };
                    }
                }
                deliveryItemsList.Add(deliveryItems1);
                ++num1;
            }
            pDataReader.Close();
            for (int index1 = 0; index1 < num1; ++index1)
            {
                if (deliveryItemsList[index1].ContactCompany.StartsWith("_*:"))
                {
                    for (int index2 = index1 + 2; index2 < num1; ++index2)
                    {
                        if (deliveryItemsList[index2].ContactCompany.Equals(deliveryItemsList[index1].ContactCompany))
                        {
                            DeliverySheet.deliveryItems deliveryItems = deliveryItemsList[index2];
                            deliveryItemsList.RemoveAt(index2);
                            deliveryItemsList.Insert(index1 + 1, deliveryItems);
                        }
                    }
                }
            }
            int index = 0;
            while (index < num1)
            {
                TableRow row = new TableRow();
                TableCell cell1 = new TableCell();
                cell1.Text = deliveryItemsList[index].Details;
                if (pPrintForm)
                {
                    cell1.Font.Size = FontUnit.XSmall;
                    cell1.Text = cell1.Text.Remove(0, cell1.Text.IndexOf(",") + 1);
                }
                else
                    cell1.Text = $"<a class='plain' href='{deliveryItemsList[index].OrderDetailURL}'>{cell1.Text.Trim()}</a>";
                row.Cells.Add(cell1);
                TableCell cell2 = new TableCell();
                if (pPrintForm)
                {
                    string str6 = deliveryItemsList[index].ContactCompany;
                    if (str6.Contains("]>"))
                    {
                        int num3 = str6.IndexOf("]>");
                        str6 = str6.Substring(num3 + 3);
                    }
                    cell2.Text = str6;
                }
                else if (deliveryItemsList[index].ContactID == "ZZ")
                {
                    cell2.Text = deliveryItemsList[index].ContactCompany;
                }
                else
                {
                    string contactCompany = deliveryItemsList[index].ContactCompany;
                    if (contactCompany.Contains("]>"))
                    {
                        int length = contactCompany.IndexOf("]>");
                        cell2.Text = $"{contactCompany.Substring(0, length)} - <a href='./CustomerDetails.aspx?ID={deliveryItemsList[index].ContactID}&'>{contactCompany.Substring(length + 3)}</a>";
                    }
                    else
                        cell2.Text = $"<a href='./CustomerDetails.aspx?ID={deliveryItemsList[index].ContactID}&'>{contactCompany}</a>";
                }
                row.Cells.Add(cell2);
                if (pPrintForm)
                {
                    TableCell cell3 = new TableCell();
                    cell3.BorderStyle = BorderStyle.Solid;
                    cell3.BorderWidth = Unit.Pixel(1);
                    cell3.BorderColor = Color.Green;
                    row.Cells.Add(cell3);
                    TableCell cell4 = new TableCell();
                    cell4.BorderStyle = BorderStyle.Solid;
                    cell4.BorderWidth = Unit.Pixel(1);
                    cell4.BorderColor = Color.Green;
                    row.Cells.Add(cell4);
                }
                TableCell cell5 = new TableCell();
                if (!string.IsNullOrWhiteSpace(deliveryItemsList[index].PurchaseOrder))
                    cell5.Text = $"<b>[PO: {deliveryItemsList[index].PurchaseOrder}]</b>";
                if (!pPrintForm && deliveryItemsList[index].InvoiceDone)
                {
                    TableCell tableCell = cell5;
                    tableCell.Text = $"{tableCell.Text}{(string.IsNullOrEmpty(cell5.Text) ? "" : " ")}<span style='background-color:green; color: white'>$Invcd$</span>";
                }
                string format = "<span  style='vertical-align:middle'> <a  href='{0}' class='plain'><img src='../images/imgButtons/EditButton.gif' alt='edit' /></a>";
                if (!deliveryItemsList[index].InvoiceDone)
                    format += "&nbsp<a href='{0}&Invoiced=Y' class='plain'><img src='../images/imgButtons/InvoicedButton.gif' alt='invcd' /></a></span>";
                if (!deliveryItemsList[index].Done)
                    format += "&nbsp<a href='{0}&Delivered=Y' class='plain'><img src='../images/imgButtons/DoneButton.gif' alt='dlvrd' /></a></span>";
                string str7 = string.Format(format, (object)deliveryItemsList[index].OrderDetailURL);
                do
                {
                    TableCell tableCell = cell5;
                    tableCell.Text = tableCell.Text + (string.IsNullOrEmpty(cell5.Text) ? "" : "; ") + deliveryItemsList[index].Items.ToString();
                    ++index;
                }
                while (index < num1 && deliveryItemsList[index - 1].ContactCompany == deliveryItemsList[index].ContactCompany);
                row.Cells.Add(cell5);
                if (pPrintForm)
                    row.Cells.Add(new TableCell());
                bool flag = (bool)this.Session["RunningOnMoble"];
                if (!pPrintForm && !flag)
                    row.Cells.Add(new TableCell() { Text = str7 });
                this.tblDeliveries.Rows.Add(row);
            }
            Style s = new Style();
            if (this.tblDeliveries.Rows.Count < 9)
                s.Height = new Unit(4.5, UnitType.Em);
            else if (this.tblDeliveries.Rows.Count > 21)
            {
                s.Height = new Unit(0.3, UnitType.Em);
                s.Font.Size = new FontUnit(11.0, UnitType.Pixel);
            }
            else
                s.Height = new Unit(2.0, UnitType.Em);
            foreach (TableRow row in this.tblDeliveries.Rows)
                row.Cells[0].ApplyStyle(s);
            this.tblDeliveries.Rows[0].Cells[1].Text = $"To ({this.tblDeliveries.Rows.Count - 1})";
            Dictionary<string, DeliverySheet.ItemTotals> dictionary = source.OrderBy<KeyValuePair<string, DeliverySheet.ItemTotals>, int>((System.Func<KeyValuePair<string, DeliverySheet.ItemTotals>, int>)(entry => entry.Value.ItemOrder)).ToDictionary<KeyValuePair<string, DeliverySheet.ItemTotals>, string, DeliverySheet.ItemTotals>((System.Func<KeyValuePair<string, DeliverySheet.ItemTotals>, string>)(pair => pair.Key), (System.Func<KeyValuePair<string, DeliverySheet.ItemTotals>, DeliverySheet.ItemTotals>)(pair => pair.Value));
            TableRow row1 = (TableRow)new TableHeaderRow();
            TableRow row2 = new TableRow();
            TableHeaderCell cell6 = new TableHeaderCell();
            cell6.Text = "Item";
            cell6.Font.Bold = true;
            row1.Cells.Add((TableCell)cell6);
            TableCell cell7 = new TableCell();
            cell7.Text = "Total";
            cell7.Font.Bold = true;
            row2.Cells.Add(cell7);
            foreach (KeyValuePair<string, DeliverySheet.ItemTotals> keyValuePair in dictionary)
            {
                TableHeaderCell cell8 = new TableHeaderCell();
                cell8.Text = keyValuePair.Value.ItemDesc;
                cell8.Font.Bold = true;
                row1.Cells.Add((TableCell)cell8);
                row2.Cells.Add(new TableCell()
                {
                    Text = $"{keyValuePair.Value.TotalsQty:0.00}",
                    HorizontalAlign = HorizontalAlign.Right
                });
            }
            this.tblTotals.Rows.Add(row1);
            this.tblTotals.Rows.Add(row2);
            if (pPrintForm)
            {
                this.tblTotals.CssClass += " small";
            }
            else
            {
                bool flag = sortedDictionary.Count > 1;
                this.ddlDeliveryBy.Items.Clear();
                this.ddlDeliveryBy.Visible = flag;
                this.lblDeliveryBy.Visible = flag;
                if (flag)
                {
                    this.ddlDeliveryBy.Items.Add(new ListItem()
                    {
                        Text = "--- All ---",
                        Value = "%",
                        Selected = true
                    });
                    foreach (KeyValuePair<string, string> keyValuePair in sortedDictionary)
                        this.ddlDeliveryBy.Items.Add(new ListItem()
                        {
                            Text = keyValuePair.Value,
                            Value = keyValuePair.Key
                        });
                }
            }
            this.upnlDeliveryItems.Update();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.ddlActiveRoastDates == null || this.ddlActiveRoastDates.SelectedIndex <= 0)
                return;
            this.Session["DeliverySheetDate"] = (object)$"{Convert.ToDateTime(this.ddlActiveRoastDates.SelectedValue):yyyy-MM-dd}";
            this.Response.Redirect("~/Pages/DeliverySheet.aspx?Print=Y");
        }

        protected void ddlActiveRoastDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["DeliverySheetDateItemSelected"] = ddlActiveRoastDates.SelectedValue;
            this.ltrlWhichDate.Text = $"{Convert.ToDateTime(this.ddlActiveRoastDates.SelectedValue):yyyy-MM-dd}";
            this.Session["DeliverySheetDate"] = (object)this.ltrlWhichDate.Text;
            this.Session["DeliverySheetDeliveryBy"] = (object)string.Empty;
            this.Session["DeliverySheetDeliveryByItemSelected"] = (object)string.Empty;
            this.ltrlWhichDate.Visible = true;
            if (string.IsNullOrEmpty(this.ltrlWhichDate.Text))
                return;
            Button control = (Button)this.pnlDeliveryDate.FindControl("btnGo");
            if (control != null)
                this.Form.DefaultButton = control.UniqueID;
            this.SetVarsAndBuildDeliverySheet();
        }
        protected void tbCalendarDate_TextChanged(object sender, EventArgs e)
        {
            string selectedDate = tbCalendarDate.Text.Trim();
            if (DateTime.TryParse(selectedDate, out DateTime dt))
            {
                string value = dt.ToString("yyyy-MM-dd");
                var item = ddlActiveRoastDates.Items.FindByValue(value);
                if (item == null)
                {
                    // Add new date to dropdown (insert after the first item)
                    ddlActiveRoastDates.Items.Insert(1, new ListItem(dt.ToString("dd-MMM-yyyy (ddd)"), value));
                    ddlActiveRoastDates.SelectedIndex = 1;
                }
                else
                {
                    ddlActiveRoastDates.ClearSelection();
                    item.Selected = true;
                }
                // Update session and UI
                ddlActiveRoastDates_SelectedIndexChanged(ddlActiveRoastDates, EventArgs.Empty);
            }
        }
        protected void ddlActiveRoastDates_DataBound(object sender, EventArgs e)
        {
            // Only set the selected value if not a postback
            if (!IsPostBack)
            {
                string str1 = Session["DeliverySheetDateItemSelected"] != null ? ((string)Session["DeliverySheetDateItemSelected"]).Trim() : "";
                if (!string.IsNullOrEmpty(str1) && ddlActiveRoastDates.Items.FindByValue(str1) != null)
                {
                    ddlActiveRoastDates.SelectedValue = str1;
                }
                SetVarsAndBuildDeliverySheet();
            }
            
        }
            //bool flag = false;
            //string str1 = this.Session["DeliverySheetDateItemSelected"] != null ? ((string)this.Session["DeliverySheetDateItemSelected"]).Trim() : "";
            //string str2 = this.Session["DeliverySheetDeliveryByItemSelected"] != null ? (string)this.Session["DeliverySheetDeliveryByItemSelected"] : "";
            //if (!string.IsNullOrEmpty(str1) && this.ddlActiveRoastDates.Items.FindByValue(str1) != null)
            //{
            //    this.ddlActiveRoastDates.SelectedValue = str1;
            //    flag = true;
            //}
            //if (!string.IsNullOrEmpty(str2) && this.ddlDeliveryBy.Items.FindByValue(str2) != null)
            //{
            //    this.ddlDeliveryBy.SelectedValue = str2;
            //    flag = true;
            //}
            //if (!flag)
            //    return;
            //this.SetVarsAndBuildDeliverySheet();
        //}

        protected void ddlDeliveryBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Session["DeliverySheetDeliveryByItemSelected"] = (object)this.ddlDeliveryBy.SelectedValue;
            this.BuildDeliverySheet();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Session["DeliverySheetDeliveryBy"] = (object)string.Empty;
            this.Session["DeliverySheetDeliveryByItemSelected"] = (object)string.Empty;
            this.Session["DeliverySheetDate"] = (object)string.Empty;
            this.Response.Redirect("DeliverySheet.aspx");
        }

        protected void SetVarsAndBuildDeliverySheet()
        {
            this.ltrlWhichDate.Text = $"{Convert.ToDateTime(this.ddlActiveRoastDates.SelectedValue):yyyy-MM-dd}";
            this.Session["DeliverySheetDate"] = (object)this.ltrlWhichDate.Text;
            this.BuildDeliverySheet();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (this.ddlActiveRoastDates == null || this.ddlActiveRoastDates.SelectedIndex <= 0)
                return;
            this.SetVarsAndBuildDeliverySheet();
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            string strSQL = $"SELECT DISTINCT OrdersTbl.OrderID, CustomersTbl.CompanyName AS CoName, OrdersTbl.CustomerID, OrdersTbl.OrderDate, OrdersTbl.RoastDate, OrdersTbl.ItemTypeID, ItemTypeTbl.ItemDesc, OrdersTbl.QuantityOrdered, ItemTypeTbl.ItemShortName, ItemTypeTbl.ItemEnabled, ItemTypeTbl.ReplacementID,  CityPrepDaysTbl.DeliveryOrder,  ItemTypeTbl.SortOrder, OrdersTbl.RequiredByDate, OrdersTbl.ToBeDeliveredBy, OrdersTbl.PurchaseOrder, OrdersTbl.Confirmed, OrdersTbl.InvoiceDone, OrdersTbl.Done, OrdersTbl.Notes, PackagingTbl.Description AS PackDesc, PackagingTbl.BGColour, PersonsTbl.Abreviation FROM ((((CityPrepDaysTbl RIGHT OUTER JOIN CustomersTbl ON CityPrepDaysTbl.CityID = CustomersTbl.City) RIGHT OUTER JOIN  (OrdersTbl LEFT OUTER JOIN PersonsTbl ON OrdersTbl.ToBeDeliveredBy = PersonsTbl.PersonID) ON CustomersTbl.CustomerID = OrdersTbl.CustomerID) LEFT OUTER JOIN   PackagingTbl ON OrdersTbl.PackagingID = PackagingTbl.PackagingID) LEFT OUTER JOIN ItemTypeTbl ON OrdersTbl.ItemTypeID = ItemTypeTbl.ItemTypeID) WHERE (CustomersTbl.CompanyName LIKE '%{this.tbxFindClient.Text}%') AND (OrdersTbl.Done = false) ORDER BY OrdersTbl.RequiredByDate, OrdersTbl.ToBeDeliveredBy, CityPrepDaysTbl.DeliveryOrder, CustomersTbl.CompanyName, ItemTypeTbl.SortOrder";
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            this.BuildDeliveryTable(dataReader, false);
            dataReader.Close();
            trackerDb.Close();
        }

        protected void tbxFindClient_OnTextChanged(object sender, EventArgs e)
        {
            Button control = (Button)this.pnlDeliveryDate.FindControl("btnFind");
            if (control != null)
                this.Form.DefaultButton = control.UniqueID;
            this.btnFind_Click(sender, e);
        }

        private class deliveryItems
        {
            private string _ContactID;
            private string _ContactCompany;
            private string _Details;
            private string _PurchaseOrder;
            private bool _Done;
            private bool _InvoiceDone;
            private string _Items;
            private string _OrderDetailURL;

            public deliveryItems()
            {
                this._ContactID = this._ContactCompany = this._Details = this._PurchaseOrder = this._Items = this._OrderDetailURL = string.Empty;
                this._Done = this._InvoiceDone = false;
            }

            public string ContactID
            {
                get => this._ContactID;
                set => this._ContactID = value;
            }

            public string ContactCompany
            {
                get => this._ContactCompany;
                set => this._ContactCompany = value;
            }

            public string Details
            {
                get => this._Details;
                set => this._Details = value;
            }

            public string PurchaseOrder
            {
                get => this._PurchaseOrder;
                set => this._PurchaseOrder = value;
            }

            public bool Done
            {
                get => this._Done;
                set => this._Done = value;
            }

            public bool InvoiceDone
            {
                get => this._InvoiceDone;
                set => this._InvoiceDone = value;
            }

            public string Items
            {
                get => this._Items;
                set => this._Items = value;
            }

            public string OrderDetailURL
            {
                get => this._OrderDetailURL;
                set => this._OrderDetailURL = value;
            }
        }

        private class ItemTotals
        {
            public string ItemID { get; set; }

            public string ItemDesc { get; set; }

            public double TotalsQty { get; set; }

            public int ItemOrder { get; set; }
        }
    }
}