<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="InstallationCheck.aspx.cs" Inherits="AssetSite.admin.InstallationCheck" %>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="panel">
        <div class="panelheader">
            Installed system integrity check
        </div>
        <div class="panelcontent">
            <asp:GridView runat="server" ID="ValidationReport" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%# ((bool)Eval("IsValid")) ? "<span class='sc'>Success</span>" : "<span class='fl'>Failed</span>"%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
</asp:Content>
