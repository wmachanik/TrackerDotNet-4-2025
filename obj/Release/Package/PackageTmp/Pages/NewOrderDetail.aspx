<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewOrderDetail.aspx.cs" 
  Inherits="TrackerDotNet.Pages.NewOrderDetail"  MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="cntOrderDetailHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntOrderDetailBdy" ContentPlaceHolderID="MainContent" runat="server">
  <br />
  <asp:ScriptManager runat="server" ID="smgrOrderDetails" />
  <asp:UpdateProgress ID="updprgOrderDetail" runat="server">
    <ProgressTemplate>&nbsp;&nbsp;
      <img src="../images/animi/QuaffeeProgress.gif" alt="please wait..." width="128" height="15" />
    </ProgressTemplate>
  </asp:UpdateProgress>
  <table class="TblSimple">
    <tr>
      <td rowspan="2" style="vertical-align: top" >
        <asp:UpdatePanel ID="upnlOrderSummary" runat="server" UpdateMode="Conditional" >
          <ContentTemplate>
            <table class="TblSimple">
            <tr>
              <td>Company</td>
              <td><asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" AutoPostBack="true"
                      AppendDataBoundItems="true" OnSelectedIndexChanged="ddlContacts_SelectedIndexChanged" >
                    <asp:ListItem Selected="True" Text="----Select name----" Value="0" />
                  </asp:DropDownList>
              </td>
            </tr>
            <tr>
              <td>Order Date</td>
              <td>
                <asp:TextBox ID="tbxOrderDate" runat="server" Text="" AutoPostBack="true"
                  ontextchanged="tbxOrderDate_TextChanged" />
                <ajaxToolkit:CalendarExtender ID="tbxOrderDate_CalendarExtender" runat="server" 
                  Enabled="True" TargetControlID="tbxOrderDate">
                </ajaxToolkit:CalendarExtender>
              </td>
            </tr>
            <tr>
              <td>Roast Date</td>
              <td>
                <asp:TextBox ID="tbxRoastDate" runat="server" Text="" AutoPostBack="true" 
                  ontextchanged="tbxRoastDate_TextChanged" />
                <ajaxToolkit:CalendarExtender ID="tbxRoastDate_CalendarExtender" runat="server" 
                  Enabled="True" TargetControlID="tbxRoastDate">
                </ajaxToolkit:CalendarExtender>
              </td>
            </tr>
            <tr>
              <td>Delivery By</td>
              <td>
                <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" OnDataBound="ddlToBeDeliveredBy_OnDataBound" 
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" 
                      DataValueField="PersonID" AutoPostBack="true" OnSelectedIndexChanged="ddlToBeDeliveredBy_SelectedIndexChanged"  />
              </td>
            </tr>
            <tr>
              <td>Required By</td>
              <td>
                <asp:TextBox ID="tbxRequiredByDate" runat="server" AutoPostBack="true"
                  ontextchanged="tbxRequiredByDate_TextChanged"  />
                <ajaxToolkit:CalendarExtender ID="tbxRequiredByDate_CalendarExtender" 
                  runat="server" Enabled="True" TargetControlID="tbxRequiredByDate">
                </ajaxToolkit:CalendarExtender>
              </td>
            </tr>
            <tr>
              <td>Confirmed</td>
              <td>
                <asp:CheckBox ID="cbxConfirmed" runat="server" Checked="true" AutoPostBack="true"
                  oncheckedchanged="cbxConfirmed_CheckedChanged" />
              </td>
            </tr>
            <tr>
              <td>Done</td>
              <td>
                <asp:CheckBox ID="cbxDone" runat="server" Checked="false" AutoPostBack="true"
                  oncheckedchanged="cbxDone_CheckedChanged" />
              </td>
            </tr>
            <tr>
              <td>Notes:</td>
              <td>
                <asp:TextBox ID="tbxNotes" runat="server" TextMode="MultiLine" Height="4em" AutoPostBack="true"
                  Width="25em" ontextchanged="tbxNotes_TextChanged" />
              </td>
            </tr>
          </table>
          <div style="text-align: center">
            <asp:Button ID="btnUpdate" Text="Update" Visible="false" runat="server" 
              onclick="btnUpdate_Click" />
          </div>
          </ContentTemplate>
          <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlContacts" EventName="SelectedIndexChanged" />
          </Triggers>
        </asp:UpdatePanel>
      </td>
      <td style="vertical-align: top" >
        <asp:UpdatePanel ID="upnlOrderLines" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:GridView ID="gvOrderLines" runat="server" AutoGenerateColumns="False" 
              DataSourceID="odsOrderDetail" OnRowDataBound="gvOrderLines_RowDataBound"
              OnSelectedIndexChanged="gvOrderLines_SelectedIndexChanged" >
              <Columns>
                <asp:CommandField ShowEditButton="True"  ShowDeleteButton="true" 
                  ButtonType="Image" CancelImageUrl="~/images/imgButtons/CancelItem.gif" 
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
                      SelectedValue='<%# Bind("ItemTypeID") %>'>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Qty" 
                  SortExpression="QuantityOrdered">
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
                <asp:TemplateField HeaderText="Order ID">
                  <EditItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' /></EditItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemTemplate>
                    <asp:HyperLink ID="hlDelete" runat="server" Text="del"
                      NavigateUrl='<%# String.Format("~/Pages/DeleteOrderLine.aspx?OrderId={0}", Eval("OrderId")) %>'></asp:HyperLink>
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
              onclick="btnNewItem_Click" />&nbsp;&nbsp;<asp:Literal ID="ltrlStatus" Text="" runat="server" />
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
                          DataTextField="ItemDesc" DataValueField="ItemTypeID" AppendDataBoundItems="true" >
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
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="text-align: center">
        <asp:Button ID="btnAddLastOrder" runat="server" Text="Last Order" onclick="btnAddLastOrder_Click"  />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnDeliverySheet" runat="server" Text="DeliverySheet" PostBackUrl="~/Pages/DeliverySheet.aspx" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnRefreshDetails" Text="Refresh Details" runat="server" 
          onclick="btnRefreshDetails_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnCancelled" Text="Order Cancelled" runat="server" 
          onclick="btnCancelled_Click" />
      </td>
    </tr>
    <tr>
      <td colspan="2" style="text-align: center">
        <a href="NewOrderDetail.aspx">New Order</a>&nbsp;&nbsp;&nbsp;&nbsp;
        <a href="DeliverySheet.aspx">DeliverySheet</a></td>
    </tr>
  </table>

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
      <asp:Parameter Name="OrderID" Type="String" />
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
      <asp:Parameter Name="MaximumRows" Type="Int32" DefaultValue="1" />
      <asp:Parameter Name="StartRowIndex" Type="Int32" DefaultValue="1" />
    </SelectParameters> 
  </asp:ObjectDataSource>

  <asp:SqlDataSource ID="sdsDeliveryBy" runat="server" 
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

  <br /><br />
  <asp:Timer ID="tmrOrderItem" Interval="500" OnTick="tmrOrderItem_OnTick" runat="server" />

</asp:Content>



