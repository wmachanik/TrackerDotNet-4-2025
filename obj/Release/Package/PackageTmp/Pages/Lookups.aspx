<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lookups.aspx.cs"
   Inherits="TrackerDotNet.Pages.Lookups" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="cntLookupHdr" ContentPlaceHolderID="HeadContent" runat="server">
  </asp:Content>
<asp:Content ID="cntLookupBdy" ContentPlaceHolderID="MainContent" runat="server">

  <asp:ScriptManager ID="scmLookup" runat="server">
  </asp:ScriptManager>
  <asp:UpdateProgress ID="uprgLookup" runat="server">
    <ProgressTemplate>
      Please Wait&nbsp;<img src="../images/animi/QuaffeeProgress.gif" alt="Please Wait..." width="128" height="15" />&nbsp;...
    </ProgressTemplate>
  </asp:UpdateProgress>
  <p>Lookup Tables...</p>
  <asp:Label ID="lblStatus" runat="server" />
    <cc1:TabContainer ID="tabcLookup" runat="server" ActiveTabIndex="2" >
    <cc1:TabPanel runat="server" HeaderText="Items" ID="tabpnlItems">
      <HeaderTemplate>Items</HeaderTemplate>
      <ContentTemplate>
        <asp:UpdatePanel ID="upnlItems" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
             <asp:GridView ID="gvItems" runat="server" DataSourceID="sdsItems" 
               AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
               OnRowCommand="gvItems_RowCommand" ShowFooter="True"
               CellPadding="0" DataKeyNames="ItemTypeID" ForeColor="#333333" PageSize="20" 
               style="font-family: Arial; font-size: small">
               <AlternatingRowStyle BackColor="White" />
               <Columns>
                 <asp:BoundField DataField="ItemTypeID" HeaderText="ItemTypeID" 
                   InsertVisible="True" ReadOnly="True" SortExpression="ItemTypeID" 
                   Visible="False" />
                 <asp:TemplateField HeaderText="Item" SortExpression="ItemDesc">
                   <EditItemTemplate>
                     <asp:TextBox ID="tbxEItem" runat="server" Text='<%# Bind("ItemDesc") %>'></asp:TextBox>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:TextBox ID="tbxItem" runat="server" Text="" />
                     <asp:RequiredFieldValidator ErrorMessage="A proper value is needed" ControlToValidate="tbxItem"
                       Font-Italic="true" ForeColor="Red" runat="server"  />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:Label ID="lblItem" runat="server" Text='<%# Bind("ItemDesc") %>'></asp:Label>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Enabled" SortExpression="ItemEnabled">
                   <EditItemTemplate>
                     <asp:CheckBox ID="cbxItemEnabled" runat="server" Checked='<%# Bind("ItemEnabled") %>' />
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:CheckBox ID="cbxItemEnabled" runat="server" Checked="true" />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:CheckBox ID="cbxItemEnabled" runat="server" Checked='<%# Bind("ItemEnabled") %>' Enabled="false" />
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Characteritics" 
                   SortExpression="ItemsCharacteritics">
                   <EditItemTemplate>
                     <asp:TextBox ID="tbxItemCharacteristics" runat="server" Text='<%# Bind("ItemsCharacteritics") %>' />
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:TextBox ID="tbxItemCharacteristics" runat="server" Text="" />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:Label ID="lblItemCharacteristics" runat="server" Text='<%# Bind("ItemsCharacteritics") %>'></asp:Label>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Detail" SortExpression="ItemDetail">
                   <EditItemTemplate>
                     <asp:TextBox ID="tbxItemDetail" runat="server" Text='<%# Bind("ItemDetail") %>'></asp:TextBox>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:TextBox ID="tbxItemDetail" runat="server" Text="" />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:Label ID="lblItemDetail" runat="server" Text='<%# Bind("ItemDetail") %>' />
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Type" SortExpression="ServiceTypeId">
                   <EditItemTemplate>
                     <asp:DropDownList ID="ddlServiceType" runat="server"
                       DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                       DataValueField="ServiceTypeId" SelectedValue='<%# Bind("ServiceTypeId") %>'>
                     </asp:DropDownList>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:DropDownList ID="ddlServiceType" runat="server"  
                       DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                       DataValueField="ServiceTypeId" SelectedValue='<%# Bind("ServiceTypeId") %>'>
                     </asp:DropDownList>
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:DropDownList ID="ddlServiceType" runat="server" 
                       DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                       DataValueField="ServiceTypeId" Enabled="False" 
                       SelectedValue='<%# Bind("ServiceTypeId") %>'>
                     </asp:DropDownList>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Replacement" SortExpression="Replacement">
                   <EditItemTemplate>
                     <asp:DropDownList ID="ddlReplacement" runat="server" 
                       AppendDataBoundItems="True" DataSourceID="sdsItems" DataTextField="ItemDesc" 
                       DataValueField="ItemTypeID" SelectedValue='<%# Bind("Replacement") %>'>
                       <asp:ListItem Value="0">none</asp:ListItem>
                     </asp:DropDownList>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:DropDownList ID="ddlReplacement" runat="server" 
                       AppendDataBoundItems="True" DataSourceID="sdsItems" DataTextField="ItemDesc" 
                       DataValueField="ItemTypeID" SelectedValue='<%# Bind("Replacement") %>'>
                       <asp:ListItem Value="0">none</asp:ListItem>
                     </asp:DropDownList>
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:DropDownList ID="ddlReplacement" runat="server" 
                       AppendDataBoundItems="True" DataSourceID="sdsItems" DataTextField="ItemDesc" 
                       DataValueField="ItemTypeID" Enabled="False" 
                       SelectedValue='<%# Bind("Replacement") %>'>
                       <asp:ListItem Value="0">none</asp:ListItem>
                     </asp:DropDownList>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Short Name" SortExpression="ItemShortName">
                   <EditItemTemplate>
                     <asp:TextBox ID="tbxItemShortName" runat="server" Text='<%# Bind("ItemShortName") %>'></asp:TextBox>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:TextBox ID="tbxItemShortName" runat="server" Text="" Width="4em" />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:Label ID="lblItemShortName" runat="server" Text='<%# Bind("ItemShortName") %>'></asp:Label>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField HeaderText="Sort Order" SortExpression="SortOrder">
                   <EditItemTemplate>
                     <asp:TextBox ID="tbxSortOrder" runat="server" Text='<%# Bind("SortOrder") %>'></asp:TextBox>
                   </EditItemTemplate>
                   <FooterTemplate>
                     <asp:TextBox ID="tbxSortOrder" runat="server" Text='5' Width="1.1em" />
                   </FooterTemplate>
                   <ItemTemplate>
                     <asp:Label ID="lblSortOrder" runat="server" Text='<%# Bind("SortOrder") %>'></asp:Label>
                   </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField ShowHeader="False">
                   <EditItemTemplate>
                     <asp:Button ID="btnUpdate" runat="server" CausesValidation="False" 
                       CommandName="Update" Text="go" />
                     &nbsp;<asp:Button ID="btnCancel" runat="server" CausesValidation="False" 
                       CommandName="Cancel" Text="no" />
                   </EditItemTemplate>
                   <ItemTemplate>
                     <asp:Button ID="btnEdit" runat="server" CausesValidation="False" 
                       CommandName="Edit" Text="Edit" />
                   </ItemTemplate>
                   <FooterTemplate>
                     <asp:Button ID="btnAdd" runat="server" CausesValidation="False" 
                       CommandName="AddItem" Text="Add" />
                   </FooterTemplate>
                 </asp:TemplateField>
               </Columns>
               <EditRowStyle BackColor="#2461BF" />
               <EmptyDataTemplate>
                 <asp:DetailsView ID="dvItemIns" runat="server" AutoGenerateRows="False" 
                   BackColor="White" BorderColor="#DEDFDE" BorderStyle="Solid" BorderWidth="1px" 
                   CellPadding="4" DataKeyNames="ItemTypeID" DataSourceID="sdsItems" 
                   OnItemInserted="dvItems_ItemInserted"
                   ForeColor="Black" GridLines="Vertical" Width="220px">
                   <AlternatingRowStyle BackColor="White" />
                   <EditRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                   <Fields>
                     <asp:BoundField DataField="ItemDesc" HeaderText="ItemDesc" 
                       SortExpression="ItemDesc" />
                     <asp:BoundField DataField="ItemsCharacteritics" 
                       HeaderText="ItemsCharacteritics" SortExpression="ItemsCharacteritics" />
                     <asp:BoundField DataField="ItemDetail" HeaderText="ItemDetail" 
                       SortExpression="ItemDetail" />
                     <asp:TemplateField HeaderText="ServiceType" SortExpression="ServiceTypeId">
                       <EditItemTemplate>
                         <asp:DropDownList ID="ddlEditServiceType" runat="server" 
                           DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                           DataValueField="ServiceTypeId" SelectedValue='<%# Bind("ServiceTypeId") %>'>
                         </asp:DropDownList>
                       </EditItemTemplate>
                       <InsertItemTemplate>
                         <asp:DropDownList ID="ddlInsServiceType" runat="server" 
                           DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                           DataValueField="ServiceTypeId" SelectedValue='<%# Bind("ServiceTypeId") %>'>
                         </asp:DropDownList>
                       </InsertItemTemplate>
                       <ItemTemplate>
                         <asp:DropDownList ID="ddlServiceType" runat="server" 
                           DataSourceID="sdsServiceTypes" DataTextField="ServiceType" 
                           DataValueField="ServiceTypeId" SelectedValue='<%# Bind("ServiceTypeId") %>'>
                         </asp:DropDownList>
                       </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Replacement" SortExpression="Replacement">
                       <EditItemTemplate>
                         <asp:DropDownList ID="ddlEditReplacement" runat="server" DataSourceID="sdsItems" 
                           DataTextField="ItemDesc" DataValueField="ItemTypeID" 
                           SelectedValue='<%# Bind("Replacement") %>'>
                         </asp:DropDownList>
                       </EditItemTemplate>
                       <InsertItemTemplate>
                         <asp:DropDownList ID="ddlInsReplacement" runat="server" DataSourceID="sdsItems" 
                           DataTextField="ItemDesc" DataValueField="ItemTypeID" 
                           SelectedValue='<%# Bind("Replacement") %>'>
                         </asp:DropDownList>
                       </InsertItemTemplate>
                       <ItemTemplate>
                         <asp:DropDownList ID="ddlReplacement" runat="server" 
                           AppendDataBoundItems="True" DataSourceID="sdsItems" DataTextField="ItemDesc" 
                           DataValueField="ItemTypeID" SelectedValue='<%# Bind("Replacement") %>'>
                           <asp:ListItem Value="0">none</asp:ListItem>
                         </asp:DropDownList>
                       </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ItemShortName" HeaderText="ItemShortName" 
                       SortExpression="ItemShortName" />
                     <asp:BoundField DataField="SortOrder" HeaderText="SortOrder" 
                       SortExpression="SortOrder" />
                     <asp:CheckBoxField DataField="ItemEnabled" HeaderText="ItemEnabled" 
                       SortExpression="ItemEnabled" />
                     <asp:CommandField ShowEditButton="True" ShowInsertButton="True" ButtonType="Button" />
                   </Fields>
                   <FooterStyle BackColor="#CCCC99" />
                   <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                   <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                   <RowStyle BackColor="#F7F7DE" />
                 </asp:DetailsView>
               </EmptyDataTemplate>
               <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
               <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
               <PagerSettings FirstPageImageUrl="~/images/imgButtons/FirstPage.gif" 
                 LastPageImageUrl="~/images/imgButtons/LastPage.gif" 
                 Mode="NextPreviousFirstLast" 
                 NextPageImageUrl="~/images/imgButtons/NextPage.gif" 
                 PreviousPageImageUrl="~/images/imgButtons/PrevPage.gif" />
               <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Right" />
               <RowStyle BackColor="#EFF3FB" />
               <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
               <SortedAscendingCellStyle BackColor="#F5F7FB" />
               <SortedAscendingHeaderStyle BackColor="#6D95E1" />
               <SortedDescendingCellStyle BackColor="#E9EBEF" />
               <SortedDescendingHeaderStyle BackColor="#4870BE" />
             </asp:GridView>

             <asp:SqlDataSource ID="sdsItems" runat="server" 
               ConflictDetection="CompareAllValues" 
               ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
               DeleteCommand="DELETE FROM [ItemTypeTbl] WHERE [ItemTypeID] = ? " 
               InsertCommand="INSERT INTO [ItemTypeTbl] ([ItemDesc], [ItemEnabled], [ItemsCharacteritics], [ItemDetail], [ServiceTypeId], [ReplacementID], [ItemShortName], [SortOrder]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)" 
               OldValuesParameterFormatString="original_{0}" 
               ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
               SelectCommand="SELECT ItemTypeID, ItemDesc, ItemEnabled, ItemsCharacteritics, ItemDetail, ServiceTypeId, iif(IsNull(ReplacementID), 0, ReplacementID) AS Replacement, ItemShortName, SortOrder FROM ItemTypeTbl ORDER BY SortOrder, ItemDesc" 
               
               UpdateCommand="UPDATE ItemTypeTbl SET ItemDesc = ?, ItemEnabled = ?, ItemsCharacteritics = ?, ItemDetail = ?, ServiceTypeId = ?, ReplacementID = ?, ItemShortName = ?, SortOrder = ? WHERE (ItemTypeID = ?)">
               <DeleteParameters>
                 <asp:Parameter Name="original_ItemTypeID" Type="Int32" />
               </DeleteParameters>
               <InsertParameters>
                 <asp:Parameter Name="ItemDesc" Type="String" />
                 <asp:Parameter Name="ItemEnabled" Type="Boolean" />
                 <asp:Parameter Name="ItemsCharacteritics" Type="String" />
                 <asp:Parameter Name="ItemDetail" Type="String" />
                 <asp:Parameter Name="ServiceTypeId" Type="Int32" />
                 <asp:Parameter Name="ReplacementID" Type="Int32" />
                 <asp:Parameter Name="ItemShortName" Type="String" />
                 <asp:Parameter Name="SortOrder" Type="Int32" />
               </InsertParameters>
               <UpdateParameters>
                 <asp:Parameter Name="ItemDesc" Type="String" />
                 <asp:Parameter Name="ItemEnabled" Type="Boolean" />
                 <asp:Parameter Name="ItemsCharacteritics" Type="String" />
                 <asp:Parameter Name="ItemDetail" Type="String" />
                 <asp:Parameter Name="ServiceTypeId" Type="Int32" />
                 <asp:Parameter Name="ReplacementID" Type="Int32" />
                 <asp:Parameter Name="ItemShortName" Type="String" />
                 <asp:Parameter Name="SortOrder" Type="Int32" />
                 <asp:Parameter Name="original_ItemTypeID" Type="Int32" />
               </UpdateParameters>
             </asp:SqlDataSource>
             <asp:SqlDataSource ID="sdsServiceTypes" runat="server" 
               ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
               ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
               SelectCommand="SELECT [ServiceTypeId], [ServiceType] FROM [ServiceTypesTbl]">
             </asp:SqlDataSource>
             <asp:SqlDataSource ID="sdsReplacementItems" runat="server" 
               ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
               ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
               SelectCommand="SELECT [ItemTypeID], [ItemDesc] FROM [ItemTypeTbl] ORDER BY [ItemDesc]">
             </asp:SqlDataSource>
          </ContentTemplate>
        </asp:UpdatePanel>
      </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="tabpnlPeople" runat="server" HeaderText="People">
      <HeaderTemplate>People</HeaderTemplate>
      <ContentTemplate>
        <asp:GridView ID="gvPeople" runat="server" AllowPaging="True" 
          AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" PageSize="20"
          DataKeyNames="PersonID" DataSourceID="sdsPeople" ForeColor="#333333" 
          GridLines="Vertical">
          <AlternatingRowStyle BackColor="White" />
          <Columns>
            <asp:CommandField ShowEditButton="True" ButtonType="Button" />
            <asp:BoundField DataField="PersonID" HeaderText="PersonID" 
              InsertVisible="False" ReadOnly="True" SortExpression="PersonID" />
            <asp:BoundField DataField="Person" HeaderText="Person" 
              SortExpression="Person" />
            <asp:BoundField DataField="Abreviation" HeaderText="Abreviation" 
              SortExpression="Abreviation" />
            <asp:CheckBoxField DataField="Enabled" HeaderText="Enabled" 
              SortExpression="Enabled" />
          </Columns>
          <EditRowStyle BackColor="#7C6F57" />
          <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
          <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
          <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
          <RowStyle BackColor="#E3EAEB" />
          <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
          <SortedAscendingCellStyle BackColor="#F8FAFA" />
          <SortedAscendingHeaderStyle BackColor="#246B61" />
          <SortedDescendingCellStyle BackColor="#D4DFE1" />
          <SortedDescendingHeaderStyle BackColor="#15524A" />
        </asp:GridView>
        <asp:SqlDataSource ID="sdsPeople" runat="server" 
          ConflictDetection="CompareAllValues" 
          ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
          DeleteCommand="DELETE FROM [PersonsTbl] WHERE [PersonID] = ? AND (([Person] = ?) OR ([Person] IS NULL AND ? IS NULL)) AND (([Abreviation] = ?) OR ([Abreviation] IS NULL AND ? IS NULL)) AND [Enabled] = ?" 
          InsertCommand="INSERT INTO [PersonsTbl] ([PersonID], [Person], [Abreviation], [Enabled]) VALUES (?, ?, ?, ?)" 
          OldValuesParameterFormatString="original_{0}" 
          ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
          SelectCommand="SELECT [PersonID], [Person], [Abreviation], [Enabled] FROM [PersonsTbl] ORDER BY [Enabled], [Abreviation]" 
          UpdateCommand="UPDATE [PersonsTbl] SET [Person] = ?, [Abreviation] = ?, [Enabled] = ? WHERE [PersonID] = ? AND (([Person] = ?) OR ([Person] IS NULL AND ? IS NULL)) AND (([Abreviation] = ?) OR ([Abreviation] IS NULL AND ? IS NULL)) AND [Enabled] = ?">
          <DeleteParameters>
            <asp:Parameter Name="original_PersonID" Type="Int32" />
            <asp:Parameter Name="original_Person" Type="String" />
            <asp:Parameter Name="original_Person" Type="String" />
            <asp:Parameter Name="original_Abreviation" Type="String" />
            <asp:Parameter Name="original_Abreviation" Type="String" />
            <asp:Parameter Name="original_Enabled" Type="Boolean" />
          </DeleteParameters>
          <InsertParameters>
            <asp:Parameter Name="PersonID" Type="Int32" />
            <asp:Parameter Name="Person" Type="String" />
            <asp:Parameter Name="Abreviation" Type="String" />
            <asp:Parameter Name="Enabled" Type="Boolean" />
          </InsertParameters>
          <UpdateParameters>
            <asp:Parameter Name="Person" Type="String" />
            <asp:Parameter Name="Abreviation" Type="String" />
            <asp:Parameter Name="Enabled" Type="Boolean" />
            <asp:Parameter Name="original_PersonID" Type="Int32" />
            <asp:Parameter Name="original_Person" Type="String" />
            <asp:Parameter Name="original_Person" Type="String" />
            <asp:Parameter Name="original_Abreviation" Type="String" />
            <asp:Parameter Name="original_Abreviation" Type="String" />
            <asp:Parameter Name="original_Enabled" Type="Boolean" />
          </UpdateParameters>
        </asp:SqlDataSource>
      </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="tabpnlEquipment" runat="server" HeaderText="Equipment">
      <ContentTemplate>
        <asp:GridView id="gvEquipment" runat="server" AllowPaging="True" EmptyDataText="No equipment found"  
          AllowSorting="True" AutoGenerateColumns="False" BackColor="White" 
          BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
          DataKeyNames="EquipTypeId" DataSourceID="odsEquipTypes" PageSize="20" >
          <AlternatingRowStyle BackColor="#F7F7F7" />
          <Columns>
            <asp:TemplateField ShowHeader="False">
              <EditItemTemplate>
                <asp:Button ID="UpdateButton" runat="server" CausesValidation="False" CommandName="Update" Text="Update" />
                &nbsp;<asp:Button ID="CancelButton" runat="server" CausesValidation="False" 
                  CommandName="Cancel" Text="Cancel" />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Button ID="EditButton" runat="server" CausesValidation="False" 
                  CommandName="Edit" Text="Edit" />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="EquipTypeId" InsertVisible="False" 
              SortExpression="EquipTypeId">
              <EditItemTemplate>
                <asp:Label ID="EquipTypeIdLabel" runat="server" Text='<%# Eval("EquipTypeId") %>' Enabled="false" />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="EquipTypeIdLabel" runat="server" Text='<%# Bind("EquipTypeId") %>' Enabled="false" />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="EquipTypeName" SortExpression="EquipTypeName">
              <EditItemTemplate>
                <asp:TextBox ID="EquipTypeNameTextBox" runat="server" Text='<%# Bind("EquipTypeName") %>'></asp:TextBox>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="EquipTypeNameLabel" runat="server" Text='<%# Bind("EquipTypeName") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="EquipTypeDesc" SortExpression="EquipTypeDesc">
              <EditItemTemplate>
                <asp:TextBox ID="EquipTypeDescTextBox" runat="server" Text='<%# Bind("EquipTypeDesc") %>'></asp:TextBox>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="EquipTypeDescLabel" runat="server" Text='<%# Bind("EquipTypeDesc") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
          <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
          <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
          <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
          <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
          <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
          <SortedAscendingCellStyle BackColor="#F4F4FD" />
          <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
          <SortedDescendingCellStyle BackColor="#D8D8F0" />
          <SortedDescendingHeaderStyle BackColor="#3E3277" />
        </asp:GridView> 
      </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="tabpnlCities" runat="server" HeaderText="Cities">
      <ContentTemplate>
        <asp:GridView ID="gvCities" runat="server" AllowPaging="True" PageSize="20"
          AllowSorting="True" AutoGenerateColumns="False" BackColor="White" 
          BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
          DataSourceID="sdsCities" ForeColor="Black" DataKeyNames="ID" 
          ShowFooter="True">
          <AlternatingRowStyle BackColor="White" />
          <Columns>
            <asp:TemplateField HeaderText="City" SortExpression="City">
              <EditItemTemplate>
                <asp:TextBox ID="tbxCity" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="lblCity" runat="server" Text='<%# Bind("City") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="RoastingDay" SortExpression="RoastingDay">
              <EditItemTemplate>
                <asp:DropDownList ID="ddlRoastingDoW" runat="server" 
                  SelectedValue='<%# Bind("RoastingDay") %>'>
                  <asp:ListItem Value="1">Sunday</asp:ListItem>
                  <asp:ListItem Value="2">Monday</asp:ListItem>
                  <asp:ListItem Selected="True" Value="3">Tuesday</asp:ListItem>
                  <asp:ListItem Value="4">Wednesday</asp:ListItem>
                  <asp:ListItem Value="5">Thursday</asp:ListItem>
                  <asp:ListItem Value="6">Friday</asp:ListItem>
                  <asp:ListItem Value="7">Saturday</asp:ListItem>
                </asp:DropDownList>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:DropDownList ID="ddlRoastingDoW" runat="server" 
                  SelectedValue='<%# Bind("RoastingDay") %>'>
                  <asp:ListItem Value="1">Sunday</asp:ListItem>
                  <asp:ListItem Value="2">Monday</asp:ListItem>
                  <asp:ListItem Selected="True" Value="3">Tuesday</asp:ListItem>
                  <asp:ListItem Value="4">Wednesday</asp:ListItem>
                  <asp:ListItem Value="5">Thursday</asp:ListItem>
                  <asp:ListItem Value="6">Friday</asp:ListItem>
                  <asp:ListItem Value="7">Saturday</asp:ListItem>
                </asp:DropDownList>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="DeliveryDelay" SortExpression="DeliveryDelay">
              <EditItemTemplate>
                <asp:TextBox ID="tbxDeliveryDelay" runat="server" Text='<%# Bind("DeliveryDelay") %>'></asp:TextBox>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:Label ID="lblDeliveryDay" runat="server" Text='<%# Bind("DeliveryDelay") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" >
              <EditItemTemplate>
                <asp:LinkButton ID="btnUpdate" runat="server" CausesValidation="True" 
                  CommandName="Update" Text="Update" />
                &nbsp;<asp:LinkButton ID="btnCancel" runat="server" CausesValidation="False" 
                  CommandName="Cancel" Text="Cancel" />
              </EditItemTemplate>
              <ItemTemplate>
                <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False"  
                  CommandName="Edit" Text="Edit" />
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
          <FooterStyle BackColor="#CCCC99" />
          <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
          <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
          <RowStyle BackColor="#F7F7DE" />
          <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
          <SortedAscendingCellStyle BackColor="#FBFBF2" />
          <SortedAscendingHeaderStyle BackColor="#848384" />
          <SortedDescendingCellStyle BackColor="#EAEAD3" />
          <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
        <asp:SqlDataSource ID="sdsCities" runat="server" 
          onselecting="sdsCities_Selecting" ConflictDetection="CompareAllValues" 
          ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
          DeleteCommand="DELETE FROM [CityTbl] WHERE [ID] = ? AND (([City] = ?) OR ([City] IS NULL AND ? IS NULL)) AND (([RoastingDay] = ?) OR ([RoastingDay] IS NULL AND ? IS NULL)) AND (([DeliveryDelay] = ?) OR ([DeliveryDelay] IS NULL AND ? IS NULL))" 
          InsertCommand="INSERT INTO [CityTbl] ([ID], [City], [RoastingDay], [DeliveryDelay]) VALUES (?, ?, ?, ?)" 
          OldValuesParameterFormatString="original_{0}" 
          ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
          SelectCommand="SELECT [ID], [City], [RoastingDay], [DeliveryDelay] FROM [CityTbl] ORDER BY [City]" 
          UpdateCommand="UPDATE [CityTbl] SET [City] = ?, [RoastingDay] = ?, [DeliveryDelay] = ? WHERE [ID] = ?">
          <DeleteParameters>
            <asp:Parameter Name="original_ID" Type="Int32" />
            <asp:Parameter Name="original_City" Type="String" />
            <asp:Parameter Name="original_City" Type="String" />
            <asp:Parameter Name="original_RoastingDay" Type="Int32" />
            <asp:Parameter Name="original_RoastingDay" Type="Int32" />
            <asp:Parameter Name="original_DeliveryDelay" Type="Int32" />
            <asp:Parameter Name="original_DeliveryDelay" Type="Int32" />
          </DeleteParameters>
          <InsertParameters>
            <asp:Parameter Name="ID" Type="Int32" />
            <asp:Parameter Name="City" Type="String" />
            <asp:Parameter Name="RoastingDay" Type="Int32" />
            <asp:Parameter Name="DeliveryDelay" Type="Int32" />
          </InsertParameters>
          <UpdateParameters>
            <asp:Parameter Name="City" Type="String" />
            <asp:Parameter Name="RoastingDay" Type="Int32" />
            <asp:Parameter Name="DeliveryDelay" Type="Int32" />
            <asp:Parameter Name="original_ID" Type="Int32" />
          </UpdateParameters>
        </asp:SqlDataSource>
        <asp:ObjectDataSource ID="odsCities" runat="server" 
          DataObjectTypeName="TrackerDotNet.DataSets.LookUpDatSets+CityTblDataTable" 
          OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
          TypeName="TrackerDotNet.DataSets.LookUpDatSetsTableAdapters.CityTblTableAdapter" 
          UpdateMethod="Update"></asp:ObjectDataSource>
      </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
    </cc1:TabPanel>
  </cc1:TabContainer>

  <asp:ObjectDataSource ID="odsEquipTypes" runat="server" TypeName="TrackerDotNet.control.EquipTypeTbl" 
      SortParameterName="SortBy" SelectMethod="GetAll" DataObjectTypeName="TrackerDotNet.control.EquipTypeTbl"
      OldValuesParameterFormatString="original_{0}" UpdateMethod="UpdateEquipItem">
      <SelectParameters>
        <asp:Parameter DefaultValue="EquipTypeName" Name="SortBy" Type="String" />
      </SelectParameters>
  </asp:ObjectDataSource>

</asp:Content>
