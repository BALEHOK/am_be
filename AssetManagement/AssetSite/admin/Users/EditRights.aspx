<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRights.aspx.cs" Inherits="AssetSite.admin.Users.AddRights"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>
    
<%@ Register Assembly="AppFramework.Core" Namespace="AppFramework.Core.PL.Components.CatTax" TagPrefix="amc" %> 
<%@ Register Assembly="AppFramework.Core" Namespace="AppFramework.Core.PL" TagPrefix="amc" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" meta:resourcekey="LabelResource1">Permissions editing for user:</asp:Label><%= this.Username %>            
        </div>
        <div class="panelcontent">
     
            <asp:ScriptManager 
                runat="server" 
                AsyncPostBackErrorMessage="Error while building list. Please refresh the page and try again"
                ID="ScriptManager1">
            </asp:ScriptManager> 
            
            <div class="options">   
                <div style="float: left">
                <asp:Label runat="server" meta:resourcekey="LabelResource2">Rule behavior:</asp:Label>                 
                <asp:RadioButtonList ID="ruleType" runat="server" 
                        meta:resourcekey="ruleTypeResource1">
                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1"></asp:ListItem>
                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2"></asp:ListItem>
                </asp:RadioButtonList>
                </div>
                <div style="float: left">
                <asp:Label runat="server" meta:resourcekey="LabelResource3">Propagate on:</asp:Label>                
                
                <asp:RadioButtonList runat="server" ID="propagateGroup" 
                        OnSelectedIndexChanged="propagateGroup_Changed" AutoPostBack="True" 
                        meta:resourcekey="propagateGroupResource1">
                    <asp:ListItem Value="0" Text="All Items" meta:resourcekey="ListItemResource3"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Specific Items" Selected="True" 
                        meta:resourcekey="ListItemResource4"></asp:ListItem>                    
                </asp:RadioButtonList>                    
                    
                <asp:CheckBoxList ID="ruleConditions" runat="server" 
                        OnSelectedIndexChanged="ruleConditions_Changed" AutoPostBack="True" 
                        meta:resourcekey="ruleConditionsResource1">
                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource5">Categories</asp:ListItem>
                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource6">Asset types</asp:ListItem>
                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource7">Departments</asp:ListItem>
                </asp:CheckBoxList>
                </div>
                <div>                    
                    <asp:Label runat="server" ID="lblPermissions" meta:resourcekey="LabelResource4">Permissions:</asp:Label>
                    <asp:CheckBoxList ID="permissionsSet" runat="server" CssClass="permissions"
                        meta:resourcekey="permissionsSetResource1">
                        <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource8">Read Normal Info</asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource9">Write Normal Info</asp:ListItem>
                        <asp:ListItem Value="2" Selected="True" meta:resourcekey="ListItemResource10">Read Financial Info</asp:ListItem>
                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource11">Write Financial Info</asp:ListItem>
                        <asp:ListItem Value="4">Delete</asp:ListItem>
                    </asp:CheckBoxList>
                </div>
                <div style="clear: both"></div>
            </div>
            
            <div id="propagateitems">            
                <asp:UpdatePanel ID="ruleItems" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <asp:Panel ID="categoriesPanel" runat="server" Visible="true">                    
                        <div class="panelsection categories">
                            <amc:TaxonomiesDropDown 
                                ID="taxonomiesDropDown" 
                                CssClass="taxonomiesDropDown"
                                runat="server"                                
                                OnSelectedTaxonomyChanged="taxonomiesDropDown_Changed">
                            </amc:TaxonomiesDropDown>  
                                                      
                           <%-- <amc:HelpButton 
                                ID="HelpButton1" 
                                Title="Be careful!" 
                                Message="All checked items will be lost on category\'s changing." 
                                Level="Error" 
                                runat="server" />--%>
                                                      
                            <amc:TaxonomiesTree                                                  
                                ID="taxonomyTree"                                 
                                runat="server"                                 
                                Visible="true"                               
                                ShowLines="true"
                                ShowCheckboxes="All"                                      
                                ShowManageLink="false"
                                CssClass="taxonomyTree"                                                                         
                                ShowAddNodeLink="false">
                            </amc:TaxonomiesTree>                       
                        </div>
                    </asp:Panel>
                    
                    <asp:Panel ID="assetTypesPanel" runat="server" Visible="false">                    
                        <div class="panelsection assettypes">
                            <asp:GridView 
                                ID="atGrid" 
                                AutoGenerateColumns="false"  
                                CssClass="rightsgrid"
                                HeaderStyle-CssClass="header"
                                AlternatingRowStyle-CssClass="alterrow"
                                RowStyle-CssClass="row"
                                GridLines="None"      
                                DataKeyNames="ID"                     
                                runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Name" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="atCheckbox" runat="server"/>
                                            <asp:HiddenField ID="atID" Value='<%# Eval("ID") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    list is empty
                                </EmptyDataTemplate>
                            </asp:GridView>                         
                        </div>                                 
                    </asp:Panel>
                   
                    <asp:Panel ID="departmentsPanel" runat="server" Visible="false">                   
                        <div class="panelsection departments">
                             <asp:GridView 
                                ID="deptGrid" 
                                AutoGenerateColumns="false"  
                                CssClass="assetsgrid"
                                HeaderStyle-CssClass="header"
                                AlternatingRowStyle-CssClass="alterrow"
                                RowStyle-CssClass="row"
                                GridLines="None"                          
                                runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Name" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="deptCheckbox" runat="server" />
                                            <asp:HiddenField ID="deptID" Value='<%# Eval("ID") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    list is empty
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ruleConditions" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            
            <div class="wizard-footer-buttons">        
                <asp:Button ID="btnSave" runat="server" Text="<% $Resources:Global, SaveText %>" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="<% $Resources:Global, CancelText %>" OnClick="btnCancel_Click" />        
            </div>

        </div>
    </div>
     <script type="text/javascript">
         $(document).ready(function() {
             $('#propagateitems').delegate('click', '.taxonomyTree :checkbox', function() {
                 var p = $(this).closest('table').next('div');
                 if (p.size() == 0) return;
                 $(' :checkbox', p).attr('checked', $(this).attr('checked'));
             });

             $('.permissions :checkbox').click(function() {
                   
                 // if deny reading normal, uncheck all   
                 if (!$(this).attr('checked') && $(this).attr('name') == $('.permissions :checkbox').attr('name')) {
                     $('.permissions :checkbox').each(function() { $(this).removeAttr('checked') });
                 }

                 // which item it is?
                 var parts = $(this).attr('name').split('$');
                 var pos = parts[parts.length - 1]; 

                 if ($(this).attr('checked')) {
                     $($('.permissions :checkbox')[0]).attr('checked', 'checked');  // always allow normal reading
                     if (pos % 2 == 1) {                                            // if allow writing (normal or financial)
                         $($('.permissions :checkbox')[pos - 1]).attr('checked', 'checked'); // then allow reading next one
                     }
                     if (pos == 4) { // if allow delete
                         $('.permissions :checkbox').attr('checked', 'checked'); // then allow everything
                     }
                 } 

                 if (!$(this).attr('checked') && (pos == 2)) { // if deny reading financial                    
                     $($('.permissions :checkbox')[3]).removeAttr('checked'); // then deny writing financial
                 }

                 if ($($('.permissions :checkbox')[4]).attr('checked')) {
                     $('.permissions :checkbox').attr('checked', 'checked');
                 }
             });
         });        
    </script>
</asp:Content>
