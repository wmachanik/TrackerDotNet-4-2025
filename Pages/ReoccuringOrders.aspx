<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReoccuringOrders.aspx.cs"
 Inherits="TrackerDotNet.Pages.ReoccuringOrders" %>

<asp:Content ID="cntReoccuringOrdersHdr" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="cntReoccuringOrdersBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>List of ReoccuringOrders</h1>
  <asp:ScriptManager ID="smReoccuringOrderSummary" runat="server"></asp:ScriptManager>
  <asp:UpdateProgress ID="uprgReoccuringOrderSummary" runat="server" AssociatedUpdatePanelID="upnlReoccuringOrderSummary" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <div class="simpleLightBrownForm">
    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" >
      <ContentTemplate>
        Filter by: 
        <asp:DropDownList ID="ddlFilterBy" runat="server" ToolTip="select which item to search form">
          <asp:ListItem value="0" Text="none" />
          <asp:ListItem Selected="True" value="CompanyName" Text="Company Name" />
        </asp:DropDownList>
        &nbsp;
        <asp:TextBox ID="tbxFilterBy" runat="server" ToolTip="add '%' to beginning to find contains" 
          OnTextChanged="tbxFilterBy_TextChanged"  />
        &nbsp;&nbsp;
        <asp:Button ID="btnGon" Text="Go" runat="server" onclick="btnGon_Click" ToolTip="search for this item" />
        &nbsp;&nbsp;
        <asp:Button ID="btnReset" Text="Reset" runat="server" 
          onclick="btnReset_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlReoccuringOrderEnabled" runat="server" AutoPostBack="true">
          <asp:ListItem Selected="True" Value="1" Text="enabled only" />
          <asp:ListItem Value="0" Text="disabled only" />
          <asp:ListItem Value="-1" Text="both" />
        </asp:DropDownList>
        <span style="float: right; padding-top:4px; padding-right: 18px">
          <asp:HyperLink ImageUrl="~/images/imgButtons/AddItem.gif" ToolTip="New Contact" 
            NavigateUrl="~/Pages/ReoccuringOrderDetails.aspx" runat="server" />
        </span>
      </ContentTemplate>
       <Triggers>
        <asp:AsyncPostBackTrigger controlid="tbxFilterBy" EventName="TextChanged" />
         <asp:AsyncPostBackTrigger ControlID="btnGon" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
  </div>
  <br />
  <asp:UpdatePanel ID="upnlReoccuringOrderSummary" runat="server" >
    <ContentTemplate>
      <div class="simpleLightBrownForm" style="padding-left: 1em; padding-right: 1em">
    <asp:GridView ID="gvReoccuringOrders" runat="server" AutoGenerateColumns="False" CssClass="TblDetailZebra"
      AllowSorting="True" DataSourceID="odsReoccuringOrderSummarys" AllowPaging="True" PageSize="25" >
    <RowStyle Font-Size="Large" />
    <Columns>
      <asp:HyperLinkField DataNavigateUrlFields="ReoccuringOrderID" HeaderText="Edit" ItemStyle-HorizontalAlign="Center"
          DataNavigateUrlFormatString="~/Pages/ReoccuringOrderDetails.aspx?ID={0}&" Text="edit" />
      <asp:BoundField DataField="ID" HeaderText="ReoccuringOrderID" SortExpression="ID" Visible="false" />
      <asp:HyperLinkField DataNavigateUrlFields="CustomerID" DataNavigateUrlFormatString="~/Pages/CustomerDetails.aspx?ID={0}" 
                    DataTextField="CompanyName" HeaderText="Company Name" SortExpression="CompanyName" />
        <asp:BoundField DataField="ReoccuranceValue" HeaderText="Value" SortExpression="ReoccuranceValue" ItemStyle-HorizontalAlign="Center" />
      <asp:BoundField DataField="ReoccuranceTypeDesc" HeaderText="Type Desc"  SortExpression="ReoccuranceTypeDesc" />
      <asp:BoundField DataField="ItemTypeDesc" HeaderText="Item Type" SortExpression="ItemTypeDesc" ItemStyle-Font-Size="Smaller" />
      <asp:BoundField DataField="QtyRequired" HeaderText="Qty" SortExpression="QtyRequired" ItemStyle-HorizontalAlign="Center" />
      <asp:BoundField DataField="DateLastDone" HeaderText="Last Done" SortExpression="LastDone" DataFormatString="{0:d}" />
      <asp:BoundField DataField="NextDateRequired" HeaderText="Next Date" SortExpression="NextDateRequired" DataFormatString="{0:d}" />
      <asp:BoundField DataField="RequireUntilDate" HeaderText="Until Date" SortExpression="RequireUntilDate" DataFormatString="{0:d}"  />
      <asp:CheckBoxField DataField="enabled" HeaderText="Enbld" SortExpression="ReoccuringOrdersTbl.enabled" />
    </Columns>
      
  </asp:GridView>
  </div>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnGon" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="ddlReoccuringOrderEnabled" EventName="SelectedIndexChanged" />
    </Triggers>
  </asp:UpdatePanel>

  <asp:ObjectDataSource ID="odsReoccuringOrderSummarys" 
    TypeName="TrackerDotNet.control.ReoccuringOrderDAL"
    SortParameterName="SortBy"
    SelectMethod="GetAll"
    runat="server" OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
      <asp:Parameter DefaultValue="CompanyName" Name="SortBy" 
        Type="String" />
      <asp:ControlParameter ControlID="ddlReoccuringOrderEnabled" DefaultValue="-1" 
        Name="IsEnabled" PropertyName="SelectedValue" Type="Int32" />
      <asp:SessionParameter DefaultValue="" Name="WhereFilter" 
        SessionField="ReoccuringOrderSummaryWhereFilter" Type="String" />
    </SelectParameters>
  </asp:ObjectDataSource>

  <br />
  <asp:UpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Label ID="lblFilter" Text="" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>

</asp:Content>