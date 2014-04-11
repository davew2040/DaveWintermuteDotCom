var DoorShutter = function (targetDiv, configParameters) {
    var _$targetDiv = $(targetDiv);
    var _$containerDiv = null;
    var _$shutterDiv = null;
    var _$contentDiv = null;

    var _configParameters = configParameters || new DoorShutterBase.getConfigObject();

    var self = {

    };

    function initialize() {
        _$targetDiv.width(_$targetDiv.outerWidth());
        _$targetDiv.height(_$targetDiv.outerHeight());

        _$containerDiv = $("<div></div>");
        _$containerDiv.addClass("DoorShutterContainer");

        _$containerDiv.width(_$targetDiv.outerWidth());
        _$containerDiv.height(_$targetDiv.outerHeight());

        _$shutterDiv = $("<div></div>");
        _$shutterDiv.addClass("DoorShutterPassive");
        _$shutterDiv.css("background-color", _configParameters.BoxColor);

        _$shutterDiv.appendTo(_$containerDiv);

        var $divChildren = _$targetDiv.children();

        _$contentDiv = $("<div></div>");
        _$contentDiv.addClass("DoorShutterContent");

        $divChildren.appendTo(_$contentDiv);
        _$contentDiv.appendTo(_$containerDiv);
        _$containerDiv.appendTo(_$targetDiv);

        initializeEvents();
    }

    function initializeEvents() {
        _$targetDiv.mouseenter(function (eventArgs) {
            _$shutterDiv.addClass("DoorShutterActive");
        });

        _$targetDiv.mouseleave(function (eventArgs) {
            _$shutterDiv.removeClass("DoorShutterActive");
        });
    }

    initialize();

    return self;
};

var DoorShutterBase = function (targetDiv, configParameters) {
    var self = {
        getConfigObject: function () {
            return {
                BoxColor: "#FF0000"
            }
        }
    };

 
    return self;
}();