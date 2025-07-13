<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderEntry.aspx.cs"
  MaintainScrollPositionOnPostback="true" Inherits="TrackerDotNet.Pages.OrderEntry" %>
<asp:Content ID="cntOrderEntryHder" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntOrderEntryBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Order Detail</h2>
   <div class="simpleLightBrownForm">
     <table class="TblWhite">
       <thead>
         <tr>
           <td>Done/Undone</td>
           <td>Search for:</td>
           <td>Value</td>
          </tr>
        </thead>
        <tbody>
         <tr>
            <td><asp:CheckBox ID="chkbxOrderDone" Text="Order Done" runat="server" Checked="false" TextAlign="Right" AutoPostBack="true"  /></td>
            <td>
              <asp:DropDownList ID="ddlSearchFor" runat="server">
                <asp:ListItem Selected="True" Value="none" Text="--Select item--" />
                <asp:ListItem Value="Company" Text="Company Name" />
                <asp:ListItem Value="PrepDate" Text="Preperation Date" />
              </asp:DropDownList>
            </td>
            <td>
              <asp:TextBox ID="tbxSearchFor" runat="server" Width="35em" ontextchanged="tbxSearchFor_TextChanged" 
                 />&nbsp;&nbsp;<asp:Button ID="btnGo"
                Text="Go" runat="server" onclick="btnGo_Click" /></td>
         </tr>
        </tbody>
      </table>
   </div>

  <asp:ObjectDataSource ID="odsDistinctOrders" runat="server" TypeName="TrackerDotNet.App_Code.OrderData" 
    SelectMethod="GetDistinctOrders" UpdateMethod="UpdateOrderRoastDate" >
    <SelectParameters>
      <asp:ControlParameter ControlID="chkbxOrderDone" Name="OrderDone" 
        PropertyName="Checked" Type="Boolean" 
        DefaultValue="false" />
      <asp:ControlParameter ControlID="ddlSearchFor" Name="SearchFor" PropertyName="SelectedValue" />
      <asp:ControlParameter ControlID="tbxSearchFor" Name="SearchValue" PropertyName="Text" />
    </SelectParameters>
  </asp:ObjectDataSource>

  
  <p>&nbsp;</p>
  <asp:GridView ID="gvListOfOrders" runat="server" CssClass="TblCoffee" 
    AllowPaging="True" AllowSorting="True" DataSourceID="odsDistinctOrders" 
    onselectedindexchanged="gvCurrent_SelectedIndexChanged" 
    AutoGenerateColumns="False" PageSize="15">
    <AlternatingRowStyle BackColor="#EFFFEF" />
    <Columns>
      <asp:CommandField ShowSelectButton="True" ButtonType="Image" SelectImageUrl="~/images/imgButtons/SelectItem.gif"   />
      <asp:BoundField DataField="CompanyName" HeaderText="CompanyName" 
        SortExpression="CompanyName" />
      <asp:HyperLinkField Text="edit..." NavigateUrl="<%# String.Format('OrderDetail.aspx?CustomerId={0}&PrepDate={1}', Bind('CustomerId'), Bind('RoastDate')) %>" />
      <asp:TemplateField HeaderText="CustomerId" SortExpression="CustomerId">
        <EditItemTemplate>
          <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CustomerId") %>'></asp:TextBox>
        </EditItemTemplate>
        <ItemTemplate>
          <asp:Label ID="Label1" runat="server" Text='<%# Bind("CustomerId") %>'></asp:Label>
        </ItemTemplate>
      </asp:TemplateField>
      <asp:BoundField DataField="OrderDate" HeaderText="Order Dt" DataFormatString="{0:d}"
        SortExpression="OrderDate" />
      <asp:BoundField DataField="RoastDate" HeaderText="Roast Dt" DataFormatString="{0:d}" 
        SortExpression="RoastDate" />
      <asp:BoundField DataField="RequiredByDate" HeaderText="On Date"  DataFormatString="{0:d}"
        SortExpression="RequiredByDate" />
      <asp:BoundField DataField="Person" HeaderText="Delivery By"  
        SortExpression="Person" />
      <asp:CheckBoxField DataField="Confirmed" HeaderText="Confirmed" 
        SortExpression="Confirmed" />
      <asp:CheckBoxField DataField="Done" HeaderText="Done" SortExpression="Done" />
      <asp:ButtonField  ButtonType="Image" ImageUrl="~/images/imgButtons/DelItem.gif" />
    </Columns>
    <PagerSettings Mode="NumericFirstLast" Position="Bottom" />
    <SelectedRowStyle BackColor="#66FF66" />
  </asp:GridView>
<%--  <asp:SqlDataSource ID="sdsOrdersDetail" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
   
    SelectCommand="SELECT DISTINCT CustomersTbl.CompanyName, OrdersTbl.CustomerId, OrdersTbl.OrderDate, OrdersTbl.RoastDate, OrdersTbl.RequiredByDate, PersonsTbl.Person, OrdersTbl.Confirmed, OrdersTbl.Done FROM ((OrdersTbl LEFT OUTER JOIN PersonsTbl ON OrdersTbl.ToBeDeliveredBy = PersonsTbl.PersonID) LEFT OUTER JOIN CustomersTbl ON OrdersTbl.CustomerId = CustomersTbl.CustomerID) WHERE (OrdersTbl.Done = ?)">
    <SelectParameters>
      <asp:ControlParameter ControlID="chkbxOrderDone" Name="Done" 
        PropertyName="Checked" Type="Boolean" 
        DefaultValue="false" />
    </SelectParameters>
  </asp:SqlDataSource>
--%>
  <asp:GridView ID="gvOrderDetails" runat="server" CssClass="TblZebra" AutoGenerateEditButton="true" 
    OnRowUpdated="gvOrderDetails_OnRowUpdated" OnRowEditing="gvOrderDetails_OnRowEditing">
  </asp:GridView>
  <br />




<%--
  <table class="TblLHCol-brown">
    <th>
      <tr class="TblLHColHdr">
        <td>Item</td>
        <td>Value</td>
      </tr>
    </th>
    <tbody>
    <tr>
      <td class="TblLHCol-first">
        Customer
      </td>
      <td>

      </td>
    </tr>
    <tr>
      <td class="TblLHCol-first">
        Order Date:
      </td>
      <td></td>
    </tr>
    <tr>
      <td class="TblLHCol-first">
        Preperation Date</td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td class="TblLHCol-first">
        Delivery By</td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td class="TblLHCol-first">
        Delivery Date</td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td class="TblLHCol-first">
        Notes</td>
      <td>&nbsp;</td>
    </tr>
    </tbody>
  </table>
  <br />
--%>  

</asp:Content>
