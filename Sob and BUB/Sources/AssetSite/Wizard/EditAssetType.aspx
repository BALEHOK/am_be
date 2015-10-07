<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master" 
AutoEventWireup="true" Inherits="AssetSite.Wizard.EditAssetType" Codebehind="EditAssetType.aspx.cs" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="WizardContent" Runat="Server">
         <div class="panel" style="margin-top: 5px;">
            <div class="panelheader">
                <asp:Label runat="server" ID="lblPageTitle" 
                meta:resourcekey="lblPageTitleResource1"></asp:Label>                
            </div>
            <div class="panelcontent"> 
                    <asp:GridView 
                        ID="assetTypesGrid"
                        runat="server"
                        AutoGenerateColumns="False"                  
                        AllowPaging="True"
                        AllowSorting="true"
                        DataKeyNames="DynEntityConfigUid"      
                        DataSourceID="DynEntityConfigDataSource" 
                        OnRowDataBound="assetTypesGrid_RowDataBound"             
                        OnRowEditing="assetTypesGrid_Edit" 
                        meta:resourcekey="assetTypesGridResource1">
                        <EmptyDataTemplate>
                            <asp:Literal runat="server" ID="lblNoData" Text="list is empty" 
                                meta:resourcekey="lblNoDataResource1"></asp:Literal>
                        </EmptyDataTemplate>                            
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Name" 
                                meta:resourcekey="BoundFieldResource1" SortExpression="Name" HeaderStyle-HorizontalAlign="Left"/>
                            <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate>
                                    <asp:Label ToolTip='<%# Eval("Comment") %>' 
                                        ID="lblComment" runat="server" Text='<%# ChopString((String) Eval("Comment")) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CheckBoxField HeaderText="Is Active" DataField="Active" HeaderStyle-HorizontalAlign="Left"
                                meta:resourcekey="CheckBoxFieldResource1" SortExpression="Active" />
                            <asp:BoundField HeaderText="Revision" DataField="Revision" HeaderStyle-HorizontalAlign="Left"
                                meta:resourcekey="BoundFieldResource3" />
                            <asp:BoundField HeaderText="Update Date" DataField="UpdateDate" SortExpression="UpdateDate" HeaderStyle-HorizontalAlign="Left" />
                            <asp:TemplateField HeaderText="Tasks">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hplEditTasks" runat="server" NavigateUrl='<%#GetTasksUrl(Eval("DynEntityConfigId")) %>'>
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/task.png" />
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Screens">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#GetScreensUrl(Eval("DynEntityConfigUid")) %>'>
                                        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/screens.png" />
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField                                         
                                ButtonType="Image" 
                                DeleteText="Delete" 
                                DeleteImageUrl="~/images/buttons/delete.png"
                                EditText="Edit"
                                EditImageUrl="~/images/buttons/edit.png"
                                ShowEditButton="true"
                                ShowDeleteButton="true"
                                meta:resourcekey="CommandFieldResource1" />
                        </Columns>                            
                    </asp:GridView>  
                    
                    <asp:EntityDataSource runat="server" ID="DynEntityConfigDataSource"
                        AutoPage="true"
                        AutoSort="true"
                        EnableDelete="true"
                        OnDeleting="DynEntityConfig_Deleting"
                        EnableFlattening="false"
                        ConnectionString="name=DataEntities"
                        DefaultContainerName="DataEntities"                                    
                        EntitySetName="DynEntityConfig"
                        OrderBy="it.Name"
                        Where="it.ActiveVersion=true">
                    </asp:EntityDataSource>                       
                <br />
            </div>
         </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftColumn" runat="server">
 <div class="wizard-menu">
 <!--
    <div class="item pointer" onclick="javascript:location.href='/Wizard/Step1.aspx'">
        <asp:HyperLink runat="server" NavigateUrl="~/Wizard/Step1.aspx">Create Asset Type</asp:HyperLink> 
    </div>
    -->
    <div class="active">
        <asp:Label runat="server" meta:resourcekey="lblPageTitleResource1" />
    </div>
 </div>
</asp:Content>