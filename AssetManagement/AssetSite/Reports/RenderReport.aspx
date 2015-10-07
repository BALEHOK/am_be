<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenderReport.aspx.cs" Inherits="AssetSite.Reports.RenderReport"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1">Filter parameters</asp:Label>            
        </div>
        <div class="panelcontent">
            <asp:GridView runat="server" ID="FilterValues" AutoGenerateColumns="False" 
                OnRowDataBound="FilterValuesRowDataBound" GridLines="None" 
                meta:resourcekey="FilterValuesResource1">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" 
                        meta:resourcekey="BoundFieldResource1" />
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                        <HeaderTemplate>
                            <asp:Literal runat="server" meta:resourcekey="LiteralResource1" Text="Filter"></asp:Literal>                            
                        </HeaderTemplate>
                        <ItemTemplate>
                            
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button runat="server" Text="View report" OnClick="ViewReportClick" 
            meta:resourcekey="ButtonResource1" />        
    </div>
</asp:Content>
