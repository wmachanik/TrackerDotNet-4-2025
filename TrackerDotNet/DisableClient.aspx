<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisableClient.aspx.cs" Inherits="TrackerDotNet.DisableClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="frmDisable" runat="server">
    <div>
      <br />
      <span style="float: right; font-family: Calibri @FreeSans; font-size: larger">
        Tracking and Customer Ssytem&nbsp;&nbsp;&nbsp;&nbsp;
      </span>
      <img src="images/logo/QuaffeeLogoSmall.jpg" alt="quaffee"/>
      <br />
      <br />
      <div style="background-color:#F9FFDF; text-align: center; height: 400px; padding:30px">
        <div style="font-family: Calibri @Arial Unicode MS; font-size:large">
          Your request to be disabled has been processed:<br />
          <div style="font-size: x-large; font-weight: bold">
            <asp:Label ID="CompanyNameLabel" Text="n/a" runat="server" />
          </div>
          has been disabled and the administrator has bee notified
  
        </div>
      </div>
    
    </div>
    <br />
    Visit Quaffee's home page here -> <a href="http://www.quaffee.co.za">quaffee.co.za</a>
    </form>
</body>
</html>
