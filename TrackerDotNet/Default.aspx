<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TrackerDotNet._Default" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Welcome to Tracker.NET</h2>
    <br />
    <table style="width:100%">
      <tr>
        <td colspan="2"><h3>Customers Stuff</h3></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/Customers.aspx">Customers</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/SendCoffeeCheckup.aspx">Send Coffee Checkup</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/SentRemindersSheet.aspx">See Reminders Sent</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/LoadSendCoffeeCheckup.aspx">Send Reminders</a></td>
      </tr>
      <tr>
        <td colspan="2"><h3>Order Stuff</h3></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/NewOrderDetail.aspx">New Order</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/DeliverySheet.aspx">View and Edit Orders</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/ReoccuringOrders.aspx">Reoccuring Orders</a></td>
      </tr>
      <tr>
        <td colspan="2"><h3>Repair Stuff</h3></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/Repairs.aspx">Repairs</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/RepairDetail.aspx">New Repair</a></td>
      </tr>
      <tr>
        <td colspan="2"><h3>Preperation Stuff</h3></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/CoffeeRequired.aspx">Required Sheet</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/DeliverySheet.aspx">Delivery Sheet</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
          <a href="Pages/PreperationSummary.aspx">Weekly Preperation Summary</a></td>
      </tr>
      <tr>
        <td>
            &nbsp;</td>
        <td>
            &nbsp;</td>
      </tr>
    </table>
    <div style="margin: 10px; padding: 10px; background-color: #EEEEEE; color: Orange; font: bold 'Franklin Gothic Medium', 'Arial Narrow', Arial, sans-serif; text-align: center ">
      Total Cups So Far:&nbsp;&nbsp;<asp:Label ID="lblTotalCupCount" Text="" runat="server" CssClass="bold"  />
    </div>
        <asp:SqlDataSource ID="sdsCupCountTotal" runat="server" 
          ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
          ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
          
      SelectCommand="SELECT SUM(LastCupCount) AS TotalCupCount FROM ClientUsageTbl"></asp:SqlDataSource>

    </asp:Content>
