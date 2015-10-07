<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="AdditionalScreenStep1.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server">Screen Configuration</asp:Label>            
        </div>
        <br />
        <div class="active">
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep1.aspx">Create/Edit Screen</asp:HyperLink>
            </span>
        </div>
        <%--<div>
            <span style="font-size:smaller;">
                <asp:HyperLink runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep2.aspx">Screen Properties</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep3.aspx">Layout Selection</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep4.aspx">Panels Configuration</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep5.aspx">Assign Attributes</asp:HyperLink>
            </span>
        </div>--%>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Item Wizard"></asp:Label>&nbsp;&mdash;&nbsp;
        <asp:Label runat="server" ID="lblStepName" Text="Attributen"></asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" ID="stepDesc" 
            Text="Mauris congue consectetuer quam."></asp:Literal>
    </p>        
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>                
        </div>
        <div class="panelcontent">
            <asp:GridView ID="gvScreens" runat="server" AutoGenerateColumns="false" CssClass="w100p" DataSourceID="edsScreens">
                <Columns>
                    <asp:BoundField HeaderText="Title" DataField="Title" />
                    <asp:BoundField HeaderText="Description" DataField="Comment" />
                    <asp:BoundField HeaderText="Default" DataField="IsDefault" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" NavigateUrl='<%#GetEditUrl(Eval("ScreenId")) %>' runat="server">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" />
                            </asp:HyperLink>&nbsp;
                            <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%#Eval("ScreenId") %>' OnCommand="OnlbtnDelete_Command">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:EntityDataSource ID="edsScreens" runat="server" 
                ConnectionString="name=DataEntities" DefaultContainerName="DataEntities" 
                EnableFlattening="False" EntitySetName="AssetTypeScreen" EntityTypeFilter="" 
                Select="" Where="it.DynEntityConfigUid = @dynEntityUid">
                <WhereParameters>
                    <asp:QueryStringParameter QueryStringField="atuid" Name="dynEntityUid" Type="Int64" DefaultValue="0" />
                </WhereParameters>
            </asp:EntityDataSource>
            <br />
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep2.aspx">
                <asp:Label runat="server" ID="lblNewScreen" Text="Add New Screen"></asp:Label>
            </asp:HyperLink>
        </div>
    </div>
</asp:Content>
