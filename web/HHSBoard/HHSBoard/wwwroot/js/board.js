$.fn.editable.defaults.mode = 'inline';
$.fn.editableform.buttons =
    '<button type="submit" class="btn btn-primary editable-submit" style="margin-right: 10px;">Save</button>' +
    '<button type="button" class="btn editable-cancel">Cancel</button>';
$.fn.combodate.defaults.maxYear = new Date().getFullYear();

// List of registered tables in order.
var tables = {
    PURPOSE: 0,
    CELEBRATION: 1,
    WIP: 2,
    NEWIMPOP: 3
};

$(function () {
    // Get current table
    var tableType = getUrlParameter("TableType");

    if (typeof tableType == 'undefined' || tableType == 0) {
        tableType = 0;

        $("#navLeft").prop("disabled", true);
    } else {
        tableType = parseInt(tableType);

        if (tableType >= Object.keys(tables).length / 2) {
            $("#navRight").prop("disabled", true);
        }
    }

    $("#navLeft").click(function () {
        $(this).prop("disabled", true);

        if (typeof getUrlParameter("TableType") == 'undefined') {
            location.href = location.href + "&TableType=" + (tableType - 1);
        } else {
            location.href = location.href.replace("TableType=" + tableType, "TableType=" + (tableType - 1));
        }
    });

    $("#navRight").click(function () {
        $(this).prop("disabled", true);

        if (typeof getUrlParameter("TableType") == 'undefined') {
            location.href = location.href + "&TableType=" + (tableType + 1);
        } else {
            location.href = location.href.replace("TableType=" + tableType, "TableType=" + (tableType + 1));
        }
    });
});