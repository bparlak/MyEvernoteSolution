$(function () {
	$('#modal_notedetail').on('show.bs.modal', function (e) {
		var btn = $(e.relatedTarget);//tıklanan yorumlar butonu yakalandı
		noteid = btn.data("note-id");//elementin data attribute larından birisi kullanılıyorsa data() methodu kullanılır

		$("#modal_notedetail_body").load("/Note/GetNoteText/" + noteid);
	})
});