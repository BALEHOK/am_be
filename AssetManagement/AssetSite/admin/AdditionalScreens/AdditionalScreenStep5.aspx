<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="AdditionalScreenStep5.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep5"
    EnableEventValidation="false" %>

<%@ Register TagPrefix="uc1" TagName="AttribList" Src="~/Controls/AssetTypeAttribList.ascx" %>
<%@ Register Src="~/admin/AdditionalScreens/SideMenu.ascx" TagPrefix="amc" TagName="SideMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript">
        var focusedItem = '';
        var prevFocus = 'none';

        function Mover(controlId) {
            this.ControlId = controlId;

            this.Up = function () {
                if ($('#' + this.ControlId + ' option:selected')[0] != $('#' + this.ControlId + ' option:first')[0]) {
                    $('#' + this.ControlId + ' option:selected').each(function () {
                        $(this).insertBefore($(this).prev());
                    });
                }
            }
            this.Down = function () {
                if ($('#' + this.ControlId + ' option:selected').last()[0] != $('#' + this.ControlId + ' option:last')[0]) {
                    $($('#' + this.ControlId + ' option:selected').get().reverse()).each(function () {
                        $(this).insertAfter($(this).next());
                    });
                }
            }
            this.MoveTop = function () {
                if ($('#' + this.ControlId + ' option:selected')[0] != $('#' + this.ControlId + ' option:first')[0]) {
                    var countpos = $('#' + this.ControlId + ' option:selected').eq(0).index();
                    $('#' + this.ControlId + ' option:selected').each(function () {
                        $(this).insertBefore($(this).parent().find("option").eq($(this).index() - countpos));
                    });
                }
            }
            this.MoveBottom = function () {
                if ($('#' + this.ControlId + ' option:selected').last()[0] != $('#' + this.ControlId + ' option:last')[0]) {
                    var countpos = $('#' + this.ControlId + ' option:last').index() - $('#' + this.ControlId + ' option:selected').last().index();
                    $($('#' + this.ControlId + ' option:selected').get().reverse()).each(function () {
                        $(this).insertAfter($(this).parent().find("option").eq($(this).index() + countpos));
                    });
                }
            }

            this.AddItem = function () {
                var options = $('#' + focusedItem + ' option:selected');
                for (i = 0; i < options.length; i++) {
                    var tempElem = $(options[i]);
                    var nOption = $("<option>" + tempElem.text() + "</option>").attr("value", tempElem.val());
                    $('#' + this.ControlId).append(nOption);
                    tempElem.remove();
                }
            }
            this.RemoveItem = function () {
                $('#' + this.ControlId + ' option:selected').each(function (i, elem) {
                    var tempElem = $(elem);
                    var val = tempElem.val();
                    var num = val.split(':')[1];

                    var $select = $('#containerN' + num).find('select');
                    var optionElems = $select.children();

                    var duplicate = false;
                    for (var i = 0; i < optionElems.length; i++) {
                        var $op = $(optionElems[i]);
                        if ($op.val() == val) {
                            duplicate = true;
                            break;
                        }
                    }

                    if (!duplicate) {
                        var nOption = $("<option>" + tempElem.text() + "</option>").attr("value", val);
                        $select.append(nOption);
                    }

                    tempElem.remove();
                });
            }
        }

        function CollectData(hfId) {
            var content = '';

            var valid = true;
            $('#assetTypes').find('select').each(function (i, elem) {
                var ddl = $(elem);
                $('.error', ddl.parent()).remove();
                var errorPh = $('<ul>');
                errorPh.addClass('error');
                ddl.after(errorPh);
                ddl.children().each(function (j, option) {
                    if ($(option).val().indexOf("*") > 0) {
                        if (!ddl.next().is('p.error')) {
                            ddl.after('<p class="error"><asp:Label runat="server" meta:resourcekey="lblRequiredAttributes" />:</p>');
                        }
                        errorPh.append('<li>' + $(option).text() + '</li>');
                        valid = false;
                    }
                });
            });

            if (!valid) {
                return false;
            }

            $('#panels').find('div').each(function (i, elem) {

                if ($(elem).attr("id").indexOf("panelDiv") >= 0) {

                    var panelId = $(elem).attr("id").replace("panelDiv", "");
                    content += "(" + panelId + ")";

                    content += "{";
                    $(elem).find('select:eq(0)').children().each(function (j, subElem) {
                        content += $(subElem).val() + ";";
                    });
                    content += "}";

                }
                content += "|";
            });
            $('#' + hfId).val(content);

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container" class="additional-screens">
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>
            </div>
            <div class="panelcontent">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="width: 42%; text-align: center;" id="assetTypes">
                            <asp:Repeater ID="repATAttributes" runat="server">
                                <ItemTemplate>
                                    <uc1:AttribList runat="server" ReferencingDynEntityAttribConfigId='<%# Eval("ReferencingDynEntityAttribConfigId") %>'
                                        ReferencingDynEntityAttribConfigName='<%# Eval("ReferencingDynEntityAttribConfigName") %>'
                                        Name='<%# Eval("Name") %>' DynEntityConfig='<%# Eval("Configuration") %>' LinkedAttributesUids='<%# AttributesToAttribute %>'
                                        IgnoreValidation='<%# Eval("IgnoreValidation") %>' />
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                        <td style="width: 16%;"></td>
                        <td style="width: 42%" id="panels">
                            <asp:Repeater ID="repPanels" runat="server" OnItemDataBound="OnPanelDataBound">
                                <ItemTemplate>
                                    <div id='<%# GetPanelDivId(Eval("AttributePanelUid")) %>'>
                                        <asp:Literal ID="litScript" runat="server"></asp:Literal>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td colspan="3" style="text-align: center;">
                                                    <asp:Label ID="Label1" runat="server" Text='<%#Eval("Name")%>'></asp:Label><br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 15%; text-align: right;">
                                                    <a style="cursor: pointer;" onclick='<%#GetAddScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="add" src="../../images/buttons/addarrow.png" />
                                                    </a>
                                                    <br />
                                                    <a style="cursor: pointer;" onclick='<%#GetRemoveScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="remove" src="../../images/buttons/deletearrow.png" />
                                                    </a>
                                                </td>
                                                <td style="width: 70%;">
                                                    <asp:ListBox ID="lstPanelAttrib" runat="server" Width="100%" SelectionMode="Multiple"
                                                        Height="200px"></asp:ListBox>
                                                </td>
                                                <td style="width: 15%;">
                                                    <a style="cursor: pointer;" onclick='<%#GetTopScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="top" src="../../images/buttons/sort_first.png" />
                                                    </a>
                                                    <br />
                                                    <a style="cursor: pointer;" onclick='<%#GetUpScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="up" src="../../images/buttons/sort_up.png" />
                                                    </a>
                                                    <br />
                                                    <a style="cursor: pointer;" onclick='<%#GetDownScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="down" src="../../images/buttons/sort_down.png" />
                                                    </a>
                                                    <br />
                                                    <a style="cursor: pointer;" onclick='<%#GetBottomScript(Eval("AttributePanelId")) %>'>
                                                        <img alt="bottom" src="../../images/buttons/sort_last.png" />
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <br />
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hfldCollectedData" runat="server" />
            </div>
        </div>
        <div class="wizard-footer-buttons">
            <asp:Button CssClass="btnPrev" Text="<%$ Resources:Global, PreviousText %>" ID="btnPrevious"
                runat="server" OnClick="btnPrevious_Click" CausesValidation="False" meta:resourcekey="btnPreviousResource1" />
            <asp:Button ID="btnFinish" runat="server" Text="Finish" OnClick="btnFinish_Click"
                        CausesValidation="false" />
            <asp:Button ID="Button1" runat="server" Text="<%$ Resources:Global, CancelText %>"
                        OnClick="btnClose_Click" CausesValidation="False" />
        </div>
        <script type="text/javascript">
            var items = $('#assetTypes').find('select').each(function (i, elem) {
                $(elem).focus(function () {
                    focusedItem = $(this).attr("id");

                    if (prevFocus != 'none' && prevFocus != focusedItem) {
                        $('#' + prevFocus + " option:selected").attr("selected", false);
                    }

                    prevFocus = focusedItem;
                });
            });
        </script>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <amc:SideMenu runat="server" id="SideMenu" />
</asp:Content>
