<%@ Page Title="Customer Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
  CodeBehind="CustomerDetails.aspx.cs" Inherits="TrackerDotNet.Pages.CustomerDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="cntCustomerDetailsHdr" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">
  function redirect(url) {
    alert("Customer Added");
    window.location = url;
  }
  function showMessage(thisMessage) {
    alert(thisMessage);
  }
</script>
</asp:Content>
<asp:Content ID="cntCustomerDetailsBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h2 class="InputFrm">
    Customer Details</h2>
  <asp:Label ID="lblCustomerID" Visible="false" runat="server" />
  <asp:ScriptManager ID="smCustomerDetails" runat="server">
  </asp:ScriptManager>
  <asp:UpdateProgress ID="uprgCustomerDetails" runat="server" AssociatedUpdatePanelID="upnlCustomerDetails">
    <ProgressTemplate>
      <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....</ProgressTemplate>
  </asp:UpdateProgress>
  <asp:UpdatePanel ID="upnlCustomerDetails" runat="server">
    <ContentTemplate>
      <table class="TblMudZebra" cellpadding="0" cellspacing="0" >
        <tr>
          <td>Company Name</td>
          <td colspan="4"><asp:TextBox ID="CompanyNameTextBox" runat="server" Text='<%# Bind("CompanyName") %>' Width="33em" /></td>
          <td><asp:Label ID="CompanyIDLabel" runat="server" Text='<%# Eval("CustomerID") %>' /></td>
        </tr>
        <tr>
          <td>First Name</td>
          <td><asp:TextBox ID="ContactFirstNameTextBox" runat="server" Text='<%# Bind("ContactFirstName") %>' /></td>
          <td>Last Name</td>
          <td><asp:TextBox ID="ContactLastNameTextBox" runat="server" Text='<%# Bind("ContactLastName") %>' /></td>
          <td>Title</td>
          <td><asp:TextBox ID="ContactTitleTextBox" runat="server" Text='<%# Bind("ContactTitle") %>' Width="3em" /></td>
        </tr>
        <tr>
          <td>Alt First Name</td>
          <td><asp:TextBox ID="ContactAltFirstNameTextBox" runat="server" Text='<%# Bind("ContactAltFirstName") %>' /></td>
          <td>Alt Last Name</td>
          <td><asp:TextBox ID="ContactAltLastNameTextBox" runat="server" Text='<%# Bind("ContactAltLastName") %>' /></td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
        </tr>
        <tr>
          <td>Address</td>
          <td colspan="3">
            <asp:TextBox ID="BillingAddressTextBox" runat="server" Text='<%# Bind("BillingAddress") %>' Width="33em" /></td>
          <td>Department</td>
          <td><asp:TextBox ID="DepartmentTextBox" runat="server" Text='<%# Bind("Department") %>' /></td>
        </tr>
        <tr>
          <td>Post Code</td>
          <td><asp:TextBox ID="PostalCodeTextBox" runat="server" Text='<%# Bind("PostalCode") %>' /></td>
          <td>Delivery Area</td>
          <td>
            <asp:DropDownList ID="ddlCities" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsCities" DataTextField="City" DataValueField="ID"  >
              <asp:ListItem Value="0" Text="none" />
            </asp:DropDownList>
            </td>
          <td>Prov</td>
          <td><asp:TextBox ID="ProvinceTextBox" runat="server" Text='<%# Bind("Province") %>' /></td>
        </tr>
        <tr>
          <td>Phone</td>
          <td><asp:TextBox ID="PhoneNumberTextBox" runat="server" Text='<%# Bind("PhoneNumber") %>' /></td>
          <td>Cell</td>
          <td><asp:TextBox ID="CellNumberTextBox" runat="server" Text='<%# Bind("CellNumber") %>' /></td>
          <td>Fax</td>
          <td><asp:TextBox ID="FaxNumberTextBox" runat="server" Text='<%# Bind("FaxNumber") %>' /></td>
        </tr>
        <tr>
          <td>Email</td>
          <td colspan="2"><asp:TextBox ID="EmailAddressTextBox" runat="server" Text='<%# Bind("EmailAddress") %>'
              Width="25em" /></td>
          <td>Alt Email Addr</td>
          <td colspan="2"><asp:TextBox ID="AltEmailAddressTextBox" runat="server" Text='<%# Bind("AltEmailAddress") %>'
              Width="25em" /></td>
        </tr>
        <tr>
          <td>Customer Type</td>
          <td>
            <asp:DropDownList ID="ddlCustomerTypes" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsCustomerTypes" DataTextField="CustTypeDesc" 
              DataValueField="CustTypeID"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
          <td>Equip Type</td>
          <td>
            <asp:DropDownList ID="ddlEquipTypes" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsEquipTypes" DataTextField="EquipTypeName" DataValueField="EquipTypeId"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
          <td>S/N</td>
          <td><asp:TextBox ID="MachineSNTextBox" runat="server" Text='<%# Bind("MachineSN") %>' /></td>
        </tr>
        <tr>
          <td>1st Preference</td>
          <td>
            <asp:DropDownList ID="ddlFirstPreference" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsItems" DataTextField="ItemDesc" DataValueField="ItemTypeID"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
          <td>Pri Pref Qty</td>
          <td><asp:TextBox ID="PriPrefQtyTextBox" runat="server" Text='<%# Bind("PriPrefQty") %>' /></td>
          <td>Packaging</td>
          <td>
            <asp:DropDownList ID="ddlPackagingTypes" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsPackagingTypes" DataTextField="Description" DataValueField="PackagingID"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>Delivery By</td>
          <td>
            <asp:DropDownList ID="ddlDeliveryBy" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsPersons" DataTextField="Abreviation" DataValueField="PersonID"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
          <td>Agent</td>
          <td>
            <asp:DropDownList ID="ddlAgent" runat="server" AppendDataBoundItems="True" 
              DataSourceID="odsPersons" DataTextField="Abreviation" DataValueField="PersonID"  >
              <asp:ListItem Text="none" Value="0" />
            </asp:DropDownList>
          </td>
          <td colspan="2">ReminderCount: 
            <asp:Label ID="ReminderCountLabel" runat="server" Text='<%# Eval("ReminderCount") %>' /></td>
        </tr>
        <tr>
          <td>Uses/Enabled/Filters</td>
          <td colspan="5"  >&nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="enabledCheckBox" runat="server" Text="enabled" TextAlign="Right" Checked='<%# Bind("enabled") %>' />
            &nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="autofulfillCheckBox" runat="server" Text="auto fulfill" TextAlign="Right"
              Checked='<%# Bind("autofulfill") %>' />
            &nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="UsesFilterCheckBox" runat="server" Text="Uses Filter" TextAlign="Right"
              Checked='<%# Bind("UsesFilter") %>' />
            &nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="PredictionDisabledCheckBox" runat="server" Text="PredictionDisabled"
                TextAlign="Right" Checked='<%# Bind("PredictionDisabled") %>' />
            &nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="AlwaysSendChkUpCheckBox" runat="server" Text="AlwaysSendChkUp"
              TextAlign="Right" Checked='<%# Bind("AlwaysSendChkUp") %>' />
            &nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="NormallyRespondsCheckBox" runat="server" Text="NormallyResponds"
              TextAlign="Right" Checked='<%# Bind("NormallyResponds") %>' />
          </td>
        </tr>
        <tr>
          <td>Notes</td>
          <td colspan="5">
            <asp:TextBox ID="NotesTextBox" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine"
              Height="4em" Width="93%" /></td>
        </tr>
        <tr>
          <td colspan="6" class="rowOddC">
            <asp:Button ID="btnUpdate" Text="Update" runat="server" onclick="btnUpdate_Click" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnUpdateAndReturn" Text="Update & Return" runat="server" onclick="btnUpdateAndReturn_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnInsert" Text="Insert" runat="server" Enabled="false" onclick="btnInsert_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnAddLasOrder" Text="Add Last" runat="server" ToolTip="Add the clients last order" onclick="btnAddLasOrder_Click"/>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnForceNext" Text="Force Next" runat="server" ToolTip="force client to skip a week" onclick="btnForceNext_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnRecalcAverage" Text="Recalc Ave" runat="server" 
              ToolTip="force a recaluation of the clients consumption average" 
              onclick="btnRecalcAverage_Click"/>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" Text="Cancel" runat="server" onclick="btnCancel_Click" />
          </td>
      </table>
