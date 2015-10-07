<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="RecoverCredentials.aspx.cs" Inherits="AssetSite.RecoverCredentials1" %>

<%@ Register TagPrefix="uc" TagName="NewsAndLoginInfo" Src="Controls/NewsAndLoginInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <style type="text/css">
        #outer-column-container
        {
            border-left: 0px;
        }
    </style>
    <div id="main-container">
        <div class="horizontal-separator">
        </div>
        <div id='recovery-area'>
            <asp:PasswordRecovery ID="PasswordRecoveryForm" OnVerifyingUser="PasswordRecoveryForm_VerifyingUser"
                OnUserLookupError="PasswordRecovery1_UserLookupError" runat="server">
                <UserNameTemplate>
                    <table cellpadding="4">
                        <tr>
                            <td valign="middle">
                                <asp:Label meta:resourcekey="UsernameOrEmailLabel" ID="UsernameOrEmailLabel" runat="server" />
                            </td>
                            <td>
                                <asp:TextBox runat="server" CssClass="input" ID="UserName"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:Button ID="SubmitButton" CommandName="Submit" ValidationGroup="RestorePassword"
                                    runat="server" CssClass="button" meta:resourcekey="SubmitButton" />
                            </td>
                        </tr>
                    </table>
                    <asp:RequiredFieldValidator Display="Dynamic" meta:resourcekey="UserNameRequired"
                        ID="UserNameRequired" Height="20px" ValidationGroup="RestorePassword" runat="server"
                        ControlToValidate="UserName" Font-Size="8" Font-Bold="true" />
                </UserNameTemplate>
                <MailDefinition BodyFileName="~\mail\PasswordRecoveryTemplate.txt" IsBodyHtml="false"
                    Subject="Asset manager: credentials recovery">
                </MailDefinition>
                <SuccessTemplate>
                    <asp:Label meta:resourcekey="SuccessText" ID="SuccessText" runat="server" /><br />
                    <asp:LinkButton meta:resourcekey="BackButton" ID="BackButton" runat="server" PostBackUrl="~/Login.aspx" />
                </SuccessTemplate>
            </asp:PasswordRecovery>
            <asp:CustomValidator Display="Dynamic" meta:resourcekey="UserNotFoundValidator" ID="UserNotFoundValidator"
                runat="server" Font-Size="8" Font-Bold="true" />
            <asp:Label runat="server" Visible="false" ForeColor="Red" EnableViewState="false" ID="lblSmtpErrorSettings"
                meta:resourcekey="lblSmtpErrorSettings" />
        </div>
        <div class="horizontal-separator">
        </div>
    </div>
</asp:Content>
