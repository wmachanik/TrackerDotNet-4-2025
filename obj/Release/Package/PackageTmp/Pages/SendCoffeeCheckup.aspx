<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SendCoffeeCheckup.aspx.cs" 
  Inherits="TrackerDotNet.Pages.SendCoffeeCheckup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="cntSendCoffeeCheckupHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntSendCoffeeCheckupBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Send Coffee Reminder to Customers</h1>
<%--  <asp:ScriptManager ID="smCustomerCheckup" runat="server"></asp:ScriptManager>
--%>
  <asp:UpdateProgress ID="uprgCustomerCheckup" runat="server" AssociatedUpdatePanelID="upnlCustomerCheckup" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <cc1:ToolkitScriptManager runat="Server" />
  <div class="simpleLightBrownForm" style="padding: 10px">
    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" >
      <ContentTemplate>
        <table class="TblZebra" border="none" style="width:98%; padding:1%">
          <tr>
            <td>Subject</td>
            <td><asp:TextBox ID="tbxEmailSubject" Text="Coffee Checkup" runat="server" Width="30em" style="padding-right:2px" /></td>
          </tr>
          <tr>
            <td>Intro
            </td>
            <td style="padding-top: 10px; padding-bottom: 10px">
              <asp:TextBox ID="tbxEmailIntro" runat="server" TextMode="MultiLine" Width="98%" 
                Rows="10" Text="Welcome to Quaffee's coffee checkup or reminder" />
              <cc1:HtmlEditorExtender ID="HtmlEditorExtenderEmailIntro" TargetControlID="tbxEmailIntro" runat="server"
                DisplaySourceTab="true" EnableSanitization="false" />
            </td>
          </tr>
        </table>
      </ContentTemplate>
      <Triggers>
      </Triggers>
    </asp:UpdatePanel>
  </div>
  <h2>Customers to receive the checkup/reminder</h2>
    <asp:UpdatePanel ID="upnlCustomerCheckup" runat="server">
      <ContentTemplate>
        <asp:GridView ID="gvCustomerCheckup" runat="server" CssClass="TblDetailZebra"
          AllowPaging="True" AutoGenerateColumns="False" DataSourceID="odsClientsToReceiveRemidner" AllowSorting="True" >
          <Columns>
            <asp:BoundField DataField="CustomerID" HeaderText="CustomerID" SortExpression="CustomerID" Visible="False" />
            <asp:HyperLinkField DataNavigateUrlFields="CustomerID" 
              DataNavigateUrlFormatString="~/Pages/CustomerDetails.aspx?ID={0}" 
              DataTextField="CompanyName" HeaderText="CompanyName" 
              SortExpression="CompanyName" />
            <asp:BoundField DataField="ContactFirstName" HeaderText="ContactFirstName" SortExpression="ContactFirstName" />
            <asp:BoundField DataField="ContactLastName" HeaderText="ContactLastName"  SortExpression="ContactLastName" Visible="False" />
            <asp:BoundField DataField="EmailAddress" HeaderText="EmailAddress" SortExpression="EmailAddress" />
            <asp:BoundField DataField="ContactAltFirstName" HeaderText="ContactAltFirstName" SortExpression="ContactAltFirstName" Visible="False"  />
            <asp:BoundField DataField="ContactAltLastName" HeaderText="ContactAltLastName" SortExpression="ContactAltLastName" Visible="False"  />
            <asp:BoundField DataField="AltEmailAddress" HeaderText="AltEmailAddress" SortExpression="AltEmailAddress" Visible="False" />
            <asp:BoundField DataField="LastCupCount" HeaderText="Last" SortExpression="LastCupCount" />
            <asp:BoundField DataField="DailyConsumption" HeaderText="DailyConsumption" SortExpression="DailyConsumption" Visible="False" />
            <asp:BoundField DataField="CityID" HeaderText="CityID" SortExpression="CityID" Visible="False" />
            <asp:BoundField DataField="PrimaryPreferenceID" HeaderText="PrimaryPreferenceID" SortExpression="PrimaryPreferenceID" Visible="False" />
            <asp:BoundField DataField="PrimaryPreferenceDesc" HeaderText="Pref" SortExpression="PrimaryPreferenceDesc" />
            <asp:BoundField DataField="PrimaryPreferenceQty" HeaderText="PrefQ" SortExpression="PrimaryPreferenceQty" />
            <asp:BoundField DataField="SecondaryPreferenceID" HeaderText="SecondaryPreferenceID" Visible="False" />
            <asp:BoundField DataField="SecondaryPreferenceQty" HeaderText="SecondaryPreferenceQty" Visible="False"  />
            <asp:BoundField DataField="SecondaryPreferenceDesc" HeaderText="SecondaryPreferenceDesc" Visible="False" />
            <asp:BoundField DataField="PrefPackagingID" HeaderText="PrefPackagingID" Visible="False" />
            <asp:BoundField DataField="PrefPrepTypeID" HeaderText="PrefPrepTypeID" Visible="False"  />
            <asp:BoundField DataField="EquipTypeID" HeaderText="EquipTypeID" Visible="False" />
            <asp:BoundField DataField="PreferedAgent" HeaderText="PreferedAgent" Visible="False" SortExpression="PreferedAgent" />
            <asp:CheckBoxField DataField="TypicallySecToo" HeaderText="TypicallySecToo" Visible="False" />
            <asp:CheckBoxField DataField="enabled" HeaderText="Enabled" 
              SortExpression="enabled" />
            <asp:CheckBoxField DataField="AutomaticFulfill" HeaderText="auto" 
              SortExpression="AutomaticFulfill" />
            <asp:CheckBoxField DataField="PredictionDisabled" 
              HeaderText="PredictionDisabled" SortExpression="PredictionDisabled" Visible="False" />
            <asp:CheckBoxField DataField="AlwaysSendChkUp" HeaderText="AlwaysSendChkUp" 
              SortExpression="AlwaysSendChkUp" Visible="False"  />
            <asp:CheckBoxField DataField="NormallyResponds" HeaderText="NormallyResponds" 
              SortExpression="NormallyResponds" Visible="False" />
            <asp:CheckBoxField DataField="UsesFilter" HeaderText="UsesFilter" 
              SortExpression="UsesFilter" Visible="False"  />
            <asp:BoundField DataField="ReminderCount" HeaderText="ReminderCount" 
              SortExpression="ReminderCount" Visible="False"  />
            <asp:BoundField DataField="NextCoffeeBy" HeaderText="NextCoffeeBy" 
              SortExpression="NextCoffeeBy" DataFormatString="{0:d}" />
            <asp:BoundField DataField="NextCleanOn" HeaderText="NextCleanOn" 
              SortExpression="NextCleanOn" Visible="False"  />
            <asp:BoundField DataField="NextFilterEst" HeaderText="NextFilterEst" 
              SortExpression="NextFilterEst" Visible="False" />
            <asp:BoundField DataField="NextDescaleEst" HeaderText="NextDescaleEst" 
              SortExpression="NextDescaleEst" Visible="False" />
            <asp:BoundField DataField="NextServiceEst" HeaderText="NextServiceEst" 
              SortExpression="NextServiceEst" Visible="False" />
          </Columns>
          <EmptyDataTemplate>
            No data to display...
          </EmptyDataTemplate>
       </asp:GridView>
        <asp:ObjectDataSource ID="odsClientsToReceiveRemidner" runat="server" 
          SelectMethod="GetCustomersToReveiveReminder" SortParameterName="SortBy"
          TypeName="TrackerDotNet.control.ClientsToReceiveReminderQry">
          <SelectParameters>
            <asp:Parameter  DefaultValue="CompanyName" Name="SortBy" Type="String" />
          </SelectParameters>
        </asp:ObjectDataSource>
      </ContentTemplate>
      <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSend" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>
    <br />
    <div class="simpleForm">
      <asp:Button ID="btnSend" Text="Send" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
      <asp:Button ID="btnRefreshCustomerCheckupList" Text="Refresh" runat="server" />
    </div>



