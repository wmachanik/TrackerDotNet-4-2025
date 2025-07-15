using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Mail;

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

                if (!message.To?.Any() ?? true)
                    message.To.Add(MailboxAddress.Parse(emailConfig.ToAddress));

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Timeout = emailConfig.Timeout;
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
                    AppLogger.WriteLog("email", $"🔌 Connected to SMTP: {emailConfig.SmtpHost}:{emailConfig.SmtpPort} ({option})");

                    client.Authenticate(emailConfig.SmtpUser, emailConfig.SmtpPass);
                    AppLogger.WriteLog("email", $"🔐 Authenticated as {emailConfig.SmtpUser}");

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
                msg.To.Add(MailboxAddress.Parse(to));

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
                    AppLogger.WriteLog("email", $"🔌 SMTP session opened for batch delivery.");

                    foreach (var msg in batchMessages)
                    {
                        client.Send(msg);
                        AppLogger.WriteLog("email", $"📧 Sent batch message to: {string.Join(", ", msg.To)}");
                    }

                    client.Disconnect(true);
                }

                batchMessages.Clear(); // empty batch after send
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"❌ Batch send failed: {ex.Message}");
                return false;
            }
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
    }

}
