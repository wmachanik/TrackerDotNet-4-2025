// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.SendCoffeeCheckup
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

//- only form later versions #nullable disable
namespace TrackerDotNet.Pages
{
    public class SendCoffeeCheckup : Page
    {
        private const string CONST_SESSIONVAR_EXISTINGORDERS = "ExistingOrdersData";
        private const string CONST_PREPDATEMERGE = "[#PREPDATE#]";
        private const string CONST_DELIVERYDATE = "[#DELIVERYDATE#]";
        private const int CONST_FORCEREMINDERDELAYCOUNT = 4;
        private const int CONST_MAXREMINDERS = 7;
        private const string CONST_SUMMARYTABLEDEF = "<table border='0' style='border: 1px solid'>";
        private const string CONST_SUMMARYTABLEHEADER = "<thead><tr><td style='background-color: #EFFFE9; font-weight: bold'>{0}</td><td style='background-color: #EFFFE9;' colspan='2'>{1}</td></thead>";
        private const string CONST_SUMMARYTABLEBODYSTART = "<tbody>";
        private const string CONST_SUMMARYTABLEBODYROWSTART = "<tr>";
        private const string CONST_SUMMARYTABLEBODYROWEND = "</tr>";
        private const string CONST_SUMMARYTABLEBODYEND = "</tbody></table>";
        private const string CONST_SUMMARYTABLECELL1 = "<td style='background-color: #E9FFE0'>{1}</td>";
        private const string CONST_SUMMARYTABLECELL2 = "<td style='background-color: #E9FFE0'>{2}</td>";
        private const string CONST_SUMMARYTABLECELLBOLD = "<td style='background-color: #E9FFE0; color: #593020; font-weight: bold'>{0}</td>";
        private const string CONST_SUMMARYTABLECELL2COL = "<td style='background-color: #E9FFE0; text-align:center' colspan='2'>{1}</td>";
        private const string CONST_SUMMARYTABLECELL3COL = "<td style='background-color: #E9FFE0; text-align:center' colspan='3'>{0}</td>";
        private const string CONST_SUMMARYTABLBODYROW = "<tr><td style='background-color: #E9FFE0; color: #593020; font-weight: bold'>{0}</td><td style='background-color: #E9FFE0'>{1}</td><td style='background-color: #E9FFE0'>{2}</td></tr>";
        private const string CONST_SUMMARYTABLBODYROW2COL = "<tr><td style='background-color: #E9FFE0; color: #593020; font-weight: bold'>{0}</td><td style='background-color: #E9FFE0; text-align:center' colspan='2'>{1}</td></tr>";
        private const string CONST_SUMMARYTABLBODYROW3COL = "<tr><td style='background-color: #E9FFE0; text-align:center' colspan='3'>{0}</td></tr>";
        private const string CONST_SUMMARYTABLEALTCELL1 = "<td style='background-color: #CFEFC9'>{1}</td>";
        private const string CONST_SUMMARYTABLEALTCELL2 = "<td style='background-color: #CFEFC9'>{2}</td>";
        private const string CONST_SUMMARYTABLEALTCELLBOLD = "<td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{0}</td>";
        private const string CONST_SUMMARYTABLEALTCELL2COL = "<td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='2'>{1}</td>";
        private const string CONST_SUMMARYTABLEALTCELL3COL = "<td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='3'>{0}</td>";
        private const string CONST_SUMMARYTABLBODYALTROW = "<tr><td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{0}</td><td style='background-color: #CFEFC9'>{1}</td><td style='background-color: #CFEFC9'>{2}</td></tr>";
        private const string CONST_SUMMARYTABLBODYALTROW2COL = "<tr><td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{0}</td><td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='2'>{1}</td></tr>";
        private const string CONST_SUMMARYTABLBODYALTROW3COL = "<tr><td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='3'>{0}</td></tr>";
        protected ScriptManager smCustomerCheckup;
        protected UpdateProgress uprgSendEmail;
        protected UpdateProgress uprgCustomerCheckup;
        protected UpdatePanel upnlSendEmail;
        protected TextBox tbxEmailSubject;
        protected Literal ltrlEmailTextID;
        protected TabContainer tabEmailBody;
        protected TabPanel tpnlEmailIntro;
        protected TextBox tbxEmailIntro;
        protected HtmlEditorExtender HtmlEditorExtenderEmailIntro;
        protected TabPanel tpnlEmailBody;
        protected TextBox tbxEmailBody;
        protected HtmlEditorExtender HtmlEditorExtenderEmailBody;
        protected TabPanel tpnlEmailFooter;
        protected TextBox tbxEmailFooter;
        protected HtmlEditorExtender HtmlEmailFooter;
        protected Literal ltrlStatus;
        protected Button btnPrepData;
        protected Button btnUpdate;
        protected Button btnReload;
        protected Button btnSend;
        protected Button btnRefreshCustomerCheckupList;
        protected UpdatePanel upnlCustomerCheckup;
        protected GridView gvCustomerCheckup;
        protected UpdatePanel upnlContactItems;
        protected GridView gvItemsToConfirm;
        protected ObjectDataSource odsContactsToSendCheckup;
        protected ObjectDataSource odsContactToBeRemindedItems;

