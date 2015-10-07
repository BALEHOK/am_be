<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="AssetSite.Controls.Calendar" %>
<asp:TextBox runat="server" ID="txtDateTime" Visible="false"></asp:TextBox>
<asp:Label runat="server" ID="lblDateTime" Visible="true"></asp:Label>
<script type="text/javascript">
    $(document).ready(function () {        
        var picker = $('#<%= txtDateTime.ClientID %>');
        var initialValue = picker.val();
        picker.datepicker({
            showOn: 'button',
            buttonImage: '/images/buttons/calendar.png',
            buttonImageOnly: true,
            changeYear: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            yearRange: '1950:2020',
            beforeShow: function (input, inst) {
                inst.dpDiv.css({ marginTop: -input.offsetHeight + 'px', marginLeft: input.offsetWidth + 'px' });
            }
        });
        picker.datepicker('option', $.datepicker.regional['<%= Locale %>']);
        picker.datepicker("option", "dateFormat", "<%= DatePattern %>");
        picker.datepicker('setDate', initialValue);
        picker.val(initialValue);
        <%= ScriptExtension %>
    });
</script>
