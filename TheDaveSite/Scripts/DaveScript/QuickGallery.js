var DaveQuickGalleryStatic = function () {
    var self = {
        initialize: function () {
            _sizer = $("<div class='daveQuickGallerySizer'>");
            _viewer = $("<div class='daveQuickGalleryViewer'>");
            _clicker = $("<div class='daveQuickGalleryClicker'>");
            _offscreenDiv = $("<div class='daveQuickGalleryOffscreenDiv'>"); 

            $("html body").append(_viewer);
            $("html body").append(_sizer);
            $("html body").append(_clicker);
            $("html body").append(_offscreenDiv);
            
            _clicker.click(function ()
            {
                self.hide();
            });
        },
        setImage: function (image) {
            _sizer.css("display", "block");
            _viewer.css("display", "block");
            _clicker.css("display", "block");
            var $image = $(image).clone();

            _viewer.empty();

            $image.css('height', '');
            $image.css('width', '');

            _viewer.append($image);

            var baseWidth = $image.width();
            var baseHeight = $image.height();
            var baseRatio = baseWidth / baseHeight;

            var windowRatio = _sizer.width() / _sizer.height();

            if (baseRatio > windowRatio) {
                _viewer.width(Math.ceil(_sizer.width()));
                _viewer.height(Math.ceil(_sizer.width() / baseRatio));
            }
            else {
                _viewer.height(Math.ceil(_sizer.height()));
                _viewer.width(Math.ceil(_sizer.height() * baseRatio));
            }

            $image.width(_viewer.width() - 50);
            $image.height(_viewer.height() - 50);
            $image.css("position", "absolute");
            $image.css("top", "50%");
            $image.css("left", "50%");
            $image.css("margin-left", -$image.width() / 2.0);
            $image.css("margin-top", -$image.height() / 2.0);

            _viewer.css("margin-left", Math.round(-_viewer.width() / 2.0));
            _viewer.css("margin-top", Math.round(-_viewer.height() / 2.0));

            _sizer.css("display", "none");
        },
        hide: function () {
            _viewer.css("display", "none");
            _sizer.css("display", "none");
            _clicker.css("display", "none");
        },
        getOffscreenDiv: function () {
            return _offscreenDiv;
        }
    };

    $(document).ready(function () {
        self.initialize();
    });

    var _sizer = null;
    var _viewer = null;
    var _clicker = null;
    var _offscreenDiv = null;

    return self;
}();

