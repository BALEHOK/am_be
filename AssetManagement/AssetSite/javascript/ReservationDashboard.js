function resDashboardInitDlg(atUid, dashboardId, dlgId, hfReservationId, ddlNeResourceId, tbBorrowerNameField, tbBorrowerIdField, tbNewStartDateId, tbNewEndDateId, tbNewReasonId, lblNewBorrowErrorId) {
    var dashboard = $('#' + dashboardId);
    var dData = {
        newDlg: $('#' + dlgId),
        hfReservationUid: $('#' + hfReservationId),
        ddlNewResource: $('#' + ddlNeResourceId),
        tbBorrowerName: $('#' + tbBorrowerNameField),
        tbBorrowerId: $('#' + tbBorrowerIdField),
        tbNewStartDate: $('#' + tbNewStartDateId),
        tbNewEndDate: $('#' + tbNewEndDateId),
        tbNewReason: $('#' + tbNewReasonId),
        lblNewBorrowError: $('#' + lblNewBorrowErrorId)
    };
    dashboard.data('DashboardData', dData);
    dData.tbNewStartDate.datepicker().datepicker('disable');
    dData.tbNewEndDate.datepicker().datepicker('disable');
    var onDialogOk = function() {
        var scheduler = eval(dashboardId + '_Scheduler');
        var reservationUid = dData.hfReservationUid.val();
        var aUid = dData.ddlNewResource.val();
        var userId = dData.tbBorrowerId.val();
        var start = dData.tbNewStartDate.datepicker('getDate');
        var end = dData.tbNewEndDate.datepicker('getDate');
        var reason = dData.tbNewReason.val();
        dData.lblNewBorrowError.html("");
        ShowProgress(dashboardId);
        AssetSite.amDataService.ReserveAssetWithTimeRange(reservationUid, atUid, aUid, start, end, reason, dashboardId, userId, OnNewReservSuccess, OnNewReservError);
        return false;
    };
    dData.newDlg.dialog({
        autoOpen: false,
        width: 420,
        height: 350,
        buttons: [
            { text: 'Save', click: onDialogOk },
            { text: 'Close', click: function() { dData.newDlg.dialog('close'); } }
        ]
    });
}

function resDashboardTimeRangeSelected(dashboardId, start, end, resource) {
    var dashboard = $('#' + dashboardId);
    var dData = dashboard.data('DashboardData');
    dData.ddlNewResource.val(resource);
    dData.tbNewStartDate.datepicker("setDate", start);
    dData.tbNewEndDate
        .datepicker('option', 'minDate', start.d)
        .datepicker("setDate", end.addDays(-1).d);
    dData.tbNewReason.val("");
    dData.newDlg.dialog('option', 'title', 'Create reservation');
    dData.newDlg.dialog('open');
    dData.tbNewStartDate.datepicker('enable');
    dData.tbNewEndDate.datepicker('enable');
    dData.lblNewBorrowError.html("");
}

function resDashboardEventClick(e) {
    if (e.data.tag[5] == 'True') {
        alert('This asset is borrowed already and reservation cannot be changed.');
        return;
    }
    var dashboard = $('#' + e.calendar.id);
    var dData = dashboard.data('DashboardData');
    dData.tbNewStartDate.datepicker("setDate", e.start().d);
    dData.tbNewEndDate.datepicker("setDate", e.end().addDays(-1).d);
    dData.newDlg.dialog('option', 'title', 'Edit reservation');
    dData.tbNewStartDate.datepicker('enable');
    dData.tbNewEndDate.datepicker('enable');
    // ReservationUid, AssetUID, Borrower, BorrowerId, Reason
    dData.hfReservationUid.val(e.data.tag[0]);
    dData.ddlNewResource.val(e.data.tag[1]);
    dData.tbBorrowerName.val(e.data.tag[2]);
    dData.tbBorrowerId.val(e.data.tag[3]);
    dData.tbNewReason.val(e.data.tag[4]);
    dData.newDlg.dialog('open');
}

function ShowProgress(dashboardId) {
    var dashboard = $('#' + dashboardId);
    var dData = dashboard.data('DashboardData');
    dData.ddlNewResource.attr('disabled', 'disabled');
    dData.tbNewStartDate.datepicker('disable');
    dData.tbNewEndDate.datepicker('disable');
    dData.tbNewReason.attr('disabled', 'disabled');
    SetWaitImage(dData.newDlg[0].id);
}

function HideProgress(dashboardId) {
    var dashboard = $('#' + dashboardId);
    var dData = dashboard.data('DashboardData');
    dData.ddlNewResource.removeAttr('disabled');
    dData.tbNewStartDate.datepicker('enable');
    dData.tbNewEndDate.datepicker('enable');
    dData.tbNewReason.removeAttr('disabled');
    RemoveWaitImage(dData.newDlg[0].id);
}

function OnNewReservSuccess(result) {
    var dashboard = $('#' + result.ContainerId);
    var dData = dashboard.data('DashboardData');
    HideProgress(result.ContainerId);
    if (!result.ErrorMessage) {
        dData.newDlg.dialog('close');
        location.reload();
    }
    else {
        dData.lblNewBorrowError.html(result.ErrorMessage);
    }
}

function OnNewReservError(result) {
}