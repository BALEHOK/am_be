<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/MasterPages/MasterPageBase.master"
CodeBehind="Search.aspx.cs" Inherits="AssetSite.Search.Search" meta:resourcekey="PageResource1" Trace="false" %>

<%@ Register Src="~/Controls/SearchControls/SearchTabs.ascx" TagPrefix="uc1" TagName="SearchTabs" %>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
    <link href="../css/Search.css" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbPlaceholder" runat="server">
    <asp:Literal ID="LiteralSearch" runat="server" Text="Search" 
        meta:resourcekey="LiteralSearchResource1"></asp:Literal> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
    <div class="reglament">
        <uc1:searchtabs runat="server" id="SearchTabs" ActiveSearchType="SearchByKeywords" />

        <div style="float:left;" class="greenBottom"></div>
            <table width="100%" class="searcher">
                <tr>
                    <td align="center" width="90%">
                        <asp:TextBox ID="tbSearch" runat="server" Width="100%" CssClass="textbox-search" autofocus="focus" meta:resourcekey="tbSearchResource1" />     
                        <input type="text" name="hidden" style="visibility:hidden;display:none;" />                     
                    </td>                     
                    <td>
                        <asp:RequiredFieldValidator runat="server" 
                            ControlToValidate="tbSearch" Text="*" />
                        <asp:Button ID="ButtonSearch" runat="server" Text="Search" 
                            onclick="Button1_Click" meta:resourcekey="ButtonSearchResource1" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="period">                        
                        <asp:RadioButtonList runat="server" ID="rbActive" RepeatLayout="Flow" RepeatDirection="Horizontal">                            
                            <asp:ListItem Text="Active" meta:resourcekey="cbActiveResource1" Selected="True" Value="1" />
                            <asp:ListItem Text="History" meta:resourcekey="cbHistoryResource1" Value="0" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
     </div>       
</asp:Content>

<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderMainContent" runat="Server">
<div class="clear-columns">
<!-- do not delete --></div>
</asp:Content>