<!-- ModalPopupExtender -->
    </ContentTemplate>
  </asp:UpdatePanel>
  <asp:Literal ID="ltrlStatus" Text="" runat="server" />
  <br />
  <cc1:TabContainer ID="tabcCustomer" runat="server" ActiveTabIndex="0" >
    <cc1:TabPanel runat="server" HeaderText="Next Required" ID="tabpnlNextRequired">
      <HeaderTemplate>Next Items Required</HeaderTemplate>
      <ContentTemplate>
        <asp:UpdatePanel ID="upnlNextItems" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
          <ContentTemplate>
            <div style="padding:4px">
              <asp:DataGrid ID="dgCustomerUsage" runat="server" CssClass="TblWhite small"  DataSourceID="dsCustomerUsage" AutoGenerateColumns="false" >
                <Columns>
                  <asp:BoundColumn DataField="CustomerID" Visible="false" /> 
                  <asp:BoundColumn DataField="LastCupCount" HeaderText="Last Count" ItemStyle-HorizontalAlign="Right" />
                  <asp:BoundColumn DataField="NextCoffeeBy" HeaderText="Next Coffee By"  DataFormatString="{0:d}" />
                  <asp:BoundColumn DataField="DailyConsumption" HeaderText="Ave" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                  <asp:BoundColumn DataField="NextCleanOn" HeaderText="Next Clean Est" DataFormatString="{0:d}" />
                  <asp:BoundColumn DataField="CleanAveCount" HeaderText="Clean Ave" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}"/>
                  <asp:BoundColumn DataField="NextFilterEst" HeaderText="Next Filter" DataFormatString="{0:d}" />
                  <asp:BoundColumn DataField="FilterAveCount" HeaderText="Filter Ave" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}"/>
                  <asp:BoundColumn DataField="NextDescaleEst" HeaderText="Next Descale" DataFormatString="{0:d}" />
                  <asp:BoundColumn DataField="DescaleAveCount" HeaderText="Descale Ave" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}"/>
                  <asp:BoundColumn DataField="NextServiceEst" HeaderText="Next Service" DataFormatString="{0:d}" />
                  <asp:BoundColumn DataField="ServiceAveCount" HeaderText="Service Ave" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}"/>
                </Columns>
              </asp:DataGrid>
            </div>
          </ContentTemplate>
          <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnForceNext"  EventName="Click" />
          </Triggers>

        </asp:UpdatePanel>
      </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel runat="server" HeaderText="Items" ID="tabpnlItems">
      <HeaderTemplate>Customer Usage</HeaderTemplate>
      <ContentTemplate>
        <asp:UpdatePanel ID="upnlItems" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
          <ContentTemplate>
            <div class="simpleForm" style="padding: 4px">
              <div class="simpleLightGreenForm">
                <asp:GridView ID="gvItems" runat="server" AllowSorting="True" CssClass="TblWhite"
                  AutoGenerateColumns="False" DataSourceID="odsItemUsage" AllowPaging="True">
                  <Columns>
                    <asp:TemplateField HeaderText="Date" SortExpression="Date">
                      <EditItemTemplate>
                        <asp:TextBox ID="tbxDate" runat="server" Text='<%# Bind("Date", "{0:d}") %>'  />
                      </EditItemTemplate>
                      <ItemTemplate>
                        <asp:Label ID="lblData" runat="server" Text='<%# Bind("Date", "{0:d}") %>' />
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item" SortExpression="ItemProvided">
                      <EditItemTemplate>
                        <asp:DropDownList ID="ddlItems" runat="server" DataSourceID="odsItems" 
                          DataTextField="ItemDesc" DataValueField="ItemTypeID" AppendDataBoundItems="true"
                          SelectedValue='<%# Bind("ItemProvided") %>'>
                          <asp:ListItem Value="0" Text="n/a" />
                        </asp:DropDownList>
                      </EditItemTemplate>
                      <ItemTemplate>
                        <asp:DropDownList ID="ddlItems" runat="server" DataSourceID="odsItems" 
                          DataTextField="ItemDesc" DataValueField="ItemTypeID" AppendDataBoundItems="true"
                          SelectedValue='<%# Bind("ItemProvided") %>' Enabled="false">
                          <asp:ListItem Value="0" Text="n/a" />
                        </asp:DropDownList>
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AmountProvided" HeaderText="Qty"
                      SortExpression="AmountProvided" />
                    <asp:TemplateField HeaderText="Packaging" SortExpression="PackagingID">
                      <EditItemTemplate>
                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("PackagingID") %>'></asp:TextBox>
                      </EditItemTemplate>
                      <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("PackagingID") %>'></asp:Label>
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Prep Type" SortExpression="PrepTypeID">
                      <EditItemTemplate>
                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PrepTypeID") %>'></asp:TextBox>
                      </EditItemTemplate>
                      <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("PrepTypeID") %>'></asp:Label>
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Notes" HeaderText="Notes" 
                      SortExpression="Notes" />
                  </Columns>
                </asp:GridView>
              </div>
            </div>
          </ContentTemplate>
        </asp:UpdatePanel>
      </ContentTemplate>
    </cc1:TabPanel>

  </cc1:TabContainer>

  <asp:ObjectDataSource ID="odsCities" runat="server" TypeName="TrackerDotNet.App_Code.CityTblDAL"
      SortParameterName="SortBy" SelectMethod="GetAllCityTblData"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="City" Name="SortBy" Type="String" />
      </SelectParameters>
    </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsItems" runat="server" TypeName="TrackerDotNet.control.ItemTypeTbl"
      SortParameterName="SortBy" SelectMethod="GetAll"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="ItemDesc" Name="SortBy" Type="String" />
      </SelectParameters>
  </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsEquipTypes" runat="server" TypeName="TrackerDotNet.control.EquipTypeTbl"
      SortParameterName="SortBy" SelectMethod="GetAll"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="EquipTypeName" Name="SortBy" Type="String" />
      </SelectParameters>
  </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsCustomerTypes" runat="server" TypeName="TrackerDotNet.control.CustomerTypeTbl"
      SortParameterName="SortBy" SelectMethod="GetAll"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="CustTypeDesc" Name="SortBy" Type="String" />
      </SelectParameters>
   </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsPersons" runat="server" TypeName="TrackerDotNet.control.PersonsTbl"
      SortParameterName="SortBy" SelectMethod="GetAll"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="Abreviation" Name="SortBy" Type="String" />
      </SelectParameters>
   </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsPackagingTypes" runat="server" TypeName="TrackerDotNet.control.PackagingTbl"
      SortParameterName="SortBy" SelectMethod="GetAll"
      OldValuesParameterFormatString="original_{0}">
      <SelectParameters>
        <asp:Parameter DefaultValue="Description" Name="SortBy" Type="String"  />
      </SelectParameters>
  </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="odsItemUsage" runat="server" TypeName="TrackerDotNet.control.ItemUsageTbl"
      SortParameterName="SortBy" 
      SelectMethod="GetAllItemsUsed" 
      OldValuesParameterFormatString="original_{0}"  DataObjectTypeName="TrackerDotNet.control.ItemTypeTbl" >
      <SelectParameters>
        <asp:ControlParameter ControlID="CompanyIDLabel" DefaultValue="0" 
          Name="pCustomerID" PropertyName="Text" Type="Int64" />
        <asp:Parameter DefaultValue="&quot;Date&quot;" Name="SortBy" Type="String"  />
      </SelectParameters>
  </asp:ObjectDataSource>
  <asp:ObjectDataSource ID="dsCustomerUsage" runat="server" TypeName="TrackerDotNet.control.ClientUsageTbl"
      SelectMethod="GetUsageData" 
      OldValuesParameterFormatString="original_{0}" >
      <SelectParameters>
        <asp:ControlParameter ControlID="CompanyIDLabel" DefaultValue="0" 
          Name="pCustomerID" PropertyName="Text" Type="Int64" />
      </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
