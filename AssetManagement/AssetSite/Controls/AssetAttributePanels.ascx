<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetAttributePanels.ascx.cs"
    Inherits="AssetSite.Controls.AssetAttributePanels" %>
<%@ Register Src="~/Controls/NewAttributePanel.ascx" TagName="NewAttributePanel"
    TagPrefix="amc" %>

<div>
    <asp:Literal ID="ValidationRep" runat="server"></asp:Literal>
</div>
<asp:Repeater runat="server" ID="panelsRepeater">
    <ItemTemplate>
        <amc:NewAttributePanel 
            runat="server" 
            ID="AttributesPanel"           
            Header='<%# Bind("Key.Name") %>' 
            AssignedAttributes='<%#  Bind("Value") %>' 
            Editable='<%# Editable %>'
            MySettingsPage='<%# MySettingsPage %>' />    
    </ItemTemplate>
</asp:Repeater>

