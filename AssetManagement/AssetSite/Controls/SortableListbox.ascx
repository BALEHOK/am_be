<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SortableListbox.ascx.cs"
    Inherits="SortableListbox" %>
<div class="updpanel_div">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="<%= this.PanelUID %>" class="<%= this.CssClass %>">
                <table cellspacing="0" cellpadding="0" class="wh100p">
                    <tr>
                        <td class="border1">
                            <div class="hdr">
                                <asp:Label ID="PanelTitle" runat="server"></asp:Label>
                            </div>
                            <div class="listAttr_div">
                                <asp:HiddenField runat="server" ID="hfSortAttributes" />
                                <asp:ListBox ID="listAttributes" SelectionMode="Multiple" CssClass="listAttributes_w"
                                    runat="server"></asp:ListBox>
                            </div>
                        </td>
                        <td>
                            <div class="buttons_p">
                                <a title="Move up" onclick="MoveListBoxItemsUp('<%=listAttributes.ClientID %>')">
                                    <img style="width: 19px" src="../images/buttons/uparrow.png" />
                                </a><a title="Move down" onclick="MoveListBoxItemsDown('<%=listAttributes.ClientID %>')">
                                    <img style="width: 19px" src="../images/buttons/downarrow.png" />
                                </a>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
