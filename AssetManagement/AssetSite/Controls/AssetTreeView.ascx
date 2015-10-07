<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetTreeView.ascx.cs"
    Inherits="AssetSite.Controls.AssetTreeView" %>
<asp:TreeView ID="AssetTree" runat="server" ExpandDepth="0" ShowLines="True" OnTreeNodePopulate="AssetTree_TreeNodePopulate"
    CssClass="treeview" SelectedNodeStyle-CssClass="selected" NodeStyle-CssClass="treenode"
    NodeWrap="true" OnSelectedNodeChanged="OnSelectedNodeChanged">
</asp:TreeView>
<asp:ImageButton ImageUrl="~/images/buttons/node.png" runat="server" CssClass="expandBtn"
    AlternateText="Expand / Collapse" ToolTip="Expand / Collapse" OnClick="ExpandCollapseClick" />
<script type="text/javascript">
    $(document).ready(function () {
        $('.expandBtn').hover(
            function () {
                $(this).attr('src', '/images/buttons/node-select-all.png');
            },
            function () {
                $(this).attr('src', '/images/buttons/node.png');
            }
            );
    }); 
</script>