        private List<ContactToRemindWithItems> GetReocurringContacts()
        {
            List<ContactToRemindWithItems> reocurringContacts = new List<ContactToRemindWithItems>();
            TrackerTools trackerTools = new TrackerTools();
            DateTime minValue1 = DateTime.MinValue;
            DateTime minValue2 = DateTime.MinValue;
            DateTime minReminderDate = new SysDataTbl().GetMinReminderDate();
            ReoccuringOrderDAL reoccuringOrderDal = new ReoccuringOrderDAL();
            if (!reoccuringOrderDal.SetReoccuringItemsLastDate())
            {
                showMessageBox showMessageBox = new showMessageBox(this.Page, "Set Reoccuring Item Last Date", "Could not set the re-occuring last date");
                return reocurringContacts;
            }
            List<ReoccuringOrderExtData> all = reoccuringOrderDal.GetAll(1, "CustomersTbl.CustomerID");
            for (int index1 = 0; index1 < all.Count; ++index1)
            {
                switch (all[index1].ReoccuranceTypeID)
                {
                    case 1:
                        all[index1].NextDateRequired = all[index1].DateLastDone.AddDays((double)(all[index1].ReoccuranceValue * 7)).Date;
                        break;
                    case 5:
                        all[index1].NextDateRequired = all[index1].DateLastDone.AddMonths(1).Date;
                        all[index1].NextDateRequired = new DateTime(all[index1].NextDateRequired.Year, all[index1].NextDateRequired.Month, all[index1].ReoccuranceValue).Date;
                        break;
                }
                if (all[index1].RequireUntilDate > TrackerTools.STATIC_TrackerMinDate && all[index1].NextDateRequired > all[index1].RequireUntilDate)
                    all[index1].Enabled = false;
                DateTime sourceDateTime = trackerTools.GetNextRoastDateByCustomerID(all[index1].CustomerID, ref minValue1);
                if (sourceDateTime < minReminderDate)
                    sourceDateTime = minReminderDate;
                if (all[index1].NextDateRequired <= sourceDateTime)
                {
                    List<OrderCheckData> similarItemInOrders = new OrderCheck().GetSimilarItemInOrders(all[index1].CustomerID, all[index1].ItemRequiredID, sourceDateTime.GetFirstDayOfWeek(), sourceDateTime.GetLastDayOfWeek());
                    if (similarItemInOrders == null)
                    {
                        ItemContactRequires _ItemRequired = new ItemContactRequires();
                        _ItemRequired.CustomerID = all[index1].CustomerID;
                        _ItemRequired.AutoFulfill = false;
                        _ItemRequired.ReoccurID = all[index1].ReoccuringOrderID;
                        _ItemRequired.ReoccurOrder = true;
                        _ItemRequired.ItemID = all[index1].ItemRequiredID;
                        _ItemRequired.ItemQty = all[index1].QtyRequired;
                        _ItemRequired.ItemPackagID = all[index1].PackagingID;
                        if (!reocurringContacts.Exists((Predicate<ContactToRemindWithItems>)(x => x.CustomerID == _ItemRequired.CustomerID)))
                        {
                            ContactToRemindWithItems customerDetails = new ContactToRemindWithItems().GetCustomerDetails(_ItemRequired.CustomerID);
                            customerDetails.ItemsContactRequires.Add(_ItemRequired);
                            reocurringContacts.Add(customerDetails);
                        }
                        else
                        {
                            int index2 = reocurringContacts.FindIndex((Predicate<ContactToRemindWithItems>)(x => x.CustomerID == _ItemRequired.CustomerID));
                            reocurringContacts[index2].ItemsContactRequires.Add(_ItemRequired);
                        }
                    }
                    else
                    {
                        List<OrderCheckData> orderCheckDataList = this.Session["ExistingOrdersData"] != null ? (List<OrderCheckData>)this.Session["ExistingOrdersData"] : new List<OrderCheckData>();
                        for (int index3 = 0; index3 < similarItemInOrders.Count; ++index3)
                            orderCheckDataList.Add(similarItemInOrders[index3]);
                    }
                }
            }
            return reocurringContacts;
        }

