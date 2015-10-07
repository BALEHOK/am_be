<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchTabs.ascx.cs" Inherits="AssetSite.Controls.SearchControls.SearchTabs" %>

<table class="searchTabs" cellpadding="0" cellspacing="0">
    <tr>
        <td <% if (ActiveSearchType == AppFramework.Core.Classes.SearchEngine.Enumerations.SearchType.SearchByKeywords) { %>class="activeButton"<% } %>>
            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Search/" meta:resourcekey="HyperLink4Resource1">Search</asp:HyperLink>
        </td>
        <td <% if (ActiveSearchType == AppFramework.Core.Classes.SearchEngine.Enumerations.SearchType.SearchByType) { %>class="activeButton"<% } %>>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Search/SearchByType.aspx" meta:resourcekey="HyperLink1Resource1">By Type</asp:HyperLink>
        </td>
        <td <% if (ActiveSearchType == AppFramework.Core.Classes.SearchEngine.Enumerations.SearchType.SearchByContext) { %>class="activeButton"<% } %>>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Search/SearchByContext.aspx" meta:resourcekey="HyperLink2Resource1">By Context</asp:HyperLink>
        </td>
        <td <% if (ActiveSearchType == AppFramework.Core.Classes.SearchEngine.Enumerations.SearchType.SearchByCategory) { %>class="activeButton"<% } %>>
            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Search/SearchByCategory.aspx" meta:resourcekey="HyperLink3Resource1">By Category</asp:HyperLink>
        </td>
        <%--<td valign="middle">
                        <asp:HyperLink ID="HyperLink6" runat="server" 
                            NavigateUrl="SearchByDocuments.aspx" meta:resourcekey="HyperLink6Resource1">Documents</asp:HyperLink>
                    </td>--%>

        <%--<%  if (AppFramework.Core.Classes.ApplicationSettings.ApplicationType != AppFramework.ConstantsEnumerators.ApplicationType.SOBenBUB)
                        { %>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink5" runat="server" 
                                    NavigateUrl="SearchByBarCode.aspx" meta:resourcekey="HyperLink5Resource1">BarCode</asp:HyperLink>
                            </td>
                    <%} %>--%>
    </tr>
</table>
