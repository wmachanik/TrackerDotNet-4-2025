<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderDone.aspx.cs"
    Inherits="TrackerDotNet.Pages.OrderDone" %>

<asp:Content ID="cntOrderDoneHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntOrderDoneBdy" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="smOrderDone" runat="server" />
    <h2 class="InputFrm">Order Delivered</h2>
    <asp:UpdateProgress ID="updtprgOrderDone" runat="server" AssociatedUpdatePanelID="updtpnlOrderDone">
        <ProgressTemplate>
            <img src="../images/animi/QuaffeeProgress.gif" alt="progress" />
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="updtpnlOrderDone" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:Panel ID="pnlOrderDetails" runat="server">
                <div class="simpleLightBrownForm">
                   <asp:FormView ID="fvOrderDone" runat="server" DataSourceID="sdsOrderDoneHeader" BackColor="#DEBA84"
                        BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="4" CellSpacing="2"
                        GridLines="Both">
                        <EmptyDataTemplate>Please access this page view the Order Detail Page</EmptyDataTemplate>
                        <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                        <ItemTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td class="TblLHCol-first">CompanyName</td>
                                        <td><asp:Label ID="CompanyNameLabel" runat="server" Text='<%# Eval("CompanyName") %>' />&nbsp;
                                            (<asp:Label ID="CustomerIDLabel" runat="server" Text='<%# Eval("CustomerID") %>' />)</td>
                                        <td class="TblLHCol-first">DeliveryDate:</td>
                                        <td><asp:TextBox ID="ByDateTextBox" runat="server" Text='<%# Eval("RequiredByDate", "{0:d}") %>' /></td>
                                    </tr>
                                </tbody>
                            </table>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td class="TblLHCol-first">CompanyName</td>
                                        <td><asp:Label ID="CompanyNameLabel" runat="server" Text='<%# Eval("CompanyName") %>' />&nbsp;
                                              (<asp:Label ID="CustomerIDLabel" runat="server" Text='<%# Eval("CustomerID") %>' />)
                                        </td>
                                        <td class="TblLHCol-first">DeliveryDate:</td>
                                        <td><asp:TextBox ID="ByDateTextBox" runat="server" Text='<%# Bind("RequiredByDate", "{0:d}") %>' /></td>
                                    </tr>
                                </tbody>
                            </table>
                        </EditItemTemplate>
                        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                    </asp:FormView>
                    <div style="padding-top: 2px">
                        <asp:GridView ID="gvOrderDoeLines" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                            DataKeyNames="TOLineID" CssClass="TblWhite" DataSourceID="sdsOrderDoneLines">
                            <Columns>
                                <asp:BoundField DataField="TOLineID" HeaderText="ID" Visible="false" InsertVisible="False"
                                    ReadOnly="True" SortExpression="TOLineID" />
                                <asp:TemplateField HeaderText="Item" SortExpression="ItemID">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItemTypes" DataTextField="ItemDesc"
                                            DataValueField="ItemTypeID" AppendDataBoundItems="True" SelectedValue='<%# Bind("ItemID") %>'>
                                            <asp:ListItem Value="0">n/a</asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlItemDesc" runat="server" DataSourceID="sdsItemTypes" DataTextField="ItemDesc" Enabled="false"
                                            DataValueField="ItemTypeID" AppendDataBoundItems="True" SelectedValue='<%# Eval("ItemID") == null ? "0" : Eval("ItemID").ToString() %>'>
                                            <asp:ListItem Value="0">n/a</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Qty" HeaderText="Quantity" SortExpression="Qty" ItemStyle-HorizontalAlign="Center">
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" Width="4em" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Packaging" SortExpression="PackagingID">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlPackaging" runat="server" AppendDataBoundItems="true" DataSourceID="sdsPackagingTypes"
                                            DataTextField="Description" DataValueField="PackagingID" SelectedValue='<%# Bind("PackagingID")  %>'>
                                            <asp:ListItem Value="0">n/a</asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlPackaging" runat="server" AppendDataBoundItems="true" DataSourceID="sdsPackagingTypes" Enabled="false"
                                            DataTextField="Description" DataValueField="PackagingID" SelectedValue='<%#  Eval("PackagingID") == null ? "0" : Eval("PackagingID").ToString()  %>'>
                                            <asp:ListItem Value="0">n/a</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ButtonType="Button" ShowDeleteButton="True" ShowEditButton="True" HeaderText="Action" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <br />
                <span class="simpleForm" style="float: right; text-align: center; padding: 16px; padding-left: 40px; padding-right: 40px">
                    <asp:Button ID="btnDone" Text="Done" runat="server" AccessKey="D" OnClick="btnDone_Click" /><br
                        style="padding-top: 2px" />
                    <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClick="btnCancel_Click" />
                </span>
                <table class="TblWhite">
                    <tbody>
                        <tr>
                            <td>Stock:</td>
                            <td>
                                <asp:TextBox ID="tbxStock" runat="server" Width="5em" /></td>
                            <td rowspan="2" valign="middle">
                                <asp:RadioButtonList ID="rbtnSendConfirm" runat="server" CssClass="TblWhite">
                                    <asp:ListItem Text="none" Selected="true" Value="none" />
                                    <asp:ListItem Text="send 'its in the post box' message" Value="postbox" />
                                    <asp:ListItem Text="send 'order dispatch' message" Value="dispatched" />
                                    <asp:ListItem Text="send 'order collected' message" Value="collected" />
                                    <asp:ListItem Text="send 'order delivered' message" Value="done" />
                                </asp:RadioButtonList>
                        </tr>
                        <tr>
                            <td>Cup Count:</td>
                            <td>
                                <asp:TextBox ID="tbxCount" runat="server" Width="5em" /></td>

                        </tr>
                    </tbody>
                </table>
                <br />
                <asp:Literal ID="ltrlStatus" Text="" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlCustomerDetailsUpdated" runat="server" Visible="false">
                <table class="TblNoBorder">
                    <thead>
                        <tr>
                            <td>Customer Updated:</td>
                            <td>
                                <asp:Label ID="tbxCustomerName" Text="" runat="server" /></td>
                        </tr>
                    </thead>
                </table>
                <asp:DataGrid ID="dgCustomerUsage" runat="server" CssClass="TblWhite small">
                    <Columns>
                        <asp:BoundColumn DataField="CustomerID" Visible="false" />
                        <asp:BoundColumn DataField="LastCupCount" HeaderText="Last Count" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundColumn DataField="NextCoffeeBy" HeaderText="NextCoffeeBy" DataFormatString="{0:d}" />
                        <asp:BoundColumn DataField="DailyConsumption" HeaderText="DailyConsumption" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                        <asp:BoundColumn DataField="NextCleanOn" HeaderText="NextCleanEst" DataFormatString="{0:d}" />
                        <asp:BoundColumn DataField="CleanAveCount" HeaderText="CleanAveCount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                        <asp:BoundColumn DataField="NextFilterEst" HeaderText="NextFilterEst" DataFormatString="{0:d}" />
                        <asp:BoundColumn DataField="FilterAveCount" HeaderText="FilterAveCount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                        <asp:BoundColumn DataField="NextDescaleEst" HeaderText="NextDescaleEst" DataFormatString="{0:d}" />
                        <asp:BoundColumn DataField="DescaleAveCount" HeaderText="DescaleAveCount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                        <asp:BoundColumn DataField="NextServiceEst" HeaderText="NextServiceEst" DataFormatString="{0:d}" />
                        <asp:BoundColumn DataField="ServiceAveCount" HeaderText="ServiceAveCount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.##}" />
                    </Columns>
                </asp:DataGrid>
                <br />
                <asp:Button ID="btnReturnToDeliveres" Text="Return to Delivery Sheet" AccessKey="D"
                    runat="server" OnClick="btnReturnToDeliveres_Click" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:SqlDataSource ID="sdsOrderDoneHeader" runat="server"
        CancelSelectOnNullParameter="True"
        ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
        SelectCommand="SELECT CustomersTbl.CompanyName, TempOrdersHeaderTbl.CustomerID, TempOrdersHeaderTbl.RequiredByDate FROM TempOrdersHeaderTbl INNER JOIN CustomersTbl ON TempOrdersHeaderTbl.CustomerID = CustomersTbl.CustomerID">
        <SelectParameters>
            <asp:Parameter Name="CustomerID" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsOrderDoneLines" runat="server"
        CancelSelectOnNullParameter="True"
        ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
        SelectCommand="SELECT [ItemID], [Qty], [PackagingID], [TOLineID] FROM [TempOrdersLinesTbl] "
        DeleteCommand="DELETE FROM [TempOrdersLinesTbl] WHERE [TOLineID] = ?"
        InsertCommand="INSERT INTO [TempOrdersLinesTbl] ([ItemID], [Qty], [PackagingID], [TOLineID]) VALUES (?, ?, ?, ?)"
        UpdateCommand="UPDATE [TempOrdersLinesTbl] SET [ItemID] = ?, [Qty] = ?, [PackagingID] = ? WHERE [TOLineID] = ?">
        <DeleteParameters>
            <asp:Parameter Name="TOLineID" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="ItemID" Type="Int32" />
            <asp:Parameter Name="Qty" Type="Single" />
            <asp:Parameter Name="PackagingID" Type="Int32" />
            <asp:Parameter Name="TOLineID" Type="Int32" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="ItemID" Type="Int32" />
            <asp:Parameter Name="Qty" Type="Single" />
            <asp:Parameter Name="PackagingID" Type="Int32" />
            <asp:Parameter Name="TOLineID" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsItemTypes" runat="server"
        ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
        SelectCommand="SELECT [ItemTypeID], [ItemDesc] FROM [ItemTypeTbl] ORDER BY [ItemEnabled], [SortOrder], [ItemDesc]"></asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsPackagingTypes" runat="server"
        ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>"
        SelectCommand="SELECT [PackagingID], [Description] FROM [PackagingTbl] ORDER BY [Description]"></asp:SqlDataSource>

</asp:Content>
