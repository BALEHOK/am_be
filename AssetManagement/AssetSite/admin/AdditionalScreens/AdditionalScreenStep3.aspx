<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="AdditionalScreenStep3.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep3" %>

<%@ Register Src="~/admin/AdditionalScreens/SideMenu.ascx" TagPrefix="amc" TagName="SideMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript">
        function checkLayout(sender, args) {
            if ($('.layoutSet :radio:checked').size() == 1) {
                args.IsValid = true;
                return args;
            } else {
                args.IsValid = false;
                return args;
            }
        }    
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container">
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelTitle" Text="Kies een schermlay-out voor weergave van de panels..."></asp:Label>
            </div>
            <div class="panelcontent">
                <asp:RadioButtonList ID="LayoutRadioSelect" runat="server" OnDataBound="layoutListBound"
                    AppendDataBoundItems="true" RepeatDirection="Horizontal" RepeatColumns="3" CellSpacing="10"
                    CssClass="w100p layoutSet" DataTextFormatString="<img src='{0}' class='LayoutImg' />">
                </asp:RadioButtonList>
            </div>
        </div>
        <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Please choose the layout."
            ClientValidationFunction="checkLayout"></asp:CustomValidator>
        <div class="wizard-footer-buttons">
            <asp:Button CssClass="btnPrev" Text="<%$ Resources:Global, PreviousText %>" ID="btnPrevious"
                runat="server" OnClick="btnPrevious_Click" CausesValidation="False" meta:resourcekey="btnPreviousResource1" />
            <asp:Button CssClass="btnNext" Text="<%$ Resources:Global, NextText %>" ID="btnNext"
                runat="server" OnClick="btnNext_Click" meta:resourcekey="btnNextResource1" />
            <asp:Button ID="btnClose" runat="server" Text="<%$ Resources:Global, CancelText %>"
                OnClick="btnClose_Click" CausesValidation="False" meta:resourcekey="btnCloseResource1" />
        </div>
    </div>
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
