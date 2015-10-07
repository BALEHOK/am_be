<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditListItem.aspx.cs" Inherits="AssetSite.DynList.EditListItem"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Debug" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        function switchAssoc() {
            if ($('#<%=AssocList.ClientID %>').css("display") == "none") {
                $('#<%=AssocList.ClientID %>').show();
            }
            else {
                $('#<%=AssocList.ClientID %>').hide();
            }
        }
        function showTranslationsDialog() {
            $('#translationsSelector').val($('#<%=NewValue.ClientID %>').val());
            showTranslations();
        }
    </script>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Items</asp:Label>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5">
                <tr>
                    <td width="200px">
                        <asp:Label runat="server" ID="lblValue" meta:resourcekey="lblValueResource1">Value</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="NewValue" runat="server" meta:resourcekey="NewValueResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="NewValue" Text="Value required"
                            meta:resourcekey="RequiredFieldValidatorResource1" ValidationGroup="addEtidDynListItem"></asp:RequiredFieldValidator>
                        <a href="javascript:void(0)" onclick="showTranslationsDialog()">Add translation</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox runat="server" ID="IsAssoc" Text="Associated with list" meta:resourcekey="IsAssocResource1" />
                    </td>
                </tr>
                <tr id="AssocList" runat="server">
                    <td>
                        <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1">Associated dynamic list</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DList" runat="server" DataTextField="Name" DataValueField="UID"
                            meta:resourcekey="DListResource1">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button runat="server" ID="AddItemButton" ValidationGroup="addEtidDynListItem" OnClick="AddItem" Text="Add" meta:resourcekey="AddItemButtonResource1" />
                        <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" meta:resourcekey="btnCancel" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="tDynList" ControlSelector="#translationsSelector" runat="server" />
</asp:Content>
