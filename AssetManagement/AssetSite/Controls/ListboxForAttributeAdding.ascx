<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListboxForAttributeAdding.ascx.cs"
    Inherits="ListboxForAttributeAdding" %>
<div>    
    <table cellspacing="0" cellpadding="0" class="wh100p">
        <tr>
            <td style="vertical-align: middle;">
                <div class="buttons_p">
                    <p><asp:ImageButton ImageUrl="~/images/buttons/addarrow.png" ID="imgbtnRigh" runat="server"
                        OnClick="btnRight_Click" /></p>
                    <p><asp:ImageButton ImageUrl="~/images/buttons/deletearrow.png" ID="imgbtnLeft" runat="server"
                        OnClick="btnLeft_Click" /></p>
                </div>
            </td>
            <td>
                <div id="listbox" class="border1">
                    <div class="hdr">
                        <asp:Label runat="server" ID="PanelTitle"></asp:Label>
                    </div>
                    <div class="listAttr_div">
                        <asp:ListBox ID="listboxControl" runat="server" SelectionMode="Multiple" CssClass="listAttrubutes_s">
                        </asp:ListBox>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <br />
</div>
