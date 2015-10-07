<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="Render.aspx.cs" Inherits="AssetSite.Reports.Render" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.XtraReports.Web" Assembly="DevExpress.XtraReports.v15.1.Web, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderLeftColumn">
    <% if (Report.ReportType == AppFramework.Reports.ReportType.AssetsListReport)
       { %>
    <div class="panel">
        <div class="panelheader">Report parameters</div>
        <div class="panelcontent">
            <asp:ListBox
                runat="server"
                ID="lbAssetTypes"
                DataTextField="Name"
                DataValueField="ID"
                OnSelectedIndexChanged="OnAssetTypeSelected"
                Width="100%"
                Height="200"
                AutoPostBack="True" />
        </div>
    </div>
    <% }  %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="panel">
        <div class="panelheader">Report</div>
        <div class="panelcontent">
            <dx:ASPxDocumentViewer Visible="False" ID="ReportViewer" runat="server"/>
        </div>
    </div>
</asp:Content>
