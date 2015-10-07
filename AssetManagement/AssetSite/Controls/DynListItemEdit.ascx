<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynListItemEdit.ascx.cs" Inherits="AssetSite.Controls.DynListItemEdit" %>

<div id="DynListItemDataContainer" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <a id="lbtnItemName" runat="server" style=" text-decoration:none;"></a>
            </td>
            <td style="text-align:right;">
                <a id="lbtnRemoveItem" runat="server" style="cursor:pointer; border:0px;">
                    <asp:Image runat="server" ImageUrl="/images/buttons/delete.png" />
                </a>
            </td>
        </tr>
    </table>
</div>
