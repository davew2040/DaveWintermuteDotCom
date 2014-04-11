

var BorderedImageFocusPopper = function (targetDiv, configObject) {
    var _popper = null;
    var _$popperDiv = null;
    var _$targetDiv = $(targetDiv);
    var _originalSize = null;
    var _originalLocation = null;
    var _$poppedImage = null;
    var _timer = null;
    var _configObject = configObject || ImageFocusPopperBase.getConfigObject();

    function resize(scaleFactor) {
        var $allChildren = _$popperDiv.find("*");

        $allChildren.each(function (index, element) {
            scaleSingleElement(element, scaleFactor);
        });

        var data = GlobalUtilities.getElementDataObject(_$popperDiv[0]);

        var newWidth = data.originalSize.x * scaleFactor;
        var newHeight = data.originalSize.y * scaleFactor;

        _$popperDiv.width(newWidth);
        _$popperDiv.height(newHeight);

        var originalCenterPosition = {
            x: data.originalLocation.left + data.originalSize.x / 2.0,
            y: data.originalLocation.top + data.originalSize.y / 2.0
        };

        _$popperDiv.css("left", originalCenterPosition.x - newWidth / 2.0);
        _$popperDiv.css("top", originalCenterPosition.y - newHeight / 2.0);

        _$popperDiv.css("z-index", 20);
    }

    function scaleSingleElement(element, scaleFactor) {
        var $element = $(element);
        var data = GlobalUtilities.getElementDataObject(element);

        var newWidth = data.originalSize.x * scaleFactor;
        var newHeight = data.originalSize.y * scaleFactor;

        $element.width(newWidth);
        $element.height(newHeight);
    }

    function reset() {
        var $allChildren = _$popperDiv.find("*");

        $allChildren.each(function (index, element) {
            var $element = $(element);
            var data = GlobalUtilities.getElementDataObject(element);

            $element.width(data.originalSize.x);
            $element.height(data.originalSize.y);
        });

        var data = GlobalUtilities.getElementDataObject(_$popperDiv[0]);

        _$popperDiv.width(data.originalSize.x);
        _$popperDiv.height(data.originalSize.y);

        _$popperDiv.css("left", data.originalLocation.left);
        _$popperDiv.css("top", data.originalLocation.top);

        _$popperDiv.css("z-index", 10);
    }

    function initialize() {
        _popper = new Popper(_$targetDiv);
        _$popperDiv = _popper.getPoppedElement();

        var result = _$popperDiv.find("*");
        result.each(function (index, element) {
            prepSingleElement(element);
        });

        var data = GlobalUtilities.getElementDataObject(_$popperDiv[0]);

        data.originalSize = {
            x: _$popperDiv.width(),
            y: _$popperDiv.height()
        }

        data.originalLocation = _$popperDiv.offset();

        GlobalUtilities.setTransition(_$popperDiv, ["left", "top", "width", "height"], _configObject.TransitionTime, "linear");
    }

    function prepSingleElement(element) {
        var $element = $(element);
        var data = GlobalUtilities.getElementDataObject(element);

        data.originalSize = {
            x: $element.width(),
            y: $element.height()
        }

        data.originalLocation = $element.offset();

        $element.width($element.width());
        $element.height($element.height());

        GlobalUtilities.setTransition($element, ["width", "height"], _configObject.TransitionTime, "linear");
    }

    var self = {
        resize: function () {
            resize(_configObject.ScaleFactor);
        },
        reset: function () {
            reset();
        },
        getPopperDiv: function () {
            return _$popperDiv;
        },
        destroy: function () {
            _popper.destroy();
        }
    };

    initialize();

    return self;
};

var ImageFocusPopper = function (targetImage, configObject) {
    var _popper = null;
    var _$popperDiv = null;
    var _$img = $(targetImage);
    var _originalSize = null;
    var _originalLocation = null;
    var _$poppedImage = null;
    var _timer = null;
    var _configObject = configObject || ImageFocusPopperBase.getConfigObject();

    function resize(scaleFactor) {
        var newWidth = _originalSize.x * scaleFactor;
        var newHeight = _originalSize.y * scaleFactor;

        _$poppedImage.width(newWidth);
        _$poppedImage.height(newHeight);

        var originalCenterPosition = {
            x: _originalLocation.left + _originalSize.x / 2.0,
            y: _originalLocation.top + _originalSize.y / 2.0 
        };

        _$popperDiv.css("left", originalCenterPosition.x - newWidth / 2.0);
        _$popperDiv.css("top", originalCenterPosition.y - newHeight / 2.0);
    }

    function reset() {
        _$poppedImage.width(_originalSize.x);
        _$poppedImage.height(_originalSize.y);

        _$popperDiv.css("left", _originalLocation.left);
        _$popperDiv.css("top", _originalLocation.top);
    }

    function initialize() {
        _popper = new Popper(_$img);
        _$popperDiv = _popper.getPoppedElement();
        _$poppedImage = _$popperDiv.find("img");

        _originalSize = {
            x: _$poppedImage.width(),
            y: _$poppedImage.height()
        };

        _$poppedImage.width(_$poppedImage.width());
        _$poppedImage.height(_$poppedImage.height());

        _originalLocation = _$popperDiv.offset();

        GlobalUtilities.setTransition(_$popperDiv, ["left", "top"], _configObject.TransitionTime, "linear");
        GlobalUtilities.setTransition(_$poppedImage, ["width", "height"], _configObject.TransitionTime, "linear");
    }

    var self = {
        resize: function () {
            resize(_configObject.ScaleFactor);
        },
        reset: function(){
            reset();
        },
        getPopperDiv: function () {
            return _$popperDiv;
        },
        destroy: function () {
            _popper.destroy();
        }
    };

    initialize();

    return self;
};

var ImageFocusPopperBase = function () {
    var self = {
        getConfigObject: function () {
            return {
                ScaleFactor: 2.0,
                TransitionTime: 0.4
            }
        }
    };

    function initialize() {

    }

    initialize();

    return self;
}();

var Popper = function (targetDiv) {
    var _$targetDiv = $(targetDiv);
    var _$holderDiv = null;

    function initialize() {
        _$holderDiv = $("<div class='poppedElement'></div>");
        _$targetDiv.clone().appendTo(_$holderDiv);
        _$holderDiv.appendTo($("html"));

        setDimensions(_$holderDiv, _$targetDiv);
    }

    function setDimensions(target, source) {
        var offset = source.offset();

        target.css("left", offset.left);
        target.css("top", offset.top);
        target.width(source.width());
        target.height(source.height());
    }

    var self = {
        getPoppedElement: function () {
            return _$holderDiv;
        },
        destroy: function () {
            _$holderDiv.remove();
        }
    };

    initialize();

    return self;
};

var PopperBase = function () {
    var self = {

    };

    function initialize() {

    }

    initialize();

    return self;
}();