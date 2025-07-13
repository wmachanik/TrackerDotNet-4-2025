<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderDetail.aspx.cs" 
  Inherits="TrackerDotNet.Pages.OrderDetail"  MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="cntOrderDetailHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntOrderDetailBdy" ContentPlaceHolderID="MainContent" runat="server">
  <br />
  <asp:ScriptManager runat="server" ID="scrmOrderDetail" />
  <asp:UpdateProgress ID="udtpOrderDetail" runat="server">
    <ProgressTemplate>&nbsp;&nbsp;
      <img src="../images/animi/QuaffeeProgress.gif" alt="please wait..." width="128" height="15" />
    </ProgressTemplate>
  </asp:UpdateProgress>
  <table class="TblSimple">
    <tr>
      <td rowspan="2" style="vertical-align: top" >
        <asp:UpdatePanel ID="pnlOrderHeader" runat="server" UpdateMode="Conditional"  >
          <ContentTemplate>
            <asp:DetailsView ID="dvOrderHeader" runat="server" DataSourceID="odsOrderSummary" 
              AutoGenerateRows="False" CssClass="TblWhite" OnItemUpdated="dvOrderHeader_OnItemUpdated" >
              <EmptyDataTemplate>Return to delivery sheet...</EmptyDataTemplate>
              <Fields>
                <asp:TemplateField HeaderText="CustomerID" SortExpression="CustomerID">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" Enabled="false" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="OrderDate" SortExpression="OrderDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxOrderDate" runat="server" Text='<%# Bind("OrderDate", "{0:d}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxOrderDate_CalendarExtender" runat="server" 
                      Enabled="True" TargetControlID="tbxOrderDate" >
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxOrderDate" runat="server" 
                      Text='<%# Bind("OrderDate", "{0:d}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblOrderDate" runat="server" Text='<%# Bind("OrderDate", "{0:d}") %>' />
                  </ItemTemplate>
                  <ItemStyle Font-Bold="True" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="RoastDate" SortExpression="RoastDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxRoastDate" runat="server" Text='<%# Bind("RoastDate", "{0:d}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxRoastDate_CalendarExtender" runat="server" 
                      Enabled="True" TargetControlID="tbxRoastDate">
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxRoastDate" runat="server" 
                      Text='<%# Bind("RoastDate", "{0:d}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblRoastDate" runat="server" Text='<%# Bind("RoastDate", "{0:d}") %>'></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ToBeDeliveredBy" 
                  SortExpression="ToBeDeliveredBy">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" OnDataBinding="OnDataBinding_ddlToBeDeliveredBy"
                      DataValueField="PersonID" SelectedValue='<%# Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>

                    </asp:DropDownList>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" OnDataBinding="OnDataBinding_ddlToBeDeliveredBy"
                      DataValueField="PersonID" SelectedValue='<%# Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>
                    </asp:DropDownList>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" Enabled="False" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" OnDataBinding="OnDataBinding_ddlToBeDeliveredBy"
                      DataValueField="PersonID" SelectedValue='<%# Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="RequiredByDate" SortExpression="RequiredByDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxRequiredByDate" runat="server" Text='<%# Bind("RequiredByDate", "{0:d}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxRequiredByDate_CalendarExtender" 
                      runat="server" Enabled="True" TargetControlID="tbxRequiredByDate">
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxRequiredByDate" runat="server" 
                      Text='<%# Bind("RequiredByDate", "{0:d}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblRequiredByDate" runat="server" 
                      Text='<%# Bind("RequiredByDate", "{0:d}") %>'></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Confirmed" SortExpression="Confirmed">
                  <EditItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" runat="server" Checked='<%# Bind("Confirmed") %>' />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" runat="server" Checked='<%# Bind("Confirmed") %>' />
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" runat="server" Checked='<%# Bind("Confirmed") %>'  Enabled="false" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Done" SortExpression="Done">
                  <EditItemTemplate>
                    <asp:CheckBox ID="cbxDone" runat="server" Checked='<%# Bind("Done") %>' Enabled="false" />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:CheckBox ID="cbxDone" runat="server" Checked='<%# Bind("Done") %>' Enabled="false" />
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:CheckBox ID="cbxDone" runat="server" Checked='<%# Bind("Done") %>' Enabled="false" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Notes" SortExpression="Notes">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxNotes" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine" />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxNotes" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine" />
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblNotes" runat="server" Text='<%# Bind("Notes") %>' />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" />
              </Fields>
            </asp:DetailsView>
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
      <td style="vertical-align: top" >
        <asp:UpdatePanel ID="upnlOrderLines" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:GridView ID="gvOrderLines" runat="server" AutoGenerateColumns="False" 
              DataSourceID="odsOrderDetail" >
              <Columns>
                <asp:CommandField ShowEditButton="True"  ShowDeleteButton="false" CancelImageUrl="~/images/imgButtons/CancelItem.gif" 
                  DeleteImageUrl="~/images/imgButtons/DelItem.gif" 
                  EditImageUrl="~/images/imgButtons/EditItem.gif" 
                  UpdateImageUrl="~/images/imgButtons/UpdateItem.gif" InsertVisible="False" />
                <asp:TemplateField HeaderText="Item Type" SortExpression="ItemTypeID">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItems" 
                      DataTextField="ItemDesc" DataValueField="ItemTypeID"  
                      SelectedValue='<%# Bind("ItemTypeID") %>'>
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItems" 
                      DataTextField="ItemDesc" DataValueField="ItemTypeID" Enabled="false"
                      SelectedValue='<%# Bind("ItemTypeID") == null ? 0 : Bind("ItemTypeID") %>'>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="QTY" SortExpression="QuantityOrdered">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxQuantityOrdered" runat="server" Text='<%# Bind("QuantityOrdered") %>' Width="2em" />
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblQuantityOrdered" runat="server" Text='<%# Bind("QuantityOrdered") %>' Width="2em" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Packaging" SortExpression="PackagingID">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlPackaging" runat="server" 
                      DataSourceID="sdsPackagingTypes" DataTextField="Description" AppendDataBoundItems="true"
                      DataValueField="PackagingID" SelectedValue='<%# Bind("PackagingID") %>'>
                      <asp:ListItem Text="n/a" Value="0" ></asp:ListItem>
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlPackaging" runat="server" 
                      DataSourceID="sdsPackagingTypes" DataTextField="Description" 
                      DataValueField="PackagingID" Enabled="False" AppendDataBoundItems="true"  
                      SelectedValue='<%# Bind("PackagingID") == null ? 0 : Bind("PackagingID")  %>'>
                      <asp:ListItem Text="n/a" Value="0" ></asp:ListItem>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID">
                  <EditItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' /></EditItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' />
                  </ItemTemplate>
                </asp:TemplateField>
