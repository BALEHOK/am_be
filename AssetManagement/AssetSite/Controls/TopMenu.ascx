<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopMenu.ascx.cs" Inherits="AssetSite.Controls.TopMenu" %>
<div id="menubarright">

	<asp:Panel runat="server" ID="menuLeft" CssClass="toleft headmenu"  
			meta:resourcekey="menuLeftResource1">
		<asp:HyperLink ID="menuSearch" runat="server"
			NavigateUrl="~/Search/Search.aspx" meta:resourcekey="menuSearchResource1">Search</asp:HyperLink>
		<asp:HyperLink ID="menuTasks" runat="server" 
			NavigateUrl="~/TaskView.aspx" meta:resourcekey="menuTasksResource">Tasks</asp:HyperLink>
		<asp:HyperLink ID="menuCategories" runat="server" 
			NavigateUrl="~/AssetView.aspx" meta:resourcekey="menuCategoiresResource">Categories</asp:HyperLink>
		<asp:HyperLink ID="menuDocuments" runat="server" 
			NavigateUrl="~/Documents/Default.aspx" meta:resourcekey="menuDocumentsResource1">Documents</asp:HyperLink>        
			<asp:HyperLink ID="menuFinancial" runat="server" 
			NavigateUrl="/Financial/Default.aspx" meta:resourcekey="menuFinancialResource1">Financial</asp:HyperLink>
		<asp:HyperLink ID="menuReport" runat="server" 
			NavigateUrl="~/Reports/List.aspx" meta:resourcekey="menuReportResource1">Reports</asp:HyperLink>
		<asp:HyperLink ID="menuLending" runat="server" 
			NavigateUrl="~/Reservations/Overview.aspx" meta:resourcekey="menuLengingResource1">Reservation</asp:HyperLink>
	</asp:Panel>

					   
	<asp:Panel runat="server" ID="menuRight" CssClass="toright headmenu">
		<asp:LoginView runat="server">
			<RoleGroups>
				<asp:RoleGroup Roles="Administrators">
					<ContentTemplate>
						<asp:HyperLink ID="menuAdmin" runat="server" 
							NavigateUrl="~/admin/Default.aspx" 
							meta:resourcekey="menuAdminResource1">Admin</asp:HyperLink>
					</ContentTemplate>
				</asp:RoleGroup>
			</RoleGroups>
		</asp:LoginView>

		<div id="LangSwitcher">									
			<asp:DropDownList runat="server" ID="langDropdown" AutoPostBack="True"
				DataSourceID="LanguagesDataSource"
				DataTextField="ShortName" 
				DataValueField="CultureName"                                    
				OnSelectedIndexChanged="Language_Changed" 
				OnDataBound="langDropdown_DataBound"
				meta:resourcekey="langDropdownResource1">		      
			</asp:DropDownList>  
								
			<asp:EntityDataSource runat="server" ID="LanguagesDataSource"
				ConnectionString="name=DataEntities"
				DefaultContainerName="DataEntities"                                    
				EntitySetName="Languages">
			</asp:EntityDataSource>
															
		</div>
	</asp:Panel> 
</div>