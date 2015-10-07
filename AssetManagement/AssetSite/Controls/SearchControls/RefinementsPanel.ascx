<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="RefinementsPanel.ascx.cs" Inherits="AssetSite.Controls.SearchControls.RefinementsPanel" %>
<div id="refinements" runat="server" class="refined">
    <h2><asp:Label runat="server" ID="lblRefinements" meta:resourcekey="lblYourRefinements"></asp:Label></h2>
    <asp:ListView runat="server" ID="AssetTypes">
        <ItemTemplate>
            <div><%# Eval("Name")%>&nbsp;(<a href="javascript:void(0);" onclick="UndoRefinement(<%# Eval("Id")%>, 'AssetType')"><%= GetLocalResourceObject("lblUndo") %></a>)</div>
        </ItemTemplate>
    </asp:ListView>
    <asp:ListView runat="server" ID="Taxonomies">
        <ItemTemplate>
            <div><%# Eval("Name")%>&nbsp;(<a href="javascript:void(0);" onclick="UndoRefinement(<%# Eval("Id")%>, 'Taxonomy')"><%= GetLocalResourceObject("lblUndo") %></a>)</div>
        </ItemTemplate>
    </asp:ListView>
    <script type="text/javascript">
        function UndoRefinement(id, type) {
            var paramName = type == "AssetType" ? "ConfigsIds" : "TaxonomyItemsIds";
            var ids = getParameterByName(paramName).split(',');
            ids.splice($.inArray(id.toString(), ids), 1)
            var url = $.url.build({
                params: {
                    'Params': getParameterByName('Params'),
                    /*'SearchId': getParameterByName("SearchId"),*/
                    'Time': getParameterByName('Time'),
                    'PageNumber': 1,
                    'PageSize': pageSize,
                    'DisplayMode': getParameterByName('DisplayMode'),
                    'OrderBy': getParameterByName('OrderBy'),
                    'ConfigsIds': type == "AssetType" ? ids.join(',') : getParameterByName('ConfigsIds'),
                    'TaxonomyItemsIds': type == "Taxonomy" ? ids.join(',') : getParameterByName('TaxonomyItemsIds')
                }
            });
            location.href = url;
        }
    </script>
</div>