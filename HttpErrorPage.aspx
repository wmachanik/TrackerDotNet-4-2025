<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HttpErrorPage.aspx.cs" Inherits="TrackerDotNet.HttpErrorPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error Page</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        .error-message { color: red; font-weight: bold; }
    </style>
</head>
<body>
    <form id="frmErrorPage" runat="server">
        <div>
            <h1>An error occurred in Web App</h1>
            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message"></asp:Label>
        </div>
    </form>
</body>
</html>
