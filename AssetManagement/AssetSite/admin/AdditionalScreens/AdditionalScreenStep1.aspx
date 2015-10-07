<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="AdditionalScreenStep1.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep1" %>

<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <asp:Panel ID="itemsContainer" runat="server" CssClass="wizard-menu" meta:resourcekey="itemsContainerResource1">
        <asp:Panel ID="title_4" runat="server" CssClass=" active">
            <asp:Label ID="Label3" CssClass="title" runat="server">Screen Configuration</asp:Label>
        </asp:Panel>
        <asp:Panel ID="desc_4" runat="server" CssClass="subitem">
            <asp:LinkButton 
                ID="lnkBtn" 
                runat="server" 
                CssClass="active_lnk"
                PostBackUrl="~/admin/AdditionalScreens/AdditionalScreenStep1.aspx"
                Text="Create/Edit Screen"></asp:LinkButton>
            <br />
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
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:GridView ID="gvScreens" runat="server" AutoGenerateColumns="false" CssClass="w100p"
                DataSourceID="edsScreens" OnRowDataBound="gvScreens_DataBound">
                <Columns>
                    <%--<asp:BoundField HeaderText="Title" DataField="Title" HeaderStyle-HorizontalAlign="Left" />--%>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Title
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="TranslatedName"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Description" DataField="Comment" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Default" DataField="IsDefault" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" NavigateUrl='<%#GetEditUrl(Eval("ScreenId")) %>' runat="server">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" />
                            </asp:HyperLink>&nbsp;
                            <asp:LinkButton
                                ID="lbtnDelete"
                                runat="server"
                                CommandArgument='<%#Eval("ScreenId") %>'
                                OnClick="lbtnDelete_OnClick">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:EntityDataSource ID="edsScreens" runat="server" ConnectionString="name=DataEntities"
                DefaultContainerName="DataEntities" EnableFlattening="False" EntitySetName="AssetTypeScreen"
                EntityTypeFilter="" Select="" Where="it.DynEntityConfigUid = @dynEntityUid">
                <WhereParameters>
                    <asp:QueryStringParameter QueryStringField="atuid" Name="dynEntityUid" Type="Int64"
                        DefaultValue="0" />
                </WhereParameters>
            </asp:EntityDataSource>
            <br />
            <asp:HyperLink ID="AddNewScreenLink" runat="server">
                <asp:Label runat="server" ID="lblNewScreen" Text="Add New Screen"></asp:Label>
            </asp:HyperLink>
        </div>
    </div>
</asp:Content>
