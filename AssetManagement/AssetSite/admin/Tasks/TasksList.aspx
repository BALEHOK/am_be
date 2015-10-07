<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="TasksList.aspx.cs" Inherits="AssetSite.admin.Tasks.TasksList" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container">
        <asp:CheckBox ID="cbShowActiveOnly" runat="server" Text="Show Active Tasks Only"
            Checked="true" /><br />
        <asp:GridView ID="gvTasks" runat="server" AutoGenerateColumns="False" Width="100%"
            AllowPaging="True" DataSourceID="edsTasks" OnRowDataBound="gvTasks_RowDataBound">
            <EmptyDataTemplate>
                No tasks assigned to this asset type.
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        Name</HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal runat="server" ID="TranslatedName"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        Description
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal runat="server" ID="TranslatedDescription"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="50%" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Function Type">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%#GetFunctionType(Eval("FunctionType")) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="20%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="lbtnEdit" runat="server" NavigateUrl='<%#GetEditUrl(Eval("TaskId")) %>'>
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" />
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnMakeInactive" runat="server" CommandArgument='<%#Eval("TaskId") %>'
                            OnClick="lbtnMakeInactive_Click">
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                        </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Width="5%" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:EntityDataSource ID="edsTasks" runat="server" ConnectionString="name=DataEntities"
            DefaultContainerName="DataEntities" EnableFlattening="False" EntitySetName="Task"
            EntityTypeFilter="" Select="" Where="it.[DynEntityConfigId]==@dynEntityConfigId && it.[IsActive]==@isActive">
            <WhereParameters>
                <asp:ControlParameter ControlID="cbShowActiveOnly" Name="isActive" Type="Boolean"
                    PropertyName="Checked" />
                <asp:QueryStringParameter DefaultValue="0" Name="dynEntityConfigId" Type="Int64"
                    QueryStringField="AssetTypeId" />
            </WhereParameters>
        </asp:EntityDataSource>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
    <asp:Panel ID="itemsContainer" runat="server" CssClass="wizard-menu" meta:resourcekey="itemsContainerResource1">
        <asp:Panel ID="title_4" runat="server" CssClass=" active">
            <asp:Label ID="Label4" runat="server">Actions</asp:Label>
        </asp:Panel>
        <asp:Panel ID="desc_4" runat="server" CssClass="subitem">
            <asp:HyperLink ID="hplCreateTask" runat="server" Target="_self"> Create Task </asp:HyperLink>
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.item').hover(
            function () {
                $(this).css('background-color', '#84d859');
            },
            function () {
                $(this).css('background-color', '#999999');
            }
        );
        });

    </script>
</asp:Content>
