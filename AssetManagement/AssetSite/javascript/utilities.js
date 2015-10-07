var ajaxLoadImage = new Image(31, 31);
ajaxLoadImage.src = '/images/ajax-loader.gif';

/* Cookie API  v1.0.1
* documentation: http://www.dithered.com/javascript/cookies/index.html
* license: http://creativecommons.org/licenses/by/1.0/
* code (mostly) by Chris Nott (chris[at]dithered[dot]com)
*/
function setCookie(name, value, expires, path, domain, secure) {
    var curCookie = name + "=" + escape(value) +
		((expires) ? "; expires=" + expires.toGMTString() : "") +
		((path) ? "; path=" + path : "") +
		((domain) ? "; domain=" + domain : "") +
		((secure) ? "; secure" : "");
    document.cookie = curCookie;
}
function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else {
        begin += 2;
    }
    var end = document.cookie.indexOf(";", begin);
    if (end == -1) {
        end = dc.length;
    }
    return unescape(dc.substring(begin + prefix.length, end));
}
function deleteCookie(name, path, domain) {
    var value = getCookie(name);
    if (value != null) {
        document.cookie = name + "=" +
			((path) ? "; path=" + path : "") +
			((domain) ? "; domain=" + domain : "") +
			"; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
    return value;
}

/* standard trim function to remove leading and trailing 
* whitespace from a given string
*/
function trim(str) {
    return str.replace(/^\s*|\s*$/g, "");
}

function getPreferredStylesheet(group) {
    return (getCookie("style_" + group));
}

function isDefined(variable) {
    return (typeof (window[variable]) == "undefined") ? false : true;
}

function RemoveTemplate(atUID, tID) {
    document.forms[0].style.cursor = 'wait';
    AssetSite.amDataService.DeleteTemplate(atUID, tID, OnTemplateDeleted, OnTemplateDeleteError);
}

function OnTemplateDeleted(result) {
    document.forms[0].style.cursor = 'default';
    if (result != 0) {
        $('#templateItemContainer' + result).remove();
    }
}
function OnTemplateDeleteError() {
    document.forms[0].style.cursor = 'default';
    alert('Error connection server!');
}

function SetWaitImage(containerId) {
    SetWaitImageWithOffset(0, 0, containerId);
}

function SetWaitImageWithOffset(x, y, containerId) {
    var maxZIndex = $('#' + containerId).css('z-index');
    if (!maxZIndex) {
        maxZIndex = 1500;
    }

    var conatinerHeight = $('#' + containerId).outerHeight();
    var containerWidth = $('#' + containerId).outerWidth();

    var marginTop = Math.round(conatinerHeight / 2 - ajaxLoadImage.height / 2) + y;
    var marginLeft = Math.round(containerWidth / 2 - ajaxLoadImage.width / 2) + x;

    var imgContainer = $("<div>");
    imgContainer.attr("id", "ajaxLoadingImageContainer");
    imgContainer.attr("style", "z-index:" + maxZIndex + "; position: absolute; margin-top: " + marginTop + "px;margin-left: " + marginLeft + "px;");
    imgContainer.append(ajaxLoadImage);
    imgContainer.insertBefore($('#' + containerId));
}

function RemoveWaitImage(containerId) {
    $('#' + containerId).parent().find('#ajaxLoadingImageContainer').remove();
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function onMoveOpClick(textId, listId) {
    var textarea = document.getElementById(textId);  //$('#' + textId);
    var val = $('select#' + listId + ' :selected').text();
    insertAtCursor(textarea, val);
}

function insertAtCursor(myField, val) {
    myValue = val + ' ';
    //IE support
    if (document.selection) {
        myField.focus();
        sel = document.selection.createRange();
        sel.text = myValue;
    }
        //Mozilla/Firefox/Netscape 7+ support
    else if (myField.selectionStart || myField.selectionStart == '0') {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
        myField.value = myField.value.substring(0, startPos) + myValue + myField.value.substring(endPos, myField.value.length);
    } else {
        myField.value += myValue;
    }
}

function attachDocument(elementId, classExist, classNew) {
    var wnd = window.open("/Asset/AttachDocument.aspx?ElementId=" + elementId + "&ClassExist=" + classExist + "&ClassNew=" + classNew, "AttachDoc", "status=0, toolbar=0, location=0, menubar=0, resizable=1, scrollbars=1, width=600, height=500", false);
    return false;
}

function returnAttachedDocument(docId, link, elementId, classExist, classNew) {
    var doc = window.opener.document;
    $('#' + elementId, doc).val(docId);
    $("." + classExist + ":first", doc).html(link);
    $("." + classNew, doc).hide();
    $("." + classExist, doc).show();
    window.close();
}

function detachDocument(docId, elementId, DeleteAttachedDocument, DeleteAttachedDocumentId, classExist, classNew) {
    var res = confirm('Delete attached document from system?');
    $('#' + elementId).val(0);
    $('#' + DeleteAttachedDocument).val(res ? "1" : "0");
    $('#' + DeleteAttachedDocumentId).val(docId);
    $("." + classExist).hide();
    $("." + classNew).show();
    return false;
}

function callback(result) {
    // asp.net ajax 3.0/3.5 compability
    if (result.hasOwnProperty('d'))
        $('.tmpBarcode').val(result.d);
    else
        $('.tmpBarcode').val(result);
}

function InitRichText(controlId, locale) {
    $('#' + controlId).ckeditor(function () {
        //var editor = $('#' + controlId).ckeditorGet();
    }, { language: locale });
}

function GenerateBarcode(controlId) {
    AssetSite.amDataService.GenerateBarcode(function (result) {
        $('#' + controlId).val(result)
    }, function () { });
}

function ShowDialog(containerId) {
    $("#" + containerId).dialog('open');
}

function ShowConfirmationDialog(onConfirm) {
    $('.confirmation').dialog('option', 'buttons', [
        {
            text: 'Cancel',
            click: function () { $(this).dialog('close'); }
        },
        {
            text: 'Ok',
            click: onConfirm
        }
    ]);
    $('.confirmation').dialog('open');
    return false;
}

function DeleteAsset(atUid, aId) {
    var onConfirmFunction = function () {
        document.forms[0].style.cursor = 'wait';
        var dialog = $(this);
        AssetSite.amDataService.DeleteAsset(atUid, aId, function (result) {
            document.forms[0].style.cursor = 'default';
            if (result.IsSuccess) {
                window.location.href = window.location.href;
            } else {
                dialog.html('<p>' + result.Message + '</p>');
                dialog.dialog('option', 'buttons', [
                {
                    text: 'Ok',
                    click: function () { window.location.href = window.location.href; }
                }]);
            }
        });
    }
    ShowConfirmationDialog(onConfirmFunction);
    return false;
}

function utilities_SaveAsset() {
    _utilities_show_overlay();
    return __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$btnSave', '');
}

function utilities_SaveAssetAndAddNew() {
    _utilities_show_overlay();
    return __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$btnSaveAndAdd', '');
}

function _utilities_show_overlay() {
    var opts = {
        lines: 13, // The number of lines to draw
        length: 20, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#fff', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 150 // Left position relative to parent in px
    };
    var target = document.getElementById('waitMessage');
    var spinner = new Spinner(opts).spin(target);

    $.blockUI({
        css: {
            border: 'none',
            backgroundColor: '#000',
            color: '#fff',
            width: 0
        },
        message: $('#waitMessage')
    });

}