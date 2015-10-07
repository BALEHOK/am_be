<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListItems.aspx.cs" Inherits="AssetSite.DynList.ListItems"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <script type="text/javascript">
        function MoveOption(listId, mv) {
            var selList = $('select#' + listId);
            var selIdx = selList.attr('selectedIndex');
            var newIdx = selIdx + mv;
            if (newIdx < 0) newIdx = 0;
            if (newIdx >= selList.attr('options').length - 1) newIdx = selList.attr('options').length - 1;

            var opt1 = selList.attr('options')[selIdx];
            var opt2 = selList.attr('options')[newIdx];
            var tmp = $(opt1).clone()[0];

            opt1.text = opt2.text;
            opt1.value = opt2.value;
            opt2.text = tmp.text;
            opt2.value = tmp.value;

            opt1.selected = "";
            opt2.selected = "selected";
        }
        function MoveFirst(listid) {
            MoveOption(listid, -10000);
            return false;
        }
        function MoveUp(listid) {
            MoveOption(listid, -1);
            return false;
        }
        function MoveDown(listid) {
            MoveOption(listid, 1);
            return false;
        }
        function MoveLast(listid) {
            MoveOption(listid, 10000);
            return false;
        }

        function arr(listId, soId) {
            var opt = $('select#' + listId).attr('options');
            var ids = new Array();
            $('select#' + listId + ' > option').each(function (i) { ids.push(this.value) });
            $('#' + soId).val(ids)
        }

        function SelForDelete(listId, dlId) {
            var ids = new Array();
            $('select#' + listId + ' > option:selected').each(function (i) { ids.push(this.value) });
            $('#' + dlId).val(ids)
        }
    </script>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Items</asp:Label>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:ListBox runat="server" ID="ItemsList" DataTextField="Value" DataValueField="UID"
                            CssClass="ItemsList" SelectionMode="Multiple" meta:resourcekey="ItemsListResource1">
                        </asp:ListBox>
                    </td>
                    <td>
                        <input type="image" src="../../images/buttons/sort_first.png" alt="First" onclick="return MoveFirst('<%= ItemsList.ClientID %>')" /><br />
                        <input type="image" src="../../images/buttons/sort_up.png" alt="Up" onclick="return MoveUp('<%= ItemsList.ClientID %>')" /><br />
                        <input type="image" src="../../images/buttons/sort_down.png" alt="Down" onclick="return MoveDown('<%= ItemsList.ClientID %>')" /><br />
                        <input type="image" src="../../images/buttons/sort_last.png" alt="Last" onclick="return MoveLast('<%= ItemsList.ClientID %>')" /><br />
                    </td>
                </tr>
            </table>
            <p>
                <asp:HyperLink runat="server" ID="AddNewUrl" Text="Add new" meta:resourcekey="AddNewUrlResource1"></asp:HyperLink>
                <asp:LinkButton runat="server" ID="linkEditItem" Text="Edit" OnClick="linkEditItem_Click"
                    meta:resourcekey="EditDynListItem"></asp:LinkButton>
                <asp:LinkButton runat="server" ID="DelSel" Text="DeleteSelected" OnClick="DelSelClicked"
                    meta:resourcekey="DelSelResource1"></asp:LinkButton>
            </p>
        </div>
        <asp:HiddenField runat="server" ID="SortOrder" />
        <asp:HiddenField runat="server" ID="DelIds" />
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="Save" runat="server" OnClick="Save_Click" Text="Finished" meta:resourcekey="SaveResource1" />
    </div>
</asp:Content>