// targetDiv - DOM element - Where to locate the gallery DOM elements
// template - DOM element - an HTML template defining a few stylistic elements of the gallery
// itemBuilder - function - a function that defines how to turn the contents of an LI element into an appropriate gallery item
var DaveQuickGallery = function (targetDiv, template, itemBuilder) {
    var self = {
        update: function (delta) {
            if (_animationState === AnimationState.Idle) {
                return;
            }
            else if (_animationState === AnimationState.ScrollingBackward) {
                // Don't scroll at all if there's not enough content to warrant it.
                if (_contentParentWidth > _contentWidth) {
                    _animationState = AnimationState.Idle;
                    return;
                }

                _rate += delta * ACCEL_RATE;
                if (_rate > MAX_MOVEMENT_RATE) {
                    _rate = MAX_MOVEMENT_RATE;
                }

                _leftMargin = _leftMargin + (delta * _rate);

                if (_leftMargin > 0) {
                    _animationState = AnimationState.Idle;
                    _leftMargin = 0;
                    _$content.css("margin-left", 0);
                    return;
                }

                _$content.css("margin-left", _leftMargin);
            }
            else if (_animationState === AnimationState.ScrollingForward) {
                // Don't scroll at all if there's not enough content to warrant it.
                if (_contentParentWidth > _contentWidth) {
                    _animationState = AnimationState.Idle;
                    return;
                }

                _rate += delta * ACCEL_RATE;
                if (_rate > MAX_MOVEMENT_RATE) {
                    _rate = MAX_MOVEMENT_RATE;
                }

                _leftMargin = _leftMargin - (delta * _rate);

                if (_leftMargin < _leftMarginMax) {
                    _animationState = AnimationState.Idle;
                    _leftMargin = _leftMarginMax;
                    _$content.css("margin-left", _leftMargin);
                    return;
                }

                _$content.css("margin-left", _leftMargin);
            }
        },
        startForwardScroll: function () {

        },
        stopForwardScroll: function () {

        },
        startBackwardScroll: function () {

        },
        endBackwardScroll: function () {

        },
    };

    var AnimationState = {
        ScrollingForward: "ScrollingForward",
        ScrollingBackward: "ScrollingBackward",
        Idle: "Idle"
    };

    var Dimension = function (width, height) {
        var self = {
            width: 0,
            height: 0
        }
        if (width !== undefined) {
            self.width = width;
        }
        if (height !== undefined) {
            self.height = height;
        }

        return self;
    };

    var IMAGE_WIDTH = 100;
    var IMAGE_HEIGHT = 100;
    var IMAGE_MARGIN_WIDTH = 10;
    var IMAGE_MARGIN_HEIGHT = 10;
    var MAX_MOVEMENT_RATE = 750;
    var ACCEL_RATE = 1500;

    var $targetDiv = null;
    var $template = null;
    var _$content = null;
    var _animationState = AnimationState.Idle;
    var _leftMargin = 0;
    var _leftMarginMax = 0;
    var _contentParentWidth = 0;
    var _contentWidth = 0;
    var _rate = 0;
    var _itemBuilder = function ($liElement) {
        return $liElement.children();
    }

    function initialize() {
        $targetDiv = $(targetDiv);
        $template = $(template);

        if ($targetDiv.length === 0) {
            throw "Gallery must have a valid target location!";
        }
        if (template.length === 0) {
            throw "Gallery must have a valid template!";
        }
        if (itemBuilder !== undefined) {
            _itemBuilder = itemBuilder;
        }

        var content = $targetDiv.clone();
        $targetDiv.empty();
        var templateCopy = $template.clone();
        templateCopy.css("display", "block");
        $targetDiv.append(templateCopy);

        _$content = $targetDiv.find(".galleryContent");

        var newContent = buildContentDiv(content);
        for (var i = 0; i < newContent.length; i++) {
            var item = newContent[i];
            $targetDiv.find(".galleryContent").append(item);
        }

        sizeContent();

        AnimationManager.registerAnimation(self);

        initializeEvents();
    }

    function initializeScrollingParameters() {
        _rate = 0;

        var $contentParent = $(_$content.parent());

        _contentWidth = _$content.width();
        _contentParentWidth = $contentParent.width();
        _leftMarginMax = _contentParentWidth - _contentWidth;
    }
    
    function initializeEvents() {
        if (!GlobalUtilities.isMobile()) {
            $targetDiv.find(".backArrow").mousedown(function () {
                if (_animationState === AnimationState.Idle) {
                    initializeScrollingParameters();
                    _animationState = AnimationState.ScrollingBackward;
                }
            });

            $targetDiv.find(".backArrow").mouseup(function () {
                if (_animationState === AnimationState.ScrollingBackward) {
                    _animationState = AnimationState.Idle;
                }
            });

            $targetDiv.find(".forwardArrow").mousedown(function () {
                if (_animationState === AnimationState.Idle) {
                    initializeScrollingParameters();
                    _animationState = AnimationState.ScrollingForward;
                }
            });
            $targetDiv.find(".forwardArrow").mouseup(function () {
                if (_animationState === AnimationState.ScrollingForward) {
                    _animationState = AnimationState.Idle;
                }
            });
        }
        else {
            $targetDiv.find(".backArrow").click(function () {
                if (_animationState === AnimationState.Idle) {
                    initializeScrollingParameters();
                    _animationState = AnimationState.ScrollingBackward;
                }
                else {
                    _animationState = AnimationState.Idle;
                }
            });

            $targetDiv.find(".forwardArrow").click(function () {
                if (_animationState === AnimationState.Idle) {
                    initializeScrollingParameters();
                    _animationState = AnimationState.ScrollingForward;
                }
                else {
                    _animationState = AnimationState.Idle;
                }
            });
        }
    }


    function buildContentDiv(source) {
        var returnContent = [];

        var $liElements = source.find("li");

        for (var i = 0; i < $liElements.length; i++) {
            var $liElement = $($liElements[i]);
            returnContent.push(_itemBuilder($liElement));
        }

        return returnContent;
    }

    function sizeContent() {
        var $imageHolders = $targetDiv.find(".galleryImageOuterHolder");
        
        _contentWidth = 0;

        for (var i = 0; i < $imageHolders.length; i++) {
            var $holder = $($imageHolders[i]);

            $holder.css("width", $holder.parent().height());
            _contentWidth += $holder.width();
            $holder.css("left", i * $holder.width());

            formatImage($holder.find("img"));
            $holder.find("img").load(function () {
                formatImage($(this));
            });
        }

        _$content.css("width", _contentWidth);
    }

    function formatImage(image) {
        var $image = $(image);
        var imageParent = $image.parent();

        var imageDims = getMiniImageDimensions(
            new Dimension($image.width(), $image.height()),
            new Dimension(imageParent.width() - IMAGE_MARGIN_WIDTH * 2, imageParent.height() - IMAGE_MARGIN_HEIGHT * 2)
            );

        $image.css("width", imageDims.width);
        $image.css("height", imageDims.height);

        $image.css("margin-top", "auto");
        $image.css("margin-bottom", "auto");
    }

    function getMiniImageDimensions(imageDims, containerDims) {
        var containerRatio = containerDims.width / containerDims.height;
        var imageRatio = imageDims.width / imageDims.height;

        var returnDims = new Dimension();

        if (imageRatio > containerRatio) {
            returnDims.width = containerDims.width;
            returnDims.height = containerDims.height / imageRatio;
        }
        else {
            returnDims.height = containerDims.height;
            returnDims.width = containerDims.width * imageRatio;
        }

        return returnDims;
    }

    function getImageDimensions(image) {
        var clone = image.clone();

        DaveQuickGalleryStatic.getOffscreenDiv().append(clone);
        
        var returnDimensions = new Dimension(clone.width(), clone.height());

        DaveQuickGalleryStatic.getOffscreenDiv().empty();

        return returnDimensions;
    }

    initialize();

    return self;
};