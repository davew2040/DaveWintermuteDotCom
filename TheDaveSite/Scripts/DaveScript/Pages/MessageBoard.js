$(document).ready(function () {
    if (CURRENT_PAGE === "MessageBoardIndex")
    {
        MessageBoardIndex.initialize();
    }
    else if (CURRENT_PAGE === "MessageBoard")
    {
        MessageBoard.initialize();
    }
});

var MessageBoardIndex = new function(){
    var Colors = {
        messageBoardOdd: "#9190ff",
        messageBoardEven: "#ffd0af"
    };

    var self = {
        initialize: function () {
            self.alignMessageBoardListColors();
        },
        alignMessageBoardListColors: function () {
            $(".messageBoard").each(function (index, element) {
                if (index % 2 == 0){
                    $(element).css("background-color", Colors.messageBoardEven);
                }
                else{
                    $(element).css("background-color", Colors.messageBoardOdd);
                }
            });
        },
    };

    return self;
}

var MessageBoard = new function () {
    var replierUndo = null;
    var ignoreEventsFlag = false;

    var pageCounter = null;

    var PostReplyUndo = function (_$commenterForm) {
        var $commenterForm = _$commenterForm;
        var self = {
            undo: function () {
                $commenterForm.parent().empty();
            }
        }

        return self;
    }

    var CommentReplyUndo = function (_$comment, _$commenter) {
        var $comment = _$comment;
        var $commenter = _$commenter;
        var self = {
            undo: function () {
                $commenter.parent().empty();
                var short = $comment.find(".shortCommentBody");
                var long = $comment.find(".longCommentBody");
                short.css("display", "block");
                long.css("display", "none");
            }
        }

        return self;
    }

    var self = {
        initialize: function () {
            pageCounter = new PageCounter({
                postCount: PageData.PostCount,
                targetDiv: $("#postCounter")[0],
                previousHandler: self.loadPosts,
                nextHandler: self.loadPosts
            });

            self.loadPosts();

            self.initializeEvents();
        },
        initializeEvents: function()
        {
            $(".newPostButtonDiv").click(self.handlePostCommentClick);
        },
        initializeAllPostEvents: function()
        {
            var posts = $(".post");
            for (var i = 0; i < posts.length; i++) {
                self.initializePostEvents($(posts[i]));
            }
        },
        initializePostEvents: function ($post) {
            $post.find(".postReplyButton").click(function (eventArgs) {
                // We'll allow anonymous commenting for now!
                //if (!GlobalScript.IsAuthenticated) {
                //    alert("You must be logged in to comment!");
                //    return;
                //}

                var $sender = $(eventArgs.target);
                self.handlePostReplyClick($sender);
            });

            $post.find(".commentReadMore").click(function (eventArgs) {
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
        handlePostCommentClick: function (eventArgs) {
            // We'll allow anonymous commenting for now!
            //if (!GlobalScript.IsAuthenticated) {
            //    alert("You must be logged in to post!");
            //    return false;
            //}

            self.replierUndo();

            var $commenter = self.copyPoster($("#newPostTarget"),
                function (eventArgs) {
                    var form = $("#newPostTarget form");

                    var formData = GlobalUtilities.buildFormDataObject($(form));

                    GlobalScript.GlobalSpinner.show();

                    $.ajax({
                        url: "/MessageBoard/AddNewMessageBoardPost",
                        data: formData,
                        type: "post",
                        async: true,
                        success: function (data, textStatus, jqXHR) {
                            ignoreEventsFlag = false;
                            pageCounter.incrementPostCount();
                            self.replierUndo();
                            GlobalScript.GlobalSpinner.hide();
                            self.loadPosts();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert("A problem occurred while trying to post this comment.");
                            GlobalScript.GlobalSpinner.hide();
                            ignoreEventsFlag = false;
                        }
                    })

                    return false;
                },
                function (eventArgs) {
                    self.cancelCommentReply();
                    return false;
                });
            replierUndo = new PostReplyUndo($commenter);
        },
        handleCommentReplyClick: function (eventArgs) {
            if (ignoreEventsFlag) {
                return false;
            }

            ignoreEventsFlag = true;

            // We'll allow anonymous commenting for now!
            //if (!GlobalScript.IsAuthenticated) {
            //    alert("You must be logged in to comment!");
            //    return false;
            //}

            var $comment = $(eventArgs.target).parents(".subComment");
            var $form = $($comment.find("form"));
            var formData = GlobalUtilities.buildFormDataObject($form);

            $.ajax({
                url: "/MessageBoard/AddNewMessageBoardComment",
                data: formData,
                type: "post",
                success: function (data, textStatus, jqXHR) {
                    location.reload();
                    ignoreEventsFlag = false;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("A problem occurred while trying to post this comment.");
                    ignoreEventsFlag = false;
                }
            })

            return false;
        },
        cancelCommentReply: function (eventArgs) {
            self.replierUndo();
        },
        replierUndo: function () {
            if (null !== replierUndo) {
                replierUndo.undo();
                replierUndo = null;
            }
        },
        copyPoster: function ($target, postAction, cancelAction) {
            var newCommenterForm = self.getPosterForm().clone();

            newCommenterForm.css("display", "block");
            newCommenterForm.appendTo($target);

            if (undefined !== postAction && undefined != cancelAction) {
                newCommenterForm.find("button#createNewComment").click(postAction);
                newCommenterForm.find("button#cancelNewComment").click(cancelAction);
            }

            return newCommenterForm;
        },
        copyCommentPoster: function ($target, postAction, cancelAction) {
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

            var post = $(sender.parents(".post"));
            replierContainer = post.find(".postResponseTarget");
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
            var post = $(sender.parents(".post"));
            var form = post.find("form");

            var formData = GlobalUtilities.buildFormDataObject($(form));

            GlobalScript.GlobalSpinner.show();

            $.ajax({
                url: "/MessageBoard/AddNewMessageBoardComment",
                data: formData,
                type: "post",
                async: true,
                success: function (data, textStatus, jqXHR) {
                    ignoreEventsFlag = false;
                    GlobalScript.GlobalSpinner.hide();
                    self.loadPosts();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("A problem occurred while trying to post this comment.");
                    GlobalScript.GlobalSpinner.hide();
                    ignoreEventsFlag = false;
                }
            })

            return false;
        },
        cancelPostComment: function (eventArgs) {
            self.replierUndo();

            return false;
        },
        getCommentsForm: function () {
            return $("#commenterForm");
        },
        getPosterForm: function () {
            return $("#posterForm");
        },
        loadPosts: function () {
            var parameters = {
                Id: PageData.MessageBoardId,
                pageNumber: pageCounter.getCurrentPage()
            };

            GlobalScript.GlobalSpinner.show();

            $.ajax({
                url: "/MessageBoard/MessageBoardPosts",
                data: parameters,
                type: "get",
                async: true,
                success: function (data, textStatus, jqXHR) {
                    ignoreEventsFlag = false;
                    
                    $("#messageBoardContent").empty();
                    $("#messageBoardContent").append(data);

                    self.initializeAllPostEvents();

                    GlobalScript.GlobalSpinner.hide();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    GlobalScript.GlobalSpinner.hide();
                    alert("A problem occurred while getting new posts.");
                    ignoreEventsFlag = false;
                }
            });
        }
    };

    return self;
}();

var PageCounter = function (initializer) {
    var self = {
        isLastPage: function () {
            return ((currentPage + 1) * itemsPerPage) > postCount;
        },
        isFirstPage: function () {
            return (currentPage === 0);
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
        incrementPostCount: function(){
            postCount++;
            update();
        },
        getCurrentPage: function () {
            return currentPage;
        }
    };

    var fullProperties = new PageCounter.PageCounterInitializer();

    GlobalScript.JsPropertyCopyIfAvailable(fullProperties, initializer);

    var postCount = fullProperties.postCount;
    var currentPage = 0;
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
        if (postCount > 0) {
            var message = "Showing page " + (currentPage + 1) + " (posts " + getFirstItem() + "-" + getLastItem() + ")";
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
        return (currentPage*itemsPerPage)+1;
    }

    function getLastItem() {
        var lastItem = ((currentPage+1)*itemsPerPage);
        if (lastItem > postCount){
            lastItem = postCount;
        }

        return lastItem;
    }

    initialize(fullProperties);

    return self;
}

PageCounter.PageCounterInitializer = function () {
    var self = {
        postCount: 0,
        itemsPerPage: 10,
        targetDiv: null,
        previousHandler: null,
        nextHandler: null
    };

    return self;
};

var MessageBoardData = function () {
    var self = {
        PostCount: -1,
        MessageBoardId: 0
    };

    return self;
}