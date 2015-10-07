<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchParametersPanel.ascx.cs" Inherits="AssetSite.Controls.SearchParametersPanel" %>
<%@ Register Src="~/Controls/SearchControls/SearchTabs.ascx" TagPrefix="uc1" TagName="SearchTabs" %>
<%@ Register Src="~/Controls/AssetDropDownListEx.ascx" TagName="AssetDropDownListEx" TagPrefix="amc" %>
<%@ Import Namespace="AppFramework.Core.Classes.SearchEngine.TypeSearchElements" %>
<asp:ScriptManager runat="server" ID="scriptManager">
    <Services>
        <asp:ServiceReference Path="~/amDataService.asmx" />
    </Services>
</asp:ScriptManager>

<script language="javascript" type="text/javascript">
    function closePanel() {
        var panel = $("#<%=bracketsPanel.ClientID%>");
        panel.show();
    }
</script>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <table width="100%" class="categorySearcher">
            <tr>
                <td align="right" style="padding-right: 20px;">
                    <asp:HiddenField ID="hfUid" runat="server" />
                    <asp:Label ID="lType" runat="server" Text="Asset Type" meta:resourcekey="lTypeResource1"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlTypes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTypes_SelectedIndexChanged"
                        meta:resourcekey="ddlTypesResource1">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>

        <asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" Visible="false"></asp:Label>

        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <table cellpadding="0" cellspacing="0" border="0" width="100%" class="searchTable">
                    <tr class="TypeSearchHeader">
                        <td>
                        </td>
                        <td style="width: 20px;">
                        </td>
                        <td style="width: 20px;">
                        </td>
                        <td style="width: 200px;">
                            <asp:Label ID="lAttribute" runat="server" Text="Attribute" meta:resourcekey="lAttributeResource1"></asp:Label>
                        </td>
                        <td style="width: 60px;">
                            <asp:Label ID="lOperator" runat="server" Text="Operator" meta:resourcekey="lOperatorResource1"></asp:Label>
                        </td>
                        <td style="width: 200px;">
                            <asp:Label ID="lSearch" runat="server" Text="Search value" meta:resourcekey="lSearchResource2"></asp:Label>
                        </td>
                        <td style="width: 80px;">
                            <asp:Label ID="lSort" runat="server" Text="Sort order" meta:resourcekey="lSortResource1"></asp:Label>
                        </td>
                        <td style="width: 80px;">
                            <asp:Label ID="lAnd" runat="server" Text="And/Or" meta:resourcekey="lAndResource1"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("StartBrackets") %>' meta:resourcekey="Label2Resource1"></asp:Label>
                    </td>
                    <td>
                        <asp:ImageButton OnClick="ImageButtonUp_Click" ID="ImageButton2" runat="server" ImageUrl="/images/buttons/uparrow.png"
                            CssClass='<%# Container.ItemIndex %>' AlternateText="U" meta:resourcekey="ImageButton2Resource1" />
                    </td>
                    <td>
                        <asp:ImageButton OnClick="ImageButtonDown_Click" ID="ImageButton3" runat="server"
                            ImageUrl="/images/buttons/downarrow.png" CssClass='<%# Container.ItemIndex %>'
                            AlternateText="D" meta:resourcekey="ImageButton3Resource1" />
                    </td>
                    <td>
                        <asp:DropDownList Width="100%" ID="ddlAttribs" runat="server" DataTextField="AttributeText"
                            DataValueField="AttributeValue" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
                            onclick="return closePanel();" DataSource='<%# CurrentType.Attributes %>' CssClass='<%# Container.ItemIndex %>'
                            meta:resourcekey="ddlAttribsResource1">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOperators" runat="server" DataTextField="OperatorText" DataValueField="OperatorValue"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlOperators_SelectedIndexChanged"
                            onclick="return closePanel();" DataSource='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators %>'
                            CssClass='<%# Container.ItemIndex %>' meta:resourcekey="ddlOperatorsResource1">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <!-- Refactor this.... -->
                        <asp:DropDownList Width="100%" ID="ddlValues" runat="server" DataTextField="Key"
                            DataValueField="Value" onclick="return closePanel();" DataSource='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].Items:null %>'
                            CssClass='<%# Container.ItemIndex %>' Visible='
                            <%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count > 0 && 
                                CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsDynListDropDown %>'
                            meta:resourcekey="ddlValuesResource1">
                        </asp:DropDownList>
                        <amc:AssetDropDownListEx registerstartupscript="true" runat="server" id="adlAssets" Visible='
                            <%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0 && 
                                CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsAssetListDropDown %>'
                            CssClass='<%# Container.ItemIndex %>' />
                        <asp:TextBox Width="85%" ID="tbValue" runat="server" Text='<%# Eval("Text") %>'
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count <= 0 ? false : !CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsAssetListDropDown &&!CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators[((AttributeElement)Container.DataItem).OperatorId].IsDynListDropDown && CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Type != AppFramework.ConstantsEnumerators.Enumerators.DataType.Bool %>'
                            onclick="return closePanel();" meta:resourcekey="tbValueResource1"></asp:TextBox>
                        <asp:DropDownList runat="server" ID="dlBoolean"
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Type == AppFramework.ConstantsEnumerators.Enumerators.DataType.Bool %>'>
                            <asp:ListItem Value="False">False</asp:ListItem>
                            <asp:ListItem Value="True">True</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList Width="100%" ID="ddlSort" runat="server" onclick="return closePanel();"
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                            meta:resourcekey="ddlSortResource1">
                            <asp:ListItem Text="ASC" Value="0" meta:resourcekey="ListItemResource4" />
                            <asp:ListItem Text="DESC" Value="1" meta:resourcekey="ListItemResource5" />
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList Width="100%" ID="ddlLogic" runat="server" onclick="return closePanel();"
                            Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                            meta:resourcekey="ddlLogicResource1">
                            <asp:ListItem Text="And" Value="0" meta:resourcekey="ListItemResource6" />
                            <asp:ListItem Text="Or" Value="1" meta:resourcekey="ListItemResource7" />
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
        <asp:AsyncPostBackTrigger ControlID="ddlTypes" EventName="SelectedIndexChanged" />
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
    <tr>
        <td style="padding-bottom: 5px; width: 30px;">
            <asp:Button ID="buttonInsertBrakets" Width="90px" runat="server" Text="Insert()"
                OnClick="buttonNInsertBrakets_Click" meta:resourcekey="buttonInsertBraketsResource1" />
        </td>
        <td style="padding-bottom: 5px; padding-left: 10px; width: 30px;">
            <asp:Button ID="buttonRemoveBrakets" Width="90px" runat="server" Text="Remove()"
                OnClick="buttonRemoveBrakets_Click" meta:resourcekey="buttonRemoveBraketsResource1" />
        </td>
    </tr>
</table>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hfRemove" runat="server" Value="0" />
        <div runat="server" id="bracketsPanel" visible="False" class="centerAlligned">
            <table style="margin: auto;">
                <tr class="TypeSearchHeader">
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Select rows..." meta:resourcekey="Label4Resource1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:ListBox ID="lbBrakets" runat="server" SelectionMode="Multiple" Width="100%"
                            meta:resourcekey="lbBraketsResource1"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Label ID="lWarning" runat="server" Text="Incorrect select. Please change you select."
                            ForeColor="Red" meta:resourcekey="lWarningResource1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="ButtonAccept" runat="server" Text="Accept" OnClick="Button3_Click"
                            meta:resourcekey="ButtonAcceptResource1" />
                        <asp:Button ID="Button5" runat="server" Text="Cancel" meta:resourcekey="Button5Resource1" />
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="buttonInsertBrakets" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="buttonRemoveBrakets" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

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
