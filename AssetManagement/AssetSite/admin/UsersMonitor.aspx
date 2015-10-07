<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsersMonitor.aspx.cs" Inherits="AssetSite.admin.UsersMonitor" MasterPageFile="~/MasterPages/MasterPageDefault.master" meta:resourcekey="PageResource1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" Runat="Server">
    <asp:GridView ID="Users" runat="server" AutoGenerateColumns="False" 
        meta:resourcekey="UsersResource1">
        <Columns>
            <asp:BoundField DataField="Username" meta:resourcekey="BoundFieldResource1" />
            <asp:CheckBoxField DataField="IsOnline" 
                meta:resourcekey="CheckBoxFieldResource1" />            
        </Columns>
    </asp:GridView>
</asp:Content>