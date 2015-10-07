<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="AssetSite.admin.Batch.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="/css/jquery.ui.timepicker.css?v=0.3.1" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderSearchBox" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Breadcrumb" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.timepicker').timepicker();
            $('.datepicker').datepicker();
            $('.datepicker').datepicker("option", "dateFormat", "<%= DatePattern %>");
            $('.datepicker').datepicker("setDate", '<%# BatchJob.BatchSchedule.ExecuteAt %>');
        });
    </script>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="Label2" 
                meta:resourcekey="panelTitleScheduling"><%= BatchJob.Title %> &mdash; Scheduling</asp:Label>            
        </div>
        <div class="panelcontent">
            <table width="100%">
                <tr>
                    <td>
                        Enabled: <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# BatchJob.BatchSchedule.IsEnabled %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        Execute at: 
                        <asp:TextBox runat="server" ID="txtDatePicker" CssClass="datepicker" Width="100px" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                            runat="server"
                            ControlToValidate="txtDatePicker"
                            ForeColor="Red"
                            ValidationGroup="dialog"
                            ErrorMessage="*" />
                        <asp:TextBox runat="server" ID="txtTimepicker" CssClass="timepicker" Width="100px" Text='<%# BatchJob.BatchSchedule.ExecuteAt.ToShortTimeString() %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                            runat="server"
                            ControlToValidate="txtTimepicker"
                            ValidationGroup="dialog"
                            ForeColor="Red"
                            ErrorMessage="*" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Repeat interval: 
                            
                            <asp:DropDownList runat="server" ID="dlRepeat">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="0.017">1 minute</asp:ListItem>
                                <asp:ListItem Value="0.084">5 minutes</asp:ListItem>
                                <asp:ListItem Value="0.17">10 minutes</asp:ListItem>
                                <asp:ListItem Value="1">1 hour</asp:ListItem>
                                <asp:ListItem Value="2">2 hour</asp:ListItem>
                                <asp:ListItem Value="6">6 hours</asp:ListItem>
                                <asp:ListItem Value="12">12 hours</asp:ListItem>
                                <asp:ListItem Value="24">1 day</asp:ListItem>
                                <asp:ListItem Value="48">2 days</asp:ListItem>
                                <asp:ListItem Value="72">3 days</asp:ListItem>
                                <asp:ListItem Value="168">1 week</asp:ListItem>
                                <asp:ListItem Value="336">2 weeks</asp:ListItem>
                                <asp:ListItem Value="504">3 weeks</asp:ListItem>
                                <asp:ListItem Value="672">4 weeks</asp:ListItem>
                            </asp:DropDownList>
                          
                    </td>
                </tr>
                <tr>
                    <td>
                        Notes: 
                        <asp:TextBox runat="server" 
                            TextMode="MultiLine"
                            Rows="3" Columns="20"
                            ID="txtNotes"
                            Text='<%# BatchJob.BatchSchedule.Notes %>' >
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                           <asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="Save" />
                           <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderRightColumn" runat="server">
</asp:Content>
