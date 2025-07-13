<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TrackerDotNet._Default" %>

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
        <td>
          <a href="Pages/CustomerDetails.aspx">New Customer</a></td>
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
          <a href="Pages/OrdersEdit.aspx">View and Edit Orders</a></td>
      </tr>
      <tr>
        <td>
          &nbsp;</td>
        <td>
          <a href="Pages/OrderSheet.aspx">Orders Sheet</a></td>
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
    <div style="margin: 10px; padding: 10px; background-color: #EEEEEE; color: Orange; font:@MS UI Gothic larger; text-align: center ">
      Total Cups So Far:&nbsp;&nbsp;<asp:Label ID="lblTotalCupCount" Text="" runat="server" CssClass="bold"  />
    </div>
        <asp:SqlDataSource ID="sdsCupCountTotal" runat="server" 
          ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
          ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
          
      SelectCommand="SELECT SUM(LastCupCount) AS TotalCupCount FROM ClientUsageTbl"></asp:SqlDataSource>
    </asp:Content>
