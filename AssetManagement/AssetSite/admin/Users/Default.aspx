<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Users.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
    <div id="main-container">
          <div class="wizard-header">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Users</asp:Label>
          </div>
          <div class="panel">
            <div class="panelheader"> 
                <asp:Label runat="server" ID="panelTitle" 
                    meta:resourcekey="panelTitleResource1">List of users</asp:Label>                
            </div>
            <div class="panelcontent">
               <asp:GridView ID="gvUsers" runat="server" 
                    AutoGenerateColumns="False" 
                    AllowPaging="true"
                    AllowSorting="true"                     
                    OnRowDataBound="gvUsers_DataBound"                    
                    DataSourceID="usersDataSource">         
                    <Columns>                   
                        <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="<% $Resources:Global, NameText %>"  />
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <a href="/Asset/View.aspx?AssetUID=<%# Eval("UID") %>&AssetTypeUID=<%# Eval("DynEntityConfigUid") %>"><asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/zoom.png" /></a>                                    
                                <a href="/Asset/Edit.aspx?AssetUID=<%# Eval("UID") %>&AssetTypeUID=<%# Eval("DynEntityConfigUid") %>"><asp:Image ID="Image3" runat="server" ImageUrl="~/images/buttons/edit.png" /></a>                                    
                                <asp:LinkButton 
                                    ID="lbtnDelete" 
                                    runat="server"      
                                    CommandArgument='<%#Eval("ID") %>'                               
                                    OnClick="lbtnDelete_Click">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No users</asp:Label>                    
                    </EmptyDataTemplate>
                </asp:GridView>
                
                <asp:ObjectDataSource 
                    runat="server"
                    EnablePaging="True"
                    ID="usersDataSource"
                    SelectMethod="GetAssetsByAssetTypeId"
                    StartRowIndexParameterName="rowStart"                                                        
                    MaximumRowsParameterName="rowsNumber"                            
                    SelectCountMethod="GetAssetsCountByAssetTypeId" 
                    OnSelecting="usersDataSource_Selecting"
                    TypeName="AppFramework.Core.Classes.AssetFactory">   
                    <SelectParameters>
                       <asp:Parameter Name="filterWithPermissions" Type="Boolean" DefaultValue="false" />
                    </SelectParameters>                                                            
                </asp:ObjectDataSource>  

                <asp:Panel ID="newAssetButtonPanel" CssClass="newAssetButton" runat="server" 
                            meta:resourcekey="newAssetButtonPanelResource1">
                    <a href="/Asset/New/Step2.aspx?atid=<%= AssetTypeId %>">
                        <asp:Image runat="server" AlternateText="New user" ToolTip="New user" 
                          ImageUrl="~/images/buttons/document--plus.png" 
                          meta:resourcekey="ImageResource1" />                        
                    </a>
                </asp:Panel>  
                             
            </div>
           </div>
    </div>
</asp:Content>