<%--
                <asp:BoundField DataField="OrderID"  HeaderText="OrderID" ReadOnly="true" />
--%>
                <asp:TemplateField>
                  <ItemTemplate>
                    <asp:HyperLink ID="hlDelete" runat="server" Text="del"
                      NavigateUrl='<%# String.Format("~/Pages/DeleteOrderLine.aspx?OrderId={0}", Eval("OrderID")) %>'></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                Add a new order
              </EmptyDataTemplate>
            </asp:GridView>
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
    </tr>
    <tr>
      <td> <%--- New item --%>
        <asp:UpdatePanel ID="upnlNewOrderItem" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:Button ID="btnNewItem" Text="New Item" runat="server" 
              onclick="btnNewItem_Click" />
            <asp:Panel ID="pnlNewItem" runat="server" Visible="false">
                <table class="TblSimple" cellpadding="0" cellspacing="0">
                  <thead>
                    <tr>
                      <td>Item</td>
                      <td>Qty</td>
                      <td>Packaging</td>
                    </tr>
                  </thead
                  <tbody>
                    <tr>
                      <td> 
                        <asp:DropDownList ID="ddlNewItemDesc" runat="server" DataSourceID="sdsItems" 
                          DataTextField="ItemDesc" DataValueField="ItemTypeID" >
                        </asp:DropDownList>
                      </td>
                      <td>
                        <asp:TextBox ID="tbxNewQuantityOrdered" runat="server" Text='1' />
                      </td>
                      <td>
                        <asp:DropDownList ID="ddlNewPackaging" runat="server" 
                          DataSourceID="sdsPackagingTypes" DataTextField="Description" AppendDataBoundItems="true"
                          DataValueField="PackagingID" >
                          <asp:ListItem Text="n/a" Value="0" ></asp:ListItem>
                        </asp:DropDownList>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <asp:Button ID="btnAdd" Text="Add" runat="server" Visible="false" 
                  onclick="btnAdd_Click" />&nbsp;&nbsp;
                <asp:Button ID="btnCancel" Text="Cancel" runat="server" Visible="false" 
                  onclick="btnCancel_Click" />
            </asp:Panel>
            &nbsp;&nbsp;<asp:Literal ID="ltrlStatus" Text="" runat="server" />
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="text-align: center">
        <asp:Button ID="btnNewOrder" runat="server" Text="New Order" PostBackUrl="~/Pages/NewOrderDetail.aspx" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnConfirmOrder" runat="server" Text="Confirm Order" 
          onclick="btnConfirmOrder_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnDlSheet" runat="server" Text="Delivery Sheet" PostBackUrl="~/Pages/DeliverySheet.aspx" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnOrderDelivered" runat="server" Text="Order Delivered" 
          onclick="btnOrderDelivered_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnOrderCancelled" Text="Order Cancelled" runat="server" 
          onclick="btnCancelled_Click" />
      </td>
    </tr>
    <tr>
      <td colspan="2" style="text-align: center">
        <a href="NewOrderDetail.aspx">New Order</a>&nbsp;&nbsp;&nbsp;&nbsp;
        <a href="DeliverySheet.aspx">DeliverySheet</a></td>
    </tr>
  </table>

  <asp:ObjectDataSource ID="odsOrderSummary" runat="server" TypeName="TrackerDotNet.control.OrderItemTbl"
    EnablePaging="True" SelectMethod="LoadOrderSummary" 
    StartRowIndexParameterName="StartRowIndex" 
    MaximumRowsParameterName="MaximumRows"  
    OldValuesParameterFormatString="original_{0}"
    OnUpdated="odsOrderSummary_OnUpdated"
    UpdateMethod="UpdateOrderDetails" >
        <SelectParameters>
          <asp:QueryStringParameter DefaultValue="1" Name="CustomerId" QueryStringField="CustomerID" Type="Int32" />
          <asp:QueryStringParameter Name="DeliveryDate" QueryStringField="DeliveryDate" Type="DateTime" />
          <asp:QueryStringParameter Name="Notes" QueryStringField="Notes" Type="String" />
          <asp:Parameter Name="MaximumRows" Type="Int32" />
          <asp:Parameter Name="StartRowIndex" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
          <asp:Parameter Name="CustomerId" Type="Int32" />
          <asp:Parameter Name="OrderDate" Type="DateTime" />
          <asp:Parameter Name="RoastDate" Type="DateTime" />
          <asp:Parameter Name="ToBeDeliveredBy" Type="Int32" />
          <asp:Parameter Name="RequiredByDate" Type="DateTime" />
          <asp:Parameter Name="Confirmed" Type="Boolean" />
          <asp:Parameter Name="Done" Type="Boolean" />
          <asp:Parameter Name="Notes" Type="String" />
          <asp:QueryStringParameter DefaultValue="1" Name="OriginalCustomerId" QueryStringField="CustomerID" Type="Int32" />
          <asp:QueryStringParameter Name="OriginalDeliveryDate" QueryStringField="DeliveryDate" Type="DateTime" />
          <asp:QueryStringParameter Name="OriginalNotes" QueryStringField="Notes" Type="String" />
        </UpdateParameters>
      </asp:ObjectDataSource>

  <asp:SqlDataSource ID="sdsCompanys" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT [CompanyName], [CustomerID] FROM [CustomersTbl] ORDER BY [enabled], [CompanyName]">
  </asp:SqlDataSource>
  <asp:ObjectDataSource ID="odsOrderDetail" runat="server" 
    TypeName="TrackerDotNet.App_Code.OrderDetailDAL" SelectMethod="LoadOrderDetailData"
    UpdateMethod="UpdateOrderDetails" 
    StartRowIndexParameterName="StartRowIndex" 
    MaximumRowsParameterName="MaximumRows" 
    OldValuesParameterFormatString="original_{0}" 
    DeleteMethod="DeleteOrderDetails" >
    <DeleteParameters>
      <asp:Parameter Name="OrderID" Type="Int32" />
    </DeleteParameters>
    <UpdateParameters>
      <asp:Parameter Name="OrderID" Type="Int32" />
      <asp:Parameter Name="ItemTypeID" Type="Int32" />
      <asp:Parameter Name="QuantityOrdered" Type="Double" />
      <asp:Parameter Name="PackagingID" Type="Int32" />
    </UpdateParameters>
    <SelectParameters>
      <asp:SessionParameter DefaultValue="1" Name="CustomerId" 
        SessionField="BoundCustomerId" Type="Int32" />
      <asp:SessionParameter DefaultValue="" Name="DeliveryDate" 
        SessionField="BoundDeliveryDate" Type="DateTime" />
      <asp:SessionParameter DefaultValue="&quot;&quot;" Name="Notes" 
        SessionField="BoundNotes" Type="String" />
