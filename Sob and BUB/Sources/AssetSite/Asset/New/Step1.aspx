<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.master" 
AutoEventWireup="true" Inherits="AssetSite.Asset.New.Step1" Codebehind="Step1.aspx.cs" meta:resourcekey="PageResource1"  %>
<%@ OutputCache Duration="60" VaryByParam="*" VaryByHeader="Expires" %>

<asp:Content ID="leftMenu" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="Server">    
    <a href="../../AssetView.aspx">Home</a>&nbsp;&gt;&nbsp;<asp:Label ForeColor="#5eba2f" runat="server" ID="lblTitle2" meta:resourcekey="metaBreadcrumb" /> 
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMainContent" Runat="Server">
    <style>
        @media screen
        {
            div#outer-column-container
            {
                border-left: 0px solid #ffffff !important;
                margin-right: 20px;
            }
        }
    </style>
   <div id="main-container">
<%--        <div class="wizard-header">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">New Asset</asp:Label>            
        </div>--%>

        <div class="panel" style="background-color:#f2f2f2 !important">
            <div class="panelSimpleHeader" style="font-size:larger;">
                <asp:Label runat="server" ID="Label1">Most recent ...</asp:Label>                
            </div>
            <div class="panelcontent">
                    <asp:GridView 
                        ID="gvRecent"
                        runat="server" CssClass="w100p" 
                        BackColor="#898989"
                        ForeColor="White"
                        AutoGenerateColumns="False"                                              
                        OnRowDataBound="gvRecent_RowDataBound"
                        EnableTheming="false"                       
                        GridLines="Horizontal" meta:resourcekey="assetTypesGridResource1">        
                        <RowStyle CssClass="cRowResent"/>        
                        <EmptyDataTemplate>
                            <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No asset types</asp:Label>                            
                        </EmptyDataTemplate>                            
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Name" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource1" ItemStyle-Width="100" />
                            <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource2" />                             
                            <asp:BoundField HeaderText="Categories" DataField="Categories" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource3" ItemStyle-Width="200" />
                            <asp:BoundField 
                                HeaderText="DateRevision" 
                                DataField="DateRevision" 
                                HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource4" ItemStyle-Width="100" />
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1" ItemStyle-Width="20">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" 
                                        ImageUrl="~/images/buttons/plus.png"
                                        NavigateUrl='<%# String.Format("~/Asset/New/Step2.aspx?atid={0}", Eval("ID").ToString()) %>'
                                        runat="server" meta:resourcekey="HyperLinkResource1"></asp:HyperLink>
                                </ItemTemplate>                            
                            </asp:TemplateField>                                
                        </Columns>                                                       
                    </asp:GridView>
            </div>
         </div>

            
         <div class="panel" style="background-color:White !important">
            <div class="panelSimpleHeader" style="font-size:larger;">
                <asp:Label runat="server" ID="panelTitle" 
                    meta:resourcekey="panelTitleResource1">Complete list ...</asp:Label>                
            </div>
            <div class="panelcontent">
            <%--DataSourceID="assetTypesDataSource" --%>
                    <asp:GridView 
                        ID="assetTypesGrid"
                        runat="server" CssClass="w100p" 
                        BackColor="#5eba2f"
                        ForeColor="White"
                        AutoGenerateColumns="False"                                              
                        OnRowDataBound="assetTypesGrid_RowDataBound"
                        AllowPaging="True" EnableTheming="false"                       
                        GridLines="Horizontal" meta:resourcekey="assetTypesGridResource1" 
                        OnPageIndexChanged="assetTypesGrid_PageIndexChanged" 
                        PageSize="15" 
                        OnPageIndexChanging="assetTypesGrid_PageIndexChanging">        
                        <RowStyle CssClass="cRowAll"/>        
                        <EmptyDataTemplate>
                            <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No asset types</asp:Label>                            
                        </EmptyDataTemplate>                            
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Name" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource1" ItemStyle-Width="100"  />
                            <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource2" />                             
                            <asp:BoundField HeaderText="Categories" DataField="Categories" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource3" ItemStyle-Width="200" />
                            <asp:BoundField HeaderText="DateRevision" DataField="DateRevision" HeaderStyle-HorizontalAlign="Left" 
                                meta:resourcekey="BoundFieldResource4" ItemStyle-Width="100" />
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1" ItemStyle-Width="20">
                                <ItemTemplate>
                                    <asp:HyperLink 
                                        ImageUrl="~/images/buttons/plus.png"
                                        NavigateUrl='<%# String.Format("~/Asset/New/Step2.aspx?atid={0}", Eval("ID").ToString()) %>'
                                        runat="server" meta:resourcekey="HyperLinkResource1"></asp:HyperLink>
                                </ItemTemplate>                            
                            </asp:TemplateField>                                
                        </Columns>                                                       
                    </asp:GridView>
            </div>
         </div>

         <%--<div class="wizard-footer-buttons">           
            <asp:Button ID="btnClose" runat="server" onclick="btnClose_Click" Text="<%$ Resources:Global, CancelText %>" />
         </div>   --%>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            $('.cRowAll').hover(
                function() {
                    $(this).css('background-color', '#ebebeb');
                },
                function() {
                    $(this).css('background-color', '#FFFFFF');
                }
            );

                $('.cRowResent').hover(
                function () {
                    $(this).css('background-color', '#FFFFFF');
                },
                function () {
                    $(this).css('background-color', '#f2f2f2');
                }
            );
        });    
    </script>
</asp:Content>