<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    AutoEventWireup="true" Inherits="AssetSite.AssetView" Theme="Default"
    Trace="false"
    Codebehind="AssetView.aspx.cs" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/AssetTreeView.ascx" TagName="AssetTreeView" TagPrefix="amc" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <div class="wizard-menu">
        <div class="item pointer" onclick="javascript:location.href='/Asset/New/Step1.aspx'">
            <a href="/Asset/New/Step1.aspx" class="title">                
                    <asp:Label runat="server" Text="New asset" meta:resourcekey="LabelResource2"></asp:Label>                
            </a>
        </div>
        <div class="active">
            <asp:Label runat="server" Text="Categories" meta:resourcekey="LabelResource3"></asp:Label>
        </div>
        <div>
            <amc:AssetTreeView 
                ID="assetTreeView" 
                runat="server" />
        </div>
        <br />
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <div id="main-container">        
        <div class="panel" style="margin-top: 5px;">
            <div class="panelheader">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblAssetTypeName" runat="server" Text="Assets"></asp:Label>
                        </td>
                        <td style="text-align:right;">
                            <asp:HyperLink runat="server" ID="lnkRenderReport" Text="Open Report" Visible="False" />&nbsp;
                            <asp:HyperLink ID="hplCreateAsset" runat="server">
                                <asp:Label ID="lblCreateAsset" runat="server"></asp:Label>
                            </asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="panelcontent" style="padding: 1px;"> 
                <asp:ScriptManager 
                    runat="server" 
                    AsyncPostBackErrorMessage="Error while building list. Please refresh the page and try again"
                    ID="ScriptManager1">  
                    <Services>
                        <asp:ServiceReference Path="~/amDataService.asmx" />
                    </Services>                 
                </asp:ScriptManager>  
                <asp:UpdatePanel
                    ID="assetsPanel"
                    runat="server"
                    ChildrenAsTriggers="true"
                    UpdateMode="Conditional">
                    <ContentTemplate>                    
                        <amc:AssetsGrid 
                            runat="server" 
                            ID="assetsGrid" 
                            CssClass="AssetViewGrid"   
                            ShowHeader="false"
                            EmptyDataText="<% $Resources:Global, ListIsEmpty %>"                       
                            DataSourceID="assetsDataSource">
                            <Columns>
                            </Columns>
                        </amc:AssetsGrid>
                    </ContentTemplate>    
                    <Triggers>                                                    
                    </Triggers>
                </asp:UpdatePanel>             
                <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="10" runat="server" AssociatedUpdatePanelID="assetsPanel">
                    <ProgressTemplate>
                        <div class="loader" id="gridLoader"></div>
                    </ProgressTemplate>                
                </asp:UpdateProgress>  

                <asp:ObjectDataSource 
                    runat="server"
                    EnablePaging="true"
                    ID="assetsDataSource"
                    SelectMethod="GetAssetsByAssetTypeId"                    
                    StartRowIndexParameterName="rowStart"
                    MaximumRowsParameterName="rowsNumber"                            
                    SelectCountMethod="GetAssetsCountByAssetTypeId"
                    TypeName="AppFramework.Core.Classes.AssetFactory">
                    <SelectParameters>                                
                        <asp:ControlParameter 
                            ControlID="PlaceHolderLeftColumn$assetTreeView" 
                            PropertyName="SelectedAssetTypeId"                                    
                            Name="assetTypeId" Type="Int64" DefaultValue="0" />  
                        <asp:Parameter Name="filterWithPermissions" Type="Boolean" DefaultValue="true" />                      
                    </SelectParameters>                            
                </asp:ObjectDataSource>
                               
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            $('.item').hover(
            function() {
                $(this).css('background-color', '#84d859');
            },
            function() {
                $(this).css('background-color', '#999999');
            }
        );
        });
    </script>
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
</asp:Content>