<%--
    <SelectParameters>
      <asp:QueryStringParameter DefaultValue="1" Name="CustomerId" QueryStringField="CustomerID" Type="Int32" />
      <asp:QueryStringParameter Name="DeliveryDate" QueryStringField="DeliveryDate" Type="DateTime" />
      <asp:QueryStringParameter Name="Notes" QueryStringField="Notes" Type="String" />
--%>
      <asp:Parameter Name="MaximumRows" Type="Int32" />
      <asp:Parameter Name="StartRowIndex" Type="Int32" />
    </SelectParameters> 
  </asp:ObjectDataSource>

  <br />

<%-- 

 <asp:SqlDataSource ID="sdsOrderLines" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    
    SelectCommand="SELECT [ItemTypeID], [QuantityOrdered], [PackagingID], OrderID FROM [OrdersTbl] WHERE (([CustomerId] = ?) AND ([RoastDate] = ?))" 
    UpdateCommand="UPDATE OrdersTbl SET ItemTypeID = ?, QuantityOrdered = ?, PackagingID = ? WHERE (OrderID = ?)">
    <SelectParameters>
      <asp:QueryStringParameter DefaultValue="1" Name="CustomerId" 
        QueryStringField="CustomerID" Type="Int32" />
      <asp:QueryStringParameter Name="RoastDate" QueryStringField="PrepDate" 
        Type="DateTime" />
    </SelectParameters>
    <UpdateParameters>
      <asp:Parameter Name="ItemTypeID" Type="Int32" />
      <asp:Parameter Name="QuantityOrdered" Type="Int32" />
      <asp:Parameter Name="PackagingID" Type="Int32" />
      <asp:Parameter Name="OrderID" Type="Int32" />
    </UpdateParameters>
  </asp:SqlDataSource>
--%>  <asp:SqlDataSource ID="sdsDeliveryBy" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    
    SelectCommand="SELECT [PersonID], [Abreviation] FROM [PersonsTbl] ORDER BY [Enabled], [Abreviation]">
  </asp:SqlDataSource>
  <asp:SqlDataSource ID="sdsItems" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    
    
    SelectCommand="SELECT [ItemTypeID], [ItemDesc] FROM [ItemTypeTbl] ORDER BY [ItemEnabled], [SortOrder], [ItemDesc]">
  </asp:SqlDataSource>
  <asp:SqlDataSource ID="sdsPackagingTypes" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT [PackagingID], [Description] FROM [PackagingTbl] ORDER BY [Description]">
  </asp:SqlDataSource>



  <asp:Label ID="lblCustomerID" Text="" runat="server" />&nbsp;
  <asp:Label ID="lblDeliveryDate" Text="" runat="server" />

  <br />

</asp:Content>
