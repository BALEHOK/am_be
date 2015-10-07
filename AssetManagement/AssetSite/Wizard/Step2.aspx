<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step2" CodeBehind="Step2.aspx.cs"
    meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
    <script type="text/javascript">
        function ShowMUSelect() {
            var mu = $('#MeasureUnitSelect');
            if (mu.is(':visible'))
                mu.hide();
            else
                mu.show();
        }
    </script>
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Asset Wizard" meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp;
        <asp:Label runat="server" ID="stepName" Text="Specifieke kenmerken" meta:resourcekey="stepNameResource1"></asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" ID="stepDesc" Text="Mauris congue consectetuer quam."
            meta:resourcekey="stepDescResource1"></asp:Literal>
    </p>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelName" Text="Specifieke kenmerken" meta:resourcekey="panelNameResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="10" class="w100p">
                <tr>
                    <td>
                        <div class="labels">
                            <asp:Label ID="Label1" runat="server" Text="Type asset " meta:resourcekey="Label1Resource1"></asp:Label>
                        </div>
                    </td>
                    <td align="left">
                        <div class="controls">
                            <asp:DropDownList CssClass="SelectControl" ID="comboAssetTypeInheritance" AutoPostBack="True"
                                OnSelectedIndexChanged="comboAssetTypeInheritance_Selected" runat="server" meta:resourcekey="comboAssetTypeInheritanceResource1">
                            </asp:DropDownList>
                        </div>
                        <div style="clear: both">
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel runat="server" CssClass="controls" ID="ctrlsInherit" Visible="False" meta:resourcekey="ctrlsInheritResource1">
                                    <div class="labels" style="padding-left: 0px; margin-top: 5px;">
                                        <asp:Label ID="lblInherit" runat="server" Text="Inherit from:" meta:resourcekey="lblInheritResource1"></asp:Label>
                                    </div>
                                    <div class="controls">
                                        <asp:DropDownList runat="server" CssClass="SelectControl" ID="comboAssetTypes" meta:resourcekey="comboAssetTypesResource1">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ID="comboAssetTypesValidator" ControlToValidate="comboAssetTypes"
                                            Text="Please select the Asset Type." meta:resourcekey="comboAssetTypesValidatorResource1"></asp:RequiredFieldValidator>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="comboAssetTypeInheritance" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <% if (AppFramework.Core.Classes.ApplicationSettings.ApplicationType != AppFramework.ConstantsEnumerators.ApplicationType.SOBenBUB)
                   { %>
                <tr>
                    <td>
                        <div class="labels">
                            <asp:Label runat="server" ID="lblInStock" Text="Is in stock?" meta:resourcekey="lblInStockResource1"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <div class="controls">
                            <asp:CheckBox runat="server" ID="IsInStock" meta:resourcekey="IsInStockResource1" />
                        </div>
                    </td>
                </tr>
                <% } %>
                <tr id="MeasureUnitSelect" style="display: none;">
                    <td>
                        <div class="labels">
                            <asp:Label runat="server" ID="lblUnit" Text="Measure unit" meta:resourcekey="lblUnitResource1"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <div class="controls">
                            <asp:DropDownList CssClass="SelectControl" ID="MeasureUnits" runat="server" DataTextField="Name"
                                DataValueField="UID" meta:resourcekey="MeasureUnitsResource1">
                            </asp:DropDownList>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="Label2" runat="server" Text="<% $Resources:Global, ActiveText %>"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" meta:resourcekey="chkActiveResource1" />
                    </td>
                </tr>
                <tr>
                    <td class="labels" style="width:auto;">
                        <asp:Label ID="lblParentChildRelations" runat="server" Text="<% $Resources:Global, ParentChildRelationsText %>"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="cbParentChildRelations" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="Label5" runat="server" Text="<% $Resources:Global, ContextIndexedText %>"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="cbContextIndexed" runat="server" />
                    </td>
                </tr>
                <% if (AppFramework.Core.Classes.ApplicationSettings.ApplicationType != AppFramework.ConstantsEnumerators.ApplicationType.SOBenBUB)
                   { %>
                <tr>
                    <td class="labels">
                        <asp:Label ID="Label3" runat="server" Text="<% $Resources:Global, AllowBorrowText %>"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="cbAllowBorrow" runat="server" />
                    </td>
                </tr>
                <%}%>
                <tr>
                    <td class="labels">
                        <asp:Label ID="Label4" runat="server" Text="<% $Resources:Global, UseForNamesText %>"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlUseForNames" runat="server">
                            <asp:ListItem Value="0" Text="<% $Resources:Global, UseForNamesTextNone %>" />
                            <asp:ListItem Value="1" Text="<% $Resources:Global, UseForNamesTextIsertOnly %>" />
                            <asp:ListItem Value="2" Text="<% $Resources:Global, UseForNamesTextInsertUpdate %>" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
