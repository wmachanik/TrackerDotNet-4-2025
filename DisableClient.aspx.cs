using System;
using System.Web.UI;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;
using TrackerDotNet.Managers;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace TrackerDotNet
{
    public partial class DisableClient : Page
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl confirmationSection;
        protected System.Web.UI.HtmlControls.HtmlGenericControl successSection;
        protected Label CompanyNameLabel;
        protected Label CompanyNameSuccessLabel;
        protected Button btnConfirmDisable;
        //protected Literal ltrlContactEmail;
        //protected Literal ltrlContactEmailSuccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadContactEmail();
                LoadCustomerInfo();
            }
        }

        private void LoadContactEmail()
        {
            try
            {
                string systemEmail = ConfigurationManager.AppSettings["SysFromEmail"];
                
                if (string.IsNullOrEmpty(systemEmail))
                {
                    systemEmail = "info@quaffee.co.za";
                    AppLogger.WriteLog("email", "DisableClient: SysFromEmail not configured, using fallback");
                }
                
                string emailLink = $"<a href='mailto:{systemEmail}'>{systemEmail}</a>";
                
                if (ltrlContactEmail != null)
                    ltrlContactEmail.Text = emailLink;
                if (ltrlContactEmailSuccess != null)
                    ltrlContactEmailSuccess.Text = emailLink;
                    
                AppLogger.WriteLog("email", $"DisableClient: Using contact email {systemEmail}");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Error loading contact email: {ex.Message}");
                string fallbackLink = "<a href='mailto:info@quaffee.co.za'>info@quaffee.co.za</a>";
                if (ltrlContactEmail != null)
                    ltrlContactEmail.Text = fallbackLink;
                if (ltrlContactEmailSuccess != null)
                    ltrlContactEmailSuccess.Text = fallbackLink;
            }
        }

        private void LoadCustomerInfo()
        {
            try
            {
                string customerIdStr = Request.QueryString["CoID"];
                string token = Request.QueryString["token"];

                if (string.IsNullOrEmpty(customerIdStr) || string.IsNullOrEmpty(token))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorInvalidParams));
                    return;
                }

                if (!int.TryParse(customerIdStr, out int customerId))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorInvalidParams));
                    return;
                }

                if (!DisableClientManager.ValidateToken(customerIdStr, token))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorInvalidToken));
                    return;
                }

                var companyNames = new CompanyNames();
                string companyName = companyNames.GetCompanyNameByCompanyID(customerId);

                if (string.IsNullOrEmpty(companyName))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorCustomerNotFound));
                    return;
                }

                CompanyNameLabel.Text = companyName;
                CompanyNameSuccessLabel.Text = companyName;

                confirmationSection.Visible = true;
                successSection.Visible = false;

                AppLogger.WriteLog("email", $"DisableClient: Loaded disable page for customer {customerId} ({companyName})");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Error loading customer info: {ex.Message}");
                ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorGeneral));
            }
        }

        protected void btnConfirmDisable_Click(object sender, EventArgs e)
        {
            try
            {
                string customerIdStr = Request.QueryString["CoID"];
                string token = Request.QueryString["token"];

                if (!int.TryParse(customerIdStr, out int customerId))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorInvalidParams));
                    return;
                }

                if (!DisableClientManager.ValidateToken(customerIdStr, token))
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorInvalidToken));
                    return;
                }

                string companyName = CompanyNameLabel.Text;
                var recurringOrdersInfo = CheckRecurringOrders(customerId);

                var customersTbl = new CustomersTbl();
                string disableReason = $"Disabled by customer request on {TimeZoneUtils.Now():yyyy-MM-dd HH:mm} via email link";
                
                bool disableResult = customersTbl.DisableCustomer(customerId, disableReason);

                if (disableResult)
                {
                    AppLogger.WriteLog("email", $"DisableClient: Customer {customerId} ({companyName}) successfully disabled via email link");

                    NotifyAdministrator(customerId, companyName, recurringOrdersInfo);
                    SendCustomerGoodbyeEmail(customerId, companyName);

                    confirmationSection.Visible = false;
                    successSection.Visible = true;
                }
                else
                {
                    ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorGeneral));
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Error disabling customer: {ex.Message}");
                ShowError(MessageProvider.Get(MessageKeys.DisableClient.ErrorGeneral));
            }
        }

        private RecurringOrdersInfo CheckRecurringOrders(int customerId)
        {
            var recurringInfo = new RecurringOrdersInfo();
            
            try
            {
                var reoccuringOrderDal = new ReoccuringOrderDAL();
                
                // Use GetAll and filter by customer ID
                var allRecurringOrders = reoccuringOrderDal.GetAll(1, "CustomersTbl.CustomerID"); // 1 = enabled only
                var customerRecurringOrders = allRecurringOrders.Where(ro => ro.CustomerID == customerId).ToList();
                
                recurringInfo.TotalRecurringOrders = customerRecurringOrders?.Count ?? 0;
                
                if (recurringInfo.TotalRecurringOrders > 0)
                {
                    AppLogger.WriteLog("email", $"DisableClient: Found {recurringInfo.TotalRecurringOrders} recurring orders for customer {customerId}");
                    
                    foreach (var recurringOrder in customerRecurringOrders)
                    {
                        try
                        {
                            var itemTypeTbl = new ItemTypeTbl();
                            string itemDesc = itemTypeTbl.GetItemTypeDesc(recurringOrder.ItemRequiredID);
                            recurringInfo.RecurringOrderDetails.Add($"- {itemDesc} (Qty: {recurringOrder.QtyRequired}) - ID: {recurringOrder.ReoccuringOrderID}");
                            
                            AppLogger.WriteLog("email", $"DisableClient: Found recurring order {recurringOrder.ReoccuringOrderID} for customer {customerId} - MANUAL DISABLE REQUIRED");
                        }
                        catch (Exception ex)
                        {
                            AppLogger.WriteLog("email", $"DisableClient: Error processing recurring order {recurringOrder.ReoccuringOrderID}: {ex.Message}");
                        }
                    }
                    
                    // Mark all as needing manual action since we can't disable automatically
                    recurringInfo.FailedToDisable = recurringInfo.TotalRecurringOrders;
                }
                else
                {
                    AppLogger.WriteLog("email", $"DisableClient: No recurring orders found for customer {customerId}");
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Error checking recurring orders for customer {customerId}: {ex.Message}");
            }
            
            return recurringInfo;
        }

        private void SendCustomerGoodbyeEmail(int customerId, string companyName)
        {
            try
            {
                var emailManager = new CoffeeCheckupEmailManager();
                
                var customersTbl = new CustomersTbl();
                var customerData = customersTbl.GetCustomerByCustomerID(customerId);
                
                if (customerData == null)
                {
                    AppLogger.WriteLog("email", $"DisableClient: Could not find customer data for goodbye email - Customer {customerId}");
                    return;
                }
                
                string customerEmail = !string.IsNullOrEmpty(customerData.EmailAddress) 
                    ? customerData.EmailAddress 
                    : customerData.AltEmailAddress;
                
                if (string.IsNullOrEmpty(customerEmail))
                {
                    AppLogger.WriteLog("email", $"DisableClient: No email address found for customer {customerId} - skipping goodbye email");
                    return;
                }
                
                // Use MessageProvider for all email content
                string subject = MessageProvider.Get(MessageKeys.DisableClient.GoodbyeSubject);
                string systemEmail = ConfigurationManager.AppSettings["SysFromEmail"] ?? "info@quaffee.co.za";
                
                string body = MessageProvider.GetFormattedHtmlEmail(
                    MessageKeys.DisableClient.GoodbyeHeader,
                    MessageKeys.DisableClient.GoodbyeMessage,
                    MessageKeys.DisableClient.GoodbyeWhatThisMeans,
                    MessageKeys.DisableClient.GoodbyeReenableInstructions,
                    MessageKeys.DisableClient.GoodbyeStillNeedCoffee,
                    MessageKeys.DisableClient.GoodbyeThankYou,
                    MessageKeys.DisableClient.GoodbyeFooter,
                    companyName,
                    systemEmail
                );
                
                bool emailSent = emailManager.SendDirectEmail(customerEmail, subject, body);
                
                if (emailSent)
                {
                    AppLogger.WriteLog("email", $"DisableClient: Goodbye email sent successfully to {customerEmail} for customer {customerId}");
                }
                else
                {
                    AppLogger.WriteLog("email", $"DisableClient: Failed to send goodbye email to {customerEmail} for customer {customerId}");
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Error sending goodbye email to customer {customerId}: {ex.Message}");
            }
        }

        private void NotifyAdministrator(int customerId, string companyName, RecurringOrdersInfo recurringInfo)
        {
            try
            {
                var emailManager = new CoffeeCheckupEmailManager();
                
                string subject = string.Format(MessageProvider.Get(MessageKeys.DisableClient.AdminSubjectTemplate), companyName);
                
                string body = string.Format(MessageProvider.Get(MessageKeys.DisableClient.AdminBodyHeader),
                    customerId,
                    companyName,
                    TimeZoneUtils.Now().ToString("yyyy-MM-dd HH:mm"));
                
                if (recurringInfo.TotalRecurringOrders > 0)
                {
                    body += string.Format(MessageProvider.Get(MessageKeys.DisableClient.AdminRecurringFound),
                        recurringInfo.TotalRecurringOrders,
                        recurringInfo.DisabledRecurringOrders,
                        recurringInfo.FailedToDisable);
                    
                    if (recurringInfo.RecurringOrderDetails.Any())
                    {
                        body += "\nRecurring orders found:\n" + string.Join("\n", recurringInfo.RecurringOrderDetails) + "\n";
                    }
                    
                    // Always show manual action message if there are recurring orders
                    body += "\n⚠️ MANUAL ACTION REQUIRED: Please disable these recurring orders manually in the system.\n" +
                           "Go to Recurring Orders page and disable all orders for this customer.\n\n";
                }
                else
                {
                    body += MessageProvider.Get(MessageKeys.DisableClient.AdminRecurringNone);
                }
                
                body += MessageProvider.Get(MessageKeys.DisableClient.AdminFooter);

                bool notificationSent = emailManager.SendAdminNotification(subject, body);
                
                if (notificationSent)
                {
                    AppLogger.WriteLog("email", $"DisableClient: Admin notification sent for customer {customerId}");
                }
                else
                {
                    AppLogger.WriteLog("email", $"DisableClient: Failed to send admin notification for customer {customerId}");
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"DisableClient: Failed to send admin notification: {ex.Message}");
            }
        }

        private void ShowError(string errorMessage)
        {
            confirmationSection.Visible = false;
            successSection.Visible = false;

            var errorDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            errorDiv.Attributes["class"] = "content";
            errorDiv.InnerHtml = $"<h2 style='color: #dc3545;'>{MessageProvider.Get(MessageKeys.DisableClient.ErrorHeader)}</h2>" +
                               $"<p style='font-size: 16px; margin: 20px 0;'>{errorMessage}</p>" +
                               $"<p style='font-size: 14px; color: #666;'>{MessageProvider.Get(MessageKeys.DisableClient.HelpMessage)} {ltrlContactEmail?.Text ?? "<a href='mailto:info@quaffee.co.za'>info@quaffee.co.za</a>"}</p>";

            frmDisable.Controls.Add(errorDiv);

            AppLogger.WriteLog("email", $"DisableClient: Error displayed - {errorMessage}");
        }
    }

    public class RecurringOrdersInfo
    {
        public int TotalRecurringOrders { get; set; } = 0;
        public int DisabledRecurringOrders { get; set; } = 0;
        public int FailedToDisable { get; set; } = 0;
        public List<string> RecurringOrderDetails { get; set; } = new List<string>();
    }
}