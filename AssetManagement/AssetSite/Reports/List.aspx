<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="AssetSite.Reports.List" %>
<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMainContent" runat="server">
    <asp:GridView runat="server" ID="gvReports" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Name"/>
            <asp:HyperLinkField DataNavigateUrlFormatString="Render.aspx?LayoutId={0}" Text="Render" DataNavigateUrlFields="Id" />
        </Columns>
    </asp:GridView>
</asp:Content>
