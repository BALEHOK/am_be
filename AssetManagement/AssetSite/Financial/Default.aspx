<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="AssetSite.Financial.Default"
    MasterPageFile="~/MasterPages/MasterPageBase.Master" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderSearchBox">
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelHeader" 
                meta:resourcekey="panelHeaderResource1">Financial Reports</asp:Label>            
        </div>
        <div class="panelcontent">
            <asp:GridView ID="ReportsList" runat="server" AutoGenerateColumns="False" 
                OnRowDeleting="ReportsListRowDeleting" meta:resourcekey="ReportsListResource1">             
                <Columns>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("UID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-Width="60%" 
                        meta:resourcekey="BoundFieldResource1" >
<HeaderStyle Width="60%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Template" HeaderText="Template" 
                        HeaderStyle-Width="20%" meta:resourcekey="BoundFieldResource2" >
<HeaderStyle Width="20%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:HyperLinkField DataNavigateUrlFields="UID" DataNavigateUrlFormatString="../Reports/RenderReport.aspx?Uid={0}"
                        Text="Render" Target="_blank" meta:resourcekey="HyperLinkFieldResource1" />                    
                </Columns>          
                <EmptyDataTemplate>
                    <asp:Literal runat="server" ID="lblNoData" 
                        meta:resourcekey="lblNoDataResource1" Text="No reports"></asp:Literal>                    
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
