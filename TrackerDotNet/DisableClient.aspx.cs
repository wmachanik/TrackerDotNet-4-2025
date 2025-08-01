﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.DisableClient
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TrackerDotNet.control;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet
{

    public class DisableClient : Page
    {
        protected HtmlForm frmDisable;
        protected Label CompanyNameLabel;

        private void DisableCustomerTracking(string pCustID)
        {
            int result;
            if (int.TryParse(pCustID, out result))
            {
                string str = ConfigurationManager.AppSettings["SysEmailFrom"] == null ? "orders@quaffee.co.za" : ConfigurationManager.AppSettings["SysEmailFrom"];
                CustomersTbl customersTbl = new CustomersTbl();
                customersTbl.DisableCustomer((long)result);
                CustomersTbl customersByCustomerId = customersTbl.GetCustomersByCustomerID((long)result);
                this.CompanyNameLabel.Text = customersByCustomerId.CompanyName;
                string empty = string.Empty;
                string pObj1 = customersByCustomerId.ContactAltFirstName;
                if (!string.IsNullOrEmpty(pObj1))
                {
                    if (!string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName))
                        pObj1 = $"{pObj1} & {customersByCustomerId.ContactAltFirstName}";
                }
                else
                    pObj1 = string.IsNullOrEmpty(customersByCustomerId.ContactAltFirstName) ? "X Coffee lover" : customersByCustomerId.ContactAltFirstName;
                EmailCls emailCls = new EmailCls();
                if (!string.IsNullOrEmpty(customersByCustomerId.EmailAddress))
                    emailCls.SetEmailTo(customersByCustomerId.EmailAddress);
                if (!string.IsNullOrEmpty(customersByCustomerId.AltEmailAddress))
                    emailCls.SetEmailTo(customersByCustomerId.AltEmailAddress);
                emailCls.SetEmailCC(str);
                emailCls.SetEmailSubject(customersByCustomerId.CompanyName + " request to be disabled in Coffee Tracker");
                emailCls.AddFormatToBody("Dear {0}, <br /><br />", (object)pObj1);
                emailCls.AddFormatToBody("As requested we have disabled: {0} in Quaffee's Coffee Tracker.<br /><br />", (object)customersByCustomerId.CompanyName);
                emailCls.AddFormatToBody("We wish you the best in the future. Should you require anything else from us please email {0}.<br /><br />", (object)str);
                emailCls.AddStrAndNewLineToBody("The Quaffee Orders Team");
                emailCls.AddStrAndNewLineToBody("web: <a href='http://www.quaffee.co.za'>quaffee.co.za</a>");
                emailCls.SendEmail();
            }
            else
                this.CompanyNameLabel.Text = "Company not found";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack || this.Request.QueryString.Count <= 0 || this.Request.QueryString["CoID"] == null)
                return;
            this.DisableCustomerTracking(this.Request.QueryString["CoID"]);
        }
    }
}
