<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="DevelopmentInformationService.Account.Manage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<table border="0" cellpadding="0" cellspacing="0" width="250px">
    <tr>
        <td>Version:</td>
        <td>
            <asp:TextBox ID="tbVersion" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="text-align:left;">
            <br />
            <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
        </td>
    </tr>
</table>
</asp:Content>
