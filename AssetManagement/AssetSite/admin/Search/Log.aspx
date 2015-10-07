<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="AssetSite.admin.Search.Log" %>
<%@ Import Namespace="AppFramework.Core.Classes.SearchEngine.Enumerations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Breadcrumb" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
<div id="main-container">   
    <div class="panel" style="margin-top: 5px;">
        <div class="panelheader">
            <asp:Label runat="server">Search tracking</asp:Label>
        </div>
        <div class="panelcontent" style="padding: 1px;"> 
            <asp:GridView 
                ID="Grid1"
                AutoGenerateColumns="false"
                DataKeyNames="Id"
                DataSourceID="SearchTrackingDataSource"
                AllowPaging="true"
                AllowSorting="true"
                ShowFooter="false"
                OnRowDataBound="Grid1_RowDataBound"
                EmptyDataText="<% $Resources:Global, ListIsEmpty %>" 
                runat="server">
                <Columns>
                    <asp:TemplateField SortExpression="SearchType" HeaderText="Search Type">
                        <ItemTemplate>
                            <asp:Literal runat="server" Text='<%# GetSearchType((short)Eval("SearchType")) %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Result Count" DataField="ResultCount" SortExpression="ResultCount" />
                    <asp:BoundField DataFormatString="{0:MMMM d, yyyy HH:mm:ss}"  HeaderText="Date" DataField="UpdateDate" SortExpression="UpdateDate" />
                    <asp:HyperLinkField 
                        HeaderText="User" 
                        DataNavigateUrlFields="UpdateUser" 
                        DataNavigateUrlFormatString="~/Asset/View.aspx?assetID={0}&assetTypeId=1" 
                        Text="User's profile"
                        SortExpression="UpdateUser" />
                     <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:HyperLink 
                                NavigateUrl='<%# GetSearchUrl((short)Eval("SearchType")) + "?Track=" + Eval("Id") %>' 
                                runat="server">Show Results</asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:EntityDataSource runat="server" ID="SearchTrackingDataSource"                
                DefaultContainerName="DataEntities"    
                ConnectionString="name=DataEntities" 
                OrderBy="it.Id DESC"                                              
                EntitySetName="SearchTracking">
            </asp:EntityDataSource>
       
        </div>
    </div>
</div>
</asp:Content>

<asp:Content  ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="item pointer" onclick="javascript:location.href='/admin/Search/'">
            <asp:HyperLink runat="server" NavigateUrl="~/admin/Search/">Search options</asp:HyperLink>             
        </div>
        <div class="active">
           <asp:Label runat="server">Search tracking</asp:Label>
        </div>
        <div class="item pointer" onclick="javascript:location.href='/admin/ServiceOps.aspx'">
            <asp:HyperLink runat="server" NavigateUrl="~/admin/ServiceOps.aspx">Service Operations</asp:HyperLink>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderRightColumn" runat="server">
</asp:Content>
