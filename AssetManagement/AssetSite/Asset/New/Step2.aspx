<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    EnableEventValidation="false" AutoEventWireup="true" Inherits="AssetSite.Asset.New.Step2"
    Trace="false" CodeBehind="Step2.aspx.cs" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/AssetAttributePanels.ascx" TagName="AssetAttributePanels"
    TagPrefix="amcl" %>
<%@ Register Src="~/Controls/AssetToolbar.ascx" TagName="AssetToolbar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreensPanel.ascx" TagName="ScreensPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreenDetails.ascx" TagName="ScreenDetails" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RestoreAssetMessage.ascx" TagPrefix="amcl" TagName="RestoreAssetMessage" %>
<%@ Register Src="~/Controls/AssetTemplates.ascx" TagPrefix="amcl" TagName="AssetTemplates" %>


<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <div class="wizard-menu">
        <uc1:ScreensPanel runat="server" ID="screensPanel" />
        <amcl:AssetTemplates runat="server" id="AssetTemplates" Visible="False" />
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Auto" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        <uc1:AssetToolbar ID="toolbar" runat="server" />
        <hr />
        <div class="wizard-header">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblNoPermissions" SkinID="labelNoPermissions" runat="server" Visible="false"
                Text="<%$ Resources:Global, NoPermissionsText%>" />

            <amcl:RestoreAssetMessage runat="server" id="RestoreAssetMessage" Visible="False" />
        </div>
        <uc1:ScreenDetails runat="server" ID="sdAssetFill" />
        <amcl:AssetAttributePanels runat="server" ID="AssetAttributePanel" Editable="true" />
        <div style="clear: both;">
        </div>
        <div class="wizard-footer-buttons">
            <asp:LinkButton ID="btnSave" runat="server" Text="save" OnClick="btnNext_Click" Visible="false"></asp:LinkButton>
            <asp:LinkButton ID="btnSaveAndAdd" runat="server" Text="savenadd" OnClick="btnSaveAndAdd_Click"
                Visible="false" />
            <asp:LinkButton ID="btnSaveTemplate" runat="server" Text="savetemplate" Visible="false"
                OnClick="btnSaveTemplate_Click" />
        </div>
        <uc1:AssetToolbar ID="bottomtoolbar" runat="server" />
    </div>
</asp:Content>
