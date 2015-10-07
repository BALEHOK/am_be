<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step1" CodeBehind="Step1.aspx.cs"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Asset Type Wizard" meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp;
        <asp:Label runat="server" Text="Asset Naam" meta:resourcekey="LabelResource1"></asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eleifend lacinia leo. Sed consequat libero a ligula! Curabitur bibendum elit sit amet orci. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Aliquam arcu."
            meta:resourcekey="LiteralResource1"></asp:Literal>
    </p>
    <div>
        <asp:PlaceHolder ID="RestoreSession" runat="server" Visible="False">
            <p>
                <asp:Literal ID="Literal1" runat="server" Text="Previous version of asset was not saved"
                    meta:resourcekey="Literal1Resource1"></asp:Literal>
                (<asp:Literal ID="Literal2" runat="server" Text="created at" meta:resourcekey="Literal2Resource1"></asp:Literal>&nbsp;<asp:Label
                    runat="server" ID="CreationTime" meta:resourcekey="CreationTimeResource1"></asp:Label>).</p>
            <a href='<%= Request.Url.OriginalString %>?Restore=1'>
                <asp:Label ID="Label1" runat="server" Text="Restore asset" meta:resourcekey="Label1Resource1"></asp:Label></a>
            <a href='<%= Request.Url.OriginalString %>?Restore=1&Remove=true'>
                <asp:Label ID="lblRemoveMessage" runat="server" Text="Remove message" meta:resourcekey="lblRemoveMessage"></asp:Label></a>
        </asp:PlaceHolder>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" Text="Asset Naam" meta:resourcekey="LabelResource2"></asp:Label>
        </div>
        <div class="panelcontent">
            <br />
            <div class="labels">
                <label>
                    <asp:Label runat="server" Text="<% $Resources:Global, NameText %>"></asp:Label></label>
            </div>
            <div class="controls">
                <asp:TextBox ID="Name" MaxLength="60" runat="server" autofocus="focus" CssClass="input-text name"
                    meta:resourcekey="NameResource1"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator"
                    SetFocusOnError="True" ControlToValidate="Name" Display="Dynamic" Text="Name is required"
                    meta:resourcekey="RequiredFieldValidator1Resource1">
                </asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="NameFormatValidator" runat="server" Display="Dynamic"
                    SetFocusOnError="True" ControlToValidate="Name" meta:resourcekey="NameFormatValidator"
                    ValidationExpression="^[A-Za-z]{1}[A-Za-z0-9\s_]+$">
                </asp:RegularExpressionValidator>
                <asp:CustomValidator 
                    ID="AssetTypeExistsValidator" 
                    runat="server" 
                    ControlToValidate="Name" 
                    OnServerValidate="AssetTypeExistsValidator_ServerValidate">
                </asp:CustomValidator>
            </div>
            <div class="third-column">
                <asp:Label runat="server" Text="Revision:" meta:resourcekey="LabelResource4"></asp:Label>&nbsp;<%= this.GetRevision() %>&nbsp;
                <asp:Label ID="Draft" runat="server" ForeColor="Red" Font-Bold="True" meta:resourcekey="DraftResource1"></asp:Label>&nbsp;
                <a href="javascript:void(0)" onclick="showTranslations()">Add translation</a>
            </div>
            <div style="clear: both">
            </div>
            <div class="labels">
                <label>
                    <asp:Label runat="server" Text="<% $Resources:Global, DescText %>"></asp:Label></label><br />
            </div>
            <div class="controls">
                <asp:TextBox ID="Description" runat="server" Height="55px" MaxLength="1000" TextMode="MultiLine"
                    Width="450px" meta:resourcekey="DescriptionResource1"></asp:TextBox>
            </div>
            <div style="clear: both">
            </div>
            <div class="labels" style="margin-top: 15px;">
                <label>
                    <asp:Label runat="server" ID="lblAssetTypeClass" Visible="false" Text="<% $Resources:Global, TypeText %>"></asp:Label></label>
            </div>
            <div class="controls" style="margin-top: 15px;">
                <asp:DropDownList runat="server" ID="ddlAssetTypeClass" Visible="false">
                    <asp:ListItem Value="0">Normal Asset Type</asp:ListItem>
                    <asp:ListItem Value="1">Data Asset Type</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <amc:Translations ID="Translations1" ControlSelector=".controls .name" runat="server" />
</asp:Content>
