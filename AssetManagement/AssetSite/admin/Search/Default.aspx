<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Search.Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
     <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
<div id="main-container">        
    <div class="panel" style="margin-top: 5px;">
        <div class="panelheader">
            <asp:Label runat="server">Search options</asp:Label>
        </div>
        <div class="panelcontent" style="padding: 1px;"> 
            <table width="100%">
                <tr>
                    <td>
                        <asp:Label runat="server">Exclude words from queries (comma or space separated):</asp:Label> <br />
                        <asp:TextBox runat="server" ID="txtExcludeWords" TextMode="MultiLine" Columns="40" Rows="5"></asp:TextBox>                
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="Save" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</asp:Content>

<asp:Content  ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label runat="server">Search options</asp:Label>
        </div>
<%--        <div class="item pointer" onclick="javascript:location.href='/admin/Search/Log.aspx'">
           <asp:HyperLink runat="server" NavigateUrl="~/admin/Search/Log.aspx">Search tracking</asp:HyperLink> 
        </div>
        <div class="item pointer" onclick="javascript:location.href='/admin/ServiceOps.aspx'">
            <asp:HyperLink runat="server" NavigateUrl="~/admin/ServiceOps.aspx">Service Operations</asp:HyperLink>
        </div>--%>
    </div>
</asp:Content>