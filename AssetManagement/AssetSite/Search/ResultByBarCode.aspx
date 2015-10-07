<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageSearchResult.master" AutoEventWireup="true" CodeBehind="ResultByBarcode.aspx.cs" Inherits="AssetSite.Search.NewResultByBarcode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SearchTopBar" runat="server">
  <div class="panel">
    <div class="panelSearch">
	    <table width="100%">
		    <tr>
			    <td width="90%">
				    <asp:TextBox ID="tbSearch" runat="server" Width="100%" 
					    meta:resourcekey="tbSearchResource1"></asp:TextBox>
			    </td>
			    <td  align="right">
				    <asp:Button ID="ButtonSearch" runat="server" Text="Search" 
					    onclick="ButtonSearch_Click" meta:resourcekey="ButtonSearchResource1" />
			    </td>
		    </tr>
						
	    </table>
    </div>
  </div>
</asp:Content>
