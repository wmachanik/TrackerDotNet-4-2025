<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PreperationSummary.aspx.cs" Inherits="TrackerDotNet.Pages.PreperationSummary" %>
<asp:Content ID="cntPreSummaryHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntPreSummaryBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Preperation Summary</h1>
  <table class="TblLHCol-brown">
    <tr>
      <td class="TblLHCol-first">Date From</td>
      <td>
        <asp:DropDownList ID="ddlDateFrom" runat="server" AppendDataBoundItems="true" 
          AutoPostBack="true" onselectedindexchanged="ddlDateFrom_SelectedIndexChanged">
        </asp:DropDownList>
      </td>
    </tr>
    <tr>
      <td class="TblLHCol-first">Date To</td>
      <td>
        <asp:DropDownList ID="ddlDateTo" runat="server" AppendDataBoundItems="true" 
          AutoPostBack="true" onselectedindexchanged="ddlDateTo_SelectedIndexChanged">
        </asp:DropDownList>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="text-align:center">
        <asp:Button ID="BackBtn" Text="Back" runat="server" onclick="BackBtn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="GoBtn" Text="Go" runat="server" onclick="GoBtn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="ForwardBtn" Text="Forward" runat="server" onclick="ForwardBtn_Click" /></td>
    </tr>
  </table>
  <br />
  <asp:GridView ID="gvPreperationSummary" runat="server" AutoGenerateColumns="False" 
    CssClass="TblZebra" OnRowDataBound="gvPreperationSummary_RowDataBound" 
    EmptyDataText="Please Select a Date range with a valid prep date" 
    ShowFooter="True"  >  
    <Columns>
      <asp:TemplateField HeaderText=" Item Description " SortExpression="ItemDesc">
        <ItemTemplate>
          <asp:Label ID="lblItemDesc" runat="server" Text='<%#Eval("ItemDesc") %>' />
        </ItemTemplate>
        <FooterTemplate>
          <asp:Label ID="lblTotalDesc" Text="Total" runat="server" Font-Bold="true" />
        </FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText=" Quantity " SortExpression="Quantity" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" >
        <ItemTemplate>
          <asp:Label ID="lblQty" runat="server"   Text='<%#Eval("Quantity") %>' />
        </ItemTemplate>
        <FooterTemplate>
          <asp:Label ID="lblTotalQty" Text="" runat="server" />
        </FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField>
        <HeaderTemplate>
          <asp:Label ID="lblDescHdr" Text="" runat="server" />
        </HeaderTemplate>
        <ItemTemplate>
          <asp:Label ID="lblDescItem" Text="" runat="server" />
        </ItemTemplate>
        <FooterTemplate>
          <asp:Label ID="lblDescFotter" Text="" runat="server" />
        </FooterTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
  <asp:Literal id="ltrlDates" Text="" runat="server" />
</asp:Content>
