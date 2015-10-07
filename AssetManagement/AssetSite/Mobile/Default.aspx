<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.Mobile.Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <h2><asp:Label runat="server" ID="MobilePageTitle">Mobile applications overview</asp:Label></h2>
    <p> Download hier de mobiele applicatie die je nodig hebt. Téléchargez l'application mobile qui vous avez besoin. </p>
    <p>Download InventScanner: <a href="../Download/InventScannerInstallation.msi">Link</a></p>
    <p>Download StockScanner: <a href="../Download/StockScannerInstallation.msi">Link</a></p>
</asp:Content>