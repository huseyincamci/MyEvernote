var noteId = -1;
var modalCommantBodyId = "#modal_comment_body";

$(function () {
    $('#modal_comment').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);
        noteId = btn.data("note-id");
        var link = "/comment/shownotecomments/" + noteId;
        $(modalCommantBodyId).load(link);
    })
});


function doComment(btn, e, commentId, spanId) {
    var button = $(btn);
    var mode = button.data("edit-mode");

    if (e === "edit_clicked") {
        if (!mode) {
            button.data("edit-mode", true);
            button.removeClass("btn-warning");
            button.addClass("btn-success");
            var btnSpan = button.find("span");
            btnSpan.removeClass("fa-edit");
            btnSpan.addClass("fa-check");

            $(spanId).addClass("editable");
            $(spanId).attr("contenteditable", true)
            $(spanId).focus();
        }
        else {
            button.data("edit-mode", false);
            button.removeClass("btn-success");
            button.addClass("btn-warning");
            var btnSpan = button.find("span");
            btnSpan.removeClass("fa-check");
            btnSpan.addClass("fa-edit");

            $(spanId).removeClass("editable");
            $(spanId).attr("contenteditable", false)

            var txt = $(spanId).text();

            $.ajax({
                method: "POST",
                url: "/comment/edit/" + commentId,
                data: { text: txt }
            }).done(function (data) {
                if (data.result) {
                    var link = "/comment/shownotecomments/" + noteId;
                    $(modalCommantBodyId).load(link);
                } else {
                    alert("Yorum güncellenemedi.");
                }
            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
    } else if (e === "delete_clicked") {
        var dialog_result = confirm("Yorum silinsin mi?");

        if (!dialog_result) return false;

        $.ajax({
            method: "GET",
            url: "/comment/delete/" + commentId
        }).done(function (data) {
            if (data.result) {
                var link = "/comment/shownotecomments/" + noteId;
                $(modalCommantBodyId).load(link);
            } else {
                alert("Yorum silinemedi.");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    } else if (e === "new_clicked") {
        var txt = $("#new_comment-text").val();

        $.ajax({
            method: "POST",
            url: "/comment/create/",
            data: { text: txt, "noteId": noteId }
        }).done(function (data) {
            if (data.result) {
                var link = "/comment/shownotecomments/" + noteId;
                $(modalCommantBodyId).load(link);
            } else {
                alert("Yorum eklenemedi.");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    }
}