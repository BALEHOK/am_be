<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewRights.aspx.cs" Inherits="AssetSite.admin.Users.Edit"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Rights</asp:Label>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Users Permissions</asp:Label>
        </div>
        <div class="panelcontent">
            <div style="margin-bottom: 50px;">
                <asp:ScriptManager runat="server" AsyncPostBackErrorMessage="Error while building list. Please refresh the page and try again"
                    ID="ScriptManager1">
                </asp:ScriptManager>
                <div style="float: left; margin-right: 30px;">
                    <asp:UpdatePanel ID="usersPanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:ListBox ID="usersList" runat="server" AutoPostBack="True" Width="200px" Height="138px"
                                OnSelectedIndexChanged="usersList_SelectedIndexChanged" meta:resourcekey="usersListResource1">
                            </asp:ListBox>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="userName" EventName="TextChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:Label runat="server" CssClass="label" meta:resourcekey="LabelResource1">Quick filter:</asp:Label>
                    <br />
                    <asp:TextBox ID="userName" runat="server" AutoPostBack="True" CssClass="autoTextbox"
                        OnTextChanged="userName_TextChanged" Height="19px" Width="200px" meta:resourcekey="userNameResource1"></asp:TextBox>
                    <script type="text/javascript">
                        $('.autoTextbox').keypress(function () {
                            $('.autoTextbox').trigger('onchange');
                        });
                    </script>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <asp:UpdatePanel ID="rightsPanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="aRulesHeader" runat="server" Visible="False" meta:resourcekey="aRulesHeaderResource1">
                        <h4 class="green">
                            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Allowing rules list for user"></asp:Literal>
                        </h4>
                    </asp:Panel>
                    <asp:GridView ID="allowRightsGrid" AutoGenerateColumns="False" CssClass="rightsgrid"
                        CellSpacing="5" CellPadding="5" GridLines="None" OnRowDeleting="allowRule_Deleting"
                        DataKeyNames="ViewID" runat="server" meta:resourcekey="allowRightsGridResource1">
                        <AlternatingRowStyle CssClass="alterrow" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" meta:resourcekey="TemplateFieldResource1">
                                <ItemTemplate>
                                    <b>
                                        <%# Container.DataItemIndex + 1 %></b>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Categories" HeaderText="Categories" meta:resourcekey="BoundFieldResource1" />
                            <asp:BoundField DataField="AssetTypes" HeaderText="AssetTypes" meta:resourcekey="BoundFieldResource2" />
                            <asp:BoundField DataField="Departments" HeaderText="Departments" meta:resourcekey="BoundFieldResource3" />
                            <asp:TemplateField HeaderText="Permission" meta:resourcekey="TemplateFieldResource2">
                                <HeaderTemplate>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="Permission"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr style="font-size: 10px">
                                            <td>
                                                <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" Text="Normal"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="Literal4" runat="server" meta:resourcekey="Literal4Resource1" Text="Financial"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="Literal6" runat="server" Text="Delete"></asp:Literal>
                                            </td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <tr>
                                            <td class="norm-perm">
                                                <%# (Eval("Permission") as string).Substring(0,2) %>
                                            </td>
                                            <td class="fin-perm">
                                                <%# (Eval("Permission") as string).Substring(2,2) %>
                                            </td>
                                            <td class="del-perm">
                                                <strong><%# Eval("CanDelete") %></strong>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
                                <ItemTemplate>
                                    <asp:HyperLink NavigateUrl='<%# String.Format("~/admin/Users/EditRights.aspx?viewid={0}&userid={1}", Eval("ViewID"), this.usersList.SelectedValue) %>'
                                        Text="Edit rule" runat="server" meta:resourcekey="HyperLinkResource1" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource4">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" ImageUrl="/images/buttons/delete.png" meta:resourcekey="btnDeleteResource1">
                                    </asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label runat="server" ID="lblNoData" Text="<%$ Resources:Global, ListIsEmpty %>">list is empty</asp:Label>
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                    </asp:GridView>
                    <asp:Panel ID="dRulesHeader" runat="server" Visible="False" meta:resourcekey="dRulesHeaderResource1">
                        <h4 class="red">
                            <asp:Literal ID="Literal5" runat="server" meta:resourcekey="Literal5Resource1" Text="Denying rules list for user"></asp:Literal>
                        </h4>
                    </asp:Panel>
                    <asp:GridView ID="denyRightsGrid" AutoGenerateColumns="False" CssClass="rightsgrid"
                        CellSpacing="5" CellPadding="5" GridLines="None" OnRowDeleting="denyRule_Deleting"
                        DataKeyNames="ViewID" runat="server" meta:resourcekey="denyRightsGridResource1">
                        <AlternatingRowStyle CssClass="alterrow-deny" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" meta:resourcekey="TemplateFieldResource5">
                                <ItemTemplate>
                                    <b>
                                        <%# Container.DataItemIndex + 1 %></b>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Categories" HeaderText="Categories" meta:resourcekey="BoundFieldResource4" />
                            <asp:BoundField DataField="AssetTypes" HeaderText="AssetTypes" meta:resourcekey="BoundFieldResource5" />
                            <asp:BoundField DataField="Departments" HeaderText="Departments" meta:resourcekey="BoundFieldResource6" />
                            <asp:TemplateField HeaderText="Permission" meta:resourcekey="TemplateFieldResource6">
                                <HeaderTemplate>
                                    <table>
                                        <asp:Literal runat="server" meta:resourcekey="LiteralResource1"></asp:Literal>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource2" Text="Permission"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr style="font-size: 10px">
                                            <td>
                                                <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource2" Text="Normal"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="Literal4" runat="server" meta:resourcekey="Literal4Resource2" Text="Financial"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="Literal6" runat="server" Text="Delete"></asp:Literal>
                                            </td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <tr>
                                            <td class="norm-perm">
                                                <%# (Eval("Permission") as string).Substring(0,2) %>
                                            </td>
                                            <td class="fin-perm">
                                                <%# (Eval("Permission") as string).Substring(2,2) %>
                                            </td>
                                            <td class="del-perm">
                                                <strong><%# Eval("CanDelete") %></strong>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource7">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" NavigateUrl='<%# String.Format("~/admin/Users/EditRights.aspx?viewid={0}&userid={1}", Eval("ViewID"), this.usersList.SelectedValue) %>'
                                        Text="Edit rule" runat="server" meta:resourcekey="HyperLink1Resource1" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource8">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" ImageUrl="/images/buttons/delete.png" meta:resourcekey="btnDeleteResource2">
                                    </asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Literal runat="server" Text="<%$ Resources:Global, ListIsEmpty %>"></asp:Literal>
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row-deny" />
                    </asp:GridView>
                    <p>
                        <asp:HyperLink ID="addLink" runat="server" Visible="False" meta:resourcekey="addLinkResource1">Add a new rule</asp:HyperLink></p>



                    <asp:Panel ID="pnlAllowTasksRights" runat="server" Visible="False">
                        <h4 class="green">
                            <asp:Literal ID="ltAllowTasksRights" runat="server" meta:resourcekey="ltAllowTasksRights" Text="Tasks allowing rules list for user"></asp:Literal>
                        </h4>
                    </asp:Panel>
                    <asp:GridView ID="gvAllowTasksRights" AutoGenerateColumns="False" CssClass="rightsgrid" CellSpacing="5"
                        CellPadding="5" GridLines="None" OnRowDeleting="gvAllowTasksRights_Deleting" DataKeyNames="ViewID"
                        runat="server" meta:resourcekey="gvAllowTasksRights">
                        <AlternatingRowStyle CssClass="alterrow" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" meta:resourcekey="columNubmerAllowTasksRights">
                                <ItemTemplate>
                                    <b>
                                        <%# Container.DataItemIndex + 1 %></b>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Categories" HeaderText="Categories" meta:resourcekey="bfCategoriesAllowTasksRights" />
                            <asp:BoundField DataField="AssetTypes" HeaderText="AssetTypes" meta:resourcekey="bfAssetTypesAllowTasksRights" />                          
                            <asp:TemplateField meta:resourcekey="columEditAllowTasksRights">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlinkEditTaskRule" NavigateUrl='<%# String.Format("~/admin/Users/EditRights.aspx?type=task&viewid={0}&userid={1}", Eval("ViewID"), this.usersList.SelectedValue) %>'
                                        Text="Edit rule" runat="server" meta:resourcekey="hlinkEditTaskRule" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="columDeleteAllowTasksRights">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnTaskRightDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" ImageUrl="/images/buttons/delete.png" meta:resourcekey="btnTaskRightDelete">
                                    </asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label runat="server" ID="lblNoData" Text="<%$ Resources:Global, ListIsEmpty %>">list is empty</asp:Label>
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                    </asp:GridView>

                      <asp:Panel ID="pnlDenyTasksRights" runat="server" Visible="False">
                        <h4 class="red">
                            <asp:Literal ID="ltDenyTasksRights" runat="server" meta:resourcekey="ltDenyTasksRights" Text="Tasks denying rules list for user"></asp:Literal>
                        </h4>
                    </asp:Panel>
                    <asp:GridView ID="gvDenyTasksRights" AutoGenerateColumns="False" CssClass="rightsgrid" CellSpacing="5"
                        CellPadding="5" GridLines="None" OnRowDeleting="gvDenyTasksRights_Deleting" DataKeyNames="ViewID"
                        runat="server" meta:resourcekey="gvDenyTasksRights">
                        <AlternatingRowStyle CssClass="alterrow" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" meta:resourcekey="columNubmerDenyTasksRights">
                                <ItemTemplate>
                                    <b>
                                        <%# Container.DataItemIndex + 1 %></b>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Categories" HeaderText="Categories" meta:resourcekey="bfCategoriesDenyTasksRights" />
                            <asp:BoundField DataField="AssetTypes" HeaderText="AssetTypes" meta:resourcekey="bfAssetTypesDenyTasksRights" />                          
                            <asp:TemplateField meta:resourcekey="columEditDenyTasksRights">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlinkEditTaskRule" NavigateUrl='<%# String.Format("~/admin/Users/EditRights.aspx?type=task&viewid={0}&userid={1}", Eval("ViewID"), this.usersList.SelectedValue) %>'
                                        Text="Edit rule" runat="server" meta:resourcekey="hlinkEditDenyTaskRule" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField meta:resourcekey="columDeleteDenyTasksRights">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnTaskRightDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" ImageUrl="/images/buttons/delete.png" meta:resourcekey="btnDeleteDenyTaskRightDelete">
                                    </asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label runat="server" ID="lblNoData" Text="<%$ Resources:Global, ListIsEmpty %>">list is empty</asp:Label>
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="row" />
                    </asp:GridView>
                     <p>
                        <asp:HyperLink ID="hlinkAddNewTaskrule" runat="server" Visible="False" meta:resourcekey="hlinkAddNewTaskrule">Add a new task rule</asp:HyperLink></p>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="usersList" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="100" runat="server" AssociatedUpdatePanelID="rightsPanel">
                <ProgressTemplate>
                    <div class="loader">
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdateProgress ID="UpdateProgress2" AssociatedUpdatePanelID="usersPanel" runat="server"
                DisplayAfter="100">
                <ProgressTemplate>
                    <div class="loader">
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>
</asp:Content>
