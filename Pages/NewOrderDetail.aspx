<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewOrderDetail.aspx.cs" 
  Inherits="TrackerDotNet.Pages.NewOrderDetail"  MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="cntOrderDetailHdr" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="cntOrderDetailBdy" ContentPlaceHolderID="MainContent" runat="server">
  <br />
  <asp:ScriptManager runat="server" ID="smgrOrderDetails" AsyncPostBackTimeout="400" />
  <asp:UpdatePanel ID="upnlNewOrder" runat="server" ChildrenAsTriggers="true">
      <ContentTemplate>
          <table class="TblSimple">
            <tr>
              <td rowspan="2" style="vertical-align: top" >
                <asp:UpdatePanel ID="upnlOrderSummary" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                  <ContentTemplate>
                    <table class="TblSimple">
                    <tr>
                      <td>
                         <asp:HyperLink runat="server" Text="Contact" ID="IDCustomerHdr" 
                           NavigateUrl='<%# Bind("CustomerID") == null ? "." : Bind("CustomerID", "~/Pages/CustomerDetails.aspx?ID={0}") %>' />
                      </td>
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
                          OnTextChanged="tbxRequiredByDate_TextChanged"  />
                        <ajaxToolkit:CalendarExtender ID="tbxRequiredByDate_CalendarExtender" 
                          runat="server" Enabled="True" TargetControlID="tbxRequiredByDate">
                        </ajaxToolkit:CalendarExtender>
                      </td>
                    </tr>
                    <tr>
                      <td>P/Order</td>
                      <td>
                        <asp:TextBox ID="tbxPurchaseOrder" runat="server"  AutoPostBack="true"
                          Width="20em" ontextchanged="tbxPurchaseOrder_TextChanged" />
                      </td>
                    </tr>
                    <tr>
                      <td>Stati</td>
                      <td>
                        <asp:CheckBox ID="cbxConfirmed" TextAlign="Left"  Text="Confirmed" runat="server" Checked="true" AutoPostBack="true"
                          OnCheckedChanged="cbxConfirmed_CheckedChanged" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:CheckBox ID="cbxInvoiceDone" TextAlign="Left" Text="Invoiced" runat="server" Checked="false" AutoPostBack="true"
                          OnCheckedChanged="cbxInvoiceDone_CheckedChanged" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:CheckBox ID="cbxDone" TextAlign="Left"  Text="Done" runat="server" Checked="false" AutoPostBack="true"
                          OnCheckedChanged="cbxDone_CheckedChanged" />
                      </td>
                    </tr>
                    <tr>
                      <td>Notes:</td>
                      <td>
                        <asp:TextBox ID="tbxNotes" runat="server" TextMode="MultiLine" Height="4em" AutoPostBack="true"
                          Width="98%" ontextchanged="tbxNotes_TextChanged" />
                      </td>
                    </tr>
                  </table>
                  <div style="text-align: center">
                    <asp:Button ID="btnUpdate" Text="Update" Visible="false" runat="server" 
                      OnClick="btnUpdate_Click" />
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
                      OnSelectedIndexChanged="gvOrderLines_SelectedIndexChanged" EmptyDataText="Please add new items"
                      OnRowUpdated="gvOrderLines_RowUpdated" OnRowCommand="gvOrderLines_RowCommand">
                      <Columns>
                        <asp:CommandField ShowEditButton="True"  ShowDeleteButton="false" 
                          ButtonType="Image" CancelImageUrl="~/images/imgButtons/CancelItem.gif" 
                          EditImageUrl="~/images/imgButtons/EditItem.gif" 
                          UpdateImageUrl="~/images/imgButtons/UpdateItem.gif" InsertVisible="False" />
                        <asp:TemplateField HeaderText="Item Type" SortExpression="ItemTypeID">
                          <EditItemTemplate>
                            <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItems" 
                              DataTextField="ItemDesc" DataValueField="ItemTypeID"  
                              AppendDataBoundItems="true"
                              SelectedValue='<%# Bind("ItemTypeID") == null ? "0" : Bind("ItemTypeID") %>'>
                            <asp:ListItem Text="--Invalid Item--" Value="0" />
                            </asp:DropDownList>
                          </EditItemTemplate>
                          <ItemTemplate>
                            <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItems" 
                              DataTextField="ItemDesc" DataValueField="ItemTypeID" Enabled="false"
                              AppendDataBoundItems="true"
                              SelectedValue='<%# Bind("ItemTypeID") == null ? "0" : Bind("ItemTypeID") %>'>
                            <asp:ListItem Text="--Invalid Item--" Value="0" />
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
                              SelectedValue='<%# Bind("PackagingID") == null ? "0" : Bind("PackagingID")  %>'>
                              <asp:ListItem Text="n/a" Value="0" ></asp:ListItem>
                            </asp:DropDownList>
                          </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                          <ItemTemplate>
                            <asp:ImageButton runat="server" ID="btnDelete" CommandArgument='<%# Eval("OrderId") %>'
                                ImageUrl="~/images/imgButtons/DelItem.gif" AlternateText="X" CommandName="DeleteItem" />
        <%--                    <asp:HyperLink ID="hlDelete" runat="server" Text="del" ImageUrl="~/images/imgButtons/DelItem.gif" 
                              NavigateUrl='<%# String.Format("~/Pages/DeleteOrderLine.aspx?OrderId={0}", Eval("OrderId")) %>'></asp:HyperLink>
          --%>
                          </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ID">
                          <EditItemTemplate>
                            <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' Font-Size="XX-Small" /></EditItemTemplate>
                          <ItemTemplate>
                            <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' Font-Size="XX-Small" />
                          </ItemTemplate>
                        </asp:TemplateField>
                      </Columns>
                      <EmptyDataTemplate>
                        Add a new order
                      </EmptyDataTemplate>
                    </asp:GridView>
                  </ContentTemplate>
                  <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvOrderLines" EventName="RowUpdated" />
                  </Triggers>
                </asp:UpdatePanel>
              </td>
            </tr>
            <tr>
              <td> <%--- New item --%>
                <asp:UpdatePanel ID="upnlNewOrderItem" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                  <ContentTemplate>
                    <asp:Button ID="btnNewItem" Text="New Item" runat="server" Enabled="false" AccessKey="I" ToolTip="add a new Item (AltShftI)"
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
                                <asp:TextBox ID="tbxNewQuantityOrdered" runat="server" Text='0' />
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
                        <asp:Button ID="btnAdd" Text="Add" runat="server" Visible="false" AccessKey="A" ToolTip="Add a Item (AltShftA)"
                          onclick="btnAdd_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btnCancel" Text="Cancel" runat="server" Visible="false" AccessKey="X" ToolTip="cancel (AltShftX)"
                          onclick="btnCancel_Click" />
                    </asp:Panel>
                  </ContentTemplate>
                </asp:UpdatePanel>
              </td>
            </tr>
            <tr>
              <td colspan="2" style="text-align: center">
                <asp:UpdatePanel ID="updtButtonPanel" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                    <asp:Button ID="btnAddLastOrder" runat="server" Text="Last Order" Enabled="false" onclick="btnAddLastOrder_Click" AccessKey="A" ToolTip="Add a Item (AltShftL)" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnDeliverySheet" runat="server" Text="DeliverySheet" PostBackUrl="~/Pages/DeliverySheet.aspx" AccessKey="S" ToolTip="delivery Sheet (AltShftS)" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCheckDetails" Text="Check Details" runat="server" Enabled="false" AccessKey="K" ToolTip="checK details (AltShftK)" 
                      onclick="btnCheckDetails_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnRefreshDetails" Text="Refresh Details" runat="server" Enabled="false" AccessKey="R" ToolTip="refresh lists (AltShftR)"
                      onclick="btnRefreshDetails_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancelled" Text="Order Cancelled" runat="server" onclick="btnCancelled_Click" />
                  </ContentTemplate>
                </asp:UpdatePanel>
              </td>
            </tr>
          </table>

    </ContentTemplate>
  </asp:UpdatePanel>

   <asp:UpdateProgress ID="updprgOrderDetail" runat="server" DisplayAfter="100" AssociatedUpdatePanelID="upnlNewOrder" >
    <ProgressTemplate>&nbsp;&nbsp;
      <img src="../images/animi/QuaffeeProgress.gif" alt="please wait..." />
      <input type="button" id="btnCancel" value="Cancel"  onclick="CancelAsyncPostBack()" />
    </ProgressTemplate>
  </asp:UpdateProgress>

