<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/MasterPages/MasterPageBase.master"
    CodeBehind="SearchByContext.aspx.cs" Inherits="AssetSite.Search.SearchByContext"
    meta:resourcekey="PageResource1" %>
<%@ Import Namespace="AppFramework.Core.Classes.SearchEngine.TypeSearchElements" %>
<%@ Register Src="~/Controls/SearchControls/SearchTabs.ascx" TagPrefix="uc1" TagName="SearchTabs" %>
<%@ Register Src="~/Controls/AssetDropDownListEx.ascx" TagName="AssetDropDownListEx"
    TagPrefix="amc" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
    <link href="../css/Search.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:Literal ID="LiteralSearch" runat="server" Text="Search" meta:resourcekey="LiteralSearchResource1"></asp:Literal>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <div class="reglament">
        <uc1:searchtabs runat="server" id="SearchTabs" ActiveSearchType="SearchByContext" />
        <div style="float: left;" class="greenBottom">
        </div>
        <br />
        <br />
        <div style="float: none;">
            <table width="100%" class="categorySearcher">
                <tr>
                    <td align="center">
                        <asp:HiddenField ID="hfUid" runat="server" />
                        <asp:Label CssClass="margin-15" ID="lType" runat="server" Text="Search by" meta:resourcekey="lTypeResource1"></asp:Label>
                        <asp:RadioButtonList runat="server" ID="rbActive" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Text="Active" meta:resourcekey="cbActiveResource1" Selected="True" Value="1" />
                            <asp:ListItem Text="cbHistory" meta:resourcekey="cbHistoryResource1" Value="0" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
            <script language="javascript" type="text/javascript">
                function closePanel() {
                    var Hi = $("#<%=bracketsPanel.ClientID%>");
                    Hi.show();
                }
            </script>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
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
                                        <asp:Label ID="lAttribute" runat="server" Text="Context" meta:resourcekey="lAttributeResource1"></asp:Label>
                                    </td>
                                    <td style="width: 60px;">
                                        <asp:Label ID="lOperator" runat="server" Text="Operator" meta:resourcekey="lOperatorResource1"></asp:Label>
                                    </td>
                                    <td style="width: 200px;">
                                        <asp:Label ID="lSearch" runat="server" Text="Search value" meta:resourcekey="lSearchResource2"></asp:Label>
                                    </td>
                                    <td>
                                        &nbsp;
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
                                    <asp:DropDownList Width="100%" ID="ddlAttribs" runat="server" DataTextField="Text"
                                        DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
                                        onclick="return closePanel();" DataSource='<%# CurrentType.Attributes %>' CssClass='<%# Container.ItemIndex %>'
                                        meta:resourcekey="ddlAttribsResource1">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList Width="100%" ID="ddlOperators" runat="server" DataTextField="OperatorText"
                                        DataValueField="OperatorValue" AutoPostBack="True" OnSelectedIndexChanged="ddlOperators_SelectedIndexChanged"
                                        onclick="return closePanel();" DataSource='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators %>'
                                        CssClass='<%# Container.ItemIndex %>' meta:resourcekey="ddlOperatorsResource1">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox Width="100%" ID="tbValue" runat="server" Text='<%# Eval("Text") %>'
                                        Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                                        onclick="return closePanel();" meta:resourcekey="tbValueResource1" />
                                    <asp:DropDownList runat="server" Width="45%" AutoPostBack="true" OnSelectedIndexChanged="ddlDynList_SelectedIndexChanged"
                                        ID="ddlDynLists" DataTextField="Name" CssClass='<%# Container.ItemIndex %>' DataValueField="UID"
                                        Visible="false" />
                                    <asp:DropDownList runat="server" Width="45%" ID="ddlDynListItems" DataTextField="Value"
                                        Visible="false" DataValueField="ID" />
                                    <asp:DropDownList ID="ddlTypes" DataTextField="Name" DataValueField="ID" runat="server"
                                        AutoPostBack="True" Visible="false" CssClass='<%# Container.ItemIndex %>' OnSelectedIndexChanged="ddlTypes_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <amc:AssetDropDownListEx RegisterStartupScript="true" runat="server" ID="adlAssets"
                                        Visible="false" />
                                    <asp:DropDownList runat="server" ID="dlBoolean"
                                        Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].DataTypeEnum == AppFramework.ConstantsEnumerators.Enumerators.DataType.Bool %>'>
                                        <asp:ListItem Value="False">False</asp:ListItem>
                                        <asp:ListItem Value="True">True</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="valueValidator" runat="server" ControlToValidate="tbValue"
                                        Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                                        Text="*" ForeColor="Red" Display="Static" />
                                </td>
                                <td>
                                    <asp:DropDownList Width="100%" ID="ddlSort" runat="server" onclick="return closePanel();"
                                        Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                                        meta:resourcekey="ddlSortResource1">
                                        <asp:ListItem Text="ASC" Value="0" meta:resourcekey="ListItemResource1" />
                                        <asp:ListItem Text="DESC" Value="1" meta:resourcekey="ListItemResource2" />
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList Width="100%" ID="ddlLogic" runat="server" onclick="return closePanel();"
                                        Visible='<%# CurrentType.Attributes[((AttributeElement)Container.DataItem).AttributeId].Operators.Count>0?true:false %>'
                                        meta:resourcekey="ddlLogicResource1">
                                        <asp:ListItem Text="And" Value="0" meta:resourcekey="ListItemResource3" />
                                        <asp:ListItem Text="Or" Value="1" meta:resourcekey="ListItemResource4" />
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:ImageButton OnClick="ImageButtonDel_Click" ID="ImageButton1" runat="server"
                                        CausesValidation="false" ImageUrl="/images/buttons/delete.jpg" CssClass='<%# Container.ItemIndex %>'
                                        AlternateText="D" meta:resourcekey="ImageButton1Resource1" />
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
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="padding-bottom: 5px; width: 30px;">
                        <asp:Button Width="100px" ID="Button2" runat="server" Text="Clear" OnClick="Button2_Click"
                            meta:resourcekey="Button2Resource2" />
                    </td>
                    <td style="padding-bottom: 5px; padding-left: 10px; width: 30px;">
                        <asp:Button Width="100px" ID="buttonNewRow" runat="server" Text="Add row" OnClick="buttonNewRow_Click"
                            meta:resourcekey="buttonNewRowResource1" />
                    </td>
                    <td align="right">
                        <asp:Button ID="Button1" runat="server" Text="Search" OnClick="ButtonSearch_Click"
                            meta:resourcekey="Button1Resource2" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-bottom: 5px; width: 30px;">
                        <asp:Button ID="buttonInsertBrakets" Width="100px" runat="server" Text="Insert()"
                            OnClick="buttonNInsertBrakets_Click" meta:resourcekey="buttonInsertBraketsResource1" />
                    </td>
                    <td style="padding-bottom: 5px; padding-left: 10px; width: 30px;">
                        <asp:Button ID="buttonRemoveBrakets" Width="100px" runat="server" Text="Remove()"
                            OnClick="buttonRemoveBrakets_Click" meta:resourcekey="buttonRemoveBraketsResource1" />
                    </td>
                    <td align="right">
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
        </div>
    </div>
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
</asp:Content>
<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderMainContent" runat="Server">
    <div class="clear-columns">
        <!-- do not delete -->
    </div>
</asp:Content>
