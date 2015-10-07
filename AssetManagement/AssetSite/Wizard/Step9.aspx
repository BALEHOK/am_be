<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step9" Codebehind="Step9.aspx.cs" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
    
        
        <div class="wizard-header">
            <asp:Label runat="server" ID="lblheader" Text="Asset Wizard" 
                meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp; 
            <asp:Literal runat="server" ID="stepTitle" 
                 Text="Schermweergave — Keywords & Index" 
                meta:resourcekey="stepTitleResource1"></asp:Literal>                   
        </div>
        <p>
            <asp:Literal runat="server" ID="stepDesc" 
                Text="Mauris congue consectetuer quam." meta:resourcekey="stepDescResource1"></asp:Literal>
        </p>
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelTitle" Text="Definieer indexes" 
                    meta:resourcekey="panelTitleResource1"></asp:Label>                
            </div>
            <div class="panelcontent">
                <asp:GridView ID="gridAttributes" runat="server" AutoGenerateColumns="False" 
                    meta:resourcekey="gridAttributesResource1">
                    <Columns>                        
                        <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                            <ItemTemplate>
                                <asp:HiddenField ID="UID" runat="server" Value='<%# Eval("UID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NameLocalized" HeaderText="Name" 
                            meta:resourcekey="BoundFieldResource1" />

                        <asp:TemplateField HeaderText="Display On Result List" 
                            meta:resourcekey="TemplateFieldResource2">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBoxDisplayOnResultList" runat="server" 
                                    Checked='<%# Bind("DisplayOnResultList") %>' 
                                    meta:resourcekey="CheckBoxDisplayOnResultListResource2" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxDisplayOnResultList" runat="server" 
                                    Checked='<%# Bind("DisplayOnResultList") %>' 
                                    meta:resourcekey="CheckBoxDisplayOnResultListResource1" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Display On Ext Result List" 
                            meta:resourcekey="TemplateFieldResource3">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBoxDisplayOnExtResultList" runat="server" 
                                    Checked='<%# Bind("DisplayOnExtResultList") %>' 
                                    meta:resourcekey="CheckBoxDisplayOnExtResultListResource2" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxDisplayOnExtResultList" runat="server" 
                                    Checked='<%# Bind("DisplayOnExtResultList") %>' 
                                    meta:resourcekey="CheckBoxDisplayOnExtResultListResource1" />
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Description" 
                            meta:resourcekey="TemplateFieldResource4">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBoxDescription" runat="server" 
                                    Checked='<%# Bind("IsDescription") %>' 
                                    meta:resourcekey="CheckBoxDescriptionResource2" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxDescription" runat="server" 
                                    Checked='<%# Bind("IsDescription") %>' 
                                    meta:resourcekey="CheckBoxDescriptionResource1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       
                        <asp:TemplateField HeaderText="Keyword" 
                            meta:resourcekey="TemplateFieldResource5">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBoxKeyword" runat="server" 
                                    Checked='<%# Bind("IsKeyword") %>' 
                                    meta:resourcekey="CheckBoxKeywordResource2" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxKeyword" runat="server" 
                                    Checked='<%# Bind("IsKeyword") %>' 
                                    meta:resourcekey="CheckBoxKeywordResource1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Index" meta:resourcekey="TemplateFieldResource6">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBoxIndex" runat="server" 
                                    Checked='<%# Bind("IsFullIndex") %>' 
                                    meta:resourcekey="CheckBoxIndexResource2" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxIndex" runat="server" 
                                    Checked='<%# Bind("IsFullIndex") %>' 
                                    meta:resourcekey="CheckBoxIndexResource1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Literal ID="LiteralNoText" runat="server" Text="No attributes" 
                            meta:resourcekey="LiteralNoTextResource1"></asp:Literal>
                    </EmptyDataTemplate>
                </asp:GridView> 
            </div>
        </div>    
</asp:Content>