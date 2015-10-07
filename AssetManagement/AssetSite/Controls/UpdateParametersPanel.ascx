<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateParametersPanel.ascx.cs" Inherits="AssetSite.Controls.UpdateParametersPanel" %>
<%@ Register Src="~/Controls/SearchControls/SearchTabs.ascx" TagPrefix="uc1" TagName="SearchTabs" %>
<%@ Register Src="~/Controls/AssetDropDownListEx.ascx" TagName="AssetDropDownListEx" TagPrefix="amc" %>

<%@ Import Namespace="AppFramework.Core.Classes.SearchEngine.TypeSearchElements" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hfUid" runat="server" />

        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="width: 140px;">
                    <asp:Label ID="lblSearchForText" runat="server" style="color: Gray;"  meta:resourcekey="lblSearchForText">Replace data:</asp:Label>
                </td>
            </tr>
        </table>

        <asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" Visible="false"></asp:Label>

        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <table cellpadding="0" cellspacing="0" border="0" class="searchTable">
                    <tr class="TypeSearchHeader">
                        <td style="width: 200px;">
                            <asp:Label ID="lAttribute" runat="server" Text="Attribute" meta:resourcekey="lAttributeResource1"></asp:Label>
                        </td>
                        <td style="width: 200px;">
                            <asp:Label ID="lSearch" runat="server" Text="New value" meta:resourcekey="lSearchResource2"></asp:Label>
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlAttribs" runat="server" DataTextField="AttributeText"
                            DataValueField="AttributeValue" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
                            DataSource='<%# CurrentType.Attributes %>' CssClass='<%# Container.ItemIndex %>'
                            meta:resourcekey="ddlAttribsResource1">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <!-- Refactor this.... -->
                        <asp:DropDownList ID="ddlValues" runat="server" DataTextField="Key"
                            DataValueField="Value" DataSource='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].Items:null %>'
                            CssClass='<%# Container.ItemIndex %>' Visible='
                            <%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count > 0 && 
                                CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsDynListDropDown %>'
                            meta:resourcekey="ddlValuesResource1">
                        </asp:DropDownList>
                        <amc:AssetDropDownListEx registerstartupscript="true" runat="server" id="adlAssets" Visible='
                            <%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0 && 
                                CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsAssetListDropDown %>'
                            CssClass='<%# Container.ItemIndex %>' />
                        <asp:TextBox ID="tbValue" runat="server" Text='<%# Eval("Text") %>'
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count <= 0 ? false : !CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsAssetListDropDown &&!CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsDynListDropDown && CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Type != AppFramework.ConstantsEnumerators.Enumerators.DataType.Bool %>'
                            meta:resourcekey="tbValueResource1"></asp:TextBox>
                        <asp:DropDownList runat="server" ID="dlBoolean"
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Type == AppFramework.ConstantsEnumerators.Enumerators.DataType.Bool %>'>
                            <asp:ListItem Value="False">False</asp:ListItem>
                            <asp:ListItem Value="True">True</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:ImageButton OnClick="ImageButtonDel_Click" ID="ImageButton1" runat="server"
                            ImageUrl="/images/buttons/delete.jpg" CssClass='<%# Container.ItemIndex %>' AlternateText="D"
                            meta:resourcekey="ImageButton1Resource1" />
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("EndBrackets") %>' meta:resourcekey="Label3Resource1"></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="buttonNewRow" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
<div class="line">
    <%--text --%></div>
<table cellpadding="0" cellspacing="0">
    <tr>
        <td style="padding-bottom: 5px; width: 30px;">
            <asp:Button Width="90px" ID="Button2" runat="server" Text="Clear" OnClick="Button2_Click"
                meta:resourcekey="Button2Resource2" />
        </td>
        <td style="padding-bottom: 5px; padding-left: 10px; width: 30px;">
            <asp:Button Width="90px" ID="buttonNewRow" runat="server" Text="Add row" OnClick="buttonNewRow_Click"
                meta:resourcekey="buttonNewRowResource1" />
        </td>
    </tr>
</table>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:Button ID="updateButton" Text="Replace" OnClick="updateButton_Click" runat="server"/>
        </td>
    </tr>
</table>


<script type="text/javascript">
    function InitDatePicker(controlId) {
        var picker = $('#' + controlId);
        var initialValue = picker.val();
        picker.datepicker({
            showOn: 'button',
            buttonImage: '/images/buttons/calendar.png',
            buttonImageOnly: true,
            changeYear: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            yearRange: '1950:2020',
            beforeShow: function (input, inst) {
                inst.dpDiv.css({ marginTop: -input.offsetHeight + 'px', marginLeft: input.offsetWidth + 'px' });
            }
        });
        picker.datepicker('option', $.datepicker.regional['<%= Locale %>']);
        picker.datepicker("option", "dateFormat", "<%= DatePattern %>");
        picker.datepicker('setDate', initialValue);
        picker.val(initialValue);
    }
</script>
