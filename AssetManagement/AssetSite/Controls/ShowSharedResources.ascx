<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShowSharedResources.ascx.cs" Inherits="AssetSite.Controls.ShowSharedResources" %>
<table>
    <tr>
        <td>
            <asp:TextBox ID="PathResource" runat="server" Enabled="false" Width="300px"></asp:TextBox> <asp:Label ID="Label1" runat="server" Text="(Directories)"></asp:Label>  <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/Go.gif" OnClick="ImageButton1_Click"     />
        </td>
    <tr>
        <td>
            <asp:TreeView ID="ShareCatalogTreeView" runat="server" OnSelectedNodeChanged="ShareCatalogTreeView_SelectedNodeChanged"  >
            </asp:TreeView>
        </td>
        <td>
            &nbsp;</td>
    </tr>

</table>