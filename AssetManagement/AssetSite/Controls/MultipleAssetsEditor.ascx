<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultipleAssetsEditor.ascx.cs" Inherits="AssetSite.Controls.MultipleAssetsEditor" %>

<asp:MultiView ID="mvMultipleAssets" runat="server">
    <asp:View ID="viewView" runat="server">
        <asp:Panel ID="pnlLinks" runat="server">
        </asp:Panel>
    </asp:View>
    <asp:View ID="viewEdit" runat="server">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:ListBox ID="lstSelectedAssets" runat="server" Width="300" Rows="4" SelectionMode="Multiple"></asp:ListBox>                    
                    <asp:HiddenField ID="hdfAddedItems" runat="server" />
                    <asp:HiddenField ID="hdfDeletedItems" runat="server" />
                </td>
                <td style="vertical-align:top;">
                    <a id="lbtnSearch" runat="server" style="cursor:pointer; border:0;"><asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>&nbsp;
                    <asp:HyperLink ID="lnkAdd" runat="server" Target="_blank"><asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/plus.png" /></asp:HyperLink> &nbsp;
                    <a runat="server" id="lbtnDelete" style="border:0; cursor:pointer;"><asp:Image ID="Image3" runat="server" ImageUrl="~/images/buttons/delete.png" /></a>
                    <div id="DialogContainer" runat="server" meta:resourcekey="DialogContainerRcrs" style="display:none;">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td style="text-align:center;">
                                    <asp:TextBox ID="tbSearchPattern" runat="server" Width="340px"></asp:TextBox>&nbsp;&nbsp;
                                    <a runat="server" id="lbtnDoSearch" style="border:0px; cursor:pointer;">
                                        <asp:Image runat="server" ImageUrl="~/images/buttons/zoom.png" />
                                    </a><br /><br />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align:center;">
                                    <asp:ListBox ID="lstAssets" runat="server" Width="380px" Height="350px" SelectionMode="Multiple"></asp:ListBox><br />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>


