<%@ Page Title="Repair List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Repairs.aspx.cs" Inherits="TrackerDotNet.Pages.Repairs" %>

<asp:Content ID="cntRepairsHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntRepairsBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>List of Repairs</h1>
    <asp:s
  <ajaxToolkit:ScriptManager ID="smRepairsSummary" runat="server"></ajaxToolkit:ScriptManager>
  <asp:UpdateProgress ID="uprgRepairsSummary" runat="server" AssociatedUpdatePanelID="upnlRepairsSummary" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <div class="simpleLightBrownForm">
    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" >
      <ContentTemplate>
        Filter by: 
        <asp:DropDownList ID="ddlFilterBy" runat="server" Font-Size="X-Small" ToolTip="select which item to search form">
          <asp:ListItem Selected="True" value="DateLogged" Text="none" />
          <asp:ListItem value="CompanyID" Text="Company Name" />
          <asp:ListItem value="MachineSerialNumber" Text="Serial Number" />
        </asp:DropDownList>
        &nbsp;
        <asp:TextBox ID="tbxFilterBy" runat="server" ToolTip="add '%' to beginning to find contains" 
          OnTextChanged="tbxFilterBy_TextChanged" Width="14em"  />
        &nbsp;&nbsp;
        <asp:Button ID="btnGo" Text="Go" runat="server" onclick="btnGo_Click" ToolTip="search for this item" />
        &nbsp;&nbsp;
        <asp:Button ID="btnReset" Text="Reset" runat="server" 
          onclick="btnReset_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <span style="float: right; padding-top:4px; padding-right: 18px">
          Repairs of Status:
          <asp:DropDownList ID="ddlRepairStatus" runat="server" AutoPostBack="True"
          AppendDataBoundItems="True" DataSourceID="odsRepairsStatuses" 
          DataTextField="RepairStatusDesc" DataValueField="RepairStatusID" 
          onselectedindexchanged="ddlRepairStatus_SelectedIndexChanged" >
            <asp:ListItem Selected="True" Value="OPEN" Text="-all open repairs-" />
          </asp:DropDownList>
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:HyperLink ID="hlAddRepair" ImageUrl="~/images/imgButtons/AddItem.gif" ToolTip="New Repair" 
            NavigateUrl="~/Pages/RepairDetail.aspx" runat="server" />
        </span>
      </ContentTemplate>
       <Triggers>
        <asp:AsyncPostBackTrigger controlid="tbxFilterBy" EventName="TextChanged" />
         <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
  </div>
  <br />
  <asp:UpdatePanel ID="upnlRepairsSummary" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
      <div class="simpleForm" style="padding-left: 1em; padding-right: 1em">
        <asp:GridView ID="gvRepairs" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvRepairs_RowDataBound"
          DataSourceID="odsRepairs" CssClass="TblWhite" AllowSorting="True"  >
          <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="RepairID" HeaderText="Edit" ItemStyle-HorizontalAlign="Center"
                DataNavigateUrlFormatString="~/Pages/RepairDetail.aspx?RepairID={0}&" 
              Text="edit" >
            <ItemStyle HorizontalAlign="Center" />
            </asp:HyperLinkField>
            <asp:TemplateField HeaderText="Status" >
              <EditItemTemplate>
                <asp:HyperLink ID="StatusUpdateHyperLink" runat="server" Text='<%# GetRepairStatusDesc((int)Eval("RepairStatusID")) %>'
                  NavigateUrl='<%# Eval("RepairID", "~/Pages/RepairStatusChange.aspx?RepairID={0}") %>' />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:HyperLink ID="StatusUpdateHyperLink" runat="server" Text='<%# GetRepairStatusDesc((int)Eval("RepairStatusID")) %>'
                  NavigateUrl='<%# Eval("RepairID", "~/Pages/RepairStatusChange.aspx?RepairID={0}") %>' />
              </ItemTemplate>
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Customer" >
              <EditItemTemplate>
                <asp:TextBox ID="CustomerTextBox" runat="server" Text='<%# Bind("CustomerID") %>'></asp:TextBox>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:HyperLink ID="CustomerHyperLink" runat="server" Text='<%# GetCompanyName((long)Eval("CustomerID")) %>'
                  NavigateUrl='<%# Eval("CustomerID", "~/Pages/CustomerDetails.aspx?ID={0}") %>' />
              </ItemTemplate>
            </asp:TemplateField> 
            <asp:BoundField DataField="DateLogged" HeaderText="Logged" 
              SortExpression="DateLogged" DataFormatString="{0:d}" />
            <asp:BoundField DataField="ContactName" HeaderText="Name" 
              SortExpression="ContactName" />
            <asp:BoundField DataField="JobCardNumber" HeaderText="J/C" 
              SortExpression="JobCardNumber" />
            <asp:TemplateField HeaderText="Machine" >
              <EditItemTemplate>
                <asp:Label ID="EquipLabel" runat="server" Text='<%# GetMachineDesc((int)Eval("MachineTypeID")) %>' />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="EquipLabel" runat="server" Text='<%# GetMachineDesc((int)Eval("MachineTypeID")) %>' />
              </ItemTemplate>
            </asp:TemplateField> 
            <asp:BoundField DataField="MachineSerialNumber" 
              HeaderText="S/N" SortExpression="MachineSerialNumber" />
            <asp:TemplateField HeaderText="Fault" >
              <EditItemTemplate>
                <asp:Label ID="FaultLabel" runat="server" Text='<%# GetRepairFaultDesc((int)Eval("RepairFaultID")) %>' />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="FaultLabel" runat="server" Text='<%# GetRepairFaultDesc((int)Eval("RepairFaultID")) %>' />
              </ItemTemplate>
            </asp:TemplateField> 
            <asp:BoundField DataField="RepairFaultDesc" HeaderText="FaultDesc" 
              SortExpression="RepairFaultDesc" />
            <asp:BoundField DataField="RelatedOrderID" SortExpression="RelatedOrderID" HeaderText="R/OID" />
          </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsRepairs" runat="server" 
          SortParameterName="SortBy" SelectMethod="GetAllRepairsOfStatus"
          TypeName="TrackerDotNet.control.RepairsTbl" 
          OldValuesParameterFormatString="original_{0}" 
          DataObjectTypeName="TrackerDotNet.control.RepairsTbl" 
          DeleteMethod="DeleteRepair" InsertMethod="InsertRepair" 
          UpdateMethod="UpdateRepair" >
          <DeleteParameters>
            <asp:Parameter Name="RepairID" Type="Int32" />
          </DeleteParameters>
          <SelectParameters>
            <asp:Parameter Name="SortBy"  Type="String" DefaultValue="" />
            <asp:ControlParameter ControlID="ddlRepairStatus" DefaultValue="" 
              Name="pRepairStatus" PropertyName="SelectedValue" Type="String" />
          </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="odsRepairsStatuses" runat="server" 
        SelectMethod="GetAll" TypeName="TrackerDotNet.control.RepairStatusesTbl" 
          OldValuesParameterFormatString="original_{0}">
          <SelectParameters>
            <asp:Parameter DefaultValue="RepairStatusID" Name="SortBy" Type="String" />
          </SelectParameters>
        </asp:ObjectDataSource>
      </div>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="ddlRepairStatus" 
        EventName="SelectedIndexChanged" />
    </Triggers>
  </asp:UpdatePanel>
  <asp:Label ID="lblFilter" runat="server" />
</asp:Content>
