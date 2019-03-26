$.fn.editable.defaults.mode = 'inline';

// List of registered tables in order.
var tables = {
    PURPOSE: 0,
    CELEBRATION: 1
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

    console.log(tableType);
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