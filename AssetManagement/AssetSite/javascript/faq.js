/// <reference path="utilities.js" />

//helper function
function ClearFaqInfo(questionTBname, answerTBname) {
    //clear textareas
    CKEDITOR.instances[questionTBname].setData(null);
    CKEDITOR.instances[answerTBname].setData(null);
}

function ShowFaqDlg(dialogId, deId) {
    DynEntityId = deId;

    if (deId != 0) {
        AssetSite.amDataService.GetFaqItem(deId, dialogId, OnGotInfo);
    }
    else {
        //find textareas
        var questionTBname = $('#' + dialogId).find('[type=textarea]:eq(0)')[0].id;
        var answerTBname = $('#' + dialogId).find('[type=textarea]:eq(1)')[0].id;

        //clear textareas
        ClearFaqInfo(questionTBname, answerTBname);
        $("#" + dialogId).dialog('open');
    }
}

function OnGotInfo(result) {
    //find textareas
    var questionTBname = $('#' + result.ContainerId).find('[type=textarea]:eq(0)')[0].id;
    var answerTBname = $('#' + result.ContainerId).find('[type=textarea]:eq(1)')[0].id;

    if (result) {
        //if existing item -> fill textareas
        CKEDITOR.instances[questionTBname].setData(result.Question);
        CKEDITOR.instances[answerTBname].setData(result.Answer);
    }
    else {
        //otherwise -> clear textareas
        ClearFaqInfo(questionTBname, answerTBname);
    }

    $('#' + result.ContainerId).dialog('open');
}

function SaveItem(dialogId) {
    //$('#' + dialogId).find('a:eq(0)')[0].visible = false; // index 21

    var links = $('#' + dialogId).find('a');
    var linkId = links[links.length - 1].id;
    $('#' + linkId).css("visibility", "hidden");

    var questionTBname = $('#' + dialogId).find('[type=textarea]:eq(0)')[0].id;
    var answerTBname = $('#' + dialogId).find('[type=textarea]:eq(1)')[0].id;
    var answer = CKEDITOR.instances[answerTBname].getData();
    var question = CKEDITOR.instances[questionTBname].getData();

    SetWaitImage(dialogId);
    AssetSite.amDataService.SaveFaqItem(DynEntityId, question, answer, cultName, dialogId, OnItemSaved);
}

function OnItemSaved(result) {
    RemoveWaitImage(result);

    var links = $('#' + result).find('a');
    var linkId = links[links.length - 1].id;
    $('#' + linkId).css("visibility", "visible");

    $('#' + result).dialog('close');

    __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$lbtnRebind', '');
}