/// <reference path="utilities.js" />

function OnPOZSelect(listId, hdfId, hdfTextId, sourceTbId, dialogId) {
    var text = $('#' + listId + ' option:selected').text().trim();
    $('#' + sourceTbId).val(text);
    $('#' + hdfTextId).val(text);
    $('#' + hdfId).val($('#' + listId).val().trim());
    $('#' + dialogId).dialog('close');
}

function OnSearchPOZ(tbId, listId) {
    var searchPattern = new String($('#' + tbId).val());
    SetWaitImage(listId);
    AssetSite.amDataService.FindPOZ(searchPattern, listId, OnPOZSearchSuccessful, OnError);

}

function OnPOZSearchSuccessful(result) {
    RemoveWaitImage(result.ContainerId);
    var list = document.getElementById(result.ContainerId);
    list.options.length = 0;
    for (i = 0; i < result.Data.length; i++) {
        var option = document.createElement('option');
        option.text = result.Data[i].Value;
        option.value = result.Data[i].Key;

        list.options.add(option);
    }
    if (result.Data.length > 0)
        $(list).highlight(result.Pattern);
}

function OnPOZEditDialog(pozId, otype, containerId) {
    poz_Id = pozId;
    pORz = otype;
    titleText = "Add Place";
    if (otype == 1) {
        titleText = "Add ZipCode";
    }
    $('#' + containerId).dialog({ title: titleText }).dialog('open');
    if (pozId != 0) {
        SetWaitImageWithOffset(-15, -15, containerId);
        AssetSite.amDataService.GetPOZ(pozId, otype, containerId, OnGetPOZSuccess, OnError);
    }
}

function OnGetPOZSuccess(result) {
    if (result.Data != '') {
        RemoveWaitImage(result.ContainerId);
        $('#' + result.ContainerId).find('[type=text]:eq(0)').val(result.Data);
    }
}

function modifyPOZ(placeId, contianerId) {
    var pozName = $('#' + contianerId).find('[type=text]:eq(0)').val();
    SetWaitImageWithOffset(-15, -15, contianerId);
    if (poz_Id == 0) {
        if (pORz == 0) {
            AssetSite.amDataService.Addplace(pozName, contianerId, OnPlaceChanged);
        }
        else {
            AssetSite.amDataService.AddZipCode(pozName, placeId, contianerId, OnZipChanged);
        }
    }
    else {
        if (pORz == 0) {
            AssetSite.amDataService.EditPlace(poz_Id, pozName, contianerId, OnPlaceChanged);
        }
        else {
            AssetSite.amDataService.EditZip(poz_Id, pozName, contianerId, OnZipChanged);
        }
    }
}

function DeleteZip(zipId) {
    AssetSite.amDataService.RemoveZip(zipId, OnZipChanged, OnError);
}

function DeletePlace(placeId) {
    AssetSite.amDataService.RemovePlace(placeId, OnPlaceChanged, OnError);
}

function OnPlaceChanged(result) {
    if (result) {
        RemoveWaitImage(result.ContainerId);
    }
    __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$lbtnRebindPlace', '');
}

function OnZipChanged(result) {
    if (result) {
        RemoveWaitImage(result.ContainerId);
    }
    __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$lbtnRebindZip', '')
}

//function OnPlaceChanged(result) {
//    RemoveWaitImage(result.ContainerId);
//    __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$lbtnRebindPlace', '');
//}

//function OnPOZRemoved() {
//    $('form')[0].submit();
//}