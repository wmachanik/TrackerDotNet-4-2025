<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeliverySheet.aspx.cs" Inherits="TrackerDotNet.Pages.DeliverySheet" %>
<asp:Content ID="cntDeliveryHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntDeliveryBdy" ContentPlaceHolderID="MainContent" runat="server">
  <asp:ScriptManager ID="smDelivery" runat="server" />
  <asp:Panel ID="pnlDeliveryDate" runat="server">
    <h1>Delivery Sheet</h1>
    <asp:UpdateProgress runat="server" ID="uprgDeliveryFilterBy">
      <ProgressTemplate>
        <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....</ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="upnlDeliveryFilterBy" runat="server" ChildrenAsTriggers="true" >
      <ContentTemplate>
        <div class="simpleLightBrownForm">
          &nbsp;Delivery Date:
          <asp:DropDownList ID="ddlActiveRoastDates" runat="server" 
            DataSourceID="odsActiveRoastDates" DataTextField="RequiredByDate"  DataTextFormatString="{0:dd-MMM-yyyy (ddd)}"
            DataValueField="RequiredByDate" AppendDataBoundItems="true" 
            OnSelectedIndexChanged="ddlActiveRoastDates_SelectedIndexChanged">
            <asp:ListItem Value="2014-01-01" Text="--- Select Date ---"  />
          </asp:DropDownList>
          <asp:Label ID="lblDeliveryBy" runat="server" Text="By:" />
          <asp:DropDownList ID="ddlDeliveryBy" runat="server" AutoPostBack="true" Visible="false"
              onselectedindexchanged="ddlDeliveryBy_SelectedIndexChanged" />&nbsp;&nbsp;
          <asp:Button ID="btnGo" runat="server" Text="Go" onclick="btnGo_Click" />
          &nbsp;&nbsp;&nbsp;
           <asp:Button ID="btnRefresh" Text="Refresh"
            runat="server" ToolTip="Referesh the active delivery days" 
            onclick="btnRefresh_Click" />&nbsp;&nbsp;
          <span style="float:right">
            To:
            <asp:TextBox ID="tbxFindClient" runat="server" OnTextChanged="tbxFindClient_OnTextChanged" AutoPostBack="true" />&nbsp;
            <asp:Button ID="btnFind" Text="Find" runat="server" onclick="btnFind_Click" />&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnPrint" runat="server" Text="Print"  onclick="btnPrint_Click" CssClass="hideWhenPrinting" />
          </span>
        </div >
      </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsActiveRoastDates" runat="server" 
      OldValuesParameterFormatString="original_{0}" 
      SelectMethod="GetActiveDeliveryDates" 
      TypeName="TrackerDotNet.DataSets.ActiveDeliveriesDataSetTableAdapters.OrdersTblTableAdapter">
    </asp:ObjectDataSource>   
   <br />
   </asp:Panel>
   <asp:UpdatePanel ID="upnlDeliveryItems" runat="server" UpdateMode="Conditional">
     <ContentTemplate>
        <asp:Table ID="tblDeliveries" runat="server" CssClass="TblZebra" Width="100%"  >  
          <asp:TableHeaderRow TableSection="TableHeader" >
            <asp:TableHeaderCell>By</asp:TableHeaderCell>
            <asp:TableHeaderCell>To</asp:TableHeaderCell>
            <asp:TableHeaderCell Width="90px">Received By</asp:TableHeaderCell>
            <asp:TableHeaderCell Width="100px">Signature</asp:TableHeaderCell>
            <asp:TableHeaderCell>Items</asp:TableHeaderCell>
            <asp:TableHeaderCell>In Stock</asp:TableHeaderCell>
          </asp:TableHeaderRow>
        </asp:Table>
      <br />
      <asp:Table ID="tblTotals" runat="server" CssClass="TblCoffee" Width="100%">
      </asp:Table>
      <div style="text-align: right; width: 98%" class="small">
      <asp:Label ID="ltrlWhichDate" Text="" runat="server" CssClass="small" />
      </div>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnFind" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="tbxFindClient" EventName="TextChanged" />
      <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="ddlDeliveryBy" 
        EventName="SelectedIndexChanged" />
    </Triggers>
  </asp:UpdatePanel>

</asp:Content>