/// <reference path="utilities.js" />

function OnOpened(atId, atrId, tbId, listId) {
    var list = $('#' + listId);
    if ($(' option', list).size() == 0) {
        var searchPattern = new String($('#' + tbId).val());
        SetWaitImage(listId);
        $(list).attr("data-current-page", 1);
        AssetSite.amDataService.SearchAssetsReturnsIdName(atId, atrId, '', listId, 1, OnSearchSuccessful, OnError);
    }
}

function OnTextChangedReturnsIdName(atId, atrId, tbId, listId) {
    var searchPattern = new String($('#' + tbId).val());
    SetWaitImage(listId);
    $('#' + listId).attr("data-current-page", 1);
    AssetSite.amDataService.SearchAssetsReturnsIdName(atId, atrId, searchPattern, listId, 1, OnSearchSuccessful, OnError);
}

function OnScrollChangedReturnsIdName(list, atId, atrId, tbId, listId) {
    var elem = $(list);
    var loading = $(list).attr('data-loading');
    if ((elem[0].scrollHeight - elem.scrollTop() <= elem.outerHeight()) && (typeof loading === 'undefined' || loading === false)) {
        var searchPattern = new String($('#' + tbId).val());        
        var itemsCount = elem.find("option").size();        
        if ($(list).attr("data-more-rows") == 'true') {
            var nextPageNumber = parseInt($(list).attr("data-current-page")) + 1;
            SetWaitImage(listId);
            $(list).attr("data-loading", 'true');
            AssetSite.amDataService.SearchAssetsReturnsIdName(atId, atrId, searchPattern, listId, nextPageNumber, OnScrollChangedSearchSuccessful, OnError);
        }
    }
}

function OnScrollChangedSearchSuccessful(result) {
    RemoveWaitImage(result.ContainerId);
    var list = document.getElementById(result.ContainerId);
    $(list).removeAttr("data-loading");
    var currentPage = parseInt($(list).attr("data-current-page"));
    if (currentPage < result.CurrentPage) {
        $(list).find("option").last().attr("selected", "selected");
        $(list).attr("data-current-page", result.CurrentPage);
        $(list).attr("data-more-rows", result.HasMoreRows);
        for (i = 0; i < result.Data.length; i++) {
            var option = document.createElement('option');
            option.text = result.Data[i].Value;
            option.value = result.Data[i].Key;
            list.options.add(option);
        }
    }
}

function OnSearchSuccessful(result) {
    RemoveWaitImage(result.ContainerId);
    var list = document.getElementById(result.ContainerId);
    $(list).find("option").remove();
    $(list).scrollTop(0);
    list.options.length = 0;
    $(list).attr("data-more-rows", result.HasMoreRows);
    for (i = 0; i < result.Data.length; i++) {
        var option = document.createElement('option');
        option.text = result.Data[i].Value;
        option.value = result.Data[i].Key;
        list.options.add(option);
    }
    var newWidth = $(list).width() + 40;
    var dialog = $(list).parents().filter('.ui-dialog');
    if (dialog && dialog.width() < newWidth)
        dialog.width(newWidth);
}

function OnSingleDelete(tbId, hdfId) {
    $('#' + tbId).val('');
    $('#' + hdfId).val('0');
}

function OnMultipleDelete(listId, hdfRemoveId) {
    var list = document.getElementById(listId);
    var hdf = document.getElementById(hdfRemoveId);

    for (i = 0; i < list.options.length; i++) {
        if (list.options[i].selected) {
            hdf.value += list.options[i].value + ",";
        }
    }

    $('#' + listId + ' option:selected').remove();
}

function OnMultipleSelect(destListId, srcListId, dialogId, hdfAddId) {
    var sourceList = document.getElementById(srcListId);
    var destList = document.getElementById(destListId);
    var hdf = document.getElementById(hdfAddId);

    for (i = 0; i < sourceList.options.length; i++) {
        if (sourceList.options[i].selected) {

            var tempOption = sourceList.options[i];
            tempOption.selected = false;

            hdf.value += tempOption.value + ",";
            destList.options.add(new Option(tempOption.text, tempOption.value));
        }
    }
    $('#' + dialogId).dialog('close');
}

function OnSelect(listId, hdfId, hdfTextId, sourceTbId, dialogId) {
    var text = $('#' + listId + ' option:selected').text().trim();
    $('#' + sourceTbId).val(text);
    $('#' + hdfTextId).val(text);
    $('#' + hdfId).val($('#' + listId).val());
    $('#' + dialogId).dialog('close');
}

function OnSelectionReady(listId, ddlId) {
    var selectedItem = $('#' + listId).value();
}