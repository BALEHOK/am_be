<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    EnableEventValidation="false" AutoEventWireup="true" Inherits="AssetSite.Asset.Edit"
    EnableViewState="true" CodeBehind="Edit.aspx.cs" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/AssetAttributePanels.ascx" TagName="AssetAttributePanels"
    TagPrefix="amcl" %>
<%@ Register Src="~/Controls/AssetToolbar.ascx" TagName="AssetToolbar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreensPanel.ascx" TagName="ScreensPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreenDetails.ascx" TagName="ScreenDetails" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RestoreAssetMessage.ascx" TagPrefix="amcl" TagName="RestoreAssetMessage" %>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <style type="text/css">
        .panelcontent input[type="text"], .panelcontent select
        {
            width: 300px !important;
        }
    </style>
    <div class="wizard-menu">
        <uc1:ScreensPanel runat="server" ID="screensPanel" />
        <div class="active">
            <asp:Label runat="server" meta:resourcekey="LabelResource1">Categories</asp:Label>
        </div>
        <span style="font-size: smaller;">
            <asp:Literal ID="litCategoryPath" runat="server"></asp:Literal>
        </span>
        <div class="active">
            <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">Taxonomies</asp:Label>
        </div>
        <span style="font-size: smaller;">
            <asp:Literal ID="litTaxonomies" runat="server"></asp:Literal>
        </span>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <asp:ScriptManager ID="MainScriptManager" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        <uc1:AssetToolbar ID="toolbar" runat="server" />
        <hr />
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td class="wizard-header">
                    <span style="color: Black!important; font-weight: normal;">Update</span>
                    <%= Asset.Name %>
                </td>
                <td style="font-size: smaller; color: Gray;">
                    &nbsp;&nbsp;&nbsp;<asp:Literal ID="litRevision" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <div>
            <amcl:RestoreAssetMessage runat="server" id="RestoreAssetMessage" Visible="False" />
        </div>
        <uc1:ScreenDetails runat="server" ID="sdAssetEdite" />
        <amcl:AssetAttributePanels runat="server" ID="AssetAttributePanel" Editable="true" />
        <div style="clear: both;">
        </div>        
        <asp:LinkButton ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" Visible="false" />
        <asp:LinkButton ID="btnSaveAndAdd" runat="server" Text="SaveNAdd" Visible="false"
            OnClick="btnSaveAndAdd_Click" />
        <hr />
        <uc1:AssetToolbar ID="bottomtoolbar" runat="server" />
    </div>
</asp:Content>
