<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Step3.aspx.cs" Inherits="AssetSite.Wizard.Step3"
    MasterPageFile="~/MasterPages/MasterPageWizard.master" meta:resourcekey="PageResource1" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Asset Wizard" meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp;
        <asp:Label runat="server" ID="lblStepName" Text="Attributen" meta:resourcekey="lblStepNameResource1"></asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" ID="stepDesc" Text="Mauris congue consectetuer quam."
            meta:resourcekey="stepDescResource1"></asp:Literal>
    </p>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblpanelHeader" Text="Specifieke kenmerken attribuut"
                meta:resourcekey="lblpanelHeaderResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:GridView ID="GridAttributes" runat="server" AutoGenerateColumns="False" DataKeyNames="UID"
                OnRowCancelingEdit="GridAttributes_RowCancelingEdit" OnRowDeleting="GridAttributes_RowDeleting"
                OnRowEditing="GridAttributes_RowEditing" OnRowUpdated="GridAttributes_RowUpdated"
                OnRowUpdating="GridAttributes_RowUpdating" meta:resourcekey="GridAttributesResource1"
                OnRowCommand="GridAttributes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NameLocalized" HeaderText="<% $Resources:Global, NameText %>"
                        HeaderStyle-Width="20%">
                        <HeaderStyle Width="20%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="DataTypeEnum" HeaderText="DataType" HeaderStyle-Width="10%">
                        <HeaderStyle Width="10%"></HeaderStyle>
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ToolTip='<%# Eval("Comment") %>' ID="lblComment" runat="server" Text='<%# ChopString((String) Eval("Comment")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" HeaderStyle-Width="10%"
                        meta:resourcekey="CheckBoxFieldResource1">
                        <HeaderStyle Width="10%"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource2" HeaderStyle-Width="20px">
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="linkEdit" Text="Edit" ImageUrl="/images/buttons/edit.png"
                                NavigateUrl='<%# String.Format("~/Wizard/EditAttribute.aspx?AttrUID={0}", Eval("UID").ToString()) %>'
                                meta:resourcekey="linkEditResource1" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource3" HeaderStyle-Width="20px">
                        <ItemTemplate>
                            <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" ImageUrl="/images/buttons/delete.png" OnClientClick="return confirm('Are you sure want to delete attribute?')"
                                meta:resourcekey="btnDeleteResource1"></asp:ImageButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton runat="server" CommandArgument='<%# Eval("UID") %>' ImageUrl="~/images/arrowdown.png" CommandName="down"
                                ID="imgbtnArrowDown" />
                            <asp:ImageButton runat="server" CommandArgument='<%# Eval("UID") %>' ImageUrl="~/images/arrowup.png" CommandName="up"
                                ID="imgbtnArrowUp" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Literal runat="server" ID="lblNoData" Text="No attributes" meta:resourcekey="lblNoDataResource1"></asp:Literal>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:HyperLink runat="server" NavigateUrl="~/Wizard/EditAttribute.aspx" Text="New attribute"
                meta:resourcekey="HyperLinkResource1"></asp:HyperLink>
        </div>
    </div>
</asp:Content>
