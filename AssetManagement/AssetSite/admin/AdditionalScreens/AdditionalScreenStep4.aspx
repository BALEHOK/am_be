<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="AdditionalScreenStep4.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep4"
    EnableEventValidation="false" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<%@ Register Src="~/admin/AdditionalScreens/SideMenu.ascx" TagPrefix="amc" TagName="SideMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript">
        var currentPanelId = 0;

        function ShowDialog(panelId, dialogId) {
            currentPanelId = panelId;

            var $container = $("#" + dialogId);
            $container.dialog("option", "width", 575);

            var $isChildPanelCheckbox = $container.find('[type=checkbox]:eq(0)');
            var $childSelect = $container.find('.childAttribute');

            if ($childSelect.find('option').length === 0) {
                <% foreach (var attr in ChildAttributes) {%>
                <%= string.Format("$childSelect.append($('<option></option>').attr('value','{0}').text('{1} ({2})'));",
                                            attr.DynEntityAttribConfigId, attr.DynEntityConfig.Name, attr.Name) %>
                <% } %>

                if ($childSelect.find('option').length === 0) {
                    $isChildPanelCheckbox.attr('disabled', 'disabled');
                    $childSelect.attr('disabled', 'disabled');
                } else {
                    $isChildPanelCheckbox.removeAttr('disabled');
                    $isChildPanelCheckbox.change(setAttrSelectState);
                    setAttrSelectState();
                }
            }

            if (panelId !== 0) {
                AssetSite.amDataService.GetPanel(panelId, dialogId, OnGotInfo);
            }
            else {
                $container.dialog('open');
            }

            function setAttrSelectState() {
                console.log('checkbox changed');
                if ($isChildPanelCheckbox.attr('checked')) {
                    $childSelect.removeAttr('disabled');
                } else {
                    $childSelect.attr('disabled', 'disabled');
                }
            }
        }
        function OnGotInfo(result) {
            if (result) {
                var $container = $('#' + result.ContainerId);
                $container.find('[type=text]:eq(0)').val(result.Name);
                $container.find('[type=textarea]:eq(0)').val(result.Description);

                var $childSelect = $container.find('.childAttribute');
                if (result.IsChildAssets) {
                    $container.find('[type=checkbox]:eq(0)').attr('checked', 'checked');
                    $childSelect.removeAttr('disabled');
                    $childSelect.val(result.ChildAssetAttrId);
                } else {
                    $container.find('[type=checkbox]:eq(0)').removeAttr('checked');
                    $childSelect.attr('disabled', 'disabled');
                    $childSelect.val(null);
                }
                $container.dialog('open');
            }
        }
        function SavePanel(atId, screen, dialogId) {
            if (!Page_ClientValidate())
                return;
            var $dialog = $('#' + dialogId);
            var name = $dialog.find('[type=text]:eq(0)').val();
            var desc = $dialog.find('[type=textarea]:eq(0)').val();
            var isChildAssets = $dialog.find('[type=checkbox]:eq(0)').attr('checked');
            var childAttr = $dialog.find('.childAttribute').val();
            SetWaitImage(dialogId);
            AssetSite.amDataService.SavePanel(currentPanelId, name, desc, isChildAssets, childAttr, atId, screen, dialogId, OnPanelSaved);
        }
        function OnPanelSaved(result) {
            if (result) {
                RemoveWaitImage(result);
                __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$lbRebind', '');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/amDataService.asmx" />
            </Services>
        </asp:ScriptManager>
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>
            </div>
            <div class="panelcontent">
                <asp:GridView ID="gvPanels" OnRowCommand="gvPanels_RowCommand" runat="server" AutoGenerateColumns="false"
                    OnRowDataBound="gvPanelsRowBound">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                Name
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Literal runat="server" ID="TranslatedName"></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Description" DataField="Description" />
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <a style="cursor: pointer;" runat="server" id="lbtnEdit" onclick='<%#GetEditScript(Eval("AttributePanelUid")) %>'>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" /></a>&nbsp;
                                <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%#Eval("AttributePanelUid") %>'
                                    CausesValidation="false" OnCommand="OnDeleteCommand">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton runat="server" CommandArgument='<%# Eval("AttributePanelUid") %>' ImageUrl="~/images/arrowdown.png"
                                    CommandName="down" ID="imgbtnArrowDown" />
                                <asp:ImageButton runat="server" CommandArgument='<%# Eval("AttributePanelUid") %>' ImageUrl="~/images/arrowup.png"
                                    CommandName="up" ID="imgbtnArrowUp" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="Label1" runat="server" Text="No Panels Available"></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <a runat="server" id="lbtnNew" style="cursor: pointer;">
                    <asp:Label ID="Label2" runat="server" Text="Add Panel"></asp:Label>
                </a>
            </div>
        </div>
        <div class="wizard-footer-buttons">
            <asp:Button CssClass="btnPrev" Text="<%$ Resources:Global, PreviousText %>" ID="btnPrevious"
                runat="server" OnClick="btnPrevious_Click" CausesValidation="False" meta:resourcekey="btnPreviousResource1" />
            <asp:Button CssClass="btnNext" Text="<%$ Resources:Global, NextText %>" ID="btnNext"
                runat="server" OnClick="btnNext_Click" meta:resourcekey="btnNextResource1" />
            <asp:Button ID="btnClose" runat="server" Text="<%$ Resources:Global, CancelText %>"
                OnClick="btnClose_Click" CausesValidation="False" meta:resourcekey="btnCloseResource1" />
        </div>
        <div id="DialogContainer" runat="server" title="Panel creation" meta:resourcekey="DialogContainerRcrs"
            style="display: none;">
            <table border="0" cellpadding="2" cellspacing="0" width="100%">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server">Name:</asp:Label>&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="tbPanelName" runat="server" CssClass="SelectControl name" ValidationGroup="Dialog"
                            Width="350px" />
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Dialog" ControlToValidate="tbPanelName"
                            Display="Static" Text="*" ForeColor="Red" />
                    </td>
                    <td class="literal">
                        <a href="#" class="translation" onclick="showTranslationsFor('.SelectControl.name')">
                            Translations</a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server">Description:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbPanelDesc" runat="server" CssClass="SelectControl desc" TextMode="MultiLine"
                            Rows="5" Width="350px"></asp:TextBox>
                    </td>
                    <td class="literal">
                        <a href="#" class="translation" onclick="showTranslationsFor('.SelectControl.desc')">
                            Translations</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label runat="server">Is child items panel:</asp:Label>
                        <asp:CheckBox ID="tbPanelIsChild" runat="server" CssClass="SelectControl ischild"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlChildAttrib" CssClass="childAttribute" runat="server"/>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <amc:translations ID="Translations1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <amc:SideMenu runat="server" id="SideMenu" />
    <script type="text/javascript">
        $(document).ready(function () {
            $('.item').hover(
            function () {
                $(this).css('background-color', '#84d859');
            },
            function () {
                $(this).css('background-color', '#999999');
            }
        );
        });

    </script>
</asp:Content>
