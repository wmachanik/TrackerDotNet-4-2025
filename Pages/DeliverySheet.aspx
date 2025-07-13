<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeliverySheet.aspx.cs" Inherits="TrackerDotNet.Pages.DeliverySheet" %>
<asp:Content ID="cntDeliveryHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntDeliveryBdy" ContentPlaceHolderID="MainContent" runat="server">
  <ajaxToolkit:ToolkitScriptManager ID="smDelivery" runat="server" />
  <asp:Panel ID="pnlDeliveryDate" runat="server">
    <h1>Delivery Sheet</h1>
    <asp:UpdateProgress runat="server" ID="uprgDeliveryFilterBy">
      <ProgressTemplate>
        <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....</ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="upnlDeliveryFilterBy" runat="server" ChildrenAsTriggers="true" >
      <ContentTemplate>
        <div class="simpleLightBrownForm">
          <span style="float: left">
          &nbsp;Delivery Date:
          <asp:DropDownList ID="ddlActiveRoastDates" runat="server" 
            DataSourceID="odsActiveRoastDates" DataTextField="RequiredByDate"  DataTextFormatString="{0:dd-MMM-yyyy (ddd)}"
            DataValueField="RequiredByDate" AppendDataBoundItems="True" OnDataBound="ddlActiveRoastDates_DataBound"
            AutoPostBack="true" 
            OnSelectedIndexChanged="ddlActiveRoastDates_SelectedIndexChanged">
            <asp:ListItem Value="2014-01-01" Text="--- Select Date ---"  />
          </asp:DropDownList>
          &nbsp;&nbsp;
          <asp:Button ID="btnGo" runat="server" Text="Go" onclick="btnGo_Click" AccessKey="G" ToolTip="get the results (AltShftG)" />
          &nbsp;&nbsp;
           <asp:Button ID="btnRefresh" Text="Refresh" AccessKey="R" ToolTip="refresh lists (AltShftR)"
            runat="server" onclick="btnRefresh_Click" />&nbsp;&nbsp;
          </span>
          &nbsp;&nbsp;&nbsp;
          <asp:Label ID="lblDeliveryBy" runat="server" Text="By:" Visible="false" />
          <asp:DropDownList ID="ddlDeliveryBy" runat="server" AutoPostBack="true" Visible="false"
              OnSelectedIndexChanged="ddlDeliveryBy_SelectedIndexChanged" />&nbsp;&nbsp;
          <span style="float:right; vertical-align: baseline">
            <span style="color:green"></span> To:
            <asp:TextBox ID="tbxFindClient" runat="server" OnTextChanged="tbxFindClient_OnTextChanged" AutoPostBack="true" />&nbsp;
            <asp:Button ID="btnFind" Text="Find" runat="server" onclick="btnFind_Click" />&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;
            <asp:Button ID="btnPrint" runat="server" Text="Print"  onclick="btnPrint_Click" CssClass="hideWhenPrinting" AccessKey="P" ToolTip="print sheet (AltShftP)" />&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;
            <asp:HyperLink ID="hlAddDeliveryItem" ImageUrl="~/images/imgButtons/AddItem.gif" ToolTip="New item(s) to deliver" 
                NavigateUrl="~/Pages/NewOrderDetail.aspx" runat="server" />

          </span>
        </div >
      </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsActiveRoastDates" runat="server" 
      OldValuesParameterFormatString="original_{0}" 
      SelectMethod="GetActiveDeliveryDates" 
      TypeName="TrackerDotNet.control.ActiveDeliveryData">
    </asp:ObjectDataSource>   
   <br />
   </asp:Panel>
   <asp:UpdatePanel ID="upnlDeliveryItems" runat="server" UpdateMode="Conditional">
     <ContentTemplate>
        <asp:Table ID="tblDeliveries" runat="server" CssClass="TblZebra" Width="100%" CellPadding="0">  
          <asp:TableHeaderRow TableSection="TableHeader" >
            <asp:TableHeaderCell>By</asp:TableHeaderCell>
            <asp:TableHeaderCell>To</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="thcReceivedBy" Width="90px">Received By</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="thcSignature" Width="100px">Signature</asp:TableHeaderCell>
            <asp:TableHeaderCell>Items</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="thcInStock">In Stock</asp:TableHeaderCell>
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