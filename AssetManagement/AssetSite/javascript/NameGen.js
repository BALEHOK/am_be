/// <reference path="utilities.js" />

function ShowSortDialog() {
    $('#Step4DialogContainer').dialog('open');
}

function SaveOrder() {
    var list = $('#Step4DialogContainer').find('select')[0];
    var containerId = $('#Step4DialogContainer').attr('id');

    var data = new String();

    for (i = 0; i < list.options.length; i++) {
        data += "(" + list.options[i].value + "," + i + ");";
    }

    SetWaitImage(containerId);
    PageMethods.SaveNameGenOrder(data, OnOrderSaved, OnError);
    
}

function OnOrderSaved() {
    var containerId = $('#Step4DialogContainer').attr('id');
    RemoveWaitImage(containerId);
    $('#Step4DialogContainer').dialog('close');
}

function moveUpAttribute() {
    var list = $('#Step4DialogContainer').find('select')[0];

    for (i = 0; i < list.options.length; i++) {
        if (list.options[i].selected) {
            if (i != 0) {
                list.insertBefore(list.options[i], list.options[i - 1]);
                break;
            }
        }
    }
}

function moveDownAttribute() {
    var list = $('#Step4DialogContainer').find('select')[0];

    for (i = 0; i < list.options.length; i++) {
        if (list.options[i].selected) {
            if (i != list.options.length - 1) {
                list.insertBefore(list.options[i + 1], list.options[i]);
                break;
            }
        }
    }
}

function moveTopAttribute() {
    var list = $('#Step4DialogContainer').find('select')[0];

    for (i = 0; i < list.options.length; i++) {
        if (list.options[i].selected) {
            list.insertBefore(list.options[i], list.options[0]);
            break;
        }
    }
}

function moveBottomAttribute() {
    var list = $('#Step4DialogContainer').find('select')[0];

    for (i = 0; i < list.options.length; i++) {
        if (list.options[i].selected) {
            list.appendChild(list.options[i]);
            break;
        }
    }
}