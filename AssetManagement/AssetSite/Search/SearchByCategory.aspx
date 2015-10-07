<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/MasterPages/MasterPageBase.master" CodeBehind="SearchByCategory.aspx.cs"
Inherits="AssetSite.Search.SearchByCategory" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/SearchControls/SearchTabs.ascx" TagPrefix="uc1" TagName="SearchTabs" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
    <link href="../css/Search.css" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:Literal ID="LiteralSearch" runat="server" Text="Search" 
        meta:resourcekey="LiteralSearchResource1"></asp:Literal> 
    <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <div class="reglament">

            <uc1:searchtabs runat="server" id="SearchTabs" ActiveSearchType="SearchByCategory" />
            <div style="float:left;" class="greenBottom"></div>
            
            <br />
            <br />
            <div style="float:none;">
                
                <table width="100%" class="categorySearcher">
                    <tr>
                        <td align="right" style="padding-right:20px;">
                            <asp:HiddenField ID="hfUid" runat="server"/>
                            <asp:Label ID="lTaxonomy" runat="server" Text="Taxonomy" 
                                meta:resourcekey="lTaxonomyResource1"></asp:Label> 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTaxonomy" runat="server" 
                                AutoPostBack="True" 
                                onselectedindexchanged="ddlTaxonomy_SelectedIndexChanged" 
                                meta:resourcekey="ddlTaxonomyResource1">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:RadioButtonList runat="server" ID="rbActive" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Text="Active" meta:resourcekey="cbActiveResource1" Selected="True" Value="1" />
                                <asp:ListItem Text="cbHistory" meta:resourcekey="cbHistoryResource1" Value="0" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
                <div class="treeSearch">     
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeStyle-ForeColor="Black"
                                meta:resourcekey="TreeView1Resource1" />
                            <asp:Label runat="server" ID="EmptyTree" Visible="false"></asp:Label>
                       </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTaxonomy" 
                                EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>           
                </div>
                
                <div>
                    <asp:Label ID="Label1" runat="server" meta:resourcekey="lblKeywords"></asp:Label>
                    <asp:TextBox ID="tbSearch" runat="server" Width="750px"
                                meta:resourcekey="tbSearchResource1"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                        runat="server" 
                        ControlToValidate="tbSearch" 
                        ErrorMessage="*"
                        ForeColor="Red" />
                    <asp:Button ID="Button1" runat="server" Text="Search" 
                        onclick="Button1_Click" meta:resourcekey="Button1Resource1" />
                </div>
            </div>
     </div>            
</asp:Content>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderMainContent" runat="Server">
    <div class="clear-columns">
<!-- do not delete --></div>
</asp:Content>