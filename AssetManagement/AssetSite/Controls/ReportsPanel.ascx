<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportsPanel.ascx.cs"
    Inherits="AssetSite.Controls.ReportsPanel" %>
<div class="active">
    <asp:Label ID="ReportsLabel" runat="server" meta:resourcekey="ReportsLabel">Reports</asp:Label>
</div>
<div style="font-size: smaller;">
    <asp:Repeater runat="server" ID="Repeater">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li><a href="<%# Eval("Value") %>"><%# Eval("Key") %></a></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</div>
