/// <reference path="utilities.js" />

function RestoreTBValue(tbId, value) {
    $("#" + tbId).val(value);
}

function OnAdd(dynListId, tbId, containerId, cbId, listId) {
    var assocListId = 0;
    var cbox = document.getElementById(cbId);
    if (cbox.checked) {
        assocListId = $('#' + listId).val();
    }

    SetWaitImage(containerId);
    AssetSite.amDataService.AddDynListItem(dynListId, $('#' + tbId).val(), assocListId, containerId, OnSuccess, OnError);
    $('#' + tbId).val('');
}

function OnDelete(itemId, containerId) {
    SetWaitImage(containerId);
    AssetSite.amDataService.DeleteDynListItem(itemId, function () {
        RemoveWaitImage(containerId);
    }, OnError);
    $('#' + containerId).hide();
}

function OnSuccess(result) {
    if (result != null && result.Action == 1) {
        ConstructRecord(result.ItemValue, result.ItemUID, result.ContainerId);
    }
    RemoveWaitImage(result.ContainerId);
}
function OnError(result) {
    RemoveWaitImage(result.ContainerId);
    alert("Unable to connect to Data Service");
}

function ConstructRecord(itemName, dynListItemUID, containerId) {
    var mainContainer = document.createElement('div');
    mainContainer.setAttribute('id', containerId + String(Math.random()).replace('.', '_'));

    var nameLink = document.createElement('a');
    nameLink.innerHTML = itemName;
    nameLink.setAttribute('style', "text-decoration:none;");

    var image = document.createElement('img');
    image.setAttribute('src', location.protocol + '//' + location.host + '/images/buttons/delete.png');

    var deleteButton = document.createElement('a');
    deleteButton.setAttribute('onclick', "return OnDelete(" + dynListItemUID + ",'" + mainContainer.getAttribute('id') + "');");
    deleteButton.setAttribute('style', "cursor:pointer; border:0px;");
    deleteButton.appendChild(image);

    var table = document.createElement('table');
    table.setAttribute('border', 0);
    table.setAttribute('cellpadding', 0);
    table.setAttribute('cellspacing', 0);
    table.setAttribute('width', "100%");

    var tblTr = document.createElement('tr');
    var tblTd1 = document.createElement('td');
    var tblTd2 = document.createElement('td');
    tblTd2.setAttribute('style', 'text-align:right;');

    tblTd1.appendChild(nameLink);
    tblTd2.appendChild(deleteButton);
    tblTr.appendChild(tblTd1);
    tblTr.appendChild(tblTd2);
    table.appendChild(tblTr);

    mainContainer.appendChild(table);

    var td = document.getElementById(containerId).getElementsByTagName('td').item(0);

    td.appendChild(mainContainer);    
}