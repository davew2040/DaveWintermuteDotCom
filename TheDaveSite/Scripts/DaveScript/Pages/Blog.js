$(document).ready(function () {
    BlogScript.initialize();
});

var BlogScript = new function () {
    var BlogPageData = function () {
        var self = {
            PageNumber: -1,
            NumberOfPages: -1,
            PostsPerPage: -1,
            ActivePosts: -1
        };

        return self;
    }

    var replierUndo = null;
    var ignoreEventsFlag = false;
    var _pageCounter = null;
    var _pageData = new BlogPageData();

    var PostReplyUndo = function (_$commenterForm) {
        var $commenterForm = _$commenterForm;
        var self = {
            undo: function(){
                $commenterForm.parent().empty();
            }
        }

        return self;
    }

    var CommentReplyUndo = function (_$comment, _$commenter) {
        var $comment = _$comment;
        var $commenter = _$commenter;
        var self = {
            undo: function(){
                $commenter.parent().empty();
                var short = $comment.find(".shortCommentBody");
                var long = $comment.find(".longCommentBody");
                short.css("display", "block");
                long.css("display", "none");
            }
        }

        return self;
    }

    function galleryItemBuilder($liElement){
        var $image = $liElement.find("img");

        $image.addClass("galleryImage");

        var $outerHolder = $("<div class='galleryImageOuterHolder'>");
        var $innerHolder = $("<div class='galleryImageInnerHolder'>");
        $innerHolder.append($image);
        $outerHolder.append($innerHolder);

        $image.click(function (eventArgs) {
            DaveQuickGalleryStatic.setImage(this);
        });

        return $outerHolder;
    }

    function realGalleryItemBuilder($liElement) {
        var $image = $liElement.find("img");

        $image.addClass("galleryImage");

        var $outerHolder = $("<div class='galleryImageOuterHolder'>");
        var $innerHolder = $("<div class='galleryImageInnerHolder'>");
        $innerHolder.append($image);
        $outerHolder.append($innerHolder);

        $image.click(function (eventArgs) {
            var id = parseInt($(this).attr("data-id"));

            var galleryData = new GalleryData();
            var $galleryView = $(this).parents(".galleryContent");
            galleryData.populateData($galleryView);
            GalleryImageViewer.loadGalleryImages(galleryData, id);
        });

        return $outerHolder;
    }

    function initializePoppers() {
        var $images = $(".galleryContentHolder img");
        for (var i = 0; i < $images.length; i++) {
            var $image = $($images[i]);

            initializeSinglePopper($image);
        }
    }

    function initializeSinglePopper(image) {
        var $image = $(image);

        var popperConfig = ImageFocusPopperBase.getConfigObject();
        popperConfig.ScaleFactor = 1.3;
        popperConfig.TransitionTime = 0.2;

        var popper = null;
        var timer = null;

        $image.mouseenter(function (eventArgs) {
            if (popper === null) {
                popper = new ImageFocusPopper($image, popperConfig);
            }
            if (timer !== null) {
                window.clearTimeout(timer);
                timer = null;
            }
            popper.resize();
        });

        $image.mouseleave(function (eventArgs) {
            if (popper !== null) {
                popper.reset();
            }
            timer = window.setTimeout(function () {
                popper.destroy();
                popper = null;
            }, popperConfig.TransitionTime * 1000);
        });
    }


    var self = {
        initialize: function () {
            DaveQuickGalleryStatic.initialize();

            var blogPosts = $(".blogPost");
            for (var i = 0; i < blogPosts.length; i++) {
                self.initializeBlogPostEvents($(blogPosts[i]));
            }

            self.initializeControls();
        },
        getPageData: function(){
            return _pageData;
        },
        initializeControls: function () {
            var $template = $(".galleryTemplate");

            var $imgLinkGalleries = $(".daveImageLinkGallery");
            for (var i = 0; i < $imgLinkGalleries.length; i++) {
                var galleryTarget = $imgLinkGalleries[i];
                var newGallery = new DaveQuickGallery(galleryTarget, $template, galleryItemBuilder);
            }

            var $realGalleries = $(".daveRealGallery");
            for (var i = 0; i < $realGalleries.length; i++) {
                var galleryTarget = $realGalleries[i];
                var newGallery = new DaveQuickGallery(galleryTarget, $template, realGalleryItemBuilder);
            }
            
            // Re-enable someday, perhaps!
            //initializePoppers();

            _pageCounter = new PageCounter({
                itemsPerPage: _pageData.PostsPerPage,
                totalPosts: _pageData.TotalPosts,
                initialPage: _pageData.PageNumber,
                targetDiv: $("#postCounter")[0],
                previousHandler: self.previousPage,
                nextHandler: self.nextPage
            });
        },
        previousPage: function ()
        {
            var url = "/Home/Blog/?pageNumber=" + _pageCounter.getCurrentPage();
            window.location = url;
        },
        nextPage: function () {
            var url = "/Home/Blog/?pageNumber=" + _pageCounter.getCurrentPage();
            window.location = url;
        },
        initializeBlogPostEvents: function ($blogPost) {
            $blogPost.find(".postReplyButton").click(function (eventArgs) {
                //if (!GlobalScript.IsAuthenticated) {
                //    alert("You must be logged in to comment!");
                //    return;
                //}

                var $sender = $(eventArgs.target);
                self.handlePostReplyClick($sender);
            });

            $blogPost.find(".commentReadMore").click(function (eventArgs) {
                var $sender = $(eventArgs.target);
                self.handleCommentReadMoreClick($sender);
            });
        },
        handleCommentReadMoreClick: function ($sender) {
            self.replierUndo();

            var $comment = $sender.parents(".subComment");
            var short = $comment.find(".shortCommentBody");
            var long = $comment.find(".longCommentBody");
            short.css("display", "none");
            long.css("display", "block");

            var $commenter = self.copyCommentPoster($($comment.find(".commentResponseTarget")),
                self.handleCommentReplyClick,
                function (eventArgs) {
                    self.cancelCommentReply();
                    return false;
                });
            replierUndo = new CommentReplyUndo($comment, $commenter);

            $commenter.find("#PostId").val($comment.attr("data-postId"));
            $commenter.find("#CommentId").val($comment.attr("data-commentId"));
        },
        handleCommentReplyClick: function (eventArgs) {
            if (ignoreEventsFlag) {
                return false;
            }

            ignoreEventsFlag = true;

            if (!GlobalScript.IsAuthenticated) {
                alert("You must be logged in to comment!");
                return false;
            }

            var $comment = $(eventArgs.target).parents(".subComment");
            var $form = $($comment.find("form"));
            var formData = GlobalUtilities.buildFormDataObject($form);

            $.ajax({
                url: "/Home/AddNewBlogComment",
                data: formData,
                type: "post",
                success: function (data, textStatus, jqXHR) {
                    location.reload();
                    ignoreEventsFlag = false;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Error submitting post.");
                    ignoreEventsFlag = false;
                }
            })

            return false;
        },
        cancelCommentReply: function(eventArgs){
            self.replierUndo();
        },
        replierUndo: function(){
            if (null !== replierUndo) {
                replierUndo.undo();
                replierUndo = null;
            }
        },
        copyCommentPoster: function($target, postAction, cancelAction){
            var newCommenterForm = self.getCommentsForm().clone();

            newCommenterForm.css("display", "block");
            newCommenterForm.appendTo($target);
            
            if (undefined !== postAction && undefined != cancelAction) {
                newCommenterForm.find("button#createNewComment").click(postAction);
                newCommenterForm.find("button#cancelNewComment").click(cancelAction);
            }

            return newCommenterForm;
        },
        handlePostReplyClick: function (sender) {
            self.replierUndo();

            var blogPost = $(sender.parents(".blogPost"));
            replierContainer = blogPost.find(".blogResponseTarget");
            var newCommenterForm = self.copyCommentPoster(replierContainer, self.postCommentButtonClicked, self.cancelPostComment);

            $(newCommenterForm.find("#PostId")).val(sender.attr("data-postId"));
            $(newCommenterForm.find("#CommentId")).val(sender.attr("data-commentId"));

            replierUndo = new PostReplyUndo(newCommenterForm);
        },
        postCommentButtonClicked: function (eventArgs) {
            if (ignoreEventsFlag) {
                return false;
            }
            ignoreEventsFlag = true;

            var sender = $(eventArgs.target);
            var blogPost = $(sender.parents(".blogPost"));
            var form = blogPost.find("form");
            
            var formData = GlobalUtilities.buildFormDataObject($(form));

            $.ajax({
                url: "/Home/AddNewBlogComment",
                data: formData,
                type: "post",
                success: function (data, textStatus, jqXHR) {
                    ignoreEventsFlag = false;
                    location.reload();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Error submitting post.");
                    ignoreEventsFlag = false; 
                }
            })
            
            return false;
        },
        cancelPostComment: function(eventArgs){
            self.replierUndo();

            return false;
        },
        getCommentsForm: function()
        {
            return $("#commenterForm");
        }
    };

    return self;
}();


