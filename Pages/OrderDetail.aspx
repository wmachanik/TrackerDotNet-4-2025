  <%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="OrderDetail.aspx.cs" 
  Inherits="TrackerDotNet.Pages.OrderDetail"  MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="cntOrderDetailHdr" ContentPlaceHolderID="HeadContent" runat="server">
  <script type="text/javascript">
    function ChangeUrl(title, url) {
      if (typeof (history.pushState) != "undefined") {
        var obj = { Title: title, Url: url };
        history.pushState(obj, obj.Title, obj.Url);
      }
      else
      {
        alert("Browser does not support HTML5.");
      }
    }
  </script>
</asp:Content>
<asp:Content ID="cntOrderDetailBdy" ContentPlaceHolderID="MainContent" runat="server">
  <br />
  <asp:ScriptManager runat="server" ID="scrmOrderDetail" />
  <asp:UpdateProgress ID="udtpOrderDetail" runat="server">
    <ProgressTemplate>&nbsp;&nbsp;
      <img src="../images/animi/QuaffeeProgress.gif" alt="please wait..." />
    </ProgressTemplate>
  </asp:UpdateProgress>
  <table class="TblSimple">
    <tr>
      <td rowspan="2" style="vertical-align: top">
        <asp:UpdatePanel ID="pnlOrderHeader" runat="server" UpdateMode="Conditional"  >
          <ContentTemplate>
            <asp:DetailsView ID="dvOrderHeader" runat="server" DataSourceID="odsOrderSummary" 
              AutoGenerateRows="False" CssClass="TblWhite"  OnItemUpdated="dvOrderHeader_OnItemUpdated"
              OnModeChanging="dvOrderHeader_OnModeChanging"  OnDataBound="dvOrderHeader_OnDataBound" >
              <EmptyDataTemplate>Return to <a href="DeliverySheet.aspx">delivery sheet...</a></EmptyDataTemplate>
              <Fields>
                <asp:TemplateField>
                  <HeaderTemplate>
                    <asp:HyperLink runat="server" Text="Contact" ID="IDCustomerHdr" NavigateUrl='<%# Bind("CustomerID") == null ? "" : Bind("CustomerID", "~/Pages/CustomerDetails.aspx?ID={0}") %>' />
                  </HeaderTemplate> 
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") == null ? "0" : Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") == null ? "0" : Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlContacts" runat="server" DataSourceID="sdsCompanys" 
                      DataTextField="CompanyName" DataValueField="CustomerID" Enabled="false" AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("CustomerID") == null ? "0" : Bind("CustomerID") %>'>
                      <asp:ListItem Value="0">none</asp:ListItem>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Order Date" SortExpression="OrderDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxOrderDate" runat="server" Text='<%# Bind("OrderDate", "{0:yyyy-MM-dd}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxOrderDate_CalendarExtender" runat="server" 
                      Enabled="True" TargetControlID="tbxOrderDate" >
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxOrderDate" runat="server" 
                      Text='<%# Bind("OrderDate", "{0:yyyy-MM-dd}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblOrderDate" runat="server" Text='<%# Bind("OrderDate", "{0:d MMM, ddd, yyyy}") %>' />
                  </ItemTemplate>
                  <ItemStyle Font-Bold="True" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Roast Date" SortExpression="RoastDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxRoastDate" runat="server" Text='<%# Bind("RoastDate", "{0:yyyy-MM-dd}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxRoastDate_CalendarExtender" runat="server" 
                      Enabled="True" TargetControlID="tbxRoastDate">
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxRoastDate" runat="server" 
                      Text='<%# Bind("RoastDate", "{0:yyyy-MM-dd}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblRoastDate" runat="server" Text='<%# Bind("RoastDate", "{0:d MMM, ddd, yyyy}") %>'></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Delivery By" SortExpression="ToBeDeliveredBy">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" DataValueField="PersonID" 
                      SelectedValue='<%# Bind("ToBeDeliveredBy") == null ? "0" : Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" DataValueField="PersonID" 
                      SelectedValue='<%# Bind("ToBeDeliveredBy") == null ? "0" : Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>
                    </asp:DropDownList>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlToBeDeliveredBy" runat="server" Enabled="False" AppendDataBoundItems="true"
                      DataSourceID="sdsDeliveryBy" DataTextField="Abreviation" DataValueField="PersonID" 
                      SelectedValue='<%# Bind("ToBeDeliveredBy") == null ? "0" : Bind("ToBeDeliveredBy") %>'>
                      <asp:ListItem Value="0">n/a</asp:ListItem>
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Required Date" SortExpression="RequiredByDate">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxRequiredByDate" runat="server" Text='<%# Bind("RequiredByDate", "{0:yyyy-MM-dd}" ) %>' />
                    <ajaxToolkit:CalendarExtender ID="tbxRequiredByDate_CalendarExtender" 
                      runat="server" Enabled="True" TargetControlID="tbxRequiredByDate">
                    </ajaxToolkit:CalendarExtender>
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxRequiredByDate" runat="server" 
                      Text='<%# Bind("RequiredByDate", "{0:yyyy-MM-dd}") %>'></asp:TextBox>
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblRequiredByDate" runat="server" 
                      Text='<%# Bind("RequiredByDate", "{0:d MMM, ddd, yyyy }") %>'></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Purchase Order" SortExpression="PurchaseOrder">
                  <EditItemTemplate>
                     <asp:TextBox ID="tbxPurchaseOrder" runat="server" Text='<%# Bind("PurchaseOrder") %>' />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                     <asp:TextBox ID="tbxPurchaseOrder" runat="server" Text='<%# Bind("PurchaseOrder") %>' />
                  </InsertItemTemplate>
                  <ItemTemplate>
                     <asp:Label ID="lblPurchaseOrder" runat="server" Text='<%# Bind("PurchaseOrder") %>' />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Stati" SortExpression="Confirmed">
                  <EditItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" TextAlign="Left" Text="Confirmed" runat="server" Checked='<%# Bind("Confirmed") %>' />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxInvoiceDone" TextAlign="Left"  Text="Invoiced" runat="server" Checked='<%# Bind("InvoiceDone") %>' />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxDone" TextAlign="Left"  Text="Complete" runat="server" Checked='<%# Bind("Done") %>' Enabled="false" />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" TextAlign="Left"  Text="Confirmed" runat="server" Checked='<%# Bind("Confirmed") %>' />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxInvoiceDone" TextAlign="Left"  Text="Invoiced" runat="server" Checked='<%# Bind("InvoiceDone") %>'  />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxDone" TextAlign="Left"  Text="Complete" runat="server" Checked='<%# Bind("Done") %>'  />
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:CheckBox ID="cbxConfirmed" TextAlign="Left" Text="Confirmed" runat="server" Checked='<%# Bind("Confirmed") %>' Enabled="false" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxInvoiceDone" TextAlign="Left"  Text="Invoiced" runat="server" Checked='<%# Bind("InvoiceDone") %>' Enabled="false" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbxDone" TextAlign="Left" Text="Complete" runat="server" Checked='<%# Bind("Done") %>' Enabled="false" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Notes" SortExpression="Notes">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxNotes" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine" width="95%" />
                  </EditItemTemplate>
                  <InsertItemTemplate>
                    <asp:TextBox ID="tbxNotes" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine" />
                  </InsertItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblNotes" runat="server" Text='<%# Bind("Notes") %>' />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ControlStyle-CssClass="padded" ShowEditButton="True" ButtonType="Image"  EditImageUrl="~/images/imgButtons/EditButton.gif"
                    UpdateImageUrl="~/images/imgButtons/UpdateButton.gif" CancelImageUrl="~/images/imgButtons/CancelButton.gif" />
              </Fields>
            </asp:DetailsView>
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
      <td style="vertical-align: top" >
        <asp:UpdatePanel ID="upnlOrderLines" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
          <ContentTemplate>
            <asp:GridView ID="gvOrderLines" runat="server" AutoGenerateColumns="False" 
              OnRowUpdated="gvOrderLines_RowUpdated" OnRowCommand="gvOrderLines_RowCommand"
              DataSourceID="odsOrderDetail" EmptyDataText="NO ITEMS ADDED">
              <Columns>
                <asp:CommandField ShowEditButton="true"  ShowDeleteButton="false" 
                  ButtonType="Image" CancelImageUrl="~/images/imgButtons/CancelItem.gif" 
                  DeleteImageUrl="~/images/imgButtons/DelItem.gif" 
                  EditImageUrl="~/images/imgButtons/EditItem.gif"  CausesValidation="false"
                  UpdateImageUrl="~/images/imgButtons/UpdateItem.gif" InsertVisible="False"
                  EditText="Edit" UpdateText="go" CancelText="no"  />
                <asp:TemplateField HeaderText="Item Type" SortExpression="ItemTypeID">
                  <EditItemTemplate>
                    <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="odsItemTypes" 
                      DataTextField="ItemDesc" DataValueField="ItemTypeID"  
                      SelectedValue='<%# Bind("ItemTypeID") == null ? "0" : Bind("ItemTypeID") %>'>
                    <asp:ListItem Text="--Invalid Item--" Value="0" />
                    </asp:DropDownList>
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="odsItemTypes" 
                      DataTextField="ItemDesc" DataValueField="ItemTypeID" Enabled="false"
                      OnSelectedIndexChanged="ddlItemDesc_SelectedIndexChanged" 
                      AppendDataBoundItems="true"
                      SelectedValue='<%# Bind("ItemTypeID") == null ? "0" : Bind("ItemTypeID") %>'>
                      <asp:ListItem Text="--Invalid Item--" Value="0" />
                    </asp:DropDownList>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="QTY" SortExpression="QuantityOrdered">
                  <EditItemTemplate>
                    <asp:TextBox ID="tbxQuantityOrdered" runat="server" Text='<%# Bind("QuantityOrdered") %>' Width="2em" />
                    <asp:Label ID="lblItemUoM" runat="server" Text='<%# GetItemUoMObj(Eval("ItemTypeID")) %>' CssClass="small" />
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:Label ID="lblQuantityOrdered" runat="server" Text='<%# Bind("QuantityOrdered") %>' />
                    <asp:Label ID="lblItemUoM" runat="server" Text='<%# GetItemUoMObj(Eval("ItemTypeID")) %>' CssClass="small" />
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
                    <asp:ImageButton ID="MoveOneDayOnImageButton" AlternateText="+date" CommandName="MoveOneDayOn" ImageUrl="~/images/imgButtons/MoveOnADay.gif"
                        runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                        ToolTip="Move this item to next week day"  />
                    <asp:ImageButton ID="DeleteItemImageButton" AlternateText="del" CommandName="DeleteOrder" ImageUrl="~/images/imgButtons/DelItem.gif"
                        OnClientClick="return confirm('Are you sure you want to delete this order item?');"
                        runat="server" CommandArgument='<%# (Eval("OrderID") != null) ? Eval("OrderID") : 0 %>'
                        ToolTip="delete this"  />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID">
                  <EditItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Bind("OrderID") %>' Font-Size="XX-Small" />
                  </EditItemTemplate>
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
            <asp:AsyncPostBackTrigger ControlID="gvOrderLines" EventName="RowEditing" />
          </Triggers>
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
                        <asp:DropDownList ID="ddlNewItemDesc" runat="server" DataSourceID="odsItemTypes" 
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
    <tr class="horizMiddle">
      <td colspan="2" style="padding: 8px; text-align: center">
        <asp:UpdatePanel ID="updtButtonPanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:Button ID="btnNewOrder" runat="server" Text="New Order" AccessKey="N" PostBackUrl="~/Pages/NewOrderDetail.aspx" ToolTip="new order (AltShftN)" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnConfirmOrder" runat="server" Text="Email Confirmation" AccessKey="E" onclick="btnConfirmOrder_Click" ToolTip="send Email confirmation (AltShftE)" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDlSheet" runat="server" Text="Delivery Sheet" PostBackUrl="~/Pages/DeliverySheet.aspx" AccessKey="S" ToolTip="new order (AltShftS)" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnOrderCancelled" runat="server" onclick="btnCancelled_Click" Text="Cancel Order" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnUnDoDone" runat="server" Text="UnDo Done" AccessKey="U" onclick="btnUnDoDone_Click" ToolTip="undo a done order (AltShftU)"  />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnOrderDelivered" runat="server" Text="Order Done" AccessKey="D" onclick="btnOrderDelivered_Click" ToolTip="start order done process (AltShftD)" />
          </ContentTemplate>
        </asp:UpdatePanel>
      </td>
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
          <asp:SessionParameter DefaultValue="-1" Name="CustomerID" SessionField="BoundCustomerID" Type="Int64" />
          <asp:SessionParameter DefaultValue="" Name="DeliveryDate" SessionField="BoundDeliveryDate" Type="DateTime" />
          <asp:SessionParameter DefaultValue="&quot;&quot;" Name="Notes" SessionField="BoundNotes" Type="String" />
          <asp:Parameter Name="MaximumRows" Type="Int32" />
          <asp:Parameter Name="StartRowIndex" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
          <asp:Parameter Name="CustomerID" Type="Int64" />
          <asp:Parameter Name="OrderDate" Type="DateTime" />
          <asp:Parameter Name="RoastDate" Type="DateTime" />
          <asp:Parameter Name="ToBeDeliveredBy" Type="Int32" />
          <asp:Parameter Name="RequiredByDate" Type="DateTime" />
          <asp:Parameter Name="Confirmed" Type="Boolean" />
          <asp:Parameter Name="Done" Type="Boolean" />
          <asp:Parameter Name="InvoiceDone" Type="Boolean" />
          <asp:Parameter Name="PurchaseOrder" Type="String" />
          <asp:Parameter Name="Notes" Type="String" />
          <asp:SessionParameter DefaultValue="-1" Name="OriginalCustomerID" SessionField="BoundCustomerID" Type="Int64" />
          <asp:SessionParameter DefaultValue="" Name="OriginalDeliveryDate" SessionField="BoundDeliveryDate" Type="DateTime" />
          <asp:SessionParameter DefaultValue="&quot;&quot;" Name="OriginalNotes" SessionField="BoundNotes" Type="String" />
        </UpdateParameters>
      </asp:ObjectDataSource>
  <%--
