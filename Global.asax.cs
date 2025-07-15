// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Global
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using TrackerDotNet.Classes;

//- only form later versions #nullable disable
namespace TrackerDotNet
{

    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Force-disable unobtrusive validation
            System.Web.UI.ValidationSettings.UnobtrusiveValidationMode =
                System.Web.UI.UnobtrusiveValidationMode.None;
        }

        private void Application_End(object sender, EventArgs e)
        {
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
            if (lastError == null)
            {
                return; // No error to log
            }
            // Log the error
            string logPath = Server.MapPath("~/App_Data/ErrorLog.txt");
            Exception root = lastError.InnerException ?? lastError;
            string logEntry = $"[{TimeZoneUtils.Now()}]\n{root.GetType()}: {root.Message}\n{root.StackTrace}\n----------------------\n";


            using (var stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(logEntry);
            }


            // Redirect to a friendly error page
            string cleanMessage = System.Text.RegularExpressions.Regex.Replace(root.Message, "<.*?>", "");
            Response.Redirect("~/HttpErrorPage.aspx?msg=" + HttpUtility.UrlEncode(cleanMessage), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Server.ClearError();

        }

        private void Session_Start(object sender, EventArgs e)
        {
        }

        private void Session_End(object sender, EventArgs e)
        {
        }
    }
}