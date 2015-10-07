<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultipleAssetsList.ascx.cs" Inherits="AssetSite.Controls.MultipleAssetsList" %>

<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:ListBox ID="lstSelectedAssets" runat="server" Width="300" Rows="4" SelectionMode="Multiple"></asp:ListBox>                    
            <asp:HiddenField ID="hdfAddedItems" runat="server" />
            <asp:HiddenField ID="hdfDeletedItems" runat="server" />
        </td>
        <td style="vertical-align:top;">
            <a id="lbtnSearch" runat="server" style="cursor:pointer; border:0;"><asp:Image runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>&nbsp;
            <div id="DialogContainer" runat="server" meta:resourcekey="DialogContainerRcrs" style="display:none;">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="text-align:center;">
                            <asp:TextBox ID="tbSearchPattern" runat="server" Width="340px"></asp:TextBox>&nbsp;&nbsp;
                            <a runat="server" id="lbtnDoSearch" style="border:0px; cursor:pointer;">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" />
                            </a><br /><br />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:center;">
                            <asp:ListBox ID="lstAssets" runat="server" Width="380px" Height="370px" SelectionMode="Multiple"></asp:ListBox><br />
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>
