<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Export.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.master" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/SearchParametersPanel.ascx" TagName="ExportParameters" TagPrefix="ctrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container">
        <div class="wizard-header">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Export</asp:Label>
        </div>
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Filter options</asp:Label>
            </div>
            <div class="panelcontent">
                <table>
                    <tr>
                        <td>
                            <ctrl:ExportParameters runat="server" id="exportParameters"></ctrl:ExportParameters>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <b><asp:Label ID="Label5" runat="server" SkinID="control" Text="Format:" meta:resourcekey="LabelFormatResource"></asp:Label></b>
                            <asp:RadioButtonList ID="dataSourceTypesList" CssClass="datasource" runat="server"
                                meta:resourcekey="dataSourceTypesListResource1">
                                <asp:ListItem Value="XLS" Selected="True" meta:resourcekey="ListItemResource2">Excel document</asp:ListItem>
                                <asp:ListItem Value="XML" meta:resourcekey="ListItemResource1">XML document</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>                    

                <script type="text/javascript">
                    function confirmExport() {
                        var isSelected = false;
                        $('.panelcontent select').each(function () {
                            isSelected |= parseInt($(this).val()) > 0 ? true : false;
                        });
                        if (!isSelected) {
                            if (confirm('Exporting of all assets can take some time. Do you want to continue?')) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            return true;
                        }
                    }                    
                </script>
                <br />
                <amc:MessagePanel runat="server" ID="messagePanel" meta:resourcekey="messagePanelResource1">
                </amc:MessagePanel>
            </div>
            <div class="panelcontent">
                <asp:Button ID="Button1" runat="server" SkinID="export" Text="Export assets" OnClientClick="return confirmExport()"
                    OnClick="GetAssets_Click" meta:resourcekey="ButtonResource1" />
            </div>
        </div>


        <div class="wizard-footer-buttons">
            <asp:Button ID="btnClose" runat="server" Text="<% $Resources:Global, CancelText %>"
                OnClick="btnClose_Click" />
        </div>
    </div>
</asp:Content>
