/// <reference path="jquery-1.4.4.min.js" />

function AddParam() {
    AddParamWithValue('', '');
}

function AddParamWithValue(paramName, paramValue) {
    var nRow = $('<tr></tr>');

    var lableTd = $('<td></td>').addClass('controls');
    var nameInput = $('<input />').attr({ type: 'text' }).addClass('SelectControl').val(paramName);
    lableTd.append(nameInput);

    var controlTd = $('<td></td>').addClass('controls');
    var valueInput = $('<input/>').attr({ type: 'text' }).addClass('SelectControl').val(unescape(paramValue));
    controlTd.append(valueInput);

    var removeTd = $('<td></td>').addClass('funcColumn');
    var removeBtn = $('<a/>').attr({ style: 'cursor: pointer; border : 0' }); //, onclick: 'return RemoveParam();'
    removeBtn.bind('click', RemoveParam);
    var removeImg = $('<img />').attr({ src: '../../images/buttons/delete.png' });
    removeBtn.append(removeImg);
    removeTd.append(removeBtn);

    nRow.append(lableTd);
    nRow.append(controlTd);
    nRow.append(removeTd);

    var rowIndex = $('table#paramsGrid tr').length - 1;
    nRow.insertBefore($('table#paramsGrid tr')[rowIndex]);
}

function RemoveParam() {
    $(this).parent().parent().remove();
}

function SearchDataCheck() {
    var result = PageMethods.CheckForFunctionData();
    if (result == false) {
        alert('Error: Task configuration was not complite!');
    }

    return result;
}

function CollectParams(fieldId) {
    var data = new String();
    $('#paramsGrid tr').each(function () {
        var inputs = $('input:text', this);
        if (inputs !== undefined && inputs.size() == 2) {
            var txtName = $(inputs[0]).val();
            var txtValue = $(inputs[1]).val();
            if (txtName != '' && txtValue != '') {
                data += txtName + ":" + txtValue + ",";
            }
        }
    });
    $('#' + fieldId).val(data);
    return true;
}