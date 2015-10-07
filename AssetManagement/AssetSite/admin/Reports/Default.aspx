<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Reports.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <div class="wizard-header">
        <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Reports</asp:Label>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1">Reports list</asp:Label>            
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
                    <asp:BoundField DataField="Name" HeaderText="<% $Resources:Global, NameText %>" HeaderStyle-Width="60%">
<HeaderStyle Width="60%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Template" HeaderText="Template" 
                        HeaderStyle-Width="20%" meta:resourcekey="BoundFieldResource2" >
<HeaderStyle Width="20%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:HyperLinkField DataNavigateUrlFields="UID" DataNavigateUrlFormatString="~/Reports/RenderReport.aspx?Uid={0}"
                        Text="Render" Target="_blank" meta:resourcekey="HyperLinkFieldResource1" />
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource2">
                        <ItemTemplate>
                            <a href='Add/SelectTemplate.aspx?Uid=<%# Eval("UID") %>'>
                                <img src="/images/buttons/edit.png" alt="edit" /></a>                        
                                
                        </ItemTemplate>
                    </asp:TemplateField>                    
                    <asp:TemplateField ShowHeader="False" meta:resourcekey="TemplateFieldResource3">
                        <ItemTemplate>
                            <asp:ImageButton runat="server" CausesValidation="False" CommandName="Delete"
                                OnClientClick='return confirm("Are you sure you want to delete this entry?");'
                                ImageUrl="/images/buttons/delete.png" 
                                meta:resourcekey="ImageButtonResource1" />                            
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>               
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No reports</asp:Label>                    
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="AddReport" Text="Add report on view" runat="server" 
            OnClick="AddReportOnViewClick" meta:resourcekey="AddReportResourceOnView" />
        <asp:Button ID="Button1" Text="Add report on TTX" runat="server" 
            OnClick="AddReportClick" meta:resourcekey="AddReportResource1" />
    </div>
</asp:Content>
