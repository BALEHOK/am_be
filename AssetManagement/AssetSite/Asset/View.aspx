<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    AutoEventWireup="True" Inherits="AssetSite.Asset.View" CodeBehind="View.aspx.cs"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/NewAttributePanel.ascx" TagName="NewAttributePanel"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/AssetToolbar.ascx" TagName="AssetToolbar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/SearchControls/SearchConditionsBar.ascx" TagName="SearchConditionsBar"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/TasksPanel.ascx" TagName="TasksPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ChildAssets.ascx" TagName="ChildAssets" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreensPanel.ascx" TagName="ScreensPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ReportsPanel.ascx" TagName="ReportsPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreenDetails.ascx" TagName="ScreenDetails" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog"
    TagPrefix="amc" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        @media print
        {
            div#logoffblock
            {
                display: none;
            }
            div.toolbar
            {
                display: none;
            }
            div#menubar
            {
                display: none;
            }
            div#breadcrumb
            {
                display: none;
            }
        }
        .active
        {
            background: #9933CC url('/images/arrowup.png') no-repeat 210px 10px;
            font-weight: bold;
            color: #ffffff;
            background-color: #5EBA2F;
            padding: 5px 5px 5px 10px;
            font-size: 12px;
            margin: 5px 0px 5px 0px;
        }
    </style>
    <link href="<%= LayoutCssClass() %>" rel="stylesheet" type="text/css" />
    <link href="/css/fancybox/jquery.fancybox-1.3.4.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="PlaceHolderSearchBox">
    <uc1:SearchConditionsBar runat="server" />
