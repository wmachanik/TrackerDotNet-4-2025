// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Tools.SystemTools
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.Tools
{

    public class SystemTools : Page
    {
        private const int CONST_MINMONTHS = 3;
        private StreamWriter _ColsStream;
        protected ToolkitScriptManager tsmSystemTools;
        protected UpdateProgress uprgSystemTools;
        protected UpdatePanel upnlSystemToolsButtons;
        protected Button btnSetClientType;
        protected Button btnXMLTOSQL;
        protected Button btnResetPrepDates;
        protected Button btnMoveDlvryDate;
        protected Button btnEditSystemData;
        protected Button btnCreateUpdateLogTables;
        protected Button btnMergQBAccData;
        protected Literal ltrlStatus;
        protected Panel pnlSetClinetType;
        protected Label ResultsTitleLabel;
        protected GridView gvResults;
        protected GridView gvCustomerTypes;
        protected ObjectDataSource odsCustomerTypes;
        protected Panel pnlResetPrepDate;
        protected GridView gvCityPrepDates;
        protected SqlDataSource sdsCityPrepDates;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private List<int> GetAllCoffeeClientTypes()
        {
            List<int> coffeeClientTypes = new List<int>()
    {
      3,
      1,
      8,
      5,
      4,
      2
    };
            coffeeClientTypes.Sort();
            return coffeeClientTypes;
        }

        private List<int> GetAllServiceOnlyClientTypes()
        {
            List<int> serviceOnlyClientTypes = new List<int>() { 6 };
            serviceOnlyClientTypes.Sort();
            return serviceOnlyClientTypes;
        }

        private SystemTools.ContactsUpdated CheckCoffeeCustomerIsOne(
          ContactType pCustomer,
          SystemTools.ContactsUpdated pContact)
        {
            ClientUsageLinesTbl clientUsageLinesTbl = new ClientUsageLinesTbl();
            ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
            DateTime minValue = DateTime.MinValue;
            ClientUsageLinesTbl latestUsageData = clientUsageLinesTbl.GetLatestUsageData(pCustomer.CustomerID, 2);
            if (latestUsageData != null)
            {
                DateTime customerInstallDate = latestUsageData.GetCustomerInstallDate(pCustomer.CustomerID);
                if (latestUsageData.LineDate <= customerInstallDate)
                {
                    if (itemUsageTbl.GetLastMaintenanceItem(pCustomer.CustomerID) == null)
                    {
                        pContact.ContactTypeID = 9;
                        pContact.PredictionDisabled = true;
                    }
                    else
                        pContact.ContactTypeID = 6;
                }
                else if (customerInstallDate.AddMonths(3) <= latestUsageData.LineDate)
                {
                    if (itemUsageTbl.GetLastMaintenanceItem(pCustomer.CustomerID) == null)
                        pContact.ContactTypeID = 3;
                    else if (pContact.ContactTypeID == 3)
                        pContact.ContactTypeID = 1;
                }
            }
            else
                pContact.ContactTypeID = itemUsageTbl.GetLastMaintenanceItem(pCustomer.CustomerID) == null ? 9 : 6;
            return pContact;
        }

        private SystemTools.ContactsUpdated CheckNoneCoffeeCustomer(
          ContactType pCustomer,
          SystemTools.ContactsUpdated pContact)
        {
            ClientUsageLinesTbl clientUsageLinesTbl = new ClientUsageLinesTbl();
            ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
            List<ItemUsageTbl> lastItemsUsed = itemUsageTbl.GetLastItemsUsed(pCustomer.CustomerID, 2);
            ItemUsageTbl lastMaintenanceItem = itemUsageTbl.GetLastMaintenanceItem(pCustomer.CustomerID);
            if (lastItemsUsed.Count > 0)
            {
                DateTime customerInstallDate = clientUsageLinesTbl.GetCustomerInstallDate(pCustomer.CustomerID);
                if (clientUsageLinesTbl.LineDate >= customerInstallDate)
                {
                    if (lastMaintenanceItem == null)
                        pContact.ContactTypeID = 9;
                }
                else if (lastMaintenanceItem == null)
                    pContact.ContactTypeID = 3;
            }
            else if (lastMaintenanceItem == null)
                pContact.ContactTypeID = 9;
            lastItemsUsed.Clear();
            return pContact;
        }

        protected void btnSetClientType_Click(object sender, EventArgs e)
        {
            this.pnlSetClinetType.Visible = true;
            this.gvCustomerTypes.Visible = true;
            List<SystemTools.ContactsUpdated> contactsUpdatedList = new List<SystemTools.ContactsUpdated>();
            ClientUsageLinesTbl clientUsageLinesTbl = new ClientUsageLinesTbl();
            ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
            ContactType contactType = new ContactType();
            this._ColsStream = new StreamWriter("c:\\temp\\" + $"SetClientType_{DateTime.Now:ddMMyyyy hh mm}.txt", false);
            this._ColsStream.WriteLine("Task, Company Name, origType, newType, PredDisabled");
            List<ContactType> contactTypeList = (List<ContactType>)null;
            int index = 0;
            try
            {
                contactTypeList = contactType.GetAllContacts("CompanyName");
                List<int> coffeeClientTypes = this.GetAllCoffeeClientTypes();
                this.GetAllServiceOnlyClientTypes();
                for (; index < contactTypeList.Count; ++index)
                {
                    SystemTools.ContactsUpdated pContact = new SystemTools.ContactsUpdated();
                    if (contactTypeList[index].CustomerTypeID != 9)
                    {
                        pContact.ContactName = contactTypeList[index].CompanyName;
                        pContact.ContactTypeID = contactTypeList[index].CustomerTypeID;
                        pContact.origContactTypeID = contactTypeList[index].CustomerTypeID;
                        pContact.PredictionDisabled = contactTypeList[index].PredictionDisabled;
                        if (pContact.ContactTypeID == 0)
                            pContact.ContactTypeID = 3;
                        SystemTools.ContactsUpdated contactsUpdated = !coffeeClientTypes.Contains(pContact.ContactTypeID) ? this.CheckNoneCoffeeCustomer(contactTypeList[index], pContact) : this.CheckCoffeeCustomerIsOne(contactTypeList[index], pContact);
                        if (!contactsUpdated.ContactTypeID.Equals(contactsUpdated.origContactTypeID))
                        {
                            contactTypeList[index].CustomerTypeID = contactsUpdated.ContactTypeID;
                            contactTypeList[index].PredictionDisabled = contactsUpdated.PredictionDisabled;
                            string str = contactTypeList[index].UpdateContact(contactTypeList[index]);
                            contactsUpdatedList.Add(contactsUpdated);
                            if (string.IsNullOrEmpty(str))
                                this._ColsStream.WriteLine("Added {0}-{1}: {2}, {3}, {4}, {5}", (object)index, (object)contactsUpdatedList.Count, (object)contactsUpdated.ContactName, (object)contactsUpdated.origContactTypeID, (object)contactsUpdated.ContactTypeID, (object)contactsUpdated.PredictionDisabled);
                            else
                                this._ColsStream.WriteLine("Error {0} Adding: {1}, {2}, {3}, {4}, {5}", (object)str, (object)index, (object)str, (object)contactsUpdated.ContactName, (object)contactsUpdated.origContactTypeID, (object)contactsUpdated.ContactTypeID, (object)contactsUpdated.PredictionDisabled);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string pMessage = ex.Message;
                string sessionErrorString = new TrackerTools().GetTrackerSessionErrorString();
                if (!string.IsNullOrWhiteSpace(sessionErrorString))
                    pMessage = $"{pMessage} TTError: {sessionErrorString}";
                showMessageBox showMessageBox = new showMessageBox(this.Page, "Error", pMessage);
                if (contactTypeList != null)
                    this._ColsStream.WriteLine("ERROR AT: {0}, Name: {1}, ID: {2}, Pred: {3}", (object)index, (object)contactTypeList[index].CompanyName, (object)contactTypeList[index].CustomerTypeID, (object)contactTypeList[index].PredictionDisabled);
                else
                    this._ColsStream.WriteLine("null customers");
                this._ColsStream.WriteLine("Error:" + pMessage);
                throw;
            }
            finally
            {
                this._ColsStream.Close();
            }
            showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Info", $"A Total of {contactsUpdatedList.Count}, contacts were updated");
            this.ltrlStatus.Text = $"A Total of {contactsUpdatedList.Count}, contacts were updated";
            this.ltrlStatus.Visible = true;
            this.ResultsTitleLabel.Text = "Set client type results";
            this.gvResults.DataSource = (object)contactsUpdatedList;
            this.gvResults.DataBind();
        }

        protected void btnResetPrepDates_Click(object sender, EventArgs e)
        {
            this.pnlResetPrepDate.Visible = true;
            new TrackerTools().SetNextRoastDateByCity();
            this.sdsCityPrepDates.DataBind();
            this.gvCityPrepDates.DataBind();
        }

        protected void btnCreateUpdateLogTables_Click(object sender, EventArgs e)
        {
            TrackerTools trackerTools = new TrackerTools();
            trackerTools.ClearTrackerSessionErrorString();
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.CreateIfDoesNotExists("LogTbl");
            this.ltrlStatus.Visible = true;
            this.ltrlStatus.Text = "Log table checked";
            if (trackerTools.IsTrackerSessionErrorString())
            {
                Literal ltrlStatus = this.ltrlStatus;
                ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                trackerTools.ClearTrackerSessionErrorString();
            }
            List<string> stringList = new PersonsTbl().SecurityUsersNotInPeopleTbl();
            this.pnlSetClinetType.Visible = true;
            this.gvCustomerTypes.Visible = false;
            this.gvResults.DataSource = (object)stringList;
            this.ResultsTitleLabel.Text = "Security Users not in People Table ";
            if (trackerTools.IsTrackerSessionErrorString())
            {
                Literal ltrlStatus = this.ltrlStatus;
                ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                trackerTools.ClearTrackerSessionErrorString();
            }
            this.gvResults.DataBind();
            if (trackerDb.CreateIfDoesNotExists("SectionTypesTbl"))
            {
                this.ltrlStatus.Text += "; Section Types table checked";
                if (trackerTools.IsTrackerSessionErrorString())
                {
                    Literal ltrlStatus = this.ltrlStatus;
                    ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                    trackerTools.ClearTrackerSessionErrorString();
                }
                if (new SectionTypesTbl().InsertDefaultSections())
                    this.ltrlStatus.Text += " - default sections added.";
                if (trackerTools.IsTrackerSessionErrorString())
                {
                    Literal ltrlStatus = this.ltrlStatus;
                    ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                    trackerTools.ClearTrackerSessionErrorString();
                }
            }
            if (trackerDb.CreateIfDoesNotExists("TransactionTypesTbl"))
            {
                this.ltrlStatus.Text += "; Transaction Types table checked";
                if (trackerTools.IsTrackerSessionErrorString())
                {
                    Literal ltrlStatus = this.ltrlStatus;
                    ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                    trackerTools.ClearTrackerSessionErrorString();
                }
                if (new TransactionTypesTbl().InsertDefaultTransactions())
                    this.ltrlStatus.Text += " - default Transactions added.";
                if (trackerTools.IsTrackerSessionErrorString())
                {
                    Literal ltrlStatus = this.ltrlStatus;
                    ltrlStatus.Text = $"{ltrlStatus.Text} - Error: {trackerTools.GetTrackerSessionErrorString()}";
                    trackerTools.ClearTrackerSessionErrorString();
                }
            }
            trackerDb.Close();
        }

        private class ContactsUpdated
        {
            private string _ContactName;
            private int _ContactTypeID;
            private int _origContactTypeID;
            private bool _PredictionDisabled;

            public ContactsUpdated()
            {
                this._ContactName = string.Empty;
                this._ContactTypeID = this._origContactTypeID = int.MinValue;
                this._PredictionDisabled = false;
            }

            public string ContactName
            {
                get => this._ContactName;
                set => this._ContactName = value;
            }

            public int ContactTypeID
            {
                get => this._ContactTypeID;
                set => this._ContactTypeID = value;
            }

            public int origContactTypeID
            {
                get => this._origContactTypeID;
                set => this._origContactTypeID = value;
            }

            public bool PredictionDisabled
            {
                get => this._PredictionDisabled;
                set => this._PredictionDisabled = value;
            }
        }
    }
}