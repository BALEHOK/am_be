<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Translations.ascx.cs" Inherits="AssetSite.Controls.Translations" %>
<div id="translationDialog">
    <asp:PlaceHolder runat="server" ID="phTranslations"></asp:PlaceHolder>
</div>        
<script type="text/javascript">

    $(document).ready(function() {
        $('#translationDialog').dialog({
            autoOpen: false,
            buttons: { "Ok": function () { setTranslations(); $(this).dialog("close"); } },
            width: 450
        });
    });

    function showTranslationsFor(key) {
        $('#translationDialog .key').val($(key).val());
        getTranslations();
        $('#translationDialog').dialog('open');
    }

    function showTranslations() {
        showTranslationsFor('<%= this.ControlSelector %>');
    }

    function getTranslations() {
        var key = $('#translationDialog .key').val();
        var counter = 0;
        $('#translationDialog tr').each(function (index, value) {
            var cultureCode = $(this).find("input:hidden").not(":text").val();
            if (cultureCode != '' && index > 0 && key != '') {
                var ctrl = $(this).find("input:text");
                var ctrlName = ctrl.attr('name');
                ctrl.attr('disabled', 'disabled');
                counter++;
                AssetSite.amDataService.GetTranslation(key, cultureCode, ctrlName, function (result) {
                    var ctrl = $('#translationDialog').find('input[name=' + result.ControlId + ']');
                    ctrl.val(result.Translation).removeAttr('disabled');
                    counter--;
                    if (counter == 0) {
                        RemoveWaitImage('translationDialog');
                    }
                });
            }
        });
        if (counter > 0) {
            SetWaitImageWithOffset(225, 50, 'translationDialog');
        }
    }

    function setTranslations() {      
        var key = $('#translationDialog .key').val();
        $('#translationDialog tr').each(function (index, value) {
            var cultureField = $(':hidden', this);
            if (cultureField.size() && index > 0 && key != '') {
                var cultureCode = cultureField.val();
                var translation = $(':text', this).val();
                if (translation != '') {
                    AssetSite.amDataService.AddTranslation(key, cultureCode, translation, OnTranslationSearchSuccessful, OnError);
                }
            }
        });
    }

    function OnTranslationSearchSuccessful(result) {
    }

    function OnError(result) {
        alert('Unable to save the value, please try again or contact the site administrator');
    }
</script>