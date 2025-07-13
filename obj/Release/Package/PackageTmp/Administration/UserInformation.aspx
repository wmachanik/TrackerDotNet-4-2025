<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserInformation.aspx.cs" Inherits="TrackerDotNet.Administration.UserInformation" %>
<asp:Content ID="cntUserInformationHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntUserInformationBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>User Information</h1>
  <table class="AutoWidthFrm">
    <tr>
      <td>User name</td>
      <td><asp:Label id="lblUserName" runat="server" /></td>
    </tr>
    <tr class="rowOdd">
      <td>Approved</td>
      <td>
        <asp:CheckBox ID="cbxUserIsApproved" Text="" runat="server" TextAlign="Right" 
          oncheckedchanged="cbxUserIsApproved_CheckedChanged" /></td>
    </tr>
    <tr>
      <td>Locked Out</td>
      <td>
        <asp:Label ID="lblUserLockedOut" Text="IsUserLockedOut" runat="server" />&nbsp;&nbsp;
        <asp:Button ID="btnUnlockUser" Text="Unlock User" runat="server" 
          onclick="btnUnlockUser_Click" />
        </td>
    </tr>
    <tr class="rowOddC" >
      <td colspan="2" style="padding: 10px 0 10px 0">
        <asp:Button ID="btnDeleteUser" Text="Delete User" runat="server" 
          onclick="btnDeleteUser_Click" />&nbsp;&nbsp;
        <asp:Button ID="btnReturnToManagerUser" Text="Back To Manage users" runat="server"
        PostBackUrl="~/Administration/ManageUsers.aspx" style="text-align: center" /> </td>
    </tr>
  </table>
  <asp:Label id="lblStatusMessage" Text="" runat="server" />

</asp:Content>
