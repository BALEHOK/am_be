<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="LocationMove.aspx.cs" Inherits="AssetSite.admin.LocationMove" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/MultipleAssetsList.ascx" TagName="AssetList" TagPrefix="uc1"  %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager ID="svcsManager" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1">Move to next location</asp:Label>            
        </div>
        <div class="panelcontent">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="lblbDescription" runat="server" 
                            Text="Process description goes here" 
                            meta:resourcekey="lblbDescriptionResource1"></asp:Label><br /><br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" meta:resourcekey="LabelResource1">Choose Locations</asp:Label>
                        <uc1:AssetList ID="locationsList" runat="server" AssetTypeName="Location" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <asp:Label runat="server" meta:resourcekey="LabelResource2">Select Asset Types</asp:Label><br />
                        <asp:ListBox ID="lstAssetTypes" runat="server" DataValueField="ID" 
                            DataTextField="Name" SelectionMode="Multiple" Width="300px" Rows="10" 
                            meta:resourcekey="lstAssetTypesResource1"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblValidation" ForeColor="Red" runat="server" Visible="False"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <asp:Button ID="btnMove" runat="server" Text="Move" onclick="btnMove_Click" 
                            meta:resourcekey="btnMoveResource1" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
</asp:Content>
