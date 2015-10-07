<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTypesValidation.aspx.cs"
    Inherits="AssetSite.admin.DataTypesValidation" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    meta:resourcekey="PageResource1" Culture="auto" UICulture="auto" %>

<asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="main-container">
        <div class="wizard-header">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1"
                Text="Data type validation"></asp:Label>
        </div>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle2" meta:resourcekey="panelTitle2Resource1"
                Text="Create new rule"></asp:Label>
        </div>
        <div class="panelcontent">
            <table cellspacing="0" cellpadding="0" class="w100p">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblName" meta:resourcekey="lblNameResource1" Text="Name"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="RuleName" runat="server" ValidationGroup="Rule" meta:resourcekey="RuleNameResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Rule" ControlToValidate="RuleName"
                            Text="Name is required" meta:resourcekey="RequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%">
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
        <asp:Button ID="Button1" runat="server" Text="<% $Resources:Global, AddText %>" OnClick="btnAdd_Click"
            ValidationGroup="Rule" />
        <asp:Button ID="Button2" runat="server" Text="<% $Resources:Global, CancelText %>"
            OnClick="btnCancel_Click" />
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="paneltitle3" meta:resourcekey="paneltitle3Resource1">Rules</asp:Label>
        </div>
        <div class="panelcontent">
            <asp:GridView ID="GridRules" runat="server" AutoGenerateColumns="False" DataKeyNames="UID"
                OnRowDeleting="GridRules_RowDeleting" meta:resourcekey="GridRulesResource1">
                <Columns>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("UID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="10%" meta:resourcekey="TemplateFieldResource2">
                        <HeaderTemplate>
                            <asp:Literal runat="server" meta:resourcekey="LiteralResource1" Text="Operator"></asp:Literal>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("ValidationOperator.Name") %>
                        </ItemTemplate>
                        <HeaderStyle Width="10%"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="<% $Resources:Global, NameText %>" ItemStyle-Width="40%">
                        <ItemStyle Width="40%"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
                        <HeaderTemplate>
                            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Operands"></asp:Literal>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# ListOperands(Container.DataItem as AppFramework.Core.Classes.Validation.ValidationRuleBase) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource4">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="/images/buttons/delete.png"
                                CommandName="Delete" OnClientClick="return confirm('Are you sure want to delete rule?')"
                                meta:resourcekey="ImageButton1Resource1" />
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
                    <td colspan="3">
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
            </table>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="btnClose" runat="server" OnClick="btnClose_Click" Text="<%$ Resources:Global, CompleteText %>"
            meta:resourcekey="btnCloseResource2" />
    </div>
</asp:Content>
