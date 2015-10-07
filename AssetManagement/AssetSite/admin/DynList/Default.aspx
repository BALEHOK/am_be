<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.DynList.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" Title="Dynamic list"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
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
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Dynamic lists</asp:Label>
        </div>
        <div class="panelcontent" style="overflow: auto">
            <asp:GridView ID="DynListAll" runat="server" AutoGenerateColumns="False" AllowPaging="true"
                AllowSorting="true" DataKeyNames="DynListUid" DataSourceID="DynListDataSource"
                OnRowCancelingEdit="DynListAll_RowCancelingEdit" OnRowEditing="DynListAll_RowEditing">
                <Columns>
                    <asp:TemplateField HeaderText="<% $Resources:Global, NameText %>">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDynListName" Text='<%# GetDynListName(Container.DataItem) %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox runat="server" ID="txtDynListName" CssClass="tarnslationedit" Text='<%# Bind("Name") %>' />
                            <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationedit')">Add
                                translation</a>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Comment" HeaderText="<% $Resources:Global, DescText %>"
                        ItemStyle-Width="50%" ItemStyle-Wrap="false">
                        <ItemStyle Wrap="False" Width="50%"></ItemStyle>
                    </asp:BoundField>
                    <asp:HyperLinkField DataNavigateUrlFields="DynListUid" DataNavigateUrlFormatString="~/admin/DynList/ListItems.aspx?Id={0}"
                        Text="Items" meta:resourcekey="HyperLinkFieldResource1" />
                    <asp:CommandField ButtonType="Image" ShowEditButton="true" ShowDeleteButton="true"
                        EditImageUrl="/images/buttons/edit.png" DeleteImageUrl="/images/buttons/delete.png"
                        UpdateImageUrl="/images/buttons/update.png" CancelImageUrl="/images/buttons/cancel.png"
                        DeleteText="Delete" />
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No dynamic lists</asp:Label>
                </EmptyDataTemplate>
            </asp:GridView>            
            <h4 runat="server" id="insertHeader">
                Add new DynList</h4>
            <asp:DetailsView runat="server" DataSourceID="DynListDataSource" DataKeyNames="DynListUid"
                AutoGenerateRows="false" DefaultMode="Insert" OnItemInserted="DynList_Inserted"
                ID="dvDynList" GridLines="None" AutoGenerateInsertButton="true">
                <Fields>
                    <asp:TemplateField HeaderText="Name">
                        <InsertItemTemplate>
                            <asp:TextBox runat="server" ID="txtName" CssClass="tarnslationinsert" Text='<%# Bind("Name") %>'
                                MaxLength="60" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName">*</asp:RequiredFieldValidator>
                            <a href="javascript:void(0)" onclick="showTranslationsDialog('tarnslationinsert')">Add
                                translation</a>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Label">
                        <InsertItemTemplate>
                            <asp:TextBox runat="server" ID="txtLabel" Text='<%# Bind("Label") %>' MaxLength="50" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLabel">*</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <InsertItemTemplate>
                            <asp:TextBox runat="server" ID="txtComment" Text='<%# Bind("Comment") %>' MaxLength="1000"
                                TextMode="MultiLine" Rows="3" Columns="30" />
                        </InsertItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </div>
    </div>
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="tDynList" ControlSelector="#translationsSelector" runat="server" />

    <asp:EntityDataSource runat="server" ID="DynListDataSource" AutoPage="true" AutoSort="true"
        EnableInsert="true" EnableUpdate="true" EnableDelete="true" OnInserting="DynList_Inserting"
        OnUpdating="DynList_Updating" OnDeleting="DynList_Deleting" EnableFlattening="false"
        ConnectionString="name=DataEntities" 
        DefaultContainerName="DataEntities" 
        EntitySetName="DynLists"
        OrderBy="it.Name" Where="it.Active=true">
    </asp:EntityDataSource>
</asp:Content>