<br />
  <asp:GridView ID="gvReoccuringOrders" runat="server" 
    AutoGenerateColumns="False" DataSourceID="sdsRocurringOrders" >
    <Columns>
      <asp:BoundField DataField="DateLastDone" HeaderText="DateLastDone" 
        SortExpression="DateLastDone" />
      <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value" />
      <asp:BoundField DataField="NextDate" HeaderText="NextDate" ReadOnly="True" 
        SortExpression="NextDate" />
      <asp:BoundField DataField="NextDate2" HeaderText="NextDate2" ReadOnly="True" 
        SortExpression="NextDate2" />
      <asp:BoundField DataField="DatePartInterval" HeaderText="DatePartInterval" 
        SortExpression="DatePartInterval" />
    </Columns>
  </asp:GridView>
  <asp:SqlDataSource ID="sdsRocurringOrders" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT ReoccuringOrderTbl.DateLastDone, ReoccuringOrderTbl.[Value], DateAdd(ReoccuranceTypeTbl.DatePartInterval, ReoccuringOrderTbl.[Value], ReoccuringOrderTbl.DateLastDone) AS NextDate, DateAdd('mm', ReoccuringOrderTbl.[Value], ReoccuringOrderTbl.DateLastDone) AS NextDate2, ReoccuranceTypeTbl.DatePartInterval FROM (ReoccuringOrderTbl INNER JOIN ReoccuranceTypeTbl ON ReoccuringOrderTbl.ReoccuranceType = ReoccuranceTypeTbl.ID)">
  </asp:SqlDataSource>

</asp:Content>
