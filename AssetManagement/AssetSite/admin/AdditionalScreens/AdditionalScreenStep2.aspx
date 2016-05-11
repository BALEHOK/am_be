<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="AdditionalScreenStep2.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep2" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<%@ Register Src="~/admin/AdditionalScreens/SideMenu.ascx" TagPrefix="amc" TagName="SideMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <amc:SideMenu runat="server" id="SideMenu" />

    <%-- <div class="wizard-menu">
    </div>--%>
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
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager ID="mgrForScreens" runat="server" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>
            </div>
            <div class="panelcontent">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox CssClass="SelectControl name" ID="txtName" runat="server" MaxLength="60"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                                ErrorMessage="*" meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator>
                        </td>
                        <td class="literal">
                            <a href="#" onclick="showTranslationsFor('.SelectControl.name')">Translations</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label runat="server" ID="lblTitle" Text="Title"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="tbTitle" CssClass="SelectControl title" runat="server" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbTitle"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                        <td class="literal">
                            <a href="#" onclick="showTranslationsFor('.SelectControl.title')">Translations</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblSubtitle" runat="server" Text="Subtitle"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="tbSubTitle" CssClass="SelectControl subtitle" runat="server"></asp:TextBox>
                        </td>
                        <td class="literal">
                            <a href="#" onclick="showTranslationsFor('.SelectControl.subtitle')">Translations</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblbPageText" runat="server" Text="Page Text"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="tbPageText" CssClass="SelectControl pageText" runat="server" TextMode="MultiLine"
                                Rows="3"></asp:TextBox>
                        </td>
                        <td class="literal">
                            <a href="#" onclick="showTranslationsFor('.SelectControl.pageText')">Translations</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblDesc" runat="server" Text="Description"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="tbDesc" runat="server" CssClass="SelectControl pageDesc" TextMode="MultiLine"
                                Rows="3"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbDesc"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                        <td class="literal">
                            <a href="#" onclick="showTranslationsFor('.SelectControl.pageDesc')">Translations</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblDefault" runat="server" Text="Is Default"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:CheckBox ID="chkIsDeafult" runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblMobile" runat="server" Text="Is Mobile"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:CheckBox ID="chkIsMobile" runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:DropDownList ID="ddlStatus" CssClass="SelectControl" runat="server">
                                <asp:ListItem Text="Runtime" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Design" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
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
    </div>
    <amc:Translations ID="Translations1" runat="server" />
</asp:Content>
