<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SystemData.aspx.cs" Inherits="TrackerDotNet.Tools.SystemData" %>
<asp:Content ID="cntSystemDataHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntSystemDataBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>System Data  </h1>
  <asp:DetailsView ID="dvSystemData" runat="server" 
      AutoGenerateRows="False" DataSourceID="odsSystemData"
      CssClass="TblWhite" OnItemCommand="dvSystemData_ItemCommand" >
    <Fields>
           <asp:TemplateField HeaderText="Do Reoccuring Orders" 
        SortExpression="DoReoccuringOrders">
        <EditItemTemplate>
          <asp:CheckBox ID="DoReoccuringOrdersCheckBox" runat="server" 
            Checked='<%# Bind("DoReoccuringOrders") %>' />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:CheckBox ID="DoReoccuringOrdersCheckBox" runat="server" 
            Checked='<%# Bind("DoReoccuringOrders") %>' Enabled="false" />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Last Reoccurring Date" 
        SortExpression="LastReoccurringDate">
        <EditItemTemplate>
          <asp:TextBox ID="LastReoccurringDateTextBox" runat="server" 
            Text='<%# Bind("LastReoccurringDate", "{0:d}") %>' />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="LastReoccurringDateLabel" runat="server" 
            Text='<%# Bind("LastReoccurringDate", "{0:d}") %>' />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Date Last Prep Date Calculated" 
        SortExpression="DateLastPrepDateCalcd">
        <EditItemTemplate>
          <asp:TextBox ID="DateLastPrepDateCalcdTextBox" runat="server" 
            Text='<%# Bind("DateLastPrepDateCalcd", "{0:d}") %>' 
            ToolTip="set the minimum date that a reminder must be sent out, useful for when closed for extended periods" />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="DateLastPrepDateCalcdLabel" runat="server" 
            Text='<%# Bind("DateLastPrepDateCalcd", "{0:d}") %>' />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Min Reminder Date" SortExpression="MinReminderDate">
        <EditItemTemplate>
          <asp:TextBox ID="MinReminderDateTextBox" runat="server" Text='<%# Bind("MinReminderDate", "{0:d}") %>' />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="MinReminderDateLabel" runat="server" 
            Text='<%# Bind("MinReminderDate", "{0:d}") %>' />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Group Item Type #" SortExpression="GroupItemTypeID">
        <EditItemTemplate>
          <asp:TextBox ID="GroupItemTypeIDTextBox" runat="server" Text='<%# Bind("GroupItemTypeID") %>' />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="GroupItemTypeIDLabel" runat="server" Text='<%# Bind("GroupItemTypeID") %>' />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
        <EditItemTemplate>
          <asp:Label ID="IDLabel" runat="server" Text='<%# Eval("ID") %>' Enabled="false" />
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="IDLabel" runat="server" Text='<%# Bind("ID") %>' />
        </ItemTemplate>
      </asp:TemplateField>
           <asp:CommandField ShowEditButton="True" />
    </Fields>
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsSystemData" runat="server" 
      DataObjectTypeName="TrackerDotNet.control.SysDataTbl" SelectMethod="GetAll" 
      TypeName="TrackerDotNet.control.SysDataTbl" UpdateMethod="Update" OldValuesParameterFormatString="original_{0}">
    </asp:ObjectDataSource>
  <p>&nbsp;</p>

</asp:Content>