</asp:Content>
<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.active').click(function () {
                var div = $(this).next('div');
                if (div.is(":hidden")) {
                    // show
                    div.slideDown("slow");
                    $(this).css('backgroundImage', "url('/images/arrowup.png')");
                } else {
                    // hide
                    div.slideUp();
                    $(this).css('backgroundImage', "url('/images/arrowdown.png')");
                }
            });
        });
    </script>
    <div class="wizard-menu">
        <uc1:TasksPanel runat="server" ID="tasksPanel" />
        <uc1:ScreensPanel runat="server" ID="screensPanel" />
        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="lblCategories">Categories</asp:Label>
        </div>
        <div style="font-size: smaller;">
            <asp:Literal ID="litCategoryPath" runat="server"></asp:Literal>
        </div>
        <div class="active">
            <asp:Label ID="Label4" runat="server" meta:resourcekey="lblTaxonomies">Taxonomies</asp:Label>
        </div>
        <div style="font-size: smaller;">
            <asp:Literal ID="litTaxonomies" runat="server"></asp:Literal>
        </div>
        <div class="active">
            <asp:Label ID="Label5" runat="server" meta:resourcekey="lblLinkedAssets">Linked Assets</asp:Label>
        </div>
        <div style="font-size: smaller;">
            <asp:Literal ID="litLinkedAssets" runat="server"></asp:Literal>
        </div>
        <uc1:ChildAssets runat="server" ID="caPanel" Visible="false" />
        <uc1:ReportsPanel runat="server" ID="ReportsPanel" />
        <div class="active">
            <asp:Label ID="Label6" runat="server" meta:resourcekey="lblExport">Export</asp:Label>
        </div>
        <div style="font-size: smaller;">
            <asp:LinkButton ID="lbtnExportToTxt" runat="server" OnClick="OnlbtnExportToTxt_Click"
                meta:resourcekey="lbtnExportToTxt">Export to txt</asp:LinkButton><br />
            <asp:LinkButton ID="lbtnExportToXml" runat="server" OnClick="OnlbtnExportToXml_Click"
                meta:resourcekey="lbtnExportToXml">Export to xml</asp:LinkButton><br />
            <asp:LinkButton ID="lbtnExportToDoc" runat="server" OnClick="OnlbtnExportToDoc_Click"
                meta:resourcekey="lbtnExportToDoc">Export to doc</asp:LinkButton><br />
            <asp:LinkButton ID="lbtnExportAndZip" runat="server" OnClick="OnlbtnExportToZip_Click"
                meta:resourcekey="lbtnExportAndZip">Export all and zip</asp:LinkButton>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
    <asp:ScriptManager runat="server" ID="ScriptManager1">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <script type="text/javascript" language="javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        var thebutton;
        var addbutton;

        function BeginRequestHandler(sender, args) {
            thebutton = args.get_postBackElement();
            addbutton = $get('<%= AddButton.ClientID %>');
            addbutton.disabled = true;
            thebutton.disabled = true;
        }

        function EndRequestHandler(sender, args) {
            thebutton.disabled = false;
            addbutton.disabled = false;
        }

        function MakeDescription() {
            var descr = $('#<%= Description.ClientID %>');
            var dep = $('#<%= UsersList.ClientID %> :selected');

            descr.text('Moved to ' + dep.text() + '');
        }
    </script>
    <div id="main-container">
        <uc1:AssetToolbar ID="toolbar" runat="server" />
        <hr />
        <div style="display: none;" id='templateContainer123'>
            <asp:Literal ID="litreservationInfo" runat="server"></asp:Literal>
        </div>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td class="wizard-header">
                    <asp:Literal ID="litAssetName" runat="server"></asp:Literal>
                </td>
                <td style="font-size: smaller; color: Gray;">
                    &nbsp;&nbsp;&nbsp;<asp:Literal ID="litRevision" runat="server"></asp:Literal>&nbsp;<asp:Literal
                        ID="litReserved" runat="server"></asp:Literal>
                </td>
                <% if (Asset != null && Asset.IsDeleted)
                   { %>
                <td>
                    &nbsp;<asp:Label runat="server" ForeColor="Red" Font-Bold="true" Text="Deleted" />
                </td>
                <% } %>
            </tr>
        </table>
        <span style="color: Red; font-size: larger;">
            <asp:Literal ID="litAssetError" runat="server"></asp:Literal>
        </span>
        <div id="DialogContainer" runat="server" title="Documents" style="display: none;">
            <asp:Repeater ID="repDocuments" runat="server">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkDocument" runat="server" Text='<%#Eval("Name") %>' NavigateUrl='<%#Eval("NavigateUrl") %>'></asp:HyperLink><br />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <uc1:ScreenDetails runat="server" ID="sdAssetView" />
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_OnItemDataBound">
            <ItemTemplate>
                <uc1:NewAttributePanel 
                    ID="Panel1" 
                    runat="server" 
                    Header='<%# Bind("Key.Name") %>'
                    CssClass="panels_leftcol_container" 
                    Editable="false" 
                    AssignedAttributes='<%# DataBinder.Eval(Container.DataItem,"Value") %>' />
            </ItemTemplate>
            <AlternatingItemTemplate>
                <uc1:NewAttributePanel 
                    ID="Panel1" 
                    runat="server" 
                    Editable="false" 
                    Header='<%# Bind("Key.Name") %>'
                    CssClass="panels_rightcol_container" 
                    AssignedAttributes='<%# DataBinder.Eval(Container.DataItem,"Value") %>' />
                <asp:Panel runat="server" CssClass="separator" Visible="<%# IsSeparatorVisible() %>"
                    meta:resourcekey="PanelResource1">
                </asp:Panel>
            </AlternatingItemTemplate>
        </asp:Repeater>
        <div style="clear: both;">
        </div>
        <asp:PlaceHolder ID="Transactions" runat="server" Visible="false">
            <div class="panel">
                <div class="panelheader">
                    <asp:Label runat="server" ID="panelHeader">Transactions</asp:Label>
                </div>
                <div class="panelcontent">
                    <table>
                        <tr>
                            <td>
                                <a href='ViewTransactions.aspx?AssetUID=<%= Asset.UID %>&AssetTypeUID=<%= AssetType.UID %>'>
                                    <asp:Label ID="Label1" runat="server">Recent transactions</asp:Label></a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server">Transaction type</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="TransactionTypes" DataTextField="Name" DataValueField="UID"
                                    AutoPostBack="true" OnSelectedIndexChanged="TransactionTypeSelectedChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server">Items count</asp:Label>
                            </td>
                            <td>
                                <p><asp:TextBox runat="server" ID="TotalCount"></asp:TextBox></p>
                                <asp:Literal runat="server" ID="CountErrorMsg"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="color: red; font-weight: bold">
                                <asp:Literal ID="ValidationReport" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TransactionTypes" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="UsersList" EventName="SelectedIndexChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:PlaceHolder ID="UserSelector" runat="server" Visible="false">
                                <table>
                                    <tr id="moveToDep">
                                        <td>
                                            <asp:Label runat="server">Move to:</asp:Label>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server">User:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="UsersList" runat="server" DataTextField="Value" DataValueField="Key" OnDataBound="DepartmentListDataBound" AutoPostBack="true">
                                                <asp:ListItem Value="Select..."></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="LocationSelector" runat="server" Visible="false">
                                <table>
                                    <tr>
                                        <td>Location</td>
                                        <td>
                                            <asp:DropDownList ID="TransactionLocationsList" runat="server" DataTextField="Value" DataValueField="Key" OnDataBound="TransactionLocationsListDataBound" AutoPostBack="true">
                                                <asp:ListItem Value="Select..."></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="FromLocationSelector" runat="server" Visible="false">
                                <table>
                                    <tr>
                                        <td>From location</td>
                                        <td>
                                            <asp:DropDownList ID="FromLocationsList" runat="server" DataTextField="Value" DataValueField="Key" OnDataBound="TransactionLocationsListDataBound" OnSelectedIndexChanged="FromLocationsList_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Text="Select..." Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="CountInfo"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PriceInfo" runat="server" Visible="true">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server">Items price</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TotalPrice" runat="server"></asp:TextBox><asp:Literal runat="server"
                                                ID="PriceErrorMsg"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                            </asp:PlaceHolder>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server">Description</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox TextMode="MultiLine" runat="server" ID="Description"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <p>
                        <asp:Literal ID="AssetErr" runat="server"></asp:Literal></p>
                    <asp:Button ID="AddButton" runat="server" OnClick="OnAddTransactionClicked" Text="Add" />
                </div>
            </div>
        </asp:PlaceHolder>
        <hr />
        <uc1:AssetToolbar ID="bottomtoolbar" runat="server" />
    </div>
</asp:Content>
