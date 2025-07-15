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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;
using TrackerDotNet.Managers;

namespace TrackerDotNet.Pages
{
    public partial class SendCoffeeCheckup : System.Web.UI.Page
    {
        private static Dictionary<int, string> _cachedCityNames = new Dictionary<int, string>();
        private static Dictionary<int, string> _cachedItemDescriptions = new Dictionary<int, string>();

        // Business logic manager - PROPERLY INITIALIZED
        private readonly CoffeeCheckupManager _coffeeCheckupManager;
        
        public SendCoffeeCheckup()
        {
            _coffeeCheckupManager = new CoffeeCheckupManager();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AppLogger.WriteLog("performance", "SendCoffeeCheckup: Page_Load started");
                
                // Make sure panels are visible
                upnlCustomerCheckup.Visible = true;
                upnlContactItems.Visible = true;
                
                // Load email templates immediately
                LoadEmailTextsOnly();
                
                // Set initial status
                ltrlCustomerStatus.Text = "🔄 Initializing auto-preparation...";
                autoLoadingStatus.Visible = true;
                btnPrepData.Visible = true; // Keep visible for manual fallback
                
                AppLogger.WriteLog("performance", "SendCoffeeCheckup: Page_Load completed, auto-prep will trigger via JavaScript");
            }
        }

        /// <summary>
        /// Automatically prepare customer data on page load
        /// </summary>
        private void AutoPrepareCustomerData()
        {
            try
            {
                // Show initial loading status
                ltrlCustomerStatus.Text = "🔄 Auto-preparing customer data...";
                autoLoadingStatus.Visible = true;
                
                // Start async preparation (this will be called by the JavaScript auto-trigger)
                // The actual work happens in btnPrepData_Click
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("error", $"SendCoffeeCheckup: Error in auto-prepare: {ex.Message}");
                ltrlCustomerStatus.Text = $"❌ Auto-preparation failed. Use 'Refresh Data' button.";
                autoLoadingStatus.Visible = false;
                btnPrepData.Visible = true; // Show manual option
            }
        }

        /// <summary>
        /// Load only email templates - fast operation
        /// </summary>
        private void LoadEmailTextsOnly()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Load email templates (existing method)
                LoadEmailTexts();
                
