<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step4" CodeBehind="Step4.aspx.cs" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">

    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Asset Wizard"
            meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp; 
             <asp:Literal runat="server" ID="stepTitle"
                 Text="Schermweergave — Define attributes "
                 meta:resourcekey="stepTitleResource1"></asp:Literal>
    </div>
    <p>
        <asp:Literal runat="server" ID="stepDesc"
            Text="Define attributes to show in grid by default"
            meta:resourcekey="stepDescResource1"></asp:Literal>
    </p>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelHeader" Text="Define attributes"
                meta:resourcekey="panelHeaderResource1"></asp:Label>
        </div>

        <div class="panelcontent">
            <asp:Label runat="server" ID="lblValidation" ForeColor="Red" Visible="false" Text="At least one attribute must be selected to use within Name autogeneration." />

            <asp:GridView ID="GridAttributes" runat="server" AutoGenerateColumns="False"
                DataKeyNames="UID" meta:resourcekey="GridAttributesResource1">
                <Columns>

                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                        <ItemTemplate>
                            <asp:HiddenField ID="UID" runat="server" Value='<%# Eval("UID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="NameLocalized" HeaderText="<% $Resources:Global, NameText %>" HeaderStyle-Width="20%">
                        <HeaderStyle Width="20%"></HeaderStyle>
                    </asp:BoundField>

                    <asp:BoundField DataField="Comment" HeaderText="<% $Resources:Global, DescText %>" HeaderStyle-Width="30%">

                        <HeaderStyle Width="30%"></HeaderStyle>
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="Show in grid" ItemStyle-HorizontalAlign="Center"
                        HeaderStyle-HorizontalAlign="Center"
                        meta:resourcekey="TemplateFieldResource2">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkShow" runat="server"
                                Checked='<%# Bind("IsShownInGrid") %>' CssClass="chk"
                                meta:resourcekey="chkShowResource1" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Use for name generation" ItemStyle-HorizontalAlign="Center"
                        HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbUseForNames" runat="server" Checked='<%# Bind("IsUsedForNames") %>' CssClass="chk1" Enabled='<%#IsNameAutogenEnabled() %>' 
                                AutoPostBack="true" OnCheckedChanged="cbUseForNames_OnCheckedChanged"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Literal runat="server" ID="lblNoData" Text="No attributes"
                        meta:resourcekey="lblNoDataResource1"></asp:Literal>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

    <asp:CustomValidator
        ID="CustomValidator1"
        runat="server"
        ErrorMessage="Please check from 1 to 5 fields"
        ClientValidationFunction="checkQuantity"
        meta:resourcekey="CustomValidator1Resource1"></asp:CustomValidator>

    <script type="text/javascript">
        function checkQuantity(sender, args) {
            if ($('.chk :checked').size() > 0 && $('.chk :checked').size() <= 5) {
                args.IsValid = true;
                return args;
            }
            args.IsValid = false;
            return args;
        }
    </script>

    <div id="Step4DialogContainer" style="display: none;">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="width: 90%;">
                    <asp:ListBox ID="lstAttribs" runat="server" SelectionMode="Single" Height="300" Width="100%"
                        DataTextField="Name" DataValueField="ID"></asp:ListBox>
                </td>
                <td style="width: 10%; text-align: right;">
                    <a style="border: 0px; cursor: pointer;" onclick=" return moveTopAttribute();">
                        <asp:Image ID="Image5" runat="server" ImageUrl="~/images/buttons/sort_first.png" />
                    </a>
                    <br />
                    <a style="border: 0px; cursor: pointer;" onclick=" return moveUpAttribute();">
                        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/buttons/sort_up.png" />
                    </a>
                    <br />
                    <a style="border: 0px; cursor: pointer;" onclick=" return moveDownAttribute();">
                        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/buttons/sort_down.png" />
                    </a>
                    <br />
                    <a style="border: 0px; cursor: pointer;" onclick=" return moveBottomAttribute();">
                        <asp:Image ID="Image8" runat="server" ImageUrl="~/images/buttons/sort_last.png" />
                    </a>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    <br />
                    <a style="text-decoration: none; cursor: pointer;" onclick="return SaveOrder();">Ok
                    </a>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        function SaveSortPanelItem(form) {
            $('table[class="wh100p"]').each(function () {
                var list = $(this).find("select:first");
                var hidenfield = $(this).find('input[type="hidden"]:first');
                $(hidenfield).val($(list).find("option").map(function () { return this.value }).get().join(";"));
            });
            return true;
        }

        function MoveListBoxItemsUp(listBoxId) {
            if ($('#' + listBoxId + ' option:selected')[0] != $('#' + listBoxId + ' option:first')[0]) {
                $('#' + listBoxId + ' option:selected').each(function () {
                    $(this).insertBefore($(this).prev());
                });
            }
        }

        function MoveListBoxItemsDown(listBoxId) {
            if ($('#' + listBoxId + ' option:selected').last()[0] != $('#' + listBoxId + ' option:last')[0]) {
                $($('#' + listBoxId + ' option:selected').get().reverse()).each(function () {
                    $(this).insertAfter($(this).next());
                });
            }
        }
    </script>
</asp:Content>
