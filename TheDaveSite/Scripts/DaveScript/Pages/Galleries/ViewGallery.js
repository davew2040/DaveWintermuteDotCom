$(document).ready(function () {
    ViewGalleryScript.initialize();
});

var ViewGalleryScript = new function () {
    var PAGES_TO_CACHE = 3;

    var _galleryData;

    function griddilize($galleryView) {
        var gridConfig = Griddlizer.getConfigObject();
        gridConfig.TableClass = "galleryTable";
        gridConfig.ItemProcessor = ViewGalleryScript.GridItemProcessor;

        $galleryView.append(Griddlizer.buildItemGrid($galleryView, gridConfig));

        $galleryView.find("ul").remove();
    }

    function initializeSinglePopper(imageHolder) {
        var $imageHolder = $(imageHolder);

        var popperConfig = ImageFocusPopperBase.getConfigObject();
        popperConfig.ScaleFactor = 1.3;
        popperConfig.TransitionTime = 0.2;

        var popper = null;

        $imageHolder.mouseenter(function (eventArgs) {
            var data = GlobalUtilities.getElementDataObject(this);
            if (!data.popper) {
                data.popper = new BorderedImageFocusPopper($imageHolder, popperConfig);
            }
            data.popper.resize();
        });

        $imageHolder.mouseleave(function (eventArgs) {
            var data = GlobalUtilities.getElementDataObject(this);
            if (data.popper) {
                data.popper.reset();
            }
        });
    }

    function initializePoppers() {
        var $imageHolders = $("#galleryView .galleryItemHolder");
        for (var i = 0; i < $imageHolders.length; i++) {
            var $imageHolder = $($imageHolders[i]);

            initializeSinglePopper($imageHolder);
        }
    }

    function removeSinglePopper(image) {
        var $image = $(image);

        var data = GlobalUtilities.getElementDataObject($image[0]);
        if (data.popper) {
            data.popper.destroy();
            delete data.popper;
        }
        $image.mouseenter();
        $image.mouseleave();
    }

    function removePoppers() {
        var $images = $("#galleryView .galleryItemHolder");
        for (var i = 0; i < $images.length; i++) {
            var $image = $($images[i]);

            removeSinglePopper($image);
        }
    }

    var self = {
        initialize: function () {
            _galleryData = new GalleryData();

            var $galleryView = $("#galleryView");

            griddilize($galleryView);

            initializePoppers();

            self.bindGalleryImages();

            $(window).resize(function () {
                removePoppers();
                initializePoppers();
            });

            if (initialViewImageId > 0) {
                var $galleryView = $("#galleryView");

                _galleryData.populateData($galleryView);
                GalleryImageViewer.loadGalleryImages(_galleryData, initialViewImageId);
            }
        },
        GridItemProcessor: function (liElement) {
            var containerDiv = $("<div></div>");
            containerDiv.addClass("galleryItemHolder");

            var centererDiv = $("<div></div>");
            centererDiv.addClass("galleryItemInnerContainer");

            containerDiv.append(centererDiv);
            centererDiv.append($(liElement).children());

            return containerDiv;
        },
        bindGalleryImages: function () {
            $("#galleryView .galleryTable .galleryItemHolder").click(function (e) {
                var id = parseInt($(this).find("img").attr("data-id"));

                var $galleryView = $("#galleryView");
                _galleryData.populateData($galleryView);
                GalleryImageViewer.loadGalleryImages(_galleryData, id);
            });
        }
    };

    return self;
}();
