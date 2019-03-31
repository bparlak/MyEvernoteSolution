$(function () {
	var noteids = [];
	//sayfadaki data-note-id özelliğine sahip divlerin herbiri
	$("div[data-note-id]").each(function (i, e) {
		noteids.push($(e).data("note-id"));
	});

	//console.log(noteids);
	$.ajax({
		method: "POST",
		url: "/Note/GetLiked",
		data: { ids: noteids }
	}).done(function (data) {
		//console.log(data);
		if (data.result != null && data.result.length > 0) {
			for (var i = 0; i < data.result.length; i++) {
				var id = data.result[i];
				var likedNote = $("div[data-note-id=" + id + "]");
				var btn = likedNote.find("button[data-liked]");
				var span = btn.find("span.like-star");

				btn.data("liked", true);
				span.removeClass("glyphicon-star-empty");
				span.addClass("glyphicon-star");
			}
		}
	}).fail(function () {

	});
	$("Button[data-liked]").click(function () {
		var btn = $(this);
		var liked = btn.data("liked");
		var noteid = btn.data("note-id");
		var spanStar = btn.find("span.like-star");
		var spanCount = btn.find("span.like-count");
		//console.log(liked);
		//console.log("like count: " + spanCount.text());
		$.ajax({
			method: "POST",
			url: "/Note/SetLikeState",
			data: { "noteid": noteid, "liked": !liked }//like için ters işlemini yani olmasını istediğimiz durumu göndedik.
		}).done(function (data) {
			//console.log(data);
			if (!data.hasError) {
				window.location.href = "/Home/Login";
			} else {
				liked = !liked;
				btn.data("liked", liked);
				spanCount.text(data.result);

				//console.log("like count(after): " + spanCount.text());
				spanStar.removeClass("glyphicon-star-empty");
				spanStar.removeClass("glyphicon-star");

				if (liked) {
					spanStar.addClass("glyphicon-star");
				} else {
					spanStar.addClass("glyphicon-star-empty");
				}

			}
		}).fail(function () {
			alert("Sunucu ile bağlantı kurulamadı.");
		});

	});
})