/// <reference path="utilities.js" />
var releaseDialog;
function initReservationDialog(releaseDlgId, cbIsDamagedId, tbRemarkId) {
    releaseDialog = {
        show: function(reservationUid) {
            $('#' + releaseDlgId).dialog({
                title: 'Release reserved asset',
                autoOpen: false,
                width: 420,
                height: 250,
                buttons: [
                    {
                        text: 'Ok',
                        click: function() {
                            AssetSite.amDataService.ReleaseBorrowedReservationByUid(
                                reservationUid,
                                $('#' + cbIsDamagedId).val() == "on",
                                $('#' + tbRemarkId).val(),
                                releaseDlgId,
                                OnSuccess);
                        }
                    },
                    { text: 'Cancel', click: function() { $('#' + releaseDlgId).dialog('close'); } }
                ]
            });
            $('#' + releaseDlgId).dialog('open');
        }
    };
}

function ShowReleaseReservationDialog(reservationUid) {
    releaseDialog.show(reservationUid);
}

function MarkReservationBorrowed(reservationUid) {
    AssetSite.amDataService.BorrowReservationByUid(reservationUid, OnSuccess);
}

function OnSuccess(result) {
    RemoveWaitImage(result.ContainerId);
    if (!result.ErrorMessage) {
        $('#' + result.ContainerId).dialog('close');        
        location.reload(false);
    }
    else {
        ShowMessage(result.ContainerId, result.ErrorMessage);
        setTimeout(function () { BorrowErrorCleanup(result); }, 5000);
    }
}

function ShowMessage(containerId, msg) {
    $('#' + containerId).dialog("option", 'height', 310);
    $('#' + containerId).css('height', 310);
    $('#' + containerId).children('span:first').html(msg);
} 

function BorrowErrorCleanup(result) {
    $('#' + result.ContainerId).children('span:first').html('');
    $('#' + result.ContainerId).css('height', 270);
    $('#' + result.ContainerId).dialog("option", 'height', 270);
}
