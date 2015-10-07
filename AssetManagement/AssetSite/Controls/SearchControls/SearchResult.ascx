<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResult.ascx.cs" Inherits="AssetSite.Controls.SearchControls.SearchResult" %>
<div runat="server" id="infoLayout" class="detail search-result">
    <asp:HyperLink ID="linkButtonName" runat="server" />
    <br/>
    <asp:Label ID="lIntro" runat="server" CssClass="sub-text"></asp:Label>
    <br/>
    <asp:Label ID="lMainInfo" runat="server" Text="allFields"></asp:Label>
    <br/>
</div>