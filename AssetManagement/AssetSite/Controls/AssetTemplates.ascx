<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetTemplates.ascx.cs" Inherits="AssetSite.Controls.AssetTemplates" %>

<div class="active">
    <asp:Label ID="lblTemplates" runat="server" meta:resourcekey="LabelResourceTemplated"></asp:Label>
</div>
<span style="font-size: smaller;">
    <asp:Literal ID="litTemplates" runat="server"></asp:Literal>
    <asp:Repeater ID="repTemplates" runat="server">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li id="<%#GetTableId(Eval("ID")) %>">
                <div style="display: none;" id='<%#GetDivId(Eval("ID")) %>'>
                    <%#GetAssetHtml(Eval("Template")) %>
                </div>
                <a href='<%#GetRestoreUrl(Eval("ID")) %>' class="assetTemplatePopupTrigger" style="cursor: pointer;"
                    rel='<%#Eval("ID") %>'>
                    <%# string.IsNullOrEmpty((string)Eval("Template.Name")) ? "Template #" + Eval("ID") : Eval("Template.Name") %></a>&nbsp; 
                    <% if (IsEditable())
                       { %>
                    <a style="cursor: pointer; border: 0;" onclick='<%#GetClickScript(Eval("ID")) %>$("#<%#GetTableId(Eval("ID")) %>").remove();'>
                        <asp:Image runat="server" ImageUrl="~/images/buttons/delete.png" ImageAlign="Middle"
                            ToolTip="<%$ Resources:Global, DeleteText %>" />
                    </a>
                    <% } %>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</span>