var PageCounter = function (initializer) {
    var self = {
        isLastPage: function () {
            return ((currentPage) * itemsPerPage) > totalPosts;
        },
        isFirstPage: function () {
            return (currentPage === 1);
        },
        nextPage: function () {
            if (!self.isLastPage()) {
                currentPage++;
                update();
                if (externalNextHandler) {
                    externalNextHandler();
                }
            }
        },
        previousPage: function () {
            if (!self.isFirstPage()) {
                currentPage--;
                if (externalPrevHandler) {
                    externalPrevHandler();
                }
                update();
            }
        },
        incrementPostCount: function () {
            totalPosts++;
            update();
        },
        getCurrentPage: function () {
            return currentPage;
        }
    };

    var fullProperties = new PageCounter.PageCounterInitializer();

    GlobalScript.JsPropertyCopyIfAvailable(fullProperties, initializer);

    var totalPosts = fullProperties.totalPosts;
    var currentPage = fullProperties.initialPage;
    var itemsPerPage = fullProperties.itemsPerPage;
    var $targetDiv = $(fullProperties.targetDiv);
    var $prevDiv, $currentDiv, $nextDiv;
    var externalNextHandler = null;
    var externalPrevHandler = null;

    function previousHandler(eventArgs) {
        self.previousPage();
    }

    function nextHandler(eventArgs) {
        self.nextPage();
    }

    function initialize(properties) {
        if ($targetDiv.length === 0) {
            throw "Must have valid input div.";
        }

        if (!$targetDiv.hasClass("counter")) {
            $targetDiv.addClass("counter");
        }

        $prevDiv = $("<a href='javascript:void(0)'><div class='counterPrev'>Previous</div></a>");
        $currentDiv = $("<div class='counterCurrent'>");
        $nextDiv = $("<a href='javascript:void(0)'><div class='counterNext'>Next</div></a>");

        $targetDiv.append($prevDiv);
        $targetDiv.append($currentDiv);
        $targetDiv.append($nextDiv);

        externalNextHandler = properties.nextHandler;
        externalPrevHandler = properties.previousHandler;

        $nextDiv.click(nextHandler);
        $prevDiv.click(previousHandler);

        update();
    }

    function update() {
        var message = "No posts.";
        if (totalPosts > 0) {
            var message = "Showing page " + currentPage + " (posts " + getFirstItem() + "-" + getLastItem() + ")";
        }
        $currentDiv.html(message);

        if (self.isLastPage()) {
            $nextDiv.addClass("disabled");
        }
        else {
            $nextDiv.removeClass("disabled");
        }

        if (self.isFirstPage()) {
            $prevDiv.addClass("disabled");
        }
        else {
            $prevDiv.removeClass("disabled");
        }
    }

    function getFirstItem() {
        return ((currentPage-1) * itemsPerPage) + 1;
    }

    function getLastItem() {
        var lastItem = currentPage * itemsPerPage;
        if (lastItem > totalPosts) {
            lastItem = totalPosts;
        }

        return lastItem;
    }

    initialize(fullProperties);

    return self;
}

PageCounter.PageCounterInitializer = function () {
    var self = {
        initialPage: 1,
        totalPosts: 0,
        itemsPerPage: 10,
        targetDiv: null,
        previousHandler: null,
        nextHandler: null
    };

    return self;
};