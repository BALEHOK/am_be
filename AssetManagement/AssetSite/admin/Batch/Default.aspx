<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Batch.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/BatchScheduleDialog.ascx" TagPrefix="amc" TagName="BatchScheduleDialog" %>

<asp:Content runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <asp:ScriptManager ID="MainScriptManager" runat="server" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>

    <amc:BatchScheduleDialog ID="BatchScheduleDialog" runat="server" />

    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Batch jobs</asp:Label>
        </div>
        <div class="panelcontent">
            <table>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" ID="chkHideFinished" Text="Hide finished jobs" OnCheckedChanged="chkHideFinished_Change"
                            AutoPostBack="True" Checked="True" meta:resourcekey="HideFinishedResource1" />
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="btnCleanFinished" OnClientClick="return confirm('Delete all finished jobs?');"
                            OnClick="btnCleanFinished_Click">Clean finished jobs</asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="btnCleanAll" OnClientClick="return confirm('Delete all jobs?');"
                            OnClick="btnCleanAll_Click">Clean all jobs</asp:LinkButton>
                    </td>
                </tr>
            </table>
            <asp:GridView runat="server" ID="JobsGrid" DataSourceID="BatchDataSource" AllowPaging="True"
                AllowSorting="True" PageSize="25" AutoGenerateColumns="False" DataKeyNames="BatchUid"
                OnRowDataBound="JobsGrid_RowDataBound" meta:resourcekey="JobsGridResource1">
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No batch jobs</asp:Label>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField HeaderText="Title" DataField="Title" meta:resourcekey="BoundFieldResource1" 
                        SortExpression="Title" />     
                    <asp:BoundField HeaderText="User" DataField="UserName" />              
                    <asp:BoundField HeaderText="Start date" DataField="StartDate" SortExpression="StartDate"
                        meta:resourcekey="BoundFieldResource3" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                    <asp:BoundField HeaderText="End date" DataField="EndDate" DataFormatString="{0:dd-MM-yyyy HH:mm}"
                        SortExpression="EndDate" meta:resourcekey="BoundFieldResource4" />
                    <asp:TemplateField SortExpression="Status" HeaderText="Status" meta:resourcekey="TemplateFieldResource2" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblStatus" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsEnabled" HeaderText="Scheduled" ItemStyle-HorizontalAlign="Center" />
                    <asp:HyperLinkField DataNavigateUrlFields="BatchUid" DataNavigateUrlFormatString="~/admin/Batch/Actions.aspx?BatchUid={0}"
                        Text="<% $Resources:Global, ViewText %>" />
                    <asp:HyperLinkField DataNavigateUrlFields="BatchUid" DataNavigateUrlFormatString="~/admin/Batch/Actions.aspx?BatchUid={0}&Execute=1"
                        Text="Execute now" meta:resourcekey="HyperLinkFieldResource2" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <asp:SqlDataSource 
        runat="server" 
        ID="BatchDataSource">
    </asp:SqlDataSource>

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div><a href="javascript:void(0);" onclick="$('#DialogContainer').dialog('open');">Create a New Batch Job</a></div>        
    </div>
</asp:Content>
