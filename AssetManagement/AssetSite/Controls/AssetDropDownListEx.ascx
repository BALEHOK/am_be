<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetDropDownListEx.ascx.cs"
    Inherits="AssetSite.Controls.AssetDropDownListEx" %>
<div style="white-space: nowrap;" class="assets-picker">
    <asp:TextBox ID="tbSelectedAsset" runat="server" ReadOnly="True" Width="200px"></asp:TextBox>&nbsp;
    <asp:HiddenField ID="hdfSelectedValue" runat="server" />
    <asp:HiddenField ID="hdfSelectedText" runat="server" />
    <a id="lbtnSearch" runat="server" style="cursor: pointer; border: 0;">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>&nbsp;
    <asp:HyperLink ID="lnkAdd" runat="server" Target="_blank">
        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/plus.png" />
    </asp:HyperLink>
    &nbsp; <a runat="server" id="lbtnDelete" style="border: 0; cursor: pointer;">
        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/buttons/delete.png" /></a>
    <div id="DialogContainer" runat="server" style="display: none;">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="min-width: 400px;">
                    <div class="loader hidden">
                    </div>
                    <asp:TextBox ID="tbSearchPattern" runat="server" Width="340px"
                        meta:resourcekey="tbSearchPatternResource1"></asp:TextBox>&nbsp;&nbsp;<a id="lbtnDoSearch"
                            runat="server" style="border: 0px; cursor: pointer;"><asp:Image runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ListBox ID="lstAssets" runat="server" Height="300px" CssClass="assets-select"></asp:ListBox>
                    <br />
                </td>
            </tr>
        </table>
    </div>
</div>
