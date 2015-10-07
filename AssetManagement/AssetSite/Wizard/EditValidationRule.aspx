<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditValidationRule.aspx.cs"
    Inherits="AssetSite.Wizard.EditValidationRule" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblTitle" meta:resourcekey="lblTitleResource1">Asset Wizard &mdash; Attributen</asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" ID="lblPageDesc" meta:resourcekey="lblPageDescResource1"
            Text="Mauris congue consectetuer quam."></asp:Literal>
    </p>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblPanelTitle" meta:resourcekey="lblPanelTitleResource1">Create new rule</asp:Label>
        </div>
        <div class="panelcontent">
            <table cellspacing="10" cellpadding="0" class="w100p">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblRuleName" meta:resourcekey="lblRuleNameResource1">Name</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="RuleName" MaxLength="60" runat="server" meta:resourcekey="RuleNameResource2"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="RuleName" ValidationGroup="Rule"
                            Text="Name is required" meta:resourcekey="RequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%;padding-top: 5px">
                        <asp:DropDownList ID="OperatorsList" runat="server" DataValueField="UID" DataTextField="Name"
                            OnSelectedIndexChanged="OperatorsListSelectChanged" AutoPostBack="True" meta:resourcekey="OperatorsListResource1">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="OperatorsList"
                            InitialValue="-1" Text="Select operator" meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 80%">
                        <asp:Repeater runat="server" ID="OperandsList">
                            <HeaderTemplate>
                                <table cellpadding="5" cellspacing="0">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="Alias" runat="server" Text='<%# Bind("Alias") %>' meta:resourcekey="AliasResource1"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Val" runat="server" Text='<%# Bind("Value") %>' meta:resourcekey="ValResource1"></asp:TextBox>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="Button1" runat="server" Text="Add" OnClick="btnAdd_Click" ValidationGroup="Rule"
            meta:resourcekey="Button1Resource1" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" OnClick="btnCancel_Click" meta:resourcekey="Button2Resource1" />
    </div>

    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblPanelTitle2" meta:resourcekey="lblPanelTitle2Resource1">Algemene kenmerken attribuut</asp:Label>
        </div>
        <div class="panelcontent">
            <asp:GridView ID="GridRules" runat="server" AutoGenerateColumns="False" DataKeyNames="UID"
                OnRowDeleting="GridRules_RowDeleting" meta:resourcekey="GridRulesResource1">
                <RowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField HeaderStyle-Width="10%" meta:resourcekey="TemplateFieldResource1">
                        <HeaderTemplate>
                            Operator</HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("ValidationOperator.Name") %>
                        </ItemTemplate>
                        <HeaderStyle Width="10%"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="40%" meta:resourcekey="BoundFieldResource1">
                        <ItemStyle Width="40%"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource2">
                        <HeaderTemplate>
                            Operands
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# ListOperands(Container.DataItem as AppFramework.Core.Classes.Validation.ValidationRuleBase) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
                        <ItemTemplate>
                            <asp:ImageButton runat="server" ImageUrl="/images/buttons/delete.png" CommandName="Delete"
                                OnClientClick="return confirm('Are you sure want to delete rule?')" meta:resourcekey="ImageButtonResource1" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Literal runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1"
                        Text="No validation rules"></asp:Literal>
                </EmptyDataTemplate>
            </asp:GridView>
            <table cellspacing="0" cellpadding="0" class="w100p">
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="CheckResult" runat="server" meta:resourcekey="CheckResultResource1"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td width="70%">
                        <asp:TextBox TextMode="MultiLine" runat="server" ID="ExprText" Height="100px" Width="100%"
                            meta:resourcekey="ExprTextResource1"></asp:TextBox>
                    </td>
                    <td valign="middle">
                        <input type="button" onclick="onMoveOpClick('<%= ExprText.ClientID %>', '<%= OpList.ClientID %>')"
                            value="&larr;" />
                    </td>
                    <td width="30%">
                        <asp:ListBox ID="OpList" runat="server" Width="100%" Height="100px" meta:resourcekey="OpListResource1">
                        </asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Error message:
                        <asp:TextBox runat="server" ID="ValidationMessage" TextMode="MultiLine" Width="100%" Height="60px"></asp:TextBox>                        
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="panel">
        <div class="panelheader">
            Check
        </div>
        <div class="panelcontent">
            <table>
                <tr>
                    <td colspan="2">
                        <asp:TextBox runat="server" ID="CheckValue" meta:resourcekey="CheckValueResource1"></asp:TextBox>
                        <asp:Button runat="server" ID="CheckExpr" OnClick="CheckExprClick" Text="Check expression"
                            meta:resourcekey="CheckExprResource1" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="CheckMessage" runat="server" Font-Bold="True" meta:resourcekey="CheckMessageResource1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label runat="server" ID="lblConsistenceValidation" ForeColor="Red" Visible="false" meta:resourcekey="lblConsistenceValidation" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="btnClose" runat="server" OnClick="btnClose_Click" Text="Finished"
            meta:resourcekey="btnCloseResource1" />
    </div>
</asp:Content>