        private void AddAllContactsToRemind(
          ref List<ContactToRemindWithItems> pContactsToRemind)
        {
            List<ContactsThayMayNeedData> thatMayNeedNextWeek = new ContactsThatMayNeedNextWeek().GetContactsThatMayNeedNextWeek();
            CustomerTrackedServiceItems trackedServiceItems = new CustomerTrackedServiceItems();
            for (int index1 = 0; index1 < thatMayNeedNextWeek.Count; ++index1)
            {
                List<CustomerTrackedServiceItems.CustomerTrackedServiceItemsData> byCustomerTypeId = trackedServiceItems.GetAllByCustomerTypeID(thatMayNeedNextWeek[index1].CustomerData.CustomerTypeID);
                ContactToRemindWithItems toRemindWithItems = new ContactToRemindWithItems();
                toRemindWithItems.CustomerID = thatMayNeedNextWeek[index1].CustomerData.CustomerID;
                toRemindWithItems.CompanyName = thatMayNeedNextWeek[index1].CustomerData.CompanyName;
                toRemindWithItems.ContactFirstName = thatMayNeedNextWeek[index1].CustomerData.ContactFirstName;
                toRemindWithItems.ContactAltFirstName = thatMayNeedNextWeek[index1].CustomerData.ContactAltFirstName;
                toRemindWithItems.EmailAddress = thatMayNeedNextWeek[index1].CustomerData.EmailAddress;
                toRemindWithItems.AltEmailAddress = thatMayNeedNextWeek[index1].CustomerData.AltEmailAddress;
                toRemindWithItems.CityID = thatMayNeedNextWeek[index1].CustomerData.City;
                toRemindWithItems.CustomerTypeID = thatMayNeedNextWeek[index1].CustomerData.CustomerTypeID;
                toRemindWithItems.enabled = thatMayNeedNextWeek[index1].CustomerData.enabled;
                toRemindWithItems.EquipTypeID = thatMayNeedNextWeek[index1].CustomerData.EquipType;
                toRemindWithItems.TypicallySecToo = thatMayNeedNextWeek[index1].CustomerData.TypicallySecToo;
                toRemindWithItems.PreferedAgentID = thatMayNeedNextWeek[index1].CustomerData.PreferedAgent;
                toRemindWithItems.SalesAgentID = thatMayNeedNextWeek[index1].CustomerData.SalesAgentID;
                toRemindWithItems.UsesFilter = thatMayNeedNextWeek[index1].CustomerData.UsesFilter;
                toRemindWithItems.enabled = thatMayNeedNextWeek[index1].CustomerData.enabled;
                toRemindWithItems.AlwaysSendChkUp = thatMayNeedNextWeek[index1].CustomerData.AlwaysSendChkUp;
                toRemindWithItems.RequiresPurchOrder = thatMayNeedNextWeek[index1].RequiresPurchOrder;
                toRemindWithItems.ReminderCount = thatMayNeedNextWeek[index1].CustomerData.ReminderCount;
                toRemindWithItems.NextPrepDate = thatMayNeedNextWeek[index1].NextRoastDateByCityData.PrepDate.Date;
                toRemindWithItems.NextDeliveryDate = thatMayNeedNextWeek[index1].NextRoastDateByCityData.DeliveryDate.Date;
                toRemindWithItems.NextCoffee = thatMayNeedNextWeek[index1].ClientUsageData.NextCoffeeBy.Date;
                toRemindWithItems.NextClean = thatMayNeedNextWeek[index1].ClientUsageData.NextCleanOn.Date;
                toRemindWithItems.NextDescal = thatMayNeedNextWeek[index1].ClientUsageData.NextDescaleEst.Date;
                toRemindWithItems.NextFilter = thatMayNeedNextWeek[index1].ClientUsageData.NextFilterEst.Date;
                toRemindWithItems.NextService = thatMayNeedNextWeek[index1].ClientUsageData.NextServiceEst.Date;
                DateTime maxValue = DateTime.MaxValue;
                ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
                for (int index2 = 0; index2 < byCustomerTypeId.Count; ++index2)
                {
                    DateTime dateTime;
                    switch (byCustomerTypeId[index2].ServiceTypeID)
                    {
                        case 1:
                            dateTime = toRemindWithItems.NextClean;
                            break;
                        case 2:
                            dateTime = toRemindWithItems.NextCoffee;
                            break;
                        case 4:
                            dateTime = toRemindWithItems.NextDescal;
                            break;
                        case 5:
                            dateTime = toRemindWithItems.NextFilter;
                            break;
                        case 10:
                            dateTime = toRemindWithItems.NextService;
                            break;
                        default:
                            dateTime = DateTime.MaxValue;
                            break;
                    }
                    if (dateTime > DateTime.Now.AddYears(-1) && dateTime <= thatMayNeedNextWeek[index1].NextRoastDateByCityData.DeliveryDate)
                    {
                        List<ItemUsageTbl> lastItemsUsed = itemUsageTbl.GetLastItemsUsed(thatMayNeedNextWeek[index1].CustomerData.CustomerID, byCustomerTypeId[index2].ServiceTypeID);
                        for (int index3 = 0; index3 < lastItemsUsed.Count; ++index3)
                        {
                            ItemContactRequires _ItemRequired = new ItemContactRequires();
                            _ItemRequired.CustomerID = thatMayNeedNextWeek[index1].CustomerData.CustomerID;
                            _ItemRequired.AutoFulfill = thatMayNeedNextWeek[index1].CustomerData.autofulfill;
                            _ItemRequired.ReoccurID = 0;
                            _ItemRequired.ItemID = lastItemsUsed[index3].ItemProvidedID;
                            _ItemRequired.ItemQty = lastItemsUsed[index3].AmountProvided;
                            _ItemRequired.ItemPackagID = lastItemsUsed[index3].PackagingID;
                            if (!pContactsToRemind.Exists((Predicate<ContactToRemindWithItems>)(x => x.CustomerID == _ItemRequired.CustomerID)))
                            {
                                toRemindWithItems.ItemsContactRequires.Add(_ItemRequired);
                                pContactsToRemind.Add(toRemindWithItems);
                            }
                            else
                            {
                                int index4 = pContactsToRemind.FindIndex((Predicate<ContactToRemindWithItems>)(x => x.CustomerID == _ItemRequired.CustomerID));
                                if (!pContactsToRemind[index4].ItemsContactRequires.Exists((Predicate<ItemContactRequires>)(x => x.ItemID == _ItemRequired.ItemID)))
                                    pContactsToRemind[index4].ItemsContactRequires.Add(_ItemRequired);
                            }
                        }
                    }
                }
            }
        }

