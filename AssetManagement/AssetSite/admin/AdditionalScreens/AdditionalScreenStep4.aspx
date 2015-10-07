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

            $("#" + dialogId).dialog("option", "width", 570);
            if (panelId != 0) {
                AssetSite.amDataService.GetPanel(panelId, dialogId, OnGotInfo);
            }
            else {
                $("#" + dialogId).dialog('open');
            }
        }
        function OnGotInfo(result) {
            if (result) {
                $('#' + result.ContainerId).find('[type=text]:eq(0)').val(result.Name);
                $('#' + result.ContainerId).find('[type=textarea]:eq(0)').val(result.Description);
                $('#' + result.ContainerId).dialog('open');
            }
        }
        function SavePanel(atId, screen, dialogId) {
            if (!Page_ClientValidate())
                return;
            var name = $('#' + dialogId).find('[type=text]:eq(0)').val();
            var desc = $('#' + dialogId).find('[type=textarea]:eq(0)').val();
            SetWaitImage(dialogId);
            AssetSite.amDataService.SavePanel(currentPanelId, name, desc, atId, screen, dialogId, OnPanelSaved);
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
                <asp:LinkButton ID="lbRebind" runat="server" OnClick="lbRebind_Click" Visible="false">rebind</asp:LinkButton>
                <asp:GridView ID="gvPanels" OnRowCommand="gvPanels_RowCommand" runat="server" AutoGenerateColumns="false"
                    OnRowDataBound="gvPanelsRowBound">
                    <Columns>
                        <%--<asp:BoundField HeaderText="Name" DataField="Name" />--%>
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
                                <a style="cursor: pointer;" runat="server" id="lbtnEdit" onclick='<%#GetEditScript(Eval("UID")) %>'>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" /></a>&nbsp;
                                <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%#Eval("UID") %>'
                                    CausesValidation="false" OnCommand="OnDeleteCommand">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton runat="server" CommandArgument='<%# Eval("UID") %>' ImageUrl="~/images/arrowdown.png"
                                    CommandName="down" ID="imgbtnArrowDown" />
                                <asp:ImageButton runat="server" CommandArgument='<%# Eval("UID") %>' ImageUrl="~/images/arrowup.png"
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
