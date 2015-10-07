<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.Documents.Default" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <div class="panel">
        <div class="panelheader">
            <asp:Label ID="Label1" runat="server" Text="Documents" 
                meta:resourcekey="Label1Resource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:ScriptManager 
                runat="server" 
                AsyncPostBackErrorMessage="Error while building list. Please refresh the page and try again"
                ID="ScriptManager1">  
                <Services>
                    <asp:ServiceReference Path="~/amDataService.asmx" />
                </Services>                 
            </asp:ScriptManager>  
            <amc:AssetsGrid 
                runat="server" 
                ID="assetsGrid"    
                EmptyDataText="<% $Resources:Global, ListIsEmpty %>"                       
                DataSourceID="assetsDataSource"></amc:AssetsGrid> 
             <asp:ObjectDataSource 
                    runat="server"
                    EnablePaging="true"
                    ID="assetsDataSource"
                    SelectMethod="GetAssetsByAssetTypeId"
                    StartRowIndexParameterName="rowStart"                                                        
                    MaximumRowsParameterName="rowsNumber"                            
                    SelectCountMethod="GetAssetsCountByAssetTypeId" 
                    OnSelecting="DataSource_Selecting"
                    TypeName="AppFramework.Core.Classes.AssetFactory">
                     <SelectParameters>
                       <asp:Parameter Name="filterWithPermissions" Type="Boolean" DefaultValue="true" />
                    </SelectParameters> 
                </asp:ObjectDataSource> 
         </div>
    </div>
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
</asp:Content>
