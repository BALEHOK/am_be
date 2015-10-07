<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SideMenu.ascx.cs" Inherits="AssetSite.admin.AdditionalScreens.SideMenu" %>

<asp:Panel ID="itemsContainer" runat="server" CssClass="wizard-menu" meta:resourcekey="itemsContainerResource1">
    <asp:Panel ID="title_4" runat="server" CssClass=" active">
        <asp:Label ID="Label3" runat="server">Screen Configuration</asp:Label>
    </asp:Panel>
    <asp:Panel ID="desc_4" runat="server" CssClass="subitem">
        <asp:HyperLink ID="HyperLink1" runat="server">Create/Edit Screen</asp:HyperLink>
        <br />
        <asp:HyperLink ID="HyperLink2" runat="server">Screen Properties</asp:HyperLink>
        <br />
        <asp:HyperLink ID="HyperLink3" runat="server">Layout Selection</asp:HyperLink>
        <br />
        <asp:HyperLink ID="HyperLink4" runat="server">Panels Configuration</asp:HyperLink>
        <br />
        <asp:HyperLink ID="HyperLink5" runat="server">Assign Attributes</asp:HyperLink>
    </asp:Panel>
</asp:Panel>
