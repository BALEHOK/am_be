<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceOps.aspx.cs" Inherits="AssetSite.admin.ServiceOps" MasterPageFile="~/MasterPages/MasterPageDefault.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <div>
        <asp:HyperLink runat="server" NavigateUrl="~/admin/CacheMonitor.aspx" Text="Cache monitor"></asp:HyperLink>        
    </div>
    <div>
        <asp:LinkButton ID="LinkButton1" runat="server" Text="Rebuild active indexes" OnClick="RebuildActive_Click"></asp:LinkButton>
    </div>
    <div>
        <asp:LinkButton ID="LinkButton2" runat="server" Text="Rebuild history indexes" OnClick="RebuildHistory_Click"></asp:LinkButton>
    </div>
    <div>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/InstallationCheck.aspx" Text="Installation check"></asp:HyperLink>
    </div>
</asp:Content>