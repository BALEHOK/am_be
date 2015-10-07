<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynListDropDown.ascx.cs"
    Inherits="AssetSite.Controls.DynListDropDown" %>
<%@ Reference Control="~/Controls/DynListItemEdit.ascx" %>
<%@ Register Src="~/Controls/DynListEditDialog.ascx" TagName="DynListEditDialog" TagPrefix="uc1" %>

<div style="white-space:nowrap;">
    <asp:PlaceHolder runat="server" ID="DynListControls">
    </asp:PlaceHolder>
     <uc1:DynListEditDialog runat="server" ID="EditDialog" Visible="False" />
</div>
