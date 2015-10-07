<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TreeEdit.aspx.cs" Inherits="AssetSite.admin.Taxonomies.TreeEdit"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Debug" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        function showTranslationsDialog(mode) {
            if (mode == "addnode") {
                $('#translationsSelector').val($('#createTaxonomy .name').val());
            } else {
                $('#translationsSelector').val($('#renameTaxonomy .name').val());
            }

            showTranslations();
        }

        function AddNode(path, name) {
            $('#AddToSelectedPanel').show();
            $('#nodeName').text(name);
            var hidden = '#<%= NodePath.ClientID %>';
            $(hidden).val(path);
        };

        function ShowRenamePane(path, name) {
            $('#RenameTaxonomyPanel').show();
            $('#<%= NewTaxonomyName.ClientID %>').val(name);
            var hidden = '#<%= NodePath.ClientID %>';
            $(hidden).val(path);
        }

        function hideRenamePanel() {
            $('#RenameTaxonomyPanel').hide();
            return false;
        }

        function DownTaxonomyItem(valuePath) {
            $('#<%= hfChangeTaxonomyItemLocation.ClientID %>').val(valuePath);
            $('#<%= btnDownTaxonomyItem.ClientID %>').click();
        }

        function UpTaxonomyItem(valuePath) {
            $('#<%= hfChangeTaxonomyItemLocation.ClientID %>').val(valuePath);
            $('#<%= btnUpTaxonomyItem.ClientID %>').click();
        }
    </script>
    <asp:HiddenField ID="NodePath" runat="server" />
    <div class="wizard-header">
        <asp:Label ID="Label1" runat="server" meta:resourcekey="LabelResource1">Taxonomy</asp:Label>
        <asp:Label ID="DraftAlert" CssClass="alert" runat="server" Visible="False" meta:resourcekey="DraftAlertResource1">Draft</asp:Label>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label ID="Label2" runat="server" meta:resourcekey="panelTitle"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:HyperLink ID="linkAddNode" runat="server" onclick="return AddNode('', 'tree as root')"
                Text="Add new root node" NavigateUrl="#" meta:resourcekey="linkAddNodeResource1"></asp:HyperLink>
            <asp:TreeView runat="server" ShowLines="true" SelectedNodeStyle-Font-Bold="true"
                ID="TaxonomiesTree">
            </asp:TreeView>
        </div>
    </div>
    <div class="panel" id="AddToSelectedPanel" style="display: none">
        <div class="panelheader">
            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Add to"></asp:Literal>&nbsp;<span
                id="nodeName"></span>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5" id="createTaxonomy">
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" ValidationGroup="addNode" meta:resourcekey="LabelResource2">Taxonomy</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="name" ID="TaxText" MaxLength="60" meta:resourcekey="TaxTextResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="TaxTextRequiredFieldValidator" runat="server" ControlToValidate="TaxText"
                            ValidationGroup="addNode" ErrorMessage="Please enter the node name." meta:resourcekey="TaxTextRequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <a href="javascript:void(0)" onclick="showTranslationsDialog('addnode')">Add translation</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="Button1" ValidationGroup="addNode" runat="server" Text="<% $Resources:Global, AddText %>"
                            OnClick="AddTaxonomyToCurrent" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="panel" id="RenameTaxonomyPanel" style="display: none">
        <div class="panelheader">
            <asp:Literal runat="server" Text="<% $resources: Rename %>"></asp:Literal>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5" id="renameTaxonomy">
                <tr>
                    <td>
                        <asp:TextBox runat="server" CssClass="name" ID="NewTaxonomyName" MaxLength="60"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NewTaxonomyName"
                            ValidationGroup="renameNode" ErrorMessage="Please enter the node name." meta:resourcekey="TaxTextRequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <a href="javascript:void(0)" onclick="showTranslationsDialog('rename')">
                            <asp:Literal runat="server" Text="<% $resources: Translations %>" /></a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="SaveRenamed" ValidationGroup="renameNode" runat="server" Text="<%$ Resources:Global, SaveText %>"
                            meta:resourcekey="SaveResource1" OnClick="RenameTaxonomy" />
                        <asp:Button ID="CancelRenaming" CausesValidation="False" runat="server" Text="<%$ Resources:Global, CancelText %>"
                            meta:resourcekey="CancelResource1" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="Cancel" CausesValidation="False" runat="server" Text="<%$ Resources:Global, CancelText %>"
            OnClick="Cancel_Click" meta:resourcekey="CancelResource1" />
        <asp:Button ID="Save" CausesValidation="False" runat="server" Text="<%$ Resources:Global, SaveText %>"
            OnClick="Save_Click" meta:resourcekey="SaveResource1" Visible="False" />
        <asp:Button ID="Publish" CausesValidation="False" runat="server" Text="<%$ Resources:Global, PublishText %>"
            OnClick="Publish_Click" meta:resourcekey="PublishResource1" Visible="False" />
    </div>
    <script type="text/javascript">
        function DeleteNode(nodepath) {
            if (confirm('<asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Global, DeleteConfirm %>" />')) {
                __doPostBack('DeleteNode', nodepath);
            }
        }
    </script>
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="Translations1" ControlSelector="#translationsSelector" runat="server" />
    <asp:Button runat="server" ID="btnDownTaxonomyItem" Style="display: none;" OnClick="btnDownTaxonomyItem_Click" />
    <asp:Button runat="server" ID="btnUpTaxonomyItem" Style="display: none;" OnClick="btnUpTaxonomyItem_Click" />
    <asp:HiddenField runat="server" ID="hfChangeTaxonomyItemLocation" />
</asp:Content>
