<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTypes.aspx.cs" Inherits="AssetSite.admin.DataTypes"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:GridView ID="DataTypesList" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="UID" meta:resourcekey="DataTypesListResource1">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="<% $Resources:Global, NameText %>" HeaderStyle-HorizontalAlign="Left"  />
            <asp:BoundField DataField="Base.DBDataType" HeaderText="Database type" 
                meta:resourcekey="BoundFieldResource3" HeaderStyle-HorizontalAlign="Left" />
            <asp:HyperLinkField DataNavigateUrlFields="UID" 
                DataNavigateUrlFormatString="DataTypesSearchOps.aspx?Id={0}" 
                Text="<% $Resources:Global, SearchOperatorsText %>" />
        </Columns>
              
        <EmptyDataTemplate>
            <asp:Literal runat="server" ID="lblNoData" 
                meta:resourcekey="lblNoDataResource1" Text="No data types"></asp:Literal>            
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
