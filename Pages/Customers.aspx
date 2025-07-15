<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="TrackerDotNet.Pages.Customers" %>

<asp:Content ID="cntCustomersHdr" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="cntCustomersBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>List of Customers</h1>
  <asp:ScriptManager ID="smCustomerSummary" runat="server"></asp:ScriptManager>
  <asp:UpdateProgress ID="uprgCustomerSummary" runat="server" AssociatedUpdatePanelID="upnlCustomerSummary" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <div class="simpleLightBrownForm">
    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" >
      <ContentTemplate>
        Filter by: 
        <asp:DropDownList ID="ddlFilterBy" runat="server" ToolTip="select which item to search form">
          <asp:ListItem value="0" Selected="True" Text="none" />
          <asp:ListItem value="CompanyName" Text="Company Name" />
          <asp:ListItem value="ContactFirstName" Text="First Name" />
          <asp:ListItem value="EmailAddress" Text="Email" />
          <asp:ListItem value="PersonsTbl.Abreviation" Text="DeliveryBy" />
          <asp:ListItem value="CityTbl.City" Text="City" />
          <asp:ListItem value="EquipTypeTbl.EquipTypeName" Text="EquipType" />
          <asp:ListItem value="CustomersTbl.MachineSN" Text="Serial No" />
        </asp:DropDownList>
        &nbsp;
        <asp:TextBox ID="tbxFilterBy" runat="server" 
          ToolTip="add '%' to beginning to find contains" 
          ontextchanged="tbxFilterBy_TextChanged"  />
        &nbsp;&nbsp;
        <asp:Button ID="btnGon" Text="Go" runat="server" onclick="btnGon_Click" ToolTip="search for this item" />
        &nbsp;&nbsp;
        <asp:Button ID="btnReset" Text="Reset" runat="server" 
          onclick="btnReset_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlCustomerEnabled" runat="server" AutoPostBack="true">
          <asp:ListItem Value="-1" Text="both" />
          <asp:ListItem Selected="True" Value="1" Text="enabled only" />
          <asp:ListItem Value="0" Text="disabled only" />
        </asp:DropDownList>
        <span style="float: right; padding-top:4px; padding-right: 18px">
          <asp:HyperLink ImageUrl="~/images/imgButtons/AddItem.gif" ToolTip="New Contact" 
            NavigateUrl="~/Pages/CustomerDetails.aspx" runat="server" />
        </span>
      </ContentTemplate>
       <Triggers>
        <asp:AsyncPostBackTrigger controlid="tbxFilterBy" 
                                  EventName="TextChanged" />
         <asp:AsyncPostBackTrigger ControlID="btnGon" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
  </div>
  <br />
  <asp:UpdatePanel ID="upnlCustomerSummary" runat="server" >
    <ContentTemplate>
      <div class="simpleLightBrownForm" style="padding-left: 1em; padding-right: 1em">
        <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" CssClass="TblDetailZebra small"
          AllowSorting="True" DataSourceID="odsCustomerSummarys" AllowPaging="True" CellPadding="0" CellSpacing="0" 
        PageSize="25" >
        <Columns>
          <asp:HyperLinkField DataNavigateUrlFields="CustomerID" DataNavigateUrlFormatString="~/Pages/CustomerDetails.aspx?ID={0}" 
            DataTextField="CompanyName" HeaderText="Company Name" SortExpression="CompanyName" />
          <asp:BoundField DataField="CustomerID" HeaderText="ID" SortExpression="CustomerID" Visible="false" />
          <asp:BoundField DataField="CompanyName" HeaderText="Company Name"  SortExpression="CompanyName" Visible="false" />
          <asp:BoundField DataField="ContactFirstName" HeaderText="First Name" SortExpression="ContactFirstName" />
          <asp:BoundField DataField="ContactLastName" HeaderText="Last Name" SortExpression="ContactLastName" />
          <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ItemStyle-Font-Size="Smaller" />
          <asp:BoundField DataField="PhoneNumber" HeaderText="Phone" SortExpression="PhoneNumber" ItemStyle-Font-Size="Smaller" />
          <asp:BoundField DataField="EmailAddress" HeaderText="Email Address" SortExpression="EmailAddress" />
          <asp:BoundField DataField="DeliveryBy" HeaderText="By" SortExpression="DeliveryBy" />
          <asp:BoundField DataField="EquipTypeName" HeaderText="Equipment" SortExpression="EquipTypeName" />
          <asp:BoundField DataField="MachineSN" HeaderText="S/N" SortExpression="MachineSN" ItemStyle-Font-Size="Smaller" />
          <asp:CheckBoxField DataField="autofulfill" HeaderText="auto" SortExpression="autofulfill" />
          <asp:CheckBoxField DataField="enabled" HeaderText="Enbld" SortExpression="CustomersTbl.enabled" />
          <asp:HyperLinkField DataNavigateUrlFields="CustomerID" HeaderText="Order" ItemStyle-HorizontalAlign="Center" 
              DataNavigateUrlFormatString="~/Pages/NewOrderDetail.aspx?CoID={0}&LastOrder=Y" Text="+last" />
        </Columns>
      
        </asp:GridView>
      </div>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnGon" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="ddlCustomerEnabled" EventName="SelectedIndexChanged" />
    </Triggers>
  </asp:UpdatePanel>

  <asp:ObjectDataSource ID="odsCustomerSummarys" 
    TypeName="TrackerDotNet.Controls.CustomerSummaryDAL"
    SortParameterName="SortBy"
    SelectMethod="GetAllCustomerSummarys"
    runat="server" OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
      <asp:Parameter DefaultValue="CompanyName" Name="SortBy" 
        Type="String" />
      <asp:ControlParameter ControlID="ddlCustomerEnabled" DefaultValue="-1" 
        Name="IsEnabled" PropertyName="SelectedValue" Type="Int32" />
      <asp:SessionParameter DefaultValue="" Name="WhereFilter" SessionField="CustomerSummaryWhereFilter" Type="String" />
    </SelectParameters>
  </asp:ObjectDataSource>

  <br />
  <asp:UpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Label ID="lblFilter" Text="" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>

</asp:Content>