        private void SetListOfContactsToSendReminderTo()
        {
            List<ContactToRemindWithItems> reocurringContacts = this.GetReocurringContacts();
            this.AddAllContactsToRemind(ref reocurringContacts);
            reocurringContacts.Sort((Comparison<ContactToRemindWithItems>)((a, b) => string.Compare(a.CompanyName, b.CompanyName)));
            TempCoffeeCheckup tempCoffeeCheckup = new TempCoffeeCheckup();
            if (!tempCoffeeCheckup.DeleteAllContactRecords() || !tempCoffeeCheckup.DeleteAllContactItems())
            {
                showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Old Temp Table delete", "Error deleting old temp tables");
            }
            else
            {
                ItemTypeTbl itemTypeTbl = new ItemTypeTbl();
                List<int> idsofServiceType = itemTypeTbl.GetAllItemIDsofServiceType(2);
                idsofServiceType.AddRange((IEnumerable<int>)itemTypeTbl.GetAllItemIDsofServiceType(21));
                bool flag1 = false;
                for (int index1 = 0; index1 < reocurringContacts.Count; ++index1)
                {
                    bool flag2 = false;
                    for (int index2 = 0; index2 < reocurringContacts[index1].ItemsContactRequires.Count && !flag2; ++index2)
                        flag2 = idsofServiceType.Contains(reocurringContacts[index1].ItemsContactRequires[index2].ItemID);
                    if (flag2)
                    {
                        flag1 = tempCoffeeCheckup.InsertContacts((ContactToRemindDetails)reocurringContacts[index1]) || flag1;
                        foreach (ItemContactRequires itemsContactRequire in reocurringContacts[index1].ItemsContactRequires)
                            flag1 = tempCoffeeCheckup.InsertContactItems(itemsContactRequire) || flag1;
                    }
                }
                if (flag1)
                    return;
                showMessageBox showMessageBox2 = new showMessageBox(this.Page, "Not all records added to Temp Table", "Error adding some records added to Temp Table");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            this.LoadEmailTexts();
            this.PrepPageData();
        }

        protected void PrepPageData()
        {
            TrackerTools trackerTools = new TrackerTools();
            if (!trackerTools.IsNextRoastDateByCityTodays())
                trackerTools.SetNextRoastDateByCity();
            this.SetListOfContactsToSendReminderTo();
            this.upnlCustomerCheckup.Visible = true;
            this.upnlContactItems.Visible = true;
        }

        public string GetItemDesc(int pItemID)
        {
            return pItemID > 0 ? new ItemTypeTbl().GetItemTypeDesc(pItemID) : string.Empty;
        }

        public string GetItemSKU(int pItemID)
        {
            return pItemID > 0 ? new ItemTypeTbl().GetItemTypeSKU(pItemID) : string.Empty;
        }

        public string GetCityName(int pCityID)
        {
            return pCityID > 0 ? new CityTblDAL().GetCityName(pCityID) : string.Empty;
        }

        public string GetPackagingDesc(int pPackagingID)
        {
            return pPackagingID > 0 ? new PackagingTbl().GetPackagingDesc(pPackagingID) : string.Empty;
        }

        public string GetItemUoM(int pItemID)
        {
            return pItemID > 0 ? new ItemTypeTbl().GetItemUnitOfMeasure(pItemID) : string.Empty;
        }

        private string AddUnitsToQty(int pItemTypeID, double pQty)
        {
            string itemUoM = this.GetItemUoM(pItemTypeID);
            if (string.IsNullOrEmpty(itemUoM))
                return pQty.ToString();
            double num = Convert.ToDouble(pQty);
            return $"{pQty.ToString()} {(num == 1.0 ? itemUoM : itemUoM + "s")}";
        }

        private string ReplaceMergeTextWithDate(string pString, string pMergeFiled, DateTime pMergeDate)
        {
            while (pString.Contains(pMergeFiled))
                pString = pString.Replace(pMergeFiled, string.Format("{0:dddd, dd MMM}", (object)pMergeDate));
            return pString;
        }

        private string PersonalizeBodyText(ContactToRemindWithItems pContact)
        {
            if (pContact.NextPrepDate < DateTime.Now.Date)
                pContact.NextPrepDate = DateTime.Now.AddDays(1.0).Date;
            string pString = this.ReplaceMergeTextWithDate(this.tbxEmailBody.Text, "[#PREPDATE#]", pContact.NextPrepDate);
            if (pContact.NextDeliveryDate < DateTime.Now.Date)
                pContact.NextDeliveryDate = DateTime.Now.AddDays(3.0).Date;
            return this.ReplaceMergeTextWithDate(pString, "[#DELIVERYDATE#]", pContact.NextDeliveryDate) + "<br /><br />";
        }

        private string GetTheOrderType(ContactToRemindWithItems pContact)
        {
            string theOrderType = string.Empty;
            bool flag1 = pContact.ItemsContactRequires.Exists((Predicate<ItemContactRequires>)(x => x.AutoFulfill));
            bool flag2 = pContact.ItemsContactRequires.Exists((Predicate<ItemContactRequires>)(x => x.ReoccurOrder));
            if (flag2 && flag1)
                theOrderType = "combination of a reoccuring and autofulfill order";
            else if (flag2)
                theOrderType = "reoccuring order";
            else if (flag1)
                theOrderType = "automatically fulfilled order";
            return theOrderType;
        }

        private string AddLastOrderTableRow(ItemContactRequires pItemContactRequires, bool bAltRow)
        {
            string str = bAltRow ? "<td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{0}</td>" : "<td style='background-color: #E9FFE0; color: #593020; font-weight: bold'>{0}</td>";
            string qty = this.AddUnitsToQty(pItemContactRequires.ItemID, pItemContactRequires.ItemQty);
            return pItemContactRequires.ItemPackagID <= 0 ? string.Format(!bAltRow ? str + "<td style='background-color: #E9FFE0; text-align:center' colspan='2'>{1}</td>" : str + "<td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='2'>{1}</td>", (object)this.GetItemDesc(pItemContactRequires.ItemID), (object)qty) : string.Format(!bAltRow ? str + "<td style='background-color: #E9FFE0'>{1}</td><td style='background-color: #E9FFE0'>{2}</td>" : str + "<td style='background-color: #CFEFC9'>{1}</td><td style='background-color: #CFEFC9'>{2}</td>", (object)this.GetItemDesc(pItemContactRequires.ItemID), (object)qty, (object)this.GetPackagingDesc(pItemContactRequires.ItemPackagID));
        }

        private bool IsVowel(char pChar)
        {
            string source = "aeiou";
            pChar = char.ToLower(pChar);
            return source.Contains<char>(pChar);
        }

        private string UsageSummaryTableHeader(ContactToRemindWithItems pContact)
        {
            return "<table border='0' style='border: 1px solid'><tbody>";
        }

        private bool SendReminder(
          SendCheckEmailTextsData pEmailTextData,
          ContactToRemindWithItems pContact,
          string pOrderType)
        {
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            SentRemindersLogTbl pSentRemindersLog = new SentRemindersLogTbl();
            string str1 = this.UsageSummaryTableHeader(pContact);
            string str2 = string.IsNullOrEmpty(pOrderType) ? "a reminder only" : pOrderType;
            string str3 = str1 + $"<thead><tr><td style='background-color: #EFFFE9; font-weight: bold'>{"Company/Contact"}</td><td style='background-color: #EFFFE9;' colspan='2'>{pContact.CompanyName}</td></thead>" + $"<tr><td style='background-color: #E9FFE0; color: #593020; font-weight: bold'>{"Next estimate prep date"}</td><td style='background-color: #E9FFE0; text-align:center' colspan='2'>{string.Format("{0:d MMM, ddd}", (object)pContact.NextPrepDate)}</td></tr>" + $"<tr><td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{"Next estimate dispatch date"}</td><td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='2'>{string.Format("{0:d MMM, ddd}", (object)pContact.NextDeliveryDate)}</td></tr>" + $"<tr><td style='background-color: #CFEFC9; color: #694030; font-weight: bold'>{"Type"}</td><td style='background-color: #CFEFC9; color: #694030; text-align:center' colspan='2'>{str2}</td></tr>";
            if (pContact.ItemsContactRequires.Count > 0)
            {
                str3 += $"<tr><td style='background-color: #E9FFE0; text-align:center' colspan='3'>{"<b>List of Items</b>"}</td></tr>";
                for (int index = 0; index < pContact.ItemsContactRequires.Count; ++index)
                    str3 = $"{str3}<tr>{this.AddLastOrderTableRow(pContact.ItemsContactRequires[index], index % 2 == 0)}</tr>";
            }
            string str4 = str3 + "</tbody></table>";
            if (!string.IsNullOrWhiteSpace(pOrderType))
            {
                OrderTblData pOrderData = new OrderTblData();
                flag2 = true;
                pOrderData.CustomerID = pContact.CustomerID;
                pOrderData.OrderDate = DateTime.Now.Date;
                pOrderData.RoastDate = pContact.NextPrepDate.Date;
                pOrderData.RequiredByDate = pContact.NextDeliveryDate.Date;
                pOrderData.ToBeDeliveredBy = pContact.PreferedAgentID < 0 ? 3 : pContact.PreferedAgentID;
                pOrderData.Confirmed = false;
                pOrderData.InvoiceDone = false;
                pOrderData.PurchaseOrder = string.Empty;
                pOrderData.Notes = pOrderType;
                ReoccuringOrderDAL reoccuringOrderDal = new ReoccuringOrderDAL();
                OrderTbl orderTbl = new OrderTbl();
                string pMessage = string.Empty;
                for (int index = 0; index < pContact.ItemsContactRequires.Count && string.IsNullOrEmpty(pMessage); ++index)
                {
                    pOrderData.ItemTypeID = pContact.ItemsContactRequires[index].ItemID;
                    pOrderData.QuantityOrdered = pContact.ItemsContactRequires[index].ItemQty;
                    pOrderData.PackagingID = pContact.ItemsContactRequires[index].ItemPackagID;
                    pOrderData.PrepTypeID = pContact.ItemsContactRequires[index].ItemPrepID;
                    pMessage = orderTbl.InsertNewOrderLine(pOrderData);
                    if (pContact.ItemsContactRequires[index].ReoccurOrder)
                    {
                        reoccuringOrderDal.SetReoccuringOrdersLastDate(pContact.NextPrepDate, pContact.ItemsContactRequires[index].ReoccurID);
                        flag3 = true;
                    }
                }
                if (!string.IsNullOrEmpty(pMessage))
                {
                    showMessageBox showMessageBox = new showMessageBox(this.Page, "Error Inserting order for :" + pContact.CompanyName, pMessage);
                }
                else
                    pEmailTextData.Footer += "<br /><br />Items added to orders.";
            }
            else
            {
                string str5 = $"Pages/NewOrderDetail.aspx?Z&{"CoID"}={pContact.CustomerID}";
                for (int index = 0; index < pContact.ItemsContactRequires.Count; ++index)
                    str5 += $"&SKU{index + 1}={this.GetItemSKU(pContact.ItemsContactRequires[index].ItemID)}&SKUQty{index + 1}={pContact.ItemsContactRequires[index].ItemQty}";
                SendCheckEmailTextsData checkEmailTextsData1 = pEmailTextData;
                checkEmailTextsData1.Footer = $"{checkEmailTextsData1.Footer}<br /><br />{$"Internal: Order Ref <a href='http://tracker.quaffee.co.za/QonT/{str5}'>here</a>."}";
                SendCheckEmailTextsData checkEmailTextsData2 = pEmailTextData;
                checkEmailTextsData2.Footer = $"{checkEmailTextsData2.Footer}<br /><br />{$"If you would prefer to be disabled then click <a href='http://tracker.quaffee.co.za/QonT/DisableClient.aspx?CoID={pContact.CustomerID}'>disable me</a>"}";
            }
            string appSetting = ConfigurationManager.AppSettings["SysEmailFrom"];
            string empty = string.Empty;
            EmailCls emailCls = new EmailCls();
            emailCls.SetEmailFrom(appSetting);
            emailCls.SetEmailSubject(this.tbxEmailSubject.Text);
            if (pContact.EmailAddress.Contains("@"))
            {
                emailCls.SetEmailTo(pContact.EmailAddress, true);
                string str6 = string.IsNullOrWhiteSpace(pContact.ContactFirstName) ? "<p>Hi Coffee Lover,</p>" : $"<p>Hi {pContact.ContactFirstName},</p>";
                emailCls.MsgBody = $"{str6}<p>{pEmailTextData.Header}</p><p>{pEmailTextData.Body}</p><br />{str4}<br /><br />{pEmailTextData.Footer}";
                flag1 = emailCls.SendEmail();
            }
            if (pContact.AltEmailAddress.Contains("@"))
            {
                emailCls.SetEmailTo(pContact.AltEmailAddress, true);
                string str7 = string.IsNullOrWhiteSpace(pContact.ContactAltFirstName) ? "<p>Hi Coffee Lover,</p>" : $"<p>Hi {pContact.ContactAltFirstName},</p>";
                emailCls.MsgBody = $"{str7}<p>{pEmailTextData.Header}</p><p>{pEmailTextData.Body}</p><br />{str4}<br /><br />{pEmailTextData.Footer}";
                flag1 = flag1 || emailCls.SendEmail();
            }
            pSentRemindersLog.CustomerID = pContact.CustomerID;
            pSentRemindersLog.DateSentReminder = DateTime.Now.Date;
            pSentRemindersLog.NextPrepDate = pContact.NextCoffee.Date;
            pSentRemindersLog.ReminderSent = flag1;
            pSentRemindersLog.HadAutoFulfilItem = flag2;
            pSentRemindersLog.HadReoccurItems = flag3;
            pSentRemindersLog.InsertLogItem(pSentRemindersLog);
            return flag1;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            this.uprgSendEmail.DisplayAfter = 0;
            List<ContactToRemindWithItems> allContactAndItems = new TempCoffeeCheckup().GetAllContactAndItems();
            SendCheckEmailTextsData pEmailTextData = new SendCheckEmailTextsData();
            CustomersTbl customersTbl = new CustomersTbl();
            int num1 = 0;
            int num2 = 0;
            for (int index = 0; index < allContactAndItems.Count; ++index)
            {
                try
                {
                    pEmailTextData.Header = this.tbxEmailIntro.Text;
                    pEmailTextData.Footer = this.tbxEmailFooter.Text;
                    if ((allContactAndItems[index].EmailAddress + allContactAndItems[index].AltEmailAddress).Contains("@"))
                    {
                        ++allContactAndItems[index].ReminderCount;
                        if (allContactAndItems[index].ReminderCount < 7)
                        {
                            pEmailTextData.Body = this.PersonalizeBodyText(allContactAndItems[index]);
                            string theOrderType = this.GetTheOrderType(allContactAndItems[index]);
                            if (string.IsNullOrWhiteSpace(theOrderType))
                            {
                                pEmailTextData.Body += "We will only place an order on your request, no order has been added, this just a reminder.";
                            }
                            else
                            {
                                SendCheckEmailTextsData checkEmailTextsData = pEmailTextData;
                                checkEmailTextsData.Body = $"{checkEmailTextsData.Body}This is a{(this.IsVowel(theOrderType[0]) ? "n " : " ")}{theOrderType}, please respond to cancel.";
                            }
                            if (allContactAndItems[index].ReminderCount == 6)
                                pEmailTextData.Body = "This is your last reminder email. Next time reminders will be disabled until you order again." + pEmailTextData.Body;
                            if (this.SendReminder(pEmailTextData, allContactAndItems[index], theOrderType))
                                ++num1;
                            else
                                ++num2;
                            if (allContactAndItems[index].ReminderCount >= 4)
                                new ClientUsageTbl().ForceNextCoffeeDate(allContactAndItems[index].NextPrepDate.AddDays((double)(10 * (allContactAndItems[index].ReminderCount - 4 + 1))), allContactAndItems[index].CustomerID);
                            customersTbl.SetSentReminderAndIncrementReminderCount(DateTime.Now.Date, allContactAndItems[index].CustomerID);
                        }
                        else
                        {
                            customersTbl.DisableCustomer(allContactAndItems[index].CustomerID);
                            ++num2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    showMessageBox showMessageBox = new showMessageBox(this.Page, "Error sending...", ex.Message);
                    ++num2;
                }
            }
            showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Reminder emails status", $"Reminders processed. Sent: {num1}; Failed: {num2}");
            this.Response.Redirect($"{this.ResolveUrl("~/Pages/SentRemindersSheet.aspx")}?LastSentDate={DateTime.Now.Date:d}");
            this.PrepPageData();
        }

        protected void LoadEmailTexts()
        {
            SendCheckEmailTextsData texts = new SendCheckEmailTextsData().GetTexts();
            if (texts.SCEMTID <= 0)
                return;
            this.ltrlEmailTextID.Text = texts.SCEMTID.ToString();
            this.tbxEmailIntro.Text = HttpUtility.HtmlDecode(texts.Header);
            this.tbxEmailBody.Text = HttpUtility.HtmlDecode(texts.Body);
            this.tbxEmailFooter.Text = texts.Footer;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.ltrlEmailTextID.Text))
                return;
            SendCheckEmailTextsData pEmailTextsData = new SendCheckEmailTextsData();
            pEmailTextsData.Header = HttpUtility.HtmlEncode(this.tbxEmailIntro.Text);
            pEmailTextsData.Body = HttpUtility.HtmlEncode(this.tbxEmailBody.Text);
            pEmailTextsData.Footer = HttpUtility.HtmlEncode(this.tbxEmailFooter.Text);
            this.ltrlStatus.Text = pEmailTextsData.UpdateTexts(pEmailTextsData, Convert.ToInt32(this.ltrlEmailTextID.Text));
        }

        protected void btnReload_Click(object sender, EventArgs e) => this.LoadEmailTexts();

        protected void btnPrepData_Click(object sender, EventArgs e) => this.PrepPageData();
    }
}