using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace TrackerDotNet.Classes
{
    public class EmailMailKitCls
    {

        public struct SendMailResults
        {
            public string sResult;
            public string sID;
        }

        public SendMailResults myResults;
        private MimeMessage message;
        private BodyBuilder bodyBuilder;
        public string LastErrorSummary { get; private set; } = "";
        private EmailSettings emailConfig;

        // Updated test mode properties that read from web.config
        public bool IsTestMode 
        { 
            get 
            { 
                // Default to false if config missing/invalid for safety
                return bool.TryParse(ConfigurationManager.AppSettings["EmailTestMode"], out bool result) && result;
            } 
        }
        
        public string TestRecipientAddress 
        { 
            get 
            { 
                return ConfigurationManager.AppSettings["EmailTestRecipient"] ?? "warren@machanik.com";
            } 
        }

        // batch message stuff
        private readonly List<MimeMessage> batchMessages = new List<MimeMessage>();
        /// <summary>
        /// Default constructor for EmailClsMailKit.
        /// Loads EmailSettings from Web.config and prepares the email infrastructure.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if EmailSettings fails to initialize from Web.config.
        /// </exception>
        public EmailMailKitCls()
        {
            var config = EmailSettings.CreateFromConfig();
            if (config == null || !config.IsInitialized)
            {
                AppLogger.WriteLog("email", "❌ Failed to initialize EmailSettings from Web.config.");
                throw new InvalidOperationException("EmailSettings failed to initialize from Web.config.");
            }

            emailConfig = config;
            message = new MimeMessage();
            bodyBuilder = new BodyBuilder();
            myResults = new SendMailResults();

            AppLogger.WriteLog("email", "✅ EmailMailKitCls initialized using Web.config settings.");
        }

        public EmailMailKitCls(EmailSettings config)
        {
            if (config == null || !config.IsInitialized)
            {
                AppLogger.WriteLog("email", "❌ Provided EmailSettings instance is not initialized.");
                throw new InvalidOperationException("EmailSettings must be initialized before use.");
            }

            emailConfig = config;
            message = new MimeMessage();
            bodyBuilder = new BodyBuilder();
            myResults = new SendMailResults();

            AppLogger.WriteLog("email", "✅ EmailMailKitCls initialized using provided EmailSettings.");
        }
        public bool SetEmailFromTo(string sFrom = null, string sTo = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(sFrom))
                    emailConfig.UpdateCommonSettings(emailConfig.SmtpUser, emailConfig.SmtpPass, newFrom: sFrom);

                if (!string.IsNullOrEmpty(sTo))
                    emailConfig.UpdateCommonSettings(emailConfig.SmtpUser, emailConfig.SmtpPass, newTo: sTo);

                AppLogger.WriteLog("email", $"✍️ Overwrote From and To in emailConfig: {emailConfig.FromAddress} → {emailConfig.ToAddress}");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"⚠️ Failed to overwrite From/To in emailConfig: {ex.Message}");
                return false;
            }
        }
        public void SetEmailSubject(string subject) => message.Subject = subject;
        public void AddToBody(string htmlBody) => bodyBuilder.HtmlBody += htmlBody;
        public void AddStrAndNewLineToBody(string htmlLine) => bodyBuilder.HtmlBody += htmlLine + "<br />";
        /// <summary>
        /// Appends a formatted string with one parameter to the HTML body.
        /// </summary>
        public void AddFormatToBody(string plainString)
        {
            bodyBuilder.HtmlBody += string.Format(plainString);
        }
        public void AddFormatToBody(string format, object arg1)
        {
            bodyBuilder.HtmlBody += string.Format(format, arg1);
        }

        public void AddFormatToBody(string format, object arg1, object arg2)
        {
            bodyBuilder.HtmlBody += string.Format(format, arg1, arg2);
        }
        public void AddFormatToBody(string format, object arg1, object arg2, object arg3)
        {
            bodyBuilder.HtmlBody += string.Format(format, arg1, arg2, arg3);
        }
        public void AddPDFAttachment(string filePath)
        {
            if (File.Exists(filePath))
            {
                bodyBuilder.Attachments.Add(filePath);
                AppLogger.WriteLog("email", $"📎 PDF attached: {Path.GetFileName(filePath)}");
            }
            else
            {
                AppLogger.WriteLog("email", $"❗ Attachment not found: {filePath}");
            }
        }
        /// <summary>
        /// Adds the 'From' address to the CC list if it's not already present.
        /// Uses MailKit-safe access without reassigning read-only properties.
        /// </summary>
        public void AddSysCCFAddress()
        {
            if (emailConfig == null || string.IsNullOrWhiteSpace(emailConfig.CcAddress))
                return;

            try
            {
                var fromAddress = MailboxAddress.Parse(emailConfig.CcAddress);

                // Avoid duplicates — message.Cc is already initialized
                if (!message.Cc.Any(cc => cc is MailboxAddress m &&
                    m.Address.Equals(fromAddress.Address, StringComparison.OrdinalIgnoreCase)))
                {
                    message.Cc.Add(fromAddress);
                    AppLogger.WriteLog("email", $"📋 CC added: {fromAddress.Address}");
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"❌ Failed to add CC from address: {ex.Message}");
            }
        }

        public bool SendEmail()
        {
            if (emailConfig == null || !emailConfig.IsInitialized)
            {
                AppLogger.WriteLog("email", "❌ SendEmail aborted: EmailSettings not initialized.");
                return false;
            }

            try
            {
                // Auto-assign From and To if not already set
                if (!message.From?.Any() ?? true)
                    message.From.Add(MailboxAddress.Parse(emailConfig.FromAddress));

                string actualTo = emailConfig.ToAddress;

                // Check test mode at runtime instead of compile time
                if (IsTestMode) 
                {
                    actualTo = TestRecipientAddress;
                }

                if (!message.To?.Any() ?? true)
                    message.To.Add(MailboxAddress.Parse(actualTo));

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Timeout = emailConfig.Timeout;
                    // Disable revocation check to avoid SSLHandshakeException
                    client.CheckCertificateRevocation = false;

                    // Optional: keep this if you're okay bypassing all validation
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;


                    SecureSocketOptions option;
                    switch (emailConfig.SocketOption)
                    {
                        case "None":
                            option = SecureSocketOptions.None;
                            break;
                        case "SslOnConnect":
                            option = SecureSocketOptions.SslOnConnect;
                            break;
                        case "StartTls":
                            option = SecureSocketOptions.StartTls;
                            break;
                        case "StartTlsWhenAvailable":
                            option = SecureSocketOptions.StartTlsWhenAvailable;
                            break;
                        default:
                            option = SecureSocketOptions.Auto;
                            break;
                    }

                    client.Connect(emailConfig.SmtpHost, emailConfig.SmtpPort, option);
                    client.Authenticate(emailConfig.SmtpUser, emailConfig.SmtpPass);

                    // Log test mode warning at runtime
                    if (IsTestMode) 
                    {
                        AppLogger.WriteLog("email", "🚨 SEND EMAIL BATCH IS IN TEST MODE 🚨");
                    }

                    AppLogger.WriteLog("email", $"🔌 SMTP session opened for batch delivery.");

                    client.Send(message);
                    AppLogger.WriteLog("email", $"📧 Sent to: {string.Join(", ", message.To)}");

                    client.Disconnect(true);
                }

                myResults.sID = Guid.NewGuid().ToString();
                myResults.sResult = "Success";
                return true;
            }
            catch (Exception ex)
            {
                var details = new System.Text.StringBuilder();
                details.AppendLine("Exception Type: " + ex.GetType().Name);
                details.AppendLine("Message: " + ex.Message);
                details.AppendLine("Stack Trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    details.AppendLine("Inner Exception: " + ex.InnerException.Message);
                    details.AppendLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
                }

                LastErrorSummary = $"{ex.GetType().Name}: {ex.Message}";
                myResults.sResult = "ERROR:\n" + details.ToString();
                AppLogger.WriteLog("email", $"❌ Send failed:\n{details}");
                return false;
            }
        }
        // Batch email stuff here
        /// <summary>
        /// Creates and adds an email message to the batch with given subject and body.
        /// Optional override for recipient addresses.
        /// </summary>
        public bool AddToBatch(string subject, string htmlBody, string sFrom = null, string sTo = null)
        {
            try
            {
                var msg = new MimeMessage();

                // Set From/To — default to config unless overridden
                string from = sFrom ?? emailConfig.FromAddress;
                string to = sTo ?? emailConfig.ToAddress;

                msg.From.Add(MailboxAddress.Parse(from));
                string actualTo = to; // Use the 'to' parameter (which can be overridden)

                // Check test mode at runtime
                if (IsTestMode) 
                {
                    actualTo = TestRecipientAddress;
                }

                msg.To.Add(MailboxAddress.Parse(actualTo));

                msg.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                msg.Body = builder.ToMessageBody();
                batchMessages.Add(msg);
                AppLogger.WriteLog("email", $"📦 Added message to batch: {subject} → {to}");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"⚠️ Failed to add message to batch: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Queues a message for batch delivery.
        /// Use this method to build up one or more messages before sending via SendEmailBatch().
        /// </summary>
        /// <param name="subject">Subject line of the email.</param>
        /// <param name="htmlBody">HTML content for the message body.</param>
        /// <param name="sFrom">Optional override for From address. Uses emailConfig if null.</param>
        /// <param name="sTo">Optional override for To address. Uses emailConfig if null.</param>
        /// <returns>True if added successfully; false if any error occurs.</returns>
        /// <remarks>
        /// Call this multiple times to queue a batch of emails. Once ready, call SendEmailBatch() to dispatch all at once.
        public bool SendEmailBatch()
        {
            if (emailConfig == null || !emailConfig.IsInitialized || !batchMessages.Any())
            {
                myResults.sResult = "ERROR: EmailSettings not initialized or batch empty";
                AppLogger.WriteLog("email", "❌ Batch send aborted: Invalid config or no messages.");
                return false;
            }

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Timeout = emailConfig.Timeout;
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    SecureSocketOptions option;
                    switch (emailConfig.SocketOption)
                    {
                        case "None": option = SecureSocketOptions.None; break;
                        case "SslOnConnect": option = SecureSocketOptions.SslOnConnect; break;
                        case "StartTls": option = SecureSocketOptions.StartTls; break;
                        case "StartTlsWhenAvailable": option = SecureSocketOptions.StartTlsWhenAvailable; break;
                        default: option = SecureSocketOptions.Auto; break;
                    }

                    client.Connect(emailConfig.SmtpHost, emailConfig.SmtpPort, option);
                    client.Authenticate(emailConfig.SmtpUser, emailConfig.SmtpPass);
                    if (IsTestMode) AppLogger.WriteLog("email", "!!!!! SendEmailBatch is in TestMode !!!!!");
                    AppLogger.WriteLog("email", $"🔌 SMTP session opened for batch delivery.");

                    foreach (var msg in batchMessages)
                    {
                        client.Send(msg);
                        AppLogger.WriteLog("email", $"📧 Sent batch message to: {string.Join(", ", msg.To)}");
                    }

                    client.Disconnect(true);
                }

                myResults.sID = Guid.NewGuid().ToString();
                myResults.sResult = "Success";
                batchMessages.Clear();

                return true;
            }
            catch (Exception ex)
            {
                var details = new System.Text.StringBuilder();
                details.AppendLine("Exception Type: " + ex.GetType().Name);
                details.AppendLine("Message: " + ex.Message);
                details.AppendLine("Stack Trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    details.AppendLine("Inner Exception: " + ex.InnerException.Message);
                    details.AppendLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
                }

                myResults.sResult = "ERROR:\n" + details.ToString();
                LastErrorSummary = $"{ex.GetType().Name}: {ex.Message}";
                AppLogger.WriteLog("email", $"❌ Batch send failed:\n{details}");

                return false;
            }
        }
        /// <summary>
        /// Returns a formatted HTML-friendly status message based on the latest result.
        /// </summary>
        public string GetFormattedResultMessage(bool isSuccess)
        {
            if (string.IsNullOrWhiteSpace(myResults.sResult))
                return "❓ No result available.";

            return isSuccess
                ? "✅ Email sent successfully."
                : $"❌ Failed:<br />{myResults.sResult.Replace("\n", "<br />")}";
        }
        /// <summary>
        /// Runs diagnostics against the SMTP server using current email configuration.
        /// Logs the results to App_Data/email.log.
        /// </summary>
        /// <returns>A formatted summary of server diagnostics.</returns>
        public string GetServerDiagnostics()
        {
            var diagnostics = new System.Text.StringBuilder();

            if (emailConfig == null || !emailConfig.IsInitialized)
            {
                diagnostics.AppendLine("❌ EmailSettings is not initialized.");
                AppLogger.WriteLog("email", diagnostics.ToString());
                return diagnostics.ToString();
            }

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Timeout = emailConfig.Timeout;

                    SecureSocketOptions option;
                    switch (emailConfig.SocketOption)
                    {
                        case "None": option = SecureSocketOptions.None; break;
                        case "SslOnConnect": option = SecureSocketOptions.SslOnConnect; break;
                        case "StartTls": option = SecureSocketOptions.StartTls; break;
                        case "StartTlsWhenAvailable": option = SecureSocketOptions.StartTlsWhenAvailable; break;
                        default: option = SecureSocketOptions.Auto; break;
                    }

                    client.Connect(emailConfig.SmtpHost, emailConfig.SmtpPort, option);
                    diagnostics.AppendLine("✅ Connected to SMTP server.");
                    diagnostics.AppendLine("Server Capabilities:");
                    diagnostics.AppendLine(client.Capabilities.ToString());

                    diagnostics.AppendLine("\nSupported Authentication Mechanisms:");
                    diagnostics.AppendLine(string.Join(", ", client.AuthenticationMechanisms));

                    client.Disconnect(true);
                }

                AppLogger.WriteLog("email", "📊 Diagnostics complete:\n" + diagnostics.ToString());
            }
            catch (Exception ex)
            {
                diagnostics.AppendLine("❌ Error during diagnostics:");
                diagnostics.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    diagnostics.AppendLine("Inner: " + ex.InnerException.Message);

                AppLogger.WriteLog("email", "⚠️ Diagnostics failed:\n" + diagnostics.ToString());
            }

            return diagnostics.ToString();
        }
        /// <summary>
        /// Appends a formatted string with one or more parameters to the HTML body, followed by a line break.
        /// </summary>
        public void AddFormatAndNewLineToBody(string format, params object[] args)
        {
            bodyBuilder.HtmlBody += string.Format(format, args) + "<br />";
        }
    }

}
