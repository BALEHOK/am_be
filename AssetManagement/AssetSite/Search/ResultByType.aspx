<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageSearchResult.master" AutoEventWireup="true" CodeBehind="ResultByType.aspx.cs" Inherits="AssetSite.Search.NewResultByType" %>
<%@ Register Src="~/Controls/SearchControls/SearchConditionsBar.ascx" TagName="SearchConditionsBar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/UpdateParametersPanel.ascx" TagName="UpdateParametersPanel" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="ReportsPanel" Src="~/Controls/ReportsPanel.ascx" %>

<asp:Content ContentPlaceHolderID="SearchTopBar" runat="server">
<%--    <%if (Roles.IsUserInRole("Administrators")) { %>--%>
    <uc1:UpdateParametersPanel runat="server" ID="replaceButtons"/>
<%--    <%} %>--%>
    <uc1:SearchConditionsBar runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="LeftPanelControlsPlaceholder" runat="server">
    <uc1:ReportsPanel runat="server" ID="ReportsPanel" Visible="false" />
</asp:Content>