<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynListEditDialog.ascx.cs"
    Inherits="AssetSite.Controls.DynListEditDialog" %>
<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:LinkButton ID="LinkButton1" runat="server" Style="display: none;">
   LinkButton 
</asp:LinkButton>
<script type="text/javascript">
    function showTranslationsDialog() {
        $('#translationsSelector').val($('#<%=tbDynListItemName.ClientID%>').val());
        showTranslations();
    }
</script>
<div runat="server" id="dialog">
    <table border="0" cellpadding="0" cellspacing="0" width="360px">
        <tr>
            <td>
                <asp:PlaceHolder runat="server" ID="DynListItems"></asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td>
                <div style="white-space: nowrap;">
                    <asp:TextBox ID="tbDynListItemName" runat="server"></asp:TextBox>&nbsp;&nbsp;<a href="javascript:void(0)"
                        onclick="showTranslationsDialog()">Add translation</a><a id="lbtnAddItem" runat="server"
                            style="cursor: pointer; border: 0px;text-decoration:none;">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/plus.png" style="margin-right:-5px;" />
                        </a>
                    <br />
                    <asp:CheckBox ID="cbAssociated" runat="server" Text="Associated with list" /><br />
                    <asp:DropDownList ID="lstAssociatedLists" runat="server" DataTextField="Name" Width="350px"
                        DataValueField="UID" meta:resourcekey="DListResource1">
                    </asp:DropDownList>
                </div>
            </td>
        </tr>
    </table>
    <input type="hidden" id="translationsSelector" />
    <amc:Translations ID="tDynList" ControlSelector="#translationsSelector" runat="server" />
</div>
<a runat="server" id="editBtn" style="border: 0; cursor: pointer;" href="javascript:void(0);">
    <img alt="edit" class='editDL' src='/images/buttons/edit.png' /></a> 