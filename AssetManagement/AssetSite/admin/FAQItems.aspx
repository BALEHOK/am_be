<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" 
CodeBehind="FAQItems.aspx.cs" Inherits="AssetSite.admin.FAQItems" EnableEventValidation="false" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="../javascript/faq.js"></script>
    <script type="text/javascript" src="../client/ckeditor/ckeditor.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
<asp:ScriptManager ID="ServiceMgr" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        <asp:Literal ID="litCultureName" runat="server" 
            meta:resourcekey="litCultureNameResource1"></asp:Literal>
        <div id="DialogContainer" runat="server" meta:resourcekey="DialogContainerRcrs" style="display:none;">
            <script type="text/javascript">
                var DynEntityId = 0;
            </script>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1"></asp:Label>&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="tbQuestion" runat="server" TextMode="MultiLine" Rows="6" CssClass="richtext ckeditor"
                            Width="350px" meta:resourcekey="tbQuestionResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbAnswer" runat="server" TextMode="MultiLine" Rows="10" CssClass="richtext ckeditor" 
                            Width="350px" meta:resourcekey="tbAnswerResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;" colspan="2">
                        <a runat="server" id="lbtnOK" style="text-decoration:none; cursor:pointer; visibility:visible">Ok</a>
                    </td>
                </tr>
            </table>
        </div>

        <div class="panels_leftcol_container">
             <div class="panel" id="printAreaDiv">
                <div class="panelheader">
                    <asp:Label ID="lblIsActive" runat="server" Text="F.A.Q. items" 
                        meta:resourcekey="lblIsActiveResource1"></asp:Label><br />
                </div>
                <div class="panelcontent">
                    <asp:LinkButton ID="lbtnRebind" runat="server" Text="rebind" Visible="False" 
                        onclick="lbtnRebind_Click" meta:resourcekey="lbtnRebindResource1"></asp:LinkButton>
                    <br />
                    <asp:GridView ID="gvFaqItems" runat="server" CssClass="w100p" 
                        AutoGenerateColumns="False" meta:resourcekey="gvFaqItemsResource1" >
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="45%" 
                                meta:resourcekey="TemplateFieldResource1">
                                <ItemTemplate>
                                    <asp:Label runat="server"><%# Eval("[\"Question\"].Value") %></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="45%"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="45%" 
                                meta:resourcekey="TemplateFieldResource2">
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server"><%# Eval("[\"Answer\"].Value") %></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="45%"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="10%" 
                                meta:resourcekey="TemplateFieldResource3">
                                <ItemTemplate>
                                    <a runat="server" style="cursor:pointer;" id="lbtnEdit" onclick='<%#GetEditScript(Eval("ID")) %>'>
                                        <img src="../images/buttons/edit.png" />
                                    </a>&nbsp;
                                    <asp:LinkButton ID="lbDelete" runat="server" 
                                        CommandArgument='<%# Eval("ID") %>' oncommand="lbDelete_Command" 
                                        meta:resourcekey="lbDeleteResource1">
                                        <img src="../images/buttons/delete.png" />
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%"></ItemStyle>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1">Actions</asp:Label>            
        </div>
        <span style="font-size:smaller;">
            <a id="lbtnShowDlg" runat="server" style="cursor:pointer;">Add F.A.Q. Item</a><br />
        </span>

        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Languages</asp:Label>            
        </div>
        <span style="font-size:smaller;">
            <asp:Repeater ID="repLangs" runat="server" DataSourceID="LanguagesDataSource">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnLanguege" runat="server" 
                        CssClass='<%# GetCssClass(Eval("CultureName")) %>' OnCommand="OnChangeCulture" 
                        CommandArgument='<%# Eval("CultureName") %>'><%#Eval("LongName") %></asp:LinkButton><br />
                </ItemTemplate>
            </asp:Repeater>
             <asp:EntityDataSource runat="server" ID="LanguagesDataSource"
                ConnectionString="name=DataEntities"
                DefaultContainerName="DataEntities"
                EntitySetName="Languages">
            </asp:EntityDataSource>
        </span>
    </div>
</asp:Content>

