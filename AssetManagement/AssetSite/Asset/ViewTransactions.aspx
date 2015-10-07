<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTransactions.aspx.cs"
    Inherits="AssetSite.Asset.ViewTransactions" MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <div class="wizard-header">
        <asp:Label runat="server" meta:resourcekey="LabelResource1">Stock info</asp:Label>        
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1">Transactions and rest</asp:Label>            
        </div>
        <div class="panelcontent">
            <a href="View.aspx?AssetTypeUID=<%= assetType.UID %>&AssetUID=<%= asset.UID %>">Back to asset view</a>
            <h3>Items rest (location)</h3>
            <asp:GridView ID="SubtotalGridByLocation" runat="server" AutoGenerateColumns="False" >
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="{0}" DataNavigateUrlFields="LocationUrl" DataTextField="LocationName" HeaderText="Location" />
                    <asp:BoundField DataField="Rest" HeaderText="Rest" />
                </Columns>         
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No rest items in stock</asp:Label>                </EmptyDataTemplate>
            </asp:GridView>
            <h3>Items rest (price)</h3>
            <asp:GridView ID="SubtotalGrid" runat="server" AutoGenerateColumns="False" 
                meta:resourcekey="SubtotalGridResource1">                
                <Columns>
                    <asp:BoundField DataField="Price" HeaderText="Price" 
                        meta:resourcekey="BoundFieldResource1" />
                    <asp:BoundField DataField="Count" HeaderText="Count" 
                        meta:resourcekey="BoundFieldResource2" />
                </Columns>         
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No rest items in stock</asp:Label>                    
                </EmptyDataTemplate>
            </asp:GridView>
            
            <h3><asp:Label runat="server" meta:resourcekey="LabelResource2">Transactions list</asp:Label></h3>
            <asp:GridView ID="TransactionsList" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="UID" meta:resourcekey="TransactionsListResource1">    
                <Columns>
                    <asp:BoundField DataField="TransactionType" HeaderText="Type" 
                        meta:resourcekey="BoundFieldResource3" />
                    <asp:BoundField DataField="TransactionDate" HeaderText="Date" 
                        meta:resourcekey="BoundFieldResource4" />
                    <asp:BoundField DataField="StockCount" HeaderText="Count" 
                        meta:resourcekey="BoundFieldResource5" />
                    <asp:BoundField DataField="RestCount" HeaderText="Rest" 
                        meta:resourcekey="BoundFieldResource6" />
                    <asp:BoundField DataField="StockPrice" HeaderText="Price" 
                        meta:resourcekey="BoundFieldResource7" />
                    <asp:BoundField DataField="CloseDate" HeaderText="Closed" 
                        meta:resourcekey="BoundFieldResource8" />
                    <asp:BoundField DataField="Description" HeaderText="Description" 
                        meta:resourcekey="BoundFieldResource9" />
                </Columns>      
                <EmptyDataTemplate>
                    <asp:Label runat="server" meta:resourcekey="LabelResource3">No transactions</asp:Label>                    
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
