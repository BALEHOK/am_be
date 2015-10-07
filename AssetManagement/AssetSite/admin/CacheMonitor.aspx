<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CacheMonitor.aspx.cs" Inherits="AssetSite.admin.CacheMonitor" 
 MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>
 
 <asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server"> 
    <b><asp:Literal ID="Literal1" runat="server" 
         meta:resourcekey="Literal1Resource1" Text="Cache statistics"></asp:Literal></b>
    <p><asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" 
            Text="Object count:"></asp:Literal> <asp:Label runat="server" ID="ObjCount" 
            meta:resourcekey="ObjCountResource1"></asp:Label></p>
    <p><asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" 
            Text="Memory usage:"></asp:Literal> <asp:Label runat="server" ID="MemUsage" 
            meta:resourcekey="MemUsageResource1"></asp:Label></p>
    <asp:Button runat="server" Text="Flush cache" OnClick="FlushCacheClicked" 
         meta:resourcekey="ButtonResource1" />            
 </asp:Content>