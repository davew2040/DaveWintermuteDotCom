var Expando = function (targetDiv, configObject) {
    var _$targetDiv = $(targetDiv);
    var _$parent = _$targetDiv.parent();
    var _state;
    var _$containerDiv = null;

    var _configObject = configObject || new ExpandoBase.getConfigObject();
    _state = _configObject.InitialState;

    var self = {
        expand: function () {
            _state = true;

            handleHeight();
        },
        collapse: function () {
            _state = false;

            handleHeight();
        },
        toggle: function () {
            _state = !_state;

            handleHeight();
        }
    };

    function handleHeight() {
        if (_state) {
            _$containerDiv.height(_$targetDiv.height());
        }
        else {
            _$containerDiv.height(0);
        }
    }

    function initialize() {
        _$containerDiv = $("<div></div>");
        _$containerDiv.insertBefore(_$targetDiv);
        _$targetDiv.appendTo(_$containerDiv);

        var data = GlobalUtilities.getElementDataObject(_$containerDiv[0]);
        data.expando = self;

        handleHeight();

        _$containerDiv.addClass("expando");

        containerHeight = _$containerDiv.height();
        if (_configObject.UseDynamicTiming) {
            GlobalUtilities.setTransition(_$containerDiv, ["height"], _configObject.DynamicTransitionRate * (containerHeight / 100), "linear");
        }
        else {
            GlobalUtilities.setTransition(_$containerDiv, ["height"], _configObject.StaticTransitionTime, "linear");
        }
    }

    initialize();

    return self;
};

var ExpandoBase = function () {
    var self = {
        getConfigObject: function () {
            return {
                UseDynamicTiming: true,
                DynamicTransitionRate: 0.2,
                StaticTransitionTime: 0.4,
                InitialState: true
            };
        }
    };

    return self;
}();