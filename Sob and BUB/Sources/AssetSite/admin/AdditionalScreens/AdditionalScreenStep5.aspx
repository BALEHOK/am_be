<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="AdditionalScreenStep5.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep5" EnableEventValidation="false" %>

<%@ Register TagPrefix="uc1" TagName="AttribList" Src="~/Controls/AssetTypeAttribList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript">
        var focusedItem = '';
        var prevFocus = 'none';

        function Mover(controlId) {
            this.ControlId = controlId;

            this.Up = function () {
                $('#' + this.ControlId + ' option:selected')
                .insertBefore($('#' + this.ControlId + ' option:selected').prev());
            }
            this.Down = function () {
                $('#' + this.ControlId + ' option:selected')
                .insertAfter($('#' + this.ControlId + ' option:selected').next());
            }
            this.MoveTop = function () {
                $('#' + this.ControlId + ' option:selected')
                .insertBefore($('#' + this.ControlId + ' option:first'));
            }
            this.MoveBottom = function () {
                $('#' + this.ControlId + ' option:selected')
                .insertAfter($('#' + this.ControlId + ' option:last'));
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
                    var nOption = $("<option>" + tempElem.text() + "</option>").attr("value", tempElem.val());
                    var num = tempElem.val().split(':')[1];
                    $('#containerN' + num).find('select').append(nOption);
                    tempElem.remove();
                });
            }
        }

        function CollectData(hfId) {
            var content = '';

            var valid = true;
            $('#assetTypes').find('select').each(function (i, elem) {
                var ddl = $(elem);
                ddl.children().each(function (j, option) {
                    var tempChild = $(option);
                    var errorSpan = $('<span></span>').attr('style', 'color:Red');
                    var childValue = new String(tempChild.val());
                    if (childValue.indexOf("*") > 0) {
                        tempChild.html(errorSpan.html(tempChild.text()));
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
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>                
        </div>
        <div class="panelcontent">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td style="width:42%; text-align:center;" id="assetTypes">
                        <asp:Repeater ID="repATAttributes" runat="server">
                            <ItemTemplate>
                                <uc1:AttribList runat="server" DynEntityConfig='<%# Container.DataItem %>' LinkedAttributesUids='<%# AllLinkedAttributesUids %>'/>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                    <td style="width:16%;"></td>
                    <td style="width:42%" id="panels">
                        <asp:Repeater ID="repPanels" runat="server" OnItemDataBound="PanelCreated">
                            <ItemTemplate>
                                <div id='<%# GetPanelDivId(Eval("UID")) %>' >
                                    <asp:Literal ID="litScript" runat="server"></asp:Literal>
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td colspan="3" style="text-align:center;">
                                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("Name") %>'></asp:Label><br />                                            
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width:15%; text-align:right;">
                                                <a style="cursor:pointer;" onclick='<%#GetAddScript(Eval("ID")) %>'>
                                                    <img alt="add" src="../../images/buttons/addarrow.png" />
                                                </a>
                                                <br />
                                                <a style="cursor:pointer;" onclick='<%#GetRemoveScript(Eval("ID")) %>'>
                                                    <img alt="remove" src="../../images/buttons/deletearrow.png" />
                                                </a>
                                            </td>
                                            <td style="width:70%;">
                                                <asp:ListBox ID="lstPanelAttrib" runat="server" Width="100%" SelectionMode="Multiple" Height="200px"></asp:ListBox>
                                            </td>
                                            <td style="width:15%;">
                                                <a style="cursor:pointer;" onclick='<%#GetTopScript(Eval("ID")) %>'>
                                                    <img alt="top" src="../../images/buttons/sort_first.png" />
                                                </a><br />
                                                <a style="cursor:pointer;" onclick='<%#GetUpScript(Eval("ID")) %>'>
                                                    <img alt="up" src="../../images/buttons/sort_up.png" />
                                                </a><br />
                                                <a style="cursor:pointer;" onclick='<%#GetDownScript(Eval("ID")) %>'>
                                                    <img alt="down" src="../../images/buttons/sort_down.png" />
                                                </a><br />
                                                <a style="cursor:pointer;" onclick='<%#GetBottomScript(Eval("ID")) %>'>
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

            <div class="wizard-footer-buttons" style="text-align:center; margin-top:10px;">
                <asp:Button Text="<%$ Resources:Global, PreviousText %>" ID="btnPrevious" runat="server" OnClick="btnPrevious_Click" CausesValidation="False"/> 

                <asp:Button ID="btnFinish" runat="server" Text="Finish" OnClick="btnFinish_Click" CausesValidation="false"/>

                <asp:Button ID="btnClose" runat="server" Text="<%$ Resources:Global, CancelText %>" OnClick="btnClose_Click" CausesValidation="False" />          
            </div> 
        </div>
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
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
   <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server">Screen Configuration</asp:Label>            
        </div>
        <br />
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep1.aspx">Create/Edit Screen</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep2.aspx">Screen Properties</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep3.aspx">Layout Selection</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep4.aspx">Panels Configuration</asp:HyperLink>
            </span>
        </div>
        <div class="active">
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep5.aspx">Assign Attributes</asp:HyperLink>
            </span>
        </div>
    </div>
</asp:Content>
