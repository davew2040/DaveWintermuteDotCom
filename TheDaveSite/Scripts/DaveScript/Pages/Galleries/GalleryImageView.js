var GalleryImageViewScript = new function () {
    var toggler = null;
    var root = null;

    var HOLDER_MAX_WIDTH = 600;
    var HOLDER_MAX_HEIGHT = 475;

    var self = {
        initialize: function (root) {
            var $root = $(root);

            $root.find(".galleryImagePanel_ImageHolder").waitForImages(function (a) {
                doImageSizing($(this));
            });

            initializeEvents($root);
        },
    };
    

    function doImageSizing($root) {
        var $holderPanel = $root;
        var $image = $root.find("img");

        var baseWidth = $image.width();
        var baseHeight = $image.height();
        var imageRatio = baseWidth / baseHeight;

        var holderRatio = $holderPanel.width() / $holderPanel.height();

        if (imageRatio > holderRatio) {
            $holderPanel.width(Math.ceil(HOLDER_MAX_WIDTH));
            $holderPanel.height(Math.ceil(HOLDER_MAX_WIDTH / imageRatio));
        }
        else {
            $holderPanel.height(Math.ceil(HOLDER_MAX_HEIGHT));
            $holderPanel.width(Math.ceil(HOLDER_MAX_HEIGHT * imageRatio));
        }

        $image.width($holderPanel.width());
        $image.height($holderPanel.height());
    }

    function handleDeleteClick(eventArgs) {
        var $target = $(eventArgs.currentTarget);

        var $parent = $target.parents(".imageComment");
        var commentId = $parent.attr("data-commentId");

        $.ajax({
            url: "/Galleries/DeleteImageComment/",
            data: {
                commentId: commentId
            },
            type: "post",
            success: function (data, textStatus, jqXHR) {
                refreshComments();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var x = 42;
            }
        });
    }

    function refreshComments() {
        var id = $("#hiddenImageId").val();

        $.ajax({
            url: "/Galleries/GetImageComments/?id=" + id,
            type: "get",
            success: function (result, textStatus, jqXHR) {
                var $target = $("#commentsDiv");
                $target.empty();
                $target.append(result);
                initializeCommentsEvents($target);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var x = 42;
            }
        });
    }

    function handleNewCommentClick(eventArgs) {
        eventArgs.preventDefault();

        var $form = $("#commenter").find("form");

        $.ajax({
            url: "/Galleries/AddImageComment/",
            data: GlobalUtilities.buildFormDataObject($form),
            type: "post",
            success: function (data, textStatus, jqXHR) {
                $("#commenter").css("display", "none");
                $("#commenter .newCommentTextArea").val("");
                refreshComments();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var x = 42;
            }
        });
    }

    function initializeCommentsEvents($root) {
        $root.find(".btn_delete").click(handleDeleteClick);
    }

    function initializeEvents($root) {
        $root.find("#addNewCommentButton").click(function () {
            $root.find("#commenter").css("display", "block");
        });
        
        $root.find("#createNewComment").click(handleNewCommentClick);

        $root.find("#cancelNewComment").click(function (e) {
            $root.find("#commenter").css("display", "none");
            e.preventDefault();
        });

        $root.find("#copyLinkButton").click(function (e) {
            var galleryId = $("#galleryId").val();
            var imageId = $("#hiddenImageId").val();
            var url = GlobalScript.GetAppUrl() + "Galleries/ViewGallery?id=" + galleryId + "&imageId=" + imageId;
            LinkDisplayer.showLink(url);
        });

        initializeCommentsEvents($root);
    }

    return self;
}();