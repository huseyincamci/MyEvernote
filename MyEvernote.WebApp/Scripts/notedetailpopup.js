$(function () {
    $('#modal_note_detail').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);
        noteId = btn.data("note-id");
        var link = "/note/getnotetext/" + noteId;
        $("#modal_note_detail_body").load(link);
    })
});