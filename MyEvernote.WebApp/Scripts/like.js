$(function () {

    var noteIds = [];
    $("div[data-note-id]").each(function (i, e) {
        noteIds.push($(e).data("note-id"));
    });

    $.ajax({
        method: "POST",
        url: "/note/getliked",
        data: { ids: noteIds }
    }).done(function (data) {

        if (data.result != null && data.result.length > 0) {

            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                var likedNote = $("div[data-note-id=" + id + "]");
                var btn = likedNote.find("button[data-liked]");
                var span = btn.find("span.like-star");

                btn.data("liked", true);
                span.removeClass("fa-star-o");
                span.addClass("fa-star");
            }
        }
    }).fail(function () {

    });

    $("button[data-liked]").click(function () {

        var btn = $(this);
        var liked = btn.data("liked");
        var noteId = btn.data("note-id");
        var spanStar = btn.find("span.like-star");
        var spanCount = btn.find("span.like-count");

        $.ajax({
            method: "POST",
            url: "/note/setlikestate",
            data: { "noteId": noteId, "liked": !liked }
        }).done(function (data) {
            if (data.hasError === undefined) {
                alert("Sunucu ile bağlantı kurulamadı.");
            }
            else {
                if (data.hasError) {
                    alert(data.errorMessage);
                } else {
                    liked = !liked;
                    btn.data("liked", liked);
                    spanCount.text(data.result)

                    spanStar.removeClass("fa-star-o");
                    spanStar.removeClass("fa-star");

                    if (liked) {
                        spanStar.addClass("fa-star");
                    } else {
                        spanStar.addClass("fa-star-o");
                    }
                }
            }
        }).fail(function () {

            alert("Sunucu ile bağlantı kurulamadı.");
        });
    });
});