SelecT:
          <asp:QueryStringParameter DefaultValue="1" Name="CustomerID" QueryStringField="CustomerID" Type="Int64" />
          <asp:QueryStringParameter Name="DeliveryDate" QueryStringField="DeliveryDate" Type="DateTime" />
          <asp:QueryStringParameter Name="Notes" QueryStringField="Notes" Type="String" />
Update:
          <asp:QueryStringParameter DefaultValue="1" Name="OriginalCustomerID" QueryStringField="CustomerID" Type="Int64" />
          <asp:QueryStringParameter Name="OriginalDeliveryDate" QueryStringField="DeliveryDate" Type="DateTime" />
          <asp:QueryStringParameter Name="OriginalNotes" QueryStringField="Notes" Type="String" />


--%>
  <asp:SqlDataSource ID="sdsCompanys" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT [CompanyName], [CustomerID] FROM [CustomersTbl] ORDER BY [enabled], [CompanyName]">
  </asp:SqlDataSource>
  <asp:ObjectDataSource ID="odsOrderDetail" runat="server" 
    TypeName="TrackerDotNet.control.OrderDetailDAL" SelectMethod="LoadOrderDetailData"
    UpdateMethod="UpdateOrderDetails" 
    StartRowIndexParameterName="StartRowIndex" 
    MaximumRowsParameterName="MaximumRows" 
    OldValuesParameterFormatString="original_{0}" 
    DeleteMethod="DeleteOrderDetails" >
    <DeleteParameters>
      <asp:Parameter Name="OrderID" Type="Int64" />
    </DeleteParameters>
    <UpdateParameters>
      <asp:Parameter Name="OrderID" Type="Int64" />
      <asp:SessionParameter DefaultValue="-1" Name="CustomerID" SessionField="BoundCustomerID" Type="Int64" />
      <asp:Parameter Name="ItemTypeID" Type="Int32" />
      <asp:SessionParameter DefaultValue="" Name="DeliveryDate" SessionField="BoundDeliveryDate" Type="DateTime" />
      <asp:Parameter Name="QuantityOrdered" Type="Double" />
      <asp:Parameter Name="PackagingID" Type="Int32" />
    </UpdateParameters>
    <SelectParameters>
      <asp:SessionParameter DefaultValue="1" Name="CustomerID" SessionField="BoundCustomerID" Type="Int64" />
      <asp:SessionParameter DefaultValue="" Name="DeliveryDate" SessionField="BoundDeliveryDate" Type="DateTime" />
      <asp:SessionParameter DefaultValue="&quot;&quot;" Name="Notes" SessionField="BoundNotes" Type="String" />
      <asp:Parameter Name="MaximumRows" Type="Int32" />
      <asp:Parameter Name="StartRowIndex" Type="Int32" />
    </SelectParameters> 
  </asp:ObjectDataSource>

  <br />

  <asp:SqlDataSource ID="sdsDeliveryBy" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"     
    SelectCommand="SELECT [PersonID], [Abreviation] FROM [PersonsTbl] ORDER BY [Enabled], [Abreviation]">
  </asp:SqlDataSource>
  <asp:ObjectDataSource ID="odsItemTypes" runat="server" SelectMethod="GetAllItemDesc" TypeName="TrackerDotNet.control.ItemTypeTbl" />

 <%--   SelectCommand="SELECT [ItemTypeID], [ItemDesc] FROM [ItemTypeTbl] ORDER BY [ItemEnabled], [SortOrder], [ItemDesc]"> --%>
  <asp:SqlDataSource ID="sdsPackagingTypes" runat="server" 
    ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
    ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
    SelectCommand="SELECT [PackagingID], [Description] FROM [PackagingTbl] ORDER BY [Description]">
  </asp:SqlDataSource>

<%--
  <asp:Label ID="lblCustomerID" Text="" runat="server" />&nbsp;
  <asp:Label ID="lblDeliveryDate" Text="" runat="server" />

  <br />
--%>

</asp:Content>
