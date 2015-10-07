<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Taxonomies.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Debug" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <script type="text/javascript">
        function showTranslationsDialog(id) {
            $('#translationsSelector').val($('.' + id).val());
            showTranslations();
        }
    </script>
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblTaxPageTitle" Text="Taxonomies list" meta:resourcekey="lblTaxPageTitleResource1"></asp:Label>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblTaxonomies" Text="Taxonomies" meta:resourcekey="lblTaxonomiesResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:Label runat="server" ID="ErrorMessage" Visible="false" ForeColor="Red"></asp:Label>
            <asp:GridView ID="TaxonomiesGrid" runat="server" AutoGenerateColumns="False" CellPadding="4"
                DataSourceID="TaxonomyDataSource" DataKeyNames="TaxonomyUid" ForeColor="#333333"
                GridLines="None" CssClass="w100p" OnRowDataBound="TaxonomiesGrid_RowDataBound"
                meta:resourcekey="TaxonomiesGridResource1">
                <RowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField HeaderText="<% $Resources:Global, NameText %>" meta:resourcekey="TemplateFieldResource5">
                        <EditItemTemplate>
                            <asp:TextBox HeaderStyle-Width="30%" ID="txtName" CssClass="tarnslationedit" Text='<%# Bind("Name") %>' runat="server"
                                meta:resourcekey="txtNameResource1" />
                            <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="TaxonomyGroup">*</asp:RequiredFieldValidator>
                            <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationedit')">Add
                                translation</a>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label HeaderStyle-Width="30%" runat="server" Text='<%# GetTaxonomyName(Container.DataItem) %>' ID="lblName"
                                meta:resourcekey="lblNameResource1"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Description" HeaderText="<% $Resources:Global, DescText %>"
                        HeaderStyle-Width="40%" meta:resourcekey="BoundFieldResource2">
                        <HeaderStyle Width="40%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
                        <HeaderTemplate>
                            <asp:Literal runat="server" meta:resourcekey="LiteralResource1" Text="Category"></asp:Literal>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <input type="radio" name="CategorySelect" value='<%# Eval("TaxonomyUid") %>' <%# IsCategoryChecked(Eval("IsCategory")) %>
                                onclick="__doPostBack('CategorySelector','<%# Eval("TaxonomyUid") %>')" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFields="TaxonomyUid" DataNavigateUrlFormatString="~/admin/Taxonomies/TreeEdit.aspx?Uid={0}"
                        Text="<% $Resources:Global, ViewText %>" />
                    <asp:HyperLinkField DataNavigateUrlFields="TaxonomyUid" DataNavigateUrlFormatString="~/admin/Taxonomies/TreeEdit.aspx?Uid={0}&Edit=1"
                        Text="<% $Resources:Global, EditText %>" />
                    <asp:CommandField ValidationGroup="TaxonomyGroup" CausesValidation="true" ShowEditButton="True"
                        HeaderStyle-Width="5%" ButtonType="Image" EditText="<% Resources:Global, EditText %>"
                        EditImageUrl="/images/buttons/edit.png" UpdateImageUrl="/images/buttons/disk.png"
                        CancelImageUrl="/images/buttons/slash.png" UpdateText="Save" CancelText="Cancel"
                        meta:resourcekey="CommandFieldResource1">
                        <HeaderStyle Width="5%"></HeaderStyle>
                    </asp:CommandField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource4">
                        <ItemTemplate>
                            <asp:ImageButton runat="server" ImageUrl="~/images/buttons/delete.png" CommandName="Delete"
                                OnClientClick="return confirm('Taxonomy tree will be deleted. Continue?')" meta:resourcekey="ImageButtonResource1" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#DFDFDF" Font-Bold="True" ForeColor="#1E1E1E" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="#E0FFC1" />
                <EmptyDataTemplate>
                    <asp:Literal runat="server" meta:resourcekey="LiteralResource2" Text="No taxonomies"></asp:Literal>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:EntityDataSource runat="server" ID="TaxonomyDataSource" AutoPage="true" AutoSort="true"
                EnableDelete="true" EnableUpdate="true" OnDeleting="Taxonomy_Deleting" OnUpdating="Taxonomy_Updating"
                EnableFlattening="false" ConnectionString="name=DataEntities" DefaultContainerName="DataEntities"
                EntitySetName="Taxonomy" OrderBy="it.Name" Where="it.ActiveVersion=true and it.IsActive=true">
            </asp:EntityDataSource>
        </div>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblNewTax" Text="New taxonomy" meta:resourcekey="lblNewTaxResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblName" Text="<%$ Resources:Global, NameText %>" meta:resourcekey="lblNameResource3"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="TaxText" CssClass="tarnslationinsert" ValidationGroup="addTax" meta:resourcekey="TaxTextResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="TaxTextRequiredFieldValidator" runat="server" ControlToValidate="TaxText" Display="Dynamic"
                            ValidationGroup="addTax" ErrorMessage="Please enter taxonomy name." meta:resourcekey="TaxTextRequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                        <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationinsert')">Add
                            translation</a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblDesc" Text="<%$ Resources:Global, DescText %>" meta:resourcekey="lblDescResource2"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="TaxDescr" meta:resourcekey="TaxDescrResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="Button1" runat="server" Text="Add" OnClick="AddTaxonomy" ValidationGroup="addTax"
                            meta:resourcekey="Button1Resource1" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="tDynList" ControlSelector="#translationsSelector" runat="server" />
</asp:Content>
