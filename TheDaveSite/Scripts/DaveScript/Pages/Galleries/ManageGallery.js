$(document).ready(function () {
    ManageGalleryScript.initialize();
});

var ManageGalleryScript = new function () {
    function griddilize($galleryView) {
        var gridConfig = Griddlizer.getConfigObject();

        gridConfig.TableClass = "galleryTable";
        gridConfig.ItemProcessor = self.GridItemProcessor;

        var newTable = Griddlizer.buildItemGrid($galleryView, gridConfig);

        $galleryView.empty();

        $galleryView.append(newTable);
    }

    function griddilize($source, $target) {
        var gridConfig = Griddlizer.getConfigObject();

        gridConfig.TableClass = "galleryTable";
        gridConfig.ItemProcessor = self.GridItemProcessor;

        var newTable = Griddlizer.buildItemGrid($source, gridConfig);

        $target.append(newTable);
    }

    function buildOrderedUlListByOrderedIndexList(orderList) {
        var $images = $(".galleryTable img");
        var $ulContainer = $("<ul>");

        for (var i = 0; i < orderList.length; i++) {
            var arrayMatch = GlobalUtilities.arrayMatch($images, function (item) {
                var $item = $(item);
                return $item.attr("data-id") === orderList[i];
            });

            var $item = $(arrayMatch[0]);
            $ulContainer.append($("<li>").append($item.clone()));
        }

        return $ulContainer;
    }

    function deleteClick(eventArgs) {
        var $eventSource = $(eventArgs.currentTarget);
        var $imageRoot = $eventSource.parents(".galleryItemHolder");
        var $targetImage = $imageRoot.find(".viewerTarget");
        var id = parseInt($targetImage.attr("data-id"));

        doDelete(id);
    }

    function doDelete(id) {
        GlobalScript.GlobalSpinner.show();
        $.ajax({
            url: "/Galleries/DeleteGalleryImage",
            data: {
                imageId: id
            },
            type: "post",
            success: function (data, textStatus, jqXHR) {
                location.reload();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error uploading image.");
                GlobalScript.GlobalSpinner.hide();
            }
        });
    }

    function refreshImageGrid(orderList) {
        $("#tempUlHolder").empty();
        $("#tempUlHolder").append(buildOrderedUlListByOrderedIndexList(orderList));

        buildManagementGrid();
    }

    function handleDrop(e) {
        e.originalEvent.preventDefault();
            var draggedId = parseInt(e.originalEvent.dataTransfer.getData("Text"));
            var targetImageId = parseInt($(this).find(".viewerTarget").attr("data-id"));

            if (draggedId === targetImageId) {
                return;
            }

            var imageIdList = "";
            $images = $("#galleryView .viewerTarget");
            for (var i = 0; i < $images.length; i++) {
                var $image = $($images[i]);

                var currentId = parseInt($image.attr("data-id"));
                if (currentId === draggedId) {
                    // skip
                }
                else if (currentId === targetImageId) {
                    imageIdList += draggedId + "|";
                    imageIdList += targetImageId + "|";
                }
                else {
                    imageIdList += currentId + "|";
                }
            }

            var galleryId = $("#galleryId").val();

            $.ajax({
                url: "/Galleries/ReorderImages",
                data: {
                    galleryId: galleryId,
                    newOrder: imageIdList
                },
                type: "post",
                success: function (data, textStatus, jqXHR) {
                    refreshImageGrid(GlobalUtilities.splitWithEmptyRemove(imageIdList, "|"));
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Error with reodering operation.");
                }
            });
    }

    function initializeDragDrop() {
        var $galleryView = $("#galleryView img");

        $(".deleteClicker").on("dragstart", function (e) {
            var originalEvent = e.originalEvent;
            originalEvent.dataTransfer.effectAllowed = 'move';

            var $eventSource = $(e.currentTarget);
            var $imageRoot = $eventSource.parents(".galleryItemHolder");
            var $targetImage = $imageRoot.find(".viewerTarget");
            var id = parseInt($targetImage.attr("data-id"));

            originalEvent.dataTransfer.setData('Text', "" + id);
        });

        $("#galleryView td").on("dragover", function (e) {
            e.originalEvent.preventDefault();
        });

        $("#galleryView td").on("drop", handleDrop);
    }

    function buildManagementGrid() {
        var $galleryView = $("#galleryView");

        $galleryView.empty();

        griddilize($("#tempUlHolder"), $galleryView);

        HoverDiv.addHovers($galleryView);

        $(".deleteClicker img").click(deleteClick);

        initializeDragDrop();
    }

    var self = {
        initialize: function () {
            buildManagementGrid();
        },
        GridItemProcessor: function (liElement) {
            var containerDiv = $("<div></div>");
            containerDiv.addClass("galleryItemHolder");
            containerDiv.addClass("draggable");
            containerDiv.addClass("droppable");

            var hoverDiv = $("<div></div>");
            hoverDiv.addClass("hoverable");
            hoverDiv.addClass("manageGalleryHover");

            containerDiv.append(hoverDiv);

            var centererDiv = $("<div></div>");
            centererDiv.addClass("galleryItemInnerContainer");

            containerDiv.append(centererDiv);
            centererDiv.append($(liElement).children());

            return containerDiv;
        }
    };

    return self;
}();

var HoverDiv = new function () {
    var initialized = false

    function initialize(target) {
        initialized = true;
    }

    function HoverableEnter(eventArgs) {
        var $eventTarget = $(eventArgs.currentTarget);
        $eventTarget.addClass("hoverOn");

        var $clickerStuff = $("#deleteClickerSource .deleteClicker").clone(true);
        $eventTarget.append($clickerStuff);
    }

    function HoverableLeave(eventArgs) {
        var $eventTarget = $(eventArgs.currentTarget);
        $eventTarget.removeClass("hoverOn");

        $eventTarget.find(".deleteClicker").remove();
    }

    var self = {
        addHovers: function (target) {
            if (!initialized) {
                initialize();
            }

            var $target = $(target);
            var $hoverables = $target.find(".hoverable");

            $hoverables.on("mouseenter", HoverableEnter);
            $hoverables.on("mouseleave", HoverableLeave);
        }
    };

    return self;
};

var DragDropper = new function () {
    var initialized = false
    
    function initialize(target) {
        var $target = $(target);

        initialized = true;

        var $draggables = $target.find(".draggable");

    }

    var self = {
        addDragDrop: function (target) {
            if (!initialized) {
                initialize();
            }
        }
    };

    return self;
};