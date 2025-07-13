// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.SiteMaster
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

#nullable disable
namespace TrackerDotNet;

public class SiteMaster : MasterPage
{
  protected ContentPlaceHolder HeadContent;
  protected HtmlForm frmMain;
  protected Menu NavigationMenu;
  protected LoginView HeadLoginView;
  protected ContentPlaceHolder MainContent;

  protected void Page_Load(object sender, EventArgs e)
  {
    string relativeVirtualPath = this.Page.AppRelativeVirtualPath;
    int startIndex = relativeVirtualPath.IndexOf("/");
    string str = "~" + relativeVirtualPath.Substring(startIndex, relativeVirtualPath.Length - startIndex).ToUpper();
    foreach (MenuItem menuItem in this.NavigationMenu.Items)
    {
      if (menuItem.NavigateUrl.ToUpper() == str)
      {
        menuItem.Selected = true;
        menuItem.Text = $">{menuItem.Text}<";
        menuItem.Enabled = false;
      }
      else
      {
        foreach (MenuItem childItem in menuItem.ChildItems)
        {
          if (childItem.NavigateUrl.ToUpper() == str)
          {
            childItem.Selected = true;
            childItem.Text = $">{childItem.Text}<";
            childItem.Enabled = false;
          }
        }
      }
    }
  }
}
