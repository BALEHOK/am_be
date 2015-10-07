<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BatchScheduleDialog.ascx.cs" Inherits="AssetSite.Controls.BatchScheduleDialog" %>
<div id="DialogContainer" style="display:none;" title="Schedule a job">
    <link rel="stylesheet" href="/css/jquery.ui.timepicker.css?v=0.3.1" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $('.timepicker').timepicker();
            $('.datepicker').datepicker();
            $('.datepicker').datepicker("option", "dateFormat", "<%= DatePattern %>");
            $('.datepicker').datepicker("setDate", new Date());
            var dialogId = 'DialogContainer';
            $('#' + dialogId).dialog({
                autoOpen: false,
                width: 440,
                height: 350,
                resizable: false,
                buttons: {
                    'Cancel': function () { $(this).dialog("close"); },
                    'Ok': function () {
                        if (Page_ClientValidate('dialog')) {
                            SetWaitImage(dialogId);
                            AssetSite.amDataService.SaveBatchTask(
                                $('#<%= dlBatchAction.ClientID %>').val(),
                                $('#<%= txtDatePicker.ClientID %>').val(),
                                $('#<%= txtTimepicker.ClientID %>').val(),
                                $('#chkRepeat').is(':checked') ? $('#dlRepeat').val() : 0,
                                $('#txtNotes').val(),
                                dialogId,
                                function (result) {
                                    RemoveWaitImage(dialogId);
                                    $('#' + dialogId).dialog('close');
                                    location.href = result.RedirectUrl;
                                });
                        }
                    }
                }
            });

            $('#chkRepeat').change(function () {
                if ($(this).attr("checked")) {
                    $('#dlRepeat').removeAttr('disabled');
                } else {
                    $('#dlRepeat').attr('disabled', 'disabled');
                }
            });
        });
    </script>
    <table border="0" cellpadding="2" cellspacing="0" width="100%">
        <tr>
            <td>
                Action:
            </td>
            <td>
                <input type="text" style="display:none" />
                <asp:DropDownList runat="server" ID="dlBatchAction" />
                <asp:RequiredFieldValidator 
                    runat="server"
                    ControlToValidate="dlBatchAction"
                    ValidationGroup="dialog"
                    ForeColor="Red"
                    ErrorMessage="*" />
            </td>
        </tr>
        <tr>
            <td><label>Execute at:</label></td>
            <td>
                <asp:TextBox runat="server" ID="txtDatePicker" CssClass="datepicker" Width="100px" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                    runat="server"
                    ControlToValidate="txtDatePicker"
                    ForeColor="Red"
                    ValidationGroup="dialog"
                    ErrorMessage="*" />
                <asp:TextBox runat="server" ID="txtTimepicker" CssClass="timepicker" Width="100px" />
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
                <input type="checkbox" id="chkRepeat"  /><label>Repeat each</label> 
            </td>
            <td>
                <select disabled="disabled" id="dlRepeat">
                    <optgroup label="Minutes">
                        <option Value="0.017">1 minute</option>
                        <option Value="0.084">5 minute</option>
                        <option Value="0.17">10 minute</option>
                    </optgroup>
                    <optgroup label="Hours">
                        <option value="1">1 hour</option>
                        <option value="2">2 hours</option>
                        <option value="6">6 hours</option>
                        <option value="12">12 hours</option>                        
                    </optgroup>
                    <optgroup label="Days">
                        <option value="24">1 day</option>
                        <option value="48">2 days</option>
                        <option value="72">3 days</option>                            
                    </optgroup>
                    <optgroup label="Weeks">
                        <option value="168" selected="selected">1 week</option>
                        <option value="336">2 weeks</option>
                        <option value="504">3 weeks</option>
                        <option value="672">4 weeks</option>
                    </optgroup>
                </select>
            </td>
        </tr>
        <tr>
            <td><label>Additional notes:</label></td>
            <td><textarea id="txtNotes" rows="3" cols="20"></textarea></td>
        </tr>
    </table>
</div>