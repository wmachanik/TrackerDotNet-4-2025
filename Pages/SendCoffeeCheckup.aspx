<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
   CodeBehind="SendCoffeeCheckup.aspx.cs" Inherits="TrackerDotNet.Pages.SendCoffeeCheckup" %>

<asp:Content ID="cntSendCoffeeCheckupHdr" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript">
</script>
</asp:Content>
<asp:Content ID="cntSendCoffeeCheckupBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Send Coffee Reminder to Customers</h1>
  <asp:ScriptManager ID="smCustomerCheckup" runat="server" />
  <asp:UpdateProgress ID="uprgSendEmail" runat="server" AssociatedUpdatePanelID="upnlSendEmail" ViewStateMode="Enabled" >
    <ProgressTemplate>
      <img src="../images/animi/QuaffeeProgress.gif" alt="sending..." width="16" height="16" />sending....
    </ProgressTemplate>
  </asp:UpdateProgress>
  <asp:UpdateProgress ID="uprgCustomerCheckup" runat="server" AssociatedUpdatePanelID="upnlCustomerCheckup" >
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating..." width="16" height="16" />updating...
    </ProgressTemplate>
  </asp:UpdateProgress>
  <div class="simpleLightBrownForm" style="padding: 10px">
    <asp:UpdatePanel ID="upnlSendEmail" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional" >
      <ContentTemplate>
        Subject: <asp:TextBox ID="tbxEmailSubject" Text="Coffee Checkup" runat="server" Width="30em" style="padding-right:2px" />
        <span class="small floatRight"><asp:Literal ID="ltrlEmailTextID" runat="server" Text="" /></span>
        <br /><br />
        <ajaxToolkit:TabContainer ID="tabEmailBody" runat="server" height="200px">
          <ajaxToolkit:TabPanel ID="tpnlEmailIntro" TabIndex="0" HeaderText="Checkup Intro" runat="server">
            <ContentTemplate>
              <asp:TextBox ID="tbxEmailIntro" runat="server" TextMode="MultiLine" height="100%" Width="99%" Rows="10" 
              Text="Welcome to Quaffee's coffee checkup or reminder" CausesValidation="false" />
              <ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtenderEmailIntro" TargetControlID="tbxEmailIntro" runat="server"
                DisplaySourceTab="true" EnableSanitization="false" />
              <br />
              <span class="small">Enter the text that will appear as the Introduction to the emails.</span><br />
            </ContentTemplate>
          </ajaxToolkit:TabPanel> 
          <ajaxToolkit:TabPanel ID="tpnlEmailBody" TabIndex="1" HeaderText="Checkup Body" runat="server">
            <ContentTemplate>
              <asp:TextBox ID="tbxEmailBody" runat="server" TextMode="MultiLine"  height="100%" Width="99%" Rows="10" 
              Text="" />
              <ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtenderEmailBody" TargetControlID="tbxEmailBody" runat="server"
                DisplaySourceTab="true" EnableSanitization="false" />
              <br />
              <span class="small">This text appears after the Intro, before the Summary of use. Use [#PREPDATE#], to place next PREPDATE,  [#DELIVERYDATE#] for customer deliver date</span>
            </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tpnlEmailFooter" TabIndex="2" HeaderText="Checkup Footer" runat="server">
              <ContentTemplate>
                <asp:TextBox ID="tbxEmailFooter" runat="server" TextMode="MultiLine"  height="100%" Width="99%" Rows="10" Text=""  />
                <ajaxToolkit:HtmlEditorExtender ID="HtmlEmailFooter" TargetControlID="tbxEmailFooter" runat="server"
                  DisplaySourceTab="true" EnableSanitization="false" ClientIDMode="Predictable" />
                <br />
                <span class="small">Enter the footer, which appears under the summary data.</span>
            </ContentTemplate>
          </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
        <asp:Literal ID="ltrlStatus" Text="" runat="server" />
        <div class="simpleLightBrownForm" style="text-align: center" >
          <asp:Button ID="btnPrepData" Text="Prep Data" runat="server" 
            onclick="btnPrepData_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Button ID="btnUpdate" Text="Update Email Text" runat="server" 
            onclick="btnUpdate_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Button ID="btnReload" Text="Email Text Reload" runat="server" 
            onclick="btnReload_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Button ID="btnSend" Text="Send Check Email" runat="server" onclick="btnSend_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Button ID="btnRefreshCustomerCheckupList" Text="Refresh List" runat="server" />
        </div>
      </ContentTemplate>
      <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSend" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>
  </div>
  <h2>Customers to receive the checkup/reminder</h2>
  <div class="simpleLightBrownForm small">
    <table border="0">
      <tr style="text-align: center; font-size: large" >
        <td><b>Customers To Get Reminder</b></td>
        <td><b>Details</b></td>
      </tr>
      <tr>
        <td>
          <asp:UpdatePanel ID="upnlCustomerCheckup" runat="server" Visible="false" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
              <asp:GridView ID="gvCustomerCheckup" runat="server" CssClass="TblWhite" 
                AllowPaging="True" PageSize="25" AutoGenerateColumns="False" DataKeyNames="CustomerID"
                DataSourceID="odsContactsToSendCheckup" AllowSorting="True" >   
                <EmptyDataTemplate>Please click on Prep Data</EmptyDataTemplate>
                <Columns>
                  <asp:CommandField ButtonType="Image" SelectImageUrl="~/images/imgButtons/SelectItem.gif" ShowSelectButton="true" />
                  <asp:BoundField DataField="CustomerID" HeaderText="CustomerID" SortExpression="CustomerID" Visible="False" />
                  <asp:HyperLinkField DataNavigateUrlFields="CustomerID" DataNavigateUrlFormatString="~/Pages/CustomerDetails.aspx?ID={0}" 
                    DataTextField="CompanyName" HeaderText="Company Name" SortExpression="CompanyName" />
                  <asp:BoundField DataField="ContactFirstName" HeaderText="First Name" SortExpression="ContactFirstName" />
                  <asp:BoundField DataField="EmailAddress" HeaderText="Email" SortExpression="EmailAddress" />
                  <asp:BoundField DataField="ContactAltFirstName" HeaderText="Alt First Name" SortExpression="ContactAltFirstName" />
                  <asp:BoundField DataField="AltEmailAddress" HeaderText="Alt Email" SortExpression="AltEmailAddress"  />
                  <asp:TemplateField HeaderText="City" SortExpression="CityID">
                    <EditItemTemplate>
                      <asp:TextBox ID="CityNameTextBox" runat="server" Text='<%# Bind("CityID") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                      <asp:Label ID="CityNameLabel" runat="server" Text='<%# GetCityName((int)Eval("CityID")) %>' />
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:BoundField DataField="NextCoffee" HeaderText="NxtCoffee" SortExpression="NextCoffee" DataFormatString="{0:d}" />
                  <asp:BoundField DataField="NextClean" HeaderText="NxtCln" SortExpression="NextClean"  DataFormatString="{0:d}" />
                  <asp:BoundField DataField="NextDescal" HeaderText="NxtDecal" SortExpression="NextDescal" DataFormatString="{0:d}" />
                  <asp:BoundField DataField="NextDeliveryDate" HeaderText="NxtDlvry" SortExpression="NextDeliveryDate" DataFormatString="{0:d}" />
                  <asp:CheckBoxField DataField="enabled" HeaderText="Enbld" SortExpression="enabled" />
                  <asp:BoundField DataField="ReminderCount" HeaderText="RCnt" SortExpression="ReminderCount" />
                </Columns>

                <EmptyDataTemplate>
                  No data to display...
                </EmptyDataTemplate>
        
              </asp:GridView>
            </ContentTemplate>
            <Triggers>
              <asp:AsyncPostBackTrigger ControlID="btnPrepData" EventName="Click" />
            </Triggers>
          </asp:UpdatePanel>
        </td>
        <td style="vertical-align: text-top; vertical-align: top; padding-left: 8px" >
          <asp:UpdatePanel ID="upnlContactItems" Visible="false" runat="server">
            <ContentTemplate>
              <asp:GridView ID="gvItemsToConfirm" runat="server" CssClass="TblZebra small"
                DataSourceID="odsContactToBeRemindedItems" AutoGenerateColumns="false" >
                <Columns>
                  <asp:BoundField DataField="TCIID" HeaderText="TCIID" SortExpression="TCIID" Visible="false" />
                  <asp:TemplateField HeaderText="Item">
                    <EditItemTemplate>
                      <asp:TextBox ID="ItemDescTextBox" runat="server" Text='<%# Bind("ItemID") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                      <asp:Label ID="ItemDescLabel" runat="server" Text='<%# GetItemDesc((int)Eval("ItemID")) %>' />
                    </ItemTemplate>
                  </asp:TemplateField> 
                  <asp:BoundField DataField="CustomerID" HeaderText="CustomerID" Visible="false" SortExpression="CustomerID" />
                  <asp:BoundField DataField="ItemQty" HeaderText="Qty" SortExpression="ItemQty" />
                  <asp:BoundField DataField="ItemPrepID" HeaderText="PrepID"  SortExpression="ItemPrepID" Visible="false" />
                  <asp:BoundField DataField="ItemPackagID" HeaderText="PackagID" Visible="false" SortExpression="ItemPackagID" />
                  <asp:CheckBoxField DataField="AutoFulfill" HeaderText="AFF" SortExpression="AutoFulfill" />
                  <asp:CheckBoxField DataField="ReoccurOrder" HeaderText="RO" SortExpression="ReoccurOrder" />
                </Columns>
                <EmptyDataTemplate>
                  Select customer to see items...
                </EmptyDataTemplate>
              </asp:GridView>
            </ContentTemplate>
            <Triggers>
            </Triggers>
          </asp:UpdatePanel>
        </td>
      </tr>
    </table>
    <asp:ObjectDataSource ID="odsContactsToSendCheckup" runat="server" 
      DataObjectTypeName="TrackerDotNet.control.ContactToRemindDetails" 
      InsertMethod="InsertContacts" SelectMethod="GetAllContacts" 
      SortParameterName="SortBy"
      TypeName="TrackerDotNet.control.TempCoffeeCheckup" 
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="CompanyName" Name="SortBy" Type="String" />
      </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsContactToBeRemindedItems" runat="server" 
      SelectMethod="GetAllContactItems" SortParameterName="SortBy" 
      TypeName="TrackerDotNet.control.TempCoffeeCheckup" 
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:ControlParameter ControlID="gvCustomerCheckup" Name="CustomerID" 
          PropertyName="SelectedValue" Type="Int64" />
        <asp:Parameter Name="SortBy" Type="String" />
      </SelectParameters>
    </asp:ObjectDataSource>
    <br />
  </div>
</asp:Content>
