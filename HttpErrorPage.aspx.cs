using System;
using System.Web;

namespace TrackerDotNet
{
    public partial class HttpErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string errorMessage = Server.HtmlEncode(Request.QueryString["msg"]);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    lblErrorMessage.Text = errorMessage;
                }
                else
                {
                    lblErrorMessage.Text = "An unknown error occurred.";
                }
            }
        }
    }
}
