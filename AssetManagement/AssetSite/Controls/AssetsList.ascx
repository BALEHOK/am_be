<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetsList.ascx.cs" Inherits="AssetSite.Controls.AssetsList" %>

<div style="white-space:nowrap;">
    <asp:TextBox ID="tbSelectedAsset" runat="server" ReadOnly="true" Width="200"></asp:TextBox>&nbsp;
    <asp:HiddenField ID="hdfSelectedValue" runat="server" />
    <asp:HiddenField ID="hdfSelectedText" runat="server" />
    <a id="lbtnSearch" runat="server" style="cursor:pointer; border:0;"><asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>&nbsp;
    <div id="DialogContainer" runat="server" meta:resourcekey="DialogContainerRcrs" style="display:none;">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="text-align:center;">
                    <asp:TextBox ID="tbSearchPattern" runat="server" Width="340px"></asp:TextBox>&nbsp;&nbsp;<a onclick="javascript:void(0);" style="border:0px; cursor:pointer;"><asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/zoom.png" />  </a><br /><br />
                </td>
            </tr>
            <tr>
                <td style="text-align:center;">
                    <asp:ListBox ID="lstAssets" runat="server" Width="380px" Height="370px"></asp:ListBox><br />
                </td>
            </tr>
            <tr>
                <td style="text-align:center;">
                    <a id="lbtnOK" runat="server" style="cursor:pointer;">OK</a>
                </td>
            </tr>
        </table>
    </div>
</div>
