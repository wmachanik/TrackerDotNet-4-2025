<%@ Page Title="QOnt Ssytem Tools" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
  CodeBehind="SystemTools.aspx.cs" Inherits="TrackerDotNet.Tools.SystemTools" %>
  <asp:Content ID="cntSystemToolsHdr" ContentPlaceHolderID="HeadContent" runat="server">
  </asp:Content>
  <asp:Content ID="cntSystemToolsBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>General System Tools</h1>
  <br />
  <ajaxToolkit:ToolkitScriptManager ID="tsmSystemTools" runat="server"></ajaxToolkit:ToolkitScriptManager>
    <asp:UpdateProgress ID="uprgSystemTools" runat="server" AssociatedUpdatePanelID="upnlSystemToolsButtons" EnableViewState="true" Visible="true" >
      <ProgressTemplate>
        <img src="../images/animi/BlueArrowsUpdate.gif" alt="updating" width="16" height="16" />updating.....
      </ProgressTemplate>
    </asp:UpdateProgress>
  <asp:UpdatePanel ID="upnlSystemToolsButtons" runat="server" ChildrenAsTriggers="true"  
    UpdateMode="Always" ViewStateMode="Enabled">
    <ContentTemplate>
      <table class="TblWhite" width="100%">
        <tbody>
          <tr>
            <td><asp:Button ID="btnSetClientType" runat="server" Text="Set Client Type" 
                onclick="btnSetClientType_Click" /></td>
            <td><asp:Button ID="btnXMLTOSQL" runat="server" PostBackUrl="~/Tools/XMLtoSQL.aspx" Text="XML file to SQL" /> </td>
          </tr>
          <tr>
            <td><asp:Button ID="btnResetPrepDates" runat="server"  
                Text="Reset Prep/Delivery Date" onclick="btnResetPrepDates_Click" /></td>
            <td><asp:Button ID="btnMoveDlvryDate" runat="server" PostBackUrl="~/Tools/MoveDeliveryDate.aspx" Text="Move Delivery Date" /> </td>          
          </tr>
          <tr>
            <td><asp:Button ID="btnEditSystemData" runat="server" PostBackUrl="~/Tools/SystemData.aspx" Text="System Data" /> </td>          
            <td><asp:Button ID="btnCreateUpdateLogTables" runat="server" Text="Create/Update Log Tables"
                OnClick="btnCreateUpdateLogTables_Click" /></td>
          </tr>
          <tr>
            <td><asp:Button ID="btnMergQBAccData" runat="server" PostBackUrl="~/Tools/MergeCustomersFromQB.aspx" Text="Merge Customers From QB Data" /> </td>          
            <td>&nbsp;</td>
          </tr>
        </tbody>
      </table>
      <br />
      <asp:Literal ID="ltrlStatus" Text="" runat="server" Visible="false" />
      <br /><br />
      <asp:Panel ID="pnlSetClinetType" runat="server" Visible="false">
        <table class="TblWhite" width="100%">
          <tr valign="top">
            <td>
              <asp:Label ID="ResultsTitleLabel"  runat="server" CssClass="title" Text="" />
              <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="true" AllowSorting="true" CssClass="TblCoffee" >
              </asp:GridView>
            </td>
            <td>
              <asp:GridView ID="gvCustomerTypes" runat="server" AllowSorting="True" CssClass="TblZebra"
                AutoGenerateColumns="False" DataSourceID="odsCustomerTypes" Visible="false">
                <Columns>
                  <asp:BoundField DataField="CustTypeID" HeaderText="CustTypeID" 
                    SortExpression="CustTypeID" />
                  <asp:BoundField DataField="CustTypeDesc" HeaderText="CustTypeDesc" 
                    SortExpression="CustTypeDesc" />
                  <asp:BoundField DataField="Notes" HeaderText="Notes" SortExpression="Notes" />
                </Columns>
              </asp:GridView>
                <asp:ObjectDataSource ID="odsCustomerTypes" runat="server" SortParameterName="SortBy"
                    SelectMethod="GetAll" TypeName="TrackerDotNet.control.CustomerTypeTbl">
                </asp:ObjectDataSource>
            </td>
          </tr>
        </table>
      </asp:Panel>
      <asp:Panel ID="pnlResetPrepDate" runat="server" Visible="false">
        
        <asp:GridView ID="gvCityPrepDates" runat="server" AllowPaging="True" CssClass="TblDetailZebra" 
          AutoGenerateColumns="False" DataSourceID="sdsCityPrepDates">
          <Columns>
            <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
            <asp:BoundField DataField="PreperationDate" DataFormatString="{0:d}" 
              HeaderText="PreperationDate" SortExpression="PreperationDate" />
            <asp:BoundField DataField="DeliveryDate" DataFormatString="{0:d}" 
              HeaderText="DeliveryDate" SortExpression="DeliveryDate" />
            <asp:BoundField DataField="NextPreperationDate" DataFormatString="{0:d}" 
              HeaderText="NextPreperationDate" SortExpression="NextPreperationDate" />
            <asp:BoundField DataField="NextDeliveryDate" DataFormatString="{0:d}" 
              HeaderText="NextDeliveryDate" SortExpression="NextDeliveryDate" />
          </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="sdsCityPrepDates" runat="server" 
          ConnectionString="<%$ ConnectionStrings:Tracker08ConnectionString %>" 
          ProviderName="<%$ ConnectionStrings:Tracker08ConnectionString.ProviderName %>" 
          SelectCommand="SELECT CityTbl.City, NextRoastDateByCityTbl.PreperationDate, NextRoastDateByCityTbl.DeliveryDate, NextRoastDateByCityTbl.NextPreperationDate, NextRoastDateByCityTbl.NextDeliveryDate FROM (NextRoastDateByCityTbl LEFT OUTER JOIN CityTbl ON NextRoastDateByCityTbl.CityID = CityTbl.ID) ORDER BY CityTbl.City">
        </asp:SqlDataSource>
        
      </asp:Panel>
    </ContentTemplate>
    <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnSetClientType" EventName="Click" />
      <asp:AsyncPostBackTrigger ControlID="gvResults" EventName="DataBound" />
    </Triggers>
  </asp:UpdatePanel>

</asp:Content>
