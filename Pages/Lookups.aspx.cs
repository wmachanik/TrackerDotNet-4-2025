// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.Lookups
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.Controls;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public partial class Lookups : Page
    {
        private const string CONST_ITEMSEARCHSESIONVAR = "SearchItemContains";
        private const int CONST_BGCOLOURCOL = 4;
        protected ScriptManager scmLookup;
        protected UpdateProgress uprgLookup;
        protected Label lblStatus;
        protected TabContainer tabcLookup;
        protected TabPanel tabpnlItems;
        protected UpdatePanel upnlItems;
        protected TextBox tbxItemSearch;
        protected Button btnGon;
        protected Button btnReset;
        protected GridView gvItems;
        protected ObjectDataSource odsItemUnits;
        protected TabPanel tabpnlPeople;
        protected UpdatePanel upnlPeople;
        protected GridView gvPeople;
        protected TabPanel tabpnlEquipment;
        protected UpdatePanel upnlEquipment;
        protected GridView gvEquipment;
        protected TabPanel tabpnlCities;
        protected UpdatePanel upnlCities;
        protected GridView gvCities;
        protected GridView gvCityDays;
        protected ObjectDataSource odsCityDays;
        protected TabPanel tabpnlPackaging;
        protected UpdatePanel UpdatePanel1;
        protected GridView gvPackaging;
        protected TabPanel tabInvoiceTypes;
        protected UpdateProgress gvInvoiceTypesUpdateProgress;
        protected UpdatePanel gvInvoiceTypesUpdatePanel;
        protected GridView gvInvoiceTypes;
        protected TabPanel tabPaymentTerms;
        protected UpdateProgress PaymentTermsUpdateProgress;
        protected UpdatePanel gvPaymentTermsUpdatePanel;
        protected GridView gvPaymentTerms;
        protected TabPanel tabPriceLevels;
        protected UpdateProgress PriceLevelUpdateProgress;
        protected UpdatePanel gvPriceLevelsUpdatePanel;
        protected GridView gvPriceLevels;
        protected SqlDataSource sdsItems;
        protected ObjectDataSource odsAllItems;
        protected ObjectDataSource odsPeople;
        protected SqlDataSource sdsUserNames;
        protected ObjectDataSource odsEquipTypes;
        protected ObjectDataSource odsInvoiceTypes;
        protected ObjectDataSource odsPaymentTerms;
        protected ObjectDataSource odsPriceLevels;
        protected SqlDataSource sdsServiceTypes;
        protected SqlDataSource sdsReplacementItems;
        protected SqlDataSource sdsCities;
        protected ObjectDataSource odsPackaging;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            this.tabcLookup.ActiveTabIndex = 0;
            this.gvCityDays.SelectedIndex = 1;
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddItem"))
                return;
            try
            {
                TextBox control1 = (TextBox)this.gvItems.FooterRow.FindControl("tbxItem");
                TextBox control2 = (TextBox)this.gvItems.FooterRow.FindControl("tbxSKU");
                CheckBox control3 = (CheckBox)this.gvItems.FooterRow.FindControl("cbxItemEnabled");
                TextBox control4 = (TextBox)this.gvItems.FooterRow.FindControl("tbxItemCharacteristics");
                TextBox control5 = (TextBox)this.gvItems.FooterRow.FindControl("tbxItemDetail");
                DropDownList control6 = (DropDownList)this.gvItems.FooterRow.FindControl("ddlServiceType");
                DropDownList control7 = (DropDownList)this.gvItems.FooterRow.FindControl("ddlReplacement");
                TextBox control8 = (TextBox)this.gvItems.FooterRow.FindControl("tbxItemShortName");
                TextBox control9 = (TextBox)this.gvItems.FooterRow.FindControl("tbxSortOrder");
                TextBox control10 = (TextBox)this.gvItems.FooterRow.FindControl("tbxUnitsPerQty");
                DropDownList control11 = (DropDownList)this.gvItems.FooterRow.FindControl("ddlUnits");
                this.sdsItems.InsertParameters.Clear();
                this.sdsItems.InsertParameters.Add("ItemDesc", DbType.String, control1.Text);
                this.sdsItems.InsertParameters.Add("SKU", DbType.String, control2.Text);
                this.sdsItems.InsertParameters.Add("ItemEnabled", DbType.Boolean, control3.Enabled.ToString());
                this.sdsItems.InsertParameters.Add("ItemCharacteristics", DbType.String, control4.Text);
                this.sdsItems.InsertParameters.Add("ItemDetail", DbType.String, control5.Text);
                this.sdsItems.InsertParameters.Add("ServiceTypeID", DbType.Int32, control6.SelectedValue.ToString());
                this.sdsItems.InsertParameters.Add("ReplacementID", DbType.Int32, control7.SelectedValue);
                this.sdsItems.InsertParameters.Add("ItemShortName", DbType.String, control8.Text);
                this.sdsItems.InsertParameters.Add("SortOrder", DbType.Int32, control9.Text);
                this.sdsItems.InsertParameters.Add("UnitsPerQty", DbType.Single, control10.Text);
                this.sdsItems.InsertParameters.Add("ItemUnitID", DbType.Int32, control11.SelectedValue);
                this.sdsItems.Insert();
                this.gvItems.DataBind();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Error adding record: " + ex.Message;
            }
        }

        protected void gvPeople_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddItem"))
                return;
            try
            {
                TextBox control1 = (TextBox)this.gvPeople.FooterRow.FindControl("tbxPerson");
                TextBox control2 = (TextBox)this.gvPeople.FooterRow.FindControl("tbxAbreviation");
                CheckBox control3 = (CheckBox)this.gvPeople.FooterRow.FindControl("cbxEnabled");
                DropDownList control4 = (DropDownList)this.gvPeople.FooterRow.FindControl("ddlDayOfWeek");
                DropDownList control5 = (DropDownList)this.gvPeople.FooterRow.FindControl("ddlSecurityNames");
                PersonsTbl pPerson = new PersonsTbl();
                pPerson.Person = control1.Text;
                pPerson.Abreviation = control2.Text;
                pPerson.Enabled = control3.Checked;
                pPerson.NormalDeliveryDoW = Convert.ToInt32(control4.SelectedValue);
                pPerson.SecurityUsername = control5.SelectedValue;
                pPerson.InsertPerson(pPerson);
                this.gvPeople.DataBind();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Error adding record: " + ex.Message;
            }
        }

        protected void dvItems_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            this.gvItems.FooterRow.Enabled = false;
            this.gvItems.DataBind();
        }

        protected void InsertItemButton_Click(object sender, EventArgs e)
        {
            this.gvItems.FooterRow.Enabled = true;
            this.gvItems.DataBind();
        }

        protected void gvEquipment_UpdateButton_Click(EventArgs e) => this.Response.Write("Do update");

        protected void sdsCities_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
        }

        protected void gvEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void gvEquipment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Insert"))
                return;
            try
            {
                TextBox control1 = (TextBox)this.gvEquipment.FooterRow.FindControl("EquipTypeNameTextBox");
                TextBox control2 = (TextBox)this.gvEquipment.FooterRow.FindControl("EquipTypeDescTextBox");
                EquipTypeTbl objEquipType = new EquipTypeTbl();
                objEquipType.EquipTypeName = control1.Text;
                objEquipType.EquipTypeDesc = control2.Text;
                objEquipType.InsertEquipObj(objEquipType);
                this.gvEquipment.DataBind();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Error adding record: " + ex.Message;
            }
        }

        protected void odsEquipTypes_OnInserting(object source, ObjectDataSourceMethodEventArgs e)
        {
            IDictionary inputParameters = (IDictionary)e.InputParameters;
            EquipTypeTbl equipTypeTbl = new EquipTypeTbl();
            equipTypeTbl.EquipTypeName = inputParameters[(object)"EquipTypeName"].ToString();
            equipTypeTbl.EquipTypeDesc = inputParameters[(object)"EquipTypeDesc"].ToString();
            inputParameters.Clear();
            inputParameters.Add((object)"objEquipType", (object)equipTypeTbl);
        }

        private void DoItemSearch()
        {
            string text = this.tbxItemSearch.Text;
            this.Session["SearchItemContains"] = !string.IsNullOrEmpty(text) ? (object)$"%{text}%" : (object)"%";
            this.sdsItems.DataBind();
            this.gvItems.DataBind();
            this.upnlItems.Update();
        }

        protected void tbxItemSearch_TextChanged(object sender, EventArgs e) => this.DoItemSearch();

        protected void btnGo_Click(object sender, EventArgs e) => this.DoItemSearch();

        protected void gvPackaging_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals((object)DataControlRowType.DataRow))
                return;
            PackagingTbl dataItem = (PackagingTbl)e.Row.DataItem;

            // Handle BGColour column (index 3)
            if (!string.IsNullOrEmpty(dataItem.BGColour))
            {
                try
                {
                    Color bgColor = ColorTranslator.FromHtml(dataItem.BGColour);
                    e.Row.Cells[3].BackColor = bgColor;

                    // Set contrasting text color for BGColour column
                    int brightness = (int)(bgColor.R * 0.299 + bgColor.G * 0.587 + bgColor.B * 0.114);
                    e.Row.Cells[3].ForeColor = brightness > 128 ? Color.Black : Color.White;
                }
                catch (Exception ex)
                {
                    this.lblStatus.Text = ex.Message;
                }
            }

            // Handle Colour column (index 4) - convert int to hex and display
            if (dataItem.Colour != 0)
            {
                try
                {
                    // Convert integer to hex color
                    Color foreColor = Color.FromArgb(dataItem.Colour);
                    string hexValue = $"#{foreColor.R:X2}{foreColor.G:X2}{foreColor.B:X2}";

                    // Set the hex value as text and apply the color as background
                    e.Row.Cells[4].Text = hexValue;
                    e.Row.Cells[4].BackColor = foreColor;

                    // Set contrasting text color for Colour column
                    int brightness = (int)(foreColor.R * 0.299 + foreColor.G * 0.587 + foreColor.B * 0.114);
                    e.Row.Cells[4].ForeColor = brightness > 128 ? Color.Black : Color.White;
                }
                catch (Exception ex)
                {
                    this.lblStatus.Text = ex.Message;
                }
            }
        }

        protected void gvPackaging_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("Insert"))
                return;
            try
            {
                TextBox controlDescription = (TextBox)this.gvPackaging.FooterRow.FindControl("TextBoxDescription");
                TextBox controlAdditionalNotes = (TextBox)this.gvPackaging.FooterRow.FindControl("TextBoxAdditionalNotes");
                TextBox controlBGColour = (TextBox)this.gvPackaging.FooterRow.FindControl("TextBoxBGColour");
                TextBox controlColour = (TextBox)this.gvPackaging.FooterRow.FindControl("TextBoxColour");
                TextBox controlSymbol = (TextBox)this.gvPackaging.FooterRow.FindControl("TextBoxSymbol");
                PackagingTbl objPackagingTbl = new PackagingTbl();
                objPackagingTbl.Description = controlDescription.Text;
                objPackagingTbl.AdditionalNotes = controlAdditionalNotes.Text;
                objPackagingTbl.BGColour = controlBGColour.Text;
                objPackagingTbl.Colour = string.IsNullOrEmpty(controlColour.Text) ? 0 : Convert.ToInt32(controlColour.Text);
                objPackagingTbl.Symbol = controlSymbol.Text;
                objPackagingTbl.InsertPackaging(objPackagingTbl);
                this.gvPackaging.DataBind();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Error adding record: " + ex.Message;
            }
        }

        protected void ColorPickerExtBGColour_OnClientColorSelectionChanged(object sender, EventArgs e)
        {
            TextBox control = (TextBox)this.gvPackaging.FindControl("TextBoxBGColour");
            control.Text = "#" + control.Text;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            this.tbxItemSearch.Text = string.Empty;
            this.DoItemSearch();
        }

        protected void gvCities_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddCity"))
                return;
            try
            {
                TextBox control = (TextBox)this.gvCities.FooterRow.FindControl("tbxCity");
                this.sdsCities.InsertParameters.Clear();
                this.sdsCities.InsertParameters.Add("City", DbType.String, control.Text);
                this.sdsCities.Insert();
                this.gvItems.DataBind();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Error adding record: " + ex.Message;
            }
        }

        protected void gvCities_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.gvCities.SelectedDataKey.Values.Count <= 0)
                return;
            this.gvCityDays.Visible = true;
        }

        protected void btnAddCity_Click(object sender, EventArgs e)
        {
            DropDownList control1 = (DropDownList)this.gvCityDays.Controls[0].Controls[0].Controls[0].FindControl("ddlPreperationDoW");
            TextBox control2 = (TextBox)this.gvCityDays.Controls[0].Controls[0].Controls[0].FindControl("tbxDeliveryDelay");
            TextBox control3 = (TextBox)this.gvCityDays.Controls[0].Controls[0].Controls[0].FindControl("tbxDeliveryOrder");
            int int32 = Convert.ToInt32(this.gvCities.SelectedDataKey.Value);
            CityPrepDaysTbl objCityPrepDaysTbl = new CityPrepDaysTbl();
            objCityPrepDaysTbl.CityID = int32;
            objCityPrepDaysTbl.PrepDayOfWeekID = Convert.ToByte(control1.SelectedValue);
            objCityPrepDaysTbl.DeliveryDelayDays = Convert.ToInt32(control2.Text);
            objCityPrepDaysTbl.DeliveryOrder = Convert.ToInt32(control3.Text);
            objCityPrepDaysTbl.InsertCityPrepDay(objCityPrepDaysTbl);
            this.gvCityDays.DataBind();
        }

        protected void gvCityDays_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
        }

        protected void gvCityDays_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Update") || e.CommandName.Equals("AddCityDays"))
            {
                GridViewRow gridViewRow = e.CommandName.Equals("Update") ? this.gvCityDays.Rows[this.gvCityDays.EditIndex] : this.gvCityDays.FooterRow;
                DropDownList control1 = (DropDownList)gridViewRow.FindControl("ddlPreperationDoW");
                TextBox control2 = (TextBox)gridViewRow.FindControl("tbxDeliveryDelay");
                TextBox control3 = (TextBox)gridViewRow.FindControl("tbxDeliveryOrder");
                var controlCityPrepDaysID = (HiddenField)gridViewRow.FindControl("CityPrepDaysIDHidden");
                int int32 = Convert.ToInt32(this.gvCities.SelectedDataKey.Value);
                CityPrepDaysTbl objCityPrepDaysTbl = new CityPrepDaysTbl();
                objCityPrepDaysTbl.CityID = int32;
                objCityPrepDaysTbl.PrepDayOfWeekID = Convert.ToByte(control1.SelectedValue);
                objCityPrepDaysTbl.DeliveryDelayDays = Convert.ToInt32(control2.Text);
                objCityPrepDaysTbl.CityPrepDaysID = Convert.ToInt32(controlCityPrepDaysID.Value);
                objCityPrepDaysTbl.DeliveryOrder = Convert.ToInt32(control3.Text);
                if (e.CommandName.Equals("Update"))
                    objCityPrepDaysTbl.UpdateCityPrepDay(objCityPrepDaysTbl);
                else
                    objCityPrepDaysTbl.InsertCityPrepDay(objCityPrepDaysTbl);
                this.gvCityDays.DataBind();
                this.upnlCities.Update();
            }
            else
            {
                if (!e.CommandName.Equals("Delete"))
                    return;
                var controlCityPrepDaysID = (HiddenField)((Control)e.CommandSource).NamingContainer.FindControl("CityPrepDaysIDHidden");
                CityPrepDaysTbl cityPrepDaysTbl = new CityPrepDaysTbl();
                cityPrepDaysTbl.CityPrepDaysID = Convert.ToInt32(controlCityPrepDaysID.Value);
                cityPrepDaysTbl.DeleteByCityPrepDayID(cityPrepDaysTbl.CityPrepDaysID);
                this.gvCityDays.DataBind();
                this.upnlCities.Update();
            }
        }

        public string GetDeliveryDay(string pPredDoW, string pDeliveryDelay)
        {
            int result1 = 0;
            int result2 = 0;
            if (!int.TryParse(pPredDoW, out result1))
                result1 = 1;
            if (!int.TryParse(pDeliveryDelay, out result2))
                result2 = 1;
            int num = result1 + result2;
            string[] strArray = new string[7]
            {"Sun",
                "Mon",
                "Tue",
                "Wed",
                "Thu",
                "Fri",
                "Sat"
            };
            if (num > 7)
                num -= 7;
            return strArray[num - 1];
        }

        protected void gvInvoiceTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow namingContainer = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            if (namingContainer == null)
                return;
            TextBox controlTypeDesc = (TextBox)namingContainer.FindControl("InvoiceTypeDescTextBox");
            if (controlTypeDesc == null || string.IsNullOrEmpty(controlTypeDesc.Text))
                return;
            InvoiceTypeTbl pInvoiceTypeTbl = new InvoiceTypeTbl();
            var controlInvoiceTypeID = (HiddenField)namingContainer.FindControl("InvoiceTypeIDHidden");
            pInvoiceTypeTbl.InvoiceTypeID = controlInvoiceTypeID != null ? Convert.ToInt32(controlInvoiceTypeID.Value) : 0;
            if (e.CommandName.Equals("Delete"))
            {
                pInvoiceTypeTbl.Delete(pInvoiceTypeTbl.InvoiceTypeID);
            }
            else
            {
                CheckBox controlEnabled = (CheckBox)namingContainer.FindControl("EnabledCheckBox");
                TextBox controlNotes = (TextBox)namingContainer.FindControl("NotesTextBox");
                pInvoiceTypeTbl.InvoiceTypeDesc = controlTypeDesc.Text;
                pInvoiceTypeTbl.Enabled = controlEnabled != null && controlEnabled.Checked;
                pInvoiceTypeTbl.Notes = controlNotes != null ? controlNotes.Text : string.Empty;
                if (e.CommandName.Equals("Add") || e.CommandName.Equals("Insert"))
                    pInvoiceTypeTbl.Insert(pInvoiceTypeTbl);
                else if (e.CommandName.Equals("Update"))
                    pInvoiceTypeTbl.Update(pInvoiceTypeTbl, pInvoiceTypeTbl.InvoiceTypeID);
            }
            this.gvInvoiceTypes.DataBind();
        }

        protected void gvPriceLevels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow namingContainer = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            if (namingContainer == null)
                return;
            TextBox controlDesc = (TextBox)namingContainer.FindControl("PriceLevelDescTextBox");
            if (controlDesc == null || string.IsNullOrEmpty(controlDesc.Text))
                return;
            TextBox controlFactor = (TextBox)namingContainer.FindControl("PricingFactorTextBox");
            CheckBox controlEnabled = (CheckBox)namingContainer.FindControl("EnabledCheckBox");
            TextBox controlNotes = (TextBox)namingContainer.FindControl("NotesTextBox");
            var controlID = (HiddenField)namingContainer.FindControl("hdnPriceLevelID");
            PriceLevelsTbl pPriceLevelsTbl = new PriceLevelsTbl();
            pPriceLevelsTbl.PriceLevelDesc = controlDesc.Text;
            pPriceLevelsTbl.PricingFactor = controlFactor != null ? (double)Convert.ToSingle(controlFactor.Text) : 1.0;
            pPriceLevelsTbl.Enabled = controlEnabled != null && controlEnabled.Checked;
            pPriceLevelsTbl.Notes = controlNotes != null ? controlNotes.Text : string.Empty;
            pPriceLevelsTbl.PriceLevelID = controlID != null ? Convert.ToInt32(controlID.Value) : 0;
            if (e.CommandName.Equals("Add") || e.CommandName.Equals("Insert"))
                pPriceLevelsTbl.Insert(pPriceLevelsTbl);
            else if (e.CommandName.Equals("Update"))
                pPriceLevelsTbl.Update(pPriceLevelsTbl, pPriceLevelsTbl.PriceLevelID);
            else if (e.CommandName.Equals("Delete"))
                pPriceLevelsTbl.Delete(pPriceLevelsTbl.PriceLevelID);
            this.gvPriceLevels.DataBind();
        }

        protected void gvPaymentTerms_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow namingContainer = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            if (namingContainer == null)
                return;
            var controlDesc = (TextBox)namingContainer.FindControl("PaymentTermDescTextBox");
            if (controlDesc == null || string.IsNullOrEmpty(controlDesc.Text))
                return;
            var controlPaymentDays = (TextBox)namingContainer.FindControl("PaymentDaysTextBox");
            var controlDayOfMonth = (TextBox)namingContainer.FindControl("DayOfMonthTextBox");
            var controlUseDays = (CheckBox)namingContainer.FindControl("UseDaysCheckBox");
            var controlEnabled = (CheckBox)namingContainer.FindControl("EnabledCheckBox");
            var controlNotes = (TextBox)namingContainer.FindControl("NotesTextBox");
            var controlPaymentTermID = (HiddenField)namingContainer.FindControl("PaymentTermIDHidden");
            PaymentTermsTbl pPaymentTermsTbl = new PaymentTermsTbl();
            pPaymentTermsTbl.PaymentTermDesc = controlDesc.Text;
            pPaymentTermsTbl.PaymentDays = controlPaymentDays != null ? Convert.ToInt32(controlPaymentDays.Text) : 0;
            pPaymentTermsTbl.DayOfMonth = controlDayOfMonth != null ? Convert.ToInt32(controlDayOfMonth.Text) : 0;
            pPaymentTermsTbl.UseDays = controlUseDays != null && controlUseDays.Checked;
            pPaymentTermsTbl.Enabled = controlEnabled == null || controlEnabled.Checked;
            pPaymentTermsTbl.Notes = controlNotes != null ? controlNotes.Text : string.Empty;
            pPaymentTermsTbl.PaymentTermID = controlPaymentTermID != null ? Convert.ToInt32(controlPaymentTermID.Value) : 0;
            if (e.CommandName.Equals("Add") || e.CommandName.Equals("Insert"))
                pPaymentTermsTbl.Insert(pPaymentTermsTbl);
            else if (e.CommandName.Equals("Update"))
                pPaymentTermsTbl.Update(pPaymentTermsTbl, pPaymentTermsTbl.PaymentTermID);
            else if (e.CommandName.Equals("Delete"))
                pPaymentTermsTbl.Delete(pPaymentTermsTbl.PaymentTermID);
            this.gvPaymentTerms.DataBind();
        }
    }
}