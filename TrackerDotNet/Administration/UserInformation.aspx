<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserInformation.aspx.cs" Inherits="TrackerDotNet.Administration.UserInformation" %>
<asp:Content ID="cntUserInformationHdr" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="cntUserInformationBdy" ContentPlaceHolderID="MainContent" runat="server">
  <h1>User Information</h1>
  <table class="TblWhite">
    <tr>
      <td><b>User name:</b></td>
      <td><asp:Label id="lblUserName" runat="server" /></td>
      <td rowspan="7" style="background-color:White" >&nbsp;</td>
      <td><b>User's Roles</b></td>
    </tr>
    <tr>
      <td><b>Approved</b></td>
      <td class="horizMiddle" >
        <asp:CheckBox ID="cbxUserIsApproved" Text="" runat="server" TextAlign="Right" 
          oncheckedchanged="cbxUserIsApproved_CheckedChanged" /></td>
      <td rowspan="6" style="clear" class="TblNoBorder" valign="top" ><asp:CheckBoxList CssClass="small" runat="server" ID="UserRolesCheckBoxList" BorderStyle="None"/> </td>
    </tr>
    <tr>
    </tr>
    <tr>
      <td><b>Locked Out</b></td>
      <td class="horizMiddle" >
        <asp:Label ID="lblUserLockedOut" Text="IsUserLockedOut" runat="server" />&nbsp;&nbsp;
        <asp:Button ID="btnUnlockUser" Text="Unlock User" runat="server" 
          onclick="btnUnlockUser_Click" />
       </td>
    </tr>
    <tr>
      <td>Online Status</td>
      <td><asp:Label runat="server" ID="OnlineLabel" Text="" /></td>
    </tr>
    <tr>
      <td>Last Login</td>
      <td><asp:Label runat="server" ID="LastLoginDateLabel" Text="" /></td>
    </tr>
    <tr>
      <td>Comment</td>
      <td><asp:Label runat="server" ID="EmailLabel" Text="" /></td>
    </tr>
    <tr class="rowOddC" >
      <td colspan="4" style="padding: 10px 0 10px 0">
        <asp:Button ID="btnDeleteUser" Text="Delete User" runat="server" 
          onclick="btnDeleteUser_Click" />&nbsp;&nbsp;
        <asp:Button ID="btnUpdate" Text="Update User" runat="server" 
          onclick="btnUpdateUser_Click" />&nbsp;&nbsp;
        <asp:Button ID="btnReturnToManagerUser" Text="Return" runat="server"
        PostBackUrl="~/Administration/ManageUsers.aspx" style="text-align: center" /> </td>
    </tr>
  </table>
  <asp:Label id="lblStatusMessage" Text="" runat="server" />

</asp:Content>
