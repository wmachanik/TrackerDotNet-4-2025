<%@ Page Title="Reoccuring Orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReoccuringOrders.aspx.cs" Inherits="TrackerDotNet.Pages.ReoccuringOrders" %>
<asp:Content ID="cntReoccuringOrdersHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntReoccuringOrdersBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Reoccuring Orders</h1>

    <asp:GridView ID="gvReoccuringOrders" runat="server" CellPadding="4" 
    ForeColor="#333333" GridLines="None">
      <AlternatingRowStyle BackColor="White" />
      <Columns>
        <asp:CommandField ShowSelectButton="True" />
      </Columns>
      <EditRowStyle BackColor="#2461BF" />
      <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
      <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
      <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
      <RowStyle BackColor="#EFF3FB" />
      <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
      <SortedAscendingCellStyle BackColor="#F5F7FB" />
      <SortedAscendingHeaderStyle BackColor="#6D95E1" />
      <SortedDescendingCellStyle BackColor="#E9EBEF" />
      <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>

  <asp:Literal id="ltrlMessage" Text="" runat="server" />

</asp:Content>
