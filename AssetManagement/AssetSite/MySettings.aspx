<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/MasterPageDefault.master"
    Trace="false" CodeBehind="MySettings.aspx.cs" Inherits="AssetSite.MySettings"
    meta:resourcekey="PageMySettings" %>

<%@ Register Src="~/Controls/AssetAttributePanels.ascx" TagName="AssetAttributePanels"
    TagPrefix="amc" %>
<asp:Content ID="cMiddleColumn" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <asp:ScriptManager ID="MainScriptManager" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <asp:Label runat="server" ID="lblInfoMessage" ForeColor="Green" EnableViewState="false"
        Visible="false" meta:resourcekey="lblInfoMessage" />
    <amc:AssetAttributePanels runat="server" MySettingsPage="true" ID="aapUserSettings"
        Editable="true" />
    <asp:Button runat="server" ID="btnUpdate" meta:resourcekey="btnUpdate" OnClick="btnUpdate_Click" />
    <asp:Button runat="server" ID="btnCancel" meta:resourcekey="btnCancel" 
        onclick="btnCancel_Click" />
</asp:Content>