<script type="text/javascript">
<!-- 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    function CancelAsyncPostBack() {
        prm.abortPostBack();
    }
    // -->
</script>

  <asp:SqlDataSource ID="sdsCompanys" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT IIF([enabled], [CompanyName], '_' + [CompanyName]) As CompanyName, [CustomerID] FROM [CustomersTbl] ORDER BY [enabled], [CompanyName]">
  </asp:SqlDataSource>

  <asp:ObjectDataSource ID="odsOrderDetail" runat="server" 
    TypeName="TrackerDotNet.control.OrderDetailDAL" SelectMethod="LoadOrderDetailData"
    UpdateMethod="UpdateOrderDetails" 
    StartRowIndexParameterName="StartRowIndex" 
    MaximumRowsParameterName="MaximumRows" 
    OldValuesParameterFormatString="original_{0}" 
    DeleteMethod="DeleteOrderDetails" >
    <DeleteParameters>
      <asp:Parameter Name="OrderID" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
      <asp:Parameter Name="OrderID" Type="Int64" />
      <asp:SessionParameter DefaultValue="-1" Name="CustomerID" 
        SessionField="BoundCustomerID" Type="Int64" />
      <asp:Parameter Name="ItemTypeID" Type="Int32" />
      <asp:SessionParameter DefaultValue="" Name="DeliveryDate" 
        SessionField="BoundDeliveryDate" Type="DateTime" />
      <asp:Parameter Name="QuantityOrdered" Type="Double" />
      <asp:Parameter Name="PackagingID" Type="Int32" />
    </UpdateParameters>
    <SelectParameters>
      <asp:SessionParameter DefaultValue="1" Name="CustomerID" 
        SessionField="BoundCustomerID" Type="Int64" />
      <asp:SessionParameter DefaultValue="" Name="DeliveryDate" 
        SessionField="BoundDeliveryDate" Type="DateTime" />
      <asp:SessionParameter DefaultValue="&quot;&quot;" Name="Notes" 
        SessionField="BoundNotes" Type="String" />
      <asp:Parameter Name="MaximumRows" Type="Int32" DefaultValue="1" />
      <asp:Parameter Name="StartRowIndex" Type="Int32" DefaultValue="1" />
    </SelectParameters> 
  </asp:ObjectDataSource>

  <br />
  <asp:SqlDataSource ID="sdsDeliveryBy" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    
    SelectCommand="SELECT [PersonID], [Abreviation] FROM [PersonsTbl] ORDER BY [Enabled], [Abreviation]">
  </asp:SqlDataSource>
  <asp:SqlDataSource ID="sdsItems" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    
    
    SelectCommand ="SELECT ItemTypeID, IIF(ItemEnabled, ItemDesc, '_' + ItemDesc) AS ItemDesc FROM ItemTypeTbl ORDER BY ItemEnabled, SortOrder, ItemDesc">
  </asp:SqlDataSource>
 <%--   SelectCommand="SELECT [ItemTypeID], [ItemDesc] FROM [ItemTypeTbl] ORDER BY [ItemEnabled], [SortOrder], [ItemDesc]"> --%>
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



