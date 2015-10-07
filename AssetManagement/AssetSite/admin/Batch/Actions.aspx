<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Actions.aspx.cs" Inherits="AssetSite.admin.Batch.Actions"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/DeleteConfirmationDialog.ascx" TagName="DeleteConfirmationDialog" TagPrefix="amc" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <div class="wizard-header">
        <asp:Label runat="server" ID="pateTitle" meta:resourcekey="pateTitleResource1">Batch job view</asp:Label>        
    </div>
    <div class="panel">
        <div class="panelheader">        
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1"><%= BatchJob.Title %></asp:Label>            
        </div>
        <div class="panelcontent">
            <div class="padded">
                <div>
                    <asp:Label ID="Label1" runat="server" meta:resourcekey="LabelResource1">Status</asp:Label>:                    
                    <asp:Literal runat="server" ID="JobStatus" meta:resourcekey="JobStatusResource1" />
                </div>
                <div>
                    <asp:Button ID="btnExecute" runat="server" OnClick="ExecuteNow" Text="ExecuteNow" 
                        Visible="False" meta:resourcekey="Button1Resource1" />
                    <asp:Button 
                        runat="server" 
                        Text="Schedule For Execution" 
                        Visible="false" 
                        ID="btnSchedule" 
                        OnClick="btnSchedule_Click" />
                </div>
                <div>
                    <asp:HyperLink runat="server" ID="Back" Text="Back to jobs" 
                        NavigateUrl="~/admin/Batch" meta:resourcekey="BackResource1" />
                    &nbsp;<asp:HyperLink ID="linkRefresh" runat="server" Text="Refresh" meta:resourcekey="JobStatusRefresh" Visible="false" />
                </div>
            </div>
        </div>
    </div>

    <% if (BatchJob.BatchSchedule != null) { %>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="Label2" 
                meta:resourcekey="panelTitleScheduling">Scheduling</asp:Label>            
        </div>
        <div class="panelcontent">
            <table width="100%">
                <tr>
                    <td>
                        Enabled: <%= BatchJob.BatchSchedule.IsEnabled %>
                    </td>
                    <td>
                        Execute at: <%= BatchJob.BatchSchedule.ExecuteAt %>
                    </td>
                    <%if (BatchJob.BatchSchedule.RepeatInHours.HasValue && BatchJob.BatchSchedule.RepeatInHours.Value > 0)
                      { %>
                    <td>
                        Repeat interval: <%= HumanizeRepeatHours(BatchJob.BatchSchedule.RepeatInHours.Value)%>
                    </td>
                    <% } %>
                    <td>
                        Last Start: 
                        <% if (BatchJob.BatchSchedule.LastStart != null)
                           {  %>
                        <%= BatchJob.BatchSchedule.LastStart %>
                        <% }
                        else
                        {  %>
                            &mdash;
                        <%} %>
                    </td>
                    <% if (!string.IsNullOrEmpty(BatchJob.BatchSchedule.Notes)) { %>
                    <td>
                        Notes: <%= BatchJob.BatchSchedule.Notes %>
                    </td>
                    <% } %>
                    <% if (BatchJob.CurrentStatus != AppFramework.ConstantsEnumerators.BatchStatus.Running) { %>
                    <td>
                        <asp:ImageButton ID="ButtonEdit" 
                            runat="server" 
                            ImageUrl="~/images/buttons/edit.png" 
                            AlternateText="Edit" 
                            ToolTip="Edit"
                            OnClick="ButtonEdit_Click" />
                        <asp:ImageButton ID="btnUnSchedule" 
                            runat="server" 
                            ImageUrl="~/images/buttons/delete.png" 
                            AlternateText="Delete" 
                            ToolTip="Delete" 
                            OnClick="btnUnSchedule_Click" />
                    </td>
                    <% } %>
                </tr>
            </table>
        </div>
    </div>
    <% } %>

    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle2" 
                meta:resourcekey="panelTitle2Resource1">Actions</asp:Label>            
        </div>
        <div class="panelcontent">
            <asp:GridView ID="ActionsGrid" runat="server" AutoGenerateColumns="False" 
                OnRowDataBound="ActionsGrid_RowDataBound"
                OnRowDeleting="ActionsGrid_RowDeleting"
                DataKeyNames="BatchActionUid"
                meta:resourcekey="ActionsGridResource1">        
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1">No batch actions</asp:Label>                    
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField HeaderText="#" DataField="Order" 
                        meta:resourcekey="BoundFieldResource1" />
                    <asp:TemplateField HeaderText="ActionType" 
                        meta:resourcekey="TemplateFieldResource1">
                        <ItemTemplate>
                            <%# System.Enum.GetName(typeof(AppFramework.ConstantsEnumerators.BatchActionType), Eval("ActionType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status" 
                        meta:resourcekey="TemplateFieldResource2">
                        <ItemTemplate>
                            <%# System.Enum.GetName(typeof(AppFramework.ConstantsEnumerators.BatchStatus), Eval("Status")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Error message" 
                        meta:resourcekey="BoundFieldResource2">
                        <ItemTemplate>
                            <pre>
                                <%# Eval("ErrorMessage") %>
                            </pre>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <amc:DeleteConfirmationDialog runat="server" ID="DeleteConfirmationDialog" />
</asp:Content>
