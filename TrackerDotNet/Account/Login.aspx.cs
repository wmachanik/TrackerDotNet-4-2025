// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Account.Login
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// // #nullable disable --- not for this version of C# --- not for this version of C#
//namespace TrackerDotNet.Account

namespace TrackerDotNet.Account
{
    public class Login : Page
    {
        protected HyperLink RegisterHyperLink;
        protected System.Web.UI.WebControls.Login LoginUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check for null controls first (common in Web Forms)
            if (RegisterHyperLink == null || Request == null || Request.QueryString == null)
            {
                return; // or handle the null case appropriately
            }

            // Get the ReturnUrl parameter safely
            string returnUrl = Request.QueryString["ReturnUrl"];

            // Set the navigation URL with null check
            RegisterHyperLink.NavigateUrl = "Register.aspx" +
                (!string.IsNullOrEmpty(returnUrl)
                    ? "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl)
                    : string.Empty);
        }
    }
}