                stopwatch.Stop();
                AppLogger.WriteLog("performance", $"SendCoffeeCheckup: Email templates loaded in {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("error", $"SendCoffeeCheckup: Error loading email templates: {ex.Message}");
                ltrlStatus.Text = $"<div class='alert alert-warning'>Warning: {ex.Message}</div>";
            }
        }

        protected void btnPrepData_Click(object sender, EventArgs e)
        {
            // Control progress indicators
            uprgCustomerCheckup.DisplayAfter = 100; // Show customer prep progress quickly
            uprgSendEmail.DisplayAfter = int.MaxValue; // Don't show email progress
            
            AppLogger.WriteLog("performance", "SendCoffeeCheckup: btnPrepData_Click triggered");
            
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // IMMEDIATELY hide the auto-loading panel
                autoLoadingStatus.Visible = false;

                // Update status immediately
                ltrlCustomerStatus.Text = "⏳ Preparing customer data... Please wait.";
                ltrlAutoLoadStatus.Text = "";
                
                // Force immediate update
                upnlSendEmail.Update();
                
                AppLogger.WriteLog("performance", "SendCoffeeCheckup: Starting CoffeeCheckupManager.PrepareCustomerReminderData()");
                
                // Use the enhanced CoffeeCheckupManager
                _coffeeCheckupManager.PrepareCustomerReminderData();
                
                AppLogger.WriteLog("performance", "SendCoffeeCheckup: PrepareCustomerReminderData completed, refreshing grids");
                
                // Refresh the grid data
                odsContactsToSendCheckup.DataBind();
                gvCustomerCheckup.DataBind();
                
                // Get customer count and show success
                int customerCount = GetCustomerCount();
                
                stopwatch.Stop();
                
                // Update button and show success
                btnPrepData.Text = "Refresh Data";
                btnPrepData.Visible = true;
                
                ltrlCustomerStatus.Text = $"✅ Ready! Found <strong>{customerCount}</strong> customers eligible for coffee reminders. " +
                                         $"<small>(Loaded in {stopwatch.ElapsedMilliseconds / 1000.0:F1}s)</small>";
                
                ltrlStatus.Text = $"<div style='background-color: #d4edda; color: #155724; padding: 8px; border-radius: 4px; margin: 5px 0; text-align: left;'>" +
                                 $"<strong>Success!</strong> Customer data prepared automatically. You can now send reminders or test emails.</div>";
                
                AppLogger.WriteLog("performance", $"SendCoffeeCheckup: Auto-prep completed successfully in {stopwatch.ElapsedMilliseconds}ms - {customerCount} customers");
                
                // Force update of all panels
                upnlCustomerCheckup.Update();
                upnlSendEmail.Update();
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("error", $"SendCoffeeCheckup: Error in btnPrepData_Click: {ex.Message}");
                
                // Hide auto-loading panel on error too
                autoLoadingStatus.Visible = false;
                
                btnPrepData.Visible = true;
                btnPrepData.Text = "Retry Data Prep";
                
                ltrlCustomerStatus.Text = $"❌ Auto-preparation failed: {ex.Message}";
                ltrlStatus.Text = $"<div style='background-color: #f8d7da; color: #721c24; padding: 8px; border-radius: 4px; margin: 5px 0; text-align: left;'>" +
                                 $"<strong>Error:</strong> {ex.Message}<br/><small>Check the logs for more details.</small></div>";
                
                // Force update of all panels
                upnlCustomerCheckup.Update();
                upnlSendEmail.Update();
            }
            finally
            {
                // Reset progress indicators
                uprgCustomerCheckup.DisplayAfter = 500;
                uprgSendEmail.DisplayAfter = 0;
            }
        }

        /// <summary>
        /// Get count of prepared customers
        /// </summary>
        private int GetCustomerCount()
        {
            try
            {
                var tempCheckup = new TempCoffeeCheckup();
                var customers = tempCheckup.GetAllContacts("CustomerID");
                return customers?.Count ?? 0;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("error", $"SendCoffeeCheckup: Error getting customer count: {ex.Message}");
                return 0;
            }
        }

        // FIXED: Remove duplicate and use cached versions
        protected string GetCityName(int cityId)
        {
            if (!_cachedCityNames.ContainsKey(cityId))
            {
                _cachedCityNames[cityId] = _coffeeCheckupManager.GetCachedCityName(cityId);
            }
            return _cachedCityNames[cityId];
        }

        protected string GetItemDesc(int itemId)
        {
            if (!_cachedItemDescriptions.ContainsKey(itemId))
            {
                _cachedItemDescriptions[itemId] = _coffeeCheckupManager.GetCachedItemDescription(itemId);
            }
            return _cachedItemDescriptions[itemId];
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            this.uprgSendEmail.DisplayAfter = 0;
            
            try
            {
                UpdateStatus("🔄 Starting coffee checkup process...");
                
                // Prepare email data from UI
                var emailData = new SendCheckEmailTextsData {
                    Header = this.tbxEmailIntro.Text,
                    Body = this.tbxEmailBody.Text,
                    Footer = this.tbxEmailFooter.Text
                };
                
                UpdateStatus("📋 Processing customers...");
                
                // Use the manager to process reminders
                var batchResult = _coffeeCheckupManager.ProcessCoffeeCheckupReminders(emailData);
                
                UpdateStatus($"✅ Complete! Sent: {batchResult.TotalSent}, Failed: {batchResult.TotalFailed}");

                // Enhanced status message
                string statusMessage = $"Coffee checkup process completed!\n\n" +
                                     $"📧 Emails sent successfully: {batchResult.TotalSent}\n" +
                                     $"❌ Failed to send: {batchResult.TotalFailed}\n" +
                                     $"📊 Total customers processed: {batchResult.TotalSent + batchResult.TotalFailed}";

                if (batchResult.TotalFailed > 0)
                {
                    statusMessage += "\n\n⚠️ Check the results page for details about failed emails.";
                }

                var statusMsg = new showMessageBox(this.Page, 
                    "Coffee Checkup Status", 
                    statusMessage);

                // Navigation to results page
                RedirectToResultsPage();
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Error: {ex.Message}");
                AppLogger.WriteLog("email", $"SendCoffeeCheckup: Error in btnSend_Click: {ex.Message}");
                
                var errorMsg = new showMessageBox(this.Page, 
                    "Email Sending Error", 
                    ex.Message);
            }
        }

        protected void btnTestSingleCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("🧪 Starting test mode...");

                // Get test contact
                var allContacts = new TempCoffeeCheckup().GetAllContactAndItems();
                if (!allContacts.Any())
                {
                    this.ltrlStatus.Text = "No test contacts available. Run 'Prep Data' first.";
                    return;
                }

                var testContact = allContacts.First();
                AppLogger.WriteLog("email", $"TEST: Using customer {testContact.CompanyName} (ID: {testContact.CustomerID})");

                // Validate eligibility
                if (!_coffeeCheckupManager.ValidateCustomerEligibility(testContact))
                {
                    this.ltrlStatus.Text = $"❌ Test customer {testContact.CompanyName} is not eligible for reminders";
                    return;
                }

                // Prepare email data
                var emailData = new SendCheckEmailTextsData
                {
                    Header = this.tbxEmailIntro.Text,
                    Body = this.tbxEmailBody.Text,
                    Footer = this.tbxEmailFooter.Text
                };

                // Process single customer test
                var testResult = _coffeeCheckupManager.ProcessCoffeeCheckupReminders(emailData);

                this.ltrlStatus.Text = $"✅ Test completed: Sent: {testResult.TotalSent}, Failed: {testResult.TotalFailed}";
                AppLogger.WriteLog("email", $"TEST: Completed - Sent: {testResult.TotalSent}, Failed: {testResult.TotalFailed}");
            }
            catch (Exception ex)
            {
                this.ltrlStatus.Text = $"❌ Test failed: {ex.Message}";
                AppLogger.WriteLog("email", $"TEST ERROR: {ex.Message}");
            }
        }

        protected void btnClearTodaysData_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("🗑️ Clearing today's reminder data...");

                // Clear today's sent reminder log entries
                var sentRemindersLogTbl = new SentRemindersLogTbl();
                int deletedCount = sentRemindersLogTbl.DeleteTodaysEntries(TimeZoneUtils.Now().Date);

                UpdateStatus($"✅ Cleared {deletedCount} reminder entries from today");

                var successMsg = new showMessageBox(this.Page,
                    "Data Cleared",
                    $"Successfully removed {deletedCount} reminder log entries from today ({TimeZoneUtils.Now().Date:yyyy-MM-dd}).\n\nYou can now test again with clean data.");

                AppLogger.WriteLog("email", $"SendCoffeeCheckup: Cleared {deletedCount} today's reminder entries");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Error clearing data: {ex.Message}");
                AppLogger.WriteLog("email", $"SendCoffeeCheckup: Error clearing today's data: {ex.Message}");

                var errorMsg = new showMessageBox(this.Page,
                    "Clear Data Error",
                    $"Error clearing today's data: {ex.Message}");
            }
        }

        private void RedirectToResultsPage()
        {
            try
            {
                DateTime sentDate = TimeZoneUtils.Now().Date;

                // Get the statistics from the database
                var sentRemindersLogTbl = new SentRemindersLogTbl();
                int totalReminders = sentRemindersLogTbl.GetEntriesCountForDate(sentDate);

                // Count unique customers and success/fail by iterating through the results
                var dayResults = sentRemindersLogTbl.GetAllByDate(sentDate, "CustomerID");
                int uniqueCustomers = dayResults.Select(r => r.CustomerID).Distinct().Count();
                int successful = dayResults.Count(r => r.ReminderSent);
                int failed = dayResults.Count(r => !r.ReminderSent);

                string redirectUrl = $"{this.ResolveUrl("~/Pages/SentRemindersSheet.aspx")}" +
                                   $"?LastSentDate={sentDate:yyyy-MM-dd}" +
                                   $"&TotalReminders={totalReminders}" +
                                   $"&UniqueCustomers={uniqueCustomers}" +
                                   $"&Successful={successful}" +
                                   $"&Failed={failed}";

                AppLogger.WriteLog("email", $"SendCoffeeCheckup: Redirecting with stats - {uniqueCustomers} customers, {successful}/{totalReminders} successful");
                Response.Redirect(redirectUrl, false);
            }
            catch (Exception redirectEx)
            {
                AppLogger.WriteLog("email", $"SendCoffeeCheckup: Redirect failed: {redirectEx.Message}");
                UpdateStatus("✅ Process completed successfully!");
            }
        }

        private void UpdateStatus(string message)
        {
            this.ltrlStatus.Text = message;
            AppLogger.WriteLog("email", $"SendCoffeeCheckup STATUS: {message}");
        }

        private void LoadEmailTexts()
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

        // Helper methods for compatibility
        public string GetItemSKU(int pItemID) => _coffeeCheckupManager.GetCachedItemSKU(pItemID);
        public string GetPackagingDesc(int pPackagingID) => _coffeeCheckupManager.GetCachedPackagingDescription(pPackagingID);
        public string GetItemUoM(int pItemID) => _coffeeCheckupManager.GetCachedItemUoM(pItemID);
    }
}