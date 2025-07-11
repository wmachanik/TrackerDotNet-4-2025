<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowTableStruct.aspx.cs" Inherits="TrackerDotNet.test.ShowTableStruct" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Table Structure Viewer</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div style="font-family: Calibri; padding: 20px;">
            <h2>Access Table Structure Viewer</h2>
            <br />
            <asp:CheckBox ID="chkFullView" runat="server" AutoPostBack="true"
                Text=" Show full schema view" OnCheckedChanged="chkFullView_CheckedChanged" />
            <asp:DropDownList ID="ddlTables" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTables_SelectedIndexChanged" />
            <br />
            <br />
            <asp:GridView ID="gvStructure" runat="server" AutoGenerateColumns="true" />
        </div>
        <br />

    </form>
</body>
</html>
