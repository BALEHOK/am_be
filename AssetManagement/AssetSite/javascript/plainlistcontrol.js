function OnSelect(listId, hdfId, hdfTextId, sourceTbId, dialogId) {
    var text = $('#' + listId + ' option:selected').text().trim();
    $('#' + sourceTbId).val(text);
    $('#' + hdfTextId).val(text);
    $('#' + hdfId).val($('#' + listId).val().trim());
    $('#' + dialogId).dialog('close');
}