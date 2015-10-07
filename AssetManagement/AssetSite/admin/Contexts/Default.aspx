<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Contexts.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.master" %>
<%@ Import Namespace="AppFramework.Core.Classes" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
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
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="tDynList" ControlSelector="#translationsSelector" runat="server" />
    <div id="main-container">
        <div class="wizard-header">
            <asp:Label runat="server" ID="MainContextLabel" meta:resourcekey="MainContextLabelResource1"></asp:Label>
        </div>
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="ContextNameLbl" meta:resourcekey="ContextNameLblResource1"></asp:Label>
            </div>
            <div class="panelcontent">
                <asp:GridView ID="ContextsGrid" runat="server" DataKeyNames="ID" DataSourceID="ContextsDataSource"
                    OnRowEditing="OnRowEditing" OnRowUpdating="OnRowUpdating" OnRowCancelingEdit="OnRowCancelingEdit"
                    AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $Resources:Global, NameText %>" HeaderStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblName" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtName" Text='<%# Eval("OriginalName") %>' MaxLength="60"
                                    ValidationGroup="grid" CssClass="tarnslationedit"></asp:TextBox>
                                <asp:RequiredFieldValidator ValidationGroup="grid" runat="server" Display="Dynamic"
                                    ErrorMessage="Please enter context name." ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationedit')">Add
                                    translation</a>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $Resources:Global, DataTypeText %>" HeaderStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataType" Text='<%# Eval("DataTypeUid") != null 
                                ? DataTypeService.GetByUid((long)Eval("DataTypeUid")).Name 
                                : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" ShowCancelButton="true"
                            ButtonType="Image" CancelText="<% $Resources:Global, CancelText %>" CancelImageUrl="~/images/buttons/slash.png"
                            DeleteText="<% $Resources:Global, DeleteText %>" DeleteImageUrl="~/images/buttons/delete.png"
                            UpdateImageUrl="~/images/buttons/disk.png" UpdateText="<% $Resources:Global, SaveText %>"
                            EditImageUrl="~/images/buttons/edit.png" EditText="<% $Resources:Global, EditText %>"
                            ItemStyle-HorizontalAlign="Right" ValidationGroup="grid" />
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label runat="server" Text="<% $Resources:Global, ListIsEmpty %>"></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource runat="server" ID="ContextsDataSource" TypeName="AppFramework.Core.Classes.EntityContext"
                    DeleteMethod="Delete" UpdateMethod="Save" OnUpdating="ContextsDataSource_Updating"
                    SelectMethod="GetAll"></asp:ObjectDataSource>
            </div>
        </div>
        <div class="panel">
            <div class="panelheader">
                <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1"></asp:Label>
            </div>
            <div class="panelcontent">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" CssClass="labels" Text="<% $Resources:Global, NameText %>"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNewContext" MaxLength="60" CssClass="tarnslationinsert"
                                ValidationGroup="add"></asp:TextBox>
                            <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationinsert')">Add
                                translation</a>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="dropDataType">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="btnAddContext" ValidationGroup="add" runat="server" Text="<% $Resources:Global, AddText %>"
                                OnClick="btnAddContext_Click" />
                        </td>
                    </tr>
                </table>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNewContext"
                                            ValidationGroup="add" ErrorMessage="Please enter context name." /> <br />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="dropDataType" 
                                            ValidationGroup="add" ErrorMessage="Please select a datatype." />
               </div>
        </div>
        <div class="wizard-footer-buttons">
            <asp:Button ID="btnClose" runat="server" Text="<% $Resources:Global, CompleteText %>"
                OnClick="btnClose_Click" />
        </div>
    </div>
</asp:Content>
