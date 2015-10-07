<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.Install.Default1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SOB en BUB - Application Global Settings</title>
     <script src="../javascript/jquery-1.4.4.min.js" type="text/javascript"></script>
    <link href="/css/theme.css" rel="stylesheet" type="text/css" />
    <link href="/css/installer.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="outerWrapper"> 
        <div class="banner">
            <img src="/images/headerban.jpg" alt="SOB en BUB logo" />
        </div>    
        <div class="innerWrapper"> 
            <h1>Application global settings</h1>
            <h3>Database settings</h3>
            <table style="width: 100%;">
                <tr>
                    <td>
                        <asp:Label ID="LabelHost" runat="server" Text="Host:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtHost" runat="server"></asp:TextBox>
                    </td>           
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Database:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDatabase" runat="server"></asp:TextBox>
                    </td>                       
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="User:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUser" runat="server" CssClass="user"></asp:TextBox>
                    </td>                         
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Password:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="pwd"></asp:TextBox>                  
                    </td>                         
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIntSec" runat="server" CssClass="chk" /><asp:Label ID="Label4" runat="server"
                            Text="Integrated security"></asp:Label>
                    </td>
                </tr>                
            </table>
            <p><asp:Label 
                Visible="false"
                ID="lblCnnState"
                runat="server"
                ForeColor="Red" 
                Text="Cannot connect to Database. Please check the settings and try again."></asp:Label></p>               
            
            <h3>Administrator's settings:</h3>
            
            <a href="javascript:void(0);" onclick="$('#pwdContainer').toggle(100);">Change password</a>  
            <div id="pwdContainer" style="display:none;">                 
                 <table style="width: 100%;">
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Old password:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAdminOldPassword" TextMode="Password"></asp:TextBox>
                        </td>           
                    </tr>   
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="New password:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAdminNewPassword" TextMode="Password"></asp:TextBox>
                        </td>           
                    </tr>       
                 </table>
             </div>
             
            <asp:Label ID="lblPwdStatus" runat="server" Visible="false"></asp:Label>
            
            <hr />
            <p>
                <asp:Button ID="Button1" runat="server" Text="Save all changes" OnClick="Apply_Click" />
            </p>
            
            <script type="text/javascript">
                $(document).ready(function() {
                    $('.chk').click(function() {                           
                        if ($('.pwd').attr('disabled')) {
                            $('.pwd').removeAttr('disabled');
                            $('.user').removeAttr('disabled');
                        }
                        else {
                            $('.pwd').attr('disabled', 'disabled');
                            $('.user').attr('disabled', 'disabled');
                        }
                    });
                });
            </script>               
        </div>          
    </div>
    </form>
</body>
</html>
