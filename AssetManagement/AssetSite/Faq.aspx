<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master" AutoEventWireup="True" CodeBehind="Faq.aspx.cs" Inherits="AssetSite.Faq" %>

<asp:Content ID="Content2" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
	<asp:SiteMapPath ID="SiteMapPath1" runat="server"></asp:SiteMapPath>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
	<div id="accordion" class="faq">
		<asp:Repeater runat="server" ID="Repeater1">
			<ItemTemplate>
				<div class="header question">
					<%# Eval("[\"Question\"].Value") %>
				</div>
				<div class="answer">
					<%# Eval("[\"Answer\"].Value") %>
				</div>
			</ItemTemplate>       
		</asp:Repeater>
	</div>
	<script>
		$(function () {
			$("#accordion").accordion();
		});
	</script>
</asp:Content>