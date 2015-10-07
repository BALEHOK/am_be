<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/MasterPages/MasterPageBase.master"
CodeBehind="SearchByDocuments.aspx.cs" Inherits="AssetSite.Search.SearchByDocuments" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
    <link href="../css/Search.css" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:Literal ID="LiteralSearch" runat="server" Text="Search" 
        meta:resourcekey="LiteralSearchResource1"></asp:Literal> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <div class="reglament">
            <div class="panelSearchHome">
                <div style="float:left;">
                    <table class="noneActiveButton" cellpadding="0" cellspacing="0">
                        <tr>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink4" runat="server" 
                                    NavigateUrl="Search.aspx" 
                                    meta:resourcekey="HyperLink4Resource1">Search</asp:HyperLink>
                            </td>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="SearchByType.aspx" 
                                    meta:resourcekey="HyperLink1Resource1">By Type</asp:HyperLink>
                            </td>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink2" runat="server" 
                                    NavigateUrl="SearchByContext.aspx" meta:resourcekey="HyperLink2Resource1">By Context</asp:HyperLink>
                            </td>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink3" runat="server" 
                                    NavigateUrl="SearchByCategory.aspx" meta:resourcekey="HyperLink3Resource1">By Category</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="float:left;">
                    <table class="activeButton" cellpadding="0" cellspacing="0">
                        <tr>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="#" 
                                    meta:resourcekey="HyperLink5Resource1">Documents</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="float:left;">
                    <table class="noneActiveButton" cellpadding="0" cellspacing="0">
                        <tr>
                        <%  if (AppFramework.Core.Classes.ApplicationSettings.ApplicationType != AppFramework.ConstantsEnumerators.ApplicationType.SOBenBUB)
                            { %>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink6" runat="server" 
                                    NavigateUrl="SearchByBarCode.aspx" meta:resourcekey="HyperLink6Resource1">BarCode</asp:HyperLink>
                            </td>
                            <%} %>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="float:left;" class="greenBottom"></div>
            <br />
            <br />
            <div style="float:none;">
                
                <table width="100%" class="searcher">
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="tbSearch" runat="server" Width="100%" 
                                meta:resourcekey="tbSearchResource1"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td  align="center">
                            <asp:Button ID="ButtonSearch" runat="server" Text="Search" 
                                onclick="Button1_Click" meta:resourcekey="ButtonSearchResource1" />
                        </td>
                    </tr>
                        
                </table>
            </div>
     </div>            
</asp:Content>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderMainContent" runat="Server">
<div class="clear-columns">
<!-- do not delete --></div>
</asp:Content>