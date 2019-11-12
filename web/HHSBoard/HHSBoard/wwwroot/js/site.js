$.notifyDefaults({
    animate: {
        enter: "animated fadeInUp",
        exit: "animated fadeOutDown"
    },
    z_index: 99999
});

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};

$(document).ready(function () {
    var resourceFile;
    var resourceUrl;

    $("[name=resourceView]").click(function () {
        resourceFile = $(this).attr("data-file");
        resourceUrl = window.location.origin + "/document/" + resourceFile; 

        $("#resourceViewerBody").empty();
        $("#resourceViewer").modal("show");
    });

    $('#resourceViewer').on('shown.bs.modal', function (e) {
        resourceUrl = encodeURIComponent(resourceUrl);
        $("#resourceViewerBody").append(`<iframe src='https://docs.google.com/gview?url=${resourceUrl}&embedded=true' width='100%' height='100%' frameborder='0'></iframe>`);
    });

    $(".viewer-refresh").click(function () {
        $('.viewer-body > iframe').attr("src", $('.viewer-body > iframe').attr("src"));
    });
});