<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master"
    AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="AssetSite.Contact"
    meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server" meta:resourcekey="SiteMapPath1Resource1">
    </asp:SiteMapPath>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <asp:MultiView runat="server" ID="ContactMultiView" ActiveViewIndex="0">
        <asp:View runat="server">
            <h2>
                Contact us</h2>
            <p>
                <asp:Label runat="server" Text="Text" meta:resourcekey="LabelResource1"></asp:Label></p>
            <table id="ContactForm">
                <tr>
                    <td>
                        <asp:Label CssClass="label" runat="server" meta:resourcekey="LabelResource2">Subject</asp:Label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSubject" ValidationGroup="ContactForm"
                            meta:resourcekey="RequiredFieldValidatorResource1">*</asp:RequiredFieldValidator>
                        <br />
                        <asp:TextBox runat="server" CssClass="textbox" ID="txtSubject" meta:resourcekey="txtSubjectResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label CssClass="label" runat="server" meta:resourcekey="LabelResource3">Message</asp:Label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMessage" ValidationGroup="ContactForm"
                            meta:resourcekey="RequiredFieldValidatorResource2">*</asp:RequiredFieldValidator>
                        <br />
                        <asp:TextBox runat="server" ID="txtMessage" CssClass="textbox" Columns="40" Rows="10"
                            TextMode="MultiLine" meta:resourcekey="txtMessageResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" ValidationGroup="ContactForm"
                            meta:resourcekey="btnSendResource1" />
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View runat="server">
            <p>
                <asp:Literal runat="server" ID="lblContactFormSubmittedText" meta:resourcekey="lblContactFormSubmittedTextResource1"
                    Text="Thank you for your responce."></asp:Literal>
            </p>
        </asp:View>
        <asp:View ID="messageError" runat="server">
            <p>
                <asp:Literal runat="server" meta:resourcekey="lblContactFormErrorMessage"
                    Text="Error in SMTP settings."></asp:Literal>
            </p>
        </asp:View>
    </asp:MultiView>
</asp:Content>
