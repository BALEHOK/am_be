<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    AutoEventWireup="true" Inherits="AssetSite.Login" Trace="false" CodeBehind="Login.aspx.cs" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">

<style type="text/css">
    #outer-column-container
    {
        border-left: 0px;
    }
</style>

    <div id="main-container">        
        <div class="horizontal-separator"></div>        
        <div id='login-area'>
            <div class="login-area-block">
                <table cellpadding="3" cellspacing="0">
                    <tr>
                        <td>                    
                            <asp:Label runat="server" ID="lblLogin" Text="Login" 
                                meta:resourcekey="lblLoginResource1"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="UserLogin" 
                                meta:resourcekey="UserLoginResource1"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPwd" Text="Password" 
                                meta:resourcekey="lblPwdResource1"></asp:Label>                    
                        </td>
                        <td>
                            <asp:TextBox TextMode="Password" runat="server" ID="Password" 
                                meta:resourcekey="PasswordResource1"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblRememberMe" Text="Remember me?" 
                                meta:resourcekey="lblRememberMeResource1"></asp:Label>                    
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="RememberMe" Checked="True" 
                                meta:resourcekey="RememberMeResource2" />
                        </td>
                    </tr>
                    <tr>
                        <asp:Literal runat="server" ID="ErrorMessage" 
                            meta:resourcekey="ErrorMessageResource1"></asp:Literal>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Button runat="server" ID="Submit" Text="Login" OnClick="LoginClicked" 
                                meta:resourcekey="SubmitResource1" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="login-area-block">
                <ul>
                    <li><asp:LinkButton runat="server" PostBackUrl="~/RecoverCredentials.aspx" ID="RecoverPasswordLink" meta:resourcekey="RecoverPasswordLink" /></li>
                    <li><asp:LinkButton runat="server" PostBackUrl="~/RecoverCredentials.aspx" ID="RecoverUsernameLink" meta:resourcekey="RecoverUsernameLink" /></li>
                </ul>
            </div>
            <div class="clear"></div>
        </div> 

        <div class="horizontal-separator"></div>
    </div>
</asp:Content>
