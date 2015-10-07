<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChildAssets.ascx.cs"
    Inherits="AssetSite.Controls.AssetTypeChildren" %>
<div class="active">
    <asp:Label ID="lblTitle" runat="server" meta:resourcekey="lblTitle"></asp:Label>
</div>
<div style="font-size: smaller;">
    <asp:Repeater runat="server" ID="rAssetTypes" OnItemDataBound="rAssetTypes_ItemDataBound">
        <ItemTemplate>
            <asp:HyperLink runat="server" Text="link" ID="linkAssetType" />
            <br />
        </ItemTemplate>
    </asp:Repeater>
</div>
