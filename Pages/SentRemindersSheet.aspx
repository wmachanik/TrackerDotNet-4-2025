<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SentRemindersSheet.aspx.cs" Inherits="TrackerDotNet.Pages.SentRemindersSheet" %>
<asp:Content ID="cntSentRemindersSheetHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntSentRemindersSheetBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>List of Reminders Sent </h1>
  <ajaxToolkit:ToolkitScriptManager ID="smSentRemindersSummary" runat="server"></ajaxToolkit:ToolkitScriptManager>
  <asp:UpdateProgress ID="uprgSentRemindersSummary" runat="server" AssociatedUpdatePanelID="upnlSentRemindersList" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <div class="simpleLightBrownForm">
    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
      <ContentTemplate>
        Filter by: 
        <asp:DropDownList ID="ddlFilterByDate" runat="server" 
          ToolTip="select which item to search form" AutoPostBack="True"  DataTextFormatString="{0:d}"
          DataSourceID="odsDatesSentReminder" DataTextField="Date" DataValueField="Date">
        </asp:DropDownList>
      </ContentTemplate>
    </asp:UpdatePanel>
  </div>
  <br />
  <asp:UpdatePanel ID="upnlSentRemindersList" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
      <div class="simpleLightBrownForm" style="padding-left: 1em; padding-right: 1em">
    <asp:GridView ID="gvSentReminders" runat="server" AutoGenerateColumns="False" CssClass="TblDetailZebra"
      AllowSorting="True" DataSourceID="odsSentRemindersSummarys" AllowPaging="True" PageSize="20" >
      <Columns>
        <asp:BoundField DataField="ReminderID" HeaderText="ReminderID" 
          SortExpression="ReminderID" Visible="false" />
        <asp:TemplateField HeaderText="Customer" >
          <EditItemTemplate>
            <asp:TextBox ID="CustomerTextBox" runat="server" Text='<%# Bind("CustomerID") %>'></asp:TextBox>
          </EditItemTemplate>
          <ItemTemplate>
            <asp:HyperLink ID="CustomerHyperLink" runat="server" Text='<%# GetCompanyName((long)Eval("CustomerID")) %>'
              NavigateUrl='<%# Eval("CustomerID", "~/Pages/CustomerDetails.aspx?ID={0}") %>' />
          </ItemTemplate>
        </asp:TemplateField> 
        <asp:BoundField DataField="DateSentReminder" HeaderText="Date Reminder Sent" 
          SortExpression="DateSentReminder" DataFormatString="{0:d}" 
          ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left" />
        <asp:BoundField DataField="NextPrepDate" HeaderText="Prep Date" 
          SortExpression="NextPrepDate" DataFormatString="{0:d}" 
          ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left" />
        <asp:CheckBoxField DataField="ReminderSent" HeaderText="Reminder Sent" 
          ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left" />
        <asp:CheckBoxField DataField="HadAutoFulfilItem" 
          HeaderText="Had Auto FulfilItems" SortExpression="HadAutoFulfilItem" 
          ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Left" />
        <asp:CheckBoxField DataField="HadReoccurItems" HeaderText="Had Reoccur Items" 
          SortExpression="HadReoccurItems" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
      </Columns>
    <RowStyle Font-Size="Large" />
  </asp:GridView>
  </div>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="ddlFilterByDate" 
        EventName="SelectedIndexChanged" />
    </Triggers>
  </asp:UpdatePanel>

  <asp:ObjectDataSource ID="odsSentRemindersSummarys" 
    TypeName="TrackerDotNet.control.SentRemindersLogTbl"
    SortParameterName="SortBy"
    SelectMethod="GetAllByDate"
    runat="server" OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
      <asp:ControlParameter ControlID="ddlFilterByDate" 
        Name="pDateSent" PropertyName="SelectedValue" Type="DateTime" />
      <asp:Parameter Name="SortBy" Type="String" />
    </SelectParameters>
  </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsDatesSentReminder" runat="server" 
    SelectMethod="GetLast20DatesReminderSent" 
    TypeName="TrackerDotNet.control.SentRemindersLogTbl"></asp:ObjectDataSource>

  <br />
  <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Label ID="lblFilter" Text="" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>

<%--            <asp:Label ID="CustomerLabel" runat="server" Text='<%# GetCustomerName((long)Eval("CustomerID")) %>' />
--%>
</asp:Content>
