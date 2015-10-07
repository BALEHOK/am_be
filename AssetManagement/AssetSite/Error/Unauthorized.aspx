<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Unauthorized.aspx.cs" Inherits="AssetSite.Error.Unauthorized" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Error</title>
    <link rel="stylesheet" type="text/css" href="~/css/layout_1200.css" />
    <link rel="stylesheet" type="text/css" href="~/css/theme_1200.css" />
    <link href="~/css/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    <link href="~/css/general.css" rel="stylesheet" type="text/css" />
    <link href="~/css/tooltip.css" rel="stylesheet" type="text/css" />
    <link href="~/css/print.css" media="print" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #source-order-container
        {
            margin: 0;
        }
    </style>
    <link href="~/App_Themes/Default/common.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="page-container">
        <div id="masthead">
            <div id="mastheadwrapper">
                <div id="headright" style="float: right;">
                </div>
                <div id="headleft">
                    <a href="/">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/companylogo.gif"></asp:Image>
                    </a>
                </div>
                <div id="menubar" class="clear toleft" style="width: 100%;">
                    <div id="menubarleft">
                        <div class="toleft BuildText">
                            <span>SOB en BUB Build</span>
                        </div>
                        <div id="homemailicons" class="toright">
                            <a href="/FAQ.aspx" id="faqIcon">
                                <img src="/images/faq_normal.jpg" alt="faq" />
                            </a><a href="/Contact.aspx" id="mailIcon">
                                <img src="/images/mail_normal.jpg" alt="mail" />
                            </a>
                        </div>
                    </div>
                    <!-- TopMenu -->
                    <asp:Panel runat="server" ID="menuLeft" CssClass="toleft headmenu" meta:resourcekey="menuLeftResource1">
                        <asp:HyperLink ID="menuSearch" runat="server" NavigateUrl="~/Search/Search.aspx"
                            meta:resourcekey="menuSearchResource1">Search</asp:HyperLink>
                        <asp:HyperLink ID="menuTasks" runat="server" NavigateUrl="~/TaskView.aspx"
                            meta:resourcekey="menuTasksResource">Tasks</asp:HyperLink>
                        <asp:HyperLink ID="menuCategories" runat="server" NavigateUrl="~/AssetView.aspx"
                            meta:resourcekey="menuCategoiresResource">Categories</asp:HyperLink>
                        <asp:HyperLink ID="menuDocuments" runat="server" NavigateUrl="~/Documents/Default.aspx"
                            meta:resourcekey="menuDocumentsResource1">Documents</asp:HyperLink>
                        <asp:HyperLink ID="menuFinancial" runat="server" NavigateUrl="/Financial/Default.aspx"
                            meta:resourcekey="menuFinancialResource1">Financial</asp:HyperLink>
                        <asp:HyperLink ID="menuReport" runat="server" NavigateUrl="~/Reports/Default.aspx"
                            meta:resourcekey="menuReportResource1">Reports</asp:HyperLink>
                        <asp:HyperLink ID="menuLending" runat="server" NavigateUrl="~/Reservations/Overview.aspx"
                            meta:resourcekey="menuLengingResource1">Reservation</asp:HyperLink>
                    </asp:Panel>
                    <div id="menubarright">
                        <div class="toleft headmenu">
                        </div>
                        <div class="toright headmenu">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="breadcrumb">
        </div>
        <h2>
            Error</h2>
        <p>
            You have insufficient permissions to access this application.
        </p>
        <div id="outer-column-container">
        </div>
    </div>
    <div class="push">
    </div>
    </form>
    <div id="footer">
        <div class="copyright inside">
            &copy;&nbsp;2012&nbsp;<span id="ctl00_lblCopyright">ACV-CSC METEA</span>
        </div>
    </div>
</body>
</html>
