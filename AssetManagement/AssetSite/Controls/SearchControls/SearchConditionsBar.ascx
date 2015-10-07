<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConditionsBar.ascx.cs" Inherits="AssetSite.Controls.SearchControls.SearchConditionsBar" %>
 <div class="panel" id="searchBacklinkHolder" runat="server">
    <div class="panelSearch">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td style="width: 140px;">
                <asp:Label ID="lblSearchForText" runat="server" style="color: Gray;"  meta:resourcekey="lblSearchForText">Your last search:</asp:Label>
            </td>
            <td>
                <div id="searchConditions">
                    <img src="/images/ajax-loader-bar.gif" alt="loading..." />
                </div>
            </td>
            <td style="width: 150px; text-align:right;">
                <asp:HyperLink runat="server" ID="backToSearchButton" style="color: Gray;" meta:resourcekey="linkBackToSearch">Back to search</asp:HyperLink>
            </td>
        </tr>
    </table>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        jQuery.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: '{searchId:<%= SearchId %>}',
            dataType: 'json',
            url: '/amDataService.asmx/GetSearchConditions',
            success: function (result) {
                $('#searchConditions').empty().append(result.d);
            }
        });
    });
</script>