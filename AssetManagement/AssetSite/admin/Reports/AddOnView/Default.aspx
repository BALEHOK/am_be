<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" MasterPageFile="~/MasterPages/MasterPageDefault.Master" Inherits="AssetSite.admin.Reports.AddOnView.Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Report info</asp:Label>
        </div>
        <div class="panelcontent">
            <table class="wh100p">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblbRepName" meta:resourcekey="lblbRepNameResource1">Report name</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="ReportName"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ReportName" Text="Name required"
                            meta:resourcekey="RequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" meta:resourcekey="SelectTemplate" Text="Select template file"></asp:Label> 
                    </td>
                    <td>
                        <asp:FileUpload runat="server" ID="TemplateFile" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
        <div class="wizard-footer-buttons">
        <asp:Button ID="Save" Text="Save report" runat="server" OnClick="SaveReportClick"
            meta:resourcekey="SaveResource1" />
    </div>
</asp:Content>