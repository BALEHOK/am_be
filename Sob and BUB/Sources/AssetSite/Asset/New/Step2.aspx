<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master" EnableEventValidation="false"
    AutoEventWireup="true" Inherits="AssetSite.Asset.New.Step2" Trace="false" CodeBehind="Step2.aspx.cs" meta:resourcekey="PageResource1"  %>

<%@ Register Src="~/Controls/AssetAttributePanels.ascx" TagName="AssetAttributePanels" TagPrefix="amcl" %>
<%@ Register Src="~/Controls/AssetToolbar.ascx" TagName="AssetToolbar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ScreensPanel.ascx" TagName="ScreensPanel" TagPrefix="uc1"  %>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
    <div class="wizard-menu">
         <uc1:ScreensPanel runat="server" ID="screensPanel" />

        <div class="item">
            <asp:Label runat="server" meta:resourcekey="LabelResource1" Text="Step 1"></asp:Label>            
        </div>
        <div class="active">
            <asp:Label runat="server" meta:resourcekey="LabelResource2" Text="Step 2"></asp:Label>            
        </div>
        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="LabelResourceTemplated"></asp:Label> 
        </div>
        <span style="font-size:smaller;">
            <asp:Literal ID="litTemplates" runat="server"></asp:Literal>
            <asp:Repeater ID="repTemplates" runat="server">
                <ItemTemplate>
                    <table border="0" cellpadding="0" cellspacing="0" id='<%#GetTableId(Eval("ID")) %>' width="100%">
                        <tr>
                            <td>
                                <div style="display:none;" id='<%#GetDivId(Eval("ID")) %>'>
                                    <%#GetAssetHtml(Eval("Template")) %>
                                </div>
                                <a href='<%#GetRestoreUrl(Eval("ID")) %>' class="assetTemplatePopupTrigger" style="cursor:pointer;" rel='<%#Eval("ID") %>' > <%#Eval("Template.Name") %> </a>
                            </td>
                            <td style="width:20%; vertical-align:middle; text-align:center;">
                                <a runat="server" id="btnRemoveTemplate" visible='<%#IsEditable() %>' style="cursor:pointer; border: 0;" onclick='<%#GetClickScript(Eval("ID")) %>' >
                                    <asp:Image runat="server" ImageUrl="~/images/buttons/delete.png" />
                                </a>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
        </span>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">    
    <asp:ScriptManager ID="MainScriptManager" runat="server" ScriptMode="Auto" EnablePageMethods="true">
     <Services>
        <asp:ServiceReference Path="~/amDataService.asmx" />
     </Services>
    </asp:ScriptManager> 
    <div id="main-container">

        <uc1:AssetToolbar ID="toolbar" runat="server" />
        <hr />
        <div class="wizard-header">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1"></asp:Label>            
        </div>
        <div>
            <asp:Label ID="lblNoPermissions" SkinID="labelNoPermissions" runat="server" Visible="false" Text="<%$ Resources:Global, NoPermissionsText%>" />                    
            <asp:PlaceHolder ID="RestoreSession" runat="server" Visible="False">
                <p><asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1">Previous version of asset was not saved</asp:Literal> (<asp:Label 
                        ID="Label1" runat="server" meta:resourcekey="Label1Resource1">created at</asp:Label>&nbsp; 
                    <asp:Label runat="server" ID="CreationTime" 
                        meta:resourcekey="CreationTimeResource1"></asp:Label>).</p>                                
                <a href='<%= Request.Url.OriginalString %>&Restore=1'><asp:Label ID="Label2" 
                    runat="server" meta:resourcekey="Label2Resource1" /></a>
            </asp:PlaceHolder>
        </div>

        <amcl:AssetAttributePanels runat="server" ID="AssetAttributePanel" Editable="true" />

        <div style="clear: both;">
        </div>
        <div class="wizard-footer-buttons">
            <asp:LinkButton ID="btnSave" runat="server" Text="save" OnClick="btnNext_Click" Visible="false"></asp:LinkButton>
            <asp:LinkButton ID="btnSaveAndAdd" runat="server" Text="savenadd" OnClick="btnSaveAndAdd_Click" Visible="false" />
            <asp:LinkButton ID="btnSaveTemplate" runat="server" Text="savetemplate" Visible="false" OnClick="btnSaveTemplate_Click" />
        </div>
    </div>
</asp:Content>
