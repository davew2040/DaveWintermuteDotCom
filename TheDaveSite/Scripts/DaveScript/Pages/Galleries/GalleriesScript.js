$(document).ready(function () {
    GalleriesScript.initialize();
});

var GalleriesScript = new function () {
    var self = {
        initialize: function () {
            initializeEvents();
        }
    };

    function initializeEvents() {
        $(".btnGalleryDelete").click(function(){
            if (false === confirm("Are you sure you want to delete this gallery?"))
            {
                return;
            }

            var $link = $(this);
            var galleryId = $link.attr("data-id");

            GlobalScript.GlobalSpinner.show();
            $.ajax({
                url: "/Galleries/DeleteGallery",
                data: {
                    galleryId: galleryId,
                },
                async: true,
                type: "post",
                success: function (data, textStatus, jqXHR) {
                    GlobalScript.GlobalSpinner.hide();
                    location.reload();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    GlobalScript.GlobalSpinner.hide();
                    alert("Error completing operation.");
                }
            });
        });

        $(".gallerySummary").click(function () {
            $(this).find(".viewGalleryLink")[0].click();
        });
    }

    return self;
}();