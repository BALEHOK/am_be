<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssetTypeAttribList.ascx.cs" Inherits="AssetSite.Controls.AssetTypeAttribList" %>

<div id='<%= this.GetDivID() %>'>
<div class="hdr">
<asp:Label ID="lblAT" runat="server" />
</div>
<div class="panelcontent">
<asp:ListBox ID="lstAttributes" runat="server" Width="80%" Height="200px" SelectionMode="Multiple"></asp:ListBox><br />
</div>
</div>