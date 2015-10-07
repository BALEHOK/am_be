<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageSearchResult.master" AutoEventWireup="true" CodeBehind="ResultByContext.aspx.cs" Inherits="AssetSite.Search.NewResultByContext" %>
<%@ Register Src="~/Controls/SearchControls/SearchConditionsBar.ascx" TagName="SearchConditionsBar" TagPrefix="uc1" %>
<asp:Content ContentPlaceHolderID="SearchTopBar" runat="server">
    <uc1:SearchConditionsBar runat="server" />
</asp:Content>