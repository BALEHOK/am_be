<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step11" Codebehind="Step11.aspx.cs" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
 
        <div class="wizard-header">
            <asp:Label runat="server" ID="lblheader" Text="Asset Wizard" 
                meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp; 
            <asp:Literal runat="server" ID="stepTitle" 
                 Text="Schermweergave — Release-informatie" 
                meta:resourcekey="stepTitleResource1"></asp:Literal> 
        </div>
        <p>
            <asp:Literal runat="server" ID="stepDesc" 
                Text="Mauris congue consectetuer quam." meta:resourcekey="stepDescResource1"></asp:Literal>
        </p>        
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelTitle" Text="Release-informatie" 
                    meta:resourcekey="panelTitleResource1"></asp:Label>                
            </div>
            <div class="panelcontent">
                <asp:Label runat="server" ID="lblValidation" ForeColor="Red" Visible="false" Text="At least one attribute must be selected to use within Name autogeneration." /><br />

                <asp:Label runat="server" ID="lblName" Text="<% $Resources:Global, NameText %>" ></asp:Label>:               
                <%= AssetType.Name %><br />
                <asp:Label runat="server" ID="lblDesc" Text="<% $Resources:Global, DescText %>"></asp:Label>:                
                <%= AssetType.Comment %><br />
                <asp:Label runat="server" ID="lblAttrs" Text="<% $Resources:Global, AttributesText %>"></asp:Label>:
                <br />
                <asp:ListBox ID="listAttributes" runat="server" CssClass="listAttributes" 
                    meta:resourcekey="listAttributesResource1"></asp:ListBox>
                <br />
                <asp:CheckBox runat="server" ID="ckNewRevision" Text="Create new revision" Checked="False" meta:resourcekey="ckNewRevision" />
            </div>
        </div>
        <div class="wizard-footer-buttons">
            <asp:Button ID="btnPrevious" Text="<% $Resources:Global, PreviousText %>" runat="server" 
                OnClick="btnPrevious_Click" />
            <asp:Button ID="btnFinish" runat="server" Text="Finish" 
                OnClick="btnFinish_Click" meta:resourcekey="btnFinishResource1" />  
            <asp:Button ID="btnPublish" runat="server" Text="Publish" 
                OnClick="Publish_Click" meta:resourcekey="btnPublishResource1" />        
        </div>
</asp:Content>
