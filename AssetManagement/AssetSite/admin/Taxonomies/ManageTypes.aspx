<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageTypes.aspx.cs" Inherits="AssetSite.admin.Taxonomies.ManageTypes"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Asset types</asp:Label>        
    </div>
   
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1">Associated asset types</asp:Label>            
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5">
                <tr>
                    <td>
                        <asp:Literal runat="server" meta:resourcekey="LiteralResource1" 
                            Text="Select asset type from list"></asp:Literal>                        
                    </td>
                    <td>
                        <asp:DropDownList 
                            ID="AllAssetTypes" 
                            runat="server" 
                            DataTextField="Name" 
                            DataValueField="ID" meta:resourcekey="AllAssetTypesResource1">
                        </asp:DropDownList>
                    </td>
                    <td colspan="2">
                        <asp:Button runat="server" ID="AddType" Text="<% $Resources:Global, AddText %>" OnClick="OnAddTypeClick" />                            
                    </td>
                </tr>
            </table>
            <asp:ScriptManager runat="server" AsyncPostBackErrorMessage="Error while building list. Refresh page and try again"
                ID="ScriptManager1">
            </asp:ScriptManager>
            <asp:Literal runat="server" meta:resourcekey="LiteralResource2" 
                Text="Quick search"></asp:Literal>            
            <table cellpadding="0" cellspacing="5">
                <tr>
                    <td>
                        <asp:Label runat="server" meta:resourcekey="LabelResource1">Asset type name (first letters)</asp:Label>                        
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="AssetName" 
                            meta:resourcekey="AssetNameResource1"></asp:TextBox>
                        <asp:Button runat="server" OnClick="UpdateFilteredList" ID="FilteredListUpdate" 
                            Text="Search" meta:resourcekey="FilteredListUpdateResource1" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">                        
                        <asp:UpdatePanel ID="QuickSearchPanel" runat="server">
                            <ContentTemplate>
                                <asp:GridView runat="server" ID="AssetFilteredList" AutoGenerateColumns="False" CellPadding="4"
                                    DataKeyNames="UID" ForeColor="#333333" GridLines="None" CssClass="w100p" 
                                    meta:resourcekey="AssetFilteredListResource1">
                                    <Columns>
                                        <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                            <HeaderTemplate>
                                                #</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("UID") %>' />
                                                <asp:CheckBox runat="server" ID="TypeSelected" 
                                                    meta:resourcekey="TypeSelectedResource1" />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Name" HeaderText="Name" 
                                            meta:resourcekey="BoundFieldResource1" >
                                        <ItemStyle Width="90%" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle BackColor="#DFDFDF" Font-Bold="True" ForeColor="#1E1E1E" HorizontalAlign="Left" />
                                    <AlternatingRowStyle BackColor="#E0FFC1" />
                                    <EmptyDataTemplate>
                                        <asp:Literal runat="server" ID="lblNoData" 
                                            meta:resourcekey="lblNoDataResource1" Text="Not found"></asp:Literal>                                        
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <asp:Button ID="AddFiltered" runat="server" Text="<% $Resources:Global, AddText %>" OnClick="OnAddFilteredClick"  />                            
                    </td>
                </tr>
            </table>
            <asp:GridView ID="AssetsTypes" runat="server" AutoGenerateColumns="False" CellPadding="4"
                DataKeyNames="UID" ForeColor="#333333" GridLines="None" CssClass="w100p" 
                meta:resourcekey="AssetsTypesResource1">
                <RowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" 
                        HeaderStyle-HorizontalAlign="Center" meta:resourcekey="TemplateFieldResource2">
                        <ItemTemplate>
                            <asp:CheckBox ID="AssetToDelete" runat="server" 
                                meta:resourcekey="AssetToDeleteResource1" />
                            <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("UID") %>' />
                        </ItemTemplate>

<HeaderStyle HorizontalAlign="Center"></HeaderStyle>

<ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" 
                        meta:resourcekey="BoundFieldResource2" />
                    <asp:BoundField DataField="Revision" HeaderText="Revision" 
                        meta:resourcekey="BoundFieldResource3" />
                </Columns>
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#DFDFDF" Font-Bold="True" ForeColor="#1E1E1E" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="#E0FFC1" />
                <EmptyDataTemplate>
                    <asp:Literal runat="server" ID="lblNoData2" 
                        meta:resourcekey="lblNoData2Resource1" Text="No assets"></asp:Literal>        
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:Button runat="server" OnClick="RemoveSelected" Text="Remove selected" 
                meta:resourcekey="ButtonResource1" />
        </div>
    </div>

    <script type="text/javascript" language="javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        var thebutton;
        var addbutton;

        function BeginRequestHandler(sender, args) {
            thebutton = args.get_postBackElement();
            addbutton = $get('<%= AddFiltered.ClientID %>');
            addbutton.disabled = true;
            thebutton.disabled = true;
        }

        function EndRequestHandler(sender, args) {            
            thebutton.disabled = false;
            addbutton.disabled = false;
        }
    </script>
    <p>
        <asp:Literal ID="subnote" runat="server" meta:resourcekey="subnoteResource">This changes will affects immediately.</asp:Literal>
    </p>
    <div class="wizard-footer-buttons">
        <asp:Button ID="Finished" runat="server" Text="<% $Resources:Global, CompleteText %>" OnClick="Finished_Click" />            
    </div>
</asp:Content>
