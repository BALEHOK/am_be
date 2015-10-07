<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    CodeBehind="TaskView.aspx.cs" Inherits="AssetSite.TaskView" %>

<%@ Register Src="~/Controls/AssetTreeView.ascx" TagName="AssetTreeView" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="dialog hidden tasks" id="dialog_task" title="Message">
        <p>
            The task is being executed in the background.</p>
    </div>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#dialog_task').dialog({
                resizable: false,
                draggable: false,
                width: 550,
                height: 350,
                modal: true,
                autoOpen: false,
                buttons: { "Ok": function () { $(this).dialog("close"); } }
            });
        });
        // Processes the button click and calls
        // the service Greetings method.
        function ExecAsync(taskId) {
            $('#dialog_task').dialog('open');
            AssetSite.amDataService.ExecuteTask(taskId, null, function (result) {
                if (result.NavigationResult) {
                    location.href = result.NavigationResult;
                } else {
                    dialog.empty();
                    if ($(result.Messages).size() > 0) {
                        $(result.Messages).each(function (index, item) {
                            dialog.append('<p>' + item + '</p>');
                        });
                    } else {
                        dialog.append('<p class="status ok">Status: Sussess</p>');
                    }
                }
            });
        }
    </script>
    <asp:ScriptManager runat="server" ID="scriptManager" EnablePageMethods="true" EnablePartialRendering="true">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div class="panel">
        <asp:Label ID="lblATname" runat="server" CssClass="wizard-header"></asp:Label>
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleText"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:Repeater ID="repTasks" runat="server">
                <HeaderTemplate>
                    <%--#E0FFC1;--%>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color: #E0FFC1;">
                        <td>
                            <div style="margin: 5px;">
                                <a href="javascript:void(0)" onclick='<%# "ExecAsync(\"" + Eval("TaskId") + "\")"%>'>
                                    <%#Eval("Name") %>
                                </a>
                                <br />
                                <asp:Label runat="server" Text='<%#Eval("Description") %>'></asp:Label>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="background-color: White;">
                        <td>
                            <div style="margin: 5px;">
                                <a href="javascript:void(0)" onclick='<%# "ExecAsync(\"" + Eval("TaskId") + "\")"%>'>
                                    <%#Eval("Name") %>
                                </a>
                                <br />
                                <asp:Label runat="server" Text='<%#Eval("Description") %>'></asp:Label>
                            </div>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label runat="server" ID="lbltaskListEmpty" Text="<% $Resources:Global, lblNoTasks %>"
                Visible="false" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <amc:AssetTreeView ID="assetTreeView" runat="server" IsTask="true" />
    </div>
</asp:Content>
