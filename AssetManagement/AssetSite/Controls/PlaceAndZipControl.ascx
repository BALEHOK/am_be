<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlaceAndZipControl.ascx.cs" Inherits="AssetSite.Controls.PlaceAndZipControl" %>

<div>
    <asp:TextBox ID="tbSelection" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;
    <a runat="server" id="lbtnSearch" style="border:0px; cursor:pointer;">
        <asp:Image runat="server" ImageUrl="~/images/buttons/zoom.png" />
    </a>
    <asp:HiddenField ID="hdfSelectedValue" runat="server" />
    <asp:HiddenField ID="hdfSelectedText" runat="server" />
</div>
<div id="DialogContainer" runat="server" meta:resourcekey="DialogContainerRcrs" style="display:none;">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td style="text-align:center;">
                <asp:TextBox ID="tbSearchPattern" ToolTip="Start typing..." runat="server" Width="340px"></asp:TextBox>&nbsp;&nbsp;<a id="lbtnDoSearch" runat="server" onclick="javascript:void(0);" style="border:0px; cursor:pointer;"><asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" /></a><br /><br />
            </td>
        </tr>
        <tr>
            <td style="text-align:center;">
                <asp:ListBox ID="lstResults" runat="server" Width="380px" Height="320px" SelectionMode="Single"></asp:ListBox><br />
            </td>
        </tr>
    </table>
</div>