<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynListListBox.ascx.cs" Inherits="AssetSite.Controls.DynListListBox" %>

<asp:ListBox ID="ListItems" runat="server" SelectionMode="Multiple" CssClass="multiselect" Visible="false">
</asp:ListBox>

<asp:Literal runat="server" ID="ItemText" Visible="true"></asp:Literal>

<asp:PlaceHolder runat="server" ID="DispControls"></asp:PlaceHolder>