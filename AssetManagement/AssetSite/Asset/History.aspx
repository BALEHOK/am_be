<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master" CodeBehind="History.aspx.cs"
    AutoEventWireup="true" Inherits="AssetSite.Asset.History" meta:resourcekey="PageResource1" %>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="LabelResource1">Categories</asp:Label>
        </div>
        <span style="font-size: smaller;">
            <asp:Literal ID="litCategoryPath" runat="server"></asp:Literal>
        </span>

        <div class="active">
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label2Resource1">Taxonomies</asp:Label>
        </div>
        <span style="font-size: smaller;">
            <asp:Literal ID="litTaxonomies" runat="server"></asp:Literal>
        </span>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">

    <style type="text/css">
        table.gridtable {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: #333333;
            border-width: 1px;
            border-color: #666666;
            border-collapse: collapse;
        }

            table.gridtable th {
                font-weight: bold;
                font-size: 12px;
                color: #ffffff;
                padding: 8px;
                background-color: #5EBA2F;
            }

            table.gridtable td {
                padding: 8px;
                border-color: #666666;
            }
    </style>

    <div id="main-container">
        <div class="wizard-header">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Asset history</asp:Label>&nbsp;-&nbsp;
            <asp:Label runat="server" ID="lblAssetName"></asp:Label>
        </div>
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelHeader"
                    meta:resourcekey="panelHeaderResource1">Asset</asp:Label>

            </div>
            <div class="panelcontent">
                <asp:PlaceHolder ID="GridPlaceholder" runat="server"></asp:PlaceHolder>
                <table id="historyTable" class="gridtable" runat="server">
                </table>
                <amc:AssetsGrid
                    DataSourceID="assetsHistoryDataSource"
                    ID="gvAssetHistory" HistoryGrid="true"
                    runat="server">
                    <EmptyDataTemplate>
                        <i>no history records</i>
                    </EmptyDataTemplate>
                </amc:AssetsGrid>
                <asp:ObjectDataSource
                    runat="server"
                    EnablePaging="true"
                    ID="assetsHistoryDataSource"
                    SelectMethod="GetHistoryAssets"
                    StartRowIndexParameterName="rowStart"
                    MaximumRowsParameterName="rowsNumber"
                    SelectCountMethod="GetHistoryAssetsCount"
                    TypeName="AppFramework.Core.Classes.AssetFactory">
                    <SelectParameters>
                        <asp:Parameter Name="assetTypeId" Type="Int64" />
                        <asp:Parameter Name="assetId" Type="Int64" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </div>
        </div>
    </div>
</asp:Content>
