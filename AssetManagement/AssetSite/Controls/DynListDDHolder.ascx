<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynListDDHolder.ascx.cs" Inherits="AssetSite.Controls.DynListDDHolder" EnableViewState="false" %>
<%@ Reference Control="~/Controls/DynListDropDown.ascx" %>
<%@ Register Src="~/Controls/DynListDropDown.ascx" TagName="DynListDropDown"
    TagPrefix="amc" %>
 
<asp:PlaceHolder runat="server" ID="DynListDDControls"></asp:PlaceHolder>
<asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Repeater ID="repDropDownLists" runat="server">
            <ItemTemplate>
                <amc:DynListDropDown ID="ddlDynList" runat="server" 
                    ItemName='<%# Eval("Value") %>'
                    ListUID='<%# Eval("ListId") %>'
                    StateIndex='<%# Eval("Index") %>' 
                    SelectedValue='<%# Eval("ItemId") %>'
                    Editable='<%# IsEditable() %>'
                    OnSelectionChanged="OnChildSelectionChanged" />
            </ItemTemplate>
        </asp:Repeater>          
    </ContentTemplate>
</asp:UpdatePanel>
