// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Global
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using TrackerDotNet.classes;

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
            string logEntry = DateTime.Now + " - " + lastError.ToString() + Environment.NewLine;

            using (var stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(logEntry);
            }


            // Optional: redirect to a friendly error page
            Response.Redirect("~/HttpErrorPage.aspx?msg=" + HttpUtility.UrlEncode(lastError.Message));

            Server.ClearError();
            //if (HttpContext.Current != null)
            //{
            //    TrackerTools trackerTools = new TrackerTools();
            //    Page handler = HttpContext.Current.Handler as Page;
            //    string sessionErrorString = trackerTools.GetTrackerSessionErrorString();
            //    if (!string.IsNullOrEmpty(sessionErrorString))
            //    {
            //        showMessageBox showMessageBox = new showMessageBox(handler, "App error", "ERROR: " + sessionErrorString);
            //        trackerTools.SetTrackerSessionErrorString(string.Empty);
            //    }
            //    Exception lastError = this.Server.GetLastError();
            //    if (lastError.GetType() == typeof(HttpException))
            //    {
            //        if (lastError.Message.Contains("NoCatch") || lastError.Message.Contains("maxUrlLength"))
            //            return;
            //        this.Server.Transfer("HttpErrorPage.aspx");
            //    }
            //    showMessageBox showMessageBox1 = new showMessageBox(handler, "App error", "ERROR: " + lastError.Message);
            //    trackerTools.SetTrackerSessionErrorString(string.Empty);
            //}
            //this.Server.ClearError();
        }

        private void Session_Start(object sender, EventArgs e)
        {
        }

        private void Session_End(object sender, EventArgs e)
        {
        }
    }
}