function LoadModalContent(identifier, url) {
    $('html, body').css("cursor", "wait");
    $("#AjaxModalTitle").text("Loading ...");
    $("#AjaxModalBody").html("<p class='text-muted'>Please wait ...</p>");

    var route = url + "?identifier=" + identifier;
    $.ajax({
        url: route,
        method: "GET",
        dataType: "json",
        cache: false,
        success: function (result) {
            $("#AjaxModalTitle").text(result.title);
            $("#AjaxModalBody").html(result.htmlContent);
            $("#AjaxModal").modal("show");
            $('html, body').css("cursor", "auto");
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText;
            $("#AjaxModalTitle").text("Error");
            $("#AjaxModalBody").html("<div class='text-danger'>" + errorMessage + "</div>");
            $('html, body').css("cursor", "auto");
        }
    });
}
