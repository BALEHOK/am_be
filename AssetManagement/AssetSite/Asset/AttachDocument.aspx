<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttachDocument.aspx.cs"
    Inherits="AssetSite.Asset.AttachDocument" Theme="Default" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/AssetAttributePanels.ascx" TagPrefix="amcl" TagName="AssetAttributePanels" %>
<%@ Register TagPrefix="aspJ" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../javascript/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../javascript/jquery-ui-1.8.7.custom.min.js" type="text/javascript"></script>
    <script src="../javascript/utilities.js" type="text/javascript"></script>
    <script src="../javascript/DynListItem.js" type="text/javascript"></script>
    <link href="../css/theme.css" rel="stylesheet" type="text/css" />
    <link href="../css/general.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    <script src="../javascript/AssetsSearch.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <%--<asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Auto" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>--%>
    <aspJ:ToolkitScriptManager ID="ToolkitScriptManager2" runat="Server">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </aspJ:ToolkitScriptManager>
    <div id="page-container-popup">
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panetTitle" meta:resourcekey="panetTitleResource1">Attach document</asp:Label>
            </div>
            <div class="panelcontent">
                <asp:RadioButtonList runat="server" ID="DocumentAttachType" AutoPostBack="True" OnSelectedIndexChanged="DocumentAttachType_SelectedIndexChanged"
                    meta:resourcekey="DocumentAttachTypeResource1">
                    <asp:ListItem Text="Create new" Value="new" Selected="True" meta:resourcekey="ListItemResource1"></asp:ListItem>
                    <asp:ListItem Text="Select existing" Value="existing" meta:resourcekey="ListItemResource2"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="NewDocument">
            <amcl:AssetAttributePanels runat="server" ID="AssetAttributePanels" Editable="true" />
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="ExistingDocument" Visible="False">
            <script type="text/javascript">
                function IAmSelected(source, eventArgs) {
                    document.getElementById('<%= TextBoxHidden.ClientID %>').value = eventArgs.get_value();
                    var txtBx = document.getElementById("TextBoxHidden");
                }
            </script>
            <div class="panel">
                <div class="panelheader">
                    Select document
                </div>
                <div class="panelcontent">
                    <asp:Label runat="server" meta:resourcekey="lblStartTyping">Start typing to select a document...</asp:Label><br />
                   <%-- <asp:DropDownList runat="server" ID="DocumentsList" DataTextField="Value" DataValueField="Key"
                        meta:resourcekey="DocumentsListResource1">
                    </asp:DropDownList>--%>
                    <asp:TextBox ID="TextBoxTarget" runat="server" />
                    <asp:HiddenField ID="TextBoxHidden" runat="server" />
                    <aspJ:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" ServicePath="~/amDataService.asmx"
                        ServiceMethod="GetCompletionList" TargetControlID="TextBoxTarget" MinimumPrefixLength="0"
                        CompletionSetCount="10" CompletionInterval="100" OnClientItemSelected ="IAmSelected">
                    </aspJ:AutoCompleteExtender>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="wizard-footer-buttons">
            <asp:Button runat="server" OnClick="AttachClicked" Text="Attach" meta:resourcekey="ButtonResource1" />
        </div>
    </div>
    </form>
</body>
</html>
