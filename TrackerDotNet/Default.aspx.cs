using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QOnT
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      sdsCupCountTotal.DataBind();
      DataView dvSql = (DataView)sdsCupCountTotal.Select(DataSourceSelectArguments.Empty);
      
      foreach (DataRowView drvSql in dvSql)
      {
        lblTotalCupCount.Text = String.Format("{0:n0}",drvSql["TotalCupCount"]);
      }
    }
